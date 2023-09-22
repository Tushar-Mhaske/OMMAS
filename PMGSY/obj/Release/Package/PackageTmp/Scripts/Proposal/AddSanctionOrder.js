$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmAddEditSanctionOrder'));

    $('#txtOrderDate').val('');

    $('#txtOrderDate').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a date',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        maxDate: new Date(),
        onSelect: function (selectedDate) {
        },
        onClose: function () {
            $(this).focus().blur();
        }
    });

    $("#btnSave").click(function () {
         
        if ($("#frmAddEditSanctionOrder").valid()) {
            $.ajax({
                type: 'POST',
                url: '/Proposal/AddSanctionOrder/',
                async: false,
                data: $("#frmAddEditSanctionOrder").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                success: function (data) {
                    if (data.success) {
                        alert('Sanction Order Generated Successfully.');
                        CloseDetails();
                        $("#tblstProposal").trigger('reloadGrid');
                        $("#tblstSanctionOrder").trigger('reloadGrid');
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
                    alert(xhr.responseText);
                }
            })
        }
        else
        {
            return false;
        }

    });

});