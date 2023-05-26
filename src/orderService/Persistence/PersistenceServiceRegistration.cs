using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;
using Application.Services.Repositories;
using Persistence.Repositories;

namespace Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BaseDbContext>(options => options.UseInMemoryDatabase("nArchitecture"));

        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        return services;
    }
}
