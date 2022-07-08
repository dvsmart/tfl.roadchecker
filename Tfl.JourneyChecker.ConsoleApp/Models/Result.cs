namespace Tfl.JourneyChecker.ConsoleApp.Models
{
    public class Result<T> where T : class
    {
        /// <summary>
        /// Return the error message of the result.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Return the result status
        /// </summary>
        public ResultStatus ResultStatus { get; set; }

        /// <summary>
        /// Return the data
        /// </summary>
        public T Data { get; set; }

        public Result(ResultStatus resultStatus, string erorMessage)
        {
            ErrorMessage = erorMessage;
            ResultStatus = resultStatus;
        }

        public Result(T data)
        {
            Data = data;
            ResultStatus = ResultStatus.Success;
        }

        public Result()
        {
            ResultStatus = ResultStatus.Success;
        }
    }
}
