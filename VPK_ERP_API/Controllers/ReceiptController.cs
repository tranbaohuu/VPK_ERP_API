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
        public IHttpActionResult DanhSachPhieuThuTheoCongTrinh(ReceiptHeader c)
        {




            var listOfReceiptHeaderAndLine = db.ReceiptHeaders.Where(w => w.RowIDBuilding == c.RowIDBuilding && w.Type == c.Type).ToList().Select(s => new
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
                rh.Type = c.Type;
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
        [Route("api/them-nhanh-phieu")]
        public IHttpActionResult ThemNhanhPhieu(PhieuNhapNhanh c)
        {

            //tìm xem công trình trong ngày đã thêm phiếu chi chưa có rồi thì gộp lại chỉ thêm receiptline

            string[] mangNgay = c.NgayNhap.Split('/');

            DateTime tuNgay = new DateTime(Int32.Parse(mangNgay[2]), Int32.Parse(mangNgay[1]), Int32.Parse(mangNgay[0]), 0, 0, 0);
            DateTime denNgay = new DateTime(Int32.Parse(mangNgay[2]), Int32.Parse(mangNgay[1]), Int32.Parse(mangNgay[0]), 23, 59, 59);

            var objHeader = db.ReceiptHeaders.Where(w => w.RowIDBuilding == c.RowIDCongTrinh && w.CreatedDate >= tuNgay && w.CreatedDate <= denNgay).FirstOrDefault();

            int affectedRows = 0;
            int RowIDReceiptHeader = 0;

            if (objHeader == null)
            {

                ReceiptHeader rh = new ReceiptHeader();

                rh.Description = c.GhiChu;
                rh.RowIDEmployeeCreated = c.EmployeeID;
                rh.RowIDBuilding = c.RowIDCongTrinh;

                rh.CreatedDate = DateTime.Now;
                rh.Type = 1;

                db.ReceiptHeaders.Add(rh);

                affectedRows = db.SaveChanges();



                if (affectedRows > 0)
                {
                    RowIDReceiptHeader = rh.RowID;


                }


            }
            else
            {
                affectedRows = 99;
                RowIDReceiptHeader = objHeader.RowID;
            }



            if (affectedRows > 0)
            {

                ReceiptLine rl = new ReceiptLine();
                rl.RowIDContract = null;
                rl.RowIDReceiptHeader = RowIDReceiptHeader;
                rl.RowIDEmployeeCreated = c.EmployeeID;
                rl.Times = null;
                rl.Description = c.GhiChu;
                rl.TotalPrice = c.ThanhTien;
                rl.CreatedDate = DateTime.Now;
                rl.Status = c.TinhTrang;
                rl.Supplier = c.NhaCungCap;
                rl.Unit = c.DonVi;
                rl.UnitPrice = c.GiaTien;
                rl.Quantity = c.SoLuong;
                rl.Item = c.NhomSanPham;
                rl.Category = c.HangMuc;

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






        [HttpGet]
        //[ResponseType(typeof(Building))]
        [Route("api/danh-sach-phieu-chi")]
        public IHttpActionResult DanhSachToanBoPhieuChiHeaderVaDetail()
        {




            //var listOfReceiptHeaderAndLine = db.ReceiptHeaders
            //    .Join(db.ReceiptLines, h => h.RowID, l => l.RowIDReceiptHeader, (h, l) => new { h, l })
            //    .Join(db.Buildings, h2 => h2.h.RowIDBuilding, b => b.RowID, (h2, b) => new { h2, b })
            //    .Where(w => w.h2.l.IsDeleted == false)
            //    .Select(s => new
            //    {

            //        s.h2.l.RowID,
            //        s.h2.l.Status,
            //        s.h2.l.TotalPrice,
            //        s.h2.l.CreatedDate,
            //        s.h2.l.Supplier,
            //        s.h2.l.Unit,
            //        s.h2.l.UnitPrice,
            //        s.h2.l.Quantity,
            //        s.h2.l.Item,
            //        s.h2.l.Category,
            //        s.b.Code,
            //        s.h2.l.Description


            //        //ListOfReceipLine = s.ReceiptLines.Select(s2 => new { s2.RowID, s2.Description, s2.RowIDContract }).ToList()


            //    }).OrderByDescending(o => o.RowID).ToList();





            var listOfReceiptHeaderAndLine = db.ReceiptHeaders
               .Join(db.ReceiptLines, h => h.RowID, l => l.RowIDReceiptHeader, (h, l) => new { h, l })
               .Join(db.Buildings, h2 => h2.h.RowIDBuilding, b => b.RowID, (h2, b) => new { h2, b })
               .Where(w => w.h2.l.IsDeleted == false)
               .Select(s => new
               {

                   s.h2.l.RowID,
                   s.h2.l.Status,
                   s.h2.l.TotalPrice,
                   s.h2.l.CreatedDate,
                   s.h2.l.Supplier,
                   s.h2.l.Unit,
                   s.h2.l.UnitPrice,
                   s.h2.l.Quantity,
                   s.h2.l.Item,
                   s.h2.l.Category,
                   s.b.Code,
                   RowIDBuilding = s.b.RowID,
                   s.h2.l.Description,
                   FullName = s.b.Customer_Building.Select(s1 => s1.Customer.FullName).FirstOrDefault()


                   //ListOfReceipLine = s.ReceiptLines.Select(s2 => new { s2.RowID, s2.Description, s2.RowIDContract }).ToList()


               }).OrderByDescending(o => o.RowID).ToList();

            return Ok(listOfReceiptHeaderAndLine);

        }




        [HttpPost]
        //[ResponseType(typeof(Building))]
        [Route("api/xoa-phieu-chi")]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult XoaPhieuChi(PhieuChiThu c)
        {

            if (c.RowID > 0)
            {

                var objPhieuChi = db.ReceiptLines.Where(w => w.RowID == c.RowID).FirstOrDefault();


                if (objPhieuChi != null)
                {
                    objPhieuChi.IsDeleted = true;
                    objPhieuChi.RowIDEmployeeEdited = c.RowIDEmployeeEdited;

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
                    return BadRequest("Không tìm thấy phiếu !");
                }





            }
            else
            {
                return BadRequest("Tham số truyền vào không đúng !");
            }

        }


    }
}
