
$(function () {
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

    FundingAgencyReportsListing();
});

function FundingAgencyReportsListing() {
    $("#FundingAgencyDetailsTable").jqGrid({
        url: '/MasterReports/FundingAgencyDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Funding Agency Name'],
        colModel:[
            {name: "MAST_FUNDING_AGENCY_NAME", width: 1050, align: 'left', height: 'auto'},
        ],
        pager: $("#FundingAgencyDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_FUNDING_AGENCY_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: false,
        width: 1100,
        shrinkToFit: false,
        height: '650',
        viewrecords: true,
        caption: 'FundingAgency Details',
        loadComplete: function () {
            $('#FundingAgencyDetailsTable_rn').html('Sr.<br/>No.');
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