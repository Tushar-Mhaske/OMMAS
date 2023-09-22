/// <reference path="../jquery-1.9.1.js" />
/// <reference path="../jquery-1.9.1-vsdoc.js" />
var isDroppedArrBefore = [];
var isDroppedArrAfter = [];
$(document).ready(function () {

    $("#idFilterDiv").click(function () {

        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divFilterForm").toggle("slow");

    });
    blockPage();

    if ($("#ddlState").is(":visible")) {
        if ($("#RoleID").val() != 3) {
            $("#ddlState").val(0);
        }
    }

    if ($("#RoleID").val() == '2' || $("#RoleID").val() == '37' || $("#RoleID").val() == '55' || $("#RoleID").val() == '8' || $("#RoleID").val() == '48' || $("#RoleID").val() == '69') {            //SRRDA - listing Only

        $("#divSRRDAProposal").show();
        $("#btnAddProposal").hide();
        LoadSRRDAProposals($("#ddlImsYear").val(), $("#ddlDistrict").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val(), $("#ddlImsConnectivity").val());
    }

    unblockPage();

    $("#btnListProposal").click(function () {

        if ($('#FilterForm').valid()) {

            blockPage();
            if ($("#RoleID").val() == '2' || $("#RoleID").val() == '37' || $("#RoleID").val() == '55' || $("#RoleID").val() == '8' || $("#RoleID").val() == '48' || $("#RoleID").val() == '69') {            //SRRDA - listing Only

                LoadSRRDAProposals($("#ddlImsYear").val(), $("#ddlDistrict").val(), $("#ddlMastBlockCode").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val(), $("#ddlImsConnectivity").val());
            }

            unblockPage();
        }

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

    //setTimeout(function () {
    //    $('#btnListProposal').trigger('click');
    //}, 300);


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

    $("#tbSRRDAProposalList").jqGrid('setGridState', 'visible');
    $("#tbSRRDALSBProposalList").jqGrid('setGridState', 'visible');


    showFilter();
}


function LoadSRRDAProposals(IMS_YEAR, MAST_DISTRICT_ID, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT) {

    if (IMS_PROPOSAL_TYPE == "P" || IMS_PROPOSAL_TYPE == "L") { ///Changes by SAMMED A. PATIL on 02JAN2018 for Bridge Proposal
        $("#tbSRRDAProposalList").show();
        $("#dvSRRDAProposalListPager").show();
        $('#tbSRRDAProposalList').jqGrid('GridUnload');

        SRRDARoadProposalGrid(IMS_YEAR, MAST_DISTRICT_ID, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
    }

}
//-------------------------------   SRRDA Dropping Proposal Details --------------------------------------//
function SRRDARoadProposalGrid(IMS_YEAR, MAST_DISTRICT_ID, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT) {
    blockPage();

    jQuery("#tbSRRDAProposalList").jqGrid({
        url: '/Proposal/GetDroppingProposalsForSRRDA',
        datatype: "json",
        mtype: "POST",
        colNames: ['District', 'Block', "Package Number", "Year", "Road Name", "1000+", "999-500", "499-250", "Less Than 250", "Total Habitations", "Pavement Length (in Kms.)", "MoRD Share (in Lakhs)", $('#PMGSYScheme').val() == 1 ? "State Share (in Lakhs)" : "State Share Excluding Higher Specification (in Lakhs)", "Higher Specification Cost (in Lakhs)", "Total Cost", "Maintenance Cost (in Lakhs)", "Renewal Amount (in Lakhs)", "Fund Sharing Ratio", "State Share Cost (in Lakhs)", "Mord Share Cost (in Lakhs)", "Total State Share (in Lakhs)", "Total Share Cost (in Lakhs)", "Stage Construction", "STA (Scrutiny Date)", "PTA (Scrutiny Date)", "Status", "Admin Nd Code", "Expenditure Incurred (Rs. in Lakhs)", "View", "Proposed For Dropping " /* "Upload"*/],
        colModel: [
                    { name: 'District', index: 'District', width: 100, sortable: false, align: "center" },
                    { name: 'Block', index: 'Block', width: 100, sortable: false, align: "center" },
                    { name: 'PackageNumber', index: 'PackageNumber', width: 80, sortable: false, align: "center" },
                    { name: 'IMS_YEAR', index: 'IMS_YEAR', width: 70, sortable: false, align: "center" },
                    { name: 'RoadName', index: 'RoadName', width: 180, sortable: false, align: "left" },
                    { name: 'Hab1000', index: 'Hab1000', width: 50, sortable: false, align: "right" },
                    { name: 'Hab999', index: 'Hab999', width: 50, sortable: false, align: "right" },
                    { name: 'Hab499', index: 'Hab499', width: 50, sortable: false, align: "right" },
                    { name: 'Hab250', index: 'Hab250', width: 50, sortable: false, align: "right" },
                    { name: 'HabTotal', index: 'HabTotal', width: 50, sortable: false, align: "right" },
                    { name: 'PavementLength', index: 'PavementLength', width: 50, sortable: false, align: "right" },
                    { name: 'PavementCost', index: 'PavementCost', width: 60, sortable: false, align: "right" },
                    { name: 'StateCost', index: 'StateCost', width: 60, sortable: false, align: "right" },
                    { name: 'HIGHER_SPECS', index: 'HIGHER_SPECS', width: 60, sortable: false, align: "right", hidden: ($("#PMGSYScheme").val() == 1 ? true : false) },
                    { name: 'TotalCost', index: 'TotalCost', width: 60, sortable: false, align: "right" },
                    { name: 'MAINT_AMT', index: 'MAINT_AMT', width: 60, sortable: false, align: "right" },
                    { name: 'RENEWAL_AMT', index: 'RENEWAL_AMT', width: 60, sortable: false, align: "right", hidden: ($("#PMGSYScheme").val() == 1 ? true : false) },
                    { name: 'FundShareRatio', index: 'FundShareRatio', width: 60, sortable: false, align: "right", },
                    { name: 'StateShareCost', index: 'StateShareCost', width: 60, sortable: false, align: "right", },
                    { name: 'MordShareCost', index: 'MordShareCost', width: 60, sortable: false, align: "right", },
                    { name: 'TotalStateShare', index: 'TotalStateShare', width: 60, sortable: false, align: "right", },
                    { name: 'TotalShareCost', index: 'TotalShareCost', width: 60, sortable: false, align: "right", hidden: true },

                    { name: 'STAGE_CONST', index: 'STAGE_CONST', width: 90, sortable: false, align: "center" },
                    { name: 'STA_SCRUTINY', index: 'STA_SCRUTINY', width: 120, sortable: false, align: "left", hidden: true },
                    { name: 'PTA_SCRUTINY', index: 'PTA_SCRUTINY', width: 120, sortable: false, align: "left", hidden: true },
                    { name: 'PROPOSAL_STATUS', index: 'PROPOSAL_STATUS', width: 60, sortable: false, align: "center", hidden: true },

                    { name: 'AdmminNdCode', index: 'AdmminNdCode', width: 60, sortable: false, align: "center", hidden: true },
                    { name: 'EXP_INCURRED', index: 'EXP_INCURRED', width: 90, sortable: false, align: "right" },
                    { name: 'ShowDetails', index: 'ShowDetails', width: 50, sortable: false, align: "center", search: false },
                    { name: 'DropProposal', index: 'DropProposal', width: 50, sortable: false, align: "center", search: false },
                  //  { name: 'UploadDetails', index: 'UploadDetails', width: 50, sortable: false, align: "center", search: false },
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "MAST_BLOCK_CODE": MAST_BLOCK_CODE, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_PROPOSAL_STATUS": IMS_PROPOSAL_STATUS, "IMS_UPGRADE_CONNECT": IMS_UPGRADE_CONNECT, __RequestVerificationToken: $('#FilterForm input[name=__RequestVerificationToken]').val() },
        pager: jQuery('#dvSRRDAProposalListPager'),
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Road Proposals For Dropping",
        height: '400px',
        width: 'auto',
        rowList: [50, 100, 150, 200],
        rowNum: 50,
        autowidth: true,
        sortname: 'District',
        rownumbers: true,
        footerrow: true,
        shrinkToFit: false,
        //multiboxonly: true,
        //multiselect: true,
        //beforeSelectRow: function(rowid, e)
        //{
        //    jQuery("#tbSRRDAProposalList").jqGrid('resetSelection');
        //    return(true);
        //},
        gridComplete: function () {

        },
        loadComplete: function (data) {

            isDroppedArrBefore.splice(0, isDroppedArrBefore.length);  //remove previous data of selected proposal

            $.each($("input[name='isDropped']:checked"), function (i, value) {
                // isDroppedArrBefore[i] = $(this).val().trim()
                // alert(isDroppedArrBefore[i]);
            });
            console.log($("input[name='isDropped']:checked"));

            $("input[name='isDropped']:not(:disabled)").change(function () {
                debugger;

                if ($(this).is(":checked"))    //
                {
                    $("input[name='isDropped']:not(:disabled)").attr("checked", false);
                    $(this).attr("checked", true);
                    isDroppedArrBefore.splice(0, isDroppedArrBefore.length);
                    isDroppedArrBefore.push($(this).val());

                }
                else {
                    if ($.inArray($(this).val(), isDroppedArrBefore)) {
                        isDroppedArrBefore.splice($.inArray($(this).val(), isDroppedArrBefore), 1);
                    }
                }
            })
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

            //Added By Pradip patil 10/04/2017 

            //$("#dvSRRDAProposalListPager_left").html("<input type='button' style='margin-left:40px' id='btnSubmit' title='Save Dropping Proposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'SubmitDroppedProposal();return false;' value='Submit'/>");

            $("#dvSRRDAProposalListPager_left").html("<input type='button' style='margin-left:40px' id='btnSubmit' title='Save Dropping Proposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddDroppedProposal();' value='Submit'/>");

            // End Here
            unblockPage();
        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                //alert(error);
                window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert(error);
                window.location.href = "/Login/SessionExpire";
            }
        },
        beforeSelectRow: function (rowid, e) {

            var $link = $('a', e.target);
            if (e.target.tagName.toUpperCase() === "A" || $link.length > 0) {
                $(this).jqGrid('setSelection', rowid);
                // link exist in the item which is clicked
                return false;
            }
            //$('.cbxDrop').prop('disabled', true);
            return true;
        }
    }); //end of grid

    jQuery("#tbSRRDAProposalList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
          { startColumnName: 'FundShareRatio', numberOfColumns: 5, titleText: 'Fund Sharing' },
        ]
    });

    //var myGrid = $("#tbSRRDAProposalList");
    //$("#jqg_" + myGrid[0].id).hide();
}

//show the Details of Road Proposal
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


    $("#tbSRRDAProposalList").jqGrid('setGridState', 'hidden');
    // $("#tbMORDLSBProposalList").jqGrid('setGridState', 'hidden');


    $('#idFilterDiv').trigger('click');

}


//===========================================================Dropped proposal  [by Pradip Patil(10/04/2017)]==========================================================

function non_duplicate(joined) {
    debugger;
    // diff = [];
    //joined = arr1.concat(arr2);
    for (i = 0; i <= joined.length; i++) {
        current = joined[i];
        if (joined.indexOf(current) == joined.lastIndexOf(current)) {
            diff.push(current);
        }
    }
    return diff.filter(Boolean);
}
Array.prototype.unique2 = function () {

    debugger;
    var n = {}, r = [];
    for (var i = 0; i < this.length; i++) {
        if (!n[this[i]]) {
            n[this[i]] = true;
            r.push(this[i]);
        }
    }
    return r;
}

$("#dvDropProposalModal").dialog({
    autoOpen: false,
    height: '300',
    width: "600",
    modal: true,
    title: 'Dropped Works'
});

function AddDroppedProposal() {
    //debugger;
    var droppedArr = [];
    var postData = jQuery('#tbSRRDAProposalList').jqGrid('getGridParam', 'postData');

    debugger;
    droppedArr = isDroppedArrBefore.unique2();
    $("input:checkbox[value='" + droppedArr[0] + "']").parent().trigger('click');
    var Selrowid = jQuery('#tbSRRDAProposalList').jqGrid('getGridParam', 'selrow');

    var exp = $('#tbSRRDAProposalList').jqGrid('getCell', Selrowid, 'EXP_INCURRED');
    //alert(exp)
    //alert(droppedArr[0]);
    if (droppedArr.length > 0) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/Proposal/AddWorksforDropping?roadCode=' + droppedArr[0],
            dataType: "html",
            async: false,
            cache: false,
            data: { expIncurred: exp },
            success: function (data) {
                isDroppedArrBefore.splice(0, isDroppedArrBefore.length); //remove the  already selected road in first transaction
                $("#dvDropProposalModal").html(data);
                $("#dvDropProposalModal").dialog('open');

                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                $.unblockUI();
            }
        })
    }
    else {
        alert('Please select work(s) for dropping.');
    }
}

