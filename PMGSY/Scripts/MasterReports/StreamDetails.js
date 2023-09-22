
$(function () {
    $("#StreamDetailsButton").click(function () {
        streamType = $("#MAST_STREAM_TYPE").val();
        StreamReportsListing(streamType);
    });
    $("#StreamDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

});



function StreamReportsListing(streamType) {
    $("#StreamDetailsTable").jqGrid("GridUnload");
    $("#StreamDetailsTable").jqGrid({
        url: '/MasterReports/StreamDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Stream Name','Stream Type'],
            colModel:[
            {name: "MAST_STREAM_NAME", width: 300, align: 'left', height: 'auto'},
            {name: "MAST_STREAM_TYPE", width: 150, align: 'left', height: 'auto'},
       ],
        postData: { "MAST_STREAM_TYPE": streamType},
        pager: $("#StreamDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STREAM_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'Stream Details',
        loadComplete: function () {
            $('#StreamDetailsTable_rn').html('Sr.<br/>No.');
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