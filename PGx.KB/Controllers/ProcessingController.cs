using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PGx.KB.Vcf;
using PGx.Model.Entities;
using System.IO;
using PGx.KB.Models;
using PGx.KB.Vcf.Haplotype.Model;

namespace PGx.KB.Controllers
{
    public class ProcessingController : BaseController
    {
        PGx_KBEntities context = new PGx_KBEntities();
        //
        // GET: /Processing/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StartingAnalyze(int id)
        {
            //获取VCF文件路径
            var file = context.RAW_DATA_FILE.Where(x => x.ID == id).FirstOrDefault();
            var forderPath = Server.MapPath("~/Upload/RawDataFile/");
            var vcfFile = Path.GetFileName(file.FILE_PATH);
            vcfFile = Path.Combine(forderPath, vcfFile);

            //read definition and initialize NamedAlleleMatcher
            PGxParse pharmParse = new PGxParse(context);
            //read and parse Vcf File,compare Vcf positions with NamedAllele definions
            Result callResult = pharmParse.CallFromVcf(vcfFile);
            callResult = pharmParse.PhenotypeCall(callResult);
            pharmParse.SaveGeneCalls(id, callResult.GeneCalls);
            return RedirectToAction("DiplotypeCalls", new { id = id });
        }


        public ActionResult ReportView(int id)//current
        {
            ViewBag.fileId = id;
            return PartialView();
        }

        public ActionResult RecommendationReportView(int guidelineId, int fileId)//current
        {
            var guideline = context.PGxGuideline.Where(x => x.ID == guidelineId).FirstOrDefault();
            var geneCallList = context.GeneCall.Where(x => x.FileId == fileId).ToList();
            List<GeneCall> callList = new List<GeneCall>();
            List<string> phenoList=new List<string>();
            foreach (var gene in guideline.GenesInStr.Split(','))
            {
                GeneCall call = geneCallList.Where(x => x.Gene == gene).FirstOrDefault();
                if (call != null && string.IsNullOrEmpty(call.Diplotype) == false)
                {
                    callList.Add(call);
                    phenoList.Add(call.Gene + ":" + call.Phenotype);
                }
            }
            phenoList.Sort();
            string genePhenotype=string.Join(";",phenoList);
            var dosingGuidence = guideline.DosingGuidence.Where(x => x.Phenotype.ToLower().Trim() == genePhenotype.ToLower()).FirstOrDefault();
            var reportModelView = new ReportViewModel { DosingGuidance = dosingGuidence, CallList = callList };
            return PartialView(reportModelView);
        }

        public ActionResult GenomicRM()
        {
            return View("VCFProcessing",context.RAW_DATA_FILE.ToList());
        }
        public PartialViewResult UploadFile(int? id = 0)
        {
            if (id == 0)
            { return PartialView(new RAW_DATA_FILE()); }
            else
            {
                var file = context.RAW_DATA_FILE.Where(x => x.ID == id).FirstOrDefault();
                return PartialView("UploadFile",file);
            }
        }

