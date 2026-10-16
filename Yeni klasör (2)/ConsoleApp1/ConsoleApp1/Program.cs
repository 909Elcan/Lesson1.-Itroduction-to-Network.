using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ConsoleApp1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //var hotEntry =await Dns.GetHostEntryAsync("www.YouTube.com");
            // bu ayridir hotEntry.AddressList.ToList().ForEach(ip => Console.WriteLine(ip));
            //Console.WriteLine($"host:{hotEntry.HostName}" );

            //foreach (var item in hotEntry.AddressList)
            //{
            //    Console.WriteLine($"{item}");
            //}

            Console.WriteLine("Server");
            var ipAddresses = IPAddress.Parse("192.168.100.133");
            var port = 5000;
            using (var socket = new Socket(AddressFamily.InterNetwork,
                   SocketType.Stream, ProtocolType.Tcp))
            {
                var ep= new IPEndPoint(ipAddresses, port);
                socket.Bind(ep);
                socket.Listen(8);
                Console.WriteLine($"Listen : {socket.LocalEndPoint}");

                while (true)
                {
                    var client = await socket.AcceptAsync();
                    _ = Task.Run(async () =>
                    {
                        Console.WriteLine($" Clinet connected ------------------------{client.LocalEndPoint}");
                        var length = 0;
                        var bytex = new byte[1024];
                        try
                        {
                            while (true)
                            {
                                length = await client.ReceiveAsync(bytex, SocketFlags.None);
                                if (length == 0)
                                {
                                    Console.WriteLine($"client disconnectyd........... {client.LocalEndPoint}");
                                    break;
                                }
                                var message = System.Text.Encoding.UTF8.GetString(bytex, 0, length);
                                Console.WriteLine($"Client esnd message: {message}");
                                if (message == "exit")
                                {
                                    Console.WriteLine($"client disconnectyd........... {client.LocalEndPoint}");
                                    client.Shutdown(SocketShutdown.Both);
                                    break;

                                }
                            }
                        }
                        catch(SocketException ex)
                        {
                            Console.WriteLine($"Socket exception: {ex}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Exception: {ex }");
                        }
                        finally
                        {
                            client.Dispose();
                        }


                    });
                }
            }


        }
    }
}
