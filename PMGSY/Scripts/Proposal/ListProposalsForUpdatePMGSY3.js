$(document).ready(function () {

    $(function () {
        //$("#accordion").accordion({
        //    icons: false,
        //    heightStyle: "content",
        //    autoHeight: false
        //});
    });

    $("#dvPIUUpdate").dialog({
        autoOpen: false,
        height: '130',
        width: "370",
        modal: true,
        title: 'Proposal PIU Updation'
    });

    $("#dvBlockUpdate").dialog({
        autoOpen: false,
        height: '130',
        width: "370",
        modal: true,
        title: 'Proposal Block Updation'
    });

    $("#idFilterDiv").click(function () {

        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divFilterForm").toggle("slow");

    });

    $("#btnListProposal").click(function () {
        if ($("#ddlState").val() <= 0) {
            alert('Please select State');
            return false;
        }

        LoadProposalsForUpdate($("#ddlImsYear").val(), $("#ddlDistrict").val(), $("#ddlMastBlockCode").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val(), $("#ddlImsConnectivity").val());
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


});
function LoadProposalsForUpdate(IMS_YEAR, MAST_DISTRICT_ID, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT) {
    blockPage();
    jQuery("#tbUpdateProposalList").jqGrid('GridUnload');
    jQuery("#tbUpdateProposalList").jqGrid({
        url: '/Proposal/GetProposalsForUpdatePMGSY3',
        datatype: "json",
        mtype: "POST",
        colNames: ['District', 'PIU', 'Block', "Package Number", "Year", "Road Name", "1000+", "999-500", "499-250", "Less Than 250", "Total Habitations", "Pavement Length (in Kms.)", "MoRD Share (in Lakhs)", "State Share (in Lakhs)", "Maintenance Cost (in Lakhs)", "Renewal Amount (in Lakhs)", "Higher Specification Cost (in Lakhs)", "Stage Construction", "STA (Scrutiny Date)", "PTA (Scrutiny Date)", "Status", "Edit", 'Stage I', 'Complete', 'Stage I To Stage II', 'Complete To Stage II', 'Change PIU', 'Change Block'],
        colModel: [
                    { name: 'District', index: 'District', width: 60, sortable: false, align: "left" },
                    { name: 'PIU', index: 'PIU', width: 100, sortable: false, align: "left" },
                    { name: 'Block', index: 'Block', width: 60, sortable: false, align: "left" },
                    { name: 'PackageNumber', index: 'PackageNumber', width: 70, sortable: false, align: "center" },
                    { name: 'IMS_YEAR', index: 'IMS_YEAR', width: 70, sortable: false, align: "center" },
                    { name: 'RoadName', index: 'RoadName', width: 200, sortable: false, align: "left" },
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
                    { name: 'PTA_SCRUTINY', index: 'PTA_SCRUTINY', width: 120, sortable: false, align: "left" },
                    { name: 'PROPOSAL_STATUS', index: 'PROPOSAL_STATUS', width: 60, sortable: false, align: "center", hidden: true },
                    { name: 'Edit', index: 'Edit', width: 60, sortable: false, align: 'center' },
                    { name: 'StageI', index: 'StageI', width: 60, sortable: false, align: 'center' },
                    { name: 'Complete', index: 'Complete', width: 60, sortable: false, align: 'center' },
                    { name: 'Stage1ToStage2', index: 'Stage1ToStage2', width: 60, sortable: false, align: 'center' },
                    { name: 'CompleteToStage2', index: 'CompleteToStage2', width: 60, sortable: false, align: 'center', hidden: true },
                    //{ name: 'ChangePIU', index: 'ChangePIU', width: 60, sortable: false, align: 'center', hidden: (MAST_DISTRICT_ID <= 0 ? true : false) },
                    { name: 'ChangePIU', index: 'ChangePIU', width: 60, sortable: false, align: 'center', hidden: true },
                    { name: 'ChangeBLOCK', index: 'ChangeBLOCK', width: 60, sortable: false, align: 'center', hidden: (MAST_DISTRICT_ID <= 0 ? true : false) },
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "MAST_BLOCK_ID": MAST_BLOCK_CODE, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_PROPOSAL_STATUS": IMS_PROPOSAL_STATUS, "IMS_UPGRADE_CONNECT": IMS_UPGRADE_CONNECT, "MAST_STATE_CODE": $("#ddlState").val() },
        pager: jQuery('#dvUpdateProposalListPager'),
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Proposals For Update",
        height: 'auto',
        width: 'auto',
        rowList: [15, 30, 45],
        rowNum: 15,
        shrinkToFit: false,
        autowidth: true,
        sortname: 'Block',
        rownumbers: true,
        footerrow: true,
        loadComplete: function (data) {
            unblockPage();
            //$("#dvUpdateProposalListPager_left").html("<input type='button' style='margin-left:27px' id='btnChangePIU' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'ChangePIU();return false;' value='Change PIU'/>")
            $("#dvUpdateProposalListPager_left").append("<input type='button' style='margin-left:5px; display:none;' id='btnChangeBlock' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'ChangeBlock();return false;' value='Change Block'/>")
        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                //window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert("Session Timeout !!!");
                alert('Error occurred');
            }
        },
    });

    jQuery("#tbUpdateProposalList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
          { startColumnName: 'StageI', numberOfColumns: 4, titleText: 'Change Status To' },
        ]
    });
}
function EditDetails(IMS_PR_ROAD_CODE) {
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

       // alert("Edit Proposal PMGSY III")

        $("#divProposalUpdateForm").load("/Proposal/UpdateProposalDetailsPMGSY3/" + IMS_PR_ROAD_CODE, function () {
            $.validator.unobtrusive.parse($('#divProposalUpdateForm'));
            unblockPage();
        });

        $('#divProposalUpdateForm').show('slow');
      //  $("#divProposalUpdateForm").css('height', 'auto');
       // $("#tbUpdateProposalList").jqGrid('setGridState', 'hidden');
    });
}
function CloseDetails() {
    $("#tbUpdateProposalList").jqGrid('setGridState', 'visible');
    $('#accordion').hide('slow');
}
function ChangeCompleteProposalToStaged(PROPOSAL_CODE) {
    $.ajax({
        url: '/Proposal/ChangeCompleteProposalToStaged/' + PROPOSAL_CODE,
        type: "POST",
        cache: false,
        beforeSend: function () {
            blockPage();
        },
        error: function (xhr, status, error) {
            unblockPage();
            Alert("Request can not be processed at this time,please try after some time!!!");
            return false;
        },
        success: function (response) {

            if (response.success == true) {
                alert("Proposal Updated Successfully.");
                $("#tbUpdateProposalList").trigger('reloadGrid');
            }
            else {
                if (response.errorMessage != "" || response.errorMessage != undefined || response.errorMessage != null) {
                    alert(response.errorMessage)
                }
                else {
                    alert("Error Occured while processing your request.");
                }
            }
            unblockPage();
        }
    });
}
function ChangeStagedProposalToComplete(PROPOSAL_CODE) {
    $.ajax({
        url: '/Proposal/ChangeStagedProposalToComplete/' + PROPOSAL_CODE,
        type: "POST",
        cache: false,
        beforeSend: function () {
            blockPage();
        },
        error: function (xhr, status, error) {
            unblockPage();
            Alert("Request can not be processed at this time,please try after some time!!!");
            return false;
        },
        success: function (response) {

            if (response.success == true) {
                alert("Proposal Updated Successfully.");
                $("#tbUpdateProposalList").trigger('reloadGrid');
            }
            else {
                if (response.errorMessage != "" || response.errorMessage != undefined || response.errorMessage != null) {
                    alert(response.errorMessage)
                }
                else {
                    alert("Error Occured while processing your request.");
                }
            }
            unblockPage();
        }
    });
}
function ChangeStage1ProposalToStage2(PROPOSAL_CODE) {
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        $("#divProposalUpdateForm").load("/Proposal/ChangeStage1ProposalToStage2/" + PROPOSAL_CODE, function () {
            $.validator.unobtrusive.parse($('#divProposalUpdateForm'));
            unblockPage();
        });

        $('#divProposalUpdateForm').show('slow');
        $("#divProposalUpdateForm").css('height', 'auto');
        $("#tbUpdateProposalList").jqGrid('setGridState', 'hidden');
    });
}
function ChangeCompleteProposalToStage2(PROPOSAL_CODE) {
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        $("#divProposalUpdateForm").load("/Proposal/ChangeCompleteProposalToStage2/" + PROPOSAL_CODE, function () {
            $.validator.unobtrusive.parse($('#divProposalUpdateForm'));
            unblockPage();
        });

        $('#divProposalUpdateForm').show('slow');
        $("#divProposalUpdateForm").css('height', 'auto');
        $("#tbUpdateProposalList").jqGrid('setGridState', 'hidden');
    });
}
var ProposalArray = [];
//function ChangePIU() {
//    ProposalArray = [];

