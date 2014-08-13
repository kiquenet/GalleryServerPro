using System;
using System.Collections.Generic;
using GalleryServerPro.Business;
using GalleryServerPro.Business.Interfaces;

namespace GalleryServerPro.Data
{
	public class MetadataRepository : Repository<GalleryDb, MetadataDto>
	{
		public MetadataRepository() { }

		public MetadataRepository(GalleryDb ctx)
		{
			Context = ctx;
		}

		private enum SaveAction
		{
			NotSpecified = 0,
			Inserted,
			Updated,
			Deleted
		}

		public void Save(IGalleryObjectMetadataItemCollection metadata)
		{
			IGalleryObjectMetadataItemCollection metadataItemsToSave = metadata.GetItemsToSave();
			if (metadataItemsToSave.Count == 0)
			{
				return; // Nothing to save
			}

			int tmpId = 0;
			var mDtos = new Dictionary<int, MetadataDto>();
			var metas = new Dictionary<int, IGalleryObjectMetadataItem>();

			// There is at least one item to persist to the data store.
			foreach (IGalleryObjectMetadataItem metaDataItem in metadataItemsToSave)
			{
				MetadataDto mDto;
				if (SaveInternal(metaDataItem, out mDto) == SaveAction.Inserted)
				{
					metas.Add(++tmpId, metaDataItem);
					mDtos.Add(tmpId, mDto);
				}
			}

			Save();

			// Loop through each metadata item again, find the matching DTO object, and update
			// the newly assigned ID.
			foreach (var kvp in metas)
			{
				MetadataDto mDto;
				if (mDtos.TryGetValue(kvp.Key, out mDto))
				{
					kvp.Value.MediaObjectMetadataId = mDto.MetadataId;
				}
			}
		}

		public void Save(IGalleryObjectMetadataItem metaDataItem)
		{
			MetadataDto mDto;
			SaveInternal(metaDataItem, out mDto);
			Save();
		}

		private SaveAction SaveInternal(IGalleryObjectMetadataItem metaDataItem, out MetadataDto mDto)
		{
			SaveAction result;

			if (metaDataItem.IsDeleted)
			{
				// The item has been marked for deletion, so let's smoke it.
				mDto = Find(metaDataItem.MediaObjectMetadataId);

				if (mDto != null)
				{
					Delete(mDto);
				}

				// Remove it from the collection.
				metaDataItem.GalleryObject.MetadataItems.Remove(metaDataItem);

				result = SaveAction.Deleted;
			}
			else if (metaDataItem.MediaObjectMetadataId == int.MinValue)
			{
				// Insert the item.
				bool isAlbum = metaDataItem.GalleryObject.GalleryObjectType == GalleryObjectType.Album;

				mDto = new MetadataDto
								 {
									 MetaName = metaDataItem.MetadataItemName,
									 FKAlbumId = isAlbum ? metaDataItem.GalleryObject.Id : (int?)null,
									 FKMediaObjectId = isAlbum ? (int?)null : metaDataItem.GalleryObject.Id,
									 RawValue = metaDataItem.RawValue,
									 Value = metaDataItem.Value
								 };

				Add(mDto);
				result = SaveAction.Inserted;
			}
			else
			{
				// Update the item.
				mDto = Find(metaDataItem.MediaObjectMetadataId);

				if (mDto != null)
				{
					mDto.MetaName = metaDataItem.MetadataItemName;
					mDto.Value = metaDataItem.Value;
					mDto.RawValue = metaDataItem.RawValue;
				}
				result = SaveAction.Updated;
			}

			SaveTags(mDto, metaDataItem.GalleryObject.GalleryId);

			return result;
		}

		/// <summary>
		/// Persists the tags, if applicable, to the data store. Applies to 
		/// <see cref="GalleryServerPro.Business.Metadata.MetadataItemName.Tags" /> and
		/// <see cref="GalleryServerPro.Business.Metadata.MetadataItemName.People" />.
		/// </summary>
		/// <param name="metadataDto">An instance of <see cref="MetadataDto"/>.</param>
		/// <param name="galleryId"></param>
		private void SaveTags(MetadataDto metadataDto, int galleryId)
		{
			if ((metadataDto.MetaName != Business.Metadata.MetadataItemName.Tags)
					&& (metadataDto.MetaName != Business.Metadata.MetadataItemName.People))
				return;

			var tags = ParseTags(metadataDto.Value);

			var tagRepo = new TagRepository(Context); // Don't put in using construct because we don't want our Context disposed
			tagRepo.Save(metadataDto.MetaName, tags);

			var metaTagRepo = new MetadataTagRepository(Context); // Don't put in using construct because we don't want our Context disposed
			metaTagRepo.Save(metadataDto, tags, galleryId);
		}

		/// <summary>
		/// Parses the comma separated tags into a collection of string values.
		/// </summary>
		/// <param name="value">The comma separated tags (e.g. "Vacation, New York, 2013").</param>
		/// <returns>Returns a list of strings.</returns>
		private List<string> ParseTags(string value)
		{
			return new List<string>(value.Trim().Split(new string[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries));
		}
	}
}