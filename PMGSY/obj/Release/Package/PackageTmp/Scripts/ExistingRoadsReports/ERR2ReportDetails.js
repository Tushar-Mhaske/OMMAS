$(document).ready(function () {
    if ($("#hdnLevelId").val() == 6) //mord
    {      
           ERR2StateReportListing();
    }
    else if ($("#hdnLevelId").val() == 4) //state
    {
          ERR2DistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val());
    }
    else if ($("#hdnLevelId").val() == 5) //District
    {
        ERR2BlockReportListing($("#MAST_STATE_CODE").val(), $("#MAST_DISTRICT_CODE").val(), $("#BLOCK_NAME").val());
    }
    //$('#tblRptContents').bind('resize', function () {
    //    resizeJqGrid();
    //}).trigger('resize');
});


/*       STATE REPORT LISTING       */
function ERR2StateReportListing() {
   
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR2DistrictReportTable").jqGrid('GridUnload');
    $("#ERR2BlockReportTable").jqGrid('GridUnload');
    $("#ERR2FinalReportTable").jqGrid('GridUnload');
    $("#ERR2StateReportTable").jqGrid('GridUnload');
    $("#ERR2StateReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR2StateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number',
            'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number',
            'Length', 'Number', 'Length', 'Number', 'Length'],
        colModel: [
            { name: 'StateName', width: 200, align: 'left',  height: 'auto', sortable: true },
            { name: 'TOTAL_NH', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_NH_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CNH', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CNH_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: 'TOTAL_SH', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_SH_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CSH', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CSH_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: 'TOTAL_MDR', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_MDR_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CMDR', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CMDR_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: 'TOTAL_RR', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_RR_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CRR', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CRR_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: 'TOTAL_OTHER', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_OTHER_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_COTHER', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_COTHER_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: 'Total', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Total_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TotalC', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TotalC_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } }

        ],
        pager: $("#ERR2StateReportPager"),
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'StateName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: false,
        height: '580',
        width: '1120',
        shrinkToFit: false,
        viewrecords: true,
        caption: 'State DRRP Roads Mapped with CN',
        loadComplete: function () {
           
            //Total of Columns
            var TOTAL_NHT = $(this).jqGrid('getCol', 'TOTAL_NH', false, 'sum');
            var TOTAL_NH_LENT = $(this).jqGrid('getCol', 'TOTAL_NH_LEN', false, 'sum');
            TOTAL_NH_LENT = parseFloat(TOTAL_NH_LENT).toFixed(3);
            var TOTAL_CNHT = $(this).jqGrid('getCol', 'TOTAL_CNH', false, 'sum');
            var TOTAL_CNH_LENT = $(this).jqGrid('getCol', 'TOTAL_CNH_LEN', false, 'sum');
            TOTAL_CNH_LENT = parseFloat(TOTAL_CNH_LENT).toFixed(3);

            var TOTAL_SHT = $(this).jqGrid('getCol', 'TOTAL_SH', false, 'sum');
            var TOTAL_SH_LENT = $(this).jqGrid('getCol', 'TOTAL_SH_LEN', false, 'sum');
            TOTAL_SH_LENT = parseFloat(TOTAL_SH_LENT).toFixed(3);
            var TOTAL_CSHT = $(this).jqGrid('getCol', 'TOTAL_CSH', false, 'sum');
            var TOTAL_CSH_LENT = $(this).jqGrid('getCol', 'TOTAL_CSH_LEN', false, 'sum');
            TOTAL_CSH_LENT = parseFloat(TOTAL_CSH_LENT).toFixed(3);

            var TOTAL_MDRT = $(this).jqGrid('getCol', 'TOTAL_MDR', false, 'sum');
            var TOTAL_MDR_LENT = $(this).jqGrid('getCol', 'TOTAL_MDR_LEN', false, 'sum');
            TOTAL_MDR_LENT = parseFloat(TOTAL_MDR_LENT).toFixed(3);
            var TOTAL_CMDRT = $(this).jqGrid('getCol', 'TOTAL_CMDR', false, 'sum');
            var TOTAL_CMDR_LENT = $(this).jqGrid('getCol', 'TOTAL_CMDR_LEN', false, 'sum');
            TOTAL_CMDR_LENT = parseFloat(TOTAL_CMDR_LENT).toFixed(3);

            var TOTAL_RRT = $(this).jqGrid('getCol', 'TOTAL_RR', false, 'sum');
            var TOTAL_RR_LENT = $(this).jqGrid('getCol', 'TOTAL_RR_LEN', false, 'sum');
            TOTAL_RR_LENT = parseFloat(TOTAL_RR_LENT).toFixed(3);
            var TOTAL_CRRT = $(this).jqGrid('getCol', 'TOTAL_CRR', false, 'sum');
            var TOTAL_CRR_LENT = $(this).jqGrid('getCol', 'TOTAL_CRR_LEN', false, 'sum');
            TOTAL_CRR_LENT = parseFloat(TOTAL_CRR_LENT).toFixed(3);

            var TOTAL_OTHERT = $(this).jqGrid('getCol', 'TOTAL_OTHER', false, 'sum');
            var TOTAL_OTHER_LENT = $(this).jqGrid('getCol', 'TOTAL_OTHER_LEN', false, 'sum');
            TOTAL_OTHER_LENT = parseFloat(TOTAL_OTHER_LENT).toFixed(3);
            var TOTAL_COTHERT = $(this).jqGrid('getCol', 'TOTAL_COTHER', false, 'sum');
            var TOTAL_COTHER_LENT = $(this).jqGrid('getCol', 'TOTAL_COTHER_LEN', false, 'sum');
            TOTAL_COTHER_LENT = parseFloat(TOTAL_COTHER_LENT).toFixed(3);

            var TotalT = $(this).jqGrid('getCol', 'Total', false, 'sum');
            var Total_LENT = $(this).jqGrid('getCol', 'Total_LEN', false, 'sum');
            Total_LENT = parseFloat(Total_LENT).toFixed(3);
            var TotalCT = $(this).jqGrid('getCol', 'TotalC', false, 'sum');
            var TotalC_LENT = $(this).jqGrid('getCol', 'TotalC_LEN', false, 'sum');
            TotalC_LENT = parseFloat(TotalC_LENT).toFixed(3);

            //
            $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_NH: TOTAL_NHT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_NH_LEN: TOTAL_NH_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CNH: TOTAL_CNHT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CNH_LEN: TOTAL_CNH_LENT }, true);

            $(this).jqGrid('footerData', 'set', { TOTAL_SH: TOTAL_SHT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_SH_LEN: TOTAL_SH_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CSH: TOTAL_CSHT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CSH_LEN: TOTAL_CSH_LENT }, true);

            $(this).jqGrid('footerData', 'set', { TOTAL_MDR: TOTAL_MDRT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_MDR_LEN: TOTAL_MDR_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CMDR: TOTAL_CMDRT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CMDR_LEN: TOTAL_CMDR_LENT }, true);

            $(this).jqGrid('footerData', 'set', { TOTAL_RR: TOTAL_RRT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_RR_LEN: TOTAL_RR_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CRR: TOTAL_CRRT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CRR_LEN: TOTAL_CRR_LENT }, true);

            $(this).jqGrid('footerData', 'set', { TOTAL_OTHER: TOTAL_OTHERT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_OTHER_LEN: TOTAL_OTHER_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_COTHER: TOTAL_COTHERT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_COTHER_LEN: TOTAL_COTHER_LENT }, true);


            $(this).jqGrid('footerData', 'set', { Total: TotalT }, true);
            $(this).jqGrid('footerData', 'set', { Total_LEN: Total_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TotalC: TotalCT }, true);
            $(this).jqGrid('footerData', 'set', { TotalC_LEN: TotalC_LENT }, true);
            $("#ERR2StateReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#ERR2StateReportTable_rn').html('Sr.<br/>No.');

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

    $("#ERR2StateReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          //{ startColumnName: 'TOTAL_NH', numberOfColumns: 2, titleText: '<em>NH DRRP</em>' },
          //{ startColumnName: 'TOTAL_CNH', numberOfColumns: 2, titleText: '<em>NH Included in CN</em>' },

          //{ startColumnName: 'TOTAL_SH', numberOfColumns: 2, titleText: '<em>SH DRRP </em>' },
          //{ startColumnName: 'TOTAL_CSH', numberOfColumns: 2, titleText: '<em>SH Included in CN </em>' },

          //{ startColumnName: 'TOTAL_MDR', numberOfColumns: 2, titleText: '<em>MDR DRRP </em>' },
          //{ startColumnName: 'TOTAL_CMDR', numberOfColumns: 2, titleText: '<em>MDR Included in CN</em>' },

          //{ startColumnName: 'TOTAL_RR', numberOfColumns: 2, titleText: '<em>RR DRRP </em>' },
          //{ startColumnName: 'TOTAL_CRR', numberOfColumns: 2, titleText: '<em>RR Included in CN </em>' },

          //{ startColumnName: 'TOTAL_OTHER', numberOfColumns: 2, titleText: '<em>Total Other DRRP</em>' },
          //{ startColumnName: 'TOTAL_COTHER', numberOfColumns: 2, titleText: '<em>Total Other Included in CN </em>' },

          //{ startColumnName: 'Total', numberOfColumns: 2, titleText: '<em>Total DRRP</em>' },
          //{ startColumnName: 'TotalC', numberOfColumns: 2, titleText: '<em>Total Included in CN</em>' }
           {
               startColumnName: 'TOTAL_NH', numberOfColumns: 4,
               titleText: '<table style="width:100%;border-spacing:0px"' +
                       '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">NH </td>  </tr>' +
                       '<tr>' +
                           '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                           '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                       '</tr>' +
                       '</table>'
           },

         {
             startColumnName: 'TOTAL_SH', numberOfColumns: 4,
                titleText: '<table style="width:100%;border-spacing:0px"' +
                        '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">SH </td>  </tr>' +
                        '<tr>' +
                            '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                            '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                        '</tr>' +
                        '</table>'
         },
        {
            startColumnName: 'TOTAL_MDR', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                    '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">MDR </td>  </tr>' +
                    '<tr>' +
                        '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                        '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                    '</tr>' +
                    '</table>'
        },
        {
            startColumnName: 'TOTAL_RR', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                    '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">RR </td>  </tr>' +
                    '<tr>' +
                        '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                        '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                    '</tr>' +
                    '</table>'
        },

        {
            startColumnName: 'TOTAL_OTHER', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                    '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Other </td>  </tr>' +
                    '<tr>' +
                        '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                        '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                    '</tr>' +
                    '</table>'
        },

        {
            startColumnName: 'Total', numberOfColumns: 4,
             titleText: '<table style="width:100%;border-spacing:0px"' +
                     '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Total </td>  </tr>' +
                     '<tr>' +
                         '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                         '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                     '</tr>' +
                     '</table>'
        },
        ]
    });
}
/**/

