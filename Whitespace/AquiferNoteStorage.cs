using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Whitespace.Helper;
using Whitespace.Models;

namespace Whitespace {
    public class AquiferNoteStorage : BaseNoteStorage {
        string host;
        string ns;
        NoteHttpHelper nhh;
        IncubationCipher ic;

        public AquiferNoteStorage(string host, string ns) {
            this.host = host;
            this.ns = ns;
            this.ic = new IncubationCipher();
            this.nhh = new NoteHttpHelper($"https://{this.host}/");
        }

        public override string GetActualPage(string page) {
            var mapping = ic.GetMapping(this.host, this.ns, page);
            return mapping;

        }

        public override async Task<Result<string>> GetText(string page) {
            var get_resp = await nhh.Get(ic.GetMapping(this.host, this.ns, page));
            if (!get_resp.HttpSuccess) return new Result<string> {
                ErrorMessage = "NoteHttpHelper: Unknown Error"
            };
            if (get_resp.StatusCode != HttpStatusCode.OK) return new Result<string> {
                ErrorMessage = get_resp.StatusCode.ToString()
            };
            // 空串是每一个新mapping的情况，并非异常。
            if (get_resp.Text == "") return new Result<string> {
                Data = ""
            };

            byte[] encrypted_bits;
            try {
                encrypted_bits = Convert.FromBase64String(get_resp.Text);
            }
            catch (FormatException) {
                return new Result<string> {
                    ErrorMessage = "Base64 formatting error from encrypted message： " + get_resp.Text
                };
            }

            var text = ic.DecryptWithRaw(this.host, this.ns, page, encrypted_bits.ToList());
            if (text.Success) {
                return new Result<string> {
                    Data = text.Data
                };
            }
            else {
                return new Result<string> {
                    ErrorMessage = "DecryptWithRaw > " + text.ErrorMessage
                };
            }
            
        }

        public override async Task<Result<bool>> SetText(string page, string text) {
            var encrypted_bits = ic.EncryptWithRaw(this.host, this.ns, page, text);
            var encrypted_b64 = Convert.ToBase64String(encrypted_bits.ToArray());
            var post_resp = await nhh.Post(
                ic.GetMapping(this.host, this.ns, page), encrypted_b64);
            if (!post_resp.HttpSuccess) return new Result<bool> {
                ErrorMessage = "NoteHttpHelper: Unknown Error"
            };
            if (post_resp.StatusCode != HttpStatusCode.OK) {
                return new Result<bool> {
                    ErrorMessage = post_resp.StatusCode.ToString()
                };
            }
            return new Result<bool> {
                Data = true
            };
        }
    }
}
