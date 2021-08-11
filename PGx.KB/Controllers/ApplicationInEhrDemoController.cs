using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PGx.Model.Entities;
using PGx.KB.Models;
using System.Web.Script.Serialization;
using PGx.KB.services;
namespace PGx.KB.Controllers
{
    public class ApplicationInEhrDemoController : BaseController
    {
        PgxCdsService ser = new PgxCdsService();

        PGx_KBEntities context = new PGx_KBEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EhrPatientEdit(int id = 0)
        {
            Patient patient = context.Patient.Where(x => x.ID == id).FirstOrDefault();
            if (patient == null)
            {
                patient = new Patient();
            }
            return PartialView("EhrPatientEdit", patient);
        }

        [HttpPost]
        public string EhrPatientEdit(Patient patient)
        {
            if (patient.ID != 0)
            {
                var oldPatient = context.Patient.Where(x => x.ID == patient.ID).FirstOrDefault();
                oldPatient.Name = patient.Name;
                oldPatient.Sex = patient.Sex;
                oldPatient.Age = patient.Age;
                oldPatient.BirthDay = patient.BirthDay;
                oldPatient.Height = patient.Height;
                oldPatient.Weight = patient.Weight;
                oldPatient.PatientID = patient.PatientID;
                oldPatient.TimeStamp = DateTime.Now;
                context.SaveChanges();
                return "Sucess!";
            }

            if (string.IsNullOrEmpty(patient.PatientID))
            {
                var count = context.Patient.Count() + 1;
                var med = new Random().Next(101, 1000);
                var year = DateTime.Now.Year.ToString();
                string head = year.Substring(2, 2);

                if (count < 10)
                {
                    med = med * 100;
                }
                else if (10 < count && count < 100)
                {
                    med = med * 10;
                }

                patient.PatientID = "Pa" + head + med.ToString() + count.ToString();

            }

            Patient newPatient = new Patient();
            newPatient.Name = patient.Name;
            newPatient.Sex = patient.Sex;
            newPatient.Age = patient.Age;
            newPatient.BirthDay = patient.BirthDay;
            newPatient.Height = patient.Height;
            newPatient.Weight = patient.Weight;
            newPatient.PatientID = patient.PatientID;
            newPatient.TimeStamp = DateTime.Now;
            context.Patient.Add(newPatient);
            context.SaveChanges();

            return "Sucess!";
        }

        public ActionResult CpoeDemo()
        {
            return View(context.Patient.ToList());
        }



        public ActionResult ApplicationDemo()
        {
            var model = context.RAW_DATA_FILE.ToList();
            return View(model);
        }

        public string DefaultExample()
        {
            var example = context.RAW_DATA_FILE.Where(x => x.GeneCall.Count() > 0).FirstOrDefault();
            if (example == null)
            {
                return string.Empty;
            }
            else
                return example.PatientID;

        }

        public ActionResult ApplicationDemoPartial(string patientId)
        {
            var patient = context.RAW_DATA_FILE.Where(x => x.PatientID == patientId).FirstOrDefault();
            return PartialView(patient);
        }

