using System;
using System.Collections.Generic;
using System.Threading;
using DiscordRPC;
using DiscordRPC.Message;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Quest_Discord_Presence_Client {
    public delegate void StatusChanged(object sender, StatusChangedEventArgs args);

    public class PresenceManager {
        private Client app;
        private DiscordRpcClient client;

        public ClientStatus LastRequestStatus {get; private set;} = ClientStatus.NoRequestMade;

        public event StatusChanged LastRequestStatusChanged;
        public PresenceManager(Client app) {
            
            this.app = app;

            // Connect to Discord RPC using our application ID
            ConnectPresence();
        }

        public void QueryQuest() {
            Console.WriteLine("Querying Quest for presence . . .");
            while(true) {
                int timeBefore = DateTime.UtcNow.Millisecond;

                client.Invoke(); // Dispatch events

                ClientStatus oldRequestStatus = LastRequestStatus;
                Status fetchedStatus;
                try {                    
                    fetchedStatus = GetStatus();
                    
                    // Set the received presence
                    client.SetPresence(fetchedStatus.ConvertToDiscord());
                    Console.WriteLine("Successfully fetched presence");

                    LastRequestStatus = ClientStatus.RequestSucceeded;
                }   catch(Exception ex) {                    
                    Console.Error.WriteLine("Exception occured while fetching the presence from the Quest (is your IP address correct and Beat Saber open?): " + ex.Message);
                    
                    // Disable the precence, since we aren't connected/Beat Saber is closed.
                    Console.WriteLine("Disabling presense due to failed connection.");
                    client.ClearPresence();

                    LastRequestStatus = ClientStatus.RequestFailed;
                }

                // Invoke the StatusChanged event if we couldn't get the presence last time but could this time, or vice-versa
                if(oldRequestStatus != LastRequestStatus) {
                    LastRequestStatusChanged.Invoke(this, new StatusChangedEventArgs(){
                        OldStatus = oldRequestStatus,
                        NewStatus = LastRequestStatus
                    });
                }

                // Sleep for the remaining time
                int timeElapsed = DateTime.UtcNow.Millisecond - timeBefore;
                Thread.Sleep(Math.Max(app.Config.UpdateInterval - timeElapsed, 0)); // Make sure we don't sleep for a negative amount of time!
            }
        }

        // Disconnects the client if connected
        private void DisconnectClient() {
            if(client != null) {
                Console.WriteLine("Disposing client (possibly due to reconnection attempt)");
                client.Dispose();
            }
        }

        private bool ConnectPresence() {
            try {
                client = new DiscordRpcClient("743131742759813160");
                client.Initialize();
                client.OnConnectionFailed += OnDiscordDisconnect;

                return true;
            }   catch(Exception ex)   {
                Console.WriteLine("Failed to connect to discord " + ex.Message);
                return false;
            }
        }
        
        // Creates a new client in order to reconnect to discord
        private void OnDiscordDisconnect(object sender, ConnectionFailedMessage args) {
            Console.WriteLine("Connecting to Discord . . .");
            DisconnectClient();
            ConnectPresence();
        }

        private Status GetStatus() {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Connect to the address and port from the config
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(app.Config.QuestAddress), app.Config.QuestPort);
            socket.Connect(endPoint);

            // First find the JSON's length
            byte[] jsonLength = new byte[4];
            socket.Receive(jsonLength);

            int length = BitConverter.ToInt32(SwapEndianness(jsonLength));

            // Then read the status as a string
            byte[] statusBytes = new byte[length];
            socket.Receive(statusBytes);
            socket.Close(); // Close the socket again.
            string statusString = Encoding.UTF8.GetString(statusBytes);

            return JsonSerializer.Deserialize<Status>(statusString); // Deserialize it as a Status object
        }

        private byte[] SwapEndianness(byte[] input) {
            List<byte> list = new List<byte>(input);
            list.Reverse();
            return list.ToArray();
        }
    }

    public class StatusChangedEventArgs {
        public ClientStatus OldStatus {get; set;}
        public ClientStatus NewStatus {get; set;}
    }
}