$(document).ready(function () {

    $("#trUnscrutinyRemark").hide();
    $("#trUnscrutinyDate").hide();

    if ($("#STA_SANCTIONED").val() == "N") {

    }
    else if ($("#STA_SANCTIONED").val() == "Y") {
        $("#trUnscrutinyRemark").hide();
        $("#trUnscrutinyDate").hide();
    }


    $(function () {
        $("#txtScrutinyDate").datepicker(
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

        $("#txtScrutinyDate").datepicker().attr('readonly', 'readonly');
        $("#txtScrutinyDate").datepicker("option", "maxDate", new Date());



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

        if ($("#STA_SANCTIONED").val() != "N") {
            if (!$("#trScrutinyRemark").is(":visible")) {
                $("#trScrutinyRemark").show('slow');
                $("#MS_STA_REMARKS").empty();
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

            $.validator.unobtrusive.parse($('frmSTAScrutiny'));
            evt.preventDefault();
            if ($('#frmSTAScrutiny').valid()) {
                $.ajax({
                    url: '/Proposal/STAFinalizeRoadProposal',
                    type: "POST",
                    cache: false,
                    asynch: false,
                    data: $("#frmSTAScrutiny").serialize(),
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
                            alert("Proposal Scrutinized Successfully.");
                            clearForm();
                            $("#MS_STA_REMARKS").val("");
                            unblockPage();
                            CloseProposalDetails();
                            $("#tbStaProposalList").trigger("reloadGrid");
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

        if ($("#MS_STA_UnScrutinised_REMARKS").is(":visible") == false) {

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

    $.validator.unobtrusive.parse($('frmSTAScrutiny'));

    if ($('#frmSTAScrutiny').valid()) {

        $.ajax({
            url: '/Proposal/STAUnFinalizeRoadProposal',
            type: "POST",
            cache: false,
            data: $("#frmSTAScrutiny").serialize(),
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
                    alert("Proposal Un-Scrutinized Successfully.");
                    clearForm();
                    $("#MS_STA_REMARKS").val("");
                    unblockPage();
                    CloseProposalDetails();

                    $("#tbStaProposalList").trigger("reloadGrid");

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
    $("#frmSTAScrutiny  ").find(':input').each(function () {
        switch (this.type) {
            case 'text':
                $(this).val('');
        }
    });
}