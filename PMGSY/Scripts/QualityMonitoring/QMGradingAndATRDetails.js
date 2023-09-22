/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMGradingAndATRDetails.js
        * Description   :   Handles events, grids in Grading And ATR Details
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/
$(document).ready(function () {

    loadQMGradingAndATRDetailsGrid("NQM", "tbGradingAndATRNQMReport", "dvGradingAndATRNQMReportPager", "I");

    ///Purposely commented on 11/09/2014 because, ATR report for SQM is not required
    //if ($("#hdnRole").val() == 8) {
    //    loadQMGradingAndATRDetailsGrid("SQM", "tbGradingAndATRSQMReport", "dvGradingAndATRSQMReportPager", "S");
    //}

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');


    $("#btnViewGradingAndATRDetails").click(function () {

        loadQMGradingAndATRDetailsGrid("NQM", "tbGradingAndATRNQMReport", "dvGradingAndATRNQMReportPager", "I");

        ///Purposely commented on 11/09/2014 because, ATR report for SQM is not required
        //if ($("#hdnRole").val() == 8) {
        //    loadQMGradingAndATRDetailsGrid("SQM", "tbGradingAndATRSQMReport", "dvGradingAndATRSQMReportPager", "S");
        //}
    });
});


function loadQMGradingAndATRDetailsGrid(userType, gridId, pagerId, qmType) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    var heightOfGrid = 430;
    if ($("#hdnRole").val() == 8) {
        heightOfGrid = 215;
    }

    $("#" + gridId).jqGrid('GridUnload');
    
    jQuery("#" + gridId).jqGrid({
        url: '/QualityMonitoring/QMGradingAndATRListing?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["State / District", "Total Inspections", "Total", "S", "SRI", "U", "R-S", "R-SRI", "R-U", "Total", "S", "SRI", "U", "R-S", "R-SRI", "R-U", "Required", "Submitted"],
        colModel: [
                        { name: 'LocationName', index: 'LocationName', width: 250, sortable: true, align: "left" },
                        { name: 'TotalInspections', index: 'TotalInspections', width: 100, sortable: true, align: "right" },
                        { name: 'TotalCompleted', index: 'TotalCompleted', width: 100, sortable: false, align: "right", search: false },
                        { name: 'CompletedS', index: 'CompletedS', width: 100, sortable: false, align: "right", search: false },
                        { name: 'CompletedSRI', index: 'CompletedSRI', width: 100, sortable: false, align: "right", search: false },
                        { name: 'CompletedU', index: 'CompletedU', width: 100, sortable: false, align: "right", search: false },
                        { name: 'CompletedRS', index: 'CompletedRS', width: 100, sortable: false, align: "right", search: false },
                        { name: 'CompletedRSRI', index: 'CompletedRSRI', width: 100, sortable: false, align: "right", search: false },
                        { name: 'CompletedRU', index: 'CompletedRU', width: 100, sortable: false, align: "right", search: false },
                        { name: 'TotalOngoing', index: 'TotalOngoing', width: 100, sortable: false, align: "right", search: false },
                        { name: 'OngoingS', index: 'OngoingS', width: 100, sortable: false, align: "right", search: false },
                        { name: 'OngoingSRI', index: 'OngoingSRI', width: 100, sortable: false, align: "right", search: false },
                        { name: 'OngoingU', index: 'OngoingU', width: 100, sortable: false, align: "right", search: false },
                        { name: 'OngoingRS', index: 'OngoingRS', width: 100, sortable: false, align: "right", search: false },
                        { name: 'OngoingRSRI', index: 'OngoingRSRI', width: 100, sortable: false, align: "right", search: false },
                        { name: 'OngoingRU', index: 'OngoingRU', width: 100, sortable: false, align: "right", search: false },
                        { name: 'ATRRequired', index: 'ATRRequired', width: 100, sortable: false, align: "right", search: false },
                        { name: 'ATRSubmitted', index: 'ATRSubmitted', width: 100, sortable: false, align: "right", search: false }
        ],
        postData: { 'fromYear': $("#ddlFromYearGradingAndATR").val(), 'toYear': $("#ddlToYearGradingAndATR").val(), 'fromMonth': $("#ddlFromMonthGradingAndATR").val(), 'toMonth': $("#ddlToMonthGradingAndATR").val(), 'qmType': qmType },
        pager: jQuery('#' + pagerId),
        rowNum: 20000,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Abstract of Grading State Wise for " + userType,
        autowidth: true,
        height: heightOfGrid,
        sortname: 'LocationName',
        rownumbers: true,
        loadComplete: function () {
            //if ($("#hdnRole").val() == 8) //for SQC hide these two columns
            //{
            //    $('#tbGradingAndATRSQMReport').jqGrid('hideCol', 'ATRRequired');
            //    $('#tbGradingAndATRSQMReport').jqGrid('hideCol', 'ATRSubmitted');
            //}
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

    $("#" + gridId).jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'TotalCompleted', numberOfColumns: 7, titleText: 'Completed Works' },
                       { startColumnName: 'TotalOngoing', numberOfColumns: 7, titleText: 'Ongoing Works' },
                       { startColumnName: 'ATRRequired', numberOfColumns: 2, titleText: 'ATR' }
        ]
    });
    
}