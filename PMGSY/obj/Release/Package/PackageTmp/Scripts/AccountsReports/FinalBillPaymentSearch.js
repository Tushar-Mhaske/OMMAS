

//$(document).ready(function () {
//    //$(function () {
//    //    if ($("#LevelID").val() == 4) {
//    //        $("#ddlStateSRRDA").attr("disabled", true);
//    //        $("#ddlStateSRRDA").val($("#AdminNDCode").val());
//    //        $("#ddlStateSRRDA").trigger("change");
//    //    } 
//    //});

   
//    $("#btnView").click(function () {

//        if ($('#frmAbstractBankAuthDetails').valid()) {

//            if ($("#LevelID").val() == 4) {
//                $("#ddlStateSRRDA").attr("disabled", false);
//            }

//        $("#DisplayYear").val($("#ddlYear option:selected").text());
//        $('#DisplayState').val($('#ddlStateSRRDA option:selected').text());
//        $('#DisplayDPIU').val($('#ddlDPIU option:selected').text());

//        $.ajax({
//            url: '/AccountsReports/AbstractBankAuthDetails',
//            type: 'POST',
//            catche: false,
//            data:$('#frmAbstractBankAuthDetails').serialize(),
//            error: function (xhr, status, error) {                
//                alert("An error occured while processing your request.");
//                return false;
//            },
//            success: function (response) {
//                $("#dvAbstractBankAuthDetails").html(response);

//                if ($("#LevelID").val() == 4) {
//                    $("#ddlStateSRRDA").attr("disabled", true);
//                }
//            }
//        });
//    }
//    });
//});


$(document).ready(function () {

    $(function () {
        if ($("#LevelID").val() == 4) {
            $("#ddlState").attr("disabled", true);
            //$("#ddlStateSRRDA").val($("#AdminNDCode").val());
            $("#ddlState").trigger("change");
        } 
    });

    $.validator.unobtrusive.parse($('#frmFinalBillPaymentDetails'));
    
    //Populate Agency
    $("#ddlState").change(function () {
        $.blockUI({ message: '<h4><label style-"font-weight:normal">Loadding Agency...</label></h4>' });
        
        $.ajax({
            url: '/AccountsReports/PopulateAgency/' + $("#ddlState option:selected").val(),
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
                    $("#ddlAgency").append("<option value="+this.Value+">"+this.Text+"</option>");
                });
                $.unblockUI();
            }
        });
        $.unblockUI();

    });//End of populate Agency

    $("#btnView").click(function () {

        if ($('#frmFinalBillPaymentDetails').valid())
        {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            
            $("#DisplayStateName").val($("#ddlState option:selected").text());
            $('#DisplayAgencyName').val($('#ddlAgency option:selected').text());
            $('#DisplayFundingAgencyName').val($('#ddlFundingAgency option:selected').text());
           
            if ($("#LevelID").val() == 4) {
                $("#ddlState").attr("disabled", false);
            }

            //disable ddlState here

            $.ajax({
                url: '/AccountsReports/FinalBillPaymentDetails/',
                type: 'POST',
                catche: false,
                data: $("#frmFinalBillPaymentDetails").serialize(),
                error: function (xhr,status,error) {
                    $.unblockUI();
                    alert("An error occured while processing your request.");
                    if ($("#LevelID").val() == 4) {
                        $("#ddlState").attr("disabled", true);
                    }                               
                    return false;
                },
                success: function (response) {
                    $("#dvFinalBIllPaymentDetails").html(response);
                    if ($("#LevelID").val() == 4) {
                        $("#ddlState").attr("disabled", true);
                    }
                    $.unblockUI();
                }
            });
           // $.unblockUI();
        }
    });

});//end of document.ready()