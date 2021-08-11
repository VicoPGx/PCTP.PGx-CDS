using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
//using PGx.KB.Vcf.Definition;
using PGx.KB.Vcf.Haplotype.Model;
using PGx.Model.Entities;
using PGx.Model.Entities.Data;


namespace PGx.KB.Vcf.Haplotype
{

    public class NamedAlleleMatcher
    {
        public static String VERSION = "1.0.0";
        private DefinitionReader m_definitionReader;
        private Dictionary<String, VariantLocus> m_locationsOfInterest;//locationofInterest means the locus position in the definitionFile,[chrPosition,variantLocus] example:[chr1:986324569,locusObject]
        private Boolean m_assumeReferenceInDefinitions;
        private Boolean m_topCandidateOnly;


        //This will only call the top candidate(s) and assume reference.
        public NamedAlleleMatcher(DefinitionReader definitionReader)
            : this(definitionReader, true, false) { }

        // @param topCandidateOnly true if only top candidate(s) should be called, false to call all possible candidates
        // @param assumeReference true if missing alleles in definitions should be treated as reference, false otherwise   
        public NamedAlleleMatcher(DefinitionReader definitionReader, Boolean assumeReference, Boolean topCandidateOnly)
        {
            m_definitionReader = definitionReader;
            m_locationsOfInterest = CalculateLocationsOfInterest(m_definitionReader);//定义中所有相关的位点共317个，如果exemption中有extra，extra位点也会加进来
            m_assumeReferenceInDefinitions = assumeReference;
            m_topCandidateOnly = topCandidateOnly;
        }
        /**
         * Collects all locations of interest (i.e. positions necessary to make a haplotype call).
         * @return a set of {@code <chr:position>} Strings
         */
        private static Dictionary<String, VariantLocus> CalculateLocationsOfInterest(DefinitionReader definitionReader)
        {

            HashSet<String> data = new HashSet<String>();

            Dictionary<String, VariantLocus> interestPositionMap = new Dictionary<string, VariantLocus>();
            foreach (String gene in definitionReader.GetGenes())
            {
                definitionReader.GetPositions(gene).ToList().ForEach(variantLocus =>
              {
                  string vcp = variantLocus.getChrPosition(variantLocus.DefinitionFile.Chromosome);
                  if (interestPositionMap.ContainsKey(vcp)==false)
                  {
                      data.Add(vcp);
                      interestPositionMap.Add(vcp, variantLocus);
                  }
              });
                DefinitionExemption exemption = definitionReader.GetExemption(gene);
                if (exemption != null)
                {
                    exemption.ExtraPositionsSorted.ToList()
                        .ForEach(extraPosition =>
                        {
                            String chrPosition = extraPosition.getChrPosition();
                            if (!data.Contains(chrPosition))
                            {
                                data.Add(chrPosition);
                                interestPositionMap.Add(chrPosition, extraPosition);
                            }
                        });
                }
            }
            return interestPositionMap;
        }


        /**
         * Calls diplotypes for the given VCF file for all genes for which a definition exists.
         */
        public Result Call(string vcfFile)
        {

            //从VCF中只读DefinitionFile中存在的位点,并组成positionAlleleMap这样的词典结构（chr1：4565227,allele object）
            VcfReader vcfReader = new VcfReader(m_locationsOfInterest, vcfFile);
            //样本VCF文档中共有228个碱基位点

            ResultBuilder resultBuilder = new ResultBuilder(m_definitionReader);
            resultBuilder=resultBuilder.SetMetadata(vcfFile, vcfReader.getWarnings());
            var geneList = m_definitionReader.GetGenes();
            // 逐个基因，构建MatchData，MatchData包括由VcfReader和DefinitionReader读取的VCF数据（PositionAlleleMap）和基因定义数据（NamedAlleleDefinition），
            //并根据VcfData的实际情况对每个Gene的NamedAllele定义进行改造适配;
            // 并组织用于DiplotypeCall的Permutation，形如locus:baseAllele.因此MatchData对象由定义和VCF数据两部分构成，并包含所有的完整的DiplotypeCall所需要的数据。
            foreach (String gene in geneList)
            {
                DefinitionExemption exemption = m_definitionReader.GetExemption(gene);
                //构建每个基因的matchData.
                MatchData matchData = InitializeMatchDataOfSingleGene(vcfReader.sampleAlleleMap, gene);
                List<DiplotypeMatch> diplotypeMatchList = null;
                if (matchData.SamplePositionMapCount() > 0)
                {
                    Boolean topCandidateOnly = exemption == null ? m_topCandidateOnly : !exemption.AllHits;
                    diplotypeMatchList = CallDiplotypes(matchData, topCandidateOnly);
                }
                resultBuilder.MakeGeneCall(gene, matchData, diplotypeMatchList);
            }
            return resultBuilder.GetResult;
        }


