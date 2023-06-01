namespace WebAPI.Jobs;

public class TestBackgroundService : BackgroundService
{
    private readonly ILogger<TestBackgroundService> _logger;

    public TestBackgroundService(ILogger<TestBackgroundService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TestBackgroundService is starting.");

        stoppingToken.Register(() =>
        {
            _logger.LogInformation("TestBackgroundService is stopping.");
        });

        while (!stoppingToken.IsCancellationRequested)
        {
            doProcess();

            try
            {
                await Task.Delay(delay: TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "TestBackgroundService is stopping due to an error.", e.Message);
            }
        }
    }

    private void doProcess()
    {
        _logger.LogInformation("TestBackgroundService is working.");
    }
}
