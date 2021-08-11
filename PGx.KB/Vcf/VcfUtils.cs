using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using PGx.KB.Vcf.model;

namespace PGx.KB.Vcf
{
    public class VcfUtils
    {
        private static  String sf_simpleAltPattern =
      "(?:"                   + // wrap the whole expression
        "(?:"                 + // allow nucleotides, symbolic IDs, or both
          "(?:[AaCcGgTtNn]+)" + // nucleotides
          "|(?:<.+>)"         + // symbolic IDs (declared in ALT metadata)
          ")+"                + // allow things like C<ctg1> (apparently)
        "|\\*"                + // indicates that the position doesn't exist due to an upstream deletion
      ")";

  private static  String sf_number =
      "(?:"                 + // wrap the whole expression
        "(?:\\d+|(?:<.+>))" + // numbers or symbolic IDs
        "(?::\\d+)?"        + // optional insertion
      ")";                    // ends the nc group of the first line

  private static   String sf_breakpointAltPattern = 
      "(?:"                                                         + // wrap the whole expression
        "\\.?"                                                      + // optional opening dot
        "(?:"                                                       + // start breakpoint types
          "(?:" + sf_simpleAltPattern + "?\\[" + sf_number + "\\[)"  + // breakpoint type 1: t[p[
          "|(?:" + sf_simpleAltPattern + "?\\]" + sf_number + "\\])" + // breakpoint type 2: t]p]
          "|(?:\\]" + sf_number + "\\]" + sf_simpleAltPattern + "?)" + // breakpoint type 3: ]p]t
          "|(?:\\[" + sf_number + "\\[" + sf_simpleAltPattern + "?)" + // breakpoint type 4: [p[t
        ")"                                                         + // end breakpoint types
        "\\.?"                                                      + // optional closing dot
      ")"                                                             // ends the nc group of the first line
  ;

  public static  String ALT_BASE_PATTERN = 
      "\\.|" +                                 // means no variant
      "(?:\\.?" + sf_simpleAltPattern + ")"  + // ex: .A
      "|(?:" + sf_simpleAltPattern + "\\.?)" + // ex: A.
      "|" + sf_breakpointAltPattern            // ex: C[2[
  ;

  public static  String REF_BASE_PATTERN = "[AaCcGgTtNn]+";
  public static  String METADATA_PATTERN = ",(?=([^\"]*\"[^\"]*\")*[^\"]*$)";
  public static  String FORMAT_PATTERN = "[A-Z0-9:]+";
  public static  String RSID_PATTERN = "rs\\d+";
  public static  String NUMBER_PATTERN = "(?:\\d+|[ARG\\.])";
        //public static  Pattern NUMBER_PATTERN = Pattern.compile("(?:\\d+|[ARG\\.])");

  public static  String FILE_FORMAT_PATTERN = "VCFv[\\d\\.]+";

  public static  String UNQUOTED_EQUAL_SIGN_PATTERN = "=(?=([^\"]*\"[^\"]*\")*[^\"]*$)";

  public static  Dictionary<String, String> extractPropertiesFromLine( String value) {
    String unescapedValue = value.Replace("\\\\", "~~~~");
    unescapedValue = unescapedValue.Replace("\\\\\"", "~!~!");
    Boolean wasEscaped = !unescapedValue.Equals(value);
    //String[] cols = VcfUtils.METADATA_PATTERN.Split(unescapedValue);
    String[] cols = Regex.Split(unescapedValue, ",");
    //var regex = new Regex(METADATA_PATTERN);
    //String[] cols = regex.Split(value);
    if (wasEscaped) {
      for (int x = 0; x < cols.Length; x++) {
        cols[x] = cols[x].Replace("~~~~", "\\");
        cols[x] = cols[x].Replace("~!~!", "\"");
      }
    }
    return extractProperties(cols);
  }

  public static  Dictionary<String, String> extractProperties( String[] props) {
    Dictionary<String, String> map = new Dictionary<String,String>();
    foreach(String prop in props) {
      KeyValuePair<String, String> pair=new KeyValuePair<string,string>();
      try {
        pair = splitProperty(prop);
      } catch {
          //(RuntimeException e) {
        //throw new IllegalArgumentException("Error parsing property \"" + prop + "\"", e);
      //}
    }
      map.Add(pair.Key, pair.Value);
    }
    return map;
  }

  /**
   * Splits a property into a key-value pair.
   * @param prop In the form "key=value"
   */
  public static KeyValuePair<String, String> splitProperty( String prop) {
    //String[] parts = Regex.Split(prop,UNQUOTED_EQUAL_SIGN_PATTERN);
      String[] parts = Regex.Split(prop, "=");
      //Regex regex = new Regex(UNQUOTED_EQUAL_SIGN_PATTERN);
    //String[] parts=UNQUOTED_EQUAL_SIGN_PATTERN.split(prop);
    if (parts.Length != 2) {
      throw new Exception("There were " + (parts.Length - 1) + " equals signs for: " + prop);
    }
    return new KeyValuePair<string,string> (parts[0], parts[1]);
  }

