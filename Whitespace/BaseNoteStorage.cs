using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Whitespace.Models;

namespace Whitespace
{
    abstract public class BaseNoteStorage
    {
        public BaseNoteStorage() { }

        public abstract Task<Result<bool>> SetText(string page, string text);
        
        public abstract Task<Result<string>> GetText(string page);

        public abstract String GetActualPage(string page);

    }
}
