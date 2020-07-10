using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using VPK_ERP_API.Models;
using VPK_ERP_API.Models.Custom_Models;

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




            var listBuildings = db.Customer_Building.Where(w => w.RowIDCustomer == c.RowID).ToList().Select(s => new
            {
                s.Building.RowID,
                s.Building.Code,
                s.Building.Name,
                Contract_Code = s.Building.Contracts.Select(s1 => s1.ContractCode).FirstOrDefault(),
                Contract_Type = s.Building.Contracts.Select(s1 => s1.ContractType).FirstOrDefault(),
                s.Building.Address,
                SignDate = s.Building.Contracts.Select(s1 => s1.SignDate) != null ? s.Building.Contracts.Select(s1 => s1.SignDate.Value.ToString("dd/MM/yyyy")).FirstOrDefault() : "",
                CreatedDate = s.Building.CreatedDate != null ? s.Building.CreatedDate.Value.ToString("dd/MM/yyyy") : ""







            }).OrderByDescending(o => o.RowID).ToList();




            return Ok(listBuildings);

        }


        [HttpPost]
        //[ResponseType(typeof(Building))]
        [Route("api/chi-tiet-cong-trinh")]
        public IHttpActionResult ChiTietCongTrinh(Building b)
        {
            var data = db.Buildings.Where(w => w.RowID == b.RowID).Select(s => new
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





        [HttpPost]
        [Route("api/them-cong-trinh")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult ThemCongTrinh(Building_CustomerBuilding b)
        {

            try
            {


                if (b.Mod_Building.Name != null && b.Mod_Building.Code != null)
                {


                    b.Mod_Building.IsDelete = false;
                    b.Mod_Building.CreatedDate = DateTime.Now;

                    db.Buildings.Add(b.Mod_Building);
                    int affected = db.SaveChanges();

                    if (affected > 0)
                    {

                        b.Mod_Customer_Building.IsDelete = false;
                        b.Mod_Customer_Building.CreatedDate = DateTime.Now;

                        b.Mod_Customer_Building.RowIDBuilding = b.Mod_Building.RowID;

                        db.Customer_Building.Add(b.Mod_Customer_Building);

                        affected = db.SaveChanges();

                        if (affected > 0)
                        {
                            return Ok("Đã thêm thành công");

                        }
                        else
                        {
                            return BadRequest("Không thành công thêm Customer_Building thất bại");


                        }



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
                return null;
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi nặng: " + ex.StackTrace);

            }


        }
    }


}





