$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmAllLockDetails');

    $("#btnCancel").click(function () {

        CloseLockDetails();
    });

    var status =  $("#LockStatus").val();

    $("#btnSubmit").click(function () {


        var proposalCode = $("#IMS_PR_ROAD_CODE").val();
        var existingCode = $("#MAST_ER_ROAD_CODE").val();
        var roadCode = $("#PLAN_CN_ROAD_CODE").val();

        if ($("#frmAddLockDetails").valid()) {
            $.ajax({
                type: 'POST',
                url: '/LockUnlock/AddLockDetails/',
                async: false,
                data: $("#frmAddLockDetails").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                success: function (data) {
                    if (data.success) {
                        alert(data.message);
                        CloseLockDetails();
                        switch ($("#IMS_UNLOCK_TABLE").val()) {
                            case "IR":
                                $("#tbProposalList").trigger("reloadGrid");
                                $("#tbProposalUnlockList").trigger("reloadGrid");
                                $("#tbProposalList").jqGrid('setGridState', 'visible');
                                $("#tbProposalUnlockList").jqGrid('setGridState', 'visible');
                                break;
                            case "ER":
                                $("#tbExistingRoadList").trigger("reloadGrid");
                                $("#tbExistingRoadsUnlockList").trigger("reloadGrid");
                                $("#tbExistingRoadList").jqGrid('setGridState', 'visible');
                                $("#tbExistingRoadsUnlockList").jqGrid('setGridState', 'visible');
                                break;
                            case "PR":
                                $("#tbCoreNetworkList").trigger("reloadGrid");
                                $("#tbCoreNetworkUnlockList").trigger("reloadGrid");
                                $("#tbCoreNetworkList").jqGrid('setGridState', 'visible');
                                $("#tbCoreNetworkUnlockList").jqGrid('setGridState', 'visible');
                                break;
                            case "AD":
                                $("#tbAgreementList").trigger("reloadGrid");
                                $("#tbAgreementList").jqGrid('setGridState', 'visible');
                                break;
                            case "NT":
                                $("#tbTenderingList").trigger("reloadGrid");
                                $("#tbTenderingList").jqGrid('setGridState', 'visible');
                                break;
                            default:
                                break;
                        }
                        
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
                    alert("Error occurred while processing the request.");
                }
            })

        }


    });

    $('#IMS_UNLOCK_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a date',
        buttonImageOnly: true,
        minDate: new Date(),
        changeMonth: true,
        changeYear:true,
        onSelect: function (selectedDate) {

        }
    });

    $('#IMS_AUTOLOCK_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a date',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        minDate: new Date(),
        onSelect: function (selectedDate) {

        }
    });


});