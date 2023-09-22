/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMItemwiseInspections.js
        * Description   :   Handles events, grids in Itemwise Inspection Report
        * Author        :   Shyam Yadav 
        * Creation Date :   02/Dec/2013
 **/

$(document).ready(function () {

    $("#accordionItemwiseInspection").accordion({
        icons: false,
        heightStyle: "content",
        autoHeight: false
    });

    loadQMItemwiseInspectionsGrid("NQM", "tbItemwiseInspectionsNQMReport", "dvItemwiseInspectionsNQMReportPager", "I");

    if ($("#hdnRole").val() == 8) {
        loadQMItemwiseInspectionsGrid("SQM", "tbItemwiseInspectionsSQMReport", "dvItemwiseInspectionsSQMReportPager", "S");
    }
    

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');


    $("#btnViewItemwiseInspectionsDetails").click(function () {

        loadQMItemwiseInspectionsGrid("NQM", "tbItemwiseInspectionsNQMReport", "dvItemwiseInspectionsNQMReportPager", "I");
        if ($("#hdnRole").val() == 8) {
            loadQMItemwiseInspectionsGrid("SQM", "tbItemwiseInspectionsSQMReport", "dvItemwiseInspectionsSQMReportPager", "S");
        }
    });


   

});


function loadQMItemwiseInspectionsGrid(userType, gridId, pagerId, qmType) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    var heightOfGrid = 430;
    if ($("#hdnRole").val() == 8) {
        heightOfGrid = 215;
    }

    $("#" + gridId).jqGrid('GridUnload');
    
    jQuery("#" + gridId).jqGrid({
        url: '/QualityMonitoring/QMItemwiseInspectionsListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Monitor", "Name of Road", "Inspection Date", "Chainage", "Road Status", "State", "District", "Item Graded as (SRI/U)", "View Details"],
        colModel: [
                        { name: 'MonitorName', index: 'MonitorName', width: 150, sortable: true, align: "left" },
                        { name: 'RoadName', index: 'RoadName', width: 200, sortable: true, align: "left" },
                        { name: 'InspDate', index: 'InspDate', width: 100, sortable: false, align: "left", search: false },
                        { name: 'Chainage', index: 'Chainage', width: 100, sortable: false, align: "left", search: false },
                        { name: 'RdStatus', index: 'RdStatus', width: 100, sortable: false, align: "left", search: false },
                        { name: 'StateName', index: 'StateName', width: 100, sortable: false, align: "left", search: false },
                        { name: 'DistrictName', index: 'DistrictName', width: 100, sortable: false, align: "left", search: false },
                        { name: 'GradeItem', index: 'GradeItem', width: 200, sortable: false, align: "left", search: false },
                        { name: 'View', index: 'View', width: 100, sortable: false, align: "right", search: false }
        ],
        postData: {
            'state': $("#ddlStatesItemwiseInspections").val(), 'grade': $('input[name=GRADE]:checked', '#frmItemwiseInspectionsReport').val(),
            'fromYear': $("#ddlFromYearItemwiseInspections").val(), 'toYear': $("#ddlToYearItemwiseInspections").val(),
            'fromMonth': $("#ddlFromMonthItemwiseInspections").val(), 'toMonth': $("#ddlToMonthItemwiseInspections").val(),
            'citem': $("#ddlItemsInItemwiseInspections").val(), 'qmType': qmType
        },
        pager: jQuery('#' + pagerId),
        rowNum: 20000,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Summary of Deficiencies Reported in " + userType +" Inspection",
        autowidth: true,
        height: heightOfGrid,
        sortname: 'MonitorName',
        //rowList: [20, 30, 40],
        rownumbers: true,
        loadComplete: function () {

            $.unblockUI();
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }

            $.unblockUI();
        }
    }); //end of grid

}

function CloseInspectionDetails() {
    $('#accordionItemwiseInspection').hide('slow');
    $('#divItemwiseInspection').hide('slow');
    $("#tbItemwiseInspectionsNQMReport").jqGrid('setGridState', 'visible');
    $("#tbItemwiseInspectionsSQMReport").jqGrid('setGridState', 'visible');
}

function ViewItemwiseGradingDetails(obsId) {

    jQuery('#tbItemwiseInspectionsNQMReport').jqGrid('setSelection', obsId);
    jQuery('#tbItemwiseInspectionsSQMReport').jqGrid('setSelection', obsId);

    $("#accordionItemwiseInspection div").html("");
    $("#accordionItemwiseInspection h3").html(
            "<a href='#' style= 'font-size:.9em;' >Inspection Details</a>" +

            '<a href="#" style="float: right;">' +
            '<img src="" style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseInspectionDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordionItemwiseInspection').show('slow', function () {
        blockPage();
        $("#divItemwiseInspection").load('/QualityMonitoring/QMObservationDetails/' + obsId, function () {
            unblockPage();
        });
    });

    $("#divItemwiseInspection").css('height', 'auto');
    $('#divItemwiseInspection').show('slow');

    $("#tbItemwiseInspectionsNQMReport").jqGrid('setGridState', 'hidden');
    $("#tbItemwiseInspectionsSQMReport").jqGrid('setGridState', 'hidden');
    //$('#id3TierFilterDiv').trigger('click');
}