$(function () {
    $('#btnGoPropMPR1').click(function () {
        var year = $('#ddYear_PropMPR1').val();
        var month = $('#ddMonth_PropMPR1').val();
        var collaboration = $('#ddAgency_PropMPR1').val();
        if ($("#hdnLevelId").val() == 6) //mord
        {
            MPR1StateReportListing(year, month, collaboration);
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
            MPR1DistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), year, month, collaboration);
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {

            MPR1BlockReportListing($("#MAST_DISTRICT_CODE").val(), $("#MAST_STATE_CODE").val(), $("#DISTRICT_NAME").val(), year, month, collaboration);
        }
    });
    $('#btnGoPropMPR1').trigger('click');
    //$('#tblRptContents').bind('resize', function () {
    //    resizeJqGrid();
    //}).trigger('resize');
});

/*       STATE REPORT LISTING       */
function MPR1StateReportListing(year, month, collaboration) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#MPR1StateReportTable").jqGrid('GridUnload');
    $("#MPR1DistrictReportTable").jqGrid('GridUnload');
    $("#MPR1BlockReportTable").jqGrid('GridUnload');
    $("#MPR1FinalReportTable").jqGrid('GridUnload');

    $("#MPR1StateReportTable").jqGrid({
        url: '/ProposalReports/MPR1StateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Sanctioned Year', 'Nos', 'Length', 'Cost', '1000+', '999-500', '499-250 (Eligible)', '<250 (Incidental)', 'Nos', 'Length', 'Cost',
            'Nos', 'Length', 'Cost', 'Funds Released', 'Nos', 'Length', 'Cost', '1000+', '999-500', '499-250 (Eligible)', '<250 (Incidental)', 'Nos',
            'Length', 'Cost', 'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost', '1000+', '999-500', '499-250 (Eligible)', '<250 (Incidental)',
            'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost'],
        colModel: [
            { name: "MAST_STATE_NAME", width: 200, align: 'left', height: 'auto', sortable: false },
            { name: "IMS_YEAR", width: 100, align: 'left', height: 'auto', sortable: false },
            { name: "TN_PROPOSALS", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TN_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TN_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "POP1000", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "POP999", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "POP499", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "POP250", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TU_PROPOSALS", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TU_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TU_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOT_PROP", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOT_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOT_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: "FUNDS_RELEASED", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: "CN_PROPOSALS", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CN_LEN", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CN_AMT", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CPOP1000", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CPOP999", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CPOP499", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CPOP250", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CU_PROPOSALS", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CU_LEN", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CU_AMT", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CTOT_PROP", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CTOT_LEN", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CTOT_AMT", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: "CCN_PROPOSALS", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCN_LEN", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCN_AMT", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCPOP1000", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCPOP999", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCPOP499", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCPOP250", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCU_PROPOSALS", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCU_LEN", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCU_AMT", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCTOT_PROP", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCTOT_LEN", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCTOT_AMT", width: 100, align: 'right',  height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        postData: { "Year": year, "Month": month, "Collaboration": collaboration },
        pager: $("#MPR1StateReportPager"),
        autowidth: false,
        shrinkToFit: false,
        width:1100,
        footerrow: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,       
        height: '520',   
        viewrecords: true,
        grouping: true,
        groupingView: {
            groupField: ['MAST_STATE_NAME'],
            groupSummary: [true],
            groupColumnShow: [false],
            groupDataSorted: true,
            showSummaryOnHide: true,
            //   groupText: ['<b>{0}- {1} </b>'],
            groupCollapse: true,
        },
        caption: 'State MPR1 Details',
        loadComplete: function () {
           

            //Total of Columns
            var TN_PROPOSALS_T = $(this).jqGrid('getCol', 'TN_PROPOSALS', false, 'sum');
            var TN_LEN_T = $(this).jqGrid('getCol', 'TN_LEN', false, 'sum');
            TN_LEN_T = parseFloat(TN_LEN_T).toFixed(3);
            var TN_AMT_T = $(this).jqGrid('getCol', 'TN_AMT', false, 'sum');
            TN_AMT_T = parseFloat(TN_AMT_T).toFixed(2);
            var POP1000_T = $(this).jqGrid('getCol', 'POP1000', false, 'sum');
            var POP999_T = $(this).jqGrid('getCol', 'POP999', false, 'sum');
            var POP499_T = $(this).jqGrid('getCol', 'POP499', false, 'sum');
            var POP250_T = $(this).jqGrid('getCol', 'POP250', false, 'sum');
            var TU_PROPOSALS_T = $(this).jqGrid('getCol', 'TU_PROPOSALS', false, 'sum');
            var TU_LEN_T = $(this).jqGrid('getCol', 'TU_LEN', false, 'sum');
            TU_LEN_T = parseFloat(TU_LEN_T).toFixed(3);
            var TU_AMT_T = $(this).jqGrid('getCol', 'TU_AMT', false, 'sum');
            TU_AMT_T = parseFloat(TU_AMT_T).toFixed(2);
            var TOT_PROP_T = $(this).jqGrid('getCol', 'TOT_PROP', false, 'sum');
            var TOT_LEN_T = $(this).jqGrid('getCol', 'TOT_LEN', false, 'sum');
            TOT_LEN_T = parseFloat(TOT_LEN_T).toFixed(3);
            var TOT_AMT_T = $(this).jqGrid('getCol', 'TOT_AMT', false, 'sum');
            TOT_AMT_T = parseFloat(TOT_AMT_T).toFixed(2);
            var FUNDS_RELEASED_T = $(this).jqGrid('getCol', 'FUNDS_RELEASED', false, 'sum');
            FUNDS_RELEASED_T = parseFloat(FUNDS_RELEASED_T).toFixed(2);
            var CN_PROPOSALS_T = $(this).jqGrid('getCol', 'CN_PROPOSALS', false, 'sum');
            var CN_LEN_T = $(this).jqGrid('getCol', 'CN_LEN', false, 'sum');
            CN_LEN_T = parseFloat(CN_LEN_T).toFixed(3);
            var CN_AMT_T = $(this).jqGrid('getCol', 'CN_AMT', false, 'sum');
            CN_AMT_T = parseFloat(CN_AMT_T).toFixed(2);
            var CPOP1000_T = $(this).jqGrid('getCol', 'CPOP1000', false, 'sum');
            var CPOP999_T = $(this).jqGrid('getCol', 'CPOP999', false, 'sum');
            var CPOP499_T = $(this).jqGrid('getCol', 'CPOP499', false, 'sum');
            var CPOP250_T = $(this).jqGrid('getCol', 'CPOP250', false, 'sum');
            var CU_PROPOSALS_T = $(this).jqGrid('getCol', 'CU_PROPOSALS', false, 'sum');
            var CU_LEN_T = $(this).jqGrid('getCol', 'CU_LEN', false, 'sum');
            CU_LEN_T = parseFloat(CU_LEN_T).toFixed(3);
            var CU_AMT_T = $(this).jqGrid('getCol', 'CU_AMT', false, 'sum');
            CU_AMT_T = parseFloat(CU_AMT_T).toFixed(2);
            var CTOT_PROP_T = $(this).jqGrid('getCol', 'CTOT_PROP', false, 'sum');
            var CTOT_LEN_T = $(this).jqGrid('getCol', 'CTOT_LEN', false, 'sum');
            CTOT_LEN_T = parseFloat(CTOT_LEN_T).toFixed(3);
            var CTOT_AMT_T = $(this).jqGrid('getCol', 'CTOT_AMT', false, 'sum');
            CTOT_AMT_T = parseFloat(CTOT_AMT_T).toFixed(2);
            var CCN_PROPOSALS_T = $(this).jqGrid('getCol', 'CCN_PROPOSALS', false, 'sum');
            var CCN_LEN_T = $(this).jqGrid('getCol', 'CCN_LEN', false, 'sum');
            CCN_LEN_T = parseFloat(CCN_LEN_T).toFixed(3);
            var CCN_AMT_T = $(this).jqGrid('getCol', 'CCN_AMT', false, 'sum');
            CCN_AMT_T = parseFloat(CCN_AMT_T).toFixed(2);
            var CCPOP1000_T = $(this).jqGrid('getCol', 'CCPOP1000', false, 'sum');
            var CCPOP999_T = $(this).jqGrid('getCol', 'CCPOP999', false, 'sum');
            var CCPOP499_T = $(this).jqGrid('getCol', 'CCPOP499', false, 'sum');
            var CCPOP250_T = $(this).jqGrid('getCol', 'CCPOP250', false, 'sum');
            var CCU_PROPOSALS_T = $(this).jqGrid('getCol', 'CCU_PROPOSALS', false, 'sum');
            var CCU_LEN_T = $(this).jqGrid('getCol', 'CCU_LEN', false, 'sum');
            CCU_LEN_T = parseFloat(CCU_LEN_T).toFixed(3);
            var CCU_AMT_T = $(this).jqGrid('getCol', 'CCU_AMT', false, 'sum');
            CCU_AMT_T = parseFloat(CCU_AMT_T).toFixed(2);
            var CCTOT_PROP_T = $(this).jqGrid('getCol', 'CCTOT_PROP', false, 'sum');
            var CCTOT_LEN_T = $(this).jqGrid('getCol', 'CCTOT_LEN', false, 'sum');
            CCTOT_LEN_T = parseFloat(CCTOT_LEN_T).toFixed(3);
            var CCTOT_AMT_T = $(this).jqGrid('getCol', 'CCTOT_AMT', false, 'sum');
            CCTOT_AMT_T = parseFloat(CCTOT_AMT_T).toFixed(2);


            //

            $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TN_PROPOSALS: TN_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { TN_LEN: TN_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TN_AMT: TN_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { POP1000: POP1000_T }, true);
            $(this).jqGrid('footerData', 'set', { POP999: POP999_T }, true);
            $(this).jqGrid('footerData', 'set', { POP499: POP499_T }, true);
            $(this).jqGrid('footerData', 'set', { POP250: POP250_T }, true);
            $(this).jqGrid('footerData', 'set', { TU_PROPOSALS: TU_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { TU_LEN: TU_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TU_AMT: TU_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { TOT_PROP: TOT_PROP_T }, true);
            $(this).jqGrid('footerData', 'set', { TOT_LEN: TOT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOT_AMT: TOT_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { FUNDS_RELEASED: FUNDS_RELEASED_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_PROPOSALS: CN_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_LEN: CN_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_AMT: CN_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { CPOP1000: CPOP1000_T }, true);
            $(this).jqGrid('footerData', 'set', { CPOP999: CPOP999_T }, true);
            $(this).jqGrid('footerData', 'set', { CPOP499: CPOP499_T }, true);
            $(this).jqGrid('footerData', 'set', { CPOP250: CPOP250_T }, true);
            $(this).jqGrid('footerData', 'set', { CU_PROPOSALS: CU_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { CU_LEN: CU_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CU_AMT: CU_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { CTOT_PROP: CTOT_PROP_T }, true);
            $(this).jqGrid('footerData', 'set', { CTOT_LEN: CTOT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CTOT_AMT: CTOT_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { CCN_PROPOSALS: CCN_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { CCN_LEN: CCN_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CCN_AMT: CCN_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { CCPOP1000: CCPOP1000_T }, true);
            $(this).jqGrid('footerData', 'set', { CCPOP999: CCPOP999_T }, true);
            $(this).jqGrid('footerData', 'set', { CCPOP499: CCPOP499_T }, true);
            $(this).jqGrid('footerData', 'set', { CCPOP250: CCPOP250_T }, true);
            $(this).jqGrid('footerData', 'set', { CCU_PROPOSALS: CCU_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { CCU_LEN: CCU_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CCU_AMT: CCU_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { CCTOT_PROP: CCTOT_PROP_T }, true);
            $(this).jqGrid('footerData', 'set', { CCTOT_LEN: CCTOT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CCTOT_AMT: CCTOT_AMT_T }, true);
       
            $("#MPR1StateReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Cost Rs in Lacs & Length in Kms.</font>");
            $('#MPR1StateReportTable_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        onSelectRow: function (id) {
            //alert(id);
            var params = id.split('$');
            MPR1DistrictReportListing(params[0], params[1], params[2],params[3],params[4]);
           
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

    $("#MPR1StateReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          //{ startColumnName: 'TN_PROPOSALS', numberOfColumns: 7, titleText: '<em> New Connectivity</em>' },
          //{ startColumnName: 'TU_PROPOSALS', numberOfColumns: 3, titleText: '<em> Upgradation</em>' },
          //{ startColumnName: 'TOT_PROP', numberOfColumns: 3, titleText: '<em> Total</em>' },

          //{ startColumnName: 'CN_PROPOSALS', numberOfColumns: 3, titleText: '<em> Completed New Connectivity</em>' },
          //{ startColumnName: 'CPOP1000', numberOfColumns: 4, titleText: '<em> Completed Upgradation</em>' },
          //{ startColumnName: 'CU_PROPOSALS', numberOfColumns: 3, titleText: '<em> Total</em>' },

          //{ startColumnName: 'CTOT_PROP', numberOfColumns: 3, titleText: '<em> Completed New Connectivity</em>' },
          //{ startColumnName: 'CCN_PROPOSALS', numberOfColumns: 3, titleText: '<em> Total Upgradation</em>' },
          //{ startColumnName: 'CCPOP1000', numberOfColumns: 4, titleText: '<em> Cumulative Completed</em>' },

          //{ startColumnName: 'CCU_PROPOSALS', numberOfColumns: 3, titleText: '<em> Cumulative Upgradation</em>' },
          //{ startColumnName: 'CCTOT_PROP', numberOfColumns: 3, titleText: '<em> Cumulative Total</em>' }

           {
               startColumnName: 'TN_PROPOSALS', numberOfColumns: 13,
               titleText: '<table style="width:100%;border-spacing:0px"' +
                       '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="13">Clearance </td>  </tr>' +
                       '<tr>' +
                           '<td id="h1" colspan="7" style="width: 51.5%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">New Connectivity</td>' +
                             '<td id="h1" colspan="3" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Upgradation</td>' +
                             '<td id="h2" colspan="3" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Total</td>' +
                       '</tr>' +
                       '</table>'
           },

            {
                startColumnName: 'CN_PROPOSALS', numberOfColumns: 13,
                titleText: '<table style="width:100%;border-spacing:0px"' +
                        '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="13">Progress during the month </td>  </tr>' +
                        '<tr>' +
                            '<td id="h1" colspan="7" style="width: 51.5%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">New Connectivity</td>' +
                              '<td id="h1" colspan="3" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Upgradation</td>' +
                              '<td id="h2" colspan="3" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Total</td>' +
                        '</tr>' +
                        '</table>'
            },

              {
                  startColumnName: 'CCN_PROPOSALS', numberOfColumns: 13,
                  titleText: '<table style="width:100%;border-spacing:0px"' +
                          '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="13">Cummulative Progress </td>  </tr>' +
                          '<tr>' +
                              '<td id="h1" colspan="7" style="width: 51.5%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">New Connectivity</td>' +
                                '<td id="h1" colspan="3" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Upgradation</td>' +
                                '<td id="h2" colspan="3" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Total</td>' +
                          '</tr>' +
                          '</table>'
              },
        ]
    });
}
/**/

/*       DISTRICT REPORT LISTING       */
function MPR1DistrictReportListing(stateCode, stateName, year, month, collaboration) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#MPR1StateReportTable").jqGrid('setGridState', 'hidden');
    $("#MPR1StateReportTable").jqGrid('setSelection', stateCode);
    $("#MPR1DistrictReportTable").jqGrid('GridUnload');
    $("#MPR1BlockReportTable").jqGrid('GridUnload');
    $("#MPR1FinalReportTable").jqGrid('GridUnload');

    $("#MPR1DistrictReportTable").jqGrid({
        url: '/ProposalReports/MPR1DistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Sanctioned Year', 'Nos', 'Length', 'Cost', '1000+', '999-500', '499-250', '<250', 'Nos', 'Length', 'Cost',
            'Nos', 'Length', 'Cost', 'Funds Released', 'Nos', 'Length', 'Cost', '1000+', '999-500', '499-250', '<250', 'Nos',
            'Length', 'Cost', 'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost', '1000+', '999-500', '499-250', '<250',
            'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost'],
        colModel: [
            { name: "MAST_DISTRICT_NAME", width: 200, align: 'left', height: 'auto', sortable: false },
            { name: "IMS_YEAR", width: 100, align: 'left', height: 'auto', sortable: false },
            { name: "TN_PROPOSALS", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TN_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TN_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "POP1000", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "POP999", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "POP499", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "POP250", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TU_PROPOSALS", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TU_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TU_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOT_PROP", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOT_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOT_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: "FUNDS_RELEASED", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: "CN_PROPOSALS", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CN_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CN_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CPOP1000", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CPOP999", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CPOP499", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CPOP250", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CU_PROPOSALS", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CU_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CU_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CTOT_PROP", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CTOT_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CTOT_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: "CCN_PROPOSALS", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCN_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCN_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCPOP1000", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCPOP999", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCPOP499", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCPOP250", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCU_PROPOSALS", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCU_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCU_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCTOT_PROP", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCTOT_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCTOT_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        postData: { "StateCode": stateCode,"Year":year,"Month":month,"Collaboration":collaboration },
        pager: $("#MPR1DistrictReportPager"),
        footerrow: true,
        sortname: 'MAST_DISTRICT_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: false,
        shrinkToFit: false,
        width: 1100,
        height: '480',
        viewrecords: true,
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
        caption: 'District MPR1 for ' + stateName,
        loadComplete: function () {
           
            //Total of Columns
            var TN_PROPOSALS_T = $(this).jqGrid('getCol', 'TN_PROPOSALS', false, 'sum');
            var TN_LEN_T = $(this).jqGrid('getCol', 'TN_LEN', false, 'sum');
            TN_LEN_T = parseFloat(TN_LEN_T).toFixed(3);
            var TN_AMT_T = $(this).jqGrid('getCol', 'TN_AMT', false, 'sum');
            TN_AMT_T = parseFloat(TN_AMT_T).toFixed(2);
            var POP1000_T = $(this).jqGrid('getCol', 'POP1000', false, 'sum');
            var POP999_T = $(this).jqGrid('getCol', 'POP999', false, 'sum');
            var POP499_T = $(this).jqGrid('getCol', 'POP499', false, 'sum');
            var POP250_T = $(this).jqGrid('getCol', 'POP250', false, 'sum');
            var TU_PROPOSALS_T = $(this).jqGrid('getCol', 'TU_PROPOSALS', false, 'sum');
            var TU_LEN_T = $(this).jqGrid('getCol', 'TU_LEN', false, 'sum');
            TU_LEN_T = parseFloat(TU_LEN_T).toFixed(3);
            var TU_AMT_T = $(this).jqGrid('getCol', 'TU_AMT', false, 'sum');
            TU_AMT_T = parseFloat(TU_AMT_T).toFixed(2);
            var TOT_PROP_T = $(this).jqGrid('getCol', 'TOT_PROP', false, 'sum');
            var TOT_LEN_T = $(this).jqGrid('getCol', 'TOT_LEN', false, 'sum');
            TOT_LEN_T = parseFloat(TOT_LEN_T).toFixed(3);
            var TOT_AMT_T = $(this).jqGrid('getCol', 'TOT_AMT', false, 'sum');
            TOT_AMT_T = parseFloat(TOT_AMT_T).toFixed(2);
            var FUNDS_RELEASED_T = $(this).jqGrid('getCol', 'FUNDS_RELEASED', false, 'sum');
            FUNDS_RELEASED_T = parseFloat(FUNDS_RELEASED_T).toFixed(2);
            var CN_PROPOSALS_T = $(this).jqGrid('getCol', 'CN_PROPOSALS', false, 'sum');
            var CN_LEN_T = $(this).jqGrid('getCol', 'CN_LEN', false, 'sum');
            CN_LEN_T = parseFloat(CN_LEN_T).toFixed(3);
            var CN_AMT_T = $(this).jqGrid('getCol', 'CN_AMT', false, 'sum');
            CN_AMT_T = parseFloat(CN_AMT_T).toFixed(2);
            var CPOP1000_T = $(this).jqGrid('getCol', 'CPOP1000', false, 'sum');
            var CPOP999_T = $(this).jqGrid('getCol', 'CPOP999', false, 'sum');
            var CPOP499_T = $(this).jqGrid('getCol', 'CPOP499', false, 'sum');
            var CPOP250_T = $(this).jqGrid('getCol', 'CPOP250', false, 'sum');
            var CU_PROPOSALS_T = $(this).jqGrid('getCol', 'CU_PROPOSALS', false, 'sum');
            var CU_LEN_T = $(this).jqGrid('getCol', 'CU_LEN', false, 'sum');
            CU_LEN_T = parseFloat(CU_LEN_T).toFixed(3);
            var CU_AMT_T = $(this).jqGrid('getCol', 'CU_AMT', false, 'sum');
            CU_AMT_T = parseFloat(CU_AMT_T).toFixed(2);
            var CTOT_PROP_T = $(this).jqGrid('getCol', 'CTOT_PROP', false, 'sum');
            var CTOT_LEN_T = $(this).jqGrid('getCol', 'CTOT_LEN', false, 'sum');
            CTOT_LEN_T = parseFloat(CTOT_LEN_T).toFixed(3);
            var CTOT_AMT_T = $(this).jqGrid('getCol', 'CTOT_AMT', false, 'sum');
            CTOT_AMT_T = parseFloat(CTOT_AMT_T).toFixed(2);
            var CCN_PROPOSALS_T = $(this).jqGrid('getCol', 'CCN_PROPOSALS', false, 'sum');
            var CCN_LEN_T = $(this).jqGrid('getCol', 'CCN_LEN', false, 'sum');
            CCN_LEN_T = parseFloat(CCN_LEN_T).toFixed(3);
            var CCN_AMT_T = $(this).jqGrid('getCol', 'CCN_AMT', false, 'sum');
            CCN_AMT_T = parseFloat(CCN_AMT_T).toFixed(2);
            var CCPOP1000_T = $(this).jqGrid('getCol', 'CCPOP1000', false, 'sum');
            var CCPOP999_T = $(this).jqGrid('getCol', 'CCPOP999', false, 'sum');
            var CCPOP499_T = $(this).jqGrid('getCol', 'CCPOP499', false, 'sum');
            var CCPOP250_T = $(this).jqGrid('getCol', 'CCPOP250', false, 'sum');
            var CCU_PROPOSALS_T = $(this).jqGrid('getCol', 'CCU_PROPOSALS', false, 'sum');
            var CCU_LEN_T = $(this).jqGrid('getCol', 'CCU_LEN', false, 'sum');
            CCU_LEN_T = parseFloat(CCU_LEN_T).toFixed(3);
            var CCU_AMT_T = $(this).jqGrid('getCol', 'CCU_AMT', false, 'sum');
            CCU_AMT_T = parseFloat(CCU_AMT_T).toFixed(2);
            var CCTOT_PROP_T = $(this).jqGrid('getCol', 'CCTOT_PROP', false, 'sum');
            var CCTOT_LEN_T = $(this).jqGrid('getCol', 'CCTOT_LEN', false, 'sum');
            CCTOT_LEN_T = parseFloat(CCTOT_LEN_T).toFixed(3);
            var CCTOT_AMT_T = $(this).jqGrid('getCol', 'CCTOT_AMT', false, 'sum');
            CCTOT_AMT_T = parseFloat(CCTOT_AMT_T).toFixed(2);

            //

            $(this).jqGrid('footerData', 'set', { MAST_DISTRICT_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TN_PROPOSALS: TN_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { TN_LEN: TN_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TN_AMT: TN_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { POP1000: POP1000_T }, true);
            $(this).jqGrid('footerData', 'set', { POP999: POP999_T }, true);
            $(this).jqGrid('footerData', 'set', { POP499: POP499_T }, true);
            $(this).jqGrid('footerData', 'set', { POP250: POP250_T }, true);
            $(this).jqGrid('footerData', 'set', { TU_PROPOSALS: TU_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { TU_LEN: TU_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TU_AMT: TU_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { TOT_PROP: TOT_PROP_T }, true);
            $(this).jqGrid('footerData', 'set', { TOT_LEN: TOT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOT_AMT: TOT_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { FUNDS_RELEASED: FUNDS_RELEASED_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_PROPOSALS: CN_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_LEN: CN_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_AMT: CN_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { CPOP1000: CPOP1000_T }, true);
            $(this).jqGrid('footerData', 'set', { CPOP999: CPOP999_T }, true);
            $(this).jqGrid('footerData', 'set', { CPOP499: CPOP499_T }, true);
            $(this).jqGrid('footerData', 'set', { CPOP250: CPOP250_T }, true);
            $(this).jqGrid('footerData', 'set', { CU_PROPOSALS: CU_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { CU_LEN: CU_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CU_AMT: CU_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { CTOT_PROP: CTOT_PROP_T }, true);
            $(this).jqGrid('footerData', 'set', { CTOT_LEN: CTOT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CTOT_AMT: CTOT_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { CCN_PROPOSALS: CCN_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { CCN_LEN: CCN_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CCN_AMT: CCN_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { CCPOP1000: CCPOP1000_T }, true);
            $(this).jqGrid('footerData', 'set', { CCPOP999: CCPOP999_T }, true);
            $(this).jqGrid('footerData', 'set', { CCPOP499: CCPOP499_T }, true);
            $(this).jqGrid('footerData', 'set', { CCPOP250: CCPOP250_T }, true);
            $(this).jqGrid('footerData', 'set', { CCU_PROPOSALS: CCU_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { CCU_LEN: CCU_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CCU_AMT: CCU_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { CCTOT_PROP: CCTOT_PROP_T }, true);
            $(this).jqGrid('footerData', 'set', { CCTOT_LEN: CCTOT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CCTOT_AMT: CCTOT_AMT_T }, true);
            
            $("#MPR1DistrictReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Cost Rs in Lacs & Length in Kms.</font>");
            $('#MPR1DistrictReportTable_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        onSelectRow: function (id) {
            //alert(id);
            var params = id.split('$');
            MPR1BlockReportListing(params[0], params[1], params[2], params[3], params[4],params[5]);

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

    $("#MPR1DistrictReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          //{ startColumnName: 'TN_PROPOSALS', numberOfColumns: 7, titleText: '<em> Total New Connectivity</em>' },
          //{ startColumnName: 'TU_PROPOSALS', numberOfColumns: 3, titleText: '<em> Total Upgradation</em>' },
          //{ startColumnName: 'TOT_PROP', numberOfColumns: 3, titleText: '<em> Total</em>' },
          //{ startColumnName: 'CN_PROPOSALS', numberOfColumns: 3, titleText: '<em> Completed New Connectivity</em>' },
          //{ startColumnName: 'CPOP1000', numberOfColumns: 4, titleText: '<em> Completed Upgradation</em>' },
          //{ startColumnName: 'CU_PROPOSALS', numberOfColumns: 3, titleText: '<em> Total</em>' },
          //{ startColumnName: 'CTOT_PROP', numberOfColumns: 3, titleText: '<em> Completed New Connectivity</em>' },
          //{ startColumnName: 'CCN_PROPOSALS', numberOfColumns: 3, titleText: '<em> Total Upgradation</em>' },
          //{ startColumnName: 'CCPOP1000', numberOfColumns: 4, titleText: '<em> Cumulative Completed</em>' },
          //{ startColumnName: 'CCU_PROPOSALS', numberOfColumns: 3, titleText: '<em> Cumulative Upgradation</em>' },
          //{ startColumnName: 'CCTOT_PROP', numberOfColumns: 3, titleText: '<em> Cumulative Total</em>' }

            {
                startColumnName: 'TN_PROPOSALS', numberOfColumns: 13,
                titleText: '<table style="width:100%;border-spacing:0px"' +
                        '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="13">Clearance </td>  </tr>' +
                        '<tr>' +
                            '<td id="h1" colspan="7" style="width: 51.5%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">New Connectivity</td>' +
                              '<td id="h1" colspan="3" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Upgradation</td>' +
                              '<td id="h2" colspan="3" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Total</td>' +
                        '</tr>' +
                        '</table>'
            },

            {
                startColumnName: 'CN_PROPOSALS', numberOfColumns: 13,
                titleText: '<table style="width:100%;border-spacing:0px"' +
                        '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="13">Progress during the month </td>  </tr>' +
                        '<tr>' +
                            '<td id="h1" colspan="7" style="width: 51.5%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">New Connectivity</td>' +
                              '<td id="h1" colspan="3" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Upgradation</td>' +
                              '<td id="h2" colspan="3" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Total</td>' +
                        '</tr>' +
                        '</table>'
            },

              {
                  startColumnName: 'CCN_PROPOSALS', numberOfColumns: 13,
                  titleText: '<table style="width:100%;border-spacing:0px"' +
                          '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="13">Cummulative Progress </td>  </tr>' +
                          '<tr>' +
                              '<td id="h1" colspan="7" style="width: 51.5%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">New Connectivity</td>' +
                                '<td id="h1" colspan="3" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Upgradation</td>' +
                                '<td id="h2" colspan="3" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Total</td>' +
                          '</tr>' +
                          '</table>'
              },
        ]
    });

}
/**/

/*       BLOCK REPORT LISTING       */
function MPR1BlockReportListing(districtCode, stateCode, districtName, year, month, collaboration) {
    var distName;
    if (districtName == '')
        distName = $("#DISTRICT_NAME").val();
    else
        distName = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#MPR1DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#MPR1StateReportTable").jqGrid('setGridState', 'hidden');
    $("#MPR1DistrictReportTable").jqGrid('setSelection', stateCode);
    $("#MPR1BlockReportTable").jqGrid('GridUnload');
    $("#MPR1FinalReportTable").jqGrid('GridUnload');

    $("#MPR1BlockReportTable").jqGrid({
        url: '/ProposalReports/MPR1BlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Sanctioned Year', 'Nos', 'Length', 'Cost', '1000+', '999-500', '499-250', '<250', 'Nos', 'Length', 'Cost',
            'Nos', 'Length', 'Cost', 'Funds Released', 'Nos', 'Length', 'Cost', '1000+', '999-500', '499-250', '<250', 'Nos',
            'Length', 'Cost', 'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost', '1000+', '999-500', '499-250', '<250',
            'Nos', 'Length', 'Cost', 'Nos', 'Length', 'Cost'],
        colModel: [
            { name: "MAST_BLOCK_NAME", width: 200, align: 'left', height: 'auto', sortable: false },
             { name: "IMS_YEAR", width: 100, align: 'left', height: 'auto', sortable: false },
            { name: "TN_PROPOSALS", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TN_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TN_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "POP1000", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "POP999", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "POP499", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "POP250", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TU_PROPOSALS", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TU_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TU_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOT_PROP", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOT_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOT_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: "FUNDS_RELEASED", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: "CN_PROPOSALS", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CN_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CN_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CPOP1000", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CPOP999", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CPOP499", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CPOP250", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CU_PROPOSALS", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CU_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CU_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CTOT_PROP", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CTOT_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CTOT_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: "CCN_PROPOSALS", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCN_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCN_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCPOP1000", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCPOP999", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCPOP499", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCPOP250", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCU_PROPOSALS", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCU_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCU_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCTOT_PROP", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCTOT_LEN", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CCTOT_AMT", width: 100, align: 'right', height: 'auto', sortable: false, summaryType: 'sum', formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "Year": year, "Month": month, "Collaboration": collaboration },
        pager: $("#MPR1BlockReportPager"),
        footerrow: true,
        sortname: 'MAST_BLOCK_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        shrinkToFit: false,
        width: 1100,
        autowidth: false,
        height: '430',
        viewrecords: true,
        grouping: true,
        groupingView: {
            groupField: ['MAST_BLOCK_NAME'],
            groupSummary: [true],
            groupColumnShow: [false],
            groupDataSorted: true,
            showSummaryOnHide: true,
            //   groupText: ['<b>{0}- {1} </b>'],
            groupCollapse: true,
        },
        caption: 'Block MPR1 for ' + distName,
        loadComplete: function () {
           
            //Total of Columns
            var TN_PROPOSALS_T = $(this).jqGrid('getCol', 'TN_PROPOSALS', false, 'sum');
            var TN_LEN_T = $(this).jqGrid('getCol', 'TN_LEN', false, 'sum');
            TN_LEN_T = parseFloat(TN_LEN_T).toFixed(3);
            var TN_AMT_T = $(this).jqGrid('getCol', 'TN_AMT', false, 'sum');
            TN_AMT_T = parseFloat(TN_AMT_T).toFixed(2);
            var POP1000_T = $(this).jqGrid('getCol', 'POP1000', false, 'sum');
            var POP999_T = $(this).jqGrid('getCol', 'POP999', false, 'sum');
            var POP499_T = $(this).jqGrid('getCol', 'POP499', false, 'sum');
            var POP250_T = $(this).jqGrid('getCol', 'POP250', false, 'sum');
            var TU_PROPOSALS_T = $(this).jqGrid('getCol', 'TU_PROPOSALS', false, 'sum');
            var TU_LEN_T = $(this).jqGrid('getCol', 'TU_LEN', false, 'sum');
            TU_LEN_T = parseFloat(TU_LEN_T).toFixed(3);
            var TU_AMT_T = $(this).jqGrid('getCol', 'TU_AMT', false, 'sum');
            TU_AMT_T = parseFloat(TU_AMT_T).toFixed(2);
            var TOT_PROP_T = $(this).jqGrid('getCol', 'TOT_PROP', false, 'sum');
            var TOT_LEN_T = $(this).jqGrid('getCol', 'TOT_LEN', false, 'sum');
            TOT_LEN_T = parseFloat(TOT_LEN_T).toFixed(3);
            var TOT_AMT_T = $(this).jqGrid('getCol', 'TOT_AMT', false, 'sum');
            TOT_AMT_T = parseFloat(TOT_AMT_T).toFixed(2);
            var FUNDS_RELEASED_T = $(this).jqGrid('getCol', 'FUNDS_RELEASED', false, 'sum');
            FUNDS_RELEASED_T = parseFloat(FUNDS_RELEASED_T).toFixed(2);
            var CN_PROPOSALS_T = $(this).jqGrid('getCol', 'CN_PROPOSALS', false, 'sum');
            var CN_LEN_T = $(this).jqGrid('getCol', 'CN_LEN', false, 'sum');
            CN_LEN_T = parseFloat(CN_LEN_T).toFixed(3);
            var CN_AMT_T = $(this).jqGrid('getCol', 'CN_AMT', false, 'sum');
            CN_AMT_T = parseFloat(CN_AMT_T).toFixed(2);
            var CPOP1000_T = $(this).jqGrid('getCol', 'CPOP1000', false, 'sum');
            var CPOP999_T = $(this).jqGrid('getCol', 'CPOP999', false, 'sum');
            var CPOP499_T = $(this).jqGrid('getCol', 'CPOP499', false, 'sum');
            var CPOP250_T = $(this).jqGrid('getCol', 'CPOP250', false, 'sum');
            var CU_PROPOSALS_T = $(this).jqGrid('getCol', 'CU_PROPOSALS', false, 'sum');
            var CU_LEN_T = $(this).jqGrid('getCol', 'CU_LEN', false, 'sum');
            CU_LEN_T = parseFloat(CU_LEN_T).toFixed(3);
            var CU_AMT_T = $(this).jqGrid('getCol', 'CU_AMT', false, 'sum');
            CU_AMT_T = parseFloat(CU_AMT_T).toFixed(2);
            var CTOT_PROP_T = $(this).jqGrid('getCol', 'CTOT_PROP', false, 'sum');
            var CTOT_LEN_T = $(this).jqGrid('getCol', 'CTOT_LEN', false, 'sum');
            CTOT_LEN_T = parseFloat(CTOT_LEN_T).toFixed(3);
            var CTOT_AMT_T = $(this).jqGrid('getCol', 'CTOT_AMT', false, 'sum');
            CTOT_AMT_T = parseFloat(CTOT_AMT_T).toFixed(2);
            var CCN_PROPOSALS_T = $(this).jqGrid('getCol', 'CCN_PROPOSALS', false, 'sum');
            var CCN_LEN_T = $(this).jqGrid('getCol', 'CCN_LEN', false, 'sum');
            CCN_LEN_T = parseFloat(CCN_LEN_T).toFixed(3);
            var CCN_AMT_T = $(this).jqGrid('getCol', 'CCN_AMT', false, 'sum');
            CCN_AMT_T = parseFloat(CCN_AMT_T).toFixed(2);
            var CCPOP1000_T = $(this).jqGrid('getCol', 'CCPOP1000', false, 'sum');
            var CCPOP999_T = $(this).jqGrid('getCol', 'CCPOP999', false, 'sum');
            var CCPOP499_T = $(this).jqGrid('getCol', 'CCPOP499', false, 'sum');
            var CCPOP250_T = $(this).jqGrid('getCol', 'CCPOP250', false, 'sum');
            var CCU_PROPOSALS_T = $(this).jqGrid('getCol', 'CCU_PROPOSALS', false, 'sum');
            var CCU_LEN_T = $(this).jqGrid('getCol', 'CCU_LEN', false, 'sum');
            CCU_LEN_T = parseFloat(CCU_LEN_T).toFixed(3);
            var CCU_AMT_T = $(this).jqGrid('getCol', 'CCU_AMT', false, 'sum');
            CCU_AMT_T = parseFloat(CCU_AMT_T).toFixed(2);
            var CCTOT_PROP_T = $(this).jqGrid('getCol', 'CCTOT_PROP', false, 'sum');
            var CCTOT_LEN_T = $(this).jqGrid('getCol', 'CCTOT_LEN', false, 'sum');
            CCTOT_LEN_T = parseFloat(CCTOT_LEN_T).toFixed(3);
            var CCTOT_AMT_T = $(this).jqGrid('getCol', 'CCTOT_AMT', false, 'sum');
            CCTOT_AMT_T = parseFloat(CCTOT_AMT_T).toFixed(2);

            //

            $(this).jqGrid('footerData', 'set', { MAST_BLOCK_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TN_PROPOSALS: TN_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { TN_LEN: TN_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TN_AMT: TN_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { POP1000: POP1000_T }, true);
            $(this).jqGrid('footerData', 'set', { POP999: POP999_T }, true);
            $(this).jqGrid('footerData', 'set', { POP499: POP499_T }, true);
            $(this).jqGrid('footerData', 'set', { POP250: POP250_T }, true);
            $(this).jqGrid('footerData', 'set', { TU_PROPOSALS: TU_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { TU_LEN: TU_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TU_AMT: TU_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { TOT_PROP: TOT_PROP_T }, true);
            $(this).jqGrid('footerData', 'set', { TOT_LEN: TOT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { TOT_AMT: TOT_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { FUNDS_RELEASED: FUNDS_RELEASED_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_PROPOSALS: CN_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_LEN: CN_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CN_AMT: CN_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { CPOP1000: CPOP1000_T }, true);
            $(this).jqGrid('footerData', 'set', { CPOP999: CPOP999_T }, true);
            $(this).jqGrid('footerData', 'set', { CPOP499: CPOP499_T }, true);
            $(this).jqGrid('footerData', 'set', { CPOP250: CPOP250_T }, true);
            $(this).jqGrid('footerData', 'set', { CU_PROPOSALS: CU_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { CU_LEN: CU_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CU_AMT: CU_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { CTOT_PROP: CTOT_PROP_T }, true);
            $(this).jqGrid('footerData', 'set', { CTOT_LEN: CTOT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CTOT_AMT: CTOT_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { CCN_PROPOSALS: CCN_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { CCN_LEN: CCN_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CCN_AMT: CCN_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { CCPOP1000: CCPOP1000_T }, true);
            $(this).jqGrid('footerData', 'set', { CCPOP999: CCPOP999_T }, true);
            $(this).jqGrid('footerData', 'set', { CCPOP499: CCPOP499_T }, true);
            $(this).jqGrid('footerData', 'set', { CCPOP250: CCPOP250_T }, true);
            $(this).jqGrid('footerData', 'set', { CCU_PROPOSALS: CCU_PROPOSALS_T }, true);
            $(this).jqGrid('footerData', 'set', { CCU_LEN: CCU_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CCU_AMT: CCU_AMT_T }, true);
            $(this).jqGrid('footerData', 'set', { CCTOT_PROP: CCTOT_PROP_T }, true);
            $(this).jqGrid('footerData', 'set', { CCTOT_LEN: CCTOT_LEN_T }, true);
            $(this).jqGrid('footerData', 'set', { CCTOT_AMT: CCTOT_AMT_T }, true);
            $("#MPR1BlockReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Cost Rs in Lacs & Length in Kms.</font>");
            $('#MPR1BlockReportTable_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        //onSelectRow: function (id) {
        //    //alert(id);
        //    var params = id.split('$');
        //  //  MPR1FinalReportListing(params[0], params[1], params[2], params[3], params[4], params[5],params[6]);

        //},
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

    $("#MPR1BlockReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          //{ startColumnName: 'TN_PROPOSALS', numberOfColumns: 7, titleText: '<em> Total New Connectivity</em>' },
          //{ startColumnName: 'TU_PROPOSALS', numberOfColumns: 3, titleText: '<em> Total Upgradation</em>' },
          //{ startColumnName: 'TOT_PROP', numberOfColumns: 3, titleText: '<em> Total</em>' },
          //{ startColumnName: 'CN_PROPOSALS', numberOfColumns: 3, titleText: '<em> Completed New Connectivity</em>' },
          //{ startColumnName: 'CPOP1000', numberOfColumns: 4, titleText: '<em> Completed Upgradation</em>' },
          //{ startColumnName: 'CU_PROPOSALS', numberOfColumns: 3, titleText: '<em> Total</em>' },
          //{ startColumnName: 'CTOT_PROP', numberOfColumns: 3, titleText: '<em> Completed New Connectivity</em>' },
          //{ startColumnName: 'CCN_PROPOSALS', numberOfColumns: 3, titleText: '<em> Total Upgradation</em>' },
          //{ startColumnName: 'CCPOP1000', numberOfColumns: 4, titleText: '<em> Cumulative Completed</em>' },
          //{ startColumnName: 'CCU_PROPOSALS', numberOfColumns: 3, titleText: '<em> Cumulative Upgradation</em>' },
          //{ startColumnName: 'CCTOT_PROP', numberOfColumns: 3, titleText: '<em> Cumulative Total</em>' }
            {
                startColumnName: 'TN_PROPOSALS', numberOfColumns: 13,
                titleText: '<table style="width:100%;border-spacing:0px"' +
                        '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="13">Clearance </td>  </tr>' +
                        '<tr>' +
                            '<td id="h1" colspan="7" style="width: 51.5%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">New Connectivity</td>' +
                              '<td id="h1" colspan="3" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Upgradation</td>' +
                              '<td id="h2" colspan="3" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Total</td>' +
                        '</tr>' +
                        '</table>'
            },

            {
                startColumnName: 'CN_PROPOSALS', numberOfColumns: 13,
                titleText: '<table style="width:100%;border-spacing:0px"' +
                        '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="13">Progress during the month </td>  </tr>' +
                        '<tr>' +
                            '<td id="h1" colspan="7" style="width: 51.5%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">New Connectivity</td>' +
                              '<td id="h1" colspan="3" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Upgradation</td>' +
                              '<td id="h2" colspan="3" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Total</td>' +
                        '</tr>' +
                        '</table>'
            },

              {
                  startColumnName: 'CCN_PROPOSALS', numberOfColumns: 13,
                  titleText: '<table style="width:100%;border-spacing:0px"' +
                          '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="13">Cummulative Progress </td>  </tr>' +
                          '<tr>' +
                              '<td id="h1" colspan="7" style="width: 51.5%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">New Connectivity</td>' +
                                '<td id="h1" colspan="3" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Upgradation</td>' +
                                '<td id="h2" colspan="3" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Total</td>' +
                          '</tr>' +
                          '</table>'
              },
        ]
    });
}

/*       FINAL REPORT LISTING       */
function MPR1FinalReportListing(blockCode, districtCode, stateCode, blockName, year, month, collaboration) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#MPR1BlockReportTable").jqGrid('setGridState', 'hidden');
    $("#MPR1DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#MPR1StateReportTable").jqGrid('setGridState', 'hidden');
    $("#MPR1BlockReportTable").jqGrid('setSelection', stateCode);
    $("#MPR1FinalReportTable").jqGrid('GridUnload');


    $("#MPR1FinalReportTable").jqGrid({
        url: '/ProposalReports/MPR1FinalReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Sanctioned Year', 'IMS Batch', 'Package ID', 'Proposal Type', 'Road Name', 'Bridge Name', 'Upgrade Connect', 'Pavilion Length', 'Bridge Length', 'Collaboration', 'Road Cost', 'Bridge Cost', 'Maintainance Cost', 'Total Length Completed', 'Total Expenses'],
        colModel: [
            { name: "MAST_BLOCK_NAME", width: 200, align: 'left', height: 'auto', sortable: true },
            { name: "IMS_YEAR", width: 100, align: 'left', height: 'auto', sortable: false },
            { name: "IMS_BATCH", width: 100, align: 'right', height: 'auto', sortable: false },
            { name: "IMS_PACKAGE_ID", width: 100, align: 'left', height: 'auto', sortable: false },
            { name: "IMS_PROPOSAL_TYPE", width: 100, align: 'left', height: 'auto', sortable: false },
            { name: "IMS_ROAD_NAME", width: 100, align: 'left', height: 'auto', sortable: false },
            { name: "IMS_BRIDGE_NAME", width: 100, align: 'left', height: 'auto', sortable: false },
            { name: "IMS_UPGRADE_CONNECT", width: 100, align: 'left', height: 'auto', sortable: false },
            { name: "IMS_PAV_LENGTH", width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "IMS_BRIDGE_LENGTH", width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "IMS_COLLABORATION", width: 100, align: 'left', height: 'auto', sortable: false },
            { name: "ROAD_AMT", width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "BRIDGE_AMT", width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MAINT_AMT", width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_LENGTH_COMPLETED", width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TOTAL_EXP", width: 100, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode, "Year": year, "Month": month, "Collaboration": collaboration },
        pager: $("#MPR1FinalReportPager"),
        footerrow: true,
        sortname: 'MAST_BLOCK_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        shrinkToFit: false,
        width: 1100,
        autowidth: false,
        height: '400',
        viewrecords: true,
        caption: 'MPR1 Details for ' + blockName,
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
            $("#MPR1FinalReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Cost Rs in Lacs & Length in Kms.</font>");
            $('#MPR1FinalReportTable_rn').html('Sr.<br/>No.');

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