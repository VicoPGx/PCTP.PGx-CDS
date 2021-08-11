using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PGx.KB.Infrastructure;

namespace PGx.KB.Vcf.model
{
public class ContigMetadata : IdMetadata 
{

  private static  Logger sf_logger =new Logger(); 


  public static  String ID = "ID";
  public static  String LENGTH = "length";
  public static  String ASSEMBLY = "assembly";
  public static  String MD5 = "md5";
  public static  String SPECIES = "species";
  public static  String TAXONOMY = "taxonomy";
  public static  String URL = "URL";

  public ContigMetadata( String id, long length,  String assembly,  String md5,
       String species,  String taxonomy,  String url) :base(id,false){
    //super(id, false);
    putPropertyRaw(LENGTH, length.ToString());
    putPropertyRaw(ASSEMBLY, assembly);
    if (md5 != null) {
      putPropertyRaw(MD5, md5);
    }
    if (species != null) {
      putAndQuoteProperty(SPECIES, species);
    }
    if (taxonomy != null) {
      putPropertyRaw(TAXONOMY, taxonomy);
    }
    if (url != null) {
      try {
        new Uri(url);
      } catch (Exception e) {
        sf_logger.warn("URL {} is malformed", url, e.Message);
      }
    }
    putPropertyRaw(URL, url);
    Init();
  }

  public ContigMetadata( Dictionary<String, String> properties):base(properties,false) {

    Init();
  }


  public long getLength() {
    return long.Parse(getPropertyRaw(LENGTH));
  }

  
  //  @return Null only when invalid
 
  
  public String GetAssembly 
  {
      get { return getPropertyRaw(ASSEMBLY); }
  }

  
  public String GetTaxonomy() {
    return getPropertyRaw(TAXONOMY);
  }

  
  public String GetSpecies() {
    return getPropertyUnquoted(SPECIES);
  }

  
  public String GetMd5() {
    return getPropertyRaw(MD5);
  }

  
  public String GetUrl() {
    return getPropertyRaw(URL);
  }

  private void Init() {
    if (getPropertyUnquoted(ASSEMBLY) == null) {
      sf_logger.warn("Required metadata property \"{}\" is missing", ASSEMBLY);
    }
    String length = getPropertyUnquoted(LENGTH);
    if (length == null) {
      sf_logger.warn("Required metadata property \"{}\" is missing", LENGTH);
    } else {
      try {
        //noinspection ResultOfMethodCallIgnored
        long.Parse(length);
      } catch (Exception e) {
        sf_logger.warn("Length is not a number", e.Message);
      }
    }
    ensureNoExtras(ID, LENGTH, ASSEMBLY, MD5, SPECIES, TAXONOMY, URL);
  }

}
}