        // Initializes data required to call a diplotype. @param alleleMap map of {@link SampleAllele}s from VCF
        //alleleMap 为Vcf数据
        private MatchData InitializeMatchDataOfSingleGene(SortedDictionary<String, SampleAllele> sampleAllelesMap, String gene)
        {

            DefinitionExemption exemption = m_definitionReader.GetExemption(gene);
            SortedSet<VariantLocus> extraPositions = null;
            List<NamedAllele> definitionNamedAlleleList = m_definitionReader.GetNamedAlleles(gene);//指定基因定义涉及的所有的star allele（haplotype）
            IEnumerable<VariantLocus> allPositionsOfOneGene = m_definitionReader.GetPositions(gene);//具体基因定义涉及的所有位点

            //unusedpositons also are ignoredPosions next step.
            SortedSet<VariantLocus> unusedPositions = null;
            if (exemption != null)
            {
                extraPositions = exemption.ExtraPositionsSorted;
                //unusedPositions are those have alt base alleles in the ignoredNamedAlleles and don't present an alt base allele at any other namedAlleles, unusedPositions are also the ignorable positions in the next compute step.  
                unusedPositions = FindUnusedPositions(exemption, allPositionsOfOneGene, definitionNamedAlleleList);
            }

            // grab SampleAlleles for all positions related to current gene
            MatchData matchData = new MatchData(sampleAllelesMap, allPositionsOfOneGene, extraPositions, unusedPositions);

            //检查变异位点的碱基是否在定义的范围之内
            matchData.checkBaseAlleles(m_definitionReader.GetDefinitionFile(gene));

            if (matchData.SamplePositionMapCount() == 0)
            {
                return matchData;
            }
            //exclude NamedAllele that should be ignored defined in the exemption
            if (exemption != null)
            {
                definitionNamedAlleleList = definitionNamedAlleleList.Where(n => !exemption.shouldIgnore(n.Name)).ToList();
            }
            // exclude missing positions (if any) from the definition of namedAlleles and generate a new namedAllele Definition, 
            //and reCalculate permutation for this new definition (definitionReader calculate the permutation of standard definition of NamedAllele)
            matchData.ReconstructNamedALleles(definitionNamedAlleleList);
            //将值为null的等位基因（baseAllele）填充为参考等位基因
            // 所以在上一行的marshallHaplotypes()方法构造sampleAllele的permutation
            Boolean assumeReference = exemption != null ? exemption.AssumeReference : m_assumeReferenceInDefinitions;
            if (assumeReference)
            {
                //set BaseAllele of which value is null or empty of the NonReferenceNamedAllele to reference allele
                matchData.defaultMissingAllelesToReference();
            }
            matchData.generateSamplePermutations();
            return matchData;
        }



        //Find positions that are only used by ignored alleles (and therefore should be eliminated from consideration).
        private SortedSet<VariantLocus> FindUnusedPositions(DefinitionExemption exemption, IEnumerable<VariantLocus> allPositions, List<NamedAllele> geneNamedAlleles)
        {
            SortedSet<VariantLocus> unusedPositions = new SortedSet<VariantLocus>();
            if (exemption.IgnoredAlleles == null || exemption.IgnoredAlleles.Count() == 0)
            {
                return unusedPositions;
            }
            //截取参考referenceAllele之外的所有NamedAllele
            List<NamedAllele> nonRefNamedAlleles = geneNamedAlleles.Skip(1).Take(geneNamedAlleles.Count() - 1).ToList();
            //=namedAlleles.Where(x => x.IsRefAllele==false).ToList();
            HashSet<VariantLocus> ignorablePositions = new HashSet<VariantLocus>();
            //此循环从非参考等位基因中查找豁免等位基因的所有位点
            foreach (NamedAllele nonRefNamedAllele in nonRefNamedAlleles)
            {
                if (exemption.shouldIgnore(nonRefNamedAllele.Name)==true)
                {
                    var shouldIgnoredAllele = nonRefNamedAllele;
                    ignorablePositions.UnionWith(FindIgnorablePositions(allPositions, shouldIgnoredAllele));
                }
            }
            //此循环从非参考等位基因中查找非豁免等位基因的所有位点是否包含需要排除的位点，如果包含则此位点被使用的，不包含则被加入unuesedPosition.Add(this.position);
            foreach (VariantLocus vl in ignorablePositions)
            {
                Boolean isUnused = true;
                foreach (NamedAllele nonRefNamedAllele in nonRefNamedAlleles)
                {
                    if (exemption.shouldIgnore(nonRefNamedAllele.Name)==false)
                    {
                        if (string.IsNullOrEmpty(nonRefNamedAllele.GetAllele(vl)) == false)
                        {
                            //An alternative
                            //if(namedAllele.NamedAlleleDefinition.Any(x=>string.IsNullOrEmpty(x.Allele)&&x.VariantLocus.ID==vl.ID)){
                            isUnused = false;
                            break;
                        }
                    }
                }
                if (isUnused)
                {
                    unusedPositions.Add(vl);
                }
            }
            return unusedPositions;
        }


        // Find potentially ignored positions from an ignored NamedAllele (and are therefore potentially ignoreable),these positons are usually characteristic positions with alternate base alleles and can be used to distiguish these  namedAllele with others.
        private HashSet<VariantLocus> FindIgnorablePositions(IEnumerable<VariantLocus> allPositions, NamedAllele ignoredNamedAllele)
        {
            HashSet<VariantLocus> ignorablePositions = new HashSet<VariantLocus>();
            foreach (var allele in ignoredNamedAllele.NamedAlleleDefinition.ToList())
            {
                if (string.IsNullOrEmpty(allele.Allele) == false)
                {
                    var varlocus = allele.VariantLocus;
                    //allPositions.ElementAt(x);
                    ignorablePositions.Add(varlocus);
                }
            }
            return ignorablePositions;
        }



        //Calls the possible diplotypes for a single gene.
        protected List<DiplotypeMatch> CallDiplotypes(MatchData matchData, Boolean topCandidateOnly)
        {

            // find matched pairs
            var diplotypeMatcher = new DiplotypeMatcher(matchData);

            List<DiplotypeMatch> diplotyoeMatches = diplotypeMatcher.Compute();

            if (topCandidateOnly && diplotyoeMatches.Count() > 1)
            {
                int ? topScore = diplotyoeMatches.ElementAt(0).Score;
                diplotyoeMatches = diplotyoeMatches.Where(dm => dm.Score == topScore).ToList();
            }

            return diplotyoeMatches;
        }
    }

}