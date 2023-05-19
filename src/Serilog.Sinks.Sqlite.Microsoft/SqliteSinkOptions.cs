using System;
using System.Collections.Generic;
using System.Text;

namespace Serilog.Sinks.Sqlite.Microsoft;
public record SqliteSinkOptions
{
    public string LogTableName { get; set; } = SqliteSinkConstants.LogTableName;
    public string TimestampFormat { get; set; } = SqliteSinkConstants.TimestampFormat;
}
