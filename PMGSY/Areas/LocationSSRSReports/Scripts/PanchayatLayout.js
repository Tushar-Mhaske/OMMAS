$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmPanchayatLayout'));

    if ($("#Mast_State_Code").val() > 0) {

        $("#StateList_PanchayatDetails").attr("disabled", "disabled");
    }
    if ($("#Mast_District_Code").val() > 0) {
        //$("#DistrictList_PanchayatDetails").val($("#Mast_District_Code").val());
        $("#DistrictList_PanchayatDetails").attr("disabled", "disabled");
        //$("#DistrictList_PanchayatDetails").trigger('change');
    }
    $("#StateList_PanchayatDetails").change(function () {

        $("#DistrictList_PanchayatDetails").val(0);
        $("#DistrictList_PanchayatDetails").empty();
        $("#BlockList_PanchayatDetails").val(0);
        $("#BlockList_PanchayatDetails").empty();
        $("#BlockList_PanchayatDetails").append("<option value='0'>All Blocks</option>");
        //$("#DistrictList").append("<option value='0'>Select District</option>");

        if ($(this).val() > 0) {
            if ($("#DistrictList_PanchayatDetails").length > 0) {
                $.ajax({
                    url: '/LocationSSRSReports/LocationSSRSReports/PanchayatLayout',
                    type: 'POST',
                    data: { "StateCode": $(this).val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#DistrictList_PanchayatDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                        if ($("#Mast_District_Code").val() > 0) {
                            $("#DistrictList_PanchayatDetails").val($("#Mast_District_Code").val());
                            $("#DistrictList_PanchayatDetails").attr("disabled", "disabled");
                            $("#DistrictList_PanchayatDetails").trigger('change');
                        }

                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        }
        else {

            $("#DistrictList_PanchayatDetails").append("<option value='0'>Select District</option>");
            $("#BlockList_PanchayatDetails").empty();
            $("#BlockList_PanchayatDetails").append("<option value='0'>All Blocks</option>");

        }
    });

    $("#DistrictList_PanchayatDetails").change(function () {

        $("#BlockList_PanchayatDetails").val(0);
        $("#BlockList_PanchayatDetails").empty();

        //$("#BlockList").append("<option value='0'>Select Block</option>");

        if ($(this).val() > 0) {
            if ($("#BlockList_PanchayatDetails").length > 0) {
                $.ajax({
                    url: '/LocationSSRSReports/LocationSSRSReports/PanchayatLayout',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_PanchayatDetails").val(), "DistrictCode": $("#DistrictList_PanchayatDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#BlockList_PanchayatDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        } else {
            $("#BlockList_PanchayatDetails").append("<option value='0'>All Block</option>");
        }
    });

    $("#PanchayatDetailsButton").click(function () {
        
        $('#State').val($("#StateList_PanchayatDetails option:selected").val());
        $('#District').val($("#DistrictList_PanchayatDetails option:selected").val());
        $('#Block').val($("#BlockList_PanchayatDetails option:selected").val());
        $('#ActiveFlag').val($("#ActiveType_PanchayatDetails option:selected").val());

        $('#StateName').val($("#StateList_PanchayatDetails option:selected").text());
        $('#DistName').val($("#DistrictList_PanchayatDetails option:selected").text());
        $('#BlockName').val($("#BlockList_PanchayatDetails option:selected").text());
        $('#ActiveFlagName').val($("#ActiveType_PanchayatDetails option:selected").text());

        if ($('#frmPanchayatLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/PanchayatReport/',
                type: 'POST',
                catche: false,
                data: $("#frmPanchayatLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadPanchayatReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });


    //$("#StateList_PanchayatDetails").trigger('change');

    $("#PanchayatDetailsButton").trigger('click');


    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmPanchayatLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");

});
