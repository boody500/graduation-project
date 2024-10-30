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
                        Url = $"https://www.youtube.com/watch?v={searchResult.Id.VideoId}"
                    };

                    videos.Add(videoinfo);


                }
            }

            return Ok(new { items = videos });
        }

        /*[HttpGet("getTranscript")]
        public async Task<IActionResult> getTranscript(string vid)
            

           

            try
            {
                // Get the list of captions for the video
                List<string> s = new List<string> {"snippet","id"};
                var captionsRequest = _youtubeService.Captions.List(s ,vid);
                var captionsResponse = await captionsRequest.ExecuteAsync();

                var captionId = captionsResponse.Items.FirstOrDefault()?.Id;
                if (captionId == null)
                {
                    Console.WriteLine($"No captions available for video ID: {vid}");
                    return null; // No transcript found
                }

                // Download the captions
                var captionRequest = _youtubeService.Captions.Download(captionId);
                var transcriptResponse = await captionRequest.ExecuteAsync();

                // Parse the transcript response (modify according to actual response format)
                //var transcriptEntries = ParseTranscript(transcriptResponse);
                return Content(transcriptResponse);
            }
            catch (Google.GoogleApiException ex)
            {
                if (ex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    Console.WriteLine($"Subtitles are disabled for video ID: {vid}");
                }
                else
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
                return null;
            }
        }

        
    }*/



        private readonly YouTubeTranscriptService _transcriptService;

        public YoutubeController(YouTubeTranscriptService transcriptService)
        {
            _transcriptService = transcriptService;
        }

        [HttpGet("GetTranscript")]
        public async Task<IActionResult> GetTranscript(string videoUrl)
        {
            if (string.IsNullOrWhiteSpace(videoUrl))
            {
                return BadRequest("A YouTube video URL must be provided.");
            }

            var transcript = await _transcriptService.GetTranscriptAsync(videoUrl);
            return Ok(transcript);
        }


    }
}
