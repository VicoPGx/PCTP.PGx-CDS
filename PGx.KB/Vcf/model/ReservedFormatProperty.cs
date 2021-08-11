using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PGx.KB.Vcf.model
{

    public  enum ReservedFormatPropertyEnum  {


        Genotype ,
        Depth ,
        Filter,
        GenotypeLikelihoods ,
        GenotypeLikelihoodsOfHeterogenousPloidy ,
        PhredScaledGenotypeLikelihoods ,
        GenotypePosteriorProbabilitiesPhredScaled ,
        GenotypeQualityConditional ,
        HaplotypeQualities,
        PhaseSet,
        PhasingQuality ,
        ExpectedAlleleCounts,
        MappingQuality ,
        // structural variants
        CopyNumber ,
        CopyNumberGenotypeQuality ,
        CopyNumberLikelihood ,
        PhredScoreForNovelty,
        HaplotypeId ,
        AncestralHaplotypeId ,



}

    public  class ReservedFormatPropertyClass
    {
        public static Dictionary<String, ReservedFormatPropertyStruct> formatPropertyDic;
        static ReservedFormatPropertyClass()
        {
            formatPropertyDic.Add("Genotype", new ReservedFormatPropertyStruct("GT", "Genotype, encoded as allele values separated by either / or |.", typeof(String), false, "1"));
            formatPropertyDic.Add("Depth", new ReservedFormatPropertyStruct("DP", "Read depth at this position for this sample.", typeof(long), false, "1"));
            formatPropertyDic.Add("Filter", new ReservedFormatPropertyStruct("FT", "Sample genotype filter indicating if this genotype was called.", typeof(String), false));
            formatPropertyDic.Add("GenotypeLikelihoods", new ReservedFormatPropertyStruct("GL", "Genotype likelihoods comprised of comma separated floating point log10-scaled likelihoods" + " for all possible genotypes given the set of alleles defined in the REF and ALT fields.", typeof(Decimal), true, "G"));
        }
    }

    
    public struct ReservedFormatPropertyStruct:ReservedProperty
  {

     private   String m_id;

     private   String m_description;

     private   Type m_type;

     private  Boolean m_isList;

     private   String m_number;

    public ReservedFormatPropertyStruct( String id,  String description,  Type type, Boolean isList):this(id, description, type, isList, null) 
    { 
    }

 public ReservedFormatPropertyStruct( String id,  String description,  Type type, Boolean isList,  String number) 
 {
    m_id = id;
    m_description = description;
    m_type = type;
    m_isList = isList;
    m_number = number;
  }
        
  public String getId() {
    return m_id;
  }

  
  public String getDescription() {
    return m_description;
  }

  
  public Type getType() {
    return m_type;
  }

  
  public String getNumber() {
    return m_number;
  }

  public Boolean isList() {
    return m_isList;
  }

}

}
