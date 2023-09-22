
// For UpGradation
$.validator.unobtrusive.adapters.add('isupgradation', ['imsupgrade'], function (options) {
    options.rules['isupgradation'] = options.params;
    options.messages['isupgradation'] = options.message;
});

$.validator.addMethod("isupgradation", function (value, element, params) {
    var IsUpgradtion = $('input:radio[name="' + params.imsupgrade + '"]:checked').val();

    if (IsUpgradtion == "U") {
        if (value == null || value == "" || value == "0") {
            return false;
        }
    }
    return true;
});

// For Upgradation Is Habitation Benifitted 
$.validator.unobtrusive.adapters.add('isreasonselected', ['ishabitationbenifitted'], function (options) {
    options.rules['isreasonselected'] = options.params;
    options.messages['isreasonselected'] = options.message;
});

$.validator.addMethod("isreasonselected", function (value, element, params) {
    var IsHabitationBenifitted = $('input:radio[name="' + params.ishabitationbenifitted + '"]:checked').val();

    if (IsHabitationBenifitted == "N") {
        if (value == null || value == "" || value == "0") {
            return false;
        }
    }
    return true;
});


// For Stage II Proposal
$.validator.unobtrusive.adapters.add('isstagetwoproposal', ['imsstagephase'], function (options) {
    options.rules['isstagetwoproposal'] = options.params;
    options.messages['isstagetwoproposal'] = options.message;
});

$.validator.addMethod("isstagetwoproposal", function (value, element, params) {

    var isStageTwo = $('input:radio[name="' + params.imsstagephase + '"]:checked').val();

    if (isStageTwo == "2") {

        if (value == null || value == "" || value == "0") {
            return false;
        }
    }
    return true;
});

// For Stage I Proposal
$.validator.unobtrusive.adapters.add('isstageoneproposal', ['imsstagephase'], function (options) {
    options.rules['isstageoneproposal'] = options.params;
    options.messages['isstageoneproposal'] = options.message;
});

$.validator.addMethod("isstageoneproposal", function (value, element, params) {


    var isStageOne = $('input:radio[name="' + params.imsstagephase + '"]:checked').val();

    // IF Not Stage  I Proposal it is compulsory so return false
    if (isStageOne == undefined || isStageOne == null || isStageOne == "2") { // && !$("#rdoComplete").is(':checked')) {        

        if (element.id == 'IMS_TRAFFIC_TYPE' || element.id == "txtTotalMaintenance") {
            if (value == null || value == "" || value == 0) {
                return false;
            }
            else {
                return true;
            }
        }
        else {
            if (value == null || value == "") {
                return false;
            }
            else {
                return true;
            }
        }

    }
    else if (isStageOne == "1") {
        return true;
    }

    /*    
    if (isStageOne == "1") {
        if (value == null || value == "" || value ==0 ) {
            return true;
        }
    }
    else if (isStageOne == "2") {
        if (value == null || value == "" || value == 0) {
            return false;
        }
    }
    */

});

// for maintenance cost validation in stage II proposal
$.validator.unobtrusive.adapters.add('istotalmaintanancecostvalid', ['imsstagephase'], function (options) {
    options.rules['istotalmaintanancecostvalid'] = options.params;
    options.messages['istotalmaintanancecostvalid'] = options.message;
});

$.validator.addMethod("istotalmaintanancecostvalid", function (value, element, params) {

    var isStageTwo = $('input:radio[name="' + params.imsstagephase + '"]:checked').val();
    if (isStageTwo == "2") {
        if (value == null || value == "" || parseFloat(value) == 0) {
            return false;
        }
    }
    return true;
});

// For New Package
$.validator.unobtrusive.adapters.add('isnewpackage', ['isnewpackage'], function (options) {
    options.rules['isnewpackage'] = options.params;
    options.messages['isnewpackage'] = options.message;
});

$.validator.addMethod("isnewpackage", function (value, element, params) {
    var IsNewPackage = $('input:radio[name="' + params.isnewpackage + '"]:checked').val();

    if (IsNewPackage == "N") {
        if (value == null || value == "" || value == "0") {
            return false;
        }
    }
    return true;
});

// For Existing Package
$.validator.unobtrusive.adapters.add('isexistingpackage', ['existingpackage'], function (options) {
    options.rules['isexistingpackage'] = options.params;
    options.messages['isexistingpackage'] = options.message;
});

$.validator.addMethod("isexistingpackage", function (value, element, params) {

    var isExistingPackage = $('input:radio[name="' + params.existingpackage + '"]:checked').val();

    if (isExistingPackage == "E") {
        if (value == null || value == "" || value == "0") {
            return false;
        }
    }
    return true;
});


////------------------- Validation Removed as per Pankaj Kumar's request on 10/06/2014 (Verified by Dev Sir) -------------
// For Validating Pavment Length
//$.validator.unobtrusive.adapters.add('ispavlengthvalid', ['imspavlen'], function (options) {
//    options.rules['ispavlengthvalid'] = options.params;
//    options.messages['ispavlengthvalid'] = options.message;
//});

//$.validator.addMethod("ispavlengthvalid", function (value, element, params) {

//    var imsPavLen = $('#' + params.imspavlen + '').val();
//    var cclength = $('#IMS_CC_LENGTH').val();
//    var btlength = $('#IMS_BT_LENGTH').val();




//   if ($('input:radio[name=IMS_UPGRADE_CONNECT]:checked').val() == "N") {

//        if ($("#stateType").val() == "R" || $("#stateType").val() == "I" ) {
//            var allowedPavementLength = parseFloat(imsPavLen) + (parseFloat(imsPavLen) * 0.2);
//            if (Boolean(parseFloat(value) > parseFloat(allowedPavementLength))) {                
//                return false;
//            }
//        }
//        if ($("#stateType").val() == "H" || $("#stateType").val() == "N" || $("#stateType").val() == "X") {
//            var allowedPavementLength = parseFloat(imsPavLen) + (parseFloat(imsPavLen) * 0.3);
//            if (Boolean(parseFloat(value) > parseFloat(allowedPavementLength))) {                                
//                return false;
//            }
//        }
//    }

//    else if ($('input:radio[name=IMS_UPGRADE_CONNECT]:checked').val() == "U") {

//        if ($("#stateType").val() == "R" || $("#stateType").val() == "I" ) {

//            var allowedPavementLength = parseFloat(imsPavLen) + (parseFloat(imsPavLen) * 0.1);

//            if (Boolean(parseFloat(value) > parseFloat(allowedPavementLength))) {
//                return false;
//            }
//        }
//        if ($("#stateType").val() == "H" || $("#stateType").val() == "N" || $("#stateType").val() == "X") {

//            var allowedPavementLength = parseFloat(imsPavLen) + (parseFloat(imsPavLen) * 0.2);

//            if (Boolean(parseFloat(value) > parseFloat(allowedPavementLength))) {
//                return false;
//            }
//        }
//    }

//    if ($('input:radio[name=IMS_STAGE_PHASE]:checked').val() != undefined) {            

//        if ($("#rdoStageI").is(":checked") &&  $("#IMS_CC_LENGTH").val() == "" && $("#IMS_BT_LENGTH").val() == "" ) {
//            return true;
//        }
//        else if ($("#rdoStageII").is(":checked")) {

//            // condition added by shyam 
//            // to ensure that 10% variation of Stage I Length, in IMS_PAV_LENGTH for Stage II Proposal
//            // and Should not greater than 30% of Core network length
//            var stage1PavLen = $("#IMS_STAGE1_PAV_LENGTH").val();

//            var allowedPavementLength = parseFloat(imsPavLen) + (parseFloat(imsPavLen) * 0.3);

//            // should not greater than 30%  of core network length check
//            if (Boolean(parseFloat(value) > parseFloat(allowedPavementLength))) {
//                return false;
//            }

