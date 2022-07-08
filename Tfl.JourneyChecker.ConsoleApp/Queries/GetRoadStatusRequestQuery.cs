using MediatR;
using Tfl.JourneyChecker.ConsoleApp.Models;

namespace Tfl.JourneyChecker.ConsoleApp.Queries
{
    public class GetRoadStatusRequestQuery : IRequest<Result<RoadStatus?>>
    {
        /// <summary>
        /// Unique Id of the road
        /// </summary>
        public string RoadId { get; set; }

        public GetRoadStatusRequestQuery(string roadId)
        {
            RoadId = roadId;
        }
    }
}
