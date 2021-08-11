using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using PGx.KB.Infrastructure;
using PGx.KB.Vcf.model;
using PGx.KB.Vcf.Haplotype;
using PGx.KB.Common;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Query.Dynamic;
using PGx.Model.Entities;
using PGx.Model.Enums;
using PGx.Model.Entities.Data;

namespace PGx.KB.Vcf
{
    public class VcfReader : IvcfLineParser
    {
        private static Logger sf_logger = new Logger();
        private static string sf_gtDelimiter = "[|/]";
        private static string sf_noCallPattern = "^[.|/]+$";
        private static string sf_allelePattern = "^[AaCcGgTt]+$";
        private Dictionary<String, VariantLocus> m_locationsOfInterest;//[chr2:695548963,locusObj]
        //private ImmutableMap<String, VariantLocus> m_locationsOfInterest;
        private String m_genomeBuild;
        // <chr:position, allele>
        public SortedDictionary<String, SampleAllele> sampleAlleleMap = new SortedDictionary<String, SampleAllele>();
        private Dictionary<String, List<String>> m_warnings=new Dictionary<string,List<string>>();


        /**
 * Gets the genome build the VCF file is using.
 * This is pulled from the VCF contig assembly metadata.
 */
        public String GenomeBuild
        {
            get { return m_genomeBuild; }
        }

        public VcfReader(Dictionary<String, VariantLocus> locationsOfInterest, string vcfFile)
        {
            m_locationsOfInterest = locationsOfInterest;
            ReadAndParseVCF(vcfFile);
        }




        public Dictionary<String, List<String>> getWarnings()
        {
            return m_warnings;
        }


        public void ReadAndParseVCF(String vcfFile)
        {

            StreamReader streamReader = new StreamReader(vcfFile.ToString());
            var vcfParserBuilder = new VcfParser.VcfParserBuilder();
            vcfParserBuilder = vcfParserBuilder.SetStreamReader(streamReader);
            vcfParserBuilder = vcfParserBuilder.InstanceLineParserInterface(this);
            VcfParser vcfParser = vcfParserBuilder.BuildVcfParser();
            var vcfMetaData = vcfParser.ParseVcfMetadata();
            Dictionary<string, ContigMetadata> contigDic = vcfMetaData.GetContigs;

            foreach (ContigMetadata cm in contigDic.Values)
            {
                if (cm.GetAssembly != null)
                {
                    if (m_genomeBuild == null)
                    {
                        m_genomeBuild = cm.GetAssembly;
                    }
                    else
                        if (!m_genomeBuild.Equals(cm.GetAssembly))
                        {
                            throw new InvalidOperationException("VCF file uses different assemblies (" + m_genomeBuild + " and " +
                                cm.GetAssembly + ")");
                        }
                }
            }
            vcfParser.ParseVCF();
        }

        private void addWarning(String chrPos, String msg)
        {
            //List<ListToLookup> list = new List<ListToLookup>(); 
            //list.Add(new ListToLookup{Key=chrPos,Value=msg});

            //var lookup=list.ToLookup(m=>m.Key,m=>m.Value);
            //var list1=lookup.SelectMany(x=>x);
            //m_warnings.SelectMany(l=>l).Concat()
            List<string> list;
            bool flag = m_warnings.TryGetValue(chrPos, out list);
            if (!flag == false)
            {
                m_warnings.Remove(chrPos);
                list.Add(msg);
                m_warnings.Add(chrPos, list);

            }
            else
            {
                if (list == null)
                    list = new List<string>();
                list.Add(msg);
                m_warnings.Add(chrPos, list);

            }

            sf_logger.warn(msg);
            return;
        }

