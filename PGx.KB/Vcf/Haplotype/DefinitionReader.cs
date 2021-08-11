using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using PGx.Model.Entities;

namespace PGx.KB.Vcf.Haplotype
{
    public class DefinitionReader
    {

        private SortedDictionary<String, DefinitionFile> m_definitionFiles = new SortedDictionary<String, DefinitionFile>();
        private Dictionary<String, DefinitionExemption> m_exemptions = new Dictionary<string, DefinitionExemption>();
        private String m_genomeBuild;

        public String getGenomeBuild()
        {

            if (m_genomeBuild == null)
            {
                foreach (DefinitionFile definitionFile in m_definitionFiles.Values)
                {
                    if (definitionFile.GenomeBuild == null)
                    {
                        definitionFile.GenomeBuild = "b38";
                    }
                    if (m_genomeBuild == null)
                    {
                        m_genomeBuild = definitionFile.GenomeBuild;
                    }
                    else if (!m_genomeBuild.ToLower().Equals(definitionFile.GenomeBuild.ToLower()))
                    {
                        throw new InvalidOperationException("Genes definition use different genome builds (" + m_genomeBuild + " vs " +
                            definitionFile.GenomeBuild + " for " + definitionFile.GeneSymbol + ")");
                    }
                }
            }
            return m_genomeBuild;
        }


        public HashSet<String> GetGenes()
        {
            return new HashSet<String>(m_definitionFiles.Keys.OrderBy(x => x));
        }


        public DefinitionFile GetDefinitionFile(String gene)
        {
            return m_definitionFiles.Where(x => x.Key == gene).FirstOrDefault().Value;
        }


        public IEnumerable<VariantLocus> GetPositions(String gene)
        {
            return m_definitionFiles[gene].VariantLocus;
        }


        public List<NamedAllele> GetNamedAlleles(String gene)
        {

            return m_definitionFiles[gene].NamedAllele.ToList();

        }

        public DefinitionExemption GetExemption(String gene)
        {
            return m_exemptions.Where(x => x.Key.ToLower() == gene.ToLower()).FirstOrDefault().Value;
        }


        public void read(PGx_KBEntities context)
        {
            //读取定义
            context.DefinitionFile.ToList().ForEach(definitionFile =>
            {
                foreach (NamedAllele namedAllele in definitionFile.NamedAllele)
                {
                    //对PGx_KB中读取的NamedAllele定义(Haplotype)进行初始化：1.计算每个NamedAllele的分数；2.根据br38 position顺序进行排列（permutation）
                    namedAllele.initialize();

                }
                m_definitionFiles.Add(definitionFile.GeneSymbol.Trim(), definitionFile);
            });
            //读取豁免
            context.DefinitionExemption.ToList().ForEach(x =>
                {
                    m_exemptions.Add(x.Gene.Trim(), x);
                });
        }

        public void LocusSet(DefinitionFile definitionFile, DefinitionFile file)
        {
            definitionFile.VariantLocus = file.VariantLocus.ToArray();
        }
    }
}

