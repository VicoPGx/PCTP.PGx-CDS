//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace PGx.Model.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class PGxGuideline
    {
        public PGxGuideline()
        {
            this.DosingGuidence = new HashSet<DosingGuidence>();
            this.Literature = new HashSet<Literature>();
            this.RelatedGene = new HashSet<RelatedGene>();
            this.DefinitionFile = new HashSet<DefinitionFile>();
        }
    
        public int ID { get; set; }
        public string M_id { get; set; }
        public string Source { get; set; }
        public string Summary { get; set; }
        public string GenesInStr { get; set; }
        public string Chemical { get; set; }
        public Nullable<int> ChemicalDictID { get; set; }
        public string ClinicalImplication { get; set; }
        public string IsAvailable { get; set; }
    
        public virtual ICollection<DosingGuidence> DosingGuidence { get; set; }
        public virtual ICollection<Literature> Literature { get; set; }
        public virtual ICollection<RelatedGene> RelatedGene { get; set; }
        public virtual ICollection<DefinitionFile> DefinitionFile { get; set; }
    }
}
