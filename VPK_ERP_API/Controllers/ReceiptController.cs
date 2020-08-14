using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using VPK_ERP_API.Models;

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
            var listOfReceiptHeaderAndLine = db.ReceiptLines.Where(w => w.RowID == c.RowID).ToList().Select(s => new
            {
                s.RowID,
                s.Description,
                s.Times,
                s.TotalPrice,
                s.CreatedDate

            }).OrderByDescending(o => o.RowID).ToList();

            return Ok(listOfReceiptHeaderAndLine);

        }



    }
}
