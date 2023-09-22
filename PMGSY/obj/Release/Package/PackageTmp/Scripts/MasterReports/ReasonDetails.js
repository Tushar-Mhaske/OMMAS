
$(function () {
    $("#ReasonDetailsButton").click(function () {
        var reasonType = $("#MAST_REASON_TYPE").val();
        ReasonReportsListing(reasonType);
    });
    $("#ReasonDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});



function ReasonReportsListing(reasonType) {
    $("#ReasonDetailsTable").jqGrid("GridUnload");
    $("#ReasonDetailsTable").jqGrid({
        url: '/MasterReports/ReasonDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Reason Name', 'Reason Type'],
        colModel: [
            { name: "MAST_REASON_NAME", width: 300, align: 'left', height: 'auto' },
            { name: "MAST_REASON_TYPE", width: 150, align: 'left', height: 'auto' }
        ],
        postData: { "MAST_REASON_TYPE": reasonType},
        pager: $("#ReasonDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_REASON_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'Reason Details',
        loadComplete: function () {
            $('#ReasonDetailsTable_rn').html('Sr.<br/>No.');
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