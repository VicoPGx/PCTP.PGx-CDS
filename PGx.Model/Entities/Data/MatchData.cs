
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PGx.Model.Concrete;
//using PGx.KB.Vcf.Definition;
//using PGx.KB.Vcf.Haplotype.Model;
using PGx.Model.Entities;
using PGx.Model.Util;

namespace PGx.Model.Entities.Data
{
    /**
 * This is the data used to compute a {@link DiplotypeMatch} for a specific gene.
 *
 * @author Mark Woon
 */
    public class MatchData
    {
        private static Logger sf_logger = new Logger();
        
        //m_samplePositionMap is the final positionAlleleMap extracted from vcf file that is valid and can be used to determin NamedAllele(e.g. star allele) and is consistent with m_positions.  
        private SortedDictionary<int, SampleAllele> m_samplePositionMap = new SortedDictionary<int, SampleAllele>();      
        //m_positions included not only in the VCF file but also in the definitionFile, namely they are the intersection of the positions in this two files, 
        //and exculded the ignored positions in exemptions and positions with null allele value
        private VariantLocus[] m_positions;
       //m_missingPositions are those included in the definitionFile but not included in the VCF file
        private HashSet<VariantLocus> m_missingPositions = new HashSet<VariantLocus>();
        //m_ignoredPositions is those used to determine the ignoredNamedAllele in the exemption file and not used to define other nonIgnoredNamedAlleles
        private HashSet<VariantLocus> m_ignoredPositions = new HashSet<VariantLocus>();
        //extrapositions in the exemption
        private SortedSet<VarOfInterest> m_extraPositions = new SortedSet<VarOfInterest>();
        //baseAlleleMismatchedPosions are vcf positions have different base alleles with the defined base alleles 
        private SortedSet<VariantLocus> m_baseAlleleMismatchedPositions = new SortedSet<VariantLocus>();
        //m_haplotypes is all the namedAlleles of a specific gene included in definitionFile and not exepmted by exemtion file
        private List<NamedAllele> m_namedAlleles;

        private HashSet<String> m_permutations;


        
         //Organizes the {@link SampleAllele} data related for the gene of interest.
         // @param alleleMap map of chr:positions to  SampleAlleles from VCF
         // @param allPositions all  VariantLocus positions of interest for the gene
         //@param extraPositions extra positions to track sample alleles for
         //@param ignoredPositions ignored positions due to ignored named alleles
         
        public MatchData(SortedDictionary<String, SampleAllele> sampleAlleleMap, IEnumerable<VariantLocus> geneSpecificAllPositions,SortedSet<VariantLocus> extraPositions, SortedSet<VariantLocus> ignoredPositions)
        {

            if (ignoredPositions != null)
            {
                foreach (var item in ignoredPositions)
                {
                    m_ignoredPositions.Add(item);
                }
            }

            List<VariantLocus> positions = new List<VariantLocus>();
            //下面的for循环，对VCF获取的位点进行过滤，即根据每个定义的位点，检查读出的Vcf数据中是否有相应的位点，
            //VCF文档中没有的定义位点加入missingPositions并写入日志，定义要求忽略的就忽略（继续执行），剩下找到的保留
            foreach (VariantLocus position in geneSpecificAllPositions)
            {
                String chrPos = position.getChrPosition();
                SampleAllele sampleAllele = sampleAlleleMap.Where(x => x.Key == chrPos).Select(x => x.Value).FirstOrDefault();
                if (sampleAllele == null)
                {
                    m_missingPositions.Add(position);
                    sf_logger.info("Sample has no allele for {}", chrPos);
                    continue;
                }
                if (m_ignoredPositions.Contains(position))
                {
                    continue;
                }
                positions.Add(position);
                sampleAllele = sampleAllele.positionAlleleConvertByType(position);
                m_samplePositionMap.Add(position.getPosition(), sampleAllele);
            }

            m_positions = positions.ToArray();
            if (extraPositions != null)
            {
                foreach (VariantLocus vl in extraPositions)
                {
                    SampleAllele sampleAllele = sampleAlleleMap.Where(x => x.Key == vl.getChrPosition()).Select(x => x.Value).FirstOrDefault();
                    if (sampleAllele != null)
                    {
                        m_extraPositions.Add(new VarOfInterest(vl, sampleAllele));
                    }
                    else
                    {
                        m_extraPositions.Add(new VarOfInterest(vl.Position, vl.Rsid, null, vl.getPosition(), null));
                    }
                }
            }
        }

