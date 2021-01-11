using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VPK_ERP_API.Models.Custom_Models
{
    public class PhieuChiThu : ReceiptLine
    {



        public string Code { get; set; }


        public string DescriptionReceiptHeader { get; set; }

        public int RowIDBuilding { get; set; }

        public int Type { get; set; }

        public List<ReceiptLine> DanhSachChiTietPhieuThu { get; set; }

        public string NgayNhap { get; set; }


    }
}