using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading;
using Tfl.JourneyChecker.ConsoleApp.Models;
using Tfl.JourneyChecker.ConsoleApp.Queries;

namespace Tfl.JourneyChecker.ConsoleApp.Handlers
{
    public class GetRoadStatusRequestHandler : IRequestHandler<GetRoadStatusRequestQuery, Result<RoadStatus>>
    {
        private readonly HttpClient _httpClient;
        private readonly TflSettings _tflSettings;

        public GetRoadStatusRequestHandler(HttpClient httpClient, IOptions<TflSettings> tflSettingOptions)
        {
            _tflSettings = tflSettingOptions.Value;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Handle the get road query request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Result<RoadStatus>> Handle(GetRoadStatusRequestQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = new Dictionary<string, string>()
                {
                    ["app_id"] = _tflSettings.AppId,
                    ["app_key"] = _tflSettings.AppKey
                };

                // Add query string parameter to include app id and app key.
                var uri = QueryHelpers.AddQueryString($"/Road/{request.RoadId}", query);

                // Add TFL api base url.
                _httpClient.BaseAddress = new Uri(_tflSettings.BaseUrl);
                var httpResponse = await _httpClient.GetAsync(uri);

                // Parse road response.
                if (httpResponse.IsSuccessStatusCode)
                {
                    var roadDetails = await ParseRoadResponse(httpResponse);
                    return new Result<RoadStatus>(roadDetails);
                }
                else
                {
                    var errorResponse = await ParseApiErrorResponse(httpResponse);
                    if (errorResponse.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return new Result<RoadStatus>(ResultStatus.NotFound, errorResponse.Message);
                    }
                    else 
                    {
                        return new Result<RoadStatus>(ResultStatus.HttpResponseError, errorResponse.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                return new Result<RoadStatus>(ResultStatus.GeneralError, ex.ToString());
            }
        }

        private static async Task<RoadStatus> ParseRoadResponse(HttpResponseMessage httpResponse)
        {
            try
            {
                // Read and parse the json.
                var content = await httpResponse.Content.ReadAsStringAsync();
                var roadList = string.IsNullOrEmpty(content)
                    ? null
                    : JsonConvert.DeserializeObject<IEnumerable<RoadStatus>>(content);

                return roadList?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error on parsing the road model {ex}");
            }
        }

        private static async Task<ApiErrorModel> ParseApiErrorResponse(HttpResponseMessage httpResponse)
        {
            try
            {
                // Read and parse the json.
                var content = await httpResponse.Content.ReadAsStringAsync();
                var errorModel = string.IsNullOrEmpty(content)
                    ? null
                    : JsonConvert.DeserializeObject<ApiErrorModel>(content);

                return errorModel;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error on parsing the api error model. Error detail: {ex}");
            }
        }
    }
}
