using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WebApplication1.Models
{
    public class Video
    {
        public string url { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string thumbnail { get; set; }
    }
}
