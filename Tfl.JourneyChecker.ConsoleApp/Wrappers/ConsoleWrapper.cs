namespace Tfl.JourneyChecker.ConsoleApp.Wrappers
{
    public class ConsoleWrapper : IConsoleWrapper
    {
        /// <inheritdoc />
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }

}
