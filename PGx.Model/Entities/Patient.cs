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
    
    public partial class Patient
    {
        public int ID { get; set; }
        public string PatientID { get; set; }
        public string Name { get; set; }
        public Nullable<bool> Sex { get; set; }
        public Nullable<int> Age { get; set; }
        public string Race { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public Nullable<int> Height { get; set; }
        public Nullable<System.DateTime> BirthDay { get; set; }
        public string Nation { get; set; }
        public Nullable<System.DateTime> TimeStamp { get; set; }
    }
}