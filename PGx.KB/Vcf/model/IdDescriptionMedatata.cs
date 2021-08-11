using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PGx.KB.Infrastructure;

namespace PGx.KB.Vcf.model
{

public class IdDescriptionMetadata : IdMetadata {

  private static  Logger sf_logger=new Logger();

  public static  String ID = "ID";
  public static  String DESCRIPTION = "Description";

  public IdDescriptionMetadata( String id,  String description):base(id) {
   putAndQuoteProperty(DESCRIPTION, description);
    init(true);
  }

  public IdDescriptionMetadata( Dictionary<String, String> properties, Boolean isBaseType):base(properties,false) {
    
    init(isBaseType);
  }

  protected IdDescriptionMetadata( String id,  String description, Boolean isBaseType):base(id) {
    putAndQuoteProperty(DESCRIPTION, description);
    init(isBaseType);
  }

  private void init(Boolean isBaseType) {
    if (getPropertyRaw(DESCRIPTION) == null) {
      sf_logger.warn("Required metadata property \"{}\" is missing", DESCRIPTION);
    }
    if (isBaseType) {
      ensureNoExtras(ID, DESCRIPTION);
    }
  }
  public String getDescription() {
    return getPropertyUnquoted(DESCRIPTION);
  }
}
}