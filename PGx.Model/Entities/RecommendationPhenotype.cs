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
    
    public partial class RecommendationPhenotype
    {
        public int ID { get; set; }
        public string Gene { get; set; }
        public string Phenotype { get; set; }
        public int DosingGuidenceID { get; set; }
    
        public virtual DosingGuidence DosingGuidence { get; set; }
    }
}
