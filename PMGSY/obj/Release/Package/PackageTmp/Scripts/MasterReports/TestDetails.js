
$(function () {
    TestReportsListing();
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

   
});

function TestReportsListing() {
    $("#TestDetailsTable").jqGrid({
        url: '/MasterReports/TestDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Test Name', 'Test Description', 'Test Status'],
        colModel: [
            { name: "MAST_TEST_NAME", width: '200', align: 'center', height: 'auto' },
            { name: "MAST_TEST_DESC", width: '200', align: 'center', height: 'auto' },
            { name: "MAST_TEST_STATUS", width: '200', align: 'center', height: 'auto' }
        ],
        pager: $("#TestDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_TEST_NAME',
        sortorder: 'asc',
        recordtext: '{2} records found',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '650',
        viewrecords: true,
        caption: 'Test Report  Details',
        loadComplete: function () {
            $('#TestDetailsTable_rn').html('Sr.<br/>No.');
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