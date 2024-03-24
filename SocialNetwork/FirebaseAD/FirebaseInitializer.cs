using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;

namespace SocialNetwork.FirebaseAD
{
    public class FirebaseInitializer
    {
        public static FirebaseApp FirebaseAppInstance;

        public static void InitializeFirebaseApp()
        {
            if (FirebaseAppInstance == null)
            {
                var firebaseCredential = GoogleCredential.FromFile("FirebaseAD/config.json");
                FirebaseAppInstance = FirebaseApp.Create(new AppOptions
                {
                    Credential = firebaseCredential,
                    ProjectId = "chaapprj"
                });
            }
        }
        /*public static FirestoreDb GetFirestoreDb()
        {
            // Lấy tên của ứng dụng Firebase
            string appName = FirebaseAppInstance.Name;

            // Khởi tạo FirestoreDb sử dụng tên của ứng dụng Firebase
            FirestoreDb firestoreDb = FirestoreDb.Create(appName);
            return firestoreDb;
        }*/

    }
}
