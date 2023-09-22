/*--------------------------------------------------------------------------------------------
File Name:ProposalMaintainanceInspection.js
Path: ~PMGSY/Scripts/ProposalMaintainanceInspection
Created By: Koustubh Nakate
Modified By:Ashish Markande
Modification Date: 27/07/2013
Purpose: To load Proposal road list and search form.
----------------------------------------------------------------------------------------------
*/




$(document).ready(function () {

    LoadCompletedRoads();
  
    $("#spCollapseIconS").click(function () {

        if ($("#dvSearchParameter").is(":visible")) {

            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
         
            $("#dvSearchParameter").slideToggle(300);
        }

        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
       
            $("#dvSearchParameter").slideToggle(300);
        }
    });

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordionFinance").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordionImage").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $('#btnSearch').click(function (e) {
        SearchDetails();
    });

    $('#btnSearch').trigger('click');

    $("#ddlFinancialYears").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlFinancialYears").find(":selected").val() },
                    "#ddlPackages", "/Agreement/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlFinancialYears option:selected').val() + "&blockCode=" + $('#ddlBlocks option:selected').val());



    }); //end function block change

    $("#ddlBlocks").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlBlocks").find(":selected").val() },
                    "#ddlPackages", "/Agreement/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlFinancialYears option:selected').val() + "&blockCode=" + $('#ddlBlocks option:selected').val());



    }); //end function block change

});


