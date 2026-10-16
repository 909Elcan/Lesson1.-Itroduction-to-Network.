using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Client....");
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {

                var ipAddress = IPAddress.Parse("192.168.100.133");
                var port = 27001;
                var ep = new IPEndPoint(ipAddress, port);

                try
                {

                    await socket.ConnectAsync(ep);
                    if (socket.Connected) 
                    {
                        Console.WriteLine("Clinent  connected to server...");
                        while (true)
                        {
                            Console.WriteLine("1- Fayl göndər (Şəkil/ Səs) ");
                            Console.WriteLine("2- Cıxış ");

                            int selection = Convert.ToInt32(Console.ReadLine());

                            if (selection ==1) {

                                Console.WriteLine("Fayıl yolunu daxil edin:");
                                var filePath = Console.ReadLine();

                                if (!File.Exists(filePath))
                                {
                                    Console.WriteLine("This File Path Was not Found !");
                                    continue;
                                }

                                var fileByts = File.ReadAllBytes(filePath);       

                                var fileNameBytes = Encoding.UTF8.GetBytes(Path.GetFileName(filePath));
                             
                                await socket.SendAsync(BitConverter.GetBytes(fileNameBytes.Length));
                                await socket.SendAsync(fileNameBytes);

                                await socket.SendAsync(BitConverter.GetBytes(fileByts.Length));

                                await socket.SendAsync(fileByts);
                                Console.WriteLine("Fayil Göndərlidi");


                            }
                            else if (selection == 2)
                            {
                                Console.WriteLine("Disconnected from server....");
                                socket.Shutdown(SocketShutdown.Both);
                                break;

                            }
                            else
                            {
                                Console.WriteLine("You enterad an invalid value ");
                               
                            }

                        }
                    }
                }
                catch (Exception)
                {
                    throw;


                }
            }
        }
    }
}
