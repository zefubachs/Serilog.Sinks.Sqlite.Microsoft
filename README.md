# Serilog.Sinks.Sqlite.Microsoft

An alternative to https://github.com/saleem-mirza/serilog-sinks-sqlite

Changes in this project
- Used Microsoft.Data.Sqlite package instead of 3 (SQLite, System.Data.Sqlite and Microsoft.Data.Sqlite)
- Used Serilog's provided PeriodicBatching packages

## Getting started

`TODO: Create NuGet package`

Configure logger by calling `WriteTo.Sqlite()`
```csharp
Log.Logger = new LoggerConfiguration()
	.WriteTo.Sqlite("Logs.sqlite")
	.CreateLogger();
```

## Future improvements
- Target lower frameworks (NET4.7) with Newtonsoft.Json
- Cleanup/rollover functionality