using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGx.Model.Enums;
using PGx.Model.Common;
using PGx.Model.Common.Util;

namespace PGx.Model.Entities
{
    public partial class VariantLocus:IComparable<VariantLocus>
    {
        public static string REPEAT_PATTERN = "([ACGT]+)\\(([ACGT]+)\\)(\\d+)([ACGT]+)";
        public String getChrPosition(string chr)
        {
            return chr + ":" + getPosition();
        }
        public String getChrPosition()
        {
            return this.DefinitionFile.Chromosome.ToLower() + ":" + getPosition();
        }

        /**
         * Gets the VCF position for this variant.
         */
        public int getPosition()
        {
            
            if (Type == VariantType.DEL.ToString())
            {
                return this.Position - 1;
            }
            return this.Position;
        }


  public override  Boolean Equals(Object o) {
    if (this == o) {
      return true;
    }

    if (!(o is VariantLocus))
    {
      return false;
    }

    VariantLocus that = (VariantLocus)o;
    return Position == that.Position &&
        Type == that.Type &&
        Chromosome==that.Chromosome&&
        ChromosomeHgvsName.Equals( that.ChromosomeHgvsName) &&
        Object.Equals(GeneHgvsName, that.GeneHgvsName);
  }

  
  public override int GetHashCode() {
      Object [] Objects=new Object[3];
      Objects[0]=Position;
      Objects[1]=ChromosomeHgvsName;
      Objects[2]=GeneHgvsName;
      //Objects[3]=ProteinNote;
      //Objects[4]=Rsid;
      //Objects[5]=ResourceNote;
      if(Objects==null||Objects.Length==0)
      {
          return 0;
      }
      int result=1;

      foreach (var element in Objects)
      {
          result = 31 * result + (element == null ? 0 : element.GetHashCode());
      }
      
    return result;
  }



  public int CompareTo(VariantLocus o)
  {

    int rez = new ChromosomeNameComparator().Compare(Chromosome, o.Chromosome);
    if (rez != 0) {
      return rez;
    }
     rez = this.getPosition().CompareTo(o.getPosition());
    if (rez != 0
) {
      return rez;
    }
    return ChromosomeHgvsName.CompareTo(o.ChromosomeHgvsName);
  }

}
  
}
