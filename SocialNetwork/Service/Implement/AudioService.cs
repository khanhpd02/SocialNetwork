using AutoMapper;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using SocialNetwork.DTO;
using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;
using SocialNetwork.Repository;
using Service.Implement.ObjectMapping;
using Audio = SocialNetwork.Entity.Audio;
using Microsoft.Extensions.Hosting;
using NAudio.Wave;

namespace SocialNetwork.Service.Implement
{
    public class AudioService : IAudioService
    {
        private readonly IAudioRepository audioRepository;
        private readonly Cloudinary _cloudinary;
        private readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }).CreateMapper();
        public AudioService(IAudioRepository audioRepository, Cloudinary _cloudinary)
        {
            this.audioRepository = audioRepository;
            this._cloudinary = _cloudinary;
        }
        public List<string> UploadFilesToCloudinary(List<IFormFile> files)
        {
            List<string> uploadedUrls = new List<string>();

            foreach (var file in files)
            {
                if (file != null && file.Length > 0)
                {
                    if (Path.GetExtension(file.FileName).Equals(".mp3", StringComparison.OrdinalIgnoreCase) ||
                        Path.GetExtension(file.FileName).Equals(".wav", StringComparison.OrdinalIgnoreCase) ||
                        Path.GetExtension(file.FileName).Equals(".m4a", StringComparison.OrdinalIgnoreCase))
                    {
                        // Tạo một đường dẫn tạm thời để lưu file
                        var tempFilePath = Path.GetTempFileName();

                        using (var stream = System.IO.File.Create(tempFilePath))
                        {
                            file.CopyTo(stream);
                        }

                        using (var reader = new AudioFileReader(tempFilePath))
                        {
                            if (reader.TotalTime.TotalSeconds <= 30)
                            {
                                var uploadParamsVideo = new VideoUploadParams
                                {
                                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                                    Folder = "SocialNetwork/Audio/",
                                };

                                var uploadResult = _cloudinary.Upload(uploadParamsVideo);
                                uploadedUrls.Add(uploadResult.SecureUrl.AbsoluteUri);
                            }
                            else
                            {
                                throw new Exception("Video chưa đúng định dạng");
                            }
                        }

                        // Xóa file tạm thời sau khi đã sử dụng
                        
                    }
                }
            }

            return uploadedUrls;
        }



        public AudioDTO Create(AudioDTO dto)
        {
            List<string> cloudinaryUrls = UploadFilesToCloudinary(dto.File);
            if (cloudinaryUrls != null)
            {
                foreach (var cloudinaryUrl in cloudinaryUrls)
                {
                    if (!string.IsNullOrEmpty(cloudinaryUrl))
                    {
                        string fileExtension = Path.GetExtension(cloudinaryUrl);
                        if (fileExtension.Equals(".mp3", StringComparison.OrdinalIgnoreCase) ||
                            fileExtension.Equals(".wav", StringComparison.OrdinalIgnoreCase) ||
                            fileExtension.Equals(".m4a", StringComparison.OrdinalIgnoreCase))
                        {
                            var audio = new Audio
                            {
                                Link = cloudinaryUrl,
                            };
                            audioRepository.Create(audio);
                            audioRepository.Save();
                        }
                    }
                }
            }
            return dto;
        }
        public List<AudioDTO> GetAllAudio()
        {
            var audio = audioRepository.FindAll();
            var listDTO = new List<AudioDTO>();
            foreach (var item in audio)
            {
                var audioDTO = mapper.Map<AudioDTO>(item);
                listDTO.Add(audioDTO);
            }
            return listDTO;
        }
    }
}
