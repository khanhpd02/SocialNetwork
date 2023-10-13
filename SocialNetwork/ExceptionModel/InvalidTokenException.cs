namespace SocialNetwork.ExceptionModel
{
    public class InvalidTokenException : UnauthorizedException
    {
        public InvalidTokenException() : base("Invalid token") { }
    }
}
