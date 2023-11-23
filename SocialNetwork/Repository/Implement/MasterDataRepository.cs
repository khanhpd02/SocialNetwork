using SocialNetwork.Entity;
using SocialNetwork.Service;

namespace SocialNetwork.Repository.Implement
{
    public class MasterDataRepository : RepositoryBase<MasterDatum>, IMasterDataRepository
    {
        public MasterDataRepository(SocialNetworkContext context, IUserService userService) : base(context, userService)
        {
        }
    }

}


