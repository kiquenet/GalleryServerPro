using Newtonsoft.Json;

namespace GalleryServerPro.Business.Entity
{
  /// <summary>
  /// A client-optimized object representing a tag or person.
  /// </summary>
  public class Tag
  {
    /// <summary>
    /// Gets or sets the value of the tag or person.
    /// </summary>
    /// <value>The value.</value>
    [JsonProperty(PropertyName = "value")]
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets the number of times this tag is used in the gallery.
    /// </summary>
    /// <value>The count.</value>
    [JsonProperty(PropertyName = "count")]
    public int Count { get; set; }
  }
}