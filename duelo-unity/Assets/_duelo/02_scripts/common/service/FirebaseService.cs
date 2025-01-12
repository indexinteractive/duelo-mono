namespace Duelo.Common.Service
{
    using Firebase.Database;
    using Ind3x.Util;

    public enum DueloCollection
    {
        Match,
        Map,
        Player
    }

    public class FirebaseService<T> : Singleton<T> where T : class, new()
    {
        public DatabaseReference GetRef(DueloCollection collection, params string[] path)
        {
            string collectionName = collection.ToString().ToLower();
            string pathString = string.Join("/", path);
            return FirebaseInstance.Instance.Db.GetReference(collectionName).Child(pathString);
        }
    }
}