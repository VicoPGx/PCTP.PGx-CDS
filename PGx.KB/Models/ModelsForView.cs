using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PGx.Model.Entities;

namespace PGx.KB.Models
{
    public class ReportViewModel
    {
        public DosingGuidence DosingGuidance { get; set; }
        public List<GeneCall> CallList { get; set; }
        public string Genotype { get; set; }
        public string Phenotype { get; set; }
    }
}