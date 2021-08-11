using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PGx.KB.Vcf.Haplotype.Model;
using PGx.Model.Entities;
using PGx.Model.Entities.Data;
using PGx.Model.Util;
namespace PGx.KB.Vcf.Haplotype
{

    public class DiplotypeMatcher
    {
        private MatchData m_matchdata;
        public DiplotypeMatcher(MatchData matchdata)
        {
            m_matchdata = matchdata;
        }


        public List<DiplotypeMatch> Compute()
        {

            // compare sample permutations to haplotypes
            SortedSet<HaplotypeMatch> matches = ComparePermutations();

            if (m_matchdata.getPermutations().Count() == 1)
            {
                return DetermineHomozygousPairs(matches);
            }
            // find matched pairs
            return DetermineHeterozygousPairs(matches);
        }




        
         //Compares a sample's allele permutations to haplotype definitions and return matches.       
        protected SortedSet<HaplotypeMatch> ComparePermutations()
        {

            HashSet<HaplotypeMatch> haplotypeMatches = new HashSet<HaplotypeMatch>(m_matchdata.GetNamedAlleles
                .Select(x => new HaplotypeMatch(x)));

            foreach (String samplePermutation in m_matchdata.getPermutations())
            {
                foreach (HaplotypeMatch hm in haplotypeMatches)
                {
                    hm.Match(samplePermutation);
                }
            }

            var sortedHaploMatches= new SortedSet<HaplotypeMatch>(haplotypeMatches
                .Where(h => !(h.Sequences == null || h.Sequences.Count() == 0)));
            return sortedHaploMatches;
        }

       // Determine possible diplotypes given a set of {@link HaplotypeMatch}'s when sample is homozygous at all positions.      
       // @param haplotypeMatches the matches that were found via {@link #comparePermutations()}

        private List<DiplotypeMatch> DetermineHomozygousPairs(SortedSet<HaplotypeMatch> haplotypeMatches)
        {

            //String seq = m_dataset.getPermutations().iterator().next();
            String seq = m_matchdata.getPermutations().First();
            List<DiplotypeMatch> matches = new List<DiplotypeMatch>();
            if (haplotypeMatches.Count() == 1)
            {
                // matched a single haplotype: need to return that as a diplotype
                HaplotypeMatch hm = haplotypeMatches.First();
                DiplotypeMatch dm = new DiplotypeMatch(hm, hm, m_matchdata);
                dm.addSequencePair(new String[] { seq, seq });
                matches.Add(dm);
            }
            else
            {
                // return all possible pairings of matched haplotypes
                List<List<HaplotypeMatch>> pairs = CombinationUtil.generatePerfectPairs(haplotypeMatches);
                foreach (List<HaplotypeMatch> pair in pairs)
                {
                    DiplotypeMatch dm = new DiplotypeMatch(pair.ElementAt(0), pair.ElementAt(1), m_matchdata);
                    dm.addSequencePair(new String[] { seq, seq });
                    matches.Add(dm);
                }
            }
            return matches;
        }




        // Determine possible diplotypes given a set of {@link HaplotypeMatch}'s when sample is heterozygous at at least one position.

         // @param haplotypeMatches the matches that were found via {@link #comparePermutations()}

        private List<DiplotypeMatch> DetermineHeterozygousPairs(SortedSet<HaplotypeMatch> haplotypeMatches)
        {

            SortedDictionary<NamedAllele, HaplotypeMatch> hapMap = new SortedDictionary<NamedAllele, HaplotypeMatch>();
            foreach (HaplotypeMatch hm in haplotypeMatches)
            {
                hapMap.Add(hm.M_haplotype, hm);
            }

            // possible pairs from what got matched
            List<List<NamedAllele>> pairs = CombinationUtil.generatePerfectPairs(hapMap.Keys);

            List<DiplotypeMatch> matches = new List<DiplotypeMatch>();
            foreach (List<NamedAllele> pair in pairs)
            {
                NamedAllele hap1 = pair.ElementAt(0);
                HaplotypeMatch hm1 = hapMap.Where(x => x.Key == hap1).FirstOrDefault().Value;
                if (hm1 == null)
                {
                    continue;
                }
                NamedAllele hap2 = pair.ElementAt(1);
                HaplotypeMatch hm2 = hapMap.Where(x => x.Key == hap2).FirstOrDefault().Value;
                if (hm2 == null)
                {
                    continue;
                }

                if (hap1 == hap2 && hm1.Sequences.Split(',').Count() == 1)
                {
                    // cannot call homozygous unless more than one sequence matches
                    continue;
                }

                HashSet<String[]> sequencePairs = findSequencePairs(hm1, hm2);
                if (!(sequencePairs == null || sequencePairs.Count() == 0))
                {
                    DiplotypeMatch dm = new DiplotypeMatch(hm1, hm2, m_matchdata);
                    sequencePairs.ToList().ForEach(x => dm.addSequencePair(x));
                    matches.Add(dm);
                }
            }
            //Collections.sort(matches);
            matches.Sort();
            return matches;
        }



         // Finds valid complementary pairs of sample's alleles for possible diplotype match.

        private HashSet<String[]> findSequencePairs(HaplotypeMatch hm1, HaplotypeMatch hm2)
        {

            HashSet<String[]> sequencePairs = new HashSet<String[]>();
            foreach (String seq1 in hm1.Sequences.Split(','))
            {
                foreach (String seq2 in hm2.Sequences.Split(','))
                {
                    if (isViableComplement(seq1, seq2))
                    {
                        sequencePairs.Add(new String[] { seq1, seq2 });
                    }
                }
            }
            return sequencePairs;
        }



        // Checks whether the two sequences is complementary based on sample alleles.

        private Boolean isViableComplement(String sequence1, String sequence2)
        {

            String[] seq1 = sequence1.Split(';');
            String[] seq2 = sequence2.Split(';');

            for (int x = 0; x < seq1.Length; x += 1)
            {
                String[] s1 = seq1[x].Split(':');
                String[] s2 = seq2[x].Split(':');
                SampleAllele sampleAllele = m_matchdata.getSampleAllele(int.Parse(s1[0]));
                if (sampleAllele.getAllele1().Equals(sampleAllele.getAllele2()))
                {
                    // expecting homozygous
                    if (!s1[1].Equals(s2[1]))
                    {
                        return false;
                    }
                }
                else
                {
                    // expecting heterozygous
                    if (s1[1].Equals(s2[1]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

}