namespace SocialNetwork.ExceptionModel
{
    public class InforNotFoundException : NotFoundException
    {
        public InforNotFoundException(Guid? id) : base($"InforId '{id}' Not Found")
        {
        }
    }
}
