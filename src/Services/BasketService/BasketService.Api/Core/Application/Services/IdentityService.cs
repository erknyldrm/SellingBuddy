using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BasketService.Api.Core.Application.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor HttpContextAccessor;
        public IdentityService(IHttpContextAccessor httpContextAccessor) => HttpContextAccessor = httpContextAccessor;

        public string GetUserName()
        {
            return HttpContextAccessor.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
        }
    }
}
