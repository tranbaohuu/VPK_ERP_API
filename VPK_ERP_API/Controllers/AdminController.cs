
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using VPK_ERP_API.Models;

namespace VPK_ERP_API.Controllers
{
    public class AdminController : ApiController
    {
        private VPK_ERPEntities db = new VPK_ERPEntities();




        [HttpGet]
        [Route("api/xuat-cham-cong")]
        public IHttpActionResult Export_XemChamCong()
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Cham Cong");
                    worksheet.Cell("A1").Value = "Loại chấm";
                    worksheet.Cell("B1").Value = "Ngày giờ";
                    worksheet.Cell("C1").Value = "Ngày giờ";
                    worksheet.Cell("D1").Value = "Ngày giờ";
                    worksheet.Cell("E1").Value = "Ngày giờ";
                    //worksheet.Cell("A2").FormulaA1 = "=MID(A1, 7, 5)";




                    var attendance_Detail = db.Attendance_Detail.Include(a => a.AttendanceReason).Include(a => a.Attendance_Header).Include(a => a.Employee);


                    int count = 2;

                    foreach (var item in attendance_Detail)
                    {
                        worksheet.Cell("A" + count).Value = item.Type;
                        worksheet.Cell("B" + count).Value = item.CreatedDate.ToString();
                        worksheet.Cell("C" + count).Value = item.AttendanceReason.Name;
                        worksheet.Cell("D" + count).Value = item.Employee.Fullname;
                        worksheet.Cell("E" + count).Value = item.RowIDAttendanceReason.ToString();
                        count++;


                    }



                    worksheet.Columns().AdjustToContents();



                    string fileName = System.Web.Hosting.HostingEnvironment.MapPath("~");

                    if (!Directory.Exists(fileName + "\\" + "Excels"))
                    {
                        Directory.CreateDirectory(fileName + "\\" + "Excels");
                    }


                    string nameOfFile = DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + "XemChamCong" + ".xlsx";

                    fileName += "\\" + "Excels" + "\\" + nameOfFile;


                    workbook.SaveAs(fileName);


                    //Close the workbook.


                    //Response.BinaryWrite(worksheet.b());
                    //Response.End();



                    //Message box confirmation to view the created spreadsheet.


                    return Redirect("https://localhost:44389/Excels/" + nameOfFile);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [HttpGet]
        [Route("api/tinh-cham-cong")]
        public void TinhToanLuongVaCapNhat()
        {
            bool flagKhongDuThoiGian = false;



            var listAllEmployee = db.Employees.Where(w => w.RowID == 3).Select(s => s.RowID).ToList();

            DateTime tungay = new DateTime(2021, 03, 02, 00, 00, 00);
            DateTime denngay = new DateTime(2021, 03, 02, 23, 59, 59);


            foreach (var item in listAllEmployee)
            {



                var listAllDays = db.Attendance_Detail.Where(w => w.RowIDEmployee == item && w.CreatedDate >= tungay && w.CreatedDate <= denngay).ToList();


                var listShortDays = listAllDays.Select(s => s.CreatedDate.Value.ToShortDateString()).Distinct().ToList();


                Dictionary<string, double> gioLamTrongNGay = new Dictionary<string, double>();

                foreach (var soNgay in listShortDays)
                {

                    var listGioVao1Ngay = listAllDays.Where(w => w.Type == "IN" && w.CreatedDate.Value.ToShortDateString() == soNgay).OrderBy(o => o.CreatedDate).ToList();
                    var listGioRa1Ngay = listAllDays.Where(w => w.Type == "OUT" && w.CreatedDate.Value.ToShortDateString() == soNgay).OrderBy(o => o.CreatedDate).ToList();


                    if (listGioVao1Ngay.Count + listGioRa1Ngay.Count < 4)
                    {
                        var arrSplitTemp1 = soNgay.Split('/');
                        DateTime d1 = new DateTime(Int32.Parse(arrSplitTemp1[2]), Int32.Parse(arrSplitTemp1[0]), Int32.Parse(arrSplitTemp1[1]));

                        var temp1 = db.Attendance_Header.Where(w => w.RowIDEmployee == item && w.AttendanceShortDate.Value == d1).FirstOrDefault();

                        temp1.TotalWorkingHours = 0;

                        db.SaveChanges();
                        continue;
                    }




                    //if (listGioRa1Ngay.Count == 2)
                    //{
                    //    var arrSplitTemp2 = soNgay.Split('/');
                    //    DateTime d2 = new DateTime(Int32.Parse(arrSplitTemp2[2]), Int32.Parse(arrSplitTemp2[0]), Int32.Parse(arrSplitTemp2[1]));
                    //    var temp2 = db.Attendance_Header.Where(w => w.RowIDEmployee == item && w.AttendanceShortDate.Value == d2).FirstOrDefault();

                    //    temp2.TotalWorkingHours = 0;

                    //    db.SaveChanges();

                    //    continue;
                    //}


                    var gioVaoSomNhat = listGioVao1Ngay.FirstOrDefault();
                    var gioVaoTreNhat = listGioVao1Ngay.LastOrDefault();

                    var gioRaSomNhat = listGioRa1Ngay.FirstOrDefault();
                    var gioRaTreNhat = listGioRa1Ngay.LastOrDefault();

                    var soGioLamSang = (gioRaSomNhat.CreatedDate.Value - gioVaoSomNhat.CreatedDate.Value).TotalHours;
                    var soGioLamChieu = (gioRaTreNhat.CreatedDate.Value - gioVaoTreNhat.CreatedDate.Value).TotalHours;

                    var tongGio1Ngay = soGioLamSang + soGioLamChieu;

                    gioLamTrongNGay.Add(soNgay, tongGio1Ngay);


                    var arrSplit = soNgay.Split('/');
                    DateTime d = new DateTime(Int32.Parse(arrSplit[2]), Int32.Parse(arrSplit[0]), Int32.Parse(arrSplit[1]));


                    var searchobj = db.Attendance_Header.Where(w => w.RowIDEmployee == item && w.AttendanceShortDate.Value == d).FirstOrDefault();

                    searchobj.TotalWorkingHours = tongGio1Ngay;

                    db.SaveChanges();

                }



                //var listIn = db.Attendance_Detail.Where(w => w.RowIDEmployee == EmployeeID && w.Type == "IN").ToList();
                //var listOut = db.Attendance_Detail.Where(w => w.RowIDEmployee == EmployeeID && w.Type == "OUT").ToList();


            }


        }

    }

}