  /**
   * Adds double quotation marks around a string.
   */
  
  public static String quote( String str) {
    return "\"" + str + "\"";
  }

  /**
   * Removes double quotation marks around a string if they are present.
   */
  
  public static String unquote( String str) {
    if (str.StartsWith("\"") && str.EndsWith("\"")) {
      return str.Substring(1, str.Length - 1);
    }
    return str;
  }

  /**
   * Converts a String representation of a property into a more useful type.
   * Specifically, can return:
   * <ul>
   *   <li>String</li>
   *   <li>Long</li>
   *   <li>BigDecimal</li>
   *   <li>The Boolean true (for flags)</li>
   *   <li>A List of any of the above types</li>
   * </ul>
   */
  public static Type convertProperty(ReservedProperty key, String value)
  {
      //ReservedFormatPropertyStruct structValue;
      //ReservedInfoPropertyClass.infoPropertyDic.TryGetValue(key.ToString(), out structValue);
   return convertProperty(key.GetType(), value, key.isList());
  }

  /**
   * @see #convertProperty(ReservedProperty, String)
   */

  public static  Type convertProperty( Type clas,  String value, Boolean isList) {
    if (value == null || "."==value) {
      return null;
    }
    if (!isList) {
      try {
        return  convertElement(clas, value).GetType();
      }  catch (InvalidCastException e) {
      //catch (ClassCastException e) {
        throw new ArgumentException("Wrong type specified", e);
      }
    }
    List<Object> list = new List<Object>();
    foreach (String part in value.Split(',')) {
      list.Add(convertElement(clas, part));
    }
    try {
      return list.GetType();
    }
    catch (InvalidCastException e)
    {
      throw new ArgumentException("Wrong type specified", e);
    }
  }

  public static  Type convertProperty( FormatType type,  String value) {
    Type clas;
    switch (type) {
      case FormatType.Integer:
        clas = typeof(long);
        break;
      case FormatType.Float:
        clas = typeof(Decimal);
        break;
      case FormatType.Character:
        clas = typeof(char);
        break;
      case FormatType.String:
        clas = typeof(String);
        break;
      default:
        //throw new RuntimeException(FormatType.class.getSimpleName() + " " + type + " isn't covered?!");
        throw new SystemException(typeof(FormatType).FullName + " " + type + " isn't covered?!");
    }
    return convertProperty(clas, value, false);
  }

  public static  Type convertProperty( InfoType type,  String value) {
    Type clas;
    switch (type) {
      case InfoType.Integer:
        clas = typeof(long);
        break;
      case InfoType.Float:
        clas = typeof(Decimal);
        break;
      case InfoType.Character:
        clas = typeof(char);
        break;
      case InfoType.String:
        clas = typeof(String);
        break;
      case InfoType.Flag:
        clas = typeof(Boolean);
        break;
      default:
        throw new SystemException(typeof(InfoType).FullName + " " + type + " isn't covered?!");
            //RuntimeException(InfoType.class.getSimpleName() + " " + type + " isn't covered?!");
    }
    return convertProperty(clas, value, false);
  }

  private static  object convertElement( Type clas,  String value) {
    if (value == null || ".".Equals(value)) {
      return null;
    }
    if (clas == typeof(String)) {
      return value;
    } else if (clas == typeof(char)) {
      if (value.Length == 1) {
        return value;
      } else {
        throw new ArgumentException("Invalid character value '" + value + "'");
      }
    } else if (clas == typeof(Boolean)) {
      value = value.Trim();
      if (value == null) {
        return true;
      }
      if (value.Equals("0") || value.ToLower().Equals("false")) {
        return false;
      }
      if (value.Equals("1") || value.ToLower().Equals("true")) {
        return true;
      }
      throw new  ArgumentException("Invalid boolean value: '" + value + "'");
          //IllegalArgumentException("Invalid boolean value: '" + value + "'");

    } else if (clas == typeof(Decimal)) {
      try {
        return Convert.ToDecimal(value);
      } catch (FormatException e){ 
          //(NumberFormatException e) {
        throw new ArgumentException("Expected float; got " + value);
      }
    } else if (clas == typeof(long)){
      try {
          
        return long.Parse(value);
      } catch (FormatException e) {
        throw new ArgumentException("Expected integer; got " + value);
      }
    }
    throw new NotSupportedException("Type " + clas + " unrecognized");           
        //UnsupportedOperationException("Type " + clas + " unrecognized");
  }
    }
}