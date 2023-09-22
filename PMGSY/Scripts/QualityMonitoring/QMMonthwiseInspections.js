/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMMonthwiseInspections.js
        * Description   :   Handles events, grids in Monthwise Inspections Report
        * Author        :   Shyam Yadav 
        * Creation Date :   02/Dec/2013
 **/

$(document).ready(function () {

    loadQMMonthwiseInspectionsGrid("NQM", "tbMonthwiseComparisionNQMReport", "dvMonthwiseComparisionNQMReportPager", "I");
    loadQMMonthwiseInspectionsGrid("SQM", "tbMonthwiseComparisionSQMReport", "dvMonthwiseComparisionSQMReportPager", "S");

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');


    $("#btnViewMonthwiseComparision").click(function () {
        loadQMMonthwiseInspectionsGrid("NQM", "tbMonthwiseComparisionNQMReport", "dvMonthwiseComparisionNQMReportPager", "I");
        loadQMMonthwiseInspectionsGrid("SQM", "tbMonthwiseComparisionSQMReport", "dvMonthwiseComparisionSQMReportPager", "S");
    });
});


function loadQMMonthwiseInspectionsGrid(userType, gridId, pagerId, qmType) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#" + gridId).jqGrid('GridUnload');

    jQuery("#" + gridId).jqGrid({
        url: '/QualityMonitoring/QMMonthwiseInspectionsListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Schedule Month", "No of Schedule Assigned", "Total No of Inspections", "No of Inspections Using Mobile", "No of Inspections using Web", "No of Schedule Having No Inspection"],
        colModel: [
                    { name: 'SchMonth', index: 'SchMonth', width: 100, sortable: false, align: "left" },
                    { name: 'NoOfSchedulesAssigned', index: 'NoOfSchedulesAssigned', width: 100, sortable: false, align: "right" },
                    { name: 'TotalInsp', index: 'TotalInsp', width: 100, sortable: false, align: "right", search: false },
                    { name: 'MobileInsp', index: 'MobileInsp', width: 100, sortable: false, align: "right", search: false },
                    { name: 'WebInsp', index: 'WebInsp', width: 100, sortable: false, align: "right", search: false },
                    { name: 'TotalNoInsp', index: 'TotalNoInsp', width: 100, sortable: false, align: "right", search: false }
        ],
        postData: { 'state': $("#ddlStateMonthwiseComparision").val(), 'year': $("#ddlYearMonthwiseComparision").val(), 'qmtype': qmType },
        pager: jQuery('#' + pagerId),
        rowNum: 20000,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Month wise Inspection Detail for " + userType,
        autowidth: true,
        height: 'auto',
        sortname: 'SchMonth',
        //rowList: [20, 30, 40],
        rownumbers: true,
        footerrow:true,
        loadComplete: function () {

            var NoOfSchedulesAssignedTotal = $(this).jqGrid('getCol', 'NoOfSchedulesAssigned', false, 'sum');
            var TotalInspTotal = $(this).jqGrid('getCol', 'TotalInsp', false, 'sum');
            var MobileInspTotal = $(this).jqGrid('getCol', 'MobileInsp', false, 'sum');
            var WebInspTotal = $(this).jqGrid('getCol', 'WebInsp', false, 'sum');
            var TotalNoInspTotal = $(this).jqGrid('getCol', 'TotalNoInsp', false, 'sum');

            $(this).jqGrid('footerData', 'set', { SchMonth: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { NoOfSchedulesAssigned: NoOfSchedulesAssignedTotal });
            $(this).jqGrid('footerData', 'set', { TotalInsp: TotalInspTotal });
            $(this).jqGrid('footerData', 'set', { MobileInsp: MobileInspTotal });
            $(this).jqGrid('footerData', 'set', { WebInsp: WebInspTotal });
            $(this).jqGrid('footerData', 'set', { TotalNoInsp: TotalNoInspTotal });

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