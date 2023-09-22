$(document).ready(function () {

    var currentTime = new Date() 
    var mindate = new Date(currentTime.getFullYear(), currentTime.getMonth(), +1); //one day next before month
    var maxdate =  new Date(currentTime.getFullYear(), currentTime.getMonth() +1, +0); // one day before next month

    $('.plcd').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Planned Date',       
        maxDate: maxdate, // to enable current month dates
        minDate: mindate, // to enable current month dates
        buttonImageOnly: true,
        buttonText: 'Planned Date',
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {

            $('.plcd').trigger('blur');
            $("#frmPlanDetailsEdit").valid();
            $(this).change();
            $('.input-validation-error').addClass('input-validation-valid');
            $('.input-validation-error').removeClass('input-validation-error');
            //Removes validation message after input-fields
            $('.field-validation-error').addClass('field-validation-valid');
            $('.field-validation-error').removeClass('field-validation-error');
            //Removes validation summary 
            $('.validation-summary-errors').addClass('validation-summary-valid');
            $('.validation-summary-errors').removeClass('validation-summary-errors');
        }

    })
   
});


function isNumericKeyStroke(event) {
    var returnValue = false;
    var keyCode = (event.which) ? event.which : event.keyCode;
    if (((keyCode >= 48) && (keyCode <= 57)) || (keyCode == 46) || (keyCode == 8) || (keyCode == 9) || (keyCode == 37) || (keyCode == 39))// All numerics
    {
        returnValue = true;
    }
    if (event.returnValue)
        event.returnValue = returnValue;
    return returnValue;
}

// btn on GetPlanDetailsToEdit.cshtml
$("#btnUpdateRoadDetail").click(function () {

    var listType = $("#ddlListType option:selected").val();
    var planid = parseInt($("#planId").val());
    var baselineno = parseInt($("#baselineNo").val());
    var completionlength = Number($("#completionlength").val()).toFixed(3);
       
    /// completion date format for validation
    var oldcompletiondate = $('#completiondate').val();
    var completiondate = oldcompletiondate.split("/").reverse().join("-");

    /// for comapare with current month
    var currentMonthYear = new Date();
    var newcompletionDate = new Date(completiondate);
    
    if (planid > 0 && baselineno > 0)
    {
        if (isNaN(completionlength) || completionlength > 50)
        {
            alert("Please enter correct length & length should be between 0-50km");
        }
         /// check valid date
        else if (!completiondate.match(/^\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])$/)) {
            alert("Please Enter Valid Date");
        }
        else if (newcompletionDate.getMonth() != currentMonthYear.getMonth() && newcompletionDate.getYear() != currentMonthYear.getYear()) {
            alert("Please Enter Current Month Date");
        }
        else {
            UpdatePlanCompletionDetails(listType, planid, baselineno, completionlength, completiondate);
        }
    }
    else
    alert("Error while processing your request...");
      
}); // btn UpdateRoadDetail end



function UpdatePlanCompletionDetails(listType, planid, baselineno, completionlength, completiondate) {

    if (confirm("Confirm to save changes ?")) {
        $.ajax({
            url: '/PMIS/PMIS/UpdatePlanCompletionDetails',
            type: "POST",
            cache: false,
            async: false,
            data: { 'listType': listType, 'planid': planid, 'baselineno': baselineno, 'completionlength': completionlength, 'completiondate': completiondate },
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },            
            success: function (response) {
                if (response.success) {
                    alert("Details Updated Successfully...");

                    // call from PMISDataDeletion.js to close this form
                    CloseDataDeleteUpdateForm();
                }
                else {
                    alert(response.errorMessage);
                }
                $.unblockUI();
            },
            error: function () {

                $.unblockUI();
                alert("Error : " + error);
                return false;
            }
        });

    }   // end of if block
}