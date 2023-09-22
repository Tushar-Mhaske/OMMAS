$(function () {
    // Wire up the click event of any dialog links
    $('.dialogLink').live('click', function () {
        var element = $(this);

        // Retrieve values from the HTML5 data attributes of the link        
        var dialogTitle = element.attr('data-dialog-title');
        var updateTargetId = '#' + element.attr('data-update-target-id');
        var updateUrl = element.attr('data-update-url');

        // Generate a unique id for the dialog div
        var dialogId = 'uniqueName-' + Math.floor(Math.random() * 1000)
        var dialogDiv = "<div id='" + dialogId + "'></div>";

        // Load the form into the dialog div
        $(dialogDiv).load(this.href, function () {
            $(this).dialog({
                modal: true,
                resizable: false,
                title: dialogTitle,
                buttons: {
                    "Save": function () {
                        // Manually submit the form                        
                        var form = $('form', this);
                        $(form).submit();
                    },
                    "Cancel": function () {
                        $(this).dialog('close');
                    }
                },
                // **** START NEW CODE ****
                close: function () {
                    // Remove all qTip tooltips
                    $('.qtip').remove();

                    // It turns out that closing a jQuery UI dialog
                    // does not actually remove the element from the
                    // page but just hides it. For the server side 
                    // validation tooltips to show up you need to
                    // remove the original form the page
                    $('#' + dialogId).remove();
                }
                // **** END NEW CODE ****
            });

            // Enable client side validation
            $.validator.unobtrusive.parse(this);

            // Setup the ajax submit logic
            wireUpForm(this, updateTargetId, updateUrl);
        });
        return false;
    });
});