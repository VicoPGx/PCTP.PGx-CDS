using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PGx.KB.Vcf.model
{
   public enum ReservedInfoPropertyEnum {

  // standard
       AncestralAllele,
       AlleleCount,
       AlleleFrequency ,
       AlleleNumber,
       BaseQuality,
       Cigar ,
       Dbsnp ,
       Depth ,
       Hapmap2,
       Hapmap3 ,
       MappingQuality ,
       MappingQualityZeroCount,
       NumberOfSamples ,
       StrandBias,
       SomaticMutation ,
       Validated ,
       ThousandGenomes,

       // imprecise structural variants

       Imprecise,
       Novel ,
       End ,// also standard

       /**
        * Note that the VCF 4.2 spec calls this "SYTYPE", which is clearly a typo.
        */
       StructuralVariantType ,
       StructuralVariantLength ,

       ConfidenceIntervalForPosition,
       ConfidenceIntervalForEnd ,
       HomologyLength ,
       HomologySequence,

       BreakpointId ,

       // precise structural variants

       MobileElementInfo ,
       MobileElementTransduction ,
       DgvId,
       DbvarId ,
       DbripId,
       MateId ,
       PartnerId,
       EventId ,
       ConfidenceIntervalForInsertedMaterial,
       ReadDepthOfAdjacency,
       CopyNumberOfSegment,
       CopyNumberOfAdjacency ,
       ConfidenceIntervalForSegmentCopyNumber ,
       ConfidenceIntervalForAdjacencyCopyNumber
}

   public  class ReservedInfoPropertyClass
   {
       public static Dictionary<String, ReservedInfoPropertyStruct> infoPropertyDic;
       static ReservedInfoPropertyClass()
       {
           infoPropertyDic.Add("AncestralAllele", new ReservedInfoPropertyStruct("AA", "Ancestral allele", typeof(String), false, "1"));
           infoPropertyDic.Add("AlleleCount", new ReservedInfoPropertyStruct("AC", "Allele count in genotypes, for each ALT allele, in the same order as listed", typeof(long), true, "A"));
           infoPropertyDic.Add("AlleleFrequency", new ReservedInfoPropertyStruct("AF", "Allele frequency for each ALT allele in the same order as listed: use this when estimated" + "from primary data, not called genotypes", typeof(Decimal), true, "A"));
           infoPropertyDic.Add("AlleleNumber", new ReservedInfoPropertyStruct("AN", "Total number of alleles in called genotypes", typeof(long), false, "1"));
       }
   }

public struct ReservedInfoPropertyStruct:ReservedProperty{

  private   String m_id;

  private   String m_description;

  private   Type m_type;

  private   String m_number;

  private  Boolean m_isList;

   //public void
   public ReservedInfoPropertyStruct( String id,  String description,  Type type, Boolean isList,
       String number) {
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
