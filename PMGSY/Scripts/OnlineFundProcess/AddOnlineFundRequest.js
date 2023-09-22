var isValid = false;

$(document).ready(function () {

    $('#btnAddRequest').click(function () {

        if ($('#frmAddRequest').valid()) {
            $.ajax({
                type: 'POST',
                url: '/OnlineFund/AddOnlineFundRequest',
                async: false,
                cache: false,
                data:$('#frmAddRequest').serialize(),
                success: function (data) {
                    if (data.Success == true) {
                        alert("Request saved successfully.");
                        $('#imgCloseDetails').trigger('click');
                        $("#tblstRequests").trigger('reloadGrid');
                    }
                    else {
                        alert(data.ErrorMessage);
                    }
                },
                error: function (data) {
                    alert("Error occurred while processing your request.");
                }
            });
        }
        else {
            return false;
        }
    });

    $('#btnUpdateRequest').click(function () {

        if ($('#frmAddRequest').valid()) {
            $.ajax({
                type: 'POST',
                url: '/OnlineFund/UpdateOnlineFundRequest',
                async: false,
                cache: false,
                data:$('#frmAddRequest').serialize(),
                success: function (data) {
                    if (data.Success == true) {
                        alert(data.ErrorMessage);
                    }
                    else {
                        alert(data.ErrorMessage);
                    }
                },
                error: function (data) {
                    alert("Error occurred while processing your request.");
                }
            });
        }
        else {
            return false;
        }
    });

    $('#imgCloseDetails').click(function () {

        $('#dvFilterRequest').show('slow');
        $('#divAddRequestDetails').hide('slow');
        $('#btnAddRequestForm').show('slow');
    });


    $('#ddlAddYears,#ddlAddBatches,#ddlAddCollaborations,#ddlAddSchemes').change(function () {

        if ($('#ddlAddYears option:selected').val() > 0 && $("#ddlAddBatches option:selected").val() > 0 && $("#ddlAddCollaborations option:selected").val() > 0 && $("#ddlAddSchemes option:selected").val() > 0) {
            ValidateConditionImposed();
            
                $.ajax({

                    type: 'POST',
                    url: '/OnlineFund/GetTotalDetailsOfState/' + $('#ddlAddYears option:selected').val() + "$" + $('#ddlAddBatches option:selected').val() + "$" + $('#ddlAddCollaborations option:selected').val() + "$" + $('#ddlAddSchemes option:selected').val(),
                    async: false,
                    cache: false,
                    success: function (data) {
                        if (data.Success == true) {
                            $('#lblRoads').text(data.ProposalDetails.totalRoads);
                            $('#lblBridges').text(data.ProposalDetails.totalBridges);
                            $('#lblPavLength').text(data.ProposalDetails.totalPavementLength);
                            $('#lblBridgeLength').text(data.ProposalDetails.totalBridgeLength);
                            $('#lblMordCost').text(data.ProposalDetails.totalMordCost);
                            $('#lblStateCost').text(data.ProposalDetails.totalStateCost);
                            $('#lblSanctionCost').text(data.ProposalDetails.totalSanctionCost);
                            $('#lblMaintenanceCost').text(data.ProposalDetails.totalMaintenanceCost);
                        }
                        else if (data.Success == false) {

                        }
                    }
                });
            
        }
    });

    $('#ddlAddYears').change(function () {

        if ($('#ddlAddYears').val() > 0) {
            FillInCascadeDropdown({ userType: $("#ddlAddYears").find(":selected").val() },
                         "#ddlAddBatches", "/OnlineFund/PopulateBatchByYear?yearCode=" + $('#ddlAddYears option:selected').val());
        }

    });

});
function ValidateConditionImposed()
{
    

        $.ajax({
            type: 'POST',
            url: '/OnlineFund/IsConditionImposed/' + $('#ddlAddYears option:selected').val() + "$" + $("#ddlAddBatches option:selected").val() + "$" + $("#ddlAddCollaborations option:selected").val() + "$" + $("#ddlAddSchemes option:selected").val(),
            async: false,
            cache: false,
            success: function (data) {
                if (data.Success == true) {
                    isValid = true;
                }
                else {
                    alert('Please reply on previous conditions imposed.');
                }
            },
            error: function (data) {
                alert("Error occurred while processing your request.");
            }
        });
    
}
function FillInCascadeDropdown(map, dropdown, action) {

    var message = '';
    if (dropdown == '#ddlProposalWorks') {
        message = '<h4><label style="font-weight:normal"> Loading Proposal Works... </label></h4>';
    }
    else {
        message = '<h4><label style="font-weight:normal"> Loading Agreements... </label></h4>';
    }

    $(dropdown).empty();
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
} 

