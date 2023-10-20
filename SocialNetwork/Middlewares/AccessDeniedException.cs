namespace SocialNetwork.Middlewares
{
    public class AccessDeniedException : ForbiddenException
    {
        public AccessDeniedException() : base("Access denied") { }
    }
}
