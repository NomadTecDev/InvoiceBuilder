namespace InvoiceBuilder.ConsoleApp;

internal interface IConsole
{
    void Write(string message);
    void WriteLine(string message);
    void WriteLine(string format, params object[] args);
    string ReadLine();
    ConsoleKeyInfo ReadKey(bool intercept);
}
