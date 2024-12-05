namespace Ind3x.Extensions
{
    using UnityEngine;

    public static class Vector3Extensions
    {
        public static void Deconstruct(this Vector3 s, out float x, out float y, out float z)
        {
            x = s.x;
            y = s.y;
            z = s.z;
        }
    }
}