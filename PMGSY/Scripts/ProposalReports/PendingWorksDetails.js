$(document).ready(function () {
    $("#ddState_PendingWorksDetails").focus();
    if ($("#MAST_STATE_CODE").val() > 0) {
        $("#ddState_PendingWorksDetails").attr("disabled", "disabled");
    }
    $('#btnGoPendingWorks').click(function () {
        var stateCode = $("#ddState_PendingWorksDetails option:selected").val();
        var reason = $("#ddReason_PendingWorksDetails option:selected").val();
        loadPendingWorksDetails(stateCode, reason);
    });


    $('#btnGoPendingWorks').trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});

function loadPendingWorksDetails(stateCode, reason) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbPendingWorksReport").jqGrid('GridUnload');
    jQuery("#tbPendingWorksReport").jqGrid({
        url: '/ProposalReports/PendingWorksReportListing',
        datatype: "json",
        mtype: "POST",
        colNames: ["State Name", "District Name", "Block Name", "Package", "Phase", "Road Name", "Road Length (Kms.)", "Sanctioned Cost (Rs. in Laks) ", "Connectivity Type", "Reason"],
        colModel: [
            { name: "MAST_STATE_NAME", width: 200, align: 'left',  height: 'auto', sortable: false },
            { name: "MAST_DISTRICT_NAME", width: 200, align: 'left',  height: 'auto', sortable: false },
            { name: "MAST_BLOCK_NAME", width: 200, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_PACKAGE_ID", width: 150, align: 'left',  height: 'auto', sortable: false },
            { name: "PHASE", width: 100, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_ROAD_NAME", width: 250, align: 'left',  height: 'auto', sortable: false, summaryType: 'count', summaryTpl: '<b>{0} Total Works</b>' },
            { name: "IMS_PAV_LENGTH", width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number',summaryType:'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "PAV_COST", width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number',summaryType:'sum', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "CON_TYPE", width: 200, align: 'left',  height: 'auto', sortable: false },
            { name: "PENDING_REASON", width: 200, align: 'left',  height: 'auto', sortable: false }
        ],
        postData: { "StateCode": stateCode, "Reason": reason },
        pager: jQuery('#dvPendingWorksReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Pending Works due to issues related to Land Acquisition, Legal Cases and Forest Clearance Details",
        height: 520,
        autowidth: true,      
        rownumbers: true,
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
            var IMS_PAV_LENGTHT = $(this).jqGrid('getCol', 'IMS_PAV_LENGTH', false, 'sum');
            IMS_PAV_LENGTHT = parseFloat(IMS_PAV_LENGTHT).toFixed(3);
            var PAV_COSTT = $(this).jqGrid('getCol', 'PAV_COST', false, 'sum');
            PAV_COSTT = parseFloat(PAV_COSTT).toFixed(2);


            ////

            $(this).jqGrid('footerData', 'set', { MAST_DISTRICT_NAME: '<b>Totals</b>' });
            $(this).jqGrid('footerData', 'set', { IMS_PAV_LENGTH: IMS_PAV_LENGTHT }, true);
            $(this).jqGrid('footerData', 'set', { PAV_COST: PAV_COSTT }, true);
            $('#tbPendingWorksReport_rn').html('Sr.<br/>No.');

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



}