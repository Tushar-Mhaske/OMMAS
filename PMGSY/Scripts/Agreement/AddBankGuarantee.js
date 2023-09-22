/// <reference path="../jquery-1.9.1.js" />
$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmBankguaranteeDetails');
     
    // for close file name and agin give option for browse file in edit mode 
     
    $('#spnIconCloseFile').click(function () {
        $(this).hide('slow');
        $('#spnBgFileName').hide('slow')
        $('#BGFile').show('slow');
    });

    //ends
   
    var myfile = "";
   
    $('#BGFile').on('change', function () {
        myfile = $(this).val();
        
        var ext = myfile.split('.').pop();
     //  alert("File extension :" + ext.toLowerCase());
       if (ext.toLowerCase() != "pdf") {
           alert("Only pdf file is allowed.");
           $(this).val('');
            return false;
       }
       var fileSizeKb = $(this)[0].files[0].size; // file size in Kb
       var fileSizeMb = fileSizeKb / 1048576;
       if (fileSizeMb > 4)
       {
           alert("File size should be less than or equal to 4 MB.")
           $(this).val('');
           return false;
       }

    })
 

    $('#txtBankName').focus();
   
    var date = new Date();
    var firstDay = new Date(date.getFullYear(), date.getMonth(), 1);
    var lastDay = new Date(date.getFullYear(), date.getMonth() + 1, 0);
    var cur = date.getDate();
    var difference = (lastDay.getDate() - cur);

    $("#txtIssueDate").datepicker({
        changeMonth: true,
        changeYear: true,
        maxDate: "+" + -1 + "D",
        dateFormat: "dd/mm/yy",
        showOn: "button",
        buttonImage: "/Content/images/calendar_2.png",
        buttonImageOnly: true,
        buttonText: "Select date",
        onSelect: function (dateText, inst) {
            //  $("span").find("[for=txtIssueDate]").text(' ');
        }
    });
    //$("#txtIssueDate").datepicker("setDate", new Date());

    $('#txtverifiedDate').datepicker({
        changeMonth: true,
        changeYear: true,
        //maxDate: "+" + -1 + "D",
        dateFormat: "dd/mm/yy",
        showOn: "button",
        buttonImage: "/Content/images/calendar_2.png",
        buttonImageOnly: true,
        buttonText: "Select date",
        onSelect: function (dateText, inst) {
            //  $("span").find("[for=txtIssueDate]").text(' ');
        }
    });


    $("#txtExpiryDate").datepicker({
        changeMonth: true,
        changeYear: true,
        //  maxDate: "+" + 0 + "D",
        dateFormat: "dd/mm/yy",
        showOn: "button",
        buttonImage: "/Content/images/calendar_2.png",
        buttonImageOnly: true,
        buttonText: "Select date",
        onSelect: function (dateText, inst) {
            var iDate = $("#txtIssueDate").val();
            var start = iDate.split("/");
            var issuedate = new Date(start[2], start[1], start[0]);
            var eDate = dateText.split("/");
            var endDate = new Date(eDate[2], eDate[1], eDate[0]);

            if (iDate == "") {
                //alert("First select issue date.");
            } else {
                if (compareDates(issuedate, endDate)) {
                    $('#expdateError').text(' ');
                } else {
                    $('#expdateError').text('Expiry Date should be greater than Issue Date.');
                }
            }
            // alert(dateText);
        }
    });

    $('#btnReset').click(function () {
        $('#expdateError').html('');
        $('#verificationdateError').html('');
        $('#frmBankguaranteeDetails')[0].reset();
        
    })
    function compareDates(issueDte, expiryDte) {
        //var startDte = new Date(issueDte);
        //var endDte = new Date(expiryDte);
        // alert(startDte + "    :   " + expiryDte);
        if (expiryDte > issueDte) {
            return true;
        } else {
            return false;
        }
    }

    $('#btnSave').click(function () {

        var start = $("#txtIssueDate").val().split("/");
        var issuedate = new Date(start[2], start[1], start[0])
        // alert(start.length)
        var end = $("#txtExpiryDate").val().split("/");
        var expdate = new Date(end[2], end[1], end[0]);
        var verified = $('#txtverifiedDate').val().split("/");
   
        var verifiedDate = new Date(verified[2], verified[1], verified[0]);

        var form = $('#frmBankguaranteeDetails');
        var formadata = new FormData(form.get(0)); //__RequestVerificationToken 
         var fileUpload = $("#BGFile").get(0);
        var FileBG = fileUpload.files[0]
        formadata.append("BGFile", FileBG);
        formadata.append("AGREEMENT_CODE", $("#AGREEMENT_CODE").val())
        formadata.append("TendBgCode", $("#TendBgCode").val());
        formadata.append("BG_BANK_NAME", $('#txtBankName').val())
        formadata.append("BG_AMOUNT", $('#txtBGAmount').val());
        formadata.append("BG_ISSUE_DATE", $('#txtIssueDate').val());
        formadata.append("BG_EXPIRY_DATE", $('#txtExpiryDate').val());
        formadata.append("BG_VERIFICATION_DATE", $('#txtverifiedDate').val());
        formadata.append("VERIFIEDBY", $('#VERIFIEDBY').val());

        if ($('#frmBankguaranteeDetails').valid()) {

            if (start.length > 1) {
                if (expdate > issuedate) {
                    $('#expdateError').html('');
                    if (verifiedDate < expdate && verifiedDate >= issuedate) {
                        $('#verificationdateError').html('');
                        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                        $.ajax({
                            url: '/Agreement/AddBankGuaranteeDetails',
                            type: 'POST',
                            cache: false,
                            async: false,
                            contentType: false,
                            processData: false,
                            beforeSend: function () {
                                
                            },
                            //contentType: "multipart/form-data",
                            data:formadata,//$('#frmBankguaranteeDetails').serialize(),
                            success: function (response) {
                                alert(response.message);
                                if (response.success) {
                                    $('#dvViewAgreementMasterBankGuarantee').hide('slow');
                                    $('#tblstAgreementsBankGuarantee').trigger('reloadGrid');
                                    $('#gview_tblstAgreementsBankGuarantee .ui-jqgrid-titlebar-close>span').trigger('click');
                                }
                                if (response.file == false)
                                    $('#BGFile').val('');
                                $.unblockUI();

                            },
                            error: function () {
                                $.unblockUI();
                                alert("An Error");
                                return false;
                            },
                        });

                    }
                    else {
                        $('#verificationdateError').html("<br/>Verification Date should be less than <br/>Expiry Date and greater than or equal to Issue Date.");
                    }
                }
                else {
                    //$('#expdateError').show();
                    $('#expdateError').html('<br/>Expiry Date should be greater than Issue Date.');
                }
            }

        } else {
            return false;
        }

    });


    $('#btnUpdate').click(function () {
        updateBankGuaranteeDetails();
    });

    $('#btnCancel').click(function () {
        $('#dvViewAgreementMasterBankGuarantee').hide('slow');
    });

    $('#spCollapseIconCN').click(function () {
        $('#dvViewAgreementMasterBankGuarantee').hide('slow');
        $('#gview_tblstAgreementsBankGuarantee .ui-jqgrid-titlebar-close>span').trigger('click');
    })

})


