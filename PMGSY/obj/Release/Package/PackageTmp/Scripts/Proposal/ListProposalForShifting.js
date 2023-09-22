$(document).ready(function () {

    blockPage();

    if ($("#ddlState").is(":visible")) {
        if ($("#RoleID").val() != 3) {
            $("#ddlState").val(0);
        }
    }

    if ($("#RoleID").val() == '2' || $("#RoleID").val() == '37' || $("#RoleID").val() == '55' || $("#RoleID").val() == '36')
    {        // ITNO    //SRRDA or SRRDARCPLWE - listing Only 

        $("#divSRRDAProposal").show();
        $("#btnAddProposal").hide();
        LoadSRRDAProposals($("#ddlImsYear").val(), $("#ddlDistrict").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val(), $("#ddlImsConnectivity").val());
    }
    else if ($("#RoleID").val() == '3') //STA
    {
        $("#divStaProposal").show();
        $("#btnAddProposal").hide();

        if ($("#ddlState").val() > 0) {
            STAListRoadProposals($("#ddlImsYear").val(), $("#ddlDistrict").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val());

        }
    }
    else if ($("#RoleID").val() == '15') {
        $("#btnAddProposal").hide();
        $("#divPtaProposal").show();

        PTAListRoadProposals($("#ddlImsYear").val(), $("#ddlState").val(), $("#ddlDistrict").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val());
    }

    else if ($("#RoleID").val() == '22' || $("#RoleID").val() == '38' || $("#RoleID").val() == '54') { ///Changes for RCPLWE
        $("#dvProposal").show();
        LoadProposals($("#ddlImsYear").val(), $("#ddlMastBlockCode").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus option:selected").val(), $("#ddlImsConnectivity option:selected").val());
    }

    unblockPage();

    $("#idFilterDiv").click(function () {

        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divFilterForm").toggle("slow");

    });

    $('#btnAddProposal').click(function () {
        $("#accordion div").html("");
        $("#accordion h3").html(
                "<a href='#' style= 'font-size:.9em;' >Add " + $("#ddlImsProposalTypes option:selected").text() + " Proposal Details</a>" +

                '<a href="#" style="float: right;">' +
                '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseProposalDetails();" /></a>' +
                '<span style="float: right;"></span>'
                );

        $('#accordion').show('slow', function () {
            blockPage();

            if ($("#ddlImsProposalTypes").val() == "P") {

                $("#divProposalForm").load('/Proposal/Create/' + $("#ddlImsYear").val() + "$" + $("#ddlMastBlockCode").val() + "$" + $("#ddlImsBatch").val() + "$" + $("#ddlImsStreams").val(), function () {
                    $.validator.unobtrusive.parse($('#divProposalForm'));
                    unblockPage();
                });
            }
            else if ($("#ddlImsProposalTypes").val() == "L") {

                $("#divProposalForm").load('/LSBProposal/CreateLSB/' + $("#ddlImsYear").val() + "$" + $("#ddlMastBlockCode").val() + "$" + $("#ddlImsBatch").val() + "$" + $("#ddlImsStreams").val(), function () {
                    $.validator.unobtrusive.parse($('#divProposalForm'));
                    unblockPage();
                });
            }
            else if ($("#ddlImsProposalTypes").val() == "B") {
                $("#divProposalForm").load('/BuildingProposal/BuildingCreate/' + $("#ddlImsYear").val() + "$" + $("#ddlMastBlockCode").val() + "$" + $("#ddlImsBatch").val() + "$" + $("#ddlImsStreams").val(), function () {
                    $.validator.unobtrusive.parse($('#divProposalForm'));
                    unblockPage();
                });
            }

            $('#divProposalForm').show('slow');
            $("#divProposalForm").css('height', 'auto');
        });

        $("#tbProposalList").jqGrid('setGridState', 'hidden');
        $("#tbLSBProposalList").jqGrid('setGridState', 'hidden');
        $('#idFilterDiv').trigger('click');
    });

    $("#btnListProposal").click(function ()
    {

        if ($("#ddlDistrict").val() == 0 || $("#ddlDistrict").val() == -1)
        {
            alert("Please select District");
            return false;
        }

        if ($("#ddlImsYear").val() == 0 || $("#ddlImsYear").val() == -1) {
            alert("Please select Year");
            return false;
        }


        blockPage();
        if ($("#RoleID").val() == '2' || $("#RoleID").val() == '37' || $("#RoleID").val() == '55' || $("#RoleID").val() == '36') {          // ITNO , SRRDA or SRRDARCPLWE - listing Only

            LoadSRRDAProposals($("#ddlImsYear").val(), $("#ddlDistrict").val(), $("#ddlMastBlockCode").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val(), $("#ddlImsConnectivity").val());
        }
        else if ($("#RoleID").val() == '3') //STA
        {
            if ($("#ddlState").val() > 0) {
                LoadSTAProposals($("#ddlImsYear").val(), $("#ddlDistrict").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val());
            }
            else {
                alert("Please Select State");
            }
        }
        else if ($("#RoleID").val() == '15') {
            if (validateFilter()) {
                LoadPTAProposals($("#ddlImsYear").val(), $("#ddlState").val(), $("#ddlDistrict").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val());
            }
        }
        else if ($("#RoleID").val() == '22' || $("#RoleID").val() == '38' || $("#RoleID").val() == '54') {  ///Changes for RCPLWE
            CloseProposalDetails();
            //LoadProposals() called in CloseProposalDetails()
        }
        else if ($("#RoleID").val() == '25' || $("#RoleID").val() == '65') {//Changes by SAMMED A. PATIL for Mord View
            if (validateFilter()) {
                LoadMordProposals($("#ddlImsYear").val(), $("#ddlState").val(), $("#ddlDistrict").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val(), $("#ddlImsConnectivity").val(), $("#ddlImsAgencies").val());
            }
        }
        unblockPage();
    });

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
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

                    PopulateAgenciesStateWise();
                    unblockPage();
                },
                error: function (err) {
                    alert("error " + err);
                    unblockPage();
                }
            });

        }
    });

    $("#ddlImsProposalTypes").change(function () {
        if ($(this).val() == "A") {
            $("#btnAddProposal").hide("blink");
        }
        else {
            $("#btnAddProposal").show("fold");
        }
    });

    $("#ddlDistrict").change(function () {

        $.ajax({
            url: '/Proposal/PopulateBlocks',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { districtCode: $("#ddlDistrict").val(), value: Math.random() },
            success: function (jsonData) {
                $("#ddlMastBlockCode").empty();
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Selected == true) {
                        $("#ddlMastBlockCode").append("<option value='" + jsonData[i].Value + "' selected=true>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlMastBlockCode").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
    validateDateListProposal();
    $('#ddlImsYear').change(function () {
        validateDateListProposal();
        //if ($('#ddlImsYear').val() < 2016)
        //{
        //    $('#btnAddProposal').hide('slow');
        //}
        //else
        //{
        //    $('#btnAddProposal').show('slow');
        //}
    });

    setTimeout(function () {
        $('#btnListProposal').trigger('click');
    }, 300);


    $("#dvShiftProposal").dialog({
        autoOpen: false,
        height: 'auto',
        width: "450",
        modal: true,
        title: 'Shift Proposal Details'
    });




});

function validateDateListProposal() {
    var currentYear = (new Date).getFullYear();
    var currentMonth = (new Date).getMonth() + 1;
    var currentDay = (new Date).getDate();

    var currFinancialYear = parseInt(currentMonth) <= 3 ? parseInt(currentYear - 1) : parseInt(currentYear);

    if (parseInt($('#ddlImsYear').val()) >= parseInt(currFinancialYear)) {
        $('#btnAddProposal').show('slow');
    }
    else {
        $('#btnAddProposal').hide('slow');
    }
}

function PopulateAgenciesStateWise() {
    $.ajax({
        url: '/Proposal/PopulateAgencies',
        type: 'POST',
        beforeSend: function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        },
        data: { stateCode: $("#ddlState").val(), value: Math.random() },
        success: function (jsonData) {
            $("#ddlImsAgencies").empty();
            for (var i = 0; i < jsonData.length; i++) {
                if (jsonData[i].Selected == true) {
                    $("#ddlImsAgencies").append("<option value='" + jsonData[i].Value + "' selected=true>" + jsonData[i].Text + "</option>");
                }
                else {
                    $("#ddlImsAgencies").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
            }

            $.unblockUI();
        },
        error: function (err) {
            //alert("error " + err);
            $.unblockUI();
        }
    });
}

function validateFilter() {
    if ($("#ddlState").val() == "0") {
        alert("Please Select State");
        return false;
    }
    if ($("#ddlDistrict").val() == "0") {
        alert("Please Select District");
        return false;
    }
    return true;
}

function showFilter() {
    if ($('#divFilterForm').is(":hidden")) {
        $("#divFilterForm").show("slow");
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    }
}

function CloseProposalDetails() {
    $('#accordion').hide('slow');
    $('#divProposalForm').hide('slow');
    $("#tbProposalList").jqGrid('setGridState', 'visible');
    $("#tbLSBProposalList").jqGrid('setGridState', 'visible');
    $('#tbStaProposalList').jqGrid('setGridState', 'visible');
    $("#tbStaLSBProposalList").jqGrid('setGridState', 'visible');
    $('#tbPtaProposalList').jqGrid('setGridState', 'visible');      //change by Ujjwal Saket on 1-11-2013
    $('#tbPtaLSBProposalList').jqGrid('setGridState', 'visible');
    $("#tbMORDProposalList").jqGrid('setGridState', 'visible');
    $("#tbMORDLSBProposalList").jqGrid('setGridState', 'visible');
    $("#tbSRRDAProposalList").jqGrid('setGridState', 'visible');
    $("#tbSRRDALSBProposalList").jqGrid('setGridState', 'visible');
    $("#tbBuildingProposalList").jqGrid('setGridState', 'visible');


    // For DPIU Login Reload the Jqgrid 
    if ($("#RoleID").val() == '22' || $("#RoleID").val() == '38' || $("#RoleID").val() == '54') {  ///Changes for RCPLWE

        LoadProposals($("#ddlImsYear").val(), $("#ddlMastBlockCode").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus option:selected").val(), $("#ddlImsConnectivity option:selected").val());
    }

    showFilter();
}
// Show Habitation Details Form

function ShowDetails(id) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Road Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
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
    $('#tbStaProposalList').jqGrid('setGridState', 'hidden');
    $('#tbPtaProposalList').jqGrid('setGridState', 'hidden');          //Change by Ujjwal Saket on 1-11-2013
    $("#tbMORDProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDAProposalList").jqGrid('setGridState', 'hidden');
    $("#tbMORDLSBProposalList").jqGrid('setGridState', 'hidden');


    $('#idFilterDiv').trigger('click');

}


function LoadSRRDAProposals(IMS_YEAR, MAST_DISTRICT_ID, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT) {

    if (IMS_PROPOSAL_TYPE == "P") {

        $("#tbSRRDALSBProposalList").hide();
        $("#dvSRRDALSBProposalListPager").hide();
        $("#tbSRRDAProposalList").show();
        $("#dvSRRDAProposalListPager").show();
        $('#tbSRRDAProposalList').jqGrid('GridUnload');
        $('#tbSRRDALSBProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');





        SRRDARoadProposalGrid(IMS_YEAR, MAST_DISTRICT_ID, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
    }

    if (IMS_PROPOSAL_TYPE == "L") {
   //     alert("LSB")

        $("#tbSRRDALSBProposalList").show();
        $("#dvSRRDALSBProposalListPager").show();
        $("#tbSRRDAProposalList").hide();
        $("#dvSRRDAProposalListPager").hide();
        $('#tbSRRDALSBProposalList').jqGrid('GridUnload');
        $('#tbSRRDAProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');


        SRRDALSBProposalGrid(IMS_YEAR, MAST_DISTRICT_ID, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
    }

    if (IMS_PROPOSAL_TYPE == "A") {

        $("#tbSRRDAProposalList").show();
        $("#dvSRRDAProposalListPager").show();
        $("#tbSRRDALSBProposalList").show();
        $("#dvSRRDALSBProposalListPager").show();
        $('#tbSRRDALSBProposalList').jqGrid('GridUnload');
        $('#tbSRRDAProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');


        SRRDARoadProposalGrid(IMS_YEAR, MAST_DISTRICT_ID, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
        SRRDALSBProposalGrid(IMS_YEAR, MAST_DISTRICT_ID, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
    }
}

//-------------------------------   SRRDA Details --------------------------------------//



function SRRDARoadProposalGrid(IMS_YEAR, MAST_DISTRICT_ID, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT) {
    blockPage();
    jQuery("#tbSRRDAProposalList").jqGrid({
        url: '/Proposal/GetProposalsForITNOForShifting',
        datatype: "json",
        mtype: "POST",
        colNames: ['District', 'Block', "Package Number", "Year", "Road Name", "1000+", "999-500", "499-250", "Less Than 250", "Total Habitations", "Pavement Length (in Kms.)", "MoRD Share (in Lakhs)", ($('#PMGSYScheme').val() == 1 || $("#PMGSYScheme").val() == 3) ? "State Share (in Lakhs)" : "State Share Excluding Higher Specification (in Lakhs)", "Higher Specification Cost (in Lakhs)", "Total Cost", "Maintenance Cost (in Lakhs)", "Renewal Amount (in Lakhs)", "Fund Sharing Ratio", "State Share Cost (in Lakhs)", "Mord Share Cost (in Lakhs)", "Total State Share (in Lakhs)", "Total Share Cost (in Lakhs)", "Stage Construction", "STA (Scrutiny Date)", "PTA (Scrutiny Date)", "Status", "View", "Upload", "Shift"],
        colModel: [
                    { name: 'District', index: 'District', width: 150, sortable: false, align: "center" },
                    { name: 'Block', index: 'Block', width: 150, sortable: false, align: "center" },
                    { name: 'PackageNumber', index: 'PackageNumber', width: 150, sortable: false, align: "center" },
                    { name: 'IMS_YEAR', index: 'IMS_YEAR', width: 150, sortable: false, align: "center" },
                    { name: 'RoadName', index: 'RoadName', width: 400, sortable: false, align: "left" },
                    { name: 'Hab1000', index: 'Hab1000', width: 100, sortable: false, align: "right", hidden: true },
                    { name: 'Hab999', index: 'Hab999', width: 100, sortable: false, align: "right", hidden: true },
                    { name: 'Hab499', index: 'Hab499', width: 100, sortable: false, align: "right", hidden: true },
                    { name: 'Hab250', index: 'Hab250', width: 100, sortable: false, align: "right", hidden: true },
                    { name: 'HabTotal', index: 'HabTotal', width: 100, sortable: false, align: "right", hidden: true },
                    { name: 'PavementLength', index: 'PavementLength', width: 120, sortable: false, align: "right" },
                    { name: 'PavementCost', index: 'PavementCost', width: 120, sortable: false, align: "right" },
                    { name: 'StateCost', index: 'StateCost', width: 60, sortable: false, align: "right", hidden: true },
                    { name: 'HIGHER_SPECS', index: 'HIGHER_SPECS', width: 60, sortable: false, align: "right",hidden:true },
                    { name: 'TotalCost', index: 'TotalCost', width: 120, sortable: false, align: "right" },
                    { name: 'MAINT_AMT', index: 'MAINT_AMT', width: 120, sortable: false, align: "right", hidden: true },
                    { name: 'RENEWAL_AMT', index: 'RENEWAL_AMT', width: 60, sortable: false, align: "right", hidden: (($("#PMGSYScheme").val() == 1 /*|| $("#PMGSYScheme").val() == 3*/) ? true : false) },
                    { name: 'FundShareRatio', index: 'FundShareRatio', width: 60, sortable: false, align: "right", hidden: true },
                    { name: 'StateShareCost', index: 'StateShareCost', width: 60, sortable: false, align: "right", hidden: true },
                    { name: 'MordShareCost', index: 'MordShareCost', width: 60, sortable: false, align: "right" ,hidden:true},
                    { name: 'TotalStateShare', index: 'TotalStateShare', width: 120, sortable: false, align: "right", hidden: true },
                    { name: 'TotalShareCost', index: 'TotalShareCost', width: 120, sortable: false, align: "right", hidden: true },

                    { name: 'STAGE_CONST', index: 'STAGE_CONST', width: 150, sortable: false, align: "center" },
                    { name: 'STA_SCRUTINY', index: 'STA_SCRUTINY', width: 150, sortable: false, align: "left", hidden: true },
                    { name: 'PTA_SCRUTINY', index: 'PTA_SCRUTINY', width: 150, sortable: false, align: "left", hidden: true },
                    { name: 'PROPOSAL_STATUS', index: 'PROPOSAL_STATUS', width: 150, sortable: false, align: "center", hidden: true },
                    { name: 'ShowDetails', index: 'ShowDetails', width: 50, sortable: false, align: "center", search: false ,hidden:true},
                    { name: 'UploadDetails', index: 'UploadDetails', width: 50, sortable: false, align: "center", search: false, hidden:true },
                    { name: 'Shift', width: 150, sortable: false, resize: false, formatter: FormatColumnShift1, align: "center", search: false }
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "MAST_BLOCK_CODE": MAST_BLOCK_CODE, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_PROPOSAL_STATUS": IMS_PROPOSAL_STATUS, "IMS_UPGRADE_CONNECT": IMS_UPGRADE_CONNECT },
        pager: jQuery('#dvSRRDAProposalListPager'),
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;List Proposals For Shifting",
        height: '400px',
        width: 'auto',
        rowList: [50, 100, 150, 200],
        rowNum: 50,
        autowidth: true,
        sortname: 'District',
        rownumbers: true,
        footerrow: true,
        shrinkToFit: false,
        loadComplete: function (data) {

            if (data.records == 0) {
                $("#tbSRRDAProposalList").css('ui-jqgrid-bdiv');
            }


            if (data.TotalColumn != null) {
                var lengthTotal = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'PavementLength', false, 'sum');
                var costTotal = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'PavementCost', false, 'sum');
                var habs1000Total = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'Hab1000', false, 'sum');
                var habs999Total = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'Hab999', false, 'sum');
                var habs499Total = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'Hab499', false, 'sum');
                var habs250Total = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'Hab250', false, 'sum');
                var habsTotal = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'HabTotal', false, 'sum');
                var stateCost = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'StateCost', false, 'sum');
                var maintenanceCost = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'MAINT_AMT', false, 'sum');
                var renewalCost = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'RENEWAL_AMT', false, 'sum');
                var higherSpecCost = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'HIGHER_SPECS', false, 'sum');

                var totalCost = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'TotalCost', false, 'sum');
                var stateShareCost = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'StateShareCost', false, 'sum');
                var mordShareCost = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'MordShareCost', false, 'sum');
                var totalStateShare = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'TotalStateShare', false, 'sum');

                jQuery("#tbSRRDAProposalList").jqGrid('footerData', 'set',
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
                    HIGHER_SPECS: parseFloat(higherSpecCost).toFixed(2),
                    TotalCost: parseFloat(totalCost).toFixed(2),
                    StateShareCost: parseFloat(stateShareCost).toFixed(2),
                    MordShareCost: parseFloat(mordShareCost).toFixed(2),
                    TotalStateShare: parseFloat(totalStateShare).toFixed(2),
                });

                jQuery("#tbSRRDAProposalList").jqGrid('footerData', 'set',
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
                    HIGHER_SPECS: parseFloat(data.TotalColumn.TOT_HIGHER_SPEC).toFixed(2),
                    TotalCost: parseFloat(data.TotalColumn.TOT_COST).toFixed(2),
                    StateShareCost: parseFloat(data.TotalColumn.STATE_SHARE_COST).toFixed(2),
                    MordShareCost: parseFloat(data.TotalColumn.MORD_SHARE_COST).toFixed(2),
                    TotalStateShare: parseFloat(data.TotalColumn.TOTAL_STATE_SHARE).toFixed(2),
                    //StateShareCost: parseFloat(stateShareCost).toFixed(2),
                    //MordShareCost: parseFloat(mordShareCost).toFixed(2),
                    //TotalStateShare: parseFloat(totalStateShare).toFixed(2),
                });
            }


            unblockPage();
        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                alert('Error occurred');
                //window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert(error);
                alert('Error occurred');
            }
        },
        beforeSelectRow: function (rowid, e) {
            var $link = $('a', e.target);
            if (e.target.tagName.toUpperCase() === "A" || $link.length > 0) {
                $(this).jqGrid('setSelection', rowid);
                // link exist in the item which is clicked
                return false;
            }
            return true;
        }
    }); //end of grid

    jQuery("#tbSRRDAProposalList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
          { startColumnName: 'FundShareRatio', numberOfColumns: 5, titleText: 'Fund Sharing' },
        ]
    });

}



