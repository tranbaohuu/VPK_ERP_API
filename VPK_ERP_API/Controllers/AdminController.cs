
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
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
        public IHttpActionResult Export_XemChamCong(DateTime TuNgay, DateTime DenNgay)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {


                    TuNgay = new DateTime(TuNgay.Year, TuNgay.Month, TuNgay.Day, 0, 0, 0);
                    DenNgay = new DateTime(DenNgay.Year, DenNgay.Month, DenNgay.Day, 23, 59, 59);


                    var worksheet = workbook.Worksheets.Add("L1 - Chi tiết chấm công");
                    worksheet.Cell("A1").Value = "Họ tên";
                    worksheet.Cell("B1").Value = "Ngày giờ chấm";
                    worksheet.Cell("C1").Value = "Vào / Ra";
                    worksheet.Cell("D1").Value = "Loại chấm";
                    //worksheet.Cell("A2").FormulaA1 = "=MID(A1, 7, 5)";


                    worksheet.Columns("B").Style.DateFormat.Format = "yyyy-mm-dd hh:mm:ss";

                    //worksheet.Columns("B").Cells().SetDataType(XLDataType.Text);
                    //worksheet.Columns("B").Cells().DataType = XLDataType.Text;


                    var attendance_Detail = db.Attendance_Detail.Include(a => a.AttendanceReason)
                        .Include(a => a.Attendance_Header)
                        .Include(a => a.Employee)
                        .Where(w => w.CreatedDate >= TuNgay && w.CreatedDate <= DenNgay)
                        .OrderBy(o => o.Employee.Fullname).ThenBy(o => o.Attendance_Header.AttendanceShortDate);


                    int count = 2;

                    foreach (var item in attendance_Detail)
                    {
                        worksheet.Cell("A" + count).Value = item.Employee.Fullname;
                        worksheet.Cell("B" + count).Value = item.CreatedDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                        worksheet.Cell("C" + count).Value = item.Type;
                        worksheet.Cell("D" + count).Value = item.AttendanceReason.Name;
                        count++;


                    }



                    worksheet.Columns().AdjustToContents();






                    //thêm sheet tổng số giờ chấm công 1 người theo ngày

                    var worksheetTongSoGio1Ngay = workbook.Worksheets.Add("L2 - Giờ làm từng người 1 ngày");
                    worksheetTongSoGio1Ngay.Cell("A1").Value = "Họ tên";
                    worksheetTongSoGio1Ngay.Cell("B1").Value = "Ngày tháng";
                    worksheetTongSoGio1Ngay.Cell("C1").Value = "Tổng số giờ";
                    worksheetTongSoGio1Ngay.Cell("D1").Value = "Lý do";


                    worksheetTongSoGio1Ngay.Columns("B").Style.DateFormat.Format = "yyyy-mm-dd";



                    var tongSoGio = db.Attendance_Header
                    .Include(a => a.Employee)
                    .Where(w => w.CreatedDate >= TuNgay && w.CreatedDate <= DenNgay)
                    .OrderBy(o => o.Employee.Fullname).ThenBy(o => o.AttendanceShortDate);


                    count = 2;

                    foreach (var item in tongSoGio)
                    {
                        worksheetTongSoGio1Ngay.Cell("A" + count).Value = item.Employee.Fullname;
                        worksheetTongSoGio1Ngay.Cell("B" + count).Value = item.AttendanceShortDate.Value.ToString("yyyy-MM-dd");
                        worksheetTongSoGio1Ngay.Cell("C" + count).Value = item.TotalWorkingHours;
                        count++;


                    }


                    worksheetTongSoGio1Ngay.Columns().AdjustToContents();



                    //thêm sheet tổng số giờ chấm công 1 người theo tháng

                    string connString = System.Configuration.ConfigurationManager.ConnectionStrings["MyServer"].ToString();


                    SqlConnection conn = new SqlConnection(connString);
                    conn.Open();


                    SqlDataAdapter adap = new SqlDataAdapter(@"SELECT a.Fullname,
                                               LEFT(b.AttendanceShortDate, 7) AS AttendanceShortDate,
                                               SUM(   CASE
                                                          WHEN b.TotalWorkingHours IS NOT NULL THEN
                                                              b.TotalWorkingHours
                                                          ELSE
                                                              0
                                                      END
                                                  ) AS TotalWorkingHours
                                        FROM dbo.Employees AS a
                                            INNER JOIN dbo.Attendance_Header AS b
                                                ON a.RowID = b.RowIDEmployee
                                        WHERE b.AttendanceShortDate
                                        BETWEEN '" + TuNgay + @"' AND '" + DenNgay + @"'
                                        GROUP BY a.Fullname,
                                                 LEFT(b.AttendanceShortDate, 7)
                                        ORDER BY a.Fullname; ", conn);


                    DataTable tb = new DataTable();
                    adap.Fill(tb);

                    conn.Close();




                    var worksheetTongSoGio1Thang = workbook.Worksheets.Add("L3 - Giờ làm từng người 1 tháng");
                    worksheetTongSoGio1Thang.Cell("A1").Value = "Họ tên";
                    worksheetTongSoGio1Thang.Cell("B1").Value = "Tháng";
                    worksheetTongSoGio1Thang.Cell("C1").Value = "Tổng số giờ";



                    worksheetTongSoGio1Thang.Columns("B").Style.DateFormat.Format = "yyyy-mm";


                    count = 2;
                    foreach (DataRow item in tb.Rows)
                    {
                        worksheetTongSoGio1Thang.Cell("A" + count).Value = item["FullName"].ToString();
                        worksheetTongSoGio1Thang.Cell("B" + count).Value = item["AttendanceShortDate"].ToString();
                        worksheetTongSoGio1Thang.Cell("C" + count).Value = item["TotalWorkingHours"].ToString();
                        count++;

                    }


                    worksheetTongSoGio1Thang.Columns().AdjustToContents();


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




        [HttpGet]
        [Route("api/tinh-cham-cong-v2")]
        public void TinhToanLuongVaCapNhat_v2()
        {



            //var listAllEmployee = db.Employees.Where(w => w.RowID == 3).Select(s => s.RowID).ToList();
            var listAllEmployee = db.Employees.Select(s => s.RowID).ToList();

            //DateTime tungay = new DateTime(2021, 03, 02, 00, 00, 00);
            //DateTime denngay = new DateTime(2021, 03, 02, 23, 59, 59);


            foreach (var item in listAllEmployee)
            {



                //var listAllDays = db.Attendance_Detail.Where(w => w.RowIDEmployee == item && w.CreatedDate >= tungay && w.CreatedDate <= denngay).ToList();
                var listAllDays = db.Attendance_Detail.Where(w => w.RowIDEmployee == item).ToList();


                var listShortDays = listAllDays.Select(s => s.CreatedDate.Value.ToShortDateString()).Distinct().ToList();


                Dictionary<string, double> gioLamTrongNGay = new Dictionary<string, double>();

                foreach (var soNgay in listShortDays)
                {

                    var listGioVao1Ngay = listAllDays.Where(w => w.Type == "IN" && w.CreatedDate.Value.ToShortDateString() == soNgay).OrderBy(o => o.CreatedDate).ToList();
                    var listGioRa1Ngay = listAllDays.Where(w => w.Type == "OUT" && w.CreatedDate.Value.ToShortDateString() == soNgay).OrderBy(o => o.CreatedDate).ToList();

                    
                    if (listGioVao1Ngay.Count < 1)
                    {
                        var arrSplitTemp1 = soNgay.Split('/');
                        DateTime d1 = new DateTime(Int32.Parse(arrSplitTemp1[2]), Int32.Parse(arrSplitTemp1[0]), Int32.Parse(arrSplitTemp1[1]));

                        var temp1 = db.Attendance_Header.Where(w => w.RowIDEmployee == item && w.AttendanceShortDate.Value == d1).FirstOrDefault();

                        temp1.TotalWorkingHours = 0;

                        db.SaveChanges();
                        continue;
                    }




                    if (listGioRa1Ngay.Count < 1)
                    {
                        var arrSplitTemp2 = soNgay.Split('/');
                        DateTime d2 = new DateTime(Int32.Parse(arrSplitTemp2[2]), Int32.Parse(arrSplitTemp2[0]), Int32.Parse(arrSplitTemp2[1]));
                        var temp2 = db.Attendance_Header.Where(w => w.RowIDEmployee == item && w.AttendanceShortDate.Value == d2).FirstOrDefault();

                        temp2.TotalWorkingHours = 0;

                        db.SaveChanges();

                        continue;
                    }


                    var gioVaoSomNhat = listGioVao1Ngay.FirstOrDefault();
                    //var gioVaoTreNhat = listGioVao1Ngay.LastOrDefault();

                    //var gioRaSomNhat = listGioRa1Ngay.FirstOrDefault();
                    var gioRaTreNhat = listGioRa1Ngay.LastOrDefault();

                    var soGioLamSang = (gioRaTreNhat.CreatedDate.Value - gioVaoSomNhat.CreatedDate.Value).TotalHours;
                    //var soGioLamChieu = (gioRaTreNhat.CreatedDate.Value - gioVaoTreNhat.CreatedDate.Value).TotalHours;

                    var tongGio1Ngay = soGioLamSang;

                    gioLamTrongNGay.Add(soNgay, tongGio1Ngay);


                    var arrSplit = soNgay.Split('/');
                    DateTime d = new DateTime(Int32.Parse(arrSplit[2]), Int32.Parse(arrSplit[0]), Int32.Parse(arrSplit[1]));


                    var searchobj = db.Attendance_Header.Where(w => w.RowIDEmployee == item && w.AttendanceShortDate.Value == d).FirstOrDefault();





                    //tru72 1 tiếng nghỉ trưa
                    tongGio1Ngay = tongGio1Ngay - 1.5;



                    //kiểm tra xem có phải t7 hay không?

                    if (d.DayOfWeek == DayOfWeek.Saturday)
                    {
                        tongGio1Ngay += 5.5;
                    }


                    searchobj.TotalWorkingHours = Math.Round(tongGio1Ngay, 2);

                    db.SaveChanges();

                }



                //var listIn = db.Attendance_Detail.Where(w => w.RowIDEmployee == EmployeeID && w.Type == "IN").ToList();
                //var listOut = db.Attendance_Detail.Where(w => w.RowIDEmployee == EmployeeID && w.Type == "OUT").ToList();


            }


        }


    }

}
