using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PGx.KB.Infrastructure;
using System.Text.RegularExpressions;

namespace PGx.KB.Vcf.model
{

 //This class represents a single VCF FORMAT metadata line.

// ##FORMAT=<ID=ID,Number=number,Type=type,Description="description">

public class FormatMetadata : IdDescriptionMetadata {

  private static  Logger sf_logger=new Logger();
      //= LoggerFactory.getLogger(MethodHandles.lookup().lookupClass());

  public static  String ID = "ID";
  public static  String DESCRIPTION = "Description";
  public static  String NUMBER = "Number";
  public static  String TYPE = "Type";

  private FormatType m_type;

  public FormatMetadata( String id,  String description,  String number,  FormatType type):base(id,description,false) {
    
    putPropertyRaw(NUMBER, number);
    putPropertyRaw(TYPE, Enum.GetName(typeof(FormatType),type));
    init();
  }

  public FormatMetadata( Dictionary<String, String> properties) :base(properties,false){
 
    init();
  }

  public void init() {
    String number = getPropertyRaw(NUMBER);
    if (number == null) {
      sf_logger.warn("Required metadata property \"{}\" is missing", NUMBER);
    } 
    else if (!Regex.IsMatch(number,VcfUtils.NUMBER_PATTERN))
    {
      sf_logger.warn("{} is not a VCF number: '{}'", NUMBER, number);
    }
    m_type =(FormatType)Enum.Parse(typeof(FormatType),getPropertyRaw(TYPE));
    ensureNoExtras(ID, DESCRIPTION, NUMBER, TYPE);
  }


  //Value is either an integer or ".".
  // @return Null only when incorrectly constructed without one

  
  public String getNumber() {
    return getPropertyRaw(NUMBER);
  }


   // @return A special (reserved) <em>Number</em> ("A", "G", "R", or "."), or null if the Number is not reserved

  
  public SpecialVcfNumber getReservedNumber() {
    return SpecialVcfNumberClass.fromId(getPropertyRaw(NUMBER));
  }

  
  public FormatType getType() {
    return m_type;
  }
}
}