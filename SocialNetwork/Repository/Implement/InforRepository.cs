using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;

namespace SocialNetwork.Repository
{

    public class InforRepository : RepositoryBase<Infor>, IInforRepository
    {
        public InforRepository(SocialNetworkContext context) : base(context)
        {
        }
    }
}
