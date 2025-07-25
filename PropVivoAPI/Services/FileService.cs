namespace PropVivoAPI.Services
{
    public interface IFileService
    {
        Task<string> uploadFile(IFormFile file, string folderName);
    }
    public class FileService:IFileService 
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public async Task<string> uploadFile(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                return null;

            // Ensure folder exists
            var uploadFolder = Path.Combine(_env.WebRootPath, folderName);
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            // Create unique filename
            var originalName = Path.GetFileName(file.FileName);
            var fileWithoutSpaces = originalName.Replace(" ", "");
            var uniqueFileName = $"{Guid.NewGuid()}_{fileWithoutSpaces}";

            // Save file
            var filePath = Path.Combine(uploadFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return uniqueFileName;
        }

    }
}
