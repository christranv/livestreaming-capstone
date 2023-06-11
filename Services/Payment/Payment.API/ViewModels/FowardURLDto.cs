namespace Payment.API.ViewModels
{
    public class ForwardUrlDto
    {
        public string ForwardUrl { get; set; }

        public ForwardUrlDto(string forwardUrl)
        {
            ForwardUrl = forwardUrl;
        }
    }
    
}