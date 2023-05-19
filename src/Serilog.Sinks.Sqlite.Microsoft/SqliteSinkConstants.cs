using System;
using System.Collections.Generic;
using System.Text;

namespace Serilog.Sinks.Sqlite.Microsoft;
public static class SqliteSinkConstants
{
    public const string LogTableName = "Log";
    public const string TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff";
}
