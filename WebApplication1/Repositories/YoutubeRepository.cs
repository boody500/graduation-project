using System.Diagnostics;

namespace WebApplication1.Repositories
{
    public class YoutubeRepository:IYoutubeRepository
    {
        private readonly string _pythonScriptPath;

        public YoutubeRepository(string pythonScriptPath)
        {
            _pythonScriptPath = pythonScriptPath;
        }

        public string GetTranscript(string videoId)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "python",
                        Arguments = $"\"{_pythonScriptPath}\" {videoId}",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(output))
                {
                    Console.WriteLine($"Error: {error}");
                    return null;
                }

                return output;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }

        public string GetBestMatchedCaption(string videoID, string prompt)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "python",
                        Arguments = $"\"{_pythonScriptPath}\" \"{videoID}\" \"{prompt}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(output))
                {
                    Console.WriteLine($"Error: {error}");
                    return null;
                }

                return output;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }
    }
}
