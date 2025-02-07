using System.Net.Http.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Supabase.Gotrue;

public class AuthService
{
    private readonly HttpClient _httpClient;
    public event Action? OnAuthStateChanged;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { Email = email, Password = password });
        if (response.IsSuccessStatusCode)
        {
            OnAuthStateChanged?.Invoke(); // Notify UI
            return true;
        }
        return false;
    }

    public async Task<bool> RegisterAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", new { Email = email, Password = password });
        if (response.IsSuccessStatusCode)
        {
            OnAuthStateChanged?.Invoke();
            return true;
        }
        return false;
    }

    public async Task LogoutAsync()
    {
        await _httpClient.PostAsync("api/auth/logout", null);
        OnAuthStateChanged?.Invoke(); // Notify UI
    }

    public async Task<bool> RecoverPasswordAsync(string email)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/recover-password", new { Email = email });
        return response.IsSuccessStatusCode;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        try
        {
            var user = await _httpClient.GetFromJsonAsync<User>("api/auth/current-user");
            return user;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching user: {ex.Message}");
            return null;
        }
    }

    // ✅ New method to get only the user's email
    public async Task<string?> GetUserEmailAsync()
    {
        var user = await GetCurrentUserAsync();
        return user?.Email;
    }

    public class User
    {
        public string Email { get; set; } = string.Empty;
    }
}