function SubmitDroppedProposal() {
    var droppedArr = [];
    debugger;//:checked
    //$.each($("input[name='isDropped']:checked:not(:disabled)"), function (i, value) {
    //  isDroppedArrAfter[i] = $(this).val().trim();
    //  //alert(isDroppedArrAfter[i]);

    //});

    var postData = jQuery('#tbSRRDAProposalList').jqGrid('getGridParam', 'postData');

    // var array3 = isDroppedArrBefore.concat(isDroppedArrAfter);
    //droppedArr = $(isDroppedArrAfter).not(array3).get();
    //alert(droppedArr);
    //var union = non_duplicate(isDroppedArrBefore);
    droppedArr = isDroppedArrBefore.unique2();;

    var filterArray = [];
    filterArray.push(postData.IMS_YEAR)
    filterArray.push(postData.IMS_BATCH);
    filterArray.push(postData.IMS_STREAM);

    if (droppedArr.length > 0) {
        $.ajax({
            url: '/Proposal/DropProposal/',
            method: 'POST',
            cache: false,
            traditional: true,
            data: AddAntiForgeryToken({ droppedArray: droppedArr }),//, filter: filterArray 
            async: true,
            success: function (data) {
                alert(data.message);
                isDroppedArrBefore.splice(0, isDroppedArrBefore.length); //remove the  already selected road in first transaction
                $('#tbSRRDAProposalList').trigger('reloadGrid');
            },
            error: function () {
                alert('An Error occured while processing ypur request');
                isDroppedArrBefore.splice(0, isDroppedArrBefore.length);
            },
        })
    }
    else {
        alert('Please select work(s) for dropping.');
    }
}


AddAntiForgeryToken = function (data) {
    debugger;
    data.__RequestVerificationToken = $('#FilterForm input[name=__RequestVerificationToken]').val();
    return data;
};