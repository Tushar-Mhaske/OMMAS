$(document).ready(function () {

    $("#trTEchnicalDetails").removeClass("ui-state-hover");
    //$("#trTEchnicalDetails").addClass("ui-widget-header");    

    $.validator.unobtrusive.parse($('frmUnlockedProposal'));

    //if ($("#hdnOperation").val() == "U") {

    //    //$("#btnCreate").attr("value", "Update");
    //    //$("#btnReset").hide();
    //    //$("#trProposalLength").show('slow');
    //    //$("#btnCancel").show();

    //    /// Restrictions For Upgradation
    //    $("#rdoNew").attr("disabled", "disabled");
    //    $("#rdoUpgrade").attr("disabled", "disabled");

    //    // if Complete Proposal dont allow to change to Staged Proposal
    //    //if ($("#rdoComplete").is(':checked')) {
    //    if ($("#hdnISSTAGED").val() == 'C') {
    //        $("#rdoStaged").attr("disabled", "disabled");
    //        $("#rdoComplete").attr("disabled", "disabled");
    //    }

    //    //alert($("#hdnStagePhase").val());
    //    // Stage II Proposal
    //    //if ($("#rdoStageII").is(':checked')) {
    //    if ($("#hdnStagePhase").val() == 'S2') {
    //        $("#rdoStaged").attr("disabled", "disabled");
    //        $("#rdoComplete").attr("disabled", "disabled");
    //        $("#rdoStageII").attr("disabled", "disabled");
    //        $("#rdoStageI").attr("disabled", "disabled");

    //        $("#rdoFullLength").attr("disabled", "disabled");
    //        $("#rdoPartialLength").attr("disabled", "disabled");
    //    }
    //}
    
    if (parseInt($('#IMS_YEAR').val()) >= 2011) {
        $("#rdoNew").attr("disabled", "disabled");
        $("#rdoUpgrade").attr("disabled", "disabled");

        if ($("#hdnISSTAGED").val() == 'C') {
            $("#rdoStaged").attr("disabled", "disabled");
            $("#rdoComplete").attr("disabled", "disabled");
        }
        if ($("#rdoStageII").is(':checked')) {
            if ($("#hdnStagePhase").val() == 'S2') {
                $("#rdoStaged").attr("disabled", "disabled");
                $("#rdoComplete").attr("disabled", "disabled");
                $("#rdoStageII").attr("disabled", "disabled");
                $("#rdoStageI").attr("disabled", "disabled");

                $("#rdoFullLength").attr("disabled", "disabled");
                $("#rdoPartialLength").attr("disabled", "disabled");
            }
        }
    }

    CalculateStateShareMordShareTotalCost();

    $("#rdoFullLength").click(function () {
        $("#IMS_PAV_LENGTH").val($("#DUP_IMS_PAV_LENGTH").val());
        //$("#IMS_PAV_LENGTH").attr('readonly', 'readonly');
        alert("Core Network Length: " + $("#DUP_IMS_PAV_LENGTH").val() + ", If Road Length is greater than CN Length than change Pavement Length");
    });

    $('#btnUpdate').click(function (evt) {
        evt.preventDefault();

        if ($('#rdoSharePercent2015No').is(':checked'))//,#rdoSharePercent2015SchemeII,#rdoSharePercent2015Old').is(':checked'))
        {
            $('#rdoSharePercent2015No').attr('disabled', false);
        }
        else if ($('#rdoSharePercent2015Yes').is(':checked')) {
            $('#rdoSharePercent2015Yes').attr('disabled', false);
        }
        else if ($('#rdoSharePercent2015SchemeII').is(':checked')) {
            $('#rdoSharePercent2015SchemeII').attr('disabled', false);
        }
        else if ($('#rdoSharePercent2015Old').is(':checked')) {
            $('#rdoSharePercent2015Old').attr('disabled', false);
        }

        if ($('#frmUnlockedProposal').valid()) {
            if (validate()) {

                $("#rdoNew").removeAttr("disabled");
                $("#rdoUpgrade").removeAttr("disabled");

                $("#rdoStaged").removeAttr("disabled");
                $("#rdoComplete").removeAttr("disabled");

                $("#rdoStageII").removeAttr("disabled");
                $("#rdoStageI").removeAttr("disabled");

                $.ajax({
                    url: '/Proposal/SaveUnLockedProposal',
                    type: "POST",
                    cache: false,
                    data: $("#frmUnlockedProposal").serialize(),
                    beforeSend: function () {
                        blockPage();
                    },
                    error: function (xhr, status, error) {
                        unblockPage();
                        Alert("Request can not be processed at this time,please try after some time!!!");
                        return false;
                    },
                    success: function (response) {
                        unblockPage();
                        if (response.Success) {
                            alert("Proposal Updated Succesfully.");
                            CloseProposalDetails();
                            unblockPage();
                        }
                        else {
                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html(response.ErrorMessage);
                            $('#mainDiv').animate({ scrollTop: 0 }, 'slow');
                            unblockPage();
                        }
                    }
                });
            }
        }
    });

    $("#btnCancel").click(function () {
        CloseProposalDetails();
    });

    // FOR Calculating the Total of Maintenance cost
    $(":text[class~=TMC]").blur(function () {
        var fltTPE = 0.0;
        $(":text[class~=TMC]").each(function () {
            var tempVal = $(this).val().replace(new RegExp("\,", "g"), "");
            if (Number(tempVal) != NaN) {
                fltTPE += Number(tempVal);
                $("#txtTotalMaintenance").val(parseFloat(fltTPE).toFixed(4));
            }
        });
    });

    $(":text[class~=TC]").blur(function () {
        var fltTPE = 0.0;
        $(":text[class~=TC]").each(function () {
            var tempVal = $(this).val().replace(new RegExp("\,", "g"), "");
            if (Number(tempVal) != NaN) {
                fltTPE += Number(tempVal);
                var totalCost = parseFloat(fltTPE).toFixed(2);
                $("#TotalCost").val(totalCost);

                // set State Share & Mord share
                // Yes means 90/10 share & No means 75/25
                if ($("#IMS_SHARE_PERCENT").val() == 1) {

                    var totalCost90Percent = parseFloat((totalCost * 90) / 100).toFixed(2);
                    var totalCost10Percent = parseFloat((totalCost * 10) / 100).toFixed(2);

                    $("#IMS_SANCTIONED_AMOUNT").val(totalCost90Percent);
                    $("#IMS_SANCTIONED_RS_AMT").val(totalCost10Percent);

                } else if ($("#IMS_SHARE_PERCENT").val() == 2) {
                    var totalCost75Percent = parseFloat((totalCost * 75) / 100).toFixed(2);
                    var totalCost25Percent = parseFloat((totalCost * 25) / 100).toFixed(2);

                    $("#IMS_SANCTIONED_AMOUNT").val(totalCost75Percent);
                    $("#IMS_SANCTIONED_RS_AMT").val(totalCost25Percent);
                }
            }
        });
    });

    $("#IMS_TRAFFIC_TYPE").change(function () {
        blockPage();
        $.ajax({
            url: '/Proposal/IsProposalEdited',
            type: 'POST',
            data: { IMS_PR_ROAD_CODE: $("#IMS_PR_ROAD_CODE").val(), IMS_TRAFFIC_TYPE: true, value: Math.random() },
            success: function (response) {
                unblockPage();
                if (response.Success == true) {

                }
                else {
                    alert(response.Errormessage);
                    $("#IMS_TRAFFIC_TYPE").val($("#hdnTrafficType").val());
                    return false;
                }
            },
            error: function (err) {
                alert("Error while getting processing your request.");
                unblockPage();
            }
        });
        unblockPage();
    });

    if ($("#rdoNew").is(":checked")) {
        $("#ExistingSurface").hide();

        $("#trNewConnectivity").show();
        $('.tdStages').hide();
    }

    if ($("#rdoUpgrade").is(":checked")) {
        $("#ExistingSurface").show();
        $("#rdoComplete").prop('checked', true);

        $("#trNewConnectivity").hide();
        //$('.tdStages').show();
    }

    $("#rdoComplete").click(function () {
        if ($(this).is(":checked")) {
            $('.tdStages').hide();
            ClearStageIIData();
        }
    });

    if ($('#hdnISSTAGED').val() == "C") {
        $("#rdoComplete").trigger('click');
    }

    $("#rdoStaged").click(function () {
        if ($(this).is(":checked")) {
            $('.tdStages').show();
            ClearStageIIData();
        }
    });

    if ($("#rdoStaged").is(":checked")) {
        $('.tdStages').show();
    }
    //alert($('#hdnStagePhase').val());
    //alert($('#rdoStageI').is(":checked"));




    $('#rdoStageI').click(function () {
        $("#hdn_IMS_STAGE_PHASE").val("S1");
        ClearStageIIData();
    });

    // populate the Packages for Stage 2
    $('#rdoStageII').click(function () {
        $("#hdn_IMS_STAGE_PHASE").val("S2");

        if ($(this).is(":checked")) {
            $('#trStageII').show();
        }
        else {
            $('#trStageII').hide();
        }
    });

    if ($('#hdnStagePhase').val() == "S1") {
        $("#rdoStageI").prop('checked', true);
        $("#rdoStageI").trigger('click');
    }

    if ($('#hdnStagePhase').val() == "S2") {
        $("#rdoStageII").prop('checked', true);
        $("#rdoStageII").trigger('click');
    }

    $("#rdoUpgrade").click(function () {

        $("#IMS_PROPOSED_SURFACE").val("S");

        if ($(this).is(":checked")) {
            $("#ExistingSurface").show("slow");

            $("#trNewConnectivity").hide();
            //$('.tdStages').show();
            ClearStageIIData();
        }
    });

    $("#rdoNew").click(function () {
        $("#IMS_PROPOSED_SURFACE").val("S");
        if ($(this).is(":checked")) {
            $("#ExistingSurface").hide("slide");

            $("#trNewConnectivity").show();
            //$('.tdStages').hide();
            if ($('#rdoStaged').is(":checked")) {
                $('.tdStages').show();
            }
            if ($('#rdoComplete').is(":checked")) {
                $('.tdStages').hide();
            }
            ClearStageIIData();
        }
    });

    $('#IMS_CC_LENGTH').blur(function () {
        CalculatePavementLength();
    });

    $("#IMS_BT_LENGTH").blur(function () {
        CalculatePavementLength();
    });

    // populate the Packages for Stage 2
    $("#Stage_2_Year").change(function () {

        if ($("#Stage_2_Year").val() == 0) {
            $('#Stage_2_Package_ID').children('option:not(:first)').remove();
            $('#IMS_STAGED_ROAD_ID').children('option:not(:first)').remove();
            $('#PLAN_CN_ROAD_CODE').children('option:not(:first)').remove();
        }
        else {
            $('#Stage_2_Package_ID').children('option:not(:first)').remove();
            $('#IMS_STAGED_ROAD_ID').children('option:not(:first)').remove();
            PopulatePakage($("#Stage_2_Year").val(), 0, 'Stage_2_Package_ID', false, false);
        }

    });

    //---------------- PMGSY 2  Starts Here----------------------------//

    $("#rdoHigherSpecificationYes").click(function () {
        if ($(this).is(":checked")) {
            $("#trHigherSpecCost").show();
        }
    });

    $("#rdoHigherSpecificationNo").click(function () {
        if ($(this).is(":checked")) {
            $("#trHigherSpecCost").hide();
        }
    });

    $("#rdoSharePercentYes").click(function () {
        if ($(this).is(":checked")) {
            $("#lblStateShare").html("(10% of Total Cost)");
            $("#lblMordShare").html("(90% of Total Cost)");

            var tempVal = $("#TotalCost").val().replace(new RegExp("\,", "g"), "");
            if (Number(tempVal) != NaN) {
                totalCost = parseFloat($("#TotalCost").val()).toFixed(2);
            }
            var totalCost90Percent = parseFloat((totalCost * 90) / 100).toFixed(2);
            var totalCost10Percent = parseFloat((totalCost * 10) / 100).toFixed(2);

            $("#IMS_SANCTIONED_AMOUNT").val(totalCost90Percent);
            $("#IMS_SANCTIONED_RS_AMT").val(totalCost10Percent);

        }
    });

    $("#rdoSharePercentNo").click(function () {
        if ($(this).is(":checked")) {
            $("#lblStateShare").html("(25% of Total Cost)");
            $("#lblMordShare").html("(75% of Total Cost)");

            var tempVal = $("#TotalCost").val().replace(new RegExp("\,", "g"), "");
            if (Number(tempVal) != NaN) {
                totalCost = parseFloat($("#TotalCost").val()).toFixed(2);
            }

            var totalCost75Percent = parseFloat((totalCost * 75) / 100).toFixed(2);
            var totalCost25Percent = parseFloat((totalCost * 25) / 100).toFixed(2);

            $("#IMS_SANCTIONED_AMOUNT").val(totalCost75Percent);
            $("#IMS_SANCTIONED_RS_AMT").val(totalCost25Percent);
        }
    });


    if ($("#rdoHigherSpecificationYes").is(":checked")) {
        $("#trHigherSpecCost").show();
    }
    else if ($("#rdoHigherSpecificationNo").is(":checked")) {
        $("#trHigherSpecCost").hide();
    }

    if ($("#rdoSharePercentYes").is(":checked")) {
        $("#lblStateShare").html("(10% of Total Cost)");
        $("#lblMordShare").html("(90% of Total Cost)");
    }
    else if ($("#rdoSharePercentNo").is(":checked")) {
        $("#lblStateShare").html("(25% of Total Cost)");
        $("#lblMordShare").html("(75% of Total Cost)");
    }

    // FOR Calculating the Total Cost in case of PMGSY Scheme 2
    $(":text[class~=TC]").blur(function () {
        var fltTPE = 0.0;
        $(":text[class~=TC]").each(function () {
            var tempVal = $(this).val().replace(new RegExp("\,", "g"), "");
            if (Number(tempVal) != NaN) {
                fltTPE += Number(tempVal);
                var totalCost = parseFloat(fltTPE).toFixed(2);
                $("#TotalCost").val(totalCost);

                // set State Share & Mord share
                // Yes means 90/10 share & No means 75/25
                if ($("#rdoSharePercentYes").is(":checked")) {

                    var totalCost90Percent = parseFloat((totalCost * 90) / 100).toFixed(2);
                    var totalCost10Percent = parseFloat((totalCost * 10) / 100).toFixed(2);

                    $("#IMS_SANCTIONED_AMOUNT").val(totalCost90Percent);
                    $("#IMS_STATE_SHARE").val(totalCost10Percent);

                } else if ($("#rdoSharePercentNo").is(":checked")) {
                    var totalCost75Percent = parseFloat((totalCost * 75) / 100).toFixed(2);
                    var totalCost25Percent = parseFloat((totalCost * 25) / 100).toFixed(2);

                    $("#IMS_SANCTIONED_AMOUNT").val(totalCost75Percent);
                    $("#IMS_STATE_SHARE").val(totalCost25Percent);
                }
            }
        });
    });


    $('#rdoSharePercent2015No,#rdoSharePercent2015Yes,#rdoSharePercent2015Old').click(function () {

        CalculateStateShareMordShareTotalCost();

    });

    $('#IMS_SANCTIONED_CD_AMT,#IMS_SANCTIONED_PW_AMT,#IMS_SANCTIONED_OW_AMT,#IMS_SANCTIONED_PAV_AMT,#IMS_STATE_SHARE,#IMS_FURNITURE_COST,#IMS_HIGHER_SPECIFICATION_COST,#IMS_SANCTIONED_RS_AMT').blur(function () {

        CalculateStateShareMordShareTotalCost();

    });

});
function CalculatePavementLength() {

    //return true;

    if (!isNaN($("#IMS_CC_LENGTH").val()) && !isNaN($("#IMS_BT_LENGTH").val()) && $.trim($("#IMS_CC_LENGTH").val()) != "" && $.trim($("#IMS_BT_LENGTH").val()) != "") {
        if (parseFloat($("#IMS_PAV_LENGTH").val()).toFixed(3) != parseFloat(parseFloat($("#IMS_CC_LENGTH").val()) + parseFloat($("#IMS_BT_LENGTH").val())).toFixed(3)) {
            //console.log('PAV Length ' + $("#IMS_PAV_LENGTH").val());
            //console.log('IMS Length ' + parseFloat($("#IMS_CC_LENGTH").val()));
            //console.log('BT Length ' + parseFloat($("#IMS_BT_LENGTH").val()));
            console.log(parseFloat($("#IMS_PAV_LENGTH").val()).toFixed(3));
            console.log(parseFloat(parseFloat($("#IMS_CC_LENGTH").val()) + parseFloat($("#IMS_BT_LENGTH").val())).toFixed(3));
            alert("Sum Of CC Length and BT Length Should be equal to Pavement Length.");
            return false;
        }
        else {
            return true;
        }
    }
    else {
        return true;
    }
}
function validate() {

    if (CalculatePavementLength()) {
        return true;
    }
    else {
        return false;
    }
}
function CalculateStateShareMordShareTotalCost() {
    var pavCost = isNaN($('#IMS_SANCTIONED_PAV_AMT').val()) ? parseFloat("0") : parseFloat($('#IMS_SANCTIONED_PAV_AMT').val());
    var cdCost = isNaN($('#IMS_SANCTIONED_CD_AMT').val()) ? parseFloat("0") : parseFloat($('#IMS_SANCTIONED_CD_AMT').val());
    var pwCost = isNaN($('#IMS_SANCTIONED_PW_AMT').val()) ? parseFloat("0") : parseFloat($('#IMS_SANCTIONED_PW_AMT').val());
    var owCost = isNaN($('#IMS_SANCTIONED_OW_AMT').val()) ? parseFloat("0") : parseFloat($('#IMS_SANCTIONED_OW_AMT').val());
    var stateCost = isNaN($('#IMS_SANCTIONED_RS_AMT').val()) ? parseFloat("0") : parseFloat($('#IMS_SANCTIONED_RS_AMT').val());
    var higherSpecCost = 0
    var furnitureCost = 0;
    if ($('#PMGSYScheme').val() == 2) {
        furnitureCost = isNaN($('#IMS_FURNITURE_COST').val()) ? parseFloat("0") : parseFloat($('#IMS_FURNITURE_COST').val());
        higherSpecCost = isNaN($('#IMS_HIGHER_SPECIFICATION_COST').val()) ? parseFloat("0") : parseFloat($('#IMS_HIGHER_SPECIFICATION_COST').val());
    }

    var totalCost = parseFloat(pavCost) + parseFloat(cdCost) + parseFloat(pwCost) + parseFloat(owCost);

    if ($('#PMGSYScheme').val() == 2) {
        totalCost = parseFloat(furnitureCost) + parseFloat(totalCost);
    }

    if ($('#rdoSharePercent2015No').is(':checked')) {
        $('#IMS_MORD_SHARE_2015').val(parseFloat(totalCost * 0.9).toFixed(2));
        $('#IMS_STATE_SHARE_2015').val(parseFloat(totalCost * 0.1).toFixed(2));

    }
    else if ($('#rdoSharePercent2015Yes').is(':checked')) {
        $('#IMS_MORD_SHARE_2015').val(parseFloat(totalCost * 1).toFixed(2));
        $('#IMS_STATE_SHARE_2015').val(parseFloat(totalCost * 0).toFixed(2));
    }
    else if ($('#rdoSharePercent2015SchemeII').is(':checked')) {
        $('#IMS_MORD_SHARE_2015').val(parseFloat(totalCost * 0.75).toFixed(2));
        $('#IMS_STATE_SHARE_2015').val(parseFloat(totalCost * 0.25).toFixed(2));
    }
    else if ($('#rdoSharePercent2015Old').is(':checked')) {
        $('#IMS_MORD_SHARE_2015').val(parseFloat(totalCost * 0.60).toFixed(2));
        $('#IMS_STATE_SHARE_2015').val(parseFloat(totalCost * 0.40).toFixed(2));
    }

    higherSpecCost = isNaN(higherSpecCost) ? 0 : parseFloat(higherSpecCost);

    var totalCostToUpdate = parseFloat(parseFloat(totalCost) + parseFloat(stateCost)).toFixed(2);

    console.log('higherSpecCost: ' + parseFloat(higherSpecCost));

    console.log('stateCost: ' + parseFloat(stateCost));
    console.log('IMS_STATE_SHARE_2015: ' + parseFloat($('#IMS_STATE_SHARE_2015').val()));

    $('#IMS_TOTAL_STATE_SHARE_2015').val(parseFloat(parseFloat(stateCost) + parseFloat($('#IMS_STATE_SHARE_2015').val()) + parseFloat(higherSpecCost)).toFixed(2));
    $('#IMS_TOTAL_COST_2015').val(totalCostToUpdate);

    if ($('#PMGSYScheme').val() == 2) {

        totalCostToUpdate = parseFloat(totalCostToUpdate) + parseFloat(higherSpecCost);
        $('#IMS_TOTAL_STATE_SHARE_2015').val(parseFloat(parseFloat(higherSpecCost) + parseFloat($('#IMS_STATE_SHARE_2015').val())).toFixed(2));
    }

    $('#IMS_TOTAL_COST_2015').val(parseFloat(parseFloat($('#IMS_TOTAL_STATE_SHARE_2015').val()) + parseFloat($('#IMS_MORD_SHARE_2015').val())).toFixed(2));
}


