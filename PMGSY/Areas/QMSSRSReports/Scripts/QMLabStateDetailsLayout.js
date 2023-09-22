$(document).ready(function () {
    $("#dvQMLabStateDetailsLayout").show();
    $("#frmLabStateDetailsLayout").show();

    $.validator.unobtrusive.parse($('#frmLabStateDetailsLayout'));
    //alert("hy");
   //  $("#btnLabStateDetails").click(function () {


   
    $("#ddlStateLabStateDetails").change(function () {
        $("#ddlAgencyMPR1").empty();
        $.ajax({
            url: '/QMSSRSReports/QMSSRSReports/PopulateAgencies',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlStateLabStateDetails").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Selected == true) {
                        $("#ddlAgencyMPR1").append("<option value='" + jsonData[i].Value + "' selected=true>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlAgencyMPR1").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
//changes ....sachin
        $("#btnLabStateDetails").click(function () {
            $("#StateName").val($("#ddlStateLabStateDetails option:selected").text());
          

            if ($('#frmLabStateDetailsLayout').valid()) {
             
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
                $.ajax({
                    url: '/QMSSRSReports/QMSSRSReports/LabStateDetailsReport/',
                    type: 'POST',
                    catche: false,
                    data: $("#frmLabStateDetailsLayout").serialize(),
                    async: false,
                    success: function (response) {
                        $.unblockUI();
                        $("#dvLoadLabStateDetailsReport").html(response);
                    },
                    error: function () {
                        $.unblockUI();
                        alert("Error ocurred");
                        return false;
                    },
                });
            }
        });
    //
            // $("#btnLabStateDetails").trigger('click');

            //$("#spCollapseIconCN").click(function () {

            //    $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
            //    $("#dvLoadLabStateDetailsReport").toggle("slow");

            //} );

            //this function call  on layout.js
            //closableNoteDiv("divCommonReport", "spnCommonReport");
        });
    