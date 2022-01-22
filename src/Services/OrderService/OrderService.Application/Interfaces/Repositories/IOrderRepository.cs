using OrderService.Domain.AggregateModels.OrderAggregate;

namespace OrderService.Application.Interfaces.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
    }
}
