using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using VPK_ERP.Models.Custom_Models;
using VPK_ERP_API.Models;
using VPK_ERP_API.Utilities;

namespace VPK_ERP_API.Controllers
{
    //nhận CORS
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UtilitiesController : ApiController
    {



        [HttpPost]
        [Route("api/tao-thanh-menu")]
        public IHttpActionResult TaoThanhMenu([FromBody] ThongTinChamCong obj)
        {


            Employee emp = obj._token;


            Toolkits t = new Toolkits();

            var newMenu = t.Select_All_Menu2_Huu_2(emp.RowID);

            return Ok(newMenu);



        }




    }
}
