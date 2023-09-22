// For Existing Road (UpGradation)
$.validator.unobtrusive.adapters.add('isexisting', ['imsexisting'], function (options) {
    options.rules['isexisting'] = options.params;
    options.messages['isexisting'] = options.message;
});

$.validator.addMethod("isexisting", function (value, element, params) {

    var IsUpgradtion = $("#rdoExistingRoad").val(); //$('input:radio[name="' + params.imsupgrade + '"]:checked').val();



    if (IsUpgradtion == "U") {
        if (value == null || value == "" || value == "0") {
            return false;
        }
    }
    return true;
});



// For Non Existing(New) Road
$.validator.unobtrusive.adapters.add('isnonexisting', ['imsnonexisting'], function (options) {
    options.rules['isnonexisting'] = options.params;
    options.messages['isnonexisting'] = options.message;
});

$.validator.addMethod("isnonexisting", function (value, element, params) {

    var IsUpgradtion = $("#rdoNonExistingRoad").val();// $('input:radio[name="' + params.imsupgrade + '"]:checked').val();

    if (IsUpgradtion == "N") {
        if (value == null || value == "" || value == "0") {
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

    if (isExistingPackage == "U") {
        if (value == null || value == "" || value == "0") {
            return false;
        }
    }
    return true;
});


//PMGSY Scheme 2
// For Higher specification cost
$.validator.unobtrusive.adapters.add('ishigherspeccostvalid', ['ishigherspec'], function (options) {
    options.rules['ishigherspeccostvalid'] = options.params;
    options.messages['ishigherspeccostvalid'] = options.message;
});

$.validator.addMethod("ishigherspeccostvalid", function (value, element, params) {
    var IsHigherSpec = $('input:radio[name="' + params.ishigherspec + '"]:checked').val();
    //PMGSY3
    if ($("#PMGSYScheme").val() == "2" || $("#PMGSYScheme").val() == "4") {
        if (IsHigherSpec == "Y") {
            if (value == null || parseFloat(value) <= 0) {
                return false;
            }
        }
    }
    return true;
});



$(document).ready(function () {

  //  alert("Here ")

    setCurrentFinancialYearLSB();
    //PMGSY3
    if ($("#PMGSYScheme").val() == "2" || $("#PMGSYScheme").val() == "4") {
        YearChangeGreater2015();
    }

    //alert($('#IMS_YEAR').val());
    //if ($('#IMS_YEAR').val() >= parseInt('2015')) {
    //    $('#rdoSharePercent2015Old').prop('checked', true);
    //}
    //else {
    //    if ($('#hdnshareCodeLSB').val() == 0 || $('#hdnshareCodeLSB').val() == 3) {
    //        $('#rdoSharePercent2015Old').prop('checked', true);
    //    }
    //    else if ($('#hdnshareCodeLSB').val() == 2) {
    //        $('#rdoSharePercent2015No').prop('checked', true);
    //    }
    //    else if ($('#hdnshareCodeLSB').val() == 4) {
    //        $('#rdoSharePercent2015Yes').prop('checked', true);
    //    }
    //    if ($('#PMGSYScheme').val() == 2 && $('#hdnshareCodeLSB').val() == 1) {
    //        $('#rdoSharePercent2015SchemeII').prop('checked', true);
    //    }
    //}

    $("input[type='reset']").on("click", function (event) {
        $("#tblNonExistingRoads").hide("slow");
        $("#tblExistingRoads").show("slow");
        $("#trPackageText").show("slow");
        $("#trPackageddl").hide("slow");
    });

    $.validator.unobtrusive.parse($('frmCreateProposalLSB'));
    $("#trPackageID").removeClass("ui-state-hover");

    // Update Operation
    if ($("#hdnOperation").val() == "U") {
        $("#btnCreate").attr("value", "Update");
        $("#btnResetLSB").hide();
        $("#btnCancel").show();
    } else if ($("#hdnOperation").val() == "C") {
        $("#btnCreate").attr("value", "Save");
        $("#btnCancel").hide();
        $("#btnResetLSB").show();
    }

    $("#btnCancel").click(function () {
        CloseProposalDetails();
    });
    //--------- rdo package check change function -----------------//
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

        if ($("#IMS_YEAR").val() == 0 || $("#IMS_BATCH").val() == 0) { // || $("#IMS_COLLABORATION").val() == 0  ) {
            alert('Please Select Year and  Batch');
            $("#rdoNewPackage").prop('checked', true);
            $("#trPackageText").show("slow");
            return;
        }
        else {
            $("#IMS_PACKAGE_ID").val("");

            $("#trPackageText").hide("slow");
            $("#trPackageddl").show("slow");
            //$("#trBenefittedHab").show("slow");

            $("#EXISTING_IMS_PACKAGE_ID").empty();
            $("#EXISTING_IMS_PACKAGE_ID").append("<option value='0'>Select Package</option>");

            PopulatePakage($("#IMS_YEAR").val(), $("#IMS_BATCH").val(), EXISTING_IMS_PACKAGE_ID);

        }
    });
    //--------- rdo package check change function -----------------//



    //--------- rdo existing road check change function -----------------//
    if ($("#rdoExistingRoad").is(":checked")) {
        $("#tblExistingRoads").show();
        $("#tblNonExistingRoads").hide();

        if ($("#hdnOperation").val() == "C") {
            $("#IMS_ROAD_NAME").val("");
        }
    }

    if ($("#rdoNonExistingRoad").is(":checked")) {
        $("#tblExistingRoads").hide();
        $("#tblNonExistingRoads").show();
    }

    $("#rdoExistingRoad").click(function () {
        $("#IMS_STAGED_YEAR").val("");

        $("#tblNonExistingRoads").hide("slow");
        $("#tblExistingRoads").show("slow");


        if ($("#MAST_BLOCK_CODE").val() == 0 || $("#MAST_BLOCK_CODE").val() == "") {
            $("#IMS_STAGED_YEAR").empty();
            $("#IMS_STAGED_YEAR").append("<option value='0'>Select Year</option>");
        }

        $("#IMS_STAGED_PACKAGE_ID").empty();
        $("#IMS_STAGED_ROAD_ID").empty();
        $("#IMS_STAGED_PACKAGE_ID").append("<option value='0'>Select Package</option>");
        $("#IMS_STAGED_ROAD_ID").append("<option value='0'>Select Road</option>");

        //PopulateYear($("#IMS_YEAR").val(), $("#IMS_BATCH").val(), EXISTING_IMS_PACKAGE_ID);
    });

    ///Changed by SAMMED A. PATIL for RCPLWE
    if ($('#hdnOperation').val() == 'U' && ($("#PMGSYScheme").val() == '1' || $("#PMGSYScheme").val() == '3') && $('#IMS_COLLABORATION').val() == '5') {

        setTimeout(function () {
            //$("#rdoExistingRoad").trigger('click');
            $("#tblNonExistingRoads").hide("slow");
            $("#tblExistingRoads").show("slow");
            $("#rdoExistingRoad").prop("checked", true);
        });
    }

    $("#rdoNonExistingRoad").click(function () {

        $("#PLAN_CN_ROAD_CODE").val("");

        $("#tblExistingRoads").hide("slow");
        $("#tblNonExistingRoads").show("slow");

        //


        if ($("#MAST_BLOCK_CODE").val() == 0 || $("#MAST_BLOCK_CODE").val() == "") {
            $('#PLAN_CN_ROAD_CODE').children('option:not(:first)').remove();
            //$("#PLAN_CN_ROAD_CODE").empty();
        }
        //if (parseInt($('#PLAN_CN_ROAD_CODE option').size()) > 1) {
        //    $('#PLAN_CN_ROAD_CODE').children('option:not(:first)').remove();
        //}
        // $("#PLAN_CN_ROAD_CODE").empty();

        $("#IMS_ROAD_FROM").val('');
        $("#IMS_ROAD_TO").val('');
    });
    //--------- rdo package check change function -----------------//
    if ($('#PMGSYScheme').val() == 1 || $("#PMGSYScheme").val() == '3')
    {
        $("#TotalEstimatedCost,#IMS_BRIDGE_WORKS_EST_COST,#IMS_BRIDGE_EST_COST_STATE").blur(function () {

           // alert("Check3")

         //   alert("IMS_BRIDGE_EST_COST_STATE Check3    BS = " + $("#IMS_BRIDGE_EST_COST_STATE").val())

          //  alert("IMS_BRIDGE_WORKS_EST_COST Check3    BW =" + $("#IMS_BRIDGE_WORKS_EST_COST").val())

            $("#TotalEstimatedCost").val(parseFloat($('#IMS_BRIDGE_WORKS_EST_COST').val()) + parseFloat($('#IMS_BRIDGE_EST_COST_STATE').val()));

            $("input[name='IMS_SHARE_PERCENT_2015']").each(function () {

                var value;

                if ($('#' + $(this).attr('id')).is(':checked')) {
                    value = $('#' + $(this).attr('id')).val();
                }
                //console.log('Value= ' + value);
                switch (parseInt(value)) {
                    case 1:
                        //$("#lblStateShare").html("(25% of Total Cost)");
                        //$("#lblMordShare").html("(75% of Total Cost)");
                        $("#IMS_STATE_SHARE_2015").val(parseFloat(.25 * $("#IMS_BRIDGE_WORKS_EST_COST").val()).toFixed(3));
                        $("#IMS_MORD_SHARE_2015").val(parseFloat(.75 * $("#IMS_BRIDGE_WORKS_EST_COST").val()).toFixed(3));
                        break;
                    case 2:
                        //$("#lblStateShare").html("(10% of Total Cost)");
                        //$("#lblMordShare").html("(90% of Total Cost)");
                        $("#IMS_STATE_SHARE_2015").val(parseFloat(.1 * $("#IMS_BRIDGE_WORKS_EST_COST").val()).toFixed(3));
                        $("#IMS_MORD_SHARE_2015").val(parseFloat(.9 * $("#IMS_BRIDGE_WORKS_EST_COST").val()).toFixed(3));
                        break;
                    case 3:
                        //$("#lblStateShare").html("(40% of Total Cost)");
                        //$("#lblMordShare").html("(60% of Total Cost)");
                        $("#IMS_STATE_SHARE_2015").val(parseFloat(.4 * $("#IMS_BRIDGE_WORKS_EST_COST").val()).toFixed(3));
                        $("#IMS_MORD_SHARE_2015").val(parseFloat(.6 * $("#IMS_BRIDGE_WORKS_EST_COST").val()).toFixed(3));
                        break;
                    case 4:
                        //$("#lblStateShare").html("(0% of Total Cost)");
                        //$("#lblMordShare").html("(100% of Total Cost)");
                        $("#IMS_STATE_SHARE_2015").val(parseFloat(0 * $("#IMS_BRIDGE_WORKS_EST_COST").val()).toFixed(3));
                        $("#IMS_MORD_SHARE_2015").val(parseFloat(1 * $("#IMS_BRIDGE_WORKS_EST_COST").val()).toFixed(3));
                        break;
                    default:
                        break;
                }
                var vl = parseFloat($("#IMS_STATE_SHARE_2015").val()) + parseFloat($('#IMS_BRIDGE_EST_COST_STATE').val());
                $("#IMS_TOTAL_STATE_SHARE_2015").val(parseFloat(vl).toFixed(3));

                var totCost = parseFloat($("#IMS_STATE_SHARE_2015").val()) + parseFloat($('#IMS_MORD_SHARE_2015').val()) + parseFloat($('#IMS_BRIDGE_EST_COST_STATE').val());
                //console.log('totCost1= ' + totCost);
                $("#IMS_TOTAL_COST_2015").val(parseFloat(totCost).toFixed(3));

            });
        });
    }

    //---------------- PMGSY 2  Starts Here----------------------------//

    $("#rdoHigherSpecificationYes").click(function () {
        if ($(this).is(":checked")) {
            $("#trHigherSpecCost").show();
        }
    });

    $("#rdoHigherSpecificationNo").click(function () {
        if ($(this).is(":checked")) {
            $("#trHigherSpecCost").hide();
            $('#IMS_HIGHER_SPECIFICATION_COST').val(0);
            calculateTotalCost();
        }
    });

    $("#rdoSharePercentYes").click(function () {
        if ($(this).is(":checked")) {
            $("#lblStateShare").html("(10% of Total Cost)");
            $("#lblMordShare").html("(90% of Total Cost)");

            var tempVal = $("#TotalEstimatedCost").val().replace(new RegExp("\,", "g"), "");
            if (Number(tempVal) != NaN) {
                totalCost = parseFloat($("#TotalEstimatedCost").val()).toFixed(2);
            }
            var totalCost90Percent = parseFloat((totalCost * 90) / 100).toFixed(2);
            var totalCost10Percent = parseFloat((totalCost * 10) / 100).toFixed(2);

            $("#IMS_BRIDGE_WORKS_EST_COST").val(totalCost90Percent);
            $("#IMS_BRIDGE_EST_COST_STATE").val(totalCost10Percent);

        }
    });

    $("#rdoSharePercentNo").click(function () {
        if ($(this).is(":checked")) {
            $("#lblStateShare").html("(25% of Total Cost)");
            $("#lblMordShare").html("(75% of Total Cost)");

            var tempVal = $("#TotalEstimatedCost").val().replace(new RegExp("\,", "g"), "");
            if (Number(tempVal) != NaN) {
                totalCost = parseFloat($("#TotalEstimatedCost").val()).toFixed(2);
            }

            var totalCost75Percent = parseFloat((totalCost * 75) / 100).toFixed(2);
            var totalCost25Percent = parseFloat((totalCost * 25) / 100).toFixed(2);

            $("#IMS_BRIDGE_WORKS_EST_COST").val(totalCost75Percent);
            $("#IMS_BRIDGE_EST_COST_STATE").val(totalCost25Percent);
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
    //PMGSY3
    $("#TotalEstimatedCost").blur(function () {

        if ($('#PMGSYScheme').val() == 2 || $('#PMGSYScheme').val() == 4) {
            var totalCost = 0;

            var tempVal = $(this).val().replace(new RegExp("\,", "g"), "");
            if (Number(tempVal) != NaN) {
                totalCost = parseFloat($(this).val()).toFixed(2);
            }

            // set State Share & Mord share
            // Yes means 90/10 share & No means 75/25
            //if ($("#rdoSharePercentYes").is(":checked")) {

            //    var totalCost90Percent = parseFloat((totalCost * 90) / 100).toFixed(2);
            //    var totalCost10Percent = parseFloat((totalCost * 10) / 100).toFixed(2);

            //    $("#IMS_BRIDGE_WORKS_EST_COST").val(totalCost90Percent);
            //    $("#IMS_BRIDGE_EST_COST_STATE").val(totalCost10Percent);

            //} else if ($("#rdoSharePercentNo").is(":checked")) {
            //    var totalCost75Percent = parseFloat((totalCost * 75) / 100).toFixed(2);
            //    var totalCost25Percent = parseFloat((totalCost * 25) / 100).toFixed(2);

            //    $("#IMS_BRIDGE_WORKS_EST_COST").val(totalCost75Percent);
            //    $("#IMS_BRIDGE_EST_COST_STATE").val(totalCost25Percent);
            //}


            //if ($('#rdoSharePercent2015No').is(':checked')) {
            //    var totalCost90Percent = parseFloat((totalCost * 90) / 100).toFixed(2);
            //    var totalCost10Percent = parseFloat((totalCost * 10) / 100).toFixed(2);

            //    $("#IMS_BRIDGE_WORKS_EST_COST").val(totalCost90Percent);
            //    $("#IMS_BRIDGE_EST_COST_STATE").val(totalCost10Percent);

            //}
            //else if ($('#rdoSharePercent2015Yes').is(':checked')) {
            //    var totalCost90Percent = parseFloat((totalCost * 100) / 100).toFixed(2);
            //    var totalCost10Percent = parseFloat((totalCost * 0) / 100).toFixed(2);

            //    $("#IMS_BRIDGE_WORKS_EST_COST").val(totalCost90Percent);
            //    $("#IMS_BRIDGE_EST_COST_STATE").val(totalCost10Percent);
            //}
            //else if ($('#rdoSharePercent2015SchemeII').is(':checked')) {
            //    var totalCost90Percent = parseFloat((totalCost * 75) / 100).toFixed(2);
            //    var totalCost10Percent = parseFloat((totalCost * 25) / 100).toFixed(2);

            //    $("#IMS_BRIDGE_WORKS_EST_COST").val(totalCost90Percent);
            //    $("#IMS_BRIDGE_EST_COST_STATE").val(totalCost10Percent);
            //}
            //else if ($('#rdoSharePercent2015Old').is(':checked')) {
            //    var totalCost90Percent = parseFloat((totalCost * 60) / 100).toFixed(2);
            //    var totalCost10Percent = parseFloat((totalCost * 40) / 100).toFixed(2);

            //    $("#IMS_BRIDGE_WORKS_EST_COST").val(totalCost90Percent);
            //    $("#IMS_BRIDGE_EST_COST_STATE").val(totalCost10Percent);
            //}

            if ($('#rdoSharePercentYes').is(':checked')) {
                var totalCost90Percent = parseFloat((totalCost * 90) / 100).toFixed(2);
                var totalCost10Percent = parseFloat((totalCost * 10) / 100).toFixed(2);

                $("#IMS_BRIDGE_WORKS_EST_COST").val(totalCost90Percent);
                $("#IMS_BRIDGE_EST_COST_STATE").val(totalCost10Percent);

            }
            else if ($('#rdoSharePercentNo').is(':checked')) {
                var totalCost90Percent = parseFloat((totalCost * 75) / 100).toFixed(2);
                var totalCost10Percent = parseFloat((totalCost * 25) / 100).toFixed(2);

                $("#IMS_BRIDGE_WORKS_EST_COST").val(totalCost90Percent);
                $("#IMS_BRIDGE_EST_COST_STATE").val(totalCost10Percent);
            }

            var higherSpecCost = parseFloat($("#IMS_HIGHER_SPECIFICATION_COST").val()).toFixed(2);
            if (higherSpecCost == NaN || higherSpecCost == 'NaN') {
                higherSpecCost = 0;
            }

            $("#TotalCostWithHigherSpecCost").val((parseFloat(totalCost)) + (parseFloat(higherSpecCost)));
            //set Higher Specification Cost
        }
    });


    //$("#IMS_HIGHER_SPECIFICATION_COST").blur(function () {
    //    var totalCost = 0;
    //    var tempVal = $("#TotalEstimatedCost").val().replace(new RegExp("\,", "g"), "");
    //    if (Number(tempVal) != NaN) {
    //        totalCost = parseFloat($("#TotalEstimatedCost").val()).toFixed(2);
    //    }

    //    var higherSpecCost = parseFloat($("#IMS_HIGHER_SPECIFICATION_COST").val()).toFixed(2);
    //    $("#TotalCostWithHigherSpecCost").val((parseFloat(totalCost)) + (parseFloat(higherSpecCost)));
    //});

    $("#TotalEstimatedCost").blur(function () {
        //var totalCost = 0;
        //var tempVal = $("#TotalEstimatedCost").val().replace(new RegExp("\,", "g"), "");
        //if (Number(tempVal) != NaN) {
        //    totalCost = parseFloat($("#TotalEstimatedCost").val()).toFixed(2);
        //}

        //var higherSpecCost = parseFloat($("#IMS_HIGHER_SPECIFICATION_COST").val()).toFixed(2);
        //$("#TotalCostWithHigherSpecCost").val((parseFloat(totalCost)) + (parseFloat(higherSpecCost)));
        if ($('#PMGSYScheme').val() == 2 || $('#PMGSYScheme').val() == 4) {//PMGSY3
            calculateTotalCost();
        }
    });
    //---------------- PMGSY 2 Ends Here----------------------------//




    //--------- On YEAR & BATCH change reset Existing Package dropdown-----------------//
    $("#IMS_YEAR").change(function () {
        YearChangeGreater2015();

        $("#rdoNewPackage").prop('checked', true);
        $("#trPackageText").show();
        $("#trPackageddl").hide();

        $("#EXISTING_IMS_PACKAGE_ID").empty();
        $("#EXISTING_IMS_PACKAGE_ID").append("<option value='0'>Select Package</option>");
    });

    $("#IMS_BATCH").change(function () {
        $("#rdoNewPackage").prop('checked', true);
        $("#trPackageText").show();
        $("#trPackageddl").hide();

        $("#EXISTING_IMS_PACKAGE_ID").empty();
        $("#EXISTING_IMS_PACKAGE_ID").append("<option value='0'>Select Package</option>");



    });
    //--------- On YEAR & BATCH change -----------------//



    // populate the Packages for Existing Road
    $("#IMS_STAGED_YEAR").change(function () {

        if ($("#MAST_BLOCK_CODE").val() == 0) {
            alert("Please select Block");
            return;
        }

        if ($("#IMS_STAGED_YEAR").val() == 0) {
            $('#IMS_STAGED_PACKAGE_ID').children('option:not(:first)').remove();
            $('#IMS_STAGED_ROAD_ID').children('option:not(:first)').remove();
        }
        else {

            PopulatePakage($("#IMS_STAGED_YEAR").val(), 0, IMS_STAGED_PACKAGE_ID, false, false);
        }
    });

    $("#IMS_COLLABORATION").change(function () {
        if ($('#IMS_STAGED_YEAR option:selected').val() > 0) {
            $("#IMS_STAGED_YEAR").trigger('change');
        }
    });

    //Staged Package Change
    $("#IMS_STAGED_PACKAGE_ID").change(function () {
        //alert($("#IMS_STAGED_PACKAGE_ID").val());
        $('#IMS_STAGED_ROAD_ID').children('option:not(:first)').remove();

        ///Changed by SAMMED A. PATIL for RCPLWE
        //if ($('#IMS_COLLABORATION option:selected').val() == 5) {

        //}
        //else {
        if ($("#IMS_STAGED_PACKAGE_ID").val() != null || $("#IMS_STAGED_PACKAGE_ID").val() != "") {

            if ($("#IMS_STAGED_ROAD_ID").length > 0) {
                $.ajax({
                    url: '/LSBProposal/GetStagedRoadList',
                    type: 'POST',
                    beforeSend: function () {
                        blockPage();
                    },
                    //data: { BlockID: $("#MAST_BLOCK_CODE").val(), Year: $("#IMS_STAGED_YEAR").val(), PackageID: $("#IMS_STAGED_PACKAGE_ID").val(), value: Math.random() }, commented by Vikram 
                    //new code added by Vikram as suggested by Dev sir - if staged year and proposal year is same then skip IMS_SANCTIONED = Y
                    data: { BlockID: $("#MAST_BLOCK_CODE").val(), StageYear: $("#IMS_STAGED_YEAR option:selected").val(), PackageID: $("#IMS_STAGED_PACKAGE_ID").val(), Year: $("#IMS_YEAR option:selected").val(), value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#IMS_STAGED_ROAD_ID").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                        unblockPage();

                    },
                    error: function (err) {
                        alert("error " + err);
                        unblockPage();
                    }
                });
            }
            //}
        }
    });


    $("#MAST_BLOCK_CODE").change(function () {

        $('#IMS_STAGED_YEAR').children('option:not(:first)').remove();
        $('#IMS_STAGED_ROAD_ID').children('option:not(:first)').remove();
        $('#IMS_STAGED_PACKAGE_ID').children('option:not(:first)').remove();
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
            $('#lbselBlock').text($("#MAST_BLOCK_CODE option:selected").text());
            $("#PLAN_CN_ROAD_CODE").empty();

            /*if ($("#rdoNonExistingRoad").is(":checked")) {
                $("#PLAN_CN_ROAD_CODE").empty();
            }*/

            if ($("#PLAN_CN_ROAD_CODE").length > 0) {
                $.ajax({
                    url: '/Proposal/GetLinkThroughList',
                    type: 'POST',
                    beforeSend: function () {
                        blockPage();
                    },
                    data: { BlockID: $("#MAST_BLOCK_CODE").val(), IMS_UPGRADE_CONNECT: $('input:radio[name=isExistingRoad]:checked').val(), PROPOSAL_TYPE: 'L', value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#PLAN_CN_ROAD_CODE").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                        unblockPage();
                    },
                    error: function (err) {
                        alert("error " + err);
                        unblockPage();
                    }
                });
            }

            PopulateStagedYear($("#MAST_BLOCK_CODE").val(), 0, "IMS_STAGED_YEAR", false, false);


        }
        else {
            $('#lbselBlock').text('');
        }

    });//Block code change ends here

    // set value to Road Name TextBox 
    $("#IMS_STAGED_ROAD_ID").change(function () {

        if ($("#IMS_STAGED_ROAD_ID").val() > 0 || $("#IMS_STAGED_ROAD_ID").val() != "" || $("#IMS_STAGED_ROAD_ID").val() != null) {

            $('#IMS_ROAD_NAME').val('');
            $('#IMS_ROAD_NAME').val($("#IMS_STAGED_ROAD_ID :selected").text());
        }
    });


    // set value to Road Name TextBox 
    $("#PLAN_CN_ROAD_CODE").change(function () {

        if ($("#PLAN_CN_ROAD_CODE").val() > 0 || $("#PLAN_CN_ROAD_CODE").val() != "" || $("#PLAN_CN_ROAD_CODE").val() != null) {

            $('#IMS_ROAD_NAME').val('');
            $('#IMS_ROAD_NAME').val($("#PLAN_CN_ROAD_CODE :selected").text());
        }
    });



    //button Create Click
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
        else if ($('#rdoSharePercent2015Old').is(':checked'))
        {
        //  alert("Check1")

               
            //IMS_BRIDGE_WORKS_EST_COST -- IMS_SANCTIONED_BW_AMT -- 60
            //IMS_BRIDGE_EST_COST_STATE -- IMS_SANCTIONED_BS_AMT   -- 40



            $('#rdoSharePercent2015Old').attr('disabled', false);




            var tCost = $('#TotalEstimatedCost').val();

         //   alert("Cost Now " + tCost)

            if ($("#PMGSYScheme").val() == 4)
            {

                $('#TotalEstimatedCost').val(parseFloat(tCost).toFixed(3));

                var totalCost75Percent = parseFloat((tCost * 60) / 100).toFixed(2);
                var totalCost25Percent = parseFloat((tCost * 40) / 100).toFixed(2);

                $("#IMS_BRIDGE_WORKS_EST_COST").val(totalCost75Percent); // 60
                $("#IMS_BRIDGE_EST_COST_STATE").val(totalCost25Percent); //40

                $("#IMS_SANCTIONED_BW_AMT").val(totalCost75Percent); //60
                $("#IMS_SANCTIONED_BS_AMT").val(totalCost25Percent); // 40

             //   alert("BW " + totalCost75Percent)
             //   alert("BS " + totalCost25Percent)
            }


        }

        if ($('#frmCreateProposalLSB').valid()) {
            // if (validate()) {
            if ($("#hdnOperation").val() == "C") {
                $.ajax({
                    url: '/LSBProposal/CreateLSB',
                    type: "POST",
                    cache: false,
                    data: $("#frmCreateProposalLSB").serialize(),
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
                            alert("Proposal Created Succesfully.");
                            //LoadProposals($("#ddlImsYear").val(), $("#ddlMastBlockCode").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), "L");
                            CloseProposalDetails();
                        }
                        else {
                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                        }
                        unblockPage();
                    }
                });
            }

            if ($("#hdnOperation").val() == "U") {
                $.ajax({
                    url: '/LSBProposal/EditLSB',
                    type: "POST",
                    cache: false,
                    data: $("#frmCreateProposalLSB").serialize(),
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
                            alert("Proposal Updated Succesfully.");
                            //$("#tbLSBProposalList").trigger("reloadGrid");
                            CloseProposalDetails();
                        }
                        else {

                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);

                            $.validator.unobtrusive.parse($('#mainDiv'));
                        }
                        unblockPage();
                    }
                });
            }
        }
        else {
            $('.qtip').show();
        }
    });//btnCreate ends here



    //-----  Calculate Toatal Estimated Cost -------
    //$('#IMS_BRIDGE_WORKS_EST_COST').blur(function () {
    //    calculateTotalCost();
    //});

    //$('#IMS_BRIDGE_EST_COST_STATE').blur(function () {
    //    calculateTotalCost();
    //});

    //------------------------------------------

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

    $('#IMS_HIGHER_SPECIFICATION_COST').blur(function () {
        calculateTotalCost();
        //CalculateStateShareMordShareTotalCostForLSB();

    });

    $('#rdoSharePercent2015No,#rdoSharePercent2015Yes,#rdoSharePercent2015SchemeII,#rdoSharePercent2015Old').click(function () {

      
        CalculateStateShareMordShareTotalCostForLSB();

    });

    $('#IMS_YEAR').change(function () {
        validateFinancialYearLSB();
        if (parseInt($(this).val()) < parseInt(2015)) {
            //PMGSY3
            if ($('#PMGSYScheme').val() == 2 || $('#PMGSYScheme').val() == 4) {
                $('#rdoSharePercent2015SchemeII').show();
                $('#lblShareScheme2').show();
            }
            else {
                $('#rdoSharePercent2015SchemeII').hide();
                $('#lblShareScheme2').hide();
            }

            //if ($('#hdnshareCodeLSB').val() == 0 || $('#hdnshareCodeLSB').val() == 3) {
            //    $('#rdoSharePercent2015Old').prop('checked', true);
            //}
            //else if ($('#hdnshareCodeLSB').val() == 2) {
            //    $('#rdoSharePercent2015No').prop('checked', true);
            //}
            //else if ($('#hdnshareCodeLSB').val() == 4) {
            //    $('#rdoSharePercent2015Yes').prop('checked', true);
            //}
            //if ($('#PMGSYScheme').val() == 2 && $('#hdnshareCodeLSB').val() == 1) {
            //    $('#rdoSharePercent2015SchemeII').prop('checked', true);
            //}
        }
        else {//PMGSY3
            if ($('#PMGSYScheme').val() == 2 || $('#PMGSYScheme').val() == 4) {
                $('#rdoSharePercent2015SchemeII').hide();
                $('#lblShareScheme2').hide();
            }
            else {
                $('#rdoSharePercent2015SchemeII').show();
                $('#lblShareScheme2').show();
            }

            //  $('#rdoSharePercent2015Old').prop('checked', true);
        }

    });

    if ($('#IMS_PR_ROAD_CODE').val() != null) {
        //CalculateStateShareMordShareTotalCostForLSB();
        if ($('#PMGSYScheme').val() == 1 || $("#PMGSYScheme").val() == '3') {

        }
        //else {
        //    setTimeout(function () {

        //        calculateTotalCost();
        //    });
        //}
        $('#TotalEstimatedCost').trigger('blur');
    }



});//doc.ready ends here

