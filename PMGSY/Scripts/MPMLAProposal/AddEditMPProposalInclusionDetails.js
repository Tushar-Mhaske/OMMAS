/* 
     *  Name : AddEditMPProposalInclusionDetails.js
     *  Path : ~/Scripts/MPMLAProposal/AddEditMPProposalInclusionDetails.js
     *  Description : AddEditMPProposalInclusionDetails.js is used to add MP Proposal Inclusion details
     *  Author : Abhishek Kamlble(PE, e-gov)
     *  Company : C-DAC,E-GOV
     *  Dates of Creation : 05/Jul/2013
 */
$(document).ready(function () {

    $.validator.unobtrusive.parse($('frmMPProposalInclusion'));

    //trigger Drrp,IMS Block to populate Road Names
    $(function () {
        $("#DRRP_BLOCK").trigger('change');
        $("#IMS_BLOCK").trigger('change');
    });


    //select radio button
    if ($("#IMS_INCLUDED_IN_CN").val() == "Y") {
        $("#radioDrrpYes").attr('checked', true);
    } else if ($("#IMS_INCLUDED_IN_CN").val() == "N") {
        $("#radioDrrpNo").attr('checked', true);
    }

    if ($("#IMS_INCLUDED_IN_PROPOSAL").val() == "Y") {
        $("#radioImsYes").attr('checked', true);
    } else if ($("#IMS_INCLUDED_IN_PROPOSAL").val() == "N") {
        $("#radioImsNo").attr('checked', true);
    }


    //radio button events drrp

    if ($("#radioDrrpYes").is(":checked")) {
        IsDrrpYes();
    }
    if ($("#radioDrrpNo").is(":checked")) {
        IsDrrpNo();
    }

    $("#radioDrrpYes").change(function () {
        IsDrrpYes();
    });

    $("#radioDrrpNo").change(function () {
        IsDrrpNo();
    });

    //radio button events ims

    if ($("#radioImsYes").is(":checked")) {
        IsImsYes();
    }
    if ($("#radioImsNo").is(":checked")) {
        IsImsNo();
    }

    $("#radioImsYes").change(function () {
        IsImsYes();
    });

    $("#radioImsNo").change(function () {
        IsImsNo();
    });

    //save MP Proposal Road Details
    $('#btnSaveMPProposal').click(function (evt) {
        evt.preventDefault();
        if ($('#frmMPProposalInclusion').valid()) {
            $.ajax({
                url: '/MPMLAProposal/AddMPProposalRoadInclusionDetails',
                type: "POST",
                cache: false,
                data: $("#frmMPProposalInclusion").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    Alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },

                success: function (response) {
                    if (response.success == undefined) {
                        $("#divMPProposedRoadForm").html(response);
                    }
                    else if (response.success) {
                        alert(response.message);
                        $("#btnReset").trigger("click");
                        CloseMpProposedRoadDetails();
                        $('#tbMPProposedRoadList').trigger("reloadGrid");
                        unblockPage();
                    }
                    else {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.message);
                        unblockPage();
                    }
                    unblockPage();
                }
            });//end of ajax        
        }
    });


    //Drrp Drop Down Change Event

    $("#DRRP_BLOCK").change(function () {
        PopulateDrrpRoadNames();
    });
    //btnReset
    $("#btnResetMPRoadProposalInclusion").click(function () {
        $("#radioDrrpNo").trigger('click');
        $("#radioImsNo").trigger('click');

        $("#IMS_REASON_ID_1").removeClass("input-validation-error IMS_REASON_ID_1").addClass("IMS_REASON_ID_1");
        $("#spnMpCoreNetworkRoadReason").removeClass("field-validation-error").addClass("field-validation-valid");

        $("#IMS_REASON_ID_2").removeClass("input-validation-error IMS_REASON_ID_2").addClass("IMS_REASON_ID_2");
        $("#spnMpIMSRoadReason").removeClass("field-validation-error").addClass("field-validation-valid");

    });

    //Ims Block,Year Drop Down Change Event
    $("#IMS_BLOCK").change(function () {
        PopulateImsRoadNames();
    });
    $("#inclusion_year").change(function () {
        PopulateImsRoadNames();
    });

});

