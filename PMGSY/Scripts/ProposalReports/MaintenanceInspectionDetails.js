$(function () {

});
$(document).ready(function () {

    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_MaintenInspectionDetails").attr("disabled", "disabled");
    }
    $("#StateList_MaintenInspectionDetails").change(function () {

        $("#DistrictList_MaintenInspectionDetails").val(0);
        $("#DistrictList_MaintenInspectionDetails").empty();
        $("#BlockList_MaintenInspectionDetails").val(0);
        $("#BlockList_MaintenInspectionDetails").empty();
        $("#BlockList_MaintenInspectionDetails").append("<option value='0'>All Block</option>");
        // $("#DistrictList").append("<option value='0'>Select District</option>");

        if ($(this).val() > 0) {

            if ($("#DistrictList_MaintenInspectionDetails").length > 0) {
                $.ajax({
                    url: '/ProposalReports/AllDistrictDetails',
                    type: 'POST',
                    data: { "StateCode": $(this).val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#DistrictList_MaintenInspectionDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                        //$('#DistrictList_MaintenInspectionDetails').find("option[value='0']").remove();
                        //$("#DistrictList_MaintenInspectionDetails").append("<option value='0'>Select District</option>");
                        //$('#DistrictList_MaintenInspectionDetails').val(0);

                        //For Disable if District Login
                        if ($("#MAST_DISTRICT_CODE").val() > 0) {
                            $("#DistrictList_MaintenInspectionDetails").val($("#MAST_DISTRICT_CODE").val());
                            $("#DistrictList_MaintenInspectionDetails").attr("disabled", "disabled");
                            $("#DistrictList_MaintenInspectionDetails").trigger('change');
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

            $("#DistrictList_MaintenInspectionDetails").append("<option value='0'>All District</option>");
            $("#BlockList_MaintenInspectionDetails").empty();
            $("#BlockList_MaintenInspectionDetails").append("<option value='0'>All Block</option>");

        }
    });

    $("#DistrictList_MaintenInspectionDetails").change(function () {

        $("#BlockList_MaintenInspectionDetails").val(0);
        $("#BlockList_MaintenInspectionDetails").empty();   

        if ($(this).val() > 0) {
            if ($("#BlockList_MaintenInspectionDetails").length > 0) {
                $.ajax({
                    url: '/ProposalReports/AllBlockDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_MaintenInspectionDetails").val(), "DistrictCode": $("#DistrictList_MaintenInspectionDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#BlockList_MaintenInspectionDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }

                        //$('#BlockList_MaintenInspectionDetails').find("option[value='0']").remove();
                        //$("#BlockList_MaintenInspectionDetails").append("<option value='0'>Select Block</option>");
                        //$('#BlockList_MaintenInspectionDetails').val(0);


                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        } else {
            $("#BlockList_MaintenInspectionDetails").append("<option value='0'>All Block</option>");
        }
    });

    $("#MaintenInspectionDetailsButton").click(function () {
        var stateCode = $("#StateList_MaintenInspectionDetails").val();
        var districtCode = $("#DistrictList_MaintenInspectionDetails").val();
        var blockCode = $("#BlockList_MaintenInspectionDetails").val();
        var year = $("#YearList_MaintenInspectionDetails").val();
        var batch = $("#BatchList_MaintenInspectionDetails").val();
        var collaboration = $("#CollaborationList_MaintenInspectionDetails").val();
        var type = $("#TypeList_MaintenInspectionDetails").val();


        MaintenInspectionListing(blockCode, districtCode, stateCode, year, batch, collaboration, type);
    });


    $("#StateList_MaintenInspectionDetails").trigger('change');

    // $("#MaintenInspectionDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});


function MaintenInspectionListing(blockCode, districtCode, stateCode, year, batch, collaboration, type) {
    if (stateCode > 0) {

        LoadMaintenInspectionGrid(blockCode, districtCode, stateCode, year, batch, collaboration, type);

    }
    else {
        alert("Please Select State");
    }

}

function LoadMaintenInspectionGrid(blockCode, districtCode, stateCode, year, batch, collaboration, type) {

    var propLength;
    var propName;
    if (type == 'P') {
        propLength = 'Road Length';
        propName = 'Road Name';
    }
    else if (type == 'L') {
        propLength = 'Bridge Length';
        propName = 'Bridge Name';
    }
    else {
        propLength = 'Road / Bridge Length';
        propName = 'Road / Bridge  Name';
    }
    $("#tbMaintenInspectionDetails").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbMaintenInspectionDetails").jqGrid({
        url: '/ProposalReports/MaintenanceInspectionReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: [propName, 'Package No.', 'Sanctioned Year', propLength, 'Inspection Date (DD/MM/YYYY)', 'Rectification Date (DD/MM/YYYY)'],
        colModel: [
            { name: 'IMS_ROAD_NAME', width: 300, align: 'left', height: 'auto', sortable: true },
            { name: 'IMS_PACKAGE_ID', width: 150, align: 'left', height: 'auto', sortable: false },
            { name: 'IMS_YEAR', width: 100, align: 'center', height: 'auto', sortable: false },
            { name: 'IMS_PAV_LENGTH', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'InspDate', width: 150, align: 'center', height: 'auto', sortable: false },
            { name: 'RectDate', width: 100, align: 'center', height: 'auto', sortable: false }
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode, "Year": year, "Batch": batch, "Collaboration": collaboration, "Type": type, "Progress": "M" },
        pager: $("#dvMaintenInspectionDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_BLOCK_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '520',
        viewrecords: true,
        footerrow: true,
        caption: 'Maintenance Inspection Details',
        loadComplete: function () {

            //Total of Columns
            var IMS_PAV_LENGTH_T = $(this).jqGrid('getCol', 'IMS_PAV_LENGTH', false, 'sum');
            IMS_PAV_LENGTH_T = parseFloat(IMS_PAV_LENGTH_T).toFixed(3);
            //var TN_LEN_T = $(this).jqGrid('getCol', 'TN_LEN', false, 'sum');
            //TN_LEN_T = parseFloat(TN_LEN_T).toFixed(2);


            $(this).jqGrid('footerData', 'set', { IMS_ROAD_NAME: '<b>Totals</b>' });
            $(this).jqGrid('footerData', 'set', { IMS_PAV_LENGTH: IMS_PAV_LENGTH_T }, true);
            //$(this).jqGrid('footerData', 'set', { TN_LEN: TN_LEN_T }, true);

            $('#tbMaintenInspectionDetails_rn').html('Sr.<br/>No.');
            $("#dvMaintenInspectionDetailsPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms</font>");

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
    });//End of Grid

    //$("#tbMaintenInspectionDetails").jqGrid('setGroupHeaders', {
    //    useColSpanStyle: false,
    //    groupHeaders: [
    //      { startColumnName: 'ISFinalPaymentYesNo', numberOfColumns: 2, titleText: '<em> Is Final Payment Made</em>' },
    //    ]
    //});
}
