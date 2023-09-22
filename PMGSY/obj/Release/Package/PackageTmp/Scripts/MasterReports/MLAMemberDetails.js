
$(document).ready(function () {

    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_MLAMemberDetails").attr("disabled", "disabled");
    }
    $("#StateList_MLAMemberDetails").change(function () {

        $("#ConstituencyList_MLAMemberDetails").val(0);
        $("#ConstituencyList_MLAMemberDetails").empty();
        $("#TermList_MLAMemberDetails").val(0);
        $("#TermList_MLAMemberDetails").empty();
        if ($(this).val() > 0) {
            if ($("#ConstituencyList_MLAMemberDetails").length > 0) {
                $.ajax({
                    url: '/MasterReports/MLAMemberDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_MLAMemberDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#ConstituencyList_MLAMemberDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
            if ($("#TermList_MLAMemberDetails").length > 0) {

                $.ajax({
                    url: '/MasterReports/MLAMemberDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_MLAMemberDetails").val(), "ConstituencyFlag": "1" },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#TermList_MLAMemberDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            $("#ConstituencyList_MLAMemberDetails").append("<option value='0'>All Constituencies</option>");
            $("#TermList_MLAMemberDetails").append("<option value='0'>All Terms</option>");
        }
    });

    $("#MLAMemberDetailsButton").click(function () {
        var stateCode = $("#StateList_MLAMemberDetails").val();
        var constituency = $("#ConstituencyList_MLAMemberDetails").val();
        var term = $("#TermList_MLAMemberDetails").val();
        var activeType = $("#ActiveType_MLAMemberDetails").val();
        MLAMemberReportsListing(constituency, term, stateCode, activeType);
    });

    $("#StateList_MLAMemberDetails").trigger('change');
    $("#MLAMemberDetailsButton").trigger('click');

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
   

 
});





function MLAMemberReportsListing(constituency, term, stateCode, activeType) {
    $("#MLAMemberDetailsTable").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#MLAMemberDetailsTable").jqGrid({
        url: '/MasterReports/MLAMemberDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name','Constituency Name','Term','Term Start Date','Term End Date','Member Name','Party Name','Start Date','End Date','Active'],
        colModel: [
            { name: "MAST_STATE_NAME", width: 150, align: 'left', height: 'auto' },
            {name: "MAST_MLA_CONST_NAME", width: 150, align: 'left', height: 'auto'},
            {name: "MAST_VS_TERM", width: 150, align: 'left', height: 'auto'},
            {name: "MAST_VS_START_DATE", width: 150, align: 'left', height: 'auto',formatter: 'date', formatoptions: { srcformat: 'ISO8601Long', newformat: 'd/m/Y' } },
            { name: "MAST_VS_END_DATE", width: 150, align: 'left', height: 'auto', formatter: 'date', formatoptions: { srcformat: 'ISO8601Long', newformat: 'd/m/Y' } },
            {name: "MAST_MEMBER", width: 150, align: 'left', height: 'auto'},
            {name: "MAST_MEMBER_PARTY", width: 130, align: 'left', height: 'auto'},
            {name: "MAST_MEMBER_START_DATE", width: 90, align: 'left', height: 'auto',formatter: 'date', formatoptions: { srcformat: 'ISO8601Long', newformat: 'd/m/Y' } },
            { name: "MAST_MEMBER_END_DATE", width: 90, align: 'left', height: 'auto', formatter: 'date', formatoptions: { srcformat: 'ISO8601Long', newformat: 'd/m/Y' } },
            { name: "MAST_MLA_CONST_ACTIVE", width: 70, align: 'left', height: 'auto' }
          
        ],
        postData: { "StateCode": stateCode, "MLAConstituency": constituency, "Term": term, "ActiveType": activeType },
        pager: $("#MLAMemberDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'MLA Member Details',
        loadComplete: function () {
            $('#MLAMemberDetailsTable_rn').html('Sr.<br/>No.');
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