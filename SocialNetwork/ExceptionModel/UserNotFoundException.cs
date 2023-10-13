namespace SocialNetwork.ExceptionModel
{
    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException(Guid? id) : base($"UserId '{id}' Not Found")
        {
        }
    }
}
