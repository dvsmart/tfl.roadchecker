using Newtonsoft.Json;
using System.Net;

namespace Tfl.JourneyChecker.ConsoleApp.Models
{
    public class ApiErrorModel
    {
        /// <summary>
        ///  Api type
        /// </summary>
        [JsonProperty("$type")]
        public string Type { get; set; }

        /// <summary>
        /// Api invoked timestamp
        /// </summary>
        public DateTime TimestampUtc { get; set; }

        /// <summary>
        /// Api Exception type
        /// </summary>
        public string ExceptionType { get; set; }

        /// <summary>
        /// Http status code
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }

        /// <summary>
        /// Http status
        /// </summary>
        public string HttpStatus { get; set; }

        /// <summary>
        /// Api relative URI
        /// </summary>
        public string RelativeUri { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }
    }
}
