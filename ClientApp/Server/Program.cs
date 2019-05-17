using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Server
{
    class Program
    {
        private const int portNum = 4545;
        
        static private string[] tips_arr = new string[250];
        static private int tips_count = 0;
        
        private static readonly string path = @"C:\Users\Main\source\repos\ClientApp\Server\sometips.txt"; //при запуске на своём компьютере указать полный путь к sometips.txt

        static NetworkStream ns;
        
        static private void DoWork()
        {
            try
            {
                byte[] bytes = new byte[1024];
                while (true)
                {
                    int bytesRead = ns.Read(bytes, 0, bytes.Length);

                    string str = Encoding.ASCII.GetString(bytes, 0, bytesRead);

                    string result_str = "";

                    for (int i = 0; i < tips_count; i++)
                    {

                        if (tips_arr[i].StartsWith(str))
                        {
                            result_str += tips_arr[i] + "\n";
                        }
                    }
    
                    byte[] byteTips = Encoding.ASCII.GetBytes(result_str);
                    ns.Write(byteTips, 0, byteTips.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static private void ReadFromFile()
        {
            try
            {
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                {
                    int i = 0;
                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (i < tips_arr.Length)
                            tips_arr[i] = line;
                        else
                            Console.WriteLine("ERROR! File size is larger than array size!");

                        i++;
                        tips_count = i;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void Main(string[] args)
        {         
            TcpListener server = null;
            Thread t = null;
                       
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, portNum);

                server.Start();
                Console.WriteLine("Waiting for connections...");

                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("The client is connected. Running query...");

                ReadFromFile();

                ns = client.GetStream();
                t = new Thread(DoWork);
                t.Start();                           
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (server != null)
                {
                    server.Stop();
                }
            }
        }
    }
}
