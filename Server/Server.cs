using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class Server
    {
        private TcpListener listener;

        public void Start()
        {
            // Server sluša na portu 9000
            listener = new TcpListener(IPAddress.Any, 9000);
            listener.Start();
            Console.WriteLine("Server je pokrenut i čeka klijente...");

            while (true)
            {
                try
                {
                    Socket klijentSoket = listener.AcceptSocket();
                    Console.WriteLine("Klijent se povezao!");

                    // Svaki klijent dobija svog "rukovodioca" (ClientHandler) u novoj niti
                    ClientHandler handler = new ClientHandler(klijentSoket);
                    Thread nit = new Thread(handler.HandleRequests);
                    nit.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Greška kod prihvatanja klijenta: " + ex.Message);
                }
            }
        }
    }
}
