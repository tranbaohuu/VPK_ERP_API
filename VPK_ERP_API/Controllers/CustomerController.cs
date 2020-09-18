using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using VPK_ERP_API.Models;

namespace VPK_ERP_API.Controllers
{


    //nhận CORS
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CustomerController : ApiController
    {


        private VPK_ERPEntities db = new VPK_ERPEntities();

        [HttpGet]
        //[ResponseType(typeof(Customer))]
        [Route("api/danh-sach-khach-hang")]
        public IHttpActionResult DanhSachKhachHang()
        {

            var listCustomers = db.Customers.Where(w => w.IsDelete == false).ToArray().Select(s => new
            {
                s.RowID,
                s.FullName,
                s.Sex,
                BirthDay = s.BirthDay != null ? s.BirthDay.Value.ToString("dd/MM/yyyy") : "",
                s.Address,
                s.WardID,
                s.DistrictID,
                s.CityID,
                s.Phone,
                s.Email,
                s.IDCardNo,
                s.DateOfIssue,
                s.PlaceOfIssue



            }).OrderByDescending(o => o.RowID).ToList();




            return Ok(listCustomers);

        }


        [HttpPost]
        [Route("api/them-khach-hang")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult ThemKhachHang(Customer c)
        {

            try
            {

                if (c.FullName != null && c.IDCardNo != null)
                {

                    c.IsActive = true;
                    c.IsDelete = false;
                    c.CreatedDate = DateTime.Now;

                    db.Customers.Add(c);
                    int affected = db.SaveChanges();

                    if (affected > 0)
                    {
                        return Ok("Đã thêm thành công");
                    }
                    else
                    {
                        return BadRequest("Không thành công");
                    }


                }
                else
                {
                    return BadRequest("Tham số truyền vào rỗng");

                }
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi nặng: " + ex.StackTrace);

            }


        }



        [HttpPost]
        [Route("api/sua-khach-hang")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult ChinhSuaThongTinKhachHang(Customer c)
        {

            try
            {



                var model = db.Customers.Where(w => w.RowID == c.RowID).FirstOrDefault();


                if (model != null)
                {

                    model.FullName = c.FullName;
                    model.Sex = c.Sex;
                    model.IDCardNo = c.IDCardNo;
                    model.Phone = c.Phone;
                    model.BirthDay = c.BirthDay;
                    model.Address = c.Address;


                    var affected = db.SaveChanges();

                    if (affected > 0)
                    {
                        return Ok("Đã chỉnh sửa thành công");
                    }
                    else
                    {
                        return BadRequest("Không thành công");
                    }

                }
                else
                {
                    return BadRequest("Không tìm thấy thông tin khách hàng");
                }




            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi nặng: " + ex.StackTrace);

            }


        }
    }

}
