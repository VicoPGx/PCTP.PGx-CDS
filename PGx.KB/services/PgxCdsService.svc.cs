using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using PGx.Model.Entities;
using PGx.KB.Models;
using System.Web.Script.Serialization;

namespace PGx.KB.services
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“PgxCdsService”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 PgxCdsService.svc 或 PgxCdsService.svc.cs，然后开始调试。
    public class PgxCdsService : IPgxCdsService
    {
        PGx_KBEntities context = new PGx_KBEntities();

        public static double totalCoef = 5.6044;
        public static double ageCoef = -0.2614;
        public static double heightCoef = 0.0087;
        public static double weightCoef = 0.0128;
        public static Dictionary<string, double> raceCoefDic = new Dictionary<string, double>()
        {
            {"Caucasian or White",0},
            {"Asian",-0.1092},
            {"Black or African American",-0.2760},           
            {"Unknown or Mixed",-0.1032}            
        };
        public static Dictionary<string, Double> cyp2c9CoefDic = new Dictionary<string, Double>() 
            { 
                { "*1/*1", 0 }, 
                { "*1/*2", -0.5211 }, 
                { "*1/*3", -0.9357 }, 
                { "*2/*3", -1.9206 }, 
                { "*2/*2", -1.0616 },
                { "*3/*3", -2.3312 },
                {"Other",0},
                {"Unknown",-0.2188}
            };

        public static Dictionary<string, Double> vkorc1CoefDic = new Dictionary<string, double>()
            {
                {"Reference/Reference",0},
                {"Reference/-1639G>A",-0.8677},
                {"-1639G>A/-1639G>A",-1.6974},
                {"Unknown",-0.4854},
                {"Other",0}
            };
        public static Dictionary<string, double> enzymeInducerStatueCoefDic = new Dictionary<string, double>()
        {
            {"Y",1.1816},
            {"N",0},
            {"Unknown",0}
        };
        public static Dictionary<string, double> amiodaroneStatusCoefDic = new Dictionary<string, double>()
        {
            {"Y",-0.5503},
            {"N",0},
            {"Unknown",0}
        };

        public PreTestAlertServiceModel PreTestAlertService(string patientId, string drugName)
        {
            var patient = context.RAW_DATA_FILE.Where(x => x.PatientID == patientId).FirstOrDefault();

            PGxGuideline guideline = new PGxGuideline();
            
            PreTestAlertServiceModel preTestAlertModel = new PreTestAlertServiceModel() { DrugId = 0,IsPGxDrug=true };
            guideline = context.PGxGuideline.Where(x=>x.Chemical==drugName.ToLower()).FirstOrDefault();
            //check if the drug affected by PGx
            if (guideline == null)
            {
                preTestAlertModel.IsPGxDrug = false;
                return preTestAlertModel;
            }
    
            if (guideline.Chemical == "warfarin")
            {
                var cyp2c9Call = patient.GeneCall.Where(x => x.Gene == "CYP2C9").FirstOrDefault();
                var vkorc1Call = patient.GeneCall.Where(x => x.Gene == "VKORC1").FirstOrDefault();
               
                if (cyp2c9Call == null || vkorc1Call == null)
                {
                    preTestAlertModel.DrugId = guideline.ID;
                }
                List<string> geneNullList = new List<string>();
                foreach (var geneDef in guideline.DefinitionFile.ToList())
                {
                    var geneCall = patient.GeneCall.Where(x => x.Gene == geneDef.GeneSymbol).FirstOrDefault();
                    if (geneCall == null)
                    {
                        geneNullList.Add(geneDef.GeneSymbol);
                    }
                }
                geneNullList.Sort();
                preTestAlertModel.GeneNull = string.Join(", ", geneNullList);
            }
            else
            {
                //search for related PGx results 
                List<string> phenotypeList = new List<string>();
                bool callFlag = false;
                List<string> geneNullList = new List<string>();
                foreach (var geneDef in guideline.DefinitionFile.ToList())
                {
                    var geneCall = patient.GeneCall.Where(x => x.Gene == geneDef.GeneSymbol).FirstOrDefault();
                    if (geneCall == null)
                    {
                        callFlag = true;
                        geneNullList.Add(geneDef.GeneSymbol);
                    }
                    else
                    {
                        phenotypeList.Add(geneDef.GeneSymbol + ":" + geneCall.Phenotype);
                    }
                }
                geneNullList.Sort();
                preTestAlertModel.GeneNull = string.Join(", ", geneNullList);
                //check if the related gene testing results existed
                if (callFlag == true)
                {
                    phenotypeList.Sort();
                    var phenotype = string.Join(";", phenotypeList);
                    var dosingGuidence = guideline.DosingGuidence.Where(x => x.Phenotype.ToLower().Trim() == phenotype.ToLower()).FirstOrDefault();
                    if (dosingGuidence == null || dosingGuidence.RxChange != "Yes")
                    {
                        preTestAlertModel.DrugId = guideline.ID;
                    }
                }
            }
            //Prepare pre-test alert
            if (preTestAlertModel.DrugId > 0)
            {
                preTestAlertModel.Summary = guideline.Summary;
                preTestAlertModel.Gene = guideline.GenesInStr;
                preTestAlertModel.Chemical = guideline.Chemical;
                preTestAlertModel.Implication = guideline.ClinicalImplication;
                preTestAlertModel.Guidelines = guideline.Literature.ToList();
                preTestAlertModel.GenePopPhenoFre = new List<SER_GenePopPhenoFre>();
                
                foreach (var geneDefinition in guideline.DefinitionFile)
                {
                     
                    SER_GenePopPhenoFre genePopPhenoFreObj = new SER_GenePopPhenoFre();
                    if (string.IsNullOrEmpty(geneDefinition.Populations) == false)
                    {
                        genePopPhenoFreObj.Populations = geneDefinition.Populations.Split(',').ToList();
                    }
                    genePopPhenoFreObj.Symbol = geneDefinition.GeneSymbol;
                    genePopPhenoFreObj.PopulationPhenotypeFrequency = new List<SER_PopulationPhneotypeFrequency>();
                    var namedPhenotypeList = geneDefinition.PhenotypeMap.ToList();
                    foreach (var population in genePopPhenoFreObj.Populations)
                    {
                        SER_PopulationPhneotypeFrequency population_PhenotypeFrequencyList = new SER_PopulationPhneotypeFrequency();
                        population_PhenotypeFrequencyList.PhenotypeFrequencyList = new List<SER_PhenoFrequency>();
                        foreach (var namedPhenotype in namedPhenotypeList)
                        {
                            string genePheno = namedPhenotype.Symbol + ":" + namedPhenotype.Phenotype.Trim();
                            DosingGuidence phenotype_DosingGuidance = guideline.DosingGuidence.FirstOrDefault(x=>x.Phenotype==genePheno);
                            if (namedPhenotype.DefinitionFile.GeneSymbol == "HLA-B"&&phenotype_DosingGuidance==null)
                            {
                                continue;                               
                            }
                            SER_PhenoFrequency phenotypeFrequency = new SER_PhenoFrequency();
                            var phenotype_PopulationFrequencyObj = namedPhenotype.PhenotypePopFreq.Where(x => x.Population.Trim() == population).FirstOrDefault();
                            if (phenotype_PopulationFrequencyObj == null || phenotype_PopulationFrequencyObj.Frequency == null || phenotype_PopulationFrequencyObj.Frequency < 0.00001M)
                                continue;
                            else
                            {
                                phenotypeFrequency.Frequency = Math.Round((decimal)phenotype_PopulationFrequencyObj.Frequency * 100, 2).ToString();
                            }
                           
                            if (phenotype_DosingGuidance != null)
                            {
                                phenotypeFrequency.Risk = phenotype_DosingGuidance.RxChange;
                                phenotypeFrequency.EvidenceLevel = phenotype_DosingGuidance.Strength;
                                phenotypeFrequency.ImpactLevel = phenotype_DosingGuidance.ImpactLevel;
                            }
                            else
                            {
                                phenotypeFrequency.Risk = null;
                                phenotypeFrequency.EvidenceLevel = null;
                                phenotypeFrequency.ImpactLevel = null;
                            }
                            phenotypeFrequency.Phenotype = namedPhenotype.Phenotype;
                            population_PhenotypeFrequencyList.PhenotypeFrequencyList.Add(phenotypeFrequency);
                        }
                        if (population_PhenotypeFrequencyList.PhenotypeFrequencyList.Count() == 0)
                            continue;
                        population_PhenotypeFrequencyList.Population = population;
                        genePopPhenoFreObj.PopulationPhenotypeFrequency.Add(population_PhenotypeFrequencyList);
                    }
                    genePopPhenoFreObj.PopulationsGeneSpecific = string.Join(",", genePopPhenoFreObj.PopulationPhenotypeFrequency.Select(x => x.Population).ToList());
                    if (genePopPhenoFreObj.PopulationPhenotypeFrequency.Count() == 0)
                        continue;
                    preTestAlertModel.GenePopPhenoFre.Add(genePopPhenoFreObj);
                }
                var js = new JavaScriptSerializer();
                preTestAlertModel.FreJson = js.Serialize(preTestAlertModel.GenePopPhenoFre);
                preTestAlertModel.RiskPhenotypes = new List<SER_RiskPhenotype>();
                var riskPhenotypeList = new List<SER_RiskPhenotype>();
                var riskDosingGuidanceList = guideline.DosingGuidence.Where(x => x.RxChange == "Yes" && x.RecommendationPhenotype.Count() == 1).Select(x => x.RecommendationPhenotype.FirstOrDefault()).ToList();
                foreach (var riskGuidance in riskDosingGuidanceList)
                {
                    var riskPhenotypeObj = new SER_RiskPhenotype();
                    riskPhenotypeObj.Symbol = riskGuidance.Gene;
                    riskPhenotypeObj.Phenotype = riskGuidance.Phenotype.Trim();
                    riskPhenotypeList.Add(riskPhenotypeObj);
                }
                foreach (var riskGroup in riskPhenotypeList.GroupBy(x => x.Symbol).OrderBy(x => x.Key))
                {
                    preTestAlertModel.RiskPhenotypes.AddRange(riskGroup.OrderBy(x => x.Phenotype).ToList());
                }
            }
            return preTestAlertModel;
        }
        public PostTestAlertServiceModel PostTestAlertService(string patientId, string drugName)
        {
            var patient = context.RAW_DATA_FILE.Where(x => x.PatientID == patientId).FirstOrDefault();
            PGxGuideline guideline = new PGxGuideline();
            PostTestAlertServiceModel postTestAlertModel = new PostTestAlertServiceModel() { guidelineId = 0};
            guideline = context.PGxGuideline.Where(x => x.Chemical == drugName).FirstOrDefault();
            //Search for related phenotype-based recommendation
            List<string> phenotypeList = new List<string>();
            List<SER_DiplotypeResult> DiplotypeResultList = new List<SER_DiplotypeResult>();
            List<string> geneNullList = new List<string>();
            foreach (var geneDef in guideline.DefinitionFile.ToList())
            {
                var geneCall = patient.GeneCall.Where(x => x.Gene == geneDef.GeneSymbol).FirstOrDefault();
                if (geneCall == null)
                {
                    geneNullList.Add(geneDef.GeneSymbol);
                    continue;
                }
                DiplotypeResultList.Add(new SER_DiplotypeResult { Diplotype = geneCall.Diplotype, GeneSymbol = geneDef.GeneSymbol, Phenotype = geneCall.Phenotype });
                phenotypeList.Add(geneDef.GeneSymbol + ":" +geneCall.Phenotype );
            }
            geneNullList.Sort();
            postTestAlertModel.GeneNull = string.Join(", ", geneNullList);

            DosingGuidence dosingGuidence;
            if (guideline.Chemical.ToLower() == "warfarin")
            {
                dosingGuidence = guideline.DosingGuidence.FirstOrDefault();
            }
            else
            {
                phenotypeList.Sort();
                var phenotype = string.Join(";", phenotypeList);
                dosingGuidence=guideline.DosingGuidence.Where(x => x.Phenotype.ToLower().Trim() == phenotype.ToLower()).FirstOrDefault();
            }
            //check if the related recommendation null, if it need to adjust dosage or use an alternative drug
            if (dosingGuidence != null && dosingGuidence.RxChange == "Yes")
            {
                //prepare post-test alert
                postTestAlertModel.guidelineId = guideline.ID;
                postTestAlertModel.Chemical = guideline.Chemical;
                postTestAlertModel.GeneSymbol = string.Join(",", guideline.DefinitionFile.Select(x => x.GeneSymbol).ToList());
                postTestAlertModel.PatientName = patient.PatientName;
                postTestAlertModel.PatientId = patient.PatientID;
                postTestAlertModel.DiplotypeResults = DiplotypeResultList;
                postTestAlertModel.GuidanceId = dosingGuidence.ID;
                postTestAlertModel.Guidelines = dosingGuidence.PGxGuideline.Literature.ToList();
                postTestAlertModel.DosingGuidance = new SER_DosingGuidance()
                {
                    Implication = dosingGuidence.Implication,
                    Frequency = dosingGuidence.Frequency,
                    Recommendation = dosingGuidence.Recommendation,
                    EvidenceLevel = dosingGuidence.Strength,
                    ClinicalImpact = dosingGuidence.ImpactLevel
                };
            }
            return postTestAlertModel;
        }

        public string WarfarinDoseCalculator(WarfarinDoseModel model)
        {
            //int/int  向下取整
            var gaeInDecade = (int)Math.Floor((double)model.Age / 10);
            var intage1 = model.Age / 10;
            var weeklyDoseSquareRoot = totalCoef
                + ageCoef * gaeInDecade
                + heightCoef * model.Height
                + weightCoef * model.Weight
                + raceCoefDic[model.Race]
                + enzymeInducerStatueCoefDic[model.EnzymeInducer]
                + amiodaroneStatusCoefDic[model.Amiodarone]
                + cyp2c9CoefDic[model.CYP2C9]
                + vkorc1CoefDic[model.VKORC1];
            int weeklyDose = (int)Math.Round(weeklyDoseSquareRoot * weeklyDoseSquareRoot, 0);
            int dailyDose = (int)Math.Round(weeklyDose / 7.0);
            string result = "WeeklyDose:" + weeklyDose.ToString() + "," + "DailyDose:" + dailyDose.ToString();
            return result;
        }
    }
}
