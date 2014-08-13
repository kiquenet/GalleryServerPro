using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalleryServerPro.Business.Interfaces;

namespace GalleryServerPro.Business
{
	/// <summary>
	/// A collection of <see cref="IMediaTemplate" /> objects.
	/// </summary>
	public class MediaTemplateCollection : Collection<IMediaTemplate>, IMediaTemplateCollection
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MediaTemplateCollection"/> class.
		/// </summary>
		public MediaTemplateCollection()
			: base(new List<IMediaTemplate>())
		{
		}

		/// <summary>
		/// Adds the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <exception cref="System.ArgumentNullException">Thrown when <paramref name="item" /> is null.</exception>
		public new void Add(IMediaTemplate item)
		{
			if (item == null)
				throw new ArgumentNullException("item", "Cannot add null to an existing MediaTemplateCollection. Items.Count = " + Items.Count);

			base.Add(item);
		}

		/// <summary>
		/// Adds the media templates to the current collection.
		/// </summary>
		/// <param name="mediaTemplates">The media templates to add to the current collection.</param>
		/// <exception cref="System.ArgumentNullException">mediaTemplates</exception>
		public void AddRange(IEnumerable<IMediaTemplate> mediaTemplates)
		{
			if (mediaTemplates == null)
				throw new ArgumentNullException("mediaTemplates");

			foreach (IMediaTemplate item in mediaTemplates)
			{
				this.Add(item);
			}
		}

		/// <overloads>
		/// Finds the matching media template in the collection, or null if no match is found.
		/// </overloads>
		/// <summary>
		/// Gets the most specific <see cref="IMediaTemplate" /> item that matches one of the <paramref name="browserIds" />, or 
		/// null if no match is found. This method loops through each of the browser IDs in <paramref name="browserIds" />, 
		/// starting with the most specific item, and looks for a match in the current collection.
		/// </summary>
		/// <param name="browserIds">A <see cref="System.Array"/> of browser ids for the current browser. This is a list of strings,
		/// ordered from most general to most specific, that represent the various categories of browsers the current
		/// browser belongs to. This is typically populated by calling ToArray() on the Request.Browser.Browsers property.
		/// </param>
		/// <returns>The <see cref="IMediaTemplate" /> that most specifically matches one of the <paramref name="browserIds" />; 
		/// otherwise, a null reference.</returns>
		/// <example>During a request where the client is Firefox, the Request.Browser.Browsers property returns an ArrayList with these 
		/// five items: default, mozilla, gecko, mozillarv, and mozillafirefox. This method starts with the most specific item 
		/// (mozillafirefox) and looks in the current collection for an item with this browser ID. If a match is found, that item 
		/// is returned. If no match is found, the next item (mozillarv) is used as the search parameter.  This continues until a match 
		/// is found. If no match is found, a null is returned.
		/// </example>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="browserIds" /> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="browserIds" /> does not have any items.</exception>
		public IMediaTemplate Find(Array browserIds)
		{
			if (browserIds == null)
				throw new ArgumentNullException("browserIds");

			// If there is only a single item in our collection, there is no need to search, so just return it. This should be the most
			// common situation - the item will have a BrowserId of "default", meaning it matches all browsers.
			if (Items.Count == 1)
			{
				return Items[0];
			}

			if (Items.Count == 0)
			{
				return null;
			}

			if (browserIds.Length == 0)
				throw new ArgumentOutOfRangeException("browserIds", "The Array parameter \"browserIds\" must have at least one item, but it was passed with 0 items.");

			IMediaTemplate matchingBrowser = null;

			// We want to iterate through each browserId, starting with the most specific id and ending with the most general (id="Default"). However, we can't
			// be sure whether the first or last item has the most specific ID, so we check the first item. If it is "default", then we loop backwards;
			// otherwise we loop forwards.
			if (browserIds.GetValue(0).ToString().Equals("default", StringComparison.OrdinalIgnoreCase))
			{
				// Loop backwards. For each item, do we have an item in our collection with a matching browser ID?
				for (int index = browserIds.Length - 1; index >= 0; index--)
				{
					string browserId = browserIds.GetValue(index).ToString();

					matchingBrowser = Find(browserId);

					if (matchingBrowser != null)
					{
						break;
					}
				}
			}
			else
			{
				// Loop forwards. For each item, do we have an item in our collection with a matching browser ID?
				for (int index = 0; index < browserIds.Length; index++)
				{
					string browserId = browserIds.GetValue(index).ToString();

					matchingBrowser = Find(browserId);

					if (matchingBrowser != null)
					{
						break;
					}
				}
			}

			return matchingBrowser;
		}

