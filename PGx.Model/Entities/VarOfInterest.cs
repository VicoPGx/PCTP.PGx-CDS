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
    
    public partial class VarOfInterest
    {
        public int ID { get; set; }
        public int Position { get; set; }
        public string Rsid { get; set; }
        public string VcfCall { get; set; }
        public Nullable<bool> IsPhased { get; set; }
        public Nullable<int> VcfPosition { get; set; }
        public string VcfAlleles { get; set; }
        public int GeneCallID { get; set; }
    
        public virtual GeneCall GeneCall { get; set; }
    }
}
