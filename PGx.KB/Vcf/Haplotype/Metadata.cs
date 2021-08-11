using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PGx.KB.Vcf.Haplotype
{
    public class Metadata
    {

        public String NamedAlleleMatcherVersion
        {
            get;
            set;
        }

        public String GenomeBuild
        {
            get;
            set;
        }

        public String InputFilename
        {
            get;
            set;
        }

        public DateTime Timetamp
        {
            get;
            set;
        }

        public Metadata(String namedAlleleMatcherVersion, String genomeBuild, String vcfFilename, DateTime date)
        {
            NamedAlleleMatcherVersion = namedAlleleMatcherVersion;
            GenomeBuild = genomeBuild;
            InputFilename = vcfFilename;
            Timetamp = date;
        }
    }
}