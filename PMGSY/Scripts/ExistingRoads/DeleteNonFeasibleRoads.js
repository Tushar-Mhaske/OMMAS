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



    $("#ddlState").change(function () {

        loadDistrict($("#ddlState").val());
       

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

        if ($("#ddlState").val() == 0) {
            alert("Please Select State, District, Block and Batch")
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

        if ($("#ddlImsBatch").val() == 0 || $("#ddlImsBatch").val() == -1) {
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
        url: '/ExistingRoads/GetNotFeasibleRoadsListForDeletion',
        datatype: "json",
        mtype: "POST",
        colNames: ["Rank", "District Name", "Block Name", "Road Number ", "Candidate Road Length", "Candidate Road Length Eligibility (should be greater than equal to 5 km)", "Eligible CUCPL Length", "Road Name", 'Request Remark and Reason', "Request For Not to be used in Proposal",'Delete'],
        colModel: [
                    { name: 'CUPL_RANK', index: 'CUPL_RANK', width: 70, sortable: false, align: "center" },
                    { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', width: 100, sortable: false, align: "left" },
                    { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', width: 100, sortable: false, align: "left" },
                    { name: 'PLAN_CN_ROAD_NUMBER', index: 'PLAN_CN_ROAD_NUMBER', width: 100, sortable: false, align: "left" },
                    { name: 'PLAN_RD_LENGTH', index: 'PLAN_RD_LENGTH', width: 100, sortable: false, align: "center" },
                    { name: 'PLAN_RD_LENGTH', index: 'PLAN_RD_LENGTH', width: 100, sortable: false, align: "center" },
                    { name: 'ELIGIBLE_CANDIDATE_LENGTH', index: 'ELIGIBLE_CANDIDATE_LENGTH', width: 120, sortable: false, align: "center" },
                    { name: 'PLAN_RD_NAME', index: 'PLAN_RD_NAME', width: 180, sortable: false, align: "left" },
                    { name: 'REQUEST_REMARKS', index: 'REQUEST_REMARKS', width: 220, sortable: false, align: "left" },
                    { name: 'Edit', index: 'Edit', width: 150, sortable: false, align: "center" },
                    { name: 'D', index: 'D', width: 150, sortable: false, align: "center" }
                    //{ name: 'a', index: 'a', formatter: deleteFormatter, width: 50, sortable: true, align: "center" }, DeleteExumptedDetails
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "MAST_BLOCK_ID": MAST_BLOCK_CODE, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_UPGRADE_CONNECT": IMS_UPGRADE_CONNECT },
        pager: jQuery('#dvUpdateProposalListPager1'),
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Exumpted Not Feasible Roads under PMGSY III",
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

function deleteFormatter(cellvalue, options, rowObject)
{
    return "<center><span style='border-color:white;cursor:pointer' class='ui-icon ui-icon-trash' title='Delete Details' onClick ='deleteExumptedRoads(\"" + cellvalue.toString() + "\");'></span></center>";
}

function DeleteExumptedDetails(urlparameter) {

    if (confirm("Are you sure you want to delete Details?")) {

        $.ajax({
            type: 'POST',
            url: '/ExistingRoads/DeleteTRMRLExemptedDetails/' + urlparameter,
            //url: '/ExistingRoads/DeleteTRMRLDetails/' + urlparameter,
            dataType: 'json',
         //   data: { roadCode: $("#EncryptedRoadCode").val() },
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert("Details deleted successfully ");
                    $("#tbUpdateProposalList").trigger('reloadGrid');
                  
                }
                else
                {
                    alert(data.message);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }
        });
    }
    else {
        return false;
    }

}
// DeleteExumptedDetails



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



function loadDistrict(statCode) {
    $("#ddlDistrict").val(0);
    $("#ddlDistrict").empty();
    $("#ddlMastBlockCode").val(0);
    $("#ddlMastBlockCode").empty();
    $("#ddlMastBlockCode").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#ddlDistrict").length > 0) {
            $.ajax({
                url: '/ExistingRoads/DistrictDetailsForDefinalizationPCI',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //For Disable if District Login
                    if ($("#MAST_DISTRICT_CODE").val() > 0) {
                        $("#ddlDistrict").val($("#MAST_DISTRICT_CODE").val());
                        // $("#DistrictList_StateListRoadDetails").attr("disabled", "disabled");
                        $("#ddlDistrict").trigger('change');
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
    else {
        $("#ddlDistrict").append("<option value='0'>All Districts</option>");
        $("#ddlMastBlockCode").empty();
        $("#ddlMastBlockCode").append("<option value='0'>All Blocks</option>");
    }
}