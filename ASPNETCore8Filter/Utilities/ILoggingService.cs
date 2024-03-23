namespace ASPNETCore8Filter.Utilities
{
    public interface ILoggingService<T>
    {
        void LogInformation(string message, object? structuredData = null);
        void LogWarning(string message, object? structuredData = null);
        void LogError(Exception exception, string message, object? structuredData = null);
    }
}
