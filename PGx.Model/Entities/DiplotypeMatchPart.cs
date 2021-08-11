
using System;
using System.Collections.Generic;
using System.Linq;
using PGx.Model.Common.Util;
using PGx.Model.Entities;
using PGx.Model.Entities.Data;

namespace PGx.Model.Entities
{
   public partial class DiplotypeMatch : IComparable<DiplotypeMatch> {

  private HashSet<String[]> m_sequences = new HashSet<String[]>();
  //private MatchData m_dataset;

       public MatchData Dataset { get; set; }
       public HaplotypeMatch Haplotype1 { get; set; }
       public HaplotypeMatch Haplotype2 { get; set; }

       public DiplotypeMatch ()
       {

       }
  public DiplotypeMatch( HaplotypeMatch hm1,  HaplotypeMatch hm2,  MatchData dataset) {
    Haplotype1 = hm1;
    Haplotype2 = hm2;
    Name = Haplotype1.Name + "/" + Haplotype2.Name;
    Score = Haplotype1.M_haplotype.getScore() + Haplotype2.M_haplotype.getScore();
    Dataset = dataset;
  }



  public void addSequencePair(String[] pair)
  {
      // Preconditions.checkNotNull(pair);
      //Preconditions.checkArgument(pair.length == 2, "Sequence pair must have 2 sequences");
      m_sequences.Add(pair);
      
  }

  public String getFunction() {
    if (Haplotype1 != null && Haplotype2 != null
        && !String.IsNullOrEmpty(Haplotype1.M_function) && !String.IsNullOrEmpty(Haplotype2.M_function)) {

      SortedSet<String> alleleNames = new SortedSet<string>();
      alleleNames.Add(Haplotype1.M_function.ToLower());
      alleleNames.Add(Haplotype2.M_function.ToLower());

      if (alleleNames.Count() == 1) {
        return "Two " + alleleNames.First() + " alleles";
      }
      else {
        //Iterator<String> alleleIt = alleleNames.Iterator();
        return "One " + alleleNames.ElementAt(0) + " allele and one " + alleleNames.ElementAt(1) + " allele";
      }

    }
    return "N/A";
  }


  //public MatchData getDataset() {
  //  return Dataset;
  //}



  public override String ToString() {
    return this.Name;
  }


  public int CompareTo( DiplotypeMatch o) {

    int rez = CompareUtils.Compare(o.Score, this.Score);
    if (rez != 0) {
      return rez;
    }
    //rez = ObjectUtils.compare(m_haplotype1, o.getHaplotype1());
    if (!this.Haplotype1.Equals(o.Haplotype1)) {
      return 1;
    }
    //return ObjectUtils.compare(m_haplotype2, o.getHaplotype2());
    if (Haplotype2.Equals(o.Haplotype2))
    {
        return 0;
    }
    else
    {
        return 1;
    }

  }
}

}
