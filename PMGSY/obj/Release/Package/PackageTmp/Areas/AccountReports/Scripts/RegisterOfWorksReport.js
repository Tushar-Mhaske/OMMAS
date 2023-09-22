$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmRegisterOfWorks");

    if ($("#LevelId").val() == 4) {

        $("#ddlSRRDA").val($("#AdminNdCode").val());
        $(function () {
            $("#ADMIN_ND_CODE").trigger("change");
        });

    }


    $("#ADMIN_ND_CODE").change(function () {

        $("#MAST_CON_ID").val(0);
        $("#MAST_CON_ID").empty();

        $("#TEND_AGREEMENT_CODE").val(0);
        $("#TEND_AGREEMENT_CODE").empty();
        $("#TEND_AGREEMENT_CODE").append("<option value='0'>Select Agreement</option>");

        if ($("#ADMIN_ND_CODE").val() == 0) {
            $("#MAST_CON_ID").append("<option value='0'>Select Contractor</option>");
        }


        if ($("#ADMIN_ND_CODE").val() > 0) {

            if ($("#MAST_CON_ID").length > 0) {

                $.ajax({
                    url: '/AccountReports/Account/PopulateContractorSupplier',
                    type: 'POST',
                    data: { id: $("#ADMIN_ND_CODE").val(), value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#MAST_CON_ID").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        }

    });//ADMIN_ND_CODE Change ends here


    $("#MAST_CON_ID").change(function () {

        $("#TEND_AGREEMENT_CODE").val(0);
        $("#TEND_AGREEMENT_CODE").empty();

        if ($("#MAST_CON_ID").val() == 0) {
            $("#TEND_AGREEMENT_CODE").append("<option value='0'>Select Agreement</option>");
        }

        if ($("#MAST_CON_ID").val() > 0) {

            if ($("#TEND_AGREEMENT_CODE").length > 0) {

                $.ajax({
                    url: '/AccountReports/Account/PopulateAgreement',
                    type: 'POST',
                    data: { id1: $("#MAST_CON_ID").val(), id2: $("#ADMIN_ND_CODE").val(), value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#TEND_AGREEMENT_CODE").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        }

    });//MAST_CON_ID Change ends here


    $("#btnView").click(function () {

        //if ($("#MAST_CON_ID").val() == 0) {
        //    return false;
        //}
        //if ($("#TEND_AGREEMENT_CODE").val() == 0) {
        //    return false;
        //}

        if ($("#frmRegisterOfWorks").valid()) {        
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        
        $.ajax({
            type: 'POST',
            url: '/AccountReports/Account/RegisterOfWorksReport/' + $("#ADMIN_ND_CODE").val() + "$" + $("#MAST_CON_ID").val() + "$" + $("#TEND_AGREEMENT_CODE").val() + "$" + $("#ddlSRRDA option:selected").text() ,
            data: { ContratorName: $("#MAST_CON_ID option:selected").text() },
            success: function (data) {
                $.unblockUI();
                $("#dvRegisterOfWorksPartial").html(data);
            },
            error: function (xhr, error, status) {
                alert("An Error occured while processig your request.");
                return false;
            }
        });

        }
    });//btnView Click ends here

    $("#ddlSRRDA").change(function () {

        $.blockUI({ message: '<h4><label style="font-weight:normal">loading DPIU...</label> ' });
        var val = $("#ddlSRRDA").val();
        $.ajax({
            type: 'POST',
            url: "/AccountReports/Account/PopulateDPIUOfSRRDA?id=" + val,
            async: false,
            success: function (data) {
                $.unblockUI();
                $("#ADMIN_ND_CODE").empty();
                $.each(data, function () {

                    if (this.Text == "All DPIU") {
                        $("#ADMIN_ND_CODE").append("<option value=" + "0" + ">" +
                                                                 "Select DPIU" + "</option>");
                    } else {
                        $("#ADMIN_ND_CODE").append("<option value=" + this.Value + ">" +
                                                                this.Text + "</option>");
                    }
                });

                $.unblockUI();
            }
        });
    });
});






