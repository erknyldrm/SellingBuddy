using System.Threading.Tasks;

namespace WebApp.Application.Services.Interfaces
{
    public interface IIdentityService
    {
        string GetUserName();

        string GetUserToken();

        bool IsLoggedIn { get; }

        Task<bool> Login(string userName, string ğassword);

        void Logout();
    }
}
