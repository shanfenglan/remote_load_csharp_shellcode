using System;
using System.IO;
namespace ConsoleApp4
{
    class HelloWorld
    {
        /* length: 833 bytes */
        public static byte[] buf = new byte[ ] { 0xfc ,0x44
        
        };

        static void Main(string[] args)
        {
            FileStream fs = new FileStream("1.txt", FileMode.Create);
            fs.Write(buf, 0, buf.Length);
            fs.Close();
        }
    }
}

