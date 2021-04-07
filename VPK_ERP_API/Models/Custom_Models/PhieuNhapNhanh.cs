using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VPK_ERP_API.Models.Custom_Models
{
    public class PhieuNhapNhanh
    {

        public string NgayNhap { get; set; }
        public string HangMuc { get; set; }
        public string NhomSanPham { get; set; }
        public string NhaCungCap { get; set; }
        public int RowIDCongTrinh { get; set; }
        public int RowIDCongTrinhThamChieu { get; set; }
        public string DonVi { get; set; }
        public long GiaTien { get; set; }
        public double SoLuong { get; set; }
        public long ThanhTien { get; set; }
        public string TinhTrang { get; set; }
        public string GhiChu { get; set; }
        public int EmployeeID { get; set; }

        public int RowIDHopDong { get; set; }
        public int ReceiptType { get; set; }





    }
}