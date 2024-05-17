using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;

using SocialNetwork.DTO;
using SocialNetwork.Repository;
using System.Diagnostics;
using NAudio.Wave;
using SocialNetwork.Entity;
using Video = SocialNetwork.Entity.Video;
using AutoMapper;
using SocialNetwork.ExceptionModel;
using SocialNetwork.Helpers;
using SocialNetwork.Repository.Implement;
using Service.Implement.ObjectMapping;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using Comment = SocialNetwork.Entity.Comment;
using Microsoft.IdentityModel.Tokens;

namespace SocialNetwork.Service.Implement
{
    public class RealService : IRealService
    {
        private readonly Cloudinary _cloudinary;
        private IUserService _userService;
        private readonly IInforRepository inforRepository;
        private readonly IVideoRepository videoRepository;
        private readonly ILikeRepository likeRepository;
        private readonly ICommentRepository commentRepository;
        private readonly IRealRepository realRepository;
        private readonly IAudioRepository audioRepository;
        private readonly IFriendRepository friendRepository;
        private readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }).CreateMapper();

        public RealService(IRealRepository realRepository,
            IAudioRepository audioRepository, Cloudinary _cloudinary, IVideoRepository videoRepository
            , IUserService _userService, IInforRepository inforRepository, ILikeRepository likeRepository,
            ICommentRepository commentRepository, IFriendRepository friendRepository)
        {
            this.realRepository = realRepository;
            this.audioRepository = audioRepository;
            this._cloudinary = _cloudinary;
            this.videoRepository = videoRepository;
            this.likeRepository = likeRepository;
            this.commentRepository = commentRepository;
            this._userService = _userService;
            this.friendRepository = friendRepository;
            this.inforRepository = inforRepository;

        }
        public List<string> UploadFilesToCloudinary(List<IFormFile> files)
        {
            List<string> uploadedUrls = new List<string>();

            foreach (var file in files)
            {
                if (file != null)
                {
                    if (Path.GetExtension(file.FileName).Equals(".mp4", StringComparison.OrdinalIgnoreCase))
                    {
                        var uploadParamsVideo = new VideoUploadParams
                        {
                            File = new FileDescription(file.FileName, file.OpenReadStream()),
                            Folder = "SocialNetwork/Video/",
                        };
                        var uploadResult = _cloudinary.Upload(uploadParamsVideo);
                        uploadedUrls.Add(uploadResult.SecureUrl.AbsoluteUri);
                    }
                    else
                    {
                        throw new Exception("video không đúng định dạng");
                    }
                }
            }
            return uploadedUrls;
        }
        public async Task<RealDTO> MergeImageWithAudio(MergeImageAndAudioDTO mergeImageAndAudioDTO)
        {
            RealDTO dto = new RealDTO();
            var imagePath = Path.GetTempFileName();
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await mergeImageAndAudioDTO.file.CopyToAsync(stream);
            }
            try
            {
                string ffmpegPath = @"FFMPEG\bin\ffmpeg.exe";
                string outputVideoPath = Path.GetTempFileName() + ".mp4";

                string arguments;
                if (mergeImageAndAudioDTO.audioId != null)
                {
                    var audio = audioRepository.FindByCondition(x => x.Id == mergeImageAndAudioDTO.audioId && x.IsDeleted == false).FirstOrDefault();
                    if (audio != null)
                    {
                        var audioLink = audio.Link;
                        arguments = $"-loop 1 -i \"{imagePath}\" -i \"{audioLink}\" -c:v libx264 -c:a aac -strict experimental -b:a 192k -shortest \"{outputVideoPath}\"";
                    }
                    else
                    {
                        arguments = $"-loop 1 -i \"{imagePath}\" -c:v libx264 -t 30 \"{outputVideoPath}\"";
                    }
                }
                else
                {
                    arguments = $"-loop 1 -i \"{imagePath}\" -c:v libx264 -t 30 \"{outputVideoPath}\"";

                }

                await Task.Run(() =>
                {
                    using (var process = new Process())
                    {
                        process.StartInfo.FileName = ffmpegPath;
                        process.StartInfo.Arguments = arguments;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;

                        process.OutputDataReceived += (sender, e) =>
                        {
                            if (!string.IsNullOrEmpty(e.Data))
                            {
                                Console.WriteLine($"Output: {e.Data}");
                            }
                        };

                        process.ErrorDataReceived += (sender, e) =>
                        {
                            if (!string.IsNullOrEmpty(e.Data))
                            {
                                Console.WriteLine($"Error: {e.Data}");
                            }
                        };

                        process.Start();

                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        process.WaitForExit();
                    }
                });

                using (var stream = File.OpenRead(outputVideoPath))
                {
                    var formFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
                    var uploadedUrls = UploadFilesToCloudinary(new List<IFormFile> { formFile });
                    if (uploadedUrls != null)
                    {
                        var real = new Real
                        {
                            Content = mergeImageAndAudioDTO.content,
                            LevelView = mergeImageAndAudioDTO.LevelView,
                            UserId = _userService.UserId
                        };
                        realRepository.Create(real);
                        realRepository.Save();
                        foreach (var item in uploadedUrls)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                string fileExtension = Path.GetExtension(item);
                                if (fileExtension.Equals(".mp4", StringComparison.OrdinalIgnoreCase))
                                {
                                    var video = new Video
                                    {
                                        Link = item,
                                        PostId = real.Id
                                    };
                                    videoRepository.Create(video);
                                    videoRepository.Save();
                                }
                            }
                        }
                        dto = GetById(real.Id);
                    }
                }

                File.Delete(imagePath);
                File.Delete(outputVideoPath);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi rồi");
            }
            return dto;
        }
        public async Task<RealDTO> MergeVideoWithAudio(MergVideoAndAudioDTO mergVideoAndAudio)
        {
            RealDTO dto = new RealDTO();

            var videoPath = Path.GetTempFileName();
            using (var stream = new FileStream(videoPath, FileMode.Create))
            {
                await mergVideoAndAudio.file.CopyToAsync(stream);
            }
            try
            {
                string ffmpegPath = @"FFMPEG\bin\ffmpeg.exe";
                string outputVideoPath = Path.GetTempFileName() + ".mp4";

                string arguments;
                if (mergVideoAndAudio.audioId != null)
                {
                    var audio = audioRepository.FindByCondition(x => x.Id == mergVideoAndAudio.audioId && x.IsDeleted == false).FirstOrDefault();
                    if (audio != null)
                    {
                        var audioLink = audio.Link;
                        if (mergVideoAndAudio.DisableVoice == true)
                        {
                            arguments = $"-i \"{videoPath}\" -i \"{audioLink}\" -c:v copy -c:a aac -map 0:v:0 -map 1:a:0 -shortest \"{outputVideoPath}\"";
                        }
                        else
                        {
                            arguments = $"-i \"{videoPath}\" -i \"{audioLink}\" -filter_complex \"[0:a]volume=1.0[a1]; [1:a]volume=0.5[a2]; [a1][a2]amix=inputs=2:duration=first:dropout_transition=2\" -c:v copy -c:a aac -shortest \"{outputVideoPath}\"";
                        }
                    }
                    else
                    {
                        arguments = $"-i \"{videoPath}\" -c:v copy -t 30 \"{outputVideoPath}\""; // Không có audio, chỉ sao chép video
                    }
                }
                else
                {
                    arguments = $"-i \"{videoPath}\" -c:v copy -t 30 \"{outputVideoPath}\""; // Không có audio, chỉ sao chép video
                }

                await Task.Run(() =>
                {
                    using (var process = new Process())
                    {
                        process.StartInfo.FileName = ffmpegPath;
                        process.StartInfo.Arguments = arguments;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;

                        process.OutputDataReceived += (sender, e) =>
                        {
                            if (!string.IsNullOrEmpty(e.Data))
                            {
                                Console.WriteLine($"Output: {e.Data}");
                            }
                        };

                        process.ErrorDataReceived += (sender, e) =>
                        {
                            if (!string.IsNullOrEmpty(e.Data))
                            {
                                Console.WriteLine($"Error: {e.Data}");
                            }
                        };

                        process.Start();

                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        process.WaitForExit();
                    }
                });

                using (var stream = File.OpenRead(outputVideoPath))
                {
                    var formFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
                    var uploadedUrls = UploadFilesToCloudinary(new List<IFormFile> { formFile });
                    if (uploadedUrls != null)
                    {
                        var real = new Real
                        {
                            Content = mergVideoAndAudio.content,
                            LevelView = mergVideoAndAudio.LevelView,
                            UserId = _userService.UserId
                        };
                        realRepository.Create(real);
                        realRepository.Save();
                        foreach (var item in uploadedUrls)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                string fileExtension = Path.GetExtension(item);
                                if (fileExtension.Equals(".mp4", StringComparison.OrdinalIgnoreCase))
                                {
                                    var video = new Video
                                    {
                                        Link = item,
                                        PostId = real.Id
                                    };
                                    videoRepository.Create(video);
                                    videoRepository.Save();
                                }
                            }
                        }
                        dto = GetById(real.Id);

                    }
                }

                File.Delete(videoPath);
                File.Delete(outputVideoPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating video: {ex.Message}");
                throw new Exception("Lỗi rồi");
            }
            return dto;
        }

        public RealDTO GetById(Guid id)
        {
            var CountLike = 0;
            var CountComment = 0;
            Real entity = realRepository.FindById(id) ?? throw new PostNotFoundException(id);
            var infor = inforRepository.FindByCondition(x => x.UserId == entity.UserId && x.IsDeleted == false).FirstOrDefault();

            List<Video> videos = videoRepository.FindByCondition(vid => vid.PostId == id && vid.IsDeleted == false).ToList();
            List<Like> likes = likeRepository.FindByCondition(img => img.PostId == id && img.IsDeleted == false).ToList();
            List<Comment> comments = commentRepository.FindByCondition(vid => vid.PostId == id && vid.IsDeleted == false).ToList();
            CountLike = likes.Count();
            CountComment = comments.Count();
            RealDTO dto = mapper.Map<RealDTO>(entity);
            dto.FullName = infor.FullName;
            dto.AvatarUrl = infor.Image;
            dto.Videos = videos;
            dto.Likes = likes;
            dto.Comments = comments;
            dto.CountLike = CountLike;
            dto.CountComment = CountComment;

            return dto;
        }
        public List<RealDTO> GetRealByUserId(Guid id)
        {
            List<Guid> idOfFriends = friendRepository.FindByCondition(x => (x.UserTo == _userService.UserId || x.UserAccept == _userService.UserId) && x.IsDeleted == false)
                .Select(x => x.UserTo == _userService.UserId ? x.UserAccept : x.UserTo)
                .ToList();
            List<Real> postsToRemove = new List<Real>();

            List<Real> entityList = realRepository.FindByCondition(x => x.UserId == id && x.IsDeleted == false).ToList() ?? throw new PostNotFoundException(id);

            List<RealDTO> dtoList = new List<RealDTO>();
            var CountLike = 0;
            var CountComment = 0;
            foreach (Real post in entityList)
            {
                int has = 0;
                if (post.UserId == _userService.UserId && post.IsDeleted == false)
                {
                    continue;
                }
                if (idOfFriends.Count != 0)
                {
                    foreach (Guid idfriend in idOfFriends)
                    {
                        if ((idfriend == post.UserId || post.LevelView == (int)(EnumLevelView.publicview)) && post.IsDeleted == false)
                        {
                            has = 1; break;
                        }
                    }
                    if (has == 0)
                    {
                        postsToRemove.Add(post);
                    }
                }
                else if (post.LevelView == (int)(EnumLevelView.friendview))
                { postsToRemove.Add(post); }
            }

            foreach (var postToRemove in postsToRemove)
            {
                entityList.Remove(postToRemove);
            }
            foreach (Real entity in entityList)
            {
                var infor = inforRepository.FindByCondition(x => x.UserId == entity.UserId && x.IsDeleted == false).FirstOrDefault();
                var like = likeRepository.FindByCondition(x => x.UserId == _userService.UserId && x.IsDeleted == false && x.PostId == entity.Id).FirstOrDefault();

                List<Video> videos = videoRepository.FindByCondition(vid => vid.PostId == entity.Id && vid.IsDeleted == false).ToList();
                List<Like> likes = likeRepository.FindByCondition(img => img.PostId == entity.Id && img.IsDeleted == false).ToList();
                List<Comment> comments = commentRepository.FindByCondition(vid => vid.PostId == entity.Id && vid.IsDeleted == false).ToList();
                CountLike = likes.Count();
                CountComment = comments.Count();
                RealDTO dto = mapper.Map<RealDTO>(entity);
                dto.AvatarUrl = infor.Image;
                dto.FullName = infor.FullName;
                dto.Videos = videos;
                dto.Likes = likes;
                dto.Comments = comments;
                dto.CountLike = CountLike;
                dto.CountComment = CountComment;
                if (like == null)
                {
                    dto.islike = false;
                }
                else
                {
                    dto.islike = true;
                }
                dtoList.Add(dto);
            }
            return dtoList;
        }
        public List<RealDTO> GetAllReal()
        {
            List<Guid> idOfFriends = friendRepository.FindByCondition(x => (x.UserTo == _userService.UserId || x.UserAccept == _userService.UserId) && x.IsDeleted == false)
                .Select(x => x.UserTo == _userService.UserId ? x.UserAccept : x.UserTo)
                .ToList();
            List<Real> postsToRemove = new List<Real>();

            List<Real> entityList = realRepository.FindByCondition(x => x.IsDeleted == false).ToList();

            List<RealDTO> dtoList = new List<RealDTO>();
            var CountLike = 0;
            var CountComment = 0;
            foreach (Real post in entityList)
            {
                int has = 0;
                if (post.UserId == _userService.UserId && post.IsDeleted == false)
                {
                    continue;
                }
                if (idOfFriends.Count != 0)
                {
                    foreach (Guid idfriend in idOfFriends)
                    {
                        if ((idfriend == post.UserId || post.LevelView == (int)(EnumLevelView.publicview)) && post.IsDeleted == false)
                        {
                            has = 1; break;
                        }
                    }
                    if (has == 0)
                    {
                        postsToRemove.Add(post);
                    }
                }
                else if (post.LevelView == (int)(EnumLevelView.friendview))
                { postsToRemove.Add(post); }
            }

            foreach (var postToRemove in postsToRemove)
            {
                entityList.Remove(postToRemove);
            }
            foreach (Real entity in entityList)
            {
                var infor = inforRepository.FindByCondition(x => x.UserId == entity.UserId && x.IsDeleted == false).FirstOrDefault();
                var like = likeRepository.FindByCondition(x => x.UserId == _userService.UserId && x.IsDeleted == false && x.PostId == entity.Id).FirstOrDefault();

                List<Video> videos = videoRepository.FindByCondition(vid => vid.PostId == entity.Id && vid.IsDeleted == false).ToList();
                List<Like> likes = likeRepository.FindByCondition(img => img.PostId == entity.Id && img.IsDeleted == false).ToList();
                List<Comment> comments = commentRepository.FindByCondition(vid => vid.PostId == entity.Id && vid.IsDeleted == false).ToList();
                CountLike = likes.Count();
                CountComment = comments.Count();
                RealDTO dto = mapper.Map<RealDTO>(entity);
                dto.AvatarUrl = infor.Image;
                dto.FullName = infor.FullName;
                dto.Videos = videos;
                dto.Likes = likes;
                dto.Comments = comments;
                dto.CountLike = CountLike;
                dto.CountComment = CountComment;
                if (like == null)
                {
                    dto.islike = false;
                }
                else
                {
                    dto.islike = true;
                }
                dtoList.Add(dto);
            }
            return dtoList;
        }
        public void DeleteReels(Guid reelIds)
        {

            var reel = realRepository.FindByCondition(x => x.Id == reelIds && x.IsDeleted == false).FirstOrDefault();
            if (reel != null)
            {
                realRepository.Delete(reel);
            }
            realRepository.Save();
        }

    }
}
