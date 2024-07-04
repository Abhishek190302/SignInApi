using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace SignInApi.Models
{
    public class KeywordActionRequest
    {
        public string Action { get; set; }
        public string Keyword { get; set; }
        public List<Keyword> Keywords { get; set; } = new List<Keyword>();
    }
}
