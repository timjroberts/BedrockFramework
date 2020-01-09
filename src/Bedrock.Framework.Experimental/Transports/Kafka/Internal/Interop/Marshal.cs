using System;
using System.Text;

namespace Bedrock.Framework.Kafka.Internal.Interop
{
    public static class Marshal
    {
        public unsafe static int GetBufferLength(IntPtr ptr)
        {
            // Is there a better way of getting this length?
            byte* pTraverse = (byte*)ptr;

            while (*pTraverse != 0)
                pTraverse += 1;

            return (int)(pTraverse - (byte*)ptr);
        }

        public unsafe static string GetStringFromUTF8Buffer(IntPtr strPtr)
        {
            return strPtr != IntPtr.Zero
                ? Encoding.UTF8.GetString((byte*)strPtr.ToPointer(), GetBufferLength(strPtr))
                : null;
        }
    }
}