function YearChangeGreater2015() {
    if ($('#IMS_YEAR').val() >= parseInt('2015')) {
        //$('#trHigherSpecCost').hide('slow');
        $('#trImsPercent').hide('slow');
        //$('#trTotEstimatedCost').hide('slow');
        $('#trImsBridgeWorksEst').hide('slow');
        $('#trImsBridgeWorksEstState').hide('slow');
        $('#trTotCostHighSpCost').hide('slow');

        //$('#rdoSharePercent2015Old').prop('checked', true);
    }
    else {
        //$('#trHigherSpecCost').show('slow');
        $('#trImsPercent').show('slow');
        //$('#trTotEstimatedCost').show('slow');
        $('#trImsBridgeWorksEst').show('slow');
        $('#trImsBridgeWorksEstState').show('slow');
        $('#trTotCostHighSpCost').show('slow');

        //if ($('#hdnshareCodeLSB').val() == 0 || $('#hdnshareCodeLSB').val() == 3) {
        //    $('#rdoSharePercent2015Old').prop('checked', true);
        //}
        //else if ($('#hdnshareCodeLSB').val() == 2) {
        //    $('#rdoSharePercent2015No').prop('checked', true);
        //}
        //else if ($('#hdnshareCodeLSB').val() == 4) {
        //    $('#rdoSharePercent2015Yes').prop('checked', true);
        //}
    }
}

