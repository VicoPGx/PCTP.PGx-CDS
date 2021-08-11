using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PGx.KB.Vcf.model
{
   /**
 * This class contains sample data for a VCF position line.
 *
 * @author Mark Woon
 */
public class VcfSample {

  //private LinkedHashMap<String, String> m_properties = new LinkedHashMap<>();
    private Dictionary<String, String> m_properties = new Dictionary<String,String>();
  public VcfSample( List<String> keys,  List<String> values) {
    if (keys == null) {
      if (values == null || values.Count() == 0) {
        return;
      }
      throw new ArgumentException("keys is null but values is not");
    } else if (values == null) {
      throw new ArgumentException("values is null but keys is not");
    }
    //Preconditions.checkArgument(keys.size() == values.size(), "Number of keys does not match number of values");
    for (int x = 0; x < keys.Count(); x++) {
      m_properties.Add(keys.ElementAt(x), values.ElementAt(x));
    }
    init();
  }

  public VcfSample( Dictionary<String, String> properties) {
    m_properties = properties;
    init();
  }

  private void init() {
    foreach (var entry in m_properties) {
      if (entry.Key.Contains("\n") || entry.Value.Contains("\n")) {
        throw new ArgumentException("FORMAT [[[" + entry.Key + "=" + entry.Value + "]]] contains a newline");
      }
    }
  }

  public  String getProperty( String key) {
      string val;
     m_properties.TryGetValue(key,out val);
      return val;
      
  }

  /**
   * Returns the value for the reserved property as the type specified by both {@link ReservedFormatProperty#getType()}
   * and {@link ReservedFormatProperty#isList()}.
   * @param <T> The type specified by {@code ReservedInfoProperty.getType()} if {@code ReservedFormatProperty.isList()}
   *           is false;
   *           otherwise {@code List<V>} where V is the type specified by {@code ReservedFormatProperty.getType()}.
   */
  //@SuppressWarnings("InfiniteRecursion")
  public Type getProperty(ReservedFormatPropertyEnum key)
  {
      ReservedFormatPropertyStruct structValue;
      ReservedFormatPropertyClass.formatPropertyDic.TryGetValue(key.ToString(), out structValue);
      return VcfUtils.convertProperty(structValue, getProperty(structValue.getId()));
  }

  public Boolean containsProperty( String key) {
    return m_properties.ContainsKey(key);
  }

  public Boolean containsProperty( ReservedFormatPropertyEnum key) {
      ReservedFormatPropertyStruct structValue;
      ReservedFormatPropertyClass.formatPropertyDic.TryGetValue(key.ToString(), out structValue);
    return m_properties.ContainsKey(structValue.getId());
  }

  public void putProperty( String key,  String value) {
    m_properties.Add(key, value);
  }

  public void putProperty( ReservedFormatPropertyEnum key,  String value) {
      ReservedFormatPropertyStruct structValue;
      ReservedFormatPropertyClass.formatPropertyDic.TryGetValue(key.ToString(), out structValue);
      m_properties.Add(structValue.getId(), value);
  }

  public void removeProperty( String key) {
    m_properties.Remove(key);
  }

  public void removeProperty( ReservedFormatPropertyEnum key) {
      ReservedFormatPropertyStruct structValue;
      ReservedFormatPropertyClass.formatPropertyDic.TryGetValue(key.ToString(), out structValue);
    m_properties.Remove(structValue.getId());
  }

  public void clearProperties() {
    m_properties.Clear();
  }

  /**
   * @return A set of the property keys, which has guaranteed order
   */
  
  public  List<String> getPropertyKeys() {
    // LinkedHashMap.keySet() returns a LinkedKeySet, which has guaranteed order
    return m_properties.Keys.ToList();
  }

  
  //public Set<Map.Entry<String, String>> propertyEntrySet() {
  //  return m_properties.entrySet();
  //}

}
}