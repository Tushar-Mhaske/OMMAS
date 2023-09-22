
$(function () {
    $("#ExecutionItemDetailsButton").click(function () {
        var headType = $("#MAST_HEAD_TYPE").val();
        ExecutionItemReportsListing(headType);
    });
    $("#ExecutionItemDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});



function ExecutionItemReportsListing(headType) {
    $("#ExecutionItemDetailsTable").jqGrid("GridUnload");
    $("#ExecutionItemDetailsTable").jqGrid({
        url: '/MasterReports/ExecutionItemDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Item Name','Short Description','Type'],
        colModel:[
            {name: "MAST_HEAD_DESC", width: 150, align: 'left', height: 'auto'},
            {name: "MAST_HEAD_SH_DESC", width: 150, align: 'left', height: 'auto'},
            {name: "MAST_HEAD_TYPE", width: 150, align: 'left', height: 'auto'},
        ],
        postData: { "MAST_HEAD_TYPE": headType},
        pager: $("#ExecutionItemDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_HEAD_DESC',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'Execution Item Details',
        loadComplete: function () {
            $('#ExecutionItemDetailsTable_rn').html('Sr.<br/>No.');

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