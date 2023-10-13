namespace SocialNetwork.ExceptionModel
{
    public class PostNotFoundException : NotFoundException
    {
        public PostNotFoundException(Guid? id) : base($"PostId '{id}' Not Found")
        {
        }
    }
}
