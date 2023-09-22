
$(function () {
    $("#AgencyDetailsButton").click(function () {
        var agencyType = $("#MAST_AGENCY_TYPE").val();
        AgencyReportsListing(agencyType);
    });
    $("#AgencyDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

    
});



function AgencyReportsListing(agencyType) {

    $("#AgencyDetailsTable").jqGrid('GridUnload');
    $("#AgencyDetailsTable").jqGrid({
        url: '/MasterReports/AgencyDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Agency Name','Agency Type'],
        colModel:[
            {name: "MAST_AGENCY_NAME", width: 150, align: 'left', height: 'auto'},
            {name: "MAST_AGENCY_TYPE", width: 150, align: 'left', height: 'auto'},
        ],
        postData: { "MAST_AGENCY_TYPE": agencyType },
        pager: $("#AgencyDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_AGENCY_NAME',
        sortorder: 'asc',
        rowNum: '2147483647',
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'Agency Details',
        loadComplete: function () {
            $('#AgencyDetailsTable_rn').html('Sr.<br/>No.');
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