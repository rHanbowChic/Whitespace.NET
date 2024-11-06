using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whitespace.Models;

namespace Whitespace.Helper {
    public abstract class BaseCipher {
        public abstract string GetMapping(string host, string ns, string page);

        public abstract string GetEncryptionKeyHex(string host, string ns, string page);

        public abstract List<byte> EncryptWithRaw(
            string host, string ns, string page, string text);

        public abstract Result<string> DecryptWithRaw(
            string host, string ns, string page, List<byte> message);

    }
}