//    $('input:checkbox.piuupdate').each(function () {
//        var sThisVal = (this.checked ? ProposalArray.push($(this).val()) : "");
//    });

//    if (ProposalArray.length == 0) {
//        alert('Please select Proposal.');
//        return false;
//    }

//    var postData = $("#tbUpdateProposalList").jqGrid('getGridParam', 'postData');

//    $("#dvPIUUpdate").load('/Proposal/ProposalPIUUpdate?id=' + ProposalArray + "&district=" + postData.MAST_DISTRICT_ID);
//    $("#dvPIUUpdate").dialog('open');
//}

function ChangePIU(RoadCode) {
    //alert(Package_Id);
    ProposalArray = [];

    var postData = $("#tbUpdateProposalList").jqGrid('getGridParam', 'postData');


  //  $("#dvPIUUpdate").load('/Proposal/ProposalPIUUpdate?id=' + Package_Id + "&district=" + postData.MAST_DISTRICT_ID);
    $("#dvPIUUpdate").load('/Proposal/ProposalPIUUpdate?district=' + postData.MAST_DISTRICT_ID + "&id=" + RoadCode);   // change 
    $("#dvPIUUpdate").dialog('open');
}

var ProposalArrayBlock = [];
function ChangeBlock() {
    ProposalArrayBlock = [];

    $('input:checkbox.blockupdate').each(function () {
        var sThisVal = (this.checked ? ProposalArrayBlock.push($(this).val()) : "");
    });

    if (ProposalArrayBlock.length == 0) {
        alert('Please select Proposal for change Block.');
        return false;
    }

    var postData = $("#tbUpdateProposalList").jqGrid('getGridParam', 'postData');

    $("#dvBlockUpdate").load('/Proposal/ProposalUpdateBlock?id=' + ProposalArrayBlock + "&district=" + postData.MAST_DISTRICT_ID);
    $("#dvBlockUpdate").dialog('open');

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
            $("#ddlImsStreams").empty();
            for (var i = 0; i < jsonData.length; i++) {
                if (jsonData[i].Selected == true) {
                    $("#ddlImsStreams").append("<option value='" + jsonData[i].Value + "' selected=true>" + jsonData[i].Text + "</option>");
                }
                else {
                    $("#ddlImsStreams").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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