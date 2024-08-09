using InvoiceBuilder.ConsoleApp;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Text;
using Xunit.Abstractions;

namespace InvoiceBuilder.Shared.Tests;

public class TestConsole(ITestOutputHelper output) : IConsole
{
    private readonly ITestOutputHelper _output = output;
    private readonly StringBuilder _stringBuilder = new StringBuilder();
    private readonly Queue<string> _inputQueue = new();
    private readonly Queue<ConsoleKeyInfo> _keyQueue = new();

    public void Write(string message)
    {
        _output.WriteLine(message);
    }

    public void WriteLine(string message)
    {
        _output.WriteLine(message);
        _stringBuilder.AppendLine(message);
    }

    public void WriteLine(string format, params object[] args)
    {
        _output.WriteLine(string.Format(format, args));
        _stringBuilder.AppendLine(string.Format(format, args));
    }

    public string ReadLine()
    {
        return _inputQueue.Count > 0 ? _inputQueue.Dequeue() : string.Empty;
    }

    public void QueueInput(string input)
    {
        _inputQueue.Enqueue(input);
    }

    public ConsoleKeyInfo ReadKey(bool intercept)
    {
        return _keyQueue.Count > 0 ? _keyQueue.Dequeue() : new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false);
    }

    public void QueueKey(char keyChar)
    {
        var keyInfo = new ConsoleKeyInfo(keyChar, (ConsoleKey)keyChar, false, false, false);
        _keyQueue.Enqueue(keyInfo);
    }

    public string GetOutput()
    {
        return _stringBuilder.ToString();
    }
}
