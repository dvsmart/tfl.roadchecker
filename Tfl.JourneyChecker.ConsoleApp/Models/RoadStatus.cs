namespace Tfl.JourneyChecker.ConsoleApp.Models
{
    public class RoadStatus
    {
        /// <summary>
        /// Unique Id of the Road
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Disaply name of the road.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Road status severity ex: good or bad
        /// </summary>
        public string StatusSeverity { get; set; } = string.Empty;

        /// <summary>
        /// Indicate Road status severity description ex: No Exceptional Delays / Exceptional Delays
        /// </summary>
        public string StatusSeverityDescription { get; set; } = string.Empty;

    }
}
