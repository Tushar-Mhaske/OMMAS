var hiddenScheme2Col = false;
$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmFilterProposal'));

    $(function () {
        $("#accordion").accordion({
            //fillSpace: true,
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $("#btnListProposal").click(function () {
        if ($("#ddlSchemes option:selected").val() == 1 || $("#ddlSchemes option:selected").val() == 3) {
            hiddenScheme2Col = true;
        }
        else {
            hiddenScheme2Col = false;
        }
        if ($("#frmFilterProposal").valid()) {
            LoadSanctionOrderList();
            LoadProposalForSanctionOrder();
        }
    });

    $('#ddlStates').change(function () {
        $("#ddlYears").empty();
        $.ajax({
            url: '/Proposal/PopulateFinancialYearsByState',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlStates").val() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Selected == true) {
                        $("#ddlYears").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlYears").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }

                $.unblockUI();
            },
            error: function (err) {
                //alert("error " + err);
                $.unblockUI();
            }
        });

        $.ajax({
            url: '/Proposal/PopulateAgencies',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlStates").val(), value: Math.random() },
            success: function (jsonData) {
                $("#ddlAgency").empty();
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Selected == true) {
                        $("#ddlAgency").append("<option value='" + jsonData[i].Value + "' selected=true>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlAgency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }

                $.unblockUI();
            },
            error: function (err) {
                //alert("error " + err);
                $.unblockUI();
            }
        });
    });

});

function LoadProposalForSanctionOrder() {
    //alert($("#ddlSchemes option:selected").val());
    $("#tblstProposal").jqGrid('GridUnload');

    jQuery("#tblstProposal").jqGrid({
        url: '/Proposal/GetProposalListByBatch',
        datatype: "json",
        mtype: "POST",
        postData: { StateCode: $("#ddlStates option:selected").val(), YearCode: $("#ddlYears option:selected").val(), StreamCode: $("#ddlStreams option:selected").val(), BatchCode: $("#ddlBatch option:selected").val(), Scheme: $("#ddlSchemes option:selected").val(), Type: $("#ddlProposalType option:selected").val(), Agency: $("#ddlAgency option:selected").val() },
        colNames: ['Block', 'District', 'Core Network No.', 'Name of Road / Bridge', 'Proposal_Type', 'Proposal Type', 'Category (Upgrade / New)', 'Road Length (in Kms) / Bridge Length (in Mtrs.)', 'Carriage Way Width', 'Stage Construction', 'No. of CD Works', 'MoRD Cost (in Lakhs)', 'State Cost (in Lakhs)', 'Total Cost (in Lakhs)', 'Maintenance Cost (in Lakhs)', 'Habs (1000+, 500+,250+, <250, Total)', 'A', 'B', 'C', 'D', 'E', 'Higher Specification Amount', 'Renewal Amount', 'Technology', 'Road Length', 'LSB Length','Riding Quality Length'],
        colModel: [

                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'PLAN_CN_ROAD_NUMBER', index: 'PLAN_CN_ROAD_NUMBER', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'ROAD_NAME', index: 'ROAD_NAME', height: 'auto', width: 200, align: "left", search: false },
                            { name: 'IMS_PROPOSAL_TYPE', index: 'IMS_PROPOSAL_TYPE', height: 'auto', width: 50, align: "center", search: false, hidden: true },
                            { name: 'ROAD_TYPE', index: 'ROAD_TYPE', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'IMS_UPGRADE_CONNECT', index: 'IMS_UPGRADE_CONNECT', height: 'auto', width: 70, align: "center", search: false },
                            { name: 'ROAD_LENGTH', index: 'ROAD_LENGTH', height: 'auto', width: 150, align: "right", search: false, formatter: "number", formatoptions: { decimalPlaces: 3 }, summaryType: 'sum' },
                            { name: 'CARRIAGE_WIDTH', index: 'CARRIAGE_WIDTH', height: 'auto', width: 80, align: "right", search: false },
                            { name: 'IMS_IS_STAGED', index: 'IMS_IS_STAGED', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_NO_OF_CDWORKS', index: 'IMS_NO_OF_CDWORKS', height: 'auto', width: 60, align: "center", search: false, summaryType: 'sum' },
                            { name: 'MORD_COST', index: 'MORD_COST', height: 'auto', width: 80, align: "right", search: false, formatter: "number", formatoptions: { decimalPlaces: 2 }, summaryType: 'sum' },
                            { name: 'STATECOST', index: 'STATECOST', height: 'auto', width: 80, align: "right", search: false, formatter: "number", formatoptions: { decimalPlaces: 2 }, summaryType: 'sum' },
                            { name: 'TOTAL_COST', index: 'TOTAL_COST', height: 'auto', width: 90, align: "right", search: false, formatter: "number", formatoptions: { decimalPlaces: 2 }, summaryType: 'sum' },
                            { name: 'MAINT_AMT', index: 'MAINT_AMT', height: 'auto', width: 80, align: "right", search: false, formatter: "number", formatoptions: { decimalPlaces: 2 }, summaryType: 'sum' },
                            { name: 'Habitation', index: 'Habitation', height: 'auto', width: 140, align: "center", search: false },
                            { name: 'Hab1000', index: 'Hab1000', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                            { name: 'Hab999', index: 'Hab999', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                            { name: 'Hab499', index: 'Hab499', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                            { name: 'Hab250', index: 'Hab250', height: 'auto', width: 80, align: "center", search: false, hidden: true },
                            { name: 'HabTotal', index: 'HabTotal', height: 'auto', width: 100, align: "center", search: false, hidden: true },
                            { name: 'HigherSpec', index: 'HigherSpec', height: 'auto', width: 100, align: "right", search: false, formatter: "number", formatoptions: { decimalPlaces: 2 }, summaryType: 'sum', hidden: hiddenScheme2Col },
                            { name: 'RenewalAmt', index: 'RenewalAmt', height: 'auto', width: 100, align: "right", search: false, formatter: "number", formatoptions: { decimalPlaces: 2 }, summaryType: 'sum', hidden: ($("#ddlSchemes option:selected").val() == 1) ? true : false },
                            { name: 'TECH', index: 'TECH', height: 'auto', width: 100, align: "center", search: false },
                            ///Changes by SAMMED A. PATIL on 20JULY2017 
                            { name: 'RD_LENGTH', index: 'RD_LENGTH', height: 'auto', width: 100, align: "center", search: false, hidden: false },
                            { name: 'LSB_LENGTH', index: 'LSB_LENGTH', height: 'auto', width: 100, align: "center", search: false, hidden: false },
                            { name: 'RidingQualityLength', index: 'RidingQualityLength', height: 'auto', width: 100, align: "center", search: false, hidden: $("#ddlSchemes option:selected").val() == 4 ? false : true },
        ],
        pager: jQuery('#dvlstPagerProposal'),
        rowNum: 2147483647,
        //rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'ROAD_TYPE',
        sortorder: "desc",
        caption: "&nbsp;&nbsp; Road List For Attachment with Sanction Letter - " + $("#ddlStates option:selected").text(),
        height: '400px',
        hidegrid: true,
        rownumbers: true,
        autowidth: true,
        shrinkToFit: false,
        footerrow: true,
        cmTemplate: { title: false },
        grouping: true,
        groupingView: {
            groupField: ['ROAD_TYPE', 'MAST_DISTRICT_NAME', 'MAST_BLOCK_NAME'],
            //groupColumnShow: false,
            groupSummary: [true, true, true],
            showSummaryOnHide: true,
            groupText: ['<b>{0}</b>', '<b>District:- {0}</b>', '<b>Block:- {0}</b>'],
        },
        loadComplete: function (data) {
            var records = jQuery("#tblstProposal").jqGrid('getGridParam', 'records');
            if (records > 0) {
                if (records > 20) {
                    jQuery("#tblstProposal").jqGrid('setGridHeight', '450px');
                }
                else {
                    jQuery("#tblstProposal").jqGrid('setGridHeight', 'auto');
                }

                if (data.IsSOGenerated == false) {
                    $("#tblstProposal #dvlstPagerProposal").css({ height: '50px' });
                    $("#dvlstPagerProposal_left").html("<input type='button' style='margin-left:27px' id='btnGenerateDetails' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'GenerateSanctionOrder();return false;' value='Generate Sanction Order'/><input type='button' style='margin-left:27px' id='btnViewDetails' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'ViewSanctionOrder();return false;' value='View Sanction Order'/><input type='button' style='margin-left:27px' id='btnViewDistrictAbstract' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'ViewDistrictAbstract();return false;' value='View District Abstract'/>")
                }
                else {
                    $("#tblstProposal #dvlstPagerProposal").css({ height: '50px' });
                    $("#dvlstPagerProposal_left").html("<input type='button' style='margin-left:27px' id='btnViewDetails' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'ViewSanctionOrder();return false;' value='View Sanction Order'/><input type='button' style='margin-left:27px' id='btnViewDistrictAbstract' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'ViewDistrictAbstract();return false;' value='View District Abstract'/>")
                }

                //if (data.IsSOGenerated == false) {
                //$("#tblstProposal #dvlstPagerProposal").css({ height: '50px' });
                //$("#dvlstPagerProposal_left").html("")
                //$("#tblstProposal #dvlstPagerProposal").css({ height: '50px' });
                //$("#dvlstPagerProposal_left").html("<input type='button' style='margin-left:10px' id='btnViewDetails' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'ViewSanctionOrder();return false;' value='View Details'/><input type='button' style='margin-left:20px' id='btnGenerateDetails' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'GenerateSanctionOrder();return false;' value='Generate Sanction Order'/>")
                //}
                //else {

                //}
            }
            else {
                jQuery("#tblstProposal").jqGrid('setGridHeight', 'auto');
            }

            //var lengthTotal = jQuery("#tblstProposal").jqGrid('getCol', 'ROAD_LENGTH', false, 'sum');

            ///Changes by SAMMED A. PATIL on 20JULY2017 
            var lengthTotal = jQuery("#tblstProposal").jqGrid('getCol', 'RD_LENGTH', false, 'sum');
            var lengthLSBTotal = jQuery("#tblstProposal").jqGrid('getCol', 'LSB_LENGTH', false, 'sum');

            var cdWorksTotal = jQuery("#tblstProposal").jqGrid('getCol', 'IMS_NO_OF_CDWORKS', false, 'sum');
            var mordcostTotal = jQuery("#tblstProposal").jqGrid('getCol', 'MORD_COST', false, 'sum');
            var statecostTotal = jQuery("#tblstProposal").jqGrid('getCol', 'STATECOST', false, 'sum');
            var costTotal = jQuery("#tblstProposal").jqGrid('getCol', 'TOTAL_COST', false, 'sum');
            var maintenanceCost = jQuery("#tblstProposal").jqGrid('getCol', 'MAINT_AMT', false, 'sum');
            var hab1000 = jQuery("#tblstProposal").jqGrid('getCol', 'Hab1000', false, 'sum');
            var hab999 = jQuery("#tblstProposal").jqGrid('getCol', 'Hab999', false, 'sum');
            var hab450 = jQuery("#tblstProposal").jqGrid('getCol', 'Hab499', false, 'sum');
            var hab250 = jQuery("#tblstProposal").jqGrid('getCol', 'Hab250', false, 'sum');
            var habTotal = jQuery("#tblstProposal").jqGrid('getCol', 'HabTotal', false, 'sum');
            var higherSpecTotal = jQuery("#tblstProposal").jqGrid('getCol', 'HigherSpec', false, 'sum');
            var renewalAmtTotal = jQuery("#tblstProposal").jqGrid('getCol', 'RenewalAmt', false, 'sum');

            jQuery("#tblstProposal").jqGrid('footerData', 'set',
            {
                IMS_UPGRADE_CONNECT: 'Total:',
                ///Changes by SAMMED A. PATIL on 20JULY2017 
                ROAD_LENGTH: 'Road Length: ' + (parseFloat(lengthTotal).toFixed(3)) + '</br> LSB Length: ' + (parseFloat(lengthLSBTotal).toFixed(3)),
                MORD_COST: parseFloat(mordcostTotal).toFixed(2),
                STATECOST: parseFloat(statecostTotal).toFixed(2),
                TOTAL_COST: parseFloat(costTotal).toFixed(2),
                IMS_NO_OF_CDWORKS: cdWorksTotal,
                MAINT_AMT: parseFloat(maintenanceCost).toFixed(2),
                Habitation: '(' + hab1000 + ',' + hab999 + ',' + hab450 + ',' + hab250 + ',' + habTotal + ')',
                HigherSpec: higherSpecTotal,
                RenewalAmt: renewalAmtTotal

            }, false);

        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });
}
function GenerateSanctionOrder() {
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Sanction Order Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseDetails();" /></a>'
            );

    var postData = $("#tblstProposal").jqGrid('getGridParam', 'postData');

    $('#accordion').show('fold', function () {
        blockPage();

        $("#divAddSanctionOrder").load("/Proposal/SanctionOrderView?" + $.param({ StateCode: postData.StateCode, StreamCode: postData.StreamCode, YearCode: postData.YearCode, BatchCode: postData.BatchCode, SchemeCode: postData.Scheme, Agency: postData.Agency }), function () {
            $.validator.unobtrusive.parse($('#divAddSanctionOrder'));
            unblockPage();
        });
        $('#divAddSanctionOrder').show('slow');
        $("#divAddSanctionOrder").css('height', 'auto');
        $("#tblstProposal").jqGrid('setGridState', 'hidden');
    });
}
function CloseDetails() {
    $("#tblstProposal").jqGrid('setGridState', 'visible');
    $('#accordion').hide('slow');
}
function ViewSanctionOrder() {
    var postData = $("#tblstProposal").jqGrid('getGridParam', 'postData');

    //window.open('/Proposal/ViewSanctionOrderReport?' + $.param({ StateCode: postData.StateCode, StreamCode: postData.StreamCode, YearCode: postData.YearCode, BatchCode: postData.BatchCode, SchemeCode: postData.Scheme}), '_blank');
    window.open('/Proposal/PreviewSanctionOrderReport?' + $.param({ StateCode: postData.StateCode, StreamCode: postData.StreamCode, YearCode: postData.YearCode, BatchCode: postData.BatchCode, SchemeCode: postData.Scheme, ProposalType: postData.Type, Agency: postData.Agency }), '_blank');
}

function ViewDistrictAbstract() {
    var postData = $("#tblstProposal").jqGrid('getGridParam', 'postData');

    //window.open('/Proposal/ViewSanctionOrderReport?' + $.param({ StateCode: postData.StateCode, StreamCode: postData.StreamCode, YearCode: postData.YearCode, BatchCode: postData.BatchCode, SchemeCode: postData.Scheme}), '_blank');
    window.open('/Proposal/PreviewDistrictAbstractReport?' + $.param({ StateCode: postData.StateCode, StreamCode: postData.StreamCode, YearCode: postData.YearCode, BatchCode: postData.BatchCode, SchemeCode: postData.Scheme, ProposalType: postData.Type, Agency: postData.Agency }), '_blank');
}

function LoadSanctionOrderList() {
    $("#tblstSanctionOrder").jqGrid('GridUnload');

    jQuery("#tblstSanctionOrder").jqGrid({
        url: '/Proposal/GetSanctionOrderList',
        datatype: "json",
        mtype: "POST",
        postData: { StateCode: $("#ddlStates option:selected").val(), YearCode: $("#ddlYears option:selected").val(), StreamCode: $("#ddlStreams option:selected").val(), BatchCode: $("#ddlBatch option:selected").val(), Scheme: $("#ddlSchemes option:selected").val(), Type: $("#ddlProposalType option:selected").val(), Agency: $("#ddlAgency option:selected").val() },
        colNames: ['Sanction Order Number', 'Order Date', 'State', 'Batch', 'Year', 'Collaboration', 'Scheme', 'View'],
        colModel: [

                            { name: 'IMS_ORDER_NUMBER', index: 'IMS_ORDER_NUMBER', height: 'auto', width: 250, align: "center", search: false },
                            { name: 'IMS_ORDER_DATE', index: 'IMS_ORDER_DATE', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'IMS_BATCH', index: 'IMS_BATCH', height: 'auto', width: 250, align: "center", search: false },
                            { name: 'IMS_YEAR', index: 'IMS_YEAR', height: 'auto', width: 250, align: "center", search: false, hidden: true },
                            { name: 'IMS_COLLABORATION', index: 'IMS_COLLABORATION', height: 'auto', width: 300, align: "center", search: false },
                            { name: 'MAST_PMGSY_SCHEME', index: 'MAST_PMGSY_SCHEME', height: 'auto', width: 250, align: "center", search: false },
                            { name: 'View', index: 'View', height: 'auto', width: 250, align: "right", search: false }
        ],
        pager: jQuery('#dvlstPagerSanctionOrder'),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'IMS_ORDER_DATE',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Generated Sanction Order List",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        autowidth: true,
        shrinkToFit: false,
        cmTemplate: { title: false },
        loadComplete: function () { },
        loadError: function () { }
    });
}
