
$(document).ready(function () {
    if ($("#MAST_STATE_CODE").val() > 0) {
        $("#StateList_VillageDetails").attr("disabled", "disabled");
    }
  
    $("#StateList_VillageDetails").change(function () {
       
        $("#DistrictList_VillageDetails").val(0);
        $("#DistrictList_VillageDetails").empty();
        $("#BlockList_VillageDetails").val(0);
        $("#BlockList_VillageDetails").empty();
        $("#BlockList_VillageDetails").append("<option value='0'>All Blocks</option>");

        //$("#DistrictList").append("<option value='0'>Select District</option>");
        if ($(this).val() > 0) {
            if ($("#DistrictList_VillageDetails").length > 0) {
                $.ajax({
                    url: '/MasterReports/VillageDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_VillageDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#DistrictList_VillageDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                        if ($("#MAST_DISTRICT_CODE").val() > 0) {
                            $("#DistrictList_VillageDetails").val($("#MAST_DISTRICT_CODE").val());
                            $("#DistrictList_VillageDetails").attr("disabled", "disabled");
                            $("#DistrictList_VillageDetails").trigger('change');
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
            $("#DistrictList_VillageDetails").append("<option value='0'>All Districts</option>");
            $("#BlockList_VillageDetails").val(0);
            $("#BlockList_VillageDetails").empty();
            $("#BlockList_VillageDetails").append("<option value='0'>All Blocks</option>");
        }
    });

    $("#DistrictList_VillageDetails").change(function () {
        $("#BlockList_VillageDetails").val(0);
        $("#BlockList_VillageDetails").empty();
        //$("#DistrictList").append("<option value='0'>Select District</option>");
        if ($(this).val() > 0) {
            if ($("#BlockList_VillageDetails").length > 0) {
                $.ajax({
                    url: '/MasterReports/VillageDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_VillageDetails").val(), "DistrictCode": $("#DistrictList_VillageDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#BlockList_VillageDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            $("#BlockList_VillageDetails").append("<option value='0'>All Blocks</option>");
        }
    });

    $("#VillageDetailsButton").click(function () {
        var stateCode = $("#StateList_VillageDetails").val();
        var districtCode = $("#DistrictList_VillageDetails").val();
        var blockCode = $("#BlockList_VillageDetails").val();
        var isSchedule5 = $("#IS_SCHEDULE5_VillageDetails").val();
        var censusYear = $("#CENSUS_YEAR_VillageDetails").val();
        var activeType = $("#ActiveType_VillageDetails").val();

        VillageMasterReportsListing(censusYear, blockCode, districtCode, stateCode, isSchedule5, activeType);
    });
    $('#StateList_VillageDetails').trigger('change'); //change 11/12/2013
    $("#VillageDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
 
});




function VillageMasterReportsListing(censusYear, blockCode, districtCode, stateCode, isSchedule5, activeType) {
   
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#VillageDetailsTable").jqGrid('GridUnload');

    $("#VillageDetailsTable").jqGrid({
        url: '/MasterReports/VillageDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name','District Name','Block Name','Village Name', 'Is Schedule5', 'SC/ST Population', 'Total Population','Active'],
        colModel: [
            { name: 'MAST_STATE_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_DISTRICT_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_BLOCK_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_VILLAGE_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_SCHEDULE5', width: 100, align: 'left', height: 'auto' },
            { name: 'VSCST_POP', width: 100, align: 'left', height: 'auto', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'VTOT_POP', width: 100, align: 'left', height: 'auto', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MAST_VILLAGE_ACTIVE', width: 100, align: 'left', height: 'auto' },

        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode, "IS_SCHEDULE5": isSchedule5, "CENSUS_YEAR": censusYear, "ActiveType": activeType },
        pager: $("#VillageDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        footerrow: true,
        rownumbers: true,
        autowidth: true,
        height: '550',
        viewrecords: true,
        caption: 'Village Reports',
        loadComplete: function () {
            var VSCST_POP_T = $(this).jqGrid('getCol', 'VSCST_POP', false, 'sum');
            var VTOT_POP_T = $(this).jqGrid('getCol', 'VTOT_POP', false, 'sum');
            $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { VSCST_POP: VSCST_POP_T }, true);
            $(this).jqGrid('footerData', 'set', { VTOT_POP: VTOT_POP_T }, true);
            $('#VillageDetailsTable_rn').html('Sr.<br/>No.');

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