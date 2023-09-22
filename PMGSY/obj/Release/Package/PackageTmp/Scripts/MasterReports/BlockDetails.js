
$(document).ready(function () {
   
    if ($("#MAST_STATE_CODE").val() > 0) {
       
        $("#StateList_BlockDetails").attr("disabled", "disabled");
    }
   
    $("#StateList_BlockDetails").change(function () {
        $("#DistrictList_BlockDetails").val(0);
        $("#DistrictList_BlockDetails").empty();
        if ($(this).val() > 0) {
            if ($("#DistrictList_BlockDetails").length > 0) {
                $.ajax({
                    url: '/MasterReports/BlockDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_BlockDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#DistrictList_BlockDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                        if ($("#MAST_DISTRICT_CODE").val() > 0) {
                            $("#DistrictList_BlockDetails").val($("#MAST_DISTRICT_CODE").val());
                            $("#DistrictList_BlockDetails").attr("disabled", "disabled");
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
            $("#DistrictList_BlockDetails").append("<option value='0'>All Districts</option>");
        }
    });

    $("#BlockDetailsButton").click(function () {
        var stateCode = $("#StateList_BlockDetails").val();
        var districtCode = $("#DistrictList_BlockDetails").val();
        var isDesert = $("#IS_DESERT_BlockDetails").val();
        var isTribal = $("#IS_TRIBAL_BlockDetails").val();
        var pmgsyIncluded = $("#PMGSY_INCLUDED_BlockDetails").val();
        var schedule5 = $("#IS_SCHEDULE5_BlockDetails").val();
        var activeType = $("#ActiveType_BlockDetails").val();

        BlockMasterReportsListing(districtCode, stateCode, isDesert, isTribal, pmgsyIncluded, schedule5, activeType);
    });

    $("#StateList_BlockDetails").trigger('change');
    $("#BlockDetailsButton").trigger('click');
    //setTimeout(function () {
       
    //    $("#BlockDetailsButton").trigger('click');
    //}, 2000);

  
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});


function BlockMasterReportsListing(districtCode, stateCode, isDesert, isTribal, pmgsyIncluded, schedule5, activeType) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
   
    $("#BlockDetailsTable").jqGrid('GridUnload');

    $("#BlockDetailsTable").jqGrid({
        url: '/MasterReports/BlockDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name','District Name','Block Name', 'Is Desert', 'Is Tribal', 'PMGSY Included', 'Is Schedule5','Active'],
        colModel: [
            { name: 'MAST_STATE_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_DISTRICT_NAME', width: 200, align: 'left', height: 'auto' },
            { name: 'MAST_BLOCK_NAME', width: 200, align: 'left', height: 'auto' },
            { name: 'MAST_IS_DESERT', width: 100, align: 'left', height: 'auto' },
            { name: 'MAST_IS_TRIBAL', width: 100, align: 'left', height: 'auto' },
            { name: 'MAST_PMGSY_INCLUDED', width: 100, align: 'left', height: 'auto' },
            { name: 'MAST_SCHEDULE5', width: 100, align: 'left', height: 'auto' },
            { name: 'MAST_BLOCK_ACTIVE', width: 100, align: 'left', height: 'auto' },

        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "IS_DESERT": isDesert, "IS_TRIBAL": isTribal, "PMGSY_INCLUDED": pmgsyIncluded, "IS_SCHEDULE5": schedule5, "ActiveType": activeType },
        pager: $("#BlockDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '550',
        viewrecords: true,
        caption: 'Block Details',
        loadComplete: function () {
            $('#BlockDetailsTable_rn').html('Sr.<br/>No.');
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