$(document).ready(function () {
    blockPage();

    LoadProposals($("#ddlImsYear option:selected").val(), $("#ddlMastBlockCode option:selected").val(), $("#ddlImsPackages option:selected").val(), $("#ddlImsProposalTypes option:selected").val());

    unblockPage();

    $("#idFilterDiv").click(function () {           

        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divFilterForm").toggle("slow");

    });

    $("#btnListProposal").click(function () {
        blockPage();
        SearchProposalList();

        CloseProposalDetails();
        $("#divProposalForm").hide("slow");


        unblockPage();
    });//end Search

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $("#ddlImsYear").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlImsYear").find(":selected").val() },
                    "#ddlImsPackages", "/Execution/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlImsYear option:selected').val() + "&blockCode=" + $('#ddlMastBlockCode option:selected').val());
    });

    $("#ddlMastBlockCode").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlMastBlockCode").find(":selected").val() },
                    "#ddlImsPackages", "/Execution/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlImsYear option:selected').val() + "&blockCode=" + $('#ddlMastBlockCode option:selected').val());
    });


});


//not required
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

function showFilter()
{    
    if ($('#divFilterForm').is(":hidden")) {
        $("#divFilterForm").show("slow");
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    }
}

function CloseProposalDetails() {
    $('#accordion').hide('slow');
    $('#divProposalForm').hide('slow');
    $("#tbProposalList").jqGrid('setGridState', 'visible');
    showFilter();
}

//show the Details of Road Proposal
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

// Jqgrid of Proposals
function LoadProposals(IMS_YEAR, MAST_BLOCK_CODE, IMS_PACKAGE, IMS_PROPOSAL_TYPE) {
        jQuery("#tbProposalList").jqGrid({
                url: '/Execution/GetProposalList',
            datatype: "json",
            mtype: "POST",
            colNames: ['Block','Year','Batch', "Package Number", "Proposal Type", "Road / LSB Name", "Road Length (in Kms) / LSB Length (in Mtrs)", "Sanctioned Amount (in Lacs)", "Work Program", "Payment Shedule"],
            colModel: [
                                { name: 'Block', index: 'Block', width: 70, sortable: true, align: "center" },
                                { name: 'Year', index: 'Year', width: 70, sortable: true, align: "center" },
                                { name: 'Batch', index: 'Batch', width: 70, sortable: true, align: "center" },
                                { name: 'PackageNumber', index: 'PackageNumber', width: 70, sortable: true, align: "center" },
                                { name: 'ProposalType', index: 'ProposalType', width: 70, sortable: false, align: "center" },
                                { name: 'RoadBridgeName', index: 'RoadBridgeName', width: 250, sortable: false, align: "center" },
                                { name: 'RoadBridgeLength', index: 'RoadBridgeLength', width: 100, sortable: false, align: "center" },
                                { name: 'SanctionedAmount', index: 'SanctionedAmount', width: 100, sortable: false, align: "center" },
                                { name: 'WorkProgram', width: 70, sortable: false, resize: false, formatter: FormatColumnWorkProgram, align: "center", sortable: false, search: false },
                                { name: 'PaymentShedule', width: 70, sortable: false, resize: false, formatter: FormatColumnPaymentShedule, align: "center", sortable: false, search: false },

            ],
            postData: { "IMS_YEAR": IMS_YEAR, "MAST_BLOCK_CODE": MAST_BLOCK_CODE, "IMS_PACKAGE": IMS_PACKAGE, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE },
            pager: jQuery('#dvProposalListPager'),
            rowNum: 10,
            sortname: 'Block',
            sortorder:'asc',
            rowList: [5, 10, 15, 20],
            viewrecords: true,
            recordtext: '{2} records found',
            caption: "&nbsp;&nbsp;Execution Details",
            height: 'auto',
            autowidth: true,
            rownumbers: true,
            pginput:true,
            loadComplete: function () {                
                unblockPage();
            },
            loadError: function (xhr, status, error) {

                if (xhr.responseText == "session expired") {
                    alert(xhr.responseText);
                    window.location.href = "/Login/Login";
                }
                else {
                    alert("Session Timeout !!!");
                    window.location.href = "/Login/LogIn";
                }
            }
            }); //end of grid
}

//Proposal grid fromat column
function FormatColumnWorkProgram(cellvalue, options, rowObject) {
    return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-plusthick ui-align-center' title='Work Program' onClick ='WorkProgram(\"" + cellvalue.toString() + "\");'></span></center>";
}

function FormatColumnPaymentShedule(cellvalue, options, rowObject) {
    return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-plusthick ui-align-center' title='Payment Shedule' onClick ='PaymentShedule(\"" + cellvalue.toString() + "\");'></span></center>";
}

function WorkProgram(urlparamater) {

    jQuery('#tbProposalList').jqGrid('setSelection', urlparamater);

    $("#accordion div").html("");

    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Schedule of Work Program</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/Execution/WorkProgramAddEdit/' + urlparamater, function () {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            $("#btnReset").trigger("click");
            unblockPage();
        });
        $("#tbProposalList").jqGrid('setGridState', 'hidden');
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });
}

function PaymentShedule(urlparamater) {

    jQuery('#tbProposalList').jqGrid('setSelection', urlparamater);

    $("#accordion div").html("");

    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Schedule of Payment</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/Execution/PaymentScheduleAddEdit/' + urlparamater, function () {
            //$.validator.unobtrusive.parse($('#frmPaymentSchedule'));
            unblockPage();
        });
        $("#tbProposalList").jqGrid('setGridState', 'hidden');
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });


    


}

//this function is used to close Data entry form and show the Proposal Grid
function CloseProposalDetails() {
    $('#accordion').hide('slow');
    $('#divProposalForm').hide('slow');
    $("#tbProposalList").jqGrid('setGridState', 'visible');
    showFilter();
}

function SearchProposalList() {
    var IMS_YEAR = $("#ddlImsYear option:selected").val();
    var MAST_BLOCK_CODE = $("#ddlMastBlockCode option:selected").val();
    //var IMS_BATCH = $("#ddlImsBatch option:selected").val();
    //var IMS_STREAM = $("#ddlImsStreams option:selected").val();
    var IMS_PACKAGE = $("#ddlImsPackages option:selected").val();
    var IMS_PROPOSAL_TYPE = $("#ddlImsProposalTypes option:selected").val();

    $('#tbProposalList').setGridParam({
        url: '/Execution/GetProposalList', datatype: 'json'
    });
    $('#tbProposalList').jqGrid("setGridParam", { "postData": { IMS_YEAR: IMS_YEAR, MAST_BLOCK_CODE: MAST_BLOCK_CODE, IMS_PACKAGE: IMS_PACKAGE, IMS_PROPOSAL_TYPE: IMS_PROPOSAL_TYPE, batch: $('#ddlBatchs option:selected').val(), collaboration: $('#ddlCollaborations option:selected').val(), upgradationType: $('#ddlUpgradations option:selected').val() } });
    $('#tbProposalList').trigger("reloadGrid", [{ page: 1 }]);
}
function FillInCascadeDropdown(map, dropdown, action) {
    var message = '';

    $(dropdown).empty();

    $.post(action, map, function (data) {
        $.each(data, function () {

            if (this.Selected == true) {
                $(dropdown).append("<option selected value=" + this.Value + ">" + this.Text + "</option>");
            }
            else {
                $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            }
        });

    }, "json");
}