function updateBankGuaranteeDetails() {

    var start = $("#txtIssueDate").val().split("/");
    var issuedate = new Date(start[2], start[1], start[0])
    // alert(start.length)
    var end = $("#txtExpiryDate").val().split("/");
    var expdate = new Date(end[2], end[1], end[0]);
    var verified = $('#txtverifiedDate').val().split("/");

    var verifiedDate = new Date(verified[2], verified[1], verified[0]);
    var form = $('#frmBankguaranteeDetails');
    var formadata = new FormData(form.get(0)); //for __verification Token
    var fileUpload = $("#BGFile").get(0);
    var FileBG = fileUpload.files[0]
    formadata.append("BGFile", FileBG);
    formadata.append("AGREEMENT_CODE", $("#AGREEMENT_CODE").val())
    formadata.append("TendBgCode", $("#TendBgCode").val());
    formadata.append("BG_BANK_NAME", $('#txtBankName').val())
    formadata.append("BG_AMOUNT", $('#txtBGAmount').val());
    formadata.append("BG_ISSUE_DATE", $('#txtIssueDate').val());
    formadata.append("BG_EXPIRY_DATE", $('#txtExpiryDate').val());
    formadata.append("BG_VERIFICATION_DATE", $('#txtverifiedDate').val());
    formadata.append("VERIFIEDBY", $('#VERIFIEDBY').val());
    

    if ($('#frmBankguaranteeDetails').valid()) {

        if (start.length > 1) {
            if (expdate > issuedate) {
                $('#expdateError').text('');
                if (verifiedDate < expdate && verifiedDate >= issuedate) {
                    $('#verificationdateError').text('');

                    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                    $.ajax({
                        url: '/Agreement/EditBankGuaranteeDetails',
                        type: 'POST',
                        cache: false,
                        async: false,
                        contentType: false,
                        processData:false,
                        data: formadata,//$('#frmBankguaranteeDetails').serialize(),
                        success: function (response) {
                            alert(response.message);
                            if (response.success) {
                                $('#dvViewAgreementMasterBankGuarantee').hide('slow');
                                $('#tblstBankGuarantee').trigger('reloadGrid');
                            }
                            $.unblockUI();

                        },
                        error: function () {
                            $.unblockUI();
                            alert("An Error occured");
                            return false;
                        },
                    });

                }
                else
                {
                    $('#verificationdateError').text("Verification Date should be less than Expiry Date and greater than or equal to Issue Date.");
                }
            }
            else {
                //$('#expdateError').show();
                $('#expdateError').text('Expiry Date should be greater than Issue Date.');
            }
        }

    } else {
        return false;
    }
}

