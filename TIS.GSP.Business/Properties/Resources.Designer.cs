﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34011
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GalleryServerPro.Business.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("GalleryServerPro.Business.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DirectoryName must be empty for the root album. It was &quot;{0}&quot;.
        /// </summary>
        internal static string Album_FullPhysicalPath_Ex_Msg {
            get {
                return ResourceManager.GetString("Album_FullPhysicalPath_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Album.Inflate() was invoked on an existing, not inflated album (IsNew = false, IsInflated = false), which should have triggered the Factory.LoadAlbumInstance() method to set IsInflated=true and HasChanges=false. Instead, this album currently has these values: IsInflated={0}; HasChanges={1}..
        /// </summary>
        internal static string Album_Inflate_Ex_Msg {
            get {
                return ResourceManager.GetString("Album_Inflate_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified IGalleryObject is not a child of this album, and thus cannot be removed. Album ID = {0}; Gallery Object ID = {1}; Gallery Object parent album ID = {2}.
        /// </summary>
        internal static string Album_Remove_Ex_Msg {
            get {
                return ResourceManager.GetString("Album_Remove_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Factory.LoadAudioInstance(IGalleryObject) method should have set IsInflated=true and HasChanges=false. Instead it currently has these values: IsInflated={0}; HasChanges={1}..
        /// </summary>
        internal static string Audio_Inflate_Ex_Msg {
            get {
                return ResourceManager.GetString("Audio_Inflate_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The path {0} was not found. Check that the gallery is correctly configured and the path exists. Note: If you create a directory to resolve this error, you may have to restart the IIS application pool to get IIS to discover the directory..
        /// </summary>
        internal static string DirectoryNotFound_Ex_Msg {
            get {
                return ResourceManager.GetString("DirectoryNotFound_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid media object file location: The media object {0} is not located in the following directories that are valid for album {1}: {2}; {3}; {4}.
        /// </summary>
        internal static string DisplayObject_FileInfo_Ex_Msg {
            get {
                return ResourceManager.GetString("DisplayObject_FileInfo_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Info: The .NET Framework 3.0 or higher is not installed on the server. It is not required but additional functionality, such as improved image metadata extraction, become automatically enabled when it is present..
        /// </summary>
        internal static string DotNet_3_Or_Higher_Not_Found_Ex_Msg {
            get {
                return ResourceManager.GetString("DotNet_3_Or_Higher_Not_Found_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Default Gallery Object Album Invalid: The gallery &apos;{0}&apos; has an album ID specified ({1}) as the default gallery object and it does not match an existing album. Review this setting in the administration area..
        /// </summary>
        internal static string Error_Default_Gallery_Object_Album_Invalid_Ex_Msg {
            get {
                return ResourceManager.GetString("Error_Default_Gallery_Object_Album_Invalid_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Default Gallery Object Media Object Invalid: The gallery &apos;{0}&apos; has a media object ID specified ({1}) as the default gallery object and it does not match an existing media object. Review this setting in the administration area..
        /// </summary>
        internal static string Error_Default_Gallery_Object_MediaObject_Invalid_Ex_Msg {
            get {
                return ResourceManager.GetString("Error_Default_Gallery_Object_MediaObject_Invalid_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User Album Parent Invalid: The gallery &apos;{0}&apos; has an album ID specified ({1}) as the user album container and it does not match an existing album. Review this setting in the administration area..
        /// </summary>
        internal static string Error_User_Album_Parent_Invalid_Ex_Msg {
            get {
                return ResourceManager.GetString("Error_User_Album_Parent_Invalid_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot create role: A role already exists with the name you entered..
        /// </summary>
        internal static string Factory_CreateGalleryServerRoleInstance_Ex_Msg {
            get {
                return ResourceManager.GetString("Factory_CreateGalleryServerRoleInstance_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to It is not valid to call LoadInstance(IAlbum album) when the album is already inflated and inflateChildMediaObjects=false..
        /// </summary>
        internal static string Factory_LoadAlbumInstance_Ex_Msg {
            get {
                return ResourceManager.GetString("Factory_LoadAlbumInstance_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot change the GalleryObjectType property of a gallery object once it has been assigned. Current value = {0}; attempted to assign {1}..
        /// </summary>
        internal static string GalleryObject_GalleryObjectType_Ex_Msg {
            get {
                return ResourceManager.GetString("GalleryObject_GalleryObjectType_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This gallery object is already inflated. It cannot be inflated again..
        /// </summary>
        internal static string GalleryObject_IsInflated_Ex_Msg {
            get {
                return ResourceManager.GetString("GalleryObject_IsInflated_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to It is invalid to set the Parent property to null. If no valid object is available, set it to a NullGalleryObject instead..
        /// </summary>
        internal static string GalleryObject_Parent_Ex_Msg {
            get {
                return ResourceManager.GetString("GalleryObject_Parent_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot save an existing object unless it has been fully inflated from the data store..
        /// </summary>
        internal static string GalleryObject_ValidateSave_Ex_Msg {
            get {
                return ResourceManager.GetString("GalleryObject_ValidateSave_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The galleryObjectType parameter must be either GalleryObjectType.Album or GalleryObjectType.MediaObject. The value specified was {0}..
        /// </summary>
        internal static string GalleryObjectCollection_FindById_Ex_Msg {
            get {
                return ResourceManager.GetString("GalleryObjectCollection_FindById_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid state of GalleryServerRole instance: The AllAlbumIds property has a count of zero but the RootAlbumIds has a count greater than zero. The count of AllAlbumIds must be equal to or greater than the count of RootAlbumIds. This situation can happen if the RootAlbumIds property is modified and not persisted to the data store. Calling the Save() method will automatically cause the AllAlbumIds property to be reloaded from the data store..
        /// </summary>
        internal static string GalleryServerRole_AllAlbumIds_Ex_Msg {
            get {
                return ResourceManager.GetString("GalleryServerRole_AllAlbumIds_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Factory.LoadGenericMediaObjectInstance(IGalleryObject) method should have set IsInflated=true and HasChanges=false. Instead it currently has these values: IsInflated={0}; HasChanges={1}..
        /// </summary>
        internal static string GenericMediaObject_Inflate_Ex_Msg {
            get {
                return ResourceManager.GetString("GenericMediaObject_Inflate_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap GenericThumbnailImage_Audio {
            get {
                object obj = ResourceManager.GetObject("GenericThumbnailImage_Audio", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap GenericThumbnailImage_Doc {
            get {
                object obj = ResourceManager.GetObject("GenericThumbnailImage_Doc", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap GenericThumbnailImage_Excel {
            get {
                object obj = ResourceManager.GetObject("GenericThumbnailImage_Excel", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap GenericThumbnailImage_Image {
            get {
                object obj = ResourceManager.GetObject("GenericThumbnailImage_Image", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap GenericThumbnailImage_PDF {
            get {
                object obj = ResourceManager.GetObject("GenericThumbnailImage_PDF", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap GenericThumbnailImage_PowerPoint {
            get {
                object obj = ResourceManager.GetObject("GenericThumbnailImage_PowerPoint", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap GenericThumbnailImage_Unknown {
            get {
                object obj = ResourceManager.GetObject("GenericThumbnailImage_Unknown", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap GenericThumbnailImage_Video {
            get {
                object obj = ResourceManager.GetObject("GenericThumbnailImage_Video", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap GSP_Logo {
            get {
                object obj = ResourceManager.GetObject("GSP_Logo", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to One or both arguments contain an empty string. dirPath = &quot;{0}&quot;; dirName = &quot;{1}&quot;.
        /// </summary>
        internal static string HelperFunctions_ValidateDirectoryName_Ex_Msg {
            get {
                return ResourceManager.GetString("HelperFunctions_ValidateDirectoryName_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to One or both arguments contain an empty string. dirPath = &quot;{0}&quot;; fileName = &quot;{1}&quot;.
        /// </summary>
        internal static string HelperFunctions_ValidateFileName_Ex_Msg1 {
            get {
                return ResourceManager.GetString("HelperFunctions_ValidateFileName_Ex_Msg1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Gallery Server Pro does not support files without an extension. (fileName = &quot;{0}&quot;).
        /// </summary>
        internal static string HelperFunctions_ValidateFileName_Ex_Msg2 {
            get {
                return ResourceManager.GetString("HelperFunctions_ValidateFileName_Ex_Msg2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Factory.LoadImageInstance(IGalleryObject) method should have set IsInflated=true and HasChanges=false. Instead it currently has these values: IsInflated={0}; HasChanges={1}..
        /// </summary>
        internal static string Image_Inflate_Ex_Msg {
            get {
                return ResourceManager.GetString("Image_Inflate_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to sRGB.
        /// </summary>
        internal static string Metadata_ColorRepresentation_sRGB {
            get {
                return ResourceManager.GetString("Metadata_ColorRepresentation_sRGB", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Uncalibrated.
        /// </summary>
        internal static string Metadata_ColorRepresentation_Uncalibrated {
            get {
                return ResourceManager.GetString("Metadata_ColorRepresentation_Uncalibrated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to step.
        /// </summary>
        internal static string Metadata_ExposureCompensation_Suffix {
            get {
                return ResourceManager.GetString("Metadata_ExposureCompensation_Suffix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to sec..
        /// </summary>
        internal static string Metadata_ExposureTime_Units {
            get {
                return ResourceManager.GetString("Metadata_ExposureTime_Units", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to mm.
        /// </summary>
        internal static string Metadata_FocalLength_Units {
            get {
                return ResourceManager.GetString("Metadata_FocalLength_Units", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to px.
        /// </summary>
        internal static string Metadata_Height_Units {
            get {
                return ResourceManager.GetString("Metadata_Height_Units", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to KB.
        /// </summary>
        internal static string Metadata_KB {
            get {
                return ResourceManager.GetString("Metadata_KB", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to meters.
        /// </summary>
        internal static string Metadata_meters {
            get {
                return ResourceManager.GetString("Metadata_meters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to meters.
        /// </summary>
        internal static string Metadata_SubjectDistance_Units {
            get {
                return ResourceManager.GetString("Metadata_SubjectDistance_Units", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to px.
        /// </summary>
        internal static string Metadata_Width_Units {
            get {
                return ResourceManager.GetString("Metadata_Width_Units", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid MIME type string found in GalleryServerPro.Business.MimeType.ValidateMimeType(). The MIME type must be valid (e.g. &quot;image/jpg&quot;, &quot;video/quicktime&quot;). The value passed to the constructor was &quot;{0}&quot;..
        /// </summary>
        internal static string MimeType_Ctor_Ex_Msg {
            get {
                return ResourceManager.GetString("MimeType_Ctor_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No MIME type found for file extension = {0} and browserId = &quot;default&quot;. Verify the configuration file contains an entry at //galleryServerPro/galleryObject/mimeTypes/mimeType that specifies this file extension and browser ID, and make sure it appears BEFORE any mimeType entries that specify a more specific browserId for this file extension..
        /// </summary>
        internal static string MimeType_LoadInstances_Ex_Msg {
            get {
                return ResourceManager.GetString("MimeType_LoadInstances_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Reduced Functionality Mode. Enter Product Key..
        /// </summary>
        internal static string Reduced_Functionality_Mode_Watermark_Text {
            get {
                return ResourceManager.GetString("Reduced_Functionality_Mode_Watermark_Text", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Welcome to Gallery Server Pro!.
        /// </summary>
        internal static string Root_Album_Default_Summary {
            get {
                return ResourceManager.GetString("Root_Album_Default_Summary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ALL ALBUMS.
        /// </summary>
        internal static string Root_Album_Default_Title {
            get {
                return ResourceManager.GetString("Root_Album_Default_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CurrentFileIndex must be an integer between 0 and TotalFileCount - 1. Attempted to assign CurrentFileIndex = {0}; TotalFileCount = {1}..
        /// </summary>
        internal static string SynchronizationStatus_CurrentFileIndex_Ex_Msg {
            get {
                return ResourceManager.GetString("SynchronizationStatus_CurrentFileIndex_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Disabled file type.
        /// </summary>
        internal static string SynchronizationStatus_Disabled_File_Type_Msg {
            get {
                return ResourceManager.GetString("SynchronizationStatus_Disabled_File_Type_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hidden directory.
        /// </summary>
        internal static string SynchronizationStatus_Hidden_Directory_Msg {
            get {
                return ResourceManager.GetString("SynchronizationStatus_Hidden_Directory_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hidden file.
        /// </summary>
        internal static string SynchronizationStatus_Hidden_File_Msg {
            get {
                return ResourceManager.GetString("SynchronizationStatus_Hidden_File_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Loading existing album &apos;{0}&apos;....
        /// </summary>
        internal static string SynchronizationStatus_Loading_Album_Msg {
            get {
                return ResourceManager.GetString("SynchronizationStatus_Loading_Album_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot access directory.
        /// </summary>
        internal static string SynchronizationStatus_Restricted_Directory_Msg {
            get {
                return ResourceManager.GetString("SynchronizationStatus_Restricted_Directory_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to TotalFileCount must be an integer greater than or equal to zero. Attempted to assign TotalFileCount = {0}..
        /// </summary>
        internal static string SynchronizationStatus_TotalFileCount_Ex_Msg {
            get {
                return ResourceManager.GetString("SynchronizationStatus_TotalFileCount_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Factory.LoadVideoInstance(IGalleryObject) method should have set IsInflated=true and HasChanges=false. Instead it currently has these values: IsInflated={0}; HasChanges={1}..
        /// </summary>
        internal static string Video_Inflate_Ex_Msg {
            get {
                return ResourceManager.GetString("Video_Inflate_Ex_Msg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ALL ALBUMS.
        /// </summary>
        internal static string Virtual_Album_Title {
            get {
                return ResourceManager.GetString("Virtual_Album_Title", resourceCulture);
            }
        }
    }
}