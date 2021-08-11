using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PGx.KB.Vcf.model;

namespace PGx.KB.Vcf
{
   
        public interface IvcfLineParser
        {
            void ProcessRawLine(VcfMetadata metadata, VcfPosition position, List<VcfSample> sampleData);
        }

}