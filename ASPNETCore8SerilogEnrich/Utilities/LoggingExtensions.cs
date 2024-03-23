namespace ASPNETCore8SerilogEnrich.Utilities
{
    using Serilog.Configuration;
    using Serilog;

    public static class LoggingExtensions
    {
        public static LoggerConfiguration WithReleaseNumber(
            this LoggerEnrichmentConfiguration enrich)
        {
            ArgumentNullException.ThrowIfNull(enrich);
            return enrich.With<ReleaseNumberEnricher>();
        }
    }
}
