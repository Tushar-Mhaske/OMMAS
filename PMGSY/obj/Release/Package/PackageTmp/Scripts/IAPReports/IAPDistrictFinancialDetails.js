$(document).ready(function () {
    //this should be added on document ready for client side validation
    $.validator.unobtrusive.parse($('#frmIAPReportF'));
    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#ddlStateF").val($("#MAST_STATE_CODE").val());
        $("#ddlStateF").attr("disabled", "disabled");
    }

    $('#btnGoF').click(function () {
        loadIAPFinancialReportGrid();
    });
    $('#btnGoF').trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});
function loadIAPFinancialReportGrid() {
    var year = $("#ddlYearF option:selected").text();
    var  month= $("#ddlMonthF option:selected").text();

    if ($('#frmIAPReportF').valid()) {
    
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $("#tbIAPReportF").jqGrid('GridUnload');
        jQuery("#tbIAPReportF").jqGrid({
            url: '/IAPReports/IAPDistrictFinancialProgressList?' + Math.random(),
            datatype: "json",
            mtype: "POST",
            postData: { MONTH: $("#ddlMonthF option:selected").val(), YEAR: $("#ddlYearF option:selected").val(), StateCode: $("#ddlStateF option:selected").val() },
            colNames: ["State", "District", "New Connectivity (Rs in Lakhs)", "Upgradation (Rs in Lakhs)", "Total (Rs in Lakhs)", "New Connectivity (Rs in Lakhs)", "Upgradation (Rs in Lakhs)", "Total (Rs in Lakhs)", "% Expenditure"],
            colModel: [
                                { name: 'StateName', index: 'StateName', width: 100, sortable: false, align: "left", search: false },
                                { name: 'DistrictName', index: 'DistrictName', width: 70, sortable: false, align: "center", search: false, summaryType: 'count', summaryTpl: 'Total' },
                                { name: 'TotalSNew', index: 'TotalSNew', width: 70, sortable: false, align: "right", search: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                                { name: 'TotalSUpgrade', index: 'TotalSUpgrade', width: 70, sortable: false, align: "right", search: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                                { name: 'TotalSanction', index: 'TotalSanction', width: 70, sortable: false, align: "right", search: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                                { name: 'TotalENew', index: 'TotalENew', width: 70, sortable: false, align: "right", search: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                                { name: 'TotalEUpgrade', index: 'TotalEUpgrade', width: 70, sortable: false, align: "right", search: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                                { name: 'TotalExp', index: 'TotalExp', width: 70, sortable: false, align: "right", search: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                                { name: 'TotalPerc', index: 'TotalPerc', width: 70, sortable: false, align: "right", search: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } }

            ],
            pager: jQuery('#dvIAPReportPagerF'),
            rowNum: 2147483647,
            viewrecords: true,
            recordtext: '{2} records found',
            caption: "&nbsp;&nbsp;Financial Progress for the IAP Districts Details",
            height: 550,
            autowidth: true,
            cmTemplate: { sortable: false },
            footerrow: true,
            grouping: true,
            rownumbers: true,
            groupingView: {
                groupField: ['StateName'],
                groupSummary: [true],
                showSummaryOnHide: true,
                groupColumnShow: [false],
                groupDataSorted: true,
                groupCollapse: true,
                groupText: ['<b>{0}</b>']
            },


            loadComplete: function () {
                //Total of Columns
                var TotalSNewT = $(this).jqGrid('getCol', 'TotalSNew', false, 'sum');
                TotalSNewT = parseFloat(TotalSNewT).toFixed(2);
                var TotalSUpgradeT = $(this).jqGrid('getCol', 'TotalSUpgrade', false, 'sum');
                TotalSUpgradeT = parseFloat(TotalSUpgradeT).toFixed(2);
                var TotalSanctionT = $(this).jqGrid('getCol', 'TotalSanction', false, 'sum');
                TotalSanctionT = parseFloat(TotalSanctionT).toFixed(2);
                var TotalENewT = $(this).jqGrid('getCol', 'TotalENew', false, 'sum');
                TotalENewT = parseFloat(TotalENewT).toFixed(2);
                var TotalEUpgradeT = $(this).jqGrid('getCol', 'TotalEUpgrade', false, 'sum');
                TotalEUpgradeT = parseFloat(TotalEUpgradeT).toFixed(2);
                var TotalExpT = $(this).jqGrid('getCol', 'TotalExp', false, 'sum');
                TotalExpT = parseFloat(TotalExpT).toFixed(2);
                var TotalPercT = $(this).jqGrid('getCol', 'TotalPerc', false, 'sum');
                TotalPercT = parseFloat(TotalPercT).toFixed(2);

                //var TotalPercT = 0;
                //if (TotalSanctionT > 0) {
                //    TotalPercT=(TotalExpT/TotalSanctionT)*100
                //}
               
                //TotalPercT = parseFloat(TotalPercT).toFixed(2);

                ////

                $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Totals</b>' });
                $(this).jqGrid('footerData', 'set', { TotalSNew: TotalSNewT }, true);
                $(this).jqGrid('footerData', 'set', { TotalSUpgrade: TotalSUpgradeT }, true);
                $(this).jqGrid('footerData', 'set', { TotalSanction: TotalSanctionT }, true);
                $(this).jqGrid('footerData', 'set', { TotalENew: TotalENewT }, true);
                $(this).jqGrid('footerData', 'set', { TotalEUpgrade: TotalEUpgradeT }, true);
                $(this).jqGrid('footerData', 'set', { TotalExp: TotalExpT }, true);
                $(this).jqGrid('footerData', 'set', { TotalPerc: TotalPercT }, true);
                $('#tbIAPReportF_rn').html('Sr.<br/>No.');

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


        $("#tbIAPReportF").jqGrid('setGroupHeaders', {
            useColSpanStyle: false,
            groupHeaders: [
              { startColumnName: 'TotalSNew', numberOfColumns: 3, titleText: '<em>Value of Projects Sanctioned </em>' },
              //{ startColumnName: 'TotalENew', numberOfColumns: 3, titleText: '<em>Expenditure Incurred Upto Jan, 2002 </em>' }
              { startColumnName: 'TotalENew', numberOfColumns: 3, titleText: '<em>Expenditure Incurred Upto '+month+ ' , '+year+ ' </em>' }


            ]

        });
    }
}


