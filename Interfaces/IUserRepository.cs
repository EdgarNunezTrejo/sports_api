using sports_api.Models;

namespace sports_api.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(string email);
    Task<User> CreateAsync(User user);
}