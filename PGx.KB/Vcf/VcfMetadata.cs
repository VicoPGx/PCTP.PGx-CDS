using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using PGx.KB.Vcf.model;
namespace PGx.KB.Vcf
{
    public class VcfMetadata
    {
        private String m_fileFormat;
        private Dictionary<String, IdDescriptionMetadata> m_alt;
        private Dictionary<String, InfoMetadata> m_info;
        private Dictionary<String, IdDescriptionMetadata> m_filter;
        private Dictionary<String, FormatMetadata> m_format;
        private List<String> m_columns;
        private Dictionary<String, List<String>> m_properties;
        private Dictionary<String, ContigMetadata> m_contig;
        private Dictionary<String, IdDescriptionMetadata> m_sample;
        private List<BaseMetadata> m_pedigree;

        private VcfMetadata(String fileFormat, Dictionary<String, IdDescriptionMetadata> alt,
       Dictionary<String, InfoMetadata> info, Dictionary<String, IdDescriptionMetadata> filter,
       Dictionary<String, FormatMetadata> format, Dictionary<String, ContigMetadata> contig,
       Dictionary<String, IdDescriptionMetadata> sample, List<BaseMetadata> pedigree,
       List<String> columns, Dictionary<String, List<String>> properties)
        {
            m_fileFormat = fileFormat;
            m_alt = alt == null ? new Dictionary<String, IdDescriptionMetadata>() : alt;
            m_info = info == null ? new Dictionary<String, InfoMetadata>() : info;
            m_filter = filter == null ? new Dictionary<String, IdDescriptionMetadata>() : filter;
            m_format = format == null ? new Dictionary<String, FormatMetadata>() : format;
            m_contig = contig == null ? new Dictionary<String, ContigMetadata>() : contig;
            m_sample = sample == null ? new Dictionary<String, IdDescriptionMetadata>() : sample;
            m_pedigree = pedigree == null ? new List<BaseMetadata>() : pedigree;
            m_properties = properties == null ? new Dictionary<String, List<String>>() : properties;
            m_columns = columns;
        }
        public String getFileFormat()
        {
            return m_fileFormat;
        }

        public void setFileFormat(String fileFormat)
        {
            if (!Regex.IsMatch(fileFormat, VcfUtils.FILE_FORMAT_PATTERN))
            //(!VcfUtils.FILE_FORMAT_PATTERN.matcher(fileFormat).matches()) 
            {
                throw new ArgumentException("VCF format must look like ex: VCFv4.2; was " + fileFormat);
            }
            m_fileFormat = fileFormat;
        }

        public Dictionary<String, IdDescriptionMetadata> getAlts()
        {
            return m_alt;
        }

        /**
         * Gets the ALT metadata for the given ID.
         *
         * @param id the ID to lookup, will unwrap ID's enclosed in angle brackets (e.g. &lt;CN1&gt; will get converted to CN1)
         */

        public IdDescriptionMetadata getAlt(String id)
        {
            IdDescriptionMetadata md;
            m_alt.TryGetValue(id, out md);
            if (md == null && id.StartsWith("<") && id.EndsWith(">"))
            {
                m_alt.TryGetValue(id.Substring(1, id.Length - 1), out md);
            }
            return md;
        }


        public Dictionary<String, InfoMetadata> getInfo()
        {
            return m_info;
        }

        public Dictionary<String, IdDescriptionMetadata> getFilters()
        {
            return m_filter;
        }

        public Dictionary<String, FormatMetadata> getFormats()
        {
            return m_format;
        }

        public Dictionary<String, ContigMetadata> GetContigs
        {
            get { return m_contig; }
        }

        public List<BaseMetadata> getPedigrees()
        {
            return m_pedigree;
        }

        public Dictionary<String, IdDescriptionMetadata> getSamples()
        {
            return m_sample;
        }

        /**
         * @return The URLs from the field(s) in the <em>assembly</em> metadata line(s)
         */
        public List<String> getAssemblies()
        {
            // spec says: ##assembly=url (without angle brackets)
            List<string> list;
            m_properties.TryGetValue("assembly", out list);
            return list;
        }

        /**
         * @return The URLs from the field(s) in the <em>pedigreeDB</em> metadata line(s), including angle brackets if any
         */
        public List<String> getPedigreeDatabases()
        {
            // spec says: ##pedigreeDB=<url> (with angle brackets)
            List<string> list;
            m_properties.TryGetValue("pedigreeDB", out list);
            return list;
        }

        /**
         * Adds {@code value} to the map of ALT metadata, using its {@link IdDescriptionMetadata#getId() ID} as the key.
         */
        public void addAlt(IdDescriptionMetadata value)
        {
            m_alt.Add(value.getId(), value);
        }

        /**
         * Adds {@code value} to the map of INFO metadata, using its {@link InfoMetadata#getId() ID} as the key.
         */
        public void addInfo(InfoMetadata value)
        {
            m_info.Add(value.getId(), value);
        }

