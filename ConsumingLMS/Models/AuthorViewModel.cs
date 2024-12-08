namespace ConsumingLMS.Models
{
    public class AuthorViewModel
    {
        public IEnumerable<Auth> Authors { get; set; }
        public Auth CurrentAuthor { get; set; }
    }
}
