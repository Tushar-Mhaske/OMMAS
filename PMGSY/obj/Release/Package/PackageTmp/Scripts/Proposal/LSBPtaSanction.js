 $(document).ready(function () {

        $.validator.unobtrusive.parse($('#frmLSBPTAScrutiny'));

        $("#trUnscrutinyDate").hide();
        $("#trUnscrutinyRemark").hide();

        if ($("#PTA_SANCTIONED").val() == "N") {
            
        }
        else if ($("#PTA_SANCTIONED").val() == "Y") {
            $("#trUnscrutinyDate").hide();
            $("#trUnscrutinyRemark").hide();
        }        

        $(function () {
            $("#txtPtaScrutinyDate").datepicker(
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
            $("#txtPtaScrutinyDate").datepicker().attr('readonly', 'readonly');
            $("#txtPtaScrutinyDate").datepicker("option", "maxDate", new Date());


            $("#txtPtaUnScrutinyDate").datepicker(
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
            $("#txtPtaUnScrutinyDate").datepicker().attr('readonly', 'readonly');
            $("#txtPtaUnScrutinyDate").datepicker("option", "maxDate", new Date());

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

                $.validator.unobtrusive.parse($('#frmLSBPTAScrutiny'));
                evt.preventDefault();
                if ($('#frmLSBPTAScrutiny').valid()) {
                    $.ajax({
                        url: '/LSBProposal/PTAFinalizeLSBProposal',
                        type: "POST",
                        cache: false,
                        data: $("#frmLSBPTAScrutiny").serialize(),
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
                                $("#tbPtaLSBProposalList").trigger("reloadGrid");
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

            if ( $("#MS_PTA_UnScrutinised_REMARKS").is(":visible") == false) {

                if (confirm("Are You Sure to Un-Scrutinize Proposal ?")) {

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

        $.validator.unobtrusive.parse($('#frmLSBPTAScrutiny'));

        if ($('#frmLSBPTAScrutiny').valid()) {

            $.ajax({
                url: '/LSBProposal/PTAUnFinalizeLSBProposal',
                type: "POST",
                cache: false,
                data: $("#frmLSBPTAScrutiny").serialize(),
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

                        $("#tbPtaLSBProposalList").trigger("reloadGrid");

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
        $("#frmLSBPTAScrutiny  ").find(':input').each(function () {
            switch (this.type) {
                case 'text':
                    $(this).val('');
            }
        });
    }