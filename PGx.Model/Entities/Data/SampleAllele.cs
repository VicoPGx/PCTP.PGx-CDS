using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using PGx.KB.Common;
using System.Text.RegularExpressions;
using System.Text;
using PGx.Model.Entities;
using PGx.Model.Common;
using PGx.Model.Enums;
namespace PGx.Model.Entities.Data
{
   public class SampleAllele : IComparable<SampleAllele> {
  static Boolean STRICT_CHECKING = false;
  private String m_chromosome;
  private int m_position;
  private String m_allele1;
  private String m_allele2;
  private Boolean m_isPhased;
  private List<String> m_vcfAlleles;

  public SampleAllele( String chromosome, long position,  String a1,  String a2,
      Boolean isPhased,  List<String> vcfAlleles) {
    m_chromosome = chromosome;
    m_position = (int)position;
    if (a1.Contains("ins") || a1.Contains("del")) {
      m_allele1 = a1;
    } else {
      m_allele1 = a1.ToUpper();
    }
    if (a2 != null) {
      if (a2.Contains("ins") || a2.Contains("del")) {
        m_allele2 = a2;
      } else {
        m_allele2 = a2.ToUpper();
      }
    }
    m_isPhased = isPhased;
    m_vcfAlleles = vcfAlleles;
  }

  public String getChromosome() {
    return m_chromosome;
  }

  public int getPosition() {
    return m_position;
  }

  public String getChrPosition() {
    return m_chromosome + ":" + m_position;
  }

  public String getAllele1() {
    return m_allele1;
  }

  public String getAllele2() {
    return m_allele2;
  }
  
  public Boolean isPhased() {
    return m_isPhased;
  }

  public List<String> getVcfAlleles() {
    return m_vcfAlleles;
  }

  
  public  String toString() {
    return m_allele1 + "/" + m_allele2 + " @ " + m_chromosome + ":" + m_position;
  }

  
  public int CompareTo( SampleAllele o) {

    int rez = new ChromosomeNameComparator().Compare(m_chromosome, o.getChromosome());
    if (rez != 0) {
      return rez;
    }
    return Compare(m_position, o.getPosition());
  }
           int Compare(int x,int y){
    return (x < y) ? -1 : ((x == y) ? 0 : 1);
}


  /**
   * Checks if VCF alleles indicates a deletion.
   * This is a trivial check that there are different allele lengths; this could be improved.
   */
  public Boolean isVcfAlleleADeletion() {
    HashSet<int> lengths=new HashSet<int>();
      m_vcfAlleles.ForEach(str=>lengths.Add(str.Length));
        //Set<Integer> lengths =  m_vcfAlleles.stream()
        //.map(String::length)
        //.collect(Collectors.toSet());
    return lengths.Count() > 1;
  }


  /**
   * Interprets the alleles in this {@link SampleAllele} in terms of the given {@link VariantLocus}.
   * This will return a <strong>new</strong> {@link SampleAllele} if the {@link VariantLocus} is not a SNP, with it's
   * alleles modified to use the format used by the allele definitions.
   */
  public SampleAllele positionAlleleConvertByType(VariantLocus variant) {

    if (variant.Type == VariantType.SNP.ToString()) {
      return this;
    }
    String a1 = m_allele1;
    String a2 = m_allele2;

    if (variant.Type == VariantType.INS.ToString()) 
    {
      // VCF:         TC  -> TCA
      // definition:  del -> insA
      a1 = convertInsertion(m_allele1);
      a2 = convertInsertion(m_allele2);

    } 
    else if (variant.Type == VariantType.DEL.ToString()) 
    {
      if (!isVcfAlleleADeletion()) {
        throw new InvalidOperationException("Expecting deletion but alleles in VCF doesn't seem to be a deletion");
      }
      // VCF:         TC -> T
      // definition:  C  -> delC
      a1 = convertDeletion(variant, m_allele1);
      a2 = convertDeletion(variant, m_allele2);
    } 
    else if (variant.Type == VariantType.REPEAT.ToString()) 
    {
      // VCF:         ATAA -> ATATATATATATATATAA
      // definition:  A(TA)6TAA  -> A(TA)7TAA

        var reg = new Regex(VariantLocus.REPEAT_PATTERN);
        var matches = reg.Match(variant.ReferenceRepeat);

        if(!reg.IsMatch(variant.ReferenceRepeat))
        {
        throw new InvalidOperationException("Invalid repeat format for " + variant.ChromosomeHgvsName);
        }
      String prefix = matches.Groups[1].Value;
      String repeat = matches.Groups[2].Value;
      String postfix = matches.Groups[4].Value;
      a1 = convertRepeat(variant, prefix, repeat, postfix, m_allele1);
      a2 = convertRepeat(variant, prefix, repeat, postfix, m_allele2);
    }
    return new SampleAllele(m_chromosome, m_position, a1, a2, m_isPhased, m_vcfAlleles);
  }


