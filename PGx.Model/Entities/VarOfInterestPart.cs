using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using PGx.KB.Vcf.Definition;
using System.Text.RegularExpressions;
using PGx.Model.Common.Util;
using PGx.Model.Entities;
using PGx.Model.Entities.Data;

namespace PGx.Model.Entities
{
  public partial  class VarOfInterest: IComparable<VarOfInterest>
    {
      

  private static  String sf_rsidFormat = "%s%s";
  private static  String sf_positionFormat = "%d%s";
  private static  String sf_vcfAlleleJoiner = ",";

  private int m_position;
  private String m_rsid;
  private String m_vcfCall;
  private Boolean m_isPhased;
  private int m_vcfPosition;
  private String m_vcfAlleles;
  public VarOfInterest() 
  { 
  }

  public VarOfInterest( VariantLocus variant,  SampleAllele allele) {
    String call;
    String vcfAlleles = String.Join(sf_vcfAlleleJoiner, allele.getVcfAlleles());
    if (allele.isPhased()) {
      call = allele.getAllele1() + "|" + allele.getAllele2();
    } else {
      call = allele.getAllele1() + "/" + allele.getAllele2();
    }
    initialize(variant.Position, variant.Rsid, call, variant.getPosition(), vcfAlleles);
  }


  public VarOfInterest(int pos, String rsids, String call, int vcfPosition, String vcfAlleles)
  {
    initialize(pos, rsids, call, vcfPosition, vcfAlleles);
  }

  private void initialize(int pos,  String rsids,  String call, int vcfPosition,  String vcfAlleles) {
    m_position = pos;
    m_rsid = rsids;
    m_vcfCall = call;
    if (call != null) {
      m_isPhased = call.Contains("|");
    }
    m_vcfPosition = vcfPosition;
    m_vcfAlleles = vcfAlleles;
  }


  public int getPosition() {
    return m_position;
  }

  public  String getRsid() {
    return m_rsid;
  }

  public  String getVcfCall() {
    return m_vcfCall;
  }

  public Boolean isPhased() {
    return m_isPhased;
  }

  public int getVcfPosition() {
    return m_vcfPosition;
  }

  public  String getVcfAlleles() {
    return m_vcfAlleles;
  }

  //@Override
  public int CompareTo( VarOfInterest o) {

    int rez =CompareUtils.Compare(m_position, o.getPosition());
    if (rez != 0) {
      return rez;
    }
    rez =CompareUtils.Compare(m_vcfPosition, o.getVcfPosition());
    if (rez != 0) {
      return rez;
    }
    return String.Compare(m_vcfCall, o.getVcfCall());
  }

  public override String ToString() {
      String vcfCall = m_vcfCall == null ? null :  Regex.Replace(m_vcfCall, "[|/]", "");
    if (m_rsid != null) {
      return String.Format(sf_rsidFormat, Rsid, vcfCall);
    } else {
      return String.Format(sf_positionFormat, getPosition(), vcfCall);
    }
  }
    }
}
