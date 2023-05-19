using Microsoft.Data.Sqlite;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;
using System.Text.Json;

namespace Serilog.Sinks.Sqlite.Microsoft;
public class SqliteSink : IBatchedLogEventSink
{
    private readonly string connectionString;

    private readonly SqliteSinkOptions sinkOptions;
    private readonly IFormatProvider? formatProvider;

    private readonly string insertCommandDefinition;

    public SqliteSink(string databasePath, SqliteSinkOptions sinkOptions, IFormatProvider? formatProvider)
    {
        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
        };
        connectionString = builder.ConnectionString;
        this.sinkOptions = sinkOptions;
        this.formatProvider = formatProvider;

        InitializeDatabase();
        insertCommandDefinition = CreateInsertDefinition();
    }


    public async Task EmitBatchAsync(IEnumerable<LogEvent> batch)
    {
        using var connection = GetConnection();
        var command = new SqliteCommand(insertCommandDefinition, connection);
        command.Parameters.Add("@Timestamp", SqliteType.Text);
        command.Parameters.Add("@Level", SqliteType.Text);
        command.Parameters.Add("@Exception", SqliteType.Text);
        command.Parameters.Add("@RenderedMessage", SqliteType.Text);
        command.Parameters.Add("@Properties", SqliteType.Text);

        foreach (var log in batch)
        {
            command.Parameters["@Timestamp"].Value = log.Timestamp.ToString(sinkOptions.TimestampFormat);
            command.Parameters["@Level"].Value = log.Level.ToString();
            command.Parameters["@Exception"].Value = log.Exception?.ToString() ?? string.Empty;
            command.Parameters["@RenderedMessage"].Value = log.RenderMessage(formatProvider);
            command.Parameters["@Properties"].Value = log.Properties.Count == 0
                ? string.Empty
                : log.Properties.Json();

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine(ex.Message);
            }
        }
    }

    public Task OnEmptyBatchAsync()
    {
        return Task.CompletedTask;
    }

    private SqliteConnection GetConnection()
    {
        var connection = new SqliteConnection(connectionString);
        connection.Open();
        return connection;
    }

    private void InitializeDatabase()
    {
        using var connection = GetConnection();
        CreateLogTable(connection);
    }

    private void CreateLogTable(SqliteConnection connection)
    {
        var commandDefinition = $@"CREATE TABLE IF NOT EXISTS [{sinkOptions.LogTableName}]
(
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT,
    [Timestamp] TEXT,
    [Level] VARCHAR(10),
    [Exception] TEXT,
    [RenderedMessage] TEXT,
    [Properties] TEXT
)";

        var command = new SqliteCommand(commandDefinition, connection);
        command.ExecuteNonQuery();
    }

    private string CreateInsertDefinition()
    {
        return $@"INSERT INTO [{sinkOptions.LogTableName}] (Timestamp, Level, Exception, RenderedMessage, Properties)
VALUES (@Timestamp, @Level, @Exception, @RenderedMessage, @Properties)";
    }
}