using MediatR;
using Microsoft.Extensions.Logging;
using Tfl.JourneyChecker.ConsoleApp.Enums;
using Tfl.JourneyChecker.ConsoleApp.Queries;
using Tfl.JourneyChecker.ConsoleApp.Wrappers;

namespace Tfl.JourneyChecker.ConsoleApp
{
    public class RoadJourneyChecker : IJourneyChecker
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RoadJourneyChecker> _logger;
        private readonly IConsoleWrapper _consoleWrapper;

        public RoadJourneyChecker(ILogger<RoadJourneyChecker> logger,
            IMediator mediator,
            IConsoleWrapper consoleWrapper)
        {
            _logger = logger;
            _mediator = mediator;
            _consoleWrapper = consoleWrapper;
        }

        /// <summary>
        /// Process the action to get journey status from the TFL API
        /// </summary>
        /// <param name="roadId"></param>
        /// <returns></returns>
        public async Task<int> CheckStatus(string roadId)
        {
            string? errorMessage;

            // Return if number of argument is lesser than 1
            if (string.IsNullOrWhiteSpace(roadId))
            {
                errorMessage = "Invalid Argument";
                _logger.LogError(errorMessage);
                _consoleWrapper.WriteLine($"{errorMessage}. Please pass the road id on running the application");
                return (int)Models.ResultStatus.ValidationError;
            }
            try
            {
                var response = await _mediator.Send(new GetRoadStatusRequestQuery(roadId));

                // Return error if the status is null. 
                if (response == null || response.ResultStatus != Models.ResultStatus.Success)
                {
                    errorMessage = response?.ErrorMessage ?? "Unexpected error ocurred on calling get journey status query.";
                    _logger.LogError(errorMessage);
                    _consoleWrapper.WriteLine(errorMessage);
                    return response == null ?
                        (int)Models.ResultStatus.GeneralError : (int)response.ResultStatus;
                } 
                _consoleWrapper.WriteLine("Completed TFL Journey status checker");
                _consoleWrapper.WriteLine(Environment.NewLine);
                _consoleWrapper.WriteLine($"The status of the {response.Data.DisplayName} is as follows");
                _consoleWrapper.WriteLine($"Road Status is {response.Data.StatusSeverity}");
                _consoleWrapper.WriteLine($"Road Status Description is {response.Data.StatusSeverityDescription}");
            }
            catch (Exception ex)
            {
                errorMessage = "Error on processing road status request";
                _logger.LogError(ex, errorMessage);
                _consoleWrapper.WriteLine($"{errorMessage}: {ex.ToString()}");
                return (int)Models.ResultStatus.GeneralError;
            }
            return (int)Models.ResultStatus.Success;
        }
    }
}
