$(document).ready(function () {


    $("#ddlSPState").change(function () {
        $("#ddlSPAgency").empty();
       // $("#ddlSPCollaboration").empty();
       //$("#ddlSPDistrict").empty();
     


        $.ajax({
            url: '/StateProfileReports/StateProfileReports/PopulateAgencies',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlSPState").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Selected == true) {
                        $("#ddlSPAgency").append("<option value='" + jsonData[i].Value + "' selected=true>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlSPAgency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }

                $.unblockUI();
            },
            error: function (err) {
                //alert("error " + err);
                $.unblockUI();
            }
        });

    });


    $("#ddlSPAgency").change(function () {
        $("#ddlDPIU").empty();
        // $("#ddlSPCollaboration").empty();
        //$("#ddlSPDistrict").empty();



        $.ajax({
            url: '/StateProfileReports/StateProfileReports/PopulateDPIU',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: {
                stateCode: $("#ddlSPState").val(), 
                agencyCode: $("#ddlSPAgency").val(), value: Math.random()
                },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {

                    //alert();
                    if (jsonData[i].Selected == true) {
                        $("#ddlDPIU").append("<option value='" + jsonData[i].Value + "' selected=true>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlDPIU").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }

                $.unblockUI();
            },
            error: function (err) {
                //alert("error " + err);
                $.unblockUI();
            }
        });

    });









    $(function ()
    {
        $("#ddlSPState").trigger('change');
    });

    $.validator.unobtrusive.parse($('#frmstateprofile'));

    $("#btnViewSPDetails").click(function () {


        if ($("#ddlSPState").val() <= 0)
        {
            alert("Please Select State");
            return;
        }


        if ($('#frmstateprofile').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/StateProfileReports/StateProfileReports/StateProfilePhaseSummaryReport/' ,
    //        
                type: 'POST',
                catche: false,
                data: $("#frmstateprofile").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#divLoadSPDetails").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

 
});