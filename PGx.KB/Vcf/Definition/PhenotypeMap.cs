using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PGx.KB.Vcf.Definition.Model;
using PGx.KB.Vcf.Haplotype.Model;
using PGx.Model.Entities;
using System.Web.Script.Serialization;
using PGx.KB.Models;
namespace PGx.KB.Vcf.Definition
{
    public class PhenotypeMap
    {
        PGx_KBEntities context = new PGx_KBEntities();
        
  private List<GenePhenotype> m_geneHapPhenoDics=new List<GenePhenotype>();
        

  public PhenotypeMap( List<GeneCall> calls)   {

      context.DefinitionFile.ToList().ForEach(x =>
      {
          GenePhenotype genePhenotype = new GenePhenotype();
          genePhenotype.Gene = x.GeneSymbol;
          foreach (NamedAllele haplotypeMap in x.NamedAllele)
          {
              genePhenotype.Haplotypes.Add(haplotypeMap.Name, haplotypeMap.M_Function);
          }

          x.PhenotypeMap.ToList().ForEach(t =>
          {
            foreach(var pD in t.PhenotypeDef)
                  genePhenotype.Diplotypes
                  .Add(new DiplotypePhenotypeGui
                  {
                      Phenotype = pD.Phenotype,
                      Diplotype = pD.Genotype.Split('/').ToList()
                  });
   
          });
          m_geneHapPhenoDics.Add(genePhenotype);
      }
          );
      if (calls != null) {
        calls.ForEach(c => {
          String gene = c.Gene;
          SortedSet<HaplotypeMatch> haplotypematches=new SortedSet<HaplotypeMatch>();
          c.HaplotypeMatch.ToList().ForEach(h => haplotypematches.Add(h));
          GenePhenotype lookupResult = lookup(gene);

          if (lookupResult!=null) {
            GenePhenotype genePhenotype = lookupResult;
            foreach (HaplotypeMatch haplotypeMatch in haplotypematches) {
              String hap = haplotypeMatch.Name;
              String newFunction = haplotypeMatch.M_function;
              String existingFunction = genePhenotype.lookupHaplotype(hap);

              if (existingFunction != null && !existingFunction.Equals(newFunction, StringComparison.OrdinalIgnoreCase))
              {
                throw new ArgumentException("Function mismatch for " + gene + " " + hap + " > " + existingFunction + " != " + newFunction);
              }
              if (existingFunction == null) {
                genePhenotype.addHaplotypeFunction(hap, newFunction);
              }
            }
          }
        });
      }
  }
  protected List<GenePhenotype> getGenes() {
    return m_geneHapPhenoDics;
  }
  public GenePhenotype lookup(String gene) {
    return m_geneHapPhenoDics.Where(p => gene.Equals(p.Gene)).FirstOrDefault();
  }

    }
}