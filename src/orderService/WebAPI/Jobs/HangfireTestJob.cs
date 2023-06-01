using Hangfire;

namespace WebAPI.Jobs;

public class HangfireTestJob
{
    private ILogger<HangfireTestJob> _logger;

    public HangfireTestJob(ILogger<HangfireTestJob> logger)
    {
        _logger = logger;
    }

    [JobDisplayName("Hangfire Test Job")]
    public async Task RunAsync()
    {
        _logger.LogInformation("HangfireTestJob is working.");
        await Task.Delay(1000);
    }
}
