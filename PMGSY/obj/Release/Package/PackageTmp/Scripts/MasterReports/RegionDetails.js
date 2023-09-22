$(document).ready(function () {

    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_RegionDetails").attr("disabled", "disabled");
    }
    $("#RegionDetailsButton").click(function () {
        var stateCode = $("#StateList_RegionDetails").val();
        var activeType = $("#ActiveType_RegionDetails").val();
        RegionDetailsListing(stateCode, activeType);
    });
    $("#RegionDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});



function RegionDetailsListing(stateCode, activeType) {
    $("#RegionDetailsTable").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#RegionDetailsTable").jqGrid({
        url: '/MasterReports/RegionDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name','Region Name','Active'],
        colModel: [
            { name: 'MAST_STATE_NAME', width: 250, align: 'left', height: 'auto' },
            { name: 'MAST_REGION_NAME', width: 250, align: 'left', height: 'auto', sortable: true },
            { name: 'MAST_REGION_ACTIVE', width: 100, align: 'left', height: 'auto', sortable: true }

        ],
        postData: { "StateCode": stateCode, "ActiveType": activeType },
        pager: $("#RegionDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'Region Details',
        loadComplete: function () {
            $('#RegionDetailsTable_rn').html('Sr.<br/>No.');
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