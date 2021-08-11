using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PGx.KB.Vcf.model
{
    public interface VcfLineParser
    {

        void parseLine(VcfMetadata metadata, VcfPosition position, List<VcfSample> sampleData);
    }
}