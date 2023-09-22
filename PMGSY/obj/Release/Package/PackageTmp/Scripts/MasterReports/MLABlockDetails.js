$(document).ready(function () {

    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_MLABlockDetails").attr("disabled", "disabled");
    }
    $("#StateList_MLABlockDetails").change(function () {
        $("#MLAConstituencyList_MLABlockDetails").val(0);
        $("#MLAConstituencyList_MLABlockDetails").empty();
        if ($(this).val() > 0) {
            if ($("#MLAConstituencyList_MLABlockDetails").length > 0) {
                $.ajax({
                    url: '/MasterReports/MLABlockDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_MLABlockDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#MLAConstituencyList_MLABlockDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            $("#MLAConstituencyList_MLABlockDetails").append("<option value='0'>All Constituencies</option>");
        }
    });

    $("#MLABlockDetailsButton").click(function () {
        var stateCode = $("#StateList_MLABlockDetails").val();
        var constCode = $("#MLAConstituencyList_MLABlockDetails").val();
        var activeType = $("#ActiveType_MLABlockDetails").val();
        MLABlockDetailsListing(constCode, stateCode, activeType);
    });
    $("#StateList_MLABlockDetails").trigger('change');
    $("#MLABlockDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});


function MLABlockDetailsListing(constCode, stateCode, activeType) {
    $("#MLABlockTable").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#MLABlockTable").jqGrid({
        url: '/MasterReports/MLABlockListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name','MLA Constituency','MLA Block Name','Active'],
        colModel: [
            { name: 'MAST_STATE_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_MLA_CONST_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_BLOCK_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_MLA_CONST_ACTIVE', width: 300, align: 'left', height: 'auto' }

        ],
        postData: { "StateCode": stateCode, "ConstituencyCode": constCode, "ActiveType": activeType },
        pager: $("#MLABlockDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'MLA Block Details',
        loadComplete: function () {
            $('#MLABlockTable_rn').html('Sr.<br/>No.');

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