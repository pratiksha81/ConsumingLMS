using System.Text;
using System.Text.Json;
using ConsumingLMS.Services;
using ConsumingLMS.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ConsumingLMS.Services.Author
{
    public class AuthorService : IAuthorService
    {
        private readonly HttpClient _httpClient;

        public AuthorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Get all authors from the API
        public async Task<IEnumerable<Auth>> GetAllAuthorsAsync()
        {
            var response = await _httpClient.GetAsync("https://localhost:7178/api/Authors"); 
            if (response.IsSuccessStatusCode)
            {
                var authors = await response.Content.ReadFromJsonAsync<IEnumerable<Auth>>();
                return authors ?? new List<Auth>(); // Return an empty list if authors is null
            }

            return new List<Auth>(); // Return an empty list if the API call fails
        }


        // Get a specific author by ID
        public async Task<Auth> GetAuthorByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7178/api/Authors/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Auth>(content);
            }
            return null;
        }

        // Add a new author
        public async Task<bool> AddAuthorAsync(Auth author)
        {
            var content = new StringContent(JsonSerializer.Serialize(author), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://localhost:7178/api/Authors", content);
            return response.IsSuccessStatusCode;
        }

        // Update an existing author
        public async Task<bool> UpdateAuthorAsync(Auth author)
        {
            var content = new StringContent(JsonSerializer.Serialize(author), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"https://localhost:7178/api/Authors/{author.AuthorID}", content);
            return response.IsSuccessStatusCode;
        }



        // Delete an author
        public async Task<bool> DeleteAuthorAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"https://localhost:7178/api/Authors/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
