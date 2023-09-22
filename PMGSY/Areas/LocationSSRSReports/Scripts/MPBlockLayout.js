$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmMPBlockLayout'));

    if ($("#Mast_State_Code").val() > 0) {

        $("#StateList_MPBlockDetails").attr("disabled", "disabled");
    }
    $("#StateList_MPBlockDetails").change(function () {
        $("#MPConstituencyList_MPBlockDetails").val(0);
        $("#MPConstituencyList_MPBlockDetails").empty();
        if ($(this).val() > 0) {
            if ($("#MPConstituencyList_MPBlockDetails").length > 0) {
                $.ajax({
                    url: '/LocationSSRSReports/LocationSSRSReports/MPBlockLayout',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_MPBlockDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#MPConstituencyList_MPBlockDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            $("#MPConstituencyList_MPBlockDetails").append("<option value='0'>All Constituencies</option>");
        }
    });
    //$("#StateList_MPBlockDetails").trigger('change');

    $("#MPBlockDetailsButton").click(function () {
        $('#StateName').val($("#StateList_MPBlockDetails option:selected").val());
        $('#MPConstituency').val($("#MPConstituencyList_MPBlockDetails option:selected").val());
        $('#ActiveFlagName').val($("#ActiveType_MPBlockDetails option:selected").val());
        
        $('#StateName').val($("#StateList_MPBlockDetails option:selected").text());
        $('#MPConstituencyName').val($("#MPConstituencyList_MPBlockDetails option:selected").text());
        $('#ActiveFlagName').val($("#ActiveType_MPBlockDetails option:selected").text());

        if ($('#frmMPBlockLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/MPBlockReport/',
                type: 'POST',
                catche: false,
                data: $("#frmMPBlockLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadMPBlockReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }

    });

    $("#MPBlockDetailsButton").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmMPBlockLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");

});
