using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using PGx.Model.Entities;
using System.Data;
using System.Web.Script.Serialization;
using PGx.KB.Models;

namespace PGx.KB.Controllers
{
    public class PGxKnowledgeBaseController : BaseController
    {
        PGx_KBEntities context = new PGx_KBEntities();
        //
        // GET: /PGxKnowledgeBase/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GuidelineEdit(int? id)
        {

            if (id == null || id == 0)
            {
                return PartialView("GuidelineEdit", new PGxGuideline());
            }
            else
            {
                var guideline = context.PGxGuideline.Where(x => x.ID == id).FirstOrDefault();
                return PartialView("GuidelineEdit", guideline);
            }
        }
        [HttpPost]
        public string GuidelineEdit(PGxGuideline guideline)
        {
            string result = "Save succeed!";
            try
            {
                if (ModelState.IsValid)
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    // string[][] riskPhenotypes = js.Deserialize<string[][]>(guideline.RiskPhenotypeJson);
                    List<Literature> literatureList = js.Deserialize<List<Literature>>(guideline.LiteratureJson);
                    PGxGuideline guidelineItem;
                    if (guideline.ID == 0)
                    {
                        guidelineItem = guideline;
                        
                        try
                        {
                            context.PGxGuideline.Add(guidelineItem);
                            context.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            result = "Save failed!" + e.Message;
                            return result;
                        }
                    }
                    else
                    {
                        guidelineItem = context.PGxGuideline.Where(x => x.ID == guideline.ID).FirstOrDefault();
                    }
                    guidelineItem.Chemical = guideline.Chemical;
                    guidelineItem.GenesInStr = guideline.GenesInStr;
                    guidelineItem.Source = guideline.Source;
                    guidelineItem.ClinicalImplication = guideline.ClinicalImplication;
                    context.SaveChanges();

                    //Save RelatedDefinitionFile

                    if (string.IsNullOrEmpty(guideline.GenesInStr) == false)
                    {
                        var genes = guideline.GenesInStr.Split(',');
                        var definitionFiles = guidelineItem.DefinitionFile.ToList();
                        if (!(definitionFiles.Count() == genes.Count() && definitionFiles.Any(x => genes.Contains(x.GeneSymbol.Trim()))))
                        {
                            foreach (var defFile in definitionFiles)
                            {
                                guidelineItem.DefinitionFile.Remove(defFile);
                                context.SaveChanges();
                            }
                            foreach (var gene in genes)
                            {
                                var relatedDefinitionFile = context.DefinitionFile.Where(x => x.GeneSymbol == gene).FirstOrDefault();
                                guidelineItem.DefinitionFile.Add(relatedDefinitionFile);
                                context.SaveChanges();
                            }
                        }
                    }

                    //save literature
                    var existedLiterature = guidelineItem.Literature.ToList();
                    foreach (var liter in existedLiterature)
                    {
                        context.Literature.Remove(liter);
                        context.SaveChanges();
                    }
                    foreach (var lit in literatureList)
                    {
                        Literature newLiterature = new Literature()
                        {
                            M_title = lit.M_title,
                            M_journal = lit.M_journal,
                            M_year = lit.M_year,
                            M_pmid = lit.M_pmid,
                            PGxGuideline = guidelineItem
                        };
                        context.Literature.Add(newLiterature);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                result = "Save failed," + e.Message + "!";
            }
            return result;
        }

        public string DeleteGuideline(int? id = 0)
        {
            var guideline = context.PGxGuideline.Where(x => x.ID == id).FirstOrDefault();
            if (guideline == null)
                return "The guideline  can't be found!";
            else
            {
                var literatureList = guideline.Literature.ToList();
                
                var relatedGeneList = guideline.RelatedGene.ToList();
                var geneDefList = guideline.DefinitionFile.ToList();
                foreach (var literature in literatureList)
                {
                    context.Literature.Remove(literature);
                    context.SaveChanges();
                }
                foreach (var gene in relatedGeneList)
                {
                    guideline.RelatedGene.Remove(gene);
                    context.SaveChanges();
                }
                foreach (var geneDef in geneDefList)
                {
                    guideline.DefinitionFile.Remove(geneDef);
                    context.SaveChanges();
                }
                
                context.PGxGuideline.Remove(guideline);
                context.SaveChanges();

                return "Success!";
            }
        }

        public ActionResult TherapeuticKM()
        {
            return View("DrugList");
        }

        public ActionResult VariantEdit(int? id)
        {
            if (id == 0 || id == null)
            {
                int geneId = int.Parse(Request["geneId"]);
                return PartialView(new VariantLocus() { DefinitionFileID = geneId });
            }
            var variant = context.VariantLocus.Where(x => x.ID == id).FirstOrDefault();
            return PartialView("VariantEdit", variant);
        }


        [HttpPost]
        public string VariantEdit(VariantLocus variantLocus)
        {
            if (variantLocus.ID != 0)
            {
                var existVariant = context.VariantLocus.Where(x => x.ID == variantLocus.ID).FirstOrDefault();
                existVariant.Position = variantLocus.Position;
                existVariant.Rsid = variantLocus.Rsid;
                existVariant.Alleles = variantLocus.Alleles;
                existVariant.ChromosomeHgvsName = variantLocus.ChromosomeHgvsName;
                existVariant.GeneHgvsName = variantLocus.GeneHgvsName;
                existVariant.ProteinNote = variantLocus.ProteinNote;
                existVariant.ResourceNote = variantLocus.ResourceNote;

                context.SaveChanges();
            }
            else
            {
                variantLocus.DefinitionFile = context.DefinitionFile.Where(x => x.ID == variantLocus.DefinitionFileID).FirstOrDefault();
                variantLocus.Chromosome = variantLocus.DefinitionFile.Chromosome;
                context.VariantLocus.Add(variantLocus);
                context.SaveChanges();
            }

            return "Success!";
        }
        public ActionResult AlleleEdit(int? id = 0)
        {

            if (id == 0)
            {
                var fileIDStr = Request["fileID"];
                int fileID = int.Parse(fileIDStr);
                return PartialView(new NamedAllele() { ID = 0, DefinitionFileID = fileID });
            }
            return PartialView(context.NamedAllele.Where(x => x.ID == id).FirstOrDefault());
        }
        [HttpPost]
        public string AlleleEdit(NamedAllele namedAllele)
        {
            if (ModelState.IsValid)
            {
                var js = new JavaScriptSerializer();
                Dictionary<string, string> locusAlleleList = new Dictionary<string, string>();
                if (namedAllele.LocusAlleleJson != null)
                    locusAlleleList = js.Deserialize<Dictionary<string, string>>(namedAllele.LocusAlleleJson);
                List<PopulationFrequency> newAlleleFrequencyList = new List<PopulationFrequency>();
                if (namedAllele.AlleleFrequency != null)
                    newAlleleFrequencyList = js.Deserialize<List<PopulationFrequency>>(namedAllele.AlleleFrequency);
                if (namedAllele.M_Function == "Unselected")
                    namedAllele.M_Function = null;
                if (namedAllele.ID != 0)
                {
                    var existAllele = context.NamedAllele.Where(x => x.ID == namedAllele.ID).FirstOrDefault();
                    existAllele.Name = namedAllele.Name;
                    existAllele.M_Function = namedAllele.M_Function;
                    existAllele.IsRefAllele = namedAllele.IsRefAllele;
                    existAllele.ActivityValue = namedAllele.ActivityValue;
                    context.SaveChanges();
                    if (existAllele.PopulationFrequency.Count > 0)//删除已有的allele frequency
                    {
                        var existFreList = existAllele.PopulationFrequency.ToList();
                        foreach (var fre in existFreList)
                        {
                            context.PopulationFrequency.Remove(fre);
                            context.SaveChanges();
                        }
                    }

                    foreach (var newAlleleFre in newAlleleFrequencyList)
                    {
                        newAlleleFre.NamedAllele = existAllele;
                        context.PopulationFrequency.Add(newAlleleFre);
                        context.SaveChanges();
                    }


                    foreach (var locusAllele in locusAlleleList)
                    {
                        var locusID = int.Parse(locusAllele.Key);
                        var existAlleleDef = existAllele.NamedAlleleDefinition.Where(x => x.VariantLocusID == locusID).FirstOrDefault();
                        if (existAlleleDef != null)
                        {
                            existAlleleDef.Allele = locusAllele.Value;
                            context.SaveChanges();
                        }
                        else
                        {
                            var locus = context.VariantLocus.Where(x => x.ID == locusID).FirstOrDefault();
                            var newAlleleDef = new NamedAlleleDefinition
                            {
                                Allele = locusAllele.Value,
                                VariantLocus = locus,
                                NamedAllele = existAllele
                            };
                            context.NamedAlleleDefinition.Add(newAlleleDef);
                            context.SaveChanges();
                        }
                    }

                }
                else
                {
                    namedAllele.DefinitionFile = context.DefinitionFile.Where(x => x.ID == namedAllele.DefinitionFileID).FirstOrDefault();
                    context.NamedAllele.Add(namedAllele);
                    context.SaveChanges();
                    foreach (var locusAllele in locusAlleleList)
                    {
                        var locusID = int.Parse(locusAllele.Key);
                        var locus = context.VariantLocus.Where(x => x.ID == locusID).FirstOrDefault();
                        var newAlleleDef = new NamedAlleleDefinition
                        {
                            Allele = locusAllele.Value,
                            VariantLocus = locus,
                            NamedAllele = namedAllele
                        };
                        context.NamedAlleleDefinition.Add(newAlleleDef);
                        context.SaveChanges();
                    }
                    foreach (var newFre in newAlleleFrequencyList)
                    {
                        newFre.NamedAllele = namedAllele;
                        context.PopulationFrequency.Add(newFre);
                        context.SaveChanges();
                    }
                }

                return "Success!";
            }
            else
                return "False!";
        }

        public string DeleteVariant(int? id = 0)
        {
            if (id == 0)
                return "The variant has not been saved.";
            else
            {
                var variant = context.VariantLocus.Where(x => x.ID == id).FirstOrDefault();
                var alleleDef = variant.AlleleDefinition.ToList();

                foreach (var locusBase in alleleDef)
                {
                    context.NamedAlleleDefinition.Remove(locusBase);
                    context.SaveChanges();
                }

                context.VariantLocus.Remove(variant);
                context.SaveChanges();
            }
            return "Allele has been deleted.";

        }

        public string DeleteGene(int? id = 0)
        {
            if (id == 0)
            {
                return "The gene is not found!";
            }
            else
            {
                var geneDef = context.DefinitionFile.Where(x => x.ID == id).FirstOrDefault();
                if (geneDef == null)
                {
                    return "The gene is not found.";
                } var variantList = geneDef.VariantLocus.ToList();
                var namedAlleleList = geneDef.NamedAllele.ToList();
                var namedPhenotypeList = geneDef.PhenotypeMap.ToList();

                foreach (var allele in namedAlleleList)
                {
                    var alleleFrequencyList = allele.PopulationFrequency.ToList();
                    foreach (var alleleFrequency in alleleFrequencyList)
                    {
                        context.PopulationFrequency.Remove(alleleFrequency);
                        context.SaveChanges();
                    }

                    var alleleDefinitionList = allele.NamedAlleleDefinition.ToList();
                    {
                        foreach (var alleleDefinition in alleleDefinitionList)
                        {
                            context.NamedAlleleDefinition.Remove(alleleDefinition);
                            context.SaveChanges();
                        }
                    }

                    context.NamedAllele.Remove(allele);
                    context.SaveChanges();
                }
                foreach (var variant in variantList)
                {
                    context.VariantLocus.Remove(variant);
                    context.SaveChanges();
                }

                foreach (var phenotype in namedPhenotypeList)
                {
                    var phenotypeFrequencyList = phenotype.PhenotypePopFreq.ToList();
                    foreach (var phenotypeFrequency in phenotypeFrequencyList)
                    {
                        context.PhenotypePopFreq.Remove(phenotypeFrequency);
                        context.SaveChanges();
                    }

                    var phenoDefList = phenotype.PhenotypeDef.ToList();
                    foreach (var pheDef in phenoDefList)
                    {
                        context.PhenotypeDef.Remove(pheDef);
                        context.SaveChanges();
                    }

                    context.NamedPhenotype.Remove(phenotype);
                    context.SaveChanges();
                }

                context.DefinitionFile.Remove(geneDef);
                context.SaveChanges();

                return "The gene has been deleted.";
            }
        }

        public string DeleteAllele(int? id = 0)
        {
            if (id == 0)
                return "The allele is not found!";
            else
            {
                var namedAllele = context.NamedAllele.Where(x => x.ID == id).FirstOrDefault();
                var alleleDef = namedAllele.NamedAlleleDefinition.ToList();
                var popFreList = namedAllele.PopulationFrequency.ToList();
                foreach (var locusBase in alleleDef)
                {
                    context.NamedAlleleDefinition.Remove(locusBase);
                    context.SaveChanges();
                }
                foreach (var popFre in popFreList)
                {
                    context.PopulationFrequency.Remove(popFre);
                    context.SaveChanges();
                }
                context.NamedAllele.Remove(namedAllele);
                context.SaveChanges();
            }
            return "Allele has been deleted.";

        }

        public ActionResult PhenotypeEdit(string phe)
        {
            var fileIDStr = Request["fileID"];
            var fileID = int.Parse(fileIDStr);
            if (string.IsNullOrEmpty(phe) == true)
            {
                return PartialView(new NamedPhenotype() { DefinitionFileID = fileID });
            }
            string gene = phe.Split(',').ElementAt(0);
            string pheno = phe.Split(',').ElementAt(1);
            return PartialView(context.NamedPhenotype.Where(x => x.Symbol == gene && x.Phenotype == pheno).FirstOrDefault());
        }
        public ActionResult GenomicKM()
        {
            return View("GeneList", context.DefinitionFile.ToList());
        }
        public ActionResult GeneEdit(int? id)
        {
            if (id == null || id == 0)
            {
                return PartialView(new DefinitionFile());
            }
            return PartialView(context.DefinitionFile.Where(x => x.ID == id).FirstOrDefault());
        }

        [HttpPost]
        public string GeneEdit(DefinitionFile defFile)
        {
            string result = "Save succeed!";
            DefinitionFile definitionFile;
            if (defFile.ID != 0)
            {
                definitionFile = context.DefinitionFile.Where(x => x.ID == defFile.ID).FirstOrDefault();
            }
            else
            {
                definitionFile = new DefinitionFile();
            }
            if (defFile.FrequencyFile != null && defFile.FrequencyFile.ContentLength > 0)
            {
                if (string.IsNullOrEmpty(definitionFile.AlleleFrequencyTable) == false)
                {
                    var oldFilePath = Server.MapPath(definitionFile.AlleleFrequencyTable);
                    System.IO.File.Delete(oldFilePath);
                }
                HttpPostedFileBase file = defFile.FrequencyFile;
                //if (file.ContentLength > 0)                  
                var fileName = Guid.NewGuid() + "-" + Path.GetFileName(file.FileName);
                var folderPath = Server.MapPath("~/Upload/RawDataFile/");
                Directory.CreateDirectory(folderPath);
                //用绝对路径存储文件
                var absolutePath = Path.Combine(folderPath, fileName);
                file.SaveAs(absolutePath);

                //数据库仅存储相对路径         
                var relativePath = "/Upload/RawDataFile/" + fileName;
                definitionFile.AlleleFrequencyTable = relativePath;

            }
            if (defFile.AlleleDefinitionFile != null && defFile.AlleleDefinitionFile.ContentLength > 0)
            {
                if (string.IsNullOrEmpty(definitionFile.AlleleDefinitionTable) == false)
                {
                    var oldFilePath = Server.MapPath(definitionFile.AlleleDefinitionTable);
                    System.IO.File.Delete(oldFilePath);
                }
                HttpPostedFileBase file = defFile.AlleleDefinitionFile;
                //if (file.ContentLength > 0)                  
                var fileName = Guid.NewGuid() + "-" + Path.GetFileName(file.FileName);
                var folderPath = Server.MapPath("~/Upload/RawDataFile/");
                Directory.CreateDirectory(folderPath);
                //用绝对路径存储文件
                var absolutePath = Path.Combine(folderPath, fileName);
                file.SaveAs(absolutePath);
                //数据库仅存储相对路径         
                var relativePath = "/Upload/RawDataFile/" + fileName;
                definitionFile.AlleleDefinitionTable = relativePath;
            }
            if (defFile.FunctionFile != null && defFile.FunctionFile.ContentLength > 0)
            {
                if (string.IsNullOrEmpty(definitionFile.FunctionTable) == false)
                {
                    var oldFilePath = Server.MapPath(definitionFile.FunctionTable);
                    System.IO.File.Delete(oldFilePath);
                }

                HttpPostedFileBase file = defFile.FunctionFile;
                //if (file.ContentLength > 0)                  
                var fileName = Guid.NewGuid() + "-" + Path.GetFileName(file.FileName);
                var folderPath = Server.MapPath("~/Upload/RawDataFile/");
                Directory.CreateDirectory(folderPath);
                //用绝对路径存储文件
                var absolutePath = Path.Combine(folderPath, fileName);
                file.SaveAs(absolutePath);
                //数据库仅存储相对路径         
                var relativePath = "/Upload/RawDataFile/" + fileName;
                definitionFile.FunctionTable = relativePath;
            }
            definitionFile.Chromosome = defFile.Chromosome;
            definitionFile.GeneName = defFile.GeneName;
            definitionFile.GeneSymbol = defFile.GeneSymbol;
            definitionFile.GenomeBuild = defFile.GenomeBuild;
            definitionFile.NamedFunctions = defFile.NamedFunctions;
            definitionFile.Populations = defFile.Populations;
            definitionFile.RefSeqChromosome = defFile.RefSeqChromosome;
            definitionFile.RefSeqGene = defFile.RefSeqGene;
            definitionFile.RefSeqProtein = defFile.RefSeqProtein;
            definitionFile.Orientation = defFile.Orientation;
            definitionFile.ModificationDate = DateTime.Now;
            if (definitionFile.ID == 0)
            {
                try
                {
                    context.DefinitionFile.Add(definitionFile);
                    context.SaveChanges();

                }
                catch (Exception e)
                {
                    result = string.Format("Save failed! {0}", e.Message);

                }
            }
            else
            {
                try
                {
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    result = string.Format("Save failed! {0}", e.Message);
                }
            }
            return result;
        }

        public string DeleteFilePath(int? id)
        {
            var pathType = Request["pathType"];
            var geneDef = context.DefinitionFile.Where(x => x.ID == id).FirstOrDefault();
            if (pathType == "definition")
            {
                var serverPath = Server.MapPath(geneDef.AlleleDefinitionTable);
                System.IO.File.Delete(serverPath);
                geneDef.AlleleDefinitionTable = null;
            }
            else
                if (pathType == "function")
                {
                    var serverPath = Server.MapPath(geneDef.FunctionTable);
                    System.IO.File.Delete(serverPath);
                    geneDef.FunctionTable = null;
                }
                else
                {
                    var serverPath = Server.MapPath(geneDef.AlleleFrequencyTable);
                    System.IO.File.Delete(serverPath);
                    geneDef.AlleleFrequencyTable = null;
                }
            context.SaveChanges();
            return "File path has been deleted";
        }

        [HttpPost]
        public string PhenotypeEdit(NamedPhenotype namedPhenotype)
        {
            if (ModelState.IsValid)
            {

                if (namedPhenotype.ID != 0)
                {
                    var oldPhe = context.NamedPhenotype.Where(x => x.ID == namedPhenotype.ID).FirstOrDefault();
                    oldPhe.Phenotype = namedPhenotype.Phenotype;
                    oldPhe.Symbol = namedPhenotype.Symbol;
                    oldPhe.Description = namedPhenotype.Description;
                    oldPhe.GenotypeList = namedPhenotype.GenotypeList;
                    oldPhe.ActivityScore = namedPhenotype.ActivityScore;
                    context.SaveChanges();
                    var js = new JavaScriptSerializer();
                    var phenoPopFreList = js.Deserialize<List<PhenotypePopFreq>>(namedPhenotype.PopulationFreList);
                    if (oldPhe.PhenotypePopFreq.Count > 0)
                    {
                        var oldFreList = oldPhe.PhenotypePopFreq.ToList();
                        foreach (var oldFre in oldFreList)
                        {
                            context.PhenotypePopFreq.Remove(oldFre);
                            context.SaveChanges();
                        }
                    }

                    foreach (var newFre in phenoPopFreList)
                    {
                        if (string.IsNullOrEmpty(newFre.Population) && newFre.Frequency == null)
                            continue;
                        newFre.NamedPhenotype = oldPhe;
                        context.PhenotypePopFreq.Add(newFre);
                        context.SaveChanges();
                    }


                    if (oldPhe.PhenotypeDef != null && oldPhe.PhenotypeDef.Count > 0)
                    {
                        var oldPheDefList = oldPhe.PhenotypeDef.ToList();
                        foreach (var oldPheDef in oldPheDefList)
                        {
                            context.PhenotypeDef.Remove(oldPheDef);
                            context.SaveChanges();
                        }
                    }

                    var funcPairArray = namedPhenotype.GenotypeList.Split(',');
                    foreach (var funcPair in funcPairArray)
                    {
                        PhenotypeDef newPheDef = new PhenotypeDef
                        {
                            NamedPhenotype = oldPhe,
                            GeneSymbol = oldPhe.Symbol,
                            Genotype = funcPair,
                            Phenotype = oldPhe.Phenotype,
                            FunctionA = funcPair.Split('/')[0],
                            FunctionB = funcPair.Split('/')[1]
                        };
                        context.PhenotypeDef.Add(newPheDef);
                        context.SaveChanges();
                    }
                }
                else
                {
                    namedPhenotype.DefinitionFile = context.DefinitionFile.Where(x => x.ID == namedPhenotype.DefinitionFileID).FirstOrDefault();
                    namedPhenotype.Symbol = namedPhenotype.DefinitionFile.GeneSymbol;
                    context.NamedPhenotype.Add(namedPhenotype);
                    context.SaveChanges();
                    var js = new JavaScriptSerializer();
                    var phenoPopFreList = js.Deserialize<List<PhenotypePopFreq>>(namedPhenotype.PopulationFreList);


                    foreach (var newFre in phenoPopFreList)
                    {
                        if (string.IsNullOrEmpty(newFre.Population) || newFre.Frequency == null)
                            continue;
                        newFre.NamedPhenotype = namedPhenotype;
                        context.PhenotypePopFreq.Add(newFre);
                        context.SaveChanges();
                    }

                    List<string> funcPairArray = new List<string>();
                    if (string.IsNullOrEmpty(namedPhenotype.GenotypeList) == false)
                    {
                        funcPairArray = namedPhenotype.GenotypeList.Split(',').ToList();
                    }
                    foreach (var funcPair in funcPairArray)
                    {
                        List<string> funAB = funcPair.Split('/').ToList();
                        funAB.Sort();
                        PhenotypeDef newPheDef = new PhenotypeDef
                        {
                            NamedPhenotype = namedPhenotype,
                            GeneSymbol = namedPhenotype.Symbol,
                            Genotype = funcPair,
                            Phenotype = namedPhenotype.Phenotype,
                            FunctionA = funAB.ElementAt(0),
                            FunctionB = funAB.ElementAt(1)
                        };
                        context.PhenotypeDef.Add(newPheDef);
                        context.SaveChanges();
                    }
                }
            }
            return "Phenotype has been saved successfully";
        }

        public string DeletePhenotype(int? id = 0)
        {
            if (id == 0)
            {
                return "The phenotype was not found!";
            }
            else
            {
                var phenotype = context.NamedPhenotype.Where(x => x.ID == id).FirstOrDefault();
                var frequencyList = phenotype.PhenotypePopFreq.ToList();
                var defList = phenotype.PhenotypeDef.ToList();
                foreach (var frequency in frequencyList)
                {
                    context.PhenotypePopFreq.Remove(frequency);
                    context.SaveChanges();
                }
                foreach (var def in defList)
                {
                    context.PhenotypeDef.Remove(def);
                    context.SaveChanges();
                }
                context.NamedPhenotype.Remove(phenotype);
                context.SaveChanges();
                return "Success!";
            }
        }

        public ActionResult DosingGuidence(int id)
        {
            return View(context.DosingGuidence.Where(x => x.PGxGuidelineID == id).ToList());
        }

        public ActionResult DosingEdit(int? id)
        {
            if (id == null || id == 0)
            {
                var guidelineId = int.Parse(Request["guidelineId"]);
                return PartialView(new DosingGuidence() { PGxGuideline = context.PGxGuideline.Where(x => x.ID == guidelineId).FirstOrDefault(), PGxGuidelineID = guidelineId });
            }
            return PartialView(context.DosingGuidence.Where(x => x.ID == id).FirstOrDefault());
        }

        [HttpPost]
        public string DosingEdit(DosingGuidence dosingGuidence)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<RecommPheno> genePhenotypeList = new List<RecommPheno>();
            if (string.IsNullOrEmpty(dosingGuidence.GenePhenotype) == false)
                genePhenotypeList = js.Deserialize<List<RecommPheno>>(dosingGuidence.GenePhenotype);
            var dosing = context.DosingGuidence.Where(x => x.ID == dosingGuidence.ID).FirstOrDefault();
            if (dosing == null)
                dosing = new DosingGuidence();
            List<string> phenoList = new List<string>();
            if (genePhenotypeList.Count > 0)
                dosing.Name = string.Empty;
            foreach (var pheno in genePhenotypeList)
            {
                if (pheno.Phenotype == "Unselected" || string.IsNullOrEmpty(pheno.Phenotype) == true)
                    continue;
                phenoList.Add(pheno.Gene + ":" + pheno.Phenotype);
                dosing.Name = dosing.Name.Trim() + " " + pheno.Gene.Trim() + " " + pheno.Phenotype.Trim();
            }

            if (phenoList.Count > 0)
            {
                phenoList.Sort();
                dosing.Phenotype = string.Join(";", phenoList);
            }

            dosing.Name = dosing.Name.Trim();
            dosing.RxChange = dosingGuidence.RxChange;
            dosing.Strength = dosingGuidence.Strength;
            dosing.Implication = dosingGuidence.Implication;
            dosing.Recommendation = dosingGuidence.Recommendation;
            dosing.PGxGuidelineID = dosingGuidence.PGxGuidelineID;

            if (dosingGuidence.ID > 0)
            {
                context.SaveChanges();
                //Remove exsted recommendatonPhenotype
                var existedPhenos = dosing.RecommendationPhenotype.ToList();
                foreach (var recommPheno in existedPhenos)
                {
                    context.RecommendationPhenotype.Remove(recommPheno);
                    context.SaveChanges();
                }
            }
            else
            {
                PGxGuideline guideline = context.PGxGuideline.Where(x => x.ID == dosingGuidence.PGxGuidelineID).FirstOrDefault();
                dosing.PGxGuideline = guideline;
                context.DosingGuidence.Add(dosing);
                context.SaveChanges();
            }
            //Add new recommendatonPhenotype
            foreach (var recomm in genePhenotypeList)
            {
                RecommendationPhenotype newRecomm = new RecommendationPhenotype
                {
                    Gene = recomm.Gene,
                    Phenotype = recomm.Phenotype,
                    DosingGuidence = dosing
                };
                context.RecommendationPhenotype.Add(newRecomm);
                context.SaveChanges();
            }
            return "Success!";
        }

