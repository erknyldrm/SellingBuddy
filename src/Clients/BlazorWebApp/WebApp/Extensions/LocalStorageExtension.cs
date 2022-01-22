using Blazored.LocalStorage;
using System.Threading.Tasks;

namespace WebApp.Extensions
{
    public static class LocalStorageExtension
    {
        public static string GetUserName(this ISyncLocalStorageService localStorageService)
        {
            return localStorageService.GetItem<string>("username"); 
        }

        public async static Task<string> GetUserName(this ILocalStorageService localStorageService)
        {
            return await localStorageService.GetItemAsync<string>("username");
        }

        public static void SetUserName(this ISyncLocalStorageService localStorageService, string value)
        {
            
            localStorageService.SetItem<string>("username", value);
        }

        public static string GetToken(this ISyncLocalStorageService localStorageService)
        {
            return localStorageService.GetItem<string>("token");
        }

        public async static Task<string> GetToken(this ILocalStorageService localStorageService)
        {
            return await localStorageService.GetItemAsync<string>("token");
        }

        public static void SetToken(this ISyncLocalStorageService localStorageService, string value)
        {

            localStorageService.SetItem<string>("token", value);
        }
    }
}
