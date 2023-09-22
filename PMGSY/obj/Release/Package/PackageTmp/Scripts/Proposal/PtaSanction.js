/*
    * Project Id    :
    * Project Name  :   OMMAS II
    * Name          :   PtaSactionProposal.cshtml
    * Description   :   This js is for PTA to Scrutinize Road Proposal
    * Author        :   Shyam Yadav
    * Creation Date :   21/Nov/2013    
*/

//$.validator.addMethod("comparesanctiondates", function (value, element, params) {

//    var staSanctionDate = $("#STA_SANCTIONED_DATE").val();
//    var ptaSanctionDate = $("#txtPtaScrutinyDate").val();
//    if (process(ptaSanctionDate) >= process(staSanctionDate)) {
//        return true;
//    }

//    return false;
//});
//jQuery.validator.unobtrusive.adapters.addBool("comparesanctiondates");

function process(date) {
    if (date == "1/1/0001") {
        date = "01/01/1900";
    }
    var parts = date.split("-");
    return new Date(parts[2], parts[1] - 1, parts[0]);
}
$(document).ready(function () {

    $("#trUnscrutinyRemark").hide();
    $("#trUnscrutinyDate").hide();

    if ($("#PTA_SANCTIONED").val() == "N") {

    }
    else if ($("#PTA_SANCTIONED").val() == "Y") {
        $("#trUnscrutinyRemark").hide();
        $("#trUnscrutinyDate").hide();
    }


    $(function () {
        $("#txtPtaScrutinyDate").datepicker(
        {
            dateFormat: "dd-M-yy",
            changeMonth: true,
            changeYear: true,
            maxDate: "+0M +0D",
            showOn: 'button',
            buttonImage: '../../Content/images/calendar_2.png',
            buttonImageOnly: true,
            onClose: function () {
                $(this).focus().blur();
            }
        });

        $("#txtPtaScrutinyDate").datepicker().attr('readonly', 'readonly');
        $("#txtPtaScrutinyDate").datepicker("option", "maxDate", new Date());

        $("#txtUnScrutinyDate").datepicker(
        {
            dateFormat: "dd-M-yy",
            changeMonth: true,
            changeYear: true,
            maxDate: "+0M +0D",
            showOn: 'button',
            buttonImage: '../../Content/images/calendar_2.png',
            buttonImageOnly: true,
            onClose: function () {
                $(this).focus().blur();
            }
        });

        $("#txtUnScrutinyDate").datepicker().attr('readonly', 'readonly');
        $("#txtUnScrutinyDate").datepicker("option", "maxDate", new Date());

    });





    $("#btnScrutinize").click(function (evt) {

        var ProceedFurtherExecution = false;

        if ($("#PTA_SANCTIONED").val() != "N") {
            if (!$("#trScrutinyRemark").is(":visible")) {
                $("#trScrutinyRemark").show('slow');
                $("#MS_PTA_REMARKS").empty();
                return false;
            }
            else {
                ProceedFurtherExecution = true;
            }
        }
        else {
            ProceedFurtherExecution = true;
        }

        if (ProceedFurtherExecution) {

            //alert("Save Data");
            //return false;

            $.validator.unobtrusive.parse($('frmPTAScrutiny'));
            evt.preventDefault();
            if ($('#frmPTAScrutiny').valid()) {
                $.ajax({
                    url: '/Proposal/PTAFinalizeRoadProposal',
                    type: "POST",
                    cache: false,
                    asynch: false,
                    data: $("#frmPTAScrutiny").serialize(),
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
                            alert("Proposal Scrutinized Succesfully.");
                            clearForm();
                            $("#MS_PTA_REMARKS").val("");
                            unblockPage();
                            CloseProposalDetails();
                            $("#tbPtaProposalList").trigger("reloadGrid");
                        }
                        else {
                            alert(response.ErrorMessage);
                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                            unblockPage();
                        }
                        unblockPage();
                    }
                });
            }
        }

    });

    $("#btnUnScrutinize").click(function (evt) {

        if ($("#MS_PTA_UnScrutinised_REMARKS").is(":visible") == false) {

            if (confirm("Are You Sure to UnScrutinize Proposal ?")) {

                $("#trUnscrutinyDate").show("slow");
                $("#trUnscrutinyRemark").show("slow");

            }
            else {
                return false;
            }
        }
        else {
            UnFinaliseProposal();
        }

    });

});

function UnFinaliseProposal() {

    $.validator.unobtrusive.parse($('frmPTAScrutiny'));

    if ($('#frmPTAScrutiny').valid()) {

        $.ajax({
            url: '/Proposal/PTAUnFinalizeRoadProposal',
            type: "POST",
            cache: false,
            data: $("#frmPTAScrutiny").serialize(),
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
                    alert("Proposal Un-Scrutinized Succesfully.");
                    clearForm();
                    $("#MS_PTA_REMARKS").val("");
                    unblockPage();
                    CloseProposalDetails();

                    $("#tbPtaProposalList").trigger("reloadGrid");

                }
                else {
                    alert(response.ErrorMessage);
                    $("#divError").show("slow");
                    $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                    unblockPage();
                }
                unblockPage();
            }
        });

    }
}

function clearForm() {
    $("#frmPTAScrutiny  ").find(':input').each(function () {
        switch (this.type) {
            case 'text':
                $(this).val('');
        }
    });
}