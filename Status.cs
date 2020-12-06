using DiscordRPC;

// Stores the JSON status sent by the Quest mod every time a request is made
// Naming conventions are different, because they need to match up with the JSON keys
public class Status {
    public string details {get; set;} // Top presense text
    public string state {get; set;} // Bottom presence text

    // The main presence image that appears when you view someone's profile
    public string largeImageKey {get; set;}
    public string largeImageText {get; set;}

    // The smaller image in the bottom right corner of the larger one
    public string smallImageKey {get; set;}
    public string smallImageText {get; set;}

    public int remaining {get; set;} // Used if elapsed is false, time remaining in seconds
    public bool elapsed {get; set;} // If this is true, time elapsed will be displayed alongside the presence instead of time remaining.

    // Converts this status into what is used by the Discord rich presence library
    public RichPresence ConvertToDiscord() {
        return new RichPresence() {
            Details = details,
            State = state,
            Assets = new Assets() {
                LargeImageKey = largeImageKey,
                LargeImageText = largeImageText,
                SmallImageKey = smallImageKey,
                SmallImageText = smallImageText
            },

            Timestamps = remaining == 0 ? null : Timestamps.FromTimeSpan(remaining)
        };
    }
}