namespace WebApplication1.Services
{
    public interface IYoutubeService
    {
       public string GetTranscript(string videoId);
       public string GetBestMatchedCaption(string videoId, string prompt);
    }
}
