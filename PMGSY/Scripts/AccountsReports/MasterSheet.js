$(document).ready(function () {
    
    $('.Top2').bind('scroll', function () {
        $(".Top1").scrollLeft($(this).scrollLeft());

    });

    $(function () {

        $("#btnViewDetails").trigger("click");
    });

    //added by abhishek kamble 6-dec-2013
    $("#btnViewDetails").click(function () {

        if ($("#Year").val() > 0) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: '/AccountsReports/MasterSheetDetails',
                type: 'POST',
                data: { year: $("#Year").val(), value: Math.random() },
                success: function (response) {

                    //commented by abhishek kamble 18-nov-2013
                    //$("#divMasterSheet").html(response);

                    //added by abhishek kamble 18-nov-2013
                    $("#masterSheetDetails").html("");
                    $("#masterSheetDetails").html(response);
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


    //$("#Year").change(function () {
       
    //    if ($("#Year").val() > 0) {
           
    //            $.ajax({
    //                url: '/AccountsReports/MasterSheetDetails',
    //                type: 'POST',
    //                data: { year: $("#Year").val(), value: Math.random() },
    //                success: function (response) {
                        
    //                    //commented by abhishek kamble 18-nov-2013
    //                    //$("#divMasterSheet").html(response);
                        
    //                    //added by abhishek kamble 18-nov-2013
    //                    $("#masterSheetDetails").html("");
    //                    $("#masterSheetDetails").html(response);
 
    //                },
    //                error: function (err) {
    //                    $("#divError").html(err);
    //                    $("#divError").show();
    //                }
    //            });
    //    }
    //});

  
});