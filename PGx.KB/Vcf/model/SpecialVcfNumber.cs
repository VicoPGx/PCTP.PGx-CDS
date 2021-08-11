using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PGx.KB.Vcf.model
{
    /**
 * A reserved value for the "Number" field in INFO and FORMAT metadata entries.
 * @author Douglas Myers-Turnbull
 */
public enum SpecialVcfNumber {

  ONE_PER_ALT,
  ONE_PER_ALT_OR_REF,
  ONE_PER_GENOTYPE,
  UNKNOWN_OR_UNBOUNDED,
  NULL
}

  public class SpecialVcfNumberClass{
  public static SpecialVcfNumber fromId( String id) {
    switch(id) {
      case "A": return SpecialVcfNumber.ONE_PER_ALT;
      case "R": return SpecialVcfNumber.ONE_PER_ALT_OR_REF;
      case "G": return SpecialVcfNumber.ONE_PER_GENOTYPE;
      case ".": return SpecialVcfNumber.UNKNOWN_OR_UNBOUNDED;
    }
      return SpecialVcfNumber.NULL;
  }

  private  String m_id;

  SpecialVcfNumberClass(String id) {
    m_id = id;
  }

    
  public String getId() {
    return m_id;
  }
}
}
