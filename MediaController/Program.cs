using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using NetFwTypeLib;

namespace MediaController
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        static void Main(string[] args)
        {
            ShowWindow(GetConsoleWindow(), 0);
            HttpListener httpServer = new HttpListener();
            httpServer.Prefixes.Add("http://+:8964/");

            httpServer.Start();

            while (true)
            {
                HttpListenerContext context = httpServer.GetContext();
                HttpListenerRequest request = context.Request;

                if (request.HttpMethod == HttpMethod.Get.Method)
                {
                    HttpListenerResponse response = context.Response;

                    string page = "";

                    if (request.Url.LocalPath == "/")
                    {
                        page = Directory.GetCurrentDirectory() + "\\index.html";
                    }
                    else
                    {
                        page = Directory.GetCurrentDirectory() + request.Url.LocalPath;
                    }

                    TextReader tr = new StreamReader(page);
                    string msg = tr.ReadToEnd();

                    byte[] buffer = Encoding.UTF8.GetBytes(msg);

                    response.ContentLength64 = buffer.Length;
                    Stream stream = response.OutputStream;
                    stream.Write(buffer, 0, buffer.Length);

                    response.Close();
                }
                else if (request.HttpMethod == HttpMethod.Post.Method)
                {

                    HttpListenerResponse response = context.Response;

                    string data = new StreamReader(request.InputStream, Encoding.UTF8).ReadToEnd();
                    Console.WriteLine(data);
                    string type = JsonConvert.DeserializeObject<JsonType>(data).Type;


                    byte _key = 0xB0;

                    switch (type)
                    {
                        case "play":
                            _key = Key.VK_MEDIA_PLAY_PAUSE;
                            break;

                        case "next":
                            _key = Key.VK_MEDIA_NEXT_TRACK;
                            break;

                        case "prev":
                            _key = Key.VK_MEDIA_PREV_TRACK;
                            break;

                        default:
                            break;
                    }
                    keybd_event(_key, 0, 0, UIntPtr.Zero);

                    string msg = "Successful!";
                    byte[] buffer = Encoding.UTF8.GetBytes(msg);

                    response.ContentLength64 = buffer.Length;
                    Stream stream = response.OutputStream;
                    stream.Write(buffer, 0, buffer.Length);

                    response.Close();
                }

            }
        }

        static class Key
        {
            public static byte VK_MEDIA_PLAY_PAUSE = 0xB3;
            public static byte VK_MEDIA_PREV_TRACK = 0xB1;
            public static byte VK_MEDIA_NEXT_TRACK = 0xB0;

        }

        public class JsonType
        {
            [JsonProperty("Type")]
            public string Type
            {
                get;
                set;
            }
        }

    }
}