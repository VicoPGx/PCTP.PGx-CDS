using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PGx.KB.Vcf.Definition.Model
{
    public class GenePhenotype
    {

   public String Gene{get;set;}

   public Dictionary<String,String> Haplotypes{get;set;}
   public List<DiplotypePhenotypeGui> Diplotypes { get; set; }

  public GenePhenotype()
   {
       this.Haplotypes = new Dictionary<string, string>();
       this.Diplotypes = new List<DiplotypePhenotypeGui>();
   }

  public void addHaplotypeFunction(String haplotype, String func) {
    if (Haplotypes != null) {
      Haplotypes.Add(haplotype, func);
    }
  }
  public String lookupHaplotype(String hap)
  {
      if (hap == null)
      {
          return null;
      }
      if (Haplotypes.Keys.Contains(hap))
      {
          return Haplotypes[hap];
      }
      else
          return null;
  }

  private String[] makePhenoPair(String diplotype) {
    String[] haps = diplotype.Split('/');
    if (haps.Length != 2) {
        throw new ArgumentException("Diplotype doesn't have two alleles");
    }

    String hap1 = haps[0];
    String hap2 = haps[1];
    String func1 = Haplotypes[hap1];
    String func2 = Haplotypes[hap2];

    if (func1 == null) {
      throw new ArgumentException("No function phenotype for " + Gene + " " + hap1);
    }
    if (func2 == null) {
      throw new ArgumentException("No function phenotype for " + Gene + " " + hap2); 
    }

    return new String[]{func1,func2};
  }

  public String makePhenotype(String diplotype) {

    String[] phenoPair = makePhenoPair(diplotype);

    HashSet<String> phenos=new HashSet<string>();
       Diplotypes
        .Where(d => ((phenoPair[0].Equals(d.Diplotype.ElementAt(0)) && phenoPair[1].Equals(d.Diplotype.ElementAt(1))) || (phenoPair[0].Equals(d.Diplotype.ElementAt(1)) && phenoPair[1].Equals(d.Diplotype.ElementAt(0)))))
        .ToList()
        .ForEach(x=>phenos.Add(x.Phenotype));
                             
       
    if (phenos.Count()>1) {
        throw new ArgumentOutOfRangeException("More than one phenotype match made");
    } else if (phenos.Count() == 0) {
      return "N/A";
    } else {
      return phenos.FirstOrDefault();
    }
  }
    }
}