/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   CQCLayout.js
        * Description   :   Handles events, grid data in Layout Page for CQC Login
        * Author        :   Shyam Yadav 
        * Creation Date :   10/Jun/2013
 **/

$(document).ready(function () {

    $("#id3TierFilterDiv").click(function () {

        $("#id3TierFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#div3TierFilterForm").toggle("slow");

    });

    $(function () {
        $("#accordionObsMonitors").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });


    $("#btnViewScheduledRoads").click(function () {

        if (validate())
        {
            ScheduledRoadListGrid($("#ADMIN_QM_CODE").val(), $("#FROM_MONTH").val(), $("#FROM_YEAR").val());
        }

        
    });




});

function closeMonitorsObsDetails() {
    $('#accordionObsMonitors').hide('slow');
    $('#divMonitorsObservationDetails').hide('slow');
    $("#tbMonitorsObsList").jqGrid('setGridState', 'visible');
}



//Road List 
function ScheduledRoadListGrid(monitorCode, inspMonth, inspYear) {

    $("#tbMonitorsObsList").jqGrid('GridUnload');

    jQuery("#tbMonitorsObsList").jqGrid({
        url: '/QualityMonitoring/CQCMonitorsScheduledRoadList?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "District", "Block", "Package", "Sanction Year", "Road Name", "Type", "Length (Road-Km / LSB-Mtr)", "Road Status", "Enquiry Inspection", "Scheme", "UnPlanned Schedule", "No. Of Photo Uploaded", "Fill Observations", "Upload File"],
        colModel: [
                        { name: 'State', index: 'State', width: 80, sortable: false, align: "left" },
                        { name: 'District', index: 'District', width: 70, sortable: false, align: "left" },
                        { name: 'Block', index: 'Block', width: 70, sortable: false, align: "left" },
                        { name: 'Package', index: 'Package', width: 60, sortable: false, align: "center" },
                        { name: 'SanctionYear', index: 'SanctionYear', width: 60, sortable: false, align: "center" },
                        { name: 'RoadName', index: 'RoadName', width: 180, sortable: false, align: "left" },
                        { name: 'PropType', index: 'PropType', width: 50, sortable: false, align: "left", search: false },
                        { name: 'RdLength', index: 'RdLength', width: 60, sortable: false, align: "center" },
                        { name: 'RdStatus', index: 'RdStatus', width: 150, sortable: false, align: "left" },
                        { name: 'IsEnquiry', index: 'IsEnquiry', width: 70, sortable: false, align: "center" },
                        { name: 'Scheme', index: 'Scheme', width: 70, sortable: false, align: "center" },
                        { name: 'IsPlanned', index: 'IsPlanned', width: 60, sortable: false, align: "center" },
                        { name: 'NoOfPhoto', index: 'NoOfPhoto', width: 60, sortable: false, align: "center" },
                        { name: 'FillObservations', index: 'FillObservations', width: 60, sortable: false, align: "center", search: false },
                        { name: 'UploadFile', index: 'UploadFile', width: 60, sortable: false, align: "center", search: false }
        ],
        postData: { "monitorCode": monitorCode, "inspMonth": inspMonth, "inspYear": inspYear },
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
            //$("#gview_tbMonitorsObsList > .ui-jqgrid-titlebar").hide();

            //$("#tbMonitorsObsList").jqGrid('setGridWidth', $("#divMonitorsObsDetails").width(), true);

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

}//Schedule Road Grid Ends Here



function validate() {
    if ($("#MAST_STATE_CODE").val() == 0 || $("#MAST_STATE_CODE").val() == "") {
        alert("Please select State");
        return false;
    }

    if ($("#ADMIN_QM_CODE").val() == 0 || $("#ADMIN_QM_CODE").val() == "") {
        alert("Please select Monitor");
        return false;
    }

    return true;
}


function QMFillObservations(scheduleCode, prRoadCode) {
    //alert(scheduleCode);
    jQuery('#tbMonitorsObsList').jqGrid('setSelection', prRoadCode);
    
    $("#accordionObsMonitors div").html("");
    $("#accordionObsMonitors h3").html(
            "<a href='#' style= 'font-size:.9em;' >Observation Details</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeMonitorsObsDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordionObsMonitors').show('slow', function () {
        blockPage();
        $("#divMonitorsObservationDetails").load('/QualityMonitoring/QMFillObservations/' + scheduleCode + "/" + prRoadCode, function () {
            unblockPage();
        });
    });

    $("#divMonitorsObservationDetails").css('height', 'auto');
    $('#divMonitorsObservationDetails').show('slow');

    $("#tbMonitorsObsList").jqGrid('setGridState', 'hidden');


    //$("#idObsDetailsNoteDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
    //$("#divlFillObsDetailsFilters").toggle("slow");
}



function UploadCqcFile(scheduleCode, prRoadCode) {
    //alert(scheduleCode + prRoadCode);
    jQuery('#tbMonitorsObsList').jqGrid('setSelection', prRoadCode);

    $("#accordionObsMonitors div").html("");
    $("#accordionObsMonitors h3").html(
            "<a href='#' style= 'font-size:.9em;' >Upload File</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeMonitorsObsDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    var number = Math.floor((Math.random() * 99999999) + 1);

    $('#accordionObsMonitors').show('slow', function () {
        blockPage();
        $("#divMonitorsObservationDetails").load('/QualityMonitoring/ImageUpload/' + scheduleCode + "$" + prRoadCode + "$" + number, function () {
            unblockPage();
        });
    });

    $("#divMonitorsObservationDetails").css('height', 'auto');
    $('#divMonitorsObservationDetails').show('slow');

    //$("#tbMonitorsObsList").jqGrid('setGridState', 'hidden');
}


