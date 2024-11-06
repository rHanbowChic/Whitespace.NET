using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Whitespace.Models;

namespace Whitespace.Helper {
    public class IncubationCipher : BaseCipher {
        Queue<PageMapping> mapping_cache;
        Queue<PageMapping> enc_cache;

        public IncubationCipher() {
            mapping_cache = new Queue<PageMapping>();
            enc_cache = new Queue<PageMapping>();
        }

        public override string GetMapping(string host, string ns, string page) {
            string salt = host + ns;
            var digest = GetDigestFromCache(salt, page, mapping_cache);
            return ByteArrayToString(digest);
        }

        public override string GetEncryptionKeyHex(string host, string ns, string page) {
            return ByteArrayToString(GetEncryptionKey(host, ns, page));
        }

        public override List<byte> EncryptWithRaw(string host, string ns, string page, string text) {
            byte[] nonce = new byte[12];
            byte[] tag = new byte[16];
            byte[] text_bits = Encoding.UTF8.GetBytes(text);
            byte[] encrypted = new byte[text_bits.Length];
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(nonce);
            AesGcm aes = new AesGcm(GetEncryptionKey(host, ns, page), 16);

            aes.Encrypt(nonce, text_bits, encrypted, tag);

            var message = new List<byte>();
            message.AddRange(nonce);
            message.AddRange(encrypted);
            message.AddRange(tag);
            return message;

        }

        public override Result<string> DecryptWithRaw(string host, string ns, string page, List<byte> message) {
            byte[] nonce, encrypted_text, tag;
            try {
                (nonce, encrypted_text, tag) = SplitEncryptedMessage(message.ToArray());
            }
            catch (ArgumentOutOfRangeException) {
                return new Result<string> {
                    ErrorMessage = "Message length is smaller than 28."
                };
            }
            var decrypted = new byte[encrypted_text.Length];
            AesGcm aes = new AesGcm(GetEncryptionKey(host, ns, page), 16);

            try {
                aes.Decrypt(nonce, encrypted_text, tag, decrypted);
            }
            catch (CryptographicException) {
                return new Result<string> {
                    ErrorMessage = "Failed to decrypt message: Cryptographic Error."
                };
            }
            
            var text = Encoding.UTF8.GetString(decrypted);
            return new Result<string> {
                Data = text,
            };
        }

        private static byte[] GetDigestFromCache(string salt, string page_name, Queue<PageMapping> cache) {
            byte[] digest;
            foreach (PageMapping pm in cache) {
                if (pm.Salt == salt && pm.PageName == page_name) {
                    digest = pm.Mapping;
                    return digest;
                }
            }
            var pbkdf2 = new Rfc2898DeriveBytes(page_name, Encoding.UTF8.GetBytes(salt), 0x3fff, HashAlgorithmName.SHA256);
            digest = pbkdf2.GetBytes(32);
            cache.Enqueue(new PageMapping {
                PageName = page_name,
                Salt = salt,
                Mapping = digest,
            });
            if (cache.Count > 0xff) {
                cache.Dequeue();
            }
            return digest;

        }

        private byte[] GetEncryptionKey (string host, string ns, string page) {
            var salt = host + ns + host;
            var digest = GetDigestFromCache(salt, page, enc_cache);
            return digest;
        }

        public static string ByteArrayToString(byte[] ba) {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static (byte[] first, byte[] middle, byte[] last) SplitEncryptedMessage(byte[] arr) {
            int firstLength = 12;
            int lastLength = 16;

            if (arr.Length < 28) throw new ArgumentOutOfRangeException();

            return (
                arr.Take(firstLength).ToArray(),
                arr.Skip(firstLength).Take(arr.Length - firstLength - lastLength).ToArray(),
                arr.Skip(arr.Length - lastLength).ToArray()
            );
        }
    }
}
