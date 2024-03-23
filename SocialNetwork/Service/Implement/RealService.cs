using DocumentFormat.OpenXml.Drawing.Wordprocessing;

using SocialNetwork.DTO;
using SocialNetwork.Repository;
using System.Diagnostics;

namespace SocialNetwork.Service.Implement
{
    public class RealService : IRealService
    {
        private readonly IRealRepository realRepository;
        private readonly IAudioRepository audioRepository;
        public RealService(IRealRepository realRepository, IAudioRepository audioRepository)
        {
            this.realRepository = realRepository;
            this.audioRepository = audioRepository;
        }

        public async Task<string> Create(Guid? audioId, IFormFile image)
        {
            var audioLink = audioRepository.FindById(audioId).Link;
            var imagePath = Path.GetTempFileName();
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            try
            {
                string ffmpegPath = @"D:\BackEnd\SocialNetwork\SocialNetwork\ffmpeg-6.1.1-full_build\bin\ffmpeg.exe"; // Đường dẫn đến ffmpeg, có thể cần chỉnh sửa nếu ffmpeg không nằm trong biến môi trường
                string outputVideoPath = @"D:\TestFolder\video\output.mp4"; 

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

                File.Delete(imagePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating video: {ex.Message}");
            }
            return "thanh cong";
        }
    }
}
