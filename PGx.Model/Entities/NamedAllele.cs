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
    
    public partial class NamedAllele
    {
        public NamedAllele()
        {
            this.NamedAlleleDefinition = new HashSet<NamedAlleleDefinition>();
            this.PopulationFrequency = new HashSet<PopulationFrequency>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string M_Id { get; set; }
        public string M_Function { get; set; }
        public Nullable<int> DefinitionFileID { get; set; }
        public bool IsRefAllele { get; set; }
        public Nullable<double> ActivityValue { get; set; }
        
    
        public virtual DefinitionFile DefinitionFile { get; set; }
        public virtual ICollection<NamedAlleleDefinition> NamedAlleleDefinition { get; set; }
        public virtual ICollection<PopulationFrequency> PopulationFrequency { get; set; }
    }
}