        public string DeleteSample(int id)
        {
            var file = context.RAW_DATA_FILE.Where(x => x.ID == id).FirstOrDefault();
            //var filePath = Server.MapPath(file.FILE_PATH);
            //System.IO.File.Delete(filePath);
            var geneCall = context.GeneCall.Where(x => x.FileId == id).ToList();
            foreach (var call in geneCall)
            {
                var diplotype = call.DiplotypeMatch.ToList();
                var haplotype = call.HaplotypeMatch.ToList();
                var variants = call.Variants.ToList();
                var variantOfInterst = call.VarOfInterest.ToList();
                foreach (var voi in variantOfInterst)
                {
                    context.VarOfInterest.Remove(voi);
                    context.SaveChanges();
                }

                foreach (var variant in variants)
                { 
                    context.Variant.Remove(variant);
                context.SaveChanges();
                }
                foreach (var haplo in haplotype)
                {
                    context.HaplotypeMatch.Remove(haplo);
                    context.SaveChanges();
                }
                foreach (var dip in diplotype)
                {
                    context.DiplotypeMatch.Remove(dip);
                    context.SaveChanges();
                }
                
                context.GeneCall.Remove(call);
                context.SaveChanges();
            }
            context.RAW_DATA_FILE.Remove(file);
            context.SaveChanges();                 
            return "File has been deleted!";
        }
        public ActionResult DiplotypeCalls(int id)
        {

            var patientFile=context.RAW_DATA_FILE.Where(x=>x.ID==id).FirstOrDefault();
 
           return PartialView(patientFile);
        }
        [HttpPost]
        public string UpdateVCF(RAW_DATA_FILE rawDataFile)//Replace file
        {
            var existFile = context.RAW_DATA_FILE.Where(x => x.ID == rawDataFile.ID).FirstOrDefault();           
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                //Create folder path
                var folderPath = Server.MapPath("~/Upload/RawDataFile/");
                //creat forder if it dosen't exist
                Directory.CreateDirectory(folderPath);
                //creat file name
                var fileName = Guid.NewGuid() + Path.GetFileName("-"+file.FileName);
                //create file path=folder path + file name
                var newFilePathAbs = Path.Combine(folderPath, fileName);
                //Delete exist file
                if (string.IsNullOrEmpty(existFile.FILE_PATH) == false)
                {                 
                    var oldfFllPath = Server.MapPath(existFile.FILE_PATH);
                    System.IO.File.Delete(oldfFllPath);
                }
                //Save new file
                file.SaveAs(newFilePathAbs);
                //save relative path in database
                existFile.FILE_PATH = "/Upload/RawDataFile/" + fileName;
                context.SaveChanges();

            }
            return "sucess!";
        }
        public PartialViewResult AddSampleDiplotype(int ? id, int callId = 0)
        {
           // var sample = context.RAW_DATA_FILE.Where(x => x.ID == id).FirstOrDefault();
            if (callId == 0)
            {
                var geneCall = new GeneCall() { FileId = id };
                return PartialView("AddSampleDiplotype", geneCall);
            }
            else
            {
                var callModel=context.GeneCall.Where(x=>x.ID==callId).FirstOrDefault();
                return PartialView("AddSampleDiplotype",callModel );
            }
        }
        [HttpPost]
        public string AddSampleDiplotype(GeneCall call)
        {
            string result = "Save succeed!";
            try
            {
                var calls = context.GeneCall.Where(x => x.FileId == call.FileId).ToList();
                if (call.CallID == 0 && calls.Any(x => x.Gene == call.Gene))
                {
                    return string.Format("{0}'s diplotype has already existed!", call.Gene);
                }
                if (string.IsNullOrEmpty(call.Gene) || string.IsNullOrEmpty(call.HaplotypeA)||string.IsNullOrEmpty(call.HaplotypeB))
                {
                    return "Gene and Diplotype can't be null!";
                }

                var geneDef = context.DefinitionFile.Where(x => x.GeneSymbol == call.Gene).FirstOrDefault();
                   
                    if (geneDef == null)
                        return "Save failed! The Gene" + call.Gene + "is not defined in genomic knoledgebase!";
                    var namedAlleles = geneDef.NamedAllele.ToList();
                    
                    var namedPhenotypes = geneDef.PhenotypeMap.ToList();

                        List<string> alleleList=new List<string>(){call.HaplotypeA,call.HaplotypeB};
                        alleleList.Sort();
                        call.Diplotype = string.Join("/", alleleList);
                        List<string> funAB = new List<string>();

                        string funA = namedAlleles.Where(x => x.Name == call.HaplotypeA).FirstOrDefault().M_Function.ToLower();
                        string funB = namedAlleles.Where(x => x.Name == call.HaplotypeB).FirstOrDefault().M_Function.ToLower();
                        funAB.Add(funA);
                        funAB.Add(funB);
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
                                call.Phenotype = pheno.Phenotype;
                                break;
                            }
                        }               
                if (call.CallID == 0)
                {
                    var sampleFile = context.RAW_DATA_FILE.Where(x => x.ID == call.FileId).FirstOrDefault();
                    call.RAW_DATA_FILE = sampleFile;
                    call.IsVcfCall = false;
                    call.Chromosome = geneDef.Chromosome;
                    call.IsPhased = false;
                    call.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    
                    context.GeneCall.Add(call);
                    context.SaveChanges();
                }
                else 
                {
                    var existCall = context.GeneCall.Where(x => x.ID == call.CallID).FirstOrDefault();
                    if (existCall.Gene!=call.Gene||existCall.Diplotype != call.Diplotype)
                    {
                        var diplotypeMatchList = existCall.DiplotypeMatch.ToList();
                        var haplotypeMatchList = existCall.HaplotypeMatch.ToList();

                        foreach (var diplotypeMatch in diplotypeMatchList)
                        {
                            context.DiplotypeMatch.Remove(diplotypeMatch);
                            context.SaveChanges();
                        }

                        foreach (var haplotypeMatch in haplotypeMatchList)
                        {
                            context.HaplotypeMatch.Remove(haplotypeMatch);
                            context.SaveChanges();
                        }
                        
                        existCall.Gene = call.Gene;
                        existCall.IsVcfCall = false;
                        existCall.HaplotypeA = call.HaplotypeA;
                        existCall.HaplotypeB = call.HaplotypeB;
                        existCall.Diplotype = call.Diplotype;
                        existCall.Phenotype = call.Phenotype;
                        existCall.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                result = result + e.Message;
            }
            return result;
        }

        public string DeleteDiplotype(int id)
        {
            var geneCall=context.GeneCall.Where(x => x.ID == id).FirstOrDefault();
            var diplotypeMatch = geneCall.DiplotypeMatch.ToList();
            var variants = geneCall.Variants.ToList();
            var varOfInteres = geneCall.VarOfInterest.ToList();
            var haplotypes = geneCall.HaplotypeMatch.ToList();
            foreach (var diplotype in diplotypeMatch)
            {
                context.DiplotypeMatch.Remove(diplotype);
                context.SaveChanges();
            }
            foreach (var variant in variants)
            {
                context.Variant.Remove(variant);
                context.SaveChanges();
            }
            foreach (var varOfIn in varOfInteres)
            { 
                context.VarOfInterest.Remove(varOfIn);
            context.SaveChanges();
            }
            foreach (var hap in haplotypes)
            {
                context.HaplotypeMatch.Remove(hap);
                context.SaveChanges();
            }
            context.GeneCall.Remove(geneCall);
            context.SaveChanges();
            return "sucess!";
        }

        public ActionResult AlleleList(string gene)
        {
            var alleleList=context.DefinitionFile.FirstOrDefault(x=>x.GeneSymbol==gene).NamedAllele.Select(x=>x.Name).ToList();
            return PartialView(alleleList);
        }
    }
}
