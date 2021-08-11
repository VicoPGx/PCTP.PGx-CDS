using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PGx.Model.Entities;
namespace PGx.KB.Models
{

    public class WarfarinDoseModel
    {

        public PostTestAlertServiceModel PostTestAlertServiceModel { get; set; }


        public int Age { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public string Race { get; set; }
        public string EnzymeInducer{get;set;}
        public string Amiodarone { get; set; }
        public string VKORC1 { get; set; }
        public string CYP2C9 { get; set; }
        public string CYP4F2 { get; set; }
    }


    public class PreTestAlertServiceModel
    {
        public int DrugId { get; set; }
        public bool IsPGxDrug { get; set; }
        public string Summary { get; set; }
        public string Chemical { get; set; }
        public string Gene { get; set; }
        public string GeneNull { get; set; }
        public string Implication { get; set; }
        public List<Literature> Guidelines { get; set; }
        public string TargetedDrugRecommendation { get; set; }        
        public List<SER_RiskPhenotype> RiskPhenotypes { get; set; }
        public List<SER_GenePopPhenoFre> GenePopPhenoFre { get; set; }
        public string FreJson { get; set; }
    }

    public class SER_RiskPhenotype
    {
        public string Symbol { get; set; }
        public string Phenotype { get; set; }
        public List<SER_Frequency> PopFrequency { get; set; }
    }

    public class SER_GenePopPhenoFre
    {
        public List<string> Populations { get; set; }
        public string Symbol { get; set; }
        public string PopulationsGeneSpecific { get; set; }
        public List<SER_PopulationPhneotypeFrequency> PopulationPhenotypeFrequency { get; set; }
    }

    public class SER_PopulationPhneotypeFrequency
    {
        public string Population { get; set; }
        public string RiskFrequency { get; set; }
        public List<SER_PhenoFrequency> PhenotypeFrequencyList { get; set; }
    }

    public class SER_PhenoFrequency
    {
        public string Phenotype { get; set; }
        public string Frequency { get; set; }
        public string Risk { get; set; }
        public string EvidenceLevel { get; set; }
        public string ImpactLevel { get; set; }
    }

    public class SER_Frequency
    {
        public string Population { get; set; }
        public string Frequency { get; set; }
    }


    public class PostTestAlertServiceModel
    {
        public int GuidanceId { get; set; }
        public string Chemical { get; set; }
        public string GeneNull { get; set; }
        public int guidelineId { get; set; }
        public string GeneSymbol { get; set; }
        public string PatientName { get; set; }
        public string PatientId { get; set; }
        public string ClinicalImpact { get; set; }
        public string EvidenceLevel { get; set; }
        public List<Literature> Guidelines { get; set; }
        public List<SER_OrdersForGuidance> OrdersForGuidance { get; set; }
        public List<SER_MedicationGuide> MedicationGuide { get; set; }
        public SER_DosingGuidance DosingGuidance { get; set; }
        public List<SER_DiplotypeResult> DiplotypeResults { get; set; }
    }
    public class SER_OrdersForGuidance
    {
        public string Name { get; set; }
        public string Dosage { get; set; }
        public string Unit { get; set; }
        public string Frequency { get; set; }
        public string Route { get; set; }
        public string Remark { get; set; }
    }
    
    public class SER_MedicationGuide
    {
        public string GuideId { get; set; }
        public string Name{get;set;}
        public string Indication{get;set;}
        public string Population{get;set;}
        public string DosageMax{get;set;}
        public string Dosage{get;set;}
        public string Unit{get;set;}
        public string Frequency{get;set;}
        public string AdministrationRoute{get;set;}
    }
    public class SER_DosingGuidance
    {
        public string Implication { get; set; }
        public string Recommendation { get; set; }
        public string Frequency { get; set; }
        public string ClinicalImpact { get; set; }
        public string EvidenceLevel { get; set; }
    }
    public class SER_DiplotypeResult
    {
        public string GeneSymbol { get; set; }
        public string Diplotype { get; set; }
        public string Phenotype { get; set; }
    }

}