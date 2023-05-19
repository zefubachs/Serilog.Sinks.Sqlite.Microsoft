

using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.WithProperty("Application", "ConsoleExample")
    .WriteTo.Sqlite("Log.sqlite")
    .CreateLogger();


try
{
    Log.Information("This is a test message at {Time} from {Machine}", DateTime.Now, Environment.MachineName);
    Log.Warning("Logging a anonymous object {Element}", new { Message = "Hallo", Value = 5, Result = false });
}
finally
{
    Log.CloseAndFlush();
}