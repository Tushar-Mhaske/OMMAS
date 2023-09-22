$(document).ready(function () {
    //this should be added on document ready for client side validation
    $.validator.unobtrusive.parse($('#frmIAPReport'));
      // var Type = $("#Type").val();
    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#ddlStateH").val($("#MAST_STATE_CODE").val());
        $("#ddlStateH").attr("disabled", "disabled");
    }
    $('#btnGoH').click(function () {
        loadIAPHabitationReportGrid(); 
    });

  
    $('#btnGoH').trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});

function loadIAPHabitationReportGrid() {
   
    if ($('#frmIAPReport').valid()) {
        //alert("");
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $("#tbIAPReportH").jqGrid('GridUnload');
        jQuery("#tbIAPReportH").jqGrid({
            url: '/IAPReports/IAPDistrictHabitationReportList',
            datatype: "json",
            mtype: "POST",
            postData: { MONTH: $("#ddlMonthH option:selected").val(), YEAR: $("#ddlYearH option:selected").val(), StateCode: $("#ddlStateH option:selected").val() },
            colNames: ["State", "IAP District", "Eligible under PMGSY", "Cleared under PMGSY", "Connected under PMGSY", "Connected under other schemes"],
            colModel: [
                                { name: 'StateName', index: 'StateName', width: 100, sortable: false, align: "left", search: false },
                                { name: 'DistrictName', index: 'DistrictName', width: 70, sortable: false, align: "center", search: false, summaryType: 'count', summaryTpl: 'Total' },
                                { name: 'TotalEHabs', index: 'TotalEHabs', width: 70, sortable: false, align: "right", search: false, formatter: 'integer', summaryType: 'sum', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                                { name: 'TotalCHABS', index: 'TotalCHABS', width: 70, sortable: false, align: "right", search: false, formatter: 'integer', summaryType: 'sum', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                                { name: 'TotalConHabsPMGSY', index: 'TotalConHabsPMGSY', width: 70, sortable: false, align: "right", search: false, formatter: 'integer', summaryType: 'sum', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                                { name: 'TotalConHabsOther', index: 'TotalConHabsOther', width: 70, sortable: false, align: "right", search: false, formatter: 'integer', summaryType: 'sum', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } }

            ],
            pager: jQuery('#dvIAPReportPagerH'),
            rowNum: 2147483647,
            viewrecords: true,
            recordtext: '{2} records found',
            caption: "&nbsp;&nbsp;IAP Districts Habitation Details",
            height: '550',
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

                var TotalEHabsT = $(this).jqGrid('getCol', 'TotalEHabs', false, 'sum');
                var TotalCHABST = $(this).jqGrid('getCol', 'TotalCHABS', false, 'sum');
                var TotalConHabsPMGSYT = $(this).jqGrid('getCol', 'TotalConHabsPMGSY', false, 'sum');
                var TotalConHabsOtherT = $(this).jqGrid('getCol', 'TotalConHabsOther', false, 'sum');

                ////

                $(this).jqGrid('footerData', 'set', { DistrictName: 'Totals' });
                $(this).jqGrid('footerData', 'set', { TotalEHabs: TotalEHabsT }, true);
                $(this).jqGrid('footerData', 'set', { TotalCHABS: TotalCHABST }, true);
                $(this).jqGrid('footerData', 'set', { TotalConHabsPMGSY: TotalConHabsPMGSYT }, true);
                $(this).jqGrid('footerData', 'set', { TotalConHabsOther: TotalConHabsOtherT }, true);
                $('#tbIAPReportH_rn').html('Sr.<br/>No.');

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
        $("#tbIAPReportH").jqGrid('setGroupHeaders', {
            useColSpanStyle: true,
            groupHeaders: [{ startColumnName: 'TotalEHabs', numberOfColumns: 4, titleText: 'Number of Habitations' }]
        });
    }


}

