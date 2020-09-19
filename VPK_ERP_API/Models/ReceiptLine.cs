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
    
    public partial class ReceiptLine
    {
        public int RowID { get; set; }
        public Nullable<int> RowIDReceiptHeader { get; set; }
        public Nullable<int> RowIDContract { get; set; }
        public string Description { get; set; }
        public string Times { get; set; }
        public Nullable<long> TotalPrice { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> EditedDate { get; set; }
        public Nullable<int> RowIDEmployeeCreated { get; set; }
        public Nullable<int> RowIDEmployeeEdited { get; set; }
        public string Category { get; set; }
        public string Item { get; set; }
        public string Supplier { get; set; }
        public string Unit { get; set; }
        public Nullable<long> UnitPrice { get; set; }
        public Nullable<double> Quantity { get; set; }
        public string Status { get; set; }
    
        public virtual Contract Contract { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Employee Employee1 { get; set; }
        public virtual ReceiptHeader ReceiptHeader { get; set; }
    }
}