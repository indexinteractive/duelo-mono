namespace Ind3x.Util
{
    using UnityEngine;

    static class Formatting
    {
        public static string EllipsisGenerator(int maxDots = 3)
        {
            return new string('.', (int)(Time.realtimeSinceStartup % maxDots) + 1);
        }
    }
}