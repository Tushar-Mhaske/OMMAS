$(function () {

});
$(document).ready(function () {

    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_ExecFinProgressDetails").attr("disabled", "disabled");
    }
    $("#StateList_ExecFinProgressDetails").change(function () {

        $("#DistrictList_ExecFinProgressDetails").val(0);
        $("#DistrictList_ExecFinProgressDetails").empty();
        $("#BlockList_ExecFinProgressDetails").val(0);
        $("#BlockList_ExecFinProgressDetails").empty();
        $("#BlockList_ExecFinProgressDetails").append("<option value='0'>All Blocks</option>");
        // $("#DistrictList").append("<option value='0'>Select District</option>");

        if ($(this).val() > 0) {

            if ($("#DistrictList_ExecFinProgressDetails").length > 0) {
                $.ajax({
                    url: '/ProposalReports/AllDistrictDetails',
                    type: 'POST',
                    data: { "StateCode": $(this).val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#DistrictList_ExecFinProgressDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                        //$('#DistrictList_ExecFinProgressDetails').find("option[value='0']").remove();
                        //$("#DistrictList_ExecFinProgressDetails").append("<option value='0'>All District</option>");
                        //$('#DistrictList_ExecFinProgressDetails').val(0);

                        //For Disable if District Login
                        if ($("#MAST_DISTRICT_CODE").val() > 0) {
                            $("#DistrictList_ExecFinProgressDetails").val($("#MAST_DISTRICT_CODE").val());
                            $("#DistrictList_ExecFinProgressDetails").attr("disabled", "disabled");
                            $("#DistrictList_ExecFinProgressDetails").trigger('change');

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

            $("#DistrictList_ExecFinProgressDetails").append("<option value='0'>All District</option>");
            $("#BlockList_ExecFinProgressDetails").empty();
            $("#BlockList_ExecFinProgressDetails").append("<option value='0'>All Block</option>");

        }
    });

    $("#DistrictList_ExecFinProgressDetails").change(function () {

        $("#BlockList_ExecFinProgressDetails").val(0);
        $("#BlockList_ExecFinProgressDetails").empty();
       

        if ($(this).val() > 0) {
            if ($("#BlockList_ExecFinProgressDetails").length > 0) {
                $.ajax({
                    url: '/ProposalReports/AllBlockDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_ExecFinProgressDetails").val(), "DistrictCode": $("#DistrictList_ExecFinProgressDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#BlockList_ExecFinProgressDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }

                        //$('#BlockList_ExecFinProgressDetails').find("option[value='0']").remove();
                        //$("#BlockList_ExecFinProgressDetails").append("<option value='0'>All Block</option>");
                        //$('#BlockList_ExecFinProgressDetails').val(0);


                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        } else {
            $("#BlockList_ExecFinProgressDetails").append("<option value='0'>All Block</option>");
        }
    });

    $("#ExecFinProgressDetailsButton").click(function () {
        var stateCode = $("#StateList_ExecFinProgressDetails").val();
        var districtCode = $("#DistrictList_ExecFinProgressDetails").val();
        var blockCode = $("#BlockList_ExecFinProgressDetails").val();
        var year = $("#YearList_ExecFinProgressDetails").val();
        var batch = $("#BatchList_ExecFinProgressDetails").val();
        var collaboration = $("#CollaborationList_ExecFinProgressDetails").val();
        var type = $("#TypeList_ExecFinProgressDetails").val();


        ExecFinProgressListing(blockCode, districtCode, stateCode, year, batch, collaboration, type);
    });


    $("#StateList_ExecFinProgressDetails").trigger('change');

    // $("#ExecFinProgressDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});


function ExecFinProgressListing(blockCode, districtCode, stateCode, year, batch, collaboration, type) {
    if (stateCode > 0) {

        LoadExecFinProgressGrid(blockCode, districtCode, stateCode, year, batch, collaboration, type);
    }
    else {
        alert("Please Select State");
    }

}

function LoadExecFinProgressGrid(blockCode, districtCode, stateCode, year, batch, collaboration, type) {
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
    $("#tbExecFinProgressDetails").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbExecFinProgressDetails").jqGrid({
        url: '/ProposalReports/ExecutionFinancialProgressReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Block Name', 'Sanctioned Year', 'Package No.', propName, propLength, 'Sanction Cost', 'Maintenance Cost (Proposal)', 'Value of Work', 'Payment Made', 'Progress as on', 'YES / No', 'Date (DD/MM/YYYY)'],
        colModel: [
            { name: 'MAST_DISTRICT_NAME', width: 150, align: 'left', height: 'auto', sortable: false },
            { name: 'MAST_BLOCK_NAME', width: 150, align: 'left', height: 'auto', sortable: false, summaryType: 'count', summaryTpl: '<b>Total {0}  </b>' },
            { name: 'IMS_YEAR', width: 120, align: 'center', height: 'auto', sortable: false },
            { name: 'IMS_PACKAGE_ID', width: 150, align: 'left', height: 'auto', sortable: false },
            { name: 'IMS_ROAD_NAME', width: 200, align: 'left', height: 'auto', sortable: false },
            { name: 'RoadLength', width: 120, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } },
            { name: 'SanctinedCost', width: 120, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2, defaulValue: "N.A" } },
            { name: 'MaintCost', width: 120, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2, defaulValue: "N.A" } },
            { name: 'ValueOfWork', width: 120, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2, defaulValue: "N.A" } },
            { name: 'PaymentMade', width: 120, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2, defaulValue: "N.A" } },
            { name: 'ProgressAsOn', width: 120, align: 'right', height: 'auto', sortable: false },
            { name: 'ISFinalPaymentYesNo', width: 120, align: 'center', height: 'auto', sortable: false },
            { name: 'IsFinalPayementDate', width: 120, align: 'center', height: 'auto', sortable: false },

        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode, "Year": year, "Batch": batch, "Collaboration": collaboration, "Type": type, "Progress": "R" },
        pager: $("#dvExecFinProgressDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_BLOCK_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '490',
        viewrecords: true,
        footerrow: true,
        caption: 'Financial Progress Details',
        grouping: true,
        groupingView: {
            groupField: ['MAST_DISTRICT_NAME'],
            groupSummary: [true],
            groupColumnShow: [false],
            groupDataSorted: true,
            showSummaryOnHide: true,
            //   groupText: ['<b>{0}- {1} </b>'],
            groupCollapse: true,
        },
        loadComplete: function () {

            //Total of Columns
            var RoadLength_T = $(this).jqGrid('getCol', 'RoadLength', false, 'sum');
            RoadLength_T = parseFloat(RoadLength_T).toFixed(3);
            var SanctinedCost_T = $(this).jqGrid('getCol', 'SanctinedCost', false, 'sum');
            SanctinedCost_T = parseFloat(SanctinedCost_T).toFixed(2);
            var MaintCost_T = $(this).jqGrid('getCol', 'MaintCost', false, 'sum');
            MaintCost_T = parseFloat(MaintCost_T).toFixed(2);
            var ValueOfWork_T = $(this).jqGrid('getCol', 'ValueOfWork', false, 'sum');
            ValueOfWork_T = parseFloat(ValueOfWork_T).toFixed(2);
            var PaymentMade_T = $(this).jqGrid('getCol', 'PaymentMade', false, 'sum');
            PaymentMade_T = parseFloat(PaymentMade_T).toFixed(2);


            $(this).jqGrid('footerData', 'set', { MAST_BLOCK_NAME: '<b>Totals</b>' });
            $(this).jqGrid('footerData', 'set', { RoadLength: RoadLength_T }, true);
            $(this).jqGrid('footerData', 'set', { SanctinedCost: SanctinedCost_T }, true);
            $(this).jqGrid('footerData', 'set', { MaintCost: MaintCost_T }, true);
            $(this).jqGrid('footerData', 'set', { ValueOfWork: ValueOfWork_T }, true);
            $(this).jqGrid('footerData', 'set', { PaymentMade: PaymentMade_T }, true);

            $('#tbExecFinProgressDetails_rn').html('Sr.<br/>No.');
            $("#dvExecFinProgressDetailsPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Amount Rs in Lacs & Length in Kms.</font>");

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

    $("#tbExecFinProgressDetails").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'ISFinalPaymentYesNo', numberOfColumns: 2, titleText: '<em> Is Final Payment Made</em>' },
        ]
    });
}

