using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PGx.KB.Models;
using PGx.Model.Entities;
using PGx.Model.Abstract;
using PGx.Model.Concrete;
using System.Data.Objects.SqlClient;
using System.Diagnostics;

namespace PGx.KB.Controllers
{
    public class AjaxHandlerController : Controller
    {
        //
        // GET: /AjaxHandler/
        
               private IPGxRepository repository;
               public AjaxHandlerController(IPGxRepository pGxRepository)
        {
            this.repository = pGxRepository;
        }



        public JsonResult DrugSelectTableAjaxHandler(JQueryDataTableParamModel param)
        {
            
            JQueryDataTableResultModel result = new JQueryDataTableResultModel();
            IQueryable<PGxGuideline> source = this.repository.PGxGuideline;
            IQueryable<PGxGuideline> filtered = null;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filtered = source.Where(x => x.Chemical.Contains(param.sSearch)||x.GenesInStr.Contains(param.sSearch));
            }
            else
            {
                filtered = source;
            }
            var displayed = filtered.OrderBy(x => x.Chemical).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            result.sEcho = param.sEcho;
            result.iTotalRecords = source.Count();
            result.iTotalDisplayRecords = filtered.Count();
            result.aaData = displayed.Select(x => new[]{
                x.ID.ToString(), 
                x.Chemical,
                x.GenesInStr,
                x.DosingGuidence.Count().ToString()
               
            });
             var json= Json(result, JsonRequestBehavior.AllowGet);
             return json;
        }
    

