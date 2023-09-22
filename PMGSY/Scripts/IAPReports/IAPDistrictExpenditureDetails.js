$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmIAPReportE'));
    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#ddlStateE").val($("#MAST_STATE_CODE").val());
        $("#ddlStateE").attr("disabled", "disabled");
    }
    $('#btnGoE').click(function () {
        loadIAPExpenditureReportGrid();
    });

    $('#btnGoE').trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});

function loadIAPExpenditureReportGrid() {
    var year = $("#ddlYearE option:selected").text();

    if ($('#frmIAPReportE').valid()) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $("#tbIAPReportE").jqGrid('GridUnload');
        jQuery("#tbIAPReportE").jqGrid({
            url: '/IAPReports/IAPDistrictExpenditureList?' + Math.random(),
            datatype: "json",
            mtype: "POST",
            postData: { YEAR: $("#ddlYearE option:selected").val(), StateCode: $("#ddlStateE option:selected").val() },
            colNames: ["State", "District", "New Connectivity", "Upgradation", "Total"],
            colModel: [

                                { name: 'StateName', index: 'StateName', width: 100, sortable: false, align: "left", search: false },
                                { name: 'DistrictName', index: 'DistrictName', width: 70, sortable: false, align: "center", search: false, summaryType: 'count', summaryTpl: 'Total' },
                                { name: 'TotalENew', index: 'TotalENew', width: 70, sortable: false, align: "right", search: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                                { name: 'TotalEUpgrade', index: 'TotalEUpgrade', width: 70, sortable: false, align: "right", search: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
                                { name: 'TotalExp', index: 'TotalExp', width: 70, sortable: false, align: "right", search: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } }

            ],
            pager: jQuery('#dvIAPReportPagerE'),
            rowNum: 2147483647,
            viewrecords: true,
            recordtext: '{2} records found',
            caption: "&nbsp;&nbsp;Expenditure for the IAP Districts  Details",
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
                var TotalENewT = $(this).jqGrid('getCol', 'TotalENew', false, 'sum');
                TotalENewT = parseFloat(TotalENewT).toFixed(2);
                var TotalEUpgradeT = $(this).jqGrid('getCol', 'TotalEUpgrade', false, 'sum');
                TotalEUpgradeT = parseFloat(TotalEUpgradeT).toFixed(2);
                var TotalExpT = $(this).jqGrid('getCol', 'TotalExp', false, 'sum');
                TotalExpT = parseFloat(TotalExpT).toFixed(2);

                ////

                $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Totals</b>' });
                $(this).jqGrid('footerData', 'set', { TotalENew: TotalENewT }, true);
                $(this).jqGrid('footerData', 'set', { TotalEUpgrade: TotalEUpgradeT }, true);
                $(this).jqGrid('footerData', 'set', { TotalExp: TotalExpT }, true);

                $('#tbIAPReportE_rn').html('Sr.<br/>No.');

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

        $("#tbIAPReportE").jqGrid('setGroupHeaders', {
            useColSpanStyle: false,
            groupHeaders: [
              //{ startColumnName: 'TotalENew', numberOfColumns: 3, titleText: '<em>Expenditure incurred during the 2001-2002 </em>' }
              { startColumnName: 'TotalENew', numberOfColumns: 3, titleText: '<em>Expenditure incurred during the year '+year+' </em>' }

            ]

        });
    }
}