function CalculateStateShareMordShareTotalCostForLSB() {

   
    var mordCost = isNaN($('#IMS_BRIDGE_WORKS_EST_COST').val()) ? parseFloat("0") : parseFloat($('#IMS_BRIDGE_WORKS_EST_COST').val());
    var stateCost = isNaN($('#IMS_BRIDGE_EST_COST_STATE').val()) ? parseFloat("0") : parseFloat($('#IMS_BRIDGE_EST_COST_STATE').val());
    var higherSpecCost = 0;
    //PMGSY3
    if ($('#PMGSYScheme').val() == 2 || $('#PMGSYScheme').val() == 4) {
        higherSpecCost = isNaN($('#IMS_HIGHER_SPECIFICATION_COST').val()) ? parseFloat("0") : parseFloat($('#IMS_HIGHER_SPECIFICATION_COST').val());
    }

    var totalCost = parseFloat(mordCost) + parseFloat(stateCost);

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

    var totalCostToUpdate = parseFloat(parseFloat(totalCost)).toFixed(2);
    //PMGSY3
    if ($('#PMGSYScheme').val() == 2 || $('#PMGSYScheme').val() == 4) {

        totalCostToUpdate = parseFloat(totalCostToUpdate) + parseFloat(higherSpecCost);
    }

    $('#IMS_TOTAL_STATE_SHARE_2015').val(parseFloat(/*parseFloat(stateCost) + */parseFloat($('#IMS_STATE_SHARE_2015').val()) + parseFloat(higherSpecCost)).toFixed(2));
    $('#IMS_TOTAL_COST_2015').val(totalCostToUpdate);

}



