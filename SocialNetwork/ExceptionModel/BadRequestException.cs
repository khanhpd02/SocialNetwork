namespace SocialNetwork.ExceptionModel
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message)
            : base(message)
        {
           
        }
    }
}
