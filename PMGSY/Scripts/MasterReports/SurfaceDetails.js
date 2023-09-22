
$(function () {
    SurfaceReportsListing();
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
   
});

function SurfaceReportsListing() {
    $("#SurfaceDetailsTable").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#SurfaceDetailsTable").jqGrid({
        url: '/MasterReports/SurfaceDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Surface Name'],
        colModel:[
            {name: "MAST_SURFACE_NAME", width: 150, align: 'left', height: 'auto'},
        ],
        pager: $("#SurfaceDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_SURFACE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '650',
        viewrecords: true,
        caption: 'Surface Details',
        loadComplete: function () {
            $('#SurfaceDetailsTable_rn').html('Sr.<br/>No.');
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