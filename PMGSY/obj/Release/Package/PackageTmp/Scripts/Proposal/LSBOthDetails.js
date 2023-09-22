
$.validator.unobtrusive.adapters.add('isvalidval', ['otherpropertyval'], function (options) {
    options.rules['isvalidval'] = options.params;
    options.messages['isvalidval'] = options.message;
});

$.validator.addMethod("isvalidval", function (value, element, params) {
    return true;    
    /*    
    if (params.otherpropertyval == "IMS_ROAD_TYPE_LEVEL") {
        return ($("#IMS_ROAD_TYPE_LEVEL").val() > value);
    }

    if (params.otherpropertyval == "IMS_HIGHEST_FLOOD_LEVEL") {
        return ($("#IMS_HIGHEST_FLOOD_LEVEL").val() > value);
    }
    
    return true;*/
    //if ($("#" + params.otherpropertyval).val() <= value)
    ////if( $("#" + $(params.otherpropertyval).attr("ID")).val()  < value)
    //    return false;
    //else
    //    return true;
   
    //return ($("#" + params.otherpropertyval).val() > value);
});

// this Function validates the Type of Bridge Details 
function validate() {
   
    //commented by Vikram as suggested by Dev sir on 10 July 2014
    /*if (parseFloat($("#IMS_HIGHEST_FLOOD_LEVEL").val()) >= parseFloat($("#IMS_ROAD_TYPE_LEVEL").val())) {
        alert("Highest Flood level(HFL) should be less than Road Top level(RTL)");
        $("#IMS_HIGHEST_FLOOD_LEVEL").focus();
        return false;
    }*/
    if (parseFloat($("#IMS_ORDINARY_FLOOD_LEVEL").val()) > parseFloat($("#IMS_HIGHEST_FLOOD_LEVEL").val())) {
        alert("Ordinary Flood level(OFL) should be less than Highest Flood level(HFL)");
        $("#IMS_ORDINARY_FLOOD_LEVEL").focus();
        return false;
    }

    if (parseFloat($("#IMS_AVERAGE_GROUND_LEVEL").val()) > parseFloat($("#IMS_ORDINARY_FLOOD_LEVEL").val())) {
        alert("Average Ground level(AGL) should be less than Ordinary Flood level(OFL)");
        $("#IMS_AVERAGE_GROUND_LEVEL").focus();
        return false;
    }
    

    if (parseFloat($("#IMS_NALA_BED_LEVEL").val()) > parseFloat($("#IMS_AVERAGE_GROUND_LEVEL").val())) {
        alert("Nala Bed level(NBL) should be less than Average Ground level(AGL)");
        $("#IMS_NALA_BED_LEVEL").focus();
        return false;
    }

    if (parseFloat($("#IMS_FOUNDATION_LEVEL").val()) > parseFloat($("#IMS_NALA_BED_LEVEL").val())) {
        alert("Foundation level(FL) should be less than Nala Bed level(NBL)");
        $("#IMS_FOUNDATION_LEVEL").focus();
        return false;
    }


    return true;
}