        public string DeleteDosing(int? id = 0)
        {
            if (id == 0)
            {
                return "The content was not found!";
            }
            else
            {
                var dosingGuidedance = context.DosingGuidence.Where(x => x.ID == id).FirstOrDefault();
                var genePhenotypeList = dosingGuidedance.RecommendationPhenotype.ToList();

                foreach (var genePhenotype in genePhenotypeList)
                {
                    context.RecommendationPhenotype.Remove(genePhenotype);
                    context.SaveChanges();
                }
                context.DosingGuidence.Remove(dosingGuidedance);
                context.SaveChanges();
                return "Success!";
            }
        }


        public PartialViewResult NamedPhenotypeList(string gene)
        {
            var geneDef = context.DefinitionFile.Where(x => x.GeneSymbol == gene).FirstOrDefault();
            var phenotypeList = geneDef.PhenotypeMap.Select(x => x.Phenotype).ToList();
            //var phenoStr = string.Join(",", phenotypeList);
            return PartialView(phenotypeList);
        }

        public JsonResult RiskPhenotypeCalc(int id)
        {

            var gene = Request["gene"];
            var phenotype = Request["phenotype"];
            var population = Request["population"];
            var definitionFile = context.DefinitionFile.Where(x => x.GeneSymbol == gene).FirstOrDefault();
            var namedPhenotype = context.NamedPhenotype.Where(x => x.Symbol == gene && x.Phenotype == phenotype).FirstOrDefault();
            var phenotypeDefinitions = namedPhenotype.PhenotypeDef.ToList();
            var namedAllele = namedPhenotype.DefinitionFile.NamedAllele.ToList();
            decimal? phenoPopulationFre = 0;
            foreach (var phenoDef in phenotypeDefinitions)
            {
                var funA = phenoDef.FunctionA.ToLower();
                var funB = phenoDef.FunctionB.ToLower();
                var alleleAlist = namedAllele.Where(x => string.IsNullOrEmpty(x.M_Function) == false && x.M_Function.ToLower() == funA).ToList();
                var alleleBlist = namedAllele.Where(x => string.IsNullOrEmpty(x.M_Function) == false && x.M_Function.ToLower() == funB).ToList();
                decimal? funAfre = 0, funBfre = 0, funcPairScore = null;
                if (funA != funB)
                {
                    foreach (var alleleA in alleleAlist)
                    {
                        var alleleAfre = alleleA.PopulationFrequency.Where(x => x.Population.Trim() == population).FirstOrDefault();
                        if (alleleAfre != null)
                        {
                            if (alleleAfre.Frequency != null)
                            {
                                funAfre += alleleAfre.Frequency;
                            }
                        }
                    }
                    foreach (var alleleB in alleleBlist)
                    {
                        var alleleBfre = alleleB.PopulationFrequency.Where(x => x.Population.Trim() == population.Trim()).FirstOrDefault();
                        if (alleleBfre != null)
                        {
                            if (alleleBfre.Frequency != null)
                            {
                                funBfre += alleleBfre.Frequency;
                            }
                        }

                    }
                    funcPairScore = 2 * funAfre * funBfre;
                }
                else
                {
                    foreach (var alleleA in alleleAlist)
                    {
                        var alleleAfre = alleleA.PopulationFrequency.Where(x => x.Population.Trim() == population.Trim()).FirstOrDefault();
                        if (alleleAfre != null)
                        {
                            if (alleleAfre.Frequency != null)
                            {
                                funAfre += alleleAfre.Frequency;
                            }
                        }
                    }
                    funcPairScore = funAfre * funAfre;

                }
                phenoPopulationFre += (decimal?)funcPairScore;
            }
            var result = new Dictionary<string, string>();
            result.Add("state", "sucess");
            result.Add("data", phenoPopulationFre.ToString());
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult AddRowPartial(string id)
        {

            return PartialView("AddRowPartial", id);
        }

        public void fun(ref List<string> wholePheList, string geneGenoPairs, Dictionary<string, List<string>> geneGenoDic, int i)
        {
            foreach (var genoStr in geneGenoDic.ElementAt(i).Value)
            {

                List<string> functionList = genoStr.Split('/').ToList();
                functionList.Sort();
                String newGeneGenoPair = string.Join(":", geneGenoDic.ElementAt(i).Key, string.Join("/", functionList));
                if (geneGenoDic.Count() - 1 == i)
                {
                    List<string> sortedGeneGenoPairs = geneGenoPairs.Split(';').ToList();
                    sortedGeneGenoPairs.Add(newGeneGenoPair);
                    sortedGeneGenoPairs.Sort();
                    wholePheList.Add(string.Join(";", sortedGeneGenoPairs));
                }
                else if (geneGenoDic.Count() - 1 > i)
                {
                    fun(ref wholePheList, string.Join(";", geneGenoPairs, newGeneGenoPair), geneGenoDic, i + 1);
                }
            }
        }
    }
}
