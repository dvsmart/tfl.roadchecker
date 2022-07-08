namespace Tfl.JourneyChecker.ConsoleApp
{
    public interface IJourneyChecker
    {
        /// <summary>
        /// check status of the journey
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<int> CheckStatus(string id);
    }
}