$(document).ready(function () {

    if ($("#OPERATION").val() == "C") {
        $("#btnAdd").attr("value", "Save");
    }
    else if ($("#OPERATION").val() == "U") {
        $("#btnReset").hide();
        $("#btnAdd").attr("value", "Update");
    }
    $('#IMS_NALA_BED_LEVEL').blur(function () {
        $('#IMS_HGT_BIRDGE_NBL').val('');
        var bridgeHeight = $('#IMS_ROAD_TYPE_LEVEL').val() - $('#IMS_NALA_BED_LEVEL').val();
        $('#IMS_HGT_BIRDGE_NBL').val(bridgeHeight);
    });


    $('#IMS_FOUNDATION_LEVEL').blur(function () { 
        $('#IMS_HGT_BRIDGE_FL').val('');
        var bridgeHeight = $('#IMS_ROAD_TYPE_LEVEL').val() - $('#IMS_FOUNDATION_LEVEL').val();
        $('#IMS_HGT_BRIDGE_FL').val(bridgeHeight);
    });


    //Calculate Total Cost
    ///-------------------------------------------------------------------------///
    $('#IMS_APPROACH_COST').blur(function () {
        calculateTotalCost();
    });

    $('#IMS_BRGD_STRUCTURE_COST').blur(function () {
        calculateTotalCost();
    });

    $('#IMS_STRUCTURE_COST').blur(function () {
        calculateTotalCost();
    });

    $('#IMS_BRGD_OTHER_COST').blur(function () {
        calculateTotalCost();
    });

    ///-------------------------------------------------------------------------///


    //button Create Click
    $('#btnAdd').click(function (evt) {
        evt.preventDefault();
        if (validate()) {
            if ($('#frmLSBOthDetails').valid()) {            

                if ($("#OPERATION").val() == "C") {
                    //alert("Test");
                    $.ajax({
                        url: '/LSBProposal/LSBOtherDetails',
                        type: "POST",
                        cache: false,
                        data: $("#frmLSBOthDetails").serialize(),
                        beforeSend: function () {
                            blockPage();
                        },
                        error: function (xhr, status, error) {
                            unblockPage();
                            Alert("Request can not be processed at this time,please try after some time!!!");
                            return false;
                        },
                        success: function (response) {
                            if (response.success) {
                                alert("Other details saved succesfully.");
                                $("#tbLSBProposalList").trigger("reloadGrid");
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

                if ($("#OPERATION").val() == "U") {
                    $.ajax({
                        url: '/LSBProposal/LSBOtherDetails',
                        type: "POST",
                        cache: false,
                        data: $("#frmLSBOthDetails").serialize(),
                        beforeSend: function () {
                            blockPage();
                        },
                        error: function (xhr, status, error) {
                            unblockPage();
                            Alert("Request can not be processed at this time,please try after some time!!!");
                            return false;
                        },
                        success: function (response) {
                            if (response.success) {
                                alert("Other details updated succesfully.");
                                $("#tbLSBProposalList").trigger("reloadGrid");
                                CloseProposalDetails();
                            }
                            else {
                                //alert(response.ErrorMessage);
                                $("#divError").show("slow");
                                $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);

                                $.validator.unobtrusive.parse($('#mainDiv'));
                            }
                            unblockPage();
                        }
                    });
                }

            }
        }
        else {
            $('.qtip').show();
        }
    });//btnCreate ends here



    
    $('#btnDelete').click(function (evt) {

        DeleteLSBOthDetails();

    });//btnDelete ends here


});//doc.Ready ends here



function calculateTotalCost()
{
    $('#TotalEstimatedCost').val('');
    
    var totalCost = (parseFloat($('#IMS_APPROACH_COST').val()) + parseFloat($('#IMS_BRGD_STRUCTURE_COST').val())
                        + parseFloat($('#IMS_STRUCTURE_COST').val()) + parseFloat($('#IMS_BRGD_OTHER_COST').val()));

    if (isNaN(totalCost)) {
        $('#TotalEstimatedCost').val(0);
    }
    else
    {
        $('#TotalEstimatedCost').val(parseFloat(totalCost));
    }

}



//Delete LSB Proposal
function DeleteLSBOthDetails() {

    if (confirm("Are you sure to delete other details for LSB? ")) {

        $.ajax({
            url: '/LSBProposal/DeleteLSBOthDetails?id=' + $("#IMS_PR_ROAD_CODE").val(),
            type: "POST",
            cache: false,
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {

                if (response.success == "true") {
                    alert("Other Details for LSB are deleted successfully.");
                    $("#tbLSBProposalList").trigger('reloadGrid');

                    //Close Other Details form DIV
                    $('#accordion').hide('slow');
                    $('#divProposalForm').hide('slow');
                    $("#tbProposalList").jqGrid('setGridState', 'visible');
                    $("#tbLSBProposalList").jqGrid('setGridState', 'visible');
                    showFilter();
                }
                else {
                    if (response.errorMessage != "" || response.errorMessage != undefined || response.errorMessage != null) {
                        alert(response.errorMessage)
                    }
                    else {
                        alert("Error Occured while processing your request.");
                    }

                }
                unblockPage();
            }
        });
    } else {
        return;
    }
}