        public JsonResult AlleleSelectTableAjaxHandler(JQueryDataTableParamModel param)
        {
            JQueryDataTableResultModel result = new JQueryDataTableResultModel();

            var geneID = Request["geneID"];
            int parseId = 0;
            int.TryParse(geneID, out parseId);
            IQueryable<NamedAllele> source = this.repository.NamedAllele.Where(x => x.DefinitionFileID==parseId);
            IQueryable<NamedAllele> filtered = null;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filtered = source.Where(c=>c.Name.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filtered = source;
            }
            var displayed = filtered.OrderBy(x => x.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            result.sEcho = param.sEcho;
            result.iTotalRecords = source.Count();
            result.iTotalDisplayRecords = filtered.Count();
            result.aaData = displayed.Select(x => new[]{
                x.Name,
                x.M_Function,
                x.ID.ToString()
            });
            var json = Json(result.aaData).ToString();
            return Json(result, JsonRequestBehavior.AllowGet);
 
        }

        public JsonResult PhenotypeSelectTableAjaxHandler(JQueryDataTableParamModel param)
        {
            JQueryDataTableResultModel result = new JQueryDataTableResultModel();

            var geneID = Request["geneID"];
            int parseId = 0;
            int.TryParse(geneID, out parseId);
            IQueryable<NamedPhenotype> source = null;
            string gene = string.Empty;
            var phenotype = this.repository.NamedPhenotype.Where(x=>x.DefinitionFileID==parseId);
            if (phenotype != null && phenotype.Count() > 0)
            {
                gene = phenotype.FirstOrDefault().Symbol;
                source = phenotype;
                IQueryable<NamedPhenotype> filtered = null;
                //Check whether entities should be filtered by keyword
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filtered = source.Where(c => c.Phenotype.ToLower().Contains(param.sSearch.ToLower()));
                }
                else
                {
                    filtered = source;
                }
                var displayed = filtered.OrderBy(x => x.Phenotype).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                result.sEcho = param.sEcho;
                result.iTotalRecords = source.Count();
                result.iTotalDisplayRecords = filtered.Count();
                result.aaData = displayed.Select(x => new[]{
                x.Symbol,
                x.Phenotype,
                x.Description,
                gene+","+x.Phenotype,
                geneID
            });
                var json = Json(result.aaData).ToString();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                result.sEcho = param.sEcho;
                result.iTotalRecords = 0;
                result.iTotalDisplayRecords = 0;
                result.aaData = null;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult VariantSelectTableAjaxHandler(JQueryDataTableParamModel param)
        {
    
            JQueryDataTableResultModel result = new JQueryDataTableResultModel();

            var geneID = Request["geneID"];
            int parseId = 0;
            int.TryParse(geneID, out parseId);
            IQueryable<VariantLocus> source = this.repository.VariantLocus.Where(x => x.DefinitionFileID == parseId);
            IQueryable<VariantLocus> filtered = null;
            //Check whether entities should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filtered = source.Where(c => SqlFunctions.StringConvert((double)c.Position).ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filtered = source;
            }
            var displayed = filtered.OrderBy(x => x.Position).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            result.sEcho = param.sEcho;
            result.iTotalRecords = source.Count();
            result.iTotalDisplayRecords = filtered.Count();
            result.aaData = displayed.Select(x => new[]{
                x.ChromosomeHgvsName,
                x.Rsid,
                x.ID.ToString(),
                x.DefinitionFileID.ToString()
            });
            var json = Json(result.aaData).ToString();
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public JsonResult RecommendationSelectTableAjaxHandler(JQueryDataTableParamModel param)
        {
            JQueryDataTableResultModel result = new JQueryDataTableResultModel();

            var guidelineID = Request["GuidelineID"];
            int parseId = 0;
            int.TryParse(guidelineID, out parseId);
            IQueryable<DosingGuidence> source = this.repository.DosingGuidence.Where(x => x.PGxGuidelineID == parseId);
            IQueryable<DosingGuidence> filtered = null;
            //Check whether entities should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filtered = source.Where(c => c.Name.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filtered = source;
            }
            var displayed = filtered.OrderBy(x => x.Name).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            result.sEcho = param.sEcho;
            result.iTotalRecords = source.Count();
            result.iTotalDisplayRecords = filtered.Count();

            result.aaData = displayed.Select(x => new[]{
                x.ID.ToString(),
                x.Name,
                x.RxChange,
                x.Strength,
                string.IsNullOrEmpty(x.Implication)? null:string.Join(" ",x.Implication.Split(' ').Take(5))+"...",
                string.IsNullOrEmpty(x.Recommendation)? null: string.Join(" ",x.Recommendation.Split(' ').Take(5))+"..."

            });
            var json = Json(result.aaData).ToString();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

       

        
            public JsonResult GeneListSelectTableAjaxHandler(JQueryDataTableParamModel param)
        {
            JQueryDataTableResultModel result = new JQueryDataTableResultModel();
            IQueryable<DefinitionFile> source = this.repository.DefinitionFile;
            IQueryable<DefinitionFile> filtered = null;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filtered = source.Where(c => c.GeneSymbol.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filtered = source;
            }
            var displayed = filtered.OrderBy(x => x.GeneSymbol).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            result.sEcho = param.sEcho;
            result.iTotalRecords = source.Count();
            result.iTotalDisplayRecords = filtered.Count();
            result.aaData = displayed.Select(x => new[]{
                x.GeneSymbol,
                x.ID.ToString()
            });
            var json = Json(result.aaData).ToString();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReportSelectTableAjaxHandler(JQueryDataTableParamModel param)
        {
            var context = new PGx_KBEntities();
            var fileId =int.Parse( Request["FileID"]);

            List<ReportViewModel> reportModelList = new List<ReportViewModel>();
            var geneCallList = context.GeneCall.Where(x => x.FileId == fileId).ToList();
            var guidelineList = context.PGxGuideline.ToList();
            List<DosingGuidence> dosingList = new List<DosingGuidence>();
            foreach (var guideline in guidelineList)
            {
                if (guideline.Chemical.ToLower() == "warfarin")
                    continue;
                List<GeneCall> callList = new List<GeneCall>();
                List<string> phenotypeList = new List<string>();
                string genotype = string.Empty;
                foreach (var gene in guideline.GenesInStr.Split(','))
                {
                    GeneCall call = geneCallList.Where(x => x.Gene == gene).FirstOrDefault();
                    if (call == null || call.Diplotype == null)
                    {
                        genotype=genotype+gene+":"+"N/A"+",";
                        continue;
                    }
                    genotype = genotype + gene + ":" + call.Diplotype+",";
                    callList.Add(call);
                    var phenotype = call.Phenotype;
                    var definition = context.DefinitionFile.Where(x => x.GeneSymbol == gene).FirstOrDefault();
                    phenotypeList.Add(gene + ":" +phenotype );
                }
                if (phenotypeList.Count() == 0)
                    continue;
                genotype = genotype.Remove(genotype.LastIndexOf(","));
                phenotypeList.Sort();
                var phenotypeCombination = string.Join(";", phenotypeList);
                var dosingGuidence = guideline.DosingGuidence.Where(x => x.Phenotype.ToLower()==phenotypeCombination.ToLower()).FirstOrDefault();
                if (dosingGuidence == null)
                    continue;
                reportModelList.Add(new ReportViewModel { DosingGuidance = dosingGuidence, CallList = callList,Genotype=genotype,Phenotype=phenotypeCombination });
            }

            JQueryDataTableResultModel result = new JQueryDataTableResultModel();
            //var geneSymbol = Request["geneSymbol"];
            List<ReportViewModel> source = reportModelList;
            List<ReportViewModel> filtered = null;
            //Check whether entities should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filtered = source.Where(x => x.DosingGuidance.PGxGuideline.Chemical.Contains(param.sSearch)).ToList();
            }
            else
            {
                filtered = source;
            }
            var displayed = filtered.OrderBy(x => x.DosingGuidance.PGxGuideline.Chemical).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            result.sEcho = param.sEcho;
            result.iTotalRecords = source.Count();
            result.iTotalDisplayRecords = filtered.Count();
            result.aaData = displayed.Select(x => new[]{
                x.DosingGuidance.PGxGuidelineID.ToString(),
                x.DosingGuidance.PGxGuideline.Chemical,
                x.DosingGuidance.PGxGuideline.GenesInStr,
                x.Genotype,
                x.DosingGuidance.RxChange
            });
            var json = Json(result.aaData).ToString();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
