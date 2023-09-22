$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmMordLSBSanction'));

    // FOR Calculating the Total Estimated cost
    $(":text[class~=TMC]").blur(function () {
        var fltTPE = 0.0;
        $(":text[class~=TMC]").each(function () {
            var tempVal = $(this).val().replace(new RegExp("\,", "g"), "");
            if (Number(tempVal) != NaN) {
                fltTPE += Number(tempVal);
                $("#TotalEstimatedCost").val(parseFloat(fltTPE).toFixed(4));
            }
        });
    });


    $(function () {
        $("#IMS_SANCTIONED_DATE").datepicker(
        {
            dateFormat: "dd-M-yy",
            changeMonth: true,
            changeYear: true,
            //minDate: -365,
            maxDate: "+0M +0D",
            showOn: 'button',
            buttonImage: '../../Content/images/calendar_2.png',
            buttonImageOnly: true,
            onClose: function () {
                $(this).focus().blur();
            }
        });
        $("#IMS_SANCTIONED_DATE").datepicker().attr('readonly', 'readonly');
        $("#IMS_SANCTIONED_DATE").datepicker("option", "maxDate", new Date());
    });


    $("input[name='IMS_SANCTIONED']").click(function () {            
        if ($("input[name='IMS_SANCTIONED']").val() != "N") {

            $("#trSanctionedBy").show("slow");
            $("#trSanctionRemark").show("slow");

            $("#txtSanctionBy").val("");
            $("#txtRemarks").val("");
        }
    });

    if ($("#rdoReconsider").is(":checked")) {
        $("#tdHabReasonLabel").show("slow");
        $("#trSanctionDate").show("slow");
        // PopulateReasons($('input:radio[name=IMS_SANCTIONED]:checked').val());
    }

    if ($("#rdoDrop").is(":checked")) {
        $("#tdHabReasonLabel").show("slow");
        $("#trSanctionDate").show("slow");
        //PopulateReasons($('input:radio[name=IMS_SANCTIONED]:checked').val());
    }

    $("#rdoDrop").click(function () {
        $("#tdHabReasonLabel").show("slow");
        $("#trSanctionDate").show("slow");
        PopulateReasons($(this).val());
    });

    $("#rdoReconsider").click(function () {
        $("#tdHabReasonLabel").show("slow");
        $("#trSanctionDate").show("slow");
        PopulateReasons($(this).val());
    });

    if ($("#tdHabReasonLabel").is(":checked")) {
        $("#tdHabReasonLabel").hide("slow");
        $("#trSanctionDate").show("slow");
    }

    $("#rdoSanction").click(function () {
        $("#tdHabReasonLabel").hide("hide");
        $("#trSanctionDate").show("slow");
    });

    $("#btnUpdateAmount").click(function (evt) {
        evt.preventDefault();

     
        if ($('#frmMordLSBSanction').valid()) {
            if (validate()) {
                $.ajax({
                    url: "/LSBProposal/UpdateMordLSBSanctionDetails/",
                    type: "POST",
                    cache: false,
                    data: $("#frmMordLSBSanction").serialize(),
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

                            $("#tblEditProposal").hide('slow');

                            if (response.Message != undefined && response.Message != "") {
                                alert(response.Message);
                            }
                            else {
                                alert("Proposal Details Updated Succesfully.");
                            }
                            CloseProposalDetails();                                                       
                            LoadMordProposals($("#ddlImsYear").val(), $("#ddlState").val(), $("#ddlDistrict").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val());
                        }
                        else {
                            alert(response.ErrorMessage);
                        }
                    }
                });
            }
        }
    });
  
});

function PopulateReasons(IMS_SANCTIONED) {
    $.ajax({
        url: '/Proposal/PopulateReasons',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        data: { IMS_SANCTIONED: IMS_SANCTIONED, value: Math.random() },
        success: function (jsonData) {
            unblockPage();
            if (jsonData.length == 0) {
                alert("Error occured while processing your Request ");
            }
            else {
                $("#IMS_REASON").empty();
                for (var i = 0; i < jsonData.length; i++) {
                    $("#IMS_REASON").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
            }
        },
        error: function (err) {
            alert("Error occured while processing your Request ");
            unblockPage();
        }
    });
}

function validate() {

    if ($("#rdoReconsider").is(":checked") || $("#rdoDrop").is(":checked")) {
        if ($("#IMS_REASON").val() == "0") {
            alert("Please Select Reason");
            return false;
        }
    }

    return true;
}
