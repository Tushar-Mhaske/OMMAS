$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmUpdateDetails'));

    $("#ddlYear").change(function () {

        //alert("Change");
        if ($("#ddlYear").val() > 0) {

            $("#ddlBatch").empty();

            $.ajax({
                url: '/Proposal/PoulateUnFreezedBatches',
                type: 'POST',
                beforeSend: function () {
                    blockPage();
                },
                data: { IMS_YEAR: $("#ddlYear option:selected").val(), value: Math.random() },
                success: function (jsonData) {
                    unblockPage();
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlBatch").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    if (jsonData.length == 1) {
                        alert("All the Batches against selected year are Freezed.");
                        return false;
                    }
                },
                error: function (err) {
                    alert("error " + err);
                    unblockPage();
                }
            });
        }
        else {
            $('#ddlBatch').children('option:not(:first)').remove();
        }
    });
    

    $("#btnUpdate").click(function () {
        if ($("#frmUpdateDetails").valid()) {
            $.ajax({

                type: 'POST',
                async: false,
                url: '/Proposal/UpdateProposalDetails/',
                data: $("#frmUpdateDetails").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        CloseDetails();
                        $("#tbUpdateProposalList").trigger('reloadGrid');
                        $("#divError").hide();
                        unblockPage();
                    }
                    else if (data.success == false) {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        $.validator.unobtrusive.parse($('#mainDiv'));
                        unblockPage();
                    }
                },
                error: function () {
                    alert('Error occurred while processing your request.');
                },
            });
        }
        else {
            return false;
        }



    });

});


