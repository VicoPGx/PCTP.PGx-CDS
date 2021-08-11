using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace PGx.KB.Vcf.Util
{
    public class VariantUtils
    {


  private static  String sf_validCallPattern = "^.+[|/].+$";

  /**
   * Check whether a call string is in the expected format.
   * @param call a String representation of a call (can be null)
   * @return true if the call is like "X/X" or "X|X"
   */
  public static Boolean isValidCall( String call) {
    if (string.IsNullOrEmpty(call)) {
      return false;
    }
    //Regex reg = new Regex(sf_validCallPattern);
    //reg.IsMatch(call);
    //reg.Matches(call);
    //reg.Match(call);
 
    return Regex.IsMatch(call, sf_validCallPattern); 
      //sf_validCallPattern.matcher(call).matches();
  }

    }
}