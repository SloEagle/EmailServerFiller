using EmailServerFiller.Services;

namespace EmailServerFiller
{
    public class WindowsBackgroundService : BackgroundService
    {
        private readonly ILogger<WindowsBackgroundService> _logger;
        private readonly EmailService _emailService;

        public WindowsBackgroundService(ILogger<WindowsBackgroundService> logger, EmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await _emailService.FillEmailServer();

                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }
            catch (TaskCanceledException)
            {

            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "{Message}", ex.Message);

                Environment.Exit(1);
            }
        }
    }
}