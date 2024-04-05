using FirebaseAdmin.Auth;
using SocialNetwork.Entity;
using SocialNetwork.Repository;

namespace SocialNetwork.Helpers
{

    public static class AdminInitializer
    {
        public static async Task InitializeAdminAccount()
        {
            var adminEmail = "admin@example.com"; // Thay đổi thành email admin mong muốn
            var adminPassword = "adminpassword"; // Thay đổi thành mật khẩu admin mong muốn

            try
            {
                var adminUser = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(adminEmail);
                if (adminUser != null)
                {
                    Console.WriteLine("Admin user already exists.");
                }
                else
                {
                    var createAdminArgs = new UserRecordArgs
                    {
                        Email = adminEmail,
                        Password = adminPassword,
                        DisplayName = "Admin", // Thay đổi thành tên hiển thị admin mong muốn
                        EmailVerified = true
                    };
                    var createdAdminUser = await FirebaseAuth.DefaultInstance.CreateUserAsync(createAdminArgs);
                    Console.WriteLine("Admin user created successfully.");
                }
            }
            catch (FirebaseAuthException ex)
            {
                Console.WriteLine($"Error creating admin user: {ex.Message}");
            }
        }
    }
}
