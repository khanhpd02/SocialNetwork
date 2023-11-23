using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;
using SocialNetwork.Service;

namespace SocialNetwork.Repository
{

    public class PinCodeRepository : RepositoryBase<PinCode>, IPinCodeRepository
    {
        public PinCodeRepository(SocialNetworkContext context, IUserService userService) : base(context, userService)
        {
        }
    }
}
