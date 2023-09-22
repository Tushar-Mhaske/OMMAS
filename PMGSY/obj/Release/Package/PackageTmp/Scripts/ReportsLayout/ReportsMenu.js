/*
    * Project Id    :
    * Project Name  :   OMMAS II
    * Name          :   ReportMenus.js
    * Description   :   Manage Menus for all Reports
    * Author        :   Shyam Yadav
    * Creation Date :   26/August/2013    
*/


$(document).ready(function () {

    $("#accordionMenu").accordion({
        heightStyle: "content"
    });

    $("#tabs-report-filters").tabs({ disabled: [1, 2, 3] });
    $("#tabs-report-filters").tabs({ active: 0 });

    $('.reports-link a').click(function (e) {
        e.preventDefault();

        //Hide Menu Panel, & then Load Report
        hideMenuPanel();

        var url = $(this).attr('href');
        var menuName = $(this).attr('id');
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        //tabs generation request
        $("#tabs-report-content").show();
        id = "dvLoadReport-" + tabCounter;
        tabTitle = menuName;
        $("#tab_title").val(menuName);
        
        addTab();
 
        $('#' + id).load(url, function (e) {
            $.unblockUI();
        });

    });

});


function funManageFilters(rptMenuName)
{
    //$('#dvLoadReport').html('');
    //removeTabs();
    gblCurrentParentName = "";
    gblCurrentMenuName = "";
    // 0 - Form, 
    // 1 - Proposal
    // 2 - Quality
    // 3 - Accounts
    switch (rptMenuName) {
        case 'Form':
            $("#tabs-report-filters").tabs({ disabled: [1, 2, 3] });
            $("#tabs-report-filters").tabs({ active: 0 });
            break;
        case 'Proposal':
            $("#tabs-report-filters").tabs({ disabled: [0, 2, 3] });
            $("#tabs-report-filters").tabs({ active: 1 });
            break;
        case 'Quality':
            $("#tabs-report-filters").tabs({ disabled: [0, 1, 3] });
            $("#tabs-report-filters").tabs({ active: 2 });
            break;
        case 'Accounts':
            $("#tabs-report-filters").tabs({ disabled: [0, 1, 2] });
            $("#tabs-report-filters").tabs({ active: 3 });
            break;
        default:
            $("#tabs-report-filters").tabs({ disabled: [1, 2, 3] });
            $("#tabs-report-filters").tabs({ active: 0 });
            break;
    }
}



function funSetCurrentURL(currentParentName, currentMenuName)
{
    gblCurrentParentName = currentParentName;
    gblCurrentMenuName = currentMenuName;
}


