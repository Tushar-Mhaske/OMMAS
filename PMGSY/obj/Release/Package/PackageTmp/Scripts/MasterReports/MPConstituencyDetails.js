$(document).ready(function () {
    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_MPConstituencyDetails").attr("disabled", "disabled");
    }
    $("#MPConstituencyDetailsButton").click(function () {
        var stateCode = $("#StateList_MPConstituencyDetails").val();
        var activeType = $("#ActiveType_MPConstituencyDetails").val();
        MPConstituencyReportsListing(stateCode, activeType);
    });
    $("#MPConstituencyDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});



function MPConstituencyReportsListing(stateCode, activeType) {
    $("#MPConstituencyDetailsTable").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#MPConstituencyDetailsTable").jqGrid({
        url: '/MasterReports/MPConstituencyListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name','MP Constituency Name','Active'],
        colModel: [
            { name: 'MAST_STATE_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_MP_CONST_NAME', width: 300, align: 'left', height: 'auto' },
             { name: 'MAST_MP_CONST_ACTIVE', width: 300, align: 'left', height: 'auto' }
        ],
        postData: { "StateCode": stateCode, "ActiveType": activeType },
        pager: $("#MPConstituencyDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'MP Constituency Details',
        loadComplete: function () {
            $('#MPConstituencyDetailsTable_rn').html('Sr.<br/>No.');
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