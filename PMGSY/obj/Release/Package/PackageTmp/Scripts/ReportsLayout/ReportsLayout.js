$(document).ready(function () {
    
    //Added By Abhishek kamble 6-May-2014 start
    $(function () {
        $.ajax({
            type: 'POST',
            url: '/Master/GetPMGSY2Status?id=' + $("#PMGSY2StateCode").val(),
            async: false,
            cache: false,
            success: function (data) {
                //alert(data.success);
                if (data.success == true) {
                    $("#spnPMGSY2").show();
                }
                else {
                    $("#spnPMGSY2").hide();
                }
            },
            error: function () {
                alert("Request can not be processed at this time.");
            }
        });

    });
    //Added By Abhishek kamble 6-May-2014 end

    isRptMenuCollapse = false;
    gblCurrentParentName = "";
    gblCurrentMenuName = "";
    
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#accordion").accordion({
        fillSpace: true,
        icons: false
    });

    var activatedAccordianIndex = 0;

    $('#spCollapseIconLeft').click(function () {

        if ($("#tdMenues").is(":visible")) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $('#tdMenues').hide('left');
            $('#tdMenues').css('width', '250px');
            $('#tdViewMenu').show('slow');
            activatedAccordianIndex = $("#accordion").accordion('option', 'active');
            isRptMenuCollapse = true;
            $('#tblRptContents').trigger('resize');

            $.unblockUI();
        }

    });

    $('#spCollapseIconRight').click(function () {
        if (!($("#tdMenues").is(":visible"))) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $('#tdViewMenu').hide();
            $('#tdMenues').show('right');
            $("#accordion").accordion('activate', activatedAccordianIndex);
            isRptMenuCollapse = false;
            $('#tblRptContents').trigger('resize');

            $.unblockUI();
        }
    });

    $.unblockUI();
  
    //Manage Content Tabs for Reports
    //----------------------------------------------------------------------//
        manageTabsReportContent();
    //----------------------------------------------------------------------//

});



//Hide Reports Menu Panel
function hideMenuPanel()
{
    if ($("#tdMenues").is(":visible")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $('#tdMenues').hide('left');
        $('#tdViewMenu').show('slow');
        activatedAccordianIndex = $("#accordion").accordion('option', 'active');
        isRptMenuCollapse = true;
        $('#tblRptContents').trigger('resize');

        $.unblockUI();
    }
}


//to set current menu name to global menu name
function setMenuName(label) {
    gblCurrentMenuName = label;
}


//Resize grid to fit to Parent Width & Height
//If Left Pane of Report Menus is Collapsed then Increase Size else Keep as Original
function resizeJqGrid() {
   
    //For Each of the Visible Grids, resizes grid
    if (grid = $('.ui-jqgrid-btable:visible')) {

        grid.each(function (index) {
            
            grid_id = $(this).attr('id');
            setWidthToJqGrid(grid_id);
            //countOfVisibleGrids++;
            //if (index == 0)
            //{
            //    visibleGridId = grid_id;
            //}
        });

        //if (countOfVisibleGrids == 1)
        //{
        //    setHeightToJqGrid(visibleGridId);
        //}
    }

    //For Each of the Hidden Grids, resizes grid
    if (grid = $('.ui-jqgrid-btable:hidden')) {
        grid.each(function (index) {
            
            grid_id = $(this).attr('id');
            setWidthToJqGrid(grid_id);

            //setHeightToOriginal(grid_id);
            
        });

        

        
    }

}

//function setHeightToJqGrid(grid_id)
//{
//     $('#' + grid_id).setGridHeight($('#tblRptContents').height() - 200, true); //Resized to new Height
//}

//function setHeightToOriginal(grid_id)
//{
//    $('#' + grid_id).setGridHeight( 240, true); //Resized to original Height
//}


function setWidthToJqGrid(grid_id)
{
    //Width Changes
    if (isRptMenuCollapse)
    {
        if ($('#tblRptContents').width() > 1199) {
            $('#' + grid_id).setGridWidth(1100, true); //Resized to new width as per window
        }
        else
        {
            $('#' + grid_id).setGridWidth($('#tblRptContents').width() - 85, true); //Resized to new width as per window
        }
    }
    else
    {
        $('#' + grid_id).setGridWidth($('#tblRptContents').width() - 300, true); //Back to original width
    }
}





// actual addTab function: adds new tab using the input from the form above
function addTab() {

    //if already opened tab for any report, then prevent to open it again
    //Compare title of currently requested Tab & already loaded Tabs
    var ul = tabs.find("ul");
    var flag = true;
    var index = 0;
    $(ul).find('li').each(function () {
        if ($(this).text().trim() == tabTitle.trim()) {
            // to prevent from creating a new tab under ul
            flag = false;
            index = $(this).index();
        }
    });

    if (flag) {
        var label = tabTitle,
        id = "dvLoadReport-" + tabCounter,
        li = $(tabTemplate.replace(/#\{href\}/g, "#" + id).replace(/#\{label\}/g, label));  //,
        tabs.find(".ui-tabs-nav").append(li);
        tabs.append("<div id='" + id + "'></div>");
        tabs.tabs("refresh");
        tabCounter++;
        $("#tab_counter").val(tabCounter);  //set current counter value to tab_counter
        $("#tabs-report-content ul").removeClass('ui-widget-header');   //remove tab header css for removing color 
        tabs.tabs('select', '#' + id);  //set active to currently opened tab
    }
    else
    {
        //set tab as active
        tabs.tabs("select", index-1);
        $.unblockUI();
    }

}



//manage dynamically created tabs 
function manageTabsReportContent()
{
    tabTitle = $("#tab_title").val(),
    tabTemplate = "<li><a href='#{href}' onclick='setMenuName($(this).text());'>#{label}</a> <span class='ui-icon ui-icon-close' role='presentation'></span></li>",
    tabCounter = $("#tab_counter").val();

    tabs = $("#tabs-report-content").tabs();
    $("#tabs-report-content ul").removeClass('ui-widget-header');


    // close icon: removing the tab on click
    tabs.delegate("span.ui-icon-close", "click", function () {
        var panelId = $(this).closest("li").remove().attr("aria-controls");
        $("#" + panelId).remove();
        tabs.tabs("refresh");
        $("#tabs-report-content ul").removeClass('ui-widget-header');

    });

    tabs.bind("keyup", function (event) {
        if (event.altKey && event.keyCode === $.ui.keyCode.BACKSPACE) {
            var panelId = tabs.find(".ui-tabs-active").remove().attr("aria-controls");
            $("#" + panelId).remove();
            tabs.tabs("refresh");
            $("#tabs-report-content ul").removeClass('ui-widget-header');
        }
    });
}


//adds new tab with response data in provided URL
function loadReportInTab(url)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    //tabs generation request
    $("#tabs-report-content").show();
    id = "dvLoadReport-" + tabCounter;
    tabTitle = gblCurrentMenuName;
    $("#tab_title").val(gblCurrentMenuName);
    addTab();

    $('#' + id).load(url, function (e) {
        $.unblockUI();
    });
}



//// close tab with a given name
function removeTabs() {

    //$("#tabs-report-content").tabs('destroy');
    //$("#tabs-report-content").html('');

    //var tab = $('#tabs-report-content a').filter(function () {
    //    return $(this).text() == name;
    //}).parent();

    //var index = $("li", $tabs).index(tab);
    //if (index >= 0) {
    //    $tabs.tabs("remove", index);
    //}
}






