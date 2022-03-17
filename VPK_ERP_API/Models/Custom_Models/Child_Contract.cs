using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VPK_ERP_API.Models.Custom_Models
{
    public class Child_Contract : Contract
    {

        public string ContractCodeAndType { get; set; }


        public int? RowID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Contract_Code { get; set; }
        public string Contract_Type { get; set; }
        public string Address { get; set; }
        public string CreatedDate { get; set; }
        public long? TotalContractPrice { get; set; }
        public long? TotalRealIncome { get; set; }
        public int? RowIDContract { get; set; }





    }
}