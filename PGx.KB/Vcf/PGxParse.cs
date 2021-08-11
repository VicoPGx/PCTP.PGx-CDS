using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PGx.KB.Infrastructure;
using System.IO;
using PGx.KB.Vcf.Haplotype;
using PGx.KB.Vcf.Haplotype.Model;
using PGx.Model.Entities;

namespace PGx.KB.Vcf
{

 //Class to run the PharmCAT tool from input VCF file to final output report.

    public class PGxParse
    {
        private PGx_KBEntities context;
        private static Logger sf_logger = new Logger();
        private NamedAlleleMatcher m_namedAlleleMatcher;
       // private PGx.KB.Vcf.Reporter.ReporterClass m_reporter;
  
        
        //private Boolean m_keepMatcherOutput = false;
        //private Boolean m_writeJsonReport = false;


        //    //初始化PharmCAT，此间读取各个json file 中的Allele定义（相当于知识库）并将所得值赋值给DefinitionReader.m_definitionFiles
        //    //一个实例化一个NamedAlleleMatcher对象赋值给PharmCAT.m_namedAlleleMatcher引用,实例化过程中将definitionReader对象赋给NamedAlleleMatcher.m_definitionReader

        public PGxParse(PGx_KBEntities con)
        {
           context = con;
           DefinitionReader m_definitionReader = new DefinitionReader();
           m_definitionReader.read(context);
           m_namedAlleleMatcher = new NamedAlleleMatcher(m_definitionReader, true, true);  
        }

        public Result CallFromVcf(string vcfFile)
        {
            return m_namedAlleleMatcher.Call(vcfFile);         
        }

        public Result PhenotypeCall(Result result)
        {

            foreach (var call in result.GeneCalls)
            {
                var geneDef = context.DefinitionFile.Where(x => x.GeneSymbol == call.Gene).FirstOrDefault();
                if (geneDef == null)
                { continue; }
                var namedAlleles = geneDef.NamedAllele.ToList();
                var namedPhenotypes = geneDef.PhenotypeMap.ToList();
                if (namedPhenotypes.Count() == 0)
                {
                    continue;
                }
                foreach (var dip in call.DiplotypeMatch)
                {
                    string[] alleles = dip.Name.Split('/');

                    string allele1 = alleles[0];
                    string allele2 = alleles[1];

                    List<string> funAB = new List<string>();
                    var namedAlleleA = namedAlleles.Where(x => x.Name == allele1).FirstOrDefault();
                    var namedAlleleB = namedAlleles.Where(x => x.Name == allele2).FirstOrDefault();
                    if (string.IsNullOrEmpty(namedAlleleA.M_Function) == true ||string.IsNullOrEmpty (namedAlleleB.M_Function) ==true)
                        continue;
                    funAB.Add(namedAlleleA.M_Function.ToLower());
                    funAB.Add(namedAlleleB.M_Function.ToLower());
                    funAB.Sort();

                    var funcPair = string.Join("/", funAB);

                    foreach (var pheno in namedPhenotypes)
                    {

                        List<string> funPairDefList = new List<string>();
                        pheno.PhenotypeDef.ToList().ForEach(x =>
                        {
                            List<string> funList = new List<string>();
                            funList.Add(x.FunctionA.ToLower());
                            funList.Add(x.FunctionB.ToLower());
                            funList.Sort();
                            funPairDefList.Add(string.Join("/", funList));
                        });

                        if (funPairDefList.Any(x => x == funcPair))
                        {
                            dip.Phenotype = pheno.Phenotype;
                            break;
                        }
                    }
                }     
            }
            return result;
        }

