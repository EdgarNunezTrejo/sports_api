using Microsoft.EntityFrameworkCore;
using sports_api.Data;
using sports_api.Interfaces;
using sports_api.Models;

namespace sports_api.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Email == email.ToLower());
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(string email)
    {
        return await context.Users
            .AnyAsync(u => u.Email == email.ToLower());
    }

    public async Task<User> CreateAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
        return user;
    }
}