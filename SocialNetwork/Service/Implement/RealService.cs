using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;

using SocialNetwork.DTO;
using SocialNetwork.Repository;
using System.Diagnostics;
using NAudio.Wave;
using SocialNetwork.Entity;
using Video = SocialNetwork.Entity.Video;

namespace SocialNetwork.Service.Implement
{
    public class RealService : IRealService
    {
        private readonly Cloudinary _cloudinary;

        private readonly IRealRepository realRepository;
        private readonly IAudioRepository audioRepository;
        private readonly IVideoRepository videoRepository;
        public RealService(IRealRepository realRepository, 
            IAudioRepository audioRepository, Cloudinary _cloudinary, IVideoRepository videoRepository)
        {
            this.realRepository = realRepository;
            this.audioRepository = audioRepository;
            this._cloudinary = _cloudinary;
            this.videoRepository = videoRepository;
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
        public async Task<string> MergeImageWithAudio(MergeImageAndAudioDTO mergeImageAndAudioDTO)
        {
            var audio = audioRepository.FindByCondition(x=>x.Id==mergeImageAndAudioDTO.audioId && x.IsDeleted==false).FirstOrDefault();
            var audioLink = audio.Link;
            var imagePath = Path.GetTempFileName();
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await mergeImageAndAudioDTO.file.CopyToAsync(stream);
            }
            try
            {
                string ffmpegPath = @"FFMPEG\bin\ffmpeg.exe"; 

                string outputVideoPath = Path.GetTempFileName() + ".mp4";

                string arguments = $"-loop 1 -i \"{imagePath}\" -i \"{audioLink}\" -c:v libx264 -c:a aac -strict experimental -b:a 192k -shortest \"{outputVideoPath}\"";
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
                            LevelView = mergeImageAndAudioDTO.LevelView
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
                    }
                }

                File.Delete(imagePath);
                File.Delete(outputVideoPath); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating video: {ex.Message}");
                return "Lỗi";
            }
            return "thành công";
        }


    }
}
