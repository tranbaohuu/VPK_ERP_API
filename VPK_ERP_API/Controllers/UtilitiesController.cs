using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
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


            //Employee emp = obj._token;


            if (obj != null && obj.ChuoiToken != null && obj.ChuoiToken.ToString() != "")
            {

                if (ValidateCurrentToken(obj.ChuoiToken.ToString()) == true)
                {

                    string RowID = GetClaim(obj.ChuoiToken.ToString(), "RowID");

                    Toolkits t = new Toolkits();

                    var newMenu = t.Select_All_Menu2_Huu_2(Int32.Parse(RowID));

                    return Ok(newMenu);
                }
                else
                {
                    return NotFound();

                }



            }
            else
            {
                return NotFound();

            }







        }


        public string GenerateToken(Employee em)
        {
            var mySecret = "asdv234234^&%&^%&^hjsdfb2%%%";
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            var myIssuer = "https://erp.vuongphuckhang.com";
            var myAudience = "https://erp.vuongphuckhang.com";

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim("RowID", em.RowID.ToString()),
            new Claim("Username", em.Username.ToString()),
            new Claim("Color", em.Color.ToString()),
            new Claim("Fullname", em.Fullname.ToString()),

                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = myIssuer,
                Audience = myAudience,
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public bool ValidateCurrentToken(string token)
        {
            var mySecret = "asdv234234^&%&^%&^hjsdfb2%%%";
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));


            var myIssuer = "https://erp.vuongphuckhang.com";
            var myAudience = "https://erp.vuongphuckhang.com";

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = myIssuer,
                    ValidAudience = myAudience,
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }


        public string GetClaim(string token, string claimType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            var stringClaimValue = securityToken.Claims.First(claim => claim.Type == claimType).Value;
            return stringClaimValue;
        }



    }
}
