using AutoMapper;
using MediatR;
using OrderService.Application.Features.Queries.ViewModels;
using OrderService.Application.Interfaces.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.Application.Features.Queries.GetOrderDetailById
{
    public class GetOrderDetailsQueryHandler : IRequestHandler<GetOrderDetailsQuery, OrderDetailViewModel>
    {
        IOrderRepository OrderRepository;
        private readonly IMapper Mapper;
        public GetOrderDetailsQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            OrderRepository = orderRepository;
            Mapper = mapper;
        }

        public async Task<OrderDetailViewModel> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
        {
            var order = await OrderRepository.GetByIdAsync(request.OrderId, i => i.OrderItems);

            var result = Mapper.Map<OrderDetailViewModel>(order);

            return result;
        }
    }
}