function ShowMPProposedRoads() {
    $('#accordion').hide('slow');
    $('#divExistingRoadsForm').hide('slow');
    $("#tbExistingRoadsList").jqGrid('setGridState', 'visible');
    showFilter();
}


function IsDrrpYes() {

    if ($("#radioDrrpYes").is(":checked")) {

        $(".TdDrrpYes").show("slow");
        $(".tdDrrpNo").hide("slow");

        //disable drop downs
        $("#IMS_REASON_ID_1").attr('disabled', true);
        $("#PLAN_CN_ROAD_CODE").attr('disabled', false);
        //$("#tdDrrpRoadNamesDdl").attr('colspan', '2');

        $("#trDrrpNo").hide();
        $("#trDrrpYes").show();
        $("#drrpHideShow").show();

    }
}

function IsDrrpNo() {
    if ($("#radioDrrpNo").is(":checked")) {

        $(".tdDrrpNo").show("slow");
        $(".TdDrrpYes").hide("slow");

        //disable drop down
        $("#IMS_REASON_ID_1").attr('disabled', false);
        $("#PLAN_CN_ROAD_CODE").attr('disabled', true);
        $("#PLAN_CN_ROAD_CODE").val(null);

        //$("#tdReasonDdl").attr('colspan', '4');

        $("#trDrrpNo").show();
        $("#trDrrpYes").hide();
        $("#drrpHideShow").hide();
    }
}

function IsImsYes() {
    if ($("#radioImsYes").is(":checked")) {
        $(".tdImsYes").show("slow");
        $(".tdImsNo").hide("slow");
        //disable drop down
        $("#IMS_REASON_ID_2").attr('disabled', true);


        $("#trImsNo").hide();
        $("#trImsYes").show();
        $("#ImsHideShow").show();
    }
}

function IsImsNo() {
    if ($("#radioImsNo").is(":checked")) {
        $(".tdImsNo").show("slow");
        $(".tdImsYes").hide("slow");

        //disable drop down
        $("#IMS_REASON_ID_2").attr('disabled', false);
        $("#IMS_PR_ROAD_CODE").attr('disabled', true);
        $("#IMS_PR_ROAD_CODE").val(null);

        $("#trImsNo").show();
        $("#trImsYes").hide();

        $("#ImsHideShow").hide();
    }
}


function PopulateDrrpRoadNames() {

    var blockCode = $("#DRRP_BLOCK option:selected").val();
    var planCnRoadCode = $("#PLAN_CN_ROAD_CODE").val();

    $("#ddlDrrpRoadNames").val(0);
    $("#ddlDrrpRoadNames").empty();

    $.ajax({
        url: '/MPMLAProposal/PopulateDrrpRoads/',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        data: { MAST_BLOCK_CODE: blockCode },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
                $("#ddlDrrpRoadNames").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
            unblockPage();
        },
        complete: function () {
            //set Road Names
            $("#ddlDrrpRoadNames").val(planCnRoadCode);
        },
        error: function (err) {
            alert("error " + err);
            unblockPage();
        }
    });

}

function PopulateImsRoadNames() {

    var blockCode = $("#IMS_BLOCK option:selected").val();
    var yearCode = $("#inclusion_year option:selected").val();

    var imsPrRoadCode = $("#IMS_PR_ROAD_CODE").val();

    $("#ddlImsRoadNames").val(0);
    $("#ddlImsRoadNames").empty();

    $.ajax({
        url: '/MPMLAProposal/PopulateImsRoads/',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        data: { BLOCK_CODE: blockCode, YEAR: yearCode },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
                $("#ddlImsRoadNames").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
            unblockPage();
        },
        complete: function () {
            //set Road Names
            $("#ddlImsRoadNames").val(imsPrRoadCode);
        },
        error: function (err) {
            alert("error " + err);
            unblockPage();
        }
    });

}