        //check if the variants allele in the definition allele
        public void checkBaseAlleles(DefinitionFile definitionFile)
        {

            foreach (VariantLocus variantLocus in m_positions)
            {
                SampleAllele sampleAllele = m_samplePositionMap.Where(x => x.Key == variantLocus.getPosition()).Select(x => x.Value).FirstOrDefault();
                if (sampleAllele != null)
                {
                    List<String> alleles = definitionFile.VariantLocus.Where(x => x == variantLocus).FirstOrDefault().Alleles.Split(',').ToList();
                    if (!alleles.Contains(sampleAllele.getAllele1()) ||
                        (sampleAllele.getAllele2() != null && !alleles.Contains(sampleAllele.getAllele2())))
                    {
                        m_baseAlleleMismatchedPositions.Add(variantLocus);
                    }
                }
            }
        }


        //Organizes the {@link NamedAllele} data for analysis.
        //exclude the missing positions from the locus of NamedAlleleDefinitions,and ReCalculate the positionAllele permutation for the new namedAllele 
        public void ReconstructNamedALleles(List<NamedAllele> definedHaplotypes)
        {

            if ((m_missingPositions == null || m_missingPositions.Count() == 0) && (m_ignoredPositions == null || m_ignoredPositions.Count() == 0))
            {
                m_namedAlleles = definedHaplotypes;

            }
            else
            {
                // handle missing positions by duplicating haplotype and eliminating missing positions
                m_namedAlleles = new List<NamedAllele>();
                foreach (NamedAllele hap in definedHaplotypes)
                {
                    // get alleles for positions we have data on
                    List<NamedAlleleDefinition> availableAlleles = new List<NamedAlleleDefinition>();
                    foreach (var pos in m_positions)
                    {
                        availableAlleles.Add(pos.AlleleDefinition.Where(x => x.NamedAlleleID == hap.ID).FirstOrDefault());
                    }


                    SortedSet<VariantLocus> missingPositions = new SortedSet<VariantLocus>(m_missingPositions.Where(l => string.IsNullOrEmpty(hap.GetAllele(l)) == false));


                    NamedAllele newHap = new NamedAllele(hap.M_Id, hap.Name, availableAlleles, missingPositions);
                    newHap.M_Function = hap.M_Function;
                    newHap.PopFreqDic = hap.PopFreqDic;
                    newHap.IsRefAllele = hap.IsRefAllele;
                    newHap.DefinitionFile = hap.DefinitionFile;
                    newHap.DefinitionFileID = hap.DefinitionFileID;
                    newHap.ID = hap.ID;
                    //newHap.initialize(m_positions);
                    newHap.initialize();
                    if (newHap.getScore() > 0)
                    {
                        m_namedAlleles.Add(newHap);
                    }
                }
            }
        }


