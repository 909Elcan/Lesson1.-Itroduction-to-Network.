using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Server işə düşdü...");
            var ipAddress = IPAddress.Parse("192.168.100.133");
            var port = 27001;

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                var ep = new IPEndPoint(ipAddress, port);
                socket.Bind(ep);
                socket.Listen(10);

                while (true)
                {
                    var client = await socket.AcceptAsync();
                    _ = Task.Run(async () =>
                    {
                        Console.WriteLine("Client qoşuldu.");

                        try
                        {
                            while (true)
                            {
                         
                                var nameLenBuf = new byte[4];
                                int received = await client.ReceiveAsync(nameLenBuf);
                                if (received == 0) break;
                                int nameLen = BitConverter.ToInt32(nameLenBuf, 0);

                                var nameBuf = new byte[nameLen];
                                await client.ReceiveAsync(nameBuf);
                                string fileName = Encoding.UTF8.GetString(nameBuf);

                               
                                var sizeBuf = new byte[4];
                                await client.ReceiveAsync(sizeBuf);
                                int fileSize = BitConverter.ToInt32(sizeBuf, 0);

                                var fileByts= new byte[fileSize];
                                await client.ReceiveAsync(fileByts);


                                double sizeInMb = Math.Round((double)fileSize / (1024 * 1024), 2);
                                string sizeFormatted = sizeInMb < 1 ? $"{Math.Round((double)fileSize / 1024, 2)} KB" : $"{sizeInMb} MB";


                                Console.WriteLine();
                                Console.WriteLine($"[Fayl Gəldi] Saat: {DateTime.Now:HH:mm:ss}");
                                Console.WriteLine($"Adı: {fileName}");
                                Console.WriteLine($"Ölçüsü: {sizeFormatted}\n");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Xəta: {ex.Message}");
                        }
                    });
                }
            }
        }
    }
}