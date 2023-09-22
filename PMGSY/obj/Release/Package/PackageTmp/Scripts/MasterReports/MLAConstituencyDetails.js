$(function () {

    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_MLAConstituencyDetails").attr("disabled", "disabled");
    }
    $("#MLAConstituencyDetailsButton").click(function () {
        var stateCode = $("#StateList_MLAConstituencyDetails").val();
        var activeType = $("#ActiveType_MLAConstituencyDetails").val();

        MLAConstituencyDetailsListing(stateCode, activeType);
    });
    $("#MLAConstituencyDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});




function MLAConstituencyDetailsListing(stateCode, activeType)
{
    $("#MLAConstituencyDetailsTable").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#MLAConstituencyDetailsTable").jqGrid({
        url: '/MasterReports/MLAConstituencyListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name','MLA Constituency Name','Active'],
        colModel: [
            { name: 'MAST_STATE_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_MLA_CONST_NAME', width: 300, align: 'left', height: 'auto' },
             { name: 'MAST_MLA_CONST_ACTIVE', width: 300, align: 'left', height: 'auto' }
        ],
        postData: { "StateCode": stateCode, "ActiveType": activeType },
        pager: $("#MLAConstituencyDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'MLA Constituency Details',
        loadComplete: function () {
            $('#MLAConstituencyDetailsTable_rn').html('Sr.<br/>No.');
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