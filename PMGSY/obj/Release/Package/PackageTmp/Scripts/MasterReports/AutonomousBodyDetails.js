
$(function () {

    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_AutonomousBodyDetails").attr("disabled", "disabled");
    }

    $("#AutonomousBodyStateButton").click(function () {
        var stateCode = $("#StateList_AutonomousBodyDetails").val();

        AutonomousBodyReportsListing(stateCode);
    });
    $("#AutonomousBodyStateButton").trigger('click');

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

});



function AutonomousBodyReportsListing(stateCode) {
    $("#AutonomousBodyDetailsTable").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#AutonomousBodyDetailsTable").jqGrid({
        url: '/MasterReports/AutonomousBodyDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Autonomous Body'],
        colModel: [
            { name: "MAST_STATE_NAME", width: 150, align: 'left', height: 'auto' },
            { name: "ADMIN_AUTONOMOUS_BODY", width: 150, align: 'left', height: 'auto' },
        ],
        postData: { "StateCode": stateCode },
        pager: $("#AutonomousBodyDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'Autonomous Body Details',
        loadComplete: function () {
            $('#AutonomousBodyDetailsTable_rn').html('Sr.<br/>No.');

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