using System;
using ServerClass = Server.Server;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            PrikaziMeni();
        }

        static void PrikaziMeni()
        {
            bool izlaz = false;
            ServerClass? server = null;

            while (!izlaz)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════╗");
                Console.WriteLine("║               SERVER MENI                ║");
                Console.WriteLine("╚══════════════════════════════════════════╝");
                Console.WriteLine();
                Console.WriteLine("  1. Pokreni server");
                Console.WriteLine("  2. Zaustavi server");
                Console.WriteLine("  3. Prikazi stanje servera");
                Console.WriteLine("  4. Izlaz");
                Console.WriteLine();
                Console.Write("Izaberi neku od opcija (1-4): ");

                string? izbor = Console.ReadLine();

                switch (izbor)
                {
                    case "1":
                        if (server != null)
                        {
                            Console.WriteLine("\nServer je već pokrenut.");
                            Console.WriteLine("Pritisni bilo koji taster...");
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.WriteLine("\nPokretanje servera...");
                            server = new ServerClass();
                            System.Threading.Thread serverThread = new System.Threading.Thread(() => server.Start());
                            serverThread.IsBackground = true;
                            serverThread.Start();
                            
                            System.Threading.Thread.Sleep(500); // Daj serveru vremena da se pokrene
                            Console.WriteLine("Server je uspešno pokrenut.");
                            Console.WriteLine("Pritisni bilo koji taster...");
                            Console.ReadKey();
                        }
                        break;

                    case "2":
                        if (server == null)
                        {
                            Console.WriteLine("\nServer nije pokrenut.");
                        }
                        else
                        {
                            Console.WriteLine("\nZaustavljanje servera...");
                            Console.WriteLine("(Napomena: Server će se zaustaviti kada zatvorite aplikaciju)");
                            server = null;
                            Console.WriteLine("Server je zaustavljen.");
                        }
                        Console.WriteLine("Pritisni bilo koji taster...");
                        Console.ReadKey();
                        break;

                    case "3":
                        Console.WriteLine("\n═══════════════ STATUS SERVERA ═══════════════");
                        if (server != null)
                        {
                            Console.WriteLine("  Status:      [AKTIVAN]");
                            Console.WriteLine("  Port:        9000");
                            Console.WriteLine("  IP Adresa:   localhost (127.0.0.1)");
                        }
                        else
                        {
                            Console.WriteLine("  Status:      [ZAUSTAVLJEN]");
                        }
                        Console.WriteLine("════════════════════════════════════════════");
                        Console.WriteLine("\nPritisni bilo koji taster...");
                        Console.ReadKey();
                        break;

                    case "4":
                        Console.WriteLine("\nIzlaz iz aplikacije...");
                        if (server != null)
                        {
                            Console.WriteLine("Server će automatski biti zaustavljen.");
                            System.Threading.Thread.Sleep(1000);
                        }
                        izlaz = true;
                        break;

                    default:
                        Console.WriteLine("\nNevalidna opcija. Izaberi neku od opcija 1-4.");
                        Console.WriteLine("Pritisni bilo koji taster...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}
