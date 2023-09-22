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
        url: '/Proposal/GetProposalsForCNMapping',
        datatype: "json",
        mtype: "POST",
        colNames: ['Block', "Batch", "Package Number", "Year", "Road Name", "Road / Bridge", "New / Upgradation", "Pavement Length (in Kms.) / Bridge Length (in Mtrs.)", 'Map Core Network', 'Change Core Network', 'Change Block'],
        colModel: [
                    { name: 'Block', index: 'Block', width: 80, sortable: false, align: "center" },
                    { name: 'IMS_BATCH', index: 'IMS_BATCH', width: 80, sortable: false, align: "center" },
                    { name: 'PackageNumber', index: 'PackageNumber', width: 100, sortable: false, align: "center" },
                    { name: 'IMS_YEAR', index: 'IMS_YEAR', width: 80, sortable: false, align: "center" },
                    { name: 'RoadName', index: 'RoadName', width: 300, sortable: false, align: "left" },
                    { name: 'RoadType', index: 'RoadType', width: 80, sortable: false, align: "center" },
                    { name: 'UpgradeConnect', index: 'UpgradeConnect', width: 80, sortable: false, align: "center" },
                    { name: 'PavementLength', index: 'PavementLength', width: 115, sortable: false, align: "right" },
                    { name: 'Edit', index: 'Edit', width: 60, sortable: false },
                    { name: 'Change', index: 'Change', width: 60, sortable: false },
                    { name: 'ChangeBlock', index: 'ChangeBlock', width: 60, sortable: false, hidden: ($("#State").val() == 29 ? false : true) }
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "MAST_BLOCK_ID": MAST_BLOCK_CODE, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_UPGRADE_CONNECT": IMS_UPGRADE_CONNECT },
        pager: jQuery('#dvUpdateProposalListPager'),
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Proposals For Update",
        height: 'auto',
        width: 'auto',
        rowList: [15, 30, 45],
        rowNum: 15,
        shrinkToFit: false,
        autowidth: false,
        sortname: 'IMS_ROAD_NAME',
        rownumbers: true,
        loadComplete: function (data) {
            unblockPage();

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
}
function MapCoreNetwork(IMS_PR_ROAD_CODE) {
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        $("#divProposalUpdateForm").load("/Proposal/MapCoreNetworkToProposals/" + IMS_PR_ROAD_CODE, function () {
            $.validator.unobtrusive.parse($('#divProposalUpdateForm'));
            unblockPage();
        });

        $('#divProposalUpdateForm').show('slow');
        $("#divProposalUpdateForm").css('height', 'auto');
        $("#tbUpdateProposalList").jqGrid('setGridState', 'hidden');
    });
}
function ChangeCoreNetwork(IMS_PR_ROAD_CODE) {
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        $("#divProposalUpdateForm").load("/Proposal/ProposalCoreNetworkUpdate/" + IMS_PR_ROAD_CODE, function () {
            $.validator.unobtrusive.parse($('#divProposalUpdateForm'));
            unblockPage();
        });

        $('#divProposalUpdateForm').show('slow');
        $("#divProposalUpdateForm").css('height', 'auto');
        $("#tbUpdateProposalList").jqGrid('setGridState', 'hidden');
    });
}
function CloseDetails() {
    $("#tbUpdateProposalList").jqGrid('setGridState', 'visible');
    $('#accordion').hide('slow');
}
function UpdateBlockDetails(IMS_PR_ROAD_CODE) {

    $("#dvBlockUpdate").load('/Proposal/ProposalBlockUpdate/' + IMS_PR_ROAD_CODE);
    $("#dvBlockUpdate").dialog('open');
}