function SRRDALSBProposalGrid(IMS_YEAR, MAST_DISTRICT_ID, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT) {
    jQuery("#tbSRRDALSBProposalList").jqGrid({
        url: '/Proposal/GetLSBDetails?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["District", "Block", "Package", "Road Name", "LSB Name", "LSB Length (mtrs)", ($("#PMGSYScheme").val() == 1 || $("#PMGSYScheme").val() == 3) ? "State Cost (Lacs)" : "State Cost Excluding Higher Specification (Lacs)", "Higher Specification Cost", "Mord Cost (Lacs)", "Total Cost", "Maintenance Cost (Lacs)", "Fund Sharing Ratio", "State Share Cost (in Lakhs)", "Mord Share Cost (in Lakhs)", "Total State Share (in Lakhs)", "Total Share Cost (in Lakhs)", "View", "Upload",'Shift'],
        colModel: [
                        { name: 'District', index: 'District', width: 80, sortable: false, align: "left" },
                        { name: 'Block', index: 'Block', width: 80, sortable: false, align: "left" },
                        { name: 'PackageNumber', index: 'PackageNumber', width: 80, sortable: false, align: "left" },
                        { name: 'RoadName', index: 'RoadName', width: 250, sortable: false, align: "left" },
                        { name: 'LSBName', index: 'LSBName', width: 180, sortable: false, align: "left" },
                        { name: 'LSBLength', index: 'LSBLength', width: 80, sortable: false, align: "right" },
                        { name: 'StateShare', index: 'StateShare', width: 80, sortable: false, align: "right" },
                        { name: 'HIGHER_SPECS', index: 'HIGHER_SPECS', width: 60, sortable: false, align: "right", hidden: (($("#PMGSYScheme").val() == 1 || $("#PMGSYScheme").val() == 3) ? true : false) },
                        { name: 'MordCost', index: 'MordCost', width: 80, sortable: false, align: "right" },
                        { name: 'TotalCost', index: 'TotalCost', width: 60, sortable: false, align: "right" },
                        { name: 'MaintenanceCost', index: 'MaintenanceCost', width: 80, sortable: false, align: "right", hidden: true },
                        { name: 'FundShareRatio', index: 'FundShareRatio', width: 60, sortable: false, align: "right", hidden: true },
                        { name: 'StateShareCost', index: 'StateShareCost', width: 60, sortable: false, align: "right", hidden: true },
                        { name: 'MordShareCost', index: 'MordShareCost', width: 60, sortable: false, align: "right", hidden: true },
                        { name: 'TotalStateShare', index: 'TotalStateShare', width: 60, sortable: false, align: "right", hidden: true },
                        { name: 'TotalShareCost', index: 'TotalShareCost', width: 60, sortable: false, align: "right", hidden: true },
                        { name: 'ShowDetails', index: 'ShowDetails', width: 50, sortable: false, align: "center", hidden: true },
                        { name: 'UploadDetails', index: 'UploadDetails', width: 50, sortable: false, align: "center", hidden: true },
                        { name: 'ShiftLSB', width: 150, sortable: false, resize: false, formatter: FormatColumnShift11, align: "center", search: false }
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "MAST_BLOCK_CODE": MAST_BLOCK_CODE, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_PROPOSAL_STATUS": IMS_PROPOSAL_STATUS, "IMS_UPGRADE_CONNECT": IMS_UPGRADE_CONNECT },
        pager: jQuery('#dvSRRDALSBProposalListPager'),
        rowList: [25, 50, 75, 100],
        rowNum: 25,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;LSB Proposals",
        height: '400px',
        //width: 'auto',
        autowidth: true,
        shrinkToFit: false,
        sortname: 'District',
        rownumbers: true,
        footerrow: true,
        loadComplete: function (data) {
            unblockPage();
            if ($("#ddlProposalStatus").val() == "N") {
                $("#idSanctionLSBProposal").show("slow");
                $("#tbSRRDALSBProposalList").jqGrid('showCol', 'cb');
            }
            else {
                $("#idSanctionLSBProposal").hide("slow");
                $("#tbSRRDALSBProposalList").jqGrid('hideCol', 'cb');
            }


            var lengthTotal = jQuery("#tbSRRDALSBProposalList").jqGrid('getCol', 'BridgeLength', false, 'sum');
            var statecostTotal = jQuery("#tbSRRDALSBProposalList").jqGrid('getCol', 'StateShare', false, 'sum');
            var mordcostTotal = jQuery("#tbSRRDALSBProposalList").jqGrid('getCol', 'MordCost', false, 'sum');
            var higherSpecCost = jQuery("#tbSRRDALSBProposalList").jqGrid('getCol', 'HIGHER_SPECS', false, 'sum');

            var totalCost = jQuery("#tbSRRDALSBProposalList").jqGrid('getCol', 'TotalCost', false, 'sum');
            var stateShareCost = jQuery("#tbSRRDALSBProposalList").jqGrid('getCol', 'StateShareCost', false, 'sum');
            var mordShareCost = jQuery("#tbSRRDALSBProposalList").jqGrid('getCol', 'MordShareCost', false, 'sum');
            var totalStateShare = jQuery("#tbSRRDALSBProposalList").jqGrid('getCol', 'TotalStateShare', false, 'sum');

            jQuery("#tbSRRDALSBProposalList").jqGrid('footerData', 'set',
            {
                LSBName: 'Grand Total:',
                LSBLength: parseFloat(data.TotalColumn.TOT_PAV_LENGTH).toFixed(3),
                StateShare: parseFloat(data.TotalColumn.TOT_STATE_COST).toFixed(2),
                MordCost: parseFloat(data.TotalColumn.TOT_MORD_COST).toFixed(2),
                HIGHER_SPECS: parseFloat(data.TotalColumn.TOT_HIGHER_SPEC).toFixed(2),
                MaintenanceCost: parseFloat(data.TotalColumn.TOT_MANE_COST).toFixed(2),

                TotalCost: parseFloat(data.TotalColumn.TOT_COST).toFixed(2),
                StateShareCost: parseFloat(data.TotalColumn.STATE_SHARE_COST).toFixed(2),
                MordShareCost: parseFloat(data.TotalColumn.MORD_SHARE_COST).toFixed(2),
                TotalStateShare: parseFloat(data.TotalColumn.TOTAL_STATE_SHARE).toFixed(2),
                //StateShareCost: parseFloat(stateShareCost).toFixed(2),
                //MordShareCost: parseFloat(mordShareCost).toFixed(2),
                //TotalStateShare: parseFloat(totalStateShare).toFixed(2),
            });



        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                //window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert("Session Timeout !!!");
                alert('Error occurred');
            }
        },
        beforeSelectRow: function (rowid, e) {

            var $link = $('a', e.target);

            if (e.target.tagName.toUpperCase() === "A" || $link.length > 0) {

                $('#tbLSBProposalList').jqGrid('setSelection', rowid);
                // link exist in the item which is clicked
                return false;
            }
            return true;
        }
    }); //end of grid


    jQuery("#tbSRRDALSBProposalList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
          { startColumnName: 'FundShareRatio', numberOfColumns: 5, titleText: 'Fund Sharing' },
        ]
    });
}