/*       DISTRICT REPORT LISTING       */
function ERR2DistrictReportListing(stateCode, stateName) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR2StateReportTable").jqGrid('setSelection', stateCode);
    $("#ERR2StateReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR2BlockReportTable").jqGrid('GridUnload');
    $("#ERR2FinalReportTable").jqGrid('GridUnload');
    $("#ERR2DistrictReportTable").jqGrid('GridUnload');

    $("#ERR2DistrictReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR2DistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number',
            'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number',
            'Length', 'Number', 'Length', 'Number', 'Length'],
        colModel: [
            { name: 'DistrictName', width: 200, align: 'left',  height: 'auto', sortable: true },
            { name: 'TOTAL_NH', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_NH_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CNH', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CNH_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: 'TOTAL_SH', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_SH_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CSH', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CSH_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: 'TOTAL_MDR', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_MDR_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CMDR', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CMDR_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: 'TOTAL_RR', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_RR_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CRR', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CRR_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: 'TOTAL_OTHER', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_OTHER_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_COTHER', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_COTHER_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: 'Total', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Total_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TotalC', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TotalC_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } }

        ],
        pager: $("#ERR2DistrictReportPager"),
        postData: { 'StateCode': stateCode },
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'DistrictName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: false,
        height: '550',
        width: '1120',
        shrinkToFit: false,
        viewrecords: true,
        caption: 'District DRRP Roads Mapped with CN for ' + stateName,
        loadComplete: function () {
             //Total of Columns
            var TOTAL_NHT = $(this).jqGrid('getCol', 'TOTAL_NH', false, 'sum');
            var TOTAL_NH_LENT = $(this).jqGrid('getCol', 'TOTAL_NH_LEN', false, 'sum');
            TOTAL_NH_LENT = parseFloat(TOTAL_NH_LENT).toFixed(3);
            var TOTAL_CNHT = $(this).jqGrid('getCol', 'TOTAL_CNH', false, 'sum');
            var TOTAL_CNH_LENT = $(this).jqGrid('getCol', 'TOTAL_CNH_LEN', false, 'sum');
            TOTAL_CNH_LENT = parseFloat(TOTAL_CNH_LENT).toFixed(3);

            var TOTAL_SHT = $(this).jqGrid('getCol', 'TOTAL_SH', false, 'sum');
            var TOTAL_SH_LENT = $(this).jqGrid('getCol', 'TOTAL_SH_LEN', false, 'sum');
            TOTAL_SH_LENT = parseFloat(TOTAL_SH_LENT).toFixed(3);
            var TOTAL_CSHT = $(this).jqGrid('getCol', 'TOTAL_CSH', false, 'sum');
            var TOTAL_CSH_LENT = $(this).jqGrid('getCol', 'TOTAL_CSH_LEN', false, 'sum');
            TOTAL_CSH_LENT = parseFloat(TOTAL_CSH_LENT).toFixed(3);

            var TOTAL_MDRT = $(this).jqGrid('getCol', 'TOTAL_MDR', false, 'sum');
            var TOTAL_MDR_LENT = $(this).jqGrid('getCol', 'TOTAL_MDR_LEN', false, 'sum');
            TOTAL_MDR_LENT = parseFloat(TOTAL_MDR_LENT).toFixed(3);
            var TOTAL_CMDRT = $(this).jqGrid('getCol', 'TOTAL_CMDR', false, 'sum');
            var TOTAL_CMDR_LENT = $(this).jqGrid('getCol', 'TOTAL_CMDR_LEN', false, 'sum');
            TOTAL_CMDR_LENT = parseFloat(TOTAL_CMDR_LENT).toFixed(3);

            var TOTAL_RRT = $(this).jqGrid('getCol', 'TOTAL_RR', false, 'sum');
            var TOTAL_RR_LENT = $(this).jqGrid('getCol', 'TOTAL_RR_LEN', false, 'sum');
            TOTAL_RR_LENT = parseFloat(TOTAL_RR_LENT).toFixed(3);
            var TOTAL_CRRT = $(this).jqGrid('getCol', 'TOTAL_CRR', false, 'sum');
            var TOTAL_CRR_LENT = $(this).jqGrid('getCol', 'TOTAL_CRR_LEN', false, 'sum');
            TOTAL_CRR_LENT = parseFloat(TOTAL_CRR_LENT).toFixed(3);

            var TOTAL_OTHERT = $(this).jqGrid('getCol', 'TOTAL_OTHER', false, 'sum');
            var TOTAL_OTHER_LENT = $(this).jqGrid('getCol', 'TOTAL_OTHER_LEN', false, 'sum');
            TOTAL_OTHER_LENT = parseFloat(TOTAL_OTHER_LENT).toFixed(3);
            var TOTAL_COTHERT = $(this).jqGrid('getCol', 'TOTAL_COTHER', false, 'sum');
            var TOTAL_COTHER_LENT = $(this).jqGrid('getCol', 'TOTAL_COTHER_LEN', false, 'sum');
            TOTAL_COTHER_LENT = parseFloat(TOTAL_COTHER_LENT).toFixed(3);

            var TotalT = $(this).jqGrid('getCol', 'Total', false, 'sum');
            var Total_LENT = $(this).jqGrid('getCol', 'Total_LEN', false, 'sum');
            Total_LENT = parseFloat(Total_LENT).toFixed(3);
            var TotalCT = $(this).jqGrid('getCol', 'TotalC', false, 'sum');
            var TotalC_LENT = $(this).jqGrid('getCol', 'TotalC_LEN', false, 'sum');
            TotalC_LENT = parseFloat(TotalC_LENT).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_NH: TOTAL_NHT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_NH_LEN: TOTAL_NH_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CNH: TOTAL_CNHT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CNH_LEN: TOTAL_CNH_LENT }, true);

            $(this).jqGrid('footerData', 'set', { TOTAL_SH: TOTAL_SHT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_SH_LEN: TOTAL_SH_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CSH: TOTAL_CSHT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CSH_LEN: TOTAL_CSH_LENT }, true);

            $(this).jqGrid('footerData', 'set', { TOTAL_MDR: TOTAL_MDRT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_MDR_LEN: TOTAL_MDR_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CMDR: TOTAL_CMDRT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CMDR_LEN: TOTAL_CMDR_LENT }, true);

            $(this).jqGrid('footerData', 'set', { TOTAL_RR: TOTAL_RRT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_RR_LEN: TOTAL_RR_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CRR: TOTAL_CRRT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CRR_LEN: TOTAL_CRR_LENT }, true);

            $(this).jqGrid('footerData', 'set', { TOTAL_OTHER: TOTAL_OTHERT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_OTHER_LEN: TOTAL_OTHER_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_COTHER: TOTAL_COTHERT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_COTHER_LEN: TOTAL_COTHER_LENT }, true);


            $(this).jqGrid('footerData', 'set', { Total: TotalT }, true);
            $(this).jqGrid('footerData', 'set', { Total_LEN: Total_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TotalC: TotalCT }, true);
            $(this).jqGrid('footerData', 'set', { TotalC_LEN: TotalC_LENT }, true);
            $("#ERR2DistrictReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#ERR2DistrictReportTable_rn').html('Sr.<br/>No.');

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

    $("#ERR2DistrictReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
         //{ startColumnName: 'TOTAL_NH', numberOfColumns: 2, titleText: '<em>NH DRRP</em>' },
         //{ startColumnName: 'TOTAL_CNH', numberOfColumns: 2, titleText: '<em>NH Included in CN</em>' },

         //{ startColumnName: 'TOTAL_SH', numberOfColumns: 2, titleText: '<em>SH DRRP </em>' },
         //{ startColumnName: 'TOTAL_CSH', numberOfColumns: 2, titleText: '<em>SH Included in CN </em>' },

         //{ startColumnName: 'TOTAL_MDR', numberOfColumns: 2, titleText: '<em>MDR DRRP </em>' },
         //{ startColumnName: 'TOTAL_CMDR', numberOfColumns: 2, titleText: '<em>MDR Included in CN</em>' },

         //{ startColumnName: 'TOTAL_RR', numberOfColumns: 2, titleText: '<em>RR DRRP </em>' },
         //{ startColumnName: 'TOTAL_CRR', numberOfColumns: 2, titleText: '<em>RR Included in CN </em>' },

         //{ startColumnName: 'TOTAL_OTHER', numberOfColumns: 2, titleText: '<em>Total Other DRRP</em>' },
         //{ startColumnName: 'TOTAL_COTHER', numberOfColumns: 2, titleText: '<em>Total Other Included in CN </em>' },

         //{ startColumnName: 'Total', numberOfColumns: 2, titleText: '<em>Total DRRP</em>' },
         //{ startColumnName: 'TotalC', numberOfColumns: 2, titleText: '<em>Total Included in CN</em>' }
          {
              startColumnName: 'TOTAL_NH', numberOfColumns: 4,
              titleText: '<table style="width:100%;border-spacing:0px"' +
                      '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">NH </td>  </tr>' +
                      '<tr>' +
                          '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                          '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                      '</tr>' +
                      '</table>'
          },

        {
            startColumnName: 'TOTAL_SH', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                    '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">SH </td>  </tr>' +
                    '<tr>' +
                        '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                        '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                    '</tr>' +
                    '</table>'
        },
       {
           startColumnName: 'TOTAL_MDR', numberOfColumns: 4,
           titleText: '<table style="width:100%;border-spacing:0px"' +
                   '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">MDR </td>  </tr>' +
                   '<tr>' +
                       '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                       '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                   '</tr>' +
                   '</table>'
       },
       {
           startColumnName: 'TOTAL_RR', numberOfColumns: 4,
           titleText: '<table style="width:100%;border-spacing:0px"' +
                   '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">RR </td>  </tr>' +
                   '<tr>' +
                       '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                       '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                   '</tr>' +
                   '</table>'
       },

       {
           startColumnName: 'TOTAL_OTHER', numberOfColumns: 4,
           titleText: '<table style="width:100%;border-spacing:0px"' +
                   '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Other </td>  </tr>' +
                   '<tr>' +
                       '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                       '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                   '</tr>' +
                   '</table>'
       },

       {
           startColumnName: 'Total', numberOfColumns: 4,
           titleText: '<table style="width:100%;border-spacing:0px"' +
                   '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Total </td>  </tr>' +
                   '<tr>' +
                       '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                       '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                   '</tr>' +
                   '</table>'
       },
        ]
    });
}
/**/

