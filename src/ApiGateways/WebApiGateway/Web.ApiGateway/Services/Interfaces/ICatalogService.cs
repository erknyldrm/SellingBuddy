using System.Collections.Generic;
using System.Threading.Tasks;
using Web.ApiGateway.Models.Catalog;

namespace Web.ApiGateway.Services.Interfaces
{
    public interface ICatalogService
    {
        Task<CatalogItem> GetCatalogItemAsync(int id);

        Task<IEnumerable<CatalogItem>> GetCatalogItemAsync(IEnumerable<int> ids);
    }
}
