using FirebaseAdmin.Auth;
using Firebase.Auth;
using Google.Apis.Auth.OAuth2;
using Firebase.Auth.Providers;
using FirebaseAdmin;
using SocialNetwork.Service.Implement;
using SocialNetwork.ExceptionModel;

namespace SocialNetwork.Firebase
{
    public class FirebaseAuthManager
    {

        public async Task<string> CreateFirebaseUserAsync(string email, string password)
        {
            var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
            {
                Email = email,
                Password = password
            });

            return userRecord.Uid;
        }
        public async Task ChangePasswordByEmailAsync(string email, string newPassword)
        {
            var user = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(email);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var args = new UserRecordArgs
            {
                Uid = user.Uid,
                Password = newPassword
            };

            await FirebaseAuth.DefaultInstance.UpdateUserAsync(args);
        }
        public async Task<string> GetFirebaseTokenByEmailAsync(string email)
        {
            try
            {
                // Lấy thông tin người dùng từ Firebase Authentication bằng email
                var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);

                if (user == null)
                {
                    throw new Exception("Không tìm thấy người dùng với email đã cung cấp.");
                }

                // Tạo custom token cho người dùng
                var customToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(user.Uid);

                // Trả về custom token
                return customToken;
            }
            catch (Exception ex)
            {
                // Xử lý khi có lỗi xảy ra
                throw new BadRequestException("Không thể tạo Firebase token. Lỗi: " + ex.Message);
            }
        }

    }

}