//            // Should be less or greater than 10% variation of Stage1 Length
//            var allowedMaximumStage2Length = parseFloat(stage1PavLen) + (parseFloat(stage1PavLen) * 0.1);
//            var allowedMinimumStage2Length = parseFloat(stage1PavLen) - (parseFloat(stage1PavLen) * 0.1);
//            if ((Boolean(parseFloat(value) > parseFloat(allowedMaximumStage2Length))) || (Boolean(parseFloat(value) < parseFloat(allowedMinimumStage2Length)))) {

//                return false;
//            }

//            if (Boolean(parseFloat($("#IMS_PAV_LENGTH").val() != parseFloat(parseFloat(cclength) + parseFloat(btlength))))) {
//                return false;
//            }
//        }
//    }


//    if (Boolean(parseFloat($("#IMS_PAV_LENGTH").val()) != parseFloat(parseFloat(cclength) + parseFloat(btlength)))) {        
//        return false;
//    }

//    return true;
//});

//PMGSY Scheme 2
// For Higher specification cost
$.validator.unobtrusive.adapters.add('ishigherspeccostvalid', ['ishigherspec'], function (options) {
    options.rules['ishigherspeccostvalid'] = options.params;
    options.messages['ishigherspeccostvalid'] = options.message;
});

$.validator.addMethod("ishigherspeccostvalid", function (value, element, params) {
    var IsHigherSpec = $('input:radio[name="' + params.ishigherspec + '"]:checked').val();

    if ($("#PMGSYScheme").val() == "2") {
        if (IsHigherSpec == "Y") {
            if (value == null || parseFloat(value) <= 0) {
                return false;
            }
        }
    }
    return true;
});


$.validator.unobtrusive.adapters.add('isvalidrangeofpavlen', ['scheme'], function (options) {
    options.rules['isvalidrangeofpavlen'] = options.params;
    options.messages['isvalidrangeofpavlen'] = options.message;
});

$.validator.addMethod("isvalidrangeofpavlen", function (value, element, params) {

    if ($("#PMGSYScheme").val() == "2") {
        if (value == null || parseFloat(value) <= 0) {
            return false;
        }
    } else if (($("#PMGSYScheme").val() == "1" || $("#PMGSYScheme").val() == 3)) {
        if (value == null || (parseFloat(value).toFixed(2)) < 0.50) {
            return false;
        }
    }

    return true;
});


