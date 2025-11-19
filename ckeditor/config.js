/**
 * @license Copyright (c) 2003-2016, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here. For example:
    // config.language = 'fr';
    // config.uiColor = '#AADC6E';
    config.contentsLangDirection = 'rtl';
    config.language = 'en';
    config.uiColor = '#d3d3d3';
    config.contentsCss = 'ckeditor/css/maincss.css';

    //the next line add the new font to the combobox in CKEditor
    //config.font_names = 'B Nazanin/BNazanin;' + 'B Yekan/BYekan;' + 'B Mitra/BMitra;' + 'IRAN Sans/IRANSans;' + 'IRAN Sans Bold/IRANSansBold;' + 'Gandom/Gandom;' + 'B Koodak Bold/BKoodak;' + 'B Yekan/BYekan;' + config.font_names;
    config.font_names = 'Vazir/Vazir;' + config.font_names;

    //*********************************************************************************
    //Set class for Body Tag (this class must define in 'ckeditor/css/maincss.css')
    config.bodyClass = "textBody";
    //*********************************************************************************

    //*********************************************************************************
    //Add Style to 'Format' Element Tollbar (منوی قالب در نوار ابزار) for HTML Tag
    // keyword 'style' for Style Css
    // Keyword 'attributes' add Property to HTML Tag
    //config.format_p = { element: "p", styles: { 'color': 'Blue','text-algin':'left' }, attributes: { 'class': 'test','id':'test' }};
    //*********************************************************************************
    //Allow SCRIPT Content Added to CKEditor
    config.allowedContent = {
        script: true,
        $1: {
            // This will set the default set of elements
            elements: CKEDITOR.dtd,
            attributes: true,
            styles: true,
            classes: true
        }
    };
    //config.allowedContent = true;
    //config.extraAllowedContent = 'p(class)';
    //allowedContent: 'p h1{text-align}; a[!href]; strong em; p(tip)';
    //allowedContent: 'p{text-align}(tip)';


    //config.toolbar = 'Full';
    //    config.toolbar_Basic =
    //[
    //	['Bold', 'Italic', '-', 'NumberedList', 'BulletedList', '-', 'Link', 'Unlink', '-', 'About']
    //];
    config.toolbar =
[
	{ name: 'document', items: ['Source', '-', 'Save', 'NewPage', 'DocProps', 'Preview', 'Print', '-', 'Templates'] },
    //'/',
	{ name: 'clipboard', items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
	{ name: 'editing', items: ['Find', 'Replace', '-', 'SelectAll', '-', 'SpellChecker', 'Scayt'] },
	//{ name: 'forms', items: ['Form', 'Checkbox', 'Radio', 'TextField', 'Textarea', 'Select', 'Button', 'ImageButton','HiddenField']},
	//'/',
	{ name: 'basicstyles', items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat'] },
	{
	    name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', 'CreateDiv',
        '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-', 'BidiLtr', 'BidiRtl']
	},
    { name: 'links', items: ['Link', 'Unlink'] },
	//{ name: 'links', items: ['Link', 'Unlink', 'Anchor'] },
	//{ name: 'insert', items: ['Image', 'Flash', 'Table', 'HorizontalRule', 'Smiley', 'SpecialChar', 'PageBreak', 'Iframe'] },
	'/',
	{ name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] },
	{ name: 'colors', items: ['TextColor', 'BGColor'] },
	{ name: 'tools', items: ['Maximize', 'ShowBlocks', '-', 'About'] }
];
};
