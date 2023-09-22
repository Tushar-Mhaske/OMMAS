$(document).ready(function () {
    $("#ddlCoreNetworks").empty();
    $("#ddlCoreNetworks").append("<option value=" + "0" + ">" + "Select DRRP" + "</option>");

    $("#btnUpdate").click(function () {

        if ($("#ddlDistrictS option:selected").val() == 0)
        {
            alert("Select District");
            return false;
        }

        if ($("#frmUpdateDetails").valid()) {
            $.ajax({

                type: 'POST',
                async: false,
                url: '/ExistingRoads/MapDRRPDetailsUnderPMGSY3/',
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

    $("#ddlDistrictS").change(function () {
        if ($("#ddlDistrictS option:selected").val() == 0)
        {
            $("#ddlCoreNetworks").empty();
            $("#ddlCoreNetworks").append("<option value=" + "0" + ">" + "Select DRRP" + "</option>");
        }
        $.ajax({
            url: "/ExistingRoads/PopulateBlockListOnDistrictSelect/",
            type: "GET",
            cache: false,
            data: { DistrictCode: $("#ddlDistrictS option:selected").val(), statename: $("#ddlDistrictS option:selected").text() },
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                unblockPage();
                $("#ddlBlocks").empty();
                for (var i = 0; i < response.length; i++) {
                    $("#ddlBlocks").append("<option value=" + response[i].Value + ">" + response[i].Text + "</option>");

                }
                $("#ddlBlocks option[value='0']").remove();
                $("#ddlBlocks").prepend("<option selected='selected' value=" + "0" + ">" + "Select Block" + "</option>");
            }
        });
    });

    $("#ddlBlocks").change(function () {
        if ($("#ddlBlocks option:selected").val() == 0) {
            $("#ddlCoreNetworks").empty();
            $("#ddlCoreNetworks").append("<option value=" + "0" + ">" + "Select DRRP" + "</option>");
        }
        $.ajax({
            url: "/ExistingRoads/PopulateDRRPonBlockChange/",
            type: "GET",
            cache: false,
            data: { DistrictCode: $("#ddlBlocks option:selected").val(), statename: $("#ddlBlocks option:selected").text(), upgardeconnect: $("#upgradeConnect").val(), proposaltype: $("#Proposaltype").val()},
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                unblockPage();
                $("#ddlCoreNetworks").empty();
                for (var i = 0; i < response.length; i++) {
                    $("#ddlCoreNetworks").append("<option value=" + response[i].Value + ">" + response[i].Text + "</option>");

                }
                $("#ddlCoreNetworks option[value='0']").remove();
                $("#ddlCoreNetworks").prepend("<option selected='selected' value=" + "0" + ">" + "Select DRRP" + "</option>");
            }
        });
    });



});