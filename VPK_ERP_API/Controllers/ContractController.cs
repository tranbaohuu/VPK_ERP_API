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
    public class ContractController : ApiController
    {


        private VPK_ERPEntities db = new VPK_ERPEntities();

        [HttpPost]
        [Route("api/danh-sach-hop-dong-theo-cong-trinh")]
        public IHttpActionResult DanhSachHopDongTheoCongTrinh(Building c)
        {




            var listContracts = db.Contracts.Where(w => w.RowIDBuilding == c.RowID && w.IsDelete == false).ToList().Select(s => new
            {
                s.RowID,
                s.ContractCode,
                s.ContractType,
                s.ContractPrice,
                SignDate = s.SignDate != null ? s.SignDate.Value.ToString("dd/MM/yyyy") : "",
                CreatedDate = s.CreatedDate != null ? s.CreatedDate.Value.ToString("dd/MM/yyyy") : ""

            }).OrderByDescending(o => o.RowID).ToList();




            return Ok(listContracts);

        }


        [HttpPost]
        [Route("api/them-hop-dong")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult ThemHopDong(Contract b)
        {

            try
            {


                if (b.ContractCode != null)
                {


                    b.IsDelete = false;
                    b.CreatedDate = DateTime.Now;


                    int TongSoContract = db.Contracts.Where(w => w.RowIDBuilding == b.RowIDBuilding).Count();

                    string mact = db.Buildings.Where(w => w.RowID == b.RowIDBuilding).Select(s => s.Code).FirstOrDefault();

                    b.ContractCode = mact + "_" + (TongSoContract.ToString().Count() == 1 ? TongSoContract.ToString().PadLeft(3, '0') : TongSoContract.ToString().PadLeft(2, '0'));



                    db.Contracts.Add(b);





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
        [Route("api/sua-hop-dong")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult SuaHopDong(Contract b)
        {

            try
            {


                var objContract = db.Contracts.Where(w => w.RowID == b.RowID).FirstOrDefault();

                if (objContract != null)
                {
                    objContract.ContractCode = b.ContractCode;
                    objContract.ContractType = b.ContractType;
                    objContract.ContractPrice = b.ContractPrice;
                    objContract.RowIDEmployeeEdited = b.RowIDEmployeeEdited;
                    objContract.SignDate = b.SignDate;

                    int affected = db.SaveChanges();

                    if (affected > 0)
                    {

                        return Ok("Đã sửa thành công");


                    }
                    else
                    {
                        return BadRequest("Sửa không thành công");
                    }

                }
                else
                {
                    return BadRequest("Không tìm thấy thông tin");
                }


            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi nặng: " + ex.StackTrace);

            }


        }



        [HttpPost]
        [Route("api/xoa-hop-dong")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult XoaHopDong(Contract b)
        {

            try
            {


                var objContract = db.Contracts.Where(w => w.RowID == b.RowID).FirstOrDefault();

                if (objContract != null)
                {
                    objContract.IsDelete = true;
                    objContract.RowIDEmployeeEdited = b.RowIDEmployeeEdited;

                    int affected = db.SaveChanges();

                    if (affected > 0)
                    {

                        return Ok("Đã xoá thành công");


                    }
                    else
                    {
                        return BadRequest("Xoá không thành công");
                    }

                }
                else
                {
                    return BadRequest("Không tìm thấy thông tin");
                }


            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi nặng: " + ex.StackTrace);

            }


        }




    }
}
