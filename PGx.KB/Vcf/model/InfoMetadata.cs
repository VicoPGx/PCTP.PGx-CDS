using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PGx.KB.Infrastructure;
using System.Text.RegularExpressions;
namespace PGx.KB.Vcf.model
{
public class InfoMetadata : IdDescriptionMetadata {

  private static  Logger sf_logger=new Logger(); 

  public static  String ID = "ID";
  public static  String DESCRIPTION = "Description"; // should be quoted
  public static  String NUMBER = "Number";
  public static  String TYPE = "Type";
  public static  String SOURCE = "Source"; // should be quoted
  public static  String VERSION = "Version"; // should be quoted

  private InfoType m_type;

  public InfoMetadata( String id,  String description,  InfoType type,  String number,
       String source,  String version):base(id,description) {
   
    putPropertyRaw(NUMBER, number);
    putPropertyRaw(TYPE, Enum.GetName(typeof(InfoType),type));
    if (source != null) {
      putAndQuoteProperty(SOURCE, source);
    }
    if (version != null) {
      putAndQuoteProperty(VERSION, version);
    }
    init();
  }

  public InfoMetadata( Dictionary<String, String> properties):base(properties,false) {
 
    init();
  }

  private void init() {
    String number = getPropertyRaw(NUMBER);
    //assert number != null;
    if (!Regex.IsMatch(number,VcfUtils.NUMBER_PATTERN))
    {
      sf_logger.warn("{} is not a number: '{}'", NUMBER, number);
    }
    m_type = (InfoType)Enum.Parse(typeof(InfoType),getPropertyRaw(TYPE));
    ensureNoExtras(ID, DESCRIPTION, NUMBER, TYPE, SOURCE, VERSION);
  }

  
  public String getNumber() {
    return getPropertyRaw(NUMBER);
  }

  
  public SpecialVcfNumber getReservedNumber() {
      return SpecialVcfNumberClass.fromId(getPropertyRaw(NUMBER));
  }


  public InfoType getType() {
    return m_type;
  }

  
  public String getSource() {
    return getPropertyUnquoted(SOURCE);
  }

  
  public String getVersion() {
    return getPropertyUnquoted(VERSION);
  }
}
}