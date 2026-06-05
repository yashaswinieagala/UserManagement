using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using UserManagement.Server.Data;
using UserManagement.Server.Hubs;
using UserManagement.Shared.Models;

namespace UserManagement.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IHubContext<UserHub> _hub;

        public UsersController(AppDbContext db, IHubContext<UserHub> hub)
        {
            _db = db;
            _hub = hub;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDetail>>> GetAll()
        {
            return await _db.Users.OrderBy(u => u.UserName).ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDetail>> GetById(int id)
        {
            var user = await _db.Users.FindAsync(id);
            return user == null ? NotFound() : user;
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<UserDetail>>> Search(
            [FromQuery] string? term,
            [FromQuery] string? department,
            [FromQuery] bool? isActive)
        {
            var q = _db.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(term))
                q = q.Where(u => u.UserName.Contains(term) || u.Email.Contains(term));
            if (!string.IsNullOrWhiteSpace(department))
                q = q.Where(u => u.Department == department);
            if (isActive.HasValue)
                q = q.Where(u => u.IsActive == isActive.Value);
            return await q.OrderBy(u => u.UserName).ToListAsync();
        }

        [HttpGet("departments")]
        public async Task<ActionResult<List<string>>> GetDepartments()
        {
            return await _db.Users.Select(u => u.Department).Distinct().OrderBy(d => d).ToListAsync();
        }

        [HttpGet("stats")]
        public async Task<ActionResult<UserStats>> GetStats()
        {
            var all = await _db.Users.ToListAsync();
            return new UserStats
            {
                Total = all.Count,
                Active = all.Count(u => u.IsActive),
                Inactive = all.Count(u => !u.IsActive),
                AverageAge = all.Count > 0 ? Math.Round(all.Average(u => (double)u.Age), 1) : 0,
                ByDepartment = all
                    .GroupBy(u => u.Department)
                    .Select(g => new DeptCount { Department = g.Key, Count = g.Count() })
                    .OrderByDescending(d => d.Count)
                    .ToList()
            };
        }

        [HttpPost]
        public async Task<ActionResult<UserDetail>> Create([FromBody] UserDetail user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            user.Id = 0;
            user.CreatedAt = DateTime.UtcNow;
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            await _hub.Clients.All.SendAsync("ReceiveNotification", new UserNotification
            {
                Action = "Added",
                Message = $"New user '{user.UserName}' was added.",
                User = user,
                Timestamp = DateTime.UtcNow
            });
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<UserDetail>> Update(int id, [FromBody] UserDetail user)
        {
            if (id != user.Id) return BadRequest("ID mismatch.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var existing = await _db.Users.FindAsync(id);
            if (existing == null) return NotFound();
            existing.UserName = user.UserName;
            existing.Email = user.Email;
            existing.Age = user.Age;
            existing.Department = user.Department;
            existing.IsActive = user.IsActive;
            await _db.SaveChangesAsync();
            await _hub.Clients.All.SendAsync("ReceiveNotification", new UserNotification
            {
                Action = "Updated",
                Message = $"User '{existing.UserName}' was updated.",
                User = existing,
                Timestamp = DateTime.UtcNow
            });
            return existing;
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            await _hub.Clients.All.SendAsync("ReceiveNotification", new UserNotification
            {
                Action = "Deleted",
                Message = $"User '{user.UserName}' was deleted.",
                Timestamp = DateTime.UtcNow
            });
            return NoContent();
        }
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