$(document).ready(function () {

    $.validator.unobtrusive.parse($('frmCreateProposal'));

    $("input[type='reset']").on("click", function (event) {

        ResetForm();
    });

    //alert($("#rdoStageI").is(":checked"));

    setCurrentFinancialYear();

    // Update Operation
    if ($("#hdnOperation").val() == "U") {

        $("#btnCreate").attr("value", "Update");
        $("#btnReset").hide();
        $("#trProposalLength").show('slow');
        $("#btnCancel").show();
        /// Restrictions For Upgradation
        $("#rdoNew").attr("disabled", "disabled");
        $("#rdoUpgrade").attr("disabled", "disabled");

        // if Complete Proposal dont allow to change to Staged Proposal
        if ($("#rdoComplete").is(':checked')) {
            $("#rdoStaged").attr("disabled", "disabled");
            $("#rdoComplete").attr("disabled", "disabled");
        }
        // Stage II Proposal
        if ($("#rdoStageII").is(':checked')) {
            $("#rdoStaged").attr("disabled", "disabled");
            $("#rdoComplete").attr("disabled", "disabled");
            $("#rdoStageII").attr("disabled", "disabled");
            $("#rdoStageI").attr("disabled", "disabled");

            $("#rdoFullLength").attr("disabled", "disabled");
            $("#rdoPartialLength").attr("disabled", "disabled");
        }



    } else if ($("#hdnOperation").val() == "C") {
        $("#btnCreate").attr("value", "Save");
        $("#btnCancel").hide();
        $("#btnReset").show();

        $('input[name=IMS_ZP_RESO_OBTAINED]:nth(0)').attr("checked", "checked");
        $('input[name=IMS_PARTIAL_LEN]:nth(0)').attr("checked", "checked");
    }

    // Restriction while updating Road Proposal


    $("#btnCancel").click(function () {
        CloseProposalDetails();
    });

    $("#trTEchnicalDetails").removeClass("ui-state-hover");
    $("#trPackageID").removeClass("ui-state-hover");

    if ($("#rdoNew").is(":checked")) {
        $("#ExistingSurface").hide();
        $("#trBenefittedHab").hide();
        $("#trComplete").show();
        //var rules = //$("#MAST_EXISTING_SURFACE_CODE").rules();           
        ////$("#MAST_EXISTING_SURFACE_CODE").rules("remove", "required");
    }

    $("#rdoNew").click(function () {
        $("#IMS_PROPOSED_SURFACE").val("S");
        if ($(this).is(":checked")) {
            $("#ExistingSurface").hide("slide");
            $("#trBenefittedHab").hide("slide");
            $("#trComplete").show("slide");

            $("#MAST_BLOCK_CODE").val(0);
            $('#MAST_MP_CONST_CODE').children('option:not(:first)').remove();
            $('#MAST_MLA_CONST_CODE').children('option:not(:first)').remove();


            $('#PLAN_CN_ROAD_CODE').children('option:not(:first)').remove();
            $('#Stage_2_Year').val(0);
            $('#Stage_2_Package_ID').children('option:not(:first)').remove();
            $('#IMS_STAGED_ROAD_ID').children('option:not(:first)').remove();
            $('#rdoFullLength').removeAttr('disabled');
            $('#rdoOldPackage').removeAttr('disabled');
        }
        else {
            $("#ExistingSurface").show("slow");
            $("#trBenefittedHab").show("slow");
        }
    });

    if ($("#rdoUpgrade").is(":checked")) {
        $("#ExistingSurface").show();
        $("#trBenefittedHab").show();
        $("#rdoComplete").prop('checked', true);
        $("#trComplete").hide();

        //$("#MAST_EXISTING_SURFACE_CODE").rules("add", "required");
    }

    $("#rdoUpgrade").click(function () {

        $("#IMS_PROPOSED_SURFACE").val("S");

        if ($(this).is(":checked")) {
            $("#ExistingSurface").show("slow");

            $("#rdoStaged").prop('checked', false);
            $("#rdoStageI").prop('checked', false);
            $("#rdoStageII").prop('checked', false);
            $("#trStages").hide("slow");

            $("#trBenefittedHab").show("slow");
            $("#trStageII").hide("slow");
            $("#trStagedRoad").hide("slow");
            $("#trComplete").hide("slow");

            $("#MAST_BLOCK_CODE").val(0);
            $('#MAST_MP_CONST_CODE').children('option:not(:first)').remove();
            $('#MAST_MLA_CONST_CODE').children('option:not(:first)').remove();

            $('#PLAN_CN_ROAD_CODE').children('option:not(:first)').remove();
            $('#Stage_2_Year').val(0);
            $('#Stage_2_Package_ID').children('option:not(:first)').remove();
            $('#IMS_STAGED_ROAD_ID').children('option:not(:first)').remove();

            $('#rdoFullLength').removeAttr('disabled');
            $('#rdoOldPackage').removeAttr('disabled');


            $("#IMS_CC_LENGTH").removeAttr('disabled');
            $("#IMS_BT_LENGTH").removeAttr('disabled');
            ClearStageIIData();
        }
        else {
            $("#ExistingSurface").hide("slow");
            $("#trBenefittedHab").hide("slow");
        }
    });

    if ($("#rdoStaged").is(":checked")) {
        $("#trStages").show();
        $("#trBenefittedHab").hide();

        if ($("hdnOperation").val() == "C") {
            $("#rdoStageI").prop('checked', true);
        }
    }
    else {
        $("#trStages").hide();
        $("#trStageII").hide();
        $("#trStagedRoad").hide("slow");
    }

    $("#rdoStaged").click(function () {
        if ($(this).is(":checked")) {
            $("#rdoNew").prop('checked', true);
            $("#trStages").show("slow");
            $("#trBenefittedHab").hide("slow");
            $("#rdoStageI").prop('checked', true);

            if ($("#rdoStageI").is(":checked")) {
                $("#IMS_PROPOSED_SURFACE").val("U");
            }


            ClearStageIIData();

            $("#IMS_CC_LENGTH").attr("disabled", "disabled");
            $("#IMS_BT_LENGTH").attr("disabled", "disabled");

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
            $("#IMS_STATE_SHARE").val(totalCost10Percent);

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
            $("#IMS_STATE_SHARE").val(totalCost25Percent);
        }
    });

    // For Edit Record
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


    //---------------- PMGSY 2 Ends Here----------------------------//



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


    if ($("#rdoStageII").is(":checked")) {
        $("#trStageII").show();
        $("#trStagedRoad").show();
        if ($("#hdnOperation").val() != "U") {
            $("#IMS_PROPOSED_SURFACE").val("U");
        }

        //$("#IMS_STAGED_ROAD_ID").attr("disabled", "disabled");
        //$("#Stage_2_Year").attr("disabled", "disabled");
        //$("#Stage_2_Package_ID").attr("disabled", "disabled");
        //$("#IMS_CC_LENGTH").removeAttr("disabled", "disabled");
        //$("#IMS_BT_LENGTH").removeAttr("disabled", "disabled");
    }

    if ($("#rdoStageI").is(":checked")) {
        $("#trStageII").hide();
        $("#trStagedRoad").hide();

        if ($("#hdnOperation").val() != "U") {
            $("#IMS_PROPOSED_SURFACE").val("S");
        }

        //$("#tdTrafficTypeLabel").hide("slow");
        //$("#tdTrafficTypeText").hide("slow");        
        $("#IMS_CC_LENGTH").attr("disabled", "disabled");
        $("#IMS_BT_LENGTH").attr("disabled", "disabled");
    }

    // populate the Packages for Stage 2
    $("#rdoStageII").click(function () {
        if ($(this).is(":checked")) {
            $("#trStageII").show("slow");
            //$("#trStagedRoad").show("slow");
            $("#IMS_PROPOSED_SURFACE").val("S");
            // $("#tdTrafficTypeLabel").show("slow");
            // $("#tdTrafficTypeText").show("slow");

            $("#MAST_BLOCK_CODE").val(0);
            $("#IMS_PAV_LENGTH").val(0);
            $("#DUP_IMS_PAV_LENGTH").val(0);
            $('#PLAN_CN_ROAD_CODE').children('option:not(:first)').remove();

            if ($("#Stage_2_Year").val() != 0) {
                //PopulatePakage($("#IMS_YEAR").val(), 0, Stage_2_Package_ID,false,false);//,true,false);
            }

            $("#IMS_CC_LENGTH").removeAttr("disabled", "disabled");
            $("#IMS_BT_LENGTH").removeAttr("disabled", "disabled");

            /* alert($("#hdnOperation").val());
             if ($("#hdnOperation").val() == "C") {
                 $('#Stage_2_Year').empty();
                 $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
                 $.ajax({
                     url: '/Proposal/GetStagedYearsList/',
                     type: 'POST',
                     catche: false,
                     data: { year: $('#IMS_YEAR').val(), proposalCode: $('#IMS_PR_ROAD_CODE').val() },
                     async: false,
                     success: function (jsonData) {
                         for (var i = 0; i < jsonData.length; i++) {
                             if (jsonData[i].Value == 2) {
                                 $("#Stage_2_Year").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                             }
                             else {
                                 $("#Stage_2_Year").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                             }
                         }
 
                         $.unblockUI();
                     },
                     error: function () {
                         $.unblockUI();
                         alert("Error ocurred");
                         return false;
                     },
                 })
             }*/
        }
        else {
            $("#trStageII").hide("slow");
            $("#trStagedRoad").hide("slow");
            //$("#tdTrafficTypeLabel").hide("slow");
            //$("#tdTrafficTypeText").hide("slow");
        }
    });

    $("#rdoStageI").click(function () {
        if ($(this).is(":checked")) {
            $("#trStageII").hide("slow");
            $("#trStagedRoad").hide("slow");
            $("#IMS_PROPOSED_SURFACE").val("U");
            //$("#tdTrafficTypeLabel").hide("slow");
            //$("#tdTrafficTypeText").hide("slow");

            ClearStageIIData();

            $("#IMS_CC_LENGTH").attr("disabled", "disabled");
            $("#IMS_BT_LENGTH").attr("disabled", "disabled");


        }
        else {
            $("#trStageII").show("slow");
            $("#trStagedRoad").hide("slow");
            //$("#tdTrafficTypeLabel").show("slow");
            //$("#tdTrafficTypeText").show("slow");
            $("#IMS_CC_LENGTH").removeAttr("disabled", "disabled");
            $("#IMS_BT_LENGTH").removeAttr("disabled", "disabled");
        }
    });

    $("#rdoComplete").click(function () {
        $("#IMS_PROPOSED_SURFACE").val("S");
        if ($(this).is(":checked")) {
            $("#trStages").hide("slow");
            $("#trStageII").hide("slow");
            $("#trStagedRoad").hide("slow");
            $("#rdoStageI").prop('checked', false);
            $("#rdoStageII").prop('checked', false);

            //$("#tdTrafficTypeLabel").hide("slow");
            //$("#tdTrafficTypeText").hide("slow");

            ClearStageIIData();

            $("#IMS_CC_LENGTH").removeAttr("disabled");
            $("#IMS_BT_LENGTH").removeAttr("disabled");

        }
    });

    $("#rdoYesHab").click(function () {
        if ($(this).is(":checked")) {
            $("#tdHabReasonLabel").hide("slow");
            $("#tdHabReasonText").hide("slow");
        }
        else {
            $("#tdHabReasonLabel").show("slow");
            $("#tdHabReasonText").show("slow");
        }
    });

    $("#MAST_BLOCK_CODE").change(function () {
        //$('#PLAN_CN_ROAD_CODE').children('option:not(:first)').remove();    

        ///PopulateFundingAgency Added for RCPLWE for both CREATE/UPDATE by SAMMED A. PATIL
        $("#IMS_COLLABORATION").empty();
        $.ajax({
            url: '/Proposal/PopulateFundingAgency?blockCode=' + $("#MAST_BLOCK_CODE").val(),
            type: 'POST',
            beforeSend: function () {
                blockPage();
            },
            //data: { BlockID: $("#MAST_BLOCK_CODE").val(), IMS_UPGRADE_CONNECT: $('input:radio[name=IMS_UPGRADE_CONNECT]:checked').val(), PROPOSAL_TYPE: 'P', value: Math.random() },

            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    $("#IMS_COLLABORATION").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
                unblockPage();
            },
            error: function (err) {
                alert("Error while getting Funding Agency.");
                unblockPage();
            }
        });

        if ($("#MAST_BLOCK_CODE").val() > 0) {
            var proceedExecutionFurther = false;
            if ($("#hdnOperation").val() == "U") {
                blockPage();

                $.ajax({
                    url: '/Proposal/IsProposalEdited',
                    async: false,
                    type: 'POST',
                    data: { IMS_PR_ROAD_CODE: $("#IMS_PR_ROAD_CODE").val(), IMS_TRAFFIC_TYPE: false, value: Math.random() },
                    success: function (response) {
                        unblockPage();
                        if (response.Success == true) {
                            proceedExecutionFurther = true;
                        }
                        else {
                            alert(response.Errormessage);
                            $("#MAST_BLOCK_CODE").val($("#hdnBlockCode").val());
                            return false;
                        }
                    },
                    error: function (err) {
                        alert("Error while getting processing your request.");
                        unblockPage();
                    }
                });
                /// Code Exexution Doesnt Stop so Code Repeated
                if (Boolean(proceedExecutionFurther)) {

                    $('#MAST_MP_CONST_CODE').children('option:not(:first)').remove();
                    $('#MAST_MLA_CONST_CODE').children('option:not(:first)').remove();


                    PopulateMpConstituency();
                    PopulateMLAConstituency();

                    if (!$("#rdoStageII").is(":checked")) {

                        $("#PLAN_CN_ROAD_CODE").empty();
                        $("#IMS_PAV_LENGTH").val(0);
                        $("#DUP_IMS_PAV_LENGTH").val(0);

                        //    $.ajax({
                        //        url: '/Proposal/GetOnlyLinkThrough',
                        //        type: 'POST',
                        //        beforeSend: function () {
                        //            blockPage();
                        //        },
                        //        data: { PrRoadCode: $('#IMS_PR_ROAD_CODE').val(), BlockID: $("#MAST_BLOCK_CODE").val(), IMS_UPGRADE_CONNECT: $('input:radio[name=IMS_UPGRADE_CONNECT]:checked').val(), PROPOSAL_TYPE: 'P', value: Math.random() },

                        //        success: function (jsonData) {
                        //            for (var i = 0; i < jsonData.length; i++) {
                        //                $("#PLAN_CN_ROAD_CODE").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        //            }
                        //            unblockPage();
                        //        },
                        //        error: function (err) {
                        //            alert("Error while getting Link/Through Routes.");
                        //            unblockPage();
                        //        }
                        //    });
                        $.ajax({
                            url: '/Proposal/GetLinkThroughList',
                            type: 'POST',
                            beforeSend: function () {
                                blockPage();
                            },
                            data: { BlockID: $("#MAST_BLOCK_CODE").val(), IMS_UPGRADE_CONNECT: $('input:radio[name=IMS_UPGRADE_CONNECT]:checked').val(), PROPOSAL_TYPE: 'P', value: Math.random() },

                            success: function (jsonData) {
                                for (var i = 0; i < jsonData.length; i++) {
                                    $("#PLAN_CN_ROAD_CODE").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                                }
                                unblockPage();
                            },
                            error: function (err) {
                                alert("Error while getting Link/Through Routes.");
                                unblockPage();
                            }
                        });
                    }
                }
            }

            else {


                $('#MAST_MP_CONST_CODE').children('option:not(:first)').remove();
                $('#MAST_MLA_CONST_CODE').children('option:not(:first)').remove();

                /// Code Exexution Doesnt Stop so Code Repeated
                PopulateMpConstituency();
                PopulateMLAConstituency();

                if (!$("#rdoStageII").is(":checked")) {

                    $("#PLAN_CN_ROAD_CODE").empty();
                    $("#IMS_PAV_LENGTH").val(0);
                    $("#DUP_IMS_PAV_LENGTH").val(0);

                    $.ajax({
                        url: '/Proposal/GetLinkThroughList',
                        type: 'POST',
                        beforeSend: function () {
                            blockPage();
                        },
                        data: { BlockID: $("#MAST_BLOCK_CODE").val(), IMS_UPGRADE_CONNECT: $('input:radio[name=IMS_UPGRADE_CONNECT]:checked').val(), PROPOSAL_TYPE: 'P', value: Math.random() },

                        success: function (jsonData) {
                            for (var i = 0; i < jsonData.length; i++) {
                                $("#PLAN_CN_ROAD_CODE").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                            }
                            unblockPage();
                        },
                        error: function (err) {
                            alert("Error while getting Link/Through Routes.");
                            unblockPage();
                        }
                    });
                }
            }
        } else {

            $("#PLAN_CN_ROAD_CODE").empty();
            if ($("#PMGSYScheme").val() == "2") {
                $("#PLAN_CN_ROAD_CODE").append("<option value=''>Select Candidate Road Major Rural Link/Through Route Number</option>");
            }
            else {
                $("#PLAN_CN_ROAD_CODE").append("<option value=''>Select Core Network Link/Through Route Number</option>");
            }

            $("#trProposalLength").hide('slow');
        }

    });

    ///Added by SAMMED A. PATIL for RCPLWE
    $("#IMS_COLLABORATION").change(function () {
        //if ($("#hdnOperation").val() != "U")
        {
            $("#PLAN_CN_ROAD_CODE").empty();
            
            if ($("#IMS_COLLABORATION option:selected").val() == 5) {
                $('#rdoNew').hide('slow');
                $('#lbNew').hide('slow');

                $('#rdoUpgrade').prop('checked', 'checked');
                $("#ExistingSurface").show();
                $("#trBenefittedHab").show();
                $("#rdoComplete").prop('checked', true);
                $("#trComplete").hide();

                $.ajax({
                    url: '/Proposal/PopulateLinkThroughListRCPLWE?blockCode=' + $("#MAST_BLOCK_CODE").val(),
                    type: 'POST',
                    beforeSend: function () {
                        blockPage();
                    },
                    //data: { BlockID: $("#MAST_BLOCK_CODE").val() },

                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#PLAN_CN_ROAD_CODE").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                        unblockPage();
                    },
                    error: function (err) {
                        alert("Error while getting Link/Through Routes.");
                        unblockPage();
                    }
                });
            }
            else {
                $('#rdoNew').show('slow');
                $('#lbNew').show('slow');

                $.ajax({
                    url: '/Proposal/GetLinkThroughList',
                    type: 'POST',
                    beforeSend: function () {
                        blockPage();
                    },
                    data: { BlockID: $("#MAST_BLOCK_CODE").val(), IMS_UPGRADE_CONNECT: $('input:radio[name=IMS_UPGRADE_CONNECT]:checked').val(), PROPOSAL_TYPE: 'P', value: Math.random() },

                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#PLAN_CN_ROAD_CODE").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                        unblockPage();
                    },
                    error: function (err) {
                        alert("Error while getting Link/Through Routes.");
                        unblockPage();
                    }
                });
            }
        }
    });

    $("#PLAN_CN_ROAD_CODE").change(function () {

        $("#IMS_PAV_LENGTH").removeAttr("readonly");
        $("#rdoPartialLength").removeAttr('disabled');
        $("#rdoFullLength").removeAttr('disabled');

        if ($("#PLAN_CN_ROAD_CODE").val() != "") {
            var ProceedExecutionFurther = false;
            /// Check if The Value can be Edited
            if ($("#hdnOperation").val() == "U") {
                blockPage();
                $.ajax({
                    url: '/Proposal/IsProposalEdited',
                    async: false,
                    type: 'POST',
                    data: { IMS_PR_ROAD_CODE: $("#IMS_PR_ROAD_CODE").val(), IMS_TRAFFIC_TYPE: false, value: Math.random() },
                    success: function (response) {
                        unblockPage();
                        if (response.Success == true && (response.message == 'S2')) {
                            ProceedExecutionFurther = true;
                            alert('Proposal Length has been changed. so please update the CBR Details added against this proposal.');
                        }
                        else if (response.Success == true) {
                            ProceedExecutionFurther = true;
                        }
                        else {
                            alert(response.Errormessage);
                            ProceedExecutionFurther = false;
                            $("#PLAN_CN_ROAD_CODE").val($("#hdnPlanCnRoad").val());
                            return false;
                        }
                    },
                    error: function (err) {
                        alert("Error while getting processing your request.");
                        unblockPage();
                    }
                });
            }
            else {
                ProceedExecutionFurther = true;
            }


            if (Boolean(ProceedExecutionFurther)) {
                blockPage();

                $("#trProposalLength").show('slow');

                $.ajax({
                    url: '/Proposal/GetRoadDetails',
                    type: 'POST',
                    data: { PLAN_CN_ROAD_CODE: $("#PLAN_CN_ROAD_CODE").val(), IMS_STAGE_PHASE: $("#rdoStageII").is(":checked"), IMS_PR_ROAD_CODE: $("#IMS_PR_ROAD_CODE").val(), IMS_STAGED_ROAD_ID: $("#IMS_STAGED_ROAD_ID").val(), value: Math.random() },
                    success: function (response) {
                        unblockPage();

                        if (response.Success) {

                            if (response.IMS_PAV_LENGTH != "-999") {

                                $("#IMS_PAV_LENGTH").val(response.IMS_PAV_LENGTH);
                                $("#DUP_IMS_PAV_LENGTH").val(response.IMS_PAV_LENGTH);


                                //alert($("#DUP_IMS_PAV_LENGTH").val());

                                $("#IMS_CC_LENGTH").val("");
                                $("#IMS_BT_LENGTH").val("");

                                if (response.IMS_PARTIAL_LEN != "") {
                                    $("#IMS_PARTIAL_LEN").val(response.IMS_PARTIAL_LEN);
                                }

                                if ($("#rdoStageII").is(":checked")) {
                                    $("#rdoPartialLength").attr('disabled', 'disabled');
                                    $("#rdoFullLength").attr('disabled', 'disabled');
                                }

                                if ($("#rdoStageII").is(":checked")) {
                                    //$("#IMS_PAV_LENGTH").attr("readonly", "readonly");

                                    //added by Shyam, for Stage II proposal, for validation on % length of Stage I Length
                                    $("#IMS_STAGE1_PAV_LENGTH").val(response.IMS_STAGE1_PAV_LENGTH);

                                    if (response.IMS_ROAD_FROM != undefined && response.IMS_ROAD_FROM != "") {
                                        $("#IMS_ROAD_FROM").val(response.IMS_ROAD_FROM);
                                    }
                                    if (response.IMS_ROAD_TO != undefined && response.IMS_ROAD_TO != "") {
                                        $("#IMS_ROAD_TO").val(response.IMS_ROAD_TO);
                                    }
                                }
                                if (response.IMS_PARTIAL_LEN != "") {
                                    $('input:radio[name="IMS_PARTIAL_LEN"]').filter('[value="' + response.IMS_PARTIAL_LEN + '"]').attr('checked', true);
                                }
                            }
                        }
                        else {
                            alert('There is an error occured while processing your request.');
                            return false;
                        }
                    },
                    error: function (err) {
                        alert("error " + err);
                        unblockPage();
                    }
                });
            }
        }
        else {
            $("#trProposalLength").hide('slow');
        }
    });

    $('#btnCreate').click(function (evt) {
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

        if ($('#frmCreateProposal').valid()) {
            if (validate()) {
                if ($("#hdnOperation").val() == "C") {

                    $("#rdoPartialLength").removeAttr('disabled');
                    $("#rdoFullLength").removeAttr('disabled');

                    $("#IMS_CC_LENGTH").removeAttr("disabled");
                    $("#IMS_BT_LENGTH").removeAttr("disabled");

                    $.ajax({
                        url: '/Proposal/Create',
                        type: "POST",
                        cache: false,
                        data: $("#frmCreateProposal").serialize(),
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
                                alert("Proposal Created Successfully.");

                                //LoadProposals(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE);

                                //LoadProposals($("#ddlImsYear").val(), $("#ddlMastBlockCode").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), "P");

                                //$("#tbLSBProposalList").jqGrid('GridUnload');

                                //$("#tbProposalList").trigger("reloadGrid");

                                ResetForm();
                                CloseProposalDetails();
                                unblockPage();
                            }
                            else {
                                $("#divError").show("slow");
                                $("#divError span:eq(1)").html(response.ErrorMessage);
                                $('#mainDiv').animate({ scrollTop: 0 }, 'slow');
                                unblockPage();

                                if ($("#rdoStageI").is(":checked")) {

                                    $("#IMS_CC_LENGTH").attr("disabled", "disabled");
                                    $("#IMS_BT_LENGTH").attr("disabled", "disabled");
                                }
                            }

                        }
                    });
                }

                if ($("#hdnOperation").val() == "U") {

                    $("#rdoNew").removeAttr("disabled");
                    $("#rdoUpgrade").removeAttr("disabled");

                    $("#rdoStaged").removeAttr("disabled");
                    $("#rdoComplete").removeAttr("disabled");

                    $("#rdoStageII").removeAttr("disabled");
                    $("#rdoStageI").removeAttr("disabled");

                    $("#IMS_CC_LENGTH").removeAttr("disabled");
                    $("#IMS_BT_LENGTH").removeAttr("disabled");

                    $("#rdoFullLength").removeAttr("disabled");
                    $("#rdoPartialLength").removeAttr("disabled");

                    $.ajax({
                        url: '/Proposal/Edit',
                        type: "POST",
                        cache: false,
                        data: $("#frmCreateProposal").serialize(),
                        beforeSend: function () {
                            blockPage();
                        },
                        error: function (xhr, status, error) {
                            unblockPage();
                            Alert("Request can not be processed at this time,please try after some time!!!");
                            return false;
                        },
                        success: function (response) {


                            if (response.Success) {
                                alert("Proposal Updated Successfully.");

                                //$("#tbProposalList").trigger("reloadGrid");
                                ResetForm();
                                CloseProposalDetails();
                                unblockPage();
                            }
                            else {

                                // Change on 01 Sept 2020
                                unblockPage();
                                $('#mainDiv').animate({ scrollTop: 0 }, 'slow');

                                $("#divError").show("slow");

                                $.validator.unobtrusive.parse($('#mainDiv'));

                                Alert("Request can not be processed at this time,please try after some time!!!");



                                //unblockPage();
                                //$('#mainDiv').animate({ scrollTop: 0 }, 'slow');

                                //$("#divError").show("slow");
                                //$("#divError span:eq(1)").html(response.ErrorMessage);
                                //$.validator.unobtrusive.parse($('#mainDiv'));

                                if ($("#rdoStageI").is(":checked")) {


                                    $("#IMS_STAGED_ROAD_ID").attr("disabled", "disabled");
                                    $("#Stage_2_Year").attr("disabled", "disabled");
                                    $("#Stage_2_Package_ID").attr("disabled", "disabled");
                                    $("#IMS_CC_LENGTH").attr("disabled", "disabled");
                                    $("#IMS_BT_LENGTH").attr("disabled", "disabled");
                                }
                            }
                            unblockPage();
                        }
                    });
                }

            }
            else {
                return false;
            }
        }
        else {

            //$('#mainDiv').animate({ scrollTop: 0 }, 'slow');
            //$('.qtip').show();
        }

    });

    if ($("#rdoYesHab").is(":checked")) {
        $("#tdHabReasonLabel").hide("slow");
        $("#tdHabReasonText").hide("slow");
    }

    if ($("#rdoNoHab").is(":checked")) {
        $("#tdHabReasonLabel").show("slow");
        $("#tdHabReasonText").show("slow");
    }

    $("#rdoYesHab").click(function () {
        $("#tdHabReasonLabel").hide("slow");
        $("#tdHabReasonText").hide("slow");
    });

    $("#rdoNoHab").click(function () {


        if ($("#hdnOperation").val() == "U") {
            blockPage();
            $.ajax({
                url: '/Proposal/IsHabitationsBenifitted',
                async: false,
                type: 'POST',
                data: { IMS_PR_ROAD_CODE: $("#IMS_PR_ROAD_CODE").val(), value: Math.random() },
                success: function (response) {
                    unblockPage();
                    if (response.Success == true) {

                    }
                    else {
                        $('input[name=IMS_ISBENEFITTED_HABS]:nth(0)').attr("checked", "checked");
                        alert(response.Errormessage);
                        return false;
                    }
                },
                error: function (err) {
                    alert("Error while getting processing your request.");
                    unblockPage();
                }
            });
        }

        if ($(this).is(":checked")) {
            $("#tdHabReasonLabel").show("slow");
            $("#tdHabReasonText").show("slow");
        }
        else {
            $("#tdHabReasonLabel").hide("slow");
            $("#tdHabReasonText").hide("slow");
        }

    });

    if ($("#rdoNewPackage").is(":checked")) {
        $("#trPackageText").show();
        $("#trPackageddl").hide();
    }

    if ($("#rdoOldPackage").is(":checked")) {
        $("#trPackageText").hide();
        $("#trPackageddl").show();
    }

    $("#rdoNewPackage").click(function () {
        $("#trPackageText").show("slow");
        $("#trPackageddl").hide("slow");
        $("#EXISTING_IMS_PACKAGE_ID").empty();
    });

    $("#rdoOldPackage").click(function () {

        if ($("#IMS_YEAR").val() == 0 || $("#IMS_BATCH").val() == 0) {
            alert('Please Select Year and  Batch');
            $("#rdoNewPackage").prop('checked', true);
            $("#trPackageText").show("slow");

            $('#EXISTING_IMS_PACKAGE_ID').children('option:not(:first)').remove();
            return;
        }
        else {
            $("#IMS_PACKAGE_ID").val("");

            $("#trPackageText").hide("slow");
            $("#trPackageddl").show("slow");


            $("#EXISTING_IMS_PACKAGE_ID").empty();
            $("#EXISTING_IMS_PACKAGE_ID").append("<option value='0'>Select Package</option>");

            //PopulatePakage($("#IMS_YEAR").val(), $("#IMS_BATCH").val(), 'EXISTING_IMS_PACKAGE_ID' );
            PopulateExistingPackages($("#IMS_YEAR").val(), $("#IMS_BATCH").val());

        }
    });

    $("#IMS_BATCH").change(function () {

        if ($("#IMS_BATCH").val() != 0 && ("#IMS_YEAR") != 0 && $("#rdoOldPackage").is(":checked")) {
            //PopulatePakage($("#IMS_YEAR").val(), $("#IMS_BATCH").val(), 'EXISTING_IMS_PACKAGE_ID', false);

            PopulateExistingPackages($("#IMS_YEAR").val(), $("#IMS_BATCH").val());
        }

        else if ($("#IMS_YEAR").val() == 0 || $("#IMS_BATCH").val() == 0) {
            $('#EXISTING_IMS_PACKAGE_ID').children('option:not(:first)').remove();
        }

    });

    $("#IMS_YEAR").change(function () {
        validateFinancialYear();
        if ($("#IMS_YEAR").val() != 0 && $("#IMS_BATCH").val() != 0 && $("#rdoOldPackage").is(":checked")) {

            PopulateExistingPackages($(this).val(), $("#IMS_BATCH").val());
        }
        else if ($("#IMS_YEAR").val() == 0 || $("#IMS_BATCH").val() == 0) {

            $('#EXISTING_IMS_PACKAGE_ID').children('option:not(:first)').remove();
        }

        // Populate the Batches
        if ($("#IMS_YEAR").val() > 0) {

            $("#IMS_BATCH").empty();
            // alert("test");

            $.ajax({
                url: '/Proposal/PoulateUnFreezedBatches',
                type: 'POST',
                beforeSend: function () {
                    blockPage();
                },
                data: { IMS_YEAR: $("#IMS_YEAR").val(), value: Math.random() },
                success: function (jsonData) {
                    unblockPage();
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#IMS_BATCH").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    if (jsonData.length == 1) {
                        alert("All the Batches against selected year are Freezed.");
                        return false;
                    }
                },
                error: function (err) {
                    alert("error " + err);
                    unblockPage();
                }
            });
        }
        else {
            $('#IMS_BATCH').children('option:not(:first)').remove();
        }
    });


    $("#Stage_2_Package_ID").change(function () {
        if ($("#Stage_2_Package_ID").val() != 0 && $("Stage_2_Year").val() != 0) {
            GetStagedProposalList($("#Stage_2_Year").val(), $("#Stage_2_Package_ID").val());
            $("#trStagedRoad").show();
        }


    });

    $("#IMS_STAGED_ROAD_ID").change(function () {

        $('#PLAN_CN_ROAD_CODE').children('option:not(:first)').remove();

        if ($("#IMS_STAGED_ROAD_ID").val() != 0) {
            GetStagedLinkThroughList($("#Stage_2_Year").val(), $("#IMS_BATCH").val(), $("#Stage_2_Package_ID").val(), $("#IMS_STAGED_ROAD_ID").val());
        }
    });

    if ($('#IMS_PR_ROAD_CODE').val() != null) {
        CalculateStateShareMordShareTotalCost();
    }
    YearChangeRoad2015();
    $('#IMS_YEAR').change(function () {

        YearChangeRoad2015();
        if (parseInt($(this).val()) < parseInt(2015)) {

            $('#lblPavCost').html('Pavement cost (MORD Share)');
            $('#lblCDCost').html('CD Works cost (MORD Share)');
            $('#lblPWCost').html('Protection cost (MORD Share)');
            $('#lblOWCost').html('Other Works cost (MORD Share)');

            if ($('#PMGSYScheme').val() == 2) {
                $('#rdoSharePercent2015SchemeII').show();
                $('#lblShareScheme2').show();
                $('#trOldSharePercent').show();
            }
            else {
                $('#rdoSharePercent2015SchemeII').hide();
                $('#lblShareScheme2').hide();
                $('#trOldSharePercent').hide();
            }

        }
        else {
            $('#lblPavCost').html('Pavement cost (Total)');
            $('#lblCDCost').html('CD Works cost (Total)');
            $('#lblPWCost').html('Protection cost (Total)');
            $('#lblOWCost').html('Other Works cost (Total)');

            if ($('#PMGSYScheme').val() == 2) {
                $('#rdoSharePercent2015SchemeII').hide();
                $('#lblShareScheme2').hide();
                $('#trOldSharePercent').hide();
            }
            else {
                $('#rdoSharePercent2015SchemeII').show();
                $('#lblShareScheme2').show();
                $('#trOldSharePercent').show();
            }
        }

    });


    if ($('#PMGSYScheme').val() == 2 && $('#hdnOperation').val() == 'U') {
        //$('#IMS_YEAR').trigger('change');
    }

    // FOR Calculating the Total of Maintenance cost
    $(":text[class~=TMC]").blur(function () {
        var fltTPE = 0.0;
        $(":text[class~=TMC]").each(function () {
            var tempVal = $(this).val().replace(new RegExp("\,", "g"), "");
            if (Number(tempVal) != NaN) {
                fltTPE += Number(tempVal);
                $("#txtTotalMaintenance").val(parseFloat(fltTPE).toFixed(2));
            }
        });
    });

    $('#IMS_CC_LENGTH').blur(function () {
        CalculatePavementLength();
    });

    $("#IMS_BT_LENGTH").blur(function () {
        CalculatePavementLength();
    });


    if ($("#rdoPartialLength").is(":checked")) {
        $("#IMS_PAV_LENGTH").removeAttr('readonly');
    }

    $("#rdoPartialLength").click(function () {
        $("#IMS_PAV_LENGTH").removeAttr('readonly');
    });

    $("#rdoFullLength").click(function () {
        $("#IMS_PAV_LENGTH").val($("#DUP_IMS_PAV_LENGTH").val());
        //$("#IMS_PAV_LENGTH").attr('readonly', 'readonly');
        alert("Core Network Length: " + $("#DUP_IMS_PAV_LENGTH").val() + ", If Road Length is greater than CN Length than change Pavement Length");
    });

    $("#IMS_TRAFFIC_TYPE").change(function () {

        if ($("#hdnOperation").val() == "U") {
            blockPage();
            $.ajax({
                url: '/Proposal/IsProposalEdited',
                type: 'POST',
                data: { IMS_PR_ROAD_CODE: $("#IMS_PR_ROAD_CODE").val(), IMS_TRAFFIC_TYPE: true, value: Math.random() },
                success: function (response) {
                    unblockPage();
                    if (response.Success == true && reponse.message == 'S2') {

                    }
                    else if (response.Success == true) {

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
        }
        unblockPage();

    });

    $('#rdoSharePercent2015No,#rdoSharePercent2015Yes,#rdoSharePercent2015SchemeII,#rdoSharePercent2015Old').click(function () {

        CalculateStateShareMordShareTotalCost();

    });

    $('#IMS_SANCTIONED_CD_AMT,#IMS_SANCTIONED_PW_AMT,#IMS_SANCTIONED_OW_AMT,#IMS_SANCTIONED_PAV_AMT,#IMS_STATE_SHARE,#IMS_HIGHER_SPECIFICATION_COST').blur(function () {

        CalculateStateShareMordShareTotalCost();
    });

    $('#IMS_FURNITURE_COST').blur(function () {
        CalculateStateShareMordShareTotalCost();

        if (parseInt($('#IMS_YEAR').val()) < 2015) {
            $("#rdoSharePercentNo").trigger('click');
            $('#trImsShare').show('slow');
        }
    });

    $("#rdoSharePercentNo").trigger('click');

    //$("input[name='IMS_SHARE_PERCENT_2015']").each(function () {

    //    var value;

    //    if ($('#' + $(this).attr('id')).is(':checked')) {
    //        value = $('#' + $(this).attr('id')).val();
    //    }
    //    console.log('Value= ' + value);
    //    switch (parseInt(value)) {
    //        case 1:
    //            $("#lblStateShare").html("(25% of Total Cost)");
    //            $("#lblMordShare").html("(75% of Total Cost)");
    //            break;
    //        case 2:
    //            $("#lblStateShare").html("(10% of Total Cost)");
    //            $("#lblMordShare").html("(90% of Total Cost)");
    //            break;
    //        case 3:
    //            $("#lblStateShare").html("(40% of Total Cost)");
    //            $("#lblMordShare").html("(60% of Total Cost)");
    //            break;
    //        case 4:
    //            $("#lblStateShare").html("(0% of Total Cost)");
    //            $("#lblMordShare").html("(100% of Total Cost)");
    //            break;
    //        default:
    //            break;
    //    }

    //});



})

function setCurrentFinancialYear() {

    var currentYearRd = (new Date).getFullYear();
    var currentMonthRd = (new Date).getMonth() + 1;
    var currentDayRd = (new Date).getDate();

    var currFinancialYearRd = parseInt(currentMonthRd) <= 3 ? parseInt(currentYearRd - 1) : parseInt(currentYearRd);

    if ($('#IMS_YEAR').val() < currFinancialYearRd) {
        $('#IMS_YEAR').val(currFinancialYearRd);
    }
}

function validateFinancialYear() {

    var currentYearRd = (new Date).getFullYear();
    var currentMonthRd = (new Date).getMonth() + 1;
    var currentDayRd = (new Date).getDate();

    var currFinancialYearRd = parseInt(currentMonthRd) <= 3 ? parseInt(currentYearRd - 1) : parseInt(currentYearRd);

    if (parseInt($('#IMS_YEAR').val()) >= parseInt(currFinancialYearRd)) {
        //$('#btnAddProposal').show('slow');
    }
    else {
        //$('#btnAddProposal').hide('slow');
        //alert("Cannot select previous years");
        alert("Proposal entry for current Financial Year and onwards is allowed");
        $('#IMS_YEAR').val(currFinancialYearRd);
    }
}

function YearChangeRoad2015() {
    if ($('#IMS_YEAR').val() >= parseInt(2015)) {
        $('#trOldSharePercent').hide('slow');
        $('#trImsShare').val('');
        $('#trImsShare').hide('slow');
        //$("#rdoSharePercentNo").trigger('click');

        //Set Share % from dataBase
        //Old Code 
        //$('#rdoSharePercent2015Old').prop('checked', true);

        //New code added by Abhisehk kamble25Jan2015
        $.ajax({
            url: '/Proposal/GetShareCodeByStateCode',
            type: 'GET',
            success: function (jsonData) {
                // alert("test : " + jsonData.ShareCode);

                var ShareCode = jsonData.ShareCode;

                if (ShareCode != 0) {
                    if (ShareCode == 2)//2-	90 : 10
                    {
                        $('#rdoSharePercent2015No').prop('checked', true);
                    }
                    else if (ShareCode == 3)//3-	60 : 40
                    {
                        $('#rdoSharePercent2015Old').prop('checked', true);
                    }
                    else if (ShareCode == 4)//4-	100 : 0
                    {
                        $('#rdoSharePercent2015Yes').prop('checked', true);
                    }
                }
            },
            error: function (err) {
                alert("error " + err);
            }
        });


    }
    else {
        $('#trOldSharePercent').show('slow');
        $('#trImsShare').show('slow');
        $("#rdoSharePercentNo").trigger('click');

        //alert($('#hdnshareCode').val() == 0);
        if ($('#hdnshareCode').val() == 0 || $('#hdnshareCode').val() == 3) {
            $('#rdoSharePercent2015Old').prop('checked', true);
        }
        else if ($('#hdnshareCode').val() == 2) {
            $('#rdoSharePercent2015No').prop('checked', true);
        }
        else if ($('#hdnshareCode').val() == 4) {
            $('#rdoSharePercent2015Yes').prop('checked', true);
        }
    }
}

function CalculateStateShareMordShareTotalCost() {

    //alert("test");
    var pavCost = isNaN($('#IMS_SANCTIONED_PAV_AMT').val()) ? parseFloat("0") : parseFloat($('#IMS_SANCTIONED_PAV_AMT').val());
    var cdCost = isNaN($('#IMS_SANCTIONED_CD_AMT').val()) ? parseFloat("0") : parseFloat($('#IMS_SANCTIONED_CD_AMT').val());
    var pwCost = isNaN($('#IMS_SANCTIONED_PW_AMT').val()) ? parseFloat("0") : parseFloat($('#IMS_SANCTIONED_PW_AMT').val());
    var owCost = isNaN($('#IMS_SANCTIONED_OW_AMT').val()) ? parseFloat("0") : parseFloat($('#IMS_SANCTIONED_OW_AMT').val());
    var stateCost = isNaN($('#IMS_STATE_SHARE').val()) ? parseFloat("0") : parseFloat($('#IMS_STATE_SHARE').val());
    var higherSpecCost = 0;
    var furnitureCost = 0;
    if ($('#PMGSYScheme').val() == 2) {
        furnitureCost = isNaN($('#IMS_FURNITURE_COST').val()) ? parseFloat("0") : parseFloat($('#IMS_FURNITURE_COST').val());
        higherSpecCost = isNaN(parseFloat($('#IMS_HIGHER_SPECIFICATION_COST').val())) ? parseFloat("0") : parseFloat($('#IMS_HIGHER_SPECIFICATION_COST').val());
        //console.log('higherSpecCost= ' + parseFloat($('#IMS_HIGHER_SPECIFICATION_COST').val()));
    }

    var totalCost = parseFloat(pavCost) + parseFloat(cdCost) + parseFloat(pwCost) + parseFloat(owCost);

    if ($('#PMGSYScheme').val() == 2) {
        totalCost = parseFloat(furnitureCost) + parseFloat(totalCost);
        console.log('New ' + totalCost);
        console.log(furnitureCost);
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

    var totalCostToUpdate = parseFloat(parseFloat(totalCost) + parseFloat(stateCost)).toFixed(2);

    if ($('#PMGSYScheme').val() == 2) {

        totalCostToUpdate = parseFloat(totalCostToUpdate) + parseFloat(higherSpecCost);

        totalCostToUpdate = parseFloat(pavCost) + parseFloat(cdCost) + parseFloat(pwCost) + parseFloat(owCost) + parseFloat(furnitureCost) + parseFloat(higherSpecCost);
    }

    //console.log(parseFloat(higherSpecCost));

    var totalStateShare = isNaN(parseFloat($('#IMS_STATE_SHARE_2015').val())) ? parseFloat(0) : parseFloat($('#IMS_STATE_SHARE_2015').val());
    //console.log('totalStateShare ' + totalStateShare);
    $('#IMS_TOTAL_STATE_SHARE_2015').val(parseFloat(/*parseFloat(stateCost) +*/(totalStateShare) + parseFloat(higherSpecCost)).toFixed(2));
    $('#IMS_TOTAL_COST_2015').val(parseFloat(totalCostToUpdate).toFixed(2));

    //console.log('totalStateShare ' + $('#IMS_TOTAL_STATE_SHARE_2015').val());
    //console.log('totalCostToUpdate ' + totalCostToUpdate);
    if (($('#PMGSYScheme').val() == 1 || $("#PMGSYScheme").val() == 3)) {
        $('#IMS_TOTAL_STATE_SHARE_2015').val(parseFloat(parseFloat(stateCost) + parseFloat($('#IMS_STATE_SHARE_2015').val())).toFixed(2));
    }
}

function CalculatePavementLength() {

    //return true;

    //if (!isNaN($("#IMS_CC_LENGTH").val()) && !isNaN($("#IMS_BT_LENGTH").val()) && $.trim($("#IMS_CC_LENGTH").val()) != "" && $.trim($("#IMS_BT_LENGTH").val()) != "") {
    //    if ($("#IMS_PAV_LENGTH").val() != parseFloat($("#IMS_CC_LENGTH").val()) + parseFloat($("#IMS_BT_LENGTH").val())) {

    //        alert("Sum Of CC Length and BT Length Should be equal to Pavement Length.");
    //        return false;
    //    }
    //}
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
        return true;
    }
    else {
        return true;
    }
}

function ResetForm() {
    $(':input', '#frmCreateProposal').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');
    //$('.qtip').hide();

    $("#trStages").hide();
    $("#rdoStageI").prop('checked', false);
    $("#rdoStageII").prop('checked', false);
    $("#trStageII").hide();
    $("#trStagedRoad").hide();

    $("#trPackageText").show("slow");
    $("#trPackageddl").hide("slow");
    $("#EXISTING_IMS_PACKAGE_ID").empty();

    $("#PLAN_CN_ROAD_CODE").children('option:not(:first)').remove();
    $("#MAST_MP_CONST_CODE").children('option:not(:first)').remove();
    $("#MAST_MLA_CONST_CODE").children('option:not(:first)').remove();

    //$('#mainDiv').animate({ scrollTop: 0 }, 'slow');    
}

function validate() {
    if (CalculatePavementLength()) {
        return true;
    }
    else {
        return false;
    }
}

// This Function Populates Existing Packages
function PopulateExistingPackages(IMS_YEAR, IMS_BATCH) {



    if (IMS_YEAR == 0 || IMS_BATCH == 0) {
        $('#IMS_BATCH').children('option:not(:first)').remove();
        return false;
    }
    else {

        $.ajax({
            url: '/Proposal/GetExistingPackage',
            type: 'POST',
            beforeSend: function () {
                blockPage();
            },
            data: { Year: IMS_YEAR, BatchID: IMS_BATCH, value: Math.random() },
            success: function (jsonData) {
                unblockPage();
                if (jsonData.Success == false) {
                    alert(jsonData.ErrorMessage);
                    return false;
                }
                if (jsonData.length == 1) {
                    $("#EXISTING_IMS_PACKAGE_ID").children('option:not(:first)').remove();
                    alert("No Package found for Selected Year and Batch");
                }
                else {
                    $("#EXISTING_IMS_PACKAGE_ID").empty();
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#EXISTING_IMS_PACKAGE_ID").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }
            },
            error: function (err) {
                alert("error " + err);
                unblockPage();
            }
        });
    }
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

function GetStagedProposalList(paramYear, paramPackageID) {

    $("#IMS_PAV_LENGTH").val(0);
    $("#DUP_IMS_PAV_LENGTH").val(0);

    $('#IMS_STAGED_ROAD_ID').children('option:not(:first)').remove();
    $('#PLAN_CN_ROAD_CODE').children('option:not(:first)').remove();



    $.ajax({
        url: '/Proposal/GetStagedProposalList',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        data: { year: paramYear, packageID: paramPackageID, value: Math.random() },
        success: function (jsonData) {


            if (jsonData.length == 0) {
                alert("No Stage1 Proposal found for Selected Year and Package.");
                unblockPage();
            }
            for (var i = 0; i < jsonData.length; i++) {
                $("#IMS_STAGED_ROAD_ID").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
            unblockPage();
        },
        error: function (request, status, error) {

            unblockPage();
        }

    });
}



function GetStagedLinkThroughList(paramYear, paramBatch, paramPackageID, paramStageRoad) {


    //    $("#PLAN_CN_ROAD_CODE").val(0);
    //    $("#PLAN_CN_ROAD_CODE").empty();
    //    $("#PLAN_CN_ROAD_CODE").append("<option value='0'>Select Core Network Link/Through Route Number</option>");
    $("#IMS_PAV_LENGTH").val(0);
    $("#DUP_IMS_PAV_LENGTH").val(0);

    $('#PLAN_CN_ROAD_CODE').children('option:not(:first)').remove();



    $.ajax({
        url: '/Proposal/GetStagedLinkThroughList',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        data: { year: paramYear, batch: paramBatch, packageID: paramPackageID, stageRoad: paramStageRoad, value: Math.random() },
        success: function (jsonData) {

            $("#PLAN_CN_ROAD_CODE").empty();
            if (jsonData.length == 0) {
                alert("No Link/Through Route found for Selected Year and Package.");
                unblockPage();
            }
            for (var i = 0; i < jsonData.length; i++) {
                $("#PLAN_CN_ROAD_CODE").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
            $("#PLAN_CN_ROAD_CODE").trigger('change');
            unblockPage();
        },
        error: function (request, status, error) {

            unblockPage();
        }

    });
}


function PopulateMpConstituency() {

    $('#MAST_MP_CONST_CODE').empty();

    $.ajax({
        url: '/Proposal/PoulateMPConstituency',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        data: { MAST_BLOCK_CODE: $("#MAST_BLOCK_CODE").val(), value: Math.random() },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
                $("#MAST_MP_CONST_CODE").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
            unblockPage();
        },
        error: function (err) {
            alert("error " + err);
            unblockPage();
        }
    });

}

function PopulateMLAConstituency() {

    $('#MAST_MLA_CONST_CODE').empty();

    $.ajax({
        url: '/Proposal/PoulateMLAConstituency',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        data: { MAST_BLOCK_CODE: $("#MAST_BLOCK_CODE").val(), value: Math.random() },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
                $("#MAST_MLA_CONST_CODE").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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

    $("#trStagedRoad").hide();

    $('#PLAN_CN_ROAD_CODE').children('option:not(:first)').remove();
    $('#Stage_2_Year').val(0);
    $('#Stage_2_Package_ID').children('option:not(:first)').remove();
    $('#IMS_STAGED_ROAD_ID').children('option:not(:first)').remove();
    $('#rdoFullLength').removeAttr('disabled');
    $('#rdoOldPackage').removeAttr('disabled');


    $("#MAST_BLOCK_CODE").val(0);
    $("#IMS_PAV_LENGTH").val(0);
    $("#DUP_IMS_PAV_LENGTH").val(0);
    $("#IMS_ROAD_FROM").val("");
    $("#IMS_ROAD_TO").val("");

    $("#IMS_CC_LENGTH").val("");
    $("#IMS_BT_LENGTH").val("");
}

