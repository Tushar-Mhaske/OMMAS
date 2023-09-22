$(document).ready(function () {

    //$(function () {
    //    $("#accordion1").accordion({
    //        icons: false,
    //        heightStyle: "content",
    //        autoHeight: false
    //    });
    //});

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

    $("#ddlDistrict").change(function () {
        //alert("clicked")
        loadBlock($("#State").val(), $("#ddlDistrict").val());

    });


    $("#btnListProposal").click(function () {

        if ($("#ddlImsYear").val() == 0) {
            alert("Please Select Year")
            return false;
        }


        if ($("#ddlDistrict").val() == 0) {
            alert("Please Select District and Block")
            return false;
        }

        if ($("#ddlMastBlockCode").val() == 0) {
            alert("Please Select Block")
            return false;
        }
        // ddlImsBatch

        if ($("#ddlImsBatch").val() == 0) {
            alert("Please Select Batch")
            return false;
        }

        LoadProposalsForUpdate($("#ddlImsYear").val(), $("#ddlDistrict").val(), $("#ddlMastBlockCode").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlImsConnectivity").val());
    });

});
function LoadProposalsForUpdate(IMS_YEAR, MAST_DISTRICT_ID, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_UPGRADE_CONNECT) {

    blockPage();
    jQuery("#tbUpdateProposalList").jqGrid('GridUnload');
    jQuery("#tbUpdateProposalList").jqGrid({
        url: '/ExistingRoads/GetNotFeasibleRoadsList',
        datatype: "json",
        mtype: "POST",
        colNames: ["Rank", "District Name", "Block Name", "Road Number ", "Candidate Road Length", "Candidate Road Length Eligibility (should be greater than equal to 5 km)", "Eligible CUCPL Length", "Road Name", 'Request Remark and Reason', "Request For Not to be used in Proposal"],
        colModel: [
                    { name: 'CUPL_RANK', index: 'CUPL_RANK', width: 70, sortable: false, align: "center" },
                    { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', width: 100, sortable: false, align: "left" },
                    { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', width: 100, sortable: false, align: "left" },

                  { name: 'PLAN_CN_ROAD_NUMBER', index: 'PLAN_CN_ROAD_NUMBER', width: 100, sortable: false, align: "left" },
                    { name: 'PLAN_RD_LENGTH', index: 'PLAN_RD_LENGTH', width: 100, sortable: false, align: "center" },
                    { name: 'PLAN_RD_LENGTH', index: 'PLAN_RD_LENGTH', width: 100, sortable: false, align: "center" },
                    { name: 'ELIGIBLE_CANDIDATE_LENGTH', index: 'ELIGIBLE_CANDIDATE_LENGTH', width: 120, sortable: false, align: "center" },
                    { name: 'PLAN_RD_NAME', index: 'PLAN_RD_NAME', width: 180, sortable: false, align: "left" },
                  //  { name: 'SCORE_PER_UNIT_LENGTH', index: 'SCORE_PER_UNIT_LENGTH', width: 80, sortable: false, align: "center" },// "Utility Value"
                 //   { name: 'REQUEST_REMARKS', index: 'REQUEST_REMARKS', width: 120, sortable: false, align: "center" },  // , "Trace Map Minimum Rank",



                    { name: 'REQUEST_REMARKS', index: 'REQUEST_REMARKS', width: 220, sortable: false, align: "left" },
                    { name: 'Edit', index: 'Edit', width: 150, sortable: false, align: "center" },


                    //{ name: 'IMS_PAV_LENGTH', index: 'IMS_PAV_LENGTH', width: 120, sortable: false, align: "center" },
                    //{ name: 'SANCTIONED_YEAR', index: 'SANCTIONED_YEAR', width: 120, sortable: false, align: "center" },
                    //{ name: 'IMS_UPGRADE_CONNECT', index: 'IMS_UPGRADE_CONNECT', width: 120, sortable: false, align: "center" },
                    //{ name: 'DRRP_NAME_MAPPED_UNDER_PMGSY_3', index: 'DRRP_NAME_MAPPED_UNDER_PMGSY_3', width: 200, sortable: false, align: "center" },
                    //{ name: 'Edit', index: 'Edit', width: 150, sortable: false },
                    //{ name: 'Change', index: 'Change', width: 50, sortable: false },

 // { name: 'ChangeBlock', index: 'ChangeBlock', width: 60, sortable: false, hidden: ($("#State").val() == 29 ? false : true) }
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "MAST_BLOCK_ID": MAST_BLOCK_CODE, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_UPGRADE_CONNECT": IMS_UPGRADE_CONNECT },
        pager: jQuery('#dvUpdateProposalListPager1'),
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Not Feasible Roads under PMGSY III",
        height: 'auto',
        width: 'auto',
        rowList: [50, 100, 200, 300],
        rowNum: 50,
        shrinkToFit: false,
        autowidth: false,
        sortname: 'CUPL_RANK',
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
function MapDetails(Code) {

    // alert("Fine")

    $("#accordion1 div").html("");
    $("#accordion1 h3").html(
            "<a href='#' style= 'font-size:.9em;' >Mapping of Not Feasible Roads under PMGSY III</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseDetails();" /></a>'
            );

    $('#accordion1').show('fold', function () {
        blockPage();




        $("#divProposalUpdateForm1").load("/ExistingRoads/AddReason/" + Code, function () {
            jQuery("#tbUpdateProposalList").trigger("reloadGrid")
            unblockPage();
        });

        $('#divProposalUpdateForm1').show('slow');
        $("#divProposalUpdateForm1").css('height', 'auto');
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
                    $('#divProposalUpdateForm1').hide('slow');

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
                    $('#divProposalUpdateForm1').hide('slow');
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
    $('#accordion1').hide('slow');
}
function UpdateBlockDetails(IMS_PR_ROAD_CODE) {

    $("#dvBlockUpdate").load('/Proposal/ProposalBlockUpdate/' + IMS_PR_ROAD_CODE);
    $("#dvBlockUpdate").dialog('open');
}


function loadBlock(stateCode, districtCode) {

    //alert("State = " + stateCode);
    //alert("districtCode " + districtCode);
    $("#ddlMastBlockCode").val(0);
    $("#ddlMastBlockCode").empty();

    if (districtCode > 0) {
        if ($("#ddlMastBlockCode").length > 0) {
            $.ajax({
                url: '/ExistingRoads/BlockDetailsForNotFeasibleRoads',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlMastBlockCode").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#ddlMastBlockCode").val() > 0) {
                        $("#ddlMastBlockCode").val($("#ddlMastBlockCode").val());
                        // $("#BlockList_HabitationClusterDetails").attr("disabled", "disabled");
                        //$("#BlockList_HabitationClusterDetails").trigger('change');
                    }
                    //$('#BlockList_HabitationClusterDetails').find("option[value='0']").remove();
                    //$("#BlockList_HabitationClusterDetails").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_HabitationClusterDetails').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#ddlMastBlockCode").append("<option value='0'>All Blocks</option>");
    }
}