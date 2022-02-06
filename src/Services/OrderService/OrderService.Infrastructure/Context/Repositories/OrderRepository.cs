using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.OrderAggregate;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.Context.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly OrderDbContext dbContext;


        public OrderRepository(OrderDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public override async Task<Order> GetByIdAsync(Guid id, params Expression<Func<Order, object>>[] includes)
        {
            var entity = await base.GetByIdAsync(id, includes);
            
            if (entity == null)
            {
                entity = dbContext.Orders.Local.FirstOrDefault(p => p.Id == id);
            }

            return entity;
        }
    }
}
