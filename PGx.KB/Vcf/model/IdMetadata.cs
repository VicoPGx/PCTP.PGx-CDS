using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PGx.KB.Infrastructure;
namespace PGx.KB.Vcf.model
{
    public class IdMetadata : BaseMetadata {

  private static  Logger sf_logger=new Logger();

  public static  String ID = "ID";

  public IdMetadata(String id)
      : base(new Dictionary<String, String>())
  {
      putPropertyRaw(ID, id);
      init(true);
  }

  public IdMetadata( Dictionary<String, String> properties):base(properties) {
    init(true);
  }

  protected IdMetadata( String id, Boolean isBaseType):base(new Dictionary<String,String>()) {
    putPropertyRaw(ID, id);
    init(isBaseType);
  }

  protected IdMetadata( Dictionary <String, String> properties, Boolean isBaseType):base(properties) {
    init(isBaseType);
  }

  private void init(Boolean isBaseType) {
    if (getPropertyRaw(ID) == null) {
      sf_logger.warn("Required metadata property \"{}\" is missing", ID);
    }
    if (isBaseType) {
      ensureNoExtras(ID);
    }
  }
  public String getId() {
    return getPropertyRaw(ID);
  }

}
}