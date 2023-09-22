
$(function () {
    RoadCategoryReportsListing();
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

   
});

function RoadCategoryReportsListing() {
    $("#RoadCategoryDetailsTable").jqGrid({
        url: '/MasterReports/RoadCategoryDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Road Category Name', 'Road Short Description'],
        colModel: [
            { name: 'MAST_ROAD_CAT_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_ROAD_SHORT_DESC', width: 300, align: 'left', height: 'auto' }
        ],
        pager: $("#RoadCategoryDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_ROAD_CAT_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '650',
        viewrecords: true,
        caption: 'Road Category Details',
        loadComplete: function () {
            $('#RoadCategoryDetailsTable_rn').html('Sr.<br/>No.');
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