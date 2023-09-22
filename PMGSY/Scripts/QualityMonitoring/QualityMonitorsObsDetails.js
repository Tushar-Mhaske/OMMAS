/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QualityMonitorsObsDetails.js
        * Description   :   Handles events & Grids for Observation Details for  Quality Monitors
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/

$(document).ready(function () {

    $.ajaxSetup({ cache: false });

    ScheduledRoadListGrid($("#ADMIN_IM_MONTH").val(), $("#ADMIN_IM_YEAR").val());

    $("#btnViewScheduledRoads").click(function () {
        ScheduledRoadListGrid($("#ADMIN_IM_MONTH").val(), $("#ADMIN_IM_YEAR").val());
    });


    $("#idObsDetailsNoteDiv").click(function () {

        $("#idObsDetailsNoteDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divlFillObsDetailsFilters").toggle("slow");
    });
});




//Road List 
function ScheduledRoadListGrid(inspMonth, inspYear) {
    var date = new Date();
    var month = date.getMonth() + 1;
    var year = date.getFullYear();

    if ((inspYear > year) || (inspYear == year && inspMonth > month)) {
        alert("Selected Month or Year cannot be greater than Current Month or Year");
    }
    else {


        $("#tbMonitorsObsList").jqGrid('GridUnload');

        jQuery("#tbMonitorsObsList").jqGrid({
            url: '/QualityMonitoring/GetMonitorsScheduledRoadList?' + Math.random(),
            datatype: "json",
            mtype: "POST",
            colNames: ["State", "District", "Block", "Package", "Sanction Year", "Road Name", "Type", "Length (Road-Km / LSB-Mtr)", "Work Status (Current)", "Work Status (As on schedule)", "Enquiry Inspection", "Scheme", "UnPlanned Schedule", "No. Of Photo Uploaded", "Fill Observations", "Upload File(Images)"],
            colModel: [
                { name: 'State', index: 'State', width: 70, sortable: false, align: "left" },
                { name: 'District', index: 'District', width: 70, sortable: false, align: "left" },
                { name: 'Block', index: 'Block', width: 70, sortable: false, align: "left" },
                { name: 'Package', index: 'Package', width: 60, sortable: false, align: "center" },
                { name: 'SanctionYear', index: 'SanctionYear', width: 60, sortable: false, align: "center" },
                { name: 'RoadName', index: 'RoadName', width: 200, sortable: false, align: "left" },
                { name: 'PropType', index: 'PropType', width: 60, sortable: false, align: "left", search: false },
                { name: 'RdLength', index: 'RdLength', width: 60, sortable: false, align: "center" },
                { name: 'RdStatus', index: 'RdStatus', width: 130, sortable: false, align: "left" },
                { name: 'SchRdStatus', index: 'SchRdStatus', width: 130, sortable: false, align: "left" },
                { name: 'IsEnquiry', index: 'IsEnquiry', width: 70, sortable: false, align: "center" },
                { name: 'Scheme', index: 'Scheme', width: 70, sortable: false, align: "center" },
                { name: 'IsPlanned', index: 'IsPlanned', width: 60, sortable: false, align: "center" },
                { name: 'NoOfPhoto', index: 'NoOfPhoto', width: 60, sortable: false, align: "center" },
                { name: 'FillObservations', index: 'FillObservations', width: 60, sortable: false, align: "center", search: false },
                { name: 'UploadFile', index: 'UploadFile', width: 60, sortable: false, align: "center", search: false }
            ],
            postData: { "inspMonth": inspMonth, "inspYear": inspYear },
            pager: jQuery('#dvMonitorsObsListPager'),
            rowNum: 10,
            rowList: [10, 20, 30],
            sortorder: "asc",
            sortname: "Block",
            viewrecords: true,
            recordtext: '{2} records found',
            caption: "&nbsp;&nbsp;Road List",
            height: 'auto',
            //autowidth: true,
            rownumbers: true,
            loadComplete: function () {
                $("#gview_tbMonitorsObsList > .ui-jqgrid-titlebar").hide();



                if ($("#hdnRoleCodeMonitors").val() == 7) {
                    $("#tbMonitorsObsList").jqGrid('setGridWidth', $("#divMonitorsObsDetails").width() + 65, true);
                    $('#tbMonitorsObsList').jqGrid('hideCol', 'NoOfPhoto');
                }
                else {
                    $("#tbMonitorsObsList").jqGrid('setGridWidth', $("#divMonitorsObsDetails").width(), true);
                }
                unblockPage();
            },
            loadError: function (xhr, status, error) {
                if (xhr.responseText == "session expired") {
                    window.location.href = "/Login/SessionExpire";
                }
                else {
                    window.location.href = "/Login/SessionExpire";
                }
            }
        }); //end of grid

    }

}//Schedule Road Grid Ends Here



function UploadObsFile(scheduleCode, prRoadCode, obsId) {

    var random = Math.random();

    jQuery('#tbMonitorsObsList').jqGrid('setSelection', prRoadCode);

    $("#accordionMonitorsInspection div").html("");

    $("#accordionObsMonitors div").html("");
    $("#accordionObsMonitors h3").html(
        "<a href='#' style= 'font-size:.9em;' >Upload File</a>" +

        '<a href="#" style="float: right;">' +
        '<img src="" style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeMonitorsObsDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    var number = Math.floor((Math.random() * 99999999) + 1);

    $('#accordionObsMonitors').show('slow', function () {
        blockPage();
        $("#divMonitorsObservationDetails").load('/QualityMonitoring/ImageUpload/' + scheduleCode + "$" + prRoadCode + "$" + obsId, function () {
            unblockPage();
        });
    });

    $("#divMonitorsObservationDetails").css('height', 'auto');
    $('#divMonitorsObservationDetails').show('slow');

    $("#tbMonitorsObsList").jqGrid('setGridState', 'hidden');
}


