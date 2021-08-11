using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PGx.KB.Vcf.Haplotype.Model;
using System.IO;
using PGx.Model.Entities;
using PGx.Model.Entities.Data;
namespace PGx.KB.Vcf.Haplotype
{
    public class ResultBuilder
    {
        private DefinitionReader m_definitionReader;
        private Result m_result = new Result();
        private String m_dateFormat = "MM/dd/yy";
        //private SimpleDateFormat m_dateFormat = new SimpleDateFormat("MM/dd/yy");


        public ResultBuilder(DefinitionReader definitionReader)
        {
            m_definitionReader = definitionReader;
        }

        public Result GetResult
        {
            get { return m_result; }
        }

        public ResultBuilder SetMetadata(string vcfFile, Dictionary<String, List<String>> warnings)
        {
            m_result.Metadata = new Metadata(NamedAlleleMatcher.VERSION, m_definitionReader.getGenomeBuild(), Path.GetFileName(vcfFile.ToString()), new DateTime());

            if (warnings != null)
            {
                m_result.VcfWarnings = warnings;
            }
            return this;
        }


        public ResultBuilder MakeGeneCall(String gene, MatchData matchData, List<DiplotypeMatch> matches)
        {
            DefinitionFile geneDefinition = m_definitionReader.GetDefinitionFile(gene);
            String definitionVersion = geneDefinition.ModificationDate.ToString(m_dateFormat);
            String chromosome = geneDefinition.Chromosome;

            HashSet<String> matchableHaps = new HashSet<string>(matchData.GetNamedAlleles.Select(x => x.Name).ToList());

            HashSet<String> uncallableHaplotypes = new HashSet<string>(m_definitionReader.GetNamedAlleles(gene)
                .Select(x => x.Name)
                .Where(n => !matchableHaps.Contains(n)));

            HashSet<String> ignoredHaplotypes;
            DefinitionExemption exemption = m_definitionReader.GetExemption(gene);
            if (exemption != null)
            {
                uncallableHaplotypes = new HashSet<string>(uncallableHaplotypes.Where(h => !exemption.shouldIgnore(h)));
                ignoredHaplotypes = new HashSet<string>(exemption.IgnoredAllelesSorted
                    .Select(x => x.ToUpper()));
            }
            else
            {
                ignoredHaplotypes = new HashSet<String>();
            }

            GeneCall geneCall = new GeneCall(definitionVersion, chromosome, gene, matchData, uncallableHaplotypes,
                ignoredHaplotypes);

            if (matches != null)
            {
                // get haplotype/diplotype info
                foreach (DiplotypeMatch dm in matches)
                {
                    geneCall.AddDiplotype(dm);
                }
            }

            // get position info
            foreach (VariantLocus variant in matchData.GetPositions)
            {
                geneCall.AddVariant(new Variant(variant, matchData.getSampleAllele(variant.getPosition())));
            }

            m_result.AddGeneCall(geneCall);

            return this;
        }
    }
}