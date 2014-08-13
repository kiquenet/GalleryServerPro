using System.Collections.Generic;
using System.Linq;
using GalleryServerPro.Business.Interfaces;

namespace GalleryServerPro.Data
{
  public class UiTemplateAlbumRepository : Repository<GalleryDb, UiTemplateAlbumDto>
  {
    public UiTemplateAlbumRepository() { }

    public UiTemplateAlbumRepository(GalleryDb ctx)
    {
      Context = ctx;
    }

    public void Save(int uiTemplateId, IIntegerCollection rootAlbumIds)
    {
      // Step 1: Copy the list of root album IDs to a new list. We'll be removing items from the list as we process them,
      // so we don't want to mess with the actual list attached to the object.
      var templateAlbumRelationshipsToPersist = new List<int>();
      foreach (var albumId in rootAlbumIds)
      {
        templateAlbumRelationshipsToPersist.Add(albumId);
      }

      // Step 2: Iterate through each template/album relationship in the data store. If it is in our list, then
      // remove it (see step 4 why). If not, the user must have unchecked it so add it to a list of 
      // relationships to be deleted.
      var templateAlbumRelationshipsToDelete = new List<int>();
      foreach (var albumId in Where(j => j.FKUiTemplateId == uiTemplateId).Select(j => j.FKAlbumId))
      {
        if (templateAlbumRelationshipsToPersist.Contains(albumId))
        {
          templateAlbumRelationshipsToPersist.Remove(albumId);
        }
        else
        {
          templateAlbumRelationshipsToDelete.Add(albumId);
        }
      }

      // Step 3: Delete the records we accumulated in our list.
      foreach (UiTemplateAlbumDto roleAlbumDto in Where(j => j.FKUiTemplateId == uiTemplateId && templateAlbumRelationshipsToDelete.Contains(j.FKAlbumId)))
      {
        Delete(roleAlbumDto);
      }

      // Step 4: Any items still left in the templateAlbumRelationshipsToPersist list must be new ones 
      // checked by the user. Add them.
      foreach (int albumId in templateAlbumRelationshipsToPersist)
      {
        Add(new UiTemplateAlbumDto { FKUiTemplateId = uiTemplateId, FKAlbumId = albumId });
      }

      Save();
    }
  }
}