/*      BLOCK REPORT LISTING       */

function ERR2BlockReportListing(stateCode, districtCode, districtName) {

    var distname;
    if (districtName == '')
        distname = $("#DISTRICT_NAME").val();
    else
        distname = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR2DistrictReportTable").jqGrid('setSelection', districtCode);
    $("#ERR2DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR2StateReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR2FinalReportTable").jqGrid('GridUnload');
    $("#ERR2BlockReportTable").jqGrid('GridUnload');

    $("#ERR2BlockReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR2BlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number',
           'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number',
           'Length', 'Number', 'Length', 'Number', 'Length'],
        colModel: [
            { name: 'BlockName', width: 200, align: 'left',  height: 'auto', sortable: true },
             { name: 'TOTAL_NH', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_NH_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CNH', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CNH_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: 'TOTAL_SH', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_SH_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CSH', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CSH_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: 'TOTAL_MDR', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_MDR_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CMDR', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CMDR_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: 'TOTAL_RR', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_RR_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CRR', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_CRR_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: 'TOTAL_OTHER', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_OTHER_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_COTHER', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TOTAL_COTHER_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },

            { name: 'Total', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Total_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TotalC', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'TotalC_LEN', width: 80, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 3, defaulValue: "N.A" } }
        ],
        pager: $("#ERR2BlockReportPager"),
        postData: { 'StateCode': stateCode, 'DistrictCode': districtCode },
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'DistrictName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: false,
        width: '1120',
        shrinkToFit: false,
        height: '500',
        viewrecords: true,
        caption: 'Block DRRP Roads Mapped with CN for ' + distname,
        loadComplete: function () {
            //Total of Columns
            var TOTAL_NHT = $(this).jqGrid('getCol', 'TOTAL_NH', false, 'sum');
            var TOTAL_NH_LENT = $(this).jqGrid('getCol', 'TOTAL_NH_LEN', false, 'sum');
            TOTAL_NH_LENT = parseFloat(TOTAL_NH_LENT).toFixed(3);
            var TOTAL_CNHT = $(this).jqGrid('getCol', 'TOTAL_CNH', false, 'sum');
            var TOTAL_CNH_LENT = $(this).jqGrid('getCol', 'TOTAL_CNH_LEN', false, 'sum');
            TOTAL_CNH_LENT = parseFloat(TOTAL_CNH_LENT).toFixed(3);

            var TOTAL_SHT = $(this).jqGrid('getCol', 'TOTAL_SH', false, 'sum');
            var TOTAL_SH_LENT = $(this).jqGrid('getCol', 'TOTAL_SH_LEN', false, 'sum');
            TOTAL_SH_LENT = parseFloat(TOTAL_SH_LENT).toFixed(3);
            var TOTAL_CSHT = $(this).jqGrid('getCol', 'TOTAL_CSH', false, 'sum');
            var TOTAL_CSH_LENT = $(this).jqGrid('getCol', 'TOTAL_CSH_LEN', false, 'sum');
            TOTAL_CSH_LENT = parseFloat(TOTAL_CSH_LENT).toFixed(3);

            var TOTAL_MDRT = $(this).jqGrid('getCol', 'TOTAL_MDR', false, 'sum');
            var TOTAL_MDR_LENT = $(this).jqGrid('getCol', 'TOTAL_MDR_LEN', false, 'sum');
            TOTAL_MDR_LENT = parseFloat(TOTAL_MDR_LENT).toFixed(3);
            var TOTAL_CMDRT = $(this).jqGrid('getCol', 'TOTAL_CMDR', false, 'sum');
            var TOTAL_CMDR_LENT = $(this).jqGrid('getCol', 'TOTAL_CMDR_LEN', false, 'sum');
            TOTAL_CMDR_LENT = parseFloat(TOTAL_CMDR_LENT).toFixed(3);

            var TOTAL_RRT = $(this).jqGrid('getCol', 'TOTAL_RR', false, 'sum');
            var TOTAL_RR_LENT = $(this).jqGrid('getCol', 'TOTAL_RR_LEN', false, 'sum');
            TOTAL_RR_LENT = parseFloat(TOTAL_RR_LENT).toFixed(3);
            var TOTAL_CRRT = $(this).jqGrid('getCol', 'TOTAL_CRR', false, 'sum');
            var TOTAL_CRR_LENT = $(this).jqGrid('getCol', 'TOTAL_CRR_LEN', false, 'sum');
            TOTAL_CRR_LENT = parseFloat(TOTAL_CRR_LENT).toFixed(3);

            var TOTAL_OTHERT = $(this).jqGrid('getCol', 'TOTAL_OTHER', false, 'sum');
            var TOTAL_OTHER_LENT = $(this).jqGrid('getCol', 'TOTAL_OTHER_LEN', false, 'sum');
            TOTAL_OTHER_LENT = parseFloat(TOTAL_OTHER_LENT).toFixed(3);
            var TOTAL_COTHERT = $(this).jqGrid('getCol', 'TOTAL_COTHER', false, 'sum');
            var TOTAL_COTHER_LENT = $(this).jqGrid('getCol', 'TOTAL_COTHER_LEN', false, 'sum');
            TOTAL_COTHER_LENT = parseFloat(TOTAL_COTHER_LENT).toFixed(3);

            var TotalT = $(this).jqGrid('getCol', 'Total', false, 'sum');
            var Total_LENT = $(this).jqGrid('getCol', 'Total_LEN', false, 'sum');
            Total_LENT = parseFloat(Total_LENT).toFixed(3);
            var TotalCT = $(this).jqGrid('getCol', 'TotalC', false, 'sum');
            var TotalC_LENT = $(this).jqGrid('getCol', 'TotalC_LEN', false, 'sum');
            TotalC_LENT = parseFloat(TotalC_LENT).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { BlockName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TOTAL_NH: TOTAL_NHT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_NH_LEN: TOTAL_NH_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CNH: TOTAL_CNHT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CNH_LEN: TOTAL_CNH_LENT }, true);

            $(this).jqGrid('footerData', 'set', { TOTAL_SH: TOTAL_SHT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_SH_LEN: TOTAL_SH_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CSH: TOTAL_CSHT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CSH_LEN: TOTAL_CSH_LENT }, true);

            $(this).jqGrid('footerData', 'set', { TOTAL_MDR: TOTAL_MDRT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_MDR_LEN: TOTAL_MDR_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CMDR: TOTAL_CMDRT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CMDR_LEN: TOTAL_CMDR_LENT }, true);

            $(this).jqGrid('footerData', 'set', { TOTAL_RR: TOTAL_RRT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_RR_LEN: TOTAL_RR_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CRR: TOTAL_CRRT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_CRR_LEN: TOTAL_CRR_LENT }, true);

            $(this).jqGrid('footerData', 'set', { TOTAL_OTHER: TOTAL_OTHERT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_OTHER_LEN: TOTAL_OTHER_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_COTHER: TOTAL_COTHERT }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_COTHER_LEN: TOTAL_COTHER_LENT }, true);


            $(this).jqGrid('footerData', 'set', { Total: TotalT }, true);
            $(this).jqGrid('footerData', 'set', { Total_LEN: Total_LENT }, true);
            $(this).jqGrid('footerData', 'set', { TotalC: TotalCT }, true);
            $(this).jqGrid('footerData', 'set', { TotalC_LEN: TotalC_LENT }, true);
            $("#ERR2BlockReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#ERR2BlockReportTable_rn').html('Sr.<br/>No.');

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

    $("#ERR2BlockReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          //{ startColumnName: 'TOTAL_NH', numberOfColumns: 2, titleText: '<em>NH DRRP</em>' },
          //{ startColumnName: 'TOTAL_CNH', numberOfColumns: 2, titleText: '<em>NH Included in CN</em>' },

          //{ startColumnName: 'TOTAL_SH', numberOfColumns: 2, titleText: '<em>SH DRRP </em>' },
          //{ startColumnName: 'TOTAL_CSH', numberOfColumns: 2, titleText: '<em>SH Included in CN </em>' },

          //{ startColumnName: 'TOTAL_MDR', numberOfColumns: 2, titleText: '<em>MDR DRRP </em>' },
          //{ startColumnName: 'TOTAL_CMDR', numberOfColumns: 2, titleText: '<em>MDR Included in CN</em>' },

          //{ startColumnName: 'TOTAL_RR', numberOfColumns: 2, titleText: '<em>RR DRRP </em>' },
          //{ startColumnName: 'TOTAL_CRR', numberOfColumns: 2, titleText: '<em>RR Included in CN </em>' },

          //{ startColumnName: 'TOTAL_OTHER', numberOfColumns: 2, titleText: '<em>Total Other DRRP</em>' },
          //{ startColumnName: 'TOTAL_COTHER', numberOfColumns: 2, titleText: '<em>Total Other Included in CN </em>' },

          //{ startColumnName: 'Total', numberOfColumns: 2, titleText: '<em>Total DRRP</em>' },
          //{ startColumnName: 'TotalC', numberOfColumns: 2, titleText: '<em>Total Included in CN</em>' }
           {
               startColumnName: 'TOTAL_NH', numberOfColumns: 4,
               titleText: '<table style="width:100%;border-spacing:0px"' +
                       '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">NH </td>  </tr>' +
                       '<tr>' +
                           '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                           '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                       '</tr>' +
                       '</table>'
           },

         {
             startColumnName: 'TOTAL_SH', numberOfColumns: 4,
             titleText: '<table style="width:100%;border-spacing:0px"' +
                     '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">SH </td>  </tr>' +
                     '<tr>' +
                         '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                         '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                     '</tr>' +
                     '</table>'
         },
        {
            startColumnName: 'TOTAL_MDR', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                    '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">MDR </td>  </tr>' +
                    '<tr>' +
                        '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                        '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                    '</tr>' +
                    '</table>'
        },
        {
            startColumnName: 'TOTAL_RR', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                    '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">RR </td>  </tr>' +
                    '<tr>' +
                        '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                        '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                    '</tr>' +
                    '</table>'
        },

        {
            startColumnName: 'TOTAL_OTHER', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                    '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Other </td>  </tr>' +
                    '<tr>' +
                        '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                        '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                    '</tr>' +
                    '</table>'
        },

        {
            startColumnName: 'Total', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                    '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Total </td>  </tr>' +
                    '<tr>' +
                        '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                        '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                    '</tr>' +
                    '</table>'
        },
        ]
    });
}

