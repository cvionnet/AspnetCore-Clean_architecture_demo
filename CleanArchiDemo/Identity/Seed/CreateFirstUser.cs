using System.Threading.Tasks;

namespace Identity.Seed;

public static class UserCreator
{
    /// <summary>
    /// To create an initial user (called in Api.Program)
    /// </summary>
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
    {
        var applicationUser = new ApplicationUser
        {
            FirstName = "Demo",
            LastName = "USER",
            UserName = "demo_user",
            Email = "demo@test.com",
            EmailConfirmed = true
        };

        var user = await userManager.FindByEmailAsync(applicationUser.Email);
        if (user == null)
        {
            await userManager.CreateAsync(applicationUser, "Demo_user&01");
        }
    }
}