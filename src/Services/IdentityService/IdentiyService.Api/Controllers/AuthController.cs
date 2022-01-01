using IdentiyService.Api.Application.Models;
using IdentiyService.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentiyService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService IdentityService;

        public AuthController(IIdentityService identityService)
        {
            IdentityService = identityService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel loginRequestModel)
        {
            var result = await IdentityService.Login(loginRequestModel);

            return Ok(result);  
        }
    }
}
