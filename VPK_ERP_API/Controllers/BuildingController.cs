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



        [HttpGet]
        //[ResponseType(typeof(Building))]
        [Route("api/danh-sach-cong-trinh")]
        public IHttpActionResult DanhSachCongTrinh()
        {




            var listBuildings = db.Customer_Building.Where(w => w.IsDelete == false).ToList().Select(s => new
            {
                s.Building.RowID,
                s.Building.Code,
                s.Building.Name,
                Contract_Code = s.Building.Contracts.Select(s1 => s1.ContractCode).FirstOrDefault(),
                Contract_Type = s.Building.Contracts.Select(s1 => s1.ContractType).FirstOrDefault(),
                s.Building.Address,
                CreatedDate = s.Building.CreatedDate != null ? s.Building.CreatedDate.Value.ToString("dd/MM/yyyy") : "",
                TotalContractPrice = s.Building.Contracts.Sum(k => k.ContractPrice)



            }).OrderByDescending(o => o.RowID).ToList();




            return Ok(listBuildings);

        }


        [HttpPost]
        //[ResponseType(typeof(Building))]
        [Route("api/danh-sach-cong-trinh-theo-khach-hang")]
        public IHttpActionResult DanhSachCongTrinhTheoKhachHang(Customer c)
        {




            var listBuildings = db.Customer_Building.Where(w => w.RowIDCustomer == c.RowID && w.IsDelete == false).ToList().Select(s => new Child_Contract
            {
                RowID = s.Building.RowID,
                Code = s.Building.Code,
                Name = s.Building.Name,
                Contract_Code = s.Building.Contracts.Select(s1 => s1.ContractCode).FirstOrDefault(),
                Contract_Type = s.Building.Contracts.Select(s1 => s1.ContractType).FirstOrDefault(),
                Address = s.Building.Address,
                CreatedDate = s.Building.CreatedDate != null ? s.Building.CreatedDate.Value.ToString("dd/MM/yyyy") : "",
                TotalContractPrice = s.Building.Contracts.Where(w2 => w2.IsDelete == false).Sum(k => k.ContractPrice),
                TotalRealIncome = 0,
                RowIDContract = s.Building.Contracts.Select(s1 => s1.RowID).FirstOrDefault(),



            }).OrderByDescending(o => o.RowID).ToList();



            foreach (var item in listBuildings)
            {

                var listPhieuThuTheoHopDong = (from rl in db.ReceiptLines
                                               join rh in db.ReceiptHeaders on rl.RowIDReceiptHeader equals rh.RowID
                                               where rh.Type == 0
                                               && rl.RowIDContract == item.RowIDContract
                                               select new
                                               {
                                                   rl.TotalPrice
                                               }).ToList();

                long tongThu = 0;

                if (listPhieuThuTheoHopDong.Count > 0)
                {
                    tongThu = listPhieuThuTheoHopDong.Sum(s => s.TotalPrice.Value);

                }



                var listPhieuChiTheoHopDong = (from rl in db.ReceiptLines
                                               join rh in db.ReceiptHeaders on rl.RowIDReceiptHeader equals rh.RowID
                                               where rh.Type == 1
                                               && rl.RowIDContract == item.RowIDContract
                                               select new
                                               {
                                                   rl.TotalPrice
                                               }).ToList();

                long tongChi = 0;

                if (listPhieuChiTheoHopDong.Count > 0)
                {
                    tongChi = listPhieuChiTheoHopDong.Sum(s => s.TotalPrice.Value);

                }



                item.TotalRealIncome = tongThu - tongChi;


            }



            return Ok(listBuildings);

        }


        [HttpPost]
        //[ResponseType(typeof(Building))]
        [Route("api/chi-tiet-cong-trinh")]
        public IHttpActionResult ChiTietCongTrinh(Building b)
        {
            var data = db.Buildings.Where(w => w.RowID == b.RowID && w.IsDelete == false).Select(s => new
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
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi nặng: " + ex.StackTrace);

            }


        }


        [HttpPost]
        [Route("api/sua-cong-trinh")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult ChinhSuaThongTinCongTrinh(Building c)
        {

            try
            {



                var model = db.Buildings.Where(w => w.RowID == c.RowID).FirstOrDefault();


                if (model != null)
                {

                    model.Name = c.Name;
                    model.Code = c.Code;
                    model.Address = c.Address;



                    var affected = db.SaveChanges();

                    if (affected > 0)
                    {
                        return Ok("Đã chỉnh sữa thành công");
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


        [HttpPost]
        //[ResponseType(typeof(Building))]
        [Route("api/xoa-cong-trinh")]
        public IHttpActionResult XoaCongTrinh(Building c)
        {

            if (c.RowID > 0)
            {

                var objCongTrinh = db.Buildings.Where(w => w.RowID == c.RowID).FirstOrDefault();

                var objCongTrinhCuaKhach = db.Customer_Building.Where(w => w.RowIDBuilding == c.RowID).FirstOrDefault();


                if (objCongTrinh != null && objCongTrinhCuaKhach != null)
                {
                    objCongTrinh.IsDelete = true;
                    objCongTrinh.RowIDEmployeeEdited = c.RowIDEmployeeEdited;
                    objCongTrinhCuaKhach.IsDelete = true;
                    objCongTrinhCuaKhach.RowIDEmployeeEdited = c.RowIDEmployeeEdited;

                    int count = db.SaveChanges();
                    if (count > 0)
                    {
                        return Ok("Xoá thành công !");

                    }
                    else
                    {
                        return BadRequest("Xoá thất bại !");

                    }
                }
                else
                {
                    return BadRequest("Không tìm thấy thông tin !");
                }





            }
            else
            {
                return BadRequest("Tham số truyền vào không đúng !");
            }

        }
    }


}





