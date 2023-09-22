$(document).ready(function () {
    $(".jqueryTable").addClass("ui-jqgrid ui-widget ui-widget-content ui-corner-all");
    $(".jqueryTable").css("width", "958px");
    $(".jqueryTable").find("div:eq(0)").addClass("ui-jqgrid-titlebar ui-widget-header ui-corner-top ui-helper-clearfix");
    $(".jqueryTable").find("span:eq(0)").addClass("ui-jqgrid-title");
    $(".jqueryTable").find("div:eq(1)").addClass("ui-state-default ui-jqgrid-hdiv");
    $(".jqueryTable").find("table:eq(0)").addClass("ui-jqgrid-htable");
    $(".jqueryTable").find("table:eq(0)").attr("cellspacing", "0");
    $(".jqueryTable").find("table:eq(0)").attr("cellpadding", "0");
    $(".jqueryTable").find("table:eq(0)").attr("border", "0");
    $(".jqueryTable").find("table:eq(0)").css("width", "958px");
    $(".jqueryTable").find("table:eq(0) tr").addClass("ui-jqgrid-labels");
    $(".jqueryTable").find("table:eq(0) th").addClass("ui-state-default jqgrow ui-row-ltr");
    $(".jqueryTable").find("table:eq(0) th").css("border-bottom", "1px");
    $(".jqueryTable").find("table:eq(0) th").css("border-left", "1px");
    $(".jqueryTable").find("div:eq(2)").addClass("ui-jqgrid-bdiv");
    $(".jqueryTable").find("table:eq(1)").addClass("ui-jqgrid-btable");
    $(".jqueryTable").find("table:eq(1)").attr("cellspacing", "0");
    $(".jqueryTable").find("table:eq(1)").attr("cellpadding", "0");
    $(".jqueryTable").find("table:eq(1)").attr("border", "0");
    $(".jqueryTable").find("table:eq(1)").css("width", "958px");
    $(".jqueryTable").find("table:eq(1) tr").addClass("ui-widget-content jqgrow ui-row-ltr");
    $(".jqueryTable").find("table:eq(1) tr:even").addClass("ui-widget-content jqgrow ui-row-ltr ui-state-hover");
    //$(".jqueryTable").find("table:eq(1) tr:even").css("background", "#fdf5ce url(../images/ui-bg_glass_100_fdf5ce_1x400.png) 50% 50% repeat-x");
    
    //dataentry
    if ($(".jqueryTable").find("table:eq(1)").hasClass("dataentry")) {
        $(".jqueryTable").find("table:eq(1) td:even").css("text-align", "right");
        $(".jqueryTable").find("table:eq(1) td:odd").css("text-align", "left"); 
    }
    else {
        $(".jqueryTable").find("table:eq(1) td").css("text-align", "center");
    }
    
    $(".jqueryTable").find("div:eq(0)").append("<a class='ui-jqgrid-titlebar-close HeaderButton' href='javascript:void(0)' style='right: 0px;'> <span class='ui-icon ui-icon-circle-triangle-n'></span></a>");
    //$(".jqueryTable").children("*").css("width", "1050px");

    $(".jqueryTable").find("a:eq(0)").mouseover(function () {
        $(this).addClass("ui-state-hover");
    });
    $(".jqueryTable").find("a:eq(0)").mouseout(function () {
        $(this).removeClass("ui-state-hover");
    });

    $(".jqueryTable").find("span:eq(1)").click(function () {

        if ($(this).hasClass("ui-icon-circle-triangle-n")) {
            $("#tableHeader").hide('slow');
            $("#tableContent").hide('slow');
            $(".jqueryTable").find("span:eq(1)").removeClass("ui-icon-circle-triangle-n");
            $(".jqueryTable").find("span:eq(1)").addClass("ui-icon-circle-triangle-s");
        }
        else {
            $("#tableHeader").show('slow');
            $("#tableContent").show('slow');
            $(".jqueryTable").find("span:eq(1)").removeClass("ui-icon-circle-triangle-s");
            $(".jqueryTable").find("span:eq(1)").addClass("ui-icon-circle-triangle-n");
        }
    });   

    /* PMGSY Button */
    $(".jqueryButton").addClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only").css("font-size", "1em");
    //$(".jqueryButton").css('height', '25px');
    $(".jqueryButton").mouseover(function () {
        $(this).addClass("ui-state-hover");
    });
    $(".jqueryButton").mouseout(function () {
        $(this).removeClass("ui-state-hover");
    });
    /* ----------- */

    $("input:text").addClass("pmgsy-textbox");
    

    $(".rowstyle").find('tr:even').addClass('ui-state-hover');
    $(".rowstyle").find('tr:even').css('font-weight', 'normal');
    //$(".rowstyle").find('tr:odd').addClass('ui-widget-content');

    $(".dataentry").find('td:even').css('text-align', 'right');
    $(".dataentry").find('td:odd').css('text-align', 'left');

    $(".table-header").find("div:eq(0)").append("<a class='ui-jqgrid-titlebar-close HeaderButton' href='javascript:void(0)' style='right: 0px; float:right'> <span class='ui-icon ui-icon-circle-triangle-n'></span></a>");

    $(".table-header").find("span:eq(1)").click(function () {

        if ($(this).hasClass("ui-icon-circle-triangle-n")) {
            //$(".table-content").attr('id');
            $(this).parent().parent().parent().next().hide('slow');
            $(".table-header").find("span:eq(1)").removeClass("ui-icon-circle-triangle-n");
            $(".table-header").find("span:eq(1)").addClass("ui-icon-circle-triangle-s");
        }
        else {
            $(this).parent().parent().parent().next().show('slow');
            //$("#tableContent").show('slow');
            $(".table-header").find("span:eq(1)").removeClass("ui-icon-circle-triangle-s");
            $(".table-header").find("span:eq(1)").addClass("ui-icon-circle-triangle-n");
        }
    });
});

function validateDate(strDate) {
    var regExDate = /^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$/;
    return regExDate.test(strDate);
}


//function scrollMainDIv()
//{
//    // the element inside of which we want to scroll
//    var $elem = $('#mainDiv');
 
//    //// show the buttons
//    //$('#nav_up').fadeIn('slow');
//    //$('#nav_down').fadeIn('slow'); 
 
//    //// whenever we scroll fade out both buttons
//    //$(window).bind('scrollstart', function(){
//    //    $('#nav_up,#nav_down').stop().animate({'opacity':'0.2'});
//    //});
//    //// ... and whenever we stop scrolling fade in both buttons
//    //$(window).bind('scrollstop', function(){
//    //    $('#nav_up,#nav_down').stop().animate({'opacity':'1'});
//    //});
 
//    // clicking the "down" button will make the page scroll to the $elem's height
//    //$('#nav_down').click(
//        //function (e) {
//        //    $('html, body').animate({scrollTop: $elem.height()}, 800);
//       // }
//    //);
//    // clicking the "up" button will make the page scroll to the top of the page
//   // $('#nav_up').click(
//        //function (e) {
//            $('html, body').animate({scrollTop: '0px'}, 800);
//        //}
//    //);
//}