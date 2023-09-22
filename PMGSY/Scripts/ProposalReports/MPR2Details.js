$(document).ready(function () {

    $("#btnMPR2Details").click(function () {

        var month = $("#ddMonth_MPR2Details").val();
        var year = $("#ddYear_MPR2Details").val();
        var collaboration = $("#ddAgency_MPR2Details").val();
        if ($("#hdnLevelId").val() == 6) //mord
        {

            MPR2StateReportListing(month, year, collaboration);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
            MPR2DistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), month, year, collaboration);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {

            MPR2BlockReportListing($("#MAST_DISTRICT_CODE").val(), $("#MAST_STATE_CODE").val(), $("#DISTRICT_NAME").val(), month, year, collaboration);
        }
    });

    $("#btnMPR2Details").trigger('click');
    //$('#tblRptContents').bind('resize', function () {
    //    resizeJqGrid();
    //}).trigger('resize');


});



/*       STATE REPORT LISTING       */
function MPR2StateReportListing(month, year, collaboration) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#MPR2StateReportTable").jqGrid('GridUnload');
    $("#MPR2DistrictReportTable").jqGrid('GridUnload');
    $("#MPR2BlockReportTable").jqGrid('GridUnload');
    $("#MPR2FinalReportTable").jqGrid('GridUnload');

    $("#MPR2StateReportTable").jqGrid({
        url: '/ProposalReports/MPR2StateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Target', 'Achievement During Month', 'Achievement During Year', '1000+', '999-500', '499-250 (Eligible)', '<250 (Incidental)', 'Total', '1000+', '999-500', '499-250 (Eligible)',
            '<250 (Incidental)', 'Total', '1000+', '999-500', '499-250 (Eligible)', '<250 (Incidental)', 'Total', 'Target for the Year', 'Upgradation', 'Renewal', 'Upgradation', 'Renewal'],
        colModel: [
            { name: "MAST_STATE_NAME", width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TN_LEN', index: 'TN_LEN', width: 100, align: 'right', sortable: false,  formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TNM_LEN', index: 'TNM_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TNY_LEN', index: 'TNY_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TPOP1000', index: 'TPOP1000', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TPOP999', index: 'TPOP999', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TPOP499', index: 'TPOP499', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TPOP250', index: 'TPOP250', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOT_TPOP', index: 'TOT_TPOP', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'POP1000', index: 'POP1000', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'POP999', index: 'POP999', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'POP499', index: 'POP499', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'POP250', index: 'POP250', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOT_POP', index: 'TOT_POP', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'YPOP1000', index: 'YPOP1000', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'YPOP999', index: 'YPOP999', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'YPOP499', index: 'YPOP499', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'YPOP250', index: 'YPOP250', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOT_YPOP', index: 'TOT_YPOP', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TU_LEN', index: 'TU_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TUM_LEN', index: 'TUM_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TRM_LEN', index: 'TRM_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TUY_LEN', index: 'TUY_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TRY_LEN', index: 'TRY_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        postData: { "Month": month, "Year": year, "Collaboration": collaboration },
        pager: $("#MPR2StateReportPager"),
        footerrow: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: false,
        height: '530',
        width: '1120',
        shrinkToFit: false,
        viewrecords: true,
        caption: 'State MPR2 Details',
        loadComplete: function () {
            //Total of Columns
            var TN_LEN_T = $(this).jqGrid('getCol', 'TN_LEN', false, 'sum');
            TN_LEN_T = parseFloat(TN_LEN_T).toFixed(3);
            var TNM_LEN_T = $(this).jqGrid('getCol', 'TNM_LEN', false, 'sum');
            TNM_LEN_T = parseFloat(TNM_LEN_T).toFixed(3);
            var TNY_LEN_T = $(this).jqGrid('getCol', 'TNY_LEN', false, 'sum');
            TNY_LEN_T = parseFloat(TNY_LEN_T).toFixed(3);
            var TPOP1000_T = $(this).jqGrid('getCol', 'TPOP1000', false, 'sum');
            var TPOP999_T = $(this).jqGrid('getCol', 'TPOP999', false, 'sum');
            var TPOP499_T = $(this).jqGrid('getCol', 'TPOP499', false, 'sum');
            var TPOP250_T = $(this).jqGrid('getCol', 'TPOP250', false, 'sum');
            var TOT_TPOP_T = $(this).jqGrid('getCol', 'TOT_TPOP', false, 'sum');
            var POP1000_T = $(this).jqGrid('getCol', 'POP1000', false, 'sum');
            var POP999_T = $(this).jqGrid('getCol', 'POP999', false, 'sum');
            var POP499_T = $(this).jqGrid('getCol', 'POP499', false, 'sum');
            var POP250_T = $(this).jqGrid('getCol', 'POP250', false, 'sum');
            var TOT_POP_T = $(this).jqGrid('getCol', 'TOT_POP', false, 'sum');
            var YPOP1000_T = $(this).jqGrid('getCol', 'YPOP1000', false, 'sum');
            var YPOP999_T = $(this).jqGrid('getCol', 'YPOP999', false, 'sum');
            var YPOP499_T = $(this).jqGrid('getCol', 'YPOP499', false, 'sum');
            var YPOP250_T = $(this).jqGrid('getCol', 'YPOP250', false, 'sum');
            var TOT_YPOP_T = $(this).jqGrid('getCol', 'TOT_YPOP', false, 'sum');
            var TU_LEN_T = $(this).jqGrid('getCol', 'TU_LEN', false, 'sum');
            TU_LEN_T = parseFloat(TU_LEN_T).toFixed(3);
            var TUM_LEN_T = $(this).jqGrid('getCol', 'TUM_LEN', false, 'sum');
            TUM_LEN_T = parseFloat(TUM_LEN_T).toFixed(3);
            var TRM_LEN_T = $(this).jqGrid('getCol', 'TRM_LEN', false, 'sum');
            TRM_LEN_T = parseFloat(TRM_LEN_T).toFixed(3);
            var TUY_LEN_T = $(this).jqGrid('getCol', 'TUY_LEN', false, 'sum');
            TUY_LEN_T = parseFloat(TUY_LEN_T).toFixed(3);
            var TRY_LEN_T = $(this).jqGrid('getCol', 'TRY_LEN', false, 'sum');
            TRY_LEN_T = parseFloat(TRY_LEN_T).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TN_LEN: TN_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TNM_LEN: TNM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TNY_LEN: TNY_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TPOP1000: TPOP1000_T }, true);
            $(this).jqGrid('footerData', 'set', { TPOP999: TPOP999_T }, true);
            $(this).jqGrid('footerData', 'set', { TPOP499: TPOP499_T }, true);
            $(this).jqGrid('footerData', 'set', { TPOP250: TPOP250_T }, true);
            $(this).jqGrid('footerData', 'set', { TOT_TPOP: TOT_TPOP_T }, true);
            $(this).jqGrid('footerData', 'set', { POP1000: POP1000_T }, true);
            $(this).jqGrid('footerData', 'set', { POP999: POP999_T }, true);
            $(this).jqGrid('footerData', 'set', { POP499: POP499_T }, true);
            $(this).jqGrid('footerData', 'set', { POP250: POP250_T }, true);
            $(this).jqGrid('footerData', 'set', { TOT_POP: TOT_POP_T }, true);
            $(this).jqGrid('footerData', 'set', { YPOP1000: YPOP1000_T }, true);
            $(this).jqGrid('footerData', 'set', { YPOP999: YPOP999_T }, true);
            $(this).jqGrid('footerData', 'set', { YPOP499: YPOP499_T }, true);
            $(this).jqGrid('footerData', 'set', { YPOP250: YPOP250_T }, true);
            $(this).jqGrid('footerData', 'set', { TOT_YPOP: TOT_YPOP_T }, true);
            $(this).jqGrid('footerData', 'set', { TU_LEN: TU_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TUM_LEN: TUM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TRM_LEN: TRM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TUY_LEN: TUY_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TRY_LEN: TRY_LEN_T }, true);
            $('#MPR2StateReportTable_rn').html('Sr.<br/>No.');
            $("#MPR2StateReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Cost Rs in Lacs & Length in Kms.</font>");

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

    $("#MPR2StateReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        //groupHeaders: [
        //  { startColumnName: 'TPOP1000', numberOfColumns: 5, titleText: '<em> New Connectivity </em>' },
        //  { startColumnName: 'POP1000', numberOfColumns: 5, titleText: '<em> Total Habitation </em>' },
        //  { startColumnName: 'YPOP1000', numberOfColumns: 5, titleText: '<em> Total Upgradation </em>' },
        //  { startColumnName: 'TUM_LEN', numberOfColumns: 2, titleText: '<em> Achievement During Month </em>' },
        //  { startColumnName: 'TUY_LEN', numberOfColumns: 2, titleText: '<em> Achievement During Year </em>' }
        //]
        groupHeaders: [
    {
        startColumnName: 'TN_LEN', numberOfColumns: 18,
        titleText: '<table style="width:100%;border-spacing:0px"' +
                '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="18">Habitations </td>  </tr>' +
                '<tr>' +
                    '<td id="h1" colspan="3" style="width: 13.2%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">New Connectivity Length (Kms)</td>' +
                      '<td id="h1" colspan="5" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Target for the Year</td>' +
                    '<td id="h1" colspan="5" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Achievement During the Month</td>' +
                '<td id="h2" colspan="5" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Total Achievement During the Year</td>' +
                '</tr>' +
                '</table>'
    },
    {
        startColumnName: 'TUM_LEN', numberOfColumns: 4,
        titleText: '<table style="width:100%;border-spacing:0px"' +
                '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Upgradation </td>  </tr>' +
                '<tr>' +
                    '<td id="h1" colspan="3" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Achievement During Month</td>' +
                        '<td id="h2" colspan="5" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Achievement During Year</td>' +
                '</tr>' +
                '</table>'
    },
        ]
    });


}
/**/

