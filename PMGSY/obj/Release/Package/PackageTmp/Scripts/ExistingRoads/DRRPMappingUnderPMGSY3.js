$(document).ready(function () {

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $("#dvBlockUpdate").dialog({
        autoOpen: false,
        height: '130',
        width: "650",
        modal: true,
        title: 'Proposal Block Updation'
    });

    $("#idFilterDiv").click(function () {

        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divFilterForm").toggle("slow");

    });

    $("#btnListProposal").click(function () {
        LoadProposalsForUpdate($("#ddlImsYear").val(), $("#ddlDistrict").val(), $("#ddlMastBlockCode").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlImsConnectivity").val());
    });

});
function LoadProposalsForUpdate(IMS_YEAR, MAST_DISTRICT_ID, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_UPGRADE_CONNECT) {

    blockPage();
    jQuery("#tbUpdateProposalList").jqGrid('GridUnload');
    jQuery("#tbUpdateProposalList").jqGrid({
        url: '/ExistingRoads/GetProposalsForDRRPMappingUnderPMGSY3',
        datatype: "json",
        mtype: "POST",
        colNames: ['State', "District", "Block", "Work Name", "Package", "Pavement Length (in Kms.)", "Sanctioned Year", "Upgrade / New", 'Mapped DRRP -II Name','Map DRRP / MAPPED DRRP-II','Finalize'],
        colModel: [
                    { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', width: 120, sortable: false, align: "left" },
                    { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', width: 120, sortable: false, align: "left" },
                    { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', width: 120, sortable: false, align: "left" },
                    { name: 'WORK_NAME', index: 'WORK_NAME', width: 300, sortable: false, align: "left" },
                    { name: 'PACKAGE', index: 'PACKAGE', width: 120, sortable: false, align: "center" },
                    { name: 'IMS_PAV_LENGTH', index: 'IMS_PAV_LENGTH', width: 120, sortable: false, align: "center" },
                    { name: 'SANCTIONED_YEAR', index: 'SANCTIONED_YEAR', width: 120, sortable: false, align: "center" },
                    { name: 'IMS_UPGRADE_CONNECT', index: 'IMS_UPGRADE_CONNECT', width: 120, sortable: false, align: "center" },
                    { name: 'DRRP_NAME_MAPPED_UNDER_PMGSY_3', index: 'DRRP_NAME_MAPPED_UNDER_PMGSY_3', width: 200, sortable: false, align: "center" },
                    { name: 'Edit', index: 'Edit', width: 150, sortable: false },
                    { name: 'Change', index: 'Change', width: 50, sortable: false },
 // { name: 'ChangeBlock', index: 'ChangeBlock', width: 60, sortable: false, hidden: ($("#State").val() == 29 ? false : true) }
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "MAST_BLOCK_ID": MAST_BLOCK_CODE, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_UPGRADE_CONNECT": IMS_UPGRADE_CONNECT },
        pager: jQuery('#dvUpdateProposalListPager'),
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;DRRP II PMGSY I Mapping",
        height: 'auto',
        width: 'auto',
        rowList: [15, 30, 45],
        rowNum: 15,
        shrinkToFit: false,
        autowidth: false,
        sortname: 'WORK_NAME',
        rownumbers: true,
        loadComplete: function (data) {
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
        },
    });
}
function MapDRRP(IMS_PR_ROAD_CODE) {
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        $("#divProposalUpdateForm").load("/ExistingRoads/MapDRRPToProposalsUnderPMGSY3/" + IMS_PR_ROAD_CODE, function () {
            jQuery("#tbUpdateProposalList").trigger("reloadGrid")
            unblockPage();
        });

        $('#divProposalUpdateForm').show('slow');
        $("#divProposalUpdateForm").css('height', 'auto');
        $("#tbUpdateProposalList").jqGrid('setGridState', 'hidden');
    });
}
function FinalizeDRRP(IMS_PR_ROAD_CODE) {


    if (confirm("After finalization you can't do further changes. Are you sure to finalize Details ?")) {
        $.ajax({
            url: '/ExistingRoads/FinalizeDRRPDetails?id=' + IMS_PR_ROAD_CODE,
            type: "POST",
            cache: false,
            //data: { IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE },
            beforeSend: function () {
                $.blockUI({ message: null });
            },
            error: function (xhr, status, error) {
                $.unblockUI();
                alert("Error : " + error);
                return false;
            },
            success: function (response) {
                if (response.Success) {
                    alert("Details Finalized successfully");

                    jQuery("#tbUpdateProposalList").trigger("reloadGrid")
                    $('#divProposalUpdateForm').hide('slow');
        
                }
                else {
                    alert("DRRP is not mapped.  Details are not finalized.");
                }
                $.unblockUI();
            }
        });
    }

}

function DeFinalizeDRRP(IMS_PR_ROAD_CODE) {


    if (confirm("Are you sure to definalize Details ?")) {
        $.ajax({
            url: '/ExistingRoads/DeFinalizeDRRPDetails?id=' + IMS_PR_ROAD_CODE,
            type: "POST",
            cache: false,
            //data: { IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE },
            beforeSend: function () {
                $.blockUI({ message: null });
            },
            error: function (xhr, status, error) {
                $.unblockUI();
                alert("Error : " + error);
                return false;
            },
            success: function (response) {
                if (response.Success) {
                    alert("Details definalized successfully");

                    jQuery("#tbUpdateProposalList").trigger("reloadGrid")
                    $('#divProposalUpdateForm').hide('slow');
                }
                else {
                    alert("DRRP is not mapped.  Details are not finalized.");
                
                }
                $.unblockUI();
            }
        });
    }

}





function CloseDetails() {
    $("#tbUpdateProposalList").jqGrid('setGridState', 'visible');
    $('#accordion').hide('slow');
}
function UpdateBlockDetails(IMS_PR_ROAD_CODE) {

    $("#dvBlockUpdate").load('/Proposal/ProposalBlockUpdate/' + IMS_PR_ROAD_CODE);
    $("#dvBlockUpdate").dialog('open');
}