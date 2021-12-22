using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepo : IPlatformRepo
    {
        private readonly AppDbContext _dbContext;

        public PlatformRepo(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        void IPlatformRepo.CreatePlatform(Platform platform)
        {
            if (platform == null)
            {
                throw new ArgumentNullException(nameof(platform));
            }

            _dbContext.Platforms.Add(platform);
        }

        Platform IPlatformRepo.GetPlatformById(int id)
        {
            return _dbContext.Platforms.FirstOrDefault(p => p.Id == id);
        }

        IEnumerable<Platform> IPlatformRepo.GeAlltPlatforms()
        {
            return _dbContext.Platforms.ToList();
        }

        bool IPlatformRepo.SaveChanges()
        {
            return (_dbContext.SaveChanges() >=0);
        }
    }
}
