using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Whitespace.Models
{
    public class NoteHttpGetResponse
    {
        public bool HttpSuccess { get; set; }
        public HttpStatusCode? StatusCode {  get; set; }
        public string? Text { get; set; }

    }
}
