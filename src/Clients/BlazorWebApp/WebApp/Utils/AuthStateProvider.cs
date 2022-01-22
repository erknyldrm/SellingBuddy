using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Extensions;

namespace WebApp.Utils
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService localStorageService;
        private readonly HttpClient httpClient;
        private readonly AuthenticationState anonymous;

        public AuthStateProvider(ILocalStorageService localStorageService, HttpClient httpClient, AuthenticationState anonymous)
        {
            this.localStorageService = localStorageService;
            this.httpClient = httpClient;
            this.anonymous = anonymous;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string apiToken = await localStorageService.GetToken();
            if (string.IsNullOrEmpty(apiToken))
                return anonymous;

            string userName = await localStorageService.GetUserName();

            var cp = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userName)

            },"jwtAuthType"));

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", apiToken);

            return new AuthenticationState(cp);
        }

        public void NotifyUserLogin(string userName)
        {
            var cp = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, userName)
            },"jwtAuthType"
            ));

            var authState = Task.FromResult(new AuthenticationState(cp));
            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(anonymous);
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
