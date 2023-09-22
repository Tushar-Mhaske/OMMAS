
$(document).ready(function () {
    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_RegionDistrictDetails").attr("disabled", "disabled");
    }
    $("#StateList_RegionDistrictDetails").change(function () {
        $("#RegionList_RegionDistrictDetails").val(0);
        $("#RegionList_RegionDistrictDetails").empty();
        if ($(this).val() > 0) {
            if ($("#RegionList_RegionDistrictDetails").length > 0) {
                $.ajax({
                    url: '/MasterReports/RegionDistrictDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_RegionDistrictDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#RegionList_RegionDistrictDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            $("#RegionList_RegionDistrictDetails").append("<option value='0'>All Regions</option>");
        }
    });

    $("#RegionDistrictDetailsButton").click(function () {
        var stateCode = $("#StateList_RegionDistrictDetails").val();
        var regionCode = $("#RegionList_RegionDistrictDetails").val();
        var activeType = $("#ActiveType_RegionDistrictDetails").val();
        RegionDistrictDetailsListing(regionCode, stateCode, activeType);
    });


    $("#StateList_RegionDistrictDetails").trigger('change');
    $("#RegionDistrictDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});


function RegionDistrictDetailsListing(regionCode, stateCode, activeType) {
    $("#RegionDistrictDetailsTable").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#RegionDistrictDetailsTable").jqGrid({
        url: '/MasterReports/RegionDistrictDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Region Name', 'District Name','Active'],
        colModel: [
            { name: 'MAST_STATE_NAME', width: 250, align: 'left', height: 'auto' },
            { name: 'MAST_REGION_NAME', width: 250, align: 'left', height: 'auto' },
            { name: 'MAST_DISTRICT_NAME', width: 250, align: 'left', height: 'auto' },
            { name: 'MAST_REGION_ACTIVE', width: 100, align: 'left', height: 'auto', sortable: true }

            
        ],
        postData: { "StateCode": stateCode, "RegionCode": regionCode, "ActiveType": activeType },
        pager: $("#RegionDistrictDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'Region District Details',
        loadComplete: function () {
            $('#RegionDistrictDetailsTable_rn').html('Sr.<br/>No.');
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