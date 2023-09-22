
$(function () {
    LokSabhaTermReportsListing();
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

  
});

function LokSabhaTermReportsListing() {
    $("#LokSabhaTermDetailsTable").jqGrid({
        url: '/MasterReports/LokSabhaTermDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Term', 'Lok Sabha Start Date', 'Lok Sabha End Date'],
        colModel: [
            { name: "MAST_LS_TERM", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_LS_START_DATE", width: 150, align: 'left', height: 'auto', sorttype: 'date', formatter: 'date', formatoptions: { srcformat: 'ISO8601Long', newformat: 'd/m/Y' } },
            { name: "MAST_LS_END_DATE", width: 150, align: 'left', height: 'auto', sorttype: 'date', formatter: 'date', formatoptions: { srcformat: 'ISO8601Long', newformat: 'd/m/Y' } }
        ],
        pager: $("#LokSabhaTermDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_LS_TERM',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '650',
        viewrecords: true,
        caption: 'Lok Sabha Term Details',
        loadComplete: function () {
            $('#LokSabhaTermDetailsTable_rn').html('Sr.<br/>No.');

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