 $(document).ready(function () {

        $.validator.unobtrusive.parse($('#frmLSBSTAScrutiny'));

        $("#trUnscrutinyDate").hide();
        $("#trUnscrutinyRemark").hide();

        if ($("#STA_SANCTIONED").val() == "N") {
            
        }
        else if ($("#STA_SANCTIONED").val() == "Y") {
            $("#trUnscrutinyDate").hide();
            $("#trUnscrutinyRemark").hide();
        }        

        $(function () {
            $("#txtScrutinyDate").datepicker(
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
            $("#txtScrutinyDate").datepicker().attr('readonly', 'readonly');
            $("#txtScrutinyDate").datepicker("option", "maxDate", new Date());


            $("#txtUnScrutinyDate").datepicker(
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
            $("#txtUnScrutinyDate").datepicker().attr('readonly', 'readonly');
            $("#txtUnScrutinyDate").datepicker("option", "maxDate", new Date());

        });

        $("#btnScrutinize").click(function (evt) {

            var ProceedFurtherExecution = false;

            if ($("#STA_SANCTIONED").val() != "N") {
                if (!$("#trScrutinyRemark").is(":visible")) {
                    $("#trScrutinyRemark").show('slow');
                    $("#trScrutinyDate").show("slow");
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
            
            $("#STA_SANCTIONED_DATE").val($("#txtScrutinyDate").val());

            if (ProceedFurtherExecution) {

                $.validator.unobtrusive.parse($('#frmLSBSTAScrutiny'));
                evt.preventDefault();
                if ($('#frmLSBSTAScrutiny').valid()) {
                    $.ajax({
                        url: '/LSBProposal/STAFinalizeLSBProposal',
                        type: "POST",
                        cache: false,
                        data: $("#frmLSBSTAScrutiny").serialize(),
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
                                $("#MS_STA_REMARKS").val("");
                                unblockPage();
                                CloseProposalDetails();
                                $("#tbStaLSBProposalList").trigger("reloadGrid");
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

            if ( $("#MS_STA_UnScrutinised_REMARKS").is(":visible") == false) {

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

        $.validator.unobtrusive.parse($('#frmLSBSTAScrutiny'));

        if ($('#frmLSBSTAScrutiny').valid()) {

            $.ajax({
                url: '/LSBProposal/STAUnFinalizeLSBProposal',
                type: "POST",
                cache: false,
                data: $("#frmLSBSTAScrutiny").serialize(),
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
                        $("#MS_STA_REMARKS").val("");
                        unblockPage();
                        CloseProposalDetails();

                        $("#tbStaLSBProposalList").trigger("reloadGrid");

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
        $("#frmLSBSTAScrutiny  ").find(':input').each(function () {
            switch (this.type) {
                case 'text':
                    $(this).val('');
            }
        });
    }