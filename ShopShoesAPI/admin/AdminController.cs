using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopShoesAPI.common;
using ShopShoesAPI.Enums;
using System.Net;

namespace ShopShoesAPI.admin
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly IAdmin iAdmin;
        public AdminController(IAdmin iAdmin)
        {
            this.iAdmin = iAdmin;
        }

        [HttpGet]
        public async Task<ApiRespone> FindAllUser([FromQuery] QueryAndPaginateDTO queryAndPaginate)
        {
            return new ApiRespone
            {
                Status = (int)HttpStatusCode.OK,
                Metadata = await this.iAdmin.FindAllUser(queryAndPaginate)
            };
        }

    }
}
