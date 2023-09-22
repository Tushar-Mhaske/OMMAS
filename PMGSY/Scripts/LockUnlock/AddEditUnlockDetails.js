$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmAddUnlockDetails');


    $("#btnSubmit").click(function () {

        $('#sanctionType').val($('#ddlTypeSearch').val());

        if ($('#UnlockLevel').val() == 'R' && $('#UnlockTable').val() == 'PR')
        {
            $('#StateCode').val($('#ddlStateSearch').val());
            $('#DistrictCode').val($('#ddlDistrictSearch').val());
            $('#BlockCode').val($('#ddlBlockSearch').val());
            $('#YearCode').val($('#ddlYearSearch').val());
            $('#BatchCode').val($('#ddlBatchSearch').val());
            $('#Package').val($('#ddlPackageSearch').val());
        }
        
        if ($("#frmAddUnlockDetails").valid()) {
            $.ajax({
                type: 'POST',
                url: '/LockUnlock/AddUnlockDetails/',
                async: false,
                data: $("#frmAddUnlockDetails").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        CloseUnlockDetails();
                        var level = $("#ddlLevel option:selected").val();
                        switch (level) {
                            case 'S':
                                $("#tblstState").trigger('reloadGrid');
                                break;
                            case 'D':
                                $("#tblstDistricts").trigger('reloadGrid');
                                break;
                            case 'B':
                                $("#tblstBlocks").trigger('reloadGrid');
                                break;
                            case 'V':
                                $("#tblstVillages").trigger('reloadGrid');
                                break;
                            case 'H':
                                $("#tblstHabs").trigger('reloadGrid');
                                break;
                            case 'T':
                                $("#tbPropBatches").trigger('reloadGrid');
                                break;
                            case 'Y':
                                $("#tbPropYears").trigger('reloadGrid');
                                break;
                            case 'R':
                                $("#tblstProposal").trigger('reloadGrid');
                                $("#tblstProposalITNO").trigger('reloadGrid');
                                $("#tblstExistingRoads").trigger('reloadGrid');
                                $("#tblstCoreNetwork").trigger('reloadGrid');
                                break;
                            default:
                                break;
                        }
                        unblockPage();
                    }
                    else if(data.success == false){
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

    if ($("#UnlockBy").val() == "M") {

        $('#UnlockStartDate').datepicker({

            dateFormat: 'dd/mm/yy',
            showOn: "button",
            buttonImage: "/Content/Images/calendar_2.png",
            showButtonText: 'Choose a date',
            buttonImageOnly: true,
            minDate: new Date(),
            changeMonth: true,
            changeYear: true,
            onSelect: function (selectedDate) {

            },
            onClose: function () {
                $(this).focus().blur();
            }
        });

        $('#UnlockEndDate').datepicker({

            dateFormat: 'dd/mm/yy',
            showOn: "button",
            buttonImage: "/Content/Images/calendar_2.png",
            showButtonText: 'Choose a date',
            buttonImageOnly: true,
            changeMonth: true,
            changeYear: true,
            minDate: new Date(),
            onSelect: function (selectedDate) {

            },
            onClose: function ()
            {
                $(this).focus().blur();
            }
        });
    }

});