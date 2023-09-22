
$(function () {
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

    ComponentReportsListing();
});

function ComponentReportsListing() {
    $("#ComponentDetailsTable").jqGrid({
        url: '/MasterReports/ComponentDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Componenet Name'],
        colModel: [
            { name: "MAST_COMPONENT_NAME", width: 1050, align: 'left', height: 'auto' },
        ],
        pager: $("#ComponentDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_COMPONENT_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: false,
        width: 1100,
        shrinkToFit: false,
        height: '650',
        viewrecords: true,
        caption: 'Component Details',
        loadComplete: function () {
            $('#ComponentDetailsTable_rn').html('Sr.<br/>No.');
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