using Newtonsoft.Json.Bson;
using System;
using System.IO;
using System.Security;
using Newtonsoft.Json;

namespace Skidbladnir.Interop.Extension
{
    public static class ConversionExtension
    {
        public static Stream ToBson(this object obj)
        {
            switch (obj)
            {
                case null:
                    throw new ArgumentNullException(nameof(obj));
                case string:
                    throw new InvalidCastException("Unable to cast string to bson.");
            }

            var ms = new MemoryStream();
            using var writer = new BsonDataWriter(ms)
            {
                Formatting = Formatting.None
            };
            var serializer = new JsonSerializer();
            serializer.Serialize(writer, obj);
            return ms;
        }

        public static SecureString ToSecureString(this string str)
        {
            var ss = new SecureString();
            foreach (var c in str)
            {
                ss.AppendChar(c);
            }

            return ss;
        }
    }
}