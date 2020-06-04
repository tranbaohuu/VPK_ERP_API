using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using System.Web.Http.Results;
using VPK_ERP.Models.Custom_Models;
using VPK_ERP_API.Models;
using VPK_ERP_API.Utilities;

namespace VPK_ERP_API.Controllers
{
    //nhận CORS
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EmployeesController : ApiController
    {
        private VPK_ERPEntities db = new VPK_ERPEntities();

        // GET: api/Employees
        public IQueryable<Employee> GetEmployees()
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db.Employees;
        }

        // GET: api/Employees/5
        [ResponseType(typeof(Employee))]
        public IHttpActionResult GetEmployee(int id)
        {
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // PUT: api/Employees/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEmployee(int id, Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != employee.RowID)
            {
                return BadRequest();
            }

            db.Entry(employee).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Employees
        [ResponseType(typeof(Employee))]
        public IHttpActionResult PostEmployee(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Employees.Add(employee);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = employee.RowID }, employee);
        }

        // DELETE: api/Employees/5
        [ResponseType(typeof(Employee))]
        public IHttpActionResult DeleteEmployee(int id)
        {
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }

            db.Employees.Remove(employee);
            db.SaveChanges();

            return Ok(employee);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EmployeeExists(int id)
        {
            return db.Employees.Count(e => e.RowID == id) > 0;
        }



        [HttpPost]
        [ResponseType(typeof(Employee))]
        [Route("api/dang-nhap")]
        public IHttpActionResult CheckLogin([FromBody] Employee emp)
        {
            string email = emp.Username;
            string password = emp.Password;

            if (password != null)
            {

                password = MyHmac.EncryptString("09330F0EF215D6F2257A5EE4EB5B8D2C", password);

            }


            var obj = db.Employees.Where(w => w.Username == email && w.Password == password).Select(s => new { s.Username, s.Fullname, s.RowIDJob, s.Job.Name, s.RowID }).FirstOrDefault();

            if (obj == null)
            {
                return NotFound();
            }

            return Ok(obj);

        }




        [HttpGet]
        [ResponseType(typeof(Employee))]
        [Route("api/danh-sach-nhan-vien")]
        public IHttpActionResult DanhSachNhanVien()
        {


            //DateTime d1 = new DateTime(2020, 05, 30, 14, 0, 0);

            //DateTime d2 = new DateTime(2020, 05, 30, 14, 0, 0);

            //var abc = d1.CompareTo(d2);


            var listEmployee = db.Employees.Select(s => new
            {
                s.RowID,
                s.CMND,
                s.Fullname,
                Detail = ""

            }).ToList();




            return Ok(listEmployee);

        }




        [HttpPost]
        [Route("api/chi-tiet-nhan-vien")]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult ChiTietNhanVien([FromBody] TestNhanData data)
        {



            int month = Int32.Parse(data.Month.Split('-')[1]);
            int year = Int32.Parse(data.Month.Split('-')[0]);
            var nowTime = DateTime.Now;

            var dayInMonth = DateTime.DaysInMonth(2020, month);

            var fromDate = new DateTime(nowTime.Year, month, 1);
            var toDate = new DateTime(nowTime.Year, month, dayInMonth);


            var totalHour = db.Attendance_Header.Where(w => w.Employee.CMND == data.CMND && w.AttendanceShortDate >= fromDate && w.AttendanceShortDate <= toDate).Sum(s => s != null ? s.TotalWorkingHours : 0);


            //ViewBag.TotalHourThisMonth = Math.Round(totalHour, 1);



            //lấy danh sách tất cả ngày trong 1 tháng

            List<DateTime> listDatetimes = new List<DateTime>();
            List<ChiTietChamCong> listGioVaoSomNhat = new List<ChiTietChamCong>();
            List<ChiTietChamCong> listGioVaoTreNhat = new List<ChiTietChamCong>();
            List<ChiTietChamCong> listGioRaSomNhat = new List<ChiTietChamCong>();


            for (int i = 1; i <= dayInMonth; i++)
            {


                DateTime d = new DateTime(year, month, i);
                DateTime fromDynamicDate = new DateTime(d.Year, d.Month, d.Day, 0, 0, 0);
                DateTime toDynamicDate = new DateTime(d.Year, d.Month, d.Day, 23, 59, 59);


                listDatetimes.Add(d);



                //lấy các giờ vào các ngày
                var listGioVao1Ngay = db.Attendance_Detail.Where(w => w.Type == "IN" && w.CreatedDate.Value >= fromDynamicDate && w.CreatedDate <= toDynamicDate).ToList();


                if (listGioVao1Ngay.Count == 0)
                {

                    continue;
                }


                var listGioRa1Ngay = db.Attendance_Detail.Where(w => w.Type == "OUT" && w.CreatedDate.Value >= fromDynamicDate && w.CreatedDate <= toDynamicDate).ToList();


                if (listGioRa1Ngay.Count == 0)
                {

                    continue;
                }


                var gioVaoSomNhat = listGioVao1Ngay.FirstOrDefault();
                var gioVaoTreNhat = listGioVao1Ngay.LastOrDefault();

                var gioRaSomNhat = listGioRa1Ngay.FirstOrDefault();
                var gioRaTreNhat = listGioRa1Ngay.LastOrDefault();


                listGioVaoSomNhat.Add(new ChiTietChamCong() { GioVaoSomNhat = gioVaoSomNhat.CreatedDate.Value, GioVaoTreNhat = gioVaoSomNhat.CreatedDate.Value, GioRaSomNhat = gioRaSomNhat.CreatedDate.Value, GioRaTreNhat = gioRaTreNhat.CreatedDate.Value, LyDo = gioVaoSomNhat.AttendanceReason.Name.ToString(), Loai = "GioVaoSomNhat" });
                //listGioVaoTreNhat.Add(new ChiTietChamCong() { NgayGio = gioVaoTreNhat.CreatedDate.Value, LyDo = gioVaoTreNhat.AttendanceReason.Name.ToString(), Loai = "GioVaoTreNhat" });


                //listGioRaSomNhat.Add(new ChiTietChamCong() { NgayGio = gioRaSomNhat.CreatedDate.Value, LyDo = gioRaSomNhat.AttendanceReason.Name.ToString(), Loai = "GioRaSomNhat" });


                //listGioRaSomNhat.Add(new ChiTietChamCong() { NgayGio = gioRaTreNhat.CreatedDate.Value, LyDo = gioRaTreNhat.AttendanceReason.Name.ToString(), Loai = "GioRaTreNhat" });





            }


            List<ChiTietChamCong> listTam = new List<ChiTietChamCong>();


            foreach (var item in listDatetimes)
            {

                listTam.Add(new ChiTietChamCong { NgayGio = item, GioVaoSomNhat = null, GioVaoTreNhat = null, GioRaSomNhat = null, GioRaTreNhat = null, Loai = "Rong", LyDo = "" });



            }



            foreach (var chitiet in listTam)
            {

                var fromDateSom = new DateTime(chitiet.NgayGio.Year, chitiet.NgayGio.Month, chitiet.NgayGio.Day, 00, 00, 00);
                var toDateTre = new DateTime(chitiet.NgayGio.Year, chitiet.NgayGio.Month, chitiet.NgayGio.Day, 23, 59, 59);




                foreach (var item in listGioVaoSomNhat)
                {




                    if ((item.GioVaoSomNhat.Value >= fromDateSom && item.GioVaoSomNhat.Value <= toDateTre) || (item.GioVaoTreNhat.Value >= fromDateSom && item.GioVaoTreNhat.Value <= toDateTre) || (item.GioRaSomNhat.Value >= fromDateSom && item.GioRaSomNhat.Value <= toDateTre) || (item.GioRaTreNhat.Value >= fromDateSom && item.GioRaTreNhat.Value <= toDateTre))
                    {
                        chitiet.GioVaoSomNhat = item.GioVaoSomNhat;
                        chitiet.GioVaoTreNhat = item.GioVaoTreNhat;
                        chitiet.GioRaSomNhat = item.GioRaSomNhat;
                        chitiet.GioRaTreNhat = item.GioRaTreNhat;
                        chitiet.LyDo = item.LyDo;
                        chitiet.Loai = item.Loai;

                    }




                }



            }







            //ViewBag.listGioVaoSomNhat = listGioVaoSomNhat.Distinct().ToList();
            //ViewBag.listGioVaoTreNhat = listGioVaoTreNhat.Distinct().ToList();
            //ViewBag.listGioRaSomNhat = listGioRaSomNhat.Distinct().ToList();
            //ViewBag.listGioRaSomNhat = listGioRaSomNhat.Distinct().ToList();



            //ViewBag.listDatetimes = listDatetimes;



            //List<List<ChiTietChamCong>> complexChiTietChamCong = new List<List<ChiTietChamCong>>();
            //complexChiTietChamCong.Add(listGioVaoSomNhat.Distinct().ToList());
            //complexChiTietChamCong.Add(listGioVaoTreNhat.Distinct().ToList());
            //complexChiTietChamCong.Add(listGioRaSomNhat.Distinct().ToList());
            //complexChiTietChamCong.Add(listGioRaSomNhat.Distinct().ToList())



            //var listCombined = listGioVaoSomNhat.Concat(listGioVaoTreNhat).Concat(listGioRaSomNhat).Concat(listGioRaSomNhat).ToList();









            return Ok(listTam.ToList());

        }





        [HttpPost]
        [Route("api/lay-thong-tin-ca-nhan")]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult LayThongTinCaNhan([FromBody] TestNhanData data)
        {


            if (data.CMND != null && data.CMND != "")
            {


                var obj = db.Employees.Where(w => w.CMND == data.CMND).Select(s => new { Fullname = s.Fullname, CMND = s.CMND, Phone = s.Phone, Address = s.Birthday }).FirstOrDefault();





                return Ok(obj);
            }
            else
            {
                return BadRequest("Tham số CMND truyền vào rỗng");
            }

        }


        [HttpPost]
        [Route("api/cham-cong")]
        public IHttpActionResult ChamCong([FromBody] ThongTinChamCong obj)
        {

            Employee emp = obj._token;

            var nowTime = DateTime.Now;

            var fromDate = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 00, 00, 00);
            var toDate = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 23, 59, 59);




            var checkExist = db.Attendance_Header.Where(w => w.RowIDEmployee == emp.RowID && w.AttendanceShortDate.Value >= fromDate && w.AttendanceShortDate.Value <= toDate).Count();
            int rowIDAttendanceHeader = 0;
            if (checkExist == 0)
            {
                Attendance_Header hd = new Attendance_Header();
                hd.AttendanceShortDate = nowTime;
                hd.CreatedDate = nowTime;
                hd.RowIDEmployee = emp.RowID;
                db.Attendance_Header.Add(hd);
                db.SaveChanges();
                rowIDAttendanceHeader = hd.RowID;


            }
            else
            {
                rowIDAttendanceHeader = db.Attendance_Header.Where(w => w.RowIDEmployee == emp.RowID && w.AttendanceShortDate.Value >= fromDate && w.AttendanceShortDate.Value <= toDate).FirstOrDefault().RowID;

            }



            Attendance_Detail a = new Attendance_Detail();
            a.RowIDAttendanceReason = obj.LyDoChamCong;
            a.RowIDEmployee = emp.RowID;
            if (obj.LoaiChamCong == "IN")
            {
                a.Type = "IN";
                a.Reason = obj.LyDoChamVao;

            }
            else
            {
                a.Type = "OUT";
                a.Reason = obj.LyDoChamRa;

            }

            if (rowIDAttendanceHeader != 0)
            {
                a.RowIDAttendanceHeader = rowIDAttendanceHeader;
            }

            a.CreatedDate = nowTime;
            db.Attendance_Detail.Add(a);
            db.SaveChanges();

            return Ok();



        }



    }
}