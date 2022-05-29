using System.Collections.Generic;

namespace Pixel.Identity.UI.Tests.Helpers;

internal static class UserCollection
{
    private static readonly List<User> users = new();

    static UserCollection()
    {
        users.Add(new User("test_user_1@pixel.com", "tesT-useR-secreT-1"));
        users.Add(new User("test_user_2@pixel.com", "tesT-useR-secreT-2"));
        users.Add(new User("test_user_3@pixel.com", "tesT-useR-secreT-3"));
        users.Add(new User("test_user_4@pixel.com", "tesT-useR-secreT-4"));
        users.Add(new User("test_user_5@pixel.com", "tesT-useR-secreT-5"));
        users.Add(new User("test_user_6@pixel.com", "tesT-useR-secreT-6"));
        users.Add(new User("test_user_7@pixel.com", "tesT-useR-secreT-7"));
        users.Add(new User("test_user_8@pixel.com", "tesT-useR-secreT-8"));
        users.Add(new User("test_user_9@pixel.com", "tesT-useR-secreT-9"));
        users.Add(new User("test_user_10@pixel.com", "tesT-useR-secreT-10"));
        users.Add(new User("test_user_11@pixel.com", "tesT-useR-secreT-11"));
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

