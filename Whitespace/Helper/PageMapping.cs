using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whitespace.Helper {
    internal class PageMapping {
        public required string PageName { get; set; }
        public required string Salt { get; set; }
        public required byte[] Mapping {  get; set; }
    }
}
