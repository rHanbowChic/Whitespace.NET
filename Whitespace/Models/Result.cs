using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whitespace.Models {
    public class Result<T> {
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }

        public bool Success => ErrorMessage is null;

    }
}
