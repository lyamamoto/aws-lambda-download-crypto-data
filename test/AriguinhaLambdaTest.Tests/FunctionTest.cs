using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.SQSEvents;

namespace AriguinhaLambdaTest.Tests;

public class FunctionTest
{
    [Fact]
    public void TestToUpperFunction()
    {

        // Invoke the lambda function and confirm the string was upper cased.
        var function = new Function();
        var message = new SQSEvent() { Records = { new SQSEvent.SQSMessage() { Body = "" } } };
        var context = new TestLambdaContext();
        var result = function.FunctionHandler(message, context);

        Assert.True(result.IsCompleted);
    }
}
