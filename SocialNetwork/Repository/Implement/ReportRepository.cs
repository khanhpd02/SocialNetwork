using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;

namespace SocialNetwork.Repository
{


    public class ReportRepository : RepositoryBase<Report>, IReportRepository
    {
        public ReportRepository(SocialNetworkContext context) : base(context)
        {
        }
    }
}
