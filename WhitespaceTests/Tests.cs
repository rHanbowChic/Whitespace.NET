using System.Security.Cryptography;
using System.Text;
using Whitespace;
using Whitespace.Helper;

namespace WhitespaceTests
{
    public class Tests
    {
        public static async Task Main(string[] args)
        {
            var aq = new AquiferNoteStorage("note.ms", "test_ns");
            //await aq.SetText("1", "Hello from C#");
            Console.WriteLine(aq.GetActualPage("1"));
            Console.WriteLine((await aq.GetText("1")).Data);
        }
    }
}
