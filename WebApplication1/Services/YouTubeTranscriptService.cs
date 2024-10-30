using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.ClosedCaptions;


public class YouTubeTranscriptService
{
    public async Task<List<string>> GetTranscriptAsync(string videoID)
    {
        var youtube = new YoutubeClient();
        //var videoId = YoutubeClient.ParseVideoId(videoUrl);
         //string videoId = ExtractVideoId(videoUrl);

        string videoId = videoID;
        string videoUrl = $"https://www.youtube.com/watch?v={videoId}";
        if (videoId == null) {
            
        }
        var captionsList = new List<string>();
        var trackManifest = await youtube.Videos.ClosedCaptions.GetManifestAsync(videoUrl);

        try
        {
            var tracks = await youtube.Videos.ClosedCaptions.GetManifestAsync(videoId);
            //var track = tracks.GetByLanguage("en");

            var trackInfo = trackManifest.GetByLanguage("en");
            var track = await youtube.Videos.ClosedCaptions.GetAsync(trackInfo);


            

            if (track != null)
            {

                var x = track.Captions;

                foreach (var xx in x) {
                    var temp = new CaptionEntry();

                    if(xx.Text != "" || xx.Text != "\n")
                        captionsList.Add(temp.getData(xx.Text, xx.Offset, xx.Duration, xx.Offset, xx.Offset + xx.Duration));
                    //captionsList.Add("\n");

                }
                
            }
            else
            {
                captionsList.Add("Transcript not available for this language.");
            }
        }
        catch (Exception ex)
        {
            captionsList.Add($"Error: {ex.Message}");
        }

        return captionsList;
    }

    private string ExtractVideoId(string videoUrl)
    {
        var regex = new Regex(@"(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})");
        var match = regex.Match(videoUrl);
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    public class CaptionEntry
    {
        public string? Text { get; set; }
        public double? Offset { get; set; }  // in milliseconds
        public double? Duration { get; set; } // in milliseconds

        public TimeSpan? startTime { get; set; }
        public TimeSpan? endTime { get; set; }

        public string getData(string Text, TimeSpan Offset, TimeSpan Duration, TimeSpan startTime, TimeSpan endTime) 
        {
            this.Text = Text;
            this.Offset = Offset.TotalMilliseconds;
            this.Duration = Duration.TotalMilliseconds;
            this.startTime = Offset;
            this.endTime = Offset + Duration;


            var data = $"{this.Text} [{this.Offset}ms, {this.Duration}ms, {this.startTime}-{this.endTime}ms]";
            return data;
         }
    }
}

