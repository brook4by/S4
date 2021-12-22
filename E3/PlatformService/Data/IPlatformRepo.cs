using PlatformService.Models;

namespace PlatformService.Data
{
    public interface IPlatformRepo
    {
        bool SaveChanges();

        IEnumerable<Platform> GeAlltPlatforms();

        Platform GetPlatformById(int id);

        void CreatePlatform(Platform platform);
    }
}
