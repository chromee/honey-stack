mergeInto(LibraryManager.library, {
	OpenNewTab: function(urlPtr) {
		var url = Pointer_stringify(urlPtr);
		window.open(url, 'newtab');
	}

});
