/* TO MODIFY: Make changes to this file and test locally under the Debug compilation configuration. When 
finished, run this text through a javascript minifier and copy the output to lib.min.js. 
There is an online minifier at http://www.refresh-sf.com/yui/. */

// Contains javascript required for read-only browsing of a gallery
//#region Gallery Server Pro javascript
; (function ($, window, document, undefined) {
	//#region Gallery-wide functions

	window.Gsp = {};
	window.Gsp.Constants = {};
	window.Gsp.Constants.ViewSize_Thumbnail = 1;
	window.Gsp.Constants.ViewSize_Optimized = 2;
	window.Gsp.Constants.ViewSize_Original = 3;
	window.Gsp.Constants.ViewSize_External = 4;
	window.Gsp.Constants.MimeType_Other = 1;
	window.Gsp.Constants.MimeType_Image = 2;
	window.Gsp.Constants.MimeType_Video = 3;
	window.Gsp.Constants.MimeType_Audio = 4;

	// These map to the enum GalleryObjectType:
	window.Gsp.Constants.ItemType_Album = 3;
	window.Gsp.Constants.ItemType_Image = 4;
	window.Gsp.Constants.ItemType_Audio = 5;
	window.Gsp.Constants.ItemType_Video = 6;
	window.Gsp.Constants.ItemType_Generic = 7;
	window.Gsp.Constants.ItemType_External = 8;

	window.Gsp.Constants.VirtualType_NotVirtual = 1; // VirtualAlbumType.NotVirtual
	window.Gsp.Constants.IntMinValue = -2147483648;
	window.Gsp.Constants.IntMaxValue = 2147483647;

	window.Gsp.AppRoot = ""; // Set to Utils.AppRoot in GalleryPage.AddGlobalStartupScript (e.g. '/dev/gallery')
	window.Gsp.GalleryResourcesRoot = ""; // Set to Utils.GalleryResourcesPath in GalleryPage.AddGlobalStartupScript (e.g. '/dev/gallery/gs')
	window.Gsp.msAjaxComponentId = ""; // Holds a reference to the ID of a Microsoft Ajax Library component that implements IDisposable. Used to hold the Silverlight component.
	window.Gsp.href = ""; // Holds location.href with hash tag removed

	// Replace apostrophes and quotation marks with their ASCII equivalents
	window.Gsp.escape = function (value) {
		return value.replace(/\'/g, '&#39;').replace(/\"/g, '&quot;');
	};

	// HTML encode a string. Note that this function will strip out extra whitespace, such as new lines and tabs.
	window.Gsp.htmlEncode = function (value) {
		return $('<div/>').text(value).html();
	};

	// HTML decode a string.
	window.Gsp.htmlDecode = function (value) {
		return $('<div/>').html(value).text();
	};

	// Define a remove method for easily removing items from an array. From http://ejohn.org/blog/javascript-array-remove/
	// Examples of usage:
	// array.remove(1); Remove the second item from the array
	// array.remove(-2); Remove the second-to-last item from the array
	// array.remove(1, 2); Remove the second and third items from the array
	// array.remove(-2, -1); Remove the last and second-to-last items from the array
	Array.prototype.gspRemove = function (from, to) {
		var rest = this.slice((to || from) + 1 || this.length);
		this.length = from < 0 ? this.length + from : from;
		return this.push.apply(this, rest);
	};

	// Define equivalent of C#'s String.Format method. From http://stackoverflow.com/questions/610406/javascript-equivalent-to-printf-string-format
	// Ex: Given "{0} is dead, but {1} is alive! {0} {2}".format("ASP", "ASP.NET"), result is
	// "ASP is dead, but ASP.NET is alive! ASP {2}"
	String.prototype.format = function () {
		var args = arguments;
		return this.replace(/{(\d+)}/g, function (match, number) {
			return typeof args[number] != 'undefined'
				? args[number]
				: match;
		});
	};

	if (!String.prototype.trim) {
		// Add trim() function for browsers that don't implement it (IE 1-8).
		String.prototype.trim = function () {
			return this.replace(/^\s+|\s+$/g, '');
		};
	}

	window.Gsp.hasFormValidation = function () {
		return (typeof document.createElement('input').checkValidity == 'function');
	};

	window.Gsp.isTouchScreen = function () {
		return !!('ontouchstart' in window) || !!navigator.msMaxTouchPoints;
	};

	window.Gsp.isWidthLessThan = function (w) {
		// Returns true for screens less than w px wide. See http://stackoverflow.com/questions/6850164
		return window.matchMedia && window.matchMedia('(max-device-width: ' + w + 'px)').matches || screen.width <= w;
	};

	window.Gsp.isNullOrEmpty = function (obj) {
		if ((!obj && obj !== false) || !(obj.length > 0)) {
			return true;
		}
		return false;
	};

	window.Gsp.deepCopy = function (o) {
		var copy = o, k;

		if (o && typeof o === 'object') {
			copy = Object.prototype.toString.call(o) === '[object Array]' ? [] : {};
			for (k in o) {
				copy[k] = window.Gsp.deepCopy(o[k]);
			}
		}

		return copy;
	};

	window.Gsp.convertAlbumToGalleryItem = function (a) {
		return { Id: a.Id, IsAlbum: true, MimeType: 0, ItemType: 3, NumAlbums: a.NumAlbums, NumMediaItems: a.NumMediaItems, Caption: a.Caption, Title: a.Title };
	};

	window.Gsp.convertMediaItemToGalleryItem = function (m) {
		return { Id: m.Id, IsAlbum: false, MimeType: m.MimeType, ItemType: m.ItemType, NumAlbums: 0, NumMediaItems: 0, Caption: '', Title: m.Title };
	};

	window.Gsp.getItemTypeDesc = function (itemType) {
		switch (itemType) {
			case window.Gsp.Constants.ItemType_Album:
				return "Album";
			case window.Gsp.Constants.ItemType_Image:
				return "Image";
			case window.Gsp.Constants.ItemType_Audio:
				return "Audio";
			case window.Gsp.Constants.ItemType_Video:
				return "Video";
			case window.Gsp.Constants.ItemType_Generic:
				return "Generic";
			case window.Gsp.Constants.ItemType_External:
				return "External";
			default:
				return "Unknown";
		}
	};

	window.Gsp.findGalleryItem = function (data, id, got) {
		if (data.Album != null && data.Album.GalleryItems != null)
			return $.grep(data.Album.GalleryItems, function (gi) { return gi.Id === id && gi.ItemType === got; })[0];
		else
			return null;
	};

	window.Gsp.findMediaItem = function (data, id, got) {
		if (data.Album != null && data.Album.MediaItems != null)
			return $.grep(data.Album.MediaItems, function (mi) { return mi.Id === id && mi.ItemType === got; })[0];
		else
			return null;
	};

	window.Gsp.findMetaItem = function (metaItems, mTypeId) {
		return $.grep(metaItems, function (mi) { return mi.MTypeId === mTypeId; })[0] || null;
	};

	window.Gsp.findAlbum = function (data, id, got) {
		if (data.Album != null) {
			if (data.Album.Id === id && got === window.Gsp.Constants.ItemType_Album)
				return data.Album;
			else if (data.Album.GalleryItems != null)
				return $.grep(data.Album.GalleryItems, function (gi) { return gi.Id === id && gi.ItemType === got; })[0];
			else
				return null;
		} else
			return null;
	};

	window.Gsp.getView = function (mediaItem, viewSize) {
		// Get the requested view for the specified media item. If the requested view is for the optimized version and
		// it does not exist, the original is returned; otherwise returns null when the requested size does not exist.
		// viewSize must be one of the constants specified at the beginning of this file 
		// (e.g. window.Gsp.Constants.ViewSize_Thumbnail, etc.)
		var orig = null;
		for (var i = 0; i < mediaItem.Views.length; i++) {
			if (mediaItem.Views[i].ViewSize == viewSize) {
				return mediaItem.Views[i];
			}
			else if (mediaItem.Views[i].ViewSize == Gsp.Constants.ViewSize_Original)
				orig = mediaItem.Views[i];
		}
		return orig;
	};

	window.Gsp.Init = function () {
		$(".gsp_ns input:submit, .gsp_ns button").button();

		// Set up jsRender converters
		$.views.converters({
			getItemTypeDesc: function (itemType) {
				return window.Gsp.getItemTypeDesc(itemType);
			},
			stripHtml: function (text) {
				return Gsp.escape(text.replace(/(<[^<>]*>)/g, ""));
			}
		});

		$.views.helpers({
			htmlEscape: function (value) {
				return window.Gsp.escape(value);
			},

			format: function (value, format, culture) {
				// Formats and parses strings, dates and numbers in over 350 cultures. See https://github.com/jquery/globalize
				return Globalize.format(value, format, culture);
			},

			parseDate: function (value, formats, culture) {
				// Parses a string representing a date into a JavaScript Date object. See https://github.com/jquery/globalize
				return Globalize.parseDate(value, formats, culture);
			},

			parseInt: function (value, radix, culture) {
				// Parses a string representing a whole number in the given radix (10 by default). See https://github.com/jquery/globalize
				return Globalize.parseInt(value, radix, culture);
			},

			parseFloat: function (value, radix, culture) {
				// Parses a string representing a floating point number in the given radix (10 by default). See https://github.com/jquery/globalize
				return Globalize.parseFloat(value, radix, culture);
			},

			findMetaItem: function (metaItems, mTypeId) {
				// Find the meta item for the specified type, returning an object set to default values if not found.
				return window.Gsp.findMetaItem(metaItems, mTypeId) || { Id: 0, GTypeId: 0, IsEditable: false, MediaId: 0, MTypeId: mTypeId, Value: '' };
			},

			getAlbumUrl: function (albumId, preserveTags) {
				// Gets URL to album. Ex: http://localhost/default.aspx?tag=desert, http://localhost/default.aspx?aid=44
				// When preserveTags=true, the qs parms tag, people, and search are included if present; otherwise they are stripped
				return Gsp.GetAlbumUrl(albumId, preserveTags);
			},

			getGalleryItemUrl: function (galleryItem, preserveTags) {
				// Generate URL to the page containing the specified gallery item. Ex: http://localhost/default.aspx?aid=44, http://localhost/default.aspx?tag=desert&moid=23
				// When preserveTags=true, the qs parms title, tag, people, search, latest & filter are included if present; otherwise they are stripped
				var qs = { aid: galleryItem.IsAlbum ? galleryItem.Id : null, moid: galleryItem.IsAlbum ? null : galleryItem.Id };

				if (!preserveTags) {
					// Generally we want to strip tags for albums and preserve them for MOs. This allows users to browse MOs
					// within the context of their tag/people/search criteria.
					qs.title = null;
					qs.tag = null;
					qs.people = null;
					qs.search = null;
					qs.latest = null;
					qs.filter = null;
					qs.rating = null;
					qs.top = null;
				}

				return Gsp.GetUrl(window.Gsp.href, qs);
			},

			getMediaUrl: function (mediaId, preserveTags) {
				// Generate URL to the page containing the specified media item. Ex: http://localhost/default.aspx?tag=desert&moid=23
				// When preserveTags=true, the qs parms tag, people, and search are included if present; otherwise they are stripped
				var qs = { aid: null, moid: mediaId };

				if (!preserveTags) {
					// Generally we want to strip tags for albums and preserve them for MOs. This allows users to browse MOs
					// within the context of their tag/people/search criteria.
					qs.title = null;
					qs.tag = null;
					qs.people = null;
					qs.search = null;
					qs.latest = null;
					qs.filter = null;
					qs.rating = null;
					qs.top = null;
				}

				return Gsp.GetUrl(document.location.href, qs);
			},

			getDownloadUrl: function (albumId) {
				// Gets URL to page where album objects can be downloaded. Ex: http://localhost/default.aspx?g=task_downloadobjects&aid=45
				return Gsp.GetUrl(window.location.href, { g: 'task_downloadobjects', moid: null, aid: albumId });
			},

			getAddUrl: function (galleryData) {
				// Gets URL to add objects page for current album. Ex: http://localhost/default.aspx?g=task_addobjects&aid=45
				return Gsp.GetUrl(window.location.href, { g: 'task_addobjects', aid: galleryData.Album.Id });
			}
		});

		// Gets reference to current URL with hash tag removed
		window.Gsp.href = window.location.href.replace(/#\d+/, '');
	};

	$(document).ready(Gsp.Init);

	window.Gsp.ReloadPage = function () {
		window.location = Gsp.RemoveQSParm(window.location.href, 'msg');
	};

	window.Gsp.GetAlbumUrl = function (albumId, preserveTags) {
		var qs = { aid: null, moid: null };

		if (!preserveTags) {
			qs.title = null;
			qs.tag = null;
			qs.people = null;
			qs.search = null;
			qs.latest = null;
			qs.filter = null;
			qs.rating = null;
			qs.top = null;
		}

		if (albumId > Gsp.Constants.IntMinValue)
			qs.aid = albumId;

		return Gsp.GetUrl(document.location.href, qs);
	};

	window.Gsp.GetUrl = function (url, parmValuePairs) {
		// We never want to include the ss and msg parms, so set them to null if the caller didn't already do it.
		if (typeof parmValuePairs.ss === 'undefined')
			parmValuePairs.ss = null; // auto-start slide show

		if (typeof parmValuePairs.msg === 'undefined')
			parmValuePairs.msg = null; // msg ID

		$.each(parmValuePairs, function (p, v) { url = Gsp.AddQSParm(Gsp.RemoveQSParm(url, p), p, v); });
		return url;
	};

	window.Gsp.IsQSParmPresent = function (param) {
		var qs = Gsp.GetQS[param];
		return ((qs != null) && (qs.length > 0));
	};

	window.Gsp.GetQS = function () {
		var result = {}, queryString = location.search.substring(1), re = /([^&=]+)=([^&]*)/g, m;

		while (m = re.exec(queryString)) {
			result[decodeURIComponent(m[1])] = decodeURIComponent(m[2]);
		}

		return result;
	};

	window.Gsp.GetQSParm = function (param) {
		return window.Gsp.GetQS()[param];
	};

	window.Gsp.AddQSParm = function (url, param, value) {
		if (!param || !value) return url;

		param = encodeURIComponent(param);
		value = encodeURIComponent(value);

		var urlparts = url.split('?');
		if (urlparts.length < 2)
			return url + '?' + param + "=" + value;

		var kvp = urlparts[1].split(/[&;]/g);
		for (var i = kvp.length - 1; i >= 0; i--) {
			var x = kvp[i].split('=');

			if (x[0] == param) {
				x[1] = value;
				kvp[i] = x.join('=');
				break;
			}
		}

		if (i < 0) {
			kvp[kvp.length] = [param, value].join('=');
		}
		return urlparts[0] + '?' + kvp.join('&');
	};

	window.Gsp.RemoveQSParm = function (url, param) {
		var urlparts = url.split('?');
		if (urlparts.length < 2)
			return url;

		var prefix = encodeURIComponent(param) + '=';
		var pars = urlparts[1].split(/[&;]/g);
		for (var i = pars.length - 1; i >= 0; i--)
			if (pars[i].lastIndexOf(prefix, 0) !== -1)
				pars.splice(i, 1);

		if (pars.length > 0)
			return urlparts[0] + '?' + pars.join('&');
		else
			return urlparts[0];
	};

	window.Gsp.DisposeAjaxComponent = function (id) {
		if (typeof Sys === 'undefined' || typeof Sys.Application === 'undefined')
			return;

		if (id && id.length > 0) {
			var obj = Sys.Application.findComponent(id);
			if (obj) obj.dispose();
		}
	};

	//#endregion End Gallery-wide functions

	//#region AJAX functions

	window.Gsp.DataService = new function () {

		logOff = function (callback) {
			$.post(window.Gsp.AppRoot + '/api/task/logoff',
				function (data) {
					callback(data);
				});
		};

		deleteMediaObject = function (mediaObjectId, completeCallback, successCallback, errorCallback) {
			$.ajax(({
				type: "DELETE",
				url: window.Gsp.AppRoot + '/api/mediaitems/' + mediaObjectId,
				complete: completeCallback,
				success: successCallback,
				error: errorCallback
			}));
		};

		saveAlbum = function (album, completeCallback, successCallback, errorCallback) {
			var a = Gsp.deepCopy(album);
			a.MediaItems = null;
			a.GalleryItems = null;
			a.MetaItems = null;
			a.Permissions = null;

			$.ajax(({
				type: "POST",
				url: url = window.Gsp.AppRoot + '/api/albums/post',
				data: JSON.stringify(a),
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				complete: completeCallback,
				success: successCallback,
				error: errorCallback
			}));
		};

		saveMeta = function (galleryItemMeta, completeCallback, successCallback, errorCallback) {
			$.ajax(({
				type: "PUT",
				url: window.Gsp.AppRoot + '/api/galleryitemmeta',
				data: JSON.stringify(galleryItemMeta),
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				complete: completeCallback,
				success: successCallback,
				error: errorCallback
			}));
		};

		return {
			logOff: logOff,
			deleteMediaObject: deleteMediaObject,
			saveAlbum: saveAlbum,
			saveMeta: saveMeta
		};

	}();

	//#endregion

	//#region gspTagCloud plug-in

	$.fn.gspTagCloud = function (data, options) {
		var self = this;
		var settings = $.extend({}, $.fn.gspTagCloud.defaults, options);

		var getTagDataAndRender = function () {
			$.ajax({
				type: "GET",
				url: options.tagCloudUrl,
				contentType: "application/json; charset=utf-8",
				complete: function () {
					self.removeClass('gsp_wait');
				},
				success: function (tags) {
					var tc = new GspTagCloud(self, tags, settings);
					tc.render();
				},
				error: function (response) {
					$.gspShowMsg("Action Aborted", response.responseText, { msgType: 'error', autoCloseDelay: 0 });
				}
			});
		};

		if (data == null) {
			getTagDataAndRender();
		} else {
			var gspTc = new GspTagCloud(this, data, settings);
			gspTc.render();
		}

		return this;
	};

	$.fn.gspTagCloud.defaults = {
		clientId: '',
		tagCloudType: 'tag', // 'tag' or 'people'
		tagCloudUrl: '', // The URL for retrieving tag data. Ignored when tag data is passed via data parameter
		shape: 'rectangular' // Shape of cloud. Set to false to get the elliptic shape (the default shape of JQCloud)
	};

	window.GspTagCloud = function (target, data, options) {
		this.$target = target; // A jQuery object to receive the tag cloud.
		this.TagCloudOptions = options;
		this.Data = data;
	};

	GspTagCloud.prototype.render = function () {
		var self = this;
		var parms = { title: null, tag: null, people: null, search: null, latest: null, filter: null, rating: null, top: null, aid: null, moid: null };
		var pageUrl = window.location.href;

		$.each(this.Data, function (i, el) {
			el.text = el.value;
			el.weight = el.count;
			parms[self.TagCloudOptions.tagCloudType] = el.value;
			el.link = Gsp.GetUrl(pageUrl, parms);
		});

		self.$target.jQCloud(self.Data, {
			encodeURI: false,
			shape: self.TagCloudOptions.shape
		});
	};

	//#endregion

	//#region gspTreeView plug-in

	$.fn.gspTreeView = function (data, options) {
		var self = this;
		var settings = $.extend({}, $.fn.gspTreeView.defaults, options);

		var getTreeDataAndRender = function () {
			$.ajax({
				type: "GET",
				url: options.treeDataUrl,
				contentType: "application/json; charset=utf-8",
				complete: function () {
					self.removeClass('gsp_wait');
				},
				success: function (tagTreeJson) {
					var tv = new GspTreeView(self, $.parseJSON(tagTreeJson), settings);
					tv.render();
				},
				error: function (response) {
					$.gspShowMsg("Action Aborted", response.responseText, { msgType: 'error', autoCloseDelay: 0 });
				}
			});
		};

		if (data == null) {
			getTreeDataAndRender();
		} else {
			var gspTv = new GspTreeView(this, data, settings);
			gspTv.render();
		}

		return this;
	};

	$.fn.gspTreeView.defaults = {
		clientId: '', // The ID of the HTML element containing the entire gallery. Used to scroll node into view in left pane. Omit if left pane scrolling not needed
		allowMultiSelect: false, // Indicates whether more than one node can be selected at a time
		albumIdsToSelect: null, // An array of the album IDs of any nodes to be selected during rendering
		checkedAlbumIdsHiddenFieldClientId: '', // The client ID of the hidden input field that stores a comma-separated list of the album IDs of currently checked nodes
		theme: 'gsp', // Used to generate the CSS class name that is applied to the HTML DOM element that contains the treeview. Ex: "gsp" is rendered as CSS class "jstree-gsp"
		requiredSecurityPermissions: 1, //ViewAlbumOrMediaObject
		navigateUrl: '', // The URL to the current page without query string parms. Used during lazy load ajax call. Example: "/dev/gs/gallery.aspx"
		enableCheckboxPlugin: false, // Indicates whether a checkbox is to be rendered for each node
		treeDataUrl: '' // The URL for retrieving tree data. Ignored when tree data is passed via data parameter
	};

	window.GspTreeView = function (target, data, options) {
		this.$target = target; // A jQuery object to receive the rendered treeview.
		this.TreeViewOptions = options;
		this.Data = data;
	};

	GspTreeView.prototype.render = function () {
		var self = this;
		
		this._updateNodeDataWithAlbumIdsToSelect();

		var jstreeOptions = {
			core: {
				data: function (node, cb) {
					if (node.id === '#') {
						return cb(self.Data);
					}

					$.ajax({
						url: window.Gsp.GalleryResourcesRoot + '/handler/gettreeview.ashx',
						data: {
							// Query string parms to be added to the AJAX request
							id: node.li_attr['data-id'],
							secaction: self.TreeViewOptions.requiredSecurityPermissions,
							sc: $.inArray('checkbox', this.settings.plugins) >= 0, // Whether checkboxes are being used
							navurl: self.TreeViewOptions.navigateUrl
						},
						dataType: "json",
						error: function (response, textStatus, errorThrown) {
							if (textStatus == "error") {
								alert("Oops! An error occurred while retrieving the treeview data. It has been logged in the gallery's event log.");
							}
						},
						success: function (data) {
							return cb(data);
						}
					});
					return null;
				},
				multiple: this.TreeViewOptions.allowMultiSelect,
				themes: {
					name: this.TreeViewOptions.theme,
					dots: false,
					icons: false,
					responsive: false
				}
			},
		};

		if (this.TreeViewOptions.enableCheckboxPlugin) {
			jstreeOptions.plugins = ['checkbox'];
			jstreeOptions.checkbox = {
				keep_selected_style: false,
				three_state: this.TreeViewOptions.allowMultiSelect
			};
		}

		this.$target.jstree(jstreeOptions)
			.on("ready.jstree", function (e, data) {
				self.onLoaded(e, data);
			})
			.on("changed.jstree", function (e, data) {
				self.onChangeState(e, data);
			})
			.on("deselect_node.jstree", function (e, data) {
				self.onDeselectNode(e, data);
			});
	};

	GspTreeView.prototype._storeSelectedNodesInHiddenFormField = function (data) {
		// Grab the data-id values from the top selected nodes, concatenate them and store them in a hidden
		// form field. This can later be retrieved by server side code to determine what was selected.
		if (this.TreeViewOptions.checkedAlbumIdsHiddenFieldClientId == null || this.TreeViewOptions.checkedAlbumIdsHiddenFieldClientId.length == 0)
			return;

		var topSelectedNodes = data.instance.get_top_selected(true);
		var albumIds = $.map(topSelectedNodes, function (val, i) {
			return val.li_attr['data-id'];
		}).join();

		$('#' + this.TreeViewOptions.checkedAlbumIdsHiddenFieldClientId).val(albumIds);
	};

	GspTreeView.prototype._updateNodeDataWithAlbumIdsToSelect = function () {
		// Process the albumIdsToSelect array - find the matching node in the data and change state.selected to true
		// Note that in many cases the nodes are pre-selected in server side code. This function isn't needed in those cases.
		if (Gsp.isNullOrEmpty(this.TreeViewOptions.albumIdsToSelect))
			return;

		var findMatch = function (nodeArray, dataId) {
			// Search nodeArray for a node having data-id=dataId, acting recursively
			if (Gsp.isNullOrEmpty(nodeArray))
				return null;

			var matchingNode = $.grep(nodeArray, function (n) { return n.li_attr['data-id'] === dataId; })[0] || null;

			if (matchingNode != null)
				return matchingNode;

			// Didn't find it, so recursively search node data
			$.each(nodeArray, function (idx, n) {
				matchingNode = findMatch(n.children, dataId);

				if (matchingNode != null) {
					return false; // Break out of $.each
				}
			});

			return matchingNode;
		};

		var self = this;
		$.each(this.TreeViewOptions.albumIdsToSelect, function (idx, id) {
			var node = findMatch(self.Data, id);

			if (node != null) {
				node.state.selected = true;
			}
		});
	};

	GspTreeView.prototype.onChangeState = function (e, data) {
		if (data.action == 'select_node') {
			var url = data.instance.get_node(data.node, true).children('a').attr('href');

			if (url != null && url.length > 1) {
				// Selected node is a hyperlink with an URL, so navigate to it.
				document.location = url;
				return;
			}
		}

		if (data.action == 'deselect_node' || data.action == 'select_node') {
			this._storeSelectedNodesInHiddenFormField(data);
		}
	};

	GspTreeView.prototype.onDeselectNode = function (e, data) {
		// Don't let user deselect the only selected node when allowMultiSelect=false
		if (!this.TreeViewOptions.allowMultiSelect && data.instance.get_selected().length == 0) {
			data.instance.select_node(data.node);
		}
	};

	GspTreeView.prototype.onLoaded = function (e, data) {
		this._storeSelectedNodesInHiddenFormField(data);

		// Scroll the left pane if necessary so that the selected node is visible
		if (this.TreeViewOptions.clientId.length < 1)
			return;

		var selectedIds = data.instance.get_selected();
		if (selectedIds != null && selectedIds.length == 1) {
			var nodeOffsetTop = $('#' + selectedIds[0]).position().top;
			var leftPaneHeight = $('#' + this.TreeViewOptions.clientId + '_lpHtml').height();
			if (nodeOffsetTop > leftPaneHeight) {
				$('#' + this.TreeViewOptions.clientId + '_lpHtml').animate({ scrollTop: nodeOffsetTop }, 200, "linear");
			}
		}
	};

	//#endregion gspTreeView plug-in

	//#region gspMedia plug-in

	$.fn.gspMedia = function (tmplName, data) {
		var gspMedia = new GspMedia(tmplName, this, data);
		gspMedia.initialize();

		return this;
	};

	// Define object for handling the viewing of a single media object. Uses the Revealing Prototype Pattern:
	// http://weblogs.asp.net/dwahlin/archive/2011/08/01/techniques-strategies-and-patterns-for-structuring-javascript-code-the-prototype-pattern.aspx
	window.GspMedia = function (tmplName, target, data) {
		this.TemplateName = tmplName; // The name of a compiled jsRender template
		this.$target = target; // A jQuery object to receive the rendered HTML from the template.
		this.IdPrefix = data.Settings.ClientId;
		this.Data = data;
		this.Toolbar = null;
		this.Timer = null;

		this.inCallback = false;
	};

	GspMedia.prototype.initialize = function () {
		if (!this.Data.MediaItem) {
			$.gspShowMsg("Cannot Render Media Object", "<p>Cannot render the media object template. Navigate to a media object and then return to this page.</p><p>You'll know you got it right when you see 'moid' In the URL's query string.</p><p>ERROR: this.Data.MediaItem is null.</p>", { msgType: 'error', autoCloseDelay: 0 });
			return;
		}

		this.jsRenderSetup();
		this.overwriteMediaObject();
		this.attachEvents();
		this.configureMediaItem();
		this.render();
		this.preloadImages();
	};

	GspMedia.prototype.jsRenderSetup = function () {
		// Create a few helper functions that can be used in the jsRender template.
		var self = this;
		$.views.helpers({
			prevUrl: function () {
				// Generate the URL to the previous media item.
				var prvMi = self.getPreviousMediaObject();
				return prvMi ? self.getPermalink(prvMi.Id) : Gsp.GetAlbumUrl(self.Data.Album.Id, true);
			},
			nextUrl: function () {
				// Generate the URL to the next media item.
				var nxtMi = self.getNextMediaObject();
				return nxtMi ? self.getPermalink(nxtMi.Id) : Gsp.GetAlbumUrl(self.Data.Album.Id, true);
			},
			getEmbedCode: function () {
				var url = Gsp.GetUrl(self.Data.App.AppUrl + '/' + self.Data.App.GalleryResourcesPath + '/embed.aspx' + location.search, { aid: null, moid: self.Data.MediaItem.Id });

				return "<iframe allowtransparency='true' frameborder='0' sandbox='allow-same-origin allow-forms allow-scripts' scrolling='auto' src='"
					+ url + "' style='width:100%;height:100%'></iframe>";
			},
		});
	};

	GspMedia.prototype.configureMediaItem = function () {
		this.setSize(Gsp.Constants.ViewSize_Optimized);
	};

	GspMedia.prototype.render = function () {
		this.dataBind();
		this.bindToolbar();
		this.runMediaObjectScript();

		if (this.Data.Settings.SlideShowIsRunning)
			this.startSlideshow();

		if (history.replaceState) history.replaceState(null, "", this.getPermalink(this.Data.MediaItem.Id));
	};

	GspMedia.prototype.setSize = function (viewSize) {
		var defaultViewIndex = 0;

		for (var i = 0; i < this.Data.MediaItem.Views.length; i++) {
			if (this.Data.MediaItem.Views[i].ViewSize == viewSize) {
				this.Data.MediaItem.ViewIndex = i; // Get index corresponding to requested size
				return;
			} else if (this.Data.MediaItem.Views[i].ViewSize == Gsp.Constants.ViewSize_Original
				|| this.Data.MediaItem.Views[i].ViewSize == Gsp.Constants.ViewSize_External)
				defaultViewIndex = i;
		}

		// If we get here, we couldn't find a match for the requested size, so default to showing original or external
		this.Data.MediaItem.ViewIndex = defaultViewIndex;
	};

	GspMedia.prototype.startSlideshow = function () {
		// Returns true when successfully started; otherwise false
		var self = this;
		if (this.Data.Settings.SlideShowType == 'FullScreen') {
			this.removeCursorNavigationHandler();

			var ss = new window.GspFullScreenSlideShow(this.Data,
			{
				on_exit: function (currentId) {
					self.Data.Settings.SlideShowIsRunning = false;
					self.addCursorNavigationHandler(self);
					
					if (self.Data.MediaItem.Id != currentId) {
						// User ended on a different image than they started. Show that image.
						self.showMediaObject(currentId);
					}
				}
			});

			this.Data.Settings.SlideShowIsRunning = ss.startSlideShow();
		}
		else if (this.Data.Settings.SlideShowType == 'Inline') {
			if (this.Timer && this.Timer.isRunning)
				return true;

			this.Data.Settings.SlideShowIsRunning = true;
			if (this.getNextMediaObject() != null) {
				this.Timer = new GspTimer(this.showNextMediaObject, this.Data.Settings.SlideShowIntervalMs, this);
				this.Timer.start();
			} else {
				this.Data.Settings.SlideShowIsRunning = false;
				$.gspShowMsg(this.Data.Resource.MoNoSsHdr, this.Data.Resource.MoNoSsBdy, { msgType: 'info' });
			}
		}

		return this.Data.Settings.SlideShowIsRunning;
	};

	GspMedia.prototype.stopSlideshow = function () {
		if (this.Timer) this.Timer.stop();
		this.Data.Settings.SlideShowIsRunning = false;
	};

	GspMedia.prototype.dataBind = function () {
		Gsp.DisposeAjaxComponent(window.Gsp.msAjaxComponentId); // Dispose Silverlight component (if necessary)
		this.$target.html($.render[this.TemplateName](this.Data)); // Render HTML template and add to page
		this.animateMediaObject(); // Execute transition effect
		this.attachMediaEvents();
		this.makeCaptionEditable();
	};

	GspMedia.prototype.makeCaptionEditable = function () {
		if (!this.Data.Album.Permissions.EditMediaObject)
			return;

		var self = this;

		$(".gsp_mediaObjectTitle", this.$target)
			.addClass('gsp_editableContent')
			.hover(function () {
				$(this).removeClass('gsp_editableContent').addClass('gsp_editableContentHover');
			}, function () {
				$(this).removeClass('gsp_editableContentHover').addClass('gsp_editableContent');
			})
			.editable(window.Gsp.AppRoot + '/api/mediaitems',
				{
					type: 'textarea',
					rows: 4,
					tooltip: this.Data.Resource.MediaCaptionEditTt,
					event: 'click',
					submit: this.Data.Resource.MediaCaptionEditSave,
					cancel: this.Data.Resource.MediaCaptionEditCancel,
					cssclass: 'gsp_editableContentForm',
					indicator: '<img class="gsp_wait" src="' + this.Data.App.SkinPath + '/images/wait-squares.gif" />',
					style: "inherit",
					onblur: '', // Setting to empty value prevents auto-cancel when blurring away from input (specify submit to submit on blur)
					oneditbegin: function (s, el, e) { return self.onEditBegin($(el), e); },
					onreset: function (frm, obj) { self.setCaptionHover($(obj), true); },
					onsubmit: function (settings, el) {
						// Get the media item from the instance and update the value with the data the user entered.
						self.Data.MediaItem.Title = $('textarea, input, select', $(el)).val();

						// Assign the media item to the AJAX options. This is how the data gets to the server.
						settings.ajaxoptions.data = JSON.stringify(self.Data.MediaItem);
					},
					oncomplete: function (xmldata) {
						var m = self.Data.MediaItem; // Get reference to media item
						var actionResult = $.parseJSON(xmldata); // Hydrate ActionResult instance returned from server
						m.Title = actionResult.ActionTarget.Title; // Update title (which may have been changed by the server (e.g. HTML removed))

						self.$target.trigger('mediaUpdate.' + self.Data.Settings.ClientId, [self.Data.ActiveGalleryItems]);

						return m.Title;
					},
					callback: function (value, settings) {
						self.Data.MediaItem.Title = value;
						self.setCaptionHover($(this), true);
					},
					ajaxoptions: {
						type: 'PUT',
						contentType: 'application/json;charset=utf-8'
					}
				});
	};

	GspMedia.prototype.onEditBegin = function (el, e) {
		this.setCaptionHover(el, false);

		// Return false when a hyperlink is clicked. Calling code will cancel the edit and allow the navigation to the link.
		return (e.target.tagName != 'A');
	};

	GspMedia.prototype.setCaptionHover = function (el, enable) {
		if (enable) {
			el.hover(function () {
				el.removeClass('gsp_editableContent').addClass('gsp_editableContentHover');
			}, function () {
				el.removeClass('gsp_editableContentHover').addClass('gsp_editableContent');
			});
		} else {
			el.removeClass('gsp_editableContentHover').addClass('gsp_editableContent').unbind('mouseenter').unbind('mouseleave');
		}
	};

	GspMedia.prototype.animateMediaObject = function () {
		var self = this;

		var hideMediaObject = function (moEl) {
			// If it is an image and a transition is specified, then hide the media object container so that it
			// can later be shown with the transition effect. Returns true when object is hidden; otherwise returns false.
			var isImage = self.Data.MediaItem.ItemType == Gsp.Constants.ItemType_Image;
			var hasTransition = self.Data.Settings.TransitionType != 'none';

			if (isImage && hasTransition) {
				// Explicitly set the height of the parent element so that the page doesn't reflow when the media object is hidden.
				// Line commented out 2012-06-05 because it added a vertical scrollbar and no longer seemed required
				//moEl.parent().height(moEl.parent().height());
				moEl.hide();
				return true;
			} else {
				return false;
			}
		};

		var anEl = $(".gsp_moContainer", this.$target);

		if (hideMediaObject(anEl)) {
			// Valid:  'none', 'fade', 'blind', 'bounce', 'clip', 'drop', 'explode', 'fold', 'highlight', 'puff', 'pulsate', 'scale', 'shake', 'size', 'slide', 'transfer'.
			switch (this.Data.Settings.TransitionType) {
				case 'none':
					anEl.show();
					break;
				case 'fade':
					anEl.fadeIn(this.Data.Settings.TransitionDurationMs);
					break;
				default:
					var options = {};
					// Some effects have required parameters
					if (this.Data.Settings.TransitionType === "scale") options = { percent: 100 };

					anEl.toggle(this.Data.Settings.TransitionType, options, this.Data.Settings.TransitionDurationMs);
					break;
			}
		}
	};

	GspMedia.prototype.showPreviousMediaObject = function (e) {
		this.Data.MediaItem = this.getPreviousMediaObject();
		if (this.Data.MediaItem) {
			if (e) e.preventDefault(); // Prevent the event from bubbling (prevents hyperlink navigation on next/previous buttons)
			this.Data.ActiveGalleryItems = [window.Gsp.convertMediaItemToGalleryItem(this.Data.MediaItem)];
			$('#' + this.IdPrefix + '_moid').val(this.Data.MediaItem.Id);
			this.setSize(Gsp.Constants.ViewSize_Optimized);
			this.render(); // Re-bind the template
			this.$target.trigger('previous.' + this.Data.Settings.ClientId, [this.Data.ActiveGalleryItems]);
		} else this.redirectToAlbum();
	};

	GspMedia.prototype.showNextMediaObject = function (e) {
		this.Data.MediaItem = this.getNextMediaObject();
		if (this.Data.MediaItem) {
			if (e) e.preventDefault(); // Prevent the event from bubbling (prevents hyperlink navigation on next/previous buttons)
			this.Data.ActiveGalleryItems = [window.Gsp.convertMediaItemToGalleryItem(this.Data.MediaItem)];
			$('#' + this.IdPrefix + '_moid').val(this.Data.MediaItem.Id);
			this.setSize(Gsp.Constants.ViewSize_Optimized);
			this.render(); // Re-bind the template
			this.$target.trigger('next.' + this.Data.Settings.ClientId, [this.Data.ActiveGalleryItems]);
		} else this.redirectToAlbum();
	};

	GspMedia.prototype.showMediaObject = function (id) {
		this.Data.MediaItem = $.grep(this.Data.Album.MediaItems, function (mi) { return mi.Id === id; })[0];
		if (this.Data.MediaItem) {
			this.Data.ActiveGalleryItems = [window.Gsp.convertMediaItemToGalleryItem(this.Data.MediaItem)];
			$('#' + this.IdPrefix + '_moid').val(this.Data.MediaItem.Id);
			this.setSize(Gsp.Constants.ViewSize_Optimized);
			this.render(); // Re-bind the template
			this.$target.trigger('mediaUpdate.' + this.Data.Settings.ClientId, [this.Data.ActiveGalleryItems]);
		} else this.redirectToAlbum();
	};

	GspMedia.prototype.getPreviousMediaObject = function () {
		return this.Data.Album.MediaItems[$.inArray(this.Data.MediaItem, this.Data.Album.MediaItems) - 1];
	};

	GspMedia.prototype.getNextMediaObject = function () {
		if (this.Data.Settings.SlideShowIsRunning) {
			// Return the next *image* media object
			var mo = this.Data.MediaItem;
			do {
				mo = this.Data.Album.MediaItems[$.inArray(mo, this.Data.Album.MediaItems) + 1];
			} while (mo && mo.MimeType != Gsp.Constants.MimeType_Image);
			return mo;
		} else {
			// Return the next media object
			return this.Data.Album.MediaItems[$.inArray(this.Data.MediaItem, this.Data.Album.MediaItems) + 1];
		}
	};

	GspMedia.prototype.redirectToAlbum = function () {
		window.location = Gsp.GetAlbumUrl(this.Data.Album.Id);
	};

	GspMedia.prototype.runMediaObjectScript = function () {
		if (this.Data.MediaItem.Views[this.Data.MediaItem.ViewIndex].ScriptOutput.length > 0) {
			(new Function((this.Data.MediaItem.Views[this.Data.MediaItem.ViewIndex].ScriptOutput)))();
		}
	};

	GspMedia.prototype.getPermalink = function (id) {
		return Gsp.GetUrl(document.location.href, { moid: id });
	};

	GspMedia.prototype.bindToolbar = function () {
		this.Toolbar = this.buildMediaToolbar(this);
	};

	GspMedia.prototype.overwriteMediaObject = function () {
		// Overwrite the this.Data.MediaItem object that was parsed from JSON with the equivalent object from the collection. We do this so that
		// we can later use $.inArray to find the current item in the array.
		for (var i = 0; i < this.Data.Album.MediaItems.length; i++) {
			if (this.Data.Album.MediaItems[i].Id == this.Data.MediaItem.Id) {
				this.Data.MediaItem = this.Data.Album.MediaItems[i];
				return;
			}
		}
	};

	GspMedia.prototype.attachEvents = function () {
		// This runs once when initialized, so don't wire up any events on items *inside* the template, since
		// they'll be erased when the user navigates between media objects. (Do that in attachMediaEvents())
		var self = this;

		// Attach a handler for when a metaitem is updated.
		$('#' + this.Data.Settings.MediaClientId).on('metaUpdate.' + this.Data.Settings.ClientId, function (e, gim) { self.onMetaUpdate(e, gim); });

		this.addCursorNavigationHandler(self);
	};

	GspMedia.prototype.attachMediaEvents = function () {
		// This runs each time the template is rendered, so here we wire up events to any elements inside the rendered HTML.
		var self = this;

		// Attach handlers for next/previous clicks.
		$(".gsp_mvPrevBtn", this.$target).on("click", function (e) { self.showPreviousMediaObject(e); });
		$(".gsp_mvNextBtn", this.$target).on("click", function (e) { self.showNextMediaObject(e); });
	};

	GspMedia.prototype.addCursorNavigationHandler = function (gspMediaInstance) {
		if (this.Data.Settings.ShowMediaObjectNavigation) {
			$(document.documentElement).on('keydown.' + this.Data.Settings.ClientId, function (e) {
				if ((e.target.tagName == 'INPUT') || (e.target.tagName == 'TEXTAREA')) return; // Ignore when focus is in editable box

				if (e.keyCode == 37) gspMediaInstance.showPreviousMediaObject(e); // 37 = left arrow
				if (e.keyCode == 39) gspMediaInstance.showNextMediaObject(e); // 39 = right arrow
			});
		}
	};

	GspMedia.prototype.removeCursorNavigationHandler = function () {
		$(document.documentElement).off('keydown.' + this.Data.Settings.ClientId);
	};

	GspMedia.prototype.onMetaUpdate = function (e, gim) {
		// Event handler for when a meta item has been updated. e is the jQuery event object; gim is the GalleryItemMeta instance.
		if (gim.MetaItem.MTypeId == 29) { // 29 = MetadataItemName.Title
			this.render(); // At some point we may want to move this outside the 'if' if the media template uses other metadata values
		} else if (gim.MetaItem.MTypeId == 112) { // 112 = MetadataItemName.HtmlSource
			this.render();
		}
	};

	GspMedia.prototype.preloadImages = function () {
		// Create an array of all optimized or original image URLs
		var urls = $.map(this.Data.Album.MediaItems, function (mo) {
			for (var i = 0; i < mo.Views.length; i++) {
				if ((mo.Views[i].ViewType == Gsp.Constants.MimeType_Image) && (mo.Views[i].ViewSize == Gsp.Constants.ViewSize_Optimized))
					return mo.Views[i].Url;
			}
		});

		// Create an image tag & set the source
		$(urls).each(function () { $('<img>').attr('src', this); });
	};

	GspMedia.prototype.buildMediaToolbar = function () {
		var self = this;

		if (!this.Data.Settings.ShowMediaObjectToolbar) {
			$(".gsp_mvToolbar", this.$target).hide();

			return null;
		}

		var tb = new GspMediaToolbar();
		tb.EmbedButton = $(".gsp_mvTbEmbed", this.$target);
		tb.SlideshowButton = $(".gsp_mvTbSlideshow", this.$target);
		tb.MoveButton = $(".gsp_mvTbMove", this.$target);
		tb.CopyButton = $(".gsp_mvTbCopy", this.$target);
		tb.RotateButton = $(".gsp_mvTbRotate", this.$target);
		tb.DeleteButton = $(".gsp_mvTbDelete", this.$target);

		if (this.Data.Settings.ShowUrlsButton) {
			var dgShare = $(".gsp_mo_share_dlg", this.$target);

			dgShare.dialog({
				appendTo: '#' + self.Data.Settings.ClientId,
				autoOpen: false,
				draggable: false,
				resizable: false,
				closeOnEscape: true,
				dialogClass: 'gsp_mo_share_dlg_container',
				width: 420,
				minHeight: 0,
				show: 'fade',
				hide: 'fade',
				position: { my: "left top", at: "left bottom", of: $(".gsp_mvTbEmbed", this.$target) },
				open: function (e, ui) {
					$(this).parent().focus();
					$(document).on("click", function (e1) {
						if ($(e1.target).parents('.gsp_mo_share_dlg_container').length == 0) {
							dgShare.dialog('close');
							$(this).unbind(e1);
						}
					});
				}
			});

			$('input.gsp_mo_share_dlg_ipt,textarea.gsp_mo_share_dlg_ipt').click(function (e) {
				$(this).select(); // Auto select text for easy copy and paste
			});

			// Update download link when user selects new size in dropdown
			$(".gsp_mo_share_dlg_ipt_select", dgShare).on('change', function (e) {
				var sel = this; // The select HTML element containing the sizes (thmb, opt, orig).

				var getMediaUrl = function (dt) {
					var url = self.Data.App.AppUrl + '/' + self.Data.App.GalleryResourcesPath +
						"/handler/getmedia.ashx?moid=" + self.Data.MediaItem.Id +
						"&dt=" + dt +
						"&g=" + self.Data.Settings.GalleryId + "&sa=1";

					return url;
				};

				$('.gsp_mo_share_dwnld', dgShare).attr('href', getMediaUrl(sel.value));

				if (self.Data.MediaItem.ItemType == window.Gsp.Constants.ItemType_External) {
					$('.gsp_mo_share_dwnld', dgShare).click(function (e1) {
						if (parseInt(sel.value) != window.Gsp.Constants.ViewSize_Thumbnail) {
							e1.preventDefault();
							e1.stopPropagation();
							$.gspShowMsg('Download not available', 'External media objects cannot be downloaded. However, the source HTML can be accessed in the right pane.', { msgType: 'info', autoCloseDelay: 0 });
						}
					});
				}
			}).change();

			tb.EmbedButton
				.button({
					text: false,
					icons: { primary: "gsp-ui-icon gsp-ui-icon-embed" }
				})
				.click(function (e) {
					if (dgShare.dialog('isOpen') === true)
						dgShare.dialog('close');
					else {
						dgShare.dialog('open');
					}
					return false;
				});
		} else {
			tb.EmbedButton.hide();
		}

		if (this.Data.Settings.ShowSlideShowButton) {
			var createInlineSlideShowBtn = function () {
				var playOptions = { text: false, label: self.Data.Resource.MoTbSsStart, icons: { primary: "gsp-ui-icon gsp-ui-icon-ssplay" } };
				var pauseOptions = { text: false, label: self.Data.Resource.MoTbSsStop, icons: { primary: "gsp-ui-icon gsp-ui-icon-sspause" } };

				tb.SlideshowButton
					.prop('checked', self.Data.Settings.SlideShowIsRunning)
					.button(self.Data.Settings.SlideShowIsRunning ? pauseOptions : playOptions)
					.click(function (e) {
						if ($(this).prop('checked')) {
							if (self.startSlideshow()) {
								$(this).removeClass('gsp_mvTbSlideshow_pause').addClass('gsp_mvTbSlideshow_play')
									.button("option", pauseOptions);
							}
						} else {
							self.stopSlideshow();
							$(this).removeClass('gsp_mvTbSlideshow_play').addClass('gsp_mvTbSlideshow_pause')
								.button("option", playOptions);
						}
					});
			};

			var createFullScreenSlideShowBtn = function () {
				tb.SlideshowButton
					.button({
						text: false,
						icons: { primary: "gsp-ui-icon gsp-ui-icon-ssplay" }
					})
					.click(function (e) {
						self.startSlideshow();
						return false;
					});
			};

			if (this.Data.Settings.SlideShowType == 'Inline')
				createInlineSlideShowBtn();
			else if (this.Data.Settings.SlideShowType == 'FullScreen')
				createFullScreenSlideShowBtn();
			else
				$.gspShowMsg("Error", 'Unrecognized SlideShowType value: ' + this.Data.Settings.SlideShowType, { msgType: 'error', autoCloseDelay: 0 });
		} else {
			tb.SlideshowButton.add(tb.SlideshowButton.next()).hide();
		}

		if (this.Data.Settings.ShowTransferMediaObjectButton && this.Data.Album.Permissions.DeleteMediaObject && this.Data.User.CanAddMediaToAtLeastOneAlbum) {
			tb.MoveButton.button({
				text: false,
				icons: {
					primary: "gsp-ui-icon gsp-ui-icon-move"
				}
			})
				.click(function (e) {
					window.location = Gsp.GetUrl(window.location.href, { g: 'task_transferobject', moid: self.Data.MediaItem.Id, tt: 'move', skipstep1: 'true' });
					e.preventDefault();
				});
		} else {
			tb.MoveButton.hide();
		}

		if (this.Data.Settings.ShowCopyMediaObjectButton && this.Data.User.CanAddMediaToAtLeastOneAlbum) {
			tb.CopyButton.button({
				text: false,
				icons: {
					primary: "gsp-ui-icon gsp-ui-icon-copy"
				}
			})
				.click(function (e) {
					window.location = Gsp.GetUrl(window.location.href, { g: 'task_transferobject', moid: self.Data.MediaItem.Id, tt: 'copy', skipstep1: 'true' });
					e.preventDefault();
				});
		} else {
			tb.CopyButton.hide();
		}

		if (this.Data.Settings.ShowRotateMediaObjectButton && this.Data.Album.Permissions.EditMediaObject && !this.Data.Settings.IsReadOnlyGallery) {
			tb.RotateButton.button({
				text: false,
				icons: {
					primary: "gsp-ui-icon gsp-ui-icon-rotate"
				}
			})
				.click(function (e) {
					window.location = Gsp.GetUrl(window.location.href, { g: 'task_rotateimage', moid: self.Data.MediaItem.Id });
					e.preventDefault();
				});
		} else {
			tb.RotateButton.hide();
		}

		if (this.Data.Settings.ShowDeleteMediaObjectButton && this.Data.Album.Permissions.DeleteMediaObject) {
			tb.DeleteButton.button({
				text: false,
				icons: {
					primary: "gsp-ui-icon gsp-ui-icon-delete"
				}
			})
				.click(function (e) {
					var removeMediaObject = function (idx) {
						// Remove the media object at the specified index from the client data and show the next media object
						// (or previous if there aren't any subsequent items).
						self.Data.Album.MediaItems.gspRemove(idx);

						if (idx >= self.Data.Album.MediaItems.length)
							idx = self.Data.Album.MediaItems.length - 1; // Deleted item was the last one; set index to 2nd to last one

						if (idx >= 0) {
							$.each(self.Data.Album.MediaItems, function (indx, mo) {
								mo.Index = indx + 1; // Re-assign the index values of each media object
							});

							self.Data.Album.NumGalleryItems--;
							self.Data.Album.NumMediaItems--;
							self.Data.MediaItem = self.Data.Album.MediaItems[idx - 1];
							self.showNextMediaObject();
						} else self.Data.MediaItem = null; // No more items in album; set to null. Calling code will detect and then redirect
					};

					if (confirm(self.Data.Resource.MediaDeleteConfirm)) {
						{
							var id = self.Data.MediaItem.Id;
							removeMediaObject($.inArray(self.Data.MediaItem, self.Data.Album.MediaItems));

							window.Gsp.DataService.deleteMediaObject(id, null, function () {
								if (self.Data.MediaItem == null) self.redirectToAlbum();
							}, function () {
								$.gspShowMsg("Cannot Delete Media Object", "An error occurred on the server. Check the gallery's event log for details.", { msgType: 'error', autoCloseDelay: 0 });
							});
						}
					}
					e.preventDefault();
				});
		} else {
			tb.DeleteButton.hide();
		}

		return tb;
	};

	window.GspMediaToolbar = function () {
		this.EmbedButton = null;
		this.SlideshowButton = null;
		this.MoveButton = null;
		this.CopyButton = null;
		this.RotateButton = null;
		this.DeleteButton = null;
	};

	//#endregion End Media view functions

	//#region gspThumbnails plug-in

	$.fn.gspThumbnails = function (tmplName, data) {
		var self = this;
		var $target = this;

		var initialize = function () {

			var attachEvents = function () {
				// Attach a handler for when a metaitem is updated.
				$('#' + data.Settings.ThumbnailClientId).on('metaUpdate.' + data.Settings.ClientId, function (e, gim) {
					// Meta data has been updated. Let's re-render the thumbnail view so that is always shows the
					// latest data (e.g. the title/caption may have changed).
					// e is the jQuery event object; gim is the GalleryItemMeta instance identifying what was updated.
					var selItemsOld = $(".ui-selected", this); // Grab a reference to which thumbnails are selected
					renderThmbView(); // Render the template (which will wipe out the current selection)

					// Re-select thumbnails that were previously selected
					$(".thmb", self).filter(function () {
						return selItemsOld.is("li[data-id=" + $(this).data("id") + "][data-it=" + $(this).data("it") + "]");
					}).addClass("ui-selected");

					// Fire the stop event function to simulate the user selecting the thumbnails.
					var s = $(".thmb", self).parent().selectable("option", "stop");
					if (typeof (s) == 'function')
						s();
				});
			};

			var renderThmbView = function () {
				var albumHtml = $.render[tmplName](data);

				if (!renderPager(albumHtml)) {
					self.html(albumHtml); // Render HTML template and add to page
					configThmbs(); // Assign width & height of thumbnails, make selectable
				}

				configHeader();
			};

			var configHeaderAlbumPrivacy = function () {
				$(".gsp_abm_sum_pvt_trigger", $target).click(function (e) {

					var self2 = this;

					var toggleAlbumPrivacy = function () {
						data.Album.IsPrivate = !data.Album.IsPrivate;

						$(self2)
							.find('img').attr("src", data.Album.IsPrivate ? data.App.SkinPath + '/images/lock-active-s.png' : data.App.SkinPath + '/images/lock-s.png')
							.attr('title', data.Album.IsPrivate ? data.Resource.AbmIsPvtTt : data.Resource.AbmNotPvtTt);
					};

					if (!data.Settings.AllowAnonBrowsing) {
						$.gspShowMsg(data.Resource.AbmAnonDisabledTitle, data.Resource.AbmAnonDisabledMsg);
						return false;
					}

					$target.addClass('gsp_wait'); // Show wait animated gif
					toggleAlbumPrivacy();

					Gsp.DataService.saveAlbum(data.Album, null, function () {
						var msg = (data.Album.IsPrivate ? data.Resource.AbmIsPvtTt : data.Resource.AbmNotPvtTt);
						$.gspShowMsg(data.Resource.AbmPvtChngd, msg);
						$target.removeClass('gsp_wait');
					},
						function (response) {
							toggleAlbumPrivacy(); // Revert back
							$.gspShowMsg("Cannot Edit Album", response.responseText, { msgType: 'error', autoCloseDelay: 0 });
							$target.removeClass('gsp_wait');
						});

					return false;
				});
			};

			var configHeaderSort = function () {
				// Handle the sorting functionality.
				var changeAlbumSortAndRebind = function () {
					// A user with edit album permission is requesting to sort the album. Send the request to the server,
					// which sorts the album and persists it to the DB. When complete, make a GET request to get the newly sorted items,
					// then assign the results to our local data object and rebind.
					$target.addClass('gsp_wait'); // Show wait animated gif

					$.ajax({
						type: "POST",
						url: url = window.Gsp.AppRoot + '/api/albums/' + data.Album.Id + '/sortalbum?sortByMetaNameId=' + data.Album.SortById + '&sortAscending=' + data.Album.SortUp,
						success: function () {
							// The items have been resorted on the server and persisted to the DB. Now get them.
							$.ajax({
								type: "GET",
								url: window.Gsp.AppRoot + '/api/albums/' + data.Album.Id + '/galleryitems',
								success: function (galleryItems) {
									data.Album.GalleryItems = galleryItems;
									renderThmbView();
									$target.removeClass('gsp_wait');
								},
								error: function (response) {
									$.gspShowMsg("Action Aborted", response.responseText, { msgType: 'error', autoCloseDelay: 0 });
									$target.removeClass('gsp_wait');
								}
							});
						},
						error: function (response) {
							$.gspShowMsg("Action Aborted", response.responseText, { msgType: 'error', autoCloseDelay: 0 });
							$target.removeClass('gsp_wait');
						}
					});
				};

				var getAlbumSortAndRebind = function () {
					// A user without edit album permission is requesting to sort the album, or the album is a virtual one.
					// Send the request to the server, which sorts the album and returns the results. Assign the results 
					// to our local data object and rebind.
					var getSortRequestData = function () {
						var galleryItemAction = {};

						galleryItemAction.Album = Gsp.deepCopy(data.Album);
						galleryItemAction.SortByMetaNameId = data.Album.SortById;
						galleryItemAction.SortAscending = data.Album.SortUp;

						galleryItemAction.Album.MediaItems = null; // Save bandwidth by getting rid of unnecessary data
						galleryItemAction.Album.MetaItems = null; // Save bandwidth by getting rid of unnecessary data

						if (galleryItemAction.Album.Id > Gsp.Constants.IntMinValue) {
							// Save bandwidth by not passing gallery items to server. The server will load them from the album ID.
							galleryItemAction.Album.GalleryItems = null;
						}

						return galleryItemAction;
					};

					$target.addClass('gsp_wait'); // Show wait animated gif

					$.ajax(({
						type: "POST",
						url: window.Gsp.AppRoot + '/api/albums/getsortedalbum',
						data: JSON.stringify(getSortRequestData()),
						contentType: "application/json; charset=utf-8",
						dataType: "json",
						success: function (galleryItems) {
							data.Album.GalleryItems = galleryItems;
							renderThmbView();
							$target.removeClass('gsp_wait');
						},
						error: function (response) {
							$.gspShowMsg("Action Aborted", response.responseText, { msgType: 'error', autoCloseDelay: 0 });
							$target.removeClass('gsp_wait');
						}
					}));
				};

				var sortAlbum = function () {
					if (data.Album.Permissions.EditAlbum && data.Album.VirtualType == Gsp.Constants.VirtualType_NotVirtual)
						changeAlbumSortAndRebind();
					else
						getAlbumSortAndRebind();
				};

				var getSortIndicatorHtml = function () {
					if (data.Album.SortById == window.Gsp.Constants.IntMinValue) {
						return '<span class="ui-icon ui-icon-circle-check" />';
					} else {
						return (data.Album.SortUp ? '<span class="ui-icon ui-icon ui-icon-circle-arrow-n" />' : '<span class="ui-icon ui-icon ui-icon-circle-arrow-s" />');
					}
				};

				$(".gsp_abm_sum_rs", $target)
					.button({ text: false, icons: { primary: "ui-icon-arrowthick-2-n-s" } })
					.click(function () {
						data.Album.SortUp = !data.Album.SortUp;
						sortAlbum();
						return false;
					})
					.next()
					.button({ text: false, icons: { primary: "ui-icon-triangle-1-s" } })
					.click(function () {
						var menu = $(this).parent().next().show().position({ my: "right top", at: "right bottom", of: this });
						$(document).one("click", function () {
							menu.hide();
						});
						return false;
					})
					.parent().buttonset().next().hide().menu() // menu() is applied to the <ul> containing the sort options
					.children().click(function () {
						var sortById = $('a', $(this)).data('id');
						if (sortById != null) {
							data.Album.SortUp = true; // When a new sort field is selected, reset the sort direction to ASC
							data.Album.SortById = sortById; // Get the sort ID (e.g. <li><a href='#' data-id='29'>Title</a></li>)
							sortAlbum();
						}
						return false;
					})
					.children('a[data-id=' + data.Album.SortById + ']').parent().prepend(getSortIndicatorHtml()); // Add the sort icon to the current sort field
			};

			var configHeaderShare = function () {
				var dgShare = $(".gsp_abm_sum_share_dlg", $target);

				dgShare.dialog({
					appendTo: '#' + data.Settings.ClientId,
					autoOpen: false,
					draggable: false,
					resizable: false,
					closeOnEscape: true,
					dialogClass: 'gsp_abm_sum_share_dlg_container',
					width: 420,
					minHeight: 0,
					show: 'fade',
					hide: 'fade',
					position: { my: "right top", at: "right bottom", of: $(".gsp_abm_sum_sa_trigger", $target) },
					open: function (e, ui) {
						$(this).parent().focus();
						$(document).on("click", function (e1) {
							if ($(e1.target).parents('.gsp_abm_sum_share_dlg_container').length == 0) {
								dgShare.dialog('close');
								$(this).unbind(e1);
							}
						});
					}
				});

				$(".gsp_abm_sum_sa_trigger", $target).click(function (e) {
					if (dgShare.dialog('isOpen') === true)
						dgShare.dialog('close');
					else {
						dgShare.dialog('option', 'hide', null).dialog('close').dialog('option', 'hide', 'fade'); // Kill, then restore fade for quicker closing
						dgShare.dialog('open');
					}
					return false;
				});

				$('input.gsp_abm_sum_share_dlg_ipt').click(function (e) {
					$(this).select(); // Auto select text for easy copy and paste
				});
			};

			var configHeaderSlideShow = function () {
				$(".gsp_abm_sum_ss_trigger", $target).click(function (e) {
					// Send the user to the media view page of the first image in this album, including
					// the 'ss=1' query string parm that will trigger an auto slide show when the page loads.
					var findFirstImage = function () {
						if (data.Album != null && data.Album.GalleryItems != null)
							return $.grep(data.Album.GalleryItems, function (gi) { return gi.ItemType === window.Gsp.Constants.ItemType_Image; })[0];
						else
							return null;
					};

					var img = findFirstImage();

					if (img != null) {
						var qs = { aid: null, moid: null, ss: 1 };
						qs.moid = img.Id;

						window.location = Gsp.GetUrl(document.location.href, qs);
					} else
						$.gspShowMsg(data.Resource.MoNoSsHdr, data.Resource.MoNoSsBdy, { msgType: 'info' });

					return false;
				});
			};

			var configHeaderAlbumOwner = function () {
				var dg = $(".gsp_abm_sum_ownr_dlg", $target);

				dg.dialog({
					appendTo: '#' + data.Settings.ClientId,
					autoOpen: false,
					draggable: false,
					resizable: false,
					closeOnEscape: true,
					dialogClass: 'gsp_abm_sum_ownr_dlg_container',
					width: 420,
					minHeight: 0,
					show: 'fade',
					hide: 'fade',
					position: { my: "left top", at: "left bottom", of: $(".gsp_abm_sum_ownr_trigger", $target) },
					open: function (e, ui) {
						$('.gsp_abm_sum_ownr_dlg_o > span', $(this)).gspTooltip({
							title: data.Resource.AbmOwnr,
							content: data.Resource.AbmOwnrTtDtl
						});

						var ts = this;
						setTimeout(function () { $(ts).parent().find('.gsp_abm_sum_ownr_dlg_ipt').focus(); }, 500); // Delay needed or else focus stays on tooltip icon for some reason

						$(document).on("click", function (e1) {
							if ($(e1.target).parents('.gsp_abm_sum_ownr_dlg_container').length == 0) {
								dg.dialog('close');
								$(this).unbind(e1);
							}
						});
					},
					buttons: {
						"Save": function () {
							var oldAbmOwnr = data.Album.Owner;
							data.Album.Owner = $(this).find('.gsp_abm_sum_ownr_dlg_ipt').val();

							if (oldAbmOwnr != data.Album.Owner) {
								$target.addClass('gsp_wait');
								Gsp.DataService.saveAlbum(data.Album, null, function () {
									if (Gsp.isNullOrEmpty(data.Album.Owner)) {
										$.gspShowMsg(data.Resource.AbmOwnrChngd, data.Resource.AbmOwnrClrd.format(oldAbmOwnr));
									} else {
										$.gspShowMsg(data.Resource.AbmOwnrChngd, data.Resource.AbmOwnrChngdDtl.format(data.Album.Owner));
									}
									$target.removeClass('gsp_wait');
								},
									function (response) {
										data.Album.Owner = oldAbmOwnr; // Error, so revert
										$.gspShowMsg("Cannot Edit Album", response.responseText, { msgType: 'error', autoCloseDelay: 0 });
										$target.removeClass('gsp_wait');
									});
							}

							$(this).dialog("close");
						},
						Cancel: function () {
							$(this).dialog("close");
						}
					}
				});

				$(".gsp_abm_sum_ownr_trigger", $target).click(function (e) {
					if (dg.dialog('isOpen') === true)
						dg.dialog('close');
					else {
						dg.dialog('option', 'hide', null).dialog('close').dialog('option', 'hide', 'fade'); // Kill, then restore fade for quicker closing
						dg.dialog('open');
					}
					return false;
				});
			};

			var configHeader = function () {
				// When user clicks header area, make the album the current item, unselect any selected thumbnails, and trigger event to be handled in meta plug-in
				$(".gsp_abm_sum", $target).click(function () {
					$(".thmb", self).removeClass('ui-selected');
					data.ActiveGalleryItems = [window.Gsp.convertAlbumToGalleryItem(data.Album)];
					$target.trigger('select.' + data.Settings.ClientId, [data.ActiveGalleryItems]);
				});

				configHeaderAlbumPrivacy();
				configHeaderSort();
				configHeaderShare();
				configHeaderSlideShow();
				configHeaderAlbumOwner();
			};

			var pagerRequired = function () {
				return (data.Settings.PageSize > 0 && data.Album.GalleryItems.length > data.Settings.PageSize);
			};

			var renderPager = function (albumHtml) {
				if (!pagerRequired())
					return false;

				var $albumHtml = $(albumHtml);
				var $albumHtmlThmbs = $albumHtml.find('.thmb').clone();
				var $albumHtmlWithoutThmbs = $albumHtml.find('.thmb').remove().end();
				var pager;

				var pagerOptions = {
					format: '[< c >]',
					page: null, // we await hashchange() event
					lapping: 0,
					perpage: data.Settings.PageSize,
					onSelect: function (page) {
						// Retrieve the HTML for the desired slice and replace existing thumbnails with them.
						var visibleIndices = this.slice; // Contains start and end indices for visible elements
						$(".thmb", self).remove();

						// Get array of thumbnail elements for the desired page, then add to page DOM, and configure
						var html = $albumHtmlThmbs.slice(visibleIndices[0], visibleIndices[1]);
						$('.gsp_abm_thmbs').append(html.hide().fadeIn());
						configThmbs();

						return true;
					},
					onFormat: function (type) {
						switch (type) {
							case 'block':
								// n and c
								return '<span class="gsp_pagerText">' + data.Resource.AbmPgrStatus.format(this.value, this.pages) + '</span>';
							case 'next':
								// >
								if (this.active)
									return '<a href="#' + this.value + '" title="' + data.Resource.AbmPgrNextTt + '">›</a>';
								else
									return '<span class="gsp_disabled">›</span>';
							case 'prev':
								// <
								if (this.active)
									return '<a href="#' + this.value + '" title="' + data.Resource.AbmPgrPrevTt + '">‹</a>';
								else
									return '<span class="gsp_disabled">‹</span>';
							case 'first':
								// [
								if (this.active)
									return '<a href="#' + this.value + '" title="' + data.Resource.AbmPgrFirstTt + '" class="gsp_first-child">«</a>';
								else
									return '<span class="gsp_disabled gsp_first-child">«</span>';
							case 'last':
								// ]
								if (this.active)
									return '<a href="#' + this.value + '" title="' + data.Resource.AbmPgrLastTt + '" class="gsp_last-child">»</a>';
								else
									return '<span class="gsp_disabled gsp_last-child">»</span>';
						}
					}
				};

				// Render album template except for thumbnails (we'll add the thumbs in onSelect)
				self.html($albumHtmlWithoutThmbs);

				var pgr = $();
				var pagerHtml = '<div class="gsp_pager"></div>';
				if (data.Settings.PagerLocation == 'Top' || data.Settings.PagerLocation == 'TopAndBottom') {
					pgr = $(pagerHtml).prependTo(self);
				}

				if (data.Settings.PagerLocation == 'Bottom' || data.Settings.PagerLocation == 'TopAndBottom') {
					pgr = pgr.add($(pagerHtml).appendTo(self));
				}

				pager = pgr.paging(data.Album.GalleryItems.length, pagerOptions);

				$(window).on('hashchange', (function () {
					if (window.location.hash)
						pager.setPage(window.location.hash.substr(1));
					else
						pager.setPage(1); // Default to 1st page
				}));

				$(window).trigger('hashchange');

				return true;
			};

			var hndleDom = "<div class='hndl'></div>"; // The drag handle for rearranging thumbnails

			var thmbSelected = function (e, ui) {
				// Get a reference to the selected gallery items, then trigger event to be handled in meta plug-in
				var selItems = $(".ui-selected", self).map(function () {
					var $this = $(this);
					var id = $this.data("id");
					var itemType = $this.data("it");
					// Get the gallery item that matches the thumbnail that was selected
					return $.map(data.Album.GalleryItems, function (obj) {
						return (obj.Id === id && obj.ItemType === itemType ? obj : null);
					})[0];
				}).get();

				if (data.Album.Permissions.EditAlbum && data.Album.SortById == window.Gsp.Constants.IntMinValue) {
					if (window.Gsp.isTouchScreen && selItems.length == 1) {
						$(".ui-selected", self).prepend(hndleDom);
					}
				}

				data.ActiveGalleryItems = selItems.length > 0 ? selItems : [window.Gsp.convertAlbumToGalleryItem(data.Album)];

				$target.trigger('select.' + data.Settings.ClientId, [data.ActiveGalleryItems]);
			};

			var thmbUnselected = function (e, ui) {
				$('.hndl', ui.unselected).remove();
			};

			var configThmbs = function () {
				var thmbs = $(".thmb", self);
				thmbs.equalSize(); // Make all thumbnail tags the same width & height

				// Make thumbnails selectable, but not for touchscreens when the left & right panes are hidden (user can't scroll otherwise)
				// Left/right panes not shown when < 750px wide (see gallery.css & media.ascx)
				var isSinglePaneTouchScreen = window.Gsp.isTouchScreen() && window.Gsp.isWidthLessThan(750);

				if (!isSinglePaneTouchScreen) {
					thmbs.parent().selectable({
						filter: 'li',
						cancel: "a,.hndl",
						stop: thmbSelected,
						unselected: thmbUnselected
					});
				}

				// Make thumbnails sortable
				if (data.Album.Permissions.EditAlbum && data.Album.SortById == window.Gsp.Constants.IntMinValue) {

					var onManualSort = function (e, ui) {
						ui.item.addClass('gsp_wait');

						// Get the items in their current sequence and pass that to the server's sort method.
						var gItems = ui.item.parent().children().map(function () {
							var $this = $(this);
							var id = $this.data("id");
							var itemType = $this.data("it");

							return { Id: id, ItemType: itemType };
						}).get();

						$.ajax(({
							type: "POST",
							url: window.Gsp.AppRoot + '/api/albums/sortgalleryobjects',
							data: JSON.stringify(gItems),
							contentType: "application/json; charset=utf-8",
							dataType: "json",
							success: function () {},
							error: function (response) {
								$.gspShowMsg("Cannot save", response.responseText, { msgType: 'error', autoCloseDelay: 0 });
							},
							complete: function () {
								ui.item.removeClass('gsp_wait');
							},
						}));
					};

					thmbs
						.css({ '-ms-touch-action': 'none', 'touch-action': 'none' }) // Required to allow dragging on touchscreens (-ms-touch-action applies to IE10 only)
						.parent()
						.sortable({
							start: function (e, ui) {
								ui.placeholder.width(ui.helper.width());
								ui.placeholder.height(ui.helper.height());
							},
							stop: onManualSort,
							scroll: true,
							containment: "document",
							cursor: "move",
							handle: ".hndl"
						});

					// Show/hide the drag bar when a user hovers over the thumbnail, but only for non-touchscreens.
					// We use stop/unselected events to handle show/hide for touchscreens.
					if (!window.Gsp.isTouchScreen()) {
						thmbs.hover(
							function () { $(this).prepend(hndleDom); },
							function () { $('.hndl', this).remove(); }
						);
					}
				}
			};

			var jsRenderSetup = function () {
				// Set up converters that can strip all HTML from some text and truncate. There is a related stripHtml converter in
				// window.Gsp.Init(). Note that the released version of jsRender may support chained converters, which would allow 
				// us to create one for stripping and one for truncating, then chain them instead of using stripHtmlAndTruncate.
				// See https://github.com/BorisMoore/jsrender/issues/127
				$.views.converters({
					stripHtmlAndTruncate: function (text) {
						var t = text.replace(/(<[^<>]*>)/g, "");
						var m = data.Settings.MaxThmbTitleDisplayLength;
						return (t.length > m ? Gsp.escape(t.substr(0, m)) + '...' : Gsp.escape(t));
					}
				});
			};

			if (!data.Album.GalleryItems) {
				$.gspShowMsg("Cannot Render Album", "<p>Cannot render the album thumbnails. Navigate to an album and then return to this page.</p><p>You'll know you got it right when you see 'aid' In the URL's query string.</p><p>ERROR: data.Album.GalleryItems is null.</p>", { msgType: 'error', autoCloseDelay: 0 });
				return;
			}

			jsRenderSetup(); // Prepare jsRender for rendering
			attachEvents();
			renderThmbView();
		};
		initialize();

		return this;
	};

	//#endregion gspThumbnails jQuery plug-in

	//#region gspMeta plug-in

	$.fn.gspMeta = function (data, options) {
		var self = this;
		return this.each(function () {
			if (!$.data(this, 'plugin_gspMeta')) {
				var gspMeta = GspMeta();
				gspMeta.init(self, data, options);
				$.data(this, 'plugin_gspMeta', gspMeta);
			}
		});
	};

	$.fn.gspMeta.defaults = {
		tmplName: ''
	};

	// Define object to handle metadata. Uses Revealing Module Pattern from here:
	// http://weblogs.asp.net/dwahlin/archive/2011/09/05/creating-multiple-javascript-objects-when-using-the-revealing-module-pattern.aspx
	window.GspMeta = function () {
		var $target,
				data,
				settings,
				init = function (target, gspData, options) {
					$target = target;
					data = gspData;
					settings = $.extend({}, $.fn.gspMeta.defaults, options);

					bindData();

					// Bind to next, previous, and mediaUpdated events from the GspMedia plug-in so that we can refresh the metadata.
					$('#' + data.Settings.MediaClientId).on('next.' + data.Settings.ClientId + ' previous.' + data.Settings.ClientId + ' mediaUpdate.' + data.Settings.ClientId, showMeta);

					// Bind to the select event from the gspThumbnails plug-in so we can refresh the metadata.
					$('#' + data.Settings.ThumbnailClientId).on('select.' + data.Settings.ClientId, showMeta);
				},
				bindData = function () {
					// Render the right pane template to the page DOM
					$target.removeClass('gsp_wait').html($.render[settings.tmplName](data));

					// Add separator row between the top and bottom sections of the metadata table
					$('.gsp_m1Row:last').after('<tr class="gsp_mSep"><td colspan="2"></td></tr>');

					var hasExistingMetaItems = data.ActiveMetaItems.length > 0;

					var hasEditPermission = function () {
						// If any of the selected items are a media object, user must have EditMediaObject perm.
						// If any of the selected items are an album, user must have EditAlbum perm.
						var hasAlbum, hasMediaItem;
						$.each(data.ActiveGalleryItems, function () {
							if (this.IsAlbum)
								hasAlbum = true;
							else
								hasMediaItem = true;
						});
						
						var canEdit = ((!hasAlbum || data.Album.Permissions.EditAlbum) && (!hasMediaItem || data.Album.Permissions.EditMediaObject));
						var isVirtualAlbum = data.Album.VirtualType != Gsp.Constants.VirtualType_NotVirtual;
						var isChildSelected = (data.ActiveGalleryItems.length >= 0 && data.ActiveGalleryItems[0].Id != data.Album.Id);
						
						if (!canEdit && isVirtualAlbum && isChildSelected && data.User.IsAuthenticated) {
							// Logged-on user is looking at a virtual album and has limited permissions. User may have edit permission to the 
							// particular items that are selected, so make a callback to see.
							$.ajax(({
								type: "POST",
								async: false,
								url: window.Gsp.AppRoot + '/api/galleryitemmeta/canuseredit',
								data: JSON.stringify(data.ActiveGalleryItems),
								contentType: "application/json; charset=utf-8",
								dataType: "json",
								success: function (result) {
									canEdit = result;
								},
								error: function (response) {
									$.gspShowMsg("Cannot Retrieve Data", response.responseText, { msgType: 'error', autoCloseDelay: 0 });
								}
							}));
						}

						return canEdit;
					};

					if (hasExistingMetaItems && hasEditPermission())
						makeMetaEditable();

					convertTagsToLinks();
					configureRating();
				},
				convertTagsToLinks = function () {
					// Look for the comma-separated tags (they have CSS class names gsp_mtag & gsp_mpeople) and convert them to hyperlinks.
					$.each(['tag', 'people'], function (i, tagType) {
						// Find the tag or people tag, but not if it's been previously made editable (we won't do anything in those cases)
						var tagContainer = $('.gsp_meta tr td.gsp_m' + tagType + ':not(.gsp_editableContent)', $target);

						// Build HTML links to represent the tags.
						var html = $.map(tagContainer.text().split(','), function (item) {
							var tag = item.trim();
							if (tag.length > 0) {
								var parms = { title: null, tag: null, people: null, search: null, latest: null, filter: null, rating: null, top: null, aid: null, moid: null };
								parms[tagType] = tag.replace(/\s+\(\d+\)$/gi, ''); // Strip off the trailing count (e.g ' (3)') if present

								return '<a href="' + Gsp.GetUrl(window.location.href, parms) + '" class="gsp_mtaglink">' + tag + '</a>';
							}
							return null;
						});

						tagContainer.text('').html(html); // Replace text with HTML links
					});
				},
				configureRating = function () {
					// Get rating element. Gets a match ONLY when the admin has configured the rating meta item as editable.
					var editableRatingEls = $('.gsp_meta tr[data-iseditable=true] td.gsp_v .gsp_rating', $target);

					if (editableRatingEls.length > 0 && (data.User.IsAuthenticated || data.Settings.AllowAnonymousRating)) {
						// Configure an editable rating
						editableRatingEls.rateit({
							min: 0,
							max: 5,
							resetable: false
						}).bind('rated', function (e, v) {
							$target.addClass('gsp_wait');

							var metaTypeId = getMetaItem($(e.target).closest('.gsp_m2Row').data('id')).MTypeId;
							var galleryItemMeta = { GalleryItems: data.ActiveGalleryItems, MetaItem: { MTypeId: metaTypeId, Value: v } };

							Gsp.DataService.saveMeta(galleryItemMeta,
								function () { $target.removeClass('gsp_wait'); },
								function (o) {
								},
								function (response) { $.gspShowMsg("Cannot Save Changes", response.responseText, { msgType: 'error', autoCloseDelay: 0 }); }
							);
						});
					} else {
						// Configure a read-only rating
						$('.gsp_rating', $target).rateit({
							min: 0,
							max: 5,
							resetable: false,
							readonly: true
						});
					}
				},
				makeMetaEditable = function () {
					// Set up the default options for the Jeditable plug-in
					var editableOptions = {
						type: 'text',
						rows: 1,
						width: 'auto',
						widthBuffer: 6,
						tooltip: data.Resource.MediaCaptionEditTt,
						event: 'click', //dblclick
						cssclass: 'gsp_editableContentForm',
						indicator: '<img class="gsp_wait_img" src="' + data.App.SkinPath + '/images/wait-squares.gif" />',
						style: "display:block",
						placeholder: data.Resource.MetaEditPlaceholder,
						submitonenter: true,
						onblur: 'submit',
						//onblur: '', // Use this instead of previous line to prevent auto-cancel when blurring away from input
						data: function (curValue, obj) { return onGetData(curValue, obj, $(this)); },
						oneditbegin: function (s, el, e) { return onEditBegin($(el), e); },
						oneditend: function () { onEditEnd($(this)); },
						onreset: function (frm, obj) { onReset($(obj)); },
						onsubmit: onsubmit,
						oncomplete: function (xmldata) { return onComplete($(this), xmldata); },
						//onerror: function (s, el, r) { }, // Don't use because the item doesn't reset itself properly
						callback: function (value, s) { setHover($(this), true); },
						ajaxoptions: {
							type: 'PUT',
							contentType: 'application/json;charset=utf-8'
						}
					};

					// Copy the default options and change those necessary for the caption metadata item.
					var editableOptionsCaption = Gsp.deepCopy(editableOptions);
					editableOptionsCaption.type = 'textarea';
					editableOptionsCaption.rows = '3';
					editableOptionsCaption.submitonenter = false;

					// Make another copy for use in metadata that use the autoSuggest plug-in (e.g. tags)
					var editableOptionsTag = Gsp.deepCopy(editableOptions);
					editableOptionsTag.onblur = ''; // Do nothing when blurring away from input tag

					// Get a reference to all metadata items and set the hover functionality
					var els = $('.gsp_meta tr[data-iseditable=true] td.gsp_v', $target).addClass('gsp_editableContent');

					// Get reference to the subset of elements that are the caption and tag
					var elsCaption = els.filter(".gsp_mCaption");
					var elsTag = els.filter(".gsp_mtag,.gsp_mpeople");
					var elsRating = els.filter(".gsp_mrating");
					var elsRemaining = els.not(elsCaption).not(elsTag).not(elsRating); // All elements that aren't the caption, tag-based item, or rating 

					// Make metadata editable, using the appropriate set of options
					elsCaption.editable(window.Gsp.AppRoot + '/api/galleryitemmeta', editableOptionsCaption);
					//elsTag.editable(window.Gsp.AppRoot + '/api/meta', editableOptionsTag);
					elsRemaining.editable(window.Gsp.AppRoot + '/api/galleryitemmeta', editableOptions); // Matches all elements not in elsCaption and elsTag

					// Set up the hover functionality
					setHover(elsCaption, true);
					setHover(elsRemaining, true);

					var getTagValue = function (e) {
						// Get the tag value, stripping off trailing count if present. Ex: If tag="Animal (3)", change it to "Animal".
						// The parameter 'e' is expected to be a jQuery reference to the li element containing the tag.
						return e.contents().filter(function () {
							return this.nodeType == 3;
						}).text().trim().replace(/\s+\(\d+\)$/gi, '');
					};

					elsTag.each(function () {
						$(this).html("<input class='gsp_mTag_ipt' value='" + Gsp.escape($(this).text()) + "' placeholder='" + data.Resource.MetaEditPlaceholder + "' />");

						var initComplete = false;
						var ipt = $('input', $(this));
						var tagType = ipt.closest('.gsp_mtag').length > 0 ? 'tags' : 'people';

						ipt.autoSuggest(window.Gsp.AppRoot + '/api/meta/' + tagType, {
							extraParams: '&galleryId=' + data.Settings.GalleryId,
							preFill: ipt.val(),
							startText: data.Resource.MetaEditPlaceholder,
							selectionClick: function (e) {
								// User clicked the tag. Navigate to a page showing all objects with that tag.
								var tagValue = e.parents('.gsp_mtag').length == 0 ? null : getTagValue(e);
								var peopleValue = e.parents('.gsp_mpeople').length == 0 ? null : getTagValue(e);
								window.location = Gsp.GetUrl(window.location.href, { title: null, tag: tagValue, people: peopleValue, search: null, latest: null, filter: null, rating: null, top: null, aid: null, moid: null });
							},
							selectionAdded: function (e) {
								if (initComplete) {
									e.addClass('gsp_wait_spinner');
									var newTag = e.contents().filter(function () {
										return this.nodeType == 3;
									}).text().trim();

									var metaTypeId = getMetaItem(e.closest('.gsp_mRowDtl').data('id')).MTypeId;
									var galleryItemMeta = { GalleryItems: data.ActiveGalleryItems, MetaItem: { MTypeId: metaTypeId, Value: newTag } };

									Gsp.DataService.saveMeta(galleryItemMeta,
										function () { e.removeClass('gsp_wait_spinner'); },
										function (gim) {
											if (data.MediaItem != null) {
												// When showing a single media object, update the data object's meta value so it is available during next/previous browsing
												var mi = Gsp.findMetaItem(data.MediaItem.MetaItems, gim.MetaItem.MTypeId);
												if (mi != null) mi.Value += ', ' + gim.MetaItem.Value;
											}

											if (data.ActiveGalleryItems.length > 1) {
												// Append the count to the end of the tag value showing how many of the selected items have that tag (e.g. "Vacation (12)")
												var textNode = e.contents().filter(function () { return this.nodeType == 3; })[0];
												textNode.textContent = textNode.textContent + ' (' + data.ActiveGalleryItems.length + ')';
											}

										},
										function (response) { $.gspShowMsg("Cannot Save Changes", response.responseText, { msgType: 'error', autoCloseDelay: 0 }); }
									);
								}
							},
							selectionRemoved: function (e) {
								e.animate({ opacity: .2 }, "slow", function () { e.addClass('gsp_wait_spinner'); });

								// Get the tag value, stripping off trailing count if present. Ex: If tag="Animal (3)", change it to "Animal".
								var newTag = getTagValue(e);

								var metaTypeId = getMetaItem(e.closest('.gsp_mRowDtl').data('id')).MTypeId;

								$.ajax(({
									type: "DELETE",
									url: window.Gsp.AppRoot + '/api/galleryitemmeta',
									data: JSON.stringify({ GalleryItems: data.ActiveGalleryItems, MetaItem: { MTypeId: metaTypeId, Value: newTag } }),
									contentType: "application/json; charset=utf-8",
									complete: function () {
									},
									success: function (o) {
										e.remove();

										if (data.MediaItem != null) {
											// When showing a single media object, update the data object's meta value so it is available during next/previous browsing
											var mi = Gsp.findMetaItem(data.MediaItem.MetaItems, metaTypeId);

											if (mi != null) {
												// Remove the tag from the comma separated list of tags
												mi.Value = $.grep(mi.Value.split(/\s*,\s*/), function (tag, i) {
													return tag != newTag;
												}).join(', ');
											}
										}
									},
									error: function (response) {
										$.gspShowMsg("Cannot Save Changes", response.responseText, { msgType: 'error', autoCloseDelay: 0 });
									}
								}));
							}
						});
						initComplete = true;
					});
				},
				getMetaItem = function (id) {
					// Find the meta item with the specified ID.
					return $.grep(data.ActiveMetaItems, function (mi) { return mi.Id === id; })[0];
				},
				onsubmit = function (settingsObj, el) {
					// Get the meta item from the instance and update the value with the data the user entered.
					var $el = $(el);
					var md = getMetaItem($el.parent().data('id'));

					// Get the user-entered value and assign the data to be sent to the server. It will later by assigned to
					// our local data entities in the onComplete event.
					var mValue = $('textarea:last, input:last, select:last', $el).val().trim();

					settingsObj.ajaxoptions.data = JSON.stringify({ GalleryItems: data.ActiveGalleryItems, MetaItem: { MTypeId: md.MTypeId, Value: mValue } }); // '{"Id":1,"TypeId":1,"Desc":"My desc","Value":"My value"}';
				},
				showMeta = function (e, gItems) {
					// gItems is an array of GalleryItem objects. It should be the same reference as data.ActiveGalleryItems
					$target.addClass('gsp_wait'); // Show wait animated gif

					// Are we showing the meta for the current media item?
					var showMetaForMediaItem = gItems.length == 1 && gItems[0].ItemType != Gsp.Constants.ItemType_Album && data.MediaItem && data.MediaItem.Id == gItems[0].Id;

					if (showMetaForMediaItem && data.MediaItem.MetaItems) {
						// We already have the meta items on the client, so grab them, bind and return (no need to get them from server).
						data.ActiveMetaItems = data.MediaItem.MetaItems;
						bindData();
						return;
					}

					if (gItems.length == 1) {
						// A single gallery item is selected.
						var i = gItems[0];

						var gt = (i.ItemType == Gsp.Constants.ItemType_Album ? 'albums' : 'mediaitems');
						var url = window.Gsp.AppRoot + '/api/' + gt + '/' + i.Id + '/meta';

						$.ajax({
							url: url,
							dataType: 'json',
							success: function (metaItems) {
								data.ActiveMetaItems = metaItems;
								if (data.MediaItem != null)
									data.MediaItem.MetaItems = metaItems;
								bindData();
							},
							statusCode: {
								404: function () {
									// When we get a 404, just bind to the current album's metadata. We'll get here when the user is viewing a 
									// virtual album and clicks in the album area. Since the ID is int.MinValue, the server will send a 404.
									data.ActiveMetaItems = data.Album.MetaItems;
									bindData();
								}
							}
						});
					} else if (gItems.length < 1) {
						// No gallery items have been passed. It is not expected that we'll get here, but just in case, clear out the active
						// meta items and re-bind.
						data.ActiveMetaItems = [];
						bindData();
					} else {
						// More than one gallery item has been passed. Send the items to the server so we can get a merged list of meta.
						$.ajax(({
							type: "POST",
							url: window.Gsp.AppRoot + '/api/galleryitemmeta/galleryitems',
							data: JSON.stringify(gItems),
							contentType: "application/json; charset=utf-8",
							dataType: "json",
							success: function (metaItems) {
								data.ActiveMetaItems = metaItems;
								if (data.MediaItem != null)
									data.MediaItem.MetaItems = metaItems;
								bindData();
							},
							error: function (response) {
								$.gspShowMsg("Cannot Retrieve Data", response.responseText, { msgType: 'error', autoCloseDelay: 0 });
							}
						}));
					}
				},
				onEditBegin = function (el, e) {
					//var pr = $(el).prev();
					// Save current padding on Desc td cell and then give it a top padding of .7em. We'll restore it when the edit is complete.
					// Without this step the desc is too high in relation to the input to its right.
					//pr.data('padding-top', pr.css('padding-top')).css('padding-top', '0.7em');
					setHover(el, false);

					// Return false when a hyperlink is clicked. Calling code will cancel the edit and allow the navigation to the link.
					return (e.target.tagName != 'A');
				},
				onGetData = function (curValue, obj, $el) {
					// Get the metadata value we want to use to populate the form element when editing. We get the metadata value from the 
					// object rather than the HTML DOM value since the DOM element might be HTML encoded; although we do use it as a last
					// resort if we can't find the data object (this should not typically happen, though).
					var md = getMetaItem($el.parent().data('id'));
					if (md && md.Value)
						return md.Value;
					else
						return curValue;
				},
				onEditEnd = function (el) {
				},
				onReset = function (el) {
					reset(el);
				},
				onComplete = function (el, xmldata) {
					// Reset control and return the new value. The calling function will assign the value to the original HTML element.
					var syncGalleryDataOnMetaUpdate = function () {
						// Update related properties on albums, media objects, and their metadata when meta items are changed.
						// 29 = MetadataItemName.Title; 41 = Caption; 112 = HtmlSource
						for (var i = 0; i < gim.GalleryItems.length; i++) {
							// Update GalleryItem, MediaItem, Album, and MetaItems
							var gNew = gim.GalleryItems[i];
							var metaItems;

							// Update gallery item if present in our data
							var gCurrent = Gsp.findGalleryItem(data, gNew.Id, gNew.ItemType);
							if (gCurrent != null) {
								switch (gim.MetaItem.MTypeId) {
									case 29:
										gCurrent.Title = gim.MetaItem.Value;
										break;
									case 41:
										gCurrent.Caption = gim.MetaItem.Value;
										break;
								}
							}

							// Update media object if present in our data
							var mCurrent = Gsp.findMediaItem(data, gNew.Id, gNew.ItemType);
							if (mCurrent != null) {
								switch (gim.MetaItem.MTypeId) {
									case 29:
										mCurrent.Title = gim.MetaItem.Value;
										break;
									case 112:
										var view = Gsp.getView(mCurrent, window.Gsp.Constants.ViewSize_External);
										if (view)
											view.HtmlOutput = gim.MetaItem.Value;
										break;
								}
								metaItems = mCurrent.MetaItems;
							}

							// Update album if present in our data
							var aCurrent = Gsp.findAlbum(data, gNew.Id, gNew.ItemType);
							if (aCurrent != null) {
								switch (gim.MetaItem.MTypeId) {
									case 29:
										aCurrent.Title = gim.MetaItem.Value;
										break;
									case 41:
										aCurrent.Caption = gim.MetaItem.Value;
										break;
								}
								metaItems = aCurrent.MetaItems;
							}

							// Update meta item if present in our data
							if (metaItems != null) {
								var mi = Gsp.findMetaItem(metaItems, gim.MetaItem.MTypeId);
								if (mi != null) mi.Value = gim.MetaItem.Value;
							}
						}
					};

					reset(el);

					var m = getMetaItem($(el).parent().data('id')); // Get reference to metadata item
					var gim = $.parseJSON(xmldata); // Hydrate GalleryItemMeta instance returned from server

					if (gim.ActionResult && gim.ActionResult.Status == "Error") {
						$.gspShowMsg(gim.ActionResult.Title, gim.ActionResult.Message, { msgType: 'error', autoCloseDelay: 0 });
						return m.Value;
					}

					m.Value = gim.MetaItem.Value; // Update value (which may have been changed by the server (e.g. HTML removed))

					syncGalleryDataOnMetaUpdate();

					$('#' + data.Settings.MediaClientId).trigger('metaUpdate.' + data.Settings.ClientId, [gim]);
					$('#' + data.Settings.ThumbnailClientId).trigger('metaUpdate.' + data.Settings.ClientId, [gim]);

					// For the HTML content metadata item, we need to encode the return value so that the HTML is visible in the browser.
					if (m.MTypeId == 112) // MetadataItemName.HtmlSource (used only in external media objects)
						return window.Gsp.htmlEncode(m.Value);
					else
						return m.Value;
				},
				reset = function (el) {
					setHover(el, true);
				},
				setHover = function (els, enable) {
					$.each(els, function (i, el) {
						var $el = $(el);
						if (enable) {
							$el.hover(function () {
								$el.removeClass('gsp_editableContent').addClass('gsp_editableContentHover');
							}, function () {
								$el.removeClass('gsp_editableContentHover').addClass('gsp_editableContent');
							});
						} else {
							$el.removeClass('gsp_editableContentHover').addClass('gsp_editableContent').unbind('mouseenter').unbind('mouseleave');
						}
					});
				};

		return {
			init: init
		};
	};

	//#endregion

	//#region gspHeader plug-in

	$.fn.gspHeader = function (tmplName, data) {
		var self = this;
		var $target = this;

		var initialize = function () {
			var renderHeader = function () {
				self.html($.render[tmplName](data)); // Render HTML template and add to page
			};

			var configLogin = function () {
				var dgLogin;
				if (data.User.IsAuthenticated) {
					$('.gsp_logoffLink', $target).click(function (e) {
						e.preventDefault();
						e.stopPropagation();
						Gsp.DataService.logOff(function () { Gsp.ReloadPage(); });
					});
				}
				else {
					dgLogin = $('#' + data.Settings.ClientId + '_loginDlg');
					dgLogin.dgLoginWidth = 420;

					dgLogin.dialog({
						appendTo: '#' + data.Settings.ClientId,
						autoOpen: false,
						draggable: false,
						resizable: false,
						closeOnEscape: true,
						dialogClass: 'gsp_loginDlgContainer',
						width: dgLogin.dgLoginWidth,
						minHeight: 0,
						show: 'fade',
						hide: 'fade',
						open: function () {
							setTimeout(function () { $('.gsp_login_textbox:first', dgLogin).focus(); }, 50); // Delay needed for IE
						}
					});

					var disableCreateUserValidation = function (disabled) {
						// When true, the required attribute of form elements in the create user control are disabled. This allows the user to log in 
						// on the create user page.
						$('.gsp_createuser input[data-required=true]').prop('required', !disabled);
					};

					$('.gsp_login_trigger', $target).click(function (e) {
						if (dgLogin.dialog('isOpen') === true) {
							dgLogin.dialog('close');
							disableCreateUserValidation(false); // Restore required attribute
						}
						else {
							dgLogin.dialog('option', 'hide', null).dialog('close').dialog('option', 'hide', 'fade'); // Kill, then restore fade for quicker closing
							dgLogin.dialog('option', 'position', [$(window).width() - dgLogin.dgLoginWidth - 20, e.pageY + 30]);
							dgLogin.dialog('open');

							disableCreateUserValidation(true);
						}
						return false;
					});

					$('.gsp_login_textbox', dgLogin).on('keydown', function (e) {
						if (e.keyCode == 13) { // Enter
							$('.gsp_login_button', dgLogin).click();
							return false;
						} else {
							return true;
						}
					});

					$('.gsp_login_button', dgLogin).button();

					// Close dialog when user clicks outside the login window
					$('body').bind('click', function (e) {
						if (dgLogin.dialog('isOpen') === true && !$(e.target).is('.ui-dialog, a') && !$(e.target).closest('.ui-dialog').length) {
							dgLogin.dialog('close');
							disableCreateUserValidation(false); // Restore required attribute
						}
					});
				}
			};

			var configSearch = function () {
				var dgSearch;
				if (data.Settings.ShowSearch) {
					dgSearch = $('#' + data.Settings.ClientId + '_searchDlg');
					dgSearch.dgSearchWidth = 420;

					dgSearch.dialog({
						appendTo: '#' + data.Settings.ClientId,
						autoOpen: false,
						draggable: false,
						resizable: false,
						closeOnEscape: true,
						dialogClass: 'gsp_searchDlgContainer',
						width: dgSearch.dgSearchWidth,
						minHeight: 0,
						show: 'fade',
						hide: 'fade',
						open: function (t, d) {
							setTimeout(function () { $('.gsp_searchbox', dgSearch).focus(); }, 50); // Delay needed for IE
						}
					});

					$('.gsp_search_trigger', $target).click(function (e) {
						if (dgSearch.dialog('isOpen') === true)
							dgSearch.dialog('close');
						else {
							dgSearch.dialog('option', 'hide', null).dialog('close').dialog('option', 'hide', 'fade'); // Kill, then restore fade for quicker closing
							dgSearch.dialog('option', 'position', [$(window).width() - dgSearch.dgSearchWidth - 20, e.pageY + 30]);
							dgSearch.dialog('open');
						}
						return false;
					});

					// Start search when search button is clicked
					$('.gsp_searchbutton', dgSearch).on('click', function (e) {
						var prepSearchTerms = function (st) {
							// Replace any spaces outside of quotes with +
							var result = '';
							var inQuote;
							$.each(st.split(''), function (idx, v) {
								if (v == '\"' || v == '\'')
									inQuote = !inQuote;

								result += (!inQuote && v == ' ' ? '+' : v);
							});
							return result;
						};

						e.preventDefault(); e.stopPropagation();

						var minSearchLen = 3;
						var searchTerm = $('.gsp_searchbox', dgSearch).val();
						if (searchTerm.length >= minSearchLen) {
							var sType = $('[name=' + data.Settings.ClientId + '_searchType]:checked').val();

							var parms = { title: null, tag: null, people: null, search: null, latest: null, filter: null, rating: null, top: null, aid: null, moid: null };
							parms[sType] = prepSearchTerms(searchTerm);
							window.location = Gsp.GetUrl(window.location.href, parms);
						}
						else {
							var $msgEl = $('.gsp_search_msg', dgSearch);
							$msgEl.css('visibility', 'visible');
							$('.gsp_searchbox', dgSearch).one('keydown', function () { $msgEl.css('visibility', 'hidden'); }).focus();
						}
					}).button();

					$('.gsp_searchbox, .gsp_search_type_container input', dgSearch).on('keydown', function (e) {
						if (e.keyCode == 13) { // Enter
							$('.gsp_searchbutton', dgSearch).click();
							return false;
						} else
							return true;
					});

					// Close dialog when user clicks outside the search window
					$('body').bind('click', function (e) {
						if (dgSearch.dialog('isOpen') === true && !$(e.target).is('.ui-dialog, a') && !$(e.target).closest('.ui-dialog').length) {
							dgSearch.dialog('close');
						}
					});
				}
			};

			renderHeader();
			configLogin();
			configSearch();
		};

		initialize();

		$(document.documentElement).trigger('gspHeaderLoaded.' + data.Settings.ClientId);

		return this;
	};

	//#endregion

	//#region gspTooltip plug-in

	$.fn.gspTooltip = function (options) {
		var self = this;
		return this.each(function () {
			if (!$.data(this, 'plugin_gspTooltip')) {
				var tt = GspTooltip();
				tt.init(self, options);
				$.data(this, 'plugin_gspTooltip', tt);
			}
		});
	};

	$.fn.gspTooltip.defaults = {
		title: '',
		content: ''
	};

	// Define object to handle tooltip. Uses Revealing Module Pattern from here:
	// http://weblogs.asp.net/dwahlin/archive/2011/09/05/creating-multiple-javascript-objects-when-using-the-revealing-module-pattern.aspx
	window.GspTooltip = function () {
		var $target,
				settings,
				$dgTrigger,
				$dgTooltip,
				init = function (target, options) {
					$target = target;
					settings = $.extend({}, $.fn.gspTooltip.defaults, options);

					initVars();
					configureDialog();
					configureTooltip();
				},

				initVars = function () {
					$dgTrigger = $("<button class='gsp_tt_tgr'></button>");
					$dgTooltip = $("<div class='gsp_tt_dlg'><div class='gsp_tt_dlg_title'>{0}</div><div class='gsp_tt_dlg_bdy'>{1}</div></div>".format(settings.title, settings.content));
				},

				configureDialog = function () {

					// Configure the tooltip dialog
					$dgTooltip.dialog({
						appendTo: $('.gsp_ns').first(),
						autoOpen: false,
						draggable: false,
						resizable: false,
						closeOnEscape: true,
						dialogClass: 'gsp_tt_dlg_container',
						width: 420,
						minHeight: 0,
						show: 'fade',
						hide: 'fade',
						position: { my: "left top", at: "left bottom", of: $dgTrigger },
						open: function (e, ui) {
							$(document).on("click", function (e1) {
								if ($(e1.target).parents('.gsp_tt_dlg_container').length == 0) {
									$dgTooltip.dialog('close');
									$(this).unbind(e1);
								}
							});
						}
					});
				},

				configureTooltip = function () {
					$dgTrigger.insertAfter($target)
						.button({
							text: false,
							icons: { primary: "gsp-ui-icon gsp-ui-icon-help" }
						})
						.click(function (e) {
							if ($dgTooltip.dialog('isOpen') === true)
								$dgTooltip.dialog('close');
							else {
								$dgTooltip.dialog('open');
							}
							return false;
						});
				};

		return {
			init: init
		};
	};

	//#endregion

	//#region FullScreenSlideShow class

	window.GspFullScreenSlideShow = function (data, options) {
		var defaults = {
			on_exit: function () { }
		};

		this.data = data;
		this.settings = $.extend({}, defaults, options);
	};

	GspFullScreenSlideShow.prototype.startSlideShow = function () {
		var self = this;
		var items = this.data.Album.MediaItems || this.data.Album.GalleryItems;

		var urls = $.map(items, function (mo) {
			if (mo.ItemType == Gsp.Constants.ItemType_Image)
				return { id: mo.Id, thumb: Gsp.getView(mo, Gsp.Constants.ViewSize_Thumbnail).Url, title: mo.Title, image: Gsp.getView(mo, Gsp.Constants.ViewSize_Optimized).Url };
			else
				return null;
		});

		if (urls.length == 0) {
			$.gspShowMsg(this.data.Resource.MoNoSsHdr, this.data.Resource.MoNoSsBdy, { msgType: 'info' });
			return false;
		};

		var ssTmpl = '\
<div class="ssControlsContainer"> \
		<!--Thumbnail Navigation--> \
		<div id="prevthumb"></div> \
		<div id="nextthumb"></div> \
\
		<!--Arrow Navigation--> \
		<a id="prevslide" class="load-item"></a> \
		<a id="nextslide" class="load-item"></a> \
\
		<div id="thumb-tray" class="load-item"> \
			<div id="thumb-back"></div> \
			<div id="thumb-forward"></div> \
		</div> \
\
		<!--Time Bar--> \
		<div id="progress-back" class="load-item"> \
			<div id="progress-bar"></div> \
		</div> \
\
		<!--Control Bar--> \
		<div id="controls-wrapper" class="load-item"> \
			<div id="controls"> \
\
				<a id="play-button"> \
					<img id="pauseplay" src="{0}/pause.png" /></a> \
\
				<a id="stop-button"> \
					<img src="{0}/stop.png" /></a> \
\
				<!--Slide counter--> \
				<div id="slidecounter"> \
					<span class="slidenumber"></span> / <span class="totalslides"></span> \
				</div> \
\
				<!--Slide captions displayed here--> \
				<div id="slidecaption"></div> \
\
				<!--Thumb Tray button--> \
				<a id="tray-button"> \
					<img id="tray-arrow" src="{0}/button-tray-up.png" /></a> \
\
				<!--Navigation--> \
				<ul id="slide-list"></ul> \
\
			</div> \
		</div> \
</div> \
				'.format(this.data.App.SkinPath + '/images/supersized');

		var getTransition = function (transitionType) {
			switch (transitionType) {
				case 'fade': return 1;
				case 'slide': return 3;
				default: return 0;
			}
		};

		var getStartSlide = function () {
			// Get the current media object and find the index of the matching one in the urls var.
			var startSlide = 1;

			if (self.data.MediaItem == null)
				return startSlide;

			$.each(urls, function (idx, ssItem) {
				if (self.data.MediaItem.Id == ssItem.id) {
					startSlide = idx + 1;
					return false; // false breaks out of $.each
				}
				return true;
			});

			return startSlide;
		};

		// Fire up the full screen slide show.
		$.supersized({

			// Functionality
			image_path: this.data.App.SkinPath + '/images/supersized/',
			slideshow: 1,			// Slideshow on/off
			autoplay: 1,			// Slideshow starts playing automatically
			auto_exit: 1,      // Exit the slideshow when the last slide is finished
			start_slide: getStartSlide(),			// Start slide (0 is random)
			loop: 0,			// Enables moving between the last and first slide.
			random: 0,			// Randomize slide order (Ignores start slide)
			slide_interval: this.data.Settings.SlideShowIntervalMs,		// Length between transitions
			transition: getTransition(this.data.Settings.TransitionType), 			// 0-None, 1-Fade, 2-Slide Top, 3-Slide Right, 4-Slide Bottom, 5-Slide Left, 6-Carousel Right, 7-Carousel Left
			transition_speed: 500,		// Speed of transition
			new_window: 1,			// Image links open in new window/tab
			pause_hover: 0,			// Pause slideshow on hover
			keyboard_nav: 1,			// Keyboard navigation on/off
			performance: 1,			// 0-Normal, 1-Hybrid speed/quality, 2-Optimizes image quality, 3-Optimizes transition speed // (Only works for Firefox/IE, not Webkit)
			image_protect: 0,			// Disables image dragging and right click with Javascript

			// Size & Position						   
			min_width: 0,			// Min width allowed (in pixels)
			min_height: 0,			// Min height allowed (in pixels)
			vertical_center: 1,			// Vertically center background
			horizontal_center: 1,			// Horizontally center background
			fit_always: 1,			// Image will never exceed browser width or height (Ignores min. dimensions)
			fit_portrait: 1,			// Portrait images will not exceed browser height
			fit_landscape: 1,			// Landscape images will not exceed browser width

			// Components							
			slide_links: 'blank',	// Individual links for each slide (Options: false, 'num', 'name', 'blank')
			thumb_links: 1,			// Individual thumb links for each slide
			thumbnail_navigation: 0,			// Thumbnail navigation
			slides: urls,

			// Theme Options			   
			progress_bar: 0,			// Timer for each slide							
			mouse_scrub: 0,
			html_template: ssTmpl, // The HTML for the controls
			on_destroy: function (currentId) {
				self.settings.on_exit.apply(null, [currentId]);
			}
		});

		// Exit slideshow when stop button is clicked.
		$('#stop-button').on('click', function () {
			api.destroy();
		});

		return true;
	};

	//#endregion

	//#region Timer class
	window.GspTimer = function (callback, milliseconds, context) {
		this.isRunning = false;
		this.milliseconds = milliseconds;
		this.callback = callback;
		this.context = context;
		if (!this.context) this.context = this;
		this.handle = null;
	};

	GspTimer.prototype.start = function () {
		var timer = this;
		var context = this.context;
		var invokeCallback = function () {
			timer.callback.apply(context);
		};
		this.handle = setInterval(invokeCallback, this.milliseconds);
		this.isRunning = true;
	};

	GspTimer.prototype.stop = function () {
		clearInterval(this.handle);
		this.isRunning = false;
	};
	//#endregion End Timer class

	//#region gspShowMsg utility function

	$.gspShowMsg = function (title, message, options) {

		var defaults = {
			msgType: 'success', // Any property of the MessageStyle enumeration: success, info, warning, error
			autoCloseDelay: 4000, // The # of milliseconds to wait until a message auto-closes. Use 0 to never auto-close.
			width: 500 // The width of the dialog window.
		};

		var settings = $.extend({}, defaults, options);

		$('.gsp_msg').remove(); // Remove any previous message that may be visible

		var $dgHtml = $('<div>');

		var cssClass = "gsp_msg";
		if (message) {
			$dgHtml.append(message);
			cssClass += " gsp_msgHasContent";
		} else {
			cssClass += " gsp_msgNoContent";
		}

		cssClass += " gsp_msg_" + settings.msgType;

		$dgHtml.dialog({
			appendTo: $('.gsp_ns').first(),
			position: { my: 'top', at: 'top' },
			title: title,
			width: settings.width,
			height: 'auto',
			resizable: false,
			dialogClass: cssClass,
			show: 'fade',
			hide: 'fade'
		});

		if (settings.autoCloseDelay > 0) {
			// Auto-close for success messages
			setTimeout(function () {
				if ($dgHtml.is(":ui-dialog")) {
					$dgHtml.dialog('destroy');
				}
			}, settings.autoCloseDelay);
		}
	};

	//#endregion

	//#region ServerTask

	window.Gsp.ServerTask = function (options) {
		/// <summary>
		///   An object for executing long running tasks on the server and periodically checking its status. The following
		///   assumptions are made: (1) The URL used to invoke the task invokes it on a background thread and returns quickly.
		///   (2) A callback function userDefinedProgressCallback is specified that monitors the returned progress data. This
		///   function is responsible for detecting when the task is complete and subsequently calling resetTask, which cancels the
		///   polling mechanism.
		/// </summary>
		/// <param name="options">The options that contain configuration data.</param>
		var createTaskId = function () {
			/// <summary>Internal function to generate a unique task ID.</summary>
			/// <returns type="String">Returns a pseudo-GUID.</returns>
			return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
				var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
				return v.toString(16);
			});
		};

		var defaults = {
			taskId: createTaskId(), // The current task ID
			timerId: 0, // The timer ID used to identify the polling mechanism
			taskBeginData: null, // An object to include when starting the task. It is serialized as JSON and sent with the taskBeginUrl.
			taskBeginUrl: null, // The URL to use to initiate the long running task. It is expected that the task will be started on a background thread and that this URL returns quickly.
			taskProgressUrl: null, // The URL to invoke to read status
			taskAbortUrl: null, // The URL to invoke to abort the operation 
			interval: 1000, // The current interval for progress refresh (in ms)
			userDefinedProgressCallback: null, // The user-defined callback that refreshes the UI 
			taskAbortedCallback: null // Get the user-defined callback that runs after aborting the call 
		};

		var taskSettings = $.extend({}, defaults, options);
		var self = this;

		var abortTask = function () {
			/// <summary>
			///   Send a signal to the server to stop the action. When the polling mechanism detects that the server task has
			///   been cancelled, it will resets the task (which cancels the timer).
			/// </summary>
			if (taskSettings.taskAbortUrl != null && taskSettings.taskAbortUrl != '') {
				$.ajax({
					url: taskSettings.taskAbortUrl,
					async: false,
					cache: false,
					headers: { 'X-ServerTask-TaskId': taskSettings.taskId }
				});
			}
		};

		var internalProgressCallback = function () {
			/// <summary>An internal function for invoking the progress URL on the server.</summary>
			$.ajax({
				url: taskSettings.taskProgressUrl,
				cache: false,
				headers: { 'X-ServerTask-TaskId': taskSettings.taskId },
				success: function (status) {
					// Set the timer to call this method again after the specified interval.
					taskSettings.timerId = window.setTimeout(internalProgressCallback, taskSettings.interval);

					if (taskSettings.userDefinedProgressCallback != null)
						taskSettings.userDefinedProgressCallback(status, self);
				}
			});
		};

		var startTask = function () {
			/// <summary>Invoke the long tunning task and begin the periodic polling to check its status.</summary>
			_xhr = $.ajax({
				url: taskSettings.taskBeginUrl,
				type: "POST",
				data: JSON.stringify(taskSettings.taskBeginData),
				contentType: "application/json; charset=utf-8",
				cache: false,
				headers: { 'X-ServerTask-TaskId': taskSettings.taskId },
				complete: function () {
					if (_xhr.status != 0) return;
					if (taskSettings.taskAbortedCallback != null)
						taskSettings.taskAbortedCallback(self);
					end();
				},
				success: function (data) {
					// Start the progress callback (if any)
					if (taskSettings.userDefinedProgressCallback != null && taskSettings.taskProgressUrl != null) {
						taskSettings.timerId = window.setTimeout(internalProgressCallback, taskSettings.interval);
					}
				},
				error: function (response) {
					$.gspShowMsg("Error starting task", response.responseText, { msgType: 'error', autoCloseDelay: 0 });
				}
			});
		};

		var resetTask = function () {
			/// <summary>
			///   Clears the existing timer function and resets the internal state of the object. Note that
			///   this function does not send an abort signal to the server.
			/// </summary>
			taskSettings.taskId = 0;
			window.clearTimeout(taskSettings.timerId);
		};

		this.startTask = startTask;
		this.abortTask = abortTask;
		this.resetTask = resetTask;
		this.taskSettings = taskSettings;
	};

	//#endregion

})(jQuery, window, document);

//#endregion