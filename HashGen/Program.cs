using Microsoft.AspNetCore.Identity;

// Create a dummy user class for the hasher
public class DummyUser { }

class Program
{
    static void Main(string[] args)
    {
        var hasher = new PasswordHasher<DummyUser>();
        var hash = hasher.HashPassword(new DummyUser(), "P@ssword1");

        Console.WriteLine("Generated hash for 'P@ssword1':");
        Console.WriteLine(hash);
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
