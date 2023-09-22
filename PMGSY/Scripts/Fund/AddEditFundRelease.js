$.validator.addMethod("compareyear", function (value, element, params) {

    if (parseInt($("#ddlYearPhase").val()) <= parseInt($("#ddlYear").val()))
    {
        return true;
    }
    
    return false;
});
jQuery.validator.unobtrusive.adapters.addBool("compareyear");

//$.validator.addMethod("compareamount", function (value, element, params) {

//    if ($("#MAST_RELEASE_TYPE").val() == "S") {
//        return true;
//    }
//    else if (parseFloat($("#MAST_RELEASE_AMOUNT").val()) <= parseFloat($("#TotalAvailable").val())) {
//        return true;
//    }

//    return false;
//});
//jQuery.validator.unobtrusive.adapters.addBool("compareamount");

//$.validator.unobtrusive.adapters.add('comparereleasedatewithreleaseyear', ['releaseyear'], function (options) {
//    options.rules['comparereleasedatewithreleaseyear'] = options.params;
//    options.messages['comparereleasedatewithreleaseyear'] = options.message;
//});

$.validator.addMethod("comparereleasedatewithreleaseyear", function (value, element, params) {

    var releaseDate = value;
    var releaseYear = $("#ddlYear option:selected").val();
    var startDateString = "01/01/"+releaseYear;
    var endDateString = "12/31/"+releaseYear;

    //alert(new Date(process(releaseDate)));
    //alert(new Date(startDateString));
    //alert(new Date(endDateString));
    //alert(new Date(releaseDate) <= new Date(endDateString));
    //alert(new Date(releaseDate) >= new Date(startDateString));

    if (releaseYear === undefined)
    {
        return false;
    }
    else if (new Date(process(releaseDate)) <= new Date(process(endDateString)) && new Date(process(releaseDate)) >= new Date(process(startDateString))) {
        return true;
    }
    else {
        return false;
    }

    return false;
});
function process(date) {
    var parts = date.split("/");
    return new Date(parts[2], parts[1] - 1, parts[0]);
}
$(document).ready(function () {

    $.validator.unobtrusive.parse("frmAddEditFundRelease");

    $('#MAST_RELEASE_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a date',
        buttonImageOnly: true,
        maxDate: new Date(),
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {
            
        },
        onClose: function ()
        {
            $(this).focus().blur();
        }
    });

    
    $("#releaseNo").html("0");

    if ($("#MAST_RELEASE_TYPE").val() == "S") {

        FillInCascadeDropdown({ userType: $("#ddlState").find(":selected").val() },
                           "#ddlExecutingAgency", "/Fund/GetExecutingAgencyByState?stateCode=" + $('#ddlState option:selected').val());
    }


    $("#btnSave").click(function () {
            
        if ($("#frmAddEditFundRelease").valid()) {
            $.ajax({
                type: 'POST',
                url: '/Fund/AddFundRelease/',
                async: false,
                data: $("#frmAddEditFundRelease").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                success: function (data) {
                    if (data.success) {
                        alert(data.message);
                        CloseProposalDetails();
                        searchReleaseDetails();
                        $("#tbFundReleaseList").trigger("reloadGrid");
                        unblockPage();
                    }
                    else {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        $.validator.unobtrusive.parse($('#mainDiv'));
                        unblockPage();
                    }
                    unblockPage();

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }
            })
        }
    });

    $("#btnUpdate").click(function () {

        if ($("#frmAddEditFundRelease").valid()) {
            $.ajax({
                type: 'POST',
                url: '/Fund/EditFundRelease/',
                async: false,
                data: $("#frmAddEditFundRelease").serialize(),
                success: function (data) {
                    if (data.success) {
                        alert(data.message);
                        CloseProposalDetails();
                        $("#tbFundReleaseList").trigger("reloadGrid");
                        unblockPage();
                    }
                    else {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }
            })
        }
    });


    $("#MAST_RELEASE_AMOUNT").blur(function (e) {

        var releaseAmount = $("#MAST_RELEASE_AMOUNT").val();
        if (($("#EncryptedReleaseCode").val()) == "") {
                GetFundAllocationTotal();
        }
    });

    $("#MAST_RELEASE_AMOUNT").focus(function (e) {

        if ($("#EncryptedReleaseCode").val() == "") {

            if ($("#ddlState").val() != null && $("#ddlFundType").val() != null && $("#ddlYear option:selected").val() != null && $("#ddlExecutingAgency option:selected").val() != null && $("#ddlFundingAgency option:selected").val() != null) {

                    GetFundReleaseData();
            }
        }

        if ($("#EncryptedReleaseCode").val() != "") {

            $("#releaseNo").html("");
            $("#releaseNo").html($("#transactionCountRelease").val());
        }
    });
    

    $("#btnCancel").click(function () {
        CloseProposalDetails();

    });

    $("#ddlState").change(function (e) {

        FillInCascadeDropdown({ userType: $("#ddlState").find(":selected").val() },
                           "#ddlExecutingAgency", "/Fund/GetExecutingAgencyByState?stateCode=" + $('#ddlState option:selected').val());
    });


    $("#btnBrowseFilesFundRelease").click(function (e) {

        $("#divfileUploadFundRelease").show();

        $.ajax({
            type: 'POST',
            url: '/Fund/FileUploadFundRelease',
            data: $("#frmAddEditFundRelease").serialize(),
            async: false,
            cache: false,
            success: function (data) {
                $("#divfileUploadFundRelease").html(data);
                $.validator.unobtrusive.parse("fileupload");
                unblockPage();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }
        })
    });

    $("#btnReset").click(function () {

        $("#availableAmount").html('0');
        $("#releaseNo").html('0');
        $("#totalRelease").html('0 Cr');
    });

    if ($("#EncryptedReleaseCode").val() != "") {
        
        $("#releaseNo").html("");
        //GetFundReleaseData();
        $("#releaseNo").html($("#transactionCountRelease").val());
        
    }


});
function GetFundAllocationTotal() {

    $.ajax({
        type: 'POST',
        url: '/Fund/GetFundAllocationTotal/',
        data: { stateCode: $("#ddlState").val(), yearCode: $("#ddlYearPhase").val(), fundType: $("#ddlFundType option:selected").val(), executingAgencyCode: $("#ddlExecutingAgency").val(), releaseType: $("#MAST_RELEASE_TYPE").val(), agencyCode: $("#ddlFundingAgency option:selected").val() },
        async: false,
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data.success) {
                var total = data.total;
                if (($("#MAST_RELEASE_AMOUNT").val() > total)) {
                    $("#msgReleaseTotal").show();
                    $("#msgReleaseTotal").html("<span>Total Fund Release Amount Exceeds Total Fund Allocation.</span>");
                    return false;
                }
            }
            else {
                $("#msgReleaseTotal").html("");
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
        }

    })
}
function GetFundReleaseData() {

    $.ajax({
        type: 'POST',
        datatype: 'json',
        url: '/Fund/GetReleaseData/',
        data: { stateCode: $("#ddlState").val(), fundType: $("#ddlFundType").val(), yearCode: $("#ddlYearPhase").val(), executingAgencyCode: $("#ddlExecutingAgency").val(), agencyCode: $("#ddlFundingAgency").val(), releaseType: $("#MAST_RELEASE_TYPE").val() },
        async: false,
        cache: false,
        success: function (data) {
            if (data.success) {
                $("#releaseNo").html(data.transactionNo);
                if ($("#MAST_RELEASE_TYPE").val() == "S") {

                }
                else {
                    $("#totalRelease").html(data.totalRelease + " Cr");
                    var remaining = data.remainingRelease - data.totalRelease;
                    $("#availableAmount").html(remaining);
                    $("#TotalRelease").val(data.totalRelease);
                    $("#TotalAvailable").val(remaining);
                    if (data.totalAllocation == null) {
                        $("#totalAllocation").html("0 Cr");
                    }
                    else
                    {
                        $("#totalAllocation").html(data.totalAllocation + " Cr");
                    }
                    $("#allocationAmount").val($("#availableAmount").html());
                    if ($("#EncryptedReleaseCode").val() != "") {
                        var release = $("#MAST_RELEASE_AMOUNT").val();
                        remaining = parseFloat(remaining) + parseFloat(release);
                        $("#availableAmount").html(remaining);
                    }
                }
            }
            else {
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert(xhr.responseText);
        }
    })

}