function calculateTotalCost() {

    // Added

    //$('#TotalEstimatedCost').val('');
    var totalCost = (parseFloat($('#IMS_BRIDGE_WORKS_EST_COST').val()) + parseFloat($('#IMS_BRIDGE_EST_COST_STATE').val()));

  //  alert("totalCost" + totalCost)

    if (isNaN(totalCost)) {
        $('#TotalEstimatedCost').val(0);
    }
    else {
      //  $('#TotalEstimatedCost').val(parseFloat(totalCost).toFixed(3));
     //   alert("Check2")
        var tCost = $('#TotalEstimatedCost').val();

       // totalCost = parseFloat($("#TotalEstimatedCost").val()).toFixed(2);
        $('#TotalEstimatedCost').val(parseFloat(tCost).toFixed(3));

        var totalCost75Percent = parseFloat((tCost * 60) / 100).toFixed(2);
        var totalCost25Percent = parseFloat((tCost * 40) / 100).toFixed(2);

        $("#IMS_BRIDGE_WORKS_EST_COST").val(totalCost75Percent);  // 60
        $("#IMS_BRIDGE_EST_COST_STATE").val(totalCost25Percent); // 40

        $("#IMS_SANCTIONED_BW_AMT").val(totalCost75Percent); // 60
        $("#IMS_SANCTIONED_BS_AMT").val(totalCost25Percent); // 40
       

    //    alert("Cost =" + tCost);
    }
    // End


    var higherSpecCost = parseFloat($("#IMS_HIGHER_SPECIFICATION_COST").val()).toFixed(2);
    higherSpecCost = isNaN(higherSpecCost) ? parseFloat(0) : parseFloat(higherSpecCost).toFixed(2);
    $("#TotalCostWithHigherSpecCost").val((parseFloat(totalCost)) + (parseFloat(higherSpecCost)));

    $("input[name='IMS_SHARE_PERCENT_2015']").each(function () {

        var value;

        if ($('#' + $(this).attr('id')).is(':checked')) {
            value = $('#' + $(this).attr('id')).val();
        }
        //console.log('Value= ' + value);
        switch (parseInt(value)) {
            case 1:
                //$("#lblStateShare").html("(25% of Total Cost)");
                //$("#lblMordShare").html("(75% of Total Cost)");
                $("#IMS_STATE_SHARE_2015").val(parseFloat(.25 * $("#TotalEstimatedCost").val()).toFixed(3));
                $("#IMS_MORD_SHARE_2015").val(parseFloat(.75 * $("#TotalEstimatedCost").val()).toFixed(3));
                break;
            case 2:
                //$("#lblStateShare").html("(10% of Total Cost)");
                //$("#lblMordShare").html("(90% of Total Cost)");
                $("#IMS_STATE_SHARE_2015").val(parseFloat(.1 * $("#TotalEstimatedCost").val()).toFixed(3));
                $("#IMS_MORD_SHARE_2015").val(parseFloat(.9 * $("#TotalEstimatedCost").val()).toFixed(3));
                break;
            case 3:
                //$("#lblStateShare").html("(40% of Total Cost)");
                //$("#lblMordShare").html("(60% of Total Cost)");
                $("#IMS_STATE_SHARE_2015").val(parseFloat(.4 * $("#TotalEstimatedCost").val()).toFixed(3));
                $("#IMS_MORD_SHARE_2015").val(parseFloat(.6 * $("#TotalEstimatedCost").val()).toFixed(3));
                break;
            case 4:
                //$("#lblStateShare").html("(0% of Total Cost)");
                //$("#lblMordShare").html("(100% of Total Cost)");
                $("#IMS_STATE_SHARE_2015").val(parseFloat(0 * $("#TotalEstimatedCost").val()).toFixed(3));
                $("#IMS_MORD_SHARE_2015").val(parseFloat(1 * $("#TotalEstimatedCost").val()).toFixed(3));
                break;
            default:
                break;
        }
        var vl = parseFloat($("#IMS_STATE_SHARE_2015").val()) + parseFloat(higherSpecCost);
        console.log('v1= ' + vl);
        $("#IMS_TOTAL_STATE_SHARE_2015").val(parseFloat(vl).toFixed(3));
        //alert($("#IMS_TOTAL_STATE_SHARE_2015").val());

        var totCost = parseFloat($("#IMS_STATE_SHARE_2015").val()) + parseFloat(higherSpecCost) + parseFloat($('#IMS_MORD_SHARE_2015').val());
        //console.log('totCost= ' + totCost);
        $("#IMS_TOTAL_COST_2015").val(parseFloat(totCost).toFixed(3));

    });
}

