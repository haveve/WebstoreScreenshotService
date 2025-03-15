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
    /// Creates a new user and adds them to the in-memory collection.
    /// </summary>
    /// <param name="user">The user to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created user if successful; otherwise, null.</returns>
    public Task<User?> CreateUser(User user)
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

    /// <summary>
    /// Retrieves the subscription plan of a user by their ID.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the subscription plan if found; otherwise, null.</returns>
    public Task<SubscriptionPlan?> GetUserSubscription(Guid userId)
    {
        _subscriptionLock.EnterReadLock();

        var result = Task.FromResult(_users.FirstOrDefault(user => user.Id == userId)?.SubscriptionPlan);

        _subscriptionLock.ExitReadLock();

        return result;
    }

    /// <summary>
    /// Updates the subscription plan when a screenshot is made by the user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated subscription plan if found; otherwise, null.</returns>
    public Task<SubscriptionPlan?> ScreenshotWasMade(Guid userId)
    {
        _subscriptionLock.EnterWriteLock();

        var user = _users.FirstOrDefault(user => user.Id == userId);
        var subscriptionPlan = user?.SubscriptionPlan;

        if (subscriptionPlan != null && subscriptionPlan.ScreenshotLeft > 0)
            subscriptionPlan.ScreenshotWasMade();

        var result = Task.FromResult(subscriptionPlan);

        _subscriptionLock.ExitWriteLock();

        return result;
    }
}

