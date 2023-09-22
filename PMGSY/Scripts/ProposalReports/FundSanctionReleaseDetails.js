$(document).ready(function () {
    $("#ddState_FundSanctRelDetails").focus();
    if ($("#MAST_STATE_CODE").val() > 0) {
        $("#ddState_FundSanctRelDetails").attr("disabled", "disabled");
    }
    $('#btnGoFundSanctRel').click(function () {
        var stateCode = $("#ddState_FundSanctRelDetails option:selected").val();
        var collaboration = $("#ddCollaboration_FundSanctRelDetails option:selected").val();
        var year = $("#ddYear_FundSanctRelDetails option:selected").val();
        var agency = $("#ddAgency_FundSanctRelDetails option:selected").val();
        var fundType = $("#ddFundType_FundSanctRelDetails option:selected").val();
        var releaseType = $("#ddReleaseType_FundSanctRelDetails option:selected").val();

        loadFundSanctRelDetails(stateCode, collaboration, year, agency, fundType, releaseType);
    });

  
    $('#btnGoFundSanctRel').trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});

function loadFundSanctRelDetails(stateCode, collaboration, year, agency, fundType, releaseType) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbFundSanctRelReport").jqGrid('GridUnload');
    jQuery("#tbFundSanctRelReport").jqGrid({
        url: '/ProposalReports/FundSanctionReleaseReportListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["Phase", "Installment No", "Financial Year", "Executing Agency", "Collaboration", "Amount Released [In Cr]", "Amount Released Date (DD/MM/YYYY)", "Sanctioned No."],
        colModel: [
            { name: "Phase", width: 200, align: 'left', height: 'auto', sortable: true },
            { name: "InstallNo", width: 200, align: 'right', height: 'auto', sortable: false },
            { name: "FinYear", width: 200, align: 'center', height: 'auto', sortable: false },
            { name: "ExecAgency", width: 150, align: 'left', height: 'auto', sortable: false },
            { name: "Collaboration", width: 100, align: 'left', height: 'auto', sortable: false },
            { name: "AmontRelased", width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "AmountReleasedDate", width: 120, align: 'center', height: 'auto', sortable: false },
            { name: "SanctionNo", width: 200, align: 'left', height: 'auto', sortable: false }
        ],
        postData: { "StateCode": stateCode, "Year": year, "Collaboration": collaboration, "Agency": agency, "FundType": fundType, "ReleaseType": releaseType },
        pager: jQuery('#dvFundSanctRelReportPager'),
        sortname: 'Phase',
        rowNum: 2147483647,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Fund Sanction Release Details",
        height: 520,
        autowidth: true,
        footerrow: true,
        rownumbers: true,
        loadComplete: function () {
            //Total of Columns
            var AmontRelasedT = $(this).jqGrid('getCol', 'AmontRelased', false, 'sum');
            AmontRelasedT = parseFloat(AmontRelasedT).toFixed(2);        

            ////

            $(this).jqGrid('footerData', 'set', { Phase: '<b>Totals</b>' });
            $(this).jqGrid('footerData', 'set', { AmontRelased: AmontRelasedT }, true);
            $('#tbFundSanctRelReport_rn').html('Sr.<br/>No.');

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



    }); //end of grid


    //$("#tbFundSanctRelReport").jqGrid('setGroupHeaders', {
    //    useColSpanStyle: false,
    //    groupHeaders: [
    //      { startColumnName: 'ConstrCompDate', numberOfColumns: 4, titleText: '<em> Contractor</em>' },
    //    ]
    //});
}
