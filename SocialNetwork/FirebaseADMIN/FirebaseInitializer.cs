using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace SocialNetwork.Firebase
{
    public class FirebaseInitializer
    {
        public static FirebaseApp FirebaseAppInstance;

        public static void InitializeFirebaseApp()
        {
            if (FirebaseAppInstance == null)
            {
                var firebaseCredential = GoogleCredential.FromFile("FirebaseADMIN/config.json");
                FirebaseAppInstance = FirebaseApp.Create(new AppOptions
                {
                    Credential = firebaseCredential
                });
            }
        }

    }
}
