﻿using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;
using SocialNetwork.Service;

namespace SocialNetwork.Repository
{

    public class InforRepository : RepositoryBase<Infor>, IInforRepository
    {
        public InforRepository(SocialNetworkContext context, IUserService userService) : base(context, userService)
        {
        }
    }
}
