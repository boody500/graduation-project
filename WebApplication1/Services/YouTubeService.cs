using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using YoutubeExplode.Videos;
using WebApplication1.Services;
using WebApplication1.Repositories;

public class YouTubeService : IYoutubeService
{
    private readonly IYoutubeRepository youtubeRepository;

    public YouTubeService(string pythonScriptPath)
    {
        youtubeRepository = new YoutubeRepository(pythonScriptPath);
    }

    public string GetTranscript(string videoId) { return youtubeRepository.GetTranscript(videoId); }

    public string GetBestMatchedCaption(string videoID, string prompt) { return youtubeRepository.GetBestMatchedCaption(videoID, prompt); }
}
