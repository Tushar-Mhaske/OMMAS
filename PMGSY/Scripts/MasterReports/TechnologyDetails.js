
$(function () {
    TechnologyReportsListing();
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');


});

function TechnologyReportsListing() {
    $("#TechnologyDetailsTable").jqGrid({
        url: '/MasterReports/TechnologyDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Technology Name', 'Technology Description', 'Technology Status'],
        colModel: [
            { name: "MAST_TECH_NAME", width: '200', align: 'center', height: 'auto' },
            { name: "MAST_TECH_DESC", width: '200', align: 'center', height: 'auto' },
            { name: "MAST_TECH_STATUS", width: '200', align: 'center', height: 'auto' }        
        ],
        pager: $("#TechnologyDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_TECH_NAME',
        sortorder: 'asc',
        recordtext: '{2} records found',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '650',
        viewrecords: true,
        caption: 'Technology Report Details',
        loadComplete: function () {
            $('#TechnologyDetailsTable_rn').html('Sr.<br/>No.');
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