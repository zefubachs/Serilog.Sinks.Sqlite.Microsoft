using Serilog.Events;
using Serilog.Parsing;

namespace Serilog.Sinks.Sqlite.Microsoft.Tests;

[TestClass]
public class SqliteSinkTests
{
    private const string DATABASE_NAME = "Log.sqite";

    private SqliteSink sink = null!;

    [TestInitialize]
    public void Initialize()
    {
        sink = new SqliteSink(DATABASE_NAME, new SqliteSinkOptions { }, null);
    }

    [TestCleanup]
    public void Cleanup()
    {
        File.Delete(DATABASE_NAME);
    }

    [TestMethod]
    public async Task Emit_Single()
    {
        var parser = new MessageTemplateParser();
        var events = new LogEvent[]
        {
            new LogEvent(DateTimeOffset.Now, LogEventLevel.Information, null, parser.Parse("Test message with {Prop1}"), new LogEventProperty[]
            {
                new LogEventProperty("Prop1", new ScalarValue("Test"))
            })
        };
        await sink.EmitBatchAsync(events);

        Assert.IsTrue(File.Exists(DATABASE_NAME));

        await Task.Delay(1000);
    }

    //[TestMethod]
    //public void Emit_Multiple()
    //{

    //}
}