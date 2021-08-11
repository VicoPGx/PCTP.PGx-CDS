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
   public partial class Variant : IComparable<Variant>  {
  private static  String sf_rsidFormat = "%s%s";
  private static  String sf_positionFormat = "%d%s";
  private static  String sf_vcfAlleleJoiner = ",";


  public Variant()
  { 
  }
  public Variant( VariantLocus variant,  SampleAllele allele) {
    String call;
    String vcfAlleles = String.Join(sf_vcfAlleleJoiner, allele.getVcfAlleles());
    if (allele.isPhased()) {
      call = allele.getAllele1() + "|" + allele.getAllele2();
    } else {
      call = allele.getAllele1() + "/" + allele.getAllele2();
    }
    initialize(variant.Position, variant.Rsid, call, variant.getPosition(), vcfAlleles);
  }


  public Variant(int pos,  String rsids,  String call, int vcfPosition,  String vcfAlleles) {
    initialize(pos, rsids, call, vcfPosition, vcfAlleles);
  }

  private void initialize(int pos,  String rsids,  String call, int vcfPosition,  String vcfAlleles) {
    //m_position = pos;
    //m_rsid = rsids;
    //m_vcfCall = call;
    //if (call != null) {
    //  m_isPhased = call.Contains("|");
    //}
    //m_vcfPosition = vcfPosition;
    //m_vcfAlleles = vcfAlleles;
      Position = pos;
      Rsid = rsids;
      VcfCall = call;
      if (call != null)
      {
          IsPhased = call.Contains("|");
      }
      VcfPosition = vcfPosition;
      VcfAlleles = vcfAlleles;
  }


  //@Override
  public int CompareTo(Variant o)
  {

    int rez =CompareUtils.Compare(Position, o.Position);
    if (rez != 0) {
      return rez;
    }
    rez =CompareUtils.Compare(VcfPosition, o.VcfPosition);
    if (rez != 0) {
      return rez;
    }
    return String.Compare(VcfCall, o.VcfCall);
  }

  public override String ToString() {
      String vcfCall = VcfCall == null ? null :  Regex.Replace(VcfCall, "[|/]", "");
      //m_vcfCall.replaceAll("[|/]", "");
    if (Rsid != null) {
      return String.Format(sf_rsidFormat, Rsid, vcfCall);
    } else {
      return String.Format(sf_positionFormat, Position, vcfCall);
    }
  }
  //public int Compare(int x, int y)
  //{
  //     return (x < y) ? -1 : ((x == y) ? 0 : 1);
  //}
}
}
