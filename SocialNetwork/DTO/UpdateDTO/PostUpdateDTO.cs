using SocialNetwork.Entity;

namespace SocialNetwork.DTO.UpdateDTO
{
    public class PostUpdateDTO
    {
        public Guid? postId { get; set; }
        public string? Content { get; set; }
        public List<Guid?> ListImageDeleteId { get; set; }
        public List<IFormFile?> File { get; set; }
        public int? LevelView { get; set; }

    }
}
