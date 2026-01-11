using ModelContextProtocol.Server;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UtilityMcpServer
{
    [McpServerToolType]
    public static class UtilityTools
    {
        [McpServerTool(Name = "uuid")]
        [Description("Generate a new UUID (GUID).")]
        public static Task<string> UuidAsync()
        {
            return Task.FromResult(Guid.NewGuid().ToString());
        }

        [McpServerTool(Name = "randpass")]
        [Description("Generate a random password (0-9a-zA-Z).")]
        public static Task<string> RandomPasswordAsync(
            [Description("Length of the password. Default 12.")] int length = 12
        )
        {
            if (length <= 0) throw new ArgumentException("Length must be positive.", nameof(length));

            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var bytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            var sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                // map byte to index in chars
                int idx = bytes[i] % chars.Length;
                sb.Append(chars[idx]);
            }

            return Task.FromResult(sb.ToString());
        }

        [McpServerTool(Name = "hash")]
        [Description("Compute hash (md5, sha1, sha256, sha512) of a string and return hex string.")]
        public static Task<string> HashAsync(
            [Description("Input string to hash.")] string input,
            [Description("Algorithm: md5, sha1, sha256, sha512. Default sha256.")] string algorithm = "sha256"
        )
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            string alg = algorithm?.Trim().ToLowerInvariant() ?? "sha256";

            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash;

            switch (alg)
            {
                case "md5":
                    using (var md5 = MD5.Create())
                        hash = md5.ComputeHash(bytes);
                    break;
                case "sha1":
                case "sha-1":
                    using (var sha1 = SHA1.Create())
                        hash = sha1.ComputeHash(bytes);
                    break;
                case "sha256":
                case "sha-256":
                    using (var sha256 = SHA256.Create())
                        hash = sha256.ComputeHash(bytes);
                    break;
                case "sha512":
                case "sha-512":
                    using (var sha512 = SHA512.Create())
                        hash = sha512.ComputeHash(bytes);
                    break;
                default:
                    throw new ArgumentException($"Unsupported algorithm: {algorithm}", nameof(algorithm));
            }

            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash) sb.Append(b.ToString("x2"));
            return Task.FromResult(sb.ToString());
        }

        [McpServerTool(Name = "dummy")]
        [Description("Generate dummy text (repeat Lorem Ipsum).")]
        public static Task<string> DummyTextAsync(
            [Description("Desired length in characters. Default 200.")] int length = 200
        )
        {
            if (length <= 0) throw new ArgumentException("Length must be positive.", nameof(length));
            const string baseLorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. ";
            var sb = new StringBuilder(length + baseLorem.Length);

            while (sb.Length < length)
            {
                sb.Append(baseLorem);
            }

            string result = sb.ToString().Substring(0, length);
            return Task.FromResult(result);
        }

        [McpServerTool(Name = "b64enc")]
        [Description("Base64 encode a string (UTF-8).")]
        public static Task<string> Base64EncodeAsync(
            [Description("Input string to encode.")] string input
        )
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            var bytes = Encoding.UTF8.GetBytes(input);
            string b64 = Convert.ToBase64String(bytes);
            return Task.FromResult(b64);
        }

        [McpServerTool(Name = "b64dec")]
        [Description("Base64 decode to string (UTF-8).")]
        public static Task<string> Base64DecodeAsync(
            [Description("Base64 string to decode.")] string base64
        )
        {
            if (base64 == null) throw new ArgumentNullException(nameof(base64));
            try
            {
                var bytes = Convert.FromBase64String(base64);
                string decoded = Encoding.UTF8.GetString(bytes);
                return Task.FromResult(decoded);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Invalid Base64 string.", nameof(base64), ex);
            }
        }

        [McpServerTool(Name = "urlenc")]
        [Description("URL-encode a string (UTF-8, percent-encoding).")]
        public static Task<string> UrlEncodeAsync(
            [Description("Input string to URL-encode.")] string input
        )
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            // WebUtility.UrlEncode uses UTF-8 percent-encoding
            return Task.FromResult(WebUtility.UrlEncode(input));
        }

        [McpServerTool(Name = "urldec")]
        [Description("URL-decode a percent-encoded string.")]
        public static Task<string> UrlDecodeAsync(
            [Description("Percent-encoded string.")] string input
        )
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            return Task.FromResult(WebUtility.UrlDecode(input));
        }

        [McpServerTool(Name = "htmlenc")]
        [Description("HTML-encode a string (escape <, >, &, etc.).")]
        public static Task<string> HtmlEncodeAsync(
            [Description("Input string to HTML-encode.")] string input
        )
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            return Task.FromResult(WebUtility.HtmlEncode(input));
        }

        [McpServerTool(Name = "htmldec")]
        [Description("HTML-decode a string (convert &lt; &gt; etc. back).")]
        public static Task<string> HtmlDecodeAsync(
            [Description("HTML-encoded string.")] string input
        )
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            return Task.FromResult(WebUtility.HtmlDecode(input));
        }

        [McpServerTool(Name = "unixtime")]
        [Description("Get current Unix time (seconds since epoch).")]
        public static Task<string> UnixTimeAsync()
        {
            long unix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return Task.FromResult(unix.ToString(CultureInfo.InvariantCulture));
        }

        [McpServerTool(Name = "now")]
        [Description("Get current local time formatted (e.g. Mon Jan 12 2026 07:25:23 GMT+0900).")]
        public static Task<string> NowAsync()
        {
            var dt = DateTimeOffset.Now;
            // Build GMT+HHMM format (without colon)
            var offset = dt.Offset;
            char sign = offset >= TimeSpan.Zero ? '+' : '-';
            int hours = Math.Abs(offset.Hours);
            int minutes = Math.Abs(offset.Minutes);
            string offsetStr = string.Format(CultureInfo.InvariantCulture, "{0}{1:00}{2:00}", sign, hours, minutes);

            // Day/Month names in English like example -> use InvariantCulture
            string prefix = dt.ToString("ddd MMM dd yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            string result = $"{prefix} GMT{offsetStr}";
            return Task.FromResult(result);
        }

        [McpServerTool(Name = "hexdec")]
        [Description("Decode a hex string (e.g. e3818ae381afe38288e38186 => おはよう).")]
        public static Task<string> HexDecodeAsync(
            [Description("Hex string (may include whitespace).")] string hex
        )
        {
            if (hex == null) throw new ArgumentNullException(nameof(hex));

            // remove whitespace
            string cleaned = hex.Replace(" ", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

            // optionally allow 0x prefixes for bytes, remove them
            if (cleaned.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Substring(2);
            }

            if (cleaned.Length % 2 != 0)
                throw new ArgumentException("Hex string must have an even length.", nameof(hex));

            try
            {
                int len = cleaned.Length / 2;
                byte[] bytes = new byte[len];
                for (int i = 0; i < len; i++)
                {
                    string byteHex = cleaned.Substring(i * 2, 2);
                    bytes[i] = Convert.ToByte(byteHex, 16);
                }

                string decoded = Encoding.UTF8.GetString(bytes);
                return Task.FromResult(decoded);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Invalid hex string.", nameof(hex), ex);
            }
        }

        [McpServerTool(Name = "hexenc")]
        [Description("Encode a string to hex (UTF-8).")]
        public static Task<string> HexEncodeAsync(
            [Description("Input string to encode.")] string input,
            [Description("Use uppercase hex letters? Default false (lowercase).")] bool upperCase = false
        )
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            var bytes = Encoding.UTF8.GetBytes(input);
            var sb = new StringBuilder(bytes.Length * 2);
            string fmt = upperCase ? "X2" : "x2";
            foreach (var b in bytes) sb.Append(b.ToString(fmt));
            return Task.FromResult(sb.ToString());
        }
    }
}