// This Function Populates Packages fot selected year and batch
// DropDownName : Name of the Dropdown where values are going to be populated
//paramShowAlert : shows alert if no data found
function PopulatePakage(paramYearID, ParamBatchID, DropDownName, paramShowAlert, populateFirstItem) {

    if (paramShowAlert == null) {
        paramShowAlert = true;
    }

    if (typeof populateFirstItem === "undefined" || populateFirstItem === null) {
        populateFirstItem = true;
    }


    if (populateFirstItem) {
        $("#" + $(DropDownName).attr("ID")).val(0);
        $("#" + $(DropDownName).attr("ID")).empty();
        $("#" + $(DropDownName).attr("ID")).append("<option value='0'>Select Package</option>");
    }
    else {
        // $("#" + $(DropDownName).attr("ID")).empty();
        $("#" + $(DropDownName).attr("ID")).children('option:not(:first)').remove();
    }



    $.ajax({
        url: '/Proposal/GetPackageId',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        data: { Year: paramYearID, BatchID: ParamBatchID, value: Math.random() },
        success: function (jsonData) {
            if (jsonData.length == 0) {
                if (paramShowAlert) {
                    alert("No Package found for Selected Year and Batch");
                }
                unblockPage();
            }
            for (var i = 0; i < jsonData.length; i++) {


                $("#" + $("#" + DropDownName).attr("ID")).append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
            unblockPage();
        },
        error: function (err) {
            alert("error " + err);
            unblockPage();
        }

    });
}

function ClearStageIIData() {
    $('#Stage_2_Year').val(0);
    $('#Stage_2_Package_ID').children('option:not(:first)').remove();
    $('#trStageII').hide();
}