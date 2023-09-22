
$(document).ready(function () {

    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_MPMemberDetails").attr("disabled", "disabled");
    }
    $("#StateList_MPMemberDetails").change(function () {
        $("#ConstituencyList_MPMemberDetails").val(0);
        $("#ConstituencyList_MPMemberDetails").empty();
        $("#TermList_MPMemberDetails").val(0);
        $("#TermList_MPMemberDetails").empty();
        if ($(this).val() > 0) {
            if ($("#ConstituencyList_MPMemberDetails").length > 0) {
                $.ajax({
                    url: '/MasterReports/MPMemberDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_MPMemberDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#ConstituencyList_MPMemberDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
            if ($("#TermList_MPMemberDetails").length > 0) {
                $.ajax({
                    url: '/MasterReports/MPMemberDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_MPMemberDetails").val(), "ConstituencyFlag": "1" },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#TermList_MPMemberDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            $("#ConstituencyList_MPMemberDetails").append("<option value='0'>All Constituencies</option>");
            $("#TermList_MPMemberDetails").append("<option value='0'>All Terms</option>");
        }
    });

    $("#MPMemberDetailsButton").click(function () {
        var stateCode = $("#StateList_MPMemberDetails").val();
        var constituency = $("#ConstituencyList_MPMemberDetails").val();
        var term = $("#TermList_MPMemberDetails").val();
        var activeType = $("#ActiveType_MPMemberDetails").val();
        MPMemberReportsListing(constituency, term, stateCode, activeType);
    });

    $("#StateList_MPMemberDetails").trigger('change');
    $("#MPMemberDetailsButton").trigger('click');

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

});




function MPMemberReportsListing(constituency, term, stateCode, activeType) {
    $("#MPMemberDetailsTable").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#MPMemberDetailsTable").jqGrid({
        url: '/MasterReports/MPMemberDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name','Constituency Name', 'Term', 'Term Start Date', 'Term End Date', 'Member Name', 'Member Party', 'Start Date', 'End Date','Active'],
        colModel: [
            { name: "MAST_STATE_NAME", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_MP_CONST_NAME", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_LS_TERM", width: 150, align: 'right', height: 'auto' },
            { name: "MAST_VS_START_DATE", width: 150, align: 'left', height: 'auto', formatter: 'date', formatoptions: { srcformat: 'ISO8601Long', newformat: 'd/m/Y' } },
            { name: "MAST_VS_END_DATE", width: 150, align: 'left', height: 'auto', formatter: 'date', formatoptions: { srcformat: 'ISO8601Long', newformat: 'd/m/Y' } },
            { name: "MAST_MEMBER", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_MEMBER_PARTY", width: 130, align: 'left', height: 'auto' },
            { name: "MAST_MEMBER_START_DATE", width: 90, align: 'left', height: 'auto', formatter: 'date', formatoptions: { srcformat: 'ISO8601Long', newformat: 'd/m/Y' } },
            { name: "MAST_MEMBER_END_DATE", width: 90, align: 'left', height: 'auto', formatter: 'date', formatoptions: { srcformat: 'ISO8601Long', newformat: 'd/m/Y' } },
            { name: "MAST_MP_CONST_ACTIVE", width: 70, align: 'left', height: 'auto' },

        ],
        postData: { "StateCode": stateCode, "MPConstituency": constituency, "Term": term, "ActiveType": activeType },
        pager: $("#MPMemberDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'MP Member Details',
        loadComplete: function () {
            $('#MPMemberDetailsTable_rn').html('Sr.<br/>No.');
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