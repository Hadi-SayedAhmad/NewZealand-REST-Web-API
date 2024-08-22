using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class LocalImageRepository : IImageRepository
    {
        // webHost will provide  information about the environment of the project including folders and so on...
        public LocalImageRepository(IWebHostEnvironment webHost, IHttpContextAccessor httpContextAccessor, NZWalksDbContext nZWalksDbContext)
        {
            WebHost = webHost;
            HttpContextAccessor = httpContextAccessor;
            NZWalksDbContext = nZWalksDbContext;
        }

        public IWebHostEnvironment WebHost { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }
        public NZWalksDbContext NZWalksDbContext { get; }

        public async Task<Image> Upload(Image image)
        {
            // we need to create a local folder to upload images to it

            // create local path variable to the Images folder using hosting environment so inject it first
            // this will point to the root folder of the api/Images/imageName/imageExtension
            var localFilePath = Path.Combine(WebHost.ContentRootPath, "Images", $"{image.FileName}{image.FileExtension}" );

            // this will like create a placeholder file that will the image be copied to
            using var stream = new FileStream(localFilePath, FileMode.Create);

            // copy the image to the specified placeholder file we created above
            await image.File.CopyToAsync(stream);

            // we need to save the path of the saved image to the database
            // url will be: https://localhost:1234/images/image.jpg for example
            // we need to get access to this: https://localhost:1234/ so we need to inject IHttpContext which provide us with the url of the running information and we also need to inject AddHttpContextAccessor in program.cs then inject IHttpContextAccessor in the constructor here
            //the scheme is https or http
            // host is where we are hosting the api like : localhost or anywhere else
            // path base is that port like :1234 after localhost
            
            var urlFilePath = $"{HttpContextAccessor.HttpContext.Request.Scheme}://{HttpContextAccessor.HttpContext.Request.Host}{HttpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";

            image.FilePath = urlFilePath;
            // add image to images table using db context
            await NZWalksDbContext.Images.AddAsync(image);
            await NZWalksDbContext.SaveChangesAsync();

            return image;

        }
    }
}