        public JsonResult DrugPrescribeDatatable(JQueryDataTableParamModel param)
        {
            JQueryDataTableResultModel result = new JQueryDataTableResultModel();
            var patientId = Request["patientId"];

            IQueryable<PGxGuideline> source = context.PGxGuideline;
            IQueryable<PGxGuideline> filtered = null;
            if (string.IsNullOrEmpty(param.sSearch) == false)
            {
                filtered = source.Where(x => x.Chemical.Contains(param.sSearch));
            }
            else
            {
                filtered = source;
            }
            var displayedGuideline = filtered.OrderBy(x => x.Chemical).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            result.sEcho = param.sEcho;
            result.iTotalRecords = source.Count();
            result.iTotalDisplayRecords = filtered.Count();
            List<DrugPrescribeDemoModel> demoModelList = new List<DrugPrescribeDemoModel>();
            foreach (var guideline in displayedGuideline)
            {
                DrugPrescribeDemoModel demoModel = new DrugPrescribeDemoModel() { Chemical = guideline.Chemical, AlertType = "No alert" };

                string genotype = string.Empty;
                var relatedGene = guideline.GenesInStr.Split(',');
                List<string> phenoList = new List<string>();
                var flag = false;
                foreach (var gene in relatedGene)
                {
                    var call = context.RAW_DATA_FILE.Where(x => x.PatientID == patientId).FirstOrDefault().GeneCall.Where(x => x.Gene == gene).FirstOrDefault();
                    if (call != null)
                    {
                        phenoList.Add(call.Gene + ":" + call.Phenotype);
                        genotype = genotype + gene + ":" + call.Diplotype + ",";
                    }
                    else
                    {
                        flag = true;
                        genotype = genotype + gene + ":" + "N/A" + ",";
                    }
                }
                genotype = genotype.Remove(genotype.LastIndexOf(","));
                demoModel.Genotype = genotype;
                phenoList.Sort();
                string genePhenotype = string.Join(";", phenoList);
                var dosing = guideline.DosingGuidence.Where(x => x.Phenotype.ToLower().Trim() == genePhenotype.ToLower()).FirstOrDefault();
                if (flag == true)
                {
                    if (string.IsNullOrEmpty(genePhenotype) == true)
                    {
                        demoModel.AlertType = "Pre-test alert";
                    }
                    else if (dosing == null || dosing.RxChange != "Yes")
                    {
                        demoModel.AlertType = "Pre-test alert";
                    }
                    else if (dosing != null && dosing.RxChange == "Yes")
                    {
                        demoModel.AlertType = "Post-test alert";
                    }
                    else if (guideline.Chemical.ToLower() == "warfarin")
                    {
                        demoModel.AlertType = "Post-test alert";
                    }
                }
                else
                {
                    if (dosing != null && dosing.RxChange == "Yes")
                        demoModel.AlertType = "Post-test alert";
                    else if (guideline.Chemical.ToLower() == "warfarin")
                    {
                        demoModel.AlertType = "Post-test alert";
                    }
                }

                demoModelList.Add(demoModel);
            }

            result.aaData = demoModelList.Select(x => new[] {
            x.Chemical,
            x.Genotype,
            x.AlertType,
            x.Chemical
            
            });


            JsonResult jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            return jsonResult;

        }

        public ActionResult DemoSubmit(string drugName)
        {
            var patientId = Request["patientId"];
            PreTestAlertServiceModel preTestAlertModel = ser.PreTestAlertService(patientId, drugName);
            if (preTestAlertModel.DrugId > 0)
            {
                return PartialView("PreTestAlertView", preTestAlertModel);
            }
            else
                if (preTestAlertModel.IsPGxDrug == true)
                {
                    PostTestAlertServiceModel postTestAlertModel = ser.PostTestAlertService(patientId, drugName);
                    if (postTestAlertModel.guidelineId > 0)
                    {
                        if (postTestAlertModel.Chemical.ToLower() == "warfarin")
                        {
                            WarfarinDoseModel warfarinDoseModel = new WarfarinDoseModel()
                            {
                                PostTestAlertServiceModel = postTestAlertModel,
                                VKORC1 = postTestAlertModel.DiplotypeResults.FirstOrDefault(x => x.GeneSymbol == "VKORC1").Diplotype,
                                CYP2C9 = postTestAlertModel.DiplotypeResults.FirstOrDefault(x => x.GeneSymbol == "CYP2C9").Diplotype,
                                CYP4F2 = postTestAlertModel.DiplotypeResults.FirstOrDefault(x => x.GeneSymbol == "CYP4F2").Diplotype,
                            };
                            return PartialView("PostTestAlertWarfarinView", warfarinDoseModel);
                        }
                        else
                        {
                            return PartialView("PostTestAlertView", postTestAlertModel);
                        }
                    }
                }
                
                    return PartialView("SubmissionContinue");
        }
        [HttpPost]
        public string WarfarinDoseCalculate(WarfarinDoseModel doseModel)
        {
            return ser.WarfarinDoseCalculator(doseModel);
        }
    }
}
