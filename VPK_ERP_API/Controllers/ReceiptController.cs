using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Web.Http;
using System.Web.Http.Cors;
using VPK_ERP_API.Models;
using VPK_ERP_API.Models.Custom_Models;

namespace VPK_ERP_API.Controllers
{

    //nhận CORS
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReceiptController : ApiController
    {


        private VPK_ERPEntities db = new VPK_ERPEntities();

        [HttpPost]
        //[ResponseType(typeof(Building))]
        [Route("api/danh-sach-phieu-thu-theo-cong-trinh")]
        public IHttpActionResult DanhSachPhieuThuTheoCongTrinh(Building c)
        {




            var listOfReceiptHeaderAndLine = db.ReceiptHeaders.Where(w => w.RowIDBuilding == c.RowID).ToList().Select(s => new
            {
                s.RowID,
                s.Code,
                s.Description,
                s.CreatedDate,
                TotalAllPrice = s.ReceiptLines.Sum(su => su.TotalPrice)

                //ListOfReceipLine = s.ReceiptLines.Select(s2 => new { s2.RowID, s2.Description, s2.RowIDContract }).ToList()


            }).OrderByDescending(o => o.RowID).ToList();




            return Ok(listOfReceiptHeaderAndLine);

        }



        [HttpPost]
        //[ResponseType(typeof(Building))]
        [Route("api/danh-sach-chi-tiet-phieu")]
        public IHttpActionResult DanhSachChiTietPhieuTheoRowIDReceiptHeader(ReceiptHeader c)
        {
            var listOfReceiptHeaderAndLine = db.ReceiptLines.Where(w => w.RowIDReceiptHeader == c.RowID).ToList().Select(s => new
            {
                s.ReceiptHeader.Code,
                DescriptionReceiptHeader = s.ReceiptHeader.Description,
                s.RowID,
                s.Description,
                s.Times,
                s.TotalPrice,
                s.CreatedDate,
                ContractCode = (s.Contract != null ? s.Contract.ContractCode : ""),
                RowIDContract = (s.Contract != null ? s.Contract.RowID : -1)


            }).OrderByDescending(o => o.RowID).ToList();

            return Ok(listOfReceiptHeaderAndLine);

        }




        [HttpPost]
        //[ResponseType(typeof(Building))]
        [Route("api/them-phieu-thu-chi")]
        public IHttpActionResult ThemPhieuThuChi(PhieuChiThu c)
        {

            if (c.DanhSachChiTietPhieuThu.Count > 0)
            {


                ReceiptHeader rh = new ReceiptHeader();
                rh.Code = c.Code;
                rh.Description = c.DescriptionReceiptHeader;
                rh.RowIDEmployeeCreated = c.RowIDEmployeeCreated;
                rh.RowIDBuilding = c.RowIDBuilding;

                rh.CreatedDate = DateTime.Now;

                db.ReceiptHeaders.Add(rh);

                int affectedRows = db.SaveChanges();


                if (affectedRows > 0)
                {

                    int RowIDReceiptHeader = rh.RowID;




                    foreach (var item in c.DanhSachChiTietPhieuThu)
                    {
                        ReceiptLine rl = new ReceiptLine();
                        rl.RowIDContract = item.RowIDContract;
                        rl.RowIDReceiptHeader = RowIDReceiptHeader;
                        rl.RowIDEmployeeCreated = item.RowIDEmployeeCreated;
                        rl.Times = item.Times;
                        rl.Description = item.Description;
                        rl.TotalPrice = item.TotalPrice;
                        rl.CreatedDate = DateTime.Now;

                        db.ReceiptLines.Add(rl);
                    }



                    db.SaveChanges();



                    return Ok("Thêm thành công !");
                }
                else
                {
                    return BadRequest("Thêm ReceipHeader không thành công !");
                }
            }
            else
            {
                return BadRequest("Không có chi tiết phiếu bên trong một tờ phiếu !");
            }

        }




        [HttpPost]
        //[ResponseType(typeof(Building))]
        [Route("api/them-phieu-thu-chi")]
        public IHttpActionResult ThemNhanhPhieu(PhieuNhapNhanh c)
        {



            ReceiptHeader rh = new ReceiptHeader();
            rh.Code = c.MaCongTrinh;
            rh.Description = c.GhiChu;
            rh.RowIDEmployeeCreated = c.RowIDEmployeeCreated;
            rh.RowIDBuilding = c.RowIDBuilding;

            rh.CreatedDate = DateTime.Now;

            db.ReceiptHeaders.Add(rh);

            int affectedRows = db.SaveChanges();


            if (affectedRows > 0)
            {

                int RowIDReceiptHeader = rh.RowID;





                ReceiptLine rl = new ReceiptLine();
                rl.RowIDContract = null;
                rl.RowIDReceiptHeader = RowIDReceiptHeader;
                rl.RowIDEmployeeCreated = c.RowIDEmployeeCreated;
                rl.Times = null;
                rl.Description = c.GhiChu;
                rl.TotalPrice = c.ThanhTien;
                rl.CreatedDate = DateTime.Now;

                db.ReceiptLines.Add(rl);




                db.SaveChanges();



                return Ok("Thêm thành công !");
            }
            else
            {
                return BadRequest("Thêm ReceipHeader không thành công !");
            }

        }



        [HttpPost]
        //[ResponseType(typeof(Building))]
        [Route("api/sua-chi-tiet-phieu")]
        public IHttpActionResult SuaChiTietPhieu(PhieuChiThu c)
        {

            if (c != null)
            {


                int count = 0;

                foreach (var item in c.DanhSachChiTietPhieuThu)
                {


                    var objPhieuThu = db.ReceiptLines.Where(w => w.RowID == item.RowID).FirstOrDefault();


                    objPhieuThu.ReceiptHeader.Code = c.Code;
                    objPhieuThu.ReceiptHeader.Description = c.DescriptionReceiptHeader;
                    objPhieuThu.RowIDContract = item.RowIDContract;
                    objPhieuThu.Times = item.Times;
                    objPhieuThu.Description = item.Description;
                    objPhieuThu.TotalPrice = item.TotalPrice;
                    objPhieuThu.EditedDate = DateTime.Now;
                    objPhieuThu.RowIDEmployeeEdited = item.RowIDEmployeeEdited;

                    count += db.SaveChanges();



                }


                if (count > 0)
                {
                    return Ok("Chỉnh sửa phiếu thành công !");

                }
                else
                {
                    return BadRequest("Chỉnh sửa phiếu thất bại !");

                }

            }
            else
            {
                return BadRequest("Không có chi tiết phiếu bên trong một tờ phiếu !");
            }

        }



    }
}
