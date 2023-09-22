$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmUpdateDetails'));

    $("#ddlStagedYears").change(function () {

        $.ajax({
            url: '/Proposal/GetPackageIdSRRDA',
            type: 'POST',
            beforeSend: function () {
                blockPage();
            },
            data: { Year: $("#ddlStagedYears option:selected").val(), BatchID: $("#Batch").val(), AdminCode: $("#AdminCode").val(), value: Math.random() },
            success: function (jsonData) {
                if (jsonData.length == 0) {
                    alert("No Package found for Selected Year and Batch");
                    unblockPage();
                }
                $("#ddlStagedPackages").empty();
                $("#ddlStagedPackages").append("<option value='0'>Select Staged Package</option>");
                for (var i = 0; i < jsonData.length; i++) {


                    $("#" + $("#ddlStagedPackages").attr("ID")).append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
                unblockPage();
            },
            error: function (err) {
                alert("error " + err);
                unblockPage();
            }

        });

    });


    $("#ddlStagedPackages").change(function () {

        $.ajax({
            url: '/Proposal/GetStagedProposalListSRRDA',
            type: 'POST',
            beforeSend: function () {
                blockPage();
            },
            data: { year: $("#ddlStagedYears option:selected").val(), packageID: $("#ddlStagedPackages option:selected").val(),AdminCode :$("#AdminCode").val(),DistrictCode : $("#DistrictCode").val(), value: Math.random() },
            success: function (jsonData) {

                $("#ddlStagedProposals").empty();
                $("#ddlStagedProposals").append("<option value='0'>Select Staged Road</option>");
                if (jsonData.length == 0) {
                    alert("No Stage1 Proposal found for Selected Year and Package.");
                    unblockPage();
                }
                for (var i = 0; i < jsonData.length; i++) {
                    $("#ddlStagedProposals").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
                unblockPage();
            },
            error: function (request, status, error) {

                unblockPage();
            }

        });

    });


    $("#btnUpdate").click(function () {
        if ($("#frmUpdateDetails").valid()) {
            $.ajax({

                type: 'POST',
                async: false,
                url: '/Proposal/ChangeStage1ProposalToStage2/',
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