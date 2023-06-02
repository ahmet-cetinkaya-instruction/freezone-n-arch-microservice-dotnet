using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core.Consul;

public static class ConsulRegistration
{
    // Consul istemcisini ServiceCollection'a ekleriyoruz.
    public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
    {
        const string section = "Consul";
        ConsulConfiguration consulConfiguration =
            configuration.GetSection(section).Get<ConsulConfiguration>()
            ?? throw new ArgumentNullException($"{section} section cannot found in configuration.");

        services.AddSingleton<IConsulClient, ConsulClient>(
            p =>
                new ConsulClient(consulConfig =>
                {
                    consulConfig.Address = new Uri(consulConfiguration.Address);
                })
        );

        return services;
    }

    // İlgili servisi Consul'a kayıt etme işlemini gerçkleştiriyoruz. Ve aynı zamanda da proje durdururken de bu durumu consul'a bildiriyoruz.
    public static IApplicationBuilder UseConsul(
        this IApplicationBuilder app,
        IHostApplicationLifetime lifeTime,
        ConsulConfiguration configuration
    )
    {
        // Gerekli servisleri aliyoruz.
        IConsulClient consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();

        // Çalışma adresini almamız gerekiyor.
        Uri serviceAddress =
            new(app.ServerFeatures.Get<IServerAddressesFeature>().Addresses.FirstOrDefault() ?? configuration.ServiceAddress);
        // Consul'a servisi kayıt ediyoruz.
        AgentServiceRegistration registration =
            new()
            {
                ID = configuration.ServiceId,
                Name = configuration.ServiceName,
                Address = serviceAddress.Host,
                Port = serviceAddress.Port,
                Tags = new[] { configuration.ServiceId, configuration.ServiceName }.Union(configuration.ServiceTags.Split(",")).ToArray(),
            };
        consulClient.Agent.ServiceDeregister(registration.ID).Wait();
        consulClient.Agent.ServiceRegister(registration).Wait();

        // Uygulama durdurulduğunda Consul'dan servisi siliyoruz.
        lifeTime.ApplicationStopping.Register(() =>
        {
            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
        });

        return app;
    }
}
