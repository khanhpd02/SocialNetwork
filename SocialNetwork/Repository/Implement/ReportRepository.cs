using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;
using SocialNetwork.Service;

namespace SocialNetwork.Repository
{


    public class ReportRepository : RepositoryBase<Report>, IReportRepository
    {
        public ReportRepository(SocialNetworkContext context, IUserService userService) : base(context, userService)
        {
        }
    }
}
