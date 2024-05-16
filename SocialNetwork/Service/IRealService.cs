using Microsoft.AspNetCore.Mvc;
using SocialNetwork.DTO;

namespace SocialNetwork.Service
{
    public interface IRealService
    {
        Task<RealDTO> MergeImageWithAudio(MergeImageAndAudioDTO mergeImageAndAudioDTO);
        Task<RealDTO> MergeVideoWithAudio(MergVideoAndAudioDTO mergVideoAndAudio);
        List<RealDTO> GetRealByUserId(Guid id);
        RealDTO GetById(Guid id);
        List<RealDTO> GetAllReal();
        void DeleteReels(List<Guid> reelIds);

    }
}
