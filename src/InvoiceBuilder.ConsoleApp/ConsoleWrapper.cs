using System.Text;

namespace InvoiceBuilder.ConsoleApp;

public class ConsoleWrapper : IConsole
{
    public void Write(string message) => Console.Write(message);
    public void WriteLine(string message) => Console.WriteLine(message);
    public void WriteLine(string format, params object[] args) => Console.WriteLine(string.Format(format, args));
    public string ReadLine() => Console.ReadLine();
    public ConsoleKeyInfo ReadKey(bool intercept) => Console.ReadKey(intercept);
}
