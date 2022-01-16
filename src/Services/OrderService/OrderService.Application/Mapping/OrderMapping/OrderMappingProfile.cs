using AutoMapper;
using OrderService.Application.Features.Commands.CreateOrder;
using OrderService.Application.Features.Queries.ViewModels;
using System.Linq;

namespace OrderService.Application.Mapping.OrderMapping
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Domain.AggregateModels.OrderAggregate.Order, CreateOrderCommand>().ReverseMap();

            CreateMap<OrderItem, OrderItemDTO>().ReverseMap();


            CreateMap<Domain.AggregateModels.OrderAggregate.Order, OrderDetailViewModel>()
                .ForMember(x => x.City, y => y.MapFrom(z => z.Address.City))
                .ForMember(x => x.Country, y => y.MapFrom(z => z.Address.Country))
                .ForMember(x => x.Street, y => y.MapFrom(z => z.Address.Street))
                .ForMember(x => x.ZipCode, y => y.MapFrom(z => z.Address.ZipCode))
                .ForMember(x => x.OrderNumber, y => y.MapFrom(z => z.Id.ToString()))
                .ForMember(x => x.Status, y => y.MapFrom(z => z.OrderStatus.Name))
                .ForMember(x => x.Total, y => y.MapFrom(z => z.OrderItems.Sum(i => i.Units * i.UnitPrice)))
                .ReverseMap();

            CreateMap<Domain.AggregateModels.OrderAggregate.OrderItem, OrderItem>();
        }
    }
}
