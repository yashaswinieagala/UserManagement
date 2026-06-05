using System.Net.Http.Json;
using System.Text.Json;
using UserManagement.Shared.Models;

namespace UserManagement.Client.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public UserService(HttpClient http) => _http = http;

        public async Task<List<UserDetail>> GetAllAsync()
        {
            try { return await _http.GetFromJsonAsync<List<UserDetail>>("api/users") ?? new(); }
            catch { return new(); }
        }

        public async Task<UserDetail?> GetByIdAsync(int id)
        {
            try { return await _http.GetFromJsonAsync<UserDetail>($"api/users/{id}"); }
            catch { return null; }
        }

        public async Task<List<UserDetail>> SearchAsync(string? term, string? department, bool? isActive)
        {
            try
            {
                var parts = new List<string>();
                if (!string.IsNullOrWhiteSpace(term)) parts.Add($"term={Uri.EscapeDataString(term)}");
                if (!string.IsNullOrWhiteSpace(department)) parts.Add($"department={Uri.EscapeDataString(department)}");
                if (isActive.HasValue) parts.Add($"isActive={isActive.Value.ToString().ToLower()}");
                var url = "api/users/search" + (parts.Any() ? "?" + string.Join("&", parts) : "");
                return await _http.GetFromJsonAsync<List<UserDetail>>(url) ?? new();
            }
            catch { return new(); }
        }

        public async Task<List<string>> GetDepartmentsAsync()
        {
            try { return await _http.GetFromJsonAsync<List<string>>("api/users/departments") ?? new(); }
            catch { return new(); }
        }

        public async Task<UserStats?> GetStatsAsync()
        {
            try { return await _http.GetFromJsonAsync<UserStats>("api/users/stats", _json); }
            catch { return null; }
        }

        public async Task<(bool Success, UserDetail? Data, string Error)> CreateAsync(UserDetail user)
        {
            try
            {
                var res = await _http.PostAsJsonAsync("api/users", user);
                if (res.IsSuccessStatusCode)
                    return (true, await res.Content.ReadFromJsonAsync<UserDetail>(), string.Empty);
                var err = await res.Content.ReadAsStringAsync();
                return (false, null, err);
            }
            catch (Exception ex) { return (false, null, ex.Message); }
        }

        public async Task<(bool Success, UserDetail? Data, string Error)> UpdateAsync(int id, UserDetail user)
        {
            try
            {
                var res = await _http.PutAsJsonAsync($"api/users/{id}", user);
                if (res.IsSuccessStatusCode)
                    return (true, await res.Content.ReadFromJsonAsync<UserDetail>(), string.Empty);
                var err = await res.Content.ReadAsStringAsync();
                return (false, null, err);
            }
            catch (Exception ex) { return (false, null, ex.Message); }
        }

        public async Task<(bool Success, string Error)> DeleteAsync(int id)
        {
            try
            {
                var res = await _http.DeleteAsync($"api/users/{id}");
                return res.IsSuccessStatusCode ? (true, string.Empty) : (false, "Delete failed.");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }
    }
}
