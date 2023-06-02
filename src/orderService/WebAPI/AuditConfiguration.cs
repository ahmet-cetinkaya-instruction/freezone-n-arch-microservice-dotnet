using Audit.Core;
using Audit.WebApi;
using Core.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;
using Core.CrossCuttingConcerns.Logging.Serilog.Logger;
using Core.CrossCuttingConcerns.Logging.Serilog.Messages;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace WebAPI;

public class AuditConfiguration
{
    public static void AddWebApiAudit(MvcOptions mvcOptions)
    {
        // Audit.WebApi.Cor
        mvcOptions.AddAuditFilter(
            config =>
                config
                    .LogAllActions()
                    .WithEventType("{verb} {controller}.{action}")
                    .IncludeHeaders()
                    .IncludeRequestBody()
                    .IncludeResponseHeaders()
        );
    }

    public static void ConfigureAudit(IServiceCollection services, IConfiguration configuration)
    {
        Audit.Core.Configuration.Setup().UseSerilog(); // Audit.Serilog
        // ILogger
        Log.Logger = GraylogLogger.CreateLogger(
            configuration.GetSection("SeriLogConfigurations:GraylogConfiguration").Get<GraylogConfiguration>()
                ?? throw new Exception(SerilogMessages.NullOptionsMessage)
        );

        Audit.Core.Configuration.AddCustomAction(
            ActionType.OnEventSaved,
            action: scope =>
            {
                AuditApiAction auditApiAction = scope.Event.GetWebApiAuditAction();
                auditApiAction.Headers.Remove("Authorization");
            }
        );
    }
}