        /**
         * Assumes that missing alleles in {@link NamedAllele}s should be the reference.
         */
        public void defaultMissingAllelesToReference()
        {

            List<NamedAllele> updatedHaplotypes = new List<NamedAllele>();

            NamedAllele referenceHaplotype = m_namedAlleles.Where(x => x.IsRefAllele == true).FirstOrDefault();
            List<NamedAlleleDefinition> refNamedAlleleDefinitionList = referenceHaplotype.NamedAlleleDefinition.ToList();

            foreach (NamedAllele hap in m_namedAlleles)
            {
                if (hap.NamedAlleleDefinition.Count == 0)
                    continue;

                if (referenceHaplotype.ID == hap.ID)
                {
                    updatedHaplotypes.Add(hap);
                    continue;
                }

                List<NamedAlleleDefinition> curNamedAlleleDefinitionList = hap.NamedAlleleDefinition.ToList();
                List<NamedAlleleDefinition> newNamedAlleleDefinitionList = new List<NamedAlleleDefinition>();

                foreach (var refNamedAlleleDefinition in refNamedAlleleDefinitionList)
                {
                    var curNamedAlleleDefinition = curNamedAlleleDefinitionList.Where(y => y.VariantLocusID == refNamedAlleleDefinition.VariantLocusID).FirstOrDefault();
                    if (curNamedAlleleDefinition==null|| string.IsNullOrEmpty(curNamedAlleleDefinition.Allele))
                    {
                        newNamedAlleleDefinitionList.Add(refNamedAlleleDefinition);
                    }
                    else
                    {
                        newNamedAlleleDefinitionList.Add(curNamedAlleleDefinition);
                    }
                }

                NamedAllele fixedHap = new NamedAllele(hap.M_Id, hap.Name, newNamedAlleleDefinitionList, hap.getMissingPositions());
                fixedHap.M_Function = hap.M_Function;
                fixedHap.PopFreqDic = hap.PopFreqDic;
                fixedHap.initialize(hap.getScore());
                updatedHaplotypes.Add(fixedHap);
            }

            m_namedAlleles = updatedHaplotypes;
        }


        public int SamplePositionMapCount()
        {
            return m_samplePositionMap.Count();
        }

        public SampleAllele getSampleAllele(int position)
        {
            SampleAllele sampleAllele = m_samplePositionMap.Where(x => x.Key == position).Select(x => x.Value).FirstOrDefault();
            if (sampleAllele == null)
            {
                throw new ArgumentException("No sample allele for position " + position);
            }
            return sampleAllele;
        }

        /**
         * Gets all permutations of sample alleles at positions of interest.
         */
        public HashSet<String> getPermutations()
        {
            if (m_permutations == null)
            {
                throw new InvalidOperationException("Not initialized - call generateSamplePermutations()");
            }
            return m_permutations;
        }

        /**
         * Generate all permutations of sample alleles at positions of interest.
         */
        public void generateSamplePermutations()
        {
            m_permutations = new HashSet<string>();
            var m_permutations1 = CombinationUtil.GeneratePermutations(
                m_samplePositionMap.Values.OrderBy(x => x).ToList());
            foreach (var permutation in m_permutations1)
            {
                m_permutations.Add(permutation.Remove(permutation.LastIndexOf(";")));
            }

        }


        /**
         * Gets the positions available for calling the haplotypes for the gene.
         */
        public VariantLocus[] GetPositions
        {
            get
            {
                return m_positions;
            }
        }

        
        // Gets the positions that are missing from the sample VCF that would have been helpful for calling the haplotypes for the gene.  
        public HashSet<VariantLocus> MissingPositions
        {
            get
            {
                return m_missingPositions;
            }
        }

        /**
         * Gets the positions that are mismatched from any allele defined for the given gene
         * @return a Set of {@link VariantLocus} objects with mismatched alleles
         */
        public SortedSet<VariantLocus> MismatchedPositions
        {
            get { return m_baseAlleleMismatchedPositions; }
        }

        /**
         * Gets the extra positions specified in {@link DefinitionExemption#getExtraPositions()}.
         */
        public SortedSet<VarOfInterest> ExtraPositions
        {
            get { return m_extraPositions; }
        }


        public List<NamedAllele> GetNamedAlleles
        {
            get
            {
                if (m_namedAlleles == null)
                {
                    if (m_samplePositionMap.Count() == 0)
                    {
                        //return Collections.emptyList();
                        return new List<NamedAllele>();
                        //return null;
                    }
                    throw new InvalidOperationException("Not initialized - call marshallHaplotypes()");
                }
                return m_namedAlleles;
            }
        }
    }
}
