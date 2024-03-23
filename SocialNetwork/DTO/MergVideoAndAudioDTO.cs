namespace SocialNetwork.DTO
{
    public class MergVideoAndAudioDTO
    {
        public Guid? audioId { get; set; }
        public IFormFile file { get; set; }
        public string content { get; set; }
        public int LevelView { get; set; }
        public bool DisableVoice { get; set; }
    }
}
