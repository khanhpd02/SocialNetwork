using FirebaseAdmin.Auth;
using Firebase.Auth;
using Google.Apis.Auth.OAuth2;
using Firebase.Auth.Providers;
using FirebaseAdmin;
using SocialNetwork.Service.Implement;
using SocialNetwork.ExceptionModel;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Google.Cloud.Storage.V1;
using System.Net.Http;
using System.Net.Http;
using MimeKit.Cryptography;
using System.Security.AccessControl;
using SocialNetwork.Helpers;
using DocumentFormat.OpenXml.Wordprocessing;


namespace SocialNetwork.FirebaseAD
{
    public class FirebaseAuthManager
    {

        public async Task CreateUserAsync(string displayName, string email, string password, string photoPath)
        {
            try
            {

                FirebaseInitializer.InitializeFirebaseApp();

                // Create user in Firebase Authentication
                var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
                {
                    DisplayName = displayName,
                    Email = email,
                    Password = password
                });
                var firestoreDb = FirestoreDb.Create("chaapprj");
                // Upload photo to Firebase Storage
                var storage = StorageClient.Create();
                var bucketName = "chaapprj.appspot.com";
                using (var httpClient = new HttpClient())
                {
                    /*// Tải hình ảnh từ URL
                    var imageBytes = await httpClient.GetByteArrayAsync(photoPath);
                    var name = SupportFunction.RemoveAccentsAndSpaces(displayName);
                    // Tải lên hình ảnh đã tải về lên Cloudinary
                    var storageObject = await storage.UploadObjectAsync(
                        bucketName,
                        $"{name}_{userRecord.Uid}", // Sử dụng tên tệp được định danh duy nhất
                        "image/jpeg", // Hoặc kiểu MIME phù hợp cho hình ảnh của bạn
                        new MemoryStream(imageBytes) // Sử dụng MemoryStream để tải lên byte của hình ảnh
                    );
                    //var photoUrl = storageObject.MediaLink;
                    
                    string objectName = $"{name}_{userRecord.Uid}";


                    // Lấy đường dẫn của hình ảnh sau khi tải lên, bao gồm cả mã token
                    string photoUrl = $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{objectName}?alt=media";
*/

                    // Update user profile
                    await FirebaseAuth.DefaultInstance.UpdateUserAsync(new UserRecordArgs
                    {
                        Uid = userRecord.Uid,
                        DisplayName = displayName,
                        PhotoUrl = photoPath
                    });

                    // Add user to Firestore
                    var userDocRef = firestoreDb.Collection("users").Document(userRecord.Uid);
                    var user = new
                    {
                        Uid = userRecord.Uid,
                        DisplayName = displayName,
                        Email = email,
                        PhotoUrl = photoPath
                    };
                    await userDocRef.SetAsync(user);

                    // Create empty user chats in Firestore
                    var userChatsDocRef = firestoreDb.Collection("userChats").Document(userRecord.Uid);
                    await userChatsDocRef.SetAsync(new { });
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating Firebase user: {ex.Message}");
                throw;
            }
        }
        public async Task UpdateUserAsync(string displayName, string email,  string photoPath)
        {
            try
            {

                FirebaseInitializer.InitializeFirebaseApp();
                var uid= await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);
               // var userfb = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(uid.Uid);
                if (uid == null)
                {
                    throw new Exception("User not found.");
                }

                var args = new UserRecordArgs
                {
                    Uid = uid.Uid,
                    PhotoUrl = photoPath,
                    DisplayName = displayName,

                };

                await FirebaseAuth.DefaultInstance.UpdateUserAsync(args);

                var firestoreDb = FirestoreDb.Create("chaapprj");
                // Upload photo to Firebase Storage


                // Add user to Firestore
                var userDocRef = firestoreDb.Collection("users").Document(uid.Uid);
                var userUpdates = new Dictionary<string, object>
                {
                    {"DisplayName", displayName},
                    {"PhotoUrl", photoPath}
                };

                // Thực hiện cập nhật tài liệu người dùng
                await userDocRef.UpdateAsync(userUpdates);

                /*// Create empty user chats in Firestore
                var userChatsDocRef = firestoreDb.Collection("userChats").Document(userfb.Uid);
                await userChatsDocRef.SetAsync(new { });*/



            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating Firebase user: {ex.Message}");
                throw;
            }
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
        public async Task ChangeNameAndPhotoUrlByEmailAsync(string email, string photoUrl, string name)
        {
            
            var user = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(email);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var args = new UserRecordArgs
            {
                Uid = user.Uid,
                PhotoUrl = photoUrl,
                DisplayName = name,

            };

            await FirebaseAuth.DefaultInstance.UpdateUserAsync(args);
        }
        public async Task ChangeNameByEmailAsync(string email, string name)
        {
            var user = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(email);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var args = new UserRecordArgs
            {
                Uid = user.Uid,

                DisplayName = name,

            };

            await FirebaseAuth.DefaultInstance.UpdateUserAsync(args);
        }
        public async Task ChangePhotoUrlByEmailAsync(string email, string photoUrl)
        {
            var user = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(email);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var args = new UserRecordArgs
            {
                Uid = user.Uid,
                PhotoUrl = photoUrl,

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
