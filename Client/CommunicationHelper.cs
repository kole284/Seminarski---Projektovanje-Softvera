using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Text.Json;
    using Models;

    namespace Client
    {
        public class CommunicationHelper
        {
            private static CommunicationHelper _instance;
            private TcpClient client;
            private StreamReader reader;
            private StreamWriter writer;
            private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Privatni konstruktor - povezuje se na server odmah
            private CommunicationHelper()
            {
                try
                {
                    // Povezujemo se na localhost, port 9000
                    client = new TcpClient("127.0.0.1", 9000);
                    NetworkStream stream = client.GetStream();

                    // StreamReader/Writer su lakši za rad sa JSON stringovima
                    reader = new StreamReader(stream);
                    writer = new StreamWriter(stream) { AutoFlush = true };
                }
                catch (Exception ex)
                {
                    throw new Exception("Server nije dostupan: " + ex.Message);
                }
            }

            // Singleton pattern - samo jedna veza postoji
            public static CommunicationHelper Instance
            {
                get
                {
                    if (_instance == null)
                        _instance = new CommunicationHelper();
                    return _instance;
                }
            }

            public Response SendRequest(Request req)
            {
                try
                {
                    // 1. Serijalizuj Request u JSON i pošalji ga kao jednu liniju teksta
                    string jsonRequest = JsonSerializer.Serialize(req, jsonOptions);
                    writer.WriteLine(jsonRequest);

                    // 2. Čekaj odgovor od servera (jedna linija teksta)
                    string jsonResponse = reader.ReadLine();

                    if (string.IsNullOrEmpty(jsonResponse))
                    {
                        throw new Exception("Server je prekinuo vezu.");
                    }

                    // 3. Deserijalizuj nazad u Response objekat
                    return JsonSerializer.Deserialize<Response>(jsonResponse, jsonOptions);
                }
                catch (Exception ex)
                {
                    return new Response { IsSuccessful = false, Message = ex.Message };
                }
            }
        }
    }
}
