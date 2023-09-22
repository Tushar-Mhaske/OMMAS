$(document).ready(function () {
    $.validator.unobtrusive.parse("#frmUC");

    $.unblockUI();

    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });

    $("#btnView").click(function () {

        $("#loadReport").html("");

        if ($("#frmUC").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });

            //$("#AgencyName").val($("#ddlAgency option:selected").text());

          //  $("#ddlState").attr('disabled', false);

            //Ajax Call
            $.ajax({
                url: "/AccountReports/Account/UCReport/",
                type: "POST",
                data: $("#frmUC").serialize(),
                success: function (data) {
                    $.unblockUI();
                    $("#loadReport").html(data);                    

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //if ($("#LevelIdStateAccount").val() != 1) {
                    //    $("#ddlState").attr('disabled', true);
                    //}
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            });
        }
        //    }


    });


    ////Populate Agency
    $("#ddlState").change(function () {



        $.blockUI({ message: '<h4><label style-"font-weight:normal">Loadding Agency...</label></h4>' });

        $.ajax({
            url: '/Account/PopulateAgency/' + $("#ddlState option:selected").val(),
            type: 'POST',
            catche: false,
            async: false,
            error: function (xhr, status, error) {
                $.unblockUI();
                alert("An error occured while proccessing your request.");
                return false;
            },
            success: function (response) {
                $("#ddlAgency").empty();
                $.each(response, function () {
                    $("#ddlAgency").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
                $.unblockUI();
            }
        });
        $.unblockUI();

    });//End of populate Agency



    ////Populate Financial Year
    $("#ddlAgency").change(function () {



        $.blockUI({ message: '<h4><label style-"font-weight:normal">Loadding Financial Years...</label></h4>' });

        $.ajax({
            url: '/Account/PopulateFinancialYearAccount/',
            type: 'POST',
            data:{state: $("#ddlState option:selected").val(),AdminNDCode:$("#ddlAgency option:selected").val()},
            catche: false,
            async: false,
            error: function (xhr, status, error) {
                $.unblockUI();
                alert("An error occured while proccessing your request.");
                return false;
            },
            success: function (response) {
                $("#ddlYear").empty();
                $.each(response, function () {
                    $("#ddlYear").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
                $.unblockUI();
            }
        });
        $.unblockUI();

    });//End of populate Agency


});