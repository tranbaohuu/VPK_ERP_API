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
        [ResponseType(typeof(Customer))]
        [Route("api/danh-sach-khach-hang")]
        public IHttpActionResult DanhSachKhachHang()
        {




            var listCustomers = db.Customers.ToArray().Select(s => new
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



            }).ToList();




            return Ok(listCustomers);

        }
    }

}
