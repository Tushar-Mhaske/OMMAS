
$(document).ready(function () {
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

    CDWorksTypeReportsListing();
});

function CDWorksTypeReportsListing() {
    $("#CDWorksTypeDetailsTable").jqGrid({
        url: '/MasterReports/CDWorksTypeDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['CD Works Type'],
        colModel: [
            { name: "MAST_CDWORKS_NAME", width: 1050, align: 'left', height: 'auto' },
        ],
        pager: $("#CDWorksTypeDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_CDWORKS_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: false,
        width: 1100,
        shrinkToFit:false,
        height: '650',
        viewrecords: true,
        caption: 'CD Works Type',
        loadComplete: function () {
            $('#CDWorksLengthDetailsTable_rn').html('Sr.<br/>No.');

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