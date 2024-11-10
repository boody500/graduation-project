using Google.Apis.YouTube.v3.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using YoutubeExplode;


public class YouTubeTranscriptService
{
    public async Task<List<object>> GetTranscriptAsync(string videoID)
    {
        var youtube = new YoutubeClient();
        

        string videoId = videoID;
        string videoUrl = $"https://www.youtube.com/watch?v={videoId}";
        if (videoId == null) {
            
        }
        var captionsList = new List<object>();

        /*var data = new Dictionary<string, string>();
        data.Add("captions", "");
        data.Add("Duration", "");
        data.Add("startTime", "");
        data.Add("endTime", "");
        data.Add("Size", "");*/
        var trackManifest = await youtube.Videos.ClosedCaptions.GetManifestAsync(videoUrl);

        try
        {
            var tracks = await youtube.Videos.ClosedCaptions.GetManifestAsync(videoId);

            var trackInfo = trackManifest.GetByLanguage("en");
            var track = await youtube.Videos.ClosedCaptions.GetAsync(trackInfo);


            

            if (track != null)
            {

                var captions = track.Captions;

                foreach (var caption in captions) {
                    if (!caption.Text.Equals("\n")) {

                        captionsList.Add(new { caption = caption.Text, Duration = caption.Duration, startTime = caption.Offset , endTime = caption.Offset + caption.Duration });
                        /*data["captions"] = caption.Text;
                        //Console.WriteLine(data["captions"]);
                        data["Duration"] = caption.Duration.ToString();
                        data["startTime"] = caption.Offset.ToString();
                        data["endTime"] = (caption.Offset + caption.Duration).ToString();
                        data["Size"]=  caption.Text.Length.ToString();
                        captionsList.Add(data.);*/
                        //data.Clear();
                    }



                }
              
                
            }
            else
            {
                captionsList.Add(new { caption = "Transcript not available for this language." } );
            }
        }
        catch (Exception ex)
        {
            captionsList.Add(new  {caption = ex.Message } );
        }

        return captionsList;
    }

    
}

