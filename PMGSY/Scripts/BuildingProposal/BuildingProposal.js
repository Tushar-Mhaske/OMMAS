
// For UpGradation
$.validator.unobtrusive.adapters.add('isupgradation', ['imsupgrade'], function (options) {
    options.rules['isupgradation'] = options.params;
    options.messages['isupgradation'] = options.message;
});

$.validator.addMethod("isupgradation", function (value, element, params) {
    var IsUpgradtion = $('input:radio[name="' + params.imsupgrade + '"]:checked').val();

    if (IsUpgradtion == "U") {
        if (value == null || value == "" || value== "0") {
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

    if (IsHabitationBenifitted  == "N") {
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

// for maintenance cost validation in stage II proposal
$.validator.unobtrusive.adapters.add('istotalmaintanancecostvalid', ['imsstagephase'], function (options) {
    options.rules['istotalmaintanancecostvalid'] = options.params;
    options.messages['istotalmaintanancecostvalid'] = options.message;
});

$.validator.addMethod("istotalmaintanancecostvalid", function (value, element, params) {

    var isStageTwo = $('input:radio[name="' + params.imsstagephase + '"]:checked').val();
    if (isStageTwo == "2") {
        if (value == null || value == "" || parseFloat(value) == 0 ) {
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



$(document).ready(function () {
    
    $.validator.unobtrusive.parse($('frmCreateBuildingProposal'));

    $("input[type='reset']").on("click", function (event) {
       
        ResetForm();        
    });

    //alert($("#rdoStageI").is(":checked"));
   
   

    // Update Operation
    if ($("#hdnOperation").val() == "U") {

        $("#btnCreate").attr("value", "Update");
        $("#btnReset").hide();
        $("#btnCancel").show();
        
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

    if($("#rdoUpgrade").is(":checked")){
        $("#ExistingSurface").show();
        $("#trBenefittedHab").show();
        $("#rdoComplete").prop('checked', true);
        $("#trComplete").hide();

        //$("#MAST_EXISTING_SURFACE_CODE").rules("add", "required");
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


    
    // just for the demos, avoids form submit
    jQuery.validator.setDefaults({
        debug: true,
        success: "valid"
    });
    var form = $('#frmCreateBuildingProposal');
    form.validate();
    

    $('#btnCreate').click(function (evt) {       
        //evt.preventDefault();
        
       // alert($('#frmCreateBuildingProposal').valid());
        if ($('#frmCreateBuildingProposal').valid()) {

            if ($("#hdnOperation").val() == "C") {
                // alert("OK: " + $("#frmCreateBuildingProposal").serialize());
               
                $.ajax({
                    url: '/BuildingProposal/BuildingCreate',
                    type: "POST",
                    cache: false,
                    data: $("#frmCreateBuildingProposal").serialize(),
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
                            alert("Proposal Created Succesfully.");

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
            else if ($("#hdnOperation").val() == "U") {


                $.ajax({
                    url: '/BuildingProposal/BuildingEdit',
                    type: "POST",
                    cache: false,
                    data: $("#frmCreateBuildingProposal").serialize(),
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
                            alert(response.ErrorMessage);

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
        }
        else {
            return false;
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

        if ($("#IMS_YEAR").val() == 0 || $("#IMS_BATCH").val() == 0 ) { 
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
        if ($("#IMS_BATCH").val() != 0 && ("#IMS_YEAR") != 0  && $("#rdoOldPackage").is(":checked")) {
            //PopulatePakage($("#IMS_YEAR").val(), $("#IMS_BATCH").val(), 'EXISTING_IMS_PACKAGE_ID', false);
            PopulateExistingPackages($("#IMS_YEAR").val(), $("#IMS_BATCH").val());
        }
        else if ($("#IMS_YEAR").val() == 0 || $("#IMS_BATCH").val() == 0) {
            $('#EXISTING_IMS_PACKAGE_ID').children('option:not(:first)').remove();
        }

    });

    $("#IMS_YEAR").change(function () {        
        if ($("#IMS_YEAR").val() != 0 && $("#IMS_BATCH").val() != 0 && $("#rdoOldPackage").is(":checked")) {
            
            PopulateExistingPackages($(this).val(), $("#IMS_BATCH").val());
        }
        else if ( $("#IMS_YEAR").val() == 0 || $("#IMS_BATCH").val() == 0 ){

            $('#EXISTING_IMS_PACKAGE_ID').children('option:not(:first)').remove();
        }

        // Populate the Batches
        if ($("#IMS_YEAR").val() > 0) {

            $("#IMS_BATCH").empty();
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
    

   

    // FOR Calculating the Total of Maintenance cost
    $(":text[class~=TMC]").blur(function () {
        var fltTPE = 0.0;
        $(":text[class~=TMC]").each(function () {
            var tempVal = $(this).val().replace(new RegExp("\,", "g"), "");
            if (Number(tempVal) != NaN) {
                fltTPE += Number(tempVal);
                $("#txtTotalMaintenance").val( parseFloat(fltTPE).toFixed(2) );
            }
        });
    });

})

function ResetForm()
{
    $(':input', '#frmCreateProposal').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');
    //$('.qtip').hide();

    
    $("#trPackageText").show("slow");
    $("#trPackageddl").hide("slow");
    $("#EXISTING_IMS_PACKAGE_ID").empty();

    $("#PLAN_CN_ROAD_CODE").children('option:not(:first)').remove();
    $("#MAST_MP_CONST_CODE").children('option:not(:first)').remove();
    $("#MAST_MLA_CONST_CODE").children('option:not(:first)').remove();

    //$('#mainDiv').animate({ scrollTop: 0 }, 'slow');    
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
function PopulatePakage(paramYearID, ParamBatchID, DropDownName, paramShowAlert,populateFirstItem) { 

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
        data: { Year:paramYearID, BatchID: ParamBatchID, value: Math.random() },
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
