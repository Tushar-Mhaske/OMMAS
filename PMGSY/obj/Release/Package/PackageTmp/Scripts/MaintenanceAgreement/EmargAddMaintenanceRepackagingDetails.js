$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmAddMaintenanceRepakagingDetails'));

    $("#divError").hide();
    $("#txtStartChainage").val('');
    $("#txtEndChainage").val('');


    $("#rdoNewPackage").click(function () {
        $("#tdNewPackage").show('slow');
        $("#tdExistingPackage").hide();

    });

    $("#rdoOldPackage").click(function () {
        $("#tdExistingPackage").show('slow');
        $("#tdNewPackage").hide();
    });



    $("#btnSave").click(function () {

        if ($("#rdoOldPackage").is(':checked')) {
            $("#NEW_PACKAGE_ID").val($("#rdoOldPackage option:selected").val());
        }

        //var startChainage =  $('#txtStartChainage').val();

        //var endChainage = $('#txtEndChainage').val();

        //var chainageLength = $('#txtEndChainage').val() - $('#txtStartChainage').val();

        //var actualRoadLength = $('#roadLength').val();

       
        //commented deendayal code as per suggestion of pankaj sir

        //if ( chainageLength > actualRoadLength ||  startChainage > actualRoadLength ||  endChainage > actualRoadLength) {

        //    alert(" Start chainage and End Chainage can not be greater than actual road length ");
        //    return false;

        //}
        //else {

        //   SaveRepackaingDetails();
        //}

        SaveRepackaingDetails();

    });

    $("#btnReset").click(function (e) {

        if ($("#rdoOldPackage").is(':checked')) {
            $("#rdoNewPackage").trigger('click');
        }
    });

});

function SaveRepackaingDetails() {

    if ($("#frmAddMaintenanceRepakagingDetails").valid()) {
        if (confirm("After repackaging, details can not be changed. Are you sure to Repackage this Road Details ?")) {
            $.ajax({
                type: 'POST',
                url: '/MaintenanceAgreement/AddEmargMaintenanceRepackagingDetails/',
                async: false,
                data: $("#frmAddMaintenanceRepakagingDetails").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                success: function (data) {
                    if (data.success) {
                        alert('Road details repackaged successfully.');
                        CloseDetails();
                        $("#tblstProposalRepackage").trigger('reloadGrid');
                        unblockPage();
                        $("#divError").hide();
                    }
                    else {

                        alert(data.message);
                    //    $("#divError").show("slow");
                     //   $("#divError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
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
    }
    else {
        return false;
    }

}