        public void SaveGeneCalls(int fileId, List<GeneCall> geneCalls)
        {
            RemoveExistGeneCall(fileId);
            AddGeneCall(fileId, geneCalls);
        }
        public void RemoveExistGeneCall(int fileId)
        {
            var file = context.RAW_DATA_FILE.Where(x => x.ID == fileId).FirstOrDefault();
            if (file == null)
            {
                return;
            }
            List<GeneCall> existCall = file.GeneCall.ToList();
            //var existCall = context.GeneCall.Where(x => x.FileId == fileId).ToList();
            if (existCall.Count==0)
            {
                return;
            }
                foreach (var e in existCall)
                {
                    var variants = e.Variants.ToList();
                    var variantsOfInterest = e.VarOfInterest.ToList();
                    var haplotypeMatch = e.HaplotypeMatch.ToList();
                    var diplotypeMatch = e.DiplotypeMatch.ToList();

                    if (variants != null && variants.Count() > 0)
                    {

                        foreach (var v in variants)
                        {
                            context.Variant.Remove(v);
                        }
                        context.SaveChanges();
                    }

                    if (e.VarOfInterest != null && e.VarOfInterest.Count > 0)
                    {
                        foreach (var o in variantsOfInterest)
                        {
                            context.VarOfInterest.Remove(o);
                        }
                        context.SaveChanges();
                    }

                    if (e.HaplotypeMatch != null && e.HaplotypeMatch.Count > 0)
                    {
                        foreach (var h in haplotypeMatch)
                        {
                            context.HaplotypeMatch.Remove(h);

                        }
                        context.SaveChanges();
                    }
                    if (e.DiplotypeMatch != null && e.DiplotypeMatch.Count > 0)
                        foreach (var d in diplotypeMatch)
                        {
                            context.DiplotypeMatch.Remove(d);

                        }
                    context.SaveChanges();
                    context.GeneCall.Remove(e);
                    context.SaveChanges();
                }           
        }
        public void AddGeneCall(int fileId, List<GeneCall> geneCalls)
        {
            var dataFile = context.RAW_DATA_FILE.Where(x => x.ID == fileId).FirstOrDefault();
            foreach (GeneCall call in geneCalls)
            {
                if (call.DiplotypeMatch == null || call.DiplotypeMatch.Count == 0)
                {
                    continue;
                }
                GeneCall geneCall = new GeneCall
                {
                    RAW_DATA_FILE=dataFile,
                    AlleleDefinitionVersion = call.AlleleDefinitionVersion,
                    Chromosome = call.Chromosome,
                    Gene = call.Gene,
                    IsPhased = call.IsPhased,
                    UncallableHaplotypes = call.UncallableHaplotypes,
                    IgnoredHaplotypes = call.IgnoredHaplotypes,
                    TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                    FileId = fileId,
                    Diplotype=call.DiplotypeMatch.FirstOrDefault().Name,
                    Phenotype=call.DiplotypeMatch.FirstOrDefault().Phenotype,
                    IsVcfCall=true
                };
                context.GeneCall.Add(geneCall);
                context.SaveChanges();
                //foreach (var v in call.Variants)
                //{
                //    v.GeneCall = geneCall;
                //    context.Variant.Add(v);
                //}
                //context.SaveChanges();
                //foreach (var i in call.VarOfInterest)
                //{
                //    i.GeneCall = geneCall;
                //    context.VarOfInterest.Add(i);
                //}
                //context.SaveChanges();

                //foreach (var h in call.HaplotypeMatch)
                //{
                //    HaplotypeMatch haplotypeMatch = new HaplotypeMatch
                //    {
                //        Name = h.Name,
                //        M_function = h.M_function,
                //        Sequences = h.Sequences, 
                //        GeneCall = geneCall
                //    };


                //    // h.M_haplotype = context.NamedAllele.Where(x => x.DefinitionFile.GeneSymbol == h.M_haplotype.DefinitionFile.GeneSymbol && x.Name == h.M_haplotype.Name).FirstOrDefault();
                //    context.HaplotypeMatch.Add(haplotypeMatch);
                //}
                //context.SaveChanges();

                foreach (var d in call.DiplotypeMatch)
                {
                    DiplotypeMatch diplotypeMatch = new DiplotypeMatch
                    {
                        GeneCall = geneCall,
                        Name = d.Name,
                        Phenotype=d.Phenotype,
                        Score = d.Score,
                    };
                    context.DiplotypeMatch.Add(diplotypeMatch);
                    context.SaveChanges();
                }
            }
            return;
        }

    }
}