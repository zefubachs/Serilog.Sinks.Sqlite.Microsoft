using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;
using Serilog.Sinks.Sqlite.Microsoft;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Serilog;
public static class SqliteSinkExtensions
{
    public static LoggerConfiguration Sqlite(this LoggerSinkConfiguration loggerSinkConfiguration, string databasePath,
        // Sqlite sink options
        string tableName = SqliteSinkConstants.LogTableName, string timestampFormat = SqliteSinkConstants.TimestampFormat,

        // Batching options
        int batchSizeLimit = 100, TimeSpan? period = null, int queueLimit = 1000,

        LogEventLevel restrictToMinimumLevel = LogEventLevel.Verbose, LoggingLevelSwitch? levelSwitch = null, IFormatProvider? formatProvider = null)
    {
        var sinkOptions = new SqliteSinkOptions
        {
            LogTableName = tableName,
            TimestampFormat = timestampFormat,
        };
        var sink = new SqliteSink(databasePath, sinkOptions, formatProvider);

        var batchingOptions = new PeriodicBatchingSinkOptions
        {
            BatchSizeLimit = batchSizeLimit,
            Period = period ?? TimeSpan.FromSeconds(2),
            EagerlyEmitFirstEvent = true,
            QueueLimit = queueLimit,
        };
        var batchingSink = new PeriodicBatchingSink(sink, batchingOptions);
        return loggerSinkConfiguration.Sink(batchingSink, restrictToMinimumLevel, levelSwitch);
    }

    public static string Json(this IReadOnlyDictionary<string, LogEventPropertyValue> properties)
    {
        var dictionary = new Dictionary<string, object?>();
        foreach (var property in properties)
        {
            dictionary.Add(property.Key, Simplify(property.Value));
        }

        return JsonSerializer.Serialize(dictionary);
    }

    private static object? Simplify(LogEventPropertyValue data)
    {
        if (data is ScalarValue scalar)
            return scalar.Value;

        if (data is DictionaryValue dictValue)
        {
            var dictionary = new Dictionary<string, object?>();
            foreach (var element in dictValue.Elements)
            {
                if (element.Key.Value is string key)
                    dictionary.Add(key, Simplify(element.Value));
            }
            return dictionary;
        }

        if (data is SequenceValue sequence)
            return sequence.Elements.Select(Simplify).ToArray();

        if (data is not StructureValue structure)
            return null;

        return null;
    }
}
