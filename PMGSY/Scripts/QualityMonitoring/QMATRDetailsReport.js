/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMATRDetailsReport.js
        * Description   :   Handles events, grids in ATR details report
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/

$(document).ready(function () {

    loadQMATRReportGrid();

    $('#btnViewATRDetails').click(function () {
        loadQMATRReportGrid();
    });

});
function loadQMATRReportGrid() {
    
    $("#tbQMATRReport").jqGrid('GridUnload');

    jQuery("#tbQMATRReport").jqGrid({
        url: '/QualityMonitoring/QMATRDetailsReportList?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "Total Inspection", "ATR Required Initially", "Submitted", "Not Submitted", "Approved", "Rejected", "Total ATR Required", "Pending For Approval"],
        colModel: [
                    { name: 'StateName', index: 'StateName', width: 150, sortable: true, align: "left" },
                    { name: 'TotalInspec', index: 'TotalInspec', width: 100, sortable: false, align: "right", search: false },
                    { name: 'ATRReqiured', index: 'ATRReqiured', width: 100, sortable: false, align: "right", search: false },
                    { name: 'Submitted', index: 'Submitted', width: 100, sortable: false, align: "right", search: false },
                    { name: 'NotSubmitted', index: 'NotSubmitted', width: 100, sortable: false, align: "right", search: false },
                    { name: 'Approved', index: 'Approved', width: 100, sortable: false, align: "right", search: false },
                    { name: 'Rejected', index: 'Rejected', width: 100, sortable: false, align: "right", search: false },
                    { name: 'TotalATRRequired', index: 'TotalATRRequired', width: 100, sortable: false, align: "right", search: false },
                    { name: 'PendingforApproval', index: 'PendingforApproval', width: 100, sortable: false, align: "right", search: false }
        ],
        postData: { 'fromyear': $("#ddlFromYearATRDetails").val(), 'frommonth': $("#ddlFromMonthATRDetails").val(), 'toyear': $("#ddlToYearATRDetails").val(), 'tomonth': $("#ddlToMonthATRDetails").val(), },
        pager: jQuery('#dvQMATRReportPager'),
        rowNum: 20000,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Action Taken Report",
        height: 520,
        autowidth: true,
        sortname: 'StateName',
        sortorder: 'asc',
        // rowList: [20, 30, 40],
        rownumbers: true,
        loadComplete: function () {

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