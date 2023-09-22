
$(document).ready(function () {
  
    $.validator.unobtrusive.parse('#fillMPVisitForm');

    //Date Picker 
    $('#DateOfVisit').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose date',
        buttonImageOnly: true,
        maxDate: $("#CurrentDate").val(),
        buttonText: "select date",
        onSelect: function (selectedDate) {
        }
    });

  

    //Save 
    $("#btnSave").click(function (e) {

        if ($("#fillMPVisitForm").valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({

                type: 'POST',
                url: '/QualityMonitoring/AddMPVisitDetails',
                async: false,
                data: $("#fillMPVisitForm").serialize(),
                success: function (data) {

                    if (data.success == true) {
                        alert(data.message);
                        //1.Grid Will be triggered.
                        $("#tbList").trigger('reloadGrid');
                        ////2.Add Edit view will be reloaded.
                        //  $("#divfillMPVisitForm").load('/QualityMonitoring/FillMPVisitDetails');
                        $('#fillMPVisitForm').each(function () {
                            this.reset();
                        });
                        $.unblockUI();
                    }
                    else if (data.success == false) {
                        if (data.message != "") {

                            $("#message").html(data.html);
                            $("#dvErrorMessage").show('slow');
                            $.unblockUI();
                        }
                    }
                    else {
                        $("#divfillMPVisitForm").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblockUI();
                }

            })

        }


    });

    //Reset
     $('#btnReset').click(function (e) {
        $('#fillMPVisitForm').each(function () {
            this.reset();
        });
     });

    //This method is for cancel button click
     $("#btnCancel").click(function () {
       
        // $("#divfillMPVisitForm").load('/QualityMonitoring/FillMPVisitDetails');
         $("#divfillMPVisitForm").load("/QualityMonitoring/FillMPVisitDetails/" + $("#PrRoadCode").val());
     });



    // Update
     $('#btnUpdate').click(function (e) {

         if ($('#fillMPVisitForm').valid()) {

             $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

             $.ajax({
                 url: "/QualityMonitoring/UpdateMPVisitDetails",
                 type: "POST",
                 data: $("#fillMPVisitForm").serialize(),
                 success: function (data) {
                     if (data.success == true) {
                         alert(data.message);

                         $('#tbList').trigger('reloadGrid');
                         $("#divfillMPVisitForm").load("/QualityMonitoring/FillMPVisitDetails/" + $("#PrRoadCode").val());
                     }
                     else if (data.success == false) {
                         if (data.message != "") {
                             $('#message').html(data.message);
                             $('#dvErrorMessage').show('slow');
                         }
                     }
                     else {
                         $("#divfillMPVisitForm").html(data);
                     }
                     $.unblockUI();
                 },
                 error: function (xhr, ajaxOptions, thrownError) {
                     $.unblockUI();
                 }
             });
         }
     });



   

});





