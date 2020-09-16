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
    
    public partial class ReceiptHeader
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ReceiptHeader()
        {
            this.ReceiptLines = new HashSet<ReceiptLine>();
        }
    
        public int RowID { get; set; }
        public Nullable<int> RowIDBuilding { get; set; }
        public Nullable<int> RowIDCustomer { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> EditedDate { get; set; }
        public Nullable<int> RowIDEmployeeCreated { get; set; }
        public Nullable<int> RowIDEmployeeEdited { get; set; }
        public Nullable<int> Type { get; set; }
    
        public virtual Building Building { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Employee Employee1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReceiptLine> ReceiptLines { get; set; }
    }
}
