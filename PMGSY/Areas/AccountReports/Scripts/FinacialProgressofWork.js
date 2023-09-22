
//validation for month
jQuery.validator.addMethod("monthvalidator", function (value, element, param) {

    var IsMonthly = $('#rdbMonth').val();
    var month = $('#Month').val();

    if ($("#rdbMonth").is(":checked")) {
        if (IsMonthly == 2 && month == 0) {
            return false;   
        }
        else {
            return true;
        }
    } else {
        return true;
    }
});
jQuery.validator.unobtrusive.adapters.addBool("monthvalidator");



$(document).ready(function () {
  
    $.validator.unobtrusive.parse('#frmScheduleFilter');
    $(function () {
        $("#btnView").trigger("click");
    });
    $("#spnFundReceived").click(function () {
        $("#spnLedger").toggleClass("ui-icon ui-icon-circle-triangle-n").toggleClass("ui-icon ui-icon-circle-triangle-s");
        $("#dvFilterForm").slideToggle();
    });

   
   // var ndlevel = $("#LevelId").val();


   
    $("#btnView").click(function () {

            if (($("#Month option:selected").val() == "0"))
            {
                return false;
            }
        

        if ($("#Year option:selected").val() == "0")
        {
            return false;
        }

        else
        {
            data = { month: $("#Month option:selected").val(), Year: $("#Year option:selected").val(), ndcode: $("#Agency option:selected").val(), StateName: $("#Agency option:selected").text() };
            loadFundReceivedReport(data);
        }

    });

    $("#Month").change(function ()
    {
        UpdateAccountSession($("#Month").val(), $("#Year").val());
    });

    $("#Year").change(function ()
    {
        UpdateAccountSession($("#Month").val(), $("#Year").val());
    });

});

//// To validate parameters on the form
//function ValidateForm(param) {
//    $.ajax({
//        url: "/AccountReports/Account/ValidateParameter",
//        type: "POST",
//        data: param,
//        success: function (data)
//        {
//            if (data.success == false)
//            {
//                $("#dvError").show();
//                $("#dvError").html('<strong>Alert : </strong>' + data.message);

//                return false;
//            }
//            else if (data.success == true)
//            {
//                loadFundReceivedReport(param)
//            }

//        }
//    });
//}

// To Load SSRS Report in the Layout page
function loadFundReceivedReport(data)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: '/AccountReports/Account/FinacialProgressReport/',
        type: 'POST',
        catche: false,
        async: false,
        data: data,
        error: function (xhr, status, error) {
            alert('An Error occured while processig your request.');
            $.unblockUI();
            return false;
        },
        success: function (data) {
            //Load Report
            $("#dvLoadReport").html(data);
            //unblockPage();
            $.unblockUI();

        }
    });

}
// to save current month and year in the session 
function UpdateAccountSession(month, year) {
    $.ajax({
        url: "/AccountReports/Account/UpdateAccountSession",
        type: "GET",
        async: false,
        cache: false,
        data:
            {
                "Month": month,
                "Year": year
            },
        success: function (data) {
            return false;
        },
        error: function () { }
    });
    return false;
}
