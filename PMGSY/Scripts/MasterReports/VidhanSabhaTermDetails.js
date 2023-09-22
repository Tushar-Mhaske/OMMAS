
$(function () {

    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_VidhanSabhaTermDetails").attr("disabled", "disabled");
    }
    $("#VidhanSabhaTermDetailsButton").click(function () {
        var stateCode = $("#StateList_VidhanSabhaTermDetails").val();
        VidhanSabhaTermReportsListing(stateCode);
    });
    $("#VidhanSabhaTermDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});



function VidhanSabhaTermReportsListing(stateCode) {
    $("#VidhanSabhaTermDetailsTable").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#VidhanSabhaTermDetailsTable").jqGrid({
        url: '/MasterReports/VidhanSabhaTermDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name','Term','Start Date','End Date'],
        colModel: [
            { name: "MAST_STATE_NAME", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_VS_TERM", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_VS_START_DATE", width: 150, align: 'left', height: 'auto', formatter: 'date', formatoptions: { srcformat: 'ISO8601Long', newformat: 'd/m/Y' } },
            { name: "MAST_VS_END_DATE", width: 150, align: 'left', height: 'auto', formatter: 'date', formatoptions: { srcformat: 'ISO8601Long', newformat: 'd/m/Y' } }
        ],
        postData: { "StateCode": stateCode },
        pager: $("#VidhanSabhaTermDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'Vidhan Sabha Term Details',
        loadComplete: function () {
            $('#VidhanSabhaTermDetailsTable_rn').html('Sr.<br/>No.');
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