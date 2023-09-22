$(document).ready(function () {
    loadPropAssetValueDetails();
    //$('#tblRptContents').bind('resize', function () {
    //    resizeJqGrid();
    //}).trigger('resize');
});

function loadPropAssetValueDetails() {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbPropAssetValueReport").jqGrid('GridUnload');
    jQuery("#tbPropAssetValueReport").jqGrid({
        url: '/ProposalReports/PropAssetValueReportListing',
        datatype: "json",
        mtype: "POST",
        //  colNames: ["State Name", "Year", "Value (Rs. in Crores)", "Length (in Kms)", "Average", "Value (Rs. in Crores)", "Length (in Kms)",
        //         "Average NC cost of last 3 year", "Asset Val (NC)=NC length * Average cost" 
        //"Asset Val (UG)=UG length *Average cost", "Current Replacement Value", "Maintenance fund required (2% of asset Value)",
        //"Maintenance fund recieved", "Average Maintenance fund during last 3 years", "% Maintenance fund recieved to fund required (Col14/Col12 * 100)"],
        colNames: ["State Name", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14","15"],

        colModel: [
            { name: "MAST_STATE_NAME", width: 150, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_YEAR", width: 100, align: 'left',  height: 'auto', sortable: false, summaryType: 'count', summaryTpl: '<b>Total </b>' },
            { name: "NC_Value", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "" } },
            { name: "NC_Length", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "" } },
            { name: "NC_Avg", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "" } },
            { name: "UP_Value", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "UP_Length", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "UP_Avg", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "NC_Avg_3Years", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "NC_Asset_Value", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "UG_Asset_Value", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "Total_Assest_Value", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "Maint_Fund_Req", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "Maint_Fund_Recv", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "Maint_Fund_Avg_3Years", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "Per_Maint_Fund_Recv", width: 100, align: 'right',  height: 'auto', sortable: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
         
        ],      
        pager: jQuery('#dvPropAssetValueReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Asset Value of PMGSY",
        height: 540,
        rownumbers: true,
        autowidth: false,
        width: 1120,
        shrinkToFit: false,
        footerrow: true,
        grouping: true,
        groupingView: {
            groupField: ['MAST_STATE_NAME'],
            groupSummary: [true],
            groupColumnShow: [false],
            groupDataSorted: true,
            showSummaryOnHide: true,
            //  groupText: ['<b>{0} - {1} Total Works</b>'],
            groupCollapse: true,
        },
        loadComplete: function () {
            //Total of Columns
            var NC_ValueT = $(this).jqGrid('getCol', 'NC_Value', false, 'sum');
            NC_ValueT = parseFloat(NC_ValueT).toFixed(2);
            var NC_LengthT = $(this).jqGrid('getCol', 'NC_Length', false, 'sum');
            NC_LengthT = parseFloat(NC_LengthT).toFixed(3);
            var NC_AvgT = $(this).jqGrid('getCol', 'NC_Avg', false, 'sum');
            NC_AvgT = parseFloat(NC_AvgT).toFixed(2);
            var UP_ValueT = $(this).jqGrid('getCol', 'UP_Value', false, 'sum');
            UP_ValueT = parseFloat(UP_ValueT).toFixed(2);
            var UP_LengthT = $(this).jqGrid('getCol', 'UP_Length', false, 'sum');
            UP_LengthT = parseFloat(UP_LengthT).toFixed(3);
            var UP_AvgT = $(this).jqGrid('getCol', 'UP_Avg', false, 'sum');
            UP_AvgT = parseFloat(UP_AvgT).toFixed(2);
            var NC_Avg_3YearsT = $(this).jqGrid('getCol', 'NC_Avg_3Years', false, 'sum');
            NC_Avg_3YearsT = parseFloat(NC_Avg_3YearsT).toFixed(2);
            var NC_Asset_ValueT = $(this).jqGrid('getCol', 'NC_Asset_Value', false, 'sum');
            NC_Asset_ValueT = parseFloat(NC_Asset_ValueT).toFixed(2);
            var UG_Asset_ValueT = $(this).jqGrid('getCol', 'UG_Asset_Value', false, 'sum');
            UG_Asset_ValueT = parseFloat(UG_Asset_ValueT).toFixed(2);
            var Total_Assest_ValueT = $(this).jqGrid('getCol', 'Total_Assest_Value', false, 'sum');
            Total_Assest_ValueT = parseFloat(Total_Assest_ValueT).toFixed(2);
            var Maint_Fund_ReqT = $(this).jqGrid('getCol', 'Maint_Fund_Req', false, 'sum');
            Maint_Fund_ReqT = parseFloat(Maint_Fund_ReqT).toFixed(2);
            var Maint_Fund_RecvT = $(this).jqGrid('getCol', 'Maint_Fund_Recv', false, 'sum');
            Maint_Fund_RecvT = parseFloat(Maint_Fund_RecvT).toFixed(2);
            var Maint_Fund_Avg_3YearsT = $(this).jqGrid('getCol', 'Maint_Fund_Avg_3Years', false, 'sum');
            Maint_Fund_Avg_3YearsT = parseFloat(Maint_Fund_Avg_3YearsT).toFixed(2);
            var Per_Maint_Fund_RecvT = $(this).jqGrid('getCol', 'Per_Maint_Fund_Recv', false, 'sum');
            Per_Maint_Fund_RecvT = parseFloat(Per_Maint_Fund_RecvT).toFixed(2);
         

            ////

            $(this).jqGrid('footerData', 'set', { IMS_YEAR: '<b>Totals</b>' });
            $(this).jqGrid('footerData', 'set', { NC_Value: NC_ValueT }, true);
            $(this).jqGrid('footerData', 'set', { NC_Length: NC_LengthT }, true);
            $(this).jqGrid('footerData', 'set', { NC_Avg: NC_AvgT }, true);
            $(this).jqGrid('footerData', 'set', { UP_Value: UP_ValueT }, true);
            $(this).jqGrid('footerData', 'set', { UP_Length: UP_LengthT }, true);
            $(this).jqGrid('footerData', 'set', { UP_Avg: UP_AvgT }, true);
            $(this).jqGrid('footerData', 'set', { NC_Avg_3Years: NC_Avg_3YearsT }, true);
            $(this).jqGrid('footerData', 'set', { NC_Asset_Value: NC_Asset_ValueT }, true);
            $(this).jqGrid('footerData', 'set', { UG_Asset_Value: UG_Asset_ValueT }, true);
            $(this).jqGrid('footerData', 'set', { Total_Assest_Value: Total_Assest_ValueT }, true);
             $(this).jqGrid('footerData', 'set', { Maint_Fund_Req: Maint_Fund_ReqT }, true);
            $(this).jqGrid('footerData', 'set', { Maint_Fund_Recv: Maint_Fund_RecvT }, true);
            $(this).jqGrid('footerData', 'set', { Maint_Fund_Avg_3Years: Maint_Fund_Avg_3YearsT }, true);
            $(this).jqGrid('footerData', 'set', { Per_Maint_Fund_Recv: Per_Maint_Fund_RecvT }, true);
            $('#tbPropAssetValueReport_rn').html('Sr.<br/>No.');

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

    $("#tbPropAssetValueReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
           //{ startColumnName: 'BTGood', numberOfColumns: 4, titleText: '<em>BT</em>' },
           //{ startColumnName: 'WBMGood', numberOfColumns: 4, titleText: '<em>WBM </em>' },
           //{ startColumnName: 'GravelGood', numberOfColumns: 4, titleText: '<em>Gravel </em>' },
           //{ startColumnName: 'TrackGood', numberOfColumns: 4, titleText: '<em>Track</em>' },
           // { startColumnName: 'OtherGood', numberOfColumns: 4, titleText: '<em>Total </em>' }

          { startColumnName: 'IMS_YEAR', numberOfColumns: 1, titleText: '<em>Year </em>' },

          {
              startColumnName: 'NC_Value', numberOfColumns: 3,
              titleText: '<table style="width:100%;border-spacing:0px"' +
              '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="3">New Connectivity (NC) </td>  </tr>' +
              '<tr>' +
                  '<td id="h1"  style="width: 16.6%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Value (Rs. in Crores)</td>' +
                  '<td id="h1"  style="width: 16.6%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Length (in Kms)</td>' +
                  '<td id="h2"  style="width: 16.6%;  border-right-color: inherit;  padding: 4px 0px;">Average</td>' +
                  '</tr>' +
              '</table>'
          },
           {
               startColumnName: 'UP_Value', numberOfColumns: 3,
               titleText: '<table style="width:100%;border-spacing:0px"' +
               '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="3">Upgradation (UP) </td>  </tr>' +
               '<tr>' +
                   '<td id="h1"  style="width: 16.6%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Value (Rs. in Crores)</td>' +
                   '<td id="h1"  style="width: 16.6%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Length (in Kms)</td>' +
                   '<td id="h2"  style="width: 16.6%;  border-right-color: inherit;  padding: 4px 0px;">Average</td>' +
                   '</tr>' +
               '</table>'
           },
          { startColumnName: 'NC_Avg_3Years', numberOfColumns: 1, titleText: '<em>Average NC cost of last 3 year </em>' },
          { startColumnName: 'NC_Asset_Value', numberOfColumns: 1, titleText: '<em>Asset Val (NC)=NC length * Average cost </em>' },
          { startColumnName: 'UG_Asset_Value', numberOfColumns: 1, titleText: '<em>Asset Val (UG)=UG length *Average cost </em>' },
          { startColumnName: 'Total_Assest_Value', numberOfColumns: 1, titleText: '<em>Current Replacement Value </em>' },
          { startColumnName: 'Maint_Fund_Req', numberOfColumns: 1, titleText: '<em>Maintenance fund required (2% of asset Value) </em>' },
          { startColumnName: 'Maint_Fund_Recv', numberOfColumns: 1, titleText: '<em>Maintenance fund recieved</em>' },
          { startColumnName: 'Maint_Fund_Avg_3Years', numberOfColumns: 1, titleText: '<em>Average Maintenance fund during last 3 years </em>' },
          { startColumnName: 'Per_Maint_Fund_Recv', numberOfColumns: 1, titleText: '<em>% Maintenance fund recieved to fund required (Col14/Col12 * 100) </em>' },
     
        ]
    });

}