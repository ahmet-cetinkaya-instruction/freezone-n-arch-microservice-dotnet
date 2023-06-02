using Core.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;
using Core.CrossCuttingConcerns.Logging.Serilog.Messages;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core.Transport;

namespace Core.CrossCuttingConcerns.Logging.Serilog.Logger;

public class GraylogLogger : LoggerServiceBase
{
    public GraylogLogger(IConfiguration configuration)
    {
        const string configurationSection = "SeriLogConfigurations:GraylogConfiguration";
        GraylogConfiguration config =
            configuration.GetSection(configurationSection).Get<GraylogConfiguration>()
            ?? throw new Exception(SerilogMessages.NullOptionsMessage);

        Logger = CreateLogger(config);
    }

    public static ILogger CreateLogger(GraylogConfiguration config) =>
        new LoggerConfiguration().WriteTo
            .Graylog(
                new GraylogSinkOptions()
                {
                    HostnameOrAddress = config.HostnameOrAddress,
                    Port = config.Port,
                    TransportType = TransportType.Udp
                }
            )
            .CreateLogger();
}
