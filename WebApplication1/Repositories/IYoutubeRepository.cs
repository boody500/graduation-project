namespace WebApplication1.Repositories
{
    public interface IYoutubeRepository
    {
        public string GetTranscript(string videoId);

        public string GetBestMatchedCaption(string videoID, string prompt);
    }
}
