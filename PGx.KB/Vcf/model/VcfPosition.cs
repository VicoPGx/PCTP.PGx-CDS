using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PGx.KB.Infrastructure;
using System.Text.RegularExpressions;

namespace PGx.KB.Vcf.model
{
   public class VcfPosition {

  private static  Logger sf_logger=new Logger();

  private static  String sf_commaJoiner = ",";
  private static  String sf_whitespace = ".*\\s.*";
  private String m_chromosome;
  private long m_position;
  private List<String> m_ids = new List<String>();
  private String m_refBases;
  private List<String> m_altBases = new List<String>();
  private List<String> m_alleles = new List<String>();
  private Decimal m_quality;
  private List<String> m_filter = new List<String>();
  private Dictionary<String, List<String>> m_info =new Dictionary<String,List<String>>();
      //= ArrayListMultimap.create();
  private List<String> m_format = new List<String>();


  public VcfPosition( String chr, 
       long pos,
       List<String> ids,
       String  refBase,
       List<String> altBases,
       Decimal qual,
       List<String> filter,
       Dictionary<String, List<String>> info,
       List<String> format) {

    /*
      1. Check the arguments, in order
     */

    if (Regex.IsMatch(chr,sf_whitespace)  || chr.Contains(":")) {
      throw new ArgumentException("CHROM column \"" + chr + "\" contains whitespace or colons");
    }

    // allow pos < 1 because that's reserved for telomers

    if (ids != null) {
      foreach (String id in ids) {
        if (Regex.IsMatch(id,sf_whitespace) || id.Contains(";")) {
          throw new ArgumentException("ID \"" + id + "\" contains whitespace or semicolons");
        }
      }
    }

    if (!Regex.IsMatch(refBase,VcfUtils.REF_BASE_PATTERN)) {
      throw new ArgumentException("Invalid reference base '" + refBase +
          "' (must match " + VcfUtils.REF_BASE_PATTERN +")");
    }

    if (altBases != null) {
      foreach (String Base in altBases) {
        if  (!Regex.IsMatch(Base,VcfUtils.ALT_BASE_PATTERN)) {
          throw new ArgumentException("Invalid alternate base '" + Base + "' (must be [AaGgCcTtNn\\*]+ or <.+>)");
        }
      }
    }

    if (filter != null) {
      foreach (String f in filter) {
        if (Regex.IsMatch(f,sf_whitespace)){
            //(sf_whitespace.matcher(f).matches()) {
          throw new ArgumentException("FILTER column entry \"" + f + "\" contains whitespace");
        }
        if (f.Equals("0")) {
          throw new ArgumentException("FILTER column entry should not be 0");
        }
        if (f.Equals("PASS")) {
          if (filter.Count() == 1) { // a user is likely to pass "PASS" instead of an empty list or null
            sf_logger.warn("FILTER is PASS, but should have been passed as null. Converting to null");
            filter = null;
            break; // unnecessary, but gets rid of the warning
          } else { // but this is illegal per VCF spec
            throw new ArgumentException("FILTER contains PASS along with other filters!");
          }
        }
      }
    }

    if (info != null) {
      foreach (var  entry in info) {
        if (Regex.IsMatch(entry.Key,sf_whitespace)) {
          throw new ArgumentException("INFO column entry \"" + entry.Key +
              "\" contains whitespace");
        }
      }
    }

    if (format != null) {
      foreach (String f in format) {
        if (!Regex.IsMatch(f,VcfUtils.FORMAT_PATTERN)||f.Contains(":")) {
            //(!VcfUtils.FORMAT_PATTERN.matcher(f).matches() || f.contains(":")) {
          throw new ArgumentException("FORMAT column is not alphanumeric");
        }
      }
    }

    /*
      2. Set the fields, in order
     */

    // not resolving ID string
    m_chromosome = chr; // required
    m_position = pos; // required

    if (ids != null) {
      m_ids = ids;
    }

    m_refBases = refBase; // required
    m_alleles.Add(m_refBases);

    if (altBases != null) {
      m_altBases = altBases;
      m_alleles.AddRange(altBases);
    }

    m_quality = qual; // required

    if (filter != null) {
      m_filter = filter;
    }

    if (info != null) {
      m_info = info;
    }

    if (format != null) {
      m_format = format;
    }
  }

  public VcfPosition(  String  chromosome, long position,  String refBases,  Decimal quality) {
    m_chromosome = chromosome;
    m_position = position;
    m_refBases = refBases;
    m_quality = quality;
  }


  public  String getChromosome() {
    return m_chromosome;
  }

  public void setChromosome( String chromosome) {
    m_chromosome = chromosome;
  }

  public void setRef( String refBase) {
    m_refBases = refBase;
  }

  public long getPosition() {
    return m_position;
  }

  public void setPosition(long position) {
    m_position = position;
  }


  public  List<String> getIds() {
    return m_ids;
  }

  public  String getRef() {
    return m_refBases;
  }

  public  List<String> getAltBases() {
    return m_altBases;
  }

  public  String getAllele(int index) {
    return m_alleles.ElementAt(index);
  }


  public  Decimal getQuality() {
    return m_quality;
  }

  public void setQuality( Decimal quality) {
    m_quality = quality;
  }

  public Boolean isPassingAllFilters() {
    return m_filter==null;
  }

  /**
   * Returns a list of filters this position failed, if any.
   */
  public  List<String> getFilters() {
    return m_filter;
  }

  /**
   * Gets all INFO fields for every key.
   */
  public  Dictionary<String, List<String>> getInfo() {
    return m_info;
  }

  public  List<String> getInfo( String id) {
    if (hasInfo(id)) {
        List<String> listStr;
        m_info.TryGetValue(id,out listStr);
      return listStr;
        //return m_info.GetEnumerator(id);
    }
    return null;
  }


      public Type  getInfo( ReservedInfoPropertyStruct key) {
    if (!hasInfo(key.getId())) {
      return null;
    }

          List<String> list;
          m_info.TryGetValue(key.getId(),out list);
    if (list==null) {
      return null;
    }
          var str=string.Join(sf_commaJoiner,list);
    return VcfUtils.convertProperty(key, string.Join(sf_commaJoiner,list));
         
  }

  public Boolean hasInfo( String id) {
    return m_info != null && m_info.ContainsKey(id);
  }


  public Boolean hasInfo( ReservedInfoPropertyStruct key) {
    return hasInfo(key.getId());
  }

  public  List<String> getFormat() {
    return m_format;
  }

  
  public List<String> getInfoKeys() {
    if (m_info == null) {
      return new List<String>();
    }
    return m_info.Keys.ToList();
  }
}
}