var isValid;
$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmAddEditFundAllocation");

    $('#MAST_ALLOCATION_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a date',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear:true,
        maxDate:new Date(),
        onSelect: function (selectedDate) {
        },
        onClose: function () {
            $(this).focus().blur();
        }
    });


    $("#btnBrowseFiles").click(function (e) {

        $("#divfileUpload").show();

        $.ajax({
            type: 'POST',
            url: '/Fund/FileUpload',
            data: $("#frmAddEditFundAllocation").serialize(),
            async: false,
            cache: false,
            success: function (data) {
                $("#divfileUpload").html(data);
                $.validator.unobtrusive.parse("fileupload");
                unblockPage();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }
        })
    });

    $("#btnSave").click(function () {

        if ($("#frmAddEditFundAllocation").valid()) {
            $("#transactionCount").val($("#txtTransaction").html());
            $.ajax({
                type: 'POST',
                url: '/Fund/AddFundAllocation/',
                async: false,
                data: $("#frmAddEditFundAllocation").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                success: function (data) {
                    if (data.success) {
                        alert(data.message);
                        //$("#btnStartUpload").trigger('click');
                        CloseProposalDetails();
                        searchDetails();
                        $("#tbFundAllocationList").trigger("reloadGrid");
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

    $("#btnCancel").click(function () {
        CloseProposalDetails();
    });


    $("#btnUpdate").click(function () {


        //ValidateReleaseAmount();


        if ($("#frmAddEditFundAllocation").valid()) {
            $.ajax({
                type: 'POST',
                url: '/Fund/EditFundAllocation/',
                async: false,
                data: $("#frmAddEditFundAllocation").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        CloseProposalDetails();
                        $("#tbFundAllocationList").trigger("reloadGrid");
                        unblockPage();
                    }
                    else if (data.success == false) {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                    }
                    else
                    {
                        alert('Error occurred while processing your request.');
                    }

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }
            })
        }

    });

    $("#ddlState").change(function (e) {

        FillInCascadeDropdown({ userType: $("#ddlState").find(":selected").val() },
                           "#ddlExecutingAgency", "/Fund/GetExecutingAgencyByState?stateCode=" + $('#ddlState option:selected').val());
    });


    if (($("#EncryptedFundCode").val()) != "") {
        
        GetAllocationData();
        $("#txtTransaction").html("");
        $("#txtTransaction").html($("#transactionCount").val());
    }


    $("#MAST_ALLOCATION_AMOUNT").focus(function (e) {

        if ($("#ddlState").val() != null && $("#ddlFundType").val() != null && $("#ddlYear option:selected").val() != null && $("#ddlExecutingAgency").val() != null && $("#ddlFundingAgency option:selected").val() != null) {

            GetAllocationData();
        }

        if (($("#EncryptedFundCode").val()) != "") {

            $("#txtTransaction").html("");
            $("#txtTransaction").html($("#transactionCount").val());
        }
    });
});

function FillInCascadeDropdown(map, dropdown, action) {
    var message = '';

    if (dropdown == '#ddlFundingAgency') {
        message = '<h4><label style="font-weight:normal"> Loading Agencies... </label></h4>';
    }

    $(dropdown).empty();
    blockPage();
    $.post(action, map, function (data) {
        $.each(data, function () {

            if (this.Selected == true) {
                $(dropdown).append("<option selected value=" + this.Value + ">" + this.Text + "</option>");
            }
            else {
                $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            }
        });

    }, "json");
    unblockPage();
}

function GetAllocationData() {

    $.ajax({
        type: 'POST',
        datatype: 'json',
        url: '/Fund/GetAllocationData/',
        data: { stateCode: $("#ddlState").val(), fundType: $("#ddlFundType").val(), yearCode: $("#ddlYear").val(), executingAgencyCode: $("#ddlExecutingAgency").val(), agencyCode: $("#ddlFundingAgency").val() },
        async: false,
        cache: false,
        success: function (data) {
            if (data.success) {
                $("#txtTransaction").html(data.transactionNo);
                $("#txtTotalAllocation").html(data.totalAllocation + " Cr");
                var remaining = data.totalAllocation - data.remainingAllocation;
                $("#txtRemainingAllocation").html(remaining + " Cr");
            }
            else {
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert(xhr.responseText);
        }
    })

}
function ValidateReleaseAmount() {

    $.ajax({

        type: 'POST',
        url: '/Fund/CheckReleaseAmount?' + $.param({ stateCode: $("#ddlState option:selected").val(), yearCode: $("#ddlYear option:selected").val(), executingAgency: $("#ddlExecutingAgency option:selected").val(), fundType: $("#ddlFundType option:selected").val(), fundingAgency: $("#ddlFundingAgency option:selected").val() }),
        async: false,
        cache: false,
        success: function (data)
        {
            if (data.success == true)
            {
                isValid = true;
            }
            else if (data.success == false)
            {
                isValid = false;
            }
        },
        error: function ()
        {
            
        }
    });

    
}
