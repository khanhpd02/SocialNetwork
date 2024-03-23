using Microsoft.AspNetCore.Mvc;
using SocialNetwork.DTO;

namespace SocialNetwork.Service
{
    public interface IRealService
    {
        Task<string> Create(Guid? audioId, IFormFile image);
    }
}
