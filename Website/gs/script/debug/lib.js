/* TO MODIFY: Make changes to this file and test locally under the Debug compilation configuration. When 
finished, run this text through a javascript minifier and copy the output to lib.min.js. 
There is an online minifier at http://www.refresh-sf.com/yui/. */

//#region javascript libraries and jQuery plug-ins */

//#region json2.js

/*
http://www.JSON.org/json2.js
2011-02-23

Public Domain.

NO WARRANTY EXPRESSED OR IMPLIED. USE AT YOUR OWN RISK.

See http://www.JSON.org/js.html


This code should be minified before deployment.
See http://javascript.crockford.com/jsmin.html

USE YOUR OWN COPY. IT IS EXTREMELY UNWISE TO LOAD CODE FROM SERVERS YOU DO
NOT CONTROL.


This file creates a global JSON object containing two methods: stringify
and parse.

JSON.stringify(value, replacer, space)
value       any JavaScript value, usually an object or array.

replacer    an optional parameter that determines how object
values are stringified for objects. It can be a
function or an array of strings.

space       an optional parameter that specifies the indentation
of nested structures. If it is omitted, the text will
be packed without extra whitespace. If it is a number,
it will specify the number of spaces to indent at each
level. If it is a string (such as '\t' or '&nbsp;'),
it contains the characters used to indent at each level.

This method produces a JSON text from a JavaScript value.

When an object value is found, if the object contains a toJSON
method, its toJSON method will be called and the result will be
stringified. A toJSON method does not serialize: it returns the
value represented by the name/value pair that should be serialized,
or undefined if nothing should be serialized. The toJSON method
will be passed the key associated with the value, and this will be
bound to the value

For example, this would serialize Dates as ISO strings.

Date.prototype.toJSON = function (key) {
function f(n) {
// Format integers to have at least two digits.
return n < 10 ? '0' + n : n;
}

return this.getUTCFullYear()   + '-' +
f(this.getUTCMonth() + 1) + '-' +
f(this.getUTCDate())      + 'T' +
f(this.getUTCHours())     + ':' +
f(this.getUTCMinutes())   + ':' +
f(this.getUTCSeconds())   + 'Z';
};

You can provide an optional replacer method. It will be passed the
key and value of each member, with this bound to the containing
object. The value that is returned from your method will be
serialized. If your method returns undefined, then the member will
be excluded from the serialization.

If the replacer parameter is an array of strings, then it will be
used to select the members to be serialized. It filters the results
such that only members with keys listed in the replacer array are
stringified.

Values that do not have JSON representations, such as undefined or
functions, will not be serialized. Such values in objects will be
dropped; in arrays they will be replaced with null. You can use
a replacer function to replace those with JSON values.
JSON.stringify(undefined) returns undefined.

The optional space parameter produces a stringification of the
value that is filled with line breaks and indentation to make it
easier to read.

If the space parameter is a non-empty string, then that string will
be used for indentation. If the space parameter is a number, then
the indentation will be that many spaces.

Example:

text = JSON.stringify(['e', {pluribus: 'unum'}]);
// text is '["e",{"pluribus":"unum"}]'


text = JSON.stringify(['e', {pluribus: 'unum'}], null, '\t');
// text is '[\n\t"e",\n\t{\n\t\t"pluribus": "unum"\n\t}\n]'

text = JSON.stringify([new Date()], function (key, value) {
return this[key] instanceof Date ?
'Date(' + this[key] + ')' : value;
});
// text is '["Date(---current time---)"]'


JSON.parse(text, reviver)
This method parses a JSON text to produce an object or array.
It can throw a SyntaxError exception.

The optional reviver parameter is a function that can filter and
transform the results. It receives each of the keys and values,
and its return value is used instead of the original value.
If it returns what it received, then the structure is not modified.
If it returns undefined then the member is deleted.

Example:

// Parse the text. Values that look like ISO date strings will
// be converted to Date objects.

myData = JSON.parse(text, function (key, value) {
var a;
if (typeof value === 'string') {
a =
/^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)Z$/.exec(value);
if (a) {
return new Date(Date.UTC(+a[1], +a[2] - 1, +a[3], +a[4],
+a[5], +a[6]));
}
}
return value;
});

myData = JSON.parse('["Date(09/09/2001)"]', function (key, value) {
var d;
if (typeof value === 'string' &&
value.slice(0, 5) === 'Date(' &&
value.slice(-1) === ')') {
d = new Date(value.slice(5, -1));
if (d) {
return d;
}
}
return value;
});


This is a reference implementation. You are free to copy, modify, or
redistribute.
*/

/*jslint evil: true, strict: false, regexp: false */

/*members "", "\b", "\t", "\n", "\f", "\r", "\"", JSON, "\\", apply,
call, charCodeAt, getUTCDate, getUTCFullYear, getUTCHours,
getUTCMinutes, getUTCMonth, getUTCSeconds, hasOwnProperty, join,
lastIndex, length, parse, prototype, push, replace, slice, stringify,
test, toJSON, toString, valueOf
*/


// Create a JSON object only if one does not already exist. We create the
// methods in a closure to avoid creating global variables.

var JSON;
if (!JSON) {
	JSON = {};
}

(function () {
	"use strict";

	function f(n) {
		// Format integers to have at least two digits.
		return n < 10 ? '0' + n : n;
	}

	if (typeof Date.prototype.toJSON !== 'function') {

		Date.prototype.toJSON = function (key) {

			return isFinite(this.valueOf()) ?
								this.getUTCFullYear() + '-' +
								f(this.getUTCMonth() + 1) + '-' +
								f(this.getUTCDate()) + 'T' +
								f(this.getUTCHours()) + ':' +
								f(this.getUTCMinutes()) + ':' +
								f(this.getUTCSeconds()) + 'Z' : null;
		};

		String.prototype.toJSON =
						Number.prototype.toJSON =
						Boolean.prototype.toJSON = function (key) {
							return this.valueOf();
						};
	}

	var cx = /[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
				escapable = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
				gap,
				indent,
				meta = {    // table of character substitutions
					'\b': '\\b',
					'\t': '\\t',
					'\n': '\\n',
					'\f': '\\f',
					'\r': '\\r',
					'"': '\\"',
					'\\': '\\\\'
				},
				rep;


	function quote(string) {

		// If the string contains no control characters, no quote characters, and no
		// backslash characters, then we can safely slap some quotes around it.
		// Otherwise we must also replace the offending characters with safe escape
		// sequences.

		escapable.lastIndex = 0;
		return escapable.test(string) ? '"' + string.replace(escapable, function (a) {
			var c = meta[a];
			return typeof c === 'string' ? c :
								'\\u' + ('0000' + a.charCodeAt(0).toString(16)).slice(-4);
		}) + '"' : '"' + string + '"';
	}


	function str(key, holder) {

		// Produce a string from holder[key].

		var i,          // The loop counter.
						k,          // The member key.
						v,          // The member value.
						length,
						mind = gap,
						partial,
						value = holder[key];

		// If the value has a toJSON method, call it to obtain a replacement value.

		if (value && typeof value === 'object' &&
								typeof value.toJSON === 'function') {
			value = value.toJSON(key);
		}

		// If we were called with a replacer function, then call the replacer to
		// obtain a replacement value.

		if (typeof rep === 'function') {
			value = rep.call(holder, key, value);
		}

		// What happens next depends on the value's type.

		switch (typeof value) {
			case 'string':
				return quote(value);

			case 'number':

				// JSON numbers must be finite. Encode non-finite numbers as null.

				return isFinite(value) ? String(value) : 'null';

			case 'boolean':
			case 'null':

				// If the value is a boolean or null, convert it to a string. Note:
				// typeof null does not produce 'null'. The case is included here in
				// the remote chance that this gets fixed someday.

				return String(value);

				// If the type is 'object', we might be dealing with an object or an array or
				// null.

			case 'object':

				// Due to a specification blunder in ECMAScript, typeof null is 'object',
				// so watch out for that case.

				if (!value) {
					return 'null';
				}

				// Make an array to hold the partial results of stringifying this object value.

				gap += indent;
				partial = [];

				// Is the value an array?

				if (Object.prototype.toString.apply(value) === '[object Array]') {

					// The value is an array. Stringify every element. Use null as a placeholder
					// for non-JSON values.

					length = value.length;
					for (i = 0; i < length; i += 1) {
						partial[i] = str(i, value) || 'null';
					}

					// Join all of the elements together, separated with commas, and wrap them in
					// brackets.

					v = partial.length === 0 ? '[]' : gap ?
										'[\n' + gap + partial.join(',\n' + gap) + '\n' + mind + ']' :
										'[' + partial.join(',') + ']';
					gap = mind;
					return v;
				}

				// If the replacer is an array, use it to select the members to be stringified.

				if (rep && typeof rep === 'object') {
					length = rep.length;
					for (i = 0; i < length; i += 1) {
						if (typeof rep[i] === 'string') {
							k = rep[i];
							v = str(k, value);
							if (v) {
								partial.push(quote(k) + (gap ? ': ' : ':') + v);
							}
						}
					}
				} else {

					// Otherwise, iterate through all of the keys in the object.

					for (k in value) {
						if (Object.prototype.hasOwnProperty.call(value, k)) {
							v = str(k, value);
							if (v) {
								partial.push(quote(k) + (gap ? ': ' : ':') + v);
							}
						}
					}
				}

				// Join all of the member texts together, separated with commas,
				// and wrap them in braces.

				v = partial.length === 0 ? '{}' : gap ?
								'{\n' + gap + partial.join(',\n' + gap) + '\n' + mind + '}' :
								'{' + partial.join(',') + '}';
				gap = mind;
				return v;
		}
	}

	// If the JSON object does not yet have a stringify method, give it one.

	if (typeof JSON.stringify !== 'function') {
		JSON.stringify = function (value, replacer, space) {

			// The stringify method takes a value and an optional replacer, and an optional
			// space parameter, and returns a JSON text. The replacer can be a function
			// that can replace values, or an array of strings that will select the keys.
			// A default replacer method can be provided. Use of the space parameter can
			// produce text that is more easily readable.

			var i;
			gap = '';
			indent = '';

			// If the space parameter is a number, make an indent string containing that
			// many spaces.

			if (typeof space === 'number') {
				for (i = 0; i < space; i += 1) {
					indent += ' ';
				}

				// If the space parameter is a string, it will be used as the indent string.

			} else if (typeof space === 'string') {
				indent = space;
			}

			// If there is a replacer, it must be a function or an array.
			// Otherwise, throw an error.

			rep = replacer;
			if (replacer && typeof replacer !== 'function' &&
										(typeof replacer !== 'object' ||
										typeof replacer.length !== 'number')) {
				throw new Error('JSON.stringify');
			}

			// Make a fake root object containing our value under the key of ''.
			// Return the result of stringifying the value.

			return str('', { '': value });
		};
	}


	// If the JSON object does not yet have a parse method, give it one.

	if (typeof JSON.parse !== 'function') {
		JSON.parse = function (text, reviver) {

			// The parse method takes a text and an optional reviver function, and returns
			// a JavaScript value if the text is a valid JSON text.

			var j;

			function walk(holder, key) {

				// The walk method is used to recursively walk the resulting structure so
				// that modifications can be made.

				var k, v, value = holder[key];
				if (value && typeof value === 'object') {
					for (k in value) {
						if (Object.prototype.hasOwnProperty.call(value, k)) {
							v = walk(value, k);
							if (v !== undefined) {
								value[k] = v;
							} else {
								delete value[k];
							}
						}
					}
				}
				return reviver.call(holder, key, value);
			}


			// Parsing happens in four stages. In the first stage, we replace certain
			// Unicode characters with escape sequences. JavaScript handles many characters
			// incorrectly, either silently deleting them, or treating them as line endings.

			text = String(text);
			cx.lastIndex = 0;
			if (cx.test(text)) {
				text = text.replace(cx, function (a) {
					return '\\u' +
												('0000' + a.charCodeAt(0).toString(16)).slice(-4);
				});
			}

			// In the second stage, we run the text against regular expressions that look
			// for non-JSON patterns. We are especially concerned with '()' and 'new'
			// because they can cause invocation, and '=' because it can cause mutation.
			// But just to be safe, we want to reject all unexpected forms.

			// We split the second stage into 4 regexp operations in order to work around
			// crippling inefficiencies in IE's and Safari's regexp engines. First we
			// replace the JSON backslash pairs with '@' (a non-JSON character). Second, we
			// replace all simple value tokens with ']' characters. Third, we delete all
			// open brackets that follow a colon or comma or that begin the text. Finally,
			// we look to see that the remaining characters are only whitespace or ']' or
			// ',' or ':' or '{' or '}'. If that is so, then the text is safe for eval.

			if (/^[\],:{}\s]*$/
										.test(text.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, '@')
												.replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']')
												.replace(/(?:^|:|,)(?:\s*\[)+/g, ''))) {

				// In the third stage we use the eval function to compile the text into a
				// JavaScript structure. The '{' operator is subject to a syntactic ambiguity
				// in JavaScript: it can begin a block or an object literal. We wrap the text
				// in parens to eliminate the ambiguity.

				j = eval('(' + text + ')');

				// In the optional fourth stage, we recursively walk the new structure, passing
				// each name/value pair to a reviver function for possible transformation.

				return typeof reviver === 'function' ?
										walk({ '': j }, '') : j;
			}

			// If the text is not JSON parseable, then a SyntaxError is thrown.

			throw new SyntaxError('JSON.parse');
		};
	}
}());

//#endregion json2.js

//#region jsrender

/*! JsRender v1.0.0-beta: http://github.com/BorisMoore/jsrender and http://jsviews.com/jsviews
informal pre V1.0 commit counter: 53 */
/*
 * Optimized version of jQuery Templates, for rendering to string.
 * Does not require jQuery, or HTML DOM
 * Integrates with JsViews (http://jsviews.com/jsviews)
 *
 * Copyright 2014, Boris Moore
 * Released under the MIT License.
 */

(function (global, jQuery, undefined) {
	// global is the this object, which is window when running in the usual browser environment.
	"use strict";

	if (jQuery && jQuery.views || global.jsviews) { return; } // JsRender is already loaded

	//========================== Top-level vars ==========================
	//onInit versus init? inherit/base/deriveFrom/extend/basetag

	var versionNumber = "v1.0.0-beta",

		$, jsvStoreName, rTag, rTmplString, indexStr, // nodeJsModule,

//TODO	tmplFnsCache = {},
		delimOpenChar0 = "{", delimOpenChar1 = "{", delimCloseChar0 = "}", delimCloseChar1 = "}", linkChar = "^",

		rPath = /^(!*?)(?:null|true|false|\d[\d.]*|([\w$]+|\.|~([\w$]+)|#(view|([\w$]+))?)([\w$.^]*?)(?:[.[^]([\w$]+)\]?)?)$/g,
		//                                     none   object     helper    view  viewProperty pathTokens      leafToken

		rParams = /(\()(?=\s*\()|(?:([([])\s*)?(?:(\^?)(!*?[#~]?[\w$.^]+)?\s*((\+\+|--)|\+|-|&&|\|\||===|!==|==|!=|<=|>=|[<>%*:?\/]|(=))\s*|(!*?[#~]?[\w$.^]+)([([])?)|(,\s*)|(\(?)\\?(?:(')|("))|(?:\s*(([)\]])(?=\s*\.|\s*\^|\s*$)|[)\]])([([]?))|(\s+)/g,
		//          lftPrn0        lftPrn        bound            path    operator err                                                eq             path2       prn    comma   lftPrn2   apos quot      rtPrn rtPrnDot                        prn2      space
		// (left paren? followed by (path? followed by operator) or (path followed by left paren?)) or comma or apos or quot or right paren or space

		rNewLine = /[ \t]*(\r\n|\n|\r)/g,
		rUnescapeQuotes = /\\(['"])/g,
		rEscapeQuotes = /['"\\]/g, // Escape quotes and \ character
		rBuildHash = /\x08(~)?([^\x08]+)\x08/g,
		rTestElseIf = /^if\s/,
		rFirstElem = /<(\w+)[>\s]/,
		rAttrEncode = /[\x00`><"'&]/g, // Includes > encoding since rConvertMarkers in JsViews does not skip > characters in attribute strings
		rHasHandlers = /^on[A-Z]|^convert(Back)?$/,
		rHtmlEncode = rAttrEncode,
		autoTmplName = 0,
		viewId = 0,
		charEntities = {
			"&": "&amp;",
			"<": "&lt;",
			">": "&gt;",
			"\x00": "&#0;",
			"'": "&#39;",
			'"': "&#34;",
			"`": "&#96;"
		},
		htmlStr = "html",
		tmplAttr = "data-jsv-tmpl",
		$render = {},
		jsvStores = {
			template: {
				compile: compileTmpl
			},
			tag: {
				compile: compileTag
			},
			helper: {},
			converter: {}
		},

		// jsviews object ($.views if jQuery is loaded)
		$views = {
			jsviews: versionNumber,
			render: $render,
			settings: function (settings) {
				$extend($viewsSettings, settings);
				dbgMode($viewsSettings._dbgMode);
				if ($viewsSettings.jsv) {
					$viewsSettings.jsv();
				}
			},
			sub: {
				// subscription, e.g. JsViews integration
				View: View,
				Err: JsViewsError,
				tmplFn: tmplFn,
				cvt: convertArgs,
				parse: parseParams,
				extend: $extend,
				err: error,
				syntaxErr: syntaxError,
				isFn: function (ob) {
					return typeof ob === "function"
				},
				DataMap: DataMap
			},
			_cnvt: convertVal,
			_tag: renderTag,

			_err: function (e) {
				// Place a breakpoint here to intercept template rendering errors
				return $viewsSettings._dbgMode ? ("Error: " + (e.message || e)) + ". " : '';
			}
		};

	function retVal(val) {
		return val;
	}

	function dbgBreak(val) {
		debugger;
		return val;
	}

	function dbgMode(debugMode) {
		$viewsSettings._dbgMode = debugMode;
		indexStr = debugMode ? "Unavailable (nested view): use #getIndex()" : ""; // If in debug mode set #index to a warning when in nested contexts
		$tags("dbg", $helpers.dbg = $converters.dbg = debugMode ? dbgBreak : retVal); // If in debug mode, register {{dbg/}}, {{dbg:...}} and ~dbg() to insert break points for debugging.
	}

	function DataMap(getTarget) {
		return {
			getTgt: getTarget,
			map: function (source) {
				var theMap = this; // Instance of DataMap
				if (theMap.src !== source) {
					if (theMap.src) {
						theMap.unmap();
					}
					if (typeof source === "object") {
						var changing,
						target = getTarget.apply(theMap, arguments);
						theMap.src = source;
						theMap.tgt = target;
					}
				}
			}
		}
	}

	function JsViewsError(message, object) {
		// Error exception type for JsViews/JsRender
		// Override of $.views.sub.Error is possible
		if (object && object.onError) {
			if (object.onError(message) === false) {
				return;
			}
		}
		this.name = ($.link ? "JsViews" : "JsRender") + " Error";
		this.message = message || this.name;
	}

	function $extend(target, source) {
		var name;
		target = target || {};
		for (name in source) {
			target[name] = source[name];
		}
		return target;
	}

	(JsViewsError.prototype = new Error()).constructor = JsViewsError;

	//========================== Top-level functions ==========================

	//===================
	// jsviews.delimiters
	//===================
	function $viewsDelimiters(openChars, closeChars, link) {
		// Set the tag opening and closing delimiters and 'link' character. Default is "{{", "}}" and "^"
		// openChars, closeChars: opening and closing strings, each with two characters

		if (!$viewsSub.rTag || openChars) {
			delimOpenChar0 = openChars ? openChars.charAt(0) : delimOpenChar0; // Escape the characters - since they could be regex special characters
			delimOpenChar1 = openChars ? openChars.charAt(1) : delimOpenChar1;
			delimCloseChar0 = closeChars ? closeChars.charAt(0) : delimCloseChar0;
			delimCloseChar1 = closeChars ? closeChars.charAt(1) : delimCloseChar1;
			linkChar = link || linkChar;
			openChars = "\\" + delimOpenChar0 + "(\\" + linkChar + ")?\\" + delimOpenChar1;  // Default is "{^{"
			closeChars = "\\" + delimCloseChar0 + "\\" + delimCloseChar1;                   // Default is "}}"
			// Build regex with new delimiters
			//          tag    (followed by / space or })   or cvtr+colon or html or code
			rTag = "(?:(?:(\\w+(?=[\\/\\s\\" + delimCloseChar0 + "]))|(?:(\\w+)?(:)|(>)|!--((?:[^-]|-(?!-))*)--|(\\*)))"
				+ "\\s*((?:[^\\" + delimCloseChar0 + "]|\\" + delimCloseChar0 + "(?!\\" + delimCloseChar1 + "))*?)";

			// make rTag available to JsViews (or other components) for parsing binding expressions
			$viewsSub.rTag = rTag + ")";

			rTag = new RegExp(openChars + rTag + "(\\/)?|(?:\\/(\\w+)))" + closeChars, "g");

			// Default:    bind           tag       converter colon html     comment            code      params            slash   closeBlock
			//           /{(\^)?{(?:(?:(\w+(?=[\/\s}]))|(?:(\w+)?(:)|(>)|!--((?:[^-]|-(?!-))*)--|(\*)))\s*((?:[^}]|}(?!}))*?)(\/)?|(?:\/(\w+)))}}/g

			rTmplString = new RegExp("<.*>|([^\\\\]|^)[{}]|" + openChars + ".*" + closeChars);
			// rTmplString looks for html tags or { or } char not preceded by \\, or JsRender tags {{xxx}}. Each of these strings are considered
			// NOT to be jQuery selectors
		}
		return [delimOpenChar0, delimOpenChar1, delimCloseChar0, delimCloseChar1, linkChar];
	}

	//=========
	// View.get
	//=========

	function getView(inner, type) { //view.get(inner, type)
		if (!type) {
			// view.get(type)
			type = inner;
			inner = undefined;
		}

		var views, i, l, found,
			view = this,
			root = !type || type === "root";
		// If type is undefined, returns root view (view under top view).

		if (inner) {
			// Go through views - this one, and all nested ones, depth-first - and return first one with given type.
			found = view.type === type ? view : undefined;
			if (!found) {
				views = view.views;
				if (view._.useKey) {
					for (i in views) {
						if (found = views[i].get(inner, type)) {
							break;
						}
					}
				} else for (i = 0, l = views.length; !found && i < l; i++) {
					found = views[i].get(inner, type);
				}
			}
		} else if (root) {
			// Find root view. (view whose parent is top view)
			while (view.parent.parent) {
				found = view = view.parent;
			}
		} else while (view && !found) {
			// Go through views - this one, and all parent ones - and return first one with given type.
			found = view.type === type ? view : undefined;
			view = view.parent;
		}
		return found;
	}

	function getNestedIndex() {
		var view = this.get("item");
		return view ? view.index : undefined;
	}

	getNestedIndex.depends = function () {
		return [this.get("item"), "index"];
	};

	function getIndex() {
		return this.index;
	}

	getIndex.depends = function () {
		return ["index"];
	};

	//==========
	// View.hlp
	//==========

	function getHelper(helper) {
		// Helper method called as view.hlp(key) from compiled template, for helper functions or template parameters ~foo
		var wrapped,
			view = this,
			ctx = view.linkCtx,
			res = (view.ctx || {})[helper];

		if (res === undefined && ctx && ctx.ctx) {
			res = ctx.ctx[helper];
		}
		if (res === undefined) {
			res = $helpers[helper];
		}

		if (res) {
			if ($isFunction(res) && !res._wrp) {
				wrapped = function () {
					// If it is of type function, and not already wrapped, we will wrap it, so if called with no this pointer it will be called with the
					// view as 'this' context. If the helper ~foo() was in a data-link expression, the view will have a 'temporary' linkCtx property too.
					// Note that helper functions on deeper paths will have specific this pointers, from the preceding path.
					// For example, ~util.foo() will have the ~util object as 'this' pointer
					return res.apply((!this || this === global) ? view : this, arguments);
				};
				wrapped._wrp = 1;
				$extend(wrapped, res); // Attach same expandos (if any) to the wrapped function
			}
		}
		return wrapped || res;
	}

	//==============
	// jsviews._cnvt
	//==============

	function convertVal(converter, view, tagCtx) {
		// self is template object or linkCtx object
		var tag, value, prop,
			boundTagCtx = +tagCtx === tagCtx && tagCtx, // if tagCtx is an integer, then it is the key for the boundTagCtx (compiled function to return the tagCtx)
			linkCtx = view.linkCtx; // For data-link="{cvt:...}"...

		if (boundTagCtx) {
			// This is a bound tag: {^{xx:yyy}}. Call compiled function which returns the tagCtxs for current data
			tagCtx = (boundTagCtx = view.tmpl.bnds[boundTagCtx - 1])(view.data, view, $views);
		}

		value = tagCtx.args[0];
		if (converter || boundTagCtx) {
			tag = linkCtx && linkCtx.tag || {
				_: {
					inline: !linkCtx,
					bnd: boundTagCtx
				},
				tagName: converter + ":",
				flow: true,
				_is: "tag"
			};

			for (prop in tagCtx.props) {
				if (rHasHandlers.test(prop)) {
					tag[prop] = tagCtx.props[prop]; // Copy over the onFoo props from tagCtx.props to tag (overrides values in tagDef).
				}
			}

			if (linkCtx) {
				linkCtx.tag = tag;
				tag.linkCtx = tag.linkCtx || linkCtx;
				tagCtx.ctx = extendCtx(tagCtx.ctx, linkCtx.view.ctx);
			}
			tag.tagCtx = tagCtx;
			tagCtx.view = view;

			tag.ctx = tagCtx.ctx || {};
			delete tagCtx.ctx;
			// Provide this tag on view, for addBindingMarkers on bound tags to add the tag to view._.bnds, associated with the tag id,
			view._.tag = tag;

			value = convertArgs(tag, tag.convert || converter !== "true" && converter)[0]; // If there is a convertBack but no convert, converter will be "true"

			// Call onRender (used by JsViews if present, to add binding annotations around rendered content)
			value = value != undefined ? value : "";
			value = boundTagCtx && view._.onRender
				? view._.onRender(value, view, boundTagCtx)
				: value;
			view._.tag = undefined;
		}
		return value;
	}

	function convertArgs(tag, converter) {
		var tagCtx = tag.tagCtx,
			view = tagCtx.view,
			args = tagCtx.args;

		converter = converter && ("" + converter === converter
			? (view.getRsc("converters", converter) || error("Unknown converter: '" + converter + "'"))
			: converter);

		args = !args.length && !tagCtx.index && tag.autoBind // On the opening tag with no args, if autoBind is true, bind the the current data context
			? [view.data]
			: converter
				? args.slice() // If there is a converter, use a copy of the tagCtx.args array for rendering, and replace the args[0] in
				// the copied array with the converted value. But we don not modify the value of tag.tagCtx.args[0] (the original args array)
				: args; // If no converter, render with the original tagCtx.args

		if (converter) {
			if (converter.depends) {
				tag.depends = $viewsSub.getDeps(tag.depends, tag, converter.depends, converter);
			}
			args[0] = converter.apply(tag, args);
		}
		return args;
	}

	//=============
	// jsviews._tag
	//=============

	function getResource(resourceType, itemName) {
		var res, store,
			view = this;
		while ((res === undefined) && view) {
			store = view.tmpl[resourceType];
			res = store && store[itemName];
			view = view.parent;
		}
		return res || $views[resourceType][itemName];
	}

	function renderTag(tagName, parentView, tmpl, tagCtxs, isRefresh) {
		// Called from within compiled template function, to render a template tag
		// Returns the rendered tag

		var render, tag, tags, attr, parentTag, i, l, itemRet, tagCtx, tagCtxCtx, content, boundTagFn, tagDef,
			callInit, map, thisMap, args, prop, props, converter, initialTmpl,
			ret = "",
			boundTagKey = +tagCtxs === tagCtxs && tagCtxs, // if tagCtxs is an integer, then it is the boundTagKey
			linkCtx = parentView.linkCtx || 0,
			ctx = parentView.ctx,
			parentTmpl = tmpl || parentView.tmpl;

		if (tagName._is === "tag") {
			tag = tagName;
			tagName = tag.tagName;
		}
		tag = tag || linkCtx.tag;

		// Provide tagCtx, linkCtx and ctx access from tag
		if (boundTagKey) {
			// if tagCtxs is an integer, we are data binding
			// Call compiled function which returns the tagCtxs for current data
			tagCtxs = (boundTagFn = parentTmpl.bnds[boundTagKey - 1])(parentView.data, parentView, $views);
		}

		l = tagCtxs.length;
		for (i = 0; i < l; i++) {
			tagCtx = tagCtxs[i];
			props = tagCtx.props;

			// Set the tmpl property to the content of the block tag, unless set as an override property on the tag
			content = tagCtx.tmpl;
			content = tagCtx.content = content && parentTmpl.tmpls[content - 1];
			tmpl = tagCtx.props.tmpl;
			if (!i && (!tmpl || !tag)) {
				tagDef = parentView.getRsc("tags", tagName) || error("Unknown tag: {{" + tagName + "}}");
			}
			tmpl = tmpl || (tag ? tag : tagDef).template || content;
			tmpl = "" + tmpl === tmpl // if a string
				? parentView.getRsc("templates", tmpl) || $templates(tmpl)
				: tmpl;

			$extend(tagCtx, {
				tmpl: tmpl,
				render: renderContent,
				index: i,
				view: parentView,
				ctx: extendCtx(tagCtx.ctx, ctx) // Extend parentView.ctx
			}); // Extend parentView.ctx

			if (!tag) {
				// This will only be hit for initial tagCtx (not for {{else}}) - if the tag instance does not exist yet
				// Instantiate tag if it does not yet exist
				if (tagDef._ctr) {
					// If the tag has not already been instantiated, we will create a new instance.
					// ~tag will access the tag, even within the rendering of the template content of this tag.
					// From child/descendant tags, can access using ~tag.parent, or ~parentTags.tagName
					//	TODO provide error handling owned by the tag - using tag.onError
					//				try {
					tag = new tagDef._ctr();
					callInit = !!tag.init;
					//				}
					//				catch(e) {
					//					tagDef.onError(e);
					//				}
					// Set attr on linkCtx to ensure outputting to the correct target attribute.
					tag.attr = tag.attr || tagDef.attr || undefined;
					// Setting either linkCtx.attr or this.attr in the init() allows per-instance choice of target attrib.
				} else {
					// This is a simple tag declared as a function, or with init set to false. We won't instantiate a specific tag constructor - just a standard instance object.
					tag = {
						// tag instance object if no init constructor
						render: tagDef.render
					};
				}
				tag._ = {
					inline: !linkCtx
				};
				if (linkCtx) {
					// Set attr on linkCtx to ensure outputting to the correct target attribute.
					linkCtx.attr = tag.attr = linkCtx.attr || tag.attr;
					linkCtx.tag = tag;
					tag.linkCtx = linkCtx;
				}
				if (tag._.bnd = boundTagFn || linkCtx.fn) {
					// Bound if {^{tag...}} or data-link="{tag...}"
					tag._.arrVws = {};
				} else if (tag.dataBoundOnly) {
					error("{^{" + tagName + "}} tag must be data-bound");
				}
				tag.tagName = tagName;
				tag.parent = parentTag = ctx && ctx.tag;
				tag._is = "tag";
				tag._def = tagDef;

				for (prop in props = tagCtx.props) {
					if (rHasHandlers.test(prop)) {
						tag[prop] = props[prop]; // Copy over the onFoo or convert or convertBack props from tagCtx.props to tag (overrides values in tagDef).
					}
				}
				//TODO better perf for childTags() - keep child tag.tags array, (and remove child, when disposed)
				// tag.tags = [];
				// Provide this tag on view, for addBindingMarkers on bound tags to add the tag to view._.bnds, associated with the tag id,
			}
			tagCtx.tag = tag;
			if (tag.map && tag.tagCtxs) {
				tagCtx.map = tag.tagCtxs[i].map; // Copy over the compiled map instance from the previous tagCtxs to the refreshed ones
			}
			if (!tag.flow) {
				tagCtxCtx = tagCtx.ctx = tagCtx.ctx || {};

				// tags hash: tag.ctx.tags, merged with parentView.ctx.tags,
				tags = tag.parents = tagCtxCtx.parentTags = ctx && extendCtx(tagCtxCtx.parentTags, ctx.parentTags) || {};
				if (parentTag) {
					tags[parentTag.tagName] = parentTag;
					//TODO better perf for childTags: parentTag.tags.push(tag);
				}
				tagCtxCtx.tag = tag;
			}
		}
		tag.tagCtxs = tagCtxs;
		parentView._.tag = tag;
		tag.rendering = {}; // Provide object for state during render calls to tag and elses. (Used by {{if}} and {{for}}...)
		for (i = 0; i < l; i++) {
			tagCtx = tag.tagCtx = tagCtxs[i];
			props = tagCtx.props;
			args = convertArgs(tag, tag.convert);

			if ((map = props.map || tag).map) {
				if (args.length || props.map) {
					thisMap = tagCtx.map = $extend(tagCtx.map || { unmap: map.unmap }, props); // Compiled map instance
					if (thisMap.src !== args[0]) {
						if (thisMap.src) {
							thisMap.unmap();
						}
						map.map.apply(thisMap, args);
					}
					args = [thisMap.tgt];
				}
			}
			tag.ctx = tagCtx.ctx;

			if (!i && callInit) {
				initialTmpl = tag.template;
				tag.init(tagCtx, linkCtx, tag.ctx);
				callInit = undefined;
				if (tag.template !== initialTmpl) {
					tag._.tmpl = tag.template; // This will override the tag.template and also tagCtx.props.tmpl for all tagCtxs
				}
			}

			itemRet = undefined;
			render = tag.render;
			if (render = tag.render) {
				itemRet = render.apply(tag, args);
			}
			args = args.length ? args : [parentView]; // no arguments - get data context from view.
			itemRet = itemRet !== undefined
				? itemRet // Return result of render function unless it is undefined, in which case return rendered template
				: tagCtx.render(args[0], true) || (isRefresh ? undefined : "");
			// No return value from render, and no template/content tagCtx.render(...), so return undefined
			ret = ret ? ret + (itemRet || "") : itemRet; // If no rendered content, this will be undefined
		}

		delete tag.rendering;

		tag.tagCtx = tag.tagCtxs[0];
		tag.ctx = tag.tagCtx.ctx;

		if (tag._.inline && (attr = tag.attr) && attr !== htmlStr) {
			// inline tag with attr set to "text" will insert HTML-encoded content - as if it was element-based innerText
			ret = attr === "text"
				? $converters.html(ret)
				: "";
		}
		return boundTagKey && parentView._.onRender
			// Call onRender (used by JsViews if present, to add binding annotations around rendered content)
			? parentView._.onRender(ret, parentView, boundTagKey)
			: ret;
	}

	//=================
	// View constructor
	//=================

	function View(context, type, parentView, data, template, key, contentTmpl, onRender) {
		// Constructor for view object in view hierarchy. (Augmented by JsViews if JsViews is loaded)
		var views, parentView_, tag,
			isArray = type === "array",
			self_ = {
				key: 0,
				useKey: isArray ? 0 : 1,
				id: "" + viewId++,
				onRender: onRender,
				bnds: {}
			},
			self = {
				data: data,
				tmpl: template,
				content: contentTmpl,
				views: isArray ? [] : {},
				parent: parentView,
				type: type,
				// If the data is an array, this is an 'array view' with a views array for each child 'item view'
				// If the data is not an array, this is an 'item view' with a views 'map' object for any child nested views
				// ._.useKey is non zero if is not an 'array view' (owning a data array). Uuse this as next key for adding to child views map
				get: getView,
				getIndex: getIndex,
				getRsc: getResource,
				hlp: getHelper,
				_: self_,
				_is: "view"
			};
		if (parentView) {
			views = parentView.views;
			parentView_ = parentView._;
			if (parentView_.useKey) {
				// Parent is an 'item view'. Add this view to its views object
				// self._key = is the key in the parent view map
				views[self_.key = "_" + parentView_.useKey++] = self;
				self.index = indexStr;
				self.getIndex = getNestedIndex;
				tag = parentView_.tag;
				self_.bnd = isArray && (!tag || !!tag._.bnd && tag); // For array views that are data bound for collection change events, set the
				// view._.bnd property to true for top-level link() or data-link="{for}", or to the tag instance for a data-bound tag, e.g. {^{for ...}}
			} else {
				// Parent is an 'array view'. Add this view to its views array
				views.splice(
					// self._.key = self.index - the index in the parent view array
					self_.key = self.index = key,
				0, self);
			}
			// If no context was passed in, use parent context
			// If context was passed in, it should have been merged already with parent context
			self.ctx = context || parentView.ctx;
		} else {
			self.ctx = context;
		}
		return self;
	}

	//=============
	// Registration
	//=============

	function compileChildResources(parentTmpl) {
		var storeName, resources, resourceName, settings, compile;
		for (storeName in jsvStores) {
			settings = jsvStores[storeName];
			if ((compile = settings.compile) && (resources = parentTmpl[storeName + "s"])) {
				for (resourceName in resources) {
					// compile child resource declarations (templates, tags, converters or helpers)
					resources[resourceName] = compile(resourceName, resources[resourceName], parentTmpl, storeName, settings);
				}
			}
		}
	}

	function compileTag(name, tagDef, parentTmpl) {
		var init, tmpl;
		if ($isFunction(tagDef)) {
			// Simple tag declared as function. No presenter instantation.
			tagDef = {
				depends: tagDef.depends,
				render: tagDef
			};
		} else {
			// Tag declared as object, used as the prototype for tag instantiation (control/presenter)
			if (tmpl = tagDef.template) {
				tagDef.template = "" + tmpl === tmpl ? ($templates[tmpl] || $templates(tmpl)) : tmpl;
			}
			if (tagDef.init !== false) {
				// Set int: false on tagDef if you want to provide just a render method, or render and template, but no constuctor or prototype.
				// so equivalent to setting tag to render function, except you can also provide a template.
				init = tagDef._ctr = function (tagCtx) { };
				(init.prototype = tagDef).constructor = init;
			}
		}
		if (parentTmpl) {
			tagDef._parentTmpl = parentTmpl;
		}
		//TODO	tagDef.onError = function(e) {
		//			var error;
		//			if (error = this.prototype.onError) {
		//				error.call(this, e);
		//			} else {
		//				throw e;
		//			}
		//		}
		return tagDef;
	}

	function compileTmpl(name, tmpl, parentTmpl, storeName, storeSettings, options) {
		// tmpl is either a template object, a selector for a template script block, the name of a compiled template, or a template object

		//==== nested functions ====
		function tmplOrMarkupFromStr(value) {
			// If value is of type string - treat as selector, or name of compiled template
			// Return the template object, if already compiled, or the markup string

			if (("" + value === value) || value.nodeType > 0) {
				try {
					elem = value.nodeType > 0
					? value
					: !rTmplString.test(value)
					// If value is a string and does not contain HTML or tag content, then test as selector
						&& jQuery && jQuery(global.document).find(value)[0]; // TODO address case where DOM is not available
					// If selector is valid and returns at least one element, get first element
					// If invalid, jQuery will throw. We will stay with the original string.
				} catch (e) { }

				if (elem) {
					// Generally this is a script element.
					// However we allow it to be any element, so you can for example take the content of a div,
					// use it as a template, and replace it by the same content rendered against data.
					// e.g. for linking the content of a div to a container, and using the initial content as template:
					// $.link("#content", model, {tmpl: "#content"});

					value = elem.getAttribute(tmplAttr);
					name = name || value;
					value = $templates[value];
					if (!value) {
						// Not already compiled and cached, so compile and cache the name
						// Create a name for compiled template if none provided
						name = name || "_" + autoTmplName++;
						elem.setAttribute(tmplAttr, name);
						// Use tmpl as options
						value = $templates[name] = compileTmpl(name, elem.innerHTML, parentTmpl, storeName, storeSettings, options);
					}
					elem = null;
				}
				return value;
			}
			// If value is not a string, return undefined
		}

		var tmplOrMarkup, elem;

		//==== Compile the template ====
		tmpl = tmpl || "";
		tmplOrMarkup = tmplOrMarkupFromStr(tmpl);

		// If options, then this was already compiled from a (script) element template declaration.
		// If not, then if tmpl is a template object, use it for options
		options = options || (tmpl.markup ? tmpl : {});
		options.tmplName = name;
		if (parentTmpl) {
			options._parentTmpl = parentTmpl;
		}
		// If tmpl is not a markup string or a selector string, then it must be a template object
		// In that case, get it from the markup property of the object
		if (!tmplOrMarkup && tmpl.markup && (tmplOrMarkup = tmplOrMarkupFromStr(tmpl.markup))) {
			if (tmplOrMarkup.fn && (tmplOrMarkup.debug !== tmpl.debug || tmplOrMarkup.allowCode !== tmpl.allowCode)) {
				// if the string references a compiled template object, but the debug or allowCode props are different, need to recompile
				tmplOrMarkup = tmplOrMarkup.markup;
			}
		}
		if (tmplOrMarkup !== undefined) {
			if (name && !parentTmpl) {
				$render[name] = function () {
					return tmpl.render.apply(tmpl, arguments);
				};
			}
			if (tmplOrMarkup.fn || tmpl.fn) {
				// tmpl is already compiled, so use it, or if different name is provided, clone it
				if (tmplOrMarkup.fn) {
					if (name && name !== tmplOrMarkup.tmplName) {
						tmpl = extendCtx(options, tmplOrMarkup);
					} else {
						tmpl = tmplOrMarkup;
					}
				}
			} else {
				// tmplOrMarkup is a markup string, not a compiled template
				// Create template object
				tmpl = TmplObject(tmplOrMarkup, options);
				// Compile to AST and then to compiled function
				tmplFn(tmplOrMarkup.replace(rEscapeQuotes, "\\$&"), tmpl);
			}
			compileChildResources(options);
			return tmpl;
		}
	}
	//==== /end of function compile ====

	function TmplObject(markup, options) {
		// Template object constructor
		var htmlTag,
			wrapMap = $viewsSettings.wrapMap || {},
			tmpl = $extend(
				{
					markup: markup,
					tmpls: [],
					links: {}, // Compiled functions for link expressions
					tags: {}, // Compiled functions for bound tag expressions
					bnds: [],
					_is: "template",
					render: renderContent
				},
				options
			);

		if (!options.htmlTag) {
			// Set tmpl.tag to the top-level HTML tag used in the template, if any...
			htmlTag = rFirstElem.exec(markup);
			tmpl.htmlTag = htmlTag ? htmlTag[1].toLowerCase() : "";
		}
		htmlTag = wrapMap[tmpl.htmlTag];
		if (htmlTag && htmlTag !== wrapMap.div) {
			// When using JsViews, we trim templates which are inserted into HTML contexts where text nodes are not rendered (i.e. not 'Phrasing Content').
			// Currently not trimmed for <li> tag. (Not worth adding perf cost)
			tmpl.markup = $.trim(tmpl.markup);
		}

		return tmpl;
	}

	function registerStore(storeName, storeSettings) {

		function theStore(name, item, parentTmpl) {
			// The store is also the function used to add items to the store. e.g. $.templates, or $.views.tags

			// For store of name 'thing', Call as:
			//    $.views.things(items[, parentTmpl]),
			// or $.views.things(name, item[, parentTmpl])

			var onStore, compile, itemName, thisStore;

			if (name && "" + name !== name && !name.nodeType && !name.markup) {
				// Call to $.views.things(items[, parentTmpl]),

				// Adding items to the store
				// If name is a map, then item is parentTmpl. Iterate over map and call store for key.
				for (itemName in name) {
					theStore(itemName, name[itemName], item);
				}
				return $views;
			}
			// Adding a single unnamed item to the store
			if (item === undefined) {
				item = name;
				name = undefined;
			}
			if (name && "" + name !== name) { // name must be a string
				parentTmpl = item;
				item = name;
				name = undefined;
			}
			thisStore = parentTmpl ? parentTmpl[storeNames] = parentTmpl[storeNames] || {} : theStore;
			compile = storeSettings.compile;
			if (onStore = $viewsSub.onBeforeStoreItem) {
				// e.g. provide an external compiler or preprocess the item.
				compile = onStore(thisStore, name, item, compile) || compile;
			}
			if (!name) {
				item = compile(undefined, item);
			} else if (item === null) {
				// If item is null, delete this entry
				delete thisStore[name];
			} else {
				thisStore[name] = compile ? (item = compile(name, item, parentTmpl, storeName, storeSettings)) : item;
			}
			if (compile && item) {
				item._is = storeName; // Only do this for compiled objects (tags, templates...)
			}
			if (onStore = $viewsSub.onStoreItem) {
				// e.g. JsViews integration
				onStore(thisStore, name, item, compile);
			}
			return item;
		}

		var storeNames = storeName + "s";

		$views[storeNames] = theStore;
		jsvStores[storeName] = storeSettings;
	}

	//==============
	// renderContent
	//==============

	function renderContent(data, context, noIteration, parentView, key, onRender) {
		// Render template against data as a tree of subviews (nested rendered template instances), or as a string (top-level template).
		// If the data is the parent view, treat as noIteration, re-render with the same data context.
		var i, l, dataItem, newView, childView, itemResult, swapContent, tagCtx, contentTmpl, tag_, outerOnRender, tmplName, tmpl,
			self = this,
			allowDataLink = !self.attr || self.attr === htmlStr,
			result = "";
		if (!!context === context) {
			noIteration = context; // passing boolean as second param - noIteration
			context = undefined;
		}

		if (key === true) {
			swapContent = true;
			key = 0;
		}
		if (self.tag) {
			// This is a call from renderTag or tagCtx.render(...)
			tagCtx = self;
			self = self.tag;
			tag_ = self._;
			tmplName = self.tagName;
			tmpl = tag_.tmpl || tagCtx.tmpl;
			context = extendCtx(context, self.ctx);
			contentTmpl = tagCtx.content; // The wrapped content - to be added to views, below
			if (tagCtx.props.link === false) {
				// link=false setting on block tag
				// We will override inherited value of link by the explicit setting link=false taken from props
				// The child views of an unlinked view are also unlinked. So setting child back to true will not have any effect.
				context = context || {};
				context.link = false;
			}
			parentView = parentView || tagCtx.view;
			data = arguments.length ? data : parentView;
		} else {
			tmpl = self.jquery && (self[0] || error('Unknown template: "' + self.selector + '"')) // This is a call from $(selector).render
				|| self;
		}
		if (tmpl) {
			if (!parentView && data && data._is === "view") {
				parentView = data; // When passing in a view to render or link (and not passing in a parent view) use the passed in view as parentView
			}
			if (parentView) {
				contentTmpl = contentTmpl || parentView.content; // The wrapped content - to be added as #content property on views, below
				onRender = onRender || parentView._.onRender;
				if (data === parentView) {
					// Inherit the data from the parent view.
					// This may be the contents of an {{if}} block
					data = parentView.data;
				}
				context = extendCtx(context, parentView.ctx);
			}
			if (!parentView || parentView.data === undefined) {
				(context = context || {}).root = data; // Provide ~root as shortcut to top-level data.
			}

			// Set additional context on views created here, (as modified context inherited from the parent, and to be inherited by child views)
			// Note: If no jQuery, $extend does not support chained copies - so limit extend() to two parameters

			if (!tmpl.fn) {
				tmpl = $templates[tmpl] || $templates(tmpl);
			}

			if (tmpl) {
				onRender = (context && context.link) !== false && allowDataLink && onRender;
				// If link===false, do not call onRender, so no data-linking marker nodes
				outerOnRender = onRender;
				if (onRender === true) {
					// Used by view.refresh(). Don't create a new wrapper view.
					outerOnRender = undefined;
					onRender = parentView._.onRender;
				}
				context = tmpl.helpers
					? extendCtx(tmpl.helpers, context)
					: context;
				if ($.isArray(data) && !noIteration) {
					// Create a view for the array, whose child views correspond to each data item. (Note: if key and parentView are passed in
					// along with parent view, treat as insert -e.g. from view.addViews - so parentView is already the view item for array)
					newView = swapContent
						? parentView :
						(key !== undefined && parentView) || View(context, "array", parentView, data, tmpl, key, contentTmpl, onRender);
					for (i = 0, l = data.length; i < l; i++) {
						// Create a view for each data item.
						dataItem = data[i];
						childView = View(context, "item", newView, dataItem, tmpl, (key || 0) + i, contentTmpl, onRender);
						itemResult = tmpl.fn(dataItem, childView, $views);
						result += newView._.onRender ? newView._.onRender(itemResult, childView) : itemResult;
					}
				} else {
					// Create a view for singleton data object. The type of the view will be the tag name, e.g. "if" or "myTag" except for
					// "item", "array" and "data" views. A "data" view is from programatic render(object) against a 'singleton'.
					newView = swapContent ? parentView : View(context, tmplName || "data", parentView, data, tmpl, key, contentTmpl, onRender);
					if (tag_ && !self.flow) {
						newView.tag = self;
					}
					result += tmpl.fn(data, newView, $views);
				}
				return outerOnRender ? outerOnRender(result, newView) : result;
			}
		}
		return "";
	}

	//===========================
	// Build and compile template
	//===========================

	// Generate a reusable function that will serve to render a template against data
	// (Compile AST then build template function)

	function error(message) {
		throw new $viewsSub.Err(message);
	}

	function syntaxError(message) {
		error("Syntax error\n" + message);
	}

	function tmplFn(markup, tmpl, isLinkExpr, convertBack) {
		// Compile markup to AST (abtract syntax tree) then build the template function code from the AST nodes
		// Used for compiling templates, and also by JsViews to build functions for data link expressions

		//==== nested functions ====
		function pushprecedingContent(shift) {
			shift -= loc;
			if (shift) {
				content.push(markup.substr(loc, shift).replace(rNewLine, "\\n"));
			}
		}

		function blockTagCheck(tagName) {
			tagName && syntaxError('Unmatched or missing tag: "{{/' + tagName + '}}" in template:\n' + markup);
		}

		function parseTag(all, bind, tagName, converter, colon, html, comment, codeTag, params, slash, closeBlock, index) {

			//    bind         tag        converter colon html     comment            code      params            slash   closeBlock
			// /{(\^)?{(?:(?:(\w+(?=[\/\s}]))|(?:(\w+)?(:)|(>)|!--((?:[^-]|-(?!-))*)--|(\*)))\s*((?:[^}]|}(?!}))*?)(\/)?|(?:\/(\w+)))}}/g
			// Build abstract syntax tree (AST): [tagName, converter, params, content, hash, bindings, contentMarkup]
			if (html) {
				colon = ":";
				converter = htmlStr;
			}
			slash = slash || isLinkExpr;
			var noError, current0,
				pathBindings = bind && [],
				code = "",
				hash = "",
				passedCtx = "",
				// Block tag if not self-closing and not {{:}} or {{>}} (special case) and not a data-link expression
				block = !slash && !colon && !comment;

			//==== nested helper function ====
			tagName = tagName || (params = params || "#data", colon); // {{:}} is equivalent to {{:#data}}
			pushprecedingContent(index);
			loc = index + all.length; // location marker - parsed up to here
			if (codeTag) {
				if (allowCode) {
					content.push(["*", "\n" + params.replace(rUnescapeQuotes, "$1") + "\n"]);
				}
			} else if (tagName) {
				if (tagName === "else") {
					if (rTestElseIf.test(params)) {
						syntaxError('for "{{else if expr}}" use "{{else expr}}"');
					}
					pathBindings = current[6];
					current[7] = markup.substring(current[7], index); // contentMarkup for block tag
					current = stack.pop();
					content = current[3];
					block = true;
				}
				if (params) {
					// remove newlines from the params string, to avoid compiled code errors for unterminated strings
					params = params.replace(rNewLine, " ");
					code = parseParams(params, pathBindings, tmpl)
						.replace(rBuildHash, function (all, isCtx, keyValue) {
							if (isCtx) {
								passedCtx += keyValue + ",";
							} else {
								hash += keyValue + ",";
							}
							hasHandlers = hasHandlers || rHasHandlers.test(keyValue.split(":")[0]);
							return "";
						});
				}
				hash = hash.slice(0, -1);
				code = code.slice(0, -1);
				noError = hash && (hash.indexOf("noerror:true") + 1) && hash || "";

				newNode = [
						tagName,
						converter || !!convertBack || hasHandlers || "",
						code,
						block && [],
						'\n\tparams:"' + params + '",\n\tprops:{' + hash + "}"
							+ (passedCtx ? ",ctx:{" + passedCtx.slice(0, -1) + "}" : ""),
						noError,
						pathBindings || 0
				];
				content.push(newNode);
				if (block) {
					stack.push(current);
					current = newNode;
					current[7] = loc; // Store current location of open tag, to be able to add contentMarkup when we reach closing tag
				}
			} else if (closeBlock) {
				current0 = current[0];
				blockTagCheck(closeBlock !== current0 && current0 !== "else" && closeBlock);
				current[7] = markup.substring(current[7], index); // contentMarkup for block tag
				current = stack.pop();
			}
			blockTagCheck(!current && closeBlock);
			content = current[3];
		}
		//==== /end of nested functions ====

		var newNode, hasHandlers,
			allowCode = tmpl && tmpl.allowCode,
			astTop = [],
			loc = 0,
			stack = [],
			content = astTop,
			current = [, , , astTop];

		//TODO	result = tmplFnsCache[markup]; // Only cache if template is not named and markup length < ...,
		//and there are no bindings or subtemplates?? Consider standard optimization for data-link="a.b.c"
		//		if (result) {
		//			tmpl.fn = result;
		//		} else {

		//		result = markup;

		blockTagCheck(stack[0] && stack[0][3].pop()[0]);
		// Build the AST (abstract syntax tree) under astTop
		markup.replace(rTag, parseTag);

		pushprecedingContent(markup.length);

		if (loc = astTop[astTop.length - 1]) {
			blockTagCheck("" + loc !== loc && (+loc[7] === loc[7]) && loc[0]);
		}
		//			result = tmplFnsCache[markup] = buildCode(astTop, tmpl);
		//		}
		return buildCode(astTop, isLinkExpr ? markup : tmpl, isLinkExpr);
	}

	function buildCode(ast, tmpl, isLinkExpr) {
		// Build the template function code from the AST nodes, and set as property on the passed-in template object
		// Used for compiling templates, and also by JsViews to build functions for data link expressions
		var i, node, tagName, converter, params, hash, hasTag, hasEncoder, getsVal, hasCnvt, useCnvt, tmplBindings, pathBindings,
			nestedTmpls, tmplName, nestedTmpl, tagAndElses, content, markup, nextIsElse, oldCode, isElse, isGetVal, prm, tagCtxFn,
			tmplBindingKey = 0,
			code = "",
			noError = "",
			tmplOptions = {},
			l = ast.length;

		if ("" + tmpl === tmpl) {
			tmplName = isLinkExpr ? 'data-link="' + tmpl.replace(rNewLine, " ").slice(1, -1) + '"' : tmpl;
			tmpl = 0;
		} else {
			tmplName = tmpl.tmplName || "unnamed";
			if (tmpl.allowCode) {
				tmplOptions.allowCode = true;
			}
			if (tmpl.debug) {
				tmplOptions.debug = true;
			}
			tmplBindings = tmpl.bnds;
			nestedTmpls = tmpl.tmpls;
		}
		for (i = 0; i < l; i++) {
			// AST nodes: [tagName, converter, params, content, hash, noError, pathBindings, contentMarkup, link]
			node = ast[i];

			// Add newline for each callout to t() c() etc. and each markup string
			if ("" + node === node) {
				// a markup string to be inserted
				code += '\nret+="' + node + '";';
			} else {
				// a compiled tag expression to be inserted
				tagName = node[0];
				if (tagName === "*") {
					// Code tag: {{* }}
					code += "" + node[1];
				} else {
					converter = node[1];
					params = node[2];
					content = node[3];
					hash = node[4];
					noError = node[5];
					markup = node[7];

					if (!(isElse = tagName === "else")) {
						tmplBindingKey = 0;
						if (tmplBindings && (pathBindings = node[6])) { // Array of paths, or false if not data-bound
							tmplBindingKey = tmplBindings.push(pathBindings);
						}
					}
					if (isGetVal = tagName === ":") {
						if (converter) {
							tagName = converter === htmlStr ? ">" : converter + tagName;
						}
						if (noError) {
							// If the tag includes noerror=true, we will do a try catch around expressions for named or unnamed parameters
							// passed to the tag, and return the empty string for each expression if it throws during evaluation
							//TODO This does not work for general case - supporting noError on multiple expressions, e.g. tag args and properties.
							//Consider replacing with try<a.b.c(p,q) + a.d, xxx> and return the value of the expression a.b.c(p,q) + a.d, or, if it throws, return xxx||'' (rather than always the empty string)
							prm = "prm" + i;
							noError = "try{var " + prm + "=[" + params + "][0];}catch(e){" + prm + '="";}\n';
							params = prm;
						}
					} else {
						if (content) {
							// Create template object for nested template
							nestedTmpl = TmplObject(markup, tmplOptions);
							nestedTmpl.tmplName = tmplName + "/" + tagName;
							// Compile to AST and then to compiled function
							buildCode(content, nestedTmpl);
							nestedTmpls.push(nestedTmpl);
						}

						if (!isElse) {
							// This is not an else tag.
							tagAndElses = tagName;
							// Switch to a new code string for this bound tag (and its elses, if it has any) - for returning the tagCtxs array
							oldCode = code;
							code = "";
						}
						nextIsElse = ast[i + 1];
						nextIsElse = nextIsElse && nextIsElse[0] === "else";
					}

					hash += ",\n\targs:[" + params + "]}";

					if (isGetVal && (pathBindings || converter && converter !== htmlStr)) {
						// For convertVal we need a compiled function to return the new tagCtx(s)
						tagCtxFn = new Function("data,view,j,u", " // "
									+ tmplName + " " + tmplBindingKey + " " + tagName + "\n" + noError + "return {" + hash + ";");
						tagCtxFn.paths = pathBindings;
						tagCtxFn._ctxs = tagName;
						if (isLinkExpr) {
							return tagCtxFn;
						}
						useCnvt = 1;
					}

					code += (isGetVal
						? "\n" + (pathBindings ? "" : noError) + (isLinkExpr ? "return " : "ret+=") + (useCnvt // Call _cnvt if there is a converter: {{cnvt: ... }} or {^{cnvt: ... }}
							? (useCnvt = 0, hasCnvt = true, 'c("' + converter + '",view,' + (pathBindings
								? ((tmplBindings[tmplBindingKey - 1] = tagCtxFn), tmplBindingKey) // Store the compiled tagCtxFn in tmpl.bnds, and pass the key to convertVal()
								: "{" + hash) + ");")
							: tagName === ">"
								? (hasEncoder = true, "h(" + params + ");")
								: (getsVal = true, "(v=" + params + ")!=" + (isLinkExpr ? "=" : "") + 'u?v:"";') // Strict equality just for data-link="title{:expr}" so expr=null will remove title attribute
						)
						: (hasTag = true, "{view:view,tmpl:" // Add this tagCtx to the compiled code for the tagCtxs to be passed to renderTag()
							+ (content ? nestedTmpls.length : "0") + "," // For block tags, pass in the key (nestedTmpls.length) to the nested content template
							+ hash + ","));

					if (tagAndElses && !nextIsElse) {
						code = "[" + code.slice(0, -1) + "]"; // This is a data-link expression or the last {{else}} of an inline bound tag. We complete the code for returning the tagCtxs array
						if (isLinkExpr || pathBindings) {
							// This is a bound tag (data-link expression or inline bound tag {^{tag ...}}) so we store a compiled tagCtxs function in tmp.bnds
							code = new Function("data,view,j,u", " // " + tmplName + " " + tmplBindingKey + " " + tagAndElses + "\nreturn " + code + ";");
							if (pathBindings) {
								(tmplBindings[tmplBindingKey - 1] = code).paths = pathBindings;
							}
							code._ctxs = tagName;
							if (isLinkExpr) {
								return code; // For a data-link expression we return the compiled tagCtxs function
							}
						}

						// This is the last {{else}} for an inline tag.
						// For a bound tag, pass the tagCtxs fn lookup key to renderTag.
						// For an unbound tag, include the code directly for evaluating tagCtxs array
						code = oldCode + '\nret+=t("' + tagAndElses + '",view,this,' + (tmplBindingKey || code) + ");";
						pathBindings = 0;
						tagAndElses = 0;
					}
				}
			}
		}
		// Include only the var references that are needed in the code
		code = "// " + tmplName
			+ "\nvar j=j||" + (jQuery ? "jQuery." : "js") + "views"
			+ (getsVal ? ",v" : "")                      // gets value
			+ (hasTag ? ",t=j._tag" : "")                // has tag
			+ (hasCnvt ? ",c=j._cnvt" : "")              // converter
			+ (hasEncoder ? ",h=j.converters.html" : "") // html converter
			+ (isLinkExpr ? ";\n" : ',ret="";\n')
			+ ($viewsSettings.tryCatch ? "try{\n" : "")
			+ (tmplOptions.debug ? "debugger;" : "")
			+ code + (isLinkExpr ? "\n" : "\nreturn ret;\n")
			+ ($viewsSettings.tryCatch ? "\n}catch(e){return j._err(e);}" : "");
		try {
			code = new Function("data,view,j,u", code);
		} catch (e) {
			syntaxError("Compiled template code:\n\n" + code, e);
		}
		if (tmpl) {
			tmpl.fn = code;
		}
		return code;
	}

	function parseParams(params, bindings, tmpl) {

		//function pushBindings() { // Consider structured path bindings
		//	if (bindings) {
		//		named ? bindings[named] = bindings.pop(): bindings.push(list = []);
		//	}
		//}

		function parseTokens(all, lftPrn0, lftPrn, bound, path, operator, err, eq, path2, prn, comma, lftPrn2, apos, quot, rtPrn, rtPrnDot, prn2, space, index, full) {
			//rParams = /(\()(?=\s*\()|(?:([([])\s*)?(?:(\^?)(!*?[#~]?[\w$.^]+)?\s*((\+\+|--)|\+|-|&&|\|\||===|!==|==|!=|<=|>=|[<>%*:?\/]|(=))\s*|(!*?[#~]?[\w$.^]+)([([])?)|(,\s*)|(\(?)\\?(?:(')|("))|(?:\s*(([)\]])(?=\s*\.|\s*\^)|[)\]])([([]?))|(\s+)/g,
			//          lftPrn0        lftPrn        bound            path    operator err                                                eq             path2       prn    comma   lftPrn2   apos quot      rtPrn rtPrnDot                        prn2      space
			// (left paren? followed by (path? followed by operator) or (path followed by paren?)) or comma or apos or quot or right paren or space
			var expr;
			operator = operator || "";
			lftPrn = lftPrn || lftPrn0 || lftPrn2;
			path = path || path2;
			prn = prn || prn2 || "";

			function parsePath(allPath, not, object, helper, view, viewProperty, pathTokens, leafToken) {
				// rPath = /^(?:null|true|false|\d[\d.]*|(!*?)([\w$]+|\.|~([\w$]+)|#(view|([\w$]+))?)([\w$.^]*?)(?:[.[^]([\w$]+)\]?)?)$/g,
				//                                        none   object     helper    view  viewProperty pathTokens      leafToken
				if (object) {
					if (bindings) {
						if (named === "linkTo") {
							bindto = bindings._jsvto = bindings._jsvto || [];
							bindto.push(path);
						}
						if (!named || boundName) {
							bindings.push(path.slice(not.length)); // Add path binding for paths on props and args,
							//							list.push(path);
						}
					}
					if (object !== ".") {
						var ret = (helper
								? 'view.hlp("' + helper + '")'
								: view
									? "view"
									: "data")
							+ (leafToken
								? (viewProperty
									? "." + viewProperty
									: helper
										? ""
										: (view ? "" : "." + object)
									) + (pathTokens || "")
								: (leafToken = helper ? "" : view ? viewProperty || "" : object, ""));

						ret = ret + (leafToken ? "." + leafToken : "");

						return not + (ret.slice(0, 9) === "view.data"
							? ret.slice(5) // convert #view.data... to data...
							: ret);
					}
				}
				return allPath;
			}

			if (err && !aposed && !quoted) {
				syntaxError(params);
			} else {
				if (bindings && rtPrnDot && !aposed && !quoted) {
					// This is a binding to a path in which an object is returned by a helper/data function/expression, e.g. foo()^x.y or (a?b:c)^x.y
					// We create a compiled function to get the object instance (which will be called when the dependent data of the subexpression changes, to return the new object, and trigger re-binding of the subsequent path)
					if (!named || boundName || bindto) {
						expr = pathStart[parenDepth];
						if (full.length - 1 > index - expr) { // We need to compile a subexpression
							expr = full.slice(expr, index + 1);
							rtPrnDot = delimOpenChar1 + ":" + expr + delimCloseChar0; // The parameter or function subexpression
							//TODO Optimize along the lines of:
							//var paths = [];
							//rtPrnDot = tmplLinks[rtPrnDot] = tmplLinks[rtPrnDot] || tmplFn(delimOpenChar0 + rtPrnDot + delimCloseChar1, tmpl, true, paths); // Compile the expression (or use cached copy already in tmpl.links)
							//rtPrnDot.paths = rtPrnDot.paths || paths;

							rtPrnDot = tmplLinks[rtPrnDot] = tmplLinks[rtPrnDot] || tmplFn(delimOpenChar0 + rtPrnDot + delimCloseChar1, tmpl, true); // Compile the expression (or use cached copy already in tmpl.links)
							if (!rtPrnDot.paths) {
								parseParams(expr, rtPrnDot.paths = [], tmpl);
							}
							(bindto || bindings).push({ _jsvOb: rtPrnDot }); // Insert special object for in path bindings, to be used for binding the compiled sub expression ()
							//list.push({_jsvOb: rtPrnDot});
						}
					}
				}
				return (aposed
					// within single-quoted string
					? (aposed = !apos, (aposed ? all : '"'))
					: quoted
					// within double-quoted string
						? (quoted = !quot, (quoted ? all : '"'))
						:
					(
						(lftPrn
								? (parenDepth++, pathStart[parenDepth] = index++, lftPrn)
								: "")
						+ (space
							? (parenDepth
								? ""
								//: (pushBindings(), named
								//	: ",")
								: named
									? (named = boundName = bindto = false, "\b")
									: ","
							)
							: eq
					// named param
					// Insert backspace \b (\x08) as separator for named params, used subsequently by rBuildHash
								? (parenDepth && syntaxError(params), named = path, boundName = bound, /*pushBindings(),*/ '\b' + path + ':')
								: path
					// path
									? (path.split("^").join(".").replace(rPath, parsePath)
										+ (prn
											? (fnCall[++parenDepth] = true, path.charAt(0) !== "." && (pathStart[parenDepth] = index), prn)
											: operator)
									)
									: operator
										? operator
										: rtPrn
					// function
											? ((fnCall[parenDepth--] = false, rtPrn)
												+ (prn
													? (fnCall[++parenDepth] = true, prn)
													: "")
											)
											: comma
												? (fnCall[parenDepth] || syntaxError(params), ",") // We don't allow top-level literal arrays or objects
												: lftPrn0
													? ""
													: (aposed = apos, quoted = quot, '"')
					))
				);
			}
		}

		var named, bindto, boundName, // list,
			tmplLinks = tmpl.links,
			fnCall = {},
			pathStart = { 0: -1 },
			parenDepth = 0,
			quoted = false, // boolean for string content in double quotes
			aposed = false; // or in single quotes

		//pushBindings();

		return (params + " ")
			.replace(/\)\^/g, ").") // Treat "...foo()^bar..." as equivalent to "...foo().bar..."
								//since preceding computed observables in the path will always be updated if their dependencies change
			.replace(rParams, parseTokens);
	}

	//==========
	// Utilities
	//==========

	// Merge objects, in particular contexts which inherit from parent contexts
	function extendCtx(context, parentContext) {
		// Return copy of parentContext, unless context is defined and is different, in which case return a new merged context
		// If neither context nor parentContext are defined, return undefined
		return context && context !== parentContext
			? (parentContext
				? $extend($extend({}, parentContext), context)
				: context)
			: parentContext && $extend({}, parentContext);
	}

	// Get character entity for HTML and Attribute encoding
	function getCharEntity(ch) {
		return charEntities[ch] || (charEntities[ch] = "&#" + ch.charCodeAt(0) + ";");
	}

	//========================== Initialize ==========================

	for (jsvStoreName in jsvStores) {
		registerStore(jsvStoreName, jsvStores[jsvStoreName]);
	}

	var $observable,
		$templates = $views.templates,
		$converters = $views.converters,
		$helpers = $views.helpers,
		$tags = $views.tags,
		$viewsSub = $views.sub,
		$isFunction = $viewsSub.isFn,
		$viewsSettings = $views.settings;

	if (jQuery) {
		////////////////////////////////////////////////////////////////////////////////////////////////
		// jQuery is loaded, so make $ the jQuery object
		$ = jQuery;
		$.fn.render = renderContent;
		if ($observable = $.observable) {
			$extend($viewsSub, $observable.sub); // jquery.observable.js was loaded before jsrender.js
			delete $observable.sub;
		}
	} else {
		////////////////////////////////////////////////////////////////////////////////////////////////
		// jQuery is not loaded.

		$ = global.jsviews = {};

		$.isArray = Array && Array.isArray || function (obj) {
			return Object.prototype.toString.call(obj) === "[object Array]";
		};

		//	//========================== Future Node.js support ==========================
		//	if ((nodeJsModule = global.module) && nodeJsModule.exports) {
		//		nodeJsModule.exports = $;
		//	}
	}

	$.render = $render;
	$.views = $views;
	$.templates = $templates = $views.templates;

	$viewsSettings({
		debugMode: dbgMode,
		delimiters: $viewsDelimiters,
		_dbgMode: true,
		tryCatch: true
	});

	//========================== Register tags ==========================

	$tags({
		"else": function () { }, // Does nothing but ensures {{else}} tags are recognized as valid
		"if": {
			render: function (val) {
				// This function is called once for {{if}} and once for each {{else}}.
				// We will use the tag.rendering object for carrying rendering state across the calls.
				// If not done (a previous block has not been rendered), look at expression for this block and render the block if expression is truthy
				// Otherwise return ""
				var self = this,
					ret = (self.rendering.done || !val && (arguments.length || !self.tagCtx.index))
						? ""
						: (self.rendering.done = true, self.selected = self.tagCtx.index,
							// Test is satisfied, so render content on current context. We call tagCtx.render() rather than return undefined
							// (which would also render the tmpl/content on the current context but would iterate if it is an array)
							self.tagCtx.render(self.tagCtx.view, true)); // no arg, so renders against parentView.data
				return ret;
			},
			onUpdate: function (ev, eventArgs, tagCtxs) {
				var tci, prevArg, different;
				for (tci = 0; (prevArg = this.tagCtxs[tci]) && prevArg.args.length; tci++) {
					prevArg = prevArg.args[0];
					different = !prevArg !== !tagCtxs[tci].args[0];
					if ((!this.convert && !!prevArg) || different) {
						return different;
						// If there is no converter, and newArg and prevArg are both truthy, return false to cancel update. (Even if values on later elses are different, we still don't want to update, since rendered output would be unchanged)
						// If newArg and prevArg are different, return true, to update
						// If newArg and prevArg are both falsey, move to the next {{else ...}}
					}
				}
				// Boolean value of all args are unchanged (falsey), so return false to cancel update
				return false;
			},
			flow: true
		},
		"for": {
			render: function (val) {
				// This function is called once for {{for}} and once for each {{else}}.
				// We will use the tag.rendering object for carrying rendering state across the calls.
				var finalElse,
					self = this,
					tagCtx = self.tagCtx,
					result = "",
					done = 0;

				if (!self.rendering.done) {
					if (finalElse = !arguments.length) {
						val = tagCtx.view.data; // For the final else, defaults to current data without iteration.
					}
					if (val !== undefined) {
						result += tagCtx.render(val, finalElse); // Iterates except on final else, if data is an array. (Use {{include}} to compose templates without array iteration)
						done += $.isArray(val) ? val.length : 1;
					}
					if (self.rendering.done = done) {
						self.selected = tagCtx.index;
					}
					// If nothing was rendered we will look at the next {{else}}. Otherwise, we are done.
				}
				return result;
			},
			flow: true,
			autoBind: true
		},
		include: {
			flow: true,
			autoBind: true
		},
		"*": {
			// {{* code... }} - Ignored if template.allowCode is false. Otherwise include code in compiled template
			render: retVal,
			flow: true
		}
	});

	function getTargetProps(source) {
		// this pointer is theMap - which has tagCtx.props too
		// arguments: tagCtx.args.
		var key, prop,
			props = [];

		if (typeof source === "object") {
			for (key in source) {
				prop = source[key];
				if (!prop || !prop.toJSON || prop.toJSON()) {
					if (!$isFunction(prop)) {
						props.push({ key: key, prop: source[key] });
					}
				}
			}
		}
		return props;
	}

	$tags({
		props: $extend($extend({}, $tags["for"]),
			DataMap(getTargetProps)
		)
	});

	$tags.props.autoBind = true;

	//========================== Register converters ==========================

	$converters({
		html: function (text) {
			// HTML encode: Replace < > & and ' and " by corresponding entities.
			return text != undefined ? String(text).replace(rHtmlEncode, getCharEntity) : ""; // null and undefined return ""
		},
		attr: function (text) {
			// Attribute encode: Replace < > & ' and " by corresponding entities.
			return text != undefined ? String(text).replace(rAttrEncode, getCharEntity) : text === null ? text : ""; // null returns null, e.g. to remove attribute. undefined returns ""
		},
		url: function (text) {
			// URL encoding helper.
			return text != undefined ? encodeURI(String(text)) : text === null ? text : ""; // null returns null, e.g. to remove attribute. undefined returns ""
		}
	});

	//========================== Define default delimiters ==========================
	$viewsDelimiters();

})(this, this.jQuery);

//#endregion End jsrender

//#region parseJSON extension

/*!
* http://erraticdev.blogspot.com/2010/12/converting-dates-in-json-strings-using.html
* jQuery.parseJSON() extension (supports ISO & Asp.net date conversion)
*
* Version 1.0 (13 Jan 2011)
*
* Copyright (c) 2011 Robert Koritnik
* Licensed under the terms of the MIT license
* http://www.opensource.org/licenses/mit-license.php
*/
(function ($) {

	// JSON RegExp
	var rvalidchars = /^[\],:{}\s]*$/;
	var rvalidescape = /\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g;
	var rvalidtokens = /"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g;
	var rvalidbraces = /(?:^|:|,)(?:\s*\[)+/g;
	var dateISO = /\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:[.,]\d+)?Z/i;
	var dateNet = /\/Date\((\d+)(?:-\d+)?\)\//i;

	// replacer RegExp
	var replaceISO = /"(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})(?:[.,](\d+))?Z"/i;
	var replaceNet = /"\\\/Date\((\d+)(?:-\d+)?\)\\\/"/i;

	// determine JSON native support
	var nativeJSON = (window.JSON && window.JSON.parse) ? true : false;
	var extendedJSON = nativeJSON && window.JSON.parse('{"x":9}', function (k, v) { return "Y"; }) === "Y";

	var jsonDateConverter = function (key, value) {
		if (typeof (value) === "string") {
			if (dateISO.test(value)) {
				return new Date(value);
			}
			if (dateNet.test(value)) {
				return new Date(parseInt(dateNet.exec(value)[1], 10));
			}
		}
		return value;
	};

	$.extend({
		parseJSON: function (data, convertDates) {
			/// <summary>Takes a well-formed JSON string and returns the resulting JavaScript object.</summary>
			/// <param name="data" type="String">The JSON string to parse.</param>
			/// <param name="convertDates" optional="true" type="Boolean">Set to true when you want ISO/Asp.net dates to be auto-converted to dates.</param>
			if (typeof data !== "string" || !data) {
				return null;
			}

			// Make sure leading/trailing whitespace is removed (IE can't handle it)
			data = $.trim(data);

			// Make sure the incoming data is actual JSON
			// Logic borrowed from http://json.org/json2.js
			if (rvalidchars.test(data
								.replace(rvalidescape, "@")
								.replace(rvalidtokens, "]")
								.replace(rvalidbraces, ""))) {
				// Try to use the native JSON parser
				if (extendedJSON || (nativeJSON && convertDates !== true)) {
					return window.JSON.parse(data, convertDates === true ? jsonDateConverter : undefined);
				}
				else {
					data = convertDates === true ?
												data.replace(replaceISO, "new Date(parseInt('$1',10),parseInt('$2',10)-1,parseInt('$3',10),parseInt('$4',10),parseInt('$5',10),parseInt('$6',10),(function(s){return parseInt(s,10)||0;})('$7'))")
														.replace(replaceNet, "new Date($1)") :
												data;
					return (new Function("return " + data))();
				}
			} else {
				$.error("Invalid JSON: " + data);
			}
		}
	});
})(jQuery);

//#endregion parseJSON extension

//#region jsTree

// NOTE: Three customizations were made to jsTree - these must be re-applied when updating the library. Look for '[Roger]'

/*globals jQuery, define, exports, require, window, document */
(function (factory) {
	"use strict";
	if (typeof define === 'function' && define.amd) {
		define(['jquery'], factory);
	}
	else if (typeof exports === 'object') {
		factory(require('jquery'));
	}
	else {
		factory(jQuery);
	}
}(function ($, undefined) {
	"use strict";
	/*
	 * jsTree 3.0.0
	 * http://jstree.com/
	 *
	 * Copyright (c) 2013 Ivan Bozhanov (http://vakata.com)
	 *
	 * Licensed same as jquery - under the terms of the MIT License
	 *   http://www.opensource.org/licenses/mit-license.php
	 */
	/*!
	 * if using jslint please allow for the jQuery global and use following options: 
	 * jslint: browser: true, ass: true, bitwise: true, continue: true, nomen: true, plusplus: true, regexp: true, unparam: true, todo: true, white: true
	 */

	// prevent another load? maybe there is a better way?
	if ($.jstree) {
		return;
	}

	/**
	 * ### jsTree core functionality
	 */

	// internal variables
	var instance_counter = 0,
		ccp_node = false,
		ccp_mode = false,
		ccp_inst = false,
		themes_loaded = [],
		src = $('script:last').attr('src'),
		_d = document, _node = _d.createElement('LI'), _temp1, _temp2;

	_node.setAttribute('role', 'treeitem');
	_temp1 = _d.createElement('I');
	_temp1.className = 'jstree-icon jstree-ocl';
	_node.appendChild(_temp1);
	_temp1 = _d.createElement('A');
	_temp1.className = 'jstree-anchor';
	_temp1.setAttribute('href', '#');
	_temp2 = _d.createElement('I');
	_temp2.className = 'jstree-icon jstree-themeicon';
	_temp1.appendChild(_temp2);
	_node.appendChild(_temp1);
	_temp1 = _temp2 = null;


	/**
	 * holds all jstree related functions and variables, including the actual class and methods to create, access and manipulate instances.
	 * @name $.jstree
	 */
	$.jstree = {
		/** 
		 * specifies the jstree version in use
		 * @name $.jstree.version
		 */
		version: '3.0.0',
		/**
		 * holds all the default options used when creating new instances
		 * @name $.jstree.defaults
		 */
		defaults: {
			/**
			 * configure which plugins will be active on an instance. Should be an array of strings, where each element is a plugin name. The default is `[]`
			 * @name $.jstree.defaults.plugins
			 */
			plugins: []
		},
		/**
		 * stores all loaded jstree plugins (used internally)
		 * @name $.jstree.plugins
		 */
		plugins: {},
		path: src && src.indexOf('/') !== -1 ? src.replace(/\/[^\/]+$/, '') : '',
		idregex: /[\\:&'".,=\- \/]/g
	};
	/**
	 * creates a jstree instance
	 * @name $.jstree.create(el [, options])
	 * @param {DOMElement|jQuery|String} el the element to create the instance on, can be jQuery extended or a selector
	 * @param {Object} options options for this instance (extends `$.jstree.defaults`)
	 * @return {jsTree} the new instance
	 */
	$.jstree.create = function (el, options) {
		var tmp = new $.jstree.core(++instance_counter),
			opt = options;
		options = $.extend(true, {}, $.jstree.defaults, options);
		if (opt && opt.plugins) {
			options.plugins = opt.plugins;
		}
		$.each(options.plugins, function (i, k) {
			if (i !== 'core') {
				tmp = tmp.plugin(k, options[k]);
			}
		});
		tmp.init(el, options);
		return tmp;
	};
	/**
	 * the jstree class constructor, used only internally
	 * @private
	 * @name $.jstree.core(id)
	 * @param {Number} id this instance's index
	 */
	$.jstree.core = function (id) {
		this._id = id;
		this._cnt = 0;
		this._data = {
			core: {
				themes: {
					name: false,
					dots: false,
					icons: false
				},
				selected: [],
				last_error: {}
			}
		};
	};
	/**
	 * get a reference to an existing instance
	 *
	 * __Examples__
	 *
	 *	// provided a container with an ID of "tree", and a nested node with an ID of "branch"
	 *	// all of there will return the same instance
	 *	$.jstree.reference('tree');
	 *	$.jstree.reference('#tree');
	 *	$.jstree.reference($('#tree'));
	 *	$.jstree.reference(document.getElementByID('tree'));
	 *	$.jstree.reference('branch');
	 *	$.jstree.reference('#branch');
	 *	$.jstree.reference($('#branch'));
	 *	$.jstree.reference(document.getElementByID('branch'));
	 *
	 * @name $.jstree.reference(needle)
	 * @param {DOMElement|jQuery|String} needle
	 * @return {jsTree|null} the instance or `null` if not found
	 */
	$.jstree.reference = function (needle) {
		var tmp = null,
			obj = null;
		if (needle && needle.id) { needle = needle.id; }

		if (!obj || !obj.length) {
			try { obj = $(needle); } catch (ignore) { }
		}
		if (!obj || !obj.length) {
			try { obj = $('#' + needle.replace($.jstree.idregex, '\\$&')); } catch (ignore) { }
		}
		if (obj && obj.length && (obj = obj.closest('.jstree')).length && (obj = obj.data('jstree'))) {
			tmp = obj;
		}
		else {
			$('.jstree').each(function () {
				var inst = $(this).data('jstree');
				if (inst && inst._model.data[needle]) {
					tmp = inst;
					return false;
				}
			});
		}
		return tmp;
	};
	/**
	 * Create an instance, get an instance or invoke a command on a instance. 
	 * 
	 * If there is no instance associated with the current node a new one is created and `arg` is used to extend `$.jstree.defaults` for this new instance. There would be no return value (chaining is not broken).
	 * 
	 * If there is an existing instance and `arg` is a string the command specified by `arg` is executed on the instance, with any additional arguments passed to the function. If the function returns a value it will be returned (chaining could break depending on function).
	 * 
	 * If there is an existing instance and `arg` is not a string the instance itself is returned (similar to `$.jstree.reference`).
	 * 
	 * In any other case - nothing is returned and chaining is not broken.
	 *
	 * __Examples__
	 *
	 *	$('#tree1').jstree(); // creates an instance
	 *	$('#tree2').jstree({ plugins : [] }); // create an instance with some options
	 *	$('#tree1').jstree('open_node', '#branch_1'); // call a method on an existing instance, passing additional arguments
	 *	$('#tree2').jstree(); // get an existing instance (or create an instance)
	 *	$('#tree2').jstree(true); // get an existing instance (will not create new instance)
	 *	$('#branch_1').jstree().select_node('#branch_1'); // get an instance (using a nested element and call a method)
	 *
	 * @name $().jstree([arg])
	 * @param {String|Object} arg
	 * @return {Mixed}
	 */
	$.fn.jstree = function (arg) {
		// check for string argument
		var is_method = (typeof arg === 'string'),
			args = Array.prototype.slice.call(arguments, 1),
			result = null;
		this.each(function () {
			// get the instance (if there is one) and method (if it exists)
			var instance = $.jstree.reference(this),
				method = is_method && instance ? instance[arg] : null;
			// if calling a method, and method is available - execute on the instance
			result = is_method && method ?
				method.apply(instance, args) :
				null;
			// if there is no instance and no method is being called - create one
			if (!instance && !is_method && (arg === undefined || $.isPlainObject(arg))) {
				$(this).data('jstree', new $.jstree.create(this, arg));
			}
			// if there is an instance and no method is called - return the instance
			if ((instance && !is_method) || arg === true) {
				result = instance || false;
			}
			// if there was a method call which returned a result - break and return the value
			if (result !== null && result !== undefined) {
				return false;
			}
		});
		// if there was a method call with a valid return value - return that, otherwise continue the chain
		return result !== null && result !== undefined ?
			result : this;
	};
	/**
	 * used to find elements containing an instance
	 *
	 * __Examples__
	 *
	 *	$('div:jstree').each(function () {
	 *		$(this).jstree('destroy');
	 *	});
	 *
	 * @name $(':jstree')
	 * @return {jQuery}
	 */
	$.expr[':'].jstree = $.expr.createPseudo(function (search) {
		return function (a) {
			return $(a).hasClass('jstree') &&
				$(a).data('jstree') !== undefined;
		};
	});

	/**
	 * stores all defaults for the core
	 * @name $.jstree.defaults.core
	 */
	$.jstree.defaults.core = {
		/**
		 * data configuration
		 * 
		 * If left as `false` the HTML inside the jstree container element is used to populate the tree (that should be an unordered list with list items).
		 *
		 * You can also pass in a HTML string or a JSON array here.
		 * 
		 * It is possible to pass in a standard jQuery-like AJAX config and jstree will automatically determine if the response is JSON or HTML and use that to populate the tree. 
		 * In addition to the standard jQuery ajax options here you can suppy functions for `data` and `url`, the functions will be run in the current instance's scope and a param will be passed indicating which node is being loaded, the return value of those functions will be used.
		 * 
		 * The last option is to specify a function, that function will receive the node being loaded as argument and a second param which is a function which should be called with the result.
		 *
		 * __Examples__
		 *
		 *	// AJAX
		 *	$('#tree').jstree({
		 *		'core' : {
		 *			'data' : {
		 *				'url' : '/get/children/',
		 *				'data' : function (node) {
		 *					return { 'id' : node.id };
		 *				}
		 *			}
		 *		});
		 *
		 *	// direct data
		 *	$('#tree').jstree({
		 *		'core' : {
		 *			'data' : [
		 *				'Simple root node',
		 *				{
		 *					'id' : 'node_2',
		 *					'text' : 'Root node with options',
		 *					'state' : { 'opened' : true, 'selected' : true },
		 *					'children' : [ { 'text' : 'Child 1' }, 'Child 2']
		 *				}
		 *			]
		 *		});
		 *	
		 *	// function
		 *	$('#tree').jstree({
		 *		'core' : {
		 *			'data' : function (obj, callback) {
		 *				callback.call(this, ['Root 1', 'Root 2']);
		 *			}
		 *		});
		 * 
		 * @name $.jstree.defaults.core.data
		 */
		data: false,
		/**
		 * configure the various strings used throughout the tree
		 *
		 * You can use an object where the key is the string you need to replace and the value is your replacement.
		 * Another option is to specify a function which will be called with an argument of the needed string and should return the replacement.
		 * If left as `false` no replacement is made.
		 *
		 * __Examples__
		 *
		 *	$('#tree').jstree({
		 *		'core' : {
		 *			'strings' : {
		 *				'Loading...' : 'Please wait ...'
		 *			}
		 *		}
		 *	});
		 *
		 * @name $.jstree.defaults.core.strings
		 */
		strings: false,
		/**
		 * determines what happens when a user tries to modify the structure of the tree
		 * If left as `false` all operations like create, rename, delete, move or copy are prevented.
		 * You can set this to `true` to allow all interactions or use a function to have better control.
		 *
		 * __Examples__
		 *
		 *	$('#tree').jstree({
		 *		'core' : {
		 *			'check_callback' : function (operation, node, node_parent, node_position, more) {
		 *				// operation can be 'create_node', 'rename_node', 'delete_node', 'move_node' or 'copy_node'
		 *				// in case of 'rename_node' node_position is filled with the new node name
		 *				return operation === 'rename_node' ? true : false;
		 *			}
		 *		}
		 *	});
		 * 
		 * @name $.jstree.defaults.core.check_callback
		 */
		check_callback: false,
		/**
		 * a callback called with a single object parameter in the instance's scope when something goes wrong (operation prevented, ajax failed, etc)
		 * @name $.jstree.defaults.core.error
		 */
		error: $.noop,
		/**
		 * the open / close animation duration in milliseconds - set this to `false` to disable the animation (default is `200`)
		 * @name $.jstree.defaults.core.animation
		 */
		animation: 200,
		/**
		 * a boolean indicating if multiple nodes can be selected
		 * @name $.jstree.defaults.core.multiple
		 */
		multiple: true,
		/**
		 * theme configuration object
		 * @name $.jstree.defaults.core.themes
		 */
		themes: {
			/**
			 * the name of the theme to use (if left as `false` the default theme is used)
			 * @name $.jstree.defaults.core.themes.name
			 */
			name: false,
			/**
			 * the URL of the theme's CSS file, leave this as `false` if you have manually included the theme CSS (recommended). You can set this to `true` too which will try to autoload the theme.
			 * @name $.jstree.defaults.core.themes.url
			 */
			url: false,
			/**
			 * the location of all jstree themes - only used if `url` is set to `true`
			 * @name $.jstree.defaults.core.themes.dir
			 */
			dir: false,
			/**
			 * a boolean indicating if connecting dots are shown
			 * @name $.jstree.defaults.core.themes.dots
			 */
			dots: true,
			/**
			 * a boolean indicating if node icons are shown
			 * @name $.jstree.defaults.core.themes.icons
			 */
			icons: true,
			/**
			 * a boolean indicating if the tree background is striped
			 * @name $.jstree.defaults.core.themes.stripes
			 */
			stripes: false,
			/**
			 * a string (or boolean `false`) specifying the theme variant to use (if the theme supports variants)
			 * @name $.jstree.defaults.core.themes.variant
			 */
			variant: false,
			/**
			 * a boolean specifying if a reponsive version of the theme should kick in on smaller screens (if the theme supports it). Defaults to `true`.
			 * @name $.jstree.defaults.core.themes.responsive
			 */
			responsive: true
		},
		/**
		 * if left as `true` all parents of all selected nodes will be opened once the tree loads (so that all selected nodes are visible to the user)
		 * @name $.jstree.defaults.core.expand_selected_onload
		 */
		expand_selected_onload: true
	};
	$.jstree.core.prototype = {
		/**
		 * used to decorate an instance with a plugin. Used internally.
		 * @private
		 * @name plugin(deco [, opts])
		 * @param  {String} deco the plugin to decorate with
		 * @param  {Object} opts options for the plugin
		 * @return {jsTree}
		 */
		plugin: function (deco, opts) {
			var Child = $.jstree.plugins[deco];
			if (Child) {
				this._data[deco] = {};
				Child.prototype = this;
				return new Child(opts, this);
			}
			return this;
		},
		/**
		 * used to decorate an instance with a plugin. Used internally.
		 * @private
		 * @name init(el, optons)
		 * @param {DOMElement|jQuery|String} el the element we are transforming
		 * @param {Object} options options for this instance
		 * @trigger init.jstree, loading.jstree, loaded.jstree, ready.jstree, changed.jstree
		 */
		init: function (el, options) {
			this._model = {
				data: {
					'#': {
						id: '#',
						parent: null,
						parents: [],
						children: [],
						children_d: [],
						state: { loaded: false }
					}
				},
				changed: [],
				force_full_redraw: false,
				redraw_timeout: false,
				default_state: {
					loaded: true,
					opened: false,
					selected: false,
					disabled: false
				}
			};

			this.element = $(el).addClass('jstree jstree-' + this._id);
			this.settings = options;
			this.element.bind("destroyed", $.proxy(this.teardown, this));

			this._data.core.ready = false;
			this._data.core.loaded = false;
			this._data.core.rtl = (this.element.css("direction") === "rtl");
			this.element[this._data.core.rtl ? 'addClass' : 'removeClass']("jstree-rtl");
			this.element.attr('role', 'tree');

			this.bind();
			/**
			 * triggered after all events are bound
			 * @event
			 * @name init.jstree
			 */
			this.trigger("init");

			this._data.core.original_container_html = this.element.find(" > ul > li").clone(true);
			this._data.core.original_container_html
				.find("li").addBack()
				.contents().filter(function () {
					return this.nodeType === 3 && (!this.nodeValue || /^\s+$/.test(this.nodeValue));
				})
				.remove();
			this.element.html("<" + "ul class='jstree-container-ul'><" + "li class='jstree-initial-node jstree-loading jstree-leaf jstree-last'><i class='jstree-icon jstree-ocl'></i><" + "a class='jstree-anchor' href='#'><i class='jstree-icon jstree-themeicon-hidden'></i>" + this.get_string("Loading ...") + "</a></li></ul>");
			this._data.core.li_height = this.get_container_ul().children("li:eq(0)").height() || 18;
			/**
			 * triggered after the loading text is shown and before loading starts
			 * @event
			 * @name loading.jstree
			 */
			this.trigger("loading");
			this.load_node('#');
		},
		/**
		 * destroy an instance
		 * @name destroy()
		 */
		destroy: function () {
			this.element.unbind("destroyed", this.teardown);
			this.teardown();
		},
		/**
		 * part of the destroying of an instance. Used internally.
		 * @private
		 * @name teardown()
		 */
		teardown: function () {
			this.unbind();
			this.element
				.removeClass('jstree')
				.removeData('jstree')
				.find("[class^='jstree']")
					.addBack()
					.attr("class", function () { return this.className.replace(/jstree[^ ]*|$/ig, ''); });
			this.element = null;
		},
		/**
		 * bind all events. Used internally.
		 * @private
		 * @name bind()
		 */
		bind: function () {
			this.element
				.on("dblclick.jstree", function () {
					if (document.selection && document.selection.empty) {
						document.selection.empty();
					}
					else {
						if (window.getSelection) {
							var sel = window.getSelection();
							try {
								sel.removeAllRanges();
								sel.collapse();
							} catch (ignore) { }
						}
					}
				})
				.on("click.jstree", ".jstree-ocl", $.proxy(function (e) {
					this.toggle_node(e.target);
				}, this))
				.on("click.jstree", ".jstree-anchor", $.proxy(function (e) {
					e.preventDefault();
					$(e.currentTarget).focus();
					this.activate_node(e.currentTarget, e);
				}, this))
				.on('keydown.jstree', '.jstree-anchor', $.proxy(function (e) {
					if (e.target.tagName === "INPUT") { return true; }
					var o = null;
					switch (e.which) {
						case 13:
						case 32:
							e.type = "click";
							$(e.currentTarget).trigger(e);
							break;
						case 37:
							e.preventDefault();
							if (this.is_open(e.currentTarget)) {
								this.close_node(e.currentTarget);
							}
							else {
								o = this.get_prev_dom(e.currentTarget);
								if (o && o.length) { o.children('.jstree-anchor').focus(); }
							}
							break;
						case 38:
							e.preventDefault();
							o = this.get_prev_dom(e.currentTarget);
							if (o && o.length) { o.children('.jstree-anchor').focus(); }
							break;
						case 39:
							e.preventDefault();
							if (this.is_closed(e.currentTarget)) {
								this.open_node(e.currentTarget, function (o) { this.get_node(o, true).children('.jstree-anchor').focus(); });
							}
							else {
								o = this.get_next_dom(e.currentTarget);
								if (o && o.length) { o.children('.jstree-anchor').focus(); }
							}
							break;
						case 40:
							e.preventDefault();
							o = this.get_next_dom(e.currentTarget);
							if (o && o.length) { o.children('.jstree-anchor').focus(); }
							break;
							// delete
						case 46:
							e.preventDefault();
							o = this.get_node(e.currentTarget);
							if (o && o.id && o.id !== '#') {
								o = this.is_selected(o) ? this.get_selected() : o;
								// this.delete_node(o);
							}
							break;
							// f2
						case 113:
							e.preventDefault();
							o = this.get_node(e.currentTarget);
							/*
							if(o && o.id && o.id !== '#') {
								// this.edit(o);
							}
							*/
							break;
						default:
							// console.log(e.which);
							break;
					}
				}, this))
				.on("load_node.jstree", $.proxy(function (e, data) {
					if (data.status) {
						if (data.node.id === '#' && !this._data.core.loaded) {
							this._data.core.loaded = true;
							/**
							 * triggered after the root node is loaded for the first time
							 * @event
							 * @name loaded.jstree
							 */
							this.trigger("loaded");
						}
						if (!this._data.core.ready && !this.get_container_ul().find('.jstree-loading:eq(0)').length) {
							this._data.core.ready = true;
							if (this._data.core.selected.length) {
								if (this.settings.core.expand_selected_onload) {
									var tmp = [], i, j;
									for (i = 0, j = this._data.core.selected.length; i < j; i++) {
										tmp = tmp.concat(this._model.data[this._data.core.selected[i]].parents);
									}
									tmp = $.vakata.array_unique(tmp);
									for (i = 0, j = tmp.length; i < j; i++) {
										this.open_node(tmp[i], false, 0);
									}
								}
								this.trigger('changed', { 'action': 'ready', 'selected': this._data.core.selected });
							}
							/**
							 * triggered after all nodes are finished loading
							 * @event
							 * @name ready.jstree
							 */
							setTimeout($.proxy(function () { this.trigger("ready"); }, this), 0);
						}
					}
				}, this))
				// THEME RELATED
				.on("init.jstree", $.proxy(function () {
					var s = this.settings.core.themes;
					this._data.core.themes.dots = s.dots;
					this._data.core.themes.stripes = s.stripes;
					this._data.core.themes.icons = s.icons;
					this.set_theme(s.name || "default", s.url);
					this.set_theme_variant(s.variant);
				}, this))
				.on("loading.jstree", $.proxy(function () {
					this[this._data.core.themes.dots ? "show_dots" : "hide_dots"]();
					this[this._data.core.themes.icons ? "show_icons" : "hide_icons"]();
					this[this._data.core.themes.stripes ? "show_stripes" : "hide_stripes"]();
				}, this))
				.on('focus.jstree', '.jstree-anchor', $.proxy(function (e) {
					this.element.find('.jstree-hovered').not(e.currentTarget).mouseleave();
					$(e.currentTarget).mouseenter();
				}, this))
				.on('mouseenter.jstree', '.jstree-anchor', $.proxy(function (e) {
					this.hover_node(e.currentTarget);
				}, this))
				.on('mouseleave.jstree', '.jstree-anchor', $.proxy(function (e) {
					this.dehover_node(e.currentTarget);
				}, this));
		},
		/**
		 * part of the destroying of an instance. Used internally.
		 * @private
		 * @name unbind()
		 */
		unbind: function () {
			this.element.off('.jstree');
			$(document).off('.jstree-' + this._id);
		},
		/**
		 * trigger an event. Used internally.
		 * @private
		 * @name trigger(ev [, data])
		 * @param  {String} ev the name of the event to trigger
		 * @param  {Object} data additional data to pass with the event
		 */
		trigger: function (ev, data) {
			if (!data) {
				data = {};
			}
			data.instance = this;
			this.element.triggerHandler(ev.replace('.jstree', '') + '.jstree', data);
		},
		/**
		 * returns the jQuery extended instance container
		 * @name get_container()
		 * @return {jQuery}
		 */
		get_container: function () {
			return this.element;
		},
		/**
		 * returns the jQuery extended main UL node inside the instance container. Used internally.
		 * @private
		 * @name get_container_ul()
		 * @return {jQuery}
		 */
		get_container_ul: function () {
			return this.element.children("ul:eq(0)");
		},
		/**
		 * gets string replacements (localization). Used internally.
		 * @private
		 * @name get_string(key)
		 * @param  {String} key
		 * @return {String}
		 */
		get_string: function (key) {
			var a = this.settings.core.strings;
			if ($.isFunction(a)) { return a.call(this, key); }
			if (a && a[key]) { return a[key]; }
			return key;
		},
		/**
		 * gets the first child of a DOM node. Used internally.
		 * @private
		 * @name _firstChild(dom)
		 * @param  {DOMElement} dom
		 * @return {DOMElement}
		 */
		_firstChild: function (dom) {
			dom = dom ? dom.firstChild : null;
			while (dom !== null && dom.nodeType !== 1) {
				dom = dom.nextSibling;
			}
			return dom;
		},
		/**
		 * gets the next sibling of a DOM node. Used internally.
		 * @private
		 * @name _nextSibling(dom)
		 * @param  {DOMElement} dom
		 * @return {DOMElement}
		 */
		_nextSibling: function (dom) {
			dom = dom ? dom.nextSibling : null;
			while (dom !== null && dom.nodeType !== 1) {
				dom = dom.nextSibling;
			}
			return dom;
		},
		/**
		 * gets the previous sibling of a DOM node. Used internally.
		 * @private
		 * @name _previousSibling(dom)
		 * @param  {DOMElement} dom
		 * @return {DOMElement}
		 */
		_previousSibling: function (dom) {
			dom = dom ? dom.previousSibling : null;
			while (dom !== null && dom.nodeType !== 1) {
				dom = dom.previousSibling;
			}
			return dom;
		},
		/**
		 * get the JSON representation of a node (or the actual jQuery extended DOM node) by using any input (child DOM element, ID string, selector, etc)
		 * @name get_node(obj [, as_dom])
		 * @param  {mixed} obj
		 * @param  {Boolean} as_dom
		 * @return {Object|jQuery}
		 */
		get_node: function (obj, as_dom) {
			if (obj && obj.id) {
				obj = obj.id;
			}
			var dom;
			try {
				if (this._model.data[obj]) {
					obj = this._model.data[obj];
				}
				else if (((dom = $(obj, this.element)).length || (dom = $('#' + obj.replace($.jstree.idregex, '\\$&'), this.element)).length) && this._model.data[dom.closest('li').attr('id')]) {
					obj = this._model.data[dom.closest('li').attr('id')];
				}
				else if ((dom = $(obj, this.element)).length && dom.hasClass('jstree')) {
					obj = this._model.data['#'];
				}
				else {
					return false;
				}

				if (as_dom) {
					obj = obj.id === '#' ? this.element : $('#' + obj.id.replace($.jstree.idregex, '\\$&'), this.element);
				}
				return obj;
			} catch (ex) { return false; }
		},
		/**
		 * get the path to a node, either consisting of node texts, or of node IDs, optionally glued together (otherwise an array)
		 * @name get_path(obj [, glue, ids])
		 * @param  {mixed} obj the node
		 * @param  {String} glue if you want the path as a string - pass the glue here (for example '/'), if a falsy value is supplied here, an array is returned
		 * @param  {Boolean} ids if set to true build the path using ID, otherwise node text is used
		 * @return {mixed}
		 */
		get_path: function (obj, glue, ids) {
			obj = obj.parents ? obj : this.get_node(obj);
			if (!obj || obj.id === '#' || !obj.parents) {
				return false;
			}
			var i, j, p = [];
			p.push(ids ? obj.id : obj.text);
			for (i = 0, j = obj.parents.length; i < j; i++) {
				p.push(ids ? obj.parents[i] : this.get_text(obj.parents[i]));
			}
			p = p.reverse().slice(1);
			return glue ? p.join(glue) : p;
		},
		/**
		 * get the next visible node that is below the `obj` node. If `strict` is set to `true` only sibling nodes are returned.
		 * @name get_next_dom(obj [, strict])
		 * @param  {mixed} obj
		 * @param  {Boolean} strict
		 * @return {jQuery}
		 */
		get_next_dom: function (obj, strict) {
			var tmp;
			obj = this.get_node(obj, true);
			if (obj[0] === this.element[0]) {
				tmp = this._firstChild(this.get_container_ul()[0]);
				return tmp ? $(tmp) : false;
			}
			if (!obj || !obj.length) {
				return false;
			}
			if (strict) {
				tmp = this._nextSibling(obj[0]);
				return tmp ? $(tmp) : false;
			}
			if (obj.hasClass("jstree-open")) {
				tmp = this._firstChild(obj.children('ul')[0]);
				return tmp ? $(tmp) : false;
			}
			if ((tmp = this._nextSibling(obj[0])) !== null) {
				return $(tmp);
			}
			return obj.parentsUntil(".jstree", "li").next("li").eq(0);
		},
		/**
		 * get the previous visible node that is above the `obj` node. If `strict` is set to `true` only sibling nodes are returned.
		 * @name get_prev_dom(obj [, strict])
		 * @param  {mixed} obj
		 * @param  {Boolean} strict
		 * @return {jQuery}
		 */
		get_prev_dom: function (obj, strict) {
			var tmp;
			obj = this.get_node(obj, true);
			if (obj[0] === this.element[0]) {
				tmp = this.get_container_ul()[0].lastChild;
				return tmp ? $(tmp) : false;
			}
			if (!obj || !obj.length) {
				return false;
			}
			if (strict) {
				tmp = this._previousSibling(obj[0]);
				return tmp ? $(tmp) : false;
			}
			if ((tmp = this._previousSibling(obj[0])) !== null) {
				obj = $(tmp);
				while (obj.hasClass("jstree-open")) {
					obj = obj.children("ul:eq(0)").children("li:last");
				}
				return obj;
			}
			tmp = obj[0].parentNode.parentNode;
			return tmp && tmp.tagName === 'LI' ? $(tmp) : false;
		},
		/**
		 * get the parent ID of a node
		 * @name get_parent(obj)
		 * @param  {mixed} obj
		 * @return {String}
		 */
		get_parent: function (obj) {
			obj = this.get_node(obj);
			if (!obj || obj.id === '#') {
				return false;
			}
			return obj.parent;
		},
		/**
		 * get a jQuery collection of all the children of a node (node must be rendered)
		 * @name get_children_dom(obj)
		 * @param  {mixed} obj
		 * @return {jQuery}
		 */
		get_children_dom: function (obj) {
			obj = this.get_node(obj, true);
			if (obj[0] === this.element[0]) {
				return this.get_container_ul().children("li");
			}
			if (!obj || !obj.length) {
				return false;
			}
			return obj.children("ul").children("li");
		},
		/**
		 * checks if a node has children
		 * @name is_parent(obj)
		 * @param  {mixed} obj
		 * @return {Boolean}
		 */
		is_parent: function (obj) {
			obj = this.get_node(obj);
			return obj && (obj.state.loaded === false || obj.children.length > 0);
		},
		/**
		 * checks if a node is loaded (its children are available)
		 * @name is_loaded(obj)
		 * @param  {mixed} obj
		 * @return {Boolean}
		 */
		is_loaded: function (obj) {
			obj = this.get_node(obj);
			return obj && obj.state.loaded;
		},
		/**
		 * check if a node is currently loading (fetching children)
		 * @name is_loading(obj)
		 * @param  {mixed} obj
		 * @return {Boolean}
		 */
		is_loading: function (obj) {
			obj = this.get_node(obj);
			return obj && obj.state && obj.state.loading;
		},
		/**
		 * check if a node is opened
		 * @name is_open(obj)
		 * @param  {mixed} obj
		 * @return {Boolean}
		 */
		is_open: function (obj) {
			obj = this.get_node(obj);
			return obj && obj.state.opened;
		},
		/**
		 * check if a node is in a closed state
		 * @name is_closed(obj)
		 * @param  {mixed} obj
		 * @return {Boolean}
		 */
		is_closed: function (obj) {
			obj = this.get_node(obj);
			return obj && this.is_parent(obj) && !obj.state.opened;
		},
		/**
		 * check if a node has no children
		 * @name is_leaf(obj)
		 * @param  {mixed} obj
		 * @return {Boolean}
		 */
		is_leaf: function (obj) {
			return !this.is_parent(obj);
		},
		/**
		 * loads a node (fetches its children using the `core.data` setting). Multiple nodes can be passed to by using an array.
		 * @name load_node(obj [, callback])
		 * @param  {mixed} obj
		 * @param  {function} callback a function to be executed once loading is conplete, the function is executed in the instance's scope and receives two arguments - the node and a boolean status
		 * @return {Boolean}
		 * @trigger load_node.jstree
		 */
		load_node: function (obj, callback) {
			var t1, t2, k, l, i, j, c;
			if ($.isArray(obj)) {
				obj = obj.slice();
				for (t1 = 0, t2 = obj.length; t1 < t2; t1++) {
					this.load_node(obj[t1], callback);
				}
				return true;
			}
			obj = this.get_node(obj);
			if (!obj) {
				if (callback) { callback.call(this, obj, false); }
				return false;
			}
			if (obj.state.loaded) {
				obj.state.loaded = false;
				for (k = 0, l = obj.children_d.length; k < l; k++) {
					for (i = 0, j = obj.parents.length; i < j; i++) {
						this._model.data[obj.parents[i]].children_d = $.vakata.array_remove_item(this._model.data[obj.parents[i]].children_d, obj.children_d[k]);
					}
					if (this._model.data[obj.children_d[k]].state.selected) {
						c = true;
						this._data.core.selected = $.vakata.array_remove_item(this._data.core.selected, obj.children_d[k]);
					}
					delete this._model.data[obj.children_d[k]];
				}
				obj.children = [];
				obj.children_d = [];
				if (c) {
					this.trigger('changed', { 'action': 'load_node', 'node': obj, 'selected': this._data.core.selected });
				}
			}
			obj.state.loading = true;
			this.get_node(obj, true).addClass("jstree-loading");
			this._load_node(obj, $.proxy(function (status) {
				obj.state.loading = false;
				obj.state.loaded = status;
				var dom = this.get_node(obj, true);
				if (obj.state.loaded && !obj.children.length && dom && dom.length && !dom.hasClass('jstree-leaf')) {
					dom.removeClass('jstree-closed jstree-open').addClass('jstree-leaf');
				}
				dom.removeClass("jstree-loading");
				/**
				 * triggered after a node is loaded
				 * @event
				 * @name load_node.jstree
				 * @param {Object} node the node that was loading
				 * @param {Boolean} status was the node loaded successfully
				 */
				this.trigger('load_node', { "node": obj, "status": status });
				if (callback) {
					callback.call(this, obj, status);
				}
			}, this));
			return true;
		},
		/**
		 * load an array of nodes (will also load unavailable nodes as soon as the appear in the structure). Used internally.
		 * @private
		 * @name _load_nodes(nodes [, callback])
		 * @param  {array} nodes
		 * @param  {function} callback a function to be executed once loading is complete, the function is executed in the instance's scope and receives one argument - the array passed to _load_nodes
		 */
		_load_nodes: function (nodes, callback, is_callback) {
			var r = true,
				c = function () { this._load_nodes(nodes, callback, true); },
				m = this._model.data, i, j;
			for (i = 0, j = nodes.length; i < j; i++) {
				if (m[nodes[i]] && (!m[nodes[i]].state.loaded || !is_callback)) {
					if (!this.is_loading(nodes[i])) {
						this.load_node(nodes[i], c);
					}
					r = false;
				}
			}
			if (r) {
				if (!callback.done) {
					callback.call(this, nodes);
					callback.done = true;
				}
			}
		},
		/**
		 * handles the actual loading of a node. Used only internally.
		 * @private
		 * @name _load_node(obj [, callback])
		 * @param  {mixed} obj
		 * @param  {function} callback a function to be executed once loading is complete, the function is executed in the instance's scope and receives one argument - a boolean status
		 * @return {Boolean}
		 */
		_load_node: function (obj, callback) {
			var s = this.settings.core.data, t;
			// use original HTML
			if (!s) {
				return callback.call(this, obj.id === '#' ? this._append_html_data(obj, this._data.core.original_container_html.clone(true)) : false);
			}
			if ($.isFunction(s)) {
				return s.call(this, obj, $.proxy(function (d) {
					return d === false ? callback.call(this, false) : callback.call(this, this[typeof d === 'string' ? '_append_html_data' : '_append_json_data'](obj, typeof d === 'string' ? $(d) : d));
				}, this));
			}
			if (typeof s === 'object') {
				if (s.url) {
					s = $.extend(true, {}, s);
					if ($.isFunction(s.url)) {
						s.url = s.url.call(this, obj);
					}
					if ($.isFunction(s.data)) {
						s.data = s.data.call(this, obj);
					}
					return $.ajax(s)
						.done($.proxy(function (d, t, x) {
							var type = x.getResponseHeader('Content-Type');
							if (type.indexOf('json') !== -1 || typeof d === "object") {
								return callback.call(this, this._append_json_data(obj, d));
							}
							if (type.indexOf('html') !== -1 || typeof d === "string") {
								return callback.call(this, this._append_html_data(obj, $(d)));
							}
							this._data.core.last_error = { 'error': 'ajax', 'plugin': 'core', 'id': 'core_04', 'reason': 'Could not load node', 'data': JSON.stringify({ 'id': obj.id, 'xhr': x }) };
							return callback.call(this, false);
						}, this))
						.fail($.proxy(function (f) {
							callback.call(this, false);
							this._data.core.last_error = { 'error': 'ajax', 'plugin': 'core', 'id': 'core_04', 'reason': 'Could not load node', 'data': JSON.stringify({ 'id': obj.id, 'xhr': f }) };
							this.settings.core.error.call(this, this._data.core.last_error);
						}, this));
				}
				t = ($.isArray(s) || $.isPlainObject(s)) ? JSON.parse(JSON.stringify(s)) : s;
				if (obj.id !== "#") { this._data.core.last_error = { 'error': 'nodata', 'plugin': 'core', 'id': 'core_05', 'reason': 'Could not load node', 'data': JSON.stringify({ 'id': obj.id }) }; }
				return callback.call(this, (obj.id === "#" ? this._append_json_data(obj, t) : false));
			}
			if (typeof s === 'string') {
				if (obj.id !== "#") { this._data.core.last_error = { 'error': 'nodata', 'plugin': 'core', 'id': 'core_06', 'reason': 'Could not load node', 'data': JSON.stringify({ 'id': obj.id }) }; }
				return callback.call(this, (obj.id === "#" ? this._append_html_data(obj, $(s)) : false));
			}
			return callback.call(this, false);
		},
		/**
		 * adds a node to the list of nodes to redraw. Used only internally.
		 * @private
		 * @name _node_changed(obj [, callback])
		 * @param  {mixed} obj
		 */
		_node_changed: function (obj) {
			obj = this.get_node(obj);
			if (obj) {
				this._model.changed.push(obj.id);
			}
		},
		/**
		 * appends HTML content to the tree. Used internally.
		 * @private
		 * @name _append_html_data(obj, data)
		 * @param  {mixed} obj the node to append to
		 * @param  {String} data the HTML string to parse and append
		 * @return {Boolean}
		 * @trigger model.jstree, changed.jstree
		 */
		_append_html_data: function (dom, data) {
			dom = this.get_node(dom);
			dom.children = [];
			dom.children_d = [];
			var dat = data.is('ul') ? data.children() : data,
				par = dom.id,
				chd = [],
				dpc = [],
				m = this._model.data,
				p = m[par],
				s = this._data.core.selected.length,
				tmp, i, j;
			dat.each($.proxy(function (i, v) {
				tmp = this._parse_model_from_html($(v), par, p.parents.concat());
				if (tmp) {
					chd.push(tmp);
					dpc.push(tmp);
					if (m[tmp].children_d.length) {
						dpc = dpc.concat(m[tmp].children_d);
					}
				}
			}, this));
			p.children = chd;
			p.children_d = dpc;
			for (i = 0, j = p.parents.length; i < j; i++) {
				m[p.parents[i]].children_d = m[p.parents[i]].children_d.concat(dpc);
			}
			/**
			 * triggered when new data is inserted to the tree model
			 * @event
			 * @name model.jstree
			 * @param {Array} nodes an array of node IDs
			 * @param {String} parent the parent ID of the nodes
			 */
			this.trigger('model', { "nodes": dpc, 'parent': par });
			if (par !== '#') {
				this._node_changed(par);
				this.redraw();
			}
			else {
				this.get_container_ul().children('.jstree-initial-node').remove();
				this.redraw(true);
			}
			if (this._data.core.selected.length !== s) {
				this.trigger('changed', { 'action': 'model', 'selected': this._data.core.selected });
			}
			return true;
		},
		/**
		 * appends JSON content to the tree. Used internally.
		 * @private
		 * @name _append_json_data(obj, data)
		 * @param  {mixed} obj the node to append to
		 * @param  {String} data the JSON object to parse and append
		 * @return {Boolean}
		 */
		_append_json_data: function (dom, data) {
			dom = this.get_node(dom);
			dom.children = [];
			dom.children_d = [];
			var dat = data,
				par = dom.id,
				chd = [],
				dpc = [],
				m = this._model.data,
				p = m[par],
				s = this._data.core.selected.length,
				tmp, i, j;
			// *%$@!!!
			if (dat.d) {
				dat = dat.d;
				if (typeof dat === "string") {
					dat = JSON.parse(dat);
				}
			}
			if (!$.isArray(dat)) { dat = [dat]; }
			if (dat.length && dat[0].id !== undefined && dat[0].parent !== undefined) {
				// Flat JSON support (for easy import from DB):
				// 1) convert to object (foreach)
				for (i = 0, j = dat.length; i < j; i++) {
					if (!dat[i].children) {
						dat[i].children = [];
					}
					m[dat[i].id.toString()] = dat[i];
				}
				// 2) populate children (foreach)
				for (i = 0, j = dat.length; i < j; i++) {
					m[dat[i].parent.toString()].children.push(dat[i].id.toString());
					// populate parent.children_d
					p.children_d.push(dat[i].id.toString());
				}
				// 3) normalize && populate parents and children_d with recursion
				for (i = 0, j = p.children.length; i < j; i++) {
					tmp = this._parse_model_from_flat_json(m[p.children[i]], par, p.parents.concat());
					dpc.push(tmp);
					if (m[tmp].children_d.length) {
						dpc = dpc.concat(m[tmp].children_d);
					}
				}
				// ?) three_state selection - p.state.selected && t - (if three_state foreach(dat => ch) -> foreach(parents) if(parent.selected) child.selected = true;
			}
			else {
				for (i = 0, j = dat.length; i < j; i++) {
					tmp = this._parse_model_from_json(dat[i], par, p.parents.concat());
					if (tmp) {
						chd.push(tmp);
						dpc.push(tmp);
						if (m[tmp].children_d.length) {
							dpc = dpc.concat(m[tmp].children_d);
						}
					}
				}
				p.children = chd;
				p.children_d = dpc;
				for (i = 0, j = p.parents.length; i < j; i++) {
					m[p.parents[i]].children_d = m[p.parents[i]].children_d.concat(dpc);
				}
			}
			this.trigger('model', { "nodes": dpc, 'parent': par });

			if (par !== '#') {
				this._node_changed(par);
				this.redraw();
			}
			else {
				// this.get_container_ul().children('.jstree-initial-node').remove();
				this.redraw(true);
			}
			if (this._data.core.selected.length !== s) {
				this.trigger('changed', { 'action': 'model', 'selected': this._data.core.selected });
			}
			return true;
		},
		/**
		 * parses a node from a jQuery object and appends them to the in memory tree model. Used internally.
		 * @private
		 * @name _parse_model_from_html(d [, p, ps])
		 * @param  {jQuery} d the jQuery object to parse
		 * @param  {String} p the parent ID
		 * @param  {Array} ps list of all parents
		 * @return {String} the ID of the object added to the model
		 */
		_parse_model_from_html: function (d, p, ps) {
			if (!ps) { ps = []; }
			else { ps = [].concat(ps); }
			if (p) { ps.unshift(p); }
			var c, e, m = this._model.data,
				data = {
					id: false,
					text: false,
					icon: true,
					parent: p,
					parents: ps,
					children: [],
					children_d: [],
					data: null,
					state: {},
					li_attr: { id: false },
					a_attr: { href: '#' },
					original: false
				}, i, tmp, tid;
			for (i in this._model.default_state) {
				if (this._model.default_state.hasOwnProperty(i)) {
					data.state[i] = this._model.default_state[i];
				}
			}
			tmp = $.vakata.attributes(d, true);
			$.each(tmp, function (i, v) {
				v = $.trim(v);
				if (!v.length) { return true; }
				data.li_attr[i] = v;
				if (i === 'id') {
					data.id = v.toString();
				}
			});
			tmp = d.children('a').eq(0);
			if (tmp.length) {
				tmp = $.vakata.attributes(tmp, true);
				$.each(tmp, function (i, v) {
					v = $.trim(v);
					if (v.length) {
						data.a_attr[i] = v;
					}
				});
			}
			tmp = d.children("a:eq(0)").length ? d.children("a:eq(0)").clone() : d.clone();
			tmp.children("ins, i, ul").remove();
			tmp = tmp.html();
			tmp = $('<div />').html(tmp);
			data.text = tmp.html();
			tmp = d.data();
			data.data = tmp ? $.extend(true, {}, tmp) : null;
			data.state.opened = d.hasClass('jstree-open');
			data.state.selected = d.children('a').hasClass('jstree-clicked');
			data.state.disabled = d.children('a').hasClass('jstree-disabled');
			if (data.data && data.data.jstree) {
				for (i in data.data.jstree) {
					if (data.data.jstree.hasOwnProperty(i)) {
						data.state[i] = data.data.jstree[i];
					}
				}
			}
			tmp = d.children("a").children(".jstree-themeicon");
			if (tmp.length) {
				data.icon = tmp.hasClass('jstree-themeicon-hidden') ? false : tmp.attr('rel');
			}
			if (data.state.icon) {
				data.icon = data.state.icon;
			}
			tmp = d.children("ul").children("li");
			do {
				tid = 'j' + this._id + '_' + (++this._cnt);
			} while (m[tid]);
			data.id = data.li_attr.id ? data.li_attr.id.toString() : tid;
			if (tmp.length) {
				tmp.each($.proxy(function (i, v) {
					c = this._parse_model_from_html($(v), data.id, ps);
					e = this._model.data[c];
					data.children.push(c);
					if (e.children_d.length) {
						data.children_d = data.children_d.concat(e.children_d);
					}
				}, this));
				data.children_d = data.children_d.concat(data.children);
			}
			else {
				if (d.hasClass('jstree-closed')) {
					data.state.loaded = false;
				}
			}
			if (data.li_attr['class']) {
				data.li_attr['class'] = data.li_attr['class'].replace('jstree-closed', '').replace('jstree-open', '');
			}
			if (data.a_attr['class']) {
				data.a_attr['class'] = data.a_attr['class'].replace('jstree-clicked', '').replace('jstree-disabled', '');
			}
			m[data.id] = data;
			if (data.state.selected) {
				this._data.core.selected.push(data.id);
			}
			return data.id;
		},
		/**
		 * parses a node from a JSON object (used when dealing with flat data, which has no nesting of children, but has id and parent properties) and appends it to the in memory tree model. Used internally.
		 * @private
		 * @name _parse_model_from_flat_json(d [, p, ps])
		 * @param  {Object} d the JSON object to parse
		 * @param  {String} p the parent ID
		 * @param  {Array} ps list of all parents
		 * @return {String} the ID of the object added to the model
		 */
		_parse_model_from_flat_json: function (d, p, ps) {
			if (!ps) { ps = []; }
			else { ps = ps.concat(); }
			if (p) { ps.unshift(p); }
			var tid = d.id.toString(),
				m = this._model.data,
				df = this._model.default_state,
				i, j, c, e,
				tmp = {
					id: tid,
					text: d.text || '',
					icon: d.icon !== undefined ? d.icon : true,
					parent: p,
					parents: ps,
					children: d.children || [],
					children_d: d.children_d || [],
					data: d.data,
					state: {},
					li_attr: { id: false },
					a_attr: { href: '#' },
					original: false
				};
			for (i in df) {
				if (df.hasOwnProperty(i)) {
					tmp.state[i] = df[i];
				}
			}
			if (d && d.data && d.data.jstree && d.data.jstree.icon) {
				tmp.icon = d.data.jstree.icon;
			}
			if (d && d.data) {
				tmp.data = d.data;
				if (d.data.jstree) {
					for (i in d.data.jstree) {
						if (d.data.jstree.hasOwnProperty(i)) {
							tmp.state[i] = d.data.jstree[i];
						}
					}
				}
			}
			if (d && typeof d.state === 'object') {
				for (i in d.state) {
					if (d.state.hasOwnProperty(i)) {
						tmp.state[i] = d.state[i];
					}
				}
			}
			if (d && typeof d.li_attr === 'object') {
				for (i in d.li_attr) {
					if (d.li_attr.hasOwnProperty(i)) {
						tmp.li_attr[i] = d.li_attr[i];
					}
				}
			}
			if (!tmp.li_attr.id) {
				tmp.li_attr.id = tid;
			}
			if (d && typeof d.a_attr === 'object') {
				for (i in d.a_attr) {
					if (d.a_attr.hasOwnProperty(i)) {
						tmp.a_attr[i] = d.a_attr[i];
					}
				}
			}
			if (d && d.children && d.children === true) {
				tmp.state.loaded = false;
				tmp.children = [];
				tmp.children_d = [];
			}
			m[tmp.id] = tmp;
			for (i = 0, j = tmp.children.length; i < j; i++) {
				c = this._parse_model_from_flat_json(m[tmp.children[i]], tmp.id, ps);
				e = m[c];
				tmp.children_d.push(c);
				if (e.children_d.length) {
					tmp.children_d = tmp.children_d.concat(e.children_d);
				}
			}
			delete d.data;
			delete d.children;
			m[tmp.id].original = d;
			if (tmp.state.selected) {
				this._data.core.selected.push(tmp.id);
			}
			return tmp.id;
		},
		/**
		 * parses a node from a JSON object and appends it to the in memory tree model. Used internally.
		 * @private
		 * @name _parse_model_from_json(d [, p, ps])
		 * @param  {Object} d the JSON object to parse
		 * @param  {String} p the parent ID
		 * @param  {Array} ps list of all parents
		 * @return {String} the ID of the object added to the model
		 */
		_parse_model_from_json: function (d, p, ps) {
			if (!ps) { ps = []; }
			else { ps = ps.concat(); }
			if (p) { ps.unshift(p); }
			var tid = false, i, j, c, e, m = this._model.data, df = this._model.default_state, tmp;
			do {
				tid = 'j' + this._id + '_' + (++this._cnt);
			} while (m[tid]);

			tmp = {
				id: false,
				text: typeof d === 'string' ? d : '',
				icon: typeof d === 'object' && d.icon !== undefined ? d.icon : true,
				parent: p,
				parents: ps,
				children: [],
				children_d: [],
				data: null,
				state: {},
				li_attr: { id: false },
				a_attr: { href: '#' },
				original: false
			};
			for (i in df) {
				if (df.hasOwnProperty(i)) {
					tmp.state[i] = df[i];
				}
			}
			if (d && d.id) { tmp.id = d.id.toString(); }
			if (d && d.text) { tmp.text = d.text; }
			if (d && d.data && d.data.jstree && d.data.jstree.icon) {
				tmp.icon = d.data.jstree.icon;
			}
			if (d && d.data) {
				tmp.data = d.data;
				if (d.data.jstree) {
					for (i in d.data.jstree) {
						if (d.data.jstree.hasOwnProperty(i)) {
							tmp.state[i] = d.data.jstree[i];
						}
					}
				}
			}
			if (d && typeof d.state === 'object') {
				for (i in d.state) {
					if (d.state.hasOwnProperty(i)) {
						tmp.state[i] = d.state[i];
					}
				}
			}
			if (d && typeof d.li_attr === 'object') {
				for (i in d.li_attr) {
					if (d.li_attr.hasOwnProperty(i)) {
						tmp.li_attr[i] = d.li_attr[i];
					}
				}
			}
			if (tmp.li_attr.id && !tmp.id) {
				tmp.id = tmp.li_attr.id.toString();
			}
			if (!tmp.id) {
				tmp.id = tid;
			}
			if (!tmp.li_attr.id) {
				tmp.li_attr.id = tmp.id;
			}
			if (d && typeof d.a_attr === 'object') {
				for (i in d.a_attr) {
					if (d.a_attr.hasOwnProperty(i)) {
						tmp.a_attr[i] = d.a_attr[i];
					}
				}
			}
			if (d && d.children && d.children.length) {
				for (i = 0, j = d.children.length; i < j; i++) {
					c = this._parse_model_from_json(d.children[i], tmp.id, ps);
					e = m[c];
					tmp.children.push(c);
					if (e.children_d.length) {
						tmp.children_d = tmp.children_d.concat(e.children_d);
					}
				}
				tmp.children_d = tmp.children_d.concat(tmp.children);
			}
			if (d && d.children && d.children === true) {
				tmp.state.loaded = false;
				tmp.children = [];
				tmp.children_d = [];
			}
			delete d.data;
			delete d.children;
			tmp.original = d;
			m[tmp.id] = tmp;
			if (tmp.state.selected) {
				this._data.core.selected.push(tmp.id);
			}
			return tmp.id;
		},
		/**
		 * redraws all nodes that need to be redrawn. Used internally.
		 * @private
		 * @name _redraw()
		 * @trigger redraw.jstree
		 */
		_redraw: function () {
			var nodes = this._model.force_full_redraw ? this._model.data['#'].children.concat([]) : this._model.changed.concat([]),
				f = document.createElement('UL'), tmp, i, j;
			for (i = 0, j = nodes.length; i < j; i++) {
				tmp = this.redraw_node(nodes[i], true, this._model.force_full_redraw);
				if (tmp && this._model.force_full_redraw) {
					f.appendChild(tmp);
				}
			}
			if (this._model.force_full_redraw) {
				f.className = this.get_container_ul()[0].className;
				this.element.empty().append(f);
				//this.get_container_ul()[0].appendChild(f);
			}
			this._model.force_full_redraw = false;
			this._model.changed = [];
			/**
			 * triggered after nodes are redrawn
			 * @event
			 * @name redraw.jstree
			 * @param {array} nodes the redrawn nodes
			 */
			this.trigger('redraw', { "nodes": nodes });
		},
		/**
		 * redraws all nodes that need to be redrawn or optionally - the whole tree
		 * @name redraw([full])
		 * @param {Boolean} full if set to `true` all nodes are redrawn.
		 */
		redraw: function (full) {
			if (full) {
				this._model.force_full_redraw = true;
			}
			//if(this._model.redraw_timeout) {
			//	clearTimeout(this._model.redraw_timeout);
			//}
			//this._model.redraw_timeout = setTimeout($.proxy(this._redraw, this),0);
			this._redraw();
		},
		/**
		 * redraws a single node. Used internally.
		 * @private
		 * @name redraw_node(node, deep, is_callback)
		 * @param {mixed} node the node to redraw
		 * @param {Boolean} deep should child nodes be redrawn too
		 * @param {Boolean} is_callback is this a recursion call
		 */
		redraw_node: function (node, deep, is_callback) {
			var obj = this.get_node(node),
				par = false,
				ind = false,
				old = false,
				i = false,
				j = false,
				k = false,
				c = '',
				d = document,
				m = this._model.data,
				f = false,
				s = false;
			if (!obj) { return false; }
			if (obj.id === '#') { return this.redraw(true); }
			deep = deep || obj.children.length === 0;
			node = !document.querySelector ? document.getElementById(obj.id) : this.element[0].querySelector('#' + ("0123456789".indexOf(obj.id[0]) !== -1 ? '\\3' + obj.id[0] + ' ' + obj.id.substr(1).replace($.jstree.idregex, '\\$&') : obj.id.replace($.jstree.idregex, '\\$&'))); //, this.element);
			if (!node) {
				deep = true;
				//node = d.createElement('LI');
				if (!is_callback) {
					par = obj.parent !== '#' ? $('#' + obj.parent.replace($.jstree.idregex, '\\$&'), this.element)[0] : null;
					if (par !== null && (!par || !m[obj.parent].state.opened)) {
						return false;
					}
					ind = $.inArray(obj.id, par === null ? m['#'].children : m[obj.parent].children);
				}
			}
			else {
				node = $(node);
				if (!is_callback) {
					par = node.parent().parent()[0];
					if (par === this.element[0]) {
						par = null;
					}
					ind = node.index();
				}
				// m[obj.id].data = node.data(); // use only node's data, no need to touch jquery storage
				if (!deep && obj.children.length && !node.children('ul').length) {
					deep = true;
				}
				if (!deep) {
					old = node.children('UL')[0];
				}
				s = node.attr('aria-selected');
				f = node.children('.jstree-anchor')[0] === document.activeElement;
				node.remove();
				//node = d.createElement('LI');
				//node = node[0];
			}
			node = _node.cloneNode(true);
			// node is DOM, deep is boolean

			c = 'jstree-node ';
			for (i in obj.li_attr) {
				if (obj.li_attr.hasOwnProperty(i)) {
					if (i === 'id') { continue; }
					if (i !== 'class') {
						node.setAttribute(i, obj.li_attr[i]);
					}
					else {
						c += obj.li_attr[i];
					}
				}
			}
			if (s && s !== "false") {
				node.setAttribute('aria-selected', true);
			}
			if (obj.state.loaded && !obj.children.length) {
				c += ' jstree-leaf';
			}
			else {
				c += obj.state.opened && obj.state.loaded ? ' jstree-open' : ' jstree-closed';
				node.setAttribute('aria-expanded', (obj.state.opened && obj.state.loaded));
			}
			if (obj.parent !== null && m[obj.parent].children[m[obj.parent].children.length - 1] === obj.id) {
				c += ' jstree-last';
			}
			node.id = obj.id;
			node.className = c;
			c = (obj.state.selected ? ' jstree-clicked' : '') + (obj.state.disabled ? ' jstree-disabled' : '');
			for (j in obj.a_attr) {
				if (obj.a_attr.hasOwnProperty(j)) {
					if (j === 'href' && obj.a_attr[j] === '#') { continue; }
					if (j !== 'class') {
						node.childNodes[1].setAttribute(j, obj.a_attr[j]);
					}
					else {
						c += ' ' + obj.a_attr[j];
					}
				}
			}
			if (c.length) {
				node.childNodes[1].className = 'jstree-anchor ' + c;
			}
			if ((obj.icon && obj.icon !== true) || obj.icon === false) {
				if (obj.icon === false) {
					node.childNodes[1].childNodes[0].className += ' jstree-themeicon-hidden';
				}
				else if (obj.icon.indexOf('/') === -1 && obj.icon.indexOf('.') === -1) {
					node.childNodes[1].childNodes[0].className += ' ' + obj.icon + ' jstree-themeicon-custom';
				}
				else {
					node.childNodes[1].childNodes[0].style.backgroundImage = 'url(' + obj.icon + ')';
					node.childNodes[1].childNodes[0].style.backgroundPosition = 'center center';
					node.childNodes[1].childNodes[0].style.backgroundSize = 'auto';
					node.childNodes[1].childNodes[0].className += ' jstree-themeicon-custom';
				}
			}
			//node.childNodes[1].appendChild(d.createTextNode(obj.text));
			node.childNodes[1].innerHTML += obj.text;
			// if(obj.data) { $.data(node, obj.data); } // always work with node's data, no need to touch jquery store

			if (deep && obj.children.length && obj.state.opened && obj.state.loaded) {
				k = d.createElement('UL');
				k.setAttribute('role', 'group');
				k.className = 'jstree-children';
				for (i = 0, j = obj.children.length; i < j; i++) {
					k.appendChild(this.redraw_node(obj.children[i], deep, true));
				}
				node.appendChild(k);
			}
			if (old) {
				node.appendChild(old);
			}
			if (!is_callback) {
				// append back using par / ind
				if (!par) {
					par = this.element[0];
				}
				if (!par.getElementsByTagName('UL').length) {
					i = d.createElement('UL');
					i.setAttribute('role', 'group');
					i.className = 'jstree-children';
					par.appendChild(i);
					par = i;
				}
				else {
					par = par.getElementsByTagName('UL')[0];
				}

				if (ind < par.childNodes.length) {
					par.insertBefore(node, par.childNodes[ind]);
				}
				else {
					par.appendChild(node);
				}
				if (f) {
					node.childNodes[1].focus();
				}
			}
			if (obj.state.opened && !obj.state.loaded) {
				obj.state.opened = false;
				setTimeout($.proxy(function () {
					this.open_node(obj.id, false, 0);
				}, this), 0);
			}
			return node;
		},
		/**
		 * opens a node, revaling its children. If the node is not loaded it will be loaded and opened once ready.
		 * @name open_node(obj [, callback, animation])
		 * @param {mixed} obj the node to open
		 * @param {Function} callback a function to execute once the node is opened
		 * @param {Number} animation the animation duration in milliseconds when opening the node (overrides the `core.animation` setting). Use `false` for no animation.
		 * @trigger open_node.jstree, after_open.jstree, before_open.jstree
		 */
		open_node: function (obj, callback, animation) {
			var t1, t2, d, t;
			if ($.isArray(obj)) {
				obj = obj.slice();
				for (t1 = 0, t2 = obj.length; t1 < t2; t1++) {
					this.open_node(obj[t1], callback, animation);
				}
				return true;
			}
			obj = this.get_node(obj);
			if (!obj || obj.id === '#') {
				return false;
			}
			animation = animation === undefined ? this.settings.core.animation : animation;
			if (!this.is_closed(obj)) {
				if (callback) {
					callback.call(this, obj, false);
				}
				return false;
			}
			if (!this.is_loaded(obj)) {
				if (this.is_loading(obj)) {
					return setTimeout($.proxy(function () {
						this.open_node(obj, callback, animation);
					}, this), 500);
				}
				this.load_node(obj, function (o, ok) {
					return ok ? this.open_node(o, callback, animation) : (callback ? callback.call(this, o, false) : false);
				});
			}
			else {
				d = this.get_node(obj, true);
				t = this;
				if (d.length) {
					if (obj.children.length && !this._firstChild(d.children('ul')[0])) {
						obj.state.opened = true;
						this.redraw_node(obj, true);
						d = this.get_node(obj, true);
					}
					if (!animation) {
						this.trigger('before_open', { "node": obj });
						d[0].className = d[0].className.replace('jstree-closed', 'jstree-open');
						d[0].setAttribute("aria-expanded", true);
					}
					else {
						this.trigger('before_open', { "node": obj });
						d
							.children("ul").css("display", "none").end()
							.removeClass("jstree-closed").addClass("jstree-open").attr("aria-expanded", true)
							.children("ul").stop(true, true)
								.slideDown(animation, function () {
									this.style.display = "";
									t.trigger("after_open", { "node": obj });
								});
					}
				}
				obj.state.opened = true;
				if (callback) {
					callback.call(this, obj, true);
				}
				if (!d.length) {
					/**
					 * triggered when a node is about to be opened (if the node is supposed to be in the DOM, it will be, but it won't be visible yet)
					 * @event
					 * @name before_open.jstree
					 * @param {Object} node the opened node
					 */
					this.trigger('before_open', { "node": obj });
				}
				/**
				 * triggered when a node is opened (if there is an animation it will not be completed yet)
				 * @event
				 * @name open_node.jstree
				 * @param {Object} node the opened node
				 */
				this.trigger('open_node', { "node": obj });
				if (!animation || !d.length) {
					/**
					 * triggered when a node is opened and the animation is complete
					 * @event
					 * @name after_open.jstree
					 * @param {Object} node the opened node
					 */
					this.trigger("after_open", { "node": obj });
				}
			}
		},
		/**
		 * opens every parent of a node (node should be loaded)
		 * @name _open_to(obj)
		 * @param {mixed} obj the node to reveal
		 * @private
		 */
		_open_to: function (obj) {
			obj = this.get_node(obj);
			if (!obj || obj.id === '#') {
				return false;
			}
			var i, j, p = obj.parents;
			for (i = 0, j = p.length; i < j; i += 1) {
				if (i !== '#') {
					this.open_node(p[i], false, 0);
				}
			}
			return $('#' + obj.id.replace($.jstree.idregex, '\\$&'), this.element);
		},
		/**
		 * closes a node, hiding its children
		 * @name close_node(obj [, animation])
		 * @param {mixed} obj the node to close
		 * @param {Number} animation the animation duration in milliseconds when closing the node (overrides the `core.animation` setting). Use `false` for no animation.
		 * @trigger close_node.jstree, after_close.jstree
		 */
		close_node: function (obj, animation) {
			var t1, t2, t, d;
			if ($.isArray(obj)) {
				obj = obj.slice();
				for (t1 = 0, t2 = obj.length; t1 < t2; t1++) {
					this.close_node(obj[t1], animation);
				}
				return true;
			}
			obj = this.get_node(obj);
			if (!obj || obj.id === '#') {
				return false;
			}
			if (this.is_closed(obj)) {
				return false;
			}
			animation = animation === undefined ? this.settings.core.animation : animation;
			t = this;
			d = this.get_node(obj, true);
			if (d.length) {
				if (!animation) {
					d[0].className = d[0].className.replace('jstree-open', 'jstree-closed');
					d.attr("aria-expanded", false).children('ul').remove();
				}
				else {
					d
						.children("ul").attr("style", "display:block !important").end()
						.removeClass("jstree-open").addClass("jstree-closed").attr("aria-expanded", false)
						.children("ul").stop(true, true).slideUp(animation, function () {
							this.style.display = "";
							d.children('ul').remove();
							t.trigger("after_close", { "node": obj });
						});
				}
			}
			obj.state.opened = false;
			/**
			 * triggered when a node is closed (if there is an animation it will not be complete yet)
			 * @event
			 * @name close_node.jstree
			 * @param {Object} node the closed node
			 */
			this.trigger('close_node', { "node": obj });
			if (!animation || !d.length) {
				/**
				 * triggered when a node is closed and the animation is complete
				 * @event
				 * @name after_close.jstree
				 * @param {Object} node the closed node
				 */
				this.trigger("after_close", { "node": obj });
			}
		},
		/**
		 * toggles a node - closing it if it is open, opening it if it is closed
		 * @name toggle_node(obj)
		 * @param {mixed} obj the node to toggle
		 */
		toggle_node: function (obj) {
			var t1, t2;
			if ($.isArray(obj)) {
				obj = obj.slice();
				for (t1 = 0, t2 = obj.length; t1 < t2; t1++) {
					this.toggle_node(obj[t1]);
				}
				return true;
			}
			if (this.is_closed(obj)) {
				return this.open_node(obj);
			}
			if (this.is_open(obj)) {
				return this.close_node(obj);
			}
		},
		/**
		 * opens all nodes within a node (or the tree), revaling their children. If the node is not loaded it will be loaded and opened once ready.
		 * @name open_all([obj, animation, original_obj])
		 * @param {mixed} obj the node to open recursively, omit to open all nodes in the tree
		 * @param {Number} animation the animation duration in milliseconds when opening the nodes, the default is no animation
		 * @param {jQuery} reference to the node that started the process (internal use)
		 * @trigger open_all.jstree
		 */
		open_all: function (obj, animation, original_obj) {
			if (!obj) { obj = '#'; }
			obj = this.get_node(obj);
			if (!obj) { return false; }
			var dom = obj.id === '#' ? this.get_container_ul() : this.get_node(obj, true), i, j, _this;
			if (!dom.length) {
				for (i = 0, j = obj.children_d.length; i < j; i++) {
					if (this.is_closed(this._model.data[obj.children_d[i]])) {
						this._model.data[obj.children_d[i]].state.opened = true;
					}
				}
				return this.trigger('open_all', { "node": obj });
			}
			original_obj = original_obj || dom;
			_this = this;
			dom = this.is_closed(obj) ? dom.find('li.jstree-closed').addBack() : dom.find('li.jstree-closed');
			dom.each(function () {
				_this.open_node(
					this,
					function (node, status) { if (status && this.is_parent(node)) { this.open_all(node, animation, original_obj); } },
					animation || 0
				);
			});
			if (original_obj.find('li.jstree-closed').length === 0) {
				/**
				 * triggered when an `open_all` call completes
				 * @event
				 * @name open_all.jstree
				 * @param {Object} node the opened node
				 */
				this.trigger('open_all', { "node": this.get_node(original_obj) });
			}
		},
		/**
		 * closes all nodes within a node (or the tree), revaling their children
		 * @name close_all([obj, animation])
		 * @param {mixed} obj the node to close recursively, omit to close all nodes in the tree
		 * @param {Number} animation the animation duration in milliseconds when closing the nodes, the default is no animation
		 * @trigger close_all.jstree
		 */
		close_all: function (obj, animation) {
			if (!obj) { obj = '#'; }
			obj = this.get_node(obj);
			if (!obj) { return false; }
			var dom = obj.id === '#' ? this.get_container_ul() : this.get_node(obj, true),
				_this = this, i, j;
			if (!dom.length) {
				for (i = 0, j = obj.children_d.length; i < j; i++) {
					this._model.data[obj.children_d[i]].state.opened = false;
				}
				return this.trigger('close_all', { "node": obj });
			}
			dom = this.is_open(obj) ? dom.find('li.jstree-open').addBack() : dom.find('li.jstree-open');
			$(dom.get().reverse()).each(function () { _this.close_node(this, animation || 0); });
			/**
			 * triggered when an `close_all` call completes
			 * @event
			 * @name close_all.jstree
			 * @param {Object} node the closed node
			 */
			this.trigger('close_all', { "node": obj });
		},
		/**
		 * checks if a node is disabled (not selectable)
		 * @name is_disabled(obj)
		 * @param  {mixed} obj
		 * @return {Boolean}
		 */
		is_disabled: function (obj) {
			obj = this.get_node(obj);
			return obj && obj.state && obj.state.disabled;
		},
		/**
		 * enables a node - so that it can be selected
		 * @name enable_node(obj)
		 * @param {mixed} obj the node to enable
		 * @trigger enable_node.jstree
		 */
		enable_node: function (obj) {
			var t1, t2;
			if ($.isArray(obj)) {
				obj = obj.slice();
				for (t1 = 0, t2 = obj.length; t1 < t2; t1++) {
					this.enable_node(obj[t1]);
				}
				return true;
			}
			obj = this.get_node(obj);
			if (!obj || obj.id === '#') {
				return false;
			}
			obj.state.disabled = false;
			this.get_node(obj, true).children('.jstree-anchor').removeClass('jstree-disabled');
			/**
			 * triggered when an node is enabled
			 * @event
			 * @name enable_node.jstree
			 * @param {Object} node the enabled node
			 */
			this.trigger('enable_node', { 'node': obj });
		},
		/**
		 * disables a node - so that it can not be selected
		 * @name disable_node(obj)
		 * @param {mixed} obj the node to disable
		 * @trigger disable_node.jstree
		 */
		disable_node: function (obj) {
			var t1, t2;
			if ($.isArray(obj)) {
				obj = obj.slice();
				for (t1 = 0, t2 = obj.length; t1 < t2; t1++) {
					this.disable_node(obj[t1]);
				}
				return true;
			}
			obj = this.get_node(obj);
			if (!obj || obj.id === '#') {
				return false;
			}
			obj.state.disabled = true;
			this.get_node(obj, true).children('.jstree-anchor').addClass('jstree-disabled');
			/**
			 * triggered when an node is disabled
			 * @event
			 * @name disable_node.jstree
			 * @param {Object} node the disabled node
			 */
			this.trigger('disable_node', { 'node': obj });
		},
		/**
		 * called when a node is selected by the user. Used internally.
		 * @private
		 * @name activate_node(obj, e)
		 * @param {mixed} obj the node
		 * @param {Object} e the related event
		 * @trigger activate_node.jstree
		 */
		activate_node: function (obj, e) {
			if (this.is_disabled(obj)) {
				return false;
			}

			// ensure last_clicked is still in the DOM, make it fresh (maybe it was moved?) and make sure it is still selected, if not - make last_clicked the last selected node
			this._data.core.last_clicked = this._data.core.last_clicked && this._data.core.last_clicked.id !== undefined ? this.get_node(this._data.core.last_clicked.id) : null;
			if (this._data.core.last_clicked && !this._data.core.last_clicked.state.selected) { this._data.core.last_clicked = null; }
			if (!this._data.core.last_clicked && this._data.core.selected.length) { this._data.core.last_clicked = this.get_node(this._data.core.selected[this._data.core.selected.length - 1]); }

			if (!this.settings.core.multiple || (!e.metaKey && !e.ctrlKey && !e.shiftKey) || (e.shiftKey && (!this._data.core.last_clicked || !this.get_parent(obj) || this.get_parent(obj) !== this._data.core.last_clicked.parent))) {
				if (!this.settings.core.multiple && (e.metaKey || e.ctrlKey || e.shiftKey) && this.is_selected(obj)) {
					this.deselect_node(obj, false, false, e);
				}
				else {
					this.deselect_all(true);
					this.select_node(obj, false, false, e);
					this._data.core.last_clicked = this.get_node(obj);
				}
			}
			else {
				if (e.shiftKey) {
					var o = this.get_node(obj).id,
						l = this._data.core.last_clicked.id,
						p = this.get_node(this._data.core.last_clicked.parent).children,
						c = false,
						i, j;
					for (i = 0, j = p.length; i < j; i += 1) {
						// separate IFs work whem o and l are the same
						if (p[i] === o) {
							c = !c;
						}
						if (p[i] === l) {
							c = !c;
						}
						if (c || p[i] === o || p[i] === l) {
							this.select_node(p[i], false, false, e);
						}
						else {
							this.deselect_node(p[i], false, false, e);
						}
					}
				}
				else {
					if (!this.is_selected(obj)) {
						this.select_node(obj, false, false, e);
					}
					else {
						this.deselect_node(obj, false, false, e);
					}
				}
			}
			/**
			 * triggered when an node is clicked or intercated with by the user
			 * @event
			 * @name activate_node.jstree
			 * @param {Object} node
			 */
			this.trigger('activate_node', { 'node': this.get_node(obj) });
		},
		/**
		 * applies the hover state on a node, called when a node is hovered by the user. Used internally.
		 * @private
		 * @name hover_node(obj)
		 * @param {mixed} obj
		 * @trigger hover_node.jstree
		 */
		hover_node: function (obj) {
			obj = this.get_node(obj, true);
			if (!obj || !obj.length || obj.children('.jstree-hovered').length) {
				return false;
			}
			var o = this.element.find('.jstree-hovered'), t = this.element;
			if (o && o.length) { this.dehover_node(o); }

			obj.children('.jstree-anchor').addClass('jstree-hovered');
			/**
			 * triggered when an node is hovered
			 * @event
			 * @name hover_node.jstree
			 * @param {Object} node
			 */
			this.trigger('hover_node', { 'node': this.get_node(obj) });
			setTimeout(function () { t.attr('aria-activedescendant', obj[0].id); obj.attr('aria-selected', true); }, 0);
		},
		/**
		 * removes the hover state from a nodecalled when a node is no longer hovered by the user. Used internally.
		 * @private
		 * @name dehover_node(obj)
		 * @param {mixed} obj
		 * @trigger dehover_node.jstree
		 */
		dehover_node: function (obj) {
			obj = this.get_node(obj, true);
			if (!obj || !obj.length || !obj.children('.jstree-hovered').length) {
				return false;
			}
			obj.attr('aria-selected', false).children('.jstree-anchor').removeClass('jstree-hovered');
			/**
			 * triggered when an node is no longer hovered
			 * @event
			 * @name dehover_node.jstree
			 * @param {Object} node
			 */
			this.trigger('dehover_node', { 'node': this.get_node(obj) });
		},
		/**
		 * select a node
		 * @name select_node(obj [, supress_event, prevent_open])
		 * @param {mixed} obj an array can be used to select multiple nodes
		 * @param {Boolean} supress_event if set to `true` the `changed.jstree` event won't be triggered
		 * @param {Boolean} prevent_open if set to `true` parents of the selected node won't be opened
		 * @trigger select_node.jstree, changed.jstree
		 */
		select_node: function (obj, supress_event, prevent_open, e) {
			var dom, t1, t2, th;
			if ($.isArray(obj)) {
				obj = obj.slice();
				for (t1 = 0, t2 = obj.length; t1 < t2; t1++) {
					this.select_node(obj[t1], supress_event, prevent_open, e);
				}
				return true;
			}
			obj = this.get_node(obj);
			if (!obj || obj.id === '#') {
				return false;
			}
			dom = this.get_node(obj, true);
			if (!obj.state.selected) {
				obj.state.selected = true;
				this._data.core.selected.push(obj.id);
				if (!prevent_open) {
					dom = this._open_to(obj);
				}
				if (dom && dom.length) {
					dom.children('.jstree-anchor').addClass('jstree-clicked');
				}
				/**
				 * triggered when an node is selected
				 * @event
				 * @name select_node.jstree
				 * @param {Object} node
				 * @param {Array} selected the current selection
				 * @param {Object} event the event (if any) that triggered this select_node
				 */
				this.trigger('select_node', { 'node': obj, 'selected': this._data.core.selected, 'event': e });
				if (!supress_event) {
					/**
					 * triggered when selection changes
					 * @event
					 * @name changed.jstree
					 * @param {Object} node
					 * @param {Object} action the action that caused the selection to change
					 * @param {Array} selected the current selection
					 * @param {Object} event the event (if any) that triggered this changed event
					 */
					this.trigger('changed', { 'action': 'select_node', 'node': obj, 'selected': this._data.core.selected, 'event': e });
				}
			}
		},
		/**
		 * deselect a node
		 * @name deselect_node(obj [, supress_event])
		 * @param {mixed} obj an array can be used to deselect multiple nodes
		 * @param {Boolean} supress_event if set to `true` the `changed.jstree` event won't be triggered
		 * @trigger deselect_node.jstree, changed.jstree
		 */
		deselect_node: function (obj, supress_event, e) {
			var t1, t2, dom;
			if ($.isArray(obj)) {
				obj = obj.slice();
				for (t1 = 0, t2 = obj.length; t1 < t2; t1++) {
					this.deselect_node(obj[t1], supress_event, e);
				}
				return true;
			}
			obj = this.get_node(obj);
			if (!obj || obj.id === '#') {
				return false;
			}
			dom = this.get_node(obj, true);
			if (obj.state.selected) {
				obj.state.selected = false;
				this._data.core.selected = $.vakata.array_remove_item(this._data.core.selected, obj.id);
				if (dom.length) {
					dom.children('.jstree-anchor').removeClass('jstree-clicked');
				}
				/**
				 * triggered when an node is deselected
				 * @event
				 * @name deselect_node.jstree
				 * @param {Object} node
				 * @param {Array} selected the current selection
				 * @param {Object} event the event (if any) that triggered this deselect_node
				 */
				this.trigger('deselect_node', { 'node': obj, 'selected': this._data.core.selected, 'event': e });
				if (!supress_event) {
					this.trigger('changed', { 'action': 'deselect_node', 'node': obj, 'selected': this._data.core.selected, 'event': e });
				}
			}
		},
		/**
		 * select all nodes in the tree
		 * @name select_all([supress_event])
		 * @param {Boolean} supress_event if set to `true` the `changed.jstree` event won't be triggered
		 * @trigger select_all.jstree, changed.jstree
		 */
		select_all: function (supress_event) {
			var tmp = this._data.core.selected.concat([]), i, j;
			this._data.core.selected = this._model.data['#'].children_d.concat();
			for (i = 0, j = this._data.core.selected.length; i < j; i++) {
				if (this._model.data[this._data.core.selected[i]]) {
					this._model.data[this._data.core.selected[i]].state.selected = true;
				}
			}
			this.redraw(true);
			/**
			 * triggered when all nodes are selected
			 * @event
			 * @name select_all.jstree
			 * @param {Array} selected the current selection
			 */
			this.trigger('select_all', { 'selected': this._data.core.selected });
			if (!supress_event) {
				this.trigger('changed', { 'action': 'select_all', 'selected': this._data.core.selected, 'old_selection': tmp });
			}
		},
		/**
		 * deselect all selected nodes
		 * @name deselect_all([supress_event])
		 * @param {Boolean} supress_event if set to `true` the `changed.jstree` event won't be triggered
		 * @trigger deselect_all.jstree, changed.jstree
		 */
		deselect_all: function (supress_event) {
			var tmp = this._data.core.selected.concat([]), i, j;
			for (i = 0, j = this._data.core.selected.length; i < j; i++) {
				if (this._model.data[this._data.core.selected[i]]) {
					this._model.data[this._data.core.selected[i]].state.selected = false;
				}
			}
			this._data.core.selected = [];
			this.element.find('.jstree-clicked').removeClass('jstree-clicked');
			/**
			 * triggered when all nodes are deselected
			 * @event
			 * @name deselect_all.jstree
			 * @param {Object} node the previous selection
			 * @param {Array} selected the current selection
			 */
			this.trigger('deselect_all', { 'selected': this._data.core.selected, 'node': tmp });
			if (!supress_event) {
				this.trigger('changed', { 'action': 'deselect_all', 'selected': this._data.core.selected, 'old_selection': tmp });
			}
		},
		/**
		 * checks if a node is selected
		 * @name is_selected(obj)
		 * @param  {mixed}  obj
		 * @return {Boolean}
		 */
		is_selected: function (obj) {
			obj = this.get_node(obj);
			if (!obj || obj.id === '#') {
				return false;
			}
			return obj.state.selected;
		},
		/**
		 * get an array of all selected nodes
		 * @name get_selected([full])
		 * @param  {mixed}  full if set to `true` the returned array will consist of the full node objects, otherwise - only IDs will be returned
		 * @return {Array}
		 */
		get_selected: function (full) {
			return full ? $.map(this._data.core.selected, $.proxy(function (i) { return this.get_node(i); }, this)) : this._data.core.selected;
		},
		/**
		 * get an array of all top level selected nodes (ignoring children of selected nodes)
		 * @name get_top_selected([full])
		 * @param  {mixed}  full if set to `true` the returned array will consist of the full node objects, otherwise - only IDs will be returned
		 * @return {Array}
		 */
		get_top_selected: function (full) {
			var tmp = this.get_selected(true),
				obj = {}, i, j, k, l;
			for (i = 0, j = tmp.length; i < j; i++) {
				obj[tmp[i].id] = tmp[i];
			}
			for (i = 0, j = tmp.length; i < j; i++) {
				for (k = 0, l = tmp[i].children_d.length; k < l; k++) {
					if (obj[tmp[i].children_d[k]]) {
						delete obj[tmp[i].children_d[k]];
					}
				}
			}
			tmp = [];
			for (i in obj) {
				if (obj.hasOwnProperty(i)) {
					tmp.push(i);
				}
			}
			return full ? $.map(tmp, $.proxy(function (i) { return this.get_node(i); }, this)) : tmp;
		},
		/**
		 * get an array of all bottom level selected nodes (ignoring selected parents)
		 * @name get_top_selected([full])
		 * @param  {mixed}  full if set to `true` the returned array will consist of the full node objects, otherwise - only IDs will be returned
		 * @return {Array}
		 */
		get_bottom_selected: function (full) {
			var tmp = this.get_selected(true),
				obj = [], i, j;
			for (i = 0, j = tmp.length; i < j; i++) {
				if (!tmp[i].children.length) {
					obj.push(tmp[i].id);
				}
			}
			return full ? $.map(obj, $.proxy(function (i) { return this.get_node(i); }, this)) : obj;
		},
		/**
		 * gets the current state of the tree so that it can be restored later with `set_state(state)`. Used internally.
		 * @name get_state()
		 * @private
		 * @return {Object}
		 */
		get_state: function () {
			var state = {
				'core': {
					'open': [],
					'scroll': {
						'left': this.element.scrollLeft(),
						'top': this.element.scrollTop()
					},
					/*
					'themes' : {
						'name' : this.get_theme(),
						'icons' : this._data.core.themes.icons,
						'dots' : this._data.core.themes.dots
					},
					*/
					'selected': []
				}
			}, i;
			for (i in this._model.data) {
				if (this._model.data.hasOwnProperty(i)) {
					if (i !== '#') {
						if (this._model.data[i].state.opened) {
							state.core.open.push(i);
						}
						if (this._model.data[i].state.selected) {
							state.core.selected.push(i);
						}
					}
				}
			}
			return state;
		},
		/**
		 * sets the state of the tree. Used internally.
		 * @name set_state(state [, callback])
		 * @private
		 * @param {Object} state the state to restore
		 * @param {Function} callback an optional function to execute once the state is restored.
		 * @trigger set_state.jstree
		 */
		set_state: function (state, callback) {
			if (state) {
				if (state.core) {
					var res, n, t, _this;
					if (state.core.open) {
						if (!$.isArray(state.core.open)) {
							delete state.core.open;
							this.set_state(state, callback);
							return false;
						}
						res = true;
						n = false;
						t = this;
						$.each(state.core.open.concat([]), function (i, v) {
							n = t.get_node(v);
							if (n) {
								if (t.is_loaded(v)) {
									if (t.is_closed(v)) {
										t.open_node(v, false, 0);
									}
									if (state && state.core && state.core.open) {
										$.vakata.array_remove_item(state.core.open, v);
									}
								}
								else {
									if (!t.is_loading(v)) {
										t.open_node(v, $.proxy(function (o, s) {
											if (!s && state && state.core && state.core.open) {
												$.vakata.array_remove_item(state.core.open, o.id);
											}
											this.set_state(state, callback);
										}, t), 0);
									}
									// there will be some async activity - so wait for it
									res = false;
								}
							}
						});
						if (res) {
							delete state.core.open;
							this.set_state(state, callback);
						}
						return false;
					}
					if (state.core.scroll) {
						if (state.core.scroll && state.core.scroll.left !== undefined) {
							this.element.scrollLeft(state.core.scroll.left);
						}
						if (state.core.scroll && state.core.scroll.top !== undefined) {
							this.element.scrollTop(state.core.scroll.top);
						}
						delete state.core.scroll;
						this.set_state(state, callback);
						return false;
					}
					/*
					if(state.core.themes) {
						if(state.core.themes.name) {
							this.set_theme(state.core.themes.name);
						}
						if(typeof state.core.themes.dots !== 'undefined') {
							this[ state.core.themes.dots ? "show_dots" : "hide_dots" ]();
						}
						if(typeof state.core.themes.icons !== 'undefined') {
							this[ state.core.themes.icons ? "show_icons" : "hide_icons" ]();
						}
						delete state.core.themes;
						delete state.core.open;
						this.set_state(state, callback);
						return false;
					}
					*/
					if (state.core.selected) {
						_this = this;
						this.deselect_all();
						$.each(state.core.selected, function (i, v) {
							_this.select_node(v);
						});
						delete state.core.selected;
						this.set_state(state, callback);
						return false;
					}
					if ($.isEmptyObject(state.core)) {
						delete state.core;
						this.set_state(state, callback);
						return false;
					}
				}
				if ($.isEmptyObject(state)) {
					state = null;
					if (callback) { callback.call(this); }
					/**
					 * triggered when a `set_state` call completes
					 * @event
					 * @name set_state.jstree
					 */
					this.trigger('set_state');
					return false;
				}
				return true;
			}
			return false;
		},
		/**
		 * refreshes the tree - all nodes are reloaded with calls to `load_node`.
		 * @name refresh()
		 * @param {Boolean} skip_loading an option to skip showing the loading indicator
		 * @trigger refresh.jstree
		 */
		refresh: function (skip_loading) {
			this._data.core.state = this.get_state();
			this._cnt = 0;
			this._model.data = {
				'#': {
					id: '#',
					parent: null,
					parents: [],
					children: [],
					children_d: [],
					state: { loaded: false }
				}
			};
			var c = this.get_container_ul()[0].className;
			if (!skip_loading) {
				this.element.html("<" + "ul class='jstree-container-ul'><" + "li class='jstree-initial-node jstree-loading jstree-leaf jstree-last'><i class='jstree-icon jstree-ocl'></i><" + "a class='jstree-anchor' href='#'><i class='jstree-icon jstree-themeicon-hidden'></i>" + this.get_string("Loading ...") + "</a></li></ul>");
			}
			this.load_node('#', function (o, s) {
				if (s) {
					this.get_container_ul()[0].className = c;
					this.set_state($.extend(true, {}, this._data.core.state), function () {
						/**
						 * triggered when a `refresh` call completes
						 * @event
						 * @name refresh.jstree
						 */
						this.trigger('refresh');
					});
				}
				this._data.core.state = null;
			});
		},
		/**
		 * refreshes a node in the tree (reload its children) all opened nodes inside that node are reloaded with calls to `load_node`.
		 * @name refresh_name(obj)
		 * @param {Boolean} skip_loading an option to skip showing the loading indicator
		 * @trigger refresh.jstree
		 */
		refresh_node: function (obj) {
			obj = this.get_node(obj);
			if (!obj || obj.id === '#') { return false; }
			var opened = [], s = this._data.core.selected.concat([]);
			if (obj.state.opened === true) { opened.push(obj.id); }
			this.get_node(obj, true).find('.jstree-open').each(function () { opened.push(this.id); });
			this._load_nodes(opened, $.proxy(function (nodes) {
				this.open_node(nodes, false, 0);
				this.select_node(this._data.core.selected);
				/**
				 * triggered when a node is refreshed
				 * @event
				 * @name move_node.jstree
				 * @param {Object} node - the refreshed node
				 * @param {Array} nodes - an array of the IDs of the nodes that were reloaded
				 */
				this.trigger('refresh_node', { 'node': obj, 'nodes': nodes });
			}, this));
		},
		/**
		 * set (change) the ID of a node
		 * @name set_id(obj, id)
		 * @param  {mixed} obj the node
		 * @param  {String} id the new ID
		 * @return {Boolean}
		 */
		set_id: function (obj, id) {
			obj = this.get_node(obj);
			if (!obj || obj.id === '#') { return false; }
			var i, j, m = this._model.data;
			id = id.toString();
			// update parents (replace current ID with new one in children and children_d)
			m[obj.parent].children[$.inArray(obj.id, m[obj.parent].children)] = id;
			for (i = 0, j = obj.parents.length; i < j; i++) {
				m[obj.parents[i]].children_d[$.inArray(obj.id, m[obj.parents[i]].children_d)] = id;
			}
			// update children (replace current ID with new one in parent and parents)
			for (i = 0, j = obj.children.length; i < j; i++) {
				m[obj.children[i]].parent = id;
			}
			for (i = 0, j = obj.children_d.length; i < j; i++) {
				m[obj.children_d[i]].parents[$.inArray(obj.id, m[obj.children_d[i]].parents)] = id;
			}
			i = $.inArray(obj.id, this._data.core.selected);
			if (i !== -1) { this._data.core.selected[i] = id; }
			// update model and obj itself (obj.id, this._model.data[KEY])
			i = this.get_node(obj.id, true);
			if (i) {
				i.attr('id', id);
			}
			delete m[obj.id];
			obj.id = id;
			m[id] = obj;
			return true;
		},
		/**
		 * get the text value of a node
		 * @name get_text(obj)
		 * @param  {mixed} obj the node
		 * @return {String}
		 */
		get_text: function (obj) {
			obj = this.get_node(obj);
			return (!obj || obj.id === '#') ? false : obj.text;
		},
		/**
		 * set the text value of a node. Used internally, please use `rename_node(obj, val)`.
		 * @private
		 * @name set_text(obj, val)
		 * @param  {mixed} obj the node, you can pass an array to set the text on multiple nodes
		 * @param  {String} val the new text value
		 * @return {Boolean}
		 * @trigger set_text.jstree
		 */
		set_text: function (obj, val) {
			var t1, t2, dom, tmp;
			if ($.isArray(obj)) {
				obj = obj.slice();
				for (t1 = 0, t2 = obj.length; t1 < t2; t1++) {
					this.set_text(obj[t1], val);
				}
				return true;
			}
			obj = this.get_node(obj);
			if (!obj || obj.id === '#') { return false; }
			obj.text = val;
			dom = this.get_node(obj, true);
			if (dom.length) {
				dom = dom.children(".jstree-anchor:eq(0)");
				tmp = dom.children("I").clone();
				dom.html(val).prepend(tmp);
				/**
				 * triggered when a node text value is changed
				 * @event
				 * @name set_text.jstree
				 * @param {Object} obj
				 * @param {String} text the new value
				 */
				this.trigger('set_text', { "obj": obj, "text": val });
			}
			return true;
		},
		/**
		 * gets a JSON representation of a node (or the whole tree)
		 * @name get_json([obj, options])
		 * @param  {mixed} obj
		 * @param  {Object} options
		 * @param  {Boolean} options.no_state do not return state information
		 * @param  {Boolean} options.no_id do not return ID
		 * @param  {Boolean} options.no_children do not include children
		 * @param  {Boolean} options.no_data do not include node data
		 * @param  {Boolean} options.flat return flat JSON instead of nested
		 * @return {Object}
		 */
		get_json: function (obj, options, flat) {
			obj = this.get_node(obj || '#');
			if (!obj) { return false; }
			if (options && options.flat && !flat) { flat = []; }
			var tmp = {
				'id': obj.id,
				'text': obj.text,
				'icon': this.get_icon(obj),
				'li_attr': obj.li_attr,
				'a_attr': obj.a_attr,
				'state': {},
				'data': options && options.no_data ? false : obj.data
				//( this.get_node(obj, true).length ? this.get_node(obj, true).data() : obj.data ),
			}, i, j;
			if (options && options.flat) {
				tmp.parent = obj.parent;
			}
			else {
				tmp.children = [];
			}
			if (!options || !options.no_state) {
				for (i in obj.state) {
					if (obj.state.hasOwnProperty(i)) {
						tmp.state[i] = obj.state[i];
					}
				}
			}
			if (options && options.no_id) {
				delete tmp.id;
				if (tmp.li_attr && tmp.li_attr.id) {
					delete tmp.li_attr.id;
				}
			}
			if (options && options.flat && obj.id !== '#') {
				flat.push(tmp);
			}
			if (!options || !options.no_children) {
				for (i = 0, j = obj.children.length; i < j; i++) {
					if (options && options.flat) {
						this.get_json(obj.children[i], options, flat);
					}
					else {
						tmp.children.push(this.get_json(obj.children[i], options));
					}
				}
			}
			return options && options.flat ? flat : (obj.id === '#' ? tmp.children : tmp);
		},
		/**
		 * create a new node (do not confuse with load_node)
		 * @name create_node([obj, node, pos, callback, is_loaded])
		 * @param  {mixed}   par       the parent node (to create a root node use either "#" (string) or `null`)
		 * @param  {mixed}   node      the data for the new node (a valid JSON object, or a simple string with the name)
		 * @param  {mixed}   pos       the index at which to insert the node, "first" and "last" are also supported, default is "last"
		 * @param  {Function} callback a function to be called once the node is created
		 * @param  {Boolean} is_loaded internal argument indicating if the parent node was succesfully loaded
		 * @return {String}            the ID of the newly create node
		 * @trigger model.jstree, create_node.jstree
		 */
		create_node: function (par, node, pos, callback, is_loaded) {
			if (par === null) { par = "#"; }
			par = this.get_node(par);
			if (!par) { return false; }
			pos = pos === undefined ? "last" : pos;
			if (!pos.toString().match(/^(before|after)$/) && !is_loaded && !this.is_loaded(par)) {
				return this.load_node(par, function () { this.create_node(par, node, pos, callback, true); });
			}
			if (!node) { node = { "text": this.get_string('New node') }; }
			if (node.text === undefined) { node.text = this.get_string('New node'); }
			var tmp, dpc, i, j;

			if (par.id === '#') {
				if (pos === "before") { pos = "first"; }
				if (pos === "after") { pos = "last"; }
			}
			switch (pos) {
				case "before":
					tmp = this.get_node(par.parent);
					pos = $.inArray(par.id, tmp.children);
					par = tmp;
					break;
				case "after":
					tmp = this.get_node(par.parent);
					pos = $.inArray(par.id, tmp.children) + 1;
					par = tmp;
					break;
				case "inside":
				case "first":
					pos = 0;
					break;
				case "last":
					pos = par.children.length;
					break;
				default:
					if (!pos) { pos = 0; }
					break;
			}
			if (pos > par.children.length) { pos = par.children.length; }
			if (!node.id) { node.id = true; }
			if (!this.check("create_node", node, par, pos)) {
				this.settings.core.error.call(this, this._data.core.last_error);
				return false;
			}
			if (node.id === true) { delete node.id; }
			node = this._parse_model_from_json(node, par.id, par.parents.concat());
			if (!node) { return false; }
			tmp = this.get_node(node);
			dpc = [];
			dpc.push(node);
			dpc = dpc.concat(tmp.children_d);
			this.trigger('model', { "nodes": dpc, "parent": par.id });

			par.children_d = par.children_d.concat(dpc);
			for (i = 0, j = par.parents.length; i < j; i++) {
				this._model.data[par.parents[i]].children_d = this._model.data[par.parents[i]].children_d.concat(dpc);
			}
			node = tmp;
			tmp = [];
			for (i = 0, j = par.children.length; i < j; i++) {
				tmp[i >= pos ? i + 1 : i] = par.children[i];
			}
			tmp[pos] = node.id;
			par.children = tmp;

			this.redraw_node(par, true);
			if (callback) { callback.call(this, this.get_node(node)); }
			/**
			 * triggered when a node is created
			 * @event
			 * @name create_node.jstree
			 * @param {Object} node
			 * @param {String} parent the parent's ID
			 * @param {Number} position the position of the new node among the parent's children
			 */
			this.trigger('create_node', { "node": this.get_node(node), "parent": par.id, "position": pos });
			return node.id;
		},
		/**
		 * set the text value of a node
		 * @name rename_node(obj, val)
		 * @param  {mixed} obj the node, you can pass an array to rename multiple nodes to the same name
		 * @param  {String} val the new text value
		 * @return {Boolean}
		 * @trigger rename_node.jstree
		 */
		rename_node: function (obj, val) {
			var t1, t2, old;
			if ($.isArray(obj)) {
				obj = obj.slice();
				for (t1 = 0, t2 = obj.length; t1 < t2; t1++) {
					this.rename_node(obj[t1], val);
				}
				return true;
			}
			obj = this.get_node(obj);
			if (!obj || obj.id === '#') { return false; }
			old = obj.text;
			if (!this.check("rename_node", obj, this.get_parent(obj), val)) {
				this.settings.core.error.call(this, this._data.core.last_error);
				return false;
			}
			this.set_text(obj, val); // .apply(this, Array.prototype.slice.call(arguments))
			/**
			 * triggered when a node is renamed
			 * @event
			 * @name rename_node.jstree
			 * @param {Object} node
			 * @param {String} text the new value
			 * @param {String} old the old value
			 */
			this.trigger('rename_node', { "node": obj, "text": val, "old": old });
			return true;
		},
		/**
		 * remove a node
		 * @name delete_node(obj)
		 * @param  {mixed} obj the node, you can pass an array to delete multiple nodes
		 * @return {Boolean}
		 * @trigger delete_node.jstree, changed.jstree
		 */
		delete_node: function (obj) {
			var t1, t2, par, pos, tmp, i, j, k, l, c;
			if ($.isArray(obj)) {
				obj = obj.slice();
				for (t1 = 0, t2 = obj.length; t1 < t2; t1++) {
					this.delete_node(obj[t1]);
				}
				return true;
			}
			obj = this.get_node(obj);
			if (!obj || obj.id === '#') { return false; }
			par = this.get_node(obj.parent);
			pos = $.inArray(obj.id, par.children);
			c = false;
			if (!this.check("delete_node", obj, par, pos)) {
				this.settings.core.error.call(this, this._data.core.last_error);
				return false;
			}
			if (pos !== -1) {
				par.children = $.vakata.array_remove(par.children, pos);
			}
			tmp = obj.children_d.concat([]);
			tmp.push(obj.id);
			for (k = 0, l = tmp.length; k < l; k++) {
				for (i = 0, j = obj.parents.length; i < j; i++) {
					pos = $.inArray(tmp[k], this._model.data[obj.parents[i]].children_d);
					if (pos !== -1) {
						this._model.data[obj.parents[i]].children_d = $.vakata.array_remove(this._model.data[obj.parents[i]].children_d, pos);
					}
				}
				if (this._model.data[tmp[k]].state.selected) {
					c = true;
					pos = $.inArray(tmp[k], this._data.core.selected);
					if (pos !== -1) {
						this._data.core.selected = $.vakata.array_remove(this._data.core.selected, pos);
					}
				}
			}
			/**
			 * triggered when a node is deleted
			 * @event
			 * @name delete_node.jstree
			 * @param {Object} node
			 * @param {String} parent the parent's ID
			 */
			this.trigger('delete_node', { "node": obj, "parent": par.id });
			if (c) {
				this.trigger('changed', { 'action': 'delete_node', 'node': obj, 'selected': this._data.core.selected, 'parent': par.id });
			}
			for (k = 0, l = tmp.length; k < l; k++) {
				delete this._model.data[tmp[k]];
			}
			this.redraw_node(par, true);
			return true;
		},
		/**
		 * check if an operation is premitted on the tree. Used internally.
		 * @private
		 * @name check(chk, obj, par, pos)
		 * @param  {String} chk the operation to check, can be "create_node", "rename_node", "delete_node", "copy_node" or "move_node"
		 * @param  {mixed} obj the node
		 * @param  {mixed} par the parent
		 * @param  {mixed} pos the position to insert at, or if "rename_node" - the new name
		 * @param  {mixed} more some various additional information, for example if a "move_node" operations is triggered by DND this will be the hovered node
		 * @return {Boolean}
		 */
		check: function (chk, obj, par, pos, more) {
			obj = obj && obj.id ? obj : this.get_node(obj);
			par = par && par.id ? par : this.get_node(par);
			var tmp = chk.match(/^move_node|copy_node|create_node$/i) ? par : obj,
				chc = this.settings.core.check_callback;
			if (chk === "move_node" || chk === "copy_node") {
				if ((!more || !more.is_multi) && (obj.id === par.id || $.inArray(obj.id, par.children) === pos || $.inArray(par.id, obj.children_d) !== -1)) {
					this._data.core.last_error = { 'error': 'check', 'plugin': 'core', 'id': 'core_01', 'reason': 'Moving parent inside child', 'data': JSON.stringify({ 'chk': chk, 'pos': pos, 'obj': obj && obj.id ? obj.id : false, 'par': par && par.id ? par.id : false }) };
					return false;
				}
			}
			if (tmp && tmp.data) { tmp = tmp.data; }
			if (tmp && tmp.functions && (tmp.functions[chk] === false || tmp.functions[chk] === true)) {
				if (tmp.functions[chk] === false) {
					this._data.core.last_error = { 'error': 'check', 'plugin': 'core', 'id': 'core_02', 'reason': 'Node data prevents function: ' + chk, 'data': JSON.stringify({ 'chk': chk, 'pos': pos, 'obj': obj && obj.id ? obj.id : false, 'par': par && par.id ? par.id : false }) };
				}
				return tmp.functions[chk];
			}
			if (chc === false || ($.isFunction(chc) && chc.call(this, chk, obj, par, pos, more) === false) || (chc && chc[chk] === false)) {
				this._data.core.last_error = { 'error': 'check', 'plugin': 'core', 'id': 'core_03', 'reason': 'User config for core.check_callback prevents function: ' + chk, 'data': JSON.stringify({ 'chk': chk, 'pos': pos, 'obj': obj && obj.id ? obj.id : false, 'par': par && par.id ? par.id : false }) };
				return false;
			}
			return true;
		},
		/**
		 * get the last error
		 * @name last_error()
		 * @return {Object}
		 */
		last_error: function () {
			return this._data.core.last_error;
		},
		/**
		 * move a node to a new parent
		 * @name move_node(obj, par [, pos, callback, is_loaded])
		 * @param  {mixed} obj the node to move, pass an array to move multiple nodes
		 * @param  {mixed} par the new parent
		 * @param  {mixed} pos the position to insert at (besides integer values, "first" and "last" are supported, as well as "before" and "after"), defaults to integer `0`
		 * @param  {function} callback a function to call once the move is completed, receives 3 arguments - the node, the new parent and the position
		 * @param  {Boolean} internal parameter indicating if the parent node has been loaded
		 * @trigger move_node.jstree
		 */
		move_node: function (obj, par, pos, callback, is_loaded) {
			var t1, t2, old_par, new_par, old_ins, is_multi, dpc, tmp, i, j, k, l, p;
			if ($.isArray(obj)) {
				obj = obj.reverse().slice();
				for (t1 = 0, t2 = obj.length; t1 < t2; t1++) {
					this.move_node(obj[t1], par, pos, callback, is_loaded);
				}
				return true;
			}
			obj = obj && obj.id ? obj : this.get_node(obj);
			par = this.get_node(par);
			pos = pos === undefined ? 0 : pos;

			if (!par || !obj || obj.id === '#') { return false; }
			if (!pos.toString().match(/^(before|after)$/) && !is_loaded && !this.is_loaded(par)) {
				return this.load_node(par, function () { this.move_node(obj, par, pos, callback, true); });
			}

			old_par = (obj.parent || '#').toString();
			new_par = (!pos.toString().match(/^(before|after)$/) || par.id === '#') ? par : this.get_node(par.parent);
			old_ins = obj.instance ? obj.instance : (this._model.data[obj.id] ? this : $.jstree.reference(obj.id));
			is_multi = !old_ins || !old_ins._id || (this._id !== old_ins._id);
			if (is_multi) {
				if (this.copy_node(obj, par, pos, callback, is_loaded)) {
					if (old_ins) { old_ins.delete_node(obj); }
					return true;
				}
				return false;
			}
			//var m = this._model.data;
			if (new_par.id === '#') {
				if (pos === "before") { pos = "first"; }
				if (pos === "after") { pos = "last"; }
			}
			switch (pos) {
				case "before":
					pos = $.inArray(par.id, new_par.children);
					break;
				case "after":
					pos = $.inArray(par.id, new_par.children) + 1;
					break;
				case "inside":
				case "first":
					pos = 0;
					break;
				case "last":
					pos = new_par.children.length;
					break;
				default:
					if (!pos) { pos = 0; }
					break;
			}
			if (pos > new_par.children.length) { pos = new_par.children.length; }
			if (!this.check("move_node", obj, new_par, pos, { 'core': true, 'is_multi': (old_ins && old_ins._id && old_ins._id !== this._id), 'is_foreign': (!old_ins || !old_ins._id) })) {
				this.settings.core.error.call(this, this._data.core.last_error);
				return false;
			}
			if (obj.parent === new_par.id) {
				dpc = new_par.children.concat();
				tmp = $.inArray(obj.id, dpc);
				if (tmp !== -1) {
					dpc = $.vakata.array_remove(dpc, tmp);
					if (pos > tmp) { pos--; }
				}
				tmp = [];
				for (i = 0, j = dpc.length; i < j; i++) {
					tmp[i >= pos ? i + 1 : i] = dpc[i];
				}
				tmp[pos] = obj.id;
				new_par.children = tmp;
				this._node_changed(new_par.id);
				this.redraw(new_par.id === '#');
			}
			else {
				// clean old parent and up
				tmp = obj.children_d.concat();
				tmp.push(obj.id);
				for (i = 0, j = obj.parents.length; i < j; i++) {
					dpc = [];
					p = old_ins._model.data[obj.parents[i]].children_d;
					for (k = 0, l = p.length; k < l; k++) {
						if ($.inArray(p[k], tmp) === -1) {
							dpc.push(p[k]);
						}
					}
					old_ins._model.data[obj.parents[i]].children_d = dpc;
				}
				old_ins._model.data[old_par].children = $.vakata.array_remove_item(old_ins._model.data[old_par].children, obj.id);

				// insert into new parent and up
				for (i = 0, j = new_par.parents.length; i < j; i++) {
					this._model.data[new_par.parents[i]].children_d = this._model.data[new_par.parents[i]].children_d.concat(tmp);
				}
				dpc = [];
				for (i = 0, j = new_par.children.length; i < j; i++) {
					dpc[i >= pos ? i + 1 : i] = new_par.children[i];
				}
				dpc[pos] = obj.id;
				new_par.children = dpc;
				new_par.children_d.push(obj.id);
				new_par.children_d = new_par.children_d.concat(obj.children_d);

				// update object
				obj.parent = new_par.id;
				tmp = new_par.parents.concat();
				tmp.unshift(new_par.id);
				p = obj.parents.length;
				obj.parents = tmp;

				// update object children
				tmp = tmp.concat();
				for (i = 0, j = obj.children_d.length; i < j; i++) {
					this._model.data[obj.children_d[i]].parents = this._model.data[obj.children_d[i]].parents.slice(0, p * -1);
					Array.prototype.push.apply(this._model.data[obj.children_d[i]].parents, tmp);
				}

				this._node_changed(old_par);
				this._node_changed(new_par.id);
				this.redraw(old_par === '#' || new_par.id === '#');
			}
			if (callback) { callback.call(this, obj, new_par, pos); }
			/**
			 * triggered when a node is moved
			 * @event
			 * @name move_node.jstree
			 * @param {Object} node
			 * @param {String} parent the parent's ID
			 * @param {Number} position the position of the node among the parent's children
			 * @param {String} old_parent the old parent of the node
			 * @param {Boolean} is_multi do the node and new parent belong to different instances
			 * @param {jsTree} old_instance the instance the node came from
			 * @param {jsTree} new_instance the instance of the new parent
			 */
			this.trigger('move_node', { "node": obj, "parent": new_par.id, "position": pos, "old_parent": old_par, 'is_multi': (old_ins && old_ins._id && old_ins._id !== this._id), 'is_foreign': (!old_ins || !old_ins._id), 'old_instance': old_ins, 'new_instance': this });
			return true;
		},
		/**
		 * copy a node to a new parent
		 * @name copy_node(obj, par [, pos, callback, is_loaded])
		 * @param  {mixed} obj the node to copy, pass an array to copy multiple nodes
		 * @param  {mixed} par the new parent
		 * @param  {mixed} pos the position to insert at (besides integer values, "first" and "last" are supported, as well as "before" and "after"), defaults to integer `0`
		 * @param  {function} callback a function to call once the move is completed, receives 3 arguments - the node, the new parent and the position
		 * @param  {Boolean} internal parameter indicating if the parent node has been loaded
		 * @trigger model.jstree copy_node.jstree
		 */
		copy_node: function (obj, par, pos, callback, is_loaded) {
			var t1, t2, dpc, tmp, i, j, node, old_par, new_par, old_ins, is_multi;
			if ($.isArray(obj)) {
				obj = obj.reverse().slice();
				for (t1 = 0, t2 = obj.length; t1 < t2; t1++) {
					this.copy_node(obj[t1], par, pos, callback, is_loaded);
				}
				return true;
			}
			obj = obj && obj.id ? obj : this.get_node(obj);
			par = this.get_node(par);
			pos = pos === undefined ? 0 : pos;

			if (!par || !obj || obj.id === '#') { return false; }
			if (!pos.toString().match(/^(before|after)$/) && !is_loaded && !this.is_loaded(par)) {
				return this.load_node(par, function () { this.copy_node(obj, par, pos, callback, true); });
			}

			old_par = (obj.parent || '#').toString();
			new_par = (!pos.toString().match(/^(before|after)$/) || par.id === '#') ? par : this.get_node(par.parent);
			old_ins = obj.instance ? obj.instance : (this._model.data[obj.id] ? this : $.jstree.reference(obj.id));
			is_multi = !old_ins || !old_ins._id || (this._id !== old_ins._id);
			if (new_par.id === '#') {
				if (pos === "before") { pos = "first"; }
				if (pos === "after") { pos = "last"; }
			}
			switch (pos) {
				case "before":
					pos = $.inArray(par.id, new_par.children);
					break;
				case "after":
					pos = $.inArray(par.id, new_par.children) + 1;
					break;
				case "inside":
				case "first":
					pos = 0;
					break;
				case "last":
					pos = new_par.children.length;
					break;
				default:
					if (!pos) { pos = 0; }
					break;
			}
			if (pos > new_par.children.length) { pos = new_par.children.length; }
			if (!this.check("copy_node", obj, new_par, pos, { 'core': true, 'is_multi': (old_ins && old_ins._id && old_ins._id !== this._id), 'is_foreign': (!old_ins || !old_ins._id) })) {
				this.settings.core.error.call(this, this._data.core.last_error);
				return false;
			}
			node = old_ins ? old_ins.get_json(obj, { no_id: true, no_data: true, no_state: true }) : obj;
			if (!node) { return false; }
			if (node.id === true) { delete node.id; }
			node = this._parse_model_from_json(node, new_par.id, new_par.parents.concat());
			if (!node) { return false; }
			tmp = this.get_node(node);
			if (obj && obj.state && obj.state.loaded === false) { tmp.state.loaded = false; }
			dpc = [];
			dpc.push(node);
			dpc = dpc.concat(tmp.children_d);
			this.trigger('model', { "nodes": dpc, "parent": new_par.id });

			// insert into new parent and up
			for (i = 0, j = new_par.parents.length; i < j; i++) {
				this._model.data[new_par.parents[i]].children_d = this._model.data[new_par.parents[i]].children_d.concat(dpc);
			}
			dpc = [];
			for (i = 0, j = new_par.children.length; i < j; i++) {
				dpc[i >= pos ? i + 1 : i] = new_par.children[i];
			}
			dpc[pos] = tmp.id;
			new_par.children = dpc;
			new_par.children_d.push(tmp.id);
			new_par.children_d = new_par.children_d.concat(tmp.children_d);

			this._node_changed(new_par.id);
			this.redraw(new_par.id === '#');
			if (callback) { callback.call(this, tmp, new_par, pos); }
			/**
			 * triggered when a node is copied
			 * @event
			 * @name copy_node.jstree
			 * @param {Object} node the copied node
			 * @param {Object} original the original node
			 * @param {String} parent the parent's ID
			 * @param {Number} position the position of the node among the parent's children
			 * @param {String} old_parent the old parent of the node
			 * @param {Boolean} is_multi do the node and new parent belong to different instances
			 * @param {jsTree} old_instance the instance the node came from
			 * @param {jsTree} new_instance the instance of the new parent
			 */
			this.trigger('copy_node', { "node": tmp, "original": obj, "parent": new_par.id, "position": pos, "old_parent": old_par, 'is_multi': (old_ins && old_ins._id && old_ins._id !== this._id), 'is_foreign': (!old_ins || !old_ins._id), 'old_instance': old_ins, 'new_instance': this });
			return tmp.id;
		},
		/**
		 * cut a node (a later call to `paste(obj)` would move the node)
		 * @name cut(obj)
		 * @param  {mixed} obj multiple objects can be passed using an array
		 * @trigger cut.jstree
		 */
		cut: function (obj) {
			if (!obj) { obj = this._data.core.selected.concat(); }
			if (!$.isArray(obj)) { obj = [obj]; }
			if (!obj.length) { return false; }
			var tmp = [], o, t1, t2;
			for (t1 = 0, t2 = obj.length; t1 < t2; t1++) {
				o = this.get_node(obj[t1]);
				if (o && o.id && o.id !== '#') { tmp.push(o); }
			}
			if (!tmp.length) { return false; }
			ccp_node = tmp;
			ccp_inst = this;
			ccp_mode = 'move_node';
			/**
			 * triggered when nodes are added to the buffer for moving
			 * @event
			 * @name cut.jstree
			 * @param {Array} node
			 */
			this.trigger('cut', { "node": obj });
		},
		/**
		 * copy a node (a later call to `paste(obj)` would copy the node)
		 * @name copy(obj)
		 * @param  {mixed} obj multiple objects can be passed using an array
		 * @trigger copy.jstre
		 */
		copy: function (obj) {
			if (!obj) { obj = this._data.core.selected.concat(); }
			if (!$.isArray(obj)) { obj = [obj]; }
			if (!obj.length) { return false; }
			var tmp = [], o, t1, t2;
			for (t1 = 0, t2 = obj.length; t1 < t2; t1++) {
				o = this.get_node(obj[t1]);
				if (o && o.id && o.id !== '#') { tmp.push(o); }
			}
			if (!tmp.length) { return false; }
			ccp_node = tmp;
			ccp_inst = this;
			ccp_mode = 'copy_node';
			/**
			 * triggered when nodes are added to the buffer for copying
			 * @event
			 * @name copy.jstree
			 * @param {Array} node
			 */
			this.trigger('copy', { "node": obj });
		},
		/**
		 * get the current buffer (any nodes that are waiting for a paste operation)
		 * @name get_buffer()
		 * @return {Object} an object consisting of `mode` ("copy_node" or "move_node"), `node` (an array of objects) and `inst` (the instance)
		 */
		get_buffer: function () {
			return { 'mode': ccp_mode, 'node': ccp_node, 'inst': ccp_inst };
		},
		/**
		 * check if there is something in the buffer to paste
		 * @name can_paste()
		 * @return {Boolean}
		 */
		can_paste: function () {
			return ccp_mode !== false && ccp_node !== false; // && ccp_inst._model.data[ccp_node];
		},
		/**
		 * copy or move the previously cut or copied nodes to a new parent
		 * @name paste(obj [, pos])
		 * @param  {mixed} obj the new parent
		 * @param  {mixed} pos the position to insert at (besides integer, "first" and "last" are supported), defaults to integer `0`
		 * @trigger paste.jstree
		 */
		paste: function (obj, pos) {
			obj = this.get_node(obj);
			if (!obj || !ccp_mode || !ccp_mode.match(/^(copy_node|move_node)$/) || !ccp_node) { return false; }
			if (this[ccp_mode](ccp_node, obj, pos)) {
				/**
				 * triggered when paste is invoked
				 * @event
				 * @name paste.jstree
				 * @param {String} parent the ID of the receiving node
				 * @param {Array} node the nodes in the buffer
				 * @param {String} mode the performed operation - "copy_node" or "move_node"
				 */
				this.trigger('paste', { "parent": obj.id, "node": ccp_node, "mode": ccp_mode });
			}
			ccp_node = false;
			ccp_mode = false;
			ccp_inst = false;
		},
		/**
		 * put a node in edit mode (input field to rename the node)
		 * @name edit(obj [, default_text])
		 * @param  {mixed} obj
		 * @param  {String} default_text the text to populate the input with (if omitted the node text value is used)
		 */
		edit: function (obj, default_text) {
			obj = this._open_to(obj);
			if (!obj || !obj.length) { return false; }
			var rtl = this._data.core.rtl,
				w = this.element.width(),
				a = obj.children('.jstree-anchor'),
				s = $('<span>'),
				/*
				oi = obj.children("i:visible"),
				ai = a.children("i:visible"),
				w1 = oi.width() * oi.length,
				w2 = ai.width() * ai.length,
				*/
				t = typeof default_text === 'string' ? default_text : this.get_text(obj),
				h1 = $("<" + "div />", { css: { "position": "absolute", "top": "-200px", "left": (rtl ? "0px" : "-1000px"), "visibility": "hidden" } }).appendTo("body"),
				h2 = $("<" + "input />", {
					"value": t,
					"class": "jstree-rename-input",
					// "size" : t.length,
					"css": {
						"padding": "0",
						"border": "1px solid silver",
						"box-sizing": "border-box",
						"display": "inline-block",
						"height": (this._data.core.li_height) + "px",
						"lineHeight": (this._data.core.li_height) + "px",
						"width": "150px" // will be set a bit further down
					},
					"blur": $.proxy(function () {
						var i = s.children(".jstree-rename-input"),
							v = i.val();
						if (v === "") { v = t; }
						h1.remove();
						s.replaceWith(a);
						s.remove();
						this.set_text(obj, t);
						if (this.rename_node(obj, v) === false) {
							this.set_text(obj, t); // move this up? and fix #483
						}
					}, this),
					"keydown": function (event) {
						var key = event.which;
						if (key === 27) {
							this.value = t;
						}
						if (key === 27 || key === 13 || key === 37 || key === 38 || key === 39 || key === 40 || key === 32) {
							event.stopImmediatePropagation();
						}
						if (key === 27 || key === 13) {
							event.preventDefault();
							this.blur();
						}
					},
					"click": function (e) { e.stopImmediatePropagation(); },
					"mousedown": function (e) { e.stopImmediatePropagation(); },
					"keyup": function (event) {
						h2.width(Math.min(h1.text("pW" + this.value).width(), w));
					},
					"keypress": function (event) {
						if (event.which === 13) { return false; }
					}
				}),
				fn = {
					fontFamily: a.css('fontFamily') || '',
					fontSize: a.css('fontSize') || '',
					fontWeight: a.css('fontWeight') || '',
					fontStyle: a.css('fontStyle') || '',
					fontStretch: a.css('fontStretch') || '',
					fontVariant: a.css('fontVariant') || '',
					letterSpacing: a.css('letterSpacing') || '',
					wordSpacing: a.css('wordSpacing') || ''
				};
			this.set_text(obj, "");
			s.attr('class', a.attr('class')).append(a.contents().clone()).append(h2);
			a.replaceWith(s);
			h1.css(fn);
			h2.css(fn).width(Math.min(h1.text("pW" + h2[0].value).width(), w))[0].select();
		},


		/**
		 * changes the theme
		 * @name set_theme(theme_name [, theme_url])
		 * @param {String} theme_name the name of the new theme to apply
		 * @param {mixed} theme_url  the location of the CSS file for this theme. Omit or set to `false` if you manually included the file. Set to `true` to autoload from the `core.themes.dir` directory.
		 * @trigger set_theme.jstree
		 */
		set_theme: function (theme_name, theme_url) {
			if (!theme_name) { return false; }
			if (theme_url === true) {
				var dir = this.settings.core.themes.dir;
				if (!dir) { dir = $.jstree.path + '/themes'; }
				theme_url = dir + '/' + theme_name + '/style.css';
			}
			if (theme_url && $.inArray(theme_url, themes_loaded) === -1) {
				$('head').append('<' + 'link rel="stylesheet" href="' + theme_url + '" type="text/css" />');
				themes_loaded.push(theme_url);
			}
			if (this._data.core.themes.name) {
				this.element.removeClass('jstree-' + this._data.core.themes.name);
			}
			this._data.core.themes.name = theme_name;
			this.element.addClass('jstree-' + theme_name);
			this.element[this.settings.core.themes.responsive ? 'addClass' : 'removeClass']('jstree-' + theme_name + '-responsive');
			/**
			 * triggered when a theme is set
			 * @event
			 * @name set_theme.jstree
			 * @param {String} theme the new theme
			 */
			this.trigger('set_theme', { 'theme': theme_name });
		},
		/**
		 * gets the name of the currently applied theme name
		 * @name get_theme()
		 * @return {String}
		 */
		get_theme: function () { return this._data.core.themes.name; },
		/**
		 * changes the theme variant (if the theme has variants)
		 * @name set_theme_variant(variant_name)
		 * @param {String|Boolean} variant_name the variant to apply (if `false` is used the current variant is removed)
		 */
		set_theme_variant: function (variant_name) {
			if (this._data.core.themes.variant) {
				this.element.removeClass('jstree-' + this._data.core.themes.name + '-' + this._data.core.themes.variant);
			}
			this._data.core.themes.variant = variant_name;
			if (variant_name) {
				this.element.addClass('jstree-' + this._data.core.themes.name + '-' + this._data.core.themes.variant);
			}
		},
		/**
		 * gets the name of the currently applied theme variant
		 * @name get_theme()
		 * @return {String}
		 */
		get_theme_variant: function () { return this._data.core.themes.variant; },
		/**
		 * shows a striped background on the container (if the theme supports it)
		 * @name show_stripes()
		 */
		show_stripes: function () { this._data.core.themes.stripes = true; this.get_container_ul().addClass("jstree-striped"); },
		/**
		 * hides the striped background on the container
		 * @name hide_stripes()
		 */
		hide_stripes: function () { this._data.core.themes.stripes = false; this.get_container_ul().removeClass("jstree-striped"); },
		/**
		 * toggles the striped background on the container
		 * @name toggle_stripes()
		 */
		toggle_stripes: function () { if (this._data.core.themes.stripes) { this.hide_stripes(); } else { this.show_stripes(); } },
		/**
		 * shows the connecting dots (if the theme supports it)
		 * @name show_dots()
		 */
		show_dots: function () { this._data.core.themes.dots = true; this.get_container_ul().removeClass("jstree-no-dots"); },
		/**
		 * hides the connecting dots
		 * @name hide_dots()
		 */
		hide_dots: function () { this._data.core.themes.dots = false; this.get_container_ul().addClass("jstree-no-dots"); },
		/**
		 * toggles the connecting dots
		 * @name toggle_dots()
		 */
		toggle_dots: function () { if (this._data.core.themes.dots) { this.hide_dots(); } else { this.show_dots(); } },
		/**
		 * show the node icons
		 * @name show_icons()
		 */
		show_icons: function () { this._data.core.themes.icons = true; this.get_container_ul().removeClass("jstree-no-icons"); },
		/**
		 * hide the node icons
		 * @name hide_icons()
		 */
		hide_icons: function () { this._data.core.themes.icons = false; this.get_container_ul().addClass("jstree-no-icons"); },
		/**
		 * toggle the node icons
		 * @name toggle_icons()
		 */
		toggle_icons: function () { if (this._data.core.themes.icons) { this.hide_icons(); } else { this.show_icons(); } },
		/**
		 * set the node icon for a node
		 * @name set_icon(obj, icon)
		 * @param {mixed} obj
		 * @param {String} icon the new icon - can be a path to an icon or a className, if using an image that is in the current directory use a `./` prefix, otherwise it will be detected as a class
		 */
		set_icon: function (obj, icon) {
			var t1, t2, dom, old;
			if ($.isArray(obj)) {
				obj = obj.slice();
				for (t1 = 0, t2 = obj.length; t1 < t2; t1++) {
					this.set_icon(obj[t1], icon);
				}
				return true;
			}
			obj = this.get_node(obj);
			if (!obj || obj.id === '#') { return false; }
			old = obj.icon;
			obj.icon = icon;
			dom = this.get_node(obj, true).children(".jstree-anchor").children(".jstree-themeicon");
			if (icon === false) {
				this.hide_icon(obj);
			}
			else if (icon === true) {
				dom.removeClass('jstree-themeicon-custom ' + old).css("background", "").removeAttr("rel");
			}
			else if (icon.indexOf("/") === -1 && icon.indexOf(".") === -1) {
				dom.removeClass(old).css("background", "");
				dom.addClass(icon + ' jstree-themeicon-custom').attr("rel", icon);
			}
			else {
				dom.removeClass(old).css("background", "");
				dom.addClass('jstree-themeicon-custom').css("background", "url('" + icon + "') center center no-repeat").attr("rel", icon);
			}
			return true;
		},
		/**
		 * get the node icon for a node
		 * @name get_icon(obj)
		 * @param {mixed} obj
		 * @return {String}
		 */
		get_icon: function (obj) {
			obj = this.get_node(obj);
			return (!obj || obj.id === '#') ? false : obj.icon;
		},
		/**
		 * hide the icon on an individual node
		 * @name hide_icon(obj)
		 * @param {mixed} obj
		 */
		hide_icon: function (obj) {
			var t1, t2;
			if ($.isArray(obj)) {
				obj = obj.slice();
				for (t1 = 0, t2 = obj.length; t1 < t2; t1++) {
					this.hide_icon(obj[t1]);
				}
				return true;
			}
			obj = this.get_node(obj);
			if (!obj || obj === '#') { return false; }
			obj.icon = false;
			this.get_node(obj, true).children("a").children(".jstree-themeicon").addClass('jstree-themeicon-hidden');
			return true;
		},
		/**
		 * show the icon on an individual node
		 * @name show_icon(obj)
		 * @param {mixed} obj
		 */
		show_icon: function (obj) {
			var t1, t2, dom;
			if ($.isArray(obj)) {
				obj = obj.slice();
				for (t1 = 0, t2 = obj.length; t1 < t2; t1++) {
					this.show_icon(obj[t1]);
				}
				return true;
			}
			obj = this.get_node(obj);
			if (!obj || obj === '#') { return false; }
			dom = this.get_node(obj, true);
			obj.icon = dom.length ? dom.children("a").children(".jstree-themeicon").attr('rel') : true;
			if (!obj.icon) { obj.icon = true; }
			dom.children("a").children(".jstree-themeicon").removeClass('jstree-themeicon-hidden');
			return true;
		}
	};

	// helpers
	$.vakata = {};
	// collect attributes
	$.vakata.attributes = function (node, with_values) {
		node = $(node)[0];
		var attr = with_values ? {} : [];
		if (node && node.attributes) {
			$.each(node.attributes, function (i, v) {
				if ($.inArray(v.nodeName.toLowerCase(), ['style', 'contenteditable', 'hasfocus', 'tabindex']) !== -1) { return; }
				if (v.nodeValue !== null && $.trim(v.nodeValue) !== '') {
					if (with_values) { attr[v.nodeName] = v.nodeValue; }
					else { attr.push(v.nodeName); }
				}
			});
		}
		return attr;
	};
	$.vakata.array_unique = function (array) {
		var a = [], i, j, l;
		for (i = 0, l = array.length; i < l; i++) {
			for (j = 0; j <= i; j++) {
				if (array[i] === array[j]) {
					break;
				}
			}
			if (j === i) { a.push(array[i]); }
		}
		return a;
	};
	// remove item from array
	$.vakata.array_remove = function (array, from, to) {
		var rest = array.slice((to || from) + 1 || array.length);
		array.length = from < 0 ? array.length + from : from;
		array.push.apply(array, rest);
		return array;
	};
	// remove item from array
	$.vakata.array_remove_item = function (array, item) {
		var tmp = $.inArray(item, array);
		return tmp !== -1 ? $.vakata.array_remove(array, tmp) : array;
	};
	// browser sniffing
	(function () {
		var browser = {},
			b_match = function (ua) {
				ua = ua.toLowerCase();

				var match = /(chrome)[ \/]([\w.]+)/.exec(ua) ||
							/(webkit)[ \/]([\w.]+)/.exec(ua) ||
							/(opera)(?:.*version|)[ \/]([\w.]+)/.exec(ua) ||
							/(msie) ([\w.]+)/.exec(ua) ||
							(ua.indexOf("compatible") < 0 && /(mozilla)(?:.*? rv:([\w.]+)|)/.exec(ua)) ||
							[];
				return {
					browser: match[1] || "",
					version: match[2] || "0"
				};
			},
			matched = b_match(window.navigator.userAgent);
		if (matched.browser) {
			browser[matched.browser] = true;
			browser.version = matched.version;
		}
		if (browser.chrome) {
			browser.webkit = true;
		}
		else if (browser.webkit) {
			browser.safari = true;
		}
		$.vakata.browser = browser;
	}());
	if ($.vakata.browser.msie && $.vakata.browser.version < 8) {
		$.jstree.defaults.core.animation = 0;
	}

	/**
	 * ### Checkbox plugin
	 *
	 * This plugin renders checkbox icons in front of each node, making multiple selection much easier. 
	 * It also supports tri-state behavior, meaning that if a node has a few of its children checked it will be rendered as undetermined, and state will be propagated up.
	 */

	var _i = document.createElement('I');
	_i.className = 'jstree-icon jstree-checkbox';
	/**
	 * stores all defaults for the checkbox plugin
	 * @name $.jstree.defaults.checkbox
	 * @plugin checkbox
	 */
	$.jstree.defaults.checkbox = {
		/**
		 * a boolean indicating if checkboxes should be visible (can be changed at a later time using `show_checkboxes()` and `hide_checkboxes`). Defaults to `true`.
		 * @name $.jstree.defaults.checkbox.visible
		 * @plugin checkbox
		 */
		visible: true,
		/**
		 * a boolean indicating if checkboxes should cascade down and have an undetermined state. Defaults to `true`.
		 * @name $.jstree.defaults.checkbox.three_state
		 * @plugin checkbox
		 */
		three_state: true,
		/**
		 * a boolean indicating if clicking anywhere on the node should act as clicking on the checkbox. Defaults to `true`.
		 * @name $.jstree.defaults.checkbox.whole_node
		 * @plugin checkbox
		 */
		whole_node: true,
		/**
		 * a boolean indicating if the selected style of a node should be kept, or removed. Defaults to `true`.
		 * @name $.jstree.defaults.checkbox.keep_selected_style
		 * @plugin checkbox
		 */
		keep_selected_style: true
	};
	$.jstree.plugins.checkbox = function (options, parent) {
		this.bind = function () {
			parent.bind.call(this);
			this._data.checkbox.uto = false;
			this.element
				.on("init.jstree", $.proxy(function () {
					this._data.checkbox.visible = this.settings.checkbox.visible;
					if (!this.settings.checkbox.keep_selected_style) {
						this.element.addClass('jstree-checkbox-no-clicked');
					}
				}, this))
				.on("loading.jstree", $.proxy(function () {
					this[this._data.checkbox.visible ? 'show_checkboxes' : 'hide_checkboxes']();
				}, this));
			if (this.settings.checkbox.three_state) {
				this.element
					.on('changed.jstree move_node.jstree copy_node.jstree redraw.jstree open_node.jstree', $.proxy(function () {
						if (this._data.checkbox.uto) { clearTimeout(this._data.checkbox.uto); }
						this._data.checkbox.uto = setTimeout($.proxy(this._undetermined, this), 50);
					}, this))
					.on('model.jstree', $.proxy(function (e, data) {
						var m = this._model.data,
							p = m[data.parent],
							dpc = data.nodes,
							chd = [],
							c, i, j, k, l, tmp;

						// apply down
						if (p.state.selected) {
							for (i = 0, j = dpc.length; i < j; i++) {
								m[dpc[i]].state.selected = true;
							}
							this._data.core.selected = this._data.core.selected.concat(dpc);
						}
						else {
							for (i = 0, j = dpc.length; i < j; i++) {
								if (m[dpc[i]].state.selected) {
									for (k = 0, l = m[dpc[i]].children_d.length; k < l; k++) {
										m[m[dpc[i]].children_d[k]].state.selected = true;
									}
									this._data.core.selected = this._data.core.selected.concat(m[dpc[i]].children_d);
								}
							}
						}

						// apply up
						for (i = 0, j = p.children_d.length; i < j; i++) {
							if (!m[p.children_d[i]].children.length) {
								chd.push(m[p.children_d[i]].parent);
							}
						}
						// [Roger] : Comment out these lines to prevent parents from getting selected
						//chd = $.vakata.array_unique(chd);
						//for (k = 0, l = chd.length; k < l; k++) {
						//	p = m[chd[k]];
						//	while (p && p.id !== '#') {
						//		c = 0;
						//		for (i = 0, j = p.children.length; i < j; i++) {
						//			c += m[p.children[i]].state.selected;
						//		}
						//		if (c === j) {
						//			p.state.selected = true;
						//			this._data.core.selected.push(p.id);
						//			tmp = this.get_node(p, true);
						//			if (tmp && tmp.length) {
						//				tmp.children('.jstree-anchor').addClass('jstree-clicked');
						//			}
						//		}
						//		else {
						//			break;
						//		}
						//		p = this.get_node(p.parent);
						//	}
						//}
						this._data.core.selected = $.vakata.array_unique(this._data.core.selected);
					}, this))
					.on('select_node.jstree', $.proxy(function (e, data) {
						var obj = data.node,
							m = this._model.data,
							par = this.get_node(obj.parent),
							dom = this.get_node(obj, true),
							i, j, c, tmp;
						this._data.core.selected = $.vakata.array_unique(this._data.core.selected.concat(obj.children_d));

						for (i = 0, j = obj.children_d.length; i < j; i++) {
							tmp = m[obj.children_d[i]];
							tmp.state.selected = true;
							if (tmp && tmp.original && tmp.original.state && tmp.original.state.undetermined) {
								tmp.original.state.undetermined = false;
							}
						}
						// [Roger] : Comment out these lines to prevent parents from getting selected
						//while (par && par.id !== '#') {
						//	c = 0;
						//	for (i = 0, j = par.children.length; i < j; i++) {
						//		c += m[par.children[i]].state.selected;
						//	}
						//	if (c === j) {
						//		par.state.selected = true;
						//		this._data.core.selected.push(par.id);
						//		tmp = this.get_node(par, true);
						//		if (tmp && tmp.length) {
						//			tmp.children('.jstree-anchor').addClass('jstree-clicked');
						//		}
						//	}
						//	else {
						//		break;
						//	}
						//	par = this.get_node(par.parent);
						//}
						if (dom.length) {
							dom.find('.jstree-anchor').addClass('jstree-clicked');
						}
					}, this))
					.on('deselect_all.jstree', $.proxy(function (e, data) {
						var obj = this.get_node('#'),
							m = this._model.data,
							i, j, tmp;
						for (i = 0, j = obj.children_d.length; i < j; i++) {
							tmp = m[obj.children_d[i]];
							if (tmp && tmp.original && tmp.original.state && tmp.original.state.undetermined) {
								tmp.original.state.undetermined = false;
							}
						}
					}, this))
					.on('deselect_node.jstree', $.proxy(function (e, data) {
						var obj = data.node,
							dom = this.get_node(obj, true),
							i, j, tmp;
						if (obj && obj.original && obj.original.state && obj.original.state.undetermined) {
							obj.original.state.undetermined = false;
						}
						for (i = 0, j = obj.children_d.length; i < j; i++) {
							tmp = this._model.data[obj.children_d[i]];
							tmp.state.selected = false;
							if (tmp && tmp.original && tmp.original.state && tmp.original.state.undetermined) {
								tmp.original.state.undetermined = false;
							}
						}
						for (i = 0, j = obj.parents.length; i < j; i++) {
							tmp = this._model.data[obj.parents[i]];
							tmp.state.selected = false;
							if (tmp && tmp.original && tmp.original.state && tmp.original.state.undetermined) {
								tmp.original.state.undetermined = false;
							}
							tmp = this.get_node(obj.parents[i], true);
							if (tmp && tmp.length) {
								tmp.children('.jstree-anchor').removeClass('jstree-clicked');
							}
						}
						tmp = [];
						for (i = 0, j = this._data.core.selected.length; i < j; i++) {
							if ($.inArray(this._data.core.selected[i], obj.children_d) === -1 && $.inArray(this._data.core.selected[i], obj.parents) === -1) {
								tmp.push(this._data.core.selected[i]);
							}
						}
						this._data.core.selected = $.vakata.array_unique(tmp);
						if (dom.length) {
							dom.find('.jstree-anchor').removeClass('jstree-clicked');
						}
					}, this))
					.on('delete_node.jstree', $.proxy(function (e, data) {
						var p = this.get_node(data.parent),
							m = this._model.data,
							i, j, c, tmp;
						while (p && p.id !== '#') {
							c = 0;
							for (i = 0, j = p.children.length; i < j; i++) {
								c += m[p.children[i]].state.selected;
							}
							if (c === j) {
								p.state.selected = true;
								this._data.core.selected.push(p.id);
								tmp = this.get_node(p, true);
								if (tmp && tmp.length) {
									tmp.children('.jstree-anchor').addClass('jstree-clicked');
								}
							}
							else {
								break;
							}
							p = this.get_node(p.parent);
						}
					}, this))
					.on('move_node.jstree', $.proxy(function (e, data) {
						var is_multi = data.is_multi,
							old_par = data.old_parent,
							new_par = this.get_node(data.parent),
							m = this._model.data,
							p, c, i, j, tmp;
						if (!is_multi) {
							p = this.get_node(old_par);
							while (p && p.id !== '#') {
								c = 0;
								for (i = 0, j = p.children.length; i < j; i++) {
									c += m[p.children[i]].state.selected;
								}
								if (c === j) {
									p.state.selected = true;
									this._data.core.selected.push(p.id);
									tmp = this.get_node(p, true);
									if (tmp && tmp.length) {
										tmp.children('.jstree-anchor').addClass('jstree-clicked');
									}
								}
								else {
									break;
								}
								p = this.get_node(p.parent);
							}
						}
						p = new_par;
						while (p && p.id !== '#') {
							c = 0;
							for (i = 0, j = p.children.length; i < j; i++) {
								c += m[p.children[i]].state.selected;
							}
							if (c === j) {
								if (!p.state.selected) {
									p.state.selected = true;
									this._data.core.selected.push(p.id);
									tmp = this.get_node(p, true);
									if (tmp && tmp.length) {
										tmp.children('.jstree-anchor').addClass('jstree-clicked');
									}
								}
							}
							else {
								if (p.state.selected) {
									p.state.selected = false;
									this._data.core.selected = $.vakata.array_remove_item(this._data.core.selected, p.id);
									tmp = this.get_node(p, true);
									if (tmp && tmp.length) {
										tmp.children('.jstree-anchor').removeClass('jstree-clicked');
									}
								}
								else {
									break;
								}
							}
							p = this.get_node(p.parent);
						}
					}, this));
			}
		};
		/**
		 * set the undetermined state where and if necessary. Used internally.
		 * @private
		 * @name _undetermined()
		 * @plugin checkbox
		 */
		this._undetermined = function () {
			return; // [Roger]: Added return to prevent parent checkboxes from showing in an undetermined state
			var i, j, m = this._model.data, s = this._data.core.selected, p = [], t = this;
			for (i = 0, j = s.length; i < j; i++) {
				if (m[s[i]] && m[s[i]].parents) {
					p = p.concat(m[s[i]].parents);
				}
			}
			// attempt for server side undetermined state
			this.element.find('.jstree-closed').not(':has(ul)')
				.each(function () {
					var tmp = t.get_node(this), tmp2;
					if (!tmp.state.loaded) {
						if (tmp.original && tmp.original.state && tmp.original.state.undetermined && tmp.original.state.undetermined === true) {
							p.push(tmp.id);
							p = p.concat(tmp.parents);
						}
					}
					else {
						for (i = 0, j = tmp.children_d.length; i < j; i++) {
							tmp2 = m[tmp.children_d[i]];
							if (!tmp2.state.loaded && tmp2.original && tmp2.original.state && tmp2.original.state.undetermined && tmp2.original.state.undetermined === true) {
								p.push(tmp2.id);
								p = p.concat(tmp2.parents);
							}
						}
					}
				});
			p = $.vakata.array_unique(p);
			p = $.vakata.array_remove_item(p, '#');

			this.element.find('.jstree-undetermined').removeClass('jstree-undetermined');
			for (i = 0, j = p.length; i < j; i++) {
				if (!m[p[i]].state.selected) {
					s = this.get_node(p[i], true);
					if (s && s.length) {
						s.children('a').children('.jstree-checkbox').addClass('jstree-undetermined');
					}
				}
			}
		};
		this.redraw_node = function (obj, deep, is_callback) {
			obj = parent.redraw_node.call(this, obj, deep, is_callback);
			if (obj) {
				var tmp = obj.getElementsByTagName('A')[0];
				tmp.insertBefore(_i.cloneNode(false), tmp.childNodes[0]);
			}
			if (!is_callback && this.settings.checkbox.three_state) {
				if (this._data.checkbox.uto) { clearTimeout(this._data.checkbox.uto); }
				this._data.checkbox.uto = setTimeout($.proxy(this._undetermined, this), 50);
			}
			return obj;
		};
		this.activate_node = function (obj, e) {
			if (this.settings.checkbox.whole_node || $(e.target).hasClass('jstree-checkbox')) {
				e.ctrlKey = true;
			}
			return parent.activate_node.call(this, obj, e);
		};
		/**
		 * show the node checkbox icons
		 * @name show_checkboxes()
		 * @plugin checkbox
		 */
		this.show_checkboxes = function () { this._data.core.themes.checkboxes = true; this.element.children("ul").removeClass("jstree-no-checkboxes"); };
		/**
		 * hide the node checkbox icons
		 * @name hide_checkboxes()
		 * @plugin checkbox
		 */
		this.hide_checkboxes = function () { this._data.core.themes.checkboxes = false; this.element.children("ul").addClass("jstree-no-checkboxes"); };
		/**
		 * toggle the node icons
		 * @name toggle_checkboxes()
		 * @plugin checkbox
		 */
		this.toggle_checkboxes = function () { if (this._data.core.themes.checkboxes) { this.hide_checkboxes(); } else { this.show_checkboxes(); } };
	};

	// include the checkbox plugin by default
	// $.jstree.defaults.plugins.push("checkbox");

	/**
	 * ### Contextmenu plugin
	 *
	 * Shows a context menu when a node is right-clicked.
	 */
	// TODO: move logic outside of function + check multiple move

	/**
	 * stores all defaults for the contextmenu plugin
	 * @name $.jstree.defaults.contextmenu
	 * @plugin contextmenu
	 */
	$.jstree.defaults.contextmenu = {
		/**
		 * a boolean indicating if the node should be selected when the context menu is invoked on it. Defaults to `true`.
		 * @name $.jstree.defaults.contextmenu.select_node
		 * @plugin contextmenu
		 */
		select_node: true,
		/**
		 * a boolean indicating if the menu should be shown aligned with the node. Defaults to `true`, otherwise the mouse coordinates are used.
		 * @name $.jstree.defaults.contextmenu.show_at_node
		 * @plugin contextmenu
		 */
		show_at_node: true,
		/**
		 * an object of actions, or a function that accepts a node and a callback function and calls the callback function with an object of actions available for that node (you can also return the items too).
		 * 
		 * Each action consists of a key (a unique name) and a value which is an object with the following properties (only label and action are required):
		 * 
		 * * `separator_before` - a boolean indicating if there should be a separator before this item
		 * * `separator_after` - a boolean indicating if there should be a separator after this item
		 * * `_disabled` - a boolean indicating if this action should be disabled
		 * * `label` - a string - the name of the action (could be a function returning a string)
		 * * `action` - a function to be executed if this item is chosen
		 * * `icon` - a string, can be a path to an icon or a className, if using an image that is in the current directory use a `./` prefix, otherwise it will be detected as a class
		 * * `shortcut` - keyCode which will trigger the action if the menu is open (for example `113` for rename, which equals F2)
		 * * `shortcut_label` - shortcut label (like for example `F2` for rename)
		 * 
		 * @name $.jstree.defaults.contextmenu.items
		 * @plugin contextmenu
		 */
		items: function (o, cb) { // Could be an object directly
			return {
				"create": {
					"separator_before": false,
					"separator_after": true,
					"_disabled": false, //(this.check("create_node", data.reference, {}, "last")),
					"label": "Create",
					"action": function (data) {
						var inst = $.jstree.reference(data.reference),
							obj = inst.get_node(data.reference);
						inst.create_node(obj, {}, "last", function (new_node) {
							setTimeout(function () { inst.edit(new_node); }, 0);
						});
					}
				},
				"rename": {
					"separator_before": false,
					"separator_after": false,
					"_disabled": false, //(this.check("rename_node", data.reference, this.get_parent(data.reference), "")),
					"label": "Rename",
					/*
					"shortcut"			: 113,
					"shortcut_label"	: 'F2',
					"icon"				: "glyphicon glyphicon-leaf",
					*/
					"action": function (data) {
						var inst = $.jstree.reference(data.reference),
							obj = inst.get_node(data.reference);
						inst.edit(obj);
					}
				},
				"remove": {
					"separator_before": false,
					"icon": false,
					"separator_after": false,
					"_disabled": false, //(this.check("delete_node", data.reference, this.get_parent(data.reference), "")),
					"label": "Delete",
					"action": function (data) {
						var inst = $.jstree.reference(data.reference),
							obj = inst.get_node(data.reference);
						if (inst.is_selected(obj)) {
							inst.delete_node(inst.get_selected());
						}
						else {
							inst.delete_node(obj);
						}
					}
				},
				"ccp": {
					"separator_before": true,
					"icon": false,
					"separator_after": false,
					"label": "Edit",
					"action": false,
					"submenu": {
						"cut": {
							"separator_before": false,
							"separator_after": false,
							"label": "Cut",
							"action": function (data) {
								var inst = $.jstree.reference(data.reference),
									obj = inst.get_node(data.reference);
								if (inst.is_selected(obj)) {
									inst.cut(inst.get_selected());
								}
								else {
									inst.cut(obj);
								}
							}
						},
						"copy": {
							"separator_before": false,
							"icon": false,
							"separator_after": false,
							"label": "Copy",
							"action": function (data) {
								var inst = $.jstree.reference(data.reference),
									obj = inst.get_node(data.reference);
								if (inst.is_selected(obj)) {
									inst.copy(inst.get_selected());
								}
								else {
									inst.copy(obj);
								}
							}
						},
						"paste": {
							"separator_before": false,
							"icon": false,
							"_disabled": function (data) {
								return !$.jstree.reference(data.reference).can_paste();
							},
							"separator_after": false,
							"label": "Paste",
							"action": function (data) {
								var inst = $.jstree.reference(data.reference),
									obj = inst.get_node(data.reference);
								inst.paste(obj);
							}
						}
					}
				}
			};
		}
	};

	$.jstree.plugins.contextmenu = function (options, parent) {
		this.bind = function () {
			parent.bind.call(this);

			var last_ts = 0;
			this.element
				.on("contextmenu.jstree", ".jstree-anchor", $.proxy(function (e) {
					e.preventDefault();
					last_ts = e.ctrlKey ? e.timeStamp : 0;
					if (!this.is_loading(e.currentTarget)) {
						this.show_contextmenu(e.currentTarget, e.pageX, e.pageY, e);
					}
				}, this))
				.on("click.jstree", ".jstree-anchor", $.proxy(function (e) {
					if (this._data.contextmenu.visible && (!last_ts || e.timeStamp - last_ts > 250)) { // work around safari & macOS ctrl+click
						$.vakata.context.hide();
					}
				}, this));
			/*
			if(!('oncontextmenu' in document.body) && ('ontouchstart' in document.body)) {
				var el = null, tm = null;
				this.element
					.on("touchstart", ".jstree-anchor", function (e) {
						el = e.currentTarget;
						tm = +new Date();
						$(document).one("touchend", function (e) {
							e.target = document.elementFromPoint(e.originalEvent.targetTouches[0].pageX - window.pageXOffset, e.originalEvent.targetTouches[0].pageY - window.pageYOffset);
							e.currentTarget = e.target;
							tm = ((+(new Date())) - tm);
							if(e.target === el && tm > 600 && tm < 1000) {
								e.preventDefault();
								$(el).trigger('contextmenu', e);
							}
							el = null;
							tm = null;
						});
					});
			}
			*/
			$(document).on("context_hide.vakata", $.proxy(function () { this._data.contextmenu.visible = false; }, this));
		};
		this.teardown = function () {
			if (this._data.contextmenu.visible) {
				$.vakata.context.hide();
			}
			parent.teardown.call(this);
		};

		/**
		 * prepare and show the context menu for a node
		 * @name show_contextmenu(obj [, x, y])
		 * @param {mixed} obj the node
		 * @param {Number} x the x-coordinate relative to the document to show the menu at
		 * @param {Number} y the y-coordinate relative to the document to show the menu at
		 * @param {Object} e the event if available that triggered the contextmenu
		 * @plugin contextmenu
		 * @trigger show_contextmenu.jstree
		 */
		this.show_contextmenu = function (obj, x, y, e) {
			obj = this.get_node(obj);
			if (!obj || obj.id === '#') { return false; }
			var s = this.settings.contextmenu,
				d = this.get_node(obj, true),
				a = d.children(".jstree-anchor"),
				o = false,
				i = false;
			if (s.show_at_node || x === undefined || y === undefined) {
				o = a.offset();
				x = o.left;
				y = o.top + this._data.core.li_height;
			}
			if (this.settings.contextmenu.select_node && !this.is_selected(obj)) {
				this.deselect_all();
				this.select_node(obj, false, false, e);
			}

			i = s.items;
			if ($.isFunction(i)) {
				i = i.call(this, obj, $.proxy(function (i) {
					this._show_contextmenu(obj, x, y, i);
				}, this));
			}
			if ($.isPlainObject(i)) {
				this._show_contextmenu(obj, x, y, i);
			}
		};
		/**
		 * show the prepared context menu for a node
		 * @name _show_contextmenu(obj, x, y, i)
		 * @param {mixed} obj the node
		 * @param {Number} x the x-coordinate relative to the document to show the menu at
		 * @param {Number} y the y-coordinate relative to the document to show the menu at
		 * @param {Number} i the object of items to show
		 * @plugin contextmenu
		 * @trigger show_contextmenu.jstree
		 * @private
		 */
		this._show_contextmenu = function (obj, x, y, i) {
			var d = this.get_node(obj, true),
				a = d.children(".jstree-anchor");
			$(document).one("context_show.vakata", $.proxy(function (e, data) {
				var cls = 'jstree-contextmenu jstree-' + this.get_theme() + '-contextmenu';
				$(data.element).addClass(cls);
			}, this));
			this._data.contextmenu.visible = true;
			$.vakata.context.show(a, { 'x': x, 'y': y }, i);
			/**
			 * triggered when the contextmenu is shown for a node
			 * @event
			 * @name show_contextmenu.jstree
			 * @param {Object} node the node
			 * @param {Number} x the x-coordinate of the menu relative to the document
			 * @param {Number} y the y-coordinate of the menu relative to the document
			 * @plugin contextmenu
			 */
			this.trigger('show_contextmenu', { "node": obj, "x": x, "y": y });
		};
	};

	// contextmenu helper
	(function ($) {
		var right_to_left = false,
			vakata_context = {
				element: false,
				reference: false,
				position_x: 0,
				position_y: 0,
				items: [],
				html: "",
				is_visible: false
			};

		$.vakata.context = {
			settings: {
				hide_onmouseleave: 0,
				icons: true
			},
			_trigger: function (event_name) {
				$(document).triggerHandler("context_" + event_name + ".vakata", {
					"reference": vakata_context.reference,
					"element": vakata_context.element,
					"position": {
						"x": vakata_context.position_x,
						"y": vakata_context.position_y
					}
				});
			},
			_execute: function (i) {
				i = vakata_context.items[i];
				return i && (!i._disabled || ($.isFunction(i._disabled) && !i._disabled({ "item": i, "reference": vakata_context.reference, "element": vakata_context.element }))) && i.action ? i.action.call(null, {
					"item": i,
					"reference": vakata_context.reference,
					"element": vakata_context.element,
					"position": {
						"x": vakata_context.position_x,
						"y": vakata_context.position_y
					}
				}) : false;
			},
			_parse: function (o, is_callback) {
				if (!o) { return false; }
				if (!is_callback) {
					vakata_context.html = "";
					vakata_context.items = [];
				}
				var str = "",
					sep = false,
					tmp;

				if (is_callback) { str += "<" + "ul>"; }
				$.each(o, function (i, val) {
					if (!val) { return true; }
					vakata_context.items.push(val);
					if (!sep && val.separator_before) {
						str += "<" + "li class='vakata-context-separator'><" + "a href='#' " + ($.vakata.context.settings.icons ? '' : 'style="margin-left:0px;"') + ">&#160;<" + "/a><" + "/li>";
					}
					sep = false;
					str += "<" + "li class='" + (val._class || "") + (val._disabled === true || ($.isFunction(val._disabled) && val._disabled({ "item": val, "reference": vakata_context.reference, "element": vakata_context.element })) ? " vakata-contextmenu-disabled " : "") + "' " + (val.shortcut ? " data-shortcut='" + val.shortcut + "' " : '') + ">";
					str += "<" + "a href='#' rel='" + (vakata_context.items.length - 1) + "'>";
					if ($.vakata.context.settings.icons) {
						str += "<" + "i ";
						if (val.icon) {
							if (val.icon.indexOf("/") !== -1 || val.icon.indexOf(".") !== -1) { str += " style='background:url(\"" + val.icon + "\") center center no-repeat' "; }
							else { str += " class='" + val.icon + "' "; }
						}
						str += "><" + "/i><" + "span class='vakata-contextmenu-sep'>&#160;<" + "/span>";
					}
					str += ($.isFunction(val.label) ? val.label({ "item": i, "reference": vakata_context.reference, "element": vakata_context.element }) : val.label) + (val.shortcut ? ' <span class="vakata-contextmenu-shortcut vakata-contextmenu-shortcut-' + val.shortcut + '">' + (val.shortcut_label || '') + '</span>' : '') + "<" + "/a>";
					if (val.submenu) {
						tmp = $.vakata.context._parse(val.submenu, true);
						if (tmp) { str += tmp; }
					}
					str += "<" + "/li>";
					if (val.separator_after) {
						str += "<" + "li class='vakata-context-separator'><" + "a href='#' " + ($.vakata.context.settings.icons ? '' : 'style="margin-left:0px;"') + ">&#160;<" + "/a><" + "/li>";
						sep = true;
					}
				});
				str = str.replace(/<li class\='vakata-context-separator'\><\/li\>$/, "");
				if (is_callback) { str += "</ul>"; }
				/**
				 * triggered on the document when the contextmenu is parsed (HTML is built)
				 * @event
				 * @plugin contextmenu
				 * @name context_parse.vakata
				 * @param {jQuery} reference the element that was right clicked
				 * @param {jQuery} element the DOM element of the menu itself
				 * @param {Object} position the x & y coordinates of the menu
				 */
				if (!is_callback) { vakata_context.html = str; $.vakata.context._trigger("parse"); }
				return str.length > 10 ? str : false;
			},
			_show_submenu: function (o) {
				o = $(o);
				if (!o.length || !o.children("ul").length) { return; }
				var e = o.children("ul"),
					x = o.offset().left + o.outerWidth(),
					y = o.offset().top,
					w = e.width(),
					h = e.height(),
					dw = $(window).width() + $(window).scrollLeft(),
					dh = $(window).height() + $(window).scrollTop();
				// може да се спести е една проверка - дали няма някой от класовете вече нагоре
				if (right_to_left) {
					o[x - (w + 10 + o.outerWidth()) < 0 ? "addClass" : "removeClass"]("vakata-context-left");
				}
				else {
					o[x + w + 10 > dw ? "addClass" : "removeClass"]("vakata-context-right");
				}
				if (y + h + 10 > dh) {
					e.css("bottom", "-1px");
				}
				e.show();
			},
			show: function (reference, position, data) {
				var o, e, x, y, w, h, dw, dh, cond = true;
				if (vakata_context.element && vakata_context.element.length) {
					vakata_context.element.width('');
				}
				switch (cond) {
					case (!position && !reference):
						return false;
					case (!!position && !!reference):
						vakata_context.reference = reference;
						vakata_context.position_x = position.x;
						vakata_context.position_y = position.y;
						break;
					case (!position && !!reference):
						vakata_context.reference = reference;
						o = reference.offset();
						vakata_context.position_x = o.left + reference.outerHeight();
						vakata_context.position_y = o.top;
						break;
					case (!!position && !reference):
						vakata_context.position_x = position.x;
						vakata_context.position_y = position.y;
						break;
				}
				if (!!reference && !data && $(reference).data('vakata_contextmenu')) {
					data = $(reference).data('vakata_contextmenu');
				}
				if ($.vakata.context._parse(data)) {
					vakata_context.element.html(vakata_context.html);
				}
				if (vakata_context.items.length) {
					e = vakata_context.element;
					x = vakata_context.position_x;
					y = vakata_context.position_y;
					w = e.width();
					h = e.height();
					dw = $(window).width() + $(window).scrollLeft();
					dh = $(window).height() + $(window).scrollTop();
					if (right_to_left) {
						x -= e.outerWidth();
						if (x < $(window).scrollLeft() + 20) {
							x = $(window).scrollLeft() + 20;
						}
					}
					if (x + w + 20 > dw) {
						x = dw - (w + 20);
					}
					if (y + h + 20 > dh) {
						y = dh - (h + 20);
					}

					vakata_context.element
						.css({ "left": x, "top": y })
						.show()
						.find('a:eq(0)').focus().parent().addClass("vakata-context-hover");
					vakata_context.is_visible = true;
					/**
					 * triggered on the document when the contextmenu is shown
					 * @event
					 * @plugin contextmenu
					 * @name context_show.vakata
					 * @param {jQuery} reference the element that was right clicked
					 * @param {jQuery} element the DOM element of the menu itself
					 * @param {Object} position the x & y coordinates of the menu
					 */
					$.vakata.context._trigger("show");
				}
			},
			hide: function () {
				if (vakata_context.is_visible) {
					vakata_context.element.hide().find("ul").hide().end().find(':focus').blur();
					vakata_context.is_visible = false;
					/**
					 * triggered on the document when the contextmenu is hidden
					 * @event
					 * @plugin contextmenu
					 * @name context_hide.vakata
					 * @param {jQuery} reference the element that was right clicked
					 * @param {jQuery} element the DOM element of the menu itself
					 * @param {Object} position the x & y coordinates of the menu
					 */
					$.vakata.context._trigger("hide");
				}
			}
		};
		$(function () {
			right_to_left = $("body").css("direction") === "rtl";
			var to = false;

			vakata_context.element = $("<ul class='vakata-context'></ul>");
			vakata_context.element
				.on("mouseenter", "li", function (e) {
					e.stopImmediatePropagation();

					if ($.contains(this, e.relatedTarget)) {
						// премахнато заради delegate mouseleave по-долу
						// $(this).find(".vakata-context-hover").removeClass("vakata-context-hover");
						return;
					}

					if (to) { clearTimeout(to); }
					vakata_context.element.find(".vakata-context-hover").removeClass("vakata-context-hover").end();

					$(this)
						.siblings().find("ul").hide().end().end()
						.parentsUntil(".vakata-context", "li").addBack().addClass("vakata-context-hover");
					$.vakata.context._show_submenu(this);
				})
				// тестово - дали не натоварва?
				.on("mouseleave", "li", function (e) {
					if ($.contains(this, e.relatedTarget)) { return; }
					$(this).find(".vakata-context-hover").addBack().removeClass("vakata-context-hover");
				})
				.on("mouseleave", function (e) {
					$(this).find(".vakata-context-hover").removeClass("vakata-context-hover");
					if ($.vakata.context.settings.hide_onmouseleave) {
						to = setTimeout(
							(function (t) {
								return function () { $.vakata.context.hide(); };
							}(this)), $.vakata.context.settings.hide_onmouseleave);
					}
				})
				.on("click", "a", function (e) {
					e.preventDefault();
				})
				.on("mouseup", "a", function (e) {
					if (!$(this).blur().parent().hasClass("vakata-context-disabled") && $.vakata.context._execute($(this).attr("rel")) !== false) {
						$.vakata.context.hide();
					}
				})
				.on('keydown', 'a', function (e) {
					var o = null;
					switch (e.which) {
						case 13:
						case 32:
							e.type = "mouseup";
							e.preventDefault();
							$(e.currentTarget).trigger(e);
							break;
						case 37:
							if (vakata_context.is_visible) {
								vakata_context.element.find(".vakata-context-hover").last().parents("li:eq(0)").find("ul").hide().find(".vakata-context-hover").removeClass("vakata-context-hover").end().end().children('a').focus();
								e.stopImmediatePropagation();
								e.preventDefault();
							}
							break;
						case 38:
							if (vakata_context.is_visible) {
								o = vakata_context.element.find("ul:visible").addBack().last().children(".vakata-context-hover").removeClass("vakata-context-hover").prevAll("li:not(.vakata-context-separator)").first();
								if (!o.length) { o = vakata_context.element.find("ul:visible").addBack().last().children("li:not(.vakata-context-separator)").last(); }
								o.addClass("vakata-context-hover").children('a').focus();
								e.stopImmediatePropagation();
								e.preventDefault();
							}
							break;
						case 39:
							if (vakata_context.is_visible) {
								vakata_context.element.find(".vakata-context-hover").last().children("ul").show().children("li:not(.vakata-context-separator)").removeClass("vakata-context-hover").first().addClass("vakata-context-hover").children('a').focus();
								e.stopImmediatePropagation();
								e.preventDefault();
							}
							break;
						case 40:
							if (vakata_context.is_visible) {
								o = vakata_context.element.find("ul:visible").addBack().last().children(".vakata-context-hover").removeClass("vakata-context-hover").nextAll("li:not(.vakata-context-separator)").first();
								if (!o.length) { o = vakata_context.element.find("ul:visible").addBack().last().children("li:not(.vakata-context-separator)").first(); }
								o.addClass("vakata-context-hover").children('a').focus();
								e.stopImmediatePropagation();
								e.preventDefault();
							}
							break;
						case 27:
							$.vakata.context.hide();
							e.preventDefault();
							break;
						default:
							//console.log(e.which);
							break;
					}
				})
				.on('keydown', function (e) {
					e.preventDefault();
					var a = vakata_context.element.find('.vakata-contextmenu-shortcut-' + e.which).parent();
					if (a.parent().not('.vakata-context-disabled')) {
						a.mouseup();
					}
				})
				.appendTo("body");

			$(document)
				.on("mousedown", function (e) {
					if (vakata_context.is_visible && !$.contains(vakata_context.element[0], e.target)) { $.vakata.context.hide(); }
				})
				.on("context_show.vakata", function (e, data) {
					vakata_context.element.find("li:has(ul)").children("a").addClass("vakata-context-parent");
					if (right_to_left) {
						vakata_context.element.addClass("vakata-context-rtl").css("direction", "rtl");
					}
					// also apply a RTL class?
					vakata_context.element.find("ul").hide().end();
				});
		});
	}($));
	// $.jstree.defaults.plugins.push("contextmenu");

	/**
	 * ### Drag'n'drop plugin
	 *
	 * Enables dragging and dropping of nodes in the tree, resulting in a move or copy operations.
	 */

	/**
	 * stores all defaults for the drag'n'drop plugin
	 * @name $.jstree.defaults.dnd
	 * @plugin dnd
	 */
	$.jstree.defaults.dnd = {
		/**
		 * a boolean indicating if a copy should be possible while dragging (by pressint the meta key or Ctrl). Defaults to `true`.
		 * @name $.jstree.defaults.dnd.copy
		 * @plugin dnd
		 */
		copy: true,
		/**
		 * a number indicating how long a node should remain hovered while dragging to be opened. Defaults to `500`.
		 * @name $.jstree.defaults.dnd.open_timeout
		 * @plugin dnd
		 */
		open_timeout: 500,
		/**
		 * a function invoked each time a node is about to be dragged, invoked in the tree's scope and receives the nodes about to be dragged as an argument (array) - return `false` to prevent dragging
		 * @name $.jstree.defaults.dnd.is_draggable
		 * @plugin dnd
		 */
		is_draggable: true,
		/**
		 * a boolean indicating if checks should constantly be made while the user is dragging the node (as opposed to checking only on drop), default is `true`
		 * @name $.jstree.defaults.dnd.check_while_dragging
		 * @plugin dnd
		 */
		check_while_dragging: true,
		/**
		 * a boolean indicating if nodes from this tree should only be copied with dnd (as opposed to moved), default is `false`
		 * @name $.jstree.defaults.dnd.always_copy
		 * @plugin dnd
		 */
		always_copy: false
	};
	// TODO: now check works by checking for each node individually, how about max_children, unique, etc?
	// TODO: drop somewhere else - maybe demo only?
	$.jstree.plugins.dnd = function (options, parent) {
		this.bind = function () {
			parent.bind.call(this);

			this.element
				.on('mousedown.jstree touchstart.jstree', '.jstree-anchor', $.proxy(function (e) {
					var obj = this.get_node(e.target),
						mlt = this.is_selected(obj) ? this.get_selected().length : 1;
					if (obj && obj.id && obj.id !== "#" && (e.which === 1 || e.type === "touchstart") &&
						(this.settings.dnd.is_draggable === true || ($.isFunction(this.settings.dnd.is_draggable) && this.settings.dnd.is_draggable.call(this, (mlt > 1 ? this.get_selected(true) : [obj]))))
					) {
						this.element.trigger('mousedown.jstree');
						return $.vakata.dnd.start(e, { 'jstree': true, 'origin': this, 'obj': this.get_node(obj, true), 'nodes': mlt > 1 ? this.get_selected() : [obj.id] }, '<div id="jstree-dnd" class="jstree-' + this.get_theme() + '"><i class="jstree-icon jstree-er"></i>' + (mlt > 1 ? mlt + ' ' + this.get_string('nodes') : this.get_text(e.currentTarget, true)) + '<ins class="jstree-copy" style="display:none;">+</ins></div>');
					}
				}, this));
		};
	};

	$(function () {
		// bind only once for all instances
		var lastmv = false,
			laster = false,
			opento = false,
			marker = $('<div id="jstree-marker">&#160;</div>').hide().appendTo('body');

		$(document)
			.bind('dnd_start.vakata', function (e, data) {
				lastmv = false;
			})
			.bind('dnd_move.vakata', function (e, data) {
				if (opento) { clearTimeout(opento); }
				if (!data.data.jstree) { return; }

				// if we are hovering the marker image do nothing (can happen on "inside" drags)
				if (data.event.target.id && data.event.target.id === 'jstree-marker') {
					return;
				}

				var ins = $.jstree.reference(data.event.target),
					ref = false,
					off = false,
					rel = false,
					l, t, h, p, i, o, ok, t1, t2, op, ps, pr;
				// if we are over an instance
				if (ins && ins._data && ins._data.dnd) {
					marker.attr('class', 'jstree-' + ins.get_theme());
					data.helper
						.children().attr('class', 'jstree-' + ins.get_theme())
						.find('.jstree-copy:eq(0)')[data.data.origin && (data.data.origin.settings.dnd.always_copy || (data.data.origin.settings.dnd.copy && (data.event.metaKey || data.event.ctrlKey))) ? 'show' : 'hide']();


					// if are hovering the container itself add a new root node
					if ((data.event.target === ins.element[0] || data.event.target === ins.get_container_ul()[0]) && ins.get_container_ul().children().length === 0) {
						ok = true;
						for (t1 = 0, t2 = data.data.nodes.length; t1 < t2; t1++) {
							ok = ok && ins.check((data.data.origin && (data.data.origin.settings.dnd.always_copy || (data.data.origin.settings.dnd.copy && (data.event.metaKey || data.event.ctrlKey))) ? "copy_node" : "move_node"), (data.data.origin && data.data.origin !== ins ? data.data.origin.get_node(data.data.nodes[t1]) : data.data.nodes[t1]), '#', 'last', { 'dnd': true, 'ref': ins.get_node('#'), 'pos': 'i', 'is_multi': (data.data.origin && data.data.origin !== ins), 'is_foreign': (!data.data.origin) });
							if (!ok) { break; }
						}
						if (ok) {
							lastmv = { 'ins': ins, 'par': '#', 'pos': 'last' };
							marker.hide();
							data.helper.find('.jstree-icon:eq(0)').removeClass('jstree-er').addClass('jstree-ok');
							return;
						}
					}
					else {
						// if we are hovering a tree node
						ref = $(data.event.target).closest('a');
						if (ref && ref.length && ref.parent().is('.jstree-closed, .jstree-open, .jstree-leaf')) {
							off = ref.offset();
							rel = data.event.pageY - off.top;
							h = ref.height();
							if (rel < h / 3) {
								o = ['b', 'i', 'a'];
							}
							else if (rel > h - h / 3) {
								o = ['a', 'i', 'b'];
							}
							else {
								o = rel > h / 2 ? ['i', 'a', 'b'] : ['i', 'b', 'a'];
							}
							$.each(o, function (j, v) {
								switch (v) {
									case 'b':
										l = off.left - 6;
										t = off.top - 5;
										p = ins.get_parent(ref);
										i = ref.parent().index();
										break;
									case 'i':
										l = off.left - 2;
										t = off.top - 5 + h / 2 + 1;
										p = ins.get_node(ref.parent()).id;
										i = 0;
										break;
									case 'a':
										l = off.left - 6;
										t = off.top - 5 + h;
										p = ins.get_parent(ref);
										i = ref.parent().index() + 1;
										break;
								}
								/*
								// TODO: moving inside, but the node is not yet loaded?
								// the check will work anyway, as when moving the node will be loaded first and checked again
								if(v === 'i' && !ins.is_loaded(p)) { }
								*/
								ok = true;
								for (t1 = 0, t2 = data.data.nodes.length; t1 < t2; t1++) {
									op = data.data.origin && (data.data.origin.settings.dnd.always_copy || (data.data.origin.settings.dnd.copy && (data.event.metaKey || data.event.ctrlKey))) ? "copy_node" : "move_node";
									ps = i;
									if (op === "move_node" && v === 'a' && (data.data.origin && data.data.origin === ins) && p === ins.get_parent(data.data.nodes[t1])) {
										pr = ins.get_node(p);
										if (ps > $.inArray(data.data.nodes[t1], pr.children)) {
											ps -= 1;
										}
									}
									ok = ok && ((ins && ins.settings && ins.settings.dnd && ins.settings.dnd.check_while_dragging === false) || ins.check(op, (data.data.origin && data.data.origin !== ins ? data.data.origin.get_node(data.data.nodes[t1]) : data.data.nodes[t1]), p, ps, { 'dnd': true, 'ref': ins.get_node(ref.parent()), 'pos': v, 'is_multi': (data.data.origin && data.data.origin !== ins), 'is_foreign': (!data.data.origin) }));
									if (!ok) {
										if (ins && ins.last_error) { laster = ins.last_error(); }
										break;
									}
								}
								if (ok) {
									if (v === 'i' && ref.parent().is('.jstree-closed') && ins.settings.dnd.open_timeout) {
										opento = setTimeout((function (x, z) { return function () { x.open_node(z); }; }(ins, ref)), ins.settings.dnd.open_timeout);
									}
									lastmv = { 'ins': ins, 'par': p, 'pos': i };
									marker.css({ 'left': l + 'px', 'top': t + 'px' }).show();
									data.helper.find('.jstree-icon:eq(0)').removeClass('jstree-er').addClass('jstree-ok');
									laster = {};
									o = true;
									return false;
								}
							});
							if (o === true) { return; }
						}
					}
				}
				lastmv = false;
				data.helper.find('.jstree-icon').removeClass('jstree-ok').addClass('jstree-er');
				marker.hide();
			})
			.bind('dnd_scroll.vakata', function (e, data) {
				if (!data.data.jstree) { return; }
				marker.hide();
				lastmv = false;
				data.helper.find('.jstree-icon:eq(0)').removeClass('jstree-ok').addClass('jstree-er');
			})
			.bind('dnd_stop.vakata', function (e, data) {
				if (opento) { clearTimeout(opento); }
				if (!data.data.jstree) { return; }
				marker.hide();
				var i, j, nodes = [];
				if (lastmv) {
					for (i = 0, j = data.data.nodes.length; i < j; i++) {
						nodes[i] = data.data.origin ? data.data.origin.get_node(data.data.nodes[i]) : data.data.nodes[i];
						if (data.data.origin) {
							nodes[i].instance = data.data.origin;
						}
					}
					lastmv.ins[data.data.origin && (data.data.origin.settings.dnd.always_copy || (data.data.origin.settings.dnd.copy && (data.event.metaKey || data.event.ctrlKey))) ? 'copy_node' : 'move_node'](nodes, lastmv.par, lastmv.pos);
				}
				else {
					i = $(data.event.target).closest('.jstree');
					if (i.length && laster && laster.error && laster.error === 'check') {
						i = i.jstree(true);
						if (i) {
							i.settings.core.error.call(this, laster);
						}
					}
				}
			})
			.bind('keyup keydown', function (e, data) {
				data = $.vakata.dnd._get();
				if (data.data && data.data.jstree) {
					data.helper.find('.jstree-copy:eq(0)')[data.data.origin && (data.data.origin.settings.dnd.always_copy || (data.data.origin.settings.dnd.copy && (e.metaKey || e.ctrlKey))) ? 'show' : 'hide']();
				}
			});
	});

	// helpers
	(function ($) {
		// private variable
		var vakata_dnd = {
			element: false,
			is_down: false,
			is_drag: false,
			helper: false,
			helper_w: 0,
			data: false,
			init_x: 0,
			init_y: 0,
			scroll_l: 0,
			scroll_t: 0,
			scroll_e: false,
			scroll_i: false
		};
		$.vakata.dnd = {
			settings: {
				scroll_speed: 10,
				scroll_proximity: 20,
				helper_left: 5,
				helper_top: 10,
				threshold: 5
			},
			_trigger: function (event_name, e) {
				var data = $.vakata.dnd._get();
				data.event = e;
				$(document).triggerHandler("dnd_" + event_name + ".vakata", data);
			},
			_get: function () {
				return {
					"data": vakata_dnd.data,
					"element": vakata_dnd.element,
					"helper": vakata_dnd.helper
				};
			},
			_clean: function () {
				if (vakata_dnd.helper) { vakata_dnd.helper.remove(); }
				if (vakata_dnd.scroll_i) { clearInterval(vakata_dnd.scroll_i); vakata_dnd.scroll_i = false; }
				vakata_dnd = {
					element: false,
					is_down: false,
					is_drag: false,
					helper: false,
					helper_w: 0,
					data: false,
					init_x: 0,
					init_y: 0,
					scroll_l: 0,
					scroll_t: 0,
					scroll_e: false,
					scroll_i: false
				};
				$(document).off("mousemove touchmove", $.vakata.dnd.drag);
				$(document).off("mouseup touchend", $.vakata.dnd.stop);
			},
			_scroll: function (init_only) {
				if (!vakata_dnd.scroll_e || (!vakata_dnd.scroll_l && !vakata_dnd.scroll_t)) {
					if (vakata_dnd.scroll_i) { clearInterval(vakata_dnd.scroll_i); vakata_dnd.scroll_i = false; }
					return false;
				}
				if (!vakata_dnd.scroll_i) {
					vakata_dnd.scroll_i = setInterval($.vakata.dnd._scroll, 100);
					return false;
				}
				if (init_only === true) { return false; }

				var i = vakata_dnd.scroll_e.scrollTop(),
					j = vakata_dnd.scroll_e.scrollLeft();
				vakata_dnd.scroll_e.scrollTop(i + vakata_dnd.scroll_t * $.vakata.dnd.settings.scroll_speed);
				vakata_dnd.scroll_e.scrollLeft(j + vakata_dnd.scroll_l * $.vakata.dnd.settings.scroll_speed);
				if (i !== vakata_dnd.scroll_e.scrollTop() || j !== vakata_dnd.scroll_e.scrollLeft()) {
					/**
					 * triggered on the document when a drag causes an element to scroll
					 * @event
					 * @plugin dnd
					 * @name dnd_scroll.vakata
					 * @param {Mixed} data any data supplied with the call to $.vakata.dnd.start
					 * @param {DOM} element the DOM element being dragged
					 * @param {jQuery} helper the helper shown next to the mouse
					 * @param {jQuery} event the element that is scrolling
					 */
					$.vakata.dnd._trigger("scroll", vakata_dnd.scroll_e);
				}
			},
			start: function (e, data, html) {
				if (e.type === "touchstart" && e.originalEvent && e.originalEvent.changedTouches && e.originalEvent.changedTouches[0]) {
					e.pageX = e.originalEvent.changedTouches[0].pageX;
					e.pageY = e.originalEvent.changedTouches[0].pageY;
					e.target = document.elementFromPoint(e.originalEvent.changedTouches[0].pageX - window.pageXOffset, e.originalEvent.changedTouches[0].pageY - window.pageYOffset);
				}
				if (vakata_dnd.is_drag) { $.vakata.dnd.stop({}); }
				try {
					e.currentTarget.unselectable = "on";
					e.currentTarget.onselectstart = function () { return false; };
					if (e.currentTarget.style) { e.currentTarget.style.MozUserSelect = "none"; }
				} catch (ignore) { }
				vakata_dnd.init_x = e.pageX;
				vakata_dnd.init_y = e.pageY;
				vakata_dnd.data = data;
				vakata_dnd.is_down = true;
				vakata_dnd.element = e.currentTarget;
				if (html !== false) {
					vakata_dnd.helper = $("<div id='vakata-dnd'></div>").html(html).css({
						"display": "block",
						"margin": "0",
						"padding": "0",
						"position": "absolute",
						"top": "-2000px",
						"lineHeight": "16px",
						"zIndex": "10000"
					});
				}
				$(document).bind("mousemove touchmove", $.vakata.dnd.drag);
				$(document).bind("mouseup touchend", $.vakata.dnd.stop);
				return false;
			},
			drag: function (e) {
				if (e.type === "touchmove" && e.originalEvent && e.originalEvent.changedTouches && e.originalEvent.changedTouches[0]) {
					e.pageX = e.originalEvent.changedTouches[0].pageX;
					e.pageY = e.originalEvent.changedTouches[0].pageY;
					e.target = document.elementFromPoint(e.originalEvent.changedTouches[0].pageX - window.pageXOffset, e.originalEvent.changedTouches[0].pageY - window.pageYOffset);
				}
				if (!vakata_dnd.is_down) { return; }
				if (!vakata_dnd.is_drag) {
					if (
						Math.abs(e.pageX - vakata_dnd.init_x) > $.vakata.dnd.settings.threshold ||
						Math.abs(e.pageY - vakata_dnd.init_y) > $.vakata.dnd.settings.threshold
					) {
						if (vakata_dnd.helper) {
							vakata_dnd.helper.appendTo("body");
							vakata_dnd.helper_w = vakata_dnd.helper.outerWidth();
						}
						vakata_dnd.is_drag = true;
						/**
						 * triggered on the document when a drag starts
						 * @event
						 * @plugin dnd
						 * @name dnd_start.vakata
						 * @param {Mixed} data any data supplied with the call to $.vakata.dnd.start
						 * @param {DOM} element the DOM element being dragged
						 * @param {jQuery} helper the helper shown next to the mouse
						 * @param {Object} event the event that caused the start (probably mousemove)
						 */
						$.vakata.dnd._trigger("start", e);
					}
					else { return; }
				}

				var d = false, w = false,
					dh = false, wh = false,
					dw = false, ww = false,
					dt = false, dl = false,
					ht = false, hl = false;

				vakata_dnd.scroll_t = 0;
				vakata_dnd.scroll_l = 0;
				vakata_dnd.scroll_e = false;
				$($(e.target).parentsUntil("body").addBack().get().reverse())
					.filter(function () {
						return (/^auto|scroll$/).test($(this).css("overflow")) &&
								(this.scrollHeight > this.offsetHeight || this.scrollWidth > this.offsetWidth);
					})
					.each(function () {
						var t = $(this), o = t.offset();
						if (this.scrollHeight > this.offsetHeight) {
							if (o.top + t.height() - e.pageY < $.vakata.dnd.settings.scroll_proximity) { vakata_dnd.scroll_t = 1; }
							if (e.pageY - o.top < $.vakata.dnd.settings.scroll_proximity) { vakata_dnd.scroll_t = -1; }
						}
						if (this.scrollWidth > this.offsetWidth) {
							if (o.left + t.width() - e.pageX < $.vakata.dnd.settings.scroll_proximity) { vakata_dnd.scroll_l = 1; }
							if (e.pageX - o.left < $.vakata.dnd.settings.scroll_proximity) { vakata_dnd.scroll_l = -1; }
						}
						if (vakata_dnd.scroll_t || vakata_dnd.scroll_l) {
							vakata_dnd.scroll_e = $(this);
							return false;
						}
					});

				if (!vakata_dnd.scroll_e) {
					d = $(document); w = $(window);
					dh = d.height(); wh = w.height();
					dw = d.width(); ww = w.width();
					dt = d.scrollTop(); dl = d.scrollLeft();
					if (dh > wh && e.pageY - dt < $.vakata.dnd.settings.scroll_proximity) { vakata_dnd.scroll_t = -1; }
					if (dh > wh && wh - (e.pageY - dt) < $.vakata.dnd.settings.scroll_proximity) { vakata_dnd.scroll_t = 1; }
					if (dw > ww && e.pageX - dl < $.vakata.dnd.settings.scroll_proximity) { vakata_dnd.scroll_l = -1; }
					if (dw > ww && ww - (e.pageX - dl) < $.vakata.dnd.settings.scroll_proximity) { vakata_dnd.scroll_l = 1; }
					if (vakata_dnd.scroll_t || vakata_dnd.scroll_l) {
						vakata_dnd.scroll_e = d;
					}
				}
				if (vakata_dnd.scroll_e) { $.vakata.dnd._scroll(true); }

				if (vakata_dnd.helper) {
					ht = parseInt(e.pageY + $.vakata.dnd.settings.helper_top, 10);
					hl = parseInt(e.pageX + $.vakata.dnd.settings.helper_left, 10);
					if (dh && ht + 25 > dh) { ht = dh - 50; }
					if (dw && hl + vakata_dnd.helper_w > dw) { hl = dw - (vakata_dnd.helper_w + 2); }
					vakata_dnd.helper.css({
						left: hl + "px",
						top: ht + "px"
					});
				}
				/**
				 * triggered on the document when a drag is in progress
				 * @event
				 * @plugin dnd
				 * @name dnd_move.vakata
				 * @param {Mixed} data any data supplied with the call to $.vakata.dnd.start
				 * @param {DOM} element the DOM element being dragged
				 * @param {jQuery} helper the helper shown next to the mouse
				 * @param {Object} event the event that caused this to trigger (most likely mousemove)
				 */
				$.vakata.dnd._trigger("move", e);
			},
			stop: function (e) {
				if (e.type === "touchend" && e.originalEvent && e.originalEvent.changedTouches && e.originalEvent.changedTouches[0]) {
					e.pageX = e.originalEvent.changedTouches[0].pageX;
					e.pageY = e.originalEvent.changedTouches[0].pageY;
					e.target = document.elementFromPoint(e.originalEvent.changedTouches[0].pageX - window.pageXOffset, e.originalEvent.changedTouches[0].pageY - window.pageYOffset);
				}
				if (vakata_dnd.is_drag) {
					/**
					 * triggered on the document when a drag stops (the dragged element is dropped)
					 * @event
					 * @plugin dnd
					 * @name dnd_stop.vakata
					 * @param {Mixed} data any data supplied with the call to $.vakata.dnd.start
					 * @param {DOM} element the DOM element being dragged
					 * @param {jQuery} helper the helper shown next to the mouse
					 * @param {Object} event the event that caused the stop
					 */
					$.vakata.dnd._trigger("stop", e);
				}
				$.vakata.dnd._clean();
			}
		};
	}(jQuery));

	// include the dnd plugin by default
	// $.jstree.defaults.plugins.push("dnd");


	/**
	 * ### Search plugin
	 *
	 * Adds search functionality to jsTree.
	 */

	/**
	 * stores all defaults for the search plugin
	 * @name $.jstree.defaults.search
	 * @plugin search
	 */
	$.jstree.defaults.search = {
		/**
		 * a jQuery-like AJAX config, which jstree uses if a server should be queried for results. 
		 * 
		 * A `str` (which is the search string) parameter will be added with the request. The expected result is a JSON array with nodes that need to be opened so that matching nodes will be revealed.
		 * Leave this setting as `false` to not query the server. You can also set this to a function, which will be invoked in the instance's scope and receive 2 parameters - the search string and the callback to call with the array of nodes to load.
		 * @name $.jstree.defaults.search.ajax
		 * @plugin search
		 */
		ajax: false,
		/**
		 * Indicates if the search should be fuzzy or not (should `chnd3` match `child node 3`). Default is `true`.
		 * @name $.jstree.defaults.search.fuzzy
		 * @plugin search
		 */
		fuzzy: true,
		/**
		 * Indicates if the search should be case sensitive. Default is `false`.
		 * @name $.jstree.defaults.search.case_sensitive
		 * @plugin search
		 */
		case_sensitive: false,
		/**
		 * Indicates if the tree should be filtered to show only matching nodes (keep in mind this can be a heavy on large trees in old browsers). Default is `false`.
		 * @name $.jstree.defaults.search.show_only_matches
		 * @plugin search
		 */
		show_only_matches: false,
		/**
		 * Indicates if all nodes opened to reveal the search result, should be closed when the search is cleared or a new search is performed. Default is `true`.
		 * @name $.jstree.defaults.search.close_opened_onclear
		 * @plugin search
		 */
		close_opened_onclear: true,
		/**
		 * Indicates if only leaf nodes should be included in search results. Default is `false`.
		 * @name $.jstree.defaults.search.search_leaves_only
		 * @plugin search
		 */
		search_leaves_only: false
	};

	$.jstree.plugins.search = function (options, parent) {
		this.bind = function () {
			parent.bind.call(this);

			this._data.search.str = "";
			this._data.search.dom = $();
			this._data.search.res = [];
			this._data.search.opn = [];

			this.element.on('before_open.jstree', $.proxy(function (e, data) {
				var i, j, f, r = this._data.search.res, s = [], o = $();
				if (r && r.length) {
					this._data.search.dom = $();
					for (i = 0, j = r.length; i < j; i++) {
						s = s.concat(this.get_node(r[i]).parents);
						f = this.get_node(r[i], true);
						if (f) {
							this._data.search.dom = this._data.search.dom.add(f);
						}
					}
					s = $.vakata.array_unique(s);
					for (i = 0, j = s.length; i < j; i++) {
						if (s[i] === "#") { continue; }
						f = this.get_node(s[i], true);
						if (f) {
							o = o.add(f);
						}
					}
					this._data.search.dom.children(".jstree-anchor").addClass('jstree-search');
					if (this.settings.search.show_only_matches && this._data.search.res.length) {
						this.element.find("li").hide().filter('.jstree-last').filter(function () { return this.nextSibling; }).removeClass('jstree-last');
						o = o.add(this._data.search.dom);
						o.parentsUntil(".jstree").addBack().show()
							.filter("ul").each(function () { $(this).children("li:visible").eq(-1).addClass("jstree-last"); });
					}
				}
			}, this));
			if (this.settings.search.show_only_matches) {
				this.element
					.on("search.jstree", function (e, data) {
						if (data.nodes.length) {
							$(this).find("li").hide().filter('.jstree-last').filter(function () { return this.nextSibling; }).removeClass('jstree-last');
							data.nodes.parentsUntil(".jstree").addBack().show()
								.filter("ul").each(function () { $(this).children("li:visible").eq(-1).addClass("jstree-last"); });
						}
					})
					.on("clear_search.jstree", function (e, data) {
						if (data.nodes.length) {
							$(this).find("li").css("display", "").filter('.jstree-last').filter(function () { return this.nextSibling; }).removeClass('jstree-last');
						}
					});
			}
		};
		/**
		 * used to search the tree nodes for a given string
		 * @name search(str [, skip_async])
		 * @param {String} str the search string
		 * @param {Boolean} skip_async if set to true server will not be queried even if configured
		 * @plugin search
		 * @trigger search.jstree
		 */
		this.search = function (str, skip_async) {
			if (str === false || $.trim(str) === "") {
				return this.clear_search();
			}
			var s = this.settings.search,
				a = s.ajax ? s.ajax : false,
				f = null,
				r = [],
				p = [], i, j;
			if (this._data.search.res.length) {
				this.clear_search();
			}
			if (!skip_async && a !== false) {
				if ($.isFunction(a)) {
					return a.call(this, str, $.proxy(function (d) {
						if (d && d.d) { d = d.d; }
						this._load_nodes(!$.isArray(d) ? [] : d, function () {
							this.search(str, true);
						});
					}, this));
				}
				else {
					a = $.extend({}, a);
					if (!a.data) { a.data = {}; }
					a.data.str = str;
					return $.ajax(a)
						.fail($.proxy(function () {
							this._data.core.last_error = { 'error': 'ajax', 'plugin': 'search', 'id': 'search_01', 'reason': 'Could not load search parents', 'data': JSON.stringify(a) };
							this.settings.core.error.call(this, this._data.core.last_error);
						}, this))
						.done($.proxy(function (d) {
							if (d && d.d) { d = d.d; }
							this._load_nodes(!$.isArray(d) ? [] : d, function () {
								this.search(str, true);
							});
						}, this));
				}
			}
			this._data.search.str = str;
			this._data.search.dom = $();
			this._data.search.res = [];
			this._data.search.opn = [];

			f = new $.vakata.search(str, true, { caseSensitive: s.case_sensitive, fuzzy: s.fuzzy });

			$.each(this._model.data, function (i, v) {
				if (v.text && f.search(v.text).isMatch && (!s.search_leaves_only || (v.state.loaded && v.children.length === 0))) {
					r.push(i);
					p = p.concat(v.parents);
				}
			});
			if (r.length) {
				p = $.vakata.array_unique(p);
				this._search_open(p);
				for (i = 0, j = r.length; i < j; i++) {
					f = this.get_node(r[i], true);
					if (f) {
						this._data.search.dom = this._data.search.dom.add(f);
					}
				}
				this._data.search.res = r;
				this._data.search.dom.children(".jstree-anchor").addClass('jstree-search');
			}
			/**
			 * triggered after search is complete
			 * @event
			 * @name search.jstree
			 * @param {jQuery} nodes a jQuery collection of matching nodes
			 * @param {String} str the search string
			 * @param {Array} res a collection of objects represeing the matching nodes
			 * @plugin search
			 */
			this.trigger('search', { nodes: this._data.search.dom, str: str, res: this._data.search.res });
		};
		/**
		 * used to clear the last search (removes classes and shows all nodes if filtering is on)
		 * @name clear_search()
		 * @plugin search
		 * @trigger clear_search.jstree
		 */
		this.clear_search = function () {
			this._data.search.dom.children(".jstree-anchor").removeClass("jstree-search");
			if (this.settings.search.close_opened_onclear) {
				this.close_node(this._data.search.opn, 0);
			}
			/**
			 * triggered after search is complete
			 * @event
			 * @name clear_search.jstree
			 * @param {jQuery} nodes a jQuery collection of matching nodes (the result from the last search)
			 * @param {String} str the search string (the last search string)
			 * @param {Array} res a collection of objects represeing the matching nodes (the result from the last search)
			 * @plugin search
			 */
			this.trigger('clear_search', { 'nodes': this._data.search.dom, str: this._data.search.str, res: this._data.search.res });
			this._data.search.str = "";
			this._data.search.res = [];
			this._data.search.opn = [];
			this._data.search.dom = $();
		};
		/**
		 * opens nodes that need to be opened to reveal the search results. Used only internally.
		 * @private
		 * @name _search_open(d)
		 * @param {Array} d an array of node IDs
		 * @plugin search
		 */
		this._search_open = function (d) {
			var t = this;
			$.each(d.concat([]), function (i, v) {
				if (v === "#") { return true; }
				try { v = $('#' + v.replace($.jstree.idregex, '\\$&'), t.element); } catch (ignore) { }
				if (v && v.length) {
					if (t.is_closed(v)) {
						t._data.search.opn.push(v[0].id);
						t.open_node(v, function () { t._search_open(d); }, 0);
					}
				}
			});
		};
	};

	// helpers
	(function ($) {
		// from http://kiro.me/projects/fuse.html
		$.vakata.search = function (pattern, txt, options) {
			options = options || {};
			if (options.fuzzy !== false) {
				options.fuzzy = true;
			}
			pattern = options.caseSensitive ? pattern : pattern.toLowerCase();
			var MATCH_LOCATION = options.location || 0,
				MATCH_DISTANCE = options.distance || 100,
				MATCH_THRESHOLD = options.threshold || 0.6,
				patternLen = pattern.length,
				matchmask, pattern_alphabet, match_bitapScore, search;
			if (patternLen > 32) {
				options.fuzzy = false;
			}
			if (options.fuzzy) {
				matchmask = 1 << (patternLen - 1);
				pattern_alphabet = (function () {
					var mask = {},
						i = 0;
					for (i = 0; i < patternLen; i++) {
						mask[pattern.charAt(i)] = 0;
					}
					for (i = 0; i < patternLen; i++) {
						mask[pattern.charAt(i)] |= 1 << (patternLen - i - 1);
					}
					return mask;
				}());
				match_bitapScore = function (e, x) {
					var accuracy = e / patternLen,
						proximity = Math.abs(MATCH_LOCATION - x);
					if (!MATCH_DISTANCE) {
						return proximity ? 1.0 : accuracy;
					}
					return accuracy + (proximity / MATCH_DISTANCE);
				};
			}
			search = function (text) {
				text = options.caseSensitive ? text : text.toLowerCase();
				if (pattern === text || text.indexOf(pattern) !== -1) {
					return {
						isMatch: true,
						score: 0
					};
				}
				if (!options.fuzzy) {
					return {
						isMatch: false,
						score: 1
					};
				}
				var i, j,
					textLen = text.length,
					scoreThreshold = MATCH_THRESHOLD,
					bestLoc = text.indexOf(pattern, MATCH_LOCATION),
					binMin, binMid,
					binMax = patternLen + textLen,
					lastRd, start, finish, rd, charMatch,
					score = 1,
					locations = [];
				if (bestLoc !== -1) {
					scoreThreshold = Math.min(match_bitapScore(0, bestLoc), scoreThreshold);
					bestLoc = text.lastIndexOf(pattern, MATCH_LOCATION + patternLen);
					if (bestLoc !== -1) {
						scoreThreshold = Math.min(match_bitapScore(0, bestLoc), scoreThreshold);
					}
				}
				bestLoc = -1;
				for (i = 0; i < patternLen; i++) {
					binMin = 0;
					binMid = binMax;
					while (binMin < binMid) {
						if (match_bitapScore(i, MATCH_LOCATION + binMid) <= scoreThreshold) {
							binMin = binMid;
						} else {
							binMax = binMid;
						}
						binMid = Math.floor((binMax - binMin) / 2 + binMin);
					}
					binMax = binMid;
					start = Math.max(1, MATCH_LOCATION - binMid + 1);
					finish = Math.min(MATCH_LOCATION + binMid, textLen) + patternLen;
					rd = new Array(finish + 2);
					rd[finish + 1] = (1 << i) - 1;
					for (j = finish; j >= start; j--) {
						charMatch = pattern_alphabet[text.charAt(j - 1)];
						if (i === 0) {
							rd[j] = ((rd[j + 1] << 1) | 1) & charMatch;
						} else {
							rd[j] = ((rd[j + 1] << 1) | 1) & charMatch | (((lastRd[j + 1] | lastRd[j]) << 1) | 1) | lastRd[j + 1];
						}
						if (rd[j] & matchmask) {
							score = match_bitapScore(i, j - 1);
							if (score <= scoreThreshold) {
								scoreThreshold = score;
								bestLoc = j - 1;
								locations.push(bestLoc);
								if (bestLoc > MATCH_LOCATION) {
									start = Math.max(1, 2 * MATCH_LOCATION - bestLoc);
								} else {
									break;
								}
							}
						}
					}
					if (match_bitapScore(i + 1, MATCH_LOCATION) > scoreThreshold) {
						break;
					}
					lastRd = rd;
				}
				return {
					isMatch: bestLoc >= 0,
					score: score
				};
			};
			return txt === true ? { 'search': search } : search(txt);
		};
	}(jQuery));

	// include the search plugin by default
	// $.jstree.defaults.plugins.push("search");

	/**
	 * ### Sort plugin
	 *
	 * Autmatically sorts all siblings in the tree according to a sorting function.
	 */

	/**
	 * the settings function used to sort the nodes.
	 * It is executed in the tree's context, accepts two nodes as arguments and should return `1` or `-1`.
	 * @name $.jstree.defaults.sort
	 * @plugin sort
	 */
	$.jstree.defaults.sort = function (a, b) {
		//return this.get_type(a) === this.get_type(b) ? (this.get_text(a) > this.get_text(b) ? 1 : -1) : this.get_type(a) >= this.get_type(b);
		return this.get_text(a) > this.get_text(b) ? 1 : -1;
	};
	$.jstree.plugins.sort = function (options, parent) {
		this.bind = function () {
			parent.bind.call(this);
			this.element
				.on("model.jstree", $.proxy(function (e, data) {
					this.sort(data.parent, true);
				}, this))
				.on("rename_node.jstree create_node.jstree", $.proxy(function (e, data) {
					this.sort(data.parent || data.node.parent, false);
					this.redraw_node(data.parent || data.node.parent, true);
				}, this))
				.on("move_node.jstree copy_node.jstree", $.proxy(function (e, data) {
					this.sort(data.parent, false);
					this.redraw_node(data.parent, true);
				}, this));
		};
		/**
		 * used to sort a node's children
		 * @private
		 * @name sort(obj [, deep])
		 * @param  {mixed} obj the node
		 * @param {Boolean} deep if set to `true` nodes are sorted recursively.
		 * @plugin sort
		 * @trigger search.jstree
		 */
		this.sort = function (obj, deep) {
			var i, j;
			obj = this.get_node(obj);
			if (obj && obj.children && obj.children.length) {
				obj.children.sort($.proxy(this.settings.sort, this));
				if (deep) {
					for (i = 0, j = obj.children_d.length; i < j; i++) {
						this.sort(obj.children_d[i], false);
					}
				}
			}
		};
	};

	// include the sort plugin by default
	// $.jstree.defaults.plugins.push("sort");

	/**
	 * ### State plugin
	 *
	 * Saves the state of the tree (selected nodes, opened nodes) on the user's computer using available options (localStorage, cookies, etc)
	 */

	var to = false;
	/**
	 * stores all defaults for the state plugin
	 * @name $.jstree.defaults.state
	 * @plugin state
	 */
	$.jstree.defaults.state = {
		/**
		 * A string for the key to use when saving the current tree (change if using multiple trees in your project). Defaults to `jstree`.
		 * @name $.jstree.defaults.state.key
		 * @plugin state
		 */
		key: 'jstree',
		/**
		 * A space separated list of events that trigger a state save. Defaults to `changed.jstree open_node.jstree close_node.jstree`.
		 * @name $.jstree.defaults.state.events
		 * @plugin state
		 */
		events: 'changed.jstree open_node.jstree close_node.jstree',
		/**
		 * Time in milliseconds after which the state will expire. Defaults to 'false' meaning - no expire.
		 * @name $.jstree.defaults.state.ttl
		 * @plugin state
		 */
		ttl: false,
		/**
		 * A function that will be executed prior to restoring state with one argument - the state object. Can be used to clear unwanted parts of the state.
		 * @name $.jstree.defaults.state.filter
		 * @plugin state
		 */
		filter: false
	};
	$.jstree.plugins.state = function (options, parent) {
		this.bind = function () {
			parent.bind.call(this);
			var bind = $.proxy(function () {
				this.element.on(this.settings.state.events, $.proxy(function () {
					if (to) { clearTimeout(to); }
					to = setTimeout($.proxy(function () { this.save_state(); }, this), 100);
				}, this));
			}, this);
			this.element
				.on("ready.jstree", $.proxy(function (e, data) {
					this.element.one("restore_state.jstree", bind);
					if (!this.restore_state()) { bind(); }
				}, this));
		};
		/**
		 * save the state
		 * @name save_state()
		 * @plugin state
		 */
		this.save_state = function () {
			var st = { 'state': this.get_state(), 'ttl': this.settings.state.ttl, 'sec': +(new Date()) };
			$.vakata.storage.set(this.settings.state.key, JSON.stringify(st));
		};
		/**
		 * restore the state from the user's computer
		 * @name restore_state()
		 * @plugin state
		 */
		this.restore_state = function () {
			var k = $.vakata.storage.get(this.settings.state.key);
			if (!!k) { try { k = JSON.parse(k); } catch (ex) { return false; } }
			if (!!k && k.ttl && k.sec && +(new Date()) - k.sec > k.ttl) { return false; }
			if (!!k && k.state) { k = k.state; }
			if (!!k && $.isFunction(this.settings.state.filter)) { k = this.settings.state.filter.call(this, k); }
			if (!!k) {
				this.element.one("set_state.jstree", function (e, data) { data.instance.trigger('restore_state', { 'state': $.extend(true, {}, k) }); });
				this.set_state(k);
				return true;
			}
			return false;
		};
		/**
		 * clear the state on the user's computer
		 * @name clear_state()
		 * @plugin state
		 */
		this.clear_state = function () {
			return $.vakata.storage.del(this.settings.state.key);
		};
	};

	(function ($, undefined) {
		$.vakata.storage = {
			// simply specifying the functions in FF throws an error
			set: function (key, val) { return window.localStorage.setItem(key, val); },
			get: function (key) { return window.localStorage.getItem(key); },
			del: function (key) { return window.localStorage.removeItem(key); }
		};
	}(jQuery));

	// include the state plugin by default
	// $.jstree.defaults.plugins.push("state");

	/**
	 * ### Types plugin
	 *
	 * Makes it possible to add predefined types for groups of nodes, which make it possible to easily control nesting rules and icon for each group.
	 */

	/**
	 * An object storing all types as key value pairs, where the key is the type name and the value is an object that could contain following keys (all optional).
	 * 
	 * * `max_children` the maximum number of immediate children this node type can have. Do not specify or set to `-1` for unlimited.
	 * * `max_depth` the maximum number of nesting this node type can have. A value of `1` would mean that the node can have children, but no grandchildren. Do not specify or set to `-1` for unlimited.
	 * * `valid_children` an array of node type strings, that nodes of this type can have as children. Do not specify or set to `-1` for no limits.
	 * * `icon` a string - can be a path to an icon or a className, if using an image that is in the current directory use a `./` prefix, otherwise it will be detected as a class. Omit to use the default icon from your theme.
	 *
	 * There are two predefined types:
	 * 
	 * * `#` represents the root of the tree, for example `max_children` would control the maximum number of root nodes.
	 * * `default` represents the default node - any settings here will be applied to all nodes that do not have a type specified.
	 * 
	 * @name $.jstree.defaults.types
	 * @plugin types
	 */
	$.jstree.defaults.types = {
		'#': {},
		'default': {}
	};

	$.jstree.plugins.types = function (options, parent) {
		this.init = function (el, options) {
			var i, j;
			if (options && options.types && options.types['default']) {
				for (i in options.types) {
					if (i !== "default" && i !== "#" && options.types.hasOwnProperty(i)) {
						for (j in options.types['default']) {
							if (options.types['default'].hasOwnProperty(j) && options.types[i][j] === undefined) {
								options.types[i][j] = options.types['default'][j];
							}
						}
					}
				}
			}
			parent.init.call(this, el, options);
			this._model.data['#'].type = '#';
		};
		this.refresh = function (skip_loading) {
			parent.refresh.call(this, skip_loading);
			this._model.data['#'].type = '#';
		};
		this.bind = function () {
			this.element
				.on('model.jstree', $.proxy(function (e, data) {
					var m = this._model.data,
						dpc = data.nodes,
						t = this.settings.types,
						i, j, c = 'default';
					for (i = 0, j = dpc.length; i < j; i++) {
						c = 'default';
						if (m[dpc[i]].original && m[dpc[i]].original.type && t[m[dpc[i]].original.type]) {
							c = m[dpc[i]].original.type;
						}
						if (m[dpc[i]].data && m[dpc[i]].data.jstree && m[dpc[i]].data.jstree.type && t[m[dpc[i]].data.jstree.type]) {
							c = m[dpc[i]].data.jstree.type;
						}
						m[dpc[i]].type = c;
						if (m[dpc[i]].icon === true && t[c].icon !== undefined) {
							m[dpc[i]].icon = t[c].icon;
						}
					}
				}, this));
			parent.bind.call(this);
		};
		this.get_json = function (obj, options, flat) {
			var i, j,
				m = this._model.data,
				opt = options ? $.extend(true, {}, options, { no_id: false }) : {},
				tmp = parent.get_json.call(this, obj, opt, flat);
			if (tmp === false) { return false; }
			if ($.isArray(tmp)) {
				for (i = 0, j = tmp.length; i < j; i++) {
					tmp[i].type = tmp[i].id && m[tmp[i].id] && m[tmp[i].id].type ? m[tmp[i].id].type : "default";
					if (options && options.no_id) {
						delete tmp[i].id;
						if (tmp[i].li_attr && tmp[i].li_attr.id) {
							delete tmp[i].li_attr.id;
						}
					}
				}
			}
			else {
				tmp.type = tmp.id && m[tmp.id] && m[tmp.id].type ? m[tmp.id].type : "default";
				if (options && options.no_id) {
					tmp = this._delete_ids(tmp);
				}
			}
			return tmp;
		};
		this._delete_ids = function (tmp) {
			if ($.isArray(tmp)) {
				for (var i = 0, j = tmp.length; i < j; i++) {
					tmp[i] = this._delete_ids(tmp[i]);
				}
				return tmp;
			}
			delete tmp.id;
			if (tmp.li_attr && tmp.li_attr.id) {
				delete tmp.li_attr.id;
			}
			if (tmp.children && $.isArray(tmp.children)) {
				tmp.children = this._delete_ids(tmp.children);
			}
			return tmp;
		};
		this.check = function (chk, obj, par, pos, more) {
			if (parent.check.call(this, chk, obj, par, pos, more) === false) { return false; }
			obj = obj && obj.id ? obj : this.get_node(obj);
			par = par && par.id ? par : this.get_node(par);
			var m = obj && obj.id ? $.jstree.reference(obj.id) : null, tmp, d, i, j;
			m = m && m._model && m._model.data ? m._model.data : null;
			switch (chk) {
				case "create_node":
				case "move_node":
				case "copy_node":
					if (chk !== 'move_node' || $.inArray(obj.id, par.children) === -1) {
						tmp = this.get_rules(par);
						if (tmp.max_children !== undefined && tmp.max_children !== -1 && tmp.max_children === par.children.length) {
							this._data.core.last_error = { 'error': 'check', 'plugin': 'types', 'id': 'types_01', 'reason': 'max_children prevents function: ' + chk, 'data': JSON.stringify({ 'chk': chk, 'pos': pos, 'obj': obj && obj.id ? obj.id : false, 'par': par && par.id ? par.id : false }) };
							return false;
						}
						if (tmp.valid_children !== undefined && tmp.valid_children !== -1 && $.inArray(obj.type, tmp.valid_children) === -1) {
							this._data.core.last_error = { 'error': 'check', 'plugin': 'types', 'id': 'types_02', 'reason': 'valid_children prevents function: ' + chk, 'data': JSON.stringify({ 'chk': chk, 'pos': pos, 'obj': obj && obj.id ? obj.id : false, 'par': par && par.id ? par.id : false }) };
							return false;
						}
						if (m && obj.children_d && obj.parents) {
							d = 0;
							for (i = 0, j = obj.children_d.length; i < j; i++) {
								d = Math.max(d, m[obj.children_d[i]].parents.length);
							}
							d = d - obj.parents.length + 1;
						}
						if (d <= 0 || d === undefined) { d = 1; }
						do {
							if (tmp.max_depth !== undefined && tmp.max_depth !== -1 && tmp.max_depth < d) {
								this._data.core.last_error = { 'error': 'check', 'plugin': 'types', 'id': 'types_03', 'reason': 'max_depth prevents function: ' + chk, 'data': JSON.stringify({ 'chk': chk, 'pos': pos, 'obj': obj && obj.id ? obj.id : false, 'par': par && par.id ? par.id : false }) };
								return false;
							}
							par = this.get_node(par.parent);
							tmp = this.get_rules(par);
							d++;
						} while (par);
					}
					break;
			}
			return true;
		};
		/**
		 * used to retrieve the type settings object for a node
		 * @name get_rules(obj)
		 * @param {mixed} obj the node to find the rules for
		 * @return {Object}
		 * @plugin types
		 */
		this.get_rules = function (obj) {
			obj = this.get_node(obj);
			if (!obj) { return false; }
			var tmp = this.get_type(obj, true);
			if (tmp.max_depth === undefined) { tmp.max_depth = -1; }
			if (tmp.max_children === undefined) { tmp.max_children = -1; }
			if (tmp.valid_children === undefined) { tmp.valid_children = -1; }
			return tmp;
		};
		/**
		 * used to retrieve the type string or settings object for a node
		 * @name get_type(obj [, rules])
		 * @param {mixed} obj the node to find the rules for
		 * @param {Boolean} rules if set to `true` instead of a string the settings object will be returned
		 * @return {String|Object}
		 * @plugin types
		 */
		this.get_type = function (obj, rules) {
			obj = this.get_node(obj);
			return (!obj) ? false : (rules ? $.extend({ 'type': obj.type }, this.settings.types[obj.type]) : obj.type);
		};
		/**
		 * used to change a node's type
		 * @name set_type(obj, type)
		 * @param {mixed} obj the node to change
		 * @param {String} type the new type
		 * @plugin types
		 */
		this.set_type = function (obj, type) {
			var t, t1, t2, old_type, old_icon;
			if ($.isArray(obj)) {
				obj = obj.slice();
				for (t1 = 0, t2 = obj.length; t1 < t2; t1++) {
					this.set_type(obj[t1], type);
				}
				return true;
			}
			t = this.settings.types;
			obj = this.get_node(obj);
			if (!t[type] || !obj) { return false; }
			old_type = obj.type;
			old_icon = this.get_icon(obj);
			obj.type = type;
			if (old_icon === true || (t[old_type] && t[old_type].icon && old_icon === t[old_type].icon)) {
				this.set_icon(obj, t[type].icon !== undefined ? t[type].icon : true);
			}
			return true;
		};
	};
	// include the types plugin by default
	// $.jstree.defaults.plugins.push("types");

	/**
	 * ### Unique plugin
	 *
	 * Enforces that no nodes with the same name can coexist as siblings.
	 */

	$.jstree.plugins.unique = function (options, parent) {
		this.check = function (chk, obj, par, pos, more) {
			if (parent.check.call(this, chk, obj, par, pos, more) === false) { return false; }
			obj = obj && obj.id ? obj : this.get_node(obj);
			par = par && par.id ? par : this.get_node(par);
			if (!par || !par.children) { return true; }
			var n = chk === "rename_node" ? pos : obj.text,
				c = [],
				m = this._model.data, i, j;
			for (i = 0, j = par.children.length; i < j; i++) {
				c.push(m[par.children[i]].text);
			}
			switch (chk) {
				case "delete_node":
					return true;
				case "rename_node":
				case "copy_node":
					i = ($.inArray(n, c) === -1);
					if (!i) {
						this._data.core.last_error = { 'error': 'check', 'plugin': 'unique', 'id': 'unique_01', 'reason': 'Child with name ' + n + ' already exists. Preventing: ' + chk, 'data': JSON.stringify({ 'chk': chk, 'pos': pos, 'obj': obj && obj.id ? obj.id : false, 'par': par && par.id ? par.id : false }) };
					}
					return i;
				case "move_node":
					i = (obj.parent === par.id || $.inArray(n, c) === -1);
					if (!i) {
						this._data.core.last_error = { 'error': 'check', 'plugin': 'unique', 'id': 'unique_01', 'reason': 'Child with name ' + n + ' already exists. Preventing: ' + chk, 'data': JSON.stringify({ 'chk': chk, 'pos': pos, 'obj': obj && obj.id ? obj.id : false, 'par': par && par.id ? par.id : false }) };
					}
					return i;
			}
			return true;
		};
	};

	// include the unique plugin by default
	// $.jstree.defaults.plugins.push("unique");


	/**
	 * ### Wholerow plugin
	 *
	 * Makes each node appear block level. Making selection easier. May cause slow down for large trees in old browsers.
	 */

	var div = document.createElement('DIV');
	div.setAttribute('unselectable', 'on');
	div.className = 'jstree-wholerow';
	div.innerHTML = '&#160;';
	$.jstree.plugins.wholerow = function (options, parent) {
		this.bind = function () {
			parent.bind.call(this);

			this.element
				.on('loading', $.proxy(function () {
					div.style.height = this._data.core.li_height + 'px';
				}, this))
				.on('ready.jstree set_state.jstree', $.proxy(function () {
					this.hide_dots();
				}, this))
				.on("ready.jstree", $.proxy(function () {
					this.get_container_ul().addClass('jstree-wholerow-ul');
				}, this))
				.on("deselect_all.jstree", $.proxy(function (e, data) {
					this.element.find('.jstree-wholerow-clicked').removeClass('jstree-wholerow-clicked');
				}, this))
				.on("changed.jstree", $.proxy(function (e, data) {
					this.element.find('.jstree-wholerow-clicked').removeClass('jstree-wholerow-clicked');
					var tmp = false, i, j;
					for (i = 0, j = data.selected.length; i < j; i++) {
						tmp = this.get_node(data.selected[i], true);
						if (tmp && tmp.length) {
							tmp.children('.jstree-wholerow').addClass('jstree-wholerow-clicked');
						}
					}
				}, this))
				.on("open_node.jstree", $.proxy(function (e, data) {
					this.get_node(data.node, true).find('.jstree-clicked').parent().children('.jstree-wholerow').addClass('jstree-wholerow-clicked');
				}, this))
				.on("hover_node.jstree dehover_node.jstree", $.proxy(function (e, data) {
					this.get_node(data.node, true).children('.jstree-wholerow')[e.type === "hover_node" ? "addClass" : "removeClass"]('jstree-wholerow-hovered');
				}, this))
				.on("contextmenu.jstree", ".jstree-wholerow", $.proxy(function (e) {
					e.preventDefault();
					var tmp = $.Event('contextmenu', { metaKey: e.metaKey, ctrlKey: e.ctrlKey, altKey: e.altKey, shiftKey: e.shiftKey, pageX: e.pageX, pageY: e.pageY });
					$(e.currentTarget).closest("li").children("a:eq(0)").trigger(tmp);
				}, this))
				.on("click.jstree", ".jstree-wholerow", function (e) {
					e.stopImmediatePropagation();
					var tmp = $.Event('click', { metaKey: e.metaKey, ctrlKey: e.ctrlKey, altKey: e.altKey, shiftKey: e.shiftKey });
					$(e.currentTarget).closest("li").children("a:eq(0)").trigger(tmp).focus();
				})
				.on("click.jstree", ".jstree-leaf > .jstree-ocl", $.proxy(function (e) {
					e.stopImmediatePropagation();
					var tmp = $.Event('click', { metaKey: e.metaKey, ctrlKey: e.ctrlKey, altKey: e.altKey, shiftKey: e.shiftKey });
					$(e.currentTarget).closest("li").children("a:eq(0)").trigger(tmp).focus();
				}, this))
				.on("mouseover.jstree", ".jstree-wholerow, .jstree-icon", $.proxy(function (e) {
					e.stopImmediatePropagation();
					this.hover_node(e.currentTarget);
					return false;
				}, this))
				.on("mouseleave.jstree", ".jstree-node", $.proxy(function (e) {
					this.dehover_node(e.currentTarget);
				}, this));
		};
		this.teardown = function () {
			if (this.settings.wholerow) {
				this.element.find(".jstree-wholerow").remove();
			}
			parent.teardown.call(this);
		};
		this.redraw_node = function (obj, deep, callback) {
			obj = parent.redraw_node.call(this, obj, deep, callback);
			if (obj) {
				var tmp = div.cloneNode(true);
				//tmp.style.height = this._data.core.li_height + 'px';
				if ($.inArray(obj.id, this._data.core.selected) !== -1) { tmp.className += ' jstree-wholerow-clicked'; }
				obj.insertBefore(tmp, obj.childNodes[0]);
			}
			return obj;
		};
	};
	// include the wholerow plugin by default
	// $.jstree.defaults.plugins.push("wholerow");

}));

//#endregion End jsTree

//#region Globalize

/*!
 * Globalize
 *
 * http://github.com/jquery/globalize
 *
 * Copyright Software Freedom Conservancy, Inc.
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 */

(function (window, undefined) {

	var Globalize,
		// private variables
		regexHex,
		regexInfinity,
		regexParseFloat,
		regexTrim,
		// private JavaScript utility functions
		arrayIndexOf,
		endsWith,
		extend,
		isArray,
		isFunction,
		isObject,
		startsWith,
		trim,
		truncate,
		zeroPad,
		// private Globalization utility functions
		appendPreOrPostMatch,
		expandFormat,
		formatDate,
		formatNumber,
		getTokenRegExp,
		getEra,
		getEraYear,
		parseExact,
		parseNegativePattern;

	// Global variable (Globalize) or CommonJS module (globalize)
	Globalize = function (cultureSelector) {
		return new Globalize.prototype.init(cultureSelector);
	};

	if (typeof require !== "undefined" &&
		typeof exports !== "undefined" &&
		typeof module !== "undefined") {
		// Assume CommonJS
		module.exports = Globalize;
	} else {
		// Export as global variable
		window.Globalize = Globalize;
	}

	Globalize.cultures = {};

	Globalize.prototype = {
		constructor: Globalize,
		init: function (cultureSelector) {
			this.cultures = Globalize.cultures;
			this.cultureSelector = cultureSelector;

			return this;
		}
	};
	Globalize.prototype.init.prototype = Globalize.prototype;

	// 1. When defining a culture, all fields are required except the ones stated as optional.
	// 2. Each culture should have a ".calendars" object with at least one calendar named "standard"
	//    which serves as the default calendar in use by that culture.
	// 3. Each culture should have a ".calendar" object which is the current calendar being used,
	//    it may be dynamically changed at any time to one of the calendars in ".calendars".
	Globalize.cultures["default"] = {
		// A unique name for the culture in the form <language code>-<country/region code>
		name: "en",
		// the name of the culture in the english language
		englishName: "English",
		// the name of the culture in its own language
		nativeName: "English",
		// whether the culture uses right-to-left text
		isRTL: false,
		// "language" is used for so-called "specific" cultures.
		// For example, the culture "es-CL" means "Spanish, in Chili".
		// It represents the Spanish-speaking culture as it is in Chili,
		// which might have different formatting rules or even translations
		// than Spanish in Spain. A "neutral" culture is one that is not
		// specific to a region. For example, the culture "es" is the generic
		// Spanish culture, which may be a more generalized version of the language
		// that may or may not be what a specific culture expects.
		// For a specific culture like "es-CL", the "language" field refers to the
		// neutral, generic culture information for the language it is using.
		// This is not always a simple matter of the string before the dash.
		// For example, the "zh-Hans" culture is netural (Simplified Chinese).
		// And the "zh-SG" culture is Simplified Chinese in Singapore, whose lanugage
		// field is "zh-CHS", not "zh".
		// This field should be used to navigate from a specific culture to it's
		// more general, neutral culture. If a culture is already as general as it
		// can get, the language may refer to itself.
		language: "en",
		// numberFormat defines general number formatting rules, like the digits in
		// each grouping, the group separator, and how negative numbers are displayed.
		numberFormat: {
			// [negativePattern]
			// Note, numberFormat.pattern has no "positivePattern" unlike percent and currency,
			// but is still defined as an array for consistency with them.
			//   negativePattern: one of "(n)|-n|- n|n-|n -"
			pattern: ["-n"],
			// number of decimal places normally shown
			decimals: 2,
			// string that separates number groups, as in 1,000,000
			",": ",",
			// string that separates a number from the fractional portion, as in 1.99
			".": ".",
			// array of numbers indicating the size of each number group.
			// TODO: more detailed description and example
			groupSizes: [3],
			// symbol used for positive numbers
			"+": "+",
			// symbol used for negative numbers
			"-": "-",
			// symbol used for NaN (Not-A-Number)
			"NaN": "NaN",
			// symbol used for Negative Infinity
			negativeInfinity: "-Infinity",
			// symbol used for Positive Infinity
			positiveInfinity: "Infinity",
			percent: {
				// [negativePattern, positivePattern]
				//   negativePattern: one of "-n %|-n%|-%n|%-n|%n-|n-%|n%-|-% n|n %-|% n-|% -n|n- %"
				//   positivePattern: one of "n %|n%|%n|% n"
				pattern: ["-n %", "n %"],
				// number of decimal places normally shown
				decimals: 2,
				// array of numbers indicating the size of each number group.
				// TODO: more detailed description and example
				groupSizes: [3],
				// string that separates number groups, as in 1,000,000
				",": ",",
				// string that separates a number from the fractional portion, as in 1.99
				".": ".",
				// symbol used to represent a percentage
				symbol: "%"
			},
			currency: {
				// [negativePattern, positivePattern]
				//   negativePattern: one of "($n)|-$n|$-n|$n-|(n$)|-n$|n-$|n$-|-n $|-$ n|n $-|$ n-|$ -n|n- $|($ n)|(n $)"
				//   positivePattern: one of "$n|n$|$ n|n $"
				pattern: ["($n)", "$n"],
				// number of decimal places normally shown
				decimals: 2,
				// array of numbers indicating the size of each number group.
				// TODO: more detailed description and example
				groupSizes: [3],
				// string that separates number groups, as in 1,000,000
				",": ",",
				// string that separates a number from the fractional portion, as in 1.99
				".": ".",
				// symbol used to represent currency
				symbol: "$"
			}
		},
		// calendars defines all the possible calendars used by this culture.
		// There should be at least one defined with name "standard", and is the default
		// calendar used by the culture.
		// A calendar contains information about how dates are formatted, information about
		// the calendar's eras, a standard set of the date formats,
		// translations for day and month names, and if the calendar is not based on the Gregorian
		// calendar, conversion functions to and from the Gregorian calendar.
		calendars: {
			standard: {
				// name that identifies the type of calendar this is
				name: "Gregorian_USEnglish",
				// separator of parts of a date (e.g. "/" in 11/05/1955)
				"/": "/",
				// separator of parts of a time (e.g. ":" in 05:44 PM)
				":": ":",
				// the first day of the week (0 = Sunday, 1 = Monday, etc)
				firstDay: 0,
				days: {
					// full day names
					names: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
					// abbreviated day names
					namesAbbr: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
					// shortest day names
					namesShort: ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"]
				},
				months: {
					// full month names (13 months for lunar calendards -- 13th month should be "" if not lunar)
					names: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December", ""],
					// abbreviated month names
					namesAbbr: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", ""]
				},
				// AM and PM designators in one of these forms:
				// The usual view, and the upper and lower case versions
				//   [ standard, lowercase, uppercase ]
				// The culture does not use AM or PM (likely all standard date formats use 24 hour time)
				//   null
				AM: ["AM", "am", "AM"],
				PM: ["PM", "pm", "PM"],
				eras: [
					// eras in reverse chronological order.
					// name: the name of the era in this culture (e.g. A.D., C.E.)
					// start: when the era starts in ticks (gregorian, gmt), null if it is the earliest supported era.
					// offset: offset in years from gregorian calendar
					{
						"name": "A.D.",
						"start": null,
						"offset": 0
					}
				],
				// when a two digit year is given, it will never be parsed as a four digit
				// year greater than this year (in the appropriate era for the culture)
				// Set it as a full year (e.g. 2029) or use an offset format starting from
				// the current year: "+19" would correspond to 2029 if the current year 2010.
				twoDigitYearMax: 2029,
				// set of predefined date and time patterns used by the culture
				// these represent the format someone in this culture would expect
				// to see given the portions of the date that are shown.
				patterns: {
					// short date pattern
					d: "M/d/yyyy",
					// long date pattern
					D: "dddd, MMMM dd, yyyy",
					// short time pattern
					t: "h:mm tt",
					// long time pattern
					T: "h:mm:ss tt",
					// long date, short time pattern
					f: "dddd, MMMM dd, yyyy h:mm tt",
					// long date, long time pattern
					F: "dddd, MMMM dd, yyyy h:mm:ss tt",
					// month/day pattern
					M: "MMMM dd",
					// month/year pattern
					Y: "yyyy MMMM",
					// S is a sortable format that does not vary by culture
					S: "yyyy\u0027-\u0027MM\u0027-\u0027dd\u0027T\u0027HH\u0027:\u0027mm\u0027:\u0027ss"
				}
				// optional fields for each calendar:
				/*
				monthsGenitive:
					Same as months but used when the day preceeds the month.
					Omit if the culture has no genitive distinction in month names.
					For an explaination of genitive months, see http://blogs.msdn.com/michkap/archive/2004/12/25/332259.aspx
				convert:
					Allows for the support of non-gregorian based calendars. This convert object is used to
					to convert a date to and from a gregorian calendar date to handle parsing and formatting.
					The two functions:
						fromGregorian( date )
							Given the date as a parameter, return an array with parts [ year, month, day ]
							corresponding to the non-gregorian based year, month, and day for the calendar.
						toGregorian( year, month, day )
							Given the non-gregorian year, month, and day, return a new Date() object
							set to the corresponding date in the gregorian calendar.
				*/
			}
		},
		// For localized strings
		messages: {}
	};

	Globalize.cultures["default"].calendar = Globalize.cultures["default"].calendars.standard;

	Globalize.cultures.en = Globalize.cultures["default"];

	Globalize.cultureSelector = "en";

	//
	// private variables
	//

	regexHex = /^0x[a-f0-9]+$/i;
	regexInfinity = /^[+\-]?infinity$/i;
	regexParseFloat = /^[+\-]?\d*\.?\d*(e[+\-]?\d+)?$/;
	regexTrim = /^\s+|\s+$/g;

	//
	// private JavaScript utility functions
	//

	arrayIndexOf = function (array, item) {
		if (array.indexOf) {
			return array.indexOf(item);
		}
		for (var i = 0, length = array.length; i < length; i++) {
			if (array[i] === item) {
				return i;
			}
		}
		return -1;
	};

	endsWith = function (value, pattern) {
		return value.substr(value.length - pattern.length) === pattern;
	};

	extend = function () {
		var options, name, src, copy, copyIsArray, clone,
			target = arguments[0] || {},
			i = 1,
			length = arguments.length,
			deep = false;

		// Handle a deep copy situation
		if (typeof target === "boolean") {
			deep = target;
			target = arguments[1] || {};
			// skip the boolean and the target
			i = 2;
		}

		// Handle case when target is a string or something (possible in deep copy)
		if (typeof target !== "object" && !isFunction(target)) {
			target = {};
		}

		for (; i < length; i++) {
			// Only deal with non-null/undefined values
			if ((options = arguments[i]) != null) {
				// Extend the base object
				for (name in options) {
					src = target[name];
					copy = options[name];

					// Prevent never-ending loop
					if (target === copy) {
						continue;
					}

					// Recurse if we're merging plain objects or arrays
					if (deep && copy && (isObject(copy) || (copyIsArray = isArray(copy)))) {
						if (copyIsArray) {
							copyIsArray = false;
							clone = src && isArray(src) ? src : [];

						} else {
							clone = src && isObject(src) ? src : {};
						}

						// Never move original objects, clone them
						target[name] = extend(deep, clone, copy);

						// Don't bring in undefined values
					} else if (copy !== undefined) {
						target[name] = copy;
					}
				}
			}
		}

		// Return the modified object
		return target;
	};

	isArray = Array.isArray || function (obj) {
		return Object.prototype.toString.call(obj) === "[object Array]";
	};

	isFunction = function (obj) {
		return Object.prototype.toString.call(obj) === "[object Function]";
	};

	isObject = function (obj) {
		return Object.prototype.toString.call(obj) === "[object Object]";
	};

	startsWith = function (value, pattern) {
		return value.indexOf(pattern) === 0;
	};

	trim = function (value) {
		return (value + "").replace(regexTrim, "");
	};

	truncate = function (value) {
		if (isNaN(value)) {
			return NaN;
		}
		return Math[value < 0 ? "ceil" : "floor"](value);
	};

	zeroPad = function (str, count, left) {
		var l;
		for (l = str.length; l < count; l += 1) {
			str = (left ? ("0" + str) : (str + "0"));
		}
		return str;
	};

	//
	// private Globalization utility functions
	//

	appendPreOrPostMatch = function (preMatch, strings) {
		// appends pre- and post- token match strings while removing escaped characters.
		// Returns a single quote count which is used to determine if the token occurs
		// in a string literal.
		var quoteCount = 0,
			escaped = false;
		for (var i = 0, il = preMatch.length; i < il; i++) {
			var c = preMatch.charAt(i);
			switch (c) {
				case "\'":
					if (escaped) {
						strings.push("\'");
					}
					else {
						quoteCount++;
					}
					escaped = false;
					break;
				case "\\":
					if (escaped) {
						strings.push("\\");
					}
					escaped = !escaped;
					break;
				default:
					strings.push(c);
					escaped = false;
					break;
			}
		}
		return quoteCount;
	};

	expandFormat = function (cal, format) {
		// expands unspecified or single character date formats into the full pattern.
		format = format || "F";
		var pattern,
			patterns = cal.patterns,
			len = format.length;
		if (len === 1) {
			pattern = patterns[format];
			if (!pattern) {
				throw "Invalid date format string \'" + format + "\'.";
			}
			format = pattern;
		}
		else if (len === 2 && format.charAt(0) === "%") {
			// %X escape format -- intended as a custom format string that is only one character, not a built-in format.
			format = format.charAt(1);
		}
		return format;
	};

	formatDate = function (value, format, culture) {
		var cal = culture.calendar,
			convert = cal.convert,
			ret;

		if (!format || !format.length || format === "i") {
			if (culture && culture.name.length) {
				if (convert) {
					// non-gregorian calendar, so we cannot use built-in toLocaleString()
					ret = formatDate(value, cal.patterns.F, culture);
				}
				else {
					var eraDate = new Date(value.getTime()),
						era = getEra(value, cal.eras);
					eraDate.setFullYear(getEraYear(value, cal, era));
					ret = eraDate.toLocaleString();
				}
			}
			else {
				ret = value.toString();
			}
			return ret;
		}

		var eras = cal.eras,
			sortable = format === "s";
		format = expandFormat(cal, format);

		// Start with an empty string
		ret = [];
		var hour,
			zeros = ["0", "00", "000"],
			foundDay,
			checkedDay,
			dayPartRegExp = /([^d]|^)(d|dd)([^d]|$)/g,
			quoteCount = 0,
			tokenRegExp = getTokenRegExp(),
			converted;

		function padZeros(num, c) {
			var r, s = num + "";
			if (c > 1 && s.length < c) {
				r = (zeros[c - 2] + s);
				return r.substr(r.length - c, c);
			}
			else {
				r = s;
			}
			return r;
		}

		function hasDay() {
			if (foundDay || checkedDay) {
				return foundDay;
			}
			foundDay = dayPartRegExp.test(format);
			checkedDay = true;
			return foundDay;
		}

		function getPart(date, part) {
			if (converted) {
				return converted[part];
			}
			switch (part) {
				case 0:
					return date.getFullYear();
				case 1:
					return date.getMonth();
				case 2:
					return date.getDate();
				default:
					throw "Invalid part value " + part;
			}
		}

		if (!sortable && convert) {
			converted = convert.fromGregorian(value);
		}

		for (; ;) {
			// Save the current index
			var index = tokenRegExp.lastIndex,
				// Look for the next pattern
				ar = tokenRegExp.exec(format);

			// Append the text before the pattern (or the end of the string if not found)
			var preMatch = format.slice(index, ar ? ar.index : format.length);
			quoteCount += appendPreOrPostMatch(preMatch, ret);

			if (!ar) {
				break;
			}

			// do not replace any matches that occur inside a string literal.
			if (quoteCount % 2) {
				ret.push(ar[0]);
				continue;
			}

			var current = ar[0],
				clength = current.length;

			switch (current) {
				case "ddd":
					//Day of the week, as a three-letter abbreviation
				case "dddd":
					// Day of the week, using the full name
					var names = (clength === 3) ? cal.days.namesAbbr : cal.days.names;
					ret.push(names[value.getDay()]);
					break;
				case "d":
					// Day of month, without leading zero for single-digit days
				case "dd":
					// Day of month, with leading zero for single-digit days
					foundDay = true;
					ret.push(
						padZeros(getPart(value, 2), clength)
					);
					break;
				case "MMM":
					// Month, as a three-letter abbreviation
				case "MMMM":
					// Month, using the full name
					var part = getPart(value, 1);
					ret.push(
						(cal.monthsGenitive && hasDay()) ?
						(cal.monthsGenitive[clength === 3 ? "namesAbbr" : "names"][part]) :
						(cal.months[clength === 3 ? "namesAbbr" : "names"][part])
					);
					break;
				case "M":
					// Month, as digits, with no leading zero for single-digit months
				case "MM":
					// Month, as digits, with leading zero for single-digit months
					ret.push(
						padZeros(getPart(value, 1) + 1, clength)
					);
					break;
				case "y":
					// Year, as two digits, but with no leading zero for years less than 10
				case "yy":
					// Year, as two digits, with leading zero for years less than 10
				case "yyyy":
					// Year represented by four full digits
					part = converted ? converted[0] : getEraYear(value, cal, getEra(value, eras), sortable);
					if (clength < 4) {
						part = part % 100;
					}
					ret.push(
						padZeros(part, clength)
					);
					break;
				case "h":
					// Hours with no leading zero for single-digit hours, using 12-hour clock
				case "hh":
					// Hours with leading zero for single-digit hours, using 12-hour clock
					hour = value.getHours() % 12;
					if (hour === 0) hour = 12;
					ret.push(
						padZeros(hour, clength)
					);
					break;
				case "H":
					// Hours with no leading zero for single-digit hours, using 24-hour clock
				case "HH":
					// Hours with leading zero for single-digit hours, using 24-hour clock
					ret.push(
						padZeros(value.getHours(), clength)
					);
					break;
				case "m":
					// Minutes with no leading zero for single-digit minutes
				case "mm":
					// Minutes with leading zero for single-digit minutes
					ret.push(
						padZeros(value.getMinutes(), clength)
					);
					break;
				case "s":
					// Seconds with no leading zero for single-digit seconds
				case "ss":
					// Seconds with leading zero for single-digit seconds
					ret.push(
						padZeros(value.getSeconds(), clength)
					);
					break;
				case "t":
					// One character am/pm indicator ("a" or "p")
				case "tt":
					// Multicharacter am/pm indicator
					part = value.getHours() < 12 ? (cal.AM ? cal.AM[0] : " ") : (cal.PM ? cal.PM[0] : " ");
					ret.push(clength === 1 ? part.charAt(0) : part);
					break;
				case "f":
					// Deciseconds
				case "ff":
					// Centiseconds
				case "fff":
					// Milliseconds
					ret.push(
						padZeros(value.getMilliseconds(), 3).substr(0, clength)
					);
					break;
				case "z":
					// Time zone offset, no leading zero
				case "zz":
					// Time zone offset with leading zero
					hour = value.getTimezoneOffset() / 60;
					ret.push(
						(hour <= 0 ? "+" : "-") + padZeros(Math.floor(Math.abs(hour)), clength)
					);
					break;
				case "zzz":
					// Time zone offset with leading zero
					hour = value.getTimezoneOffset() / 60;
					ret.push(
						(hour <= 0 ? "+" : "-") + padZeros(Math.floor(Math.abs(hour)), 2) +
						// Hard coded ":" separator, rather than using cal.TimeSeparator
						// Repeated here for consistency, plus ":" was already assumed in date parsing.
						":" + padZeros(Math.abs(value.getTimezoneOffset() % 60), 2)
					);
					break;
				case "g":
				case "gg":
					if (cal.eras) {
						ret.push(
							cal.eras[getEra(value, eras)].name
						);
					}
					break;
				case "/":
					ret.push(cal["/"]);
					break;
				default:
					throw "Invalid date format pattern \'" + current + "\'.";
			}
		}
		return ret.join("");
	};

	// formatNumber
	(function () {
		var expandNumber;

		expandNumber = function (number, precision, formatInfo) {
			var groupSizes = formatInfo.groupSizes,
				curSize = groupSizes[0],
				curGroupIndex = 1,
				factor = Math.pow(10, precision),
				rounded = Math.round(number * factor) / factor;

			if (!isFinite(rounded)) {
				rounded = number;
			}
			number = rounded;

			var numberString = number + "",
				right = "",
				split = numberString.split(/e/i),
				exponent = split.length > 1 ? parseInt(split[1], 10) : 0;
			numberString = split[0];
			split = numberString.split(".");
			numberString = split[0];
			right = split.length > 1 ? split[1] : "";

			var l;
			if (exponent > 0) {
				right = zeroPad(right, exponent, false);
				numberString += right.slice(0, exponent);
				right = right.substr(exponent);
			}
			else if (exponent < 0) {
				exponent = -exponent;
				numberString = zeroPad(numberString, exponent + 1, true);
				right = numberString.slice(-exponent, numberString.length) + right;
				numberString = numberString.slice(0, -exponent);
			}

			if (precision > 0) {
				right = formatInfo["."] +
					((right.length > precision) ? right.slice(0, precision) : zeroPad(right, precision));
			}
			else {
				right = "";
			}

			var stringIndex = numberString.length - 1,
				sep = formatInfo[","],
				ret = "";

			while (stringIndex >= 0) {
				if (curSize === 0 || curSize > stringIndex) {
					return numberString.slice(0, stringIndex + 1) + (ret.length ? (sep + ret + right) : right);
				}
				ret = numberString.slice(stringIndex - curSize + 1, stringIndex + 1) + (ret.length ? (sep + ret) : "");

				stringIndex -= curSize;

				if (curGroupIndex < groupSizes.length) {
					curSize = groupSizes[curGroupIndex];
					curGroupIndex++;
				}
			}

			return numberString.slice(0, stringIndex + 1) + sep + ret + right;
		};

		formatNumber = function (value, format, culture) {
			if (!isFinite(value)) {
				if (value === Infinity) {
					return culture.numberFormat.positiveInfinity;
				}
				if (value === -Infinity) {
					return culture.numberFormat.negativeInfinity;
				}
				return culture.numberFormat.NaN;
			}
			if (!format || format === "i") {
				return culture.name.length ? value.toLocaleString() : value.toString();
			}
			format = format || "D";

			var nf = culture.numberFormat,
				number = Math.abs(value),
				precision = -1,
				pattern;
			if (format.length > 1) precision = parseInt(format.slice(1), 10);

			var current = format.charAt(0).toUpperCase(),
				formatInfo;

			switch (current) {
				case "D":
					pattern = "n";
					number = truncate(number);
					if (precision !== -1) {
						number = zeroPad("" + number, precision, true);
					}
					if (value < 0) number = "-" + number;
					break;
				case "N":
					formatInfo = nf;
					/* falls through */
				case "C":
					formatInfo = formatInfo || nf.currency;
					/* falls through */
				case "P":
					formatInfo = formatInfo || nf.percent;
					pattern = value < 0 ? formatInfo.pattern[0] : (formatInfo.pattern[1] || "n");
					if (precision === -1) precision = formatInfo.decimals;
					number = expandNumber(number * (current === "P" ? 100 : 1), precision, formatInfo);
					break;
				default:
					throw "Bad number format specifier: " + current;
			}

			var patternParts = /n|\$|-|%/g,
				ret = "";
			for (; ;) {
				var index = patternParts.lastIndex,
					ar = patternParts.exec(pattern);

				ret += pattern.slice(index, ar ? ar.index : pattern.length);

				if (!ar) {
					break;
				}

				switch (ar[0]) {
					case "n":
						ret += number;
						break;
					case "$":
						ret += nf.currency.symbol;
						break;
					case "-":
						// don't make 0 negative
						if (/[1-9]/.test(number)) {
							ret += nf["-"];
						}
						break;
					case "%":
						ret += nf.percent.symbol;
						break;
				}
			}

			return ret;
		};

	}());

	getTokenRegExp = function () {
		// regular expression for matching date and time tokens in format strings.
		return (/\/|dddd|ddd|dd|d|MMMM|MMM|MM|M|yyyy|yy|y|hh|h|HH|H|mm|m|ss|s|tt|t|fff|ff|f|zzz|zz|z|gg|g/g);
	};

	getEra = function (date, eras) {
		if (!eras) return 0;
		var start, ticks = date.getTime();
		for (var i = 0, l = eras.length; i < l; i++) {
			start = eras[i].start;
			if (start === null || ticks >= start) {
				return i;
			}
		}
		return 0;
	};

	getEraYear = function (date, cal, era, sortable) {
		var year = date.getFullYear();
		if (!sortable && cal.eras) {
			// convert normal gregorian year to era-shifted gregorian
			// year by subtracting the era offset
			year -= cal.eras[era].offset;
		}
		return year;
	};

	// parseExact
	(function () {
		var expandYear,
			getDayIndex,
			getMonthIndex,
			getParseRegExp,
			outOfRange,
			toUpper,
			toUpperArray;

		expandYear = function (cal, year) {
			// expands 2-digit year into 4 digits.
			if (year < 100) {
				var now = new Date(),
					era = getEra(now),
					curr = getEraYear(now, cal, era),
					twoDigitYearMax = cal.twoDigitYearMax;
				twoDigitYearMax = typeof twoDigitYearMax === "string" ? new Date().getFullYear() % 100 + parseInt(twoDigitYearMax, 10) : twoDigitYearMax;
				year += curr - (curr % 100);
				if (year > twoDigitYearMax) {
					year -= 100;
				}
			}
			return year;
		};

		getDayIndex = function (cal, value, abbr) {
			var ret,
				days = cal.days,
				upperDays = cal._upperDays;
			if (!upperDays) {
				cal._upperDays = upperDays = [
					toUpperArray(days.names),
					toUpperArray(days.namesAbbr),
					toUpperArray(days.namesShort)
				];
			}
			value = toUpper(value);
			if (abbr) {
				ret = arrayIndexOf(upperDays[1], value);
				if (ret === -1) {
					ret = arrayIndexOf(upperDays[2], value);
				}
			}
			else {
				ret = arrayIndexOf(upperDays[0], value);
			}
			return ret;
		};

		getMonthIndex = function (cal, value, abbr) {
			var months = cal.months,
				monthsGen = cal.monthsGenitive || cal.months,
				upperMonths = cal._upperMonths,
				upperMonthsGen = cal._upperMonthsGen;
			if (!upperMonths) {
				cal._upperMonths = upperMonths = [
					toUpperArray(months.names),
					toUpperArray(months.namesAbbr)
				];
				cal._upperMonthsGen = upperMonthsGen = [
					toUpperArray(monthsGen.names),
					toUpperArray(monthsGen.namesAbbr)
				];
			}
			value = toUpper(value);
			var i = arrayIndexOf(abbr ? upperMonths[1] : upperMonths[0], value);
			if (i < 0) {
				i = arrayIndexOf(abbr ? upperMonthsGen[1] : upperMonthsGen[0], value);
			}
			return i;
		};

		getParseRegExp = function (cal, format) {
			// converts a format string into a regular expression with groups that
			// can be used to extract date fields from a date string.
			// check for a cached parse regex.
			var re = cal._parseRegExp;
			if (!re) {
				cal._parseRegExp = re = {};
			}
			else {
				var reFormat = re[format];
				if (reFormat) {
					return reFormat;
				}
			}

			// expand single digit formats, then escape regular expression characters.
			var expFormat = expandFormat(cal, format).replace(/([\^\$\.\*\+\?\|\[\]\(\)\{\}])/g, "\\\\$1"),
				regexp = ["^"],
				groups = [],
				index = 0,
				quoteCount = 0,
				tokenRegExp = getTokenRegExp(),
				match;

			// iterate through each date token found.
			while ((match = tokenRegExp.exec(expFormat)) !== null) {
				var preMatch = expFormat.slice(index, match.index);
				index = tokenRegExp.lastIndex;

				// don't replace any matches that occur inside a string literal.
				quoteCount += appendPreOrPostMatch(preMatch, regexp);
				if (quoteCount % 2) {
					regexp.push(match[0]);
					continue;
				}

				// add a regex group for the token.
				var m = match[0],
					len = m.length,
					add;
				switch (m) {
					case "dddd": case "ddd":
					case "MMMM": case "MMM":
					case "gg": case "g":
						add = "(\\D+)";
						break;
					case "tt": case "t":
						add = "(\\D*)";
						break;
					case "yyyy":
					case "fff":
					case "ff":
					case "f":
						add = "(\\d{" + len + "})";
						break;
					case "dd": case "d":
					case "MM": case "M":
					case "yy": case "y":
					case "HH": case "H":
					case "hh": case "h":
					case "mm": case "m":
					case "ss": case "s":
						add = "(\\d\\d?)";
						break;
					case "zzz":
						add = "([+-]?\\d\\d?:\\d{2})";
						break;
					case "zz": case "z":
						add = "([+-]?\\d\\d?)";
						break;
					case "/":
						add = "(\\/)";
						break;
					default:
						throw "Invalid date format pattern \'" + m + "\'.";
				}
				if (add) {
					regexp.push(add);
				}
				groups.push(match[0]);
			}
			appendPreOrPostMatch(expFormat.slice(index), regexp);
			regexp.push("$");

			// allow whitespace to differ when matching formats.
			var regexpStr = regexp.join("").replace(/\s+/g, "\\s+"),
				parseRegExp = { "regExp": regexpStr, "groups": groups };

			// cache the regex for this format.
			return re[format] = parseRegExp;
		};

		outOfRange = function (value, low, high) {
			return value < low || value > high;
		};

		toUpper = function (value) {
			// "he-IL" has non-breaking space in weekday names.
			return value.split("\u00A0").join(" ").toUpperCase();
		};

		toUpperArray = function (arr) {
			var results = [];
			for (var i = 0, l = arr.length; i < l; i++) {
				results[i] = toUpper(arr[i]);
			}
			return results;
		};

		parseExact = function (value, format, culture) {
			// try to parse the date string by matching against the format string
			// while using the specified culture for date field names.
			value = trim(value);
			var cal = culture.calendar,
				// convert date formats into regular expressions with groupings.
				// use the regexp to determine the input format and extract the date fields.
				parseInfo = getParseRegExp(cal, format),
				match = new RegExp(parseInfo.regExp).exec(value);
			if (match === null) {
				return null;
			}
			// found a date format that matches the input.
			var groups = parseInfo.groups,
				era = null, year = null, month = null, date = null, weekDay = null,
				hour = 0, hourOffset, min = 0, sec = 0, msec = 0, tzMinOffset = null,
				pmHour = false;
			// iterate the format groups to extract and set the date fields.
			for (var j = 0, jl = groups.length; j < jl; j++) {
				var matchGroup = match[j + 1];
				if (matchGroup) {
					var current = groups[j],
						clength = current.length,
						matchInt = parseInt(matchGroup, 10);
					switch (current) {
						case "dd": case "d":
							// Day of month.
							date = matchInt;
							// check that date is generally in valid range, also checking overflow below.
							if (outOfRange(date, 1, 31)) return null;
							break;
						case "MMM": case "MMMM":
							month = getMonthIndex(cal, matchGroup, clength === 3);
							if (outOfRange(month, 0, 11)) return null;
							break;
						case "M": case "MM":
							// Month.
							month = matchInt - 1;
							if (outOfRange(month, 0, 11)) return null;
							break;
						case "y": case "yy":
						case "yyyy":
							year = clength < 4 ? expandYear(cal, matchInt) : matchInt;
							if (outOfRange(year, 0, 9999)) return null;
							break;
						case "h": case "hh":
							// Hours (12-hour clock).
							hour = matchInt;
							if (hour === 12) hour = 0;
							if (outOfRange(hour, 0, 11)) return null;
							break;
						case "H": case "HH":
							// Hours (24-hour clock).
							hour = matchInt;
							if (outOfRange(hour, 0, 23)) return null;
							break;
						case "m": case "mm":
							// Minutes.
							min = matchInt;
							if (outOfRange(min, 0, 59)) return null;
							break;
						case "s": case "ss":
							// Seconds.
							sec = matchInt;
							if (outOfRange(sec, 0, 59)) return null;
							break;
						case "tt": case "t":
							// AM/PM designator.
							// see if it is standard, upper, or lower case PM. If not, ensure it is at least one of
							// the AM tokens. If not, fail the parse for this format.
							pmHour = cal.PM && (matchGroup === cal.PM[0] || matchGroup === cal.PM[1] || matchGroup === cal.PM[2]);
							if (
								!pmHour && (
									!cal.AM || (matchGroup !== cal.AM[0] && matchGroup !== cal.AM[1] && matchGroup !== cal.AM[2])
								)
							) return null;
							break;
						case "f":
							// Deciseconds.
						case "ff":
							// Centiseconds.
						case "fff":
							// Milliseconds.
							msec = matchInt * Math.pow(10, 3 - clength);
							if (outOfRange(msec, 0, 999)) return null;
							break;
						case "ddd":
							// Day of week.
						case "dddd":
							// Day of week.
							weekDay = getDayIndex(cal, matchGroup, clength === 3);
							if (outOfRange(weekDay, 0, 6)) return null;
							break;
						case "zzz":
							// Time zone offset in +/- hours:min.
							var offsets = matchGroup.split(/:/);
							if (offsets.length !== 2) return null;
							hourOffset = parseInt(offsets[0], 10);
							if (outOfRange(hourOffset, -12, 13)) return null;
							var minOffset = parseInt(offsets[1], 10);
							if (outOfRange(minOffset, 0, 59)) return null;
							tzMinOffset = (hourOffset * 60) + (startsWith(matchGroup, "-") ? -minOffset : minOffset);
							break;
						case "z": case "zz":
							// Time zone offset in +/- hours.
							hourOffset = matchInt;
							if (outOfRange(hourOffset, -12, 13)) return null;
							tzMinOffset = hourOffset * 60;
							break;
						case "g": case "gg":
							var eraName = matchGroup;
							if (!eraName || !cal.eras) return null;
							eraName = trim(eraName.toLowerCase());
							for (var i = 0, l = cal.eras.length; i < l; i++) {
								if (eraName === cal.eras[i].name.toLowerCase()) {
									era = i;
									break;
								}
							}
							// could not find an era with that name
							if (era === null) return null;
							break;
					}
				}
			}
			var result = new Date(), defaultYear, convert = cal.convert;
			defaultYear = convert ? convert.fromGregorian(result)[0] : result.getFullYear();
			if (year === null) {
				year = defaultYear;
			}
			else if (cal.eras) {
				// year must be shifted to normal gregorian year
				// but not if year was not specified, its already normal gregorian
				// per the main if clause above.
				year += cal.eras[(era || 0)].offset;
			}
			// set default day and month to 1 and January, so if unspecified, these are the defaults
			// instead of the current day/month.
			if (month === null) {
				month = 0;
			}
			if (date === null) {
				date = 1;
			}
			// now have year, month, and date, but in the culture's calendar.
			// convert to gregorian if necessary
			if (convert) {
				result = convert.toGregorian(year, month, date);
				// conversion failed, must be an invalid match
				if (result === null) return null;
			}
			else {
				// have to set year, month and date together to avoid overflow based on current date.
				result.setFullYear(year, month, date);
				// check to see if date overflowed for specified month (only checked 1-31 above).
				if (result.getDate() !== date) return null;
				// invalid day of week.
				if (weekDay !== null && result.getDay() !== weekDay) {
					return null;
				}
			}
			// if pm designator token was found make sure the hours fit the 24-hour clock.
			if (pmHour && hour < 12) {
				hour += 12;
			}
			result.setHours(hour, min, sec, msec);
			if (tzMinOffset !== null) {
				// adjust timezone to utc before applying local offset.
				var adjustedMin = result.getMinutes() - (tzMinOffset + result.getTimezoneOffset());
				// Safari limits hours and minutes to the range of -127 to 127.  We need to use setHours
				// to ensure both these fields will not exceed this range.	adjustedMin will range
				// somewhere between -1440 and 1500, so we only need to split this into hours.
				result.setHours(result.getHours() + parseInt(adjustedMin / 60, 10), adjustedMin % 60);
			}
			return result;
		};
	}());

	parseNegativePattern = function (value, nf, negativePattern) {
		var neg = nf["-"],
			pos = nf["+"],
			ret;
		switch (negativePattern) {
			case "n -":
				neg = " " + neg;
				pos = " " + pos;
				/* falls through */
			case "n-":
				if (endsWith(value, neg)) {
					ret = ["-", value.substr(0, value.length - neg.length)];
				}
				else if (endsWith(value, pos)) {
					ret = ["+", value.substr(0, value.length - pos.length)];
				}
				break;
			case "- n":
				neg += " ";
				pos += " ";
				/* falls through */
			case "-n":
				if (startsWith(value, neg)) {
					ret = ["-", value.substr(neg.length)];
				}
				else if (startsWith(value, pos)) {
					ret = ["+", value.substr(pos.length)];
				}
				break;
			case "(n)":
				if (startsWith(value, "(") && endsWith(value, ")")) {
					ret = ["-", value.substr(1, value.length - 2)];
				}
				break;
		}
		return ret || ["", value];
	};

	//
	// public instance functions
	//

	Globalize.prototype.findClosestCulture = function (cultureSelector) {
		return Globalize.findClosestCulture.call(this, cultureSelector);
	};

	Globalize.prototype.format = function (value, format, cultureSelector) {
		return Globalize.format.call(this, value, format, cultureSelector);
	};

	Globalize.prototype.localize = function (key, cultureSelector) {
		return Globalize.localize.call(this, key, cultureSelector);
	};

	Globalize.prototype.parseInt = function (value, radix, cultureSelector) {
		return Globalize.parseInt.call(this, value, radix, cultureSelector);
	};

	Globalize.prototype.parseFloat = function (value, radix, cultureSelector) {
		return Globalize.parseFloat.call(this, value, radix, cultureSelector);
	};

	Globalize.prototype.culture = function (cultureSelector) {
		return Globalize.culture.call(this, cultureSelector);
	};

	//
	// public singleton functions
	//

	Globalize.addCultureInfo = function (cultureName, baseCultureName, info) {

		var base = {},
			isNew = false;

		if (typeof cultureName !== "string") {
			// cultureName argument is optional string. If not specified, assume info is first
			// and only argument. Specified info deep-extends current culture.
			info = cultureName;
			cultureName = this.culture().name;
			base = this.cultures[cultureName];
		} else if (typeof baseCultureName !== "string") {
			// baseCultureName argument is optional string. If not specified, assume info is second
			// argument. Specified info deep-extends specified culture.
			// If specified culture does not exist, create by deep-extending default
			info = baseCultureName;
			isNew = (this.cultures[cultureName] == null);
			base = this.cultures[cultureName] || this.cultures["default"];
		} else {
			// cultureName and baseCultureName specified. Assume a new culture is being created
			// by deep-extending an specified base culture
			isNew = true;
			base = this.cultures[baseCultureName];
		}

		this.cultures[cultureName] = extend(true, {},
			base,
			info
		);
		// Make the standard calendar the current culture if it's a new culture
		if (isNew) {
			this.cultures[cultureName].calendar = this.cultures[cultureName].calendars.standard;
		}
	};

	Globalize.findClosestCulture = function (name) {
		var match;
		if (!name) {
			return this.findClosestCulture(this.cultureSelector) || this.cultures["default"];
		}
		if (typeof name === "string") {
			name = name.split(",");
		}
		if (isArray(name)) {
			var lang,
				cultures = this.cultures,
				list = name,
				i, l = list.length,
				prioritized = [];
			for (i = 0; i < l; i++) {
				name = trim(list[i]);
				var pri, parts = name.split(";");
				lang = trim(parts[0]);
				if (parts.length === 1) {
					pri = 1;
				}
				else {
					name = trim(parts[1]);
					if (name.indexOf("q=") === 0) {
						name = name.substr(2);
						pri = parseFloat(name);
						pri = isNaN(pri) ? 0 : pri;
					}
					else {
						pri = 1;
					}
				}
				prioritized.push({ lang: lang, pri: pri });
			}
			prioritized.sort(function (a, b) {
				if (a.pri < b.pri) {
					return 1;
				} else if (a.pri > b.pri) {
					return -1;
				}
				return 0;
			});
			// exact match
			for (i = 0; i < l; i++) {
				lang = prioritized[i].lang;
				match = cultures[lang];
				if (match) {
					return match;
				}
			}

			// neutral language match
			for (i = 0; i < l; i++) {
				lang = prioritized[i].lang;
				do {
					var index = lang.lastIndexOf("-");
					if (index === -1) {
						break;
					}
					// strip off the last part. e.g. en-US => en
					lang = lang.substr(0, index);
					match = cultures[lang];
					if (match) {
						return match;
					}
				}
				while (1);
			}

			// last resort: match first culture using that language
			for (i = 0; i < l; i++) {
				lang = prioritized[i].lang;
				for (var cultureKey in cultures) {
					var culture = cultures[cultureKey];
					if (culture.language == lang) {
						return culture;
					}
				}
			}
		}
		else if (typeof name === "object") {
			return name;
		}
		return match || null;
	};

	Globalize.format = function (value, format, cultureSelector) {
		var culture = this.findClosestCulture(cultureSelector);
		if (value instanceof Date) {
			value = formatDate(value, format, culture);
		}
		else if (typeof value === "number") {
			value = formatNumber(value, format, culture);
		}
		return value;
	};

	Globalize.localize = function (key, cultureSelector) {
		return this.findClosestCulture(cultureSelector).messages[key] ||
			this.cultures["default"].messages[key];
	};

	Globalize.parseDate = function (value, formats, culture) {
		culture = this.findClosestCulture(culture);

		var date, prop, patterns;
		if (formats) {
			if (typeof formats === "string") {
				formats = [formats];
			}
			if (formats.length) {
				for (var i = 0, l = formats.length; i < l; i++) {
					var format = formats[i];
					if (format) {
						date = parseExact(value, format, culture);
						if (date) {
							break;
						}
					}
				}
			}
		} else {
			patterns = culture.calendar.patterns;
			for (prop in patterns) {
				date = parseExact(value, patterns[prop], culture);
				if (date) {
					break;
				}
			}
		}

		return date || null;
	};

	Globalize.parseInt = function (value, radix, cultureSelector) {
		return truncate(Globalize.parseFloat(value, radix, cultureSelector));
	};

	Globalize.parseFloat = function (value, radix, cultureSelector) {
		// radix argument is optional
		if (typeof radix !== "number") {
			cultureSelector = radix;
			radix = 10;
		}

		var culture = this.findClosestCulture(cultureSelector);
		var ret = NaN,
			nf = culture.numberFormat;

		if (value.indexOf(culture.numberFormat.currency.symbol) > -1) {
			// remove currency symbol
			value = value.replace(culture.numberFormat.currency.symbol, "");
			// replace decimal seperator
			value = value.replace(culture.numberFormat.currency["."], culture.numberFormat["."]);
		}

		//Remove percentage character from number string before parsing
		if (value.indexOf(culture.numberFormat.percent.symbol) > -1) {
			value = value.replace(culture.numberFormat.percent.symbol, "");
		}

		// remove spaces: leading, trailing and between - and number. Used for negative currency pt-BR
		value = value.replace(/ /g, "");

		// allow infinity or hexidecimal
		if (regexInfinity.test(value)) {
			ret = parseFloat(value);
		}
		else if (!radix && regexHex.test(value)) {
			ret = parseInt(value, 16);
		}
		else {

			// determine sign and number
			var signInfo = parseNegativePattern(value, nf, nf.pattern[0]),
				sign = signInfo[0],
				num = signInfo[1];

			// #44 - try parsing as "(n)"
			if (sign === "" && nf.pattern[0] !== "(n)") {
				signInfo = parseNegativePattern(value, nf, "(n)");
				sign = signInfo[0];
				num = signInfo[1];
			}

			// try parsing as "-n"
			if (sign === "" && nf.pattern[0] !== "-n") {
				signInfo = parseNegativePattern(value, nf, "-n");
				sign = signInfo[0];
				num = signInfo[1];
			}

			sign = sign || "+";

			// determine exponent and number
			var exponent,
				intAndFraction,
				exponentPos = num.indexOf("e");
			if (exponentPos < 0) exponentPos = num.indexOf("E");
			if (exponentPos < 0) {
				intAndFraction = num;
				exponent = null;
			}
			else {
				intAndFraction = num.substr(0, exponentPos);
				exponent = num.substr(exponentPos + 1);
			}
			// determine decimal position
			var integer,
				fraction,
				decSep = nf["."],
				decimalPos = intAndFraction.indexOf(decSep);
			if (decimalPos < 0) {
				integer = intAndFraction;
				fraction = null;
			}
			else {
				integer = intAndFraction.substr(0, decimalPos);
				fraction = intAndFraction.substr(decimalPos + decSep.length);
			}
			// handle groups (e.g. 1,000,000)
			var groupSep = nf[","];
			integer = integer.split(groupSep).join("");
			var altGroupSep = groupSep.replace(/\u00A0/g, " ");
			if (groupSep !== altGroupSep) {
				integer = integer.split(altGroupSep).join("");
			}
			// build a natively parsable number string
			var p = sign + integer;
			if (fraction !== null) {
				p += "." + fraction;
			}
			if (exponent !== null) {
				// exponent itself may have a number patternd
				var expSignInfo = parseNegativePattern(exponent, nf, "-n");
				p += "e" + (expSignInfo[0] || "+") + expSignInfo[1];
			}
			if (regexParseFloat.test(p)) {
				ret = parseFloat(p);
			}
		}
		return ret;
	};

	Globalize.culture = function (cultureSelector) {
		// setter
		if (typeof cultureSelector !== "undefined") {
			this.cultureSelector = cultureSelector;
		}
		// getter
		return this.findClosestCulture(cultureSelector) || this.cultures["default"];
	};

}(this));

//#endregion End Globalize

//#region Jeditable
/*
 * Jeditable - jQuery in place edit plugin
 *
 * Copyright (c) 2006-2009 Mika Tuupola, Dylan Verheul
 *
 * Licensed under the MIT license:
 *   http://www.opensource.org/licenses/mit-license.php
 *
 * Project home:
 *   http://www.appelsiini.net/projects/jeditable
 *
 * Based on editable by Dylan Verheul <dylan_at_dyve.net>:
 *    http://www.dyve.net/jquery/?editable
 *
 * Modified by Roger: Added 'oncomplete' event. Search for "[GSP]" to see lines that have been modified.
 *
 */

/**
	* Version 1.7.2-dev
	*
	* ** means there is basic unit tests for this parameter.
	*
	* @name  Jeditable
	* @type  jQuery
	* @param String  target             (POST) URL or function to send edited content to **
	* @param Hash    options            additional options
	* @param String  options[method]    method to use to send edited content (POST or PUT) **
	* @param Function options[callback] Function to run after submitting edited content **
	* @param String  options[name]      POST parameter name of edited content
	* @param String  options[id]        POST parameter name of edited div id
	* @param Hash    options[submitdata] Extra parameters to send when submitting edited content.
	* @param String  options[type]      text, textarea or select (or any 3rd party input type) **
	* @param Integer options[rows]      number of rows if using textarea **
	* @param Integer options[cols]      number of columns if using textarea **
	* @param Mixed   options[height]    'auto', 'none' or height in pixels **
	* @param Mixed   options[width]     'auto', 'none' or width in pixels **
	* @param Mixed   options[widthBuffer] Number of pixels to subtract from auto-calculated width, applies only when width=auto ** [GSP] Added to allow for padding in form elements
	* @param String  options[loadurl]   URL to fetch input content before editing **
	* @param String  options[loadtype]  Request type for load url. Should be GET or POST.
	* @param String  options[loadtext]  Text to display while loading external content.
	* @param Mixed   options[loaddata]  Extra parameters to pass when fetching content before editing.
	* @param Mixed   options[data]      Or content given as paramameter. String or function.**
	* @param String  options[indicator] indicator html to show when saving
	* @param String  options[tooltip]   optional tooltip text via title attribute **
	* @param String  options[event]     jQuery event such as 'click' of 'dblclick' **
	* @param String  options[submit]    submit button value, empty means no button **
	* @param String  options[cancel]    cancel button value, empty means no button **
	* @param String  options[cssclass]  CSS class to apply to input form. 'inherit' to copy from parent. **
	* @param String  options[style]     Style to apply to input form 'inherit' to copy from parent. **
	* @param String  options[select]    true or false, when true text is highlighted ??
	* @param String  options[placeholder] Placeholder text or html to insert when element is empty. **
	* @param String  options[onblur]    'cancel', 'submit', 'ignore' or function ??
	* @param String  options[oncomplete]  [GSP]: Specifies a function to be called after the web service is returned. Can be used to process the returned data.
	* @param String  options[oneditbegin]  [GSP]: Specifies a function to be called when the user initiates an editing action, *before* the DOM is manipulated. Was called onedit in original version
	* @param String  options[oneditend]  [GSP]: Specifies a function to be called when the user initiates an editing action, *after* the DOM is manipulated
	* @param String  options[submitontab]  [GSP]: When true, the form is submitted when the user clicks tab.
	* @param String  options[submitonenter]  [GSP]: When true, the form is submitted when the user clicks enter.
	*
	* @param Function options[onsubmit] function(settings, original) { ... } called before submit
	* @param Function options[onreset]  function(settings, original) { ... } called before reset
	* @param Function options[onerror]  function(settings, original, xhr) { ... } called on error
	*
	* @param Hash    options[ajaxoptions]  jQuery Ajax options. See docs.jquery.com.
	*
	*/

(function ($) {

	$.fn.editable = function (target, options) {

		if ('disable' == target) {
			$(this).data('disabled.editable', true);
			return;
		}
		if ('enable' == target) {
			$(this).data('disabled.editable', false);
			return;
		}
		if ('destroy' == target) {
			$(this)
								.unbind($(this).data('event.editable'))
								.removeData('disabled.editable')
								.removeData('event.editable');
			return;
		}

		var settings = $.extend({}, $.fn.editable.defaults, { target: target }, options);

		/* setup some functions */
		var plugin = $.editable.types[settings.type].plugin || function () { };
		var submit = $.editable.types[settings.type].submit || function () { };
		var buttons = $.editable.types[settings.type].buttons
										|| $.editable.types['defaults'].buttons;
		var content = $.editable.types[settings.type].content
										|| $.editable.types['defaults'].content;
		var element = $.editable.types[settings.type].element
										|| $.editable.types['defaults'].element;
		var reset = $.editable.types[settings.type].reset
										|| $.editable.types['defaults'].reset;
		var callback = settings.callback || function () { };
		var oneditbegin = settings.oneditbegin || settings.onedit || function () { }; // [GSP] If caller specified onedit, use that (preserves backward compatibility)
		var oneditend = settings.oneditend || function () { };
		var onsubmit = settings.onsubmit || function () { };
		var onreset = settings.onreset || function () { };
		var onerror = settings.onerror || reset;
		var oncomplete = settings.oncomplete || function (s) { return s; }; // [GSP]

		/* Show tooltip. */
		if (settings.tooltip) {
			$(this).attr('title', settings.tooltip);
		}

		settings.autowidth = 'auto' == settings.width;
		settings.autoheight = 'auto' == settings.height;

		return this.each(function () {

			/* Save this to self because this changes when scope changes. */
			var self = this;

			/* Inlined block elements lose their width and height after first edit. */
			/* Save them for later use as workaround. */
			var savedwidth = $(self).width();
			var savedheight = $(self).height();

			/* Save so it can be later used by $.editable('destroy') */
			$(this).data('event.editable', settings.event);

			/* If element is empty add something clickable (if requested) */
			if (!$.trim($(this).html())) {
				$(this).html(settings.placeholder);
			}

			$(this).bind(settings.event, function (e) {

				/* Abort if element is disabled. */
				if (true === $(this).data('disabled.editable')) {
					return;
				}

				/* Prevent throwing an exeption if edit field is clicked again. */
				if (self.editing) {
					return;
				}

				/* Abort if oneditbegin hook returns false. */
				if (false === oneditbegin.apply(this, [settings, self, e])) { //[GSP] Passed event object as 3rd parm
					return;
				}

				/* Prevent default action and bubbling. */
				e.preventDefault();
				e.stopPropagation();

				/* Remove tooltip. */
				if (settings.tooltip) {
					$(self).removeAttr('title');
				}

				/* Figure out how wide and tall we are, saved width and height. */
				/* Workaround for http://dev.jquery.com/ticket/2190 */
				if (0 == $(self).width()) {
					settings.width = savedwidth;
					settings.height = savedheight;
				} else {
					if (settings.width != 'none') {
						settings.width = settings.autowidth ? $(self).width() - settings.widthBuffer : settings.width; //[GSP] Subtracted widthBuffer to allow for padding in form elements
					}
					if (settings.height != 'none') {
						settings.height = settings.autoheight ? $(self).outerHeight() : settings.height; //[GSP] Use outerHeight() instead of height() to capture padding
					}
				}

				/* Remove placeholder text, replace is here because of IE. */
				if ($(this).html().toLowerCase().replace(/(;|"|\/)/g, '') ==
										settings.placeholder.toLowerCase().replace(/(;|"|\/)/g, '')) {
					$(this).html('');
				}

				self.editing = true;
				self.revert = $(self).html();
				$(self).html('');

				/* Create the form object. */
				var form = $('<form />');

				/* Apply css or style or both. */
				if (settings.cssclass) {
					if ('inherit' == settings.cssclass) {
						form.attr('class', $(self).attr('class'));
					} else {
						form.attr('class', settings.cssclass);
					}
				}

				if (settings.style) {
					if ('inherit' == settings.style) {
						form.attr('style', $(self).attr('style'));
						/* IE needs the second line or display wont be inherited. */
						form.css('display', $(self).css('display'));
					} else {
						form.attr('style', settings.style);
					}
				}

				/* Add main input element to form and store it in input. */
				var input = element.apply(form, [settings, self]);

				/* Set input content via POST, GET, given data or existing value. */
				var input_content;

				if (settings.loadurl) {
					var t = setTimeout(function () {
						input.disabled = true;
						content.apply(form, [settings.loadtext, settings, self]);
					}, 100);

					var loaddata = {};
					loaddata[settings.id] = self.id;
					if ($.isFunction(settings.loaddata)) {
						$.extend(loaddata, settings.loaddata.apply(self, [self.revert, settings]));
					} else {
						$.extend(loaddata, settings.loaddata);
					}
					$.ajax({
						type: settings.loadtype,
						url: settings.loadurl,
						data: loaddata,
						async: false,
						success: function (result) {
							window.clearTimeout(t);
							input_content = result;
							input.disabled = false;
						}
					});
				} else if (settings.data) {
					input_content = settings.data;
					if ($.isFunction(settings.data)) {
						input_content = settings.data.apply(self, [self.revert, settings]);
					}
				} else {
					input_content = self.revert;
				}
				content.apply(form, [input_content, settings, self]);

				input.attr('name', settings.name);

				/* Add buttons to the form. */
				buttons.apply(form, [settings, self]);

				/* Add created form to self. */
				$(self).append(form);

				/* Attach 3rd party plugin if requested. */
				plugin.apply(form, [settings, self]);

				/* Focus to first visible form element. */
				$(':input:visible:enabled:first', form).focus();

				/* Highlight input contents when requested. */
				if (settings.select) {
					input.select();
				}

				/* discard changes if pressing esc */
				input.keydown(function (e) {
					if (e.keyCode == 27) { // escape
						e.preventDefault();
						reset.apply(form, [settings, self]);
					}
					else if (((e.keyCode == 9 && settings.submitontab) || (e.keyCode == 13 && settings.submitonenter))
						&& ($(this).val().length == 0)) { // [GSP] User clicked tab or enter; submit form
						form.submit();
					}
				});

				/* Discard, submit or nothing with changes when clicking outside. */
				/* Do nothing is usable when navigating with tab. */
				var t;
				if ('cancel' == settings.onblur) {
					input.blur(function (e) {
						/* Prevent canceling if submit was clicked. */
						t = setTimeout(function () {
							reset.apply(form, [settings, self]);
						}, 100); // [GSP] Reduce from 500 ms to 100 ms
					});
				} else if ('submit' == settings.onblur) {
					input.blur(function (e) {
						/* Prevent double submit if submit was clicked. */
						t = setTimeout(function () {
							form.submit();
						}, 200);
					});
				} else if ($.isFunction(settings.onblur)) {
					input.blur(function (e) {
						settings.onblur.apply(self, [input.val(), settings]);
					});
				} else {
					input.blur(function (e) {
						/* TODO: maybe something here */
					});
				}

				form.submit(function (e) {
					if (t) {
						clearTimeout(t);
					}

					/* Do no submit. */
					e.preventDefault();

					/* Call before submit hook. */
					/* If it returns false abort submitting. */
					if (false !== onsubmit.apply(form, [settings, self])) {
						/* Custom inputs call before submit hook. */
						/* If it returns false abort submitting. */
						if (false !== submit.apply(form, [settings, self])) {

							/* Check if given target is function */
							if ($.isFunction(settings.target)) {
								var str = settings.target.apply(self, [input.val(), settings]);
								$(self).html(str);
								self.editing = false;
								callback.apply(self, [self.innerHTML, settings]);
								/* TODO: this is not dry */
								if (!$.trim($(self).html())) {
									$(self).html(settings.placeholder);
								}
							} else {
								/* Add edited content and id of edited element to POST. */
								var submitdata = {};
								submitdata[settings.name] = input.val();
								submitdata[settings.id] = self.id;
								/* Add extra data to be POST:ed. */
								if ($.isFunction(settings.submitdata)) {
									$.extend(submitdata, settings.submitdata.apply(self, [self.revert, settings]));
								} else {
									$.extend(submitdata, settings.submitdata);
								}

								/* Quick and dirty PUT support. */
								if ('PUT' == settings.method) {
									submitdata['_method'] = 'put';
								}

								/* Show the saving indicator. */
								$(self).html(settings.indicator);

								/* Defaults for ajaxoptions. */
								var ajaxoptions = {
									type: 'POST',
									data: submitdata,
									dataType: 'html',
									url: settings.target,
									success: function (result, status) {
										result = oncomplete.apply(self, [result]); // [GSP] Added call to oncomplete event to get updated text
										if (ajaxoptions.dataType == 'html') {
											$(self).html(result);
										}
										self.editing = false;
										callback.apply(self, [result, settings]);
										if (!$.trim($(self).html())) {
											$(self).html(settings.placeholder);
										}
									},
									error: function (xhr, status, error) {
										onerror.apply(form, [settings, self, xhr]);
									}
								};

								/* Override with what is given in settings.ajaxoptions. */
								$.extend(ajaxoptions, settings.ajaxoptions);
								$.ajax(ajaxoptions);

							}
						}
					}

					/* Show tooltip again. */
					$(self).attr('title', settings.tooltip);

					return false;
				});

				oneditend.apply(this, [settings, self]);
			});

			/* Privileged methods */
			this.reset = function (form) {
				/* Prevent calling reset twice when blurring. */
				if (this.editing) {
					/* Before reset hook, if it returns false abort reseting. */
					if (false !== onreset.apply(form, [settings, self])) {
						$(self).html(self.revert);
						self.editing = false;
						if (!$.trim($(self).html())) {
							$(self).html(settings.placeholder);
						}
						/* Show tooltip again. */
						if (settings.tooltip) {
							$(self).attr('title', settings.tooltip);
						}
					}
				}
			};
		});

	};


	$.editable = {
		types: {
			defaults: {
				element: function (settings, original) {
					var input = $('<input type="hidden"></input>');
					$(this).append(input);
					return (input);
				},
				content: function (string, settings, original) {
					string = string.replace(/&amp;/g, '&'); // [GSP]: Replace encoded ampersand with regular one
					$(':input:first', this).val(string);
				},
				reset: function (settings, original) {
					original.reset(this);
				},
				buttons: function (settings, original) {
					var form = this;
					if (settings.submit) {
						/* If given html string use that. */
						if (settings.submit.match(/>$/)) {
							var submit = $(settings.submit).click(function () {
								if (submit.attr("type") != "submit") {
									form.submit();
								}
							});
							/* Otherwise use button with given string as text. */
						} else {
							var submit = $('<button type="submit" />');
							submit.html(settings.submit);
						}
						$(this).append(submit);
					}
					if (settings.cancel) {
						/* If given html string use that. */
						if (settings.cancel.match(/>$/)) {
							var cancel = $(settings.cancel);
							/* otherwise use button with given string as text */
						} else {
							var cancel = $('<button type="cancel" />');
							cancel.html(settings.cancel);
						}
						$(this).append(cancel);

						$(cancel).click(function (event) {
							if ($.isFunction($.editable.types[settings.type].reset)) {
								var reset = $.editable.types[settings.type].reset;
							} else {
								var reset = $.editable.types['defaults'].reset;
							}
							reset.apply(form, [settings, original]);
							return false;
						});
					}
				}
			},
			text: {
				element: function (settings, original) {
					var input = $('<input />');
					if (settings.width != 'none') { input.css('width', settings.width); } // [GSP] Change attr to css for better standards compliance
					if (settings.height != 'none') { input.css('height', settings.height); } // [GSP] Change attr to css for better standards compliance
					/* https://bugzilla.mozilla.org/show_bug.cgi?id=236791 */
					//input[0].setAttribute('autocomplete','off');
					input.attr('autocomplete', 'off');
					$(this).append(input);
					return (input);
				}
			},
			textarea: {
				element: function (settings, original) {
					var textarea = $('<textarea />');
					if (settings.rows) {
						textarea.attr('rows', settings.rows);
					} else if (settings.height != "none") {
						textarea.height(settings.height);
					}
					if (settings.cols) {
						textarea.attr('cols', settings.cols);
					} else if (settings.width != "none") {
						textarea.width(settings.width);
					}
					$(this).append(textarea);
					return (textarea);
				}
			},
			select: {
				element: function (settings, original) {
					var select = $('<select />');
					$(this).append(select);
					return (select);
				},
				content: function (data, settings, original) {
					/* If it is string assume it is json. */
					if (String == data.constructor) {
						eval('var json = ' + data);
					} else {
						/* Otherwise assume it is a hash already. */
						var json = data;
					}
					for (var key in json) {
						if (!json.hasOwnProperty(key)) {
							continue;
						}
						if ('selected' == key) {
							continue;
						}
						var option = $('<option />').val(key).append(json[key]);
						$('select', this).append(option);
					}
					/* Loop option again to set selected. IE needed this... */
					$('select', this).children().each(function () {
						if ($(this).val() == json['selected'] ||
														$(this).text() == $.trim(original.revert)) {
							$(this).attr('selected', 'selected');
						}
					});
					/* Submit on change if no submit button defined. */
					if (!settings.submit) {
						var form = this;
						$('select', this).change(function () {
							form.submit();
						});
					}
				}
			}
		},

		/* Add new input type */
		addInputType: function (name, input) {
			$.editable.types[name] = input;
		}
	};

	/* Publicly accessible defaults. */
	$.fn.editable.defaults = {
		name: 'value',
		id: 'id',
		type: 'text',
		width: 'auto',
		widthBuffer: 0,
		height: 'auto',
		event: 'click.editable',
		onblur: 'cancel',
		submitontab: true,
		submitonenter: true,
		loadtype: 'GET',
		loadtext: 'Loading...',
		placeholder: 'Click to edit',
		loaddata: {},
		submitdata: {},
		ajaxoptions: {}
	};

})(jQuery);

//#endregion End Jeditable

//#region cookie

/*!
 * jQuery Cookie Plugin v1.3.1
 * https://github.com/carhartl/jquery-cookie
 *
 * Copyright 2013 Klaus Hartl
 * Released under the MIT license
 */
(function (factory) {
	if (typeof define === 'function' && define.amd && define.amd.jQuery) {
		// AMD. Register as anonymous module.
		define(['jquery'], factory);
	} else {
		// Browser globals.
		factory(jQuery);
	}
}(function ($) {

	var pluses = /\+/g;

	function raw(s) {
		return s;
	}

	function decoded(s) {
		return decodeURIComponent(s.replace(pluses, ' '));
	}

	function converted(s) {
		if (s.indexOf('"') === 0) {
			// This is a quoted cookie as according to RFC2068, unescape
			s = s.slice(1, -1).replace(/\\"/g, '"').replace(/\\\\/g, '\\');
		}
		try {
			return config.json ? JSON.parse(s) : s;
		} catch (er) { }
	}

	var config = $.cookie = function (key, value, options) {

		// write
		if (value !== undefined) {
			options = $.extend({}, config.defaults, options);

			if (typeof options.expires === 'number') {
				var days = options.expires, t = options.expires = new Date();
				t.setDate(t.getDate() + days);
			}

			value = config.json ? JSON.stringify(value) : String(value);

			return (document.cookie = [
				encodeURIComponent(key), '=', config.raw ? value : encodeURIComponent(value),
				options.expires ? '; expires=' + options.expires.toUTCString() : '', // use expires attribute, max-age is not supported by IE
				options.path ? '; path=' + options.path : '',
				options.domain ? '; domain=' + options.domain : '',
				options.secure ? '; secure' : ''
			].join(''));
		}

		// read
		var decode = config.raw ? raw : decoded;
		var cookies = document.cookie.split('; ');
		var result = key ? undefined : {};
		for (var i = 0, l = cookies.length; i < l; i++) {
			var parts = cookies[i].split('=');
			var name = decode(parts.shift());
			var cookie = decode(parts.join('='));

			if (key && key === name) {
				result = converted(cookie);
				break;
			}

			if (!key) {
				result[name] = converted(cookie);
			}
		}

		return result;
	};

	config.defaults = {};

	$.removeCookie = function (key, options) {
		if ($.cookie(key) !== undefined) {
			$.cookie(key, '', $.extend(options, { expires: -1 }));
			return true;
		}
		return false;
	};

}));

//#endregion End cookie

//#region equalHeights, equalSize

/**
* equalHeights: Make all elements same height according to tallest one in the collection
* equalSize: Make all elements same width & height according to widest and tallest one in the collection
*/
(function ($) {
	jQuery.fn.equalHeights = function (hBuffer) {
		hBuffer = hBuffer || 0;
		return this.height(hBuffer + Math.max.apply(null,
			this.map(function () {
				return jQuery(this).height();
			}).get()
		));
	};

	jQuery.fn.equalWidths = function (wBuffer) {
		wBuffer = wBuffer || 0;
		if ($.support.cssFloat || jQuery(".gsp_i_c", this).length == 0) {
			return this.width(wBuffer + Math.max.apply(null,
				this.map(function () {
					return jQuery(this).width();
				}).get()
			));
		}
		else {
			// Hack for IE7, which makes floated divs that do not have a width assigned 100% wide.
			// We'll grab the width of the child div tag having class 'gsp_i_c' and use that as the
			// basis for our width calculation. (Height calculation remains the same)
			return this.width(wBuffer + 10 + Math.max.apply(null,
				this.map(function () {
					return jQuery(".gsp_i_c", this).width();
				}).get()
			));
		}
	};

	jQuery.fn.equalSize = function (wBuffer, hBuffer) {
		wBuffer = wBuffer || 0;
		hBuffer = hBuffer || 0;
		if ($.support.cssFloat || jQuery(".gsp_i_c", this).length == 0) {
			return this.width(wBuffer + Math.max.apply(null,
				this.map(function () {
					return jQuery(this).width();
				}).get()
			)).height(hBuffer + Math.max.apply(null,
				this.map(function () {
					return jQuery(this).height();
				}).get()
			));
		}
		else {
			// Hack for IE7, which makes floated divs that do not have a width assigned 100% wide.
			// We'll grab the width of the child div tag having class 'gsp_i_c' and use that as the
			// basis for our width calculation. (Height calculation remains the same)
			return this.height(hBuffer + Math.max.apply(null,
				this.map(function () {
					return jQuery(this).height();
				}).get()
			)).width(wBuffer + 10 + Math.max.apply(null,
				this.map(function () {
					return jQuery(".gsp_i_c", this).width();
				}).get()
			));
		}
	};
})(jQuery);

//#endregion

//#region Paging

/**
 * @license jQuery paging plugin v1.1.0 02/05/2013
 * http://www.xarg.org/2011/09/jquery-pagination-revised/
 *
 * Copyright (c) 2011, Robert Eisele (robert@xarg.org)
 * Dual licensed under the MIT or GPL Version 2 licenses.
 **/

(function ($, window, undefined) {


	$["fn"]["paging"] = function (number, opts) {

		var self = this,
		Paging = {

			"setOptions": function (opts) {

				function parseFormat(format) {

					var gndx = 0, group = 0, num = 1, res = {
						fstack: [], // format stack
						asterisk: 0, // asterisk?
						inactive: 0, // fill empty pages with inactives up to w?
						blockwide: 5, // width of number block
						current: 3, // position of current element in number block
						rights: 0, // num of rights
						lefts: 0 // num of lefts
					}, tok, pattern = /[*<>pq\[\]().-]|[nc]+!?/g;

					var known = {
						"[": "first",
						"]": "last",
						"<": "prev",
						">": "next",
						"q": "left",
						"p": "right",
						"-": "fill",
						".": "leap"
					}, count = {};

					while ((tok = pattern["exec"](format))) {

						tok = String(tok);

						if (undefined === known[tok]) {

							if ("(" === tok) {
								group = ++gndx;
							} else if (")" === tok) {
								group = 0;
							} else if (num) {

								if ("*" === tok) {
									res.asterisk = 1;
									res.inactive = 0;
								} else {
									// number block is the only thing left here
									res.asterisk = 0;
									res.inactive = "!" === tok.charAt(tok.length - 1);
									res.blockwide = tok["length"] - res.inactive;
									if (!(res.current = 1 + tok.indexOf("c"))) {
										res.current = (1 + res.blockwide) >> 1;
									}
								}

								res.fstack[res.fstack.length] = ({
									ftype: "block",	// type
									fgroup: 0,		// group
									fpos: 0		// pos
								});
								num = 0;
							}

						} else {

							res.fstack[res.fstack.length] = ({
								ftype: known[tok], // type
								fgroup: group,      // group
								fpos: undefined === count[tok] ? count[tok] = 1 : ++count[tok] // pos
							});

							if ("q" === tok)
								++res.lefts;
							else if ("p" === tok)
								++res.rights;
						}
					}
					return res;
				}

				this.opts = $.extend(this.opts || {
					"lapping": 0,	// number of elements overlap
					"perpage": 10,	// number of elements per page
					"page": 1,	// current page
					"refresh": {
						"interval": 10,
						"url": null
					},	// refresh callback information

					"format": "",	// visual format string

					"onLock": null, // empty callback. set it if you want to lock the entire pagination

					"onFormat": function (type) {	// callback for every format element

						/** EXAMPLE **

		switch (type) {

			case 'block':

				if (!this.active)
					return '<span class="disabled">' + this.value + '</span>';
				else if (this.value != this.page)
					return '<em><a href="#' + this.value + '">' + this.value + '</a></em>';
				return '<span class="current">' + this.value + '</span>';

			case 'right':
			case 'left':

				if (!this.active) {
					return "";
				}
				return '<a href="#' + this.value + '">' + this.value + '</a>';

			case 'next':

				if (this.active) {
					return '<a href="#' + this.value + '" class="next">Next &raquo;</a>';
				}
				return '<span class="disabled">Next &raquo;</span>';

			case 'prev':

				if (this.active) {
					return '<a href="#' + this.value + '" class="prev">&laquo; Previous</a>';
				}
				return '<span class="disabled">&laquo; Previous</span>';

			case 'first':

				if (this.active) {
					return '<a href="#' + this.value + '" class="first">|&lt;</a>';
				}
				return '<span class="disabled">|&lt;</span>';

			case 'last':

				if (this.active) {
					return '<a href="#' + this.value + '" class="prev">&gt;|</a>';
				}
				return '<span class="disabled">&gt;|</span>';

			case 'fill':
				if (this.active) {
					return "...";
				}
		}
		return ""; // return nothing for missing branches

		**/
					},
					"onSelect": function (page) {	// callback for page selection

						/** EXAMPLE SLICE **

var data = this.slice;

content.slice(prev[0], prev[1]).css('display', 'none');
content.slice(data[0], data[1]).css('display', 'block');

prev = data;

**/


						/** EXAMPLE AJAX **

$.ajax({
	"url": '/data.php?start=' + this.slice[0] + '&end=' + this.slice[1] + '&page=' + page,
	"success": function(data) {
		// content replace
	}
});

 **/

						// Return code indicates if the link of the clicked format element should be followed (otherwise only the click-event is used)
						return true;
					},
					"onRefresh": function (json) {// callback for new data of refresh api

						/** EXAMPLE **
		if (json.number) {
			Paging.setNumber(json.number);
		}

		if (json.options) {
			Paging.setOptions(json.options);
		}

		Paging.setPage(); // Call with empty params to reload the paginator
		**/
					}
				}, opts || {});

				this.opts["lapping"] |= 0;
				this.opts["perpage"] |= 0;
				if (this.opts["page"] !== null) this.opts["page"] |= 0;

				// If the number of elements per page is less then 1, set it to default
				if (this.opts["perpage"] < 1) {
					this.opts["perpage"] = 10;
				}

				if (this.interval) window.clearInterval(this.interval);

				if (this.opts["refresh"]["url"]) {

					this.interval = window.setInterval(function (o) {

						$["ajax"]({
							"url": o.opts["refresh"]["url"],
							"success": function (data) {

								if (typeof (data) === "string") {

									try {
										data = $["parseJSON"](data);
									} catch (o) {
										return;
									}
								}
								o.opts["onRefresh"](data);
							}
						});

					}, 1000 * this.opts["refresh"]["interval"], this);
				}

				this.format = parseFormat(this.opts["format"]);
				return this;
			},

			"setNumber": function (number) {
				this.number = (undefined === number || number < 0) ? -1 : number;
				return this;
			},

			"setPage": function (page) {

				if (undefined === page) {

					if (page = this.opts["page"], null === page) {
						return this;
					}

				} else if (this.opts["page"] == page) {
					return this;
				}

				this.opts["page"] = (page |= 0);

				if (null !== this.opts["onLock"]) {
					this.opts["onLock"].call(null, page);
					return this;
				}

				var number = this.number;
				var opts = this.opts;

				var rStart, rStop;

				var pages, buffer;

				var groups = 1, format = this.format;

				var data, tmp, node, lapping;

				var count = format.fstack["length"], i = count;


				// If the lapping is greater than perpage, reduce it to perpage - 1 to avoid endless loops
				if (opts["perpage"] <= opts["lapping"]) {
					opts["lapping"] = opts["perpage"] - 1;
				}

				lapping = number <= opts["lapping"] ? 0 : opts["lapping"] | 0;


				// If the number is negative, the value doesn"t matter, we loop endlessly with a constant width
				if (number < 0) {

					number = -1;
					pages = -1;

					rStart = Math.max(1, page - format.current + 1 - lapping);
					rStop = rStart + format.blockwide;

				} else {

					// Calculate the number of pages
					pages = 1 + Math.ceil((number - opts["perpage"]) / (opts["perpage"] - lapping));

					// If current page is negative, start at the end and
					// Set the current page into a valid range, includes 0, which is set to 1
					page = Math.max(1, Math.min(page < 0 ? 1 + pages + page : page, pages));

					// Do we need to print all numbers?
					if (format.asterisk) {
						rStart = 1;
						rStop = 1 + pages;

						// Disable :first and :last for asterisk mode as we see all buttons
						format.current = page;
						format.blockwide = pages;

					} else {

						// If no, start at the best position and stop at max width or at num of pages
						rStart = Math.max(1, Math.min(page - format.current, pages - format.blockwide) + 1);
						rStop = format.inactive ? rStart + format.blockwide : Math.min(rStart + format.blockwide, 1 + pages);
					}
				}

				while (i--) {

					tmp = 0; // default everything is visible
					node = format.fstack[i];

					switch (node.ftype) {

						case "left":
							tmp = (node.fpos < rStart);
							break;
						case "right":
							tmp = (rStop <= pages - format.rights + node.fpos);
							break;

						case "first":
							tmp = (format.current < page);
							break;
						case "last":
							tmp = (format.blockwide < format.current + pages - page);
							break;

						case "prev":
							tmp = (1 < page);
							break;
						case "next":
							tmp = (page < pages);
							break;
					}
					groups |= tmp << node.fgroup; // group visible?
				}

				data = {
					"number": number,	// number of elements
					"lapping": lapping,	// overlapping
					"pages": pages,	// number of pages
					"perpage": opts["perpage"], // number of elements per page
					"page": page,		// current page
					"slice": [			// two element array with bounds of the current page selection
					(tmp = page * (opts["perpage"] - lapping) + lapping) - opts["perpage"], // Lower bound
					Math.min(tmp, number) // Upper bound
					]
				};

				buffer = "";

				function buffer_append(opts, data, type) {

					type = String(opts["onFormat"].call(data, type));

					if (data["value"])
						buffer += type.replace(/<a/i, '<a data-page="' + data["value"] + '"');
					else
						buffer += type
				}

				while (++i < count) {

					node = format.fstack[i];

					tmp = (groups >> node.fgroup & 1);

					switch (node.ftype) {
						case "block":
							for (; rStart < rStop; ++rStart) {

								data["value"] = rStart;
								data["pos"] = 1 + format.blockwide - rStop + rStart;

								data["active"] = rStart <= pages || number < 0;     // true if infinity series and rStart <= pages
								data["first"] = 1 === rStart;                      // check if it is the first page
								data["last"] = rStart == pages && 0 < number;     // false if infinity series or rStart != pages

								buffer_append(opts, data, node.ftype);
							}
							continue;

						case "left":
							data["value"] = node.fpos;
							data["active"] = node.fpos < rStart; // Don't take group-visibility into account!
							break;

						case "right":
							data["value"] = pages - format.rights + node.fpos;
							data["active"] = rStop <= data["value"]; // Don't take group-visibility into account!
							break;

						case "first":
							data["value"] = 1;
							data["active"] = tmp && 1 < page;
							break;

						case "prev":
							data["value"] = Math.max(1, page - 1);
							data["active"] = tmp && 1 < page;
							break;

						case "last":
							if ((data["active"] = (number < 0))) {
								data["value"] = 1 + page;
							} else {
								data["value"] = pages;
								data["active"] = tmp && page < pages;
							}
							break;

						case "next":
							if ((data["active"] = (number < 0))) {
								data["value"] = 1 + page;
							} else {
								data["value"] = Math.min(1 + page, pages);
								data["active"] = tmp && page < pages;
							}
							break;

						case "leap":
						case "fill":
							data["pos"] = node.fpos;
							data["active"] = tmp; // tmp is true by default and changes only for group behaviour
							buffer_append(opts, data, node.ftype);
							continue;
					}

					data["pos"] = node.fpos;
					data["last"] = /* void */
					data["first"] = undefined;

					buffer_append(opts, data, node.ftype);
				}

				if (self.length) {

					$("a", self["html"](buffer)).click(function (ev) {
						ev["preventDefault"]();

						var obj = this;

						do {

							if ('a' === obj["nodeName"].toLowerCase()) {
								break;
							}

						} while ((obj = obj["parentNode"]));

						Paging["setPage"]($(obj).data("page"));

						if (Paging.locate) {
							window.location = obj["href"];
						}
					});

					this.locate = opts["onSelect"].call({
						"number": number,
						"lapping": lapping,
						"pages": pages,
						"slice": data["slice"]
					}, page);
				}
				return this;
			}
		};

		return Paging
		["setNumber"](number)
		["setOptions"](opts)
		["setPage"]();
	}

}(jQuery, this));

//#endregion End Paging

//#region Splitter

/*
 * jQuery.splitter.js - two-pane splitter window plugin
 *
 * version 1.6 (2010/01/03)
 * version 1.61 (2012/05/09) -- Fixes by Roger Martin
 *  * Added check in window resize event handler to run only when the target is the window. This fixes a breaking
 *    change introduced in jQuery 1.6.
 *  * Added support for IE 9+
 * version 1.62 (2012/05/16) -- Fixes by Roger Martin
 *  * Included bottom padding of body and html elements when calculating height. This elimates vertical scroll bar and thus a need for overflow:none on the body element
 * version 1.63 (2012/08/12) -- Fixes by Roger Martin
 *  * Changed curCSS to css (curCSS was removed in jQuery 1.8)
 * version 1.64 (2013/01/08) -- Fixes by Roger Martin
 *  * sizeLeft and sizeRight was being ignored when cookie option was used
 * version 1.65 (2013/01/09) -- Fixes by Roger Martin
 *  * Fixed issue where scrollbars were still appearing in IE.
 *
 * Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 */

/**
 * The splitter() plugin implements a two-pane resizable splitter window.
 * The selected elements in the jQuery object are converted to a splitter;
 * each selected element should have two child elements, used for the panes
 * of the splitter. The plugin adds a third child element for the splitbar.
 *
 * For more details see: http://www.methvin.com/splitter/
 *
 *
 * @example $('#MySplitter').splitter();
 * @desc Create a vertical splitter with default settings
 *
 * @example $('#MySplitter').splitter({type: 'h', accessKey: 'M'});
 * @desc Create a horizontal splitter resizable via Alt+Shift+M
 *
 * @name splitter
 * @type jQuery
 * @param Object options Options for the splitter (not required)
 * @cat Plugins/Splitter
 * @return jQuery
 * @author Dave Methvin (dave.methvin@gmail.com)
 */
; (function ($) {

	var splitterCounter = 0;

	$.fn.splitter = function (args) {
		args = args || {};
		return this.each(function () {
			if ($(this).is(".splitter"))	// already a splitter
				return;
			var zombie;		// left-behind splitbar for outline resizes
			function setBarState(state) {
				bar.removeClass(opts.barStateClasses).addClass(state);
			}
			function startSplitMouse(evt) {
				if (evt.which != 1)
					return;		// left button only
				bar.removeClass(opts.barHoverClass);
				if (opts.outline) {
					zombie = zombie || bar.clone(false).insertAfter(A);
					bar.removeClass(opts.barDockedClass);
				}
				setBarState(opts.barActiveClass)
				// Safari selects A/B text on a move; iframes capture mouse events so hide them
				panes.css("-webkit-user-select", "none").find("iframe").addClass(opts.iframeClass);
				A._posSplit = A[0][opts.pxSplit] - evt[opts.eventPos];
				$(document)
			 .bind("mousemove" + opts.eventNamespace, doSplitMouse)
			 .bind("mouseup" + opts.eventNamespace, endSplitMouse);
			}
			function doSplitMouse(evt) {
				var pos = A._posSplit + evt[opts.eventPos],
			 range = Math.max(0, Math.min(pos, splitter._DA - bar._DA)),
			 limit = Math.max(A._min, splitter._DA - B._max,
					 Math.min(pos, A._max, splitter._DA - bar._DA - B._min));
				if (opts.outline) {
					// Let docking splitbar be dragged to the dock position, even if min width applies
					if ((opts.dockPane == A && pos < Math.max(A._min, bar._DA)) ||
					(opts.dockPane == B && pos > Math.min(pos, A._max, splitter._DA - bar._DA - B._min))) {
						bar.addClass(opts.barDockedClass).css(opts.origin, range);
					}
					else {
						bar.removeClass(opts.barDockedClass).css(opts.origin, limit);
					}
					bar._DA = bar[0][opts.pxSplit];
				} else
					resplit(pos);
				setBarState(pos == limit ? opts.barActiveClass : opts.barLimitClass);
			}
			function endSplitMouse(evt) {
				setBarState(opts.barNormalClass);
				bar.addClass(opts.barHoverClass);
				var pos = A._posSplit + evt[opts.eventPos];
				if (opts.outline) {
					zombie.remove(); zombie = null;
					resplit(pos);
				}
				panes.css("-webkit-user-select", "text").find("iframe").removeClass(opts.iframeClass);
				$(document)
			 .unbind("mousemove" + opts.eventNamespace + " mouseup" + opts.eventNamespace);
			}
			function resplit(pos) {
				bar._DA = bar[0][opts.pxSplit];		// bar size may change during dock
				// Constrain new splitbar position to fit pane size and docking limits
				if ((opts.dockPane == A && pos < Math.max(A._min, bar._DA)) ||
				(opts.dockPane == B && pos > Math.min(pos, A._max, splitter._DA - bar._DA - B._min))) {
					bar.addClass(opts.barDockedClass);
					bar._DA = bar[0][opts.pxSplit];
					pos = opts.dockPane == A ? 0 : splitter._DA - bar._DA;
					if (bar._pos == null)
						bar._pos = A[0][opts.pxSplit];
				}
				else {
					bar.removeClass(opts.barDockedClass);
					bar._DA = bar[0][opts.pxSplit];
					bar._pos = null;
					pos = Math.max(A._min, splitter._DA - B._max,
					 Math.min(pos, A._max, splitter._DA - bar._DA - B._min));
				}
				// Resize/position the two panes
				bar.css(opts.origin, pos).css(opts.fixed, splitter._DF);
				A.css(opts.origin, 0).css(opts.split, pos).css(opts.fixed, splitter._DF);
				B.css(opts.origin, pos + bar._DA)
			 .css(opts.split, splitter._DA - bar._DA - pos).css(opts.fixed, splitter._DF);
				// IE fires resize for us; all others pay cash
				if (!browser_resize_auto_fired()) {
					for (i = 0; i <= splitterCounter; i++) {
						panes.trigger("resize" + eventNamespaceBase + i);
					}
				}
			}
			function dimSum(jq, dims) {
				// Opera returns -1 for missing min/max width, turn into 0
				var sum = 0;
				for (var i = 1; i < arguments.length; i++)
					sum += Math.max(parseInt(jq.css(arguments[i]), 10) || 0, 0);
				return sum;
			}
			function browser_resize_auto_fired() {
				// Returns true when the browser natively fires the resize event attached to the panes elements
				return ($.browser.msie && (parseInt($.browser.version) < 9));
			}

			// Determine settings based on incoming opts, element classes, and defaults
			var vh = (args.splitHorizontal ? 'h' : args.splitVertical ? 'v' : args.type) || 'v';
			var eventNamespaceBase = ".splitter";
			var opts = $.extend({
				// Defaults here allow easy use with ThemeRoller
				splitterClass: "splitter gsp-ui-widget gsp-ui-widget-content",
				paneClass: "splitter-pane",
				barClass: "splitter-bar",
				barNormalClass: "gsp-ui-state-default",			// splitbar normal
				barHoverClass: "gsp-ui-state-hover",			// splitbar mouse hover
				barActiveClass: "gsp-ui-state-highlight",		// splitbar being moved
				barLimitClass: "gsp-ui-state-error",			// splitbar at limit
				iframeClass: "splitter-iframe-hide",		// hide iframes during split
				eventNamespace: eventNamespaceBase + (++splitterCounter),
				pxPerKey: 8,			// splitter px moved per keypress
				tabIndex: 0,			// tab order indicator
				accessKey: ''			// accessKey for splitbar
			}, {
				// user can override
				v: {					// Vertical splitters:
					keyLeft: 39, keyRight: 37, cursor: "e-resize",
					barStateClass: "splitter-bar-vertical",
					barDockedClass: "splitter-bar-vertical-docked"
				},
				h: {					// Horizontal splitters:
					keyTop: 40, keyBottom: 38, cursor: "n-resize",
					barStateClass: "splitter-bar-horizontal",
					barDockedClass: "splitter-bar-horizontal-docked"
				}
			}[vh], args, {
				// user cannot override
				v: {					// Vertical splitters:
					type: 'v', eventPos: "pageX", origin: "left",
					split: "width", pxSplit: "offsetWidth", side1: "Left", side2: "Right",
					fixed: "height", pxFixed: "offsetHeight", side3: "Top", side4: "Bottom"
				},
				h: {					// Horizontal splitters:
					type: 'h', eventPos: "pageY", origin: "top",
					split: "height", pxSplit: "offsetHeight", side1: "Top", side2: "Bottom",
					fixed: "width", pxFixed: "offsetWidth", side3: "Left", side4: "Right"
				}
			}[vh]);
			opts.barStateClasses = [opts.barNormalClass, opts.barHoverClass, opts.barActiveClass, opts.barLimitClass].join(' ');

			// Create jQuery object closures for splitter and both panes
			var splitter = $(this).css({ position: "relative" }).addClass(opts.splitterClass);
			var panes = $(">*", splitter[0]).addClass(opts.paneClass).css({
				position: "absolute", 			// positioned inside splitter container
				"z-index": "1",					// splitbar is positioned above
				"-moz-outline-style": "none"	// don't show dotted outline
			});
			var A = $(panes[0]), B = $(panes[1]);	// A = left/top, B = right/bottom
			opts.dockPane = opts.dock && (/right|bottom/.test(opts.dock) ? B : A);

			// Focuser element, provides keyboard support; title is shown by Opera accessKeys
			var focuser = $('<a href="javascript:void(0)"></a>')
		 .attr({ accessKey: opts.accessKey, tabIndex: opts.tabIndex, title: opts.splitbarClass })
		 .bind(($.browser.opera ? "click" : "focus") + opts.eventNamespace,
			 function () { this.focus(); bar.addClass(opts.barActiveClass) })
		 .bind("keydown" + opts.eventNamespace, function (e) {
			 var key = e.which || e.keyCode;
			 var dir = key == opts["key" + opts.side1] ? 1 : key == opts["key" + opts.side2] ? -1 : 0;
			 if (dir)
				 resplit(A[0][opts.pxSplit] + dir * opts.pxPerKey, false);
		 })
		 .bind("blur" + opts.eventNamespace,
			 function () { bar.removeClass(opts.barActiveClass) });

			// Splitbar element
			var bar = $('<div></div>')
		 .insertAfter(A).addClass(opts.barClass).addClass(opts.barStateClass)
		 .append(focuser).attr({ unselectable: "on" })
		 .css({
			 position: "absolute", "user-select": "none", "-webkit-user-select": "none",
			 "-khtml-user-select": "none", "-moz-user-select": "none", "z-index": "100"
		 })
		 .bind("mousedown" + opts.eventNamespace, startSplitMouse)
		 .bind("mouseover" + opts.eventNamespace, function () {
			 $(this).addClass(opts.barHoverClass);
		 })
		 .bind("mouseout" + opts.eventNamespace, function () {
			 $(this).removeClass(opts.barHoverClass);
		 });
			// Use our cursor unless the style specifies a non-default cursor
			if (/^(auto|default|)$/.test(bar.css("cursor")))
				bar.css("cursor", opts.cursor);

			// Cache several dimensions for speed, rather than re-querying constantly
			// These are saved on the A/B/bar/splitter jQuery vars, which are themselves cached
			// DA=dimension adjustable direction, PBF=padding/border fixed, PBA=padding/border adjustable
			bar._DA = bar[0][opts.pxSplit];
			splitter._PBF = dimSum(splitter, "border" + opts.side3 + "Width", "border" + opts.side4 + "Width");
			splitter._PBA = dimSum(splitter, "border" + opts.side1 + "Width", "border" + opts.side2 + "Width");
			A._pane = opts.side1;
			B._pane = opts.side2;
			$.each([A, B], function () {
				this._splitter_style = this.style;
				this._min = opts["min" + this._pane] || dimSum(this, "min-" + opts.split);
				this._max = opts["max" + this._pane] || dimSum(this, "max-" + opts.split) || 9999;
				this._init = opts["size" + this._pane] === true ?
			 parseInt($.css(this[0], opts.split), 10) : opts["size" + this._pane]; //[RDM] Changed curCSS to css (curCSS was removed in jQuery 1.8)
			});

			// Determine initial position, get from cookie if specified
			var initPos = A._init;
			if (!isNaN(B._init))	// recalc initial B size as an offset from the top or left side
				initPos = splitter[0][opts.pxSplit] - splitter._PBA - B._init - bar._DA;
			if (opts.cookie) {
				if (!$.cookie)
					alert('jQuery.splitter(): jQuery cookie plugin required');
				var cookieVal = parseInt($.cookie(opts.cookie), 10);
				if (!isNaN(cookieVal))
					initPos = cookieVal; //[RDM] Overwrite initPos only when we found a cookie (instead of always)
				$(window).bind("unload" + opts.eventNamespace, function () {
					var state = String(bar.css(opts.origin));	// current location of splitbar
					$.cookie(opts.cookie, state, {
						expires: opts.cookieExpires || 365,
						path: opts.cookiePath || document.location.pathname
					});
				});
			}
			if (isNaN(initPos))	// King Solomon's algorithm
				initPos = Math.round((splitter[0][opts.pxSplit] - splitter._PBA - bar._DA) / 2);

			// Resize event propagation and splitter sizing
			if (opts.anchorToWindow)
				opts.resizeTo = window;
			if (opts.resizeTo) {
				splitter._hadjust = dimSum(splitter, "borderTopWidth", "borderBottomWidth", "marginBottom", "paddingBottom");
				splitter._hadjust += dimSum($('body'), 'paddingBottom'); // Added by Roger
				splitter._hadjust += dimSum($('html'), 'paddingBottom'); // Added by Roger
				splitter._hadjust += 1; // [RDM] Need a fudge factor of one extra pixel to prevent scrollbars in IE & Chrome
				splitter._hmin = Math.max(dimSum(splitter, "minHeight"), 20);
				$(window).bind("resize" + opts.eventNamespace, function (e) {
					if (e.target == window) {
						var top = splitter.offset().top;
						var eh = $(opts.resizeTo).height();
						splitter.css("height", Math.max(eh - top - splitter._hadjust - 0, splitter._hmin) + "px");
						if (!browser_resize_auto_fired()) splitter.trigger("resize" + opts.eventNamespace);
					}
				}).trigger("resize" + opts.eventNamespace);
			}
			else if (opts.resizeToWidth && !browser_resize_auto_fired()) {
				$(window).bind("resize" + opts.eventNamespace, function (e) {
					if (e.target == window) {
						splitter.trigger("resize" + opts.eventNamespace);
					}
				});
			}

			// Docking support
			if (opts.dock) {
				splitter
			 .bind("toggleDock" + opts.eventNamespace, function () {
				 var pw = opts.dockPane[0][opts.pxSplit];
				 splitter.trigger(pw ? "dock" + opts.eventNamespace : "undock" + opts.eventNamespace);
			 })
			 .bind("dock" + opts.eventNamespace, function () {
				 var pw = A[0][opts.pxSplit];
				 if (!pw) return;
				 bar._pos = pw;
				 var x = {};
				 x[opts.origin] = opts.dockPane == A ? 0 :
					 splitter[0][opts.pxSplit] - splitter._PBA - bar[0][opts.pxSplit];
				 bar.animate(x, opts.dockSpeed || 1, opts.dockEasing, function () {
					 bar.addClass(opts.barDockedClass);
					 resplit(x[opts.origin]);
				 });
			 })
			 .bind("undock" + opts.eventNamespace, function () {
				 var pw = opts.dockPane[0][opts.pxSplit];
				 if (pw) return;
				 var x = {}; x[opts.origin] = bar._pos + "px";
				 bar.removeClass(opts.barDockedClass)
					 .animate(x, opts.undockSpeed || opts.dockSpeed || 1, opts.undockEasing || opts.dockEasing, function () {
						 resplit(bar._pos);
						 bar._pos = null;
					 });
			 });
				if (opts.dockKey)
					$('<a title="' + opts.splitbarClass + ' toggle dock" href="javascript:void(0)"></a>')
				 .attr({ accessKey: opts.dockKey, tabIndex: -1 }).appendTo(bar)
				 .bind($.browser.opera ? "click" : "focus", function () {
					 splitter.trigger("toggleDock" + opts.eventNamespace); this.blur();
				 });
				bar.bind("dblclick", function () { splitter.trigger("toggleDock" + opts.eventNamespace); });
			}


			// Resize event handler; triggered immediately to set initial position
			splitter
		 .bind("destroy" + opts.eventNamespace, function () {
			 $([window, document]).unbind(opts.eventNamespace);
			 bar.unbind().remove();
			 panes.removeClass(opts.paneClass);
			 splitter
				 .removeClass(opts.splitterClass)
				 .add(panes)
					 .unbind(opts.eventNamespace)
					 .attr("style", function (el) {
						 return this._splitter_style || "";	//TODO: save style
					 });
			 splitter = bar = focuser = panes = A = B = opts = args = null;
		 })
		 .bind("resize" + opts.eventNamespace, function (e, size) {
			 // Custom events bubble in jQuery 1.3; avoid recursion
			 if (e.target != this) return;
			 // Determine new width/height of splitter container
			 splitter._DF = splitter[0][opts.pxFixed] - splitter._PBF;
			 splitter._DA = splitter[0][opts.pxSplit] - splitter._PBA;
			 // Bail if splitter isn't visible or content isn't there yet
			 if (splitter._DF <= 0 || splitter._DA <= 0) return;
			 // Re-divvy the adjustable dimension; maintain size of the preferred pane
			 resplit(!isNaN(size) ? size : (!(opts.sizeRight || opts.sizeBottom) ? A[0][opts.pxSplit] :
				 splitter._DA - B[0][opts.pxSplit] - bar._DA));
			 setBarState(opts.barNormalClass);
		 })
		 .trigger("resize" + opts.eventNamespace, [initPos]);
		});
	};

})(jQuery);

//#endregion End Splitter

//#region autoSuggest

/*
* AutoSuggest
* Copyright 2009-2010 Drew Wilson
* www.drewwilson.com
* http://code.drewwilson.com/entry/autosuggest-jquery-plugin
*
* Version 1.4   -   Updated: Mar. 23, 2010
*
* This Plug-In will auto-complete or auto-suggest completed search queries
* for you as you type. You can add multiple selections and remove them on
* the fly. It supports keybord navigation (UP + DOWN + RETURN), as well
* as multiple AutoSuggest fields on the same page.
*
* Inspied by the Autocomplete plugin by: Jšrn Zaefferer
* and the Facelist plugin by: Ian Tearle (iantearle.com)
*
* This AutoSuggest jQuery plug-in is dual licensed under the MIT and GPL licenses:
*   http://www.opensource.org/licenses/mit-license.php
*   http://www.gnu.org/licenses/gpl.html
*/

(function ($) {
	$.fn.autoSuggest = function (data, options) {
		var defaults = {
			asHtmlID: false,
			startText: "Enter Name Here",
			emptyText: "No Results Found",
			preFill: {},
			limitText: "No More Selections Are Allowed",
			selectedItemProp: "value", //name of object property
			selectedValuesProp: "value", //name of object property
			searchObjProps: "value", //comma separated list of object property names
			queryParam: "q",
			retrieveLimit: false, //number for 'limit' param on ajax request
			extraParams: "",
			matchCase: false,
			minChars: 1,
			keyDelay: 400,
			resultsHighlight: true,
			neverSubmit: false,
			selectionLimit: false,
			showResultList: true,
			start: function () { },
			selectionClick: function (elem) { },
			selectionAdded: function (elem) { },
			selectionRemoved: function (elem) { elem.remove(); },
			formatList: false, //callback function
			beforeRetrieve: function (string) { return string; },
			retrieveComplete: function (data) { return data; },
			resultClick: function (data) { },
			resultsComplete: function () { }
		};
		var opts = $.extend(defaults, options);

		var d_type = "object";
		var d_count = 0;
		if (typeof data == "string") {
			d_type = "string";
			var req_string = data;
		} else {
			var org_data = data;
			for (k in data) if (data.hasOwnProperty(k)) d_count++;
		}
		if ((d_type == "object" && d_count > 0) || d_type == "string") {
			return this.each(function (x) {
				if (!opts.asHtmlID) {
					x = x + "" + Math.floor(Math.random() * 100); //this ensures there will be unique IDs on the page if autoSuggest() is called multiple times
					var x_id = "as-input-" + x;
				} else {
					x = opts.asHtmlID;
					var x_id = x;
				}
				opts.start.call(this);
				var input = $(this);
				input.attr("autocomplete", "off").addClass("as-input").attr("id", x_id).val(opts.startText);
				var input_focus = false;

				// Setup basic elements and render them to the DOM
				input.wrap('<ul class="as-selections" id="as-selections-' + x + '"></ul>').wrap('<li class="as-original" id="as-original-' + x + '"></li>');
				var selections_holder = $("#as-selections-" + x);
				var org_li = $("#as-original-" + x);
				var results_holder = $('<div class="as-results" id="as-results-' + x + '"></div>').hide();
				var results_ul = $('<ul class="as-list"></ul>');
				var values_input = $('<input type="hidden" class="as-values" name="as_values_' + x + '" id="as-values-' + x + '" />');
				var prefill_value = "";
				if (typeof opts.preFill == "string") {
					var vals = opts.preFill.split(",");
					for (var i = 0; i < vals.length; i++) {
						var v_data = {};
						v_data[opts.selectedValuesProp] = vals[i];
						if (vals[i] != "") {
							add_selected_item(v_data, "000" + i);
						}
					}
					prefill_value = opts.preFill;
				} else {
					prefill_value = "";
					var prefill_count = 0;
					for (k in opts.preFill) if (opts.preFill.hasOwnProperty(k)) prefill_count++;
					if (prefill_count > 0) {
						for (var i = 0; i < prefill_count; i++) {
							var new_v = opts.preFill[i][opts.selectedValuesProp];
							if (new_v == undefined) { new_v = ""; }
							prefill_value = prefill_value + new_v + ",";
							if (new_v != "") {
								add_selected_item(opts.preFill[i], "000" + i);
							}
						}
					}
				}
				if (prefill_value != "") {
					input.val("");
					var lastChar = prefill_value.substring(prefill_value.length - 1);
					if (lastChar != ",") { prefill_value = prefill_value + ","; }
					values_input.val("," + prefill_value);
					$("li.as-selection-item", selections_holder).addClass("blur").removeClass("selected");
				}
				input.after(values_input);
				selections_holder.click(function () {
					input_focus = true;
					input.focus();
				}).mousedown(function () { input_focus = false; }).after(results_holder);

				var timeout = null;
				var prev = "";
				var totalSelections = 0;
				var tab_press = false;

				// Handle input field events
				input.focus(function () {
					if ($(this).val() == opts.startText && values_input.val() == "") {
						$(this).val("");
					} else if (input_focus) {
						$("li.as-selection-item", selections_holder).removeClass("blur");
						if ($(this).val() != "") {
							results_ul.css("width", selections_holder.outerWidth());
							results_holder.show();
						}
					}
					input_focus = true;
					return true;
				}).blur(function () {
					if ($(this).val() == "" && values_input.val() == "" && prefill_value == "") {
						$(this).val(opts.startText);
					} else if (input_focus) {
						$("li.as-selection-item", selections_holder).addClass("blur").removeClass("selected");
						results_holder.hide();
					}
				}).keydown(function (e) {
					// track last key pressed
					lastKeyPressCode = e.keyCode;
					first_focus = false;
					switch (e.keyCode) {
						case 38: // up
							e.preventDefault();
							moveSelection("up");
							break;
						case 40: // down
							e.preventDefault();
							moveSelection("down");
							break;
						case 8:  // delete
							if (input.val() == "") {
								var last = values_input.val().split(",");
								last = last[last.length - 2];
								selections_holder.children().not(org_li.prev()).removeClass("selected");
								if (org_li.prev().hasClass("selected")) {
									values_input.val(values_input.val().replace("," + last + ",", ","));
									opts.selectionRemoved.call(this, org_li.prev());
								} else {
									opts.selectionClick.call(this, org_li.prev());
									org_li.prev().addClass("selected");
								}
							}
							if (input.val().length == 1) {
								results_holder.hide();
								prev = "";
							}
							if ($(":visible", results_holder).length > 0) {
								if (timeout) { clearTimeout(timeout); }
								timeout = setTimeout(function () { keyChange(); }, opts.keyDelay);
							}
							break;
						case 9: case 188: case 13:  // tab or comma or enter [GSP] Added case 13 because we want enter behavior same as tab & comma
							var active = $("li.active:first", results_holder);
							if (active.length > 0) {
								// An item in the drop down is selected. Use that.
								tab_press = false;
								active.click().removeClass("active"); //[GSP] Added removeClass("active") so that subsequent 'enter' presses can submit data when used in Jeditable
								results_holder.hide();
								if (opts.neverSubmit || active.length > 0) {
									e.preventDefault();
								}
							} else {
								// If text has been entered, use that.
								tab_press = true;
								var i_input = input.val().replace(/(,)/g, "");
								if (i_input != "" && values_input.val().search("," + i_input + ",") < 0 && i_input.length >= opts.minChars) {
									e.preventDefault();
									var n_data = {};
									n_data[opts.selectedItemProp] = i_input;
									n_data[opts.selectedValuesProp] = i_input;
									var lis = $("li", selections_holder).length;
									add_selected_item(n_data, "00" + (lis + 1));
									input.val("");
								}
							}
							break;
						case 27: // [GSP] Added case for escape to clear input
							results_holder.hide();
							input.val("");
							break;
						default:
							if (opts.showResultList) {
								if (opts.selectionLimit && $("li.as-selection-item", selections_holder).length >= opts.selectionLimit) {
									results_ul.html('<li class="as-message">' + opts.limitText + '</li>');
									results_holder.show();
								} else {
									if (timeout) { clearTimeout(timeout); }
									timeout = setTimeout(function () { keyChange(); }, opts.keyDelay);
								}
							}
							break;
					}
				});

				function keyChange() {
					// ignore if the following keys are pressed: [del] [shift] [capslock]
					if (lastKeyPressCode == 46 || (lastKeyPressCode > 8 && lastKeyPressCode < 32)) { return results_holder.hide(); }
					var string = input.val().replace(/[\\]+|[\/]+/g, "");
					if (string == prev) return;
					prev = string;
					if (string.length >= opts.minChars) {
						selections_holder.addClass("loading");
						if (d_type == "string") {
							var limit = "";
							if (opts.retrieveLimit) {
								limit = "&limit=" + encodeURIComponent(opts.retrieveLimit);
							}
							if (opts.beforeRetrieve) {
								string = opts.beforeRetrieve.call(this, string);
							}
							$.getJSON(req_string + "?" + opts.queryParam + "=" + encodeURIComponent(string) + limit + opts.extraParams, function (data) {
								d_count = 0;
								var new_data = opts.retrieveComplete.call(this, data);
								for (k in new_data) if (new_data.hasOwnProperty(k)) d_count++;
								processData(new_data, string);
							});
						} else {
							if (opts.beforeRetrieve) {
								string = opts.beforeRetrieve.call(this, string);
							}
							processData(org_data, string);
						}
					} else {
						selections_holder.removeClass("loading");
						results_holder.hide();
					}
				}
				var num_count = 0;
				function processData(data, query) {
					if (!opts.matchCase) { query = query.toLowerCase(); }
					var matchCount = 0;
					results_holder.html(results_ul.html("")).hide();
					for (var i = 0; i < d_count; i++) {
						var num = i;
						num_count++;
						var forward = false;
						if (opts.searchObjProps == "value") {
							var str = data[num].value;
						} else {
							var str = "";
							var names = opts.searchObjProps.split(",");
							for (var y = 0; y < names.length; y++) {
								var name = $.trim(names[y]);
								str = str + data[num][name] + " ";
							}
						}
						if (str) {
							if (!opts.matchCase) { str = str.toLowerCase(); }
							if (str.search(query) != -1 && values_input.val().search("," + data[num][opts.selectedValuesProp] + ",") == -1) {
								forward = true;
							}
						}
						if (forward) {
							var formatted = $('<li class="as-result-item" id="as-result-item-' + num + '"></li>').click(function () {
								var raw_data = $(this).data("data");
								var number = raw_data.num;
								if ($("#as-selection-" + number, selections_holder).length <= 0 && !tab_press) {
									var data = raw_data.attributes;
									input.val("").focus();
									prev = "";
									add_selected_item(data, number);
									opts.resultClick.call(this, raw_data);
									results_holder.hide();
								}
								tab_press = false;
							}).mousedown(function () { input_focus = false; }).mouseover(function () {
								$("li", results_ul).removeClass("active");
								$(this).addClass("active");
							}).data("data", { attributes: data[num], num: num_count });
							var this_data = $.extend({}, data[num]);
							if (!opts.matchCase) {
								var regx = new RegExp("(?![^&;]+;)(?!<[^<>]*)(" + query + ")(?![^<>]*>)(?![^&;]+;)", "gi");
							} else {
								var regx = new RegExp("(?![^&;]+;)(?!<[^<>]*)(" + query + ")(?![^<>]*>)(?![^&;]+;)", "g");
							}

							if (opts.resultsHighlight) {
								this_data[opts.selectedItemProp] = this_data[opts.selectedItemProp].replace(regx, "<em>$1</em>");
							}
							if (!opts.formatList) {
								formatted = formatted.html(this_data[opts.selectedItemProp]);
							} else {
								formatted = opts.formatList.call(this, this_data, formatted);
							}
							results_ul.append(formatted);
							delete this_data;
							matchCount++;
							if (opts.retrieveLimit && opts.retrieveLimit == matchCount) { break; }
						}
					}
					selections_holder.removeClass("loading");
					if (matchCount <= 0) {
						results_ul.html('<li class="as-message">' + opts.emptyText + '</li>');
					}
					results_ul.css("width", selections_holder.outerWidth());
					results_holder.show();
					opts.resultsComplete.call(this);
				}

				function add_selected_item(data, num) {
					values_input.val(values_input.val() + data[opts.selectedValuesProp] + ",");
					var item = $('<li class="as-selection-item" id="as-selection-' + num + '"></li>').click(function () {
						opts.selectionClick.call(this, $(this));
						selections_holder.children().removeClass("selected");
						$(this).addClass("selected");
					}).mousedown(function () { input_focus = false; });
					var close = $('<a class="as-close">&times;</a>').click(function () {
						values_input.val(values_input.val().replace("," + data[opts.selectedValuesProp] + ",", ","));
						opts.selectionRemoved.call(this, item);
						input_focus = true;
						input.focus();
						return false;
					});
					org_li.before(item.html(data[opts.selectedItemProp]).prepend(close));
					opts.selectionAdded.call(this, org_li.prev());
				}

				function moveSelection(direction) {
					if ($(":visible", results_holder).length > 0) {
						var lis = $("li", results_holder);
						if (direction == "down") {
							var start = lis.eq(0);
						} else {
							var start = lis.filter(":last");
						}
						var active = $("li.active:first", results_holder);
						if (active.length > 0) {
							if (direction == "down") {
								start = active.next();
							} else {
								start = active.prev();
							}
						}
						lis.removeClass("active");
						start.addClass("active");
					}
				}

			});
		}
	}
})(jQuery);

//#endregion End autoSuggest

//#region menubar 2013-03-11 https://github.com/rdogmartin/jquery-ui/blob/menubar/ui/jquery.ui.menubar.js
// This is a branch from https://github.com/jquery/jquery-ui/blob/menubar/ui/jquery.ui.menubar.js with these changes:

// * Replaced show() with slideDown(200) in _open function
// * Added open delay to prevent inadvertently opening menu when mouse is quickly passing over menu button

/*
 * jQuery UI Menubar @VERSION
 *
 * Copyright 2011, AUTHORS.txt (http://jqueryui.com/about)
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 *
 * http://docs.jquery.com/UI/Menubar
 *
 * Depends:
 *	jquery.ui.core.js
 *	jquery.ui.widget.js
 *	jquery.ui.position.js
 *	jquery.ui.menu.js
 */
(function ($) {

	// TODO when mixing clicking menus and keyboard navigation, focus handling is broken
	// there has to be just one item that has tabindex
	$.widget("ui.menubar", {
		version: "@VERSION",
		options: {
			autoExpand: false,
			buttons: false,
			items: "li",
			menuElement: "ul",
			menuIcon: false,
			position: {
				my: "left top",
				at: "left bottom"
			}
		},
		_create: function () {
			var that = this;
			this.menuItems = this.element.children(this.options.items);
			this.items = this.menuItems.children("button, a");

			this.menuItems
				.addClass("ui-menubar-item")
				.attr("role", "presentation");
			// let only the first item receive focus
			this.items.slice(1).attr("tabIndex", -1);

			this.element
				.addClass("ui-menubar ui-widget-header ui-helper-clearfix")
				.attr("role", "menubar");
			this._focusable(this.items);
			this._hoverable(this.items);
			this.items.siblings(this.options.menuElement)
				.menu({
					position: {
						within: this.options.position.within
					},
					select: function (event, ui) {
						ui.item.parents("ul.ui-menu:last").hide();
						that._close();
						// TODO what is this targetting? there's probably a better way to access it
						$(event.target).prev().focus();
						that._trigger("select", event, ui);
					},
					menus: that.options.menuElement
				})
				.hide()
				.attr({
					"aria-hidden": "true",
					"aria-expanded": "false"
				})
				// TODO use _on
				.bind("keydown.menubar", function (event) {
					var menu = $(this);
					if (menu.is(":hidden")) {
						return;
					}
					switch (event.keyCode) {
						case $.ui.keyCode.LEFT:
							that.previous(event);
							event.preventDefault();
							break;
						case $.ui.keyCode.RIGHT:
							that.next(event);
							event.preventDefault();
							break;
					}
				});
			this.items.each(function () {
				var input = $(this),
					// TODO menu var is only used on two places, doesn't quite justify the .each
					menu = input.next(that.options.menuElement);

				// might be a non-menu button
				if (menu.length) {
					// TODO use _on
					input.bind("click.menubar focus.menubar mouseenter.menubar", function (event) {
						// ignore triggered focus event
						if (event.type === "focus" && !event.originalEvent) {
							return;
						}
						event.preventDefault();
						// TODO can we simplify or extractthis check? especially the last two expressions
						// there's a similar active[0] == menu[0] check in _open
						if (event.type === "click" && menu.is(":visible") && that.active && that.active[0] === menu[0]) {
							that._close();
							return;
						}
						if ((that.open && event.type === "mouseenter") || event.type === "click" || that.options.autoExpand) {
							if (that.options.autoExpand) {
								clearTimeout(that.closeTimer);
							}

							if (that.options.autoExpand) {
								// Expand after a slight delay, which we'll cancel if the mouse leaves the element
								// before the delay is up. This prevents inadvertently opening the menu when the mouse
								// is just passing through the area.
								that.openTimer = window.setTimeout(function () {
									that._open(event, menu);
								}, 200);
							} else {
								that._open(event, menu);
							}
						}
					})
					// TODO use _on
					.bind("keydown", function (event) {
						switch (event.keyCode) {
							case $.ui.keyCode.SPACE:
							case $.ui.keyCode.UP:
							case $.ui.keyCode.DOWN:
								that._open(event, $(this).next());
								event.preventDefault();
								break;
							case $.ui.keyCode.LEFT:
								that.previous(event);
								event.preventDefault();
								break;
							case $.ui.keyCode.RIGHT:
								that.next(event);
								event.preventDefault();
								break;
						}
					})
					.attr("aria-haspopup", "true");

					// TODO review if these options (menuIcon and buttons) are a good choice, maybe they can be merged
					if (that.options.menuIcon) {
						input.addClass("ui-state-default").append("<span class='ui-button-icon-secondary ui-icon ui-icon-triangle-1-s'></span>");
						input.removeClass("ui-button-text-only").addClass("ui-button-text-icon-secondary");
					}
				} else {
					// TODO use _on
					input.bind("click.menubar mouseenter.menubar", function (event) {
						if ((that.open && event.type === "mouseenter") || event.type === "click") {
							that._close();
						}
					});
				}

				input
					.addClass("ui-button ui-widget ui-button-text-only ui-menubar-link")
					.attr("role", "menuitem")
					.wrapInner("<span class='ui-button-text'></span>");

				if (that.options.buttons) {
					input.removeClass("ui-menubar-link").addClass("ui-state-default");
				}
			});
			that._on({
				keydown: function (event) {
					if (event.keyCode === $.ui.keyCode.ESCAPE && that.active && that.active.menu("collapse", event) !== true) {
						var active = that.active;
						that.active.blur();
						that._close(event);
						active.prev().focus();
					}
				},
				focusin: function (event) {
					clearTimeout(that.closeTimer);
				},
				focusout: function (event) {
					clearTimeout(that.openTimer);
					that.closeTimer = setTimeout(function () {
						that._close(event);
					}, 150);
				},
				"mouseleave .ui-menubar-item": function (event) {
					if (that.options.autoExpand) {
						clearTimeout(that.openTimer);
						that.closeTimer = setTimeout(function () {
							that._close(event);
						}, 150);
					}
				},
				"mouseenter .ui-menubar-item": function (event) {
					clearTimeout(that.closeTimer);
				}
			});

			// Keep track of open submenus
			this.openSubmenus = 0;
		},

		_destroy: function () {
			this.menuItems
				.removeClass("ui-menubar-item")
				.removeAttr("role");

			this.element
				.removeClass("ui-menubar ui-widget-header ui-helper-clearfix")
				.removeAttr("role")
				.unbind(".menubar");

			this.items
				.unbind(".menubar")
				.removeClass("ui-button ui-widget ui-button-text-only ui-menubar-link ui-state-default")
				.removeAttr("role")
				.removeAttr("aria-haspopup")
				// TODO unwrap?
				.children("span.ui-button-text").each(function (i, e) {
					var item = $(this);
					item.parent().html(item.html());
				})
				.end()
				.children(".ui-icon").remove();

			this.element.find(":ui-menu")
				.menu("destroy")
				.show()
				.removeAttr("aria-hidden")
				.removeAttr("aria-expanded")
				.removeAttr("tabindex")
				.unbind(".menubar");
		},

		_close: function () {
			if (!this.active || !this.active.length) {
				return;
			}
			this.active
				.menu("collapseAll")
				.hide()
				.attr({
					"aria-hidden": "true",
					"aria-expanded": "false"
				});
			this.active
				.prev()
				.removeClass("ui-state-active")
				.removeAttr("tabIndex");
			this.active = null;
			this.open = false;
			this.openSubmenus = 0;
		},

		_open: function (event, menu) {
			// on a single-button menubar, ignore reopening the same menu
			if (this.active && this.active[0] === menu[0]) {
				return;
			}
			// TODO refactor, almost the same as _close above, but don't remove tabIndex
			if (this.active) {
				this.active
					.menu("collapseAll")
					.hide()
					.attr({
						"aria-hidden": "true",
						"aria-expanded": "false"
					});
				this.active
					.prev()
					.removeClass("ui-state-active");
			}
			// set tabIndex -1 to have the button skipped on shift-tab when menu is open (it gets focus)
			var button = menu.prev().addClass("ui-state-active").attr("tabIndex", -1);
			this.active = menu
				.slideDown(200) // Replace show() with slideDown()
				.position($.extend({
					of: button
				}, this.options.position))
				.removeAttr("aria-hidden")
				.attr("aria-expanded", "true")
				.menu("focus", event, menu.children(".ui-menu-item").first())
				// TODO need a comment here why both events are triggered
				.focus()
				.focusin();
			this.open = true;
		},

		next: function (event) {
			if (this.open && this.active.data("menu").active.has(".ui-menu").length) {
				// Track number of open submenus and prevent moving to next menubar item
				this.openSubmenus++;
				return;
			}
			this.openSubmenus = 0;
			this._move("next", "first", event);
		},

		previous: function (event) {
			if (this.open && this.openSubmenus) {
				// Track number of open submenus and prevent moving to previous menubar item
				this.openSubmenus--;
				return;
			}
			this.openSubmenus = 0;
			this._move("prev", "last", event);
		},

		_move: function (direction, filter, event) {
			var next,
				wrapItem;
			if (this.open) {
				next = this.active.closest(".ui-menubar-item")[direction + "All"](this.options.items).first().children(".ui-menu").eq(0);
				wrapItem = this.menuItems[filter]().children(".ui-menu").eq(0);
			} else {
				if (event) {
					next = $(event.target).closest(".ui-menubar-item")[direction + "All"](this.options.items).children(".ui-menubar-link").eq(0);
					wrapItem = this.menuItems[filter]().children(".ui-menubar-link").eq(0);
				} else {
					next = wrapItem = this.menuItems.children("a").eq(0);
				}
			}

			if (next.length) {
				if (this.open) {
					this._open(event, next);
				} else {
					next.removeAttr("tabIndex")[0].focus();
				}
			} else {
				if (this.open) {
					this._open(event, wrapItem);
				} else {
					wrapItem.removeAttr("tabIndex")[0].focus();
				}
			}
		}
	});

}(jQuery));

//#endregion End menubar

//#region RateIt
/*
		RateIt
		version 1.0.9
		10/31/2012
		http://rateit.codeplex.com
		Twitter: @gjunge

*/
(function ($) {
	$.fn.rateit = function (p1, p2) {
		//quick way out.
		var options = {}; var mode = 'init';
		var capitaliseFirstLetter = function (string) {
			return string.charAt(0).toUpperCase() + string.substr(1);
		};

		if (this.length == 0) return this;


		var tp1 = $.type(p1);
		if (tp1 == 'object' || p1 === undefined || p1 == null) {
			options = $.extend({}, $.fn.rateit.defaults, p1); //wants to init new rateit plugin(s).
		}
		else if (tp1 == 'string' && p2 === undefined) {
			return this.data('rateit' + capitaliseFirstLetter(p1)); //wants to get a value.
		}
		else if (tp1 == 'string') {
			mode = 'setvalue'
		}

		return this.each(function () {
			var item = $(this);

			//shorten all the item.data('rateit-XXX'), will save space in closure compiler, will be like item.data('XXX') will become x('XXX')
			var itemdata = function (key, value) {
				arguments[0] = 'rateit' + capitaliseFirstLetter(key);
				return item.data.apply(item, arguments); ////Fix for WI: 523
			};

			//add the rate it class.
			if (!item.hasClass('rateit')) item.addClass('rateit');

			var ltr = item.css('direction') != 'rtl';

			// set value mode
			if (mode == 'setvalue') {
				if (!itemdata('init')) throw 'Can\'t set value before init';


				//if readonly now and it wasn't readonly, remove the eventhandlers.
				if (p1 == 'readonly' && !itemdata('readonly')) {
					item.find('.rateit-range').unbind();
					itemdata('wired', false);
				}
				if (p1 == 'value' && p2 == null) p2 = itemdata('min'); //when we receive a null value, reset the score to its min value.

				if (itemdata('backingfld')) {
					//if we have a backing field, check which fields we should update. 
					//In case of input[type=range], although we did read its attributes even in browsers that don't support it (using fld.attr())
					//we only update it in browser that support it (&& fld[0].min only works in supporting browsers), not only does it save us from checking if it is range input type, it also is unnecessary.
					var fld = $(itemdata('backingfld'));
					if (p1 == 'value') fld.val(p2);
					if (p1 == 'min' && fld[0].min) fld[0].min = p2;
					if (p1 == 'max' && fld[0].max) fld[0].max = p2;
					if (p1 == 'step' && fld[0].step) fld[0].step = p2;
				}

				itemdata(p1, p2);
			}

			//init rateit plugin
			if (!itemdata('init')) {

				//get our values, either from the data-* html5 attribute or from the options.
				itemdata('min', itemdata('min') || options.min);
				itemdata('max', itemdata('max') || options.max);
				itemdata('step', itemdata('step') || options.step);
				itemdata('readonly', itemdata('readonly') !== undefined ? itemdata('readonly') : options.readonly);
				itemdata('resetable', itemdata('resetable') !== undefined ? itemdata('resetable') : options.resetable);
				itemdata('backingfld', itemdata('backingfld') || options.backingfld);
				itemdata('starwidth', itemdata('starwidth') || options.starwidth);
				itemdata('starheight', itemdata('starheight') || options.starheight);
				itemdata('value', itemdata('value') || options.value || options.min);
				itemdata('ispreset', itemdata('ispreset') !== undefined ? itemdata('ispreset') : options.ispreset);
				//are we LTR or RTL?

				if (itemdata('backingfld')) {
					//if we have a backing field, hide it, and get its value, and override defaults if range.
					var fld = $(itemdata('backingfld'));
					itemdata('value', fld.hide().val());

					if (fld.attr('disabled') || fld.attr('readonly'))
						itemdata('readonly', true); //http://rateit.codeplex.com/discussions/362055 , if a backing field is disabled or readonly at instantiation, make rateit readonly.


					if (fld[0].nodeName == 'INPUT') {
						if (fld[0].type == 'range' || fld[0].type == 'text') { //in browsers not support the range type, it defaults to text

							itemdata('min', parseInt(fld.attr('min')) || itemdata('min')); //if we would have done fld[0].min it wouldn't have worked in browsers not supporting the range type.
							itemdata('max', parseInt(fld.attr('max')) || itemdata('max'));
							itemdata('step', parseInt(fld.attr('step')) || itemdata('step'));
						}
					}
					if (fld[0].nodeName == 'SELECT' && fld[0].options.length > 1) {
						itemdata('min', Number(fld[0].options[0].value));
						itemdata('max', Number(fld[0].options[fld[0].length - 1].value));
						itemdata('step', Number(fld[0].options[1].value) - Number(fld[0].options[0].value));
					}
				}

				//Create the necessary tags.
				item.append('<div class="rateit-reset"></div><div class="rateit-range"><div class="rateit-selected" style="height:' + itemdata('starheight') + 'px"></div><div class="rateit-hover" style="height:' + itemdata('starheight') + 'px"></div></div>');

				//if we are in RTL mode, we have to change the float of the "reset button"
				if (!ltr) {
					item.find('.rateit-reset').css('float', 'right');
					item.find('.rateit-selected').addClass('rateit-selected-rtl');
					item.find('.rateit-hover').addClass('rateit-hover-rtl');
				}

				itemdata('init', true);
			}


			//set the range element to fit all the stars.
			var range = item.find('.rateit-range');
			range.width(itemdata('starwidth') * (itemdata('max') - itemdata('min'))).height(itemdata('starheight'));

			//add/remove the preset class
			var presetclass = 'rateit-preset' + ((ltr) ? '' : '-rtl');
			if (itemdata('ispreset'))
				item.find('.rateit-selected').addClass(presetclass);
			else
				item.find('.rateit-selected').removeClass(presetclass);

			//set the value if we have it.
			if (itemdata('value') != null) {
				var score = (itemdata('value') - itemdata('min')) * itemdata('starwidth');
				item.find('.rateit-selected').width(score);
			}

			var resetbtn = item.find('.rateit-reset');
			if (resetbtn.data('wired') !== true) {
				resetbtn.click(function () {
					itemdata('value', itemdata('min'));
					range.find('.rateit-hover').hide().width(0);
					range.find('.rateit-selected').width(0).show();
					if (itemdata('backingfld')) $(itemdata('backingfld')).val(itemdata('min'));
					item.trigger('reset');
				}).data('wired', true);

			}


			var calcRawScore = function (element, event) {
				var pageX = (event.changedTouches) ? event.changedTouches[0].pageX : event.pageX;

				var offsetx = pageX - $(element).offset().left;
				if (!ltr) offsetx = range.width() - offsetx;
				if (offsetx > range.width()) offsetx = range.width();
				if (offsetx < 0) offsetx = 0;

				return score = Math.ceil(offsetx / itemdata('starwidth') * (1 / itemdata('step')));
			};


			//

			if (!itemdata('readonly')) {
				//if we are not read only, add all the events

				//if we have a reset button, set the event handler.
				if (!itemdata('resetable'))
					resetbtn.hide();

				//when the mouse goes over the range div, we set the "hover" stars.
				if (!itemdata('wired')) {
					range.bind('touchmove touchend', touchHandler); //bind touch events
					range.mousemove(function (e) {
						var score = calcRawScore(this, e);
						var w = score * itemdata('starwidth') * itemdata('step');
						var h = range.find('.rateit-hover');
						if (h.data('width') != w) {
							range.find('.rateit-selected').hide();
							h.width(w).show().data('width', w);
							var data = [(score * itemdata('step')) + itemdata('min')];
							item.trigger('hover', data).trigger('over', data);
						}
					});
					//when the mouse leaves the range, we have to hide the hover stars, and show the current value.
					range.mouseleave(function (e) {
						range.find('.rateit-hover').hide().width(0).data('width', '');
						item.trigger('hover', [null]).trigger('over', [null]);
						range.find('.rateit-selected').show();
					});
					//when we click on the range, we have to set the value, hide the hover.
					range.mouseup(function (e) {
						var score = calcRawScore(this, e);

						var newvalue = (score * itemdata('step')) + itemdata('min');
						itemdata('value', newvalue);
						if (itemdata('backingfld')) {
							$(itemdata('backingfld')).val(newvalue);
						}
						if (itemdata('ispreset')) { //if it was a preset value, unset that.
							range.find('.rateit-selected').removeClass(presetclass);
							itemdata('ispreset', false);
						}
						range.find('.rateit-hover').hide();
						range.find('.rateit-selected').width(score * itemdata('starwidth') * itemdata('step')).show();
						item.trigger('hover', [null]).trigger('over', [null]).trigger('rated', [newvalue]);
					});

					itemdata('wired', true);
				}
				if (itemdata('resetable')) {
					resetbtn.show();
				}
			}
			else {
				resetbtn.hide();
			}
		});
	};

	//touch converter http://ross.posterous.com/2008/08/19/iphone-touch-events-in-javascript/
	function touchHandler(event) {

		var touches = event.originalEvent.changedTouches,
						first = touches[0],
						type = "";
		switch (event.type) {
			case "touchmove": type = "mousemove"; break;
			case "touchend": type = "mouseup"; break;
			default: return;
		}

		var simulatedEvent = document.createEvent("MouseEvent");
		simulatedEvent.initMouseEvent(type, true, true, window, 1,
													first.screenX, first.screenY,
													first.clientX, first.clientY, false,
													false, false, false, 0/*left*/, null);

		first.target.dispatchEvent(simulatedEvent);
		event.preventDefault();
	};

	//some default values.
	$.fn.rateit.defaults = { min: 0, max: 5, step: 0.5, starwidth: 16, starheight: 16, readonly: false, resetable: true, ispreset: false };

	//invoke it on all div.rateit elements. This could be removed if not wanted.
	//$(function () { $('div.rateit').rateit(); });

})(jQuery);

//#endregion End RateIt

//#region supersized

/*
	supersized.3.2.7.js
	Supersized - Fullscreen Slideshow jQuery Plugin
	Version : 3.2.7
	Site	: www.buildinternet.com/project/supersized
	
	Author	: Sam Dunn
	Company : One Mighty Roar (www.onemightyroar.com)
	License : MIT License / GPL License
	
*/

(function ($) {

	$.supersized = function (options) {

		/* Variables
	----------------------------*/
		var base = this;

		base.init = function () {
			// Combine options and vars
			$.supersized.vars = $.extend($.supersized.vars, $.supersized.themeVars);
			$.supersized.vars.options = $.extend({}, $.supersized.defaultOptions, $.supersized.themeOptions, options);
			base.options = $.supersized.vars.options;

			base._build();
		};


		/* Build Elements
----------------------------*/
		base._build = function () {
			// Add in slide markers
			var thisSlide = 0,
				slideSet = '',
		markers = '',
		markerContent,
		thumbMarkers = '',
		thumbImage;


			// Hide current page contents and add Supersized Elements
			$('body').children(':visible').hide().addClass('supersized_hidden');
			$('body').append($($.supersized.vars.options.html_template), '<div id="supersized-loader"></div><ul id="supersized"></ul>');

			var el = '#supersized';
			// Access to jQuery and DOM versions of element
			base.$el = $(el);
			base.el = el;
			vars = $.supersized.vars;
			// Add a reverse reference to the DOM object
			base.$el.data("supersized", base);
			api = base.$el.data('supersized');


			while (thisSlide <= base.options.slides.length - 1) {
				//Determine slide link content
				switch (base.options.slide_links) {
					case 'num':
						markerContent = thisSlide;
						break;
					case 'name':
						markerContent = base.options.slides[thisSlide].title;
						break;
					case 'blank':
						markerContent = '';
						break;
				}

				slideSet = slideSet + '<li class="slide-' + thisSlide + '"></li>';

				if (thisSlide == base.options.start_slide - 1) {
					// Slide links
					if (base.options.slide_links) markers = markers + '<li class="slide-link-' + thisSlide + ' current-slide"><a>' + markerContent + '</a></li>';
					// Slide Thumbnail Links
					if (base.options.thumb_links) {
						base.options.slides[thisSlide].thumb ? thumbImage = base.options.slides[thisSlide].thumb : thumbImage = base.options.slides[thisSlide].image;
						thumbMarkers = thumbMarkers + '<li class="thumb' + thisSlide + ' current-thumb"><img src="' + thumbImage + '"/></li>';
					};
				} else {
					// Slide links
					if (base.options.slide_links) markers = markers + '<li class="slide-link-' + thisSlide + '" ><a>' + markerContent + '</a></li>';
					// Slide Thumbnail Links
					if (base.options.thumb_links) {
						base.options.slides[thisSlide].thumb ? thumbImage = base.options.slides[thisSlide].thumb : thumbImage = base.options.slides[thisSlide].image;
						thumbMarkers = thumbMarkers + '<li class="thumb' + thisSlide + '"><img src="' + thumbImage + '"/></li>';
					};
				}
				thisSlide++;
			}

			if (base.options.slide_links) $(vars.slide_list).html(markers);
			if (base.options.thumb_links && vars.thumb_tray.length) {
				$(vars.thumb_tray).append('<ul id="' + vars.thumb_list.replace('#', '') + '">' + thumbMarkers + '</ul>');
			}

			$(base.el).append(slideSet);

			// Add in thumbnails
			if (base.options.thumbnail_navigation) {
				// Load previous thumbnail
				vars.current_slide - 1 < 0 ? prevThumb = base.options.slides.length - 1 : prevThumb = vars.current_slide - 1;
				$(vars.prev_thumb).show().html($("<img/>").attr("src", base.options.slides[prevThumb].image));

				// Load next thumbnail
				vars.current_slide == base.options.slides.length - 1 ? nextThumb = 0 : nextThumb = vars.current_slide + 1;
				$(vars.next_thumb).show().html($("<img/>").attr("src", base.options.slides[nextThumb].image));
			}

			base._start(); // Get things started
		};


		/* Initialize
----------------------------*/
		base._start = function () {

			// Determine if starting slide random
			if (base.options.start_slide) {
				vars.current_slide = base.options.start_slide - 1;
			} else {
				vars.current_slide = Math.floor(Math.random() * base.options.slides.length);	// Generate random slide number
			}

			// If links should open in new window
			var linkTarget = base.options.new_window ? ' target="_blank"' : '';

			// Set slideshow quality (Supported only in FF and IE, no Webkit)
			if (base.options.performance == 3) {
				base.$el.addClass('speed'); 		// Faster transitions
			} else if ((base.options.performance == 1) || (base.options.performance == 2)) {
				base.$el.addClass('quality');	// Higher image quality
			}

			// Shuffle slide order if needed		
			if (base.options.random) {
				arr = base.options.slides;
				for (var j, x, i = arr.length; i; j = parseInt(Math.random() * i), x = arr[--i], arr[i] = arr[j], arr[j] = x);	// Fisher-Yates shuffle algorithm (jsfromhell.com/array/shuffle)
				base.options.slides = arr;
			}

			/*-----Load initial set of images-----*/

			if (base.options.slides.length > 1) {
				if (base.options.slides.length > 2) {
					// Set previous image
					vars.current_slide - 1 < 0 ? loadPrev = base.options.slides.length - 1 : loadPrev = vars.current_slide - 1;	// If slide is 1, load last slide as previous
					var imageLink = (base.options.slides[loadPrev].url) ? "href='" + base.options.slides[loadPrev].url + "'" : "";

					var imgPrev = $('<img src="' + base.options.slides[loadPrev].image + '"/>');
					var slidePrev = base.el + ' li:eq(' + loadPrev + ')';
					imgPrev.appendTo(slidePrev).wrap('<a ' + imageLink + linkTarget + '></a>').parent().parent().addClass('image-loading prevslide');

					imgPrev.load(function () {
						$(this).data('origWidth', $(this).width()).data('origHeight', $(this).height());
						base.resizeNow();	// Resize background image
					});	// End Load
				}
			} else {
				// Slideshow turned off if there is only one slide
				//base.options.slideshow = 0; //[RDM] Commented out because this disables buttons when there is only one slide
			}

			// Set current image
			imageLink = (api.getField('url')) ? "href='" + api.getField('url') + "'" : "";
			var img = $('<img src="' + api.getField('image') + '"/>');

			var slideCurrent = base.el + ' li:eq(' + vars.current_slide + ')';
			img.appendTo(slideCurrent).wrap('<a ' + imageLink + linkTarget + '></a>').parent().parent().addClass('image-loading activeslide').css('visibility', 'visible');

			img.load(function () {
				base._origDim($(this));
				base.resizeNow();	// Resize background image
				base.launch();
				if (typeof theme != 'undefined' && typeof theme._init == "function") theme._init();	// Load Theme
			});

			if (base.options.slides.length > 1) {
				// Set next image
				vars.current_slide == base.options.slides.length - 1 ? loadNext = 0 : loadNext = vars.current_slide + 1;	// If slide is last, load first slide as next
				imageLink = (base.options.slides[loadNext].url) ? "href='" + base.options.slides[loadNext].url + "'" : "";

				var imgNext = $('<img src="' + base.options.slides[loadNext].image + '"/>');
				var slideNext = base.el + ' li:eq(' + loadNext + ')';
				imgNext.appendTo(slideNext).wrap('<a ' + imageLink + linkTarget + '></a>').parent().parent().addClass('image-loading');

				imgNext.load(function () {
					$(this).data('origWidth', $(this).width()).data('origHeight', $(this).height());
					base.resizeNow();	// Resize background image
				});	// End Load
			}
			/*-----End load initial images-----*/

			//  Hide elements to be faded in
			base.$el.css('visibility', 'hidden');
			$('.load-item').hide();

		};


		/* Launch Supersized
		----------------------------*/
		base.launch = function () {

			//base.$el.css('visibility', 'visible');
			$('#supersized-loader').remove();		//Hide loading animation

			// Call theme function for before slide transition
			if (typeof theme != 'undefined' && typeof theme.beforeAnimation == "function") theme.beforeAnimation('next');
			$('.load-item').show();

			// Keyboard Navigation
			if (base.options.keyboard_nav) {
				$(document.documentElement).on('keyup.supersized', function (event) {

					if (vars.in_animation) return false;		// Abort if currently animating
					if ($(document.activeElement).is("input, textarea")) return false; // Abort if active element is an input or a textarea.

					// Left Arrow or Down Arrow
					if ((event.keyCode == 37) || (event.keyCode == 40)) {
						clearInterval(vars.slideshow_interval);	// Stop slideshow, prevent buildup
						base.prevSlide();

						// Right Arrow or Up Arrow
					} else if ((event.keyCode == 39) || (event.keyCode == 38)) {
						clearInterval(vars.slideshow_interval);	// Stop slideshow, prevent buildup
						base.nextSlide();

						// Spacebar	
					} else if (event.keyCode == 32 && !vars.hover_pause) {
						clearInterval(vars.slideshow_interval);	// Stop slideshow, prevent buildup
						base.playToggle();
					}

				});
			}

			// Pause when hover on image
			if (base.options.slideshow && base.options.pause_hover) {
				$(base.el).hover(function () {
					if (vars.in_animation) return false;		// Abort if currently animating
					vars.hover_pause = true;	// Mark slideshow paused from hover
					if (!vars.is_paused) {
						vars.hover_pause = 'resume';	// It needs to resume afterwards
						base.playToggle();
					}
				}, function () {
					if (vars.hover_pause == 'resume') {
						base.playToggle();
						vars.hover_pause = false;
					}
				});
			}

			if (base.options.slide_links) {
				// Slide marker clicked
				$(vars.slide_list + '> li').click(function () {

					index = $(vars.slide_list + '> li').index(this);
					targetSlide = index + 1;

					base.goTo(targetSlide);
					return false;

				});
			}

			// Thumb marker clicked
			if (base.options.thumb_links) {
				$(vars.thumb_list + '> li').click(function () {

					index = $(vars.thumb_list + '> li').index(this);
					targetSlide = index + 1;

					api.goTo(targetSlide);
					return false;

				});
			}

			// Start slideshow if enabled
			if (base.options.slideshow && base.options.slides.length > 1) {

				// Start slideshow if autoplay enabled
				if (base.options.autoplay && base.options.slides.length > 1) {
					vars.slideshow_interval = setInterval(base.nextSlide, base.options.slide_interval);	// Initiate slide interval
				} else {
					vars.is_paused = true;	// Mark as paused
				}

				//Prevent navigation items from being dragged					
				$('.load-item img').bind("contextmenu mousedown", function () {
					return false;
				});

			}

			// Adjust image when browser is resized
			$(window).resize(function () {
				base.resizeNow();
			});

		};


		/* Resize Images
----------------------------*/
		base.resizeNow = function () {

			return base.$el.each(function () {
				//  Resize each image seperately
				$('img', base.el).each(function () {

					thisSlide = $(this);
					var ratio = (thisSlide.data('origHeight') / thisSlide.data('origWidth')).toFixed(2);	// Define image ratio

					// Gather browser size
					var browserwidth = base.$el.width(),
						browserheight = base.$el.height(),
						offset;

					/*-----Resize Image-----*/
					if (base.options.fit_always) {	// Fit always is enabled
						if ((browserheight / browserwidth) > ratio) {
							resizeWidth();
						} else {
							resizeHeight();
						}
					} else {	// Normal Resize
						if ((browserheight <= base.options.min_height) && (browserwidth <= base.options.min_width)) {	// If window smaller than minimum width and height

							if ((browserheight / browserwidth) > ratio) {
								base.options.fit_landscape && ratio < 1 ? resizeWidth(true) : resizeHeight(true);	// If landscapes are set to fit
							} else {
								base.options.fit_portrait && ratio >= 1 ? resizeHeight(true) : resizeWidth(true);		// If portraits are set to fit
							}

						} else if (browserwidth <= base.options.min_width) {		// If window only smaller than minimum width

							if ((browserheight / browserwidth) > ratio) {
								base.options.fit_landscape && ratio < 1 ? resizeWidth(true) : resizeHeight();	// If landscapes are set to fit
							} else {
								base.options.fit_portrait && ratio >= 1 ? resizeHeight() : resizeWidth(true);		// If portraits are set to fit
							}

						} else if (browserheight <= base.options.min_height) {	// If window only smaller than minimum height

							if ((browserheight / browserwidth) > ratio) {
								base.options.fit_landscape && ratio < 1 ? resizeWidth() : resizeHeight(true);	// If landscapes are set to fit
							} else {
								base.options.fit_portrait && ratio >= 1 ? resizeHeight(true) : resizeWidth();		// If portraits are set to fit
							}

						} else {	// If larger than minimums

							if ((browserheight / browserwidth) > ratio) {
								base.options.fit_landscape && ratio < 1 ? resizeWidth() : resizeHeight();	// If landscapes are set to fit
							} else {
								base.options.fit_portrait && ratio >= 1 ? resizeHeight() : resizeWidth();		// If portraits are set to fit
							}

						}
					}
					/*-----End Image Resize-----*/


					/*-----Resize Functions-----*/

					function resizeWidth(minimum) {
						if (minimum) {	// If minimum height needs to be considered
							if (thisSlide.width() < browserwidth || thisSlide.width() < base.options.min_width) {
								if (thisSlide.width() * ratio >= base.options.min_height) {
									thisSlide.width(base.options.min_width);
									thisSlide.height(thisSlide.width() * ratio);
								} else {
									resizeHeight();
								}
							}
						} else {
							if (base.options.min_height >= browserheight && !base.options.fit_landscape) {	// If minimum height needs to be considered
								if (browserwidth * ratio >= base.options.min_height || (browserwidth * ratio >= base.options.min_height && ratio <= 1)) {	// If resizing would push below minimum height or image is a landscape
									thisSlide.width(browserwidth);
									thisSlide.height(browserwidth * ratio);
								} else if (ratio > 1) {		// Else the image is portrait
									thisSlide.height(base.options.min_height);
									thisSlide.width(thisSlide.height() / ratio);
								} else if (thisSlide.width() < browserwidth) {
									thisSlide.width(browserwidth);
									thisSlide.height(thisSlide.width() * ratio);
								}
							} else {	// Otherwise, resize as normal
								thisSlide.width(browserwidth);
								thisSlide.height(browserwidth * ratio);
							}
						}
					};

					function resizeHeight(minimum) {
						if (minimum) {	// If minimum height needs to be considered
							if (thisSlide.height() < browserheight) {
								if (thisSlide.height() / ratio >= base.options.min_width) {
									thisSlide.height(base.options.min_height);
									thisSlide.width(thisSlide.height() / ratio);
								} else {
									resizeWidth(true);
								}
							}
						} else {	// Otherwise, resized as normal
							if (base.options.min_width >= browserwidth) {	// If minimum width needs to be considered
								if (browserheight / ratio >= base.options.min_width || ratio > 1) {	// If resizing would push below minimum width or image is a portrait
									thisSlide.height(browserheight);
									thisSlide.width(browserheight / ratio);
								} else if (ratio <= 1) {		// Else the image is landscape
									thisSlide.width(base.options.min_width);
									thisSlide.height(thisSlide.width() * ratio);
								}
							} else {	// Otherwise, resize as normal
								thisSlide.height(browserheight);
								thisSlide.width(browserheight / ratio);
							}
						}
					};

					/*-----End Resize Functions-----*/

					if (thisSlide.parents('li').hasClass('image-loading')) {
						$('.image-loading').removeClass('image-loading');
					}

					// Horizontally Center
					if (base.options.horizontal_center) {
						$(this).css('left', (browserwidth - $(this).width()) / 2);
					}

					// Vertically Center
					if (base.options.vertical_center) {
						$(this).css('top', (browserheight - $(this).height()) / 2);
					}

				});

				// Basic image drag and right click protection
				if (base.options.image_protect) {

					$('img', base.el).bind("contextmenu mousedown", function () {
						return false;
					});

				}

				return false;

			});

		};


		/* Next Slide
----------------------------*/
		base.nextSlide = function () {
			if (base.options.slideshow && !vars.is_paused && base.options.auto_exit && (vars.current_slide == base.options.slides.length - 1)) {
				// We're on the last slide of a running slideshow where auto_exit is enabled, so exit.
				base.destroy();
				return false;
			}

			var old_slide_number = vars.current_slide;
			// Get the slide number of new slide
			if (vars.current_slide < base.options.slides.length - 1) {
				vars.current_slide++;
			} else if (base.options.loop) {
				vars.current_slide = 0;
			}

			if (old_slide_number == vars.current_slide) {
				vars.in_animation = false;
				return false;
			}

			if (vars.in_animation || !api.options.slideshow) return false;		// Abort if currently animating
			else vars.in_animation = true;		// Otherwise set animation marker

			clearInterval(vars.slideshow_interval);	// Stop slideshow

			var slides = base.options.slides,					// Pull in slides array
			liveslide = base.$el.find('.activeslide');		// Find active slide
			$('.prevslide').removeClass('prevslide');
			liveslide.removeClass('activeslide').addClass('prevslide');	// Remove active class & update previous slide


			var nextslide = $(base.el + ' li:eq(' + vars.current_slide + ')'),
				prevslide = base.$el.find('.prevslide');

			// If hybrid mode is on drop quality for transition
			if (base.options.performance == 1) base.$el.removeClass('quality').addClass('speed');


			/*-----Load Image-----*/

			loadSlide = false;

			vars.current_slide == base.options.slides.length - 1 ? loadSlide = 0 : loadSlide = vars.current_slide + 1;	// Determine next slide

			var targetList = base.el + ' li:eq(' + loadSlide + ')';
			if (!$(targetList).html()) {

				// If links should open in new window
				var linkTarget = base.options.new_window ? ' target="_blank"' : '';

				imageLink = (base.options.slides[loadSlide].url) ? "href='" + base.options.slides[loadSlide].url + "'" : "";	// If link exists, build it
				var img = $('<img src="' + base.options.slides[loadSlide].image + '"/>');

				img.appendTo(targetList).wrap('<a ' + imageLink + linkTarget + '></a>').parent().parent().addClass('image-loading').css('visibility', 'hidden');

				img.load(function () {
					base._origDim($(this));
					base.resizeNow();
				});	// End Load
			};

			// Update thumbnails (if enabled)
			if (base.options.thumbnail_navigation == 1) {

				// Load previous thumbnail
				vars.current_slide - 1 < 0 ? prevThumb = base.options.slides.length - 1 : prevThumb = vars.current_slide - 1;
				$(vars.prev_thumb).html($("<img/>").attr("src", base.options.slides[prevThumb].image));

				// Load next thumbnail
				nextThumb = loadSlide;
				$(vars.next_thumb).html($("<img/>").attr("src", base.options.slides[nextThumb].image));

			}



			/*-----End Load Image-----*/


			// Call theme function for before slide transition
			if (typeof theme != 'undefined' && typeof theme.beforeAnimation == "function") theme.beforeAnimation('next');

			//Update slide markers
			if (base.options.slide_links) {
				$('.current-slide').removeClass('current-slide');
				$(vars.slide_list + '> li').eq(vars.current_slide).addClass('current-slide');
			}

			nextslide.css('visibility', 'hidden').addClass('activeslide');	// Update active slide

			switch (base.options.transition) {
				case 0: case 'none':	// No transition
					nextslide.css('visibility', 'visible'); vars.in_animation = false; base.afterAnimation();
					break;
				case 1: case 'fade':	// Fade
					nextslide.css({ opacity: 0, 'visibility': 'visible' }).animate({ opacity: 1, avoidTransforms: false }, base.options.transition_speed, function () { base.afterAnimation(); });
					break;
				case 2: case 'slideTop':	// Slide Top
					nextslide.css({ top: -base.$el.height(), 'visibility': 'visible' }).animate({ top: 0, avoidTransforms: false }, base.options.transition_speed, function () { base.afterAnimation(); });
					break;
				case 3: case 'slideRight':	// Slide Right
					nextslide.css({ left: base.$el.width(), 'visibility': 'visible' }).animate({ left: 0, avoidTransforms: false }, base.options.transition_speed, function () { base.afterAnimation(); });
					break;
				case 4: case 'slideBottom': // Slide Bottom
					nextslide.css({ top: base.$el.height(), 'visibility': 'visible' }).animate({ top: 0, avoidTransforms: false }, base.options.transition_speed, function () { base.afterAnimation(); });
					break;
				case 5: case 'slideLeft':  // Slide Left
					nextslide.css({ left: -base.$el.width(), 'visibility': 'visible' }).animate({ left: 0, avoidTransforms: false }, base.options.transition_speed, function () { base.afterAnimation(); });
					break;
				case 6: case 'carouselRight':	// Carousel Right
					nextslide.css({ left: base.$el.width(), 'visibility': 'visible' }).animate({ left: 0, avoidTransforms: false }, base.options.transition_speed, function () { base.afterAnimation(); });
					liveslide.animate({ left: -base.$el.width(), avoidTransforms: false }, base.options.transition_speed);
					break;
				case 7: case 'carouselLeft':   // Carousel Left
					nextslide.css({ left: -base.$el.width(), 'visibility': 'visible' }).animate({ left: 0, avoidTransforms: false }, base.options.transition_speed, function () { base.afterAnimation(); });
					liveslide.animate({ left: base.$el.width(), avoidTransforms: false }, base.options.transition_speed);
					break;
			}
			return false;
		};


		/* Previous Slide
		----------------------------*/
		base.prevSlide = function () {

			if (vars.in_animation || !api.options.slideshow) return false;		// Abort if currently animating
			else vars.in_animation = true;		// Otherwise set animation marker

			var old_slide_number = vars.current_slide;
			// Get current slide number
			if (vars.current_slide > 0) {
				vars.current_slide--;
			} else if (base.options.loop) {
				vars.current_slide = base.options.slides.length - 1;
			}

			if (old_slide_number == vars.current_slide) {
				vars.in_animation = false;
				return false;
			}

			clearInterval(vars.slideshow_interval);	// Stop slideshow

			var slides = base.options.slides,					// Pull in slides array
				liveslide = base.$el.find('.activeslide');		// Find active slide
			$('.prevslide').removeClass('prevslide');
			liveslide.removeClass('activeslide').addClass('prevslide');		// Remove active class & update previous slide

			var nextslide = $(base.el + ' li:eq(' + vars.current_slide + ')'),
				prevslide = base.$el.find('.prevslide');

			// If hybrid mode is on drop quality for transition
			if (base.options.performance == 1) base.$el.removeClass('quality').addClass('speed');


			/*-----Load Image-----*/

			loadSlide = vars.current_slide;

			var targetList = base.el + ' li:eq(' + loadSlide + ')';
			if (!$(targetList).html()) {
				// If links should open in new window
				var linkTarget = base.options.new_window ? ' target="_blank"' : '';
				imageLink = (base.options.slides[loadSlide].url) ? "href='" + base.options.slides[loadSlide].url + "'" : "";	// If link exists, build it
				var img = $('<img src="' + base.options.slides[loadSlide].image + '"/>');

				img.appendTo(targetList).wrap('<a ' + imageLink + linkTarget + '></a>').parent().parent().addClass('image-loading').css('visibility', 'hidden');

				img.load(function () {
					base._origDim($(this));
					base.resizeNow();
				});	// End Load
			};

			// Update thumbnails (if enabled)
			if (base.options.thumbnail_navigation == 1) {

				// Load previous thumbnail
				//prevThumb = loadSlide;
				loadSlide == 0 ? prevThumb = base.options.slides.length - 1 : prevThumb = loadSlide - 1;
				$(vars.prev_thumb).html($("<img/>").attr("src", base.options.slides[prevThumb].image));

				// Load next thumbnail
				vars.current_slide == base.options.slides.length - 1 ? nextThumb = 0 : nextThumb = vars.current_slide + 1;
				$(vars.next_thumb).html($("<img/>").attr("src", base.options.slides[nextThumb].image));
			}

			/*-----End Load Image-----*/


			// Call theme function for before slide transition
			if (typeof theme != 'undefined' && typeof theme.beforeAnimation == "function") theme.beforeAnimation('prev');

			//Update slide markers
			if (base.options.slide_links) {
				$('.current-slide').removeClass('current-slide');
				$(vars.slide_list + '> li').eq(vars.current_slide).addClass('current-slide');
			}

			nextslide.css('visibility', 'hidden').addClass('activeslide');	// Update active slide

			switch (base.options.transition) {
				case 0: case 'none':	// No transition
					nextslide.css('visibility', 'visible'); vars.in_animation = false; base.afterAnimation();
					break;
				case 1: case 'fade':	// Fade
					nextslide.css({ opacity: 0, 'visibility': 'visible' }).animate({ opacity: 1, avoidTransforms: false }, base.options.transition_speed, function () { base.afterAnimation(); });
					break;
				case 2: case 'slideTop':	// Slide Top (reverse)
					nextslide.css({ top: base.$el.height(), 'visibility': 'visible' }).animate({ top: 0, avoidTransforms: false }, base.options.transition_speed, function () { base.afterAnimation(); });
					break;
				case 3: case 'slideRight':	// Slide Right (reverse)
					nextslide.css({ left: -base.$el.width(), 'visibility': 'visible' }).animate({ left: 0, avoidTransforms: false }, base.options.transition_speed, function () { base.afterAnimation(); });
					break;
				case 4: case 'slideBottom': // Slide Bottom (reverse)
					nextslide.css({ top: -base.$el.height(), 'visibility': 'visible' }).animate({ top: 0, avoidTransforms: false }, base.options.transition_speed, function () { base.afterAnimation(); });
					break;
				case 5: case 'slideLeft':  // Slide Left (reverse)
					nextslide.css({ left: base.$el.width(), 'visibility': 'visible' }).animate({ left: 0, avoidTransforms: false }, base.options.transition_speed, function () { base.afterAnimation(); });
					break;
				case 6: case 'carouselRight':	// Carousel Right (reverse)
					nextslide.css({ left: -base.$el.width(), 'visibility': 'visible' }).animate({ left: 0, avoidTransforms: false }, base.options.transition_speed, function () { base.afterAnimation(); });
					liveslide.css({ left: 0 }).animate({ left: base.$el.width(), avoidTransforms: false }, base.options.transition_speed);
					break;
				case 7: case 'carouselLeft':   // Carousel Left (reverse)
					nextslide.css({ left: base.$el.width(), 'visibility': 'visible' }).animate({ left: 0, avoidTransforms: false }, base.options.transition_speed, function () { base.afterAnimation(); });
					liveslide.css({ left: 0 }).animate({ left: -base.$el.width(), avoidTransforms: false }, base.options.transition_speed);
					break;
			}
			return false;
		};


		/* Play/Pause Toggle
		----------------------------*/
		base.playToggle = function () {

			if (vars.in_animation || !api.options.slideshow) return false;		// Abort if currently animating

			if (vars.is_paused) {

				vars.is_paused = false;

				// Call theme function for play
				if (typeof theme != 'undefined' && typeof theme.playToggle == "function") theme.playToggle('play');

				// Resume slideshow
				vars.slideshow_interval = setInterval(base.nextSlide, base.options.slide_interval);

			} else {

				vars.is_paused = true;

				// Call theme function for pause
				if (typeof theme != 'undefined' && typeof theme.playToggle == "function") theme.playToggle('pause');

				// Stop slideshow
				clearInterval(vars.slideshow_interval);

			}

			return false;

		};

		/* Tear down this instance of supersized
		----------------------------*/
		base.destroy = function () {
			if (vars.in_animation || !api.options.slideshow) return;		// Abort if currently animating

			// Start slideshow if paused. Without this, the slideshow is paused and the play/pause button has the wrong icon
			// when the user clicks the 'start slideshow' button a second time.
			if (vars.is_paused)
				api.playToggle();

			clearInterval(vars.slideshow_interval);

			// Unbind events (requires jQuery 1.7+)
			$(document.documentElement).off('.supersized');
			$('.ssControlsContainer *').off('click');

			var currentSlideId = vars.options.slides[vars.current_slide].id;

			vars = null;
			api = null;

			// Remove slideshow DOM elements and restore the page.
			$('#supersized-loader,#supersized,.ssControlsContainer').remove();
			$('body .supersized_hidden').show().removeClass('supersized_hidden');

			// Trigger on_destroy event
			base.options.on_destroy.apply(null, [currentSlideId]);
		};

		/* Go to specific slide
	----------------------------*/
		base.goTo = function (targetSlide) {
			if (vars.in_animation || !api.options.slideshow) return false;		// Abort if currently animating

			var totalSlides = base.options.slides.length;

			// If target outside range
			if (targetSlide < 0) {
				targetSlide = totalSlides;
			} else if (targetSlide > totalSlides) {
				targetSlide = 1;
			}
			targetSlide = totalSlides - targetSlide + 1;

			clearInterval(vars.slideshow_interval);	// Stop slideshow, prevent buildup

			// Call theme function for goTo trigger
			if (typeof theme != 'undefined' && typeof theme.goTo == "function") theme.goTo();

			if (vars.current_slide == totalSlides - targetSlide) {
				if (!(vars.is_paused)) {
					vars.slideshow_interval = setInterval(base.nextSlide, base.options.slide_interval);
				}
				return false;
			}

			// If ahead of current position
			if (totalSlides - targetSlide > vars.current_slide) {

				// Adjust for new next slide
				vars.current_slide = totalSlides - targetSlide - 1;
				vars.update_images = 'next';
				base._placeSlide(vars.update_images);

				//Otherwise it's before current position
			} else if (totalSlides - targetSlide < vars.current_slide) {

				// Adjust for new prev slide
				vars.current_slide = totalSlides - targetSlide + 1;
				vars.update_images = 'prev';
				base._placeSlide(vars.update_images);

			}

			// set active markers
			if (base.options.slide_links) {
				$(vars.slide_list + '> .current-slide').removeClass('current-slide');
				$(vars.slide_list + '> li').eq((totalSlides - targetSlide)).addClass('current-slide');
			}

			if (base.options.thumb_links) {
				$(vars.thumb_list + '> .current-thumb').removeClass('current-thumb');
				$(vars.thumb_list + '> li').eq((totalSlides - targetSlide)).addClass('current-thumb');
			}

		};


		/* Place Slide
----------------------------*/
		base._placeSlide = function (place) {

			// If links should open in new window
			var linkTarget = base.options.new_window ? ' target="_blank"' : '';

			loadSlide = false;

			if (place == 'next') {

				vars.current_slide == base.options.slides.length - 1 ? loadSlide = 0 : loadSlide = vars.current_slide + 1;	// Determine next slide

				var targetList = base.el + ' li:eq(' + loadSlide + ')';

				if (!$(targetList).html()) {
					// If links should open in new window
					var linkTarget = base.options.new_window ? ' target="_blank"' : '';

					imageLink = (base.options.slides[loadSlide].url) ? "href='" + base.options.slides[loadSlide].url + "'" : "";	// If link exists, build it
					var img = $('<img src="' + base.options.slides[loadSlide].image + '"/>');

					img.appendTo(targetList).wrap('<a ' + imageLink + linkTarget + '></a>').parent().parent().addClass('image-loading').css('visibility', 'hidden');

					img.load(function () {
						base._origDim($(this));
						base.resizeNow();
					});	// End Load
				};

				base.nextSlide();

			} else if (place == 'prev') {

				vars.current_slide - 1 < 0 ? loadSlide = base.options.slides.length - 1 : loadSlide = vars.current_slide - 1;	// Determine next slide

				var targetList = base.el + ' li:eq(' + loadSlide + ')';

				if (!$(targetList).html()) {
					// If links should open in new window
					var linkTarget = base.options.new_window ? ' target="_blank"' : '';

					imageLink = (base.options.slides[loadSlide].url) ? "href='" + base.options.slides[loadSlide].url + "'" : "";	// If link exists, build it
					var img = $('<img src="' + base.options.slides[loadSlide].image + '"/>');

					img.appendTo(targetList).wrap('<a ' + imageLink + linkTarget + '></a>').parent().parent().addClass('image-loading').css('visibility', 'hidden');

					img.load(function () {
						base._origDim($(this));
						base.resizeNow();
					});	// End Load
				};
				base.prevSlide();
			}

		};


		/* Get Original Dimensions
		----------------------------*/
		base._origDim = function (targetSlide) {
			targetSlide.data('origWidth', targetSlide.width()).data('origHeight', targetSlide.height());
		};


		/* After Slide Animation
		----------------------------*/
		base.afterAnimation = function () {

			// If hybrid mode is on swap back to higher image quality
			if (base.options.performance == 1) {
				base.$el.removeClass('speed').addClass('quality');
			}

			// Update previous slide
			if (vars.update_images) {
				vars.current_slide - 1 < 0 ? setPrev = base.options.slides.length - 1 : setPrev = vars.current_slide - 1;
				vars.update_images = false;
				$('.prevslide').removeClass('prevslide');
				$(base.el + ' li:eq(' + setPrev + ')').addClass('prevslide');
			}

			vars.in_animation = false;

			// Resume slideshow
			if (!vars.is_paused && base.options.slideshow) {
				vars.slideshow_interval = setInterval(base.nextSlide, base.options.slide_interval);
				if (!base.options.loop && !base.options.auto_exit && vars.current_slide == base.options.slides.length - 1) base.playToggle();
			}

			// Call theme function for after slide transition
			if (typeof theme != 'undefined' && typeof theme.afterAnimation == "function") theme.afterAnimation();

			return false;

		};

		base.getField = function (field) {
			return base.options.slides[vars.current_slide][field];
		};

		// Make it go!
		base.init();
	};


	/* Global Variables
	----------------------------*/
	$.supersized.vars = {

		// Elements							
		thumb_tray: '#thumb-tray',	// Thumbnail tray
		thumb_list: '#thumb-list',	// Thumbnail list
		slide_list: '#slide-list',	// Slide link list

		// Internal variables
		current_slide: 0,			// Current slide number
		in_animation: false,		// Prevents animations from stacking
		is_paused: false,		// Tracks paused on/off
		hover_pause: false,		// If slideshow is paused from hover
		slideshow_interval: false,		// Stores slideshow timer					
		update_images: false,		// Trigger to update images after slide jump
		options: {}			// Stores assembled options list

	};


	/* Default Options
	----------------------------*/
	$.supersized.defaultOptions = {

		// Functionality
		slideshow: 1,			// Slideshow on/off
		autoplay: 1,			// Slideshow starts playing automatically
		auto_exit: 0,      // Exit the slideshow when the last slide is finished
		start_slide: 1,			// Start slide (0 is random)
		loop: 1,			// Enables moving between the last and first slide.
		random: 0,			// Randomize slide order (Ignores start slide)
		slide_interval: 5000,		// Length between transitions
		transition: 1, 			// 0-None, 1-Fade, 2-Slide Top, 3-Slide Right, 4-Slide Bottom, 5-Slide Left, 6-Carousel Right, 7-Carousel Left
		transition_speed: 750,		// Speed of transition
		new_window: 1,			// Image links open in new window/tab
		pause_hover: 0,			// Pause slideshow on hover
		keyboard_nav: 1,			// Keyboard navigation on/off
		performance: 1,			// 0-Normal, 1-Hybrid speed/quality, 2-Optimizes image quality, 3-Optimizes transition speed //  (Only works for Firefox/IE, not Webkit)
		image_protect: 1,			// Disables image dragging and right click with Javascript

		// Size & Position
		fit_always: 0,			// Image will never exceed browser width or height (Ignores min. dimensions)
		fit_landscape: 0,			// Landscape images will not exceed browser width
		fit_portrait: 1,			// Portrait images will not exceed browser height  			   
		min_width: 0,			// Min width allowed (in pixels)
		min_height: 0,			// Min height allowed (in pixels)
		horizontal_center: 1,			// Horizontally center background
		vertical_center: 1,			// Vertically center background


		// Components							
		slide_links: 1,			// Individual links for each slide (Options: false, 'num', 'name', 'blank')
		thumb_links: 1,			// Individual thumb links for each slide
		thumbnail_navigation: 0,			// Thumbnail navigation
		on_destroy: function () { } // Empty implementation for on_destroy event, may be overridden by user

	};

	$.fn.supersized = function (options) {
		return this.each(function () {
			(new $.supersized(options));
		});
	};

})(jQuery);

/*
	supersized.shutter.js
	Supersized - Fullscreen Slideshow jQuery Plugin
	Version : 3.2.7
	Theme 	: Shutter 1.1
	
	Site	: www.buildinternet.com/project/supersized
	Author	: Sam Dunn
	Company : One Mighty Roar (www.onemightyroar.com)
	License : MIT License / GPL License

*/

(function ($) {

	theme = {


		/* Initial Placement
		----------------------------*/
		_init: function () {

			// Configure Slide Links
			if (api.options.slide_links) {
				// Note: This code is repeated in the resize event, so if you change it here do it there, too.
				var maxSlideListWidth = $(vars.slide_list).parent().width() - 400; // Constrain the slide bullets area width so they don't cover buttons
				$(vars.slide_list).css('margin-left', -$(vars.slide_list).width() / 2).css('max-width', maxSlideListWidth);
			}

			// Start progressbar if autoplay enabled
			if (api.options.autoplay) {
				if (api.options.progress_bar) theme.progressBar(); else $(vars.progress_bar).parent().hide();
			} else {
				if ($(vars.play_button).attr('src')) $(vars.play_button).attr("src", api.options.image_path + "play.png");	// If pause play button is image, swap src
				if (api.options.progress_bar)
					$(vars.progress_bar).stop().css({ left: -$(window).width() });	//  Place progress bar
				else
					$(vars.progress_bar).parent().hide();
			}


			/* Thumbnail Tray
			----------------------------*/
			// Hide tray off screen
			$(vars.thumb_tray).css({ bottom: -($(vars.thumb_tray).outerHeight() + 5) });

			// Thumbnail Tray Toggle
			$(vars.tray_button).toggle(function () {
				$(vars.thumb_tray).stop().animate({ bottom: 0, avoidTransforms: true }, 300);
				if ($(vars.tray_arrow).attr('src')) $(vars.tray_arrow).attr("src", api.options.image_path + "button-tray-down.png");
				return false;
			}, function () {
				$(vars.thumb_tray).stop().animate({ bottom: -($(vars.thumb_tray).outerHeight() + 5), avoidTransforms: true }, 300);
				if ($(vars.tray_arrow).attr('src')) $(vars.tray_arrow).attr("src", api.options.image_path + "button-tray-up.png");
				return false;
			});

			// Make thumb tray proper size
			$(vars.thumb_list).width($('> li', vars.thumb_list).length * $('> li', vars.thumb_list).outerWidth(true));	//Adjust to true width of thumb markers

			// Display total slides
			if ($(vars.slide_total).length) {
				$(vars.slide_total).html(api.options.slides.length);
			}


			/* Thumbnail Tray Navigation
			----------------------------*/
			if (api.options.thumb_links) {
				//Hide thumb arrows if not needed
				if ($(vars.thumb_list).width() <= $(vars.thumb_tray).width()) {
					$(vars.thumb_back + ',' + vars.thumb_forward).fadeOut(0);
				}

				// Thumb Intervals
				vars.thumb_interval = Math.floor($(vars.thumb_tray).width() / $('> li', vars.thumb_list).outerWidth(true)) * $('> li', vars.thumb_list).outerWidth(true);
				vars.thumb_page = 0;

				// Cycle thumbs forward
				$(vars.thumb_forward).click(function () {
					if (vars.thumb_page - vars.thumb_interval <= -$(vars.thumb_list).width()) {
						vars.thumb_page = 0;
						$(vars.thumb_list).stop().animate({ 'left': vars.thumb_page }, { duration: 500, easing: 'easeOutExpo' });
					} else {
						vars.thumb_page = vars.thumb_page - vars.thumb_interval;
						$(vars.thumb_list).stop().animate({ 'left': vars.thumb_page }, { duration: 500, easing: 'easeOutExpo' });
					}
				});

				// Cycle thumbs backwards
				$(vars.thumb_back).click(function () {
					if (vars.thumb_page + vars.thumb_interval > 0) {
						vars.thumb_page = Math.floor($(vars.thumb_list).width() / vars.thumb_interval) * -vars.thumb_interval;
						if ($(vars.thumb_list).width() <= -vars.thumb_page) vars.thumb_page = vars.thumb_page + vars.thumb_interval;
						$(vars.thumb_list).stop().animate({ 'left': vars.thumb_page }, { duration: 500, easing: 'easeOutExpo' });
					} else {
						vars.thumb_page = vars.thumb_page + vars.thumb_interval;
						$(vars.thumb_list).stop().animate({ 'left': vars.thumb_page }, { duration: 500, easing: 'easeOutExpo' });
					}
				});

			}


			/* Navigation Items
			----------------------------*/
			$(vars.next_slide).click(function () {
				api.nextSlide();
			});

			$(vars.prev_slide).click(function () {
				api.prevSlide();
			});

			// Full Opacity on Hover
			if (jQuery.support.opacity) {
				$(vars.prev_slide + ',' + vars.next_slide).mouseover(function () {
					$(this).stop().animate({ opacity: 1 }, 100);
				}).mouseout(function () {
					$(this).stop().animate({ opacity: 0.6 }, 100);
				});
			}

			if (api.options.thumbnail_navigation) {
				// Next thumbnail clicked
				$(vars.next_thumb).click(function () {
					api.nextSlide();
				});
				// Previous thumbnail clicked
				$(vars.prev_thumb).click(function () {
					api.prevSlide();
				});
			}

			$(vars.play_button).click(function () {
				api.playToggle();
			});


			/* Thumbnail Mouse Scrub
			----------------------------*/
			if (api.options.mouse_scrub) {
				$(vars.thumb_tray).mousemove(function (e) {
					var containerWidth = $(vars.thumb_tray).width(),
						listWidth = $(vars.thumb_list).width();
					if (listWidth > containerWidth) {
						var mousePos = 1,
							diff = e.pageX - mousePos;
						if (diff > 10 || diff < -10) {
							mousePos = e.pageX;
							newX = (containerWidth - listWidth) * (e.pageX / containerWidth);
							diff = parseInt(Math.abs(parseInt($(vars.thumb_list).css('left')) - newX)).toFixed(0);
							$(vars.thumb_list).stop().animate({ 'left': newX }, { duration: diff * 3, easing: 'easeOutExpo' });
						}
					}
				});
			}


			/* Window Resize
			----------------------------*/
			$(window).resize(function () {

				// Delay progress bar on resize
				if (api.options.progress_bar && !vars.in_animation) {
					if (vars.slideshow_interval) clearInterval(vars.slideshow_interval);
					if (api.options.slides.length - 1 > 0) clearInterval(vars.slideshow_interval);

					$(vars.progress_bar).stop().css({ left: -$(window).width() });

					if (!vars.progressDelay && api.options.slideshow) {
						// Delay slideshow from resuming so Chrome can refocus images
						vars.progressDelay = setTimeout(function () {
							if (!vars.is_paused) {
								theme.progressBar();
								vars.slideshow_interval = setInterval(api.nextSlide, api.options.slide_interval);
							}
							vars.progressDelay = false;
						}, 1000);
					}
				}

				// Thumb Links
				if (api.options.thumb_links && vars.thumb_tray.length) {
					// Update Thumb Interval & Page
					vars.thumb_page = 0;
					vars.thumb_interval = Math.floor($(vars.thumb_tray).width() / $('> li', vars.thumb_list).outerWidth(true)) * $('> li', vars.thumb_list).outerWidth(true);

					// Adjust thumbnail markers
					if ($(vars.thumb_list).width() > $(vars.thumb_tray).width()) {
						$(vars.thumb_back + ',' + vars.thumb_forward).fadeIn('fast');
						$(vars.thumb_list).stop().animate({ 'left': 0 }, 200);
					} else {
						$(vars.thumb_back + ',' + vars.thumb_forward).fadeOut('fast');
					}

				}

				// Configure Slide Links
				if (api.options.slide_links) {
					// Note: This code is repeated in the _init function, so if you change it here do it there, too.
					maxSlideListWidth = $(vars.slide_list).parent().width() - 400; // Constrain the slide bullets area width so they don't cover buttons
					$(vars.slide_list).css('margin-left', -$(vars.slide_list).width() / 2).css('max-width', maxSlideListWidth);
					console.log(maxSlideListWidth);
				}
			});


		},


		/* Go To Slide
		----------------------------*/
		goTo: function () {
			if (api.options.progress_bar && !vars.is_paused) {
				$(vars.progress_bar).stop().css({ left: -$(window).width() });
				theme.progressBar();
			}
		},

		/* Play & Pause Toggle
		----------------------------*/
		playToggle: function (state) {

			if (state == 'play') {
				// If image, swap to pause
				if ($(vars.play_button).attr('src')) $(vars.play_button).attr("src", api.options.image_path + "pause.png");
				if (api.options.progress_bar && !vars.is_paused) theme.progressBar();
			} else if (state == 'pause') {
				// If image, swap to play
				if ($(vars.play_button).attr('src')) $(vars.play_button).attr("src", api.options.image_path + "play.png");
				if (api.options.progress_bar && vars.is_paused) $(vars.progress_bar).stop().css({ left: -$(window).width() });
			}

		},


		/* Before Slide Transition
		----------------------------*/
		beforeAnimation: function (direction) {
			if (api.options.progress_bar && !vars.is_paused) $(vars.progress_bar).stop().css({ left: -$(window).width() });

			/* Update Fields
			----------------------------*/
			// Update slide caption
			if ($(vars.slide_caption).length) {
				(api.getField('title')) ? $(vars.slide_caption).html(api.getField('title')) : $(vars.slide_caption).html('');
			}
			// Update slide number
			if (vars.slide_current.length) {
				$(vars.slide_current).html(vars.current_slide + 1);
			}


			// Highlight current thumbnail and adjust row position
			if (api.options.thumb_links) {

				$('.current-thumb').removeClass('current-thumb');
				$('li', vars.thumb_list).eq(vars.current_slide).addClass('current-thumb');

				// If thumb out of view
				if ($(vars.thumb_list).width() > $(vars.thumb_tray).width()) {
					// If next slide direction
					if (direction == 'next') {
						if (vars.current_slide == 0) {
							vars.thumb_page = 0;
							$(vars.thumb_list).stop().animate({ 'left': vars.thumb_page }, { duration: 500, easing: 'easeOutExpo' });
						} else if ($('.current-thumb').offset().left - $(vars.thumb_tray).offset().left >= vars.thumb_interval) {
							vars.thumb_page = vars.thumb_page - vars.thumb_interval;
							$(vars.thumb_list).stop().animate({ 'left': vars.thumb_page }, { duration: 500, easing: 'easeOutExpo' });
						}
						// If previous slide direction
					} else if (direction == 'prev') {
						if (vars.current_slide == api.options.slides.length - 1) {
							vars.thumb_page = Math.floor($(vars.thumb_list).width() / vars.thumb_interval) * -vars.thumb_interval;
							if ($(vars.thumb_list).width() <= -vars.thumb_page) vars.thumb_page = vars.thumb_page + vars.thumb_interval;
							$(vars.thumb_list).stop().animate({ 'left': vars.thumb_page }, { duration: 500, easing: 'easeOutExpo' });
						} else if ($('.current-thumb').offset().left - $(vars.thumb_tray).offset().left < 0) {
							if (vars.thumb_page + vars.thumb_interval > 0) return false;
							vars.thumb_page = vars.thumb_page + vars.thumb_interval;
							$(vars.thumb_list).stop().animate({ 'left': vars.thumb_page }, { duration: 500, easing: 'easeOutExpo' });
						}
					}
				}


			}

		},


		/* After Slide Transition
		----------------------------*/
		afterAnimation: function () {
			if (api.options.progress_bar && !vars.is_paused) theme.progressBar();	//  Start progress bar
		},


		/* Progress Bar
		----------------------------*/
		progressBar: function () {
			$(vars.progress_bar).stop().css({ left: -$(window).width() }).animate({ left: 0 }, api.options.slide_interval);
		}


	};


	/* Theme Specific Variables
	----------------------------*/
	$.supersized.themeVars = {

		// Internal Variables
		progress_delay: false,				// Delay after resize before resuming slideshow
		thumb_page: false,				// Thumbnail page
		thumb_interval: false,				// Thumbnail interval

		// General Elements							
		play_button: '#pauseplay',		// Play/Pause button
		next_slide: '#nextslide',		// Next slide button
		prev_slide: '#prevslide',		// Prev slide button
		next_thumb: '#nextthumb',		// Next slide thumb button
		prev_thumb: '#prevthumb',		// Prev slide thumb button

		slide_caption: '#slidecaption',	// Slide caption
		slide_current: '.slidenumber',		// Current slide number
		slide_total: '.totalslides',		// Total Slides
		slide_list: '#slide-list',		// Slide jump list							

		thumb_tray: '#thumb-tray',		// Thumbnail tray
		thumb_list: '#thumb-list',		// Thumbnail list
		thumb_forward: '#thumb-forward',	// Cycles forward through thumbnail list
		thumb_back: '#thumb-back',		// Cycles backwards through thumbnail list
		tray_arrow: '#tray-arrow',		// Thumbnail tray button arrow
		tray_button: '#tray-button',		// Thumbnail tray button

		progress_bar: '#progress-bar'		// Progress bar

	};

	/* Theme Specific Options
	----------------------------*/
	$.supersized.themeOptions = {

		progress_bar: 1,		// Timer for each slide											
		image_path: 'img/',				// Default image path
		mouse_scrub: 0,		// Thumbnails move with mouse
		// html_template contains the HTML for the slideshow controls
		html_template: '\
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
					<img id="pauseplay" src="img/pause.png" /></a> \
\
				<a id="stop-button"> \
					<img src="img/stop.png" /></a> \
\
				<!--Slide counter--> \
				<div id="slidecounter"> \
					<span class="slidenumber"></span>/ <span class="totalslides"></span> \
				</div> \
\
				<!--Slide captions displayed here--> \
				<div id="slidecaption"></div> \
\
				<!--Thumb Tray button--> \
				<a id="tray-button"> \
					<img id="tray-arrow" src="img/button-tray-up.png" /></a> \
\
				<!--Navigation--> \
				<ul id="slide-list"></ul> \
\
			</div> \
		</div> \
</div>'

	};


})(jQuery);

//#endregion End supersized

//#region MultiSelect

/*
 * jQuery MultiSelect UI Widget 1.13
 * Copyright (c) 2012 Eric Hynds
 *
 * http://www.erichynds.com/jquery/jquery-ui-multiselect-widget/
 *
 * Depends:
 *   - jQuery 1.4.2+
 *   - jQuery UI 1.8 widget factory
 *
 * Optional:
 *   - jQuery UI effects
 *   - jQuery UI position utility
 *
 * Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 *
 * CHANGE LOG
 * 2014-05-30 - HTML-encode quotes. See [Roger]
 *
*/
(function ($, undefined) {

	var multiselectID = 0;

	$.widget("ech.multiselect", {

		// default options
		options: {
			header: true,
			height: 175,
			minWidth: 225,
			classes: '',
			checkAllText: 'Check all',
			uncheckAllText: 'Uncheck all',
			noneSelectedText: 'Select options',
			selectedText: '# selected',
			selectedList: 0,
			show: null,
			hide: null,
			autoOpen: false,
			multiple: true,
			position: {}
		},

		_create: function () {
			var el = this.element.hide(),
				o = this.options;

			this.speed = $.fx.speeds._default; // default speed for effects
			this._isOpen = false; // assume no

			var
				button = (this.button = $('<button type="button"><span class="ui-icon ui-icon-triangle-2-n-s"></span></button>'))
					.addClass('ui-multiselect ui-widget ui-state-default ui-corner-all')
					.addClass(o.classes)
					.attr({ 'title': el.attr('title'), 'aria-haspopup': true, 'tabIndex': el.attr('tabIndex') })
					.insertAfter(el),

				buttonlabel = (this.buttonlabel = $('<span />'))
					.html(o.noneSelectedText)
					.appendTo(button),

				menu = (this.menu = $('<div />'))
					.addClass('ui-multiselect-menu ui-widget ui-widget-content ui-corner-all')
					.addClass(o.classes)
					.appendTo(document.body),

				header = (this.header = $('<div />'))
					.addClass('ui-widget-header ui-corner-all ui-multiselect-header ui-helper-clearfix')
					.appendTo(menu),

				headerLinkContainer = (this.headerLinkContainer = $('<ul />'))
					.addClass('ui-helper-reset')
					.html(function () {
						if (o.header === true) {
							return '<li><a class="ui-multiselect-all" href="#"><span class="ui-icon ui-icon-check"></span><span>' + o.checkAllText + '</span></a></li><li><a class="ui-multiselect-none" href="#"><span class="ui-icon ui-icon-closethick"></span><span>' + o.uncheckAllText + '</span></a></li>';
						} else if (typeof o.header === "string") {
							return '<li>' + o.header + '</li>';
						} else {
							return '';
						}
					})
					.append('<li class="ui-multiselect-close"><a href="#" class="ui-multiselect-close"><span class="ui-icon ui-icon-circle-close"></span></a></li>')
					.appendTo(header),

				checkboxContainer = (this.checkboxContainer = $('<ul />'))
					.addClass('ui-multiselect-checkboxes ui-helper-reset')
					.appendTo(menu);

			// perform event bindings
			this._bindEvents();

			// build menu
			this.refresh(true);

			// some addl. logic for single selects
			if (!o.multiple) {
				menu.addClass('ui-multiselect-single');
			}
		},

		_init: function () {
			if (this.options.header === false) {
				this.header.hide();
			}
			if (!this.options.multiple) {
				this.headerLinkContainer.find('.ui-multiselect-all, .ui-multiselect-none').hide();
			}
			if (this.options.autoOpen) {
				this.open();
			}
			if (this.element.is(':disabled')) {
				this.disable();
			}
		},

		refresh: function (init) {
			var el = this.element,
				o = this.options,
				menu = this.menu,
				checkboxContainer = this.checkboxContainer,
				optgroups = [],
				html = "",
				id = el.attr('id') || multiselectID++; // unique ID for the label & option tags

			// build items
			el.find('option').each(function (i) {
				var $this = $(this),
					parent = this.parentNode,
					title = this.innerHTML,
					description = this.title,
					value = this.value,
					inputID = 'ui-multiselect-' + (this.id || id + '-option-' + i),
					isDisabled = this.disabled,
					isSelected = this.selected,
					labelClasses = ['ui-corner-all'],
					liClasses = (isDisabled ? 'ui-multiselect-disabled ' : ' ') + this.className,
					optLabel;

				// is this an optgroup?
				if (parent.tagName === 'OPTGROUP') {
					optLabel = parent.getAttribute('label');

					// has this optgroup been added already?
					if ($.inArray(optLabel, optgroups) === -1) {
						html += '<li class="ui-multiselect-optgroup-label ' + parent.className + '"><a href="#">' + optLabel + '</a></li>';
						optgroups.push(optLabel);
					}
				}

				if (isDisabled) {
					labelClasses.push('ui-state-disabled');
				}

				// browsers automatically select the first option
				// by default with single selects
				if (isSelected && !o.multiple) {
					labelClasses.push('ui-state-active');
				}

				html += '<li class="' + liClasses + '">';

				// create the label
				html += '<label for="' + inputID + '" title="' + description + '" class="' + labelClasses.join(' ') + '">';
				html += '<input id="' + inputID + '" name="multiselect_' + id + '" type="' + (o.multiple ? "checkbox" : "radio") + '" value="' + value.replace(/\"/g, '&quot;') + '" title="' + title.replace(/\"/g, '&quot;') + '"'; // [Roger] Added replace function to HTML encode quotes

				// pre-selected?
				if (isSelected) {
					html += ' checked="checked"';
					html += ' aria-selected="true"';
				}

				// disabled?
				if (isDisabled) {
					html += ' disabled="disabled"';
					html += ' aria-disabled="true"';
				}

				// add the title and close everything off
				html += ' /><span>' + title + '</span></label></li>';
			});

			// insert into the DOM
			checkboxContainer.html(html);

			// cache some moar useful elements
			this.labels = menu.find('label');
			this.inputs = this.labels.children('input');

			// set widths
			this._setButtonWidth();
			this._setMenuWidth();

			// remember default value
			this.button[0].defaultValue = this.update();

			// broadcast refresh event; useful for widgets
			if (!init) {
				this._trigger('refresh');
			}
		},

		// updates the button text. call refresh() to rebuild
		update: function () {
			var o = this.options,
				$inputs = this.inputs,
				$checked = $inputs.filter(':checked'),
				numChecked = $checked.length,
				value;

			if (numChecked === 0) {
				value = o.noneSelectedText;
			} else {
				if ($.isFunction(o.selectedText)) {
					value = o.selectedText.call(this, numChecked, $inputs.length, $checked.get());
				} else if (/\d/.test(o.selectedList) && o.selectedList > 0 && numChecked <= o.selectedList) {
					value = $checked.map(function () { return $(this).next().html(); }).get().join(', ');
				} else {
					value = o.selectedText.replace('#', numChecked).replace('#', $inputs.length);
				}
			}

			this.buttonlabel.html(value);
			return value;
		},

		// binds events
		_bindEvents: function () {
			var self = this, button = this.button;

			function clickHandler() {
				self[self._isOpen ? 'close' : 'open']();
				return false;
			}

			// webkit doesn't like it when you click on the span :(
			button
				.find('span')
				.bind('click.multiselect', clickHandler);

			// button events
			button.bind({
				click: clickHandler,
				keypress: function (e) {
					switch (e.which) {
						case 27: // esc
						case 38: // up
						case 37: // left
							self.close();
							break;
						case 39: // right
						case 40: // down
							self.open();
							break;
					}
				},
				mouseenter: function () {
					if (!button.hasClass('ui-state-disabled')) {
						$(this).addClass('ui-state-hover');
					}
				},
				mouseleave: function () {
					$(this).removeClass('ui-state-hover');
				},
				focus: function () {
					if (!button.hasClass('ui-state-disabled')) {
						$(this).addClass('ui-state-focus');
					}
				},
				blur: function () {
					$(this).removeClass('ui-state-focus');
				}
			});

			// header links
			this.header
				.delegate('a', 'click.multiselect', function (e) {
					// close link
					if ($(this).hasClass('ui-multiselect-close')) {
						self.close();

						// check all / uncheck all
					} else {
						self[$(this).hasClass('ui-multiselect-all') ? 'checkAll' : 'uncheckAll']();
					}

					e.preventDefault();
				});

			// optgroup label toggle support
			this.menu
				.delegate('li.ui-multiselect-optgroup-label a', 'click.multiselect', function (e) {
					e.preventDefault();

					var $this = $(this),
						$inputs = $this.parent().nextUntil('li.ui-multiselect-optgroup-label').find('input:visible:not(:disabled)'),
						nodes = $inputs.get(),
						label = $this.parent().text();

					// trigger event and bail if the return is false
					if (self._trigger('beforeoptgrouptoggle', e, { inputs: nodes, label: label }) === false) {
						return;
					}

					// toggle inputs
					self._toggleChecked(
						$inputs.filter(':checked').length !== $inputs.length,
						$inputs
					);

					self._trigger('optgrouptoggle', e, {
						inputs: nodes,
						label: label,
						checked: nodes[0].checked
					});
				})
				.delegate('label', 'mouseenter.multiselect', function () {
					if (!$(this).hasClass('ui-state-disabled')) {
						self.labels.removeClass('ui-state-hover');
						$(this).addClass('ui-state-hover').find('input').focus();
					}
				})
				.delegate('label', 'keydown.multiselect', function (e) {
					e.preventDefault();

					switch (e.which) {
						case 9: // tab
						case 27: // esc
							self.close();
							break;
						case 38: // up
						case 40: // down
						case 37: // left
						case 39: // right
							self._traverse(e.which, this);
							break;
						case 13: // enter
							$(this).find('input')[0].click();
							break;
					}
				})
				.delegate('input[type="checkbox"], input[type="radio"]', 'click.multiselect', function (e) {
					var $this = $(this),
						val = this.value,
						checked = this.checked,
						tags = self.element.find('option');

					// bail if this input is disabled or the event is cancelled
					if (this.disabled || self._trigger('click', e, { value: val, text: this.title, checked: checked }) === false) {
						e.preventDefault();
						return;
					}

					// make sure the input has focus. otherwise, the esc key
					// won't close the menu after clicking an item.
					$this.focus();

					// toggle aria state
					$this.attr('aria-selected', checked);

					// change state on the original option tags
					tags.each(function () {
						if (this.value === val) {
							this.selected = checked;
						} else if (!self.options.multiple) {
							this.selected = false;
						}
					});

					// some additional single select-specific logic
					if (!self.options.multiple) {
						self.labels.removeClass('ui-state-active');
						$this.closest('label').toggleClass('ui-state-active', checked);

						// close menu
						self.close();
					}

					// fire change on the select box
					self.element.trigger("change");

					// setTimeout is to fix multiselect issue #14 and #47. caused by jQuery issue #3827
					// http://bugs.jquery.com/ticket/3827
					setTimeout($.proxy(self.update, self), 10);
				});

			// close each widget when clicking on any other element/anywhere else on the page
			$(document).bind('mousedown.multiselect', function (e) {
				if (self._isOpen && !$.contains(self.menu[0], e.target) && !$.contains(self.button[0], e.target) && e.target !== self.button[0]) {
					self.close();
				}
			});

			// deal with form resets.  the problem here is that buttons aren't
			// restored to their defaultValue prop on form reset, and the reset
			// handler fires before the form is actually reset.  delaying it a bit
			// gives the form inputs time to clear.
			$(this.element[0].form).bind('reset.multiselect', function () {
				setTimeout($.proxy(self.refresh, self), 10);
			});
		},

		// set button width
		_setButtonWidth: function () {
			var width = this.element.outerWidth(),
				o = this.options;

			if (/\d/.test(o.minWidth) && width < o.minWidth) {
				width = o.minWidth;
			}

			// set widths
			this.button.width(width);
		},

		// set menu width
		_setMenuWidth: function () {
			var m = this.menu,
				width = this.button.outerWidth() -
					parseInt(m.css('padding-left'), 10) -
					parseInt(m.css('padding-right'), 10) -
					parseInt(m.css('border-right-width'), 10) -
					parseInt(m.css('border-left-width'), 10);

			m.width(width || this.button.outerWidth());
		},

		// move up or down within the menu
		_traverse: function (which, start) {
			var $start = $(start),
				moveToLast = which === 38 || which === 37,

				// select the first li that isn't an optgroup label / disabled
				$next = $start.parent()[moveToLast ? 'prevAll' : 'nextAll']('li:not(.ui-multiselect-disabled, .ui-multiselect-optgroup-label)')[moveToLast ? 'last' : 'first']();

			// if at the first/last element
			if (!$next.length) {
				var $container = this.menu.find('ul').last();

				// move to the first/last
				this.menu.find('label')[moveToLast ? 'last' : 'first']().trigger('mouseover');

				// set scroll position
				$container.scrollTop(moveToLast ? $container.height() : 0);

			} else {
				$next.find('label').trigger('mouseover');
			}
		},

		// This is an internal function to toggle the checked property and
		// other related attributes of a checkbox.
		//
		// The context of this function should be a checkbox; do not proxy it.
		_toggleState: function (prop, flag) {
			return function () {
				if (!this.disabled) {
					this[prop] = flag;
				}

				if (flag) {
					this.setAttribute('aria-selected', true);
				} else {
					this.removeAttribute('aria-selected');
				}
			};
		},

		_toggleChecked: function (flag, group) {
			var $inputs = (group && group.length) ? group : this.inputs,
				self = this;

			// toggle state on inputs
			$inputs.each(this._toggleState('checked', flag));

			// give the first input focus
			$inputs.eq(0).focus();

			// update button text
			this.update();

			// gather an array of the values that actually changed
			var values = $inputs.map(function () {
				return this.value;
			}).get();

			// toggle state on original option tags
			this.element
				.find('option')
				.each(function () {
					if (!this.disabled && $.inArray(this.value, values) > -1) {
						self._toggleState('selected', flag).call(this);
					}
				});

			// trigger the change event on the select
			if ($inputs.length) {
				this.element.trigger("change");
			}
		},

		_toggleDisabled: function (flag) {
			this.button
				.attr({ 'disabled': flag, 'aria-disabled': flag })[flag ? 'addClass' : 'removeClass']('ui-state-disabled');

			var inputs = this.menu.find('input');
			var key = "ech-multiselect-disabled";

			if (flag) {
				// remember which elements this widget disabled (not pre-disabled)
				// elements, so that they can be restored if the widget is re-enabled.
				inputs = inputs.filter(':enabled')
					.data(key, true)
			} else {
				inputs = inputs.filter(function () {
					return $.data(this, key) === true;
				}).removeData(key);
			}

			inputs
				.attr({ 'disabled': flag, 'arial-disabled': flag })
				.parent()[flag ? 'addClass' : 'removeClass']('ui-state-disabled');

			this.element
				.attr({ 'disabled': flag, 'aria-disabled': flag });
		},

		// open the menu
		open: function (e) {
			var self = this,
				button = this.button,
				menu = this.menu,
				speed = this.speed,
				o = this.options,
				args = [];

			// bail if the multiselectopen event returns false, this widget is disabled, or is already open
			if (this._trigger('beforeopen') === false || button.hasClass('ui-state-disabled') || this._isOpen) {
				return;
			}

			var $container = menu.find('ul').last(),
				effect = o.show,
				pos = button.offset();

			// figure out opening effects/speeds
			if ($.isArray(o.show)) {
				effect = o.show[0];
				speed = o.show[1] || self.speed;
			}

			// if there's an effect, assume jQuery UI is in use
			// build the arguments to pass to show()
			if (effect) {
				args = [effect, speed];
			}

			// set the scroll of the checkbox container
			$container.scrollTop(0).height(o.height);

			// position and show menu
			if ($.ui.position && !$.isEmptyObject(o.position)) {
				o.position.of = o.position.of || button;

				menu
					.show()
					.position(o.position)
					.hide();

				// if position utility is not available...
			} else {
				menu.css({
					top: pos.top + button.outerHeight(),
					left: pos.left
				});
			}

			// show the menu, maybe with a speed/effect combo
			$.fn.show.apply(menu, args);

			// select the first option
			// triggering both mouseover and mouseover because 1.4.2+ has a bug where triggering mouseover
			// will actually trigger mouseenter.  the mouseenter trigger is there for when it's eventually fixed
			this.labels.eq(0).trigger('mouseover').trigger('mouseenter').find('input').trigger('focus');

			button.addClass('ui-state-active');
			this._isOpen = true;
			this._trigger('open');
		},

		// close the menu
		close: function () {
			if (this._trigger('beforeclose') === false) {
				return;
			}

			var o = this.options,
					effect = o.hide,
					speed = this.speed,
					args = [];

			// figure out opening effects/speeds
			if ($.isArray(o.hide)) {
				effect = o.hide[0];
				speed = o.hide[1] || this.speed;
			}

			if (effect) {
				args = [effect, speed];
			}

			$.fn.hide.apply(this.menu, args);
			this.button.removeClass('ui-state-active').trigger('blur').trigger('mouseleave');
			this._isOpen = false;
			this._trigger('close');
		},

		enable: function () {
			this._toggleDisabled(false);
		},

		disable: function () {
			this._toggleDisabled(true);
		},

		checkAll: function (e) {
			this._toggleChecked(true);
			this._trigger('checkAll');
		},

		uncheckAll: function () {
			this._toggleChecked(false);
			this._trigger('uncheckAll');
		},

		getChecked: function () {
			return this.menu.find('input').filter(':checked');
		},

		destroy: function () {
			// remove classes + data
			$.Widget.prototype.destroy.call(this);

			this.button.remove();
			this.menu.remove();
			this.element.show();

			return this;
		},

		isOpen: function () {
			return this._isOpen;
		},

		widget: function () {
			return this.menu;
		},

		getButton: function () {
			return this.button;
		},

		// react to option changes after initialization
		_setOption: function (key, value) {
			var menu = this.menu;

			switch (key) {
				case 'header':
					menu.find('div.ui-multiselect-header')[value ? 'show' : 'hide']();
					break;
				case 'checkAllText':
					menu.find('a.ui-multiselect-all span').eq(-1).text(value);
					break;
				case 'uncheckAllText':
					menu.find('a.ui-multiselect-none span').eq(-1).text(value);
					break;
				case 'height':
					menu.find('ul').last().height(parseInt(value, 10));
					break;
				case 'minWidth':
					this.options[key] = parseInt(value, 10);
					this._setButtonWidth();
					this._setMenuWidth();
					break;
				case 'selectedText':
				case 'selectedList':
				case 'noneSelectedText':
					this.options[key] = value; // these all needs to update immediately for the update() call
					this.update();
					break;
				case 'classes':
					menu.add(this.button).removeClass(this.options.classes).addClass(value);
					break;
				case 'multiple':
					menu.toggleClass('ui-multiselect-single', !value);
					this.options.multiple = value;
					this.element[0].multiple = value;
					this.refresh();
			}

			$.Widget.prototype._setOption.apply(this, arguments);
		}
	});

})(jQuery);

//#endregion End MultiSelect

//#region JQCloud

/*!
 * jQCloud Plugin for jQuery
 *
 * Version 1.0.4
 *
 * Copyright 2011, Luca Ongaro
 * Licensed under the MIT license.
 *
 * Date: 2013-05-09 18:54:22 +0200
*/

(function ($) {
	"use strict";
	$.fn.jQCloud = function (word_array, options) {
		// Reference to the container element
		var $this = this;
		// Namespace word ids to avoid collisions between multiple clouds
		var cloud_namespace = $this.attr('id') || Math.floor((Math.random() * 1000000)).toString(36);

		// Default options value
		var default_options = {
			width: $this.width(),
			height: $this.height(),
			center: {
				x: ((options && options.width) ? options.width : $this.width()) / 2.0,
				y: ((options && options.height) ? options.height : $this.height()) / 2.0
			},
			delayedMode: word_array.length > 50,
			shape: false, // It defaults to elliptic shape
			encodeURI: true,
			removeOverflowing: true
		};

		options = $.extend(default_options, options || {});

		// Add the "jqcloud" class to the container for easy CSS styling, set container width/height
		$this.addClass("jqcloud").width(options.width).height(options.height);

		// Container's CSS position cannot be 'static'
		if ($this.css("position") === "static") {
			$this.css("position", "relative");
		}

		var drawWordCloud = function () {
			// Helper function to test if an element overlaps others
			var hitTest = function (elem, other_elems) {
				// Pairwise overlap detection
				var overlapping = function (a, b) {
					if (Math.abs(2.0 * a.offsetLeft + a.offsetWidth - 2.0 * b.offsetLeft - b.offsetWidth) < a.offsetWidth + b.offsetWidth) {
						if (Math.abs(2.0 * a.offsetTop + a.offsetHeight - 2.0 * b.offsetTop - b.offsetHeight) < a.offsetHeight + b.offsetHeight) {
							return true;
						}
					}
					return false;
				};
				var i = 0;
				// Check elements for overlap one by one, stop and return false as soon as an overlap is found
				for (i = 0; i < other_elems.length; i++) {
					if (overlapping(elem, other_elems[i])) {
						return true;
					}
				}
				return false;
			};

			// Make sure every weight is a number before sorting
			for (var i = 0; i < word_array.length; i++) {
				word_array[i].weight = parseFloat(word_array[i].weight, 10);
			}

			// Sort word_array from the word with the highest weight to the one with the lowest
			word_array.sort(function (a, b) { if (a.weight < b.weight) { return 1; } else if (a.weight > b.weight) { return -1; } else { return 0; } });

			var step = (options.shape === "rectangular") ? 18.0 : 2.0,
					already_placed_words = [],
					aspect_ratio = options.width / options.height;

			// Function to draw a word, by moving it in spiral until it finds a suitable empty place. This will be iterated on each word.
			var drawOneWord = function (index, word) {
				// Define the ID attribute of the span that will wrap the word, and the associated jQuery selector string
				var word_id = cloud_namespace + "_word_" + index,
						word_selector = "#" + word_id,
						angle = 6.28 * Math.random(),
						radius = 0.0,

						// Only used if option.shape == 'rectangular'
						steps_in_direction = 0.0,
						quarter_turns = 0.0,

						weight = 5,
						custom_class = "",
						inner_html = "",
						word_span;

				// Extend word html options with defaults
				word.html = $.extend(word.html, { id: word_id });

				// If custom class was specified, put them into a variable and remove it from html attrs, to avoid overwriting classes set by jQCloud
				if (word.html && word.html["class"]) {
					custom_class = word.html["class"];
					delete word.html["class"];
				}

				// Check if min(weight) > max(weight) otherwise use default
				if (word_array[0].weight > word_array[word_array.length - 1].weight) {
					// Linearly map the original weight to a discrete scale from 1 to 10
					weight = Math.round((word.weight - word_array[word_array.length - 1].weight) /
															(word_array[0].weight - word_array[word_array.length - 1].weight) * 9.0) + 1;
				}
				word_span = $('<span>').attr(word.html).addClass('w' + weight + " " + custom_class);

				// Append link if word.url attribute was set
				if (word.link) {
					// If link is a string, then use it as the link href
					if (typeof word.link === "string") {
						word.link = { href: word.link };
					}

					// Extend link html options with defaults
					if (options.encodeURI) {
						word.link = $.extend(word.link, { href: encodeURI(word.link.href).replace(/'/g, "%27") });
					}

					inner_html = $('<a>').attr(word.link).text(word.text);
				} else {
					inner_html = word.text;
				}
				word_span.append(inner_html);

				// Bind handlers to words
				if (!!word.handlers) {
					for (var prop in word.handlers) {
						if (word.handlers.hasOwnProperty(prop) && typeof word.handlers[prop] === 'function') {
							$(word_span).bind(prop, word.handlers[prop]);
						}
					}
				}

				$this.append(word_span);

				var width = word_span.width(),
						height = word_span.height(),
						left = options.center.x - width / 2.0,
						top = options.center.y - height / 2.0;

				// Save a reference to the style property, for better performance
				var word_style = word_span[0].style;
				word_style.position = "absolute";
				word_style.left = left + "px";
				word_style.top = top + "px";

				while (hitTest(word_span[0], already_placed_words)) {
					// option shape is 'rectangular' so move the word in a rectangular spiral
					if (options.shape === "rectangular") {
						steps_in_direction++;
						if (steps_in_direction * step > (1 + Math.floor(quarter_turns / 2.0)) * step * ((quarter_turns % 4 % 2) === 0 ? 1 : aspect_ratio)) {
							steps_in_direction = 0.0;
							quarter_turns++;
						}
						switch (quarter_turns % 4) {
							case 1:
								left += step * aspect_ratio + Math.random() * 2.0;
								break;
							case 2:
								top -= step + Math.random() * 2.0;
								break;
							case 3:
								left -= step * aspect_ratio + Math.random() * 2.0;
								break;
							case 0:
								top += step + Math.random() * 2.0;
								break;
						}
					} else { // Default settings: elliptic spiral shape
						radius += step;
						angle += (index % 2 === 0 ? 1 : -1) * step;

						left = options.center.x - (width / 2.0) + (radius * Math.cos(angle)) * aspect_ratio;
						top = options.center.y + radius * Math.sin(angle) - (height / 2.0);
					}
					word_style.left = left + "px";
					word_style.top = top + "px";
				}

				// Don't render word if part of it would be outside the container
				if (options.removeOverflowing && (left < 0 || top < 0 || (left + width) > options.width || (top + height) > options.height)) {
					word_span.remove()
					return;
				}


				already_placed_words.push(word_span[0]);

				// Invoke callback if existing
				if ($.isFunction(word.afterWordRender)) {
					word.afterWordRender.call(word_span);
				}
			};

			var drawOneWordDelayed = function (index) {
				index = index || 0;
				if (!$this.is(':visible')) { // if not visible then do not attempt to draw
					setTimeout(function () { drawOneWordDelayed(index); }, 10);
					return;
				}
				if (index < word_array.length) {
					drawOneWord(index, word_array[index]);
					setTimeout(function () { drawOneWordDelayed(index + 1); }, 10);
				} else {
					if ($.isFunction(options.afterCloudRender)) {
						options.afterCloudRender.call($this);
					}
				}
			};

			// Iterate drawOneWord on every word. The way the iteration is done depends on the drawing mode (delayedMode is true or false)
			if (options.delayedMode) {
				drawOneWordDelayed();
			}
			else {
				$.each(word_array, drawOneWord);
				if ($.isFunction(options.afterCloudRender)) {
					options.afterCloudRender.call($this);
				}
			}
		};

		// Delay execution so that the browser can render the page before the computatively intensive word cloud drawing
		setTimeout(function () { drawWordCloud(); }, 10);
		return $this;
	};
})(jQuery);

//#endregion End JQCloud

//#region jQuery UI Touch Punch

 /*!
	* jQuery UI Touch Punch 0.2.3
	* http://touchpunch.furf.com
	*
	* Copyright 2011–2014, Dave Furfero
	* Dual licensed under the MIT or GPL Version 2 licenses.
	*
	* Depends:
	*  jquery.ui.widget.js
	*  jquery.ui.mouse.js
	*/
 (function ($) {

 	// Detect touch support
 	$.support.touch = 'ontouchend' in document;

 	// Ignore browsers without touch support
 	if (!$.support.touch) {
 		return;
 	}

 	var mouseProto = $.ui.mouse.prototype,
      _mouseInit = mouseProto._mouseInit,
      _mouseDestroy = mouseProto._mouseDestroy,
      touchHandled;

 	/**
   * Simulate a mouse event based on a corresponding touch event
   * @param {Object} event A touch event
   * @param {String} simulatedType The corresponding mouse event
   */
 	function simulateMouseEvent (event, simulatedType) {

 		// Ignore multi-touch events
 		if (event.originalEvent.touches.length > 1) {
 			return;
 		}

 		event.preventDefault();

 		var touch = event.originalEvent.changedTouches[0],
        simulatedEvent = document.createEvent('MouseEvents');
    
 		// Initialize the simulated mouse event using the touch event's coordinates
 		simulatedEvent.initMouseEvent(
      simulatedType,    // type
      true,             // bubbles                    
      true,             // cancelable                 
      window,           // view                       
      1,                // detail                     
      touch.screenX,    // screenX                    
      touch.screenY,    // screenY                    
      touch.clientX,    // clientX                    
      touch.clientY,    // clientY                    
      false,            // ctrlKey                    
      false,            // altKey                     
      false,            // shiftKey                   
      false,            // metaKey                    
      0,                // button                     
      null              // relatedTarget              
    );

 		// Dispatch the simulated event to the target element
 		event.target.dispatchEvent(simulatedEvent);
 	}

 	/**
   * Handle the jQuery UI widget's touchstart events
   * @param {Object} event The widget element's touchstart event
   */
 	mouseProto._touchStart = function (event) {

 		var self = this;

 		// Ignore the event if another widget is already being handled
 		if (touchHandled || !self._mouseCapture(event.originalEvent.changedTouches[0])) {
 			return;
 		}

 		// Set the flag to prevent other widgets from inheriting the touch event
 		touchHandled = true;

 		// Track movement to determine if interaction was a click
 		self._touchMoved = false;

 		// Simulate the mouseover event
 		simulateMouseEvent(event, 'mouseover');

 		// Simulate the mousemove event
 		simulateMouseEvent(event, 'mousemove');

 		// Simulate the mousedown event
 		simulateMouseEvent(event, 'mousedown');
 	};

 	/**
   * Handle the jQuery UI widget's touchmove events
   * @param {Object} event The document's touchmove event
   */
 	mouseProto._touchMove = function (event) {

 		// Ignore event if not handled
 		if (!touchHandled) {
 			return;
 		}

 		// Interaction was not a click
 		this._touchMoved = true;

 		// Simulate the mousemove event
 		simulateMouseEvent(event, 'mousemove');
 	};

 	/**
   * Handle the jQuery UI widget's touchend events
   * @param {Object} event The document's touchend event
   */
 	mouseProto._touchEnd = function (event) {

 		// Ignore event if not handled
 		if (!touchHandled) {
 			return;
 		}

 		// Simulate the mouseup event
 		simulateMouseEvent(event, 'mouseup');

 		// Simulate the mouseout event
 		simulateMouseEvent(event, 'mouseout');

 		// If the touch interaction did not move, it should trigger a click
 		if (!this._touchMoved) {

 			// Simulate the click event
 			simulateMouseEvent(event, 'click');
 		}

 		// Unset the flag to allow other widgets to inherit the touch event
 		touchHandled = false;
 	};

 	/**
   * A duck punch of the $.ui.mouse _mouseInit method to support touch events.
   * This method extends the widget with bound touch event handlers that
   * translate touch events to mouse events and pass them to the widget's
   * original mouse event handling methods.
   */
 	mouseProto._mouseInit = function () {
    
 		var self = this;

 		// Delegate the touch handlers to the widget's element
 		self.element.bind({
 			touchstart: $.proxy(self, '_touchStart'),
 			touchmove: $.proxy(self, '_touchMove'),
 			touchend: $.proxy(self, '_touchEnd')
 		});

 		// Call the original $.ui.mouse init method
 		_mouseInit.call(self);
 	};

 	/**
   * Remove the touch event handlers
   */
 	mouseProto._mouseDestroy = function () {
    
 		var self = this;

 		// Delegate the touch handlers to the widget's element
 		self.element.unbind({
 			touchstart: $.proxy(self, '_touchStart'),
 			touchmove: $.proxy(self, '_touchMove'),
 			touchend: $.proxy(self, '_touchEnd')
 		});

 		// Call the original $.ui.mouse destroy method
 		_mouseDestroy.call(self);
 	};

 })(jQuery);

//#endregion End jQuery UI Touch Punch

//#region plug-in name goes here

//#endregion End custom plug-in


//#endregion End javascript libraries and jQuery plug-ins