
$(document).ready(function () {
    $.validator.unobtrusive.parse("#frmAbstractFund");
    if ($("#LevelId").val() == 4) {      
        $(function () {
            $("#ddlSRRDA").trigger("change");
        });
    }


    $("#btnViewDetails").click(function () {

        $("#DPIUName").val($("#ddlDPIU option:selected").text());
        $("#HeadName").val($("#ddlFund option:selected").text());
        //$("#MonthName").val($("#ddlMonth option:selected").text());
        $("#StateName").val($("#ddlSRRDA option:selected").text());
        $("#YearName").val($("#ddlYear option:selected").text());
      
      //   $("#ddlSRRDA").attr("disabled", false);
        
        //  if (isValid() == true) {
         if ($("#frmAbstractFund").valid())
         {
             $("#ddlSRRDA").attr("disabled", false);
            $.ajax({
                url: "/AccountsReports/AbstractFundDetails/",
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmAbstractFund").serialize(),
                success: function (data) {
                    if (data.success == false) {
                        $("#dvError").show();
                        $("#dvError").html(data);
                        if ($("#LevelId").val() == 4) {
                            $("#ddlSRRDA").attr("disabled", true);
                        }
                        return false;
                    }
                    else {
                        $("#AbstractFundDetails").html(data);
                        if ($("#LevelId").val() == 4) {
                            $("#ddlSRRDA").attr("disabled", true);
                        }
                    }
                },
                error: function (data) {
                    $("#ddlState").attr("disabled", false);
                }
            });
        }       

    });


    $("#ddlSRRDA").change(function () {

        //alert("Change");
       
            $.blockUI({ message: '<h4><label style="font-weight:normal">loading DPIU...</label> ' });
            var val = $("#ddlSRRDA").val() +"$"+"A";
            $.ajax({
                type: 'POST',
                url: "/AccountsReports/PopulateDPIU?id=" + val,
                async: false,
                success: function (data) {
                    $.unblockUI();
                    $("#ddlDPIU").empty();
                    $.each(data, function () {
                        $("#ddlDPIU").append("<option value=" + this.Value + ">" +
                                                                this.Text + "</option>");
                    });

                    $.unblockUI();
                }

            });
       

    });
});

function isValid() {

    if ($("#ddlYear").val() == 0) {
        alert("Please select Year");
        return false;
    }
    else if ($("#ddlFund").val() == 0) {
        alert("Please select Head");
        return false;
    }
    else if ($("#ddlSRRDA").val() == 0) {
        alert("Please select State");
        return false;
    }
    //else if ($("#ddlDPIU").val() == 0) {
    //    alert("Please select DPIU");
    //    return false;
    //}
    return true;

}