using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VPK_ERP_API.Models;

namespace VPK_ERP.Models.Custom_Models
{
    public class ChiTietChamCong
    {

        public int RowIDChiTietChamCong { get; set; }

        public DateTime NgayGio { get; set; }


        public string LyDo { get; set; }

        public string Loai { get; set; }


        public float SoGioTangCa { get; set; }

        public string LyDoTangCa { get; set; }


        public DateTime? GioVaoSomNhat { get; set; }
        public DateTime? GioVaoTreNhat { get; set; }

        public DateTime? GioRaSomNhat { get; set; }

        public DateTime? GioRaTreNhat { get; set; }


        public int RowIDEmployeeEdited { get; set; }







    }


    public class TestNhanData
    {
        public string CMND { get; set; }

        public string Month { get; set; }
        public int Year { get; set; }

        public string TuNgay { get; set; }
        public string DenNgay { get; set; }


    }



    public class ThongTinChamCong
    {

        public Employee _token { get; set; }
        public int LyDoChamCong { get; set; }

        public string LoaiChamCong { get; set; }

        public string LyDoChamVao { get; set; }

        public string LyDoChamRa { get; set; }

        public float TotalHour { get; set; }

        public string Reason { get; set; }

        public string ChuoiToken { get; set; }

        public DateTime NgayDangKy { get; set; }

        public int IDNguoiDuocChamCong { get; set; }

    }
}



