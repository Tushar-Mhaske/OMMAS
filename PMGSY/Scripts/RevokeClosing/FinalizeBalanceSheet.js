

$(document).ready(function () {
    
    $.validator.unobtrusive.parse($("#frmFinalizeBalanceSheet"));


    $("#txtAuditDate").datepicker({
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a date',
        changeMonth: true,
        buttonImageOnly: true,
        buttonText: 'Date',
        changeYear: true,
        dateFormat: "dd/mm/yy",
        maxDate: new Date(),
    });

    $("#btnView").click(function () {
        if ($("#frmFinalizeBalanceSheet").valid()) {
           
                if (confirm("Once Finalize, Account Can't be Revoke And Alter.")) {
                    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                    $.ajax({
                        url: "/RevokeClosing/FinalizeBalanceSheet/",
                        type: "POST",
                        async: false,
                        cache: false,
                        data: $("#frmFinalizeBalanceSheet").serialize(),
                        success: function (data) {
                            $.unblockUI();
                            if (data.status) {
                                alert(data.message);
                                $("#mainDiv").load("/RevokeClosing/FinalizeBalanceSheet");
                            } else {
                                alert(data.message);
                                return false;
                            }
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            $.unblockUI();
                            alert("Error while processing request");
                        }
                    });
                }           
        }
        $.unblockUI();  
    });
});


//function IsBalanceSheetFinalize()
//{
//    var status;

//    $.ajax({
//        url: "/RevokeClosing/IsBalanceSheetFinalize/",
//        type: "POST",
//        async: false,
//        cache: false,
//        data: { year: $("#Year").val() },
//        success: function (data) {
//           // alert(data.status);
//            var status = data.status;
//            return status;
//        },
//        error: function (xhr, ajaxOptions, thrownError) {            
//            alert("Error while processing request");
//            var status = false;
//            return false;
//        }
//    });

//    return status;
//}