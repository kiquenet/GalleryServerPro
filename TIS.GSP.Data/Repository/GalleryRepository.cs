using GalleryServerPro.Business.Interfaces;

namespace GalleryServerPro.Data
{
  public class GalleryRepository : Repository<GalleryDb, GalleryDto>
  {
    public string ConnectionStringName
    {
      get
      {
        var fullName = Context.GetType().ToString();

        return fullName.Substring(fullName.LastIndexOf('.') + 1);
      }
    }
  }
}