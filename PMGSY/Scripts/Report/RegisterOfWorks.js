$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmRegisterOfWorks");

    if ($("#LevelId").val() == 4) {
     
        $("#ddlSRRDA").val($("#AdminNdCode").val());
        $(function () {
            $("#ADMIN_ND_CODE").trigger("change");
        });

    }


    $("#ADMIN_ND_CODE").change(function(){

        $("#MAST_CON_ID").val(0);
        $("#MAST_CON_ID").empty();

        $("#TEND_AGREEMENT_CODE").val(0);
        $("#TEND_AGREEMENT_CODE").empty();
        $("#TEND_AGREEMENT_CODE").append("<option value='0'>Select Agreement</option>");

        if ($("#ADMIN_ND_CODE").val() == 0)
        {
            $("#MAST_CON_ID").append("<option value='0'>Select Contractor</option>");
        }
        

        if ($("#ADMIN_ND_CODE").val() > 0) {

            if ($("#MAST_CON_ID").length > 0) {

                $.ajax({
                    url: '/Reports/PopulateContractorSupplier',
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
                    url: '/Reports/PopulateAgreement',
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
        //modified by abhishek kamble 4-dec-2013
            if ($("#MAST_CON_ID").val() == 0)
            {
                //alert("Please select Contractor");
                return false;
            }
            if ($("#TEND_AGREEMENT_CODE").val() == 0) {
                //alert("Please select Agreement");
                return false;
        }
        //alert($("#frmRegisterOfWorks").valid());

        //if ($("#frmRegisterOfWorks").valid()) {
            //blockPage();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
          //  alert("t");
            $("#dvRegisterOfWorksPartial").load('/Reports/RegisterOfWorksHeaderPartial/' + $("#ADMIN_ND_CODE").val() + "$" + $("#MAST_CON_ID").val() + "$" + $("#TEND_AGREEMENT_CODE").val(), function () {

                $("#dvWorkOfRegisterDetails").show();
                $.unblockUI();
            });
        //}
    });//btnView Click ends here

    $("#ddlSRRDA").change(function () {

        //alert("Change");

        $.blockUI({ message: '<h4><label style="font-weight:normal">loading DPIU...</label> ' });
        var val = $("#ddlSRRDA").val();
        $.ajax({
            type: 'POST',
            url: "/Reports/PopulateDPIUOfSRRDA?id=" + val,
            async: false,
            success: function (data) {
                $.unblockUI();
                $("#ADMIN_ND_CODE").empty();
                $.each(data, function () {
                    $("#ADMIN_ND_CODE").append("<option value=" + this.Value + ">" +
                                                            this.Text + "</option>");
                });

                $.unblockUI();
            }

        });


    });

    
});





    
