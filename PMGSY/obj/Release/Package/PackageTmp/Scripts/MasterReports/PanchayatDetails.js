$(function () {
   
});
$(document).ready(function () {

    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_PanchayatDetails").attr("disabled", "disabled");
    }
    $("#StateList_PanchayatDetails").change(function () {

        $("#DistrictList_PanchayatDetails").val(0);
        $("#DistrictList_PanchayatDetails").empty();
        $("#BlockList_PanchayatDetails").val(0);
        $("#BlockList_PanchayatDetails").empty();
        $("#BlockList_PanchayatDetails").append("<option value='0'>All Blocks</option>");
        //$("#DistrictList").append("<option value='0'>Select District</option>");

        if ($(this).val() > 0) {
            if ($("#DistrictList_PanchayatDetails").length > 0) {
                $.ajax({
                    url: '/MasterReports/PanchayatDetails',
                    type: 'POST',
                    data: { "StateCode": $(this).val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#DistrictList_PanchayatDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                        if ($("#MAST_DISTRICT_CODE").val() > 0) {
                            $("#DistrictList_PanchayatDetails").val($("#MAST_DISTRICT_CODE").val());
                            $("#DistrictList_PanchayatDetails").attr("disabled", "disabled");
                            $("#DistrictList_PanchayatDetails").trigger('change');
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

            $("#DistrictList_PanchayatDetails").append("<option value='0'>Select District</option>");
            $("#BlockList_PanchayatDetails").empty();
            $("#BlockList_PanchayatDetails").append("<option value='0'>All Blocks</option>");

        }
    });

    $("#DistrictList_PanchayatDetails").change(function () {

        $("#BlockList_PanchayatDetails").val(0);
        $("#BlockList_PanchayatDetails").empty();

        //$("#BlockList").append("<option value='0'>Select Block</option>");

        if ($(this).val() > 0) {
            if ($("#BlockList_PanchayatDetails").length > 0) {
                $.ajax({
                    url: '/MasterReports/PanchayatDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_PanchayatDetails").val(), "DistrictCode": $("#DistrictList_PanchayatDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#BlockList_PanchayatDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        } else {
            $("#BlockList_PanchayatDetails").append("<option value='0'>All Block</option>");
        }
    });

    $("#PanchayatDetailsButton").click(function () {
        var stateCode = $("#StateList_PanchayatDetails").val();
        var districtCode = $("#DistrictList_PanchayatDetails").val();
        var blockCode = $("#BlockList_PanchayatDetails").val();
        var activeType = $("#ActiveType_PanchayatDetails").val();

        PanchayatDetailsListing(blockCode, districtCode, stateCode, activeType);
    });


    $("#StateList_PanchayatDetails").trigger('change');

    $("#PanchayatDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});


function PanchayatDetailsListing(blockCode, districtCode, stateCode, activeType) {
    $("#PanchayatDetailsTable").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#PanchayatDetailsTable").jqGrid({
        url: '/MasterReports/PanchayatListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name','District Name','Block Name','Panchayat Name','Active'],
        colModel: [
            { name: 'MAST_STATE_NAME', width: 300, align: 'left', height: 'auto' , sortable: true},
            { name: 'MAST_DISTRICT_NAME', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'MAST_BLOCK_NAME', width: 250, align: 'left', height: 'auto', sortable: true },
            { name: 'MAST_PANCHAYAT_NAME', width: 250, align: 'left', height: 'auto', sortable: true },
            { name: 'MAST_PANCHAYAT_ACTIVE', width: 100, align: 'left', height: 'auto', sortable: true }
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode, "ActiveType": activeType },
        pager: $("#PanchayatDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'Panchayat Details',
        loadComplete: function () {
            $('#PanchayatDetailsTable_rn').html('Sr.<br/>No.');
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