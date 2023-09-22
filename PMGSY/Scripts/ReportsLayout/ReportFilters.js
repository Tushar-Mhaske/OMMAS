/*
    * Project Id    :
    * Project Name  :   OMMAS II
    * Name          :   ReportFilters.js
    * Description   :   Manage Common Filters for all Reports
    * Author        :   Shyam Yadav
    * Creation Date :   26/August/2013    
*/

$(document).ready(function () {

    isrptFiltersCollapse = false;


    $("#tabs-report-filters").tabs();
    $('#tabs-report-filters ul').removeClass('ui-widget-header');

    $("#idFilterDiv").click(function () {
        $("#idFilterDiv").toggleClass("ui-icon-carat-1-e").toggleClass("ui-icon-circle-triangle-s");
        resizeReportTable();
    });

    $("#btnViewReportDetails").click(function () {
        if (gblCurrentParentName == ""  &&  gblCurrentMenuName == "")
        {
            alert("Please click on Menu in Report Menu List.");
            return;
        }

        redirectToCurrentAction(gblCurrentParentName, gblCurrentMenuName);

    });
   

    //------------------------ Dropdown Onchange Events ---------------------------//

    $("#ddlFormReportsStates").change(function () {
        populateDistricts("ddlFormReportsDistricts", "ddlFormReportsStates", false);
    });


    $("#ddlFormReportsDistricts").change(function () {
        populateBlocks("ddlFormReportsBlocks", "ddlFormReportsDistricts", false);
    });

    //------------------------ Dropdown Onchange Events ---------------------------//



});


function resizeReportTable()
{

    if ($("#divFilterForm").is(":visible")) {
        
        $('#tblRptContents').css('height', '95%');
        $('#tdMenus').css('height', '95%');
        $('#dvReportsMenu').css('height', '95%');
        $('#dvLoadReport').css('height', '100%');
        $('#tabs-report-content').css('height', '100%');
        $('#tdViewMenu').css('height', '100%');
        $("#divFilterForm").hide("slow");

        isrptFiltersCollapse = true;
    }
    else {
        
        $('#tblRptContents').css('height', '86%');
        $('#tdMenus').css('height', '86%');
        $('#dvReportsMenu').css('height', '90%');
        $('#dvLoadReport').css('height', '100%');
        $('#tabs-report-content').css('height', '100%');
        $('#tdViewMenu').css('height', '100%');
        $("#divFilterForm").show("fast");

        isrptFiltersCollapse = false;
    }

    $("#tblRptContents").trigger('resize');
}






//as per module selected by user, load appropriate report by redirecting to appropriate action
function redirectToCurrentAction(currentParentName, currentMenuName)
{
    var url = "";
    var params = "";
    //alert(currentParentName);
    switch (currentParentName) {
        case 'Form':
            params = currentMenuName + "$" + $("#ddlFormReportsStates").val() + "$" + $("#ddlFormReportsDistricts").val() + "$" + $("#ddlFormReportsBlocks").val();
            url = "/FormReports/FormReportsBaseAction/" + params;
            break;
        case 'Proposal':
            params = currentMenuName + "$" + $("#ddlProposalStates").val() + "$" + $("#ddlProposalDistricts").val() + "$" + $("#ddlProposalBlocks").val() + "$" + $("#ddlProposalYears").val() + "$" + $("#ddlProposalMonths").val() + "$" + $("#ddlProposalBatch").val() + "$" + $("#ddlProposalStreams").val() + "$" + $("#ddlProposalTypes").val();
            url = "/ProposalReports/ProposalReportsBaseAction/" + params;
            break;
        case 'Quality':
           
            break;
        case 'Accounts':
            
            break;
        default:
            break;
    }


    ////Hide Menu Panel, & then Load Report
    //hideMenuPanel();
    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    //$('#dvLoadReport').load(url, function (e) {
    //    $.unblockUI();
    //});

    //tabs generation request
    $("#tabs-report-content").show();
    id = "dvLoadReport-" + tabCounter;
    tabTitle = currentMenuName;
    $("#tab_title").val(currentMenuName);
    addTab();
  
    $('#' + id).load(url, function (e) {
        $.unblockUI();
    });

   
}




function populateDistricts(ddlToBePopulated, ddlCurrent, isAllSelected)
{
    $("#" + ddlToBePopulated).empty();

    if ($("#" + ddlCurrent).val() == 0)
    {
        if (isAllSelected)
        {
            $("#" + ddlToBePopulated).append("<option value='0'>All Districts</option>");
        }
        else
        {
            $("#" + ddlToBePopulated).append("<option value='0'>Select District</option>");
        }
    }

    if ($("#" + ddlCurrent).val() > 0) {

        if ($("#" + ddlToBePopulated).length > 0) {

            $.ajax({
                url: '/ReportsLayout/PopulateDistricts',
                type: 'POST',
                data: { selectedValue: $("#" + ddlCurrent).val(), isAllSelected: isAllSelected, value: Math.random() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#" + ddlToBePopulated).append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
}



function populateBlocks(ddlToBePopulated, ddlCurrent, isAllSelected) {

    $("#" + ddlToBePopulated).empty();

    if ($("#" + ddlCurrent).val() == 0) {
        if (isAllSelected) {
            $("#" + ddlToBePopulated).append("<option value='0'>All Blocks</option>");
        }
        else {
            $("#" + ddlToBePopulated).append("<option value='0'>Select Block</option>");
        }
    }

    if ($("#" + ddlCurrent).val() > 0) {

        if ($("#" + ddlToBePopulated).length > 0) {

            $.ajax({
                url: '/ReportsLayout/PopulateBlocks',
                type: 'POST',
                data: { selectedValue: $("#" + ddlCurrent).val(), isAllSelected: isAllSelected, value: Math.random() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#" + ddlToBePopulated).append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
}




