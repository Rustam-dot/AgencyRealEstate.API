using System.Net.Http.Json;
using AgencyRealEstate.WebUI.Models; 

namespace AgencyRealEstate.WebUI.Services;

public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http)
    {
        _http = http;
    }


    public async Task<List<PropertyDto>> GetPropertiesAsync()
    {
        return await _http.GetFromJsonAsync<List<PropertyDto>>("properties");
        
    }

        public async Task<PropertyDto> GetPropertyAsync(int id)
        {
            return await _http.GetFromJsonAsync<PropertyDto>($"properties/{id}");
        }
    public async Task CreateShowingAsync(CreateShowingRequest request)
    {
        var response = await _http.PostAsJsonAsync("showings", request);
        response.EnsureSuccessStatusCode();
    }

}