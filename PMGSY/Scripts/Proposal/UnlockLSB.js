$(document).ready(function () {



    $('#btnUpdate').click(function (evt) {
        evt.preventDefault();

        if ($('#rdoSharePercent2015No').is(':checked')) {
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


        if ($('#frmUpdateLockedProposalLSB').valid()) {

            $.ajax({
                url: '/LSBProposal/UpdateUnlockedLSB',
                type: "POST",
                cache: false,
                data: $("#frmUpdateLockedProposalLSB").serialize(),
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
    });

    $("#btnCancel").click(function () {
        CloseProposalDetails();
    });

    $('#IMS_SANCTIONED_BS_AMT').blur(function () {
        calculateTotalCost();
    });

    $('#IMS_SANCTIONED_BW_AMT').blur(function () {
        calculateTotalCost();
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


    //---------------- PMGSY 2  Starts Here----------------------------//

    $("#rdoHigherSpecificationYes").click(function () {
        if ($(this).is(":checked")) {
            $("#trHigherSpecCost").show();
        }
    });

    $("#rdoHigherSpecificationNo").click(function () {
        if ($(this).is(":checked")) {
            $("#trHigherSpecCost").hide();
            $("#IMS_HIGHER_SPECIFICATION_COST").val(0);
            
            $("#IMS_HIGHER_SPECIFICATION_COST").trigger('blur');
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

            $("#IMS_SANCTIONED_BW_AMT").val(totalCost90Percent);
            $("#IMS_SANCTIONED_BS_AMT").val(totalCost10Percent);

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

            //$("#IMS_SANCTIONED_BW_AMT").val(totalCost75Percent);
            //$("#IMS_SANCTIONED_BS_AMT").val(totalCost25Percent);
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
    $("#TotalEstimatedCost").blur(function () {
        var totalCost = 0;

        var tempVal = $(this).val().replace(new RegExp("\,", "g"), "");
        if (Number(tempVal) != NaN) {
            totalCost = parseFloat($(this).val()).toFixed(2);
        }

        // set State Share & Mord share
        // Yes means 90/10 share & No means 75/25
        if ($("#rdoSharePercentYes").is(":checked")) {

            var totalCost90Percent = parseFloat((totalCost * 90) / 100).toFixed(2);
            var totalCost10Percent = parseFloat((totalCost * 10) / 100).toFixed(2);

            $("#IMS_BRIDGE_WORKS_EST_COST").val(totalCost90Percent);
            $('#IMS_SANCTIONED_BW_AMT').val(totalCost90Percent);
            $("#IMS_BRIDGE_EST_COST_STATE").val(totalCost10Percent);
            $('#IMS_SANCTIONED_BS_AMT').val(totalCost10Percent);

        } else if ($("#rdoSharePercentNo").is(":checked")) {
            var totalCost75Percent = parseFloat((totalCost * 75) / 100).toFixed(2);
            var totalCost25Percent = parseFloat((totalCost * 25) / 100).toFixed(2);

            $("#IMS_BRIDGE_WORKS_EST_COST").val(totalCost75Percent);
            $('#IMS_SANCTIONED_BW_AMT').val(totalCost75Percent);
            $("#IMS_BRIDGE_EST_COST_STATE").val(totalCost25Percent);
            $('#IMS_SANCTIONED_BS_AMT').val(totalCost25Percent);
        }


        var higherSpecCost = parseFloat($("#IMS_HIGHER_SPECIFICATION_COST").val()).toFixed(2);
        if (higherSpecCost == NaN || higherSpecCost == 'NaN') {
            higherSpecCost = 0;
        }

        $("#TotalCostWithHigherSpecCost").val(parseFloat((parseFloat(totalCost)) + (parseFloat(higherSpecCost))).toFixed(2));
        //set Higher Specification Cost

    });

    $('#TotalEstimatedCost').trigger('blur');

    $("#IMS_HIGHER_SPECIFICATION_COST").blur(function () {
        var totalCost = 0;
        var tempVal = $("#TotalEstimatedCost").val().replace(new RegExp("\,", "g"), "");
        if (Number(tempVal) != NaN) {
            totalCost = parseFloat($("#TotalEstimatedCost").val()).toFixed(2);
        }

        var higherSpecCost = parseFloat($("#IMS_HIGHER_SPECIFICATION_COST").val()).toFixed(2);
        $("#TotalCostWithHigherSpecCost").val(parseFloat((parseFloat(totalCost)) + (parseFloat(higherSpecCost))).toFixed(2));
    });
    //---------------- PMGSY 2 Ends Here----------------------------//


    $('#rdoSharePercent2015No,#rdoSharePercent2015Yes,#rdoSharePercent2015Old').click(function () {

        CalculateStateShareMordShareTotalCostForLSB();

    });

    $('#IMS_SANCTIONED_BS_AMT,#IMS_SANCTIONED_BW_AMT,#IMS_HIGHER_SPECIFICATION_COST').blur(function () {

        CalculateStateShareMordShareTotalCostForLSB();

    });

    if ($('#IMS_PR_ROAD_CODE').val() != null) {
        CalculateStateShareMordShareTotalCostForLSB();
    }


});
function calculateTotalCost() {
    $('#TotalEstimatedCost').val('');
    var totalCost = (parseFloat($('#IMS_SANCTIONED_BS_AMT').val()) + parseFloat($('#IMS_SANCTIONED_BW_AMT').val()));

    if (isNaN(totalCost)) {
        $('#TotalEstimatedCost').val(0);
    }
    else {
        $('#TotalEstimatedCost').val(parseFloat(totalCost).toFixed(2));
    }

}




function CalculateStateShareMordShareTotalCostForLSB()
{
    
    var mordCost = isNaN($('#IMS_SANCTIONED_BW_AMT').val()) ? parseFloat("0") : parseFloat($('#IMS_SANCTIONED_BW_AMT').val());
    var stateCost = isNaN($('#IMS_SANCTIONED_BS_AMT').val()) ? parseFloat("0") : parseFloat($('#IMS_SANCTIONED_BS_AMT').val());

    var totalCost = parseFloat(mordCost) + parseFloat(stateCost);

    // Comment Starts


    //if ($('#rdoSharePercent2015No').is(':checked'))
    //{
    //    $('#IMS_MORD_SHARE_2015').val(parseFloat(totalCost * 0.9).toFixed(2));
    //    $('#IMS_STATE_SHARE_2015').val(parseFloat(totalCost * 0.1).toFixed(2));

    //}
    //else if ($('#rdoSharePercent2015Yes').is(':checked'))
    //{
       
    //    $('#IMS_MORD_SHARE_2015').val(parseFloat(totalCost * 1).toFixed(2));
    //    $('#IMS_STATE_SHARE_2015').val(parseFloat(totalCost * 0).toFixed(2));
    //}
    //else if ($('#rdoSharePercent2015SchemeII').is(':checked'))
    //{
       
    //    $('#IMS_MORD_SHARE_2015').val(parseFloat(totalCost * 0.75).toFixed(2));
    //    $('#IMS_STATE_SHARE_2015').val(parseFloat(totalCost * 0.25).toFixed(2));
    //}
    //else if ($('#rdoSharePercent2015Old').is(':checked'))
    //{
    //    $('#IMS_MORD_SHARE_2015').val(parseFloat(totalCost * 0.60).toFixed(2));
    //    $('#IMS_STATE_SHARE_2015').val(parseFloat(totalCost * 0.40).toFixed(2));
    //}
    // Comment Ends





    //console.log(mordCost);
    if ($('#PMGSYScheme').val() == 1 || $('#PMGSYScheme').val() == 3 || $('#PMGSYScheme').val() == 4)
    {

        /* Added for requirement from Pankaj sir*/
        var mordCost1 = isNaN(parseFloat($('#IMS_SANCTIONED_BW_AMT').val())) ? parseFloat(0) : parseFloat($('#IMS_SANCTIONED_BW_AMT').val());

        if ($('#rdoSharePercent2015No').is(':checked')) {
            $('#IMS_MORD_SHARE_2015').val(parseFloat(mordCost1 * 0.9).toFixed(2));
            $('#IMS_STATE_SHARE_2015').val(parseFloat(mordCost1 * 0.1).toFixed(2));
        }
        else if ($('#rdoSharePercent2015Yes').is(':checked')) {
            $('#IMS_MORD_SHARE_2015').val(parseFloat(mordCost1 * 1).toFixed(2));
            $('#IMS_STATE_SHARE_2015').val(parseFloat(mordCost1 * 0).toFixed(2));
        }
        else if ($('#rdoSharePercent2015Old').is(':checked')) {
            $('#IMS_MORD_SHARE_2015').val(parseFloat(mordCost1 * 0.60).toFixed(2));
            $('#IMS_STATE_SHARE_2015').val(parseFloat(mordCost1 * 0.40).toFixed(2));
        }
        /* Added for requirement from Pankaj sir*/

        var totalCostToUpdate = parseFloat(parseFloat(totalCost)).toFixed(2);
        $('#IMS_TOTAL_STATE_SHARE_2015').val(parseFloat(parseFloat(stateCost) + parseFloat($('#IMS_STATE_SHARE_2015').val())).toFixed(2));
        $('#IMS_TOTAL_COST_2015').val(totalCostToUpdate);
    }
    else if ($('#PMGSYScheme').val() == 2 )
    {

        var higherSpecCost = isNaN(parseFloat($('#IMS_HIGHER_SPECIFICATION_COST').val())) ? parseFloat("0") : parseFloat($('#IMS_HIGHER_SPECIFICATION_COST').val());

        $('#IMS_TOTAL_STATE_SHARE_2015').val(parseFloat(parseFloat(higherSpecCost) + parseFloat($('#IMS_STATE_SHARE_2015').val())).toFixed(2));

        $('#IMS_TOTAL_COST_2015').val(parseFloat($('#TotalCostWithHigherSpecCost').val()).toFixed(2));
    }




}