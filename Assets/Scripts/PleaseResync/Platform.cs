using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PleaseResync.Perf")]
[assembly: InternalsVisibleTo("PleaseResync.Test")]

namespace PleaseResync
{
    internal static class Platform
    {
        private readonly static Random RandomNumberGenerator = new Random();
        public enum DebugType{ Log, Warning, Error};

        public static uint GetCurrentTimeMS()
        {
            return (uint)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public static ushort GetRandomUnsignedShort()
        {
            return (ushort)RandomNumberGenerator.Next();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] CloneByteArray(byte[] array)
        {
            var newArray = new byte[array.Length];
            Array.Copy(array, 0, newArray, 0, array.Length);
            return newArray;
        }
        public static void UnityLog(string message = "", DebugType type = DebugType.Log)
        {
            switch (type)
            {
                case DebugType.Log:
                    UnityEngine.Debug.Log(message);
                    break;
                case DebugType.Warning:
                    UnityEngine.Debug.LogWarning(message);
                    break;
                case DebugType.Error:
                    UnityEngine.Debug.LogError(message);
                    break;
            }
        }
    }
}
