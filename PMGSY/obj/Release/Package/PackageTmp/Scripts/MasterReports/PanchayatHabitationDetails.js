

$(document).ready(function () {
    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_PanchayatHabitationDetails").attr("disabled", "disabled");
    }
    $("#StateList_PanchayatHabitationDetails").change(function () {

        $("#DistrictList_PanchayatHabitationDetails").val(0);
        $("#DistrictList_PanchayatHabitationDetails").empty();

        //$("#DistrictList").append("<option value='0'>Select District</option>");

        if ($(this).val() > 0) {
            if ($("#DistrictList_PanchayatHabitationDetails").length > 0) {
                $.ajax({
                    url: '/MasterReports/PanchayatHabitationDetails',
                    type: 'POST',
                    data: { "StateCode": $(this).val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#DistrictList_PanchayatHabitationDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                        if ($("#MAST_DISTRICT_CODE").val() > 0) {
                            $("#DistrictList_PanchayatHabitationDetails").val($("#MAST_DISTRICT_CODE").val());
                            $("#DistrictList_PanchayatHabitationDetails").attr("disabled", "disabled");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        } else {
            $("#DistrictList_PanchayatHabitationDetails").append("<option value='0'>All Districts</option>");
            $("#BlockList_PanchayatHabitationDetails").val(0);
            $("#BlockList_PanchayatHabitationDetails").empty();
            $("#BlockList_PanchayatHabitationDetails").append("<option value='0'>All Blocks</option>");
            $("#PanchayatList_PanchayatHabitationDetails").val(0);
            $("#PanchayatList_PanchayatHabitationDetails").empty();
            $("#PanchayatList_PanchayatHabitationDetails").append("<option value='0'>All Panchayats</option>");
        }
    });

    $("#DistrictList_PanchayatHabitationDetails").change(function () {

        $("#BlockList_PanchayatHabitationDetails").val(0);
        $("#BlockList_PanchayatHabitationDetails").empty();

        if ($(this).val() > 0) {
            if ($("#BlockList_PanchayatHabitationDetails").length > 0) {
                $.ajax({
                    url: '/MasterReports/PanchayatHabitationDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_PanchayatHabitationDetails").val(), "DistrictCode": $("#DistrictList_PanchayatHabitationDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#BlockList_PanchayatHabitationDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            alert("Val=0");
            $("#PanchayatList_PanchayatHabitationDetails").empty();
            $("#PanchayatList_PanchayatHabitationDetails").append("<option value='0'>All Panchayats</option>");

        }
    });

    $("#BlockList_PanchayatHabitationDetails").change(function () {

        $("#PanchayatList_PanchayatHabitationDetails").val(0);
        $("#PanchayatList_PanchayatHabitationDetails").empty();

        //$("#PanchayatList_PanchayatHabitationDetails").append("<option value='0'>Select Panchayat</option>");


        if ($(this).val() > 0) {
            if ($("#PanchayatList_PanchayatHabitationDetails").length > 0) {
                $.ajax({
                    url: '/MasterReports/PanchayatHabitationDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_PanchayatHabitationDetails").val(), "DistrictCode": $("#DistrictList_PanchayatHabitationDetails").val(), "BlockCode": $("#BlockList_PanchayatHabitationDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#PanchayatList_PanchayatHabitationDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
          
            $("#PanchayatList_PanchayatHabitationDetails").empty();
            $("#PanchayatList_PanchayatHabitationDetails").append("<option value='0'>All Panchayats</option>");

       
        }

    });
    $("#PanchayatHabitationDetailsButton").click(function () {
        var stateCode = $("#StateList_PanchayatHabitationDetails").val();
        var districtCode = $("#DistrictList_PanchayatHabitationDetails").val();
        var blockCode = $("#BlockList_PanchayatHabitationDetails").val();
        var activeType = $("#ActiveType_PanchayatHabitationDetails").val();
        if (stateCode == 0) {
            alert("Please Select State");
            return;
        }
        if (districtCode == 0) {
            alert("Please Select District");
            return;
        }
        if (blockCode == 0) {
            alert("Please Select Block");
            return;
        }
        var panchayatCode = $("#PanchayatList_PanchayatHabitationDetails").val();
        PanchayatHabitationDetailsListing(panchayatCode, blockCode, districtCode, stateCode, activeType);
    });

    $("#StateList_PanchayatHabitationDetails").trigger('change');
   
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

});







function PanchayatHabitationDetailsListing(panchayatCode, blockCode, districtCode, stateCode, activeType)
{
    $("#PanchayatHabitationDetailsTable").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#PanchayatHabitationDetailsTable").jqGrid({
        url: '/MasterReports/PanchayatHabitationListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name','District Name','Block Name','Panchayat Name','Panchayat Habitation Name','Active'],
        colModel: [
            { name: 'MAST_STATE_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_DISTRICT_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_BLOCK_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_PANCHAYAT_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_HAB_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_PANCHAYAT_ACTIVE', width: 300, align: 'left', height: 'auto' }
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode, "PanchayatCode": panchayatCode, "ActiveType": activeType },
        pager: $("#PanchayatHabitationDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'Panchayat Habitation Details',
        loadComplete: function () {
            $('#PanchayatHabitationDetailsTable_rn').html('Sr.<br/>No.');
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