        /**
         * Adds {@code value} to the map of FORMAT metadata, using its {@link FormatMetadata#getId() ID} as the key.
         */
        public void addFormat(FormatMetadata value)
        {
            m_format.Add(value.getId(), value);
        }

        /**
         * Adds {@code value} to the map of CONTIG metadata, using its {@link ContigMetadata#getId() ID} as the key.
         */
        public void addContig(ContigMetadata value)
        {
            m_contig.Add(value.getId(), value);
        }

        /**
         * Adds {@code value} to the map of FILTER metadata, using its {@link IdDescriptionMetadata#getId() ID} as the key.
         */
        public void addFilter(IdDescriptionMetadata value)
        {
            m_filter.Add(value.getId(), value);
        }

        /**
         * Adds {@code value} to the list of assembly metadata.
         * @param value Should not be wrapped in angle brackets
         */
        public void addAssembly(List<String> value)
        {
            m_properties.Add("assembly", value);
        }

        /**
         * Adds {@code value} to the list of pedigreeDB.
         * @param value Must be wrapped in angle brackets
         * @throws IllegalArgumentException If {@code value} is not wrapped in angle brackets
         */
        public void addPedigreeDatabase(List<String> value)
        {
            //if (value.StartsWith("<") && value.EndsWith(">")) {
            m_properties.Add("pedigreeDB", value);
            //} else {
            //  throw new IllegalArgumentException("pedigreeDB string " + value + " should be enclosed in angle brackets according to spec");
        }


        public void removeAlt(IdDescriptionMetadata value)
        {
            m_alt.Remove(value.getId());
        }

        public void removeInfo(InfoMetadata value)
        {
            m_info.Remove(value.getId());
        }

        public void removeFormat(FormatMetadata value)
        {
            m_format.Remove(value.getId());
        }

        public void removeContig(ContigMetadata value)
        {
            m_contig.Remove(value.getId());
        }

        public void removeFilter(IdDescriptionMetadata value)
        {
            m_filter.Remove(value.getId());
        }

        public void removeAssembly(String value)
        {
            List<string> list;
            var a = m_properties.TryGetValue("assembly", out list);
            m_properties.Remove("assembly");
            list.Remove(value);
            if (list.Count() > 0)
            {
                m_properties.Add("assembly", list);
            }
        }

        /**
         * Adds {@code value} to the list of pedigreeDB.
         * @param value Must be wrapped in angle brackets
         * @throws IllegalArgumentException If {@code value} is not wrapped in angle brackets
         */
        public void removePedigreeDb(String value)
        {
            //if (value.startsWith("<") && value.endsWith(">")) {
            List<string> list;
            var a = m_properties.TryGetValue("pedigreeDB", out list);
            m_properties.Remove("pedigreeDB");
            list.Remove(value);
            m_properties.Add("pedigreeDB", list);

            //} else { // be strict to avoid needing to delete both value and <value>
            //  throw new IllegalArgumentException("pedigreeDB string " + value + " should be enclosed in angle brackets according to spec");
            //}
        }

        /**
         * Returns a map from every property key to each of its values.
         * Call {@link ListMultimap#asMap} to get a Map&lt;String, Collection&lt;String&gt;&gt;.
         * @return <em>Contains every property except those contained in:</em>
         * <ul>
         *   <li>{@link #getInfo}</li>
         *   <li>{@link #getFilters}</li>
         *   <li>{@link #getFormats}</li>
         *   <li>{@link #getContigs}</li>
         *   <li>{@link #getPedigrees}</li>
         *   <li>{@link #getInfo}</li>
         *   <li>{@link #getSamples}</li>
         * </ul>
         * However, contains any in {@link #getAssemblies} and {@link #getPedigreeDatabases}.
         */
        public Dictionary<String, List<String>> getRawProperties()
        {
            return m_properties;
        }

        /**
         * Returns the value of a property, or null if the property is not set or has no value.
         * <strong>This method will return null for a reserved property of the form XX=&lt;ID=value,ID=value,...&gt;;
         * {@code assembly} and {@code pedigreeDB} are still included.</strong>
         */
        public List<String> getRawValuesOfProperty(String propertyKey)
        {
            List<string> list;
            m_properties.TryGetValue(propertyKey, out list);
            return list;
        }

        /**
         * Returns a list of the properties defined.
         * <strong>Reserved properties of the form XX=&lt;ID=value,ID=value,...&gt; are excluded, though {@code assembly}
         * and {@code pedigreeDB} are still included.</strong>
         * @return <em>Contains every property except those contained in:</em>
         * <ul>
         *   <li>{@link #getInfo}</li>
         *   <li>{@link #getFilters}</li>
         *   <li>{@link #getFormats}</li>
         *   <li>{@link #getContigs}</li>
         *   <li>{@link #getPedigrees}</li>
         *   <li>{@link #getInfo}</li>
         *   <li>{@link #getSamples}</li>
         * </ul>
         * However, contains any in {@link #getAssemblies} and {@link #getPedigreeDatabases}.
         */
        public List<String> getRawPropertyKeys()
        {
            return m_properties.Keys.ToList();
        }

