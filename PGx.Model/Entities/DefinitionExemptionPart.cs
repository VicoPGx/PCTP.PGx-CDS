using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGx.Model.Entities
{
    public partial class DefinitionExemption
    {
        private SortedSet<VariantLocus> m_extraPositions; 
        private SortedSet<string> m_ignoredAlleles;
        private SortedSet<string> m_ignoredAllelesLc; 
        public SortedSet<VariantLocus> ExtraPositionsSorted
        {
            get
            {
                m_extraPositions = new SortedSet<VariantLocus>();
                if (this.ExtraPositions != null && this.ExtraPositions.Count() > 0)
                {
                    this.ExtraPositions.ToList().ForEach(x => { m_extraPositions.Add(x); });
                    return m_extraPositions;
                }
                else
                {  
                    return m_extraPositions;
                }
            }
            set { this.m_extraPositions = value; }
        }
      
        public SortedSet<String> IgnoredAllelesSorted{
        get {
            m_ignoredAlleles = new SortedSet<string>();
            if (this.IgnoredAlleles != null && this.IgnoredAlleles.Count() > 0)
            {
                IgnoredAlleles.Split(',').ToList().ForEach(x => { m_ignoredAlleles.Add(x); });
            }
            return m_ignoredAlleles;
        }
            set { this.m_ignoredAlleles = value; }
        }

        public SortedSet<string> IgnoredAllelesLcSorted
        {
            get {
                m_ignoredAllelesLc = new SortedSet<string>();
                if (IgnoredAllelesLc != null && IgnoredAllelesLc.Count() > 0)
                {
                    IgnoredAllelesLc.Split(',').ToList().ForEach(x => { m_ignoredAllelesLc.Add(x); });
                }
                return m_ignoredAllelesLc; }
        }
    


        //public DefinitionExemption(String gene, SortedSet<VariantLocus> extraPositions,
        //     SortedSet<String> ignoredAlleles, Boolean allHits, Boolean assumeReference)
        //{
        //    Gene = gene;
        //    if (extraPositions == null)
        //    {
        //        ExtraPositions = new SortedSet<VariantLocus>();
        //    }
        //    else
        //    {
        //        ExtraPositions = extraPositions;
        //    }
        //    if (ignoredAlleles == null)
        //    {
        //        m_ignoredAlleles = new SortedSet<string>();
        //        //m_ignoredAllelesLc = m_ignoredAlleles;
        //    }
        //    else
        //    {
        //        IgnoredAllelesSorted = ignoredAlleles;
        //        //m_ignoredAllelesLc = new SortedSet<string>(ignoredAlleles.Select(x => x.ToLower()));
        //    }
        //    AllHits = allHits;
        //    AssumeReference = assumeReference;
        //}




        /**
         * Gets the extra positions to pull allele information for.
         */
        //public SortedSet<VariantLocus> getExtraPositions()
        //{
        //    return m_extraPositions;
        //}

        /**
         * Gets the named alleles to ignore.
         */
        //public SortedSet<String> getIgnoredAlleles()
        //{
        //    return m_ignoredAlleles;
        //}

        /**
         * Checks if should ignore the given named allele.
         */
        public Boolean shouldIgnore(String allele)
        {
            return IgnoredAllelesLcSorted.Contains(allele.ToLower());
        }




    }
}
