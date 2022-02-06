using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.BuyerAggregate;

namespace OrderService.Infrastructure.Context.Repositories
{
    public class BuyerRepository : GenericRepository<Buyer>, IBuyerReporsitory
    {
        public BuyerRepository(OrderDbContext dbContext) : base(dbContext)
        {
        }
    }
}
