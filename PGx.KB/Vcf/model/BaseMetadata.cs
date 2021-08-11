using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PGx.KB.Infrastructure;

namespace PGx.KB.Vcf.model
{
    public class BaseMetadata 
    {

         private static  Logger sf_logger=new Logger();

  private Dictionary<String, String> m_properties;

  public BaseMetadata() 
  { }

  public BaseMetadata( Dictionary<String, String> properties) {
    foreach (var entry in properties) {
      if (entry.Key.Contains("\n") || entry.Value.Contains("\n")) {
        throw new Exception("INFO [[[" + entry.Key + "=" + entry.Value + "]]] contains a newline");
      }
    }
    m_properties = properties;
  }

  
  public String getPropertyUnquoted( String key) {
    String got = m_properties[key];
    if (got == null) {
      return null;
    }
    return VcfUtils.unquote(got);
  }

  
  public String getPropertyRaw( String key) {
    return m_properties[key];
  }

  
  public Dictionary<String, String> getPropertiesUnquoted() {
    Dictionary<String, String> map = new Dictionary<String,String>();
    foreach (var entry in m_properties) {
      map.Add(entry.Key, VcfUtils.unquote(entry.Value));
    }
    return map;
  }

  
  public Dictionary<String, String> getPropertiesRaw() {
    return m_properties;
  }

  
  public List<String> getPropertyKeys() {
     
        return m_properties.Keys.ToList();
      
  }

  public void putAndQuoteProperty( String key,  String value) {
    if (value == null) {
      m_properties.Remove(key);
    } else {
      m_properties.Add(key, VcfUtils.quote(value));
    }
  }

  public void putPropertyRaw( String key,  String value) {
    m_properties.Add(key, value);
  }


   // Should be used only for base classes.
   // Logs a warning if this metadata contains a property key not in the array passed.
   // @param names An array of permitted property keys

  protected void ensureNoExtras(params String[] names) {
    HashSet<String> set = new HashSet<String>();
    m_properties.Keys.Where(property => !set.Contains(property)).ToList()
      .ForEach(property =>
     sf_logger.warn("Metadata line contains unexpected property {}", property));
  }
}
}