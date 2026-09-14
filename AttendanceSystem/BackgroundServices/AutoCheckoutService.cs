using AttendanceSystem.Contracts.Services;

namespace AttendanceSystem.BackgroundServices
{
    // Har roz raat 10 PM (config se) par jis employee ne checkout nahi kiya,
    // uska checkout khud-ba-khud 10:00 PM laga deta hai.
    // Startup par bhi ek baar turant catch-up chalata hai - taake agar app us waqt
    // band thi (raat 10 baje) to agle din on hone par bhi purane open records close ho jayen.
    public class AutoCheckoutService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AutoCheckoutService> _logger;

        public AutoCheckoutService(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<AutoCheckoutService> logger)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var hour = _configuration.GetValue<int>("AutoCheckout:Hour", 22);
            var minute = _configuration.GetValue<int>("AutoCheckout:Minute", 0);

            // App start hote hi ek baar catch-up run - purane din ke open records close karo
            await RunCatchUpAsync(hour, minute);

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var target = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
                if (now >= target) target = target.AddDays(1);

                var delay = target - now;
                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }

                await RunCatchUpAsync(hour, minute);
            }
        }

        private async Task RunCatchUpAsync(int hour, int minute)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var attendanceService = scope.ServiceProvider.GetRequiredService<IAttendanceService>();
                await attendanceService.RunAutoCheckoutAsync(hour, minute);
                _logger.LogInformation("Auto-checkout run completed at {time}", DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Auto-checkout run failed");
            }
        }
    }
}