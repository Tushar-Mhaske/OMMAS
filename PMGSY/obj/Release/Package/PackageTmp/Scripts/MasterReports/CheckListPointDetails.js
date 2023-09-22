
$(document).ready(function () {

    $("#CheckListPointDetailsButton").click(function () {
        var checkListActive = $("#MAST_CHECKLIST_ACTIVE").val();
        CheckListPointReportsListing(checkListActive);
    });
    $("#CheckListPointDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

});


function CheckListPointReportsListing(checkListActive) {
    $("#CheckListPointDetailsTable").jqGrid("GridUnload");
    $("#CheckListPointDetailsTable").jqGrid({
        url: '/MasterReports/CheckListPointDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Checklist Issues', 'Active'],
        colModel: [
            { name: "MAST_CHECKLIST_ISSUES", width: 600, align: 'left', height: 'auto' },
            { name: "MAST_CHECKLIST_ACTIVE", width: 150, align: 'left', height: 'auto' }
        ],
        postData: { "MAST_CHECKLIST_ACTIVE": checkListActive },
        pager: $("#CheckListPointDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_CHECKLIST_ISSUES',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'Checklist Point Details',
        loadComplete: function () {
            $('#CheckListPointDetailsTable_rn').html('Sr.<br/>No.');
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