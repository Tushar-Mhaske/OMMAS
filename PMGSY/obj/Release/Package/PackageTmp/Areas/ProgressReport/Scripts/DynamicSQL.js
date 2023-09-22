$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmTargetAchievement');

    $("#btnViewDynamicReport").click(function () {


        if ($("#frmTargetAchievement").valid()) {
            $.ajax({
                url: "/ProgressReport/Progress/DynamicReport/",
                cache: false,
                type: "POST",
                async: false,
                data: $("#frmTargetAchievement").serialize(),
                success: function (data) {
                    if (data.success == false)
                    {
                        alert(data.message);
                        
                        
                    }
                    else
                    {
                        $("#loadReport").html('');
                        $("#loadReport").html(data);

                        $("#dvhdCreateNewAgencyDetails").trigger("click");

                       
                    }
                },
                error: function () {
                    alert("error");
                }
            })
        }


    });


    $("#dvhdCreateNewAgencyDetails").click(function ()
    {
       // alert("here")

        if ($("#loadFilters").is(":visible"))
        {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#loadFilters").slideToggle(300);
        }
        else
        {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#loadFilters").slideToggle(300);
        }
    });

});