using Supabase.Gotrue;
using System.Text.Json;
using Microsoft.JSInterop;

public class SupabaseAuthService
{
    private readonly Supabase.Client _client;

    public SupabaseAuthService()
    {
        var options = new Supabase.ClientOptions
        {
            AutoRefreshToken = true,
            PersistSession = false // No session persistence in server-side context
        };

        _client = new Supabase.Client(
            "https://zmbwovmoriuapqluaxjn.supabase.co",
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InptYndvdm1vcml1YXBxbHVheGpuIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzczNTExNzEsImV4cCI6MjA1MjkyNzE3MX0.1FeFwemTq-y6GPRMhN-DJf2DQ2-1qRtKA9qrShVDOEU",
            options
        );

        _client.InitializeAsync().Wait(); // Initialize synchronously for server context
    }

    // Login user
    public async Task<User?> LoginAsync(string email, string password)
    {
        try
        {
            var session = await _client.Auth.SignIn(email, password);
            return session?.User;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login failed: {ex.Message}");
            throw;
        }
    }

    // Register user
    public async Task<User?> RegisterAsync(string email, string password)
    {
        try
        {
            var session = await _client.Auth.SignUp(email, password);
            return session?.User;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Registration failed: {ex.Message}");
            throw;
        }
    }

    // Logout user
    public async Task LogoutAsync()
    {
        await _client.Auth.SignOut();
    }

    // Get currently logged-in user
    public User? GetCurrentUser()
    {
        return _client.Auth.CurrentUser;
    }

    // Send password recovery email
    public async Task SendPasswordRecoveryEmailAsync(string email)
    {
        try
        {
            await _client.Auth.ResetPasswordForEmail(email);
            Console.WriteLine($"Password recovery email sent to: {email}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Password recovery failed: {ex.Message}");
            throw;
        }
    }
}
