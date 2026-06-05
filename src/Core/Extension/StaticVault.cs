using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Xanadu.Skidbladnir.Core.Extension
{
    /// <summary>
    /// Static vault for common static properties and methods. It can be used to avoid creating multiple instances of the same object, such as UTF8Encoding without BOM and JsonSerializerOptions with specific settings.
    /// </summary>
    public static class StaticVault
    {
        /// <summary>
        /// UTF8Encoding without BOM. It can be used for git command line and other scenarios where UTF8 encoding is required but BOM is not desired.
        /// </summary>
        public static UTF8Encoding Utf8WithoutBomEncoding => new(false);

        /// <summary>
        /// Default JsonSerializerOptions with Web defaults, ignoring null values, and encoding all Unicode characters. It can be used for JSON serialization and deserialization with common settings.
        /// </summary>
        public static JsonSerializerOptions DefaultJsonSerializerOptions => new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = false
        };
    }
}
