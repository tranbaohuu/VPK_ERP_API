using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
                s.CreatedDate
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
                s.RowID,
                s.Description,
                s.Times,
                s.TotalPrice,
                s.CreatedDate

            }).OrderByDescending(o => o.RowID).ToList();

            return Ok(listOfReceiptHeaderAndLine);

        }




        [HttpPost]
        //[ResponseType(typeof(Building))]
        [Route("api/them-phieu-thu-chi")]
        public IHttpActionResult ThemPhieuThuChi(PhieuChiThu c)
        {


            ReceiptHeader rh = new ReceiptHeader();
            rh.Code = c.Code;
            rh.Description = c.DescriptionReceiptHeader;
            rh.RowIDEmployeeCreated = c.RowIDEmployeeCreated;
            rh.RowIDBuilding = c.RowIDBuilding;

            db.ReceiptHeaders.Add(rh);

            int affectedRows = db.SaveChanges();


            if (affectedRows > 0)
            {

                int RowIDReceiptHeader = rh.RowID;


                ReceiptLine rl = new ReceiptLine();
                rl.RowIDContract = c.RowIDContract;
                rl.RowIDReceiptHeader = RowIDReceiptHeader;
                rl.RowIDEmployeeCreated = c.RowIDEmployeeCreated;
                rl.Times = c.Times;
                rl.Description = c.Description;
                rl.TotalPrice = c.TotalPrice;

                db.ReceiptLines.Add(rl);

                db.SaveChanges();



                return Ok("Thêm thành công !");
            }
            else
            {
                return BadRequest("Thêm ReceipHeader không thành công !");
            }

        }


    }
}
