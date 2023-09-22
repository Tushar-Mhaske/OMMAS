﻿/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMATRRegrade.js
        * Description   :   Handles events, grids in ATR regrade report
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/
/*Valodation for if */
$.validator.unobtrusive.adapters.add('checkatrregradewiserejectvalidationattribute', ['previousval'], function (options) {
    options.rules['checkatrregradewiserejectvalidationattribute'] = options.params;
    options.messages['checkatrregradewiserejectvalidationattribute'] = options.message;
});

$.validator.addMethod("checkatrregradewiserejectvalidationattribute", function (value, element, params) {
  
    if ($("#rdoRejectATR").is(":checked") && (value.trim() == '' || value == null)) {
        return false;
    }
    else {
        return true;
    }
});
/*End Rje*/
$(document).ready(function () {


    $("input[name='ATR_REGRADE_STATUS']").change(function () {
        $("#ATR_REGRADE_REMARKS1").val('');
        document.getElementById('ddlreason').options[0].selected = true;

    });

    $('#ddlreason').click(function () {

        $("#ATR_REGRADE_REMARKS1").val('');

        $.ajax({
            url: '/QualityMonitoring/selectATRReasonStatus',
            type: 'POST',
            cache: false,
            data: { reasonCode: $("#ddlreason").val(), },
            success: function (jsonData) {

                if (jsonData.reasonStatus == "A") {
                    document.getElementById("rdoAcceptATR").checked = true;
                }
                else if (jsonData.reasonStatus == "R") {
                    document.getElementById("rdoRejectATR").checked = true;
                }
                else if (jsonData.reasonStatus == "V") {
                    document.getElementById("rdoVerifyATR").checked = true;

                }
                else if (jsonData.reasonStatus == "C") {
                    document.getElementById("rdoCommiteeATR").checked = true;

                }
                $("#ATR_REGRADE_REMARKS1").val(jsonData.reason);

            },
            error: function (err) {
            }
        });
    });




    $.validator.unobtrusive.parse('#frmQMATRRegrade');
    $("#btnCancelATRRegrade").click(function () {
        CloseATR3TierCqcDetails();
    });


    $('#btnSaveATRRegrade').click(function (evt) {
        evt.preventDefault();
        if ($('#frmQMATRRegrade').valid()) {
            $.ajax({
                url: '/QualityMonitoring/QMATRRegrade',
                type: "POST",
                cache: false,
                data: $("#frmQMATRRegrade").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },
                success: function (response) {
                    if (response.Success) {
                        alert("Regrade saved successfully.");
                        //$("#tb3TierATRList").trigger('reloadGrid');
                        //if (response.message == "CQC") {
                        if ($('#roleCode').val() == 9) {
                            viewATRDetails();
                            CloseATR3TierCqcDetails();
                            unblockPage();
                        }
                        else {
                            viewMaintenanceATRDetails();
                            Close2TierSQCATRDetailsPIU();
                            unblockPage();
                        }
                    }
                    else {
                        $("#divATRError").show("slow");
                        $("#divATRError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                    }
                    unblockPage();
                }
            });
        }
    });//btnSaveATRRegrade ends here

});

//function displayRemarks()
//{
//    $("#trATRRemarks").show();

//    //if ($("#rdoRejectATR").is(":checked")) {
//    //    $("#trATRRemarks").show();
        
//    //}
//    //else {
//    //    $("#trATRRemarks").hide();
//    //    $("#divATRError").hide();
//    //}
//}