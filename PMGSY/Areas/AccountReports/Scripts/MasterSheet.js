$(document).ready(function () {

    $('.Top2').bind('scroll', function () {
        $(".Top1").scrollLeft($(this).scrollLeft());

    });

    $(function () {

        $("#btnViewDetails").trigger("click");
    });

    //added by Rohit Jadhav 20 Aug 2014
    $("#btnViewDetails").click(function () {

        if ($("#Year").val() > 0) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: '/AccountReports/Account/MasterSheetReport',
                type: 'POST',
                data: { year: $("#Year").val(), value: Math.random() },
                success: function (response) {
                    $("#masterSheetDetals").html("");
                    $("#masterSheetDetals").html(response);
                    $("#spnYear").html($("#Year option:selected").text());
                    $.unblockUI();

                },
                error: function (err) {
                    $("#divError").html(err);
                    $("#divError").show();
                    $.unblockUI();
                }
            });
        }

    });





});