/*       DISTRICT REPORT LISTING       */
function MPR2DistrictReportListing(stateCode, stateName, month, year, collaboration) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#MPR2StateReportTable").jqGrid('setGridState', 'hidden');
    $("#MPR2StateReportTable").jqGrid('setSelection', stateCode);
    $("#MPR2DistrictReportTable").jqGrid('GridUnload');
    $("#MPR2BlockReportTable").jqGrid('GridUnload');
    $("#MPR2FinalReportTable").jqGrid('GridUnload');

    $("#MPR2DistrictReportTable").jqGrid({
        url: '/ProposalReports/MPR2DistrictReportListing?' + Math.random(),
        datatype: 'json',
        mtype: 'POST',
        colNames: ["District Name", 'Target', 'Achievement During Month', 'Achievement During Year', '1000+', '999-500', '499-250 (Eligible)', '<250 (Incidental)', 'Total', '1000+', '999-500', '499-250 (Eligible)',
            '<250 (Incidental)', 'Total', '1000+', '999-500', '499-250 (Eligible)', '<250 (Incidental)', 'Total', 'Target for the Year', 'Upgradation', 'Renewal', 'Upgradation', 'Renewal'],
        colModel: [
                    { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', width: 200, align: 'left', sortable: true },
                    { name: 'TN_LEN', index: 'TN_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'TNM_LEN', index: 'TNM_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'TNY_LEN', index: 'TNY_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'TPOP1000', index: 'TPOP1000', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'TPOP999', index: 'TPOP999', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'TPOP499', index: 'TPOP499', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'TPOP250', index: 'TPOP250', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'TOT_TPOP', index: 'TOT_TPOP', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'POP1000', index: 'POP1000', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'POP999', index: 'POP999', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'POP499', index: 'POP499', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'POP250', index: 'POP250', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'TOT_POP', index: 'TOT_POP', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'YPOP1000', index: 'YPOP1000', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'YPOP999', index: 'YPOP999', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'YPOP499', index: 'YPOP499', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'YPOP250', index: 'YPOP250', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'TOT_YPOP', index: 'TOT_YPOP', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'TU_LEN', index: 'TU_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'TUM_LEN', index: 'TUM_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'TRM_LEN', index: 'TRM_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'TUY_LEN', index: 'TUY_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                    { name: 'TRY_LEN', index: 'TRY_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } }
                ],
        postData: { "StateCode": stateCode, "Month": month, "Year": year, "Collaboration": collaboration },
        pager: $("#MPR2DistrictReportPager"),
        footerrow: true,
        sortname: 'MAST_DISTRICT_NAME',
        sortorder: 'asc',
        rowNum: 100,
        rownumbers: true,
        autowidth: false,
        height: '480',
        width: '1120',
        shrinkToFit: false,
        viewrecords: true,
        caption: 'District MPR2 for ' + stateName,
        loadComplete: function () {
            //Total of Columns
            var TN_LEN_T = $(this).jqGrid('getCol', 'TN_LEN', false, 'sum');
            TN_LEN_T = parseFloat(TN_LEN_T).toFixed(3);
            var TNM_LEN_T = $(this).jqGrid('getCol', 'TNM_LEN', false, 'sum');
            TNM_LEN_T = parseFloat(TNM_LEN_T).toFixed(3);
            var TNY_LEN_T = $(this).jqGrid('getCol', 'TNY_LEN', false, 'sum');
            TNY_LEN_T = parseFloat(TNY_LEN_T).toFixed(3);
            var TPOP1000_T = $(this).jqGrid('getCol', 'TPOP1000', false, 'sum');
            var TPOP999_T = $(this).jqGrid('getCol', 'TPOP999', false, 'sum');
            var TPOP499_T = $(this).jqGrid('getCol', 'TPOP499', false, 'sum');
            var TPOP250_T = $(this).jqGrid('getCol', 'TPOP250', false, 'sum');
            var TOT_TPOP_T = $(this).jqGrid('getCol', 'TOT_TPOP', false, 'sum');
            var POP1000_T = $(this).jqGrid('getCol', 'POP1000', false, 'sum');
            var POP999_T = $(this).jqGrid('getCol', 'POP999', false, 'sum');
            var POP499_T = $(this).jqGrid('getCol', 'POP499', false, 'sum');
            var POP250_T = $(this).jqGrid('getCol', 'POP250', false, 'sum');
            var TOT_POP_T = $(this).jqGrid('getCol', 'TOT_POP', false, 'sum');
            var YPOP1000_T = $(this).jqGrid('getCol', 'YPOP1000', false, 'sum');
            var YPOP999_T = $(this).jqGrid('getCol', 'YPOP999', false, 'sum');
            var YPOP499_T = $(this).jqGrid('getCol', 'YPOP499', false, 'sum');
            var YPOP250_T = $(this).jqGrid('getCol', 'YPOP250', false, 'sum');
            var TOT_YPOP_T = $(this).jqGrid('getCol', 'TOT_YPOP', false, 'sum');
            var TU_LEN_T = $(this).jqGrid('getCol', 'TU_LEN', false, 'sum');
            TU_LEN_T = parseFloat(TU_LEN_T).toFixed(3);
            var TUM_LEN_T = $(this).jqGrid('getCol', 'TUM_LEN', false, 'sum');
            TUM_LEN_T = parseFloat(TUM_LEN_T).toFixed(3);
            var TRM_LEN_T = $(this).jqGrid('getCol', 'TRM_LEN', false, 'sum');
            TRM_LEN_T = parseFloat(TRM_LEN_T).toFixed(3);
            var TUY_LEN_T = $(this).jqGrid('getCol', 'TUY_LEN', false, 'sum');
            TUY_LEN_T = parseFloat(TUY_LEN_T).toFixed(3);
            var TRY_LEN_T = $(this).jqGrid('getCol', 'TRY_LEN', false, 'sum');
            TRY_LEN_T = parseFloat(TRY_LEN_T).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { MAST_DISTRICT_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TN_LEN: TN_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TNM_LEN: TNM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TNY_LEN: TNY_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TPOP1000: TPOP1000_T }, true);
            $(this).jqGrid('footerData', 'set', { TPOP999: TPOP999_T }, true);
            $(this).jqGrid('footerData', 'set', { TPOP499: TPOP499_T }, true);
            $(this).jqGrid('footerData', 'set', { TPOP250: TPOP250_T }, true);
            $(this).jqGrid('footerData', 'set', { TOT_TPOP: TOT_TPOP_T }, true);
            $(this).jqGrid('footerData', 'set', { POP1000: POP1000_T }, true);
            $(this).jqGrid('footerData', 'set', { POP999: POP999_T }, true);
            $(this).jqGrid('footerData', 'set', { POP499: POP499_T }, true);
            $(this).jqGrid('footerData', 'set', { POP250: POP250_T }, true);
            $(this).jqGrid('footerData', 'set', { TOT_POP: TOT_POP_T }, true);
            $(this).jqGrid('footerData', 'set', { YPOP1000: YPOP1000_T }, true);
            $(this).jqGrid('footerData', 'set', { YPOP999: YPOP999_T }, true);
            $(this).jqGrid('footerData', 'set', { YPOP499: YPOP499_T }, true);
            $(this).jqGrid('footerData', 'set', { YPOP250: YPOP250_T }, true);
            $(this).jqGrid('footerData', 'set', { TOT_YPOP: TOT_YPOP_T }, true);
            $(this).jqGrid('footerData', 'set', { TU_LEN: TU_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TUM_LEN: TUM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TRM_LEN: TRM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TUY_LEN: TUY_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TRY_LEN: TRY_LEN_T }, true);
            $('#MPR2DistrictReportTable_rn').html('Sr.<br/>No.');
            $("#MPR2DistrictReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Cost Rs in Lacs & Length in Kms.</font>");

            $.unblockUI();
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/SessionExpire";
            }
            else {
                alert(xhr.message);
                window.location.href = "/Login/Error";
            }

            $.unblockUI();
        }
    });

    $("#MPR2DistrictReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        //groupHeaders: [
        //  { startColumnName: 'TPOP1000', numberOfColumns: 5, titleText: '<em> New Connectivity </em>' },
        //  { startColumnName: 'POP1000', numberOfColumns: 5, titleText: '<em> Total Habitation </em>' },
        //  { startColumnName: 'YPOP1000', numberOfColumns: 5, titleText: '<em> Total Upgradation </em>' },
        //  { startColumnName: 'TUM_LEN', numberOfColumns: 2, titleText: '<em> Achievement During Month </em>' },
        //  { startColumnName: 'TUY_LEN', numberOfColumns: 2, titleText: '<em> Achievement During Year </em>' }
        //]
        groupHeaders: [
     {
         startColumnName: 'TN_LEN', numberOfColumns: 18,
         titleText: '<table style="width:100%;border-spacing:0px"' +
                 '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="18">Habitations </td>  </tr>' +
                 '<tr>' +
                     '<td id="h1" colspan="3" style="width: 13.2%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">New Connectivity Length (Kms)</td>' +
                       '<td id="h1" colspan="5" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Target for the Year</td>' +
                     '<td id="h1" colspan="5" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Achievement During the Month</td>' +
                 '<td id="h2" colspan="5" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Total Achievement During the Year</td>' +
                 '</tr>' +
                 '</table>'
     },
     {
         startColumnName: 'TUM_LEN', numberOfColumns: 4,
         titleText: '<table style="width:100%;border-spacing:0px"' +
                 '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Upgradation </td>  </tr>' +
                 '<tr>' +
                     '<td id="h1" colspan="3" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Achievement During Month</td>' +
                         '<td id="h2" colspan="5" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Achievement During Year</td>' +
                 '</tr>' +
                 '</table>'
     },
     ]
    });

}
/**/



