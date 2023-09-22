//This js file contains function for custom alert, dialog, Confirm and Block page control
// dependent on jquery.qtip.js


// /////////////////////////////qtip functions may be used for displaying tooltip alerts etc//////////////////////////////////////////////////////

$(document).ready(function () { 




    //apply custom tooltip to title
    $('[title]').qtip({
        content: {

            /* title: {
                 text: 'PMGSY',
                 button: 'Close'
             }*/
        },
        position: {
            at: 'bottom center', // Position the tooltip above the link
            my: 'top center',
            viewport: $(window), // Keep the tooltip on-screen at all times
            effect: false // Disable positioning animation
        },
        style: {
            classes: 'qtip-plan', // Optional shadow...
            widget: true //themeroller
        }
    })

});//doc.ready ends 




//function for blockong the page
function blockPage() {

    var message = '<img src="/Content/images/ajax-loader.gif"/>';
    blockPageDialog(message);
    return false;

}


//function to unblockpage
function unblockPage() {

    $('#mainbody').qtip('destroy');

}

//function to display dialogue
function dialogue(content, title) {
    /* 
    * Since the dialogue isn't really a tooltip as such, we'll use a dummy
    * out-of-DOM element as our target instead of an actual element like document.body
    */
    $('<div />').qtip(
		        {
		            content: {
		                text: content
		               , title: {
		                   text: 'PMGSY ',
		                   button: 'Close'
		               }
		            },
		            position: {
		                my: 'center', at: 'center', // Center it...
		                target: $(window) // ... in the window
		            },
		            show: {
		                ready: true, // Show it straight away
		                modal: {
		                    on: true, // Make it modal (darken the rest of the page)...
		                    blur: false, // ... but don't close the tooltip when clicked
		                    escape: false
		                }
		            },
		            hide: false, // We'll hide it maunally so disable hide events

		            style: {
		                classes: 'qtip-shadow qtip-rounded qtip-dialogue', // Optional shadow...
		                widget: true //themeroller
		            },

		            events: {
		                // Hide the tooltip when any buttons in the dialogue are clicked
		                render: function (event, api) {
		                    $('button', api.elements.content).click(api.hide);
		                },
		                // Destroy the tooltip once it's hidden as we no longer need it!
		                hide: function (event, api) { api.destroy(); }
		            }
		        });
}




// Our Alert method
function Alert(message) {
    // Content will consist of the message and an ok button
    var message = $('<p />', { text: message }),
	ok = $('<button />', { text: 'Ok', 'class': 'full' });
    dialogue(message.add(ok), 'Alert!');
}


// Our Prompt method
function Prompt(question, initial, callback) {
    // Content will consist of a question elem and input, with ok/cancel buttons
    var message = $('<p />', { text: question }),
			input = $('<input  />', { val: initial }),
			    ok = $('<button />', {
			        text: 'Ok',
			        click: function () { callback(input.val()); }
			    }),
			cancel = $('<button />', {
			    text: 'Cancel',
			    click: function () { callback(null); }
			});

    dialogue(message.add(input).add(ok).add(cancel), 'Attention!');
}

// Our Confirm method
function Confirm(question, callback) {
    // Content will consist of the question and ok/cancel buttons
    var message = $('<p />', { text: question }),
			   ok = $('<button />', {
			       text: 'Yes',
			       click: function () { callback(true); }
			   }),
			cancel = $('<button />', {
			    text: 'No',
			    click: function () { callback(false); }
			});

    dialogue(message.add(ok).add(cancel), ' ');
}


//function to block page
function blockPageDialog(content, title) {

    $('#mainbody').qtip(
		        {
		            content: {
		                text: '<img src="/Content/images/ajax-loader.gif"/>'

		            },
		            position: {
		                my: 'center', at: 'center', // Center it...
		                target: $(window) // ... in the window
		            },
		            show: {
		                ready: true, // Show it straight away
		                modal: {
		                    on: true, // Make it modal (darken the rest of the page)...
		                    blur: false, // ... but don't close the tooltip when clicked
		                    escape: false
		                }
		            },
		            hide: true, // We'll hide it maunally so disable hide events

		            style: {
		                classes: 'qtip-shadow qtip-rounded qtip-dialogue', // Optional shadow...
		                widget: true //themeroller
		            },

		            events: {
		                // Hide the tooltip when any buttons in the dialogue are clicked
		                render: function (event, api) {
		                    // $('button', api.elements.content).click(api.hide);

		                }
		                // Destroy the tooltip once it's hidden as we no longer need it!
		                , hide: function (event, api) { api.destroy(); }
		            }
		        });
}


///////////////////////////////////////////////////////////////////////end///////////////////////////////////////////////////////////////////