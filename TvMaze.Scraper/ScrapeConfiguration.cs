namespace TvMaze.Scraper
{
    public class ScrapeConfiguration
    {
        public string BaseUrl { get; set; }
        public int RequestTimeOut { get; set; }
        public int MaxRetriesOnErrorOrTimeOut { get; set; }
        public int WaitOnErrorOrTimeOut { get; set; } 
        public int WaitOnTooManyRequests { get; set; }
    }
}
