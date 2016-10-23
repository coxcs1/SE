/*
Copyright (c) 2003-2012, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.editorConfig = function( config )
{
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
	// config.uiColor = '#AADC6E';
    config.skin = 'kama';
    config.toolbar = [
    ['Cut', 'Copy', 'Paste', 'PasteText', 'Undo', 'Redo',
     'Bold', 'Italic', 'Underline', 'BulletedList', 'NumberedList',
     'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'Link', 'Unlnik',
     'TextColor']
    ];

    config.width = '66%';

};
