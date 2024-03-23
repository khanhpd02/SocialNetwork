using SocialNetwork.DTO;

namespace SocialNetwork.Service
{
    public interface IAudioService
    {
        AudioDTO Create(AudioDTO dto);
        List<AudioDTO> GetAllAudio();
    }
}
