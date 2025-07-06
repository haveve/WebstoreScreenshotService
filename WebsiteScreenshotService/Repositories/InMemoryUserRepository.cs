using System.Collections.Concurrent;
using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Repositories;

/// <summary>
/// Provides an in-memory implementation of the <see cref="IUserRepository"/> interface for managing users and their subscriptions.
/// </summary>
public class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentBag<User> _users = new()
    {
        new User(new Guid("{bbb13e58-0cf0-4063-8b83-7e0bf15f7e4d}"),"Ivan","Pohoidash","i.pohoidash@gmail.com","abc123", SubscriptionPlan.GetRegularSubscriptionPlan())
    };

    private ReaderWriterLockSlim _subscriptionLock = new();

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
    public Task<User?> GetUserByIdAsync(Guid id)
    {
        return Task.FromResult(_users.FirstOrDefault(user => user.Id == id));
    }

    /// <summary>
    /// Creates a new user and adds them to the in-memory collection.
    /// </summary>
    /// <param name="user">The user to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created user if successful; otherwise, null.</returns>
    public Task<User?> CreateUserAsync(User user)
    {
        var userWithSameEmail = _users.FirstOrDefault(u => u.Email == user.Email);

        if (userWithSameEmail is not null)
            return Task.FromResult(null as User);

        _users.Add(user);
        return Task.FromResult<User?>(user);
    }

    /// <summary>
    /// Retrieves a user by their email and password.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
    public Task<User?> GetUserByEmailAndPasswordAsync(string email, string password)
        => Task.FromResult(_users.FirstOrDefault(user => user.Email == email && user.Password == password));
}

