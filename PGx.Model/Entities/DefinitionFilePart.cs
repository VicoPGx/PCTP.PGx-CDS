using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PGx.Model.Entities
{


    public partial class DefinitionFile
    {
        [FileTypes("pdf,txt,rtf,doc,xsl,ppt,docx,xslx,pptx,html,jpg,jpeg,png,gif,bmp")]
        public HttpPostedFileBase FrequencyFile { get; set; }
        [FileTypes("pdf,txt,rtf,doc,xsl,ppt,docx,xslx,pptx,html,jpg,jpeg,png,gif,bmp")]
        public HttpPostedFileBase FunctionFile { get; set; }
        [FileTypes("pdf,txt,rtf,doc,xsl,ppt,docx,xslx,pptx,html,jpg,jpeg,png,gif,bmp")]
        public HttpPostedFileBase AlleleDefinitionFile { get; set; }
        [FileTypes("pdf,txt,rtf,doc,xsl,ppt,docx,xslx,pptx,html,jpg,jpeg,png,gif,bmp")]
        public HttpPostedFileBase DiplotypePhenotypeFile { get; set; }
        public Dictionary<VariantLocus, List<String>> VariantAllelesMap{get;set;}

        private SortedDictionary<String, VariantLocus> RsidMap = new SortedDictionary<string, VariantLocus>();


        public List<String> getVariantAlleles(VariantLocus vl)
        {
            return vl.Alleles.Split(',').ToList();
        }

        public String toString()
        {
            return "Allele definition for " + GeneSymbol;
        }

      public override Boolean Equals(Object o) {
    if (this == o) return true;
    if (!(o is DefinitionFile)) return false;
    DefinitionFile that = (DefinitionFile)o;
    return Object.Equals(this.FormatVersion, that.FormatVersion) &&
        Object.Equals(this.ModificationDate, that.ModificationDate) &&
       Object.Equals(this.GeneSymbol, that.GeneSymbol) &&
       Object.Equals(this.Orientation, that.Orientation) &&
        Object.Equals(this.Chromosome, that.Chromosome) &&
        Object.Equals(this.GenomeBuild, that.GenomeBuild) &&
       Object.Equals(this.RefSeqChromosome, that.RefSeqChromosome) &&
        Object.Equals(this.RefSeqGene, that.RefSeqGene) &&
        Object.Equals(this.RefSeqProtein, that.RefSeqProtein) &&
        Object.Equals(this.Notes, that.Notes) &&
        Object.Equals(this.Populations, that.Populations) &&
        Array.Equals(this.VariantLocus, that.VariantLocus) &&
        //Object.Equals(this.VariantAllele, that.VariantAllele) &&
        Object.Equals(this.NamedAllele, that.NamedAllele);
  }
      public override int GetHashCode()
      {
          return HashCode(FormatVersion, ModificationDate, GeneSymbol, Orientation, Chromosome, GenomeBuild, RefSeqChromosome, this.HashCode(RefSeqGene),
             RefSeqProtein, Notes, this.HashCode(Populations),VariantLocus, NamedAllele);
      }
      public int HashCode(params Object[] a)
      {
          if (a == null)
              return 0;

          int result = 1;

          foreach (Object element in a)
          {
              result=31 *result + (element==null? 0:element.GetHashCode());

          }
          return result;
      }
      public int HashCode(Object a)
      {
          if (a == null)
              return 0;
          else
              return a.GetHashCode();
      }     
    } 
}
