using UserManagement.Shared.Models;

namespace UserManagement.Client.Services
{
    public interface IUserService
    {
        Task<List<UserDetail>> GetAllAsync();
        Task<UserDetail?> GetByIdAsync(int id);
        Task<List<UserDetail>> SearchAsync(string? term, string? department, bool? isActive);
        Task<List<string>> GetDepartmentsAsync();
        Task<UserStats?> GetStatsAsync();
        Task<(bool Success, UserDetail? Data, string Error)> CreateAsync(UserDetail user);
        Task<(bool Success, UserDetail? Data, string Error)> UpdateAsync(int id, UserDetail user);
        Task<(bool Success, string Error)> DeleteAsync(int id);
    }

    public class UserStats
    {
        public int Total { get; set; }
        public int Active { get; set; }
        public int Inactive { get; set; }
        public double AverageAge { get; set; }
        public List<DeptCount> ByDepartment { get; set; } = new();
    }

    public class DeptCount
    {
        public string Department { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
