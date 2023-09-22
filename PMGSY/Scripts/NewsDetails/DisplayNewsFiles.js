//$.noConflict();
$(document).ready(function () {

    //Execute the slideShow
    //slideShow();

    //$(function () {
    //    setInterval("slideSwitch()", 500);
    //});


    // Image swap on hover
    //$("#gallery li img").hover(function () {
    //    $('#main-img').attr('src', $(this).attr('src').replace('thumb/', ''));
    //});
    //// Image preload
    //var imgSwap = [];
    //$("#gallery li img").each(function () {
    //    imgUrl = this.src.replace('thumb/', '');
    //    imgSwap.push(imgUrl);
    //});
    //$(imgSwap).preload();


    //s();

    $('.slideshow').cycle({
        fx: 'fade',
        pause: 1,
        prev: '#prev',
        next: '#next'

    });
});

//function slideSwitch() {
//    var $active = $('div#slideshow IMG.active');
//    var $next = $active.next();

//    $next.addClass('active');

//    $active.removeClass('active');
//}



//function slideShow() {
//    //Set the opacity of all images to 0
//    $('#fullgallery a').css({ opacity: 0.0 });
//    //Get the first image and display it (set it to full opacity)
//    $('#fullgallery a:first').css({ opacity: 1.0 });
//    //Set the caption background to semi-transparent
//    $('#fullgallery .caption').css({ opacity: 0.7 });
//    //Resize the width of the caption according to the image width
//    $('#fullgallery .caption').css({ width: $('#fullgallery a').find('img').css('width') });
//    //Get the caption of the first image from REL attribute and display it
//    $('#fullgallery .content').html($('#fullgallery a:first').find('img').attr('rel')).animate({ opacity: 0.7 }, 400);
//    //Call the gallery function to run the slideshow, 6000 = change to next image after 6 seconds
//    setInterval('myGallery()', 6000);
//}

//function myGallery() {
//    //if no IMGs have the show class, grab the first image
//    var current = ($('#fullgallery a.show') ? $('#fullgallery a.show') : $('#fullgallery a:first'));
//    //Get next image, if it reached the end of the slideshow, rotate it back to the first image
//    var next = ((current.next().length) ? ((current.next().hasClass('caption')) ? $('#fullgallery a:first') : current.next()) : $('#fullgallery a:first'));
//    //Get next image caption
//    var caption = next.find('img').attr('rel');
//    //Set the fade in effect for the next image, show class has higher z-index
//    next.css({ opacity: 0.0 })
//    .addClass('show')
//    .animate({ opacity: 1.0 }, 1000);
//    //Hide the current image
//    current.animate({ opacity: 0.0 }, 1000).removeClass('show');
//    //Set the opacity to 0 and height to 1px
//    $('#fullgallery .caption').animate({ opacity: 0.0 }/* { queue:false, duration:0 }).animate({height: '0px'}, { queue:true, duration:300 }*/);
//    //Animate the caption, opacity to 0.7 and heigth to 100px, a slide up effect
//    $('#fullgallery .caption').animate({ opacity: 0.7 }, 100)/*.animate({height: '100px'},500 )*/;
//    //Display the content
//    $('#fullgallery .content').html(caption);
//}


//$.fn.preload = function () {
//    this.each(function () {
//        $('<img/>')[0].src = this;
//    });
//}


//function s() {
//    $('#slides').slidesjs({
//        width: 940,
//        height: 528,
//        navigation: {
//            effect: "fade"
//        },
//        pagination: {
//            effect: "fade"
//        },
//        effect: {
//            fade: {
//                speed: 400
//            }
//        }
//    });
//};


