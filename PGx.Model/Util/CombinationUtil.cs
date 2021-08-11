using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using PGx.Model.Entities.Data;

namespace PGx.Model.Util
{
    public class CombinationUtil
    {

        /**
         * Builds permutations for given alleles based on phasing.
         */
        public static HashSet<String> GeneratePermutations(List<SampleAllele> sampleAlleles)
        {

            HashSet<String> rez = generatePermutations(sampleAlleles, 0, true, "");

            if (sampleAlleles.ElementAt(0).isPhased())
            {
                foreach (var item in generatePermutations(sampleAlleles, 0, false, ""))
                {
                    rez.Add(item);
                }
            }
            if (rez.Count() == 0)
            {
                throw new InvalidOperationException("No permutations generated from " + sampleAlleles.Count() + " alleles");
            }
            return rez;
        }


        /**
         * Builds permutations for given variants based on phasing.
         */
        private static HashSet<String> generatePermutations(List<SampleAllele> sampleAlleles, int position,Boolean firstAllele, String alleleSoFar)
        {

            if (position >= sampleAlleles.Count())
            {
                return new HashSet<String>(new List<string> { alleleSoFar });
            }
            SampleAllele allele = sampleAlleles.ElementAt(position);

            HashSet<String> alleles = new HashSet<String>();
            if (allele.isPhased())
            {
                foreach (var item in generatePermutations(sampleAlleles, position + 1, firstAllele, AppendAllele(alleleSoFar, allele, firstAllele)))
                {
                    alleles.Add(item);
                }
            }
            else
            {
                foreach (var item in generatePermutations(sampleAlleles, position + 1, firstAllele, AppendAllele(alleleSoFar, allele, true)))
                {
                    alleles.Add(item);
                }
                foreach (var item in generatePermutations(sampleAlleles, position + 1, firstAllele, AppendAllele(alleleSoFar, allele, false)))
                {
                    alleles.Add(item);
                }
                //alleles.addAll(generatePermutations(sampleAlleles, position + 1, firstAllele, appendAllele(alleleSoFar, allele, false)));
            }
            return alleles;
        }

        private static String AppendAllele(String alleleSoFar, SampleAllele allele, Boolean firstAllele)
        {
            StringBuilder sb = new StringBuilder()
                .Append(alleleSoFar)
                .Append(allele.getPosition())
                .Append(":");
            if (firstAllele)
            {
                sb.Append(allele.getAllele1());
            }
            else
            {
                sb.Append(allele.getAllele2());
            }
            sb.Append(";");
            return sb.ToString();
        }


        public static List<List<T>> generatePerfectPairs<T>(ICollection<T> data)
        {

            List<T> list;
            if (data is List<T>)
            {
                //noinspection unchecked
                list = (List<T>)data;
            }
            else
            {
                list = new List<T>(data);
            }
            List<List<T>> rez = new List<List<T>>();
            for (int x = 0; x < list.Count(); x++)
            {
                for (int y = x; y < list.Count(); y++)
                {
                    rez.Add(new List<T> { list.ElementAt(x), list.ElementAt(y) });
                }
            }
            return rez;
        }
    }
}
