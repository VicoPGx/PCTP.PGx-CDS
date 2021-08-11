using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PGx.Model.Entities;
using PGx.KB.Models;
using System.Text;
using PGx.Model.Abstract;
using System.Web.Script.Serialization;
namespace PGx.KB.Controllers
{
    public class AdminController : BaseController
    {
        //
        // GET: /Admin/
        private IPGxRepository repository;
        public AdminController(IPGxRepository pGxRepository)
        {
            this.repository = pGxRepository;
        }
        public ViewResult Index()
        {
            return View(repository.Chemicals);
        }
        public ViewResult Edit(int? Id)
        {
            Chemical chemical = repository.Chemicals.FirstOrDefault(c => c.Id == Id);
            return View(chemical);
        }
        public ViewResult dosingDetails(int? Id)
        {
            ViewBag.Repo = repository;
            //ViewBag.geneIdList = context.GeneChemical.Where(x => x.ChemicalId == Id).ToList();
            var recommendations = repository.RecommendationEntitySet.Where(x => x.ChemicalId == Id);
            Dictionary<string,IEnumerable<Haplotype>> haplotypeDic=new Dictionary<string,IEnumerable<Haplotype>>();
               var genes=repository.Chemicals.Where(x=>x.Id==Id).FirstOrDefault().Genes.ToList();
               foreach (var g in genes)
               {
                   haplotypeDic.Add(g.GeneSymbol, repository.Haplotypes.Where(x => x.Gene.Id == g.Id));
               }
            KnowledgeOverview detailsModel = new KnowledgeOverview
            {
                Chemical = repository.Chemicals.Where(x => x.Id == Id).FirstOrDefault(),
                Recomendations = repository.RecommendationEntitySet.Where(x => x.ChemicalId == Id),
                Haplotype = repository.Haplotypes,
                HaplotypeDic=haplotypeDic
            };

            return View(detailsModel);
        }
        public ActionResult DosingRecomendation(int? Id)
        {
            ViewBag.Repo = repository;
            //ViewBag.geneIdList = context.GeneChemical.Where(x => x.ChemicalId == Id).ToList();

            KnowledgeOverview dosingModel = new KnowledgeOverview
            {
                Chemical = repository.Chemicals.Where(x => x.Id == Id).FirstOrDefault(),
                //GeneChemical = repository.GeneChemical.Where(x => x.ChemicalId == Id),
                Recomendations = repository.RecommendationEntitySet.Where(x => x.ChemicalId == Id),
                Haplotype = repository.Haplotypes
            };

            return View(dosingModel);
        }

        public PartialViewResult DiplotypeSpecifiedRecomendation(string  diplotypeStr)
        {
            ViewBag.Repo = repository;
            //JObject jsonObject = JObject.Parse(diplotypeStr);
            //GeneAllelObject listGene = js.Deserialize<GeneAllelObject>(jsonObject["arr"].ToString());
            if (string.IsNullOrEmpty(diplotypeStr))
                return null;
            JavaScriptSerializer js = new JavaScriptSerializer();
            GeneAllelObject listGene = js.Deserialize<GeneAllelObject>(diplotypeStr);
            //Diplotype diplotype;
            //Haplotype haplotypeA, haplotypeB;
            //List<Recomendation> recomendationList=new List<Recomendation>();
            List<RecommendationEntity > recommendationList= new List<RecommendationEntity>();
            Phenotype phenotype;
            List<DiplotypePhenotype> diplotypePhnotypelist = new List<DiplotypePhenotype>();
            foreach (var geneAllel in listGene.GeneAllelList)
            {
                var geneItem = repository.Genes.Where(x => x.GeneSymbol == geneAllel.GeneSymbol).FirstOrDefault();
              
                var diplotypeDric = repository.DiplotypePhenotypeSet.Where(x => x.GeneSymbol == geneAllel.GeneSymbol && (x.AlleleA == geneAllel.AlleleA && x.AlleleB == geneAllel.AlleleB || x.AlleleA == geneAllel.AlleleB && x.AlleleB == geneAllel.AlleleA)).FirstOrDefault();
                diplotypePhnotypelist.Add(diplotypeDric);
                var recommendation=repository.RecommendationEntitySet.Where(x=>x.ChemicalId==listGene.ChemicalId&&x.PhenotypeName==diplotypeDric.PhenotypeName).FirstOrDefault();
                recommendationList.Add(recommendation);
                //phenotype = diplotype.Phenotype;
            }

            if (diplotypePhnotypelist.Count() > 1)
            {
                string phe=string.Empty;
                for(int i=0;i<diplotypePhnotypelist.Count();i++)
                {
                   string str1=diplotypePhnotypelist.ElementAt(i).PhenotypeName.Trim();
                    string str2=diplotypePhnotypelist.ElementAt(i).GeneSymbol.Trim();
                        var xman=repository.PhenotypeEntitySet.Where(x=>x.PhenotypeSymbol==str1&&x.GeneSymbol==str2).FirstOrDefault().Id.ToString();
                        phe = phe + xman;
                    if (i!=diplotypePhnotypelist.Count-1)
                    {
                        phe=phe+',';
                    }
                }
                string phe1 = string.Empty;
                for (int i = diplotypePhnotypelist.Count() - 1; i >= 0; i--)
                {
                    string str1 = diplotypePhnotypelist.ElementAt(i).PhenotypeName.Trim();
                    string str2 = diplotypePhnotypelist.ElementAt(i).GeneSymbol.Trim();
                    phe1 = phe1 + repository.PhenotypeEntitySet.Where(x => x.PhenotypeSymbol == str1 && x.GeneSymbol == str2).FirstOrDefault().Id.ToString();
                    if (i != 0)
                    {
                        phe1 = phe1 + ',';
                    }
                }
                var recommendation = repository.RecommendationEntitySet.Where(x => x.ChemicalId == listGene.ChemicalId && (x.Phenotype == phe||x.Phenotype==phe1)).FirstOrDefault();
                recommendationList.Add(recommendation);
            }

            //foreach (var geneAllel in listGene.GeneAllelList)
            //{
            //    var geneItem=repository.Genes.Where(x => x.GeneSymbol == geneAllel.GeneSymbol).FirstOrDefault();
            //    haplotypeA = repository.Haplotypes.Where(x => x.Gene.Id == geneItem.Id && x.HaplotypeSymbol == geneAllel.AlleleA).FirstOrDefault();
            //    haplotypeB = repository.Haplotypes.Where(x => x.Gene.Id == geneItem.Id && x.HaplotypeSymbol == geneAllel.AlleleB).FirstOrDefault();
               
            //    if (haplotypeA == haplotypeB)
            //    {
            //        diplotype = geneItem.Diplotypes.Where(x => (x.Haplotype1==haplotypeA && x.Haplotype2== haplotypeB)).FirstOrDefault();
            //    }
            //    else
            //    {
            //         diplotype = geneItem.Diplotypes.Where(x => (x.Haplotype1==haplotypeA||x.Haplotype2==haplotypeA) && (x.Haplotype2==haplotypeB||x.Haplotype2==haplotypeB)).FirstOrDefault();
            //    }

            //    phenotype = diplotype.Phenotype;

            //    var recomendation = repository.Recomendations.Where(x =>x.Chemical.Id==geneAllel.ChemicalId&& x.Phenotype.Id==phenotype.Id).FirstOrDefault();
            //    recomendationList.Add(recomendation);
            //}
                return PartialView(recommendationList);
        }
    }
}
