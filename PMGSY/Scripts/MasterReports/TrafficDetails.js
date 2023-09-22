
$(function () {
    $("#TrafficDetailsButton").click(function () {
        var trafficType = $("#MAST_TRAFFIC_STATUS").val();
        TrafficReportsListing(trafficType);
    });
    $("#TrafficDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

});


function TrafficReportsListing(trafficType) {
    $("#TrafficDetailsTable").jqGrid("GridUnload");
    $("#TrafficDetailsTable").jqGrid({
        url: '/MasterReports/TrafficDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Traffic Name','Traffic Active Status'],
        colModel:[
            {name: "MAST_TRAFFIC_NAME", width: 150, align: 'left', height: 'auto'},
            {name: "MAST_TRAFFIC_STATUS", width: 150, align: 'left', height: 'auto'},
        ],
        postData: { "MAST_TRAFFIC_STATUS": trafficType },
        pager: $("#TrafficDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_TRAFFIC_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'Traffic Details',
        loadComplete: function () {
            $('#TrafficDetailsTable_rn').html('Sr.<br/>No.');
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