using System.Diagnostics;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Gauge;
using Microsoft.AspNetCore.Http;

namespace Team5.BuildingBlocks.Metrics.Middleware
{
    public class MemoryMetricsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMetrics _metrics;

        public MemoryMetricsMiddleware(RequestDelegate next, IMetrics metrics)
        {
            _next = next;
            _metrics = metrics;
        }

        public Task Invoke(HttpContext httpContext)
        {
            var processPhysicalMemoryGauge = new GaugeOptions
            {
                Name = "Process Physical Memory",
                MeasurementUnit = Unit.Bytes
            };

            var process = Process.GetCurrentProcess();

            _metrics.Measure.Gauge.SetValue(processPhysicalMemoryGauge, process.WorkingSet64);

            return _next(httpContext);
        }
    }
}
