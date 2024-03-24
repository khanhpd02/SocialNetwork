using Firebase.Auth;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Google.Cloud.Storage.V1;
using System;
using System.Threading.Tasks;
namespace SocialNetwork.FirebaseAD
{
    public class AuthManager
    {
        public static FirebaseApp FirebaseAppInstance;
        private FirestoreDb firestoreDb;

        

        public async Task CreateUserAsync(string displayName, string email, string password, string photoPath)
        {
            try
            {
                FirebaseInitializer.InitializeFirebaseApp();
                // Tạo người dùng trong Firebase Authentication
                var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
                {
                    DisplayName = displayName,
                    Email = email,
                    Password = password
                });

                FirestoreDb db = FirestoreDb.Create("chaapprj");

                // Tải ảnh lên Firebase Storage
                var storage = StorageClient.Create();
                var bucketName = "chaapprj.appspot.com";
                using (var httpClient = new HttpClient())
                {
                    // Tải hình ảnh từ URL
                    var imageBytes = await httpClient.GetByteArrayAsync(photoPath);

                    // Tải lên hình ảnh đã tải về lên Cloudinary
                    var storageObject = await storage.UploadObjectAsync(
                        bucketName,
                        $"{displayName}_{DateTime.Now.Ticks}", // Sử dụng tên tệp được định danh duy nhất
                        "image/jpeg", // Hoặc kiểu MIME phù hợp cho hình ảnh của bạn
                        new MemoryStream(imageBytes) // Sử dụng MemoryStream để tải lên byte của hình ảnh
                    );
                    var photoUrl = storageObject.MediaLink;

                    // Cập nhật hồ sơ người dùng
                    await FirebaseAuth.DefaultInstance.UpdateUserAsync(new UserRecordArgs
                    {
                        Uid = userRecord.Uid,
                        DisplayName = displayName,
                        PhotoUrl = photoUrl
                    });

                    // Thêm người dùng vào Firestore
                    await AddUserToFirestore(userRecord.Uid, displayName, email, photoUrl);

                    // Tạo các cuộc trò chuyện trống cho người dùng trên Firestore
                    await CreateEmptyUserChats(userRecord.Uid);
                }

                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                // Xử lý lỗi một cách thích hợp
            }
        }

        private async Task AddUserToFirestore(string userId, string displayName, string email, string photoUrl)
        {
            try
            {
                var userDocRef = firestoreDb.Collection("users").Document(userId);
                var user = new
                {
                    Uid = userId,
                    DisplayName = displayName,
                    Email = email,
                    PhotoUrl = photoUrl
                };
                await userDocRef.SetAsync(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                throw;
            }
        }

        private async Task CreateEmptyUserChats(string userId)
        {
            try
            {
                var userChatsDocRef = firestoreDb.Collection("userChats").Document(userId);
                await userChatsDocRef.SetAsync(new { });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                throw;
            }
        }
    }
}