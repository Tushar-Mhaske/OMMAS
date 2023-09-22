
$(function () {
    TaxesReportsListing();
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

    
});

function TaxesReportsListing() {
    $("#TaxesDetailsTable").jqGrid({
        url: '/MasterReports/TaxesDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['TDS','SC','Effective Date'],
        colModel:[
            {name: "MAST_TDS", width: 150, align: 'right', height: 'auto'},
            {name: "MAST_TDS_SC", width: 150, align: 'right', height: 'auto'},
            {name: "MAST_EFFECTIVE_DATE", width: 150, align: 'right', height: 'auto'},
        ],
        pager: $("#TaxesDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_TDS',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '650',
        viewrecords: true,
        caption: 'Taxes Details',
        loadComplete: function () {
            $('#TaxesDetailsTable_rn').html('Sr.<br/>No.');
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