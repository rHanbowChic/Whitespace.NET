using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Whitespace.Models;

namespace Whitespace.Helper
{
    public class NoteHttpHelper
    {
        HttpClient c;
        string url_with_host = String.Empty;

        public NoteHttpHelper(string url_with_host)
        {
            var handler = new HttpClientHandler()
            {
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
            };
            this.c = new HttpClient(handler);
            c.DefaultRequestVersion = new Version(2, 0);
            c.DefaultRequestHeaders.Add("User-Agent", "curl/8.9.1 whitespace/1");
            this.url_with_host = url_with_host.EndsWith("/") ? url_with_host : url_with_host + "/";
        }

        public async Task<NoteHttpPostResponse> Post(string page, string text)
        {
            Dictionary<string, string> postData = new Dictionary<string, string>();
            postData.Add("t", text);
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url_with_host + page),
                Version = new Version(2, 0),
                Headers = {
                    { "Referer", url_with_host + page },
                    { "User-Agent", "whitespace/1" }

                },
                Content = new FormUrlEncodedContent(postData)
            };
            HttpResponseMessage resp;
            try {
                resp = await c.SendAsync(httpRequestMessage);
                return new NoteHttpPostResponse {
                    HttpSuccess = true,
                    StatusCode = resp.StatusCode,
                };
            }
            catch (Exception) {
                return new NoteHttpPostResponse {
                    HttpSuccess = false,
                };
            }
        }

        public async Task<NoteHttpGetResponse> Get(string page)
        {
            try {
                var resp = await c.GetAsync(url_with_host + page);
                return new NoteHttpGetResponse {
                    HttpSuccess = true,
                    StatusCode = resp.StatusCode,
                    Text = await resp.Content.ReadAsStringAsync(),
                };
            }
            catch (Exception) {
                return new NoteHttpGetResponse {
                    HttpSuccess = false,
                };
            }
            
            
            
        }
    }
}
