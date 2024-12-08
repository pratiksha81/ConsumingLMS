using ConsumingLMS.Models.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsumingLMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Constructor to inject HttpClient and IHttpContextAccessor
        public AccountController(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            // Set the base address for HttpClient (could be moved to Startup or Program.cs)
            _httpClient.BaseAddress = new Uri("https://localhost:7178/api/");
        }

        // Login Page
        [HttpGet]
        public IActionResult Login() => View();

        // Post Login Data
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Validate the model state
            if (!ModelState.IsValid)
                return View(model);

            // Send POST request to login endpoint
            var response = await SendPostRequest("Auth/login", model);

            // Extract the JWT token from the response
            var token = await ExtractTokenFromResponse(response);
            if (!string.IsNullOrEmpty(token))
            {

                SetJwtCookie(token);
                // Store the token in session
                _httpContextAccessor.HttpContext.Session.SetString("JwtToken", token);
                // Redirect to home page after successful login
                return RedirectToAction("Index", "Home");
            }

            // If token is not found, show error message
            ViewBag.ErrorMessage = "Invalid username or password.";
            return View(model);
        }

        // Sign-Up Page
        [HttpGet]
        public IActionResult SignUp() => View();

        // Post Sign-Up Data
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            // Validate the model state
            if (!ModelState.IsValid)
                return View(model);

            // Send POST request to sign-up endpoint
            var response = await SendPostRequest("Auth/signup", model);

            // Check if the registration was successful
            if (response.IsSuccessStatusCode)
            {
                // Redirect to Login page with a success message
                ViewBag.Message = "Registration successful. Please log in.";
                return RedirectToAction("Login");
            }

            // If registration failed, show error message
            ViewBag.ErrorMessage = "Registration failed. Please try again.";
            return View(model);
        }

        // Helper Method: Send POST request
        private async Task<HttpResponseMessage> SendPostRequest(string endpoint, object data)
        {
            // Convert data to JSON
            var jsonData = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            // Send the POST request and return the response
            return await _httpClient.PostAsync(endpoint, content);
        }

        // Helper Method: Extract token from the response
        private async Task<string> ExtractTokenFromResponse(HttpResponseMessage response)
        {
            // Check if the response is successful
            if (response.IsSuccessStatusCode)
            {
                // Read response content as string
                var result = await response.Content.ReadAsStringAsync();
                // Deserialize the response content to extract the token
                var tokenResponse = JsonConvert.DeserializeObject<dynamic>(result);
                return tokenResponse?.token;
            }
            return null;
        }
        private void SetJwtCookie(string token) { CookieOptions cookieOptions = new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict, Expires = DateTime.UtcNow.AddDays(7) }; Response.Cookies.Append("jwtToken", token, cookieOptions); }
    }

}