        public int getColumnIndex(String column)
        {
            return m_columns.IndexOf(column);
        }

        /**
         * Sample numbering starts at 0.
         */
        public int getSampleIndex(String sampleId)
        {
            return m_columns.IndexOf(sampleId) - 9;
        }

        /**
         * Gets the number of samples in the VCF file.
         */
        public int getNumSamples()
        {
            if (m_columns.Count() < 9)
            {
                return 0; // necessary because if we have no samples, we'll be missing FORMAT
            }
            return m_columns.Count() - 9;
        }

        /**
         * Gets the sample name (column name).
         *
         * @param idx sample index, first sample is at index 0
         *
         * @throws ArrayIndexOutOfBoundsException If the sample doesn't exist
         */
        public String getSampleName(int idx)
        {
            return m_columns.ElementAt(9 + idx);
        }



        public class MetadataBuilder
        {
            private String m_fileFormat;
            private Dictionary<String, IdDescriptionMetadata> m_alt = new Dictionary<string, IdDescriptionMetadata>();
            private Dictionary<String, InfoMetadata> m_info = new Dictionary<String, InfoMetadata>();
            private Dictionary<String, IdDescriptionMetadata> m_filter = new Dictionary<String, IdDescriptionMetadata>();
            private Dictionary<String, FormatMetadata> m_format = new Dictionary<String, FormatMetadata>();
            private Dictionary<String, ContigMetadata> m_contig = new Dictionary<String, ContigMetadata>();
            private Dictionary<String, IdDescriptionMetadata> m_sample = new Dictionary<String, IdDescriptionMetadata>();
            private List<BaseMetadata> m_pedigree = new List<BaseMetadata>();
            private List<String> m_columns = new List<String>();
            private Dictionary<String, List<String>> m_properties = new Dictionary<String, List<String>>();

            public MetadataBuilder setFileFormat(String fileFormat)
            {
                m_fileFormat = fileFormat;
                if (!Regex.IsMatch(fileFormat, VcfUtils.FILE_FORMAT_PATTERN))
                {
                    throw new Exception("Not a VCF file: fileformat is " + m_fileFormat);
                }
                return this;
            }

            public MetadataBuilder addAlt(IdDescriptionMetadata md)
            {
                if (m_alt.ContainsKey(md.getId()))
                {
                    throw new ArgumentException("Duplicate ID " + md.getId() + " for ALT");
                }
                m_alt.Add(md.getId(), md);
                return this;
            }

            public MetadataBuilder addInfo(InfoMetadata md)
            {
                if (m_info.ContainsKey(md.getId()))
                {
                    throw new ArgumentException("Duplicate ID " + md.getId() + " for INFO");
                }
                m_info.Add(md.getId(), md);
                return this;
            }

            public MetadataBuilder addFilter(IdDescriptionMetadata md)
            {
                if (m_filter.ContainsKey(md.getId()))
                {
                    throw new ArgumentException("Duplicate ID " + md.getId() + " for FILTER");
                }
                m_filter.Add(md.getId(), md);
                return this;
            }

            public MetadataBuilder addFormat(FormatMetadata md)
            {
                if (m_format.ContainsKey(md.getId()))
                {
                    throw new ArgumentException("Duplicate ID " + md.getId() + " for FORMAT");
                }
                m_format.Add(md.getId(), md);
                return this;
            }

            public MetadataBuilder addContig(ContigMetadata md)
            {
                if (m_contig.ContainsKey(md.getId()))
                {
                    throw new ArgumentException("Duplicate ID " + md.getId() + " for CONTIG");
                }
                m_contig.Add(md.getId(), md);
                return this;
            }

            public MetadataBuilder addSample(IdDescriptionMetadata md)
            {
                if (m_sample.ContainsKey(md.getId()))
                {
                    throw new ArgumentException("Duplicate ID " + md.getId() + " for SAMPLE");
                }
                m_sample.Add(md.getId(), md);
                return this;
            }

            public MetadataBuilder addPedigree(BaseMetadata md)
            {
                m_pedigree.Add(md);
                return this;
            }

            public MetadataBuilder addRawProperty(String name, String value)
            {
                List<string> list;
                
                Boolean flag=m_properties.TryGetValue(name, out list);
                if (!flag == true)
                {
                    list = new List<string>();
                    list.Add(value);
                    m_properties.Add(name, list);
                }
                else
                {
                    m_properties.Remove(name);
                    list.Add(value);
                    m_properties.Add(name, list);
                }
                return this;
            }
            public MetadataBuilder SetColumns(List<String> cols)
            {
                m_columns = cols;
                return this;
            }

            public VcfMetadata build()
            {
                if (m_fileFormat == null)
                {
                    throw new InvalidOperationException("Not a VCF file: no ##fileformat line");
                    //throw new IllegalStateException("Not a VCF file: no ##fileformat line");
                }
                return new VcfMetadata(m_fileFormat, m_alt, m_info, m_filter, m_format, m_contig, m_sample, m_pedigree,
                    m_columns, m_properties);
            }

        }
    }
}