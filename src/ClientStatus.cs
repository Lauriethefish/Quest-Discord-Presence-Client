namespace Quest_Discord_Presence_Client
{

    // Used to pass info from the presence manager to the UI
    public class ClientStatus
    {
        public string Info { get; }

        private ClientStatus(string info)
        {
            this.Info = info;
        }

        public static ClientStatus RequestFailed
        {
            get
            {
                return new ClientStatus("The client could not reach your quest.\nMake sure that beat saber is open, your headset on\nand your IP address correct");
            }
        }

        public static ClientStatus RequestSucceeded
        {
            get
            {
                return new ClientStatus("Successfully received presence from Quest!");
            }
        }

        public static ClientStatus NoRequestMade
        {
            get
            {
                return new ClientStatus("No request to your quest has been made yet");
            }
        }
    }
}
