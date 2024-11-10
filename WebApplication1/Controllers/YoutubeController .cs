using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using System.IO;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YoutubeController : ControllerBase
    {
        private readonly string apiKey = "AIzaSyAWuPDQf91s8CGiw2jvs2ejfoPuUEMVMd0"; // Your API Key
        //private readonly YouTubeService _youtubeService;



        /*public YoutubeController()
        {
            // Path to the service account key file
            var credentialFilePath = "C:/Users/Abdelrahman Elsayed_/Downloads/youtube-api-440104-26c2dce275ab.json";

            GoogleCredential credential;
            using (var stream = new FileStream(credentialFilePath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                .CreateScoped(new[]
                {
                    YouTubeService.Scope.YoutubeReadonly,
                    YouTubeService.Scope.YoutubeForceSsl
                });
            }



            // Initialize the YouTube service
            _youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = this.GetType().ToString()
            });
        }*/



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

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
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

       


        private readonly YouTubeTranscriptService _transcriptService;

        public YoutubeController(YouTubeTranscriptService transcriptService)
        {
            _transcriptService = transcriptService;
        }

        [HttpGet("GetTranscript")]
        public async Task<IActionResult> GetTranscript(string videoID)
        {
            if (string.IsNullOrWhiteSpace(videoID))
            {
                return BadRequest("A YouTube video ID must be provided.");
            }

            var transcript = await _transcriptService.GetTranscriptAsync(videoID);
            return Ok(transcript);
        }


    }
}