/*       BLOCK REPORT LISTING       */
function MPR2BlockReportListing(districtCode, stateCode, districtName, month, year, collaboration) {


    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#MPR2StateReportTable").jqGrid('setGridState', 'hidden');
    $("#MPR2DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#MPR2DistrictReportTable").jqGrid('setSelection', stateCode);
    $("#MPR2BlockReportTable").jqGrid('GridUnload');
    $("#MPR2FinalReportTable").jqGrid('GridUnload');

    $("#MPR2BlockReportTable").jqGrid({
        url: '/ProposalReports/MPR2BlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Target', 'Achievement During Month', 'Achievement During Year', '1000+', '999-500', '499-250 (Eligible)', '<250 (Incidental)', 'Total', '1000+', '999-500', '499-250 (Eligible)',
            '<250 (Incidental)', 'Total', '1000+', '999-500', '499-250 (Eligible)', '<250 (Incidental)', 'Total', 'Target for the Year', 'Upgradation', 'Renewal', 'Upgradation', 'Renewal'],
        colModel: [
            { name: "MAST_BLOCK_NAME", width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'TN_LEN', index: 'TN_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TNM_LEN', index: 'TNM_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TNY_LEN', index: 'TNY_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TPOP1000', index: 'TPOP1000', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TPOP999', index: 'TPOP999', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TPOP499', index: 'TPOP499', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TPOP250', index: 'TPOP250', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOT_TPOP', index: 'TOT_TPOP', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'POP1000', index: 'POP1000', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'POP999', index: 'POP999', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'POP499', index: 'POP499', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'POP250', index: 'POP250', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOT_POP', index: 'TOT_POP', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'YPOP1000', index: 'YPOP1000', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'YPOP999', index: 'YPOP999', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'YPOP499', index: 'YPOP499', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'YPOP250', index: 'YPOP250', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOT_YPOP', index: 'TOT_YPOP', width: 100, align: 'right', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TU_LEN', index: 'TU_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TUM_LEN', index: 'TUM_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TRM_LEN', index: 'TRM_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TUY_LEN', index: 'TUY_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TRY_LEN', index: 'TRY_LEN', width: 100, align: 'right', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } }
       ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "Month": month, "Year": year, "Collaboration": collaboration },
        pager: $("#MPR2BlockReportPager"),
        footerrow: true,
        sortname: 'MAST_BLOCK_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: false,
        height: '420',
        width: '1120',
        shrinkToFit: false,
        viewrecords: true,
        caption: 'Block MPR2 for ' + districtName,
        loadComplete: function () {
            //Total of Columns
            var TN_LEN_T = $(this).jqGrid('getCol', 'TN_LEN', false, 'sum');
            TN_LEN_T = parseFloat(TN_LEN_T).toFixed(3);
            var TNM_LEN_T = $(this).jqGrid('getCol', 'TNM_LEN', false, 'sum');
            TNM_LEN_T = parseFloat(TNM_LEN_T).toFixed(3);
            var TNY_LEN_T = $(this).jqGrid('getCol', 'TNY_LEN', false, 'sum');
            TNY_LEN_T = parseFloat(TNY_LEN_T).toFixed(3);
            var TPOP1000_T = $(this).jqGrid('getCol', 'TPOP1000', false, 'sum');
            var TPOP999_T = $(this).jqGrid('getCol', 'TPOP999', false, 'sum');
            var TPOP499_T = $(this).jqGrid('getCol', 'TPOP499', false, 'sum');
            var TPOP250_T = $(this).jqGrid('getCol', 'TPOP250', false, 'sum');
            var TOT_TPOP_T = $(this).jqGrid('getCol', 'TOT_TPOP', false, 'sum');
            var POP1000_T = $(this).jqGrid('getCol', 'POP1000', false, 'sum');
            var POP999_T = $(this).jqGrid('getCol', 'POP999', false, 'sum');
            var POP499_T = $(this).jqGrid('getCol', 'POP499', false, 'sum');
            var POP250_T = $(this).jqGrid('getCol', 'POP250', false, 'sum');
            var TOT_POP_T = $(this).jqGrid('getCol', 'TOT_POP', false, 'sum');
            var YPOP1000_T = $(this).jqGrid('getCol', 'YPOP1000', false, 'sum');
            var YPOP999_T = $(this).jqGrid('getCol', 'YPOP999', false, 'sum');
            var YPOP499_T = $(this).jqGrid('getCol', 'YPOP499', false, 'sum');
            var YPOP250_T = $(this).jqGrid('getCol', 'YPOP250', false, 'sum');
            var TOT_YPOP_T = $(this).jqGrid('getCol', 'TOT_YPOP', false, 'sum');
            var TU_LEN_T = $(this).jqGrid('getCol', 'TU_LEN', false, 'sum');
            TU_LEN_T = parseFloat(TU_LEN_T).toFixed(3);
            var TUM_LEN_T = $(this).jqGrid('getCol', 'TUM_LEN', false, 'sum');
            TUM_LEN_T = parseFloat(TUM_LEN_T).toFixed(3);
            var TRM_LEN_T = $(this).jqGrid('getCol', 'TRM_LEN', false, 'sum');
            TRM_LEN_T = parseFloat(TRM_LEN_T).toFixed(3);
            var TUY_LEN_T = $(this).jqGrid('getCol', 'TUY_LEN', false, 'sum');
            TUY_LEN_T = parseFloat(TUY_LEN_T).toFixed(3);
            var TRY_LEN_T = $(this).jqGrid('getCol', 'TRY_LEN', false, 'sum');
            TRY_LEN_T = parseFloat(TRY_LEN_T).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { MAST_BLOCK_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TN_LEN: TN_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TNM_LEN: TNM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TNY_LEN: TNY_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TPOP1000: TPOP1000_T }, true);
            $(this).jqGrid('footerData', 'set', { TPOP999: TPOP999_T }, true);
            $(this).jqGrid('footerData', 'set', { TPOP499: TPOP499_T }, true);
            $(this).jqGrid('footerData', 'set', { TPOP250: TPOP250_T }, true);
            $(this).jqGrid('footerData', 'set', { TOT_TPOP: TOT_TPOP_T }, true);
            $(this).jqGrid('footerData', 'set', { POP1000: POP1000_T }, true);
            $(this).jqGrid('footerData', 'set', { POP999: POP999_T }, true);
            $(this).jqGrid('footerData', 'set', { POP499: POP499_T }, true);
            $(this).jqGrid('footerData', 'set', { POP250: POP250_T }, true);
            $(this).jqGrid('footerData', 'set', { TOT_POP: TOT_POP_T }, true);
            $(this).jqGrid('footerData', 'set', { YPOP1000: YPOP1000_T }, true);
            $(this).jqGrid('footerData', 'set', { YPOP999: YPOP999_T }, true);
            $(this).jqGrid('footerData', 'set', { YPOP499: YPOP499_T }, true);
            $(this).jqGrid('footerData', 'set', { YPOP250: YPOP250_T }, true);
            $(this).jqGrid('footerData', 'set', { TOT_YPOP: TOT_YPOP_T }, true);
            $(this).jqGrid('footerData', 'set', { TU_LEN: TU_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TUM_LEN: TUM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TRM_LEN: TRM_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TUY_LEN: TUY_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TRY_LEN: TRY_LEN_T }, true);
            $('#MPR2BlockReportTable_rn').html('Sr.<br/>No.');
            $("#MPR2BlockReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Cost Rs in Lacs & Length in Kms.</font>");

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

    $("#MPR2BlockReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        //groupHeaders: [
        //  { startColumnName: 'TPOP1000', numberOfColumns: 5, titleText: '<em> New Connectivity </em>' },
        //  { startColumnName: 'POP1000', numberOfColumns: 5, titleText: '<em> Total Habitation </em>' },
        //  { startColumnName: 'YPOP1000', numberOfColumns: 5, titleText: '<em> Total Upgradation </em>' },
        //  { startColumnName: 'TUM_LEN', numberOfColumns: 2, titleText: '<em> Achievement During Month </em>' },
        //  { startColumnName: 'TUY_LEN', numberOfColumns: 2, titleText: '<em> Achievement During Year </em>' }
        //]
        groupHeaders: [
      {
          startColumnName: 'TN_LEN', numberOfColumns: 18,
          titleText: '<table style="width:100%;border-spacing:0px"' +
                  '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="18">Habitations </td>  </tr>' +
                  '<tr>' +
                      '<td id="h1" colspan="3" style="width: 13.2%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">New Connectivity Length (Kms)</td>' +
                        '<td id="h1" colspan="5" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Target for the Year</td>' +
                      '<td id="h1" colspan="5" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Achievement During the Month</td>' +
                  '<td id="h2" colspan="5" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Total Achievement During the Year</td>' +
                  '</tr>' +
                  '</table>'
      },
      {
          startColumnName: 'TUM_LEN', numberOfColumns: 4,
          titleText: '<table style="width:100%;border-spacing:0px"' +
                  '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Upgradation </td>  </tr>' +
                  '<tr>' +
                      '<td id="h1" colspan="3" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Achievement During Month</td>' +
                          '<td id="h2" colspan="5" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Achievement During Year</td>' +
                  '</tr>' +
                  '</table>'
      },
        ]
    });
}

/*       FINAL REPORT LISTING       */
function MPR2FinalReportListing(blockCode, blockName, districtCode, stateCode, month, year, collaboration) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#MPR2StateReportTable").jqGrid('setGridState', 'hidden');
    $("#MPR2DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#MPR2BlockReportTable").jqGrid('setGridState', 'hidden');
    $("#MPR2BlockReportTable").jqGrid('setSelection', stateCode);
    $("#MPR2FinalReportTable").jqGrid('GridUnload');
    $("#MPR2FinalReportTable").jqGrid({
        url: '/ProposalReports/MPR2FinalReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'IMS Year', 'IMS Batch', 'Package ID', 'Road Code', 'Proposal Type', 'Road Name', 'Bridge Name', 'Upgrade Connect', 'Pavilion Length', 'Bridge Length ', 'Collaboration', 'Road Cost ', 'Bridge Cost ', 'Maintainance Cost ', 'Total Length Completed ', 'Total Expenses'],
        colModel: [
            { name: "MAST_BLOCK_NAME", width: 200, align: 'left',  height: 'auto', sortable: true },
            { name: "IMS_YEAR", width: 100, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_BATCH", width: 100, align: 'center',  height: 'auto', sortable: false },
            { name: "IMS_PACKAGE_ID", width: 100, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_PROPOSAL_TYPE", width: 100, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_ROAD_NAME", width: 100, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_BRIDGE_NAME", width: 100, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_UPGRADE_CONNECT", width: 100, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_PAV_LENGTH", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "IMS_BRIDGE_LENGTH", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "IMS_COLLABORATION", width: 100, align: 'right',  height: 'auto', sortable: false },
            { name: "ROAD_AMT", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "BRIDGE_AMT", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT_AMT", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LENGTH_COMPLETED", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_EXP", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode, "Month": month, "Year": year, "Collaboration": collaboration },
        pager: $("#MPR2FinalReportPager"),
        footerrow: true,
        sortname: 'RoadName',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '380',
        viewrecords: true,
        caption: 'MPR2 for ' + blockName,
        loadComplete: function () {
            //Total of Columns
            var IMS_PAV_LENGTH_T = $(this).jqGrid('getCol', 'IMS_PAV_LENGTH', false, 'sum');
            IMS_PAV_LENGTH_T = parseFloat(IMS_PAV_LENGTH_T).toFixed(3);
            var IMS_BRIDGE_LENGTH_T = $(this).jqGrid('getCol', 'IMS_BRIDGE_LENGTH', false, 'sum');
            IMS_BRIDGE_LENGTH_T = parseFloat(IMS_BRIDGE_LENGTH_T).toFixed(3);
            var ROAD_AMT_T = $(this).jqGrid('getCol', 'ROAD_AMT', false, 'sum');
            ROAD_AMT_T = parseFloat(ROAD_AMT_T).toFixed(2);
            var BRIDGE_AMT_T = $(this).jqGrid('getCol', 'BRIDGE_AMT', false, 'sum');
            BRIDGE_AMT_T = parseFloat(BRIDGE_AMT_T).toFixed(2);
            var MAINT_AMT_T = $(this).jqGrid('getCol', 'MAINT_AMT', false, 'sum');
            MAINT_AMT_T = parseFloat(MAINT_AMT_T).toFixed(2);
            var TOTAL_LENGTH_COMPLETED_T = $(this).jqGrid('getCol', 'TOTAL_LENGTH_COMPLETED', false, 'sum');
            TOTAL_LENGTH_COMPLETED_T = parseFloat(TOTAL_LENGTH_COMPLETED_T).toFixed(3);
            var TOTAL_EXP_T = $(this).jqGrid('getCol', 'TOTAL_EXP', false, 'sum');
            TOTAL_EXP_T = parseFloat(TOTAL_EXP_T).toFixed(2);
            ////

            $(this).jqGrid('footerData', 'set', { MAST_BLOCK_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { IMS_PAV_LENGTH: IMS_PAV_LENGTH_T }, true);
            $(this).jqGrid('footerData', 'set', { IMS_BRIDGE_LENGTH: IMS_BRIDGE_LENGTH_T }, true);
            $(this).jqGrid('footerData', 'set', { ROAD_AMT: ROAD_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { BRIDGE_AMT: BRIDGE_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { MAINT_AMT: MAINT_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_LENGTH_COMPLETED: TOTAL_LENGTH_COMPLETED_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_EXP: TOTAL_EXP_T }, true);
            $('#MPR2FinalReportTable_rn').html('Sr.<br/>No.');
            $("#MPR2FinalReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Cost Rs in Lacs & Length in Kms.</font>");

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
/**/