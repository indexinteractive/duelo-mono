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

        public DatabaseReference GetRef(DueloCollection collection, params string[] path)
        {
            string collectionName = collection.ToString().ToLower();
            string pathString = string.Join("/", path);
            return FirebaseDatabase.DefaultInstance.GetReference(collectionName).Child(pathString);
        }
    }
}