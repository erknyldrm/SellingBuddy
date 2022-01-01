using IdentiyService.Api.Application.Models;
using System.Threading.Tasks;

namespace IdentiyService.Api.Application.Services
{
    public interface IIdentityService
    {
        Task<LoginResponseModel> Login(LoginRequestModel loginRequest); 
    }
}
