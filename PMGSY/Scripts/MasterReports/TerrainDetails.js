
$(function () {
    TerrainReportsListing();
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

   
});

function TerrainReportsListing() {
    $("#TerrainDetailsTable").jqGrid({
        url: '/MasterReports/TerrainDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Terrain Type','Terrain Roadway Width','Terrain Slope From','Terrain Slope To'],
        colModel: [
            {name: "MAST_TERRAIN_TYPE_NAME", width: 150, align: 'left', height: 'auto'},
            {name: "MAST_TERRAIN_ROADWAY_WIDTH", width: 150, align: 'left', height: 'auto'},
            { name: "MAST_TERRAIN_SLOP_FROM", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_TERRAIN_SLOP_TO", width: 150, align: 'left', height: 'auto' },
        ],
        pager: $("#TerrainDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_TERRAIN_TYPE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '650',
        viewrecords: true,
        caption: 'Terrain Details',
        loadComplete: function () {
            $('#TerrainDetailsTable_rn').html('Sr.<br/>No.');
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