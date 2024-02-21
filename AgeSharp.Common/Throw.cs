using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace AgeSharp.Common
{
    public static class Throw
    {
        [DoesNotReturn]
        public static void Always<T>(string message) where T : Exception => If<T>(true, message);
        public static void IfNull<T>([NotNull] object? obj, string message) where T : Exception => If<T>(obj is null, message);

        public static void If<T>([DoesNotReturnIf(true)] bool condition, string message) where T : Exception
        {
            if (condition)
            {
                throw CreateException<T>(message);
            }
        }

        private static T CreateException<T>(string message) where T : Exception
        {
            var args = new object[] { message };
            var exc = (T)Activator.CreateInstance(typeof(T), args)!;

            return exc;
        }
    }
}
