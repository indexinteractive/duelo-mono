namespace Duelo.Common.Service
{
    using Firebase.Database;
    using Ind3x.Util;

    public class FirebaseService<T> : Singleton<T> where T : class, new()
    {
        public enum DueloCollection
        {
            Match,
            User
        }

        public DatabaseReference GetRef(DueloCollection collection, string path)
        {
            string collectionName = collection.ToString().ToLower();
            return FirebaseDatabase.DefaultInstance.GetReference(collectionName).Child(path);
        }
    }
}