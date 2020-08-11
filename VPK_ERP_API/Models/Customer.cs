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
    
    public partial class Customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Customer()
        {
            this.Customer_Building = new HashSet<Customer_Building>();
            this.ReceiptHeaders = new HashSet<ReceiptHeader>();
        }
    
        public string FullName { get; set; }
        public string FullNameEnglish { get; set; }
        public Nullable<int> Sex { get; set; }
        public Nullable<System.DateTime> BirthDay { get; set; }
        public Nullable<int> Age { get; set; }
        public string Address { get; set; }
        public Nullable<int> WardID { get; set; }
        public Nullable<int> DistrictID { get; set; }
        public Nullable<int> CityID { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> EditedDate { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public string TaxCode { get; set; }
        public string IDCardNo { get; set; }
        public Nullable<System.DateTime> DateOfIssue { get; set; }
        public string PlaceOfIssue { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public int RowID { get; set; }
        public Nullable<int> RowIDEmployeeCreated { get; set; }
        public Nullable<int> RowIDEmployeeEdited { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Customer_Building> Customer_Building { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Employee Employee1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReceiptHeader> ReceiptHeaders { get; set; }
    }
}
