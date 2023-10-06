using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;

namespace SocialNetwork.Repository
{

    public class PinCodeRepository : RepositoryBase<PinCode>, IPinCodeRepository
    {
        public PinCodeRepository(SocialNetworkContext context) : base(context)
        {
        }
    }
}
