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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Contract()
        {
            this.ReceiptLines = new HashSet<ReceiptLine>();
        }
    
        public int RowID { get; set; }
        public string ContractCode { get; set; }
        public string ContractType { get; set; }
        public Nullable<long> ContractPrice { get; set; }
        public Nullable<System.DateTime> SignDate { get; set; }
        public Nullable<int> RowIDEmployee { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> RowIDBuilding { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<int> RowIDEmployeeCreated { get; set; }
        public Nullable<int> RowIDEmployeeEdited { get; set; }
    
        public virtual Building Building { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Employee Employee1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReceiptLine> ReceiptLines { get; set; }
    }
}
