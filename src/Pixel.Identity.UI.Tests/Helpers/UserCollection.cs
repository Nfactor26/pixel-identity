using System.Collections.Generic;
using System.Security.Claims;

namespace Pixel.Identity.UI.Tests.Helpers;

internal static class UserCollection
{
    private static readonly List<User> users = new();

    static UserCollection()
    {
        users.Add(new User("test_user_1@pixel.com", "tesT-useR-secreT-1"));
        users.Add(new User("test_user_2@pixel.com", "tesT-useR-secreT-2"));
    }

    public static IEnumerable<User> GetAllUsers() => users;
}

internal class User
{
    public string Email { get; set; }

    public string Password { get; set; }

    public List<string> Roles { get; private set; } = new ();

    public List<Claim> Claims { get; private set; } = new ();

    public User(string email, string password)
    {
        this.Email = email;
        this.Password = password;
    }
}

