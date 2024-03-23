namespace SocialNetwork.DTO
{
    public class MergeImageAndAudioDTO
    {
        public Guid? audioId { get; set; }
        public IFormFile file { get; set; }
        public string content { get; set; }
        public int LevelView { get; set; }
    }
}
