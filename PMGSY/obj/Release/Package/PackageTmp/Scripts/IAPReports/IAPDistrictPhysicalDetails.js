$(document).ready(function () {
    //this should be added on document ready for client side validation
    $.validator.unobtrusive.parse($('#frmIAPReportP'));
    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#ddlStateP").val($("#MAST_STATE_CODE").val());
        $("#ddlStateP").attr("disabled", "disabled");
    }
    $('#btnGoP').click(function () {
        loadIAPPhysicalReportGrid();
    });
    $('#btnGoP').trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});

function loadIAPPhysicalReportGrid() {

    if ($('#frmIAPReportP').valid()) {
        //alert("1");
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $("#tbIAPReportP").jqGrid('GridUnload');
        jQuery("#tbIAPReportP").jqGrid({
            url: '/IAPReports/IAPDistrictPhysicalProgressList?' + Math.random(),
            datatype: "json",
            mtype: "POST",
            postData: { MONTH: $("#ddlMonthP option:selected").val(), YEAR: $("#ddlYearP option:selected").val(), StateCode: $("#ddlStateP option:selected").val() },
            colNames: ["State", "District", "Roads", "Length", "Roads", "Length", "Roads", "Length", "Roads", "Length", "Roads", "Length", "Roads", "Length"],
            colModel: [

                                { name: 'StateName', index: 'StateName', width: 100, sortable: false, align: "left", search: false },
                                { name: 'DistrictName', index: 'DistrictName', width: 70, sortable: false, align: "center", search: false, summaryType: 'count', summaryTpl: 'Total' },
                                { name: 'TotalSNew', index: 'TotalSNew', width: 70, sortable: false, align: "right", search: false, formatter: 'integer', summaryType: 'sum', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                                { name: 'TotalSNewLen', index: 'TotalSNewLen', width: 70, sortable: false, align: "right", search: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                                { name: 'TotalSUpagrade', index: 'TotalSUpagrade', width: 70, sortable: false, align: "right", search: false, formatter: 'integer', summaryType: 'sum', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                                { name: 'TotalSUplen', index: 'TotalSUplen', width: 70, sortable: false, align: "right", search: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                                { name: 'TotalSanction', index: 'TotalSanction', width: 70, sortable: false, align: "right", search: false, formatter: 'integer', summaryType: 'sum', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                                { name: 'TotalSLen', index: 'TotalSLen', width: 70, sortable: false, align: "right", search: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                                { name: 'TotalCNew', index: 'TotalCNew', width: 70, sortable: false, align: "right", search: false, formatter: 'integer', summaryType: 'sum', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                                { name: 'TotalCNLen', index: 'TotalCNLen', width: 70, sortable: false, align: "right", search: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                                { name: 'TotalCUpagrad', index: 'TotalCUpagrad', width: 70, sortable: false, align: "right", search: false, formatter: 'integer', summaryType: 'sum', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                                { name: 'TotalCUpLen', index: 'TotalCUpLen', width: 70, sortable: false, align: "right", search: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
                                { name: 'TotalCSanction', index: 'TotalCSanction', width: 70, sortable: false, align: "right", search: false, formatter: 'integer', summaryType: 'sum', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                                { name: 'TotalCLen', index: 'TotalCLen', width: 70, sortable: false, align: "right", search: false, formatter: 'number', summaryType: 'sum', formatoptions: { decimalSeparator: ".",  decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } }

            ],
            pager: jQuery('#dvIAPReportPagerP'),
            rowNum: 2147483647,
            viewrecords: true,
            recordtext: '{2} records found',
            caption: "&nbsp;&nbsp;IAP Districts Physical Progress Details",
            height:520,
            autowidth: true,
            cmTemplate: { sortable: false },
            footerrow: true,
            rownumbers: true,
            grouping: true,
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
                var TotalSNewLenT = $(this).jqGrid('getCol', 'TotalSNewLen', false, 'sum');
                TotalSNewLenT = parseFloat(TotalSNewLenT).toFixed(3);
                var TotalSUpagradeT = $(this).jqGrid('getCol', 'TotalSUpagrade', false, 'sum');
                var TotalSUplenT = $(this).jqGrid('getCol', 'TotalSUplen', false, 'sum');
                TotalSUplenT = parseFloat(TotalSUplenT).toFixed(3);
                var TotalSanctionT = $(this).jqGrid('getCol', 'TotalSanction', false, 'sum');
                var TotalSLenT = $(this).jqGrid('getCol', 'TotalSLen', false, 'sum');
                TotalSLenT = parseFloat(TotalSLenT).toFixed(3);
                var TotalCNewT = $(this).jqGrid('getCol', 'TotalCNew', false, 'sum');
                var TotalCNLenT = $(this).jqGrid('getCol', 'TotalCNLen', false, 'sum');
                TotalCNLenT = parseFloat(TotalCNLenT).toFixed(3);
                var TotalCUpagradT = $(this).jqGrid('getCol', 'TotalCUpagrad', false, 'sum');
                var TotalCUpLenT = $(this).jqGrid('getCol', 'TotalCUpLen', false, 'sum');
                TotalCUpLenT = parseFloat(TotalCUpLenT).toFixed(3);
                var TotalCSanctionT = $(this).jqGrid('getCol', 'TotalCSanction', false, 'sum');
                var TotalCLenT = $(this).jqGrid('getCol', 'TotalCLen', false, 'sum');
                TotalCLenT = parseFloat(TotalCLenT).toFixed(3);

                ////

                $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Totals</b>' });
                $(this).jqGrid('footerData', 'set', { TotalSNew: TotalSNewT }, true);
                $(this).jqGrid('footerData', 'set', { TotalSNewLen: TotalSNewLenT }, true);
                $(this).jqGrid('footerData', 'set', { TotalSUpagrade: TotalSUpagradeT }, true);
                $(this).jqGrid('footerData', 'set', { TotalSUplen: TotalSUplenT }, true);
                $(this).jqGrid('footerData', 'set', { TotalSanction: TotalSanctionT }, true);
                $(this).jqGrid('footerData', 'set', { TotalSLen: TotalSLenT }, true);
                $(this).jqGrid('footerData', 'set', { TotalCNew: TotalCNewT }, true);
                $(this).jqGrid('footerData', 'set', { TotalCNLen: TotalCNLenT }, true);
                $(this).jqGrid('footerData', 'set', { TotalCUpagrad: TotalCUpagradT }, true);
                $(this).jqGrid('footerData', 'set', { TotalCUpLen: TotalCUpLenT }, true);
                $(this).jqGrid('footerData', 'set', { TotalCSanction: TotalCSanctionT }, true);
                $(this).jqGrid('footerData', 'set', { TotalCLen: TotalCLenT }, true);
                $('#tbIAPReportP_rn').html('Sr.<br/>No.');
                $("#dvIAPReportPagerP_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms. </font>");

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



        $("#tbIAPReportP").jqGrid('setGroupHeaders', {
            useColSpanStyle: false,
            groupHeaders: [
              //{ startColumnName: 'TRCNNumber', numberOfColumns: 2, titleText: '<em>Core Network </em>' },
              //{ startColumnName: 'TRPNumber', numberOfColumns: 2, titleText: '<em>Proposals </em>' }
            {
                startColumnName: 'TotalSNew', numberOfColumns: 6,
                titleText: '<table style="width:100%;border-spacing:0px"' +
                        '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="6">Sanctioned </td>  </tr>' +
                        '<tr>' +
                            '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">New Connectivity</td>' +
                              '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Upgradation</td>' +
                              '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Total</td>' +
                        '</tr>' +
                        '</table>'
            },

            {
                startColumnName: 'TotalCNew', numberOfColumns: 6,
                titleText: '<table style="width:100%;border-spacing:0px"' +
                        '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="6">Completed </td>  </tr>' +
                        '<tr>' +
                            '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">New Connectivity</td>' +
                              '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">Upgradation</td>' +
                              '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Total</td>' +
                        '</tr>' +
                        '</table>'
            }

            ]

        });

    }


}