function setCurrentFinancialYearLSB() {

    var currentYearLSB = (new Date).getFullYear();
    var currentMonthLSB = (new Date).getMonth() + 1;
    var currentDayLSB = (new Date).getDate();

    var currFinancialYearLSB = parseInt(currentMonthLSB) <= 3 ? parseInt(currentYearLSB - 1) : parseInt(currentYearLSB);

    if ($('#IMS_YEAR').val() < currFinancialYearLSB) {
        $('#IMS_YEAR').val(currFinancialYearLSB);
    }
}

function validateFinancialYearLSB() {

    var currentYearLSB = (new Date).getFullYear();
    var currentMonthLSB = (new Date).getMonth() + 1;
    var currentDayLSB = (new Date).getDate();

    var currFinancialYearLSB = parseInt(currentMonthLSB) <= 3 ? parseInt(currentYearLSB - 1) : parseInt(currentYearLSB);

    if (parseInt($('#IMS_YEAR').val()) >= parseInt(currFinancialYearLSB)) {
        //$('#btnAddProposal').show('slow');
    }
    else {
        //$('#btnAddProposal').hide('slow');
        //alert("Cannot select previous years");
        if ($("#StateCodeForComparision").val() == 30)
        {/// Allowed only for Sikkim
           // alert("Go On " + $("#StateCodeForComparision").val())
        }
        else
        {
        alert("Proposal entry for current Financial Year and onwards is allowed 12!@!");
        $('#IMS_YEAR').val(currFinancialYearLSB);
    }
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


    if ($('#IMS_COLLABORATION option:selected').val() == 5) {
        $.ajax({
            url: '/LSBProposal/GetPackageIDForLSBRCPLWE',
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
                    $("#" + $(DropDownName).attr("ID")).append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
                unblockPage();
            },
            error: function (err) {
                alert("error " + err);
                unblockPage();
            }

        });
    }
    else {
        $.ajax({
            url: '/LSBProposal/GetPackageIDForLSB',
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
                    $("#" + $(DropDownName).attr("ID")).append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
                unblockPage();
            },
            error: function (err) {
                alert("error " + err);
                unblockPage();
            }

        });
    }
}




