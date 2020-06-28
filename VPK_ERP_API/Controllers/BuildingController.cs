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
    public class BuildingController : ApiController
    {


        private VPK_ERPEntities db = new VPK_ERPEntities();

        [HttpPost]
        //[ResponseType(typeof(Building))]
        [Route("api/danh-sach-cong-trinh-theo-khach-hang")]
        public IHttpActionResult DanhSachCongTrinhTheoKhachHang(Customer c)
        {




            var listBuildings = db.Customer_Building.Where(w => w.RowIDCustomer == c.RowID).Select(s => new
            {
                s.Building.RowID,
                s.Building.Name




            }).ToList();




            return Ok(listBuildings);

        }


        [HttpPost]
        //[ResponseType(typeof(Building))]
        [Route("api/chi-tiet-cong-trinh")]
        public IHttpActionResult ChiTietCongTrinh(Building c)
        {
            var data = db.Buildings.Where(w => w.RowID == c.RowID).Select(s => new
            {
                s.RowID,
                s.Code,
                s.Name,
                s.Address,
                WarName = s.Ward.Name,
                DistrictName = s.District.Name,
                CityName = s.City.Name


            }).FirstOrDefault();


            return Ok(data);

        }


    }




}
