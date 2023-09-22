$(document).ready(function () {
    if ($("#MAST_STATE_CODE").val() > 0) {
        $("#ddState_PropScruitinyDetails").attr("disabled","disabled");
    }
    $("#ddState_PropScruitinyDetails").focus();

    $('#btnGoScruitiny').click(function () {
        var stateCode = $("#ddState_PropScruitinyDetails option:selected").val();
        var type = $("#ddStaPta_PropScruitinyDetails option:selected").val();
        var agency = $("#ddAgency_PropScruitinyDetails option:selected").val();
        var year = $("#ddYear_PropScruitinyDetails option:selected").val();
        var batch = $("#ddBatch_PropScruitinyDetails option:selected").val();
        var scheme = $("#ddScheme_PropScruitinyDetails option:selected").val();
       
            loadPropScrutinyReportGrid(stateCode, type, agency, year, batch, scheme);
       
    });

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
    $('#btnGoScruitiny').trigger('click');

    $("#ddState_PropScruitinyDetails").change(function () {

        loadAgencyList();
    });
    $("#ddStaPta_PropScruitinyDetails").change(function () {

        loadAgencyList();
    });
    $("#ddStaPta_PropScruitinyDetails").trigger('change');
});

function loadPropScrutinyReportGrid(stateCode,type, agency, year, batch, scheme) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbPropScrutinyReport").jqGrid('GridUnload');
    $("#tbPropScrutinyTASDReport").jqGrid('GridUnload');

    jQuery("#tbPropScrutinyReport").jqGrid({
        url: '/ProposalReports/PropScrutinyReportListing',
        datatype: "json",
        mtype: "POST",
        postData: { StateCode: stateCode,TYPE: type, AGENCY: agency, YEAR: year, BATCH: batch, SCHEME: scheme },
        colNames: ["State Name", "District Name", "Year", "Scheme", "Name", "Nos.", "TA_PROPOSALSHidden", "Total Cost", "Nos.", "MRD_PROPOSALSHidden", "Total Cost"],
        colModel: [
            { name: "MAST_STATE_NAME", width: 200, align: 'left',  height: 'auto', sortable: true },
            { name: "MAST_DISTRICT_NAME", width: 200, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_YEAR", width: 100, align: 'left',  height: 'auto', sortable: false },
            { name: "MAST_FUNDING_AGENCY_NAME", width: 200, align: 'left',  height: 'auto', sortable: false },
            { name: "TA_NAME", width: 250, align: 'left',  height: 'auto', sortable: false },
            { name: "TA_PROPOSALS", width: 120, align: 'center',  height: 'auto', sortable: false },
            { name: "TA_PROPOSALSHidden", width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', hidden: true, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TotalTAAmount", width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "MRD_PROPOSALS", width: 120, align: 'center',  height: 'auto', sortable: false },
            { name: "MRD_PROPOSALSHidden", width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', hidden: true, formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "TotalMRDAmount", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        pager: jQuery('#dvPropScrutinyReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Scrutinized Proposals Details",
        height: 520,
        sortname: 'MAST_STATE_NAME',
        rownumbers: true,
        autowidth: false,
        width: 1100,
        footerrow: true,
        sortname: 'MAST_STATE_NAME',
        loadComplete: function () {
            //Total of Columns
            var TA_PROPOSALSHiddenT = $(this).jqGrid('getCol', 'TA_PROPOSALSHidden', false, 'sum');
            var MRD_PROPOSALSHiddenT = $(this).jqGrid('getCol', 'MRD_PROPOSALSHidden', false, 'sum');

            var TotalTAAmountT = $(this).jqGrid('getCol', 'TotalTAAmount', false, 'sum');
            TotalTAAmountT = parseFloat(TotalTAAmountT).toFixed(2);
            var TotalMRDAmountT = $(this).jqGrid('getCol', 'TotalMRDAmount', false, 'sum');
            TotalMRDAmountT = parseFloat(TotalMRDAmountT).toFixed(2);
               ////

            $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TotalTAAmount: TotalTAAmountT }, true);
            $(this).jqGrid('footerData', 'set', { TotalMRDAmount: TotalMRDAmountT }, true);
            $(this).jqGrid('footerData', 'set', { TA_PROPOSALS: TA_PROPOSALSHiddenT }, true);
            $(this).jqGrid('footerData', 'set', { MRD_PROPOSALS: MRD_PROPOSALSHiddenT }, true);
          //  $('#dvPropScrutinyReportPager_left').html('<b>Amount:Rs in Lacs</b>');
            $("#dvPropScrutinyReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>[Click on Road Cost / Bridge Cost Column values to view proposal details]</font>");
            $('#tbPropScrutinyReport_rn').html('Sr.<br/>No.');

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

    $("#tbPropScrutinyReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'TA_PROPOSALS', numberOfColumns: 3, titleText: '<em>Proposals Online Scrutinized by STA </em>' },
          { startColumnName: 'MRD_PROPOSALS', numberOfColumns: 3, titleText: '<em>Proposals Online Sanctioned by MoRD </em>' }

        ]

    });

}
function loadPropScrutinyTASDReportGrid(type, stateCode, districtCode, year, batch, scheme, TACode,taName, proposal) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbPropScrutinyReport").jqGrid('setGridState', 'hidden');   
    $("#tbPropScrutinyReport").jqGrid('setSelection', stateCode);
    $("#tbPropScrutinyTASDReport").jqGrid('GridUnload');
    jQuery("#tbPropScrutinyTASDReport").jqGrid({
        url: '/ProposalReports/PropScrutinyTASDReportListing',
        datatype: "json",
        mtype: "POST",
        postData: { StateCode: stateCode, DistrictCode: districtCode, TYPE: type, AGENCY: TACode, YEAR: year, BATCH: batch, SCHEME: scheme, TAName: taName },
        colNames: ["State Name","Name", "District Name", "Block Name", "Package", "Road Name", "Sanctioned Year", "Category of Road (N/U)", "Length", "Sanctioned Cost",
                    "Work", "Sanctioned Cost", "Other Sanctioned Cost", "Protection Work Cost ", "Total Cost"],
        colModel: [
            { name: "MAST_STATE_NAME", width: 200, align: 'left',  height: 'auto', sortable: false, hidden: true },
            { name: "MAST_FUNDING_AGENCY_NAME", width: 200, align: 'left',  height: 'auto', sortable: false, hidden: true },
            { name: "MAST_DISTRICT_NAME", width: 120, align: 'left',  height: 'auto', sortable: true },
            { name: "MAST_BLOCK_NAME", width: 120, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_PACKAGE_ID", width: 120, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_ROAD_NAME", width: 200, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_YEAR", width: 100, align: 'left',  height: 'auto', sortable: false },
            { name: "IMS_UPGRADE_CONNECT", width: 120, align: 'center',  height: 'auto', sortable: false },
            { name: "PROP_LEN", width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "IMS_SANCTIONED_PAV_AMT", width: 150, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "IMS_NO_OF_CDWORKS", width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "IMS_SANCTIONED_CD_AMT", width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "IMS_SANCTIONED_OW_AMT", width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "IMS_SANCTIONED_PW_AMT", width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: "Total_AMT", width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } }
        ],
        pager: jQuery('#dvPropScrutinyTASDReportPager'),
        rowNum: 2147483647,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Road Details",
        height: 450,
        rownumbers: true,
        autowidth: false,
        width: 1100,
        footerrow: true,
        sortname: 'MAST_STATE_NAME',

        loadComplete: function () {
            //Total of Columns
            var IMS_NO_OF_CDWORKST = $(this).jqGrid('getCol', 'IMS_NO_OF_CDWORKS', false, 'sum');
            var PROP_LENT = $(this).jqGrid('getCol', 'PROP_LEN', false, 'sum');
            PROP_LENT = parseFloat(PROP_LENT).toFixed(3);
            var IMS_SANCTIONED_PAV_AMTT = $(this).jqGrid('getCol', 'IMS_SANCTIONED_PAV_AMT', false, 'sum');
            IMS_SANCTIONED_PAV_AMTT = parseFloat(IMS_SANCTIONED_PAV_AMTT).toFixed(2);
            var IMS_SANCTIONED_PW_AMTT = $(this).jqGrid('getCol', 'IMS_SANCTIONED_PW_AMT', false, 'sum');
            IMS_SANCTIONED_PW_AMTT = parseFloat(IMS_SANCTIONED_PW_AMTT).toFixed(2);
            var IMS_SANCTIONED_OW_AMTT = $(this).jqGrid('getCol', 'IMS_SANCTIONED_OW_AMT', false, 'sum');
            IMS_SANCTIONED_OW_AMTT = parseFloat(IMS_SANCTIONED_OW_AMTT).toFixed(2);
            var IMS_SANCTIONED_CD_AMTT = $(this).jqGrid('getCol', 'IMS_SANCTIONED_CD_AMT', false, 'sum');
            IMS_SANCTIONED_CD_AMTT = parseFloat(IMS_SANCTIONED_CD_AMTT).toFixed(2);
            var Total_AMTT = $(this).jqGrid('getCol', 'Total_AMT', false, 'sum');
            Total_AMTT = parseFloat(Total_AMTT).toFixed(2);
            ////

            $(this).jqGrid('footerData', 'set', { MAST_DISTRICT_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { IMS_NO_OF_CDWORKS: IMS_NO_OF_CDWORKST }, true);
            $(this).jqGrid('footerData', 'set', { PROP_LEN: PROP_LENT }, true);
            $(this).jqGrid('footerData', 'set', { IMS_SANCTIONED_PAV_AMT: IMS_SANCTIONED_PAV_AMTT }, true);
            $(this).jqGrid('footerData', 'set', { IMS_SANCTIONED_PW_AMT: IMS_SANCTIONED_PW_AMTT }, true);
            $(this).jqGrid('footerData', 'set', { IMS_SANCTIONED_OW_AMT: IMS_SANCTIONED_OW_AMTT }, true);
            $(this).jqGrid('footerData', 'set', { IMS_SANCTIONED_CD_AMT: IMS_SANCTIONED_CD_AMTT }, true);
            $(this).jqGrid('footerData', 'set', { Total_AMT: Total_AMTT }, true);
            $("#dvPropScrutinyTASDReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Cost Rs in Lacs & Length in Kms.</font>");
            $('#tbPropScrutinyTASDReport_rn').html('Sr.<br/>No.');

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

    $("#tbPropScrutinyTASDReport").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'PROP_LEN', numberOfColumns: 2, titleText: '<em>Pavement </em>' },
          { startColumnName: 'IMS_NO_OF_CDWORKS', numberOfColumns: 2, titleText: '<em>CD Work Details </em>' },
        


        ]

    });

}
function loadAgencyList() {
    $("#ddAgency_PropScruitinyDetails").val(0);
    $("#ddAgency_PropScruitinyDetails").empty();
    $.ajax({
        url: '/ProposalReports/GetTechAgencyName_ByAgencyType?type=' + $("#ddStaPta_PropScruitinyDetails").val() + '&' + 'stateCode=' + $("#ddState_PropScruitinyDetails").val(),
        type: 'POST',
        // data: { "Type": $("#ddStaPta_PropScruitinyDetails").val() },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
                $("#ddAgency_PropScruitinyDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });

}
