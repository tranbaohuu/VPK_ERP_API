//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VPK_ERP_API.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Contract
    {
        public int RowID { get; set; }
        public string ContractCode { get; set; }
        public string ContractType { get; set; }
        public Nullable<long> ContractPrice { get; set; }
        public Nullable<System.DateTime> SignDate { get; set; }
        public Nullable<int> RowIDEmployee { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> RowIDBuilding { get; set; }
    
        public virtual Building Building { get; set; }
    }
}
