using System;
using System.Text.Json;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using DiscordRPC;

namespace Quest_Discord_Presence_Client
{
    class Program
    {
        static void Main(string[] args) {new Program();}

        private Config _config;
        private DiscordRpcClient _client;
        public Program() {
            _config = Config.load();

            // Connect to Discord RPC using our application ID
            Console.WriteLine("Connecting to Discord . . .");
            _client = new DiscordRpcClient("743131742759813160");
            _client.Initialize();

            bool presenceShown = false;
            Console.WriteLine("Connection made. Querying Quest . . .");
            while(true) {
                int timeBefore = DateTime.UtcNow.Millisecond;

                Status status;
                try {
                    status = GetStatus();

                    // Set the received presence
                    _client.SetPresence(status.ConvertToDiscord());
                    presenceShown = true;
                }   catch(Exception ex) {                    
                    if(presenceShown) {
                        // Only print exceptions when we disconnect for the first time
                        Console.Error.WriteLine("Exception occured while fetching the presence from the Quest (is your IP address correct and Beat Saber open?): " + ex.Message);
                        
                        // Disable the precence, since we aren't connected/Beat Saber is closed.
                        Console.WriteLine("Disabling presense due to failed connection.");
                        _client.ClearPresence();
                        presenceShown = false;
                    }
                }

                // Sleep for the remaining time
                int timeElapsed = DateTime.UtcNow.Millisecond - timeBefore;
                Thread.Sleep(Math.Max(_config.UpdateInterval - timeElapsed, 0)); // Make sure we don't sleep for a negative amount of time!
            }
        }

        private Status GetStatus() {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Connect to the address and port from the config
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(_config.QuestAddress), _config.QuestPort);
            socket.Connect(endPoint);

            // First find the JSON's length
            byte[] jsonLength = new byte[4];
            socket.Receive(jsonLength);

            int length = BitConverter.ToInt32(swapEndianness(jsonLength));

            // Then read the status as a string
            byte[] statusBytes = new byte[length];
            socket.Receive(statusBytes);
            string statusString = Encoding.UTF8.GetString(statusBytes);

            return JsonSerializer.Deserialize<Status>(statusString); // Deserialize it as a Status object
        }

        private byte[] swapEndianness(byte[] input) {
            List<byte> list = new List<byte>(input);
            list.Reverse();
            return list.ToArray();
        }
    }
}
