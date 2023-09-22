//$(document).ready(function () {

//    $.validator.unobtrusive.parse("#frmQMAutoScheduleDetailsLayout");


//    $('#btnAddNQMDetails').click(function () {

//        if (!$('#frmQMAutoScheduleDetailsLayout').valid()) {
//            return false;
//        }

//        $.ajax({
//            url: "/QualityMonitoring/QMAddAutoScheduleDetails",
//            type: "POST",
//            dataType: "json",
//            async: false,
//            cache: false,
//            data: $("#frmQMAutoScheduleDetailsLayout").serialize(),
//            success: function (data) {
//                if (data.Success) {
//                    alert(data.message);

//                    window.location.href = "/QualityMonitoring/QMAutoScheduleDetailsLayout";
//                }
//                else {
//                    if (data.message != "") {
//                        alert(data.message);
//                        //$('#message').html(data.message);
//                        //$('#dvErrorMessage').show('slow');
//                    }
//                }
//            }
//        });

//    });
//});







$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmQMAutoScheduleDetailsLayout");


    $('#btnAddNQMDetails').click(function () {

        if (!$('#frmQMAutoScheduleDetailsLayout').valid()) {
            return false;
        }

        $.ajax({
            url: "/QualityMonitoring/QMAddAutoScheduleDetails",
            type: "POST",
            dataType: "json",
            async: false,
            cache: false,
            data: $("#frmQMAutoScheduleDetailsLayout").serialize(),
            success: function (data) {
                if (data.Success) {
                    alert(data.message);

                    window.location.href = "/QualityMonitoring/QMAutoScheduleDetailsLayout";
                }
                else {
                    if (data.Success) {
                        alert(data.ErrorMessage);
                        window.location.href = "/QualityMonitoring/QMAutoScheduleDetailsLayout";
                        //$('#message').html(data.message);
                        //$('#dvErrorMessage').show('slow');
                    }
                    else {
                        alert(data.ErrorMessage);
                        // Now Check
                    }
                }
            }
        });

    });
});