using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Whitespace.Helper;
using Whitespace.Models;

namespace Whitespace {
    public class NetworkNoteStorage : BaseNoteStorage {
        string url_with_host;
        NoteHttpHelper nhh;
        public NetworkNoteStorage(string host) {
            this.url_with_host = $"https://{host}/";
            this.nhh = new NoteHttpHelper(this.url_with_host);
        }

        public override string GetActualPage(string page) {
            return page;
        }

        public override async Task<Result<string>> GetText(string page) {
            var get_resp = await nhh.Get(page);
            if (get_resp.HttpSuccess) {
                if (get_resp.StatusCode == HttpStatusCode.OK) {
                    return new Result<string> {
                        Data = get_resp.Text,
                    };
                }
                return new Result<string> {
                    ErrorMessage = get_resp.StatusCode.ToString(),
                };
                
            }
            return new Result<string> {
                ErrorMessage = "NoteHttpHelper: Unknown Error"
            };


        }

        public override async Task<Result<bool>> SetText(string page, string text) {
            var post_resp = await nhh.Post(page, text);
            if (post_resp.HttpSuccess) {
                if (post_resp.StatusCode == HttpStatusCode.OK)
                    return new Result<bool> {
                        Data = true,
                    };
                return new Result<bool> {
                    ErrorMessage = post_resp.StatusCode.ToString(),
                };
            }
            return new Result<bool> {
                ErrorMessage = "NoteHttpHelper: Unknown error"
            };
        }
    }
}
