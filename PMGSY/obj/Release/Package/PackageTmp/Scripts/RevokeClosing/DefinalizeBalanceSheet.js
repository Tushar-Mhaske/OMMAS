

$(document).ready(function () {

    $.validator.unobtrusive.parse($("#frmDeFinalizeBalanceSheet"));

    $("#btnDefinalizeBalSheet").click(function () {
        if ($("#frmDeFinalizeBalanceSheet").valid()) {

            if (confirm("Are You sure you want to Definalize balance sheet.")) {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                $.ajax({
                    url: "/RevokeClosing/DefinalizeBalanceSheet/",
                    type: "POST",
                    async: false,
                    cache: false,
                    data: $("#frmDeFinalizeBalanceSheet").serialize(),
                    success: function (data) {
                        $.unblockUI();
                        if (data.status) {
                            $("#ddlFundType").trigger("change");
                            alert(data.message);
                            //$("#mainDiv").load("/RevokeClosing/FinalizeBalanceSheet");
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

    $("#ddlFundType").change(function () {
                
        if ($("#ddlFundType").val() != 0) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: '/RevokeClosing/GetDefinalizeBalSheetYear',
                type: "POST",
                catche: false,
                data: { AdminNdCode: $("#ddlAgency").val(), FundType: $("#ddlFundType option:selected").val() },
                success: function (data) {
                    $.unblockUI();
                    if (data !== undefined || data != null) {
                        $("#ddlYear").empty();
                        $.each(data, function () {
                            $("#ddlYear").append("<option value='" + this.Value + "'selected='selected'>" + this.Text + "</option>");
                        });
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblockUI();
                    alert("Error while processing request");
                }
            });
        }
    });


    $("#ddlAgency").change(function () {
        $("#ddlFundType").val(0);
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