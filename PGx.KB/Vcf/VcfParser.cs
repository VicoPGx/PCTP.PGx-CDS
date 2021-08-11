using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using PGx.KB.Vcf.model;
using PGx.KB.Infrastructure;
using System.Text.RegularExpressions;

namespace PGx.KB.Vcf
{
    public class VcfParser
    {
        private static Logger sf_logger = new Logger();
        private static String sf_tabSplitter = "\t";
        private static String sf_commaSplitter = ",";
        private static String sf_colonSplitter = ":";
        private static String sf_semicolonSplitter = ";";

        private Boolean m_rsidsOnly;
        private StreamReader m_streamReader;
        private VcfMetadata m_vcfMetadata;
        private IvcfLineParser m_vcfLineParser;

        private int m_lineNumber;
        private Boolean m_alreadyFinished;


        public VcfParser(StreamReader reader, Boolean rsidsOnly, IvcfLineParser lineParser)
        {
            m_streamReader = reader;
            m_rsidsOnly = rsidsOnly;
            m_vcfLineParser = lineParser;
        }


        //this parseMetadata() overload method used to parse the vcf metadata(strated with "##") and columnInfo(started with "#")
        public VcfMetadata ParseVcfMetadata()
        {

            if (m_vcfMetadata != null)
            {
                throw new Exception("Metadata has already been parsed.");
            }
            VcfMetadata.MetadataBuilder mdBuilder = new VcfMetadata.MetadataBuilder();
            String line;
            while ((line = m_streamReader.ReadLine()) != null)
            {
                m_lineNumber++;
                if (line.StartsWith("##"))
                {
                    try
                    {
                        ParseMetadata(mdBuilder, line);
                    }
                    catch (SystemException e)
                    {
                        throw new ArgumentException("Error parsing metadata on line #" + m_lineNumber + ": " + line, e);
                    }
                }
                else if (line.StartsWith("#"))
                {
                    try
                    {
                        ParseColumnInfo(mdBuilder, line);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error parsing column (# header) on line #" + m_lineNumber + ": " + line, e);
                    }
                    break;
                }
            }
            m_vcfMetadata = mdBuilder.build();

            // check sample lists
            if (m_vcfMetadata.getNumSamples() == m_vcfMetadata.getSamples().Count())
            {
                for (int i = 0; i < m_vcfMetadata.getNumSamples(); i++)
                {
                    String sampleName = m_vcfMetadata.getSampleName(i);
                    if (!m_vcfMetadata.getSamples().Keys.Contains(sampleName))
                    {
                        sf_logger.warn("Sample {} is missing in the metadata", sampleName);
                    }
                }
            }
            else
            {
                sf_logger.warn("There are {} samples in the header but {} in the metadata", m_vcfMetadata.getNumSamples().ToString(),
                    m_vcfMetadata.getSamples().Count().ToString());
            }

            return m_vcfMetadata;
        }

        private void ParseColumnInfo(VcfMetadata.MetadataBuilder mdBuilder, String line)
        {

            mdBuilder.SetColumns(line.Split('\t').ToList());
        }

        private void ParseMetadata(VcfMetadata.MetadataBuilder mdBuilder, String line)
        {

            int idx = line.IndexOf("=");
            String propName = line.Substring(2, idx - 2).Trim();
            String propValue = line.Substring(idx + 1).Trim();



            switch (propName)
            {
                case "fileformat":
                    mdBuilder.setFileFormat(propValue);
                    break;

                case "ALT":
                case "FILTER":
                case "INFO":
                case "FORMAT":
                case "contig":
                case "SAMPLE":
                case "PEDIGREE":
                    ParseMetadataProperty(mdBuilder, propName, RemoveAngleBrackets(propValue));
                    break;

                case "assembly":
                case "pedigreeDB":
                default:
                    mdBuilder.addRawProperty(propName, propValue);
                    break;
            }
        }
        private void ParseMetadataProperty(VcfMetadata.MetadataBuilder mdBuilder, String propName, String value)
        {
            Dictionary<String, String> props = VcfUtils.extractPropertiesFromLine(value);
            switch (propName.ToLower())
            {
                case "alt":
                    mdBuilder.addAlt(new IdDescriptionMetadata(props, true));
                    break;
                case "filter":
                    mdBuilder.addFilter(new IdDescriptionMetadata(props, true));
                    break;
                case "info":
                    mdBuilder.addInfo(new InfoMetadata(props));
                    break;
                case "format":
                    mdBuilder.addFormat(new FormatMetadata(props));
                    break;
                case "contig":
                    mdBuilder.addContig(new ContigMetadata(props));
                    break;
                case "sample":
                    mdBuilder.addSample(new IdDescriptionMetadata(props, true));
                    break;
                case "pedigree":
                    mdBuilder.addPedigree(new BaseMetadata(props));
                    break;
            }
        }

        private List<String> ConvertStringToList(String pattern, String inputString)
        {
            String[] array = Regex.Split(inputString, pattern);
            return array.ToList();
        }

        /**
         * Removes double quotation marks around a string.
         * @throws IllegalArgumentException If angle brackets are not present
         */
        private static String RemoveAngleBrackets(String str)
        {
            if (str.StartsWith("<") && str.EndsWith(">"))
            {
                return str.Substring(1, str.Length - 2);
            }
            throw new Exception("Angle brackets not present for: " + str);
        }

        public void ParseVCF()
        {
            Boolean hasNext = true;
            while (hasNext)
            {
                hasNext = ParseNextLine();
            }
            m_streamReader.Close();
        }
        //Parse each raw line read directly from the VCF file.
        public Boolean ParseNextLine()
        {

            if (m_vcfMetadata == null)
            {
                ParseVcfMetadata();
            }

            String line = m_streamReader.ReadLine();
            if (line == null)
            {
                m_alreadyFinished = true;
                return false;
            }

            if (m_alreadyFinished)
            {
                // prevents user errors from causing infinite loops
                throw new InvalidOperationException("Already finished reading the stream");
            }

            m_lineNumber++;
            if (m_lineNumber == 8)
            {
                int aaaaa = 3;
            }
            try
            {

                List<String> data = ConvertStringToList(sf_tabSplitter, line);

                // CHROM
                String chromosome = data.ElementAt(0);

                // POS
                long position;
                try
                {
                    position = long.Parse(data.ElementAt(1));
                }
                catch (FormatException e)
                {
                    throw new ArgumentException("Position " + data.ElementAt(1) + " is not numerical");
                }

                // ID
                List<String> ids = null;
                if (!data.ElementAt(2).Equals("."))
                {
                    if (m_rsidsOnly && !Regex.IsMatch(data.ElementAt(2), VcfUtils.RSID_PATTERN))
                    {

                        return true;
                    }
                    ids = ConvertStringToList(sf_semicolonSplitter, data.ElementAt(2));
                }
                else if (m_rsidsOnly)
                {
                    return true;
                }

                // REF
                String refBase = data.ElementAt(3);

                // ALT
                List<String> alt = null;
                if (!(data.ElementAt(7) == string.Empty) && !(data.ElementAt(4) == "."))
                {
                    alt = ConvertStringToList(sf_commaSplitter, data.ElementAt(4));
                }

                // QUAL
                Decimal quality = 0;
                if (!(data.ElementAt(5) == string.Empty) && !(data.ElementAt(5) == "."))
                {
                    quality = Convert.ToDecimal(data.ElementAt(5));
                }

                // FILTER
                List<String> filters = null;
                if (!data.ElementAt(6).Equals("PASS"))
                {
                    filters = ConvertStringToList(sf_semicolonSplitter, data.ElementAt(6));
                }

                // INFO
                Dictionary<String, List<String>> info = null;
                //ListMultimap<String, String> info = null;
                if (!data.ElementAt(7).Equals("") && !data.ElementAt(7).Equals("."))
                {
                    //info = ArrayListMultimap.create();
                    info = new Dictionary<String, List<String>>();
                    string data7 = data.ElementAt(7);
                    if (data7.Last() == ';')
                    {
                        data7 = data7.Remove(data7.Count() - 1);
                    }
                    List<String> props = ConvertStringToList(sf_semicolonSplitter, data7);
                    foreach (String prop in props)
                    {
                        int idx = prop.IndexOf('=');
                        if (idx == -1)
                        {
                            info.Add(prop, null);
                        }
                        else
                        {
                            String key = prop.Substring(0, idx);
                            String value = prop.Substring(idx + 1);
                            info.Add(key, ConvertStringToList(sf_commaSplitter, value));
                        }
                    }
                }

                // FORMAT
                List<String> format = null;
                if (data.Count() >= 9 && data.ElementAt(8) != null)
                {
                    format = ConvertStringToList(sf_colonSplitter, data.ElementAt(8));
                }

                // samples
                VcfPosition pos = new VcfPosition(chromosome, position, ids, refBase, alt,
                    quality, filters, info, format);
                List<VcfSample> samples = new List<VcfSample>();
                for (int x = 9; x < data.Count(); x++)
                {
                    samples.Add(new VcfSample(format, ConvertStringToList(sf_colonSplitter, (data.ElementAt(x)))));
                }

                m_vcfLineParser.ProcessRawLine(m_vcfMetadata, pos, samples);

                //m_lineNumber++;

            }
            catch (Exception e)
            {
                throw new Exception("Error parsing VCF data line #" + m_lineNumber + ": " + line, e);
            }
            return true;
        }


        public class VcfParserBuilder
        {
            private StreamReader m_streamReader;
            private String m_vcfFile;
            //private Path m_vcfFile;
            private Boolean m_rsidsOnly;
            private IvcfLineParser m_vcfLineParser;



            //public VcfParserBuilder fromFile(String dataFile)
            //{

            //    if (m_streamReader != null)
            //    {
            //        throw new InvalidOperationException("Already loading from reader");
            //    }
            //    if (!dataFile.ToString().EndsWith(".vcf"))
            //    {
            //        throw new ArgumentException("Not a VCF file (doesn't end with .vcf extension");
            //    }
            //    m_vcfFile = dataFile;
            //    return this;
            //}


            public VcfParserBuilder SetStreamReader(StreamReader streamReader)
            {

                if (m_vcfFile != null)
                {
                    throw new InvalidOperationException("Already loading from file");
                    //throw new IllegalStateException("Already loading from file");
                }
                m_streamReader = streamReader;
                return this;
            }

            /**
             * Tells parser to ignore data lines that are not associated with an RSID.
             */
            public VcfParserBuilder rsidsOnly()
            {
                m_rsidsOnly = true;
                return this;
            }

            public VcfParserBuilder InstanceLineParserInterface(IvcfLineParser lineParser)
            {
                //Preconditions.checkNotNull(lineParser);
                m_vcfLineParser = lineParser;
                return this;
            }


            public VcfParser BuildVcfParser()
            {
                if (m_vcfLineParser == null)
                {
                    //throw new IllegalStateException("Missing VcfLineParser")
                    throw new System.InvalidOperationException("Missing VcfLineParser");
                }
                if (m_vcfFile != null)
                {
                    m_streamReader = new StreamReader(m_vcfFile.ToString());
                    //m_reader = Files.newBufferedReader(m_vcfFile);
                }
                if (m_streamReader == null)
                {
                    throw new InvalidOperationException("Must specify either file or reader to parse");
                }
                return new VcfParser(m_streamReader, m_rsidsOnly, m_vcfLineParser);
            }
        }

    }
}