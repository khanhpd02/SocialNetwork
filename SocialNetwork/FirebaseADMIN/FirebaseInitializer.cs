using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace SocialNetwork.Firebase
{
    public class FirebaseInitializer
    {
        public static void InitializeFirebaseApp()
        {
            var firebaseCredential = GoogleCredential.FromFile("Firebase/config.json");
            var firebaseApp = FirebaseApp.Create(new AppOptions
            {
                Credential = firebaseCredential
            });
        }
    }
}
