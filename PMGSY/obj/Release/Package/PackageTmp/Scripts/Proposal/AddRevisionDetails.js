$(document).ready(function () {


    $.validator.unobtrusive.parse("frmAddRevisedCostLength");

    $(":input").bind("keypress", function (e) {
        if (e.keyvalue == 13) {
            return false;
        }
    })


    $("#btnAddDetails").click(function () {

        if ($("#frmAddRevisedCostLength").valid()) {

            $.ajax({
                type: 'POST',
                url: '/Proposal/AddRevisedCostLength/',
                data: $("#frmAddRevisedCostLength").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $("#tblstRevisedCost").trigger('reloadGrid');
                        $("#divAddRevisedDetails").load('/Proposal/AddRevisionDetails?id=' + $("#IMS_PR_ROAD_CODE").val(), function () {
                            $.validator.unobtrusive.parse($('#frmAddRevisedCostLength'));
                            $("#IMS_NEW_PAV_COST,#IMS_NEW_CD_COST,#IMS_NEW_PW_COST,#IMS_NEW_OW_COST,#IMS_NEW_FC_COST").trigger('blur');
                            unblockPage();
                        });
                    }
                    else if (data.success == false) {
                        $("#divError").show();
                        $("#divError").html('<strong>Alert : </strong>' + data.message);
                    }
                },
                error: function () {
                    alert("Request can not be processed at this time.");
                }
            })
        }

    });

    //update road details button click
    $("#btnUpdateDetails").click(function () {

        if ($("#frmAddRevisedCostLength").valid()) {
            $.ajax({
                type: 'POST',
                url: '/Proposal/EditRevisionDetails/',
                data: $("#frmAddRevisedCostLength").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $("#tblstRevisedCost").trigger('reloadGrid');
                        $("#divAddRevisedDetails").load('/Proposal/AddRevisionDetails?id=' + $("#IMS_PR_ROAD_CODE").val(), function () {
                            $.validator.unobtrusive.parse($('#frmAddRevisedCostLength'));
                            $("#IMS_NEW_PAV_COST,#IMS_NEW_CD_COST,#IMS_NEW_PW_COST,#IMS_NEW_OW_COST,#IMS_NEW_FC_COST").trigger('blur');
                            unblockPage();
                        });
                    }
                    else if (data.success == false) {
                        $("#divError").show();
                        var messages = [];
                        messages = data.message.split('$');
                        for (var i = 0; i < messages.length; i++) {
                            if (i == 0) {
                                $("#divError").html('<strong>Alert : </strong>' + messages[i]);
                            }
                            else {
                                $("#divError").append('<br/><strong>Alert : </strong>' + messages[i]);
                            }
                        }
                    }
                },
                error: function () {
                    alert("Request can not be processed at this time.");
                }
            })
        }
    });

    $("#btnCancelDetails").click(function () {

        $("#divAddRevisedDetails").load('/Proposal/AddRevisionDetails?id=' + $("#IMS_PR_ROAD_CODE").val(), function () {
            $.validator.unobtrusive.parse($('#frmAddRevisedCostLength'));
            unblockPage();
        });

    });

    $("#btnResetRoadDetails").click(function () {

        $("#divError").hide('slow');
        EnableValues();
    })


    $("#IMS_NEW_PAV_COST,#IMS_NEW_CD_COST,#IMS_NEW_PW_COST,#IMS_NEW_OW_COST,#IMS_NEW_FC_COST").blur(function () {

        var pavCost = 0;
        var cdCost = 0;
        var otherworkCost = 0;
        var protectionCost = 0;
        var hsCost = 0;
        var fcCost = 0;
        var totalCost = 0;
        var totalCostInHS = 0;

        if (Number($("#IMS_NEW_PAV_COST").val()) != NaN) {
            pavCost = Number($("#IMS_NEW_PAV_COST").val());
        }

        if (Number($("#IMS_NEW_CD_COST").val()) != NaN) {
            cdCost = Number($("#IMS_NEW_CD_COST").val());
        }

        if (Number($("#IMS_NEW_PW_COST").val()) != NaN) {
            protectionCost = Number($("#IMS_NEW_PW_COST").val());
        }

        if (Number($("#IMS_NEW_OW_COST").val()) != NaN) {
            otherworkCost = Number($("#IMS_NEW_OW_COST").val());
        }

        if (Number($("#IMS_NEW_FC_COST").val()) != NaN) {
            fcCost = Number($("#IMS_NEW_FC_COST").val());
        }

        if (Number(pavCost) != NaN && Number(cdCost) != NaN && Number(protectionCost) != NaN && Number(otherworkCost) != NaN && Number(fcCost) != NaN) {
            totalCost = pavCost + cdCost + protectionCost + otherworkCost + fcCost;
            //totalCost = Number(pavCost).toFixed(2) + Number(cdCost).toFixed(2) + Number(protectionCost).toFixed(2) + Number(otherworkCost).toFixed(2) + Number(fcCost).toFixed(2);
        }

        if (Number(pavCost) != NaN && Number(cdCost) != NaN && Number(protectionCost) != NaN && Number(otherworkCost) != NaN && Number(fcCost) != NaN && Number(hsCost) != NaN) {
            totalCostInHS = pavCost + cdCost + protectionCost + otherworkCost + fcCost + hsCost;
            //totalCostInHS = Number(pavCost).toFixed(2) + Number(cdCost).toFixed(2) + Number(protectionCost).toFixed(2) + Number(otherworkCost).toFixed(2) + Number(fcCost).toFixed(2) + Number(hsCost).toFixed(2);
        }
        
        if ($("#SharePercent").val() == 1) {
            $("#lblStateShare").html(Number(totalCost * 0.10).toFixed(2));
            $("#lblMoRDShare").html(Number(totalCost * 0.90).toFixed(2));
        }
        else if ($("#SharePercent").val() == 2) {
            $("#lblStateShare").html(Number(totalCost * 0.25).toFixed(2));
            $("#lblMoRDShare").html(Number(totalCost * 0.75).toFixed(2));
        }

        $("#lblTotalCost").html(Number(totalCostInHS).toFixed(2));
        $("#lblTotalCostExHS").html(Number(totalCost).toFixed(2));

    });


});