$(function () {
    FeedbackReportsListing();
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');


});

function FeedbackReportsListing() {
    $("#FeedbackDetailsTable").jqGrid({
        url: '/MasterReports/FeedbackDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Category Name'],
        colModel: [
            { name: "MAST_FEED_NAME", width: '200', align: 'center', height: 'auto' }         
        ],
        pager: $("#FeedbackDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_FEED_NAME',
        sortorder: 'asc',
        recordtext: '{2} records found',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '650',
        viewrecords: true,
        caption: 'Feedback Category Details',
        loadComplete: function () {
            $('#FeedbackDetailsTable_rn').html('Sr.<br/>No.');
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
    });
}