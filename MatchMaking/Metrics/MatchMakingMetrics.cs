using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Metrics;
using Metrics.Utils;

namespace MatchMaking.Metrics
{
    class MatchMakingMetrics
    {
        public int Errors { get; private set; }

        private readonly Timer timer = Metric.Timer("Requests", Unit.Requests);
        private readonly Counter counter = Metric.Counter("ConcurrentRequests", Unit.Requests);

        public MatchMakingMetrics()
        {
            Metric.Config.WithHttpEndpoint("http://localhost:1245/")
                .WithAllCounters()
                .WithInternalMetrics()
                .WithSystemCounters()
                .WithReporting(config => config
                .WithConsoleReport(TimeSpan.FromHours(12)));

            using(var scheduler = new ActionScheduler())
            {
                Metric.Gauge("Errors", () => 1, Unit.None);
                Metric.Gauge("% Percent/Gauge|test", () => 1, Unit.None);
                Metric.Gauge("& AmpGauge", () => 1, Unit.None);
                Metric.Gauge("()[]{} ParantesisGauge", () => 1, Unit.None);
                Metric.Gauge("Gauge With No Value", () => double.NaN, Unit.None);
            }

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ErrorHandlerUncatched);
        }

        private void ErrorHandlerUncatched(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("Hi");
            Metric.Gauge("Errors", () => Errors, Unit.Errors);
        }
    }
}
