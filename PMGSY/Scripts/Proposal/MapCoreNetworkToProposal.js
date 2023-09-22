$(document).ready(function () {

    $("#btnListProposal").click(function () {
        LoadProposalsForUpdate($("#ddlImsYear").val(), $("#ddlDistrict").val(), $("#ddlMastBlockCode").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val(), $("#ddlImsConnectivity").val());
    });


    $("#ddlBlocks").change(function () {

        $.ajax({
            url: '/Proposal/GetLinkThroughList',
            type: 'POST',
            beforeSend: function () {
                blockPage();
            },
            data: { BlockID: $("#ddlBlocks option:selected").val(), IMS_UPGRADE_CONNECT: $("#UpgradeConnect").val(),PROPOSAL_TYPE:$("#ProposalType").val(), value: Math.random() },

            success: function (jsonData) {
                $("#ddlCoreNetworks").empty();
                for (var i = 0; i < jsonData.length; i++) {
                    $("#ddlCoreNetworks").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
                unblockPage();
            },
            error: function (err) {
                alert("Error while getting Link/Through Routes.");
                unblockPage();
            }
        });

    });


    $("#btnUpdate").click(function () {

        if ($("#frmUpdateDetails").valid()) {
            $.ajax({

                type: 'POST',
                async: false,
                url: '/Proposal/MapCoreNetworkDetails/',
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