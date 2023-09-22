
$(function () {
    SoilReportsListing();
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

   
});

function SoilReportsListing() {
    $("#SoilDetailsTable").jqGrid({
        url: '/MasterReports/SoilDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Soil Type Name'],
        colModel: [
            {name: "MAST_SOIL_TYPE_NAME", width: 150, align: 'left', height: 'auto'},
            ],
        pager: $("#SoilDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_SOIL_TYPE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '650',
        viewrecords: true,
        caption: 'Soil Details',
        loadComplete: function () {
            $('#SoilDetailsTable_rn').html('Sr.<br/>No.');
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