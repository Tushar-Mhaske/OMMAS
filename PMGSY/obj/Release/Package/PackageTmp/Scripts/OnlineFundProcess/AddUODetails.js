$(document).ready(function () {

    $('#btnAddObservation').click(function () {

        $.ajax({

            type: 'POST',
            url: '/OnlineFund/AddUODetails',
            cache: false,
            async: false,
            data:$('#frmAddUODetails').serialize(),
            success: function (data) {
                if (data.Success == true)
                {
                    alert('Request details updated successfully.');
                    AddRequestUODetails($("#RequestId").val());
                    //$('#dvReleaseDetails').load('/OnlineFund/AddUODetails/' + $("#RequestId").val());
                }
                else if (data.Success == false)
                {
                    alert(data.ErrorMessage);
                }
            },
            error: function () {
            }
        });

    });


    $('#RELEASE_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'Date',
        onSelect: function (selectedDate) {
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });


});