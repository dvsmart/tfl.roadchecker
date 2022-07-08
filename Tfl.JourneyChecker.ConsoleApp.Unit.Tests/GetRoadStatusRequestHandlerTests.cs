using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Shouldly;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tfl.JourneyChecker.ConsoleApp.Handlers;
using Tfl.JourneyChecker.ConsoleApp.Models;
using Tfl.JourneyChecker.ConsoleApp.Queries;
using Xunit;

namespace Tfl.JourneyChecker.ConsoleApp.Unit.Tests
{
    public class GetRoadStatusRequestHandlerTests
    {

        [Fact]
        public async Task GivenAValidRoadId_WhenHandlerCalled_ShouldReturnRoadDetails()
        {
            // Arrange
            var roadId = "A2";
            var displayName = "A2";
            var statusSeverity = "Good";
            var statusSeverityDescription = "No Exceptional Delays";

            var mockedHttpClient = GetValidHttpClientWith200Status();
            var tflSettingMock = new Mock<IOptions<TflSettings>>();
            tflSettingMock.Setup(x => x.Value).Returns(
                new TflSettings
                {
                    BaseUrl = "https://tfl.journey.co.uk",
                    AppKey = "key",
                    AppId = "appid"
                }
            );
            var roadManagementProcessor = new GetRoadStatusRequestHandler(mockedHttpClient, tflSettingMock.Object);

            // Act
            var response = await roadManagementProcessor.Handle(new GetRoadStatusRequestQuery(roadId), CancellationToken.None);

            // Assert 
            response.ShouldNotBeNull();
            response.Data.Id.ShouldBe(roadId);
            response.Data.DisplayName.ShouldBe(displayName);
            response.Data.StatusSeverity.ShouldBe(statusSeverity);
            response.Data.StatusSeverityDescription.ShouldBe(statusSeverityDescription);
        }

        [Fact]
        public async Task GivenInValidRoadId_WhenHandlerCalled_ShouldReturnNullWithNotfoundStatus()
        {
            // Arrange
            var roadId = "A2"; 

            var mockedHttpClient = GetHttpClientWith404Status();
            var tflSettingMock = new Mock<IOptions<TflSettings>>();
            tflSettingMock.Setup(x => x.Value).Returns(
                new TflSettings
                {
                    BaseUrl = "https://tfl.journey.co.uk",
                    AppKey = "key",
                    AppId = "appid"
                }
            );
            var roadManagementProcessor = new GetRoadStatusRequestHandler(mockedHttpClient, tflSettingMock.Object);

            // Act
            var response = await roadManagementProcessor.Handle(new GetRoadStatusRequestQuery(roadId), CancellationToken.None);

            // Assert 
            response.ShouldNotBeNull();
            response.Data.ShouldBeNull();
            response.ResultStatus.ShouldBe(ResultStatus.NotFound );
            response.ErrorMessage.ShouldBe("The following road id is not recognised:A233");
        }
        
        [Fact]
        public async Task GivenInValidRoadId_WhenHandlerCalled_ShouldReturnNullWithHttpResponseError()
        {
            // Arrange
            var roadId = "A2";
             
            var mockedHttpClient = GetInValidHttpClientWith400Status();
            var tflSettingMock = new Mock<IOptions<TflSettings>>();
            tflSettingMock.Setup(x => x.Value).Returns(
                new TflSettings
                {
                    BaseUrl = "https://tfl.journey.co.uk",
                    AppKey = "key",
                    AppId = "appid"
                }
            );
            var roadManagementProcessor = new GetRoadStatusRequestHandler(mockedHttpClient, tflSettingMock.Object);

            // Act
            var response = await roadManagementProcessor.Handle(new GetRoadStatusRequestQuery(roadId), CancellationToken.None);

            // Assert 
            response.ShouldNotBeNull();
            response.Data.ShouldBeNull();
            response.ResultStatus.ShouldBe(ResultStatus.HttpResponseError );
            response.ErrorMessage.ShouldBe("bad request");
        }
        
        [Fact]
        public async Task GivenValidRoadId_WhenApiReturnInvalidJson_ShouldReturnNullGeneralError()
        {
            // Arrange
            var roadId = "A2";
             
            var mockedHttpClient = GetHttpClientWithInValidJsonContent();
            var tflSettingMock = new Mock<IOptions<TflSettings>>();
            tflSettingMock.Setup(x => x.Value).Returns(
                new TflSettings
                {
                    BaseUrl = "https://tfl.journey.co.uk",
                    AppKey = "key",
                    AppId = "appid"
                }
            );
            var roadManagementProcessor = new GetRoadStatusRequestHandler(mockedHttpClient, tflSettingMock.Object);

            // Act
            var response = await roadManagementProcessor.Handle(new GetRoadStatusRequestQuery(roadId), CancellationToken.None);

            // Assert 
            response.ShouldNotBeNull();
            response.Data.ShouldBeNull();
            response.ResultStatus.ShouldBe(ResultStatus.GeneralError); 
        }


        private HttpClient GetInValidHttpClientWith400Status()
        {
             var content = "{\"$type\":\"Tfl.Api.Presentation.Entities.ApiError,Tfl.Api.Presentation.Entities\",\"timestampUtc\":\"2017-11-21T14:37:39.7206118Z\",\"exceptionType\":\"BadRequestException\",\"httpStatusCode\":400,\"httpStatus\":\"NotFound\",\"relativeUri\":\"/Road/A233\",\"message\":\"bad request\"}";

            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(content)
                });
            return new HttpClient(mockMessageHandler.Object);

        }
        private HttpClient GetValidHttpClientWith200Status()
        {
            var content = "[{\"$type\":\"Tfl.Api.Presentation.Entities.RoadCorridor,Tfl.Api.Presentation.Entities\",\"id\":\"A2\",\"displayName\":\"A2\",\"statusSeverity\":\"Good\",\"statusSeverityDescription\":\"No Exceptional Delays\",\"bounds\":\"[[-0.0857,51.44091],[0.17118,51.49438]]\",\"envelope\":\"[[-0.0857,51.44091],[-0.0857,51.49438],[0.17118,51.49438],[0.17118,51.44091],[-0.0857,51.44091]]\",\"url\":\"/Road/a2\"}]";
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(content)
                });
            return new HttpClient(mockMessageHandler.Object);

        }
        private HttpClient GetHttpClientWith404Status()
        {
            var notFoundErrorContent = "{\"$type\":\"Tfl.Api.Presentation.Entities.ApiError,Tfl.Api.Presentation.Entities\",\"timestampUtc\":\"2017-11-21T14:37:39.7206118Z\",\"exceptionType\":\"EntityNotFoundException\",\"httpStatusCode\":404,\"httpStatus\":\"NotFound\",\"relativeUri\":\"/Road/A233\",\"message\":\"The following road id is not recognised:A233\"}";
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent(notFoundErrorContent)
                });
            return new HttpClient(mockMessageHandler.Object);

        }
        
        private HttpClient GetHttpClientWithInValidJsonContent()
        {
            var notFoundErrorContent = "{\"$type:\"Tfl.Api.Presentation.Entities.ApiError,Tfl.Api.Presentation.Entities\",\"timestampUtc\":\"2017-11-21T14:37:39.7206118Z\",\"exceptionType\":\"EntityNotFoundException\",\"httpStatusCode\":404,\"httpStatus\":\"NotFound\",\"relativeUri\":\"/Road/A233\",\"message\":\"The following road id is not recognised:A233\"}";
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent(notFoundErrorContent)
                });
            return new HttpClient(mockMessageHandler.Object);

        }
    }
}
