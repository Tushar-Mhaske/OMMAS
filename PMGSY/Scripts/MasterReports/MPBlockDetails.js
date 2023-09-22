$(document).ready(function () {

    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_MPBlockDetails").attr("disabled", "disabled");
    }
    $("#StateList_MPBlockDetails").change(function () {
        $("#MPConstituencyList_MPBlockDetails").val(0);
        $("#MPConstituencyList_MPBlockDetails").empty();
        if ($(this).val() > 0) {
            if ($("#MPConstituencyList_MPBlockDetails").length > 0) {
                $.ajax({
                    url: '/MasterReports/MPBlockDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_MPBlockDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#MPConstituencyList_MPBlockDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        }
        else {
            $("#MPConstituencyList_MPBlockDetails").append("<option value='0'>All Constituencies</option>");
        }
    });
    $("#StateList_MPBlockDetails").trigger('change');

    $("#MPBlockDetailsButton").click(function () {
        var stateCode = $("#StateList_MPBlockDetails").val();
        var constCode = $("#MPConstituencyList_MPBlockDetails").val();
        var activeType = $("#ActiveType_MPBlockDetails").val();
        MPBlockDetailsListing(constCode, stateCode, activeType);
    });

    $("#MPBlockDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

});





function MPBlockDetailsListing(constCode, stateCode, activeType)
{
    $("#MPBlockTable").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#MPBlockTable").jqGrid({
        url: '/MasterReports/MPBlockListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name','MP Constituency','MP Block Name','Active'],
        colModel: [
            { name: 'MAST_STATE_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_MP_CONST_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_BLOCK_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_MP_CONST_ACTIVE', width: 300, align: 'left', height: 'auto' }

        ],
        postData: { "StateCode": stateCode, "ConstituencyCode": constCode, "ActiveType": activeType },
        pager: $("#MPBlockDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'MP Block Details',
        loadComplete: function () {
            $('#MPBlockTable_rn').html('Sr.<br/>No.');
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