		/// <summary>
		/// Gets the <see cref="IMediaTemplate" /> item that matches the <paramref name="browserId" />, or null if no match is found.
		/// </summary>
		/// <param name="browserId">The identifier of a browser as specified in the .Net Framework's browser definition file. Typically
		/// this parameter is populated from one of the entries in the Browsers property of the HttpContext.Current.Request.Browser object.</param>
		/// <returns>Returns the <see cref="IMediaTemplate" /> item that matches the <paramref name="browserId" />, or null if no match is found.</returns>
		public IMediaTemplate Find(string browserId)
		{
			return Items.FirstOrDefault(item => item.BrowserId.Equals(browserId, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Gets one or more media templates in the collection that match the <paramref name="mimeType" />. If no item is found, then
		/// the MIME type that matches the major portion is returned. For example, if the collection does not contain a specific item 
		/// for "image/jpeg", then the MIME type for "image/*" is returned. This method returns multiple items when more than one 
		/// template has been specified for browsers. That is, all returned items will have the same value for 
		/// <see cref="IMediaTemplate.MimeType" /> but the <see cref="IMediaTemplate.BrowserId" /> property will vary. At least one
		/// item in the collection will have the <see cref="IMediaTemplate.BrowserId" /> property set to "default". Guaranteed to not
		/// return null. If no items are found (which shouldn't happen), an empty collection is returned.
		/// </summary>
		/// <param name="mimeType">The MIME type for which to retrieve matching media templates.</param>
		/// <returns>Returns a <see cref="IMediaTemplateCollection" /> containing media templates that match the 
		/// <paramref name="mimeType" />. </returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="mimeType" /> is null.</exception>
		public IMediaTemplateCollection Find(IMimeType mimeType)
		{
			if (mimeType == null)
				throw new ArgumentNullException("mimeType");

			IMediaTemplateCollection copy = new MediaTemplateCollection();

			string fullType = mimeType.FullType;

			foreach (var item in (List<IMediaTemplate>)Items)
			{
				if (item.MimeType.Equals(fullType, StringComparison.OrdinalIgnoreCase))
				{
					copy.Add(item);
				}
			}
			
			if (!HasDefaultTemplate(copy))
			{
				// No specific MIME type was found (such as "video/mp4"), or one was found but it
				// didn't have a default variant. Find the generic ones (such as "video/*").
				string genericMimeType = String.Concat(mimeType.MajorType, "/*");
				foreach (var item in (List<IMediaTemplate>)Items)
				{
					if (item.MimeType.Equals(genericMimeType, StringComparison.OrdinalIgnoreCase))
					{
						copy.Add(item);
					}
				}
			}

			return copy;
		}

		private bool HasDefaultTemplate(IMediaTemplateCollection items)
		{
			return items.Any(t => t.BrowserId.Equals("default", StringComparison.Ordinal));
		}

		/// <summary>
		/// Creates a deep copy of this instance.
		/// </summary>
		/// <returns>Returns a deep copy of this instance.</returns>
		public IMediaTemplateCollection Copy()
		{
			IMediaTemplateCollection copy = new MediaTemplateCollection();

			foreach (var item in (List<IMediaTemplate>)Items)
			{
				copy.Add(item.Copy());
			}

			return copy;
		}

		/// <summary>
		/// Creates a new, empty instance of an <see cref="IMediaTemplate" /> object. This method can be used by code that only has a 
		/// reference to the interface layer and therefore cannot create a new instance of an object on its own.
		/// </summary>
		/// <returns>Returns a new, empty instance of an <see cref="IMediaTemplate" /> object.</returns>
		public IMediaTemplate CreateEmptyMediaTemplateInstance()
		{
			return new MediaTemplate();
		}
	}
}
