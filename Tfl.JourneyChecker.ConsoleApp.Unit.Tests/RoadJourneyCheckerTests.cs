using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Tfl.JourneyChecker.ConsoleApp.Enums;
using Tfl.JourneyChecker.ConsoleApp.Models;
using Tfl.JourneyChecker.ConsoleApp.Queries;
using Tfl.JourneyChecker.ConsoleApp.Wrappers;
using Xunit;

namespace Tfl.JourneyChecker.ConsoleApp.Unit.Tests
{
    public class RoadJourneyCheckerTests
    {
        [Fact]
        public async Task GivenAValidRoadId_WhenClientRun_ShouldReturnDisplayNameAndXeroExitCode()
        {
            // Arrange
            var roadId = "A2";
            var displayName = "A2";
            var expectedMessage = $"The status of the {displayName} is as follows";
            var loggerMock = new Mock<ILogger<RoadJourneyChecker>>();
            var consoleWrapperMock = new Mock<IConsoleWrapper>();

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(It.Is<GetRoadStatusRequestQuery>(x => x.RoadId == roadId), It.IsAny<CancellationToken>())).
                ReturnsAsync(new Result<RoadStatus>(
                new RoadStatus
                {
                    Id = roadId,
                    DisplayName = displayName,
                    StatusSeverity = "Good",
                }));
            var roadManagementProcessor = new RoadJourneyChecker(loggerMock.Object, mediatorMock.Object, consoleWrapperMock.Object);

            // Act
            var response = await roadManagementProcessor.CheckStatus(roadId);

            // Assert 
            response.ShouldBe((int)ResultStatus.Success);
            consoleWrapperMock.Verify(x => x.WriteLine(It.Is<string>(y => y == expectedMessage)), Times.Once);
        }

        [Fact]
        public async Task GivenAValidRoadId_WhenClientRun_ShouldReturnStatusSeverityAndXeroExitCode()
        {
            // Arrange
            var roadId = "A2";
            var displayName = "A2";
            var statusSeverity = "Good";
            var expectedMessage = $"Road Status is {statusSeverity}";

            var loggerMock = new Mock<ILogger<RoadJourneyChecker>>();
            var consoleWrapperMock = new Mock<IConsoleWrapper>();

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(It.Is<GetRoadStatusRequestQuery>(x => x.RoadId == roadId), It.IsAny<CancellationToken>())).
                ReturnsAsync(new Result<RoadStatus>(
                new RoadStatus
                {
                    Id = roadId,
                    DisplayName = displayName,
                    StatusSeverity = statusSeverity,
                    StatusSeverityDescription = "No Exceptional Delays"
                }));
            var roadManagementProcessor = new RoadJourneyChecker(loggerMock.Object, mediatorMock.Object, consoleWrapperMock.Object);

            // Act
            var response = await roadManagementProcessor.CheckStatus(roadId);

