$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmAddRepakagingDetails'));

    $("#divError").hide();

    $("#rdoNewPackage").click(function () {
        $("#tdNewPackage").show('slow');
        $(".tdExistingPackage").hide('slow');

    });

    $("#rdoNewPackage").trigger('click');

    $("#rdoOldPackage").click(function () {
        $(".tdExistingPackage").show('slow');
        $("#tdNewPackage").hide('slow');
    });

    $('#ddlYear').change(function () {
        $("#ddlNewPackage").empty();
        $.ajax({
            type: 'POST',
            url: '/Proposal/PopulatePackages/',
            async: false,
            data: { Year: $('#ddlYear').val(), ProposalCode: $('#EncProposalCode').val() },
            beforeSend: function () {
                blockPage();
            },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlNewPackage").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlNewPackage").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }

                unblockPage();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }
        })
    });

    $("#btnSave").click(function () {

        if ($("#rdoOldPackage").is(':checked')) {
            $("#NEW_PACKAGE_ID").val($("#rdoOldPackage option:selected").val());
        }

        if ($("#frmAddRepakagingDetails").valid()) {
            $.ajax({
                type: 'POST',
                url: '/Proposal/AddRepackagingDetails/',
                async: false,
                data: $("#frmAddRepakagingDetails").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                success: function (data) {
                    if (data.success) {
                        alert('Package changed successfully.');
                        CloseDetails();
                        $("#tblstProposalRepackage").trigger('reloadGrid');
                        unblockPage();
                        $("#divError").hide();
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
        else {
            return false;
        }

    });

    $("#btnReset").click(function (e) {

        if ($("#rdoOldPackage").is(':checked')) {
            $("#rdoNewPackage").trigger('click');
        }
    });

});