/*       FINAL BLOCK REPORT LISTING       */

function ERR2FinalReportListing(blockCode, districtCode, stateCode, blockName) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR2BlockReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR2DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR2StateReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR2BlockReportTable").jqGrid('setSelection', blockCode);
    $("#ERR2FinalReportTable").jqGrid('GridUnload');

    $("#ERR2FinalReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR2FinalReportListing?' + Math.random(),
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Road Code', 'Road Name', 'Road Category', 'Road Type', 'Length (Kms.)', 'Year of Construction', 'Included in Core Network (Y/N)', 'Habitations Status (Y/N)', 'Habitations Name','Population', 'Soil type', 'Terrain Type'],
        colModel: [
            { name: 'PlannedRoadNumber', width: 150, align: 'left',  height: 'auto', sortable: true },
            { name: 'PlannedRoadName', width: 250, align: 'left',  height: 'auto', sortable: false },
            { name: 'MAST_ROAD_CAT_CODE', width: 120, align: 'left',  height: 'auto', sortable: false },
            { name: 'MAST_ER_ROAD_TYPE', width: 150, align: 'left',  height: 'auto', sortable: false },
            { name: 'ROAD_LENGTH', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MAST_CONS_YEAR', width: 120, align: 'right',  height: 'auto', sortable: false },
            { name: 'MAST_CORE_NETWORK', width: 120, align: 'center',  height: 'auto', sortable: false },
            { name: 'MAST_HAB_STATUS', width: 120, align: 'center',  height: 'auto', sortable: false },
            { name: 'MAST_HAB_NAME', width: 120, align: 'left',  height: 'auto', sortable: false },
            { name: 'MAST_HAB_TOT_POP', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: {thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MAST_SOIL_TYPE_NAME', width: 200, align: 'left',  height: 'auto', sortable: false },
            { name: 'MAST_TERRAIN_TYPE_NAME', width: 200, align: 'left',  height: 'auto', sortable: false }

        ],
        postData: { "BlockCode": blockCode, "StateCode": stateCode, "DistrictCode": districtCode },
        rowNum: '2147483647',
        pager: $("#ERR2FinalReportPager"),
        footerrow: true,
        pgbuttons: true,
        sortname: 'PlannedRoadNumber',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '450',
        viewrecords: true,
        caption: 'DRRP Roads Mapped with CN for ' + blockName,
        loadComplete: function () {
            //Total of Columns
            var ROAD_LENGTHT = $(this).jqGrid('getCol', 'ROAD_LENGTH', false, 'sum');
            var RoadLengthT = $(this).jqGrid('getCol', 'RoadLength', false, 'sum');
            RoadLengthT = parseFloat(RoadLengthT).toFixed(3);
            var MAST_HAB_TOT_POPT = $(this).jqGrid('getCol', 'MAST_HAB_TOT_POP', false, 'sum');

            $(this).jqGrid('footerData', 'set', { PlannedRoadNumber: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { ROAD_LENGTH: ROAD_LENGTHT });
            $(this).jqGrid('footerData', 'set', { MAST_HAB_TOT_POP: MAST_HAB_TOT_POPT });
            $('#ERR2FinalReportTable_rn').html('Sr.<br/>No.');

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