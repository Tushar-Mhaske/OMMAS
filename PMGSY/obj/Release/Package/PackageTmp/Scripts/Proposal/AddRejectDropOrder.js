$(document).ready(function () {
    $('#txtDropOrderNumber').focus(); //kepp focues on first field
    $.validator.unobtrusive.parse($('#frmAddEditDropOrder'));

    $('#txtDropOrderDate').val('');

    $('#txtDropOrderDate').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a date',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: false,
        maxDate: new Date(),
        minDate: $('#IMS_REQUEST_ORDER_DATE').val(),
        onSelect: function (selectedDate) {
        },
        onClose: function () {
            $(this).focus().blur();
        }
    });

    $("#btnSave").click(function () {
      //  var records = $('#tblstDropProposal').jqGrid('getGridParam', 'records');
      //  var SubmittedArray = [];

       // $.each($("input[name='Approve']:checked"), function (i, value) {
      //      SubmittedArray[i] = $(this).val().trim();
      //  });
      //  alert('records :' + records + "lenght checked :" + SubmittedArray.length)
       // if (records == SubmittedArray.length) {
        if ($("#frmAddEditDropOrder").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                $.ajax({
                    type: 'POST',
                    url: '/Proposal/AddRejectDropOrder?',
                    async: false,
                    data: $("#frmAddEditDropOrder").serialize(),
                    beforeSend: function () {
                        $.unblockUI();
                    },
                    success: function (data) {
                        if (data.success) {
                            alert(data.message);
                           // SubmittedArray.splice(0, SubmittedArray.length);
                            CloseDetails();
                            $("#tblstDropProposal").trigger('reloadGrid');
                            $("#tblstDroppedOrder").trigger('reloadGrid');
                            $("#tblProposalsForDroppingMRD").trigger('reloadGrid');
                            
                            $('#dropAll').prop('checked', false); //after reloading Grid [All Approve] button should be uncheck

                            if ($('#dvDetailDroppedOrderList').is(":visible"))
                            {
                                $('#tblstDetailDroppedOrder').trigger("reloadGrid");
                            }
                            $.unblockUI();
                        }
                        else {
                            $("#errorDiv").show("slow");
                            $("#errorDiv span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                            $.validator.unobtrusive.parse($('#mainDiv'));
                            $('#dropAll').prop('checked', false);//after reloading Grid [All Approve] button shoulb be uncheck
                            $('.dropped').prop('checked',false);
                            // SubmittedArray.splice(0, SubmittedArray.length);
                            $.unblockUI();
                        }
                        
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.responseText);
                    }
                });
            }

            else {
                return false;
            }
        //}
        //else {
        //    alert('Please select all roads to  generate drop order');
        //    CloseDetails();
        //    $('#tblstDropProposal').jqGrid('setGridState', 'visible');

        //}

    });

});