function LoadCompletedRoads() {

    jQuery("#tbProposedRoadList").jqGrid({
        url: '/MaintainanceInspection/GetCompletedRoadList/',
        datatype: "json",
        postData: { sanctionedYear: $('#ddlFinancialYears option:selected').val(), blockCode: $('#ddlBlocks option:selected').val() },
        mtype: "POST",
        colNames: ['Block', 'Year', 'Batch', 'Package', 'Work Name', 'Road Length', 'Collaboration', 'Sanctioned Cost', 'Maintenance Cost', 'Inspection', 'Financial Progress', 'Tree Plant', 'Images Upload','Image Upload'],
        colModel: [
                            { name: 'Block', index: 'Block', width: 10, sortable: true, align: "center" },
                            { name: 'SanctionedYear', index: 'SanctionedYear', height: 'auto', width: 10, sortable: true, align: "center" },
                            { name: 'Batch', index: 'Batch', width: 10, sortable: true, align: "center" },
                            { name: 'Package', index: 'Package', width: 10, sortable: true, align: "center" },
                            { name: 'RoadName', index: 'RoadName', height: 'auto', width: 22, align: "left", sortable: true },
                            { name: 'RoadLength', index: 'RoadLength', height: 'auto', width: 10, sortable: true, align: "left" },
                            { name: 'Collaboration', index: 'Collaboration', width: 10, sortable: true },
                            { name: 'SanctionedCost', index: 'SanctionedCost', width: 13, sortable: false, align: "right" },
                            { name: 'MaintenanceCost', index: 'MaintenanceCost', width: 13, sortable: false, align: "right" },
                            { name: 'Inspection', width: 7, sortable: false, resize: false, formatter: FormatColumnView, align: "center" },
                            { name: 'Financial Progress', width: 7, sortable: false, resize: false, formatter: FormatColumnProgrss, align: "center" },
                            { name: 'Tree Plant', width: 7, sortable: false, resize: false, formatter: FormatColumnTreePlant, align: "center" },
                           // { name: 'a', width: 80, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
                            { name: 'Upload', width: 7, sortable: false, resize: false, formatter: FormatColumnUpload, align: "center"},
                          //  { name: 'View', width: 7, sortable: false, resize: false, formatter: FormatColumnView, align: "center" } //PhotoUpload
                             { name: 'a', index: 'a', width: 13, sortable: false, align: "right", hidden: true },

        ],
        pager: jQuery('#dvProposedRoadListPager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Proposed Work List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: true,
        sortname: 'SanctionedYear,Package,RoadName',
        sortorder: "asc",
        loadComplete: function () {

            var reccount = $('#tbProposedRoadList').getGridParam('reccount');
            if (reccount > 0) {
                $('#dvProposedRoadListPager_left').html('[<b> Note</b>: 1. All Amounts are in Lakhs. 2.All Lengths are in Kms ]');
            }


        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                // alert(xhr.responseText);
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }

    }); //end of grid
}

function SearchDetails() {

    $('#tbProposedRoadList').setGridParam({
        url: '/MaintainanceInspection/GetCompletedRoadList',
        datatype: 'json'
    });

    $('#tbProposedRoadList').jqGrid("setGridParam", { "postData": { sanctionedYear: $('#ddlFinancialYears option:selected').val(), blockCode: $('#ddlBlocks option:selected').val(), packageID: $('#ddlPackages option:selected').val(), proposalType: $('#ddlProposalTypes option:selected').val(), batch: $('#ddlBatchs option:selected').val(), collaboration: $('#ddlCollaborations option:selected').val(), upgradationType: $('#ddlUpgradations option:selected').val() } });
    $('#tbProposedRoadList').trigger("reloadGrid", [{ page: 1 }]);

}

function FormatColumnView(cellvalue, options, rowObject) {

    return "<span class='ui-icon ui-icon-plusthick ui-align-center' title='Add Maintenance Inspection' onClick ='AddMaintenanceInspectionDetails(\"" + cellvalue.toString() + "\");'></span>";
}

function FormatColumnProgrss(cellvalue, options, rowObject) {

    return "<span class='ui-icon ui-icon-plusthick ui-align-center' title='Add Financial Progress' onClick ='AddFinancialProgressDetails(\"" + cellvalue.toString() + "\");'></span>";
}

function FormatColumnUpload(cellvalue, options, rowObject) {

    return "<span class='ui-icon ui-icon-plusthick ui-align-center' title='Click here to upload Images' onClick ='AddUploadDetailsProgress(\"" + cellvalue.toString() + "\");'></span>";
}

function AddMaintenanceInspectionDetails(parameter) {
   
    $.ajax({
        url: "/MaintainanceInspection/AddInspectionView?id=" + parameter,
        type: "GET",
        async: false,
        cache: false,
        dataType: 'html',
        success: function (data) {
           
            $("#accordion h3").html(
                 "<a href='#' style= 'font-size:.9em;' >Add Inspection Details</a>" +
                 '<a href="#" style="float: right;">' +
                 '<img class="ui-icon ui-icon-closethick" onclick="CloseInspectionDetails();" /></a>'
                 );
            $("#dvAddMaintenanceInspection").html(data);
            $('#accordion').show('slow');
            $('#dvAddMaintenanceInspection').show('slow');
            if ($("#dvSearchProposedRoad").is(":visible")) {
                $('#dvSearchProposedRoad').hide('slow');
            }
            $('#tbProposedRoadList').jqGrid("setGridState", "hidden");
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Error occurred while processing your request.');
        }
    });
}


function AddFinancialProgressDetails(parameter) {
 
    $.ajax({
        url: "/MaintainanceInspection/ListFinancialDetails?urlparameter=" + parameter,
        type: "GET",
        async: false,
        cache: false,
        dataType: 'html',
        success: function (data) {
                 $("#accordion h3").html(
                   "<a href='#' style= 'font-size:.9em;' >Add Financial Details</a>" +
                   '<a href="#" style="float: right;">' +
                   '<img class="ui-icon ui-icon-closethick" onclick="CloseFinanceDetails();" /></a>'
                   );
            $("#dvAddMaintenanceInspection").html(data);
            $('#accordion').show('slow');
            $('#dvAddMaintenanceInspection').show('slow');
            if ($("#dvSearchProposedRoad").is(":visible")) {
                $('#dvSearchProposedRoad').hide('slow');
            }
            $('#tbProposedRoadList').jqGrid("setGridState", "hidden");
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Error occurred while processing your request.');
        }
    });
}

function AddUploadDetails(urlparameter) {
    $("#Urlparameter").val(urlparameter);

    $("#accordionImage div").html("");
    $("#accordionImage h3").html(
            "<a href='#' style= 'font-size:.9em;' >Upload Images</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseInspectionDetails();" /></a>'
            );

    $('#accordionImage').show('fold', function () {
        blockPage();
        $("#divAddMaintenance").unload();
        $("#divAddMaintenance").load('/MaintainanceInspection/FileUpload/' + urlparameter, function (data) {
            $.validator.unobtrusive.parse($('#fileupload'));
            unblockPage();
            if (data.success == false) {
                alert(data.message);
            }
        });
        $('#divAddMaintenance').show('slow');
        $("#divAddMaintenance").css('height', 'auto');
    });
    $("#tbProposedRoadList").jqGrid('setGridState', 'hidden');
    //$('#idFilterDiv').trigger('click');
}

function CloseFinanceDetails() {

    if ($("#accordion").is(":visible")) {
        $('#accordion').hide('slow');
    }

    ViewSearchDiv();
    $('#tbProposedRoadList').jqGrid("setGridState", "visible");

    $("#dvAgreement").animate({
        scrollTop: 0
    });

}

function CloseInspectionDetails() {

    if ($("#accordionImage").is(":visible")) {
        $('#accordionImage').hide('slow');
    }

    ViewSearchDiv();
    $('#tbProposedRoadList').jqGrid("setGridState", "visible");

    $("#dvAgreement").animate({
        scrollTop: 0
    });

}



function ViewSearchDiv() {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    if (!$("#dvSearchProposedRoad").is(":visible")) {

        var data = $('#tbProposedRoadList').jqGrid("getGridParam", "postData");

        if (!(data === undefined)) {

            $('#ddlFinancialYears').val(data.sanctionedYear);
            $('#ddlBlocks').val(data.blockCode);
        }

        $("#dvSearchProposedRoad").show('slow');
        $.unblockUI();
    }
    $.unblockUI();

}
function FillInCascadeDropdown(map, dropdown, action) {

    //message = '<img src="/Content/images/busy.gif"/>';
    var message = '';
    message = '<h4><label style="font-weight:normal"> Loading Packages... </label></h4>';

    $(dropdown).empty();
    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
} //end FillInCascadeDropdown()

/***


Tree Plantation Details

********/


function FormatColumnTreePlant(cellvalue, options, rowObject) {
    //alert('AddFinancialProgressDetails(' + cellvalue.toString() + ');');;
    return "<span class='ui-icon ui-icon-plusthick ui-align-center' title='Add Financial Progress' onClick ='AddTreePlantDetails(" + cellvalue.toString() + ");'></span>";
}

$(function () {
    $("#accordionTreePlant").accordion({
        icons: false,
        heightStyle: "content",
        autoHeight: false
    });
});

function AddTreePlantDetails(id) {

    $.ajax({
        url: "/TreePlant/Index/" + id,
        type: "GET",
        async: false,
        cache: false,
        dataType: 'html',
        success: function (data) {
            //alert("Success" + id);
            $("#accordionTreePlant h3").html(
              "<a href='#' style= 'font-size:.9em;' >Add Tree Plant Details</a>" +
              '<a href="#" style="float: right;">' +
              '<img class="ui-icon ui-icon-closethick" onclick="CloseTreePlantDetails();" /></a>'
              );
            $("#dvAddTreePlant").html(data);
            $('#accordionTreePlant').show('slow');
            $('#dvAddTreePlant').show('slow');
            $('#tbProposedRoadList').jqGrid("setGridState", "hidden");
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Error occurred while processing your request.');
        }
    });
}

function CloseTreePlantDetails() {
    $('#accordionTreePlant').hide('slow');



    $('#tbProposedRoadList').jqGrid("setGridState", "visible");

    $("#dvAgreement").animate({
        scrollTop: 0
    });

}


////PhotoUpload 
//function FormatColumnView(cellvalue, options, rowObject) {

//    // return "<center><table><tr><td  style='border-color:white'><a href='#' title='View Agreement' onClick ='ViewAgreementDetails(\"" + cellvalue.toString() + "\");' >View</a></td></tr></table></center>";

//    return "<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-zoomin' title='Upload Image' onClick ='UploadImageProgress(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
//}



//PhotoUpload 
function UploadImageProgress(parameter) {


    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/MaintainanceInspection/ImageUploadProgress/" + parameter,
        type: "GET",
        async: false,
        cache: false,

        success: function (data) {

            $("#dvAddMaintenanceAgreementAgainstRoad1").html(data);

            $('#accordion1').show('slow');

            $('#dvAddMaintenanceAgreementAgainstRoad1').show('slow');


            if ($("#dvSearchProposedRoad").is(":visible")) {
                $('#dvSearchProposedRoad').hide('slow');
            }

            //    $('#tbProposedRoadList').jqGrid("setGridState", "hidden");

            $('#tbProposedRoadList').hide('slow');

            $.unblockUI();


        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });
}




function AddUploadDetailsProgress(urlparameter) {
    $("#Urlparameter").val(urlparameter);

    $("#accordionImage div").html("");
    $("#accordionImage h3").html(
            "<a href='#' style= 'font-size:.9em;' >Upload Images</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseInspectionDetails();" /></a>'
            );

    $('#accordionImage').show('fold', function () {
        blockPage();
        $("#divAddMaintenance").unload();
        $("#divAddMaintenance").load('/MaintainanceInspection/FileUploadProgress/' + urlparameter, function (data) {
            $.validator.unobtrusive.parse($('#fileupload'));
            unblockPage();
            if (data.success == false) {
                alert(data.message);
            }
        });
        $('#divAddMaintenance').show('slow');
        $("#divAddMaintenance").css('height', 'auto');
    });
    $("#tbProposedRoadList").jqGrid('setGridState', 'hidden');
    //$('#idFilterDiv').trigger('click');
}