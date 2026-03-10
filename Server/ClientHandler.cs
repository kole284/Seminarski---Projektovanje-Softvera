using System;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using Models;

namespace Server
{
    public class ClientHandler
    {
        private Socket soket;
        private NetworkStream stream;
        private StreamReader reader;
        private StreamWriter writer;
        private Controller controller = new Controller();
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public ClientHandler(Socket soket)
        {
            this.soket = soket;
            this.stream = new NetworkStream(soket);
            // AutoFlush osigurava da podaci odmah odu kroz mrežu
            this.reader = new StreamReader(stream);
            this.writer = new StreamWriter(stream) { AutoFlush = true };
        }

        public void HandleRequests()
        {
            try
            {
                while (true)
                {
                    // Čitamo jednu liniju teksta (jedan JSON Request)
                    string jsonRequest = reader.ReadLine();
                    if (jsonRequest == null) break;

                    Console.WriteLine($"[IN] Request JSON: {jsonRequest}");
                    Request req = JsonSerializer.Deserialize<Request>(jsonRequest, jsonOptions);

                    // Obrada
                    Response res = controller.HandleRequest(req);

                    // Serijalizacija odgovora u jednu liniju i slanje
                    string jsonResponse = JsonSerializer.Serialize(res, jsonOptions);
                    try { Console.WriteLine($"[OUT] Response JSON: {jsonResponse}"); } catch { }
                    writer.WriteLine(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Klijent se diskonektovao: {ex.Message}");
            }
            finally
            {
                soket.Close();
            }
        }
    }
}