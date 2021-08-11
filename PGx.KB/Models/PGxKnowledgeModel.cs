using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FileHelpers;

namespace PGx.KB.Models
{

    [DelimitedRecord("\t")]
    public class Variants
    {
        public string RsId;
        public string Chromsome;
        public string Position;
        public string Genotype;
    }

    [DelimitedRecord("\t")]
    public class VariantsCus
    {
        public string RsId;
        public string Ref;
        public string Alt_A;
        public string Alt_B;
        public int? AltA_Score;
        public int? AltB_Score;
        public int? DipScore;
    }
    [DelimitedRecord("\t")]
    public class VariantsFromFile
    {
        public string RsId;
        public string Ref;
        public string Alt_A;
        public string Alt_B;
    }
    public class VerboseSelect
    {
        public string RsId;
        public string Ref;
        public string Alt_A;
        public string Alt_B;

    }

    [DelimitedRecord("\t")]
    public class Verbose
    {
        public string Chr;
        public string Start;
        public string End;
        public string Ref;
        public string Alt;
        public string Func_refGeneWithVer;
        public string Gene_refGeneWithVer;
        public string GeneDetail_refGeneWithVer;
        public string ExonicFunc_refGeneWithVer;
        public string AAChange_refGeneWithVer;
        public string AAChange_refGeneWithVer_Selected;
        public string OMIM_Link;
        public string OMIM_Phenotype;
        public string Disease_CH_CHPO;
        public string Inheritence_Model;
        public string cytoBand;
        public string Transcript;
        public string DieaseName;
        public string Nucleotide_Changes;
        public string Amino_Acid_Changes;
        public string Pathogenicity;
        public string Interpretation_Evidence;
        public string First_Report_Reference;
        public string Important_Reference;
        public string Secondary_Reference;
        public string ClinicalSignificance;
        public string Submitter;
        public string CollectionMethod;
        public string ClinVar_Link;
        public string InterVar_automated;//PVS1;PS1;PS2;PS3;PS4;PM1;PM2;PM3;PM4;PM5;PM6;PP1;PP2;PP3;PP4;PP5;BA1;BS1;BS2;BS3;BS4;BP1;BP2;BP3;BP4;BP5;BP6;BP7;
        public string PublicDB_ClinSig;
        public string PublicDB_phenotype;
        public string PublicDB_Ref;
        public string PublicDB_ExtraInfo;
        public string Local_Freq_Value;
        public string ExAC_EAS_Value;
        public string ExAC_ALL_Value;
        public string GnomAD_EAS_Value;
        public string GnomAD_ALL_Value;
        public string Local_Freq;
        public string ExAC_EAS;
        public string ExAC_ALL;
        public string ExAC_QUAL;
        public string GnomAD_Exome_EAS;
        public string GnomAD_Exome_ALL;
        public string GnomAD_Exome_Hom;
        public string GnomAD_Exome_Hom_Male;
        public string GnomAD_Exome_Hom_Female;
        public string GnomAD_QUAL;
        public string esp6500siv2_all;
        public string g2015aug_all;
        public string avsnp;
        public string PP3_BP4; //:SIFT,Polyphen2_HDIV,Polyphen2_HVAR,LRT,MutationTaster,MutationAssessor,FATHMM,PROVEAN,fathmm_MKL_coding,MetaSVM,MetaLR;
        public string QUAL;
        public string FILTER;
        public string GT_AD_DP_GQ_PL;
        public string GHR_Link;
        public string HPO_ID;
        public string HPO_Term;
        public string CHPO_Term;
    }

    public class AnnatatedRsid
    {
        public string GeneSymbol;
        public string RsId;
        public string AltA;
        public string AltB;
        public int? AltA_Score;
        public int? AltB_Score;
        public int? DipScore;
    }


    public class AnnatatedRsidComparer : IEqualityComparer<AnnatatedRsid>
    {
        public bool Equals(AnnatatedRsid x, AnnatatedRsid y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;
            return x.RsId == y.RsId && x.DipScore == y.DipScore;
        }

        public int GetHashCode(AnnatatedRsid annatatedRsid)
        {
            if (Object.ReferenceEquals(annatatedRsid, null)) return 0;
            int hashRsid = annatatedRsid.RsId == null ? 0 : annatatedRsid.RsId.GetHashCode();
            int hashDipScore = annatatedRsid.DipScore.GetHashCode();
            return hashRsid ^ hashDipScore;
        }
    }
}