  /**
   * Convert from VCF insertion to allele definition insertion format.
   * <pre><code>
   * VCF:         TC  -> TCA
   * definition:  del -> insA
   * </code></pre>
   */
  private  String convertInsertion( String allele) {

    String refBase = m_vcfAlleles.ElementAt(0);
    if (allele.Equals(refBase)) {
      return "del";
    }

    // must be an ALT, and therefore longer than REF
    //Preconditions.checkState(allele.length() > ref.length(), "Not an insertion: " + ref + " >" + allele);
    return "ins" + allele.Substring(refBase.Length);
  }

  /**
   * Convert from VCF deletion to allele definition deletion format.
   * <pre><code>
   * VCF:         TC -> T
   * definition:  C  -> delC
   * </code></pre>
   */
  private  String convertDeletion( VariantLocus variant,  String allele) {

    String refBase = m_vcfAlleles.ElementAt(0);
    if (allele.Equals(refBase)) {
      return allele.Substring(1);
    }

    // must be an ALT, and therefore shorter than REF
    //Preconditions.checkState(allele.length() < ref.length(), "Not an deletion: " + ref + " >" + allele + " @ " +
        //variant.getChromosomeHgvsName());
    return "del" + refBase.Substring(1);
  }

  private  String convertRepeat( VariantLocus variant,  String prefix,  String repeat,
       String postfix,  String allele) {

    if (allele.Contains("(")) {
      // already a repeat
      if (STRICT_CHECKING) {
          var reg=new Regex(VariantLocus.REPEAT_PATTERN);
          var m = reg.Match(allele);
        //Matcher m = VariantLocus.REPEAT_PATTERN.matcher(allele);
        //if (!m.matches()) {
           if (!reg.IsMatch(allele)) {
          throw new ArgumentException("Sample has " + allele + ", which is an invalid repeat format @ " +
              variant.ChromosomeHgvsName);
        }
        if (!m.Groups[1].Value.StartsWith(repeat) || !m.Groups[2].Value.EndsWith(postfix) || !m.Groups[4].Value.EndsWith(postfix)) {
          throw new ArgumentException("Sample has " + allele + ", which doesn't match expected repeat (" +
              prefix + "(" + repeat + ")#" + postfix + " @ " + variant.ChromosomeHgvsName);
        }
      }
      return allele;
    }

    // validate
    if (!allele.StartsWith(prefix) || !allele.EndsWith(postfix)) {
      if (STRICT_CHECKING) {
        throw new ArgumentException("Sample has " + allele + ", which doesn't match expected repeat " +
            prefix + "(" + repeat + ")#" + postfix + " @ " + variant.ChromosomeHgvsName);
      }
      return allele;
    }

    String rep = allele.Substring(prefix.Length, allele.Length - postfix.Length);
    int numReps =  rep.Length / repeat.Length;
    if (!rep.Equals(StrRepeat(repeat, numReps))) {
      if (STRICT_CHECKING) {
        throw new ArgumentException("Sample has " + allele + ", which doesn't match expected repeat " +
            prefix + "(" + repeat + ")#" + postfix + " @ " + variant.ChromosomeHgvsName);
      } else {
        return allele;
      }
    }

    return prefix + "(" + repeat + ")" + numReps + postfix;
  }

       //Method from StringUtils
       public  String StrRepeat( String str,  int repeat) {
        // Performance tuned for 2.0 (JDK1.4)

        if (str == null) {
            return null;
        }
        if (repeat <= 0) {
            return string.Empty;
        }
         int inputLength = str.Length;
        if (repeat == 1 || inputLength == 0) {
            return str;
        }
        if (inputLength == 1 && repeat <= 8192) {
            return ChrRepeat(str.ElementAt(0), repeat);
        }

         int outputLength = inputLength * repeat;
        switch (inputLength) {
            case 1 :
                return ChrRepeat(str.ElementAt(0), repeat);
            case 2 :
                 char ch0 = str.ElementAt(0);
                 char ch1 = str.ElementAt(1);
                 char[] output2 = new char[outputLength];
                for (int i = repeat * 2 - 2; i >= 0; i--, i--) {
                    output2[i] = ch0;
                    output2[i + 1] = ch1;
                }
                return new String(output2);
            default :
                StringBuilder buf = new StringBuilder(outputLength);
                for (int i = 0; i < repeat; i++) {
                    buf.Append(str);
                }
                return buf.ToString();
        }
    }
       //Method from StringUtils
           public  String ChrRepeat( char ch,  int repeat) {
         char[] buf = new char[repeat];
        for (int i = repeat - 1; i >= 0; i--) {
            buf[i] = ch;
        }
        return new String(buf);
    }
}
}
