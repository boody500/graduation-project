using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;


namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YoutubeController : ControllerBase
    {
        private readonly string apiKey = "AIzaSyAWuPDQf91s8CGiw2jvs2ejfoPuUEMVMd0"; // Your API Key
       



        [HttpGet("search")]
        public async Task<IActionResult> Search(string query, int maxResults)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Search query cannot be empty.");
            }

            if (maxResults <= 0)
            {
                return BadRequest("maxResults cannot be empty or negative.");
            }

            var youtubeService = new Google.Apis.YouTube.v3.YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,

                ApplicationName = this.GetType().ToString()
            });

            Console.WriteLine(this.GetType().ToString());
            var searchListRequest = youtubeService.Search.List("snippet");

            searchListRequest.Q = query; // Use the query parameter from the URL
            searchListRequest.MaxResults = maxResults;

            // Execute the search request
            var searchListResponse = await searchListRequest.ExecuteAsync();

            List<object> videos = new List<object>();

            // Process the results
            foreach (var searchResult in searchListResponse.Items)
            {
                if (searchResult.Id.Kind == "youtube#video")
                {
                    var videoinfo = new
                    {
                        Title = searchResult.Snippet.Title,
                        VID = searchResult.Id.VideoId,
                        Url = $"https://www.youtube.com/watch?v={searchResult.Id.VideoId}",
                        Thumbnail = searchResult.Snippet.Thumbnails.Default__.Url
                    };

                    videos.Add(videoinfo);


                }
            }

            return Ok(  videos );
        }

       


        
        private readonly YouTubeService _transcriptFetcher;

        public YoutubeController()
        {
            string pythonScriptPath = @"C:\Users\Abdelrahman Elsayed_\Documents\.Net\Projects\WebApplication1\transcript.py"; // Adjust the path
            _transcriptFetcher = new YouTubeService(pythonScriptPath);
        }

        [HttpGet("GetTranscript")]
        public IActionResult GetTranscript(string videoID)
        {
            string transcript = _transcriptFetcher.GetTranscript(videoID);

            if (string.IsNullOrEmpty(transcript))
            {
                return NotFound(new { error = "cannot find a transcript for the given video" });
            }
            
            return Ok(transcript);

           
        }

       

        [HttpGet("GetBestMatchedCaption")]
        public IActionResult GetBestMatchedCaption(string videoID, string prompt)
        {
            string bestMatchedCaption = _transcriptFetcher.GetBestMatchedCaption(videoID, prompt);
            if (string.IsNullOrEmpty(bestMatchedCaption))
            {
                return NotFound(new { error = "cannot find a caption for the given video" });
            }

            return Ok(bestMatchedCaption);
        }

    }
}
