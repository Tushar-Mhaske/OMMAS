$(document).ready(function () {

    $("#revisionFilterDetails").load('/Proposal/RevisionFilterDetails/', function () {
    });

    $("#btnListProposal").click(function () {

        if ($("#ddlDistrict").val() == '0') {
            alert('Please select district.');
            return false;
        }
        LoadMordProposals($("#ddlImsYear").val(), $("#ddlState").val(), $("#ddlDistrict").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val());
    });

    $("#ddlState").change(function () {
        if ($("#ddlState").val() > 0) {

            $("#ddlDistrict").empty();

            $.ajax({
                url: '/Proposal/GetDistricts',
                type: 'POST',
                beforeSend: function () {
                    blockPage();
                },
                data: { MAST_STATE_CODE: $("#ddlState").val(), value: Math.random() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    unblockPage();
                },
                error: function (err) {
                    alert("error " + err);
                    unblockPage();
                }
            });

        }
    });

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });


});
function LoadMordProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS) {

    if (IMS_PROPOSAL_TYPE == "P") {

        $("#tbMORDLSBProposalList").hide();
        $("#dvMORDLSBProposalListPager").hide();
        $("#tbMORDProposalList").show();
        $("#dvMORDProposalListPager").show();
        $('#tbMORDProposalList').jqGrid('GridUnload');
        $('#tbMORDLSBProposalList').jqGrid('GridUnload');

        // MordListRoadProposals(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE);
        MordListRoadProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
    }

    if (IMS_PROPOSAL_TYPE == "L") {

        $("#tbMORDLSBProposalList").show();
        $("#dvMORDLSBProposalListPager").show();
        $("#tbMORDProposalList").hide();
        $("#dvMORDProposalListPager").hide();
        $('#tbMORDProposalList').jqGrid('GridUnload');
        $('#tbMORDLSBProposalList').jqGrid('GridUnload');

        MordListLSBProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
    }

    if (IMS_PROPOSAL_TYPE == "A") {

        $("#tbMORDProposalList").show();
        $("#dvMORDProposalListPager").show();
        $("#tbMORDLSBProposalList").show();
        $("#dvMORDLSBProposalListPager").show();
        $('#tbMORDLSBProposalList').jqGrid('GridUnload');
        $('#tbMORDProposalList').jqGrid('GridUnload');

        MordListRoadProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
        MordListLSBProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
    }
}
function MordListRoadProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS) {

    blockPage();
    jQuery("#tbMORDProposalList").jqGrid({
        url: '/Proposal/GetMORDRoadProposals',
        datatype: "json",
        mtype: "POST",
        //colNames: ['District', 'Block', "Package Number", "Road Name", "No. of Benifitted Habitaions", "Pavement Length", "Total Cost", "View"],
        //colModel: [
        //            { name: 'District', index: 'District', width: 140, sortable: false, align: "center" },
        //            { name: 'Block', index: 'Block', width: 140, sortable: false, align: "center" },
        //            { name: 'PackageNumber', index: 'PackageNumber', width: 140, sortable: false, align: "center" },
        //            { name: 'RoadName', index: 'RoadName', width: 200, sortable: false, align: "left" },
        //            { name: 'habs', index: 'habs', width: 100, sortable: false, align: "center" },
        //            { name: 'PavementLength', index: 'PavementLength', width: 150, sortable: false, align: "center" },
        //            { name: 'PavementCost', index: 'PavementCost', width: 180, sortable: false, align: "center" },
        //            { name: 'ShowDetails', index: 'ShowDetails', width: 40, sortable: false, align: "center" },

        //],
        colNames: ['District', 'Block', "Package Number", "Year", "Road Name", "1000+", "999-500", "499-250", "Less Than 250", "Total Habitations", "Pavement Length (in Kms.)", "MoRD Share (in Lakhs)", "State Share (in Lakhs)", "Maintenance Cost (in Lakhs)", "Renewal Amount (in Lakhs)", "Higher Specification Cost (in Lakhs)", "Stage Construction", "STA (Scrutiny Date)", "PTA (Scrutiny Date)", "Status", "View"],
        colModel: [
                    { name: 'District', index: 'District', width: 60, sortable: false, align: "center" },
                    { name: 'Block', index: 'Block', width: 60, sortable: false, align: "center" },
                    { name: 'PackageNumber', index: 'PackageNumber', width: 80, sortable: false, align: "center" },
                    { name: 'IMS_YEAR', index: 'IMS_YEAR', width: 60, sortable: false, align: "center" },
                    { name: 'RoadName', index: 'RoadName', width: 200, sortable: false, align: "left" },
                    //{ name: 'habs', index: 'habs', width: 100, sortable: false, align: "right" },
                    { name: 'Hab1000', index: 'Hab1000', width: 50, sortable: false, align: "right" },
                    { name: 'Hab999', index: 'Hab999', width: 50, sortable: false, align: "right" },
                    { name: 'Hab499', index: 'Hab499', width: 50, sortable: false, align: "right" },
                    { name: 'Hab250', index: 'Hab250', width: 50, sortable: false, align: "right" },
                    { name: 'HabTotal', index: 'HabTotal', width: 50, sortable: false, align: "right" },
                    { name: 'PavementLength', index: 'PavementLength', width: 50, sortable: false, align: "right" },
                    { name: 'PavementCost', index: 'PavementCost', width: 60, sortable: false, align: "right" },
                    { name: 'StateCost', index: 'StateCost', width: 60, sortable: false, align: "right" },
                    { name: 'MAINT_AMT', index: 'MAINT_AMT', width: 60, sortable: false, align: "right" },
                    { name: 'RENEWAL_AMT', index: 'RENEWAL_AMT', width: 60, sortable: false, align: "right", hidden: ($("#PMGSYScheme").val() == 1 ? true : false) },
                    { name: 'HIGHER_SPECS', index: 'HIGHER_SPECS', width: 60, sortable: false, align: "right", hidden: ($("#PMGSYScheme").val() == 1 ? true : false) },
                    { name: 'STAGE_CONST', index: 'STAGE_CONST', width: 60, sortable: false, align: "center" },
                    { name: 'STA_SCRUTINY', index: 'STA_SCRUTINY', width: 120, sortable: false, align: "left" },
                    //{ name: 'STA_SANCTIONED_BY', index: 'STA_SANCTIONED_BY', width: 60, sortable: false, align: "right" },
                    //{ name: 'STA_SANCTIONED_DATE', index: 'STA_SANCTIONED_DATE', width: 60, sortable: false, align: "right" },
                    { name: 'PTA_SCRUTINY', index: 'PTA_SCRUTINY', width: 120, sortable: false, align: "left" },
                    //{ name: 'PTA_SANCTIONED_BY', index: 'PTA_SANCTIONED_BY', width: 60, sortable: false, align: "right" },
                    //{ name: 'PTA_SANCTIONED_DATE', index: 'PTA_SANCTIONED_DATE', width: 60, sortable: false, align: "right" },
                    { name: 'PROPOSAL_STATUS', index: 'PROPOSAL_STATUS', width: 60, sortable: false, align: "center", hidden: true },
                    { name: 'ShowDetails', index: 'ShowDetails', width: 50, sortable: false, align: "center", search: false }
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_STATE_ID": MAST_STATE_ID, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_PROPOSAL_STATUS": IMS_PROPOSAL_STATUS },
        pager: jQuery('#dvMORDProposalListPager'),
        rowList: [04, 08, 12],
        rowNum: 4,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Road Proposals",
        height: 'auto',
        autowidth: false,
        width: 1180,
        //autowidth: true,
        sortname: 'District',
        rownumbers: true,
        footerrow: true,
        shrinkToFit: false,
        loadComplete: function (data) {

            if (data.records == 0)
            {
                $("#tbMORDProposalList").css('ui-jqgrid-bdiv');
            }


            if (data.TotalColumn != null) {
                var lengthTotal = jQuery("#tbMORDProposalList").jqGrid('getCol', 'PavementLength', false, 'sum');
                var costTotal = jQuery("#tbMORDProposalList").jqGrid('getCol', 'PavementCost', false, 'sum');
                var habs1000Total = jQuery("#tbMORDProposalList").jqGrid('getCol', 'Hab1000', false, 'sum');
                var habs999Total = jQuery("#tbMORDProposalList").jqGrid('getCol', 'Hab999', false, 'sum');
                var habs499Total = jQuery("#tbMORDProposalList").jqGrid('getCol', 'Hab499', false, 'sum');
                var habs250Total = jQuery("#tbMORDProposalList").jqGrid('getCol', 'Hab250', false, 'sum');
                var habsTotal = jQuery("#tbMORDProposalList").jqGrid('getCol', 'HabTotal', false, 'sum');
                var stateCost = jQuery("#tbMORDProposalList").jqGrid('getCol', 'StateCost', false, 'sum');
                var maintenanceCost = jQuery("#tbMORDProposalList").jqGrid('getCol', 'MAINT_AMT', false, 'sum');
                var renewalCost = jQuery("#tbMORDProposalList").jqGrid('getCol', 'RENEWAL_AMT', false, 'sum');
                var higherSpecCost = jQuery("#tbMORDProposalList").jqGrid('getCol', 'HIGHER_SPECS', false, 'sum');


                jQuery("#tbMORDProposalList").jqGrid('footerData', 'set',
                {
                    RoadName: 'Page Total:',
                    PavementLength: parseFloat(lengthTotal).toFixed(3),
                    PavementCost: parseFloat(costTotal).toFixed(2),
                    Hab1000: habs1000Total,
                    Hab999: habs1000Total,
                    Hab499: habs1000Total,
                    Hab250: habs1000Total,
                    HabTotal: habsTotal,
                    StateCost: parseFloat(stateCost).toFixed(2),
                    MAINT_AMT: parseFloat(maintenanceCost).toFixed(2),
                    RENEWAL_AMT: parseFloat(renewalCost).toFixed(2),
                    HIGHER_SPECS: parseFloat(higherSpecCost).toFixed(2)
                });

                jQuery("#tbMORDProposalList").jqGrid('footerData', 'set',
                {
                    RoadName: 'Grand Total:',
                    PavementLength: parseFloat(data.TotalColumn.TOT_PAV_LENGTH).toFixed(3),
                    PavementCost: parseFloat(data.TotalColumn.TOT_MORD_COST).toFixed(2),
                    Hab1000: data.TotalColumn.TOT_HAB1000,
                    Hab999: data.TotalColumn.TOT_HAB999,
                    Hab499: data.TotalColumn.TOT_HAB499,
                    Hab250: data.TotalColumn.TOT_HAB250,
                    HabTotal: data.TotalColumn.TOT_HABS,
                    StateCost: parseFloat(data.TotalColumn.TOT_STATE_COST).toFixed(2),
                    MAINT_AMT: parseFloat(data.TotalColumn.TOT_MANE_COST).toFixed(2),
                    RENEWAL_AMT: parseFloat(data.TotalColumn.TOT_RENEWAL_COST).toFixed(2),
                    HIGHER_SPECS: parseFloat(data.TotalColumn.TOT_HIGHER_SPEC).toFixed(2)
                });
            }


            unblockPage();
        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                //alert(xhr.responseText);
                window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert("Session Timeout !!!");
                window.location.href = "/Login/SessionExpire";
            }
        }
    }); //end of grid   
}
function MordListLSBProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS) {

    blockPage();
    jQuery("#tbMORDLSBProposalList").jqGrid({
        url: '/LSBProposal/GetMORDLSBProposals',
        datatype: "json",
        mtype: "POST",
        colNames: ['District', 'Block', "Package Number", "Sanctioned Year", "Road Name", "LSB Name", "LSB Length (mtrs)", "State Share (lakhs)", "MoRD Cost (lakhs)", "Higher Specification Cost (in Lakhs)", "Maintenance Cost (Lacs)", "STA (Scrutiny Date)", "PTA (Scrutiny Date)", "Status", "View"],
        colModel: [
                    { name: 'District', index: 'District', width: 100, sortable: false, align: "center" },
                    { name: 'Block', index: 'Block', width: 100, sortable: false, align: "center" },
                    { name: 'PackageNumber', index: 'PackageNumber', width: 100, sortable: false, align: "center" },
                    { name: 'SanctionYear', index: 'SanctionYear', width: 100, sortable: false, align: "center" },
                    { name: 'RoadName', index: 'RoadName', width: 250, sortable: false, align: "left" },
                    { name: 'BridgeName', index: 'BridgeName', width: 230, sortable: false, align: "center" },
                    { name: 'BridgeLength', index: 'BridgeLength', width: 90, sortable: false, align: "center" },
                    { name: 'StateShare', index: 'StateShare', width: 90, sortable: false, align: "center" },
                    { name: 'MordCost', index: 'MordCost', width: 90, sortable: false, align: "center" },
                    { name: 'HigherSpec', index: 'HigherSpec', width: 80, sortable: false, align: "right" },
                    { name: 'MaintenanceCost', index: 'MaintenanceCost', width: 80, sortable: false, align: "right" },
                    { name: 'STA_SCRUTINY', index: 'STA_SCRUTINY', width: 120, sortable: false, align: "left" },
                    { name: 'PTA_SCRUTINY', index: 'PTA_SCRUTINY', width: 120, sortable: false, align: "left" },
                    { name: 'PROPOSAL_STATUS', index: 'PROPOSAL_STATUS', width: 60, sortable: false, align: "center", hidden: true },
                    { name: 'ShowDetails', index: 'ShowDetails', width: 40, sortable: false, align: "center" }
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_STATE_ID": MAST_STATE_ID, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_PROPOSAL_STATUS": IMS_PROPOSAL_STATUS },
        pager: jQuery('#dvMORDLSBProposalListPager'),
        rowList: [04, 08, 12],
        rowNum: 4,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;LSB Proposals",
        height: 'auto',
        autowidth: false,
        shrinkToFit: false,
        width: 1180,
        footerrow: true,
        sortname: 'District',
        rownumbers: true,
        loadComplete: function () {
            unblockPage();
            var TBridgeLength = $(this).jqGrid('getCol', 'BridgeLength', false, 'sum');
            TBridgeLength = parseFloat(TBridgeLength).toFixed(3);
            var TStateShare = $(this).jqGrid('getCol', 'StateShare', false, 'sum');
            TStateShare = parseFloat(TStateShare).toFixed(2);
            var TMordCost = $(this).jqGrid('getCol', 'MordCost', false, 'sum');
            TMordCost = parseFloat(TMordCost).toFixed(2);
            var THigherSpec = $(this).jqGrid('getCol', 'HigherSpec', false, 'sum');
            THigherSpec = parseFloat(THigherSpec).toFixed(2);
            var TMaintenanceCost = $(this).jqGrid('getCol', 'MaintenanceCost', false, 'sum');
            TMaintenanceCost = parseFloat(TMaintenanceCost).toFixed(2);

            $(this).jqGrid('footerData', 'set', { RoadName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { BridgeLength: TBridgeLength }, true);
            $(this).jqGrid('footerData', 'set', { StateShare: TStateShare }, true);
            $(this).jqGrid('footerData', 'set', { MordCost: TMordCost }, true);
            $(this).jqGrid('footerData', 'set', { HigherSpec: THigherSpec }, true);
            $(this).jqGrid('footerData', 'set', { MaintenanceCost: TMaintenanceCost }, true);

        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                //alert(xhr.responseText);
                window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert("Session Timeout !!!");
                window.location.href = "/Login/SessionExpire";
            }
        }
    }); //end of grid   
    unblockPage();
}
function ShowLSBDetails(id) {
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >LSB Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/LSBProposal/LSBDetails?id=' + id, function () {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');
}
function ShowDetails(id) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Road Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/Proposal/Details?id=' + id, function () {
            $.validator.unobtrusive.parse($('#divProposalForm'));

            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');

    jQuery('#tbStaProposalList').jqGrid('setGridState', 'hidden');

}
function CloseProposalDetails()
{
    $("#accordion").hide('slow');
    $("#tbProposalList").jqGrid('setGridState', 'visible');
}