function PopulateStagedYear(paramYearID, ParamBatchID, DropDownName, paramShowAlert, populateFirstItem) {

    if (paramShowAlert == null) {
        paramShowAlert = true;
    }

    if (typeof populateFirstItem === "undefined" || populateFirstItem === null) {
        populateFirstItem = true;
    }


    if (populateFirstItem) {
        $("#" + $(DropDownName).attr("ID")).val(0);
        $("#" + $(DropDownName).attr("ID")).empty();
        $("#" + $(DropDownName).attr("ID")).append("<option value='0'>Select Year</option>");
    }
    else {
        // $("#" + $(DropDownName).attr("ID")).empty();
        $("#" + $(DropDownName).attr("ID")).children('option:not(:first)').remove();
    }


    if ($("#MAST_BLOCK_CODE").val() > 0) {
        $.ajax({
            url: '/LSBProposal/GetStagedYearList',
            type: 'POST',
            beforeSend: function () {
                blockPage();
            },
            data: { value: Math.random() },
            success: function (jsonData) {
                if (jsonData.length == 0) {
                    if (paramShowAlert) {
                        alert("Error while populating years");
                    }
                    //unblockPage();
                }
                for (var i = 0; i < jsonData.length; i++) {
                    $("#" + DropDownName).append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
                unblockPage();
            },
            error: function (err) {
                alert("error " + err);
                unblockPage();
            }
        });//ajax ends here
    }
}