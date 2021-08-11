using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PGx.Model.Entities;

namespace PGx.KB.Vcf.Haplotype.Model
{
   
public class Result {

  private Metadata m_metadata;
  private List<GeneCall> m_geneCalls = new List<GeneCall>();
  private Dictionary<String, List<String>> m_vcfWarnings;

  public Metadata Metadata
  {
      get { return m_metadata; }
     set {m_metadata = value;}
  }

  public List<GeneCall> GeneCalls{
      get { return m_geneCalls; }
  }

  public void AddGeneCall(GeneCall call) {
    m_geneCalls.Add(call);
  }

  public Dictionary<String, List<String>> VcfWarnings {
      get { return m_vcfWarnings; }
      set { m_vcfWarnings = value; }
  }
}
}