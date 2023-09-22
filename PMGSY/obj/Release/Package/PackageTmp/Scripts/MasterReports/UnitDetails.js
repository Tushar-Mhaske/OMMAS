
$(function () {
    UnitReportsListing();
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

   
});

function UnitReportsListing() {
    $("#UnitDetailsTable").jqGrid({
        url: '/MasterReports/UnitDetailsListing',
        datatype: 'json',
        mtype: 'POST',        
        colNames: ['Unit Name', 'Unit Short Name', 'Unit Dimension'],
        colModel: [
            { name: 'MAST_UNIT_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_UNIT_SHORT_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_UNIT_DIMENSION', width: 300, align: 'left', height: 'auto' }
        ],
        pager: $("#UnitDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_UNIT_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '650',
        viewrecords: true,
        caption: 'Unit Details',
        loadComplete: function () {
            $('#UnitDetailsTable_rn').html('Sr.<br/>No.');
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