            // Assert 
            response.ShouldBe((int)ResultStatus.Success);
            consoleWrapperMock.Verify(x => x.WriteLine(It.Is<string>(y => y == expectedMessage)), Times.Once);
        }

        [Fact]
        public async Task GivenAValidRoadId_WhenClientRun_ShouldReturnStatusSeverityDescriptionAndXeroExitCode()
        {
            // Arrange
            var roadId = "A2";
            var displayName = "A2";
            var statusSeverity = "Good";
            var statusSeverityDescription = "No Exceptional Delays";
            var expectedMessage = $"Road Status Description is {statusSeverityDescription}";

            var loggerMock = new Mock<ILogger<RoadJourneyChecker>>();
            var consoleWrapperMock = new Mock<IConsoleWrapper>();
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(It.Is<GetRoadStatusRequestQuery>(x => x.RoadId == roadId), It.IsAny<CancellationToken>())).
                ReturnsAsync(new Result<RoadStatus>(
                new RoadStatus
                {
                    Id = roadId,
                    DisplayName = displayName,
                    StatusSeverity = statusSeverity,
                    StatusSeverityDescription = statusSeverityDescription
                }));
            var roadManagementProcessor = new RoadJourneyChecker(loggerMock.Object, mediatorMock.Object, consoleWrapperMock.Object);

            // Act
            var response = await roadManagementProcessor.CheckStatus(roadId);

            // Assert 
            response.ShouldBe((int)ResultStatus.Success);
            consoleWrapperMock.Verify(x => x.WriteLine(It.Is<string>(y => y == expectedMessage)), Times.Once);
        }

        [Fact]
        public async Task GivenInValidRoadId_WhenClientRun_ShouldReturnAnErrorMessageAndNonXeroExitCode()
        {
            // Arrange
            var roadId = "A2";
            var expectedMessage = $"{roadId} is not a valid road";

            var loggerMock = new Mock<ILogger<RoadJourneyChecker>>();
            var consoleWrapperMock = new Mock<IConsoleWrapper>();

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(It.Is<GetRoadStatusRequestQuery>(x => x.RoadId == roadId), It.IsAny<CancellationToken>())).
                ReturnsAsync(new Result<RoadStatus>(ResultStatus.NotFound, expectedMessage));
            var roadManagementProcessor = new RoadJourneyChecker(loggerMock.Object, mediatorMock.Object, consoleWrapperMock.Object);

            // Act
            var response = await roadManagementProcessor.CheckStatus(roadId);

            // Assert 
            response.ShouldBe((int)ResultStatus.NotFound);
            consoleWrapperMock.Verify(x => x.WriteLine(It.Is<string>(y => y == expectedMessage)), Times.Once);
        }

        [Fact]
        public async Task GivenEmptyRoadId_WhenClientRun_ShouldReturnAnErrorMessageAndNonXeroExitCode()
        {
            // Arrange 
            var expectedMessage = "Invalid Argument. Please pass the road id on running the application";

            var loggerMock = new Mock<ILogger<RoadJourneyChecker>>();
            var consoleWrapperMock = new Mock<IConsoleWrapper>();
            var mediatorMock = new Mock<IMediator>();
            var roadManagementProcessor = new RoadJourneyChecker(loggerMock.Object, mediatorMock.Object, consoleWrapperMock.Object);

            // Act
            var response = await roadManagementProcessor.CheckStatus(string.Empty);

            // Assert 
            response.ShouldBe((int)ResultStatus.ValidationError);
            consoleWrapperMock.Verify(x => x.WriteLine(It.Is<string>(y => y == expectedMessage)), Times.Once);
        }

        [Fact]
        public async Task GivenNullRoadId_WhenClientRun_ShouldReturnAnErrorMessageAndNonXeroExitCode()
        {
            // Arrange 
            var expectedMessage = "Invalid Argument. Please pass the road id on running the application";

            var loggerMock = new Mock<ILogger<RoadJourneyChecker>>();
            var consoleWrapperMock = new Mock<IConsoleWrapper>();
            var mediatorMock = new Mock<IMediator>();
            var roadManagementProcessor = new RoadJourneyChecker(loggerMock.Object, mediatorMock.Object, consoleWrapperMock.Object);

            // Act
            var response = await roadManagementProcessor.CheckStatus(null);

            // Assert 
            response.ShouldBe((int)ResultStatus.ValidationError);
            consoleWrapperMock.Verify(x => x.WriteLine(It.Is<string>(y => y == expectedMessage)), Times.Once);
        }

        [Fact]
        public async Task GivenErrorOnGettingRoadDetail_WhenClientRun_ShouldReturnAnErrorMessageAndNonXeroExitCode()
        {
            // Arrange
            var roadId = "A2";
            var exception = new System.Exception("error");
            var expectedMessage = $"Error on processing road status request";

            var loggerMock = new Mock<ILogger<RoadJourneyChecker>>();
            var consoleWrapperMock = new Mock<IConsoleWrapper>();
            var mediatorMock = new Mock<IMediator>();

            mediatorMock.Setup(x => x.Send(It.Is<GetRoadStatusRequestQuery>(x => x.RoadId == roadId), It.IsAny<CancellationToken>())).
                ThrowsAsync(exception);
            var roadManagementProcessor = new RoadJourneyChecker(loggerMock.Object, mediatorMock.Object, consoleWrapperMock.Object);

            // Act
            var response = await roadManagementProcessor.CheckStatus(roadId);

            // Assert 
            response.ShouldBe((int)ResultStatus.GeneralError);
            consoleWrapperMock.Verify(x => x.WriteLine(It.Is<string>(y => y.Contains(expectedMessage))), Times.Once);
        }
    }
}