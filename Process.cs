using Microsoft.Extensions.Hosting;

namespace SortFIlesDown
{
    public class Process : BackgroundService
    {
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await GetFoldersAndFiles();

                await Task.Delay(TimeSpan.FromMinutes(5));
            }
        }

        private async static Task GetFoldersAndFiles()
        {
            try
            {
                var currentUser = @"C:\Users\SeuUser\";
                var fullPath = Path.Combine(currentUser, "Downloads");
                var queueFiles = Directory.EnumerateFiles(fullPath);
                var fileInfo = new FileInfo(fullPath);

                if (!queueFiles.Any())
                    return;

                foreach (var queueFile in queueFiles)
                {
                    fileInfo = new FileInfo(queueFile);

                    var folderName = Path.Combine(fullPath, $"Arquivos-{fileInfo.Extension.Trim('.')}");

                    if (!Directory.Exists(folderName))
                        Directory.CreateDirectory(folderName).Create();
                
                }
                
                var pahtFiles = new Queue<string>();

                foreach (var queueDirectories in Directory.EnumerateDirectories(fullPath))
                {
                    foreach (var queue in queueFiles)
                    {
                        fileInfo = new FileInfo(queue);
                        var folderName = Path.Combine(queueDirectories, $"{fileInfo.LastWriteTimeUtc.ToShortDateString().Replace("/", "-")}");

                        if (!Directory.Exists(folderName))
                            Directory.CreateDirectory(folderName).Create();

                        if (!pahtFiles.Contains(folderName))
                            pahtFiles.Enqueue(folderName);
                    }
                }

                MoveFiles(ref pahtFiles, ref queueFiles, ref fileInfo);
            }
            catch(Exception)
            {
                
            }
           await Task.FromResult(0);
        }
        private static void MoveFiles(ref Queue<string> pathFiles, ref IEnumerable<string> fileList, ref FileInfo fileInfo)
        {
            try
            {
                foreach (var file in fileList)
                {
                    fileInfo = new FileInfo(file);

                    foreach (var folder in pathFiles)
                    {
                        var fileInfoDate = fileInfo.LastWriteTimeUtc.ToShortDateString().Replace("/", "-");


                        if (folder.EndsWith(fileInfoDate) && folder.ToLower().Contains(fileInfo.Extension.Replace('.', '-').ToLower()))
                        {

                             var result = fileExistsInPath(folder, fileInfo.Name, fileInfo.Extension);
                            
                             File.Move(file, result);
                            Console.WriteLine("Passei aqui e movi");

                            break;
                        }
                         
                    }

                 
                }
            }catch(Exception) { }
        }
        private static string  fileExistsInPath(string directory, string file, string extension)
        {

            var fullPath = Path.Combine(directory, file);

            if (!File.Exists(fullPath))
            {
               return  fullPath;
            }

            file = file.Replace(extension, "");

            var newFileName = Path.Combine(directory, $"{file}-Copy-{DateTime.Now.Nanosecond}{extension}");

            return newFileName;
        }

    }
}
