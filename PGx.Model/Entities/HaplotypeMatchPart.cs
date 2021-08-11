using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PGx.Model.Common.Util;
using PGx.Model.Entities;

namespace PGx.Model.Entities
{
    public partial class HaplotypeMatch : IComparable<HaplotypeMatch>
    {
        public NamedAllele M_haplotype { get; set; }

        public HaplotypeMatch()
        { }
        public HaplotypeMatch(NamedAllele haplotype)
        {
            M_haplotype = haplotype;
            Name = M_haplotype.Name;
            M_function = M_haplotype.M_Function;
        }

        public Boolean Match(String samplePermutation)
        {
            string definedPermutation = M_haplotype.getPermutations();
            if (Regex.IsMatch(samplePermutation, definedPermutation))
            {
                if (string.IsNullOrEmpty(Sequences) == true)
                {
                    Sequences = samplePermutation;
                }
                else
                    Sequences = Sequences + "," + samplePermutation;
                return true;
            }
            return false;
        }

        public int CompareTo(HaplotypeMatch o)
        {

            int rez = String.Compare(Name, o.Name);
            if (rez != 0)
            {
                return rez;
            }
            if (!M_haplotype.Equals(o.M_haplotype))
            {
                return 1;
            }
            return CompareUtils.Compare(Sequences.Split(',').Count(), o.Sequences.Split(',').Count());
        }


        public override String ToString()
        {
            return Name;
        }
    }
}