//Edit Other Details for LSB
function BuildingUpdate(id) {




    $("#accordion div").html("");
    $("#accordion h3").html(
                    "<a href='#' style= 'font-size:.9em;' >Building Proposal Update</a>" +
                    '<a href="#" style="float: right;">' +
                    '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
                    );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load("/BuildingProposal/BuildingEdit/" + id, function () {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });

        $("#divProposalForm").css('height', 'auto');
        $('#divProposalForm').show('slow');
    });

    $("#tbBuildingProposalList").jqGrid('setGridState', 'hidden');


}




//-------------------------------- SRRDA Details Ends Here --------------------------------//



function FormatColumnShift1(cellvalue, options, rowObject)
{
    return "<center><table><tr><td  style='border:none'><a href='#' title='Shift Details' onClick ='ShiftProposalDetails(\"" + cellvalue.toString() + "\");' >Shift</a></td></tr></table></center>";
}

function FormatColumnShift11(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none'><a href='#' title='Shift Details' onClick ='ShiftProposalDetails(\"" + cellvalue.toString() + "\");' >Shift</a></td></tr></table></center>";
}



function ShiftProposalDetails(parameter) {

    var id = parameter;

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#dvShiftProposal').empty();

    $("#dvShiftProposal").load("/Proposal/GetShiftDialogBox/" + id, function () {
        $("#dvShiftProposal").dialog('open');
        $.unblockUI();
    })
}

