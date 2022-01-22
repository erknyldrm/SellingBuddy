using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Features.Commands.CreateOrder;
using System.Reflection;

namespace OrderService.Application
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationRegistration(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddAutoMapper(assembly);
            services.AddMediatR(assembly);

            return services;
        }
    }
}