        //This parseline() method process each line (position) of raw data read from the VCF file, excluding line/position that doesn't included in the definitionFile( i.e. m_locationOfInterest) 
        //and translate the rest of positions included in the definitionFile into the required format,i.e. position allele maping,e.g.<chr1:3656122,SampleAllele object> 
        public void ProcessRawLine(VcfMetadata metadata, VcfPosition position, List<VcfSample> sampleData)
        {

            String chrPos = position.getChromosome() + ":" + position.getPosition();

            if (sampleData == null)
            {
                sf_logger.warn("Missing sample data on {}", chrPos);
                return;
            }

            VariantLocus varLoc;
            m_locationsOfInterest.TryGetValue(chrPos, out varLoc);
            if (varLoc == null)
            {
                sf_logger.warn("Ignoring {}", chrPos);
                return;
            }
            if (sampleAlleleMap.Keys.Contains(chrPos))
            {
                addWarning(chrPos, "Duplicate entry: first valid position wins");
                return;
            }

            if (sampleData.Count() > 1)
            {
                addWarning(chrPos, "Multiple samples found, only using first entry");
            }

            String gt = sampleData.ElementAt(0).getProperty("GT");
            if (gt == null)
            {
                addWarning(chrPos, "Ignoring: no genotype");
                return;
            }
            //sf_noCallPattern.matcher(gt).matches()
            if (Regex.IsMatch(gt, sf_noCallPattern))
            {
                addWarning(chrPos, "Ignoring: no call (" + gt + ")");
                return;
            }

            var gtStrList = Regex.Split(gt, sf_gtDelimiter).ToList();

            List<int> alleleIdxsList = new List<int>();
            gtStrList.ForEach(allele => alleleIdxsList.Add(int.Parse(allele)));
            int[] alleleIdxs = alleleIdxsList.ToArray();
            // normalize alleles to use same syntax as haplotype definition
            List<String> alleles = new List<String>();
            if (position.getAltBases().Count() == 0)
            {
                String gt1 = position.getAllele(0);
                validateAlleles(chrPos, gt1, null);

                String[] g = normalizeAlleles(gt1, null);
                alleles.Add(g[0]);

            }
            else
            {
                foreach (String gt2 in position.getAltBases())
                {
                    String gt1 = position.getAllele(0);
                    validateAlleles(chrPos, gt1, gt2);

                    String[] g = normalizeAlleles(gt1, gt2);
                    String g1 = g[0];
                    String g2 = g[1];
                    if (alleles.Count() == 0)
                    {
                        alleles.Add(g1);
                        alleles.Add(g2);
                    }
                    else
                    {
                        if (!alleles.ElementAt(0).Equals(g1))
                        {
                            throw new ArgumentException("Getting different reference allele for " + chrPos);
                        }
                        alleles.Add(g2);
                    }
                }
            }

            String a1 = alleles.ElementAt(alleleIdxs[0]);
            String a2 = null;
            if (alleleIdxs.Length > 1)
            {
                a2 = alleles.ElementAt(alleleIdxs[1]);
            }
            else
            {
                addWarning(chrPos, "Only a single allele found");
            }

            // genotype divided by "|" if phased and "/" if unphased
            Boolean isPhased = true;
            if (gt.Contains("/") && a2 != null && !a1.ToLower().Equals(a2.ToLower()))
            {
                isPhased = false;
            }

            List<String> vcfAlleles = new List<String>();
            vcfAlleles.Add(position.getRef());
            vcfAlleles.AddRange(position.getAltBases());

            SampleAllele sampleAllele = new SampleAllele(position.getChromosome(), position.getPosition(), a1, a2, isPhased, vcfAlleles);
            if (varLoc.Type == VariantType.DEL.ToString() && !sampleAllele.isVcfAlleleADeletion())
            {
                // must be deletion if expecting deletion because deletions require anchor bases and -1 in position
                string str = string.Empty;
                sampleAllele.getVcfAlleles().ForEach(x => str = str + x + "/");
                str = str.Remove(str.LastIndexOf('/'));
                addWarning(chrPos, "Ignoring: expecting deletion but alleles do not appear to be in expected format (got " +
                    str + ")");
                return;
            }
            sampleAlleleMap.Add(chrPos, sampleAllele);
        }
        /**
   * Normalize alleles from VCF to match syntax from allele definitions
   */
        private String[] normalizeAlleles(String refAllele, String varAllele)
        {
            //Preconditions.checkNotNull(refAllele);

            refAllele = refAllele.ToUpper();

            if (varAllele != null)
            {
                varAllele = varAllele.ToUpper();
                return new String[] { refAllele, varAllele };
            }
            else
            {
                return new String[] { refAllele };
            }
        }


        /**
         * Validate GT input per VCF 4.2 specification.
         */
        private static void validateAlleles(String chrPos, String gt1, String gt2)
        {

            StringBuilder problems = new StringBuilder();
            if (gt1.StartsWith("<"))
            {
                problems.Append("Don't know how to handle ref structural variant '")
                    .Append(gt1)
                    .Append("'");
            }
            else if (gt1.ToUpper().Contains("N"))
            {
                problems.Append("Don't know how to handle ambiguous allele in ref '")
                    .Append(gt1)
                    .Append("'");
            }
            else if (!Regex.IsMatch(gt1, sf_allelePattern))
            {
                problems.Append("Unsupported bases in ref '")
                    .Append(gt1)
                    .Append("'");
            }

            if (gt2 != null)
            {
                if (gt2.StartsWith("<"))
                {
                    if (problems.Length > 0)
                    {
                        problems.Append("\r\n");
                    }
                    problems.Append("Don't know how to handle alt structural variant '")
                        .Append(gt2)
                        .Append("'");
                }
                else
                {
                    if (!Regex.IsMatch(gt1, sf_allelePattern))
                    {
                        //if (!sf_allelePattern.matcher(gt1).matches()) {
                        if (problems.Length > 0)
                        {
                            problems.Append("\r\n");
                        }
                        if (gt2.ToUpper().Contains("N"))
                        {
                            problems.Append("Don't know how to handle ambiguous allele in alt '")
                                .Append(gt2)
                                .Append("'");
                        }
                        else if (gt2.Contains("*"))
                        {
                            problems.Append("Don't know how to handle missing allele in alt '")
                                .Append(gt2)
                                .Append("'");
                        }
                        else
                        {
                            problems.Append("Unsupported bases in alt '")
                                .Append(gt1)
                                .Append("'");
                        }
                    }
                }
            }

            if (problems.Length > 0)
            {
                throw new Exception("Problem at " + chrPos + ":" + "\r\n" + problems.ToString());
            }
        }

    }

    public class ListToLookup
    {
        public String Key { get; set; }
        public String Value { get; set; }
    }

}