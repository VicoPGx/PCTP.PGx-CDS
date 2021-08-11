using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PGx.Model.Entities.Data;
namespace PGx.Model.Entities
{
    public partial class GeneCall
    {

        public int CallID { get; set; }
        public GeneCall(String alleleDefinitionVersion, String chromosome, String gene, MatchData matchData, HashSet<String> uncallableHaplotypes, HashSet<String> ignoredHaplotypes)
        {

            AlleleDefinitionVersion = alleleDefinitionVersion;
            Chromosome = chromosome;
            Gene = gene;
            MatchData = matchData;
            UncallableHaplotypes = string.Join(",", uncallableHaplotypes.ToList());
            IgnoredHaplotypes = String.Join(",", ignoredHaplotypes.ToList());
            VarOfInterest = matchData.ExtraPositions.ToList();
            IsPhased = true;
            this.DiplotypeMatch = new HashSet<DiplotypeMatch>();
            this.HaplotypeMatch = new HashSet<HaplotypeMatch>();
            this.Variants = new HashSet<Variant>();
            this.VarOfInterest = new HashSet<VarOfInterest>();
        }

        public MatchData MatchData
        {
            get;
            set;
        }

        public void AddDiplotype(DiplotypeMatch diplotype)
        {
            DiplotypeMatch.Add(diplotype);
            HaplotypeMatch.Add(diplotype.Haplotype1);
            HaplotypeMatch.Add(diplotype.Haplotype2);
        }

        public void AddVariant(Variant pos)
        {
            Variants.Add(pos);
            if (pos.IsPhased==false)
            {
                IsPhased = false;
            }
        }

        public override String ToString()
        {
            return Gene;
        }

    }
}
