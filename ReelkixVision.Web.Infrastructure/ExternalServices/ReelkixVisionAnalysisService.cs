using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using ReelkixVision.Web.Application.DTOs;
using ReelkixVision.Web.Application.Interfaces;

namespace ReelkixVision.Web.Infrastructure.ExternalServices
{
    public class ReelkixVisionAnalysisService : IAnalysisService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ReelkixVisionAnalysisService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<ShoeAnalysisResultDto> AnalyzeImageAsync(string imageUrl)
        {
            // Retrieve the Node.js API URL from configuration.
            string apiUrl = _configuration["ReelkixVisionApi:Url"] ?? "http://localhost:3001/api/analyze";

            // Create the request payload.
            var requestData = new { imageUrl = imageUrl };

            // Send POST request to the Node.js API.
            var response = await _httpClient.PostAsJsonAsync(apiUrl, requestData);

            if (response.IsSuccessStatusCode)
            {
                // Deserialize the response to our DTO.
                var result = await response.Content.ReadFromJsonAsync<ShoeAnalysisResultDto>();
                return result ?? new ShoeAnalysisResultDto();
            }
            else
            {
                throw new Exception("Failed to get analysis from the Node.js API.");
            }
        }
    }
}