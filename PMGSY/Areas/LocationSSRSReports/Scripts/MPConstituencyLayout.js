$(function () {

    $.validator.unobtrusive.parse($('#frmMPConstituencyLayout'));

    if ($("#Mast_State_Code").val() > 0) {

        $("#StateList_MPConstituencyDetails").attr("disabled", "disabled");
    }
    $("#MPConstituencyDetailsButton").click(function () {

        $('#State').val($("#StateList_MPConstituencyDetails option:selected").val());
        $('#ActiveFlag').val($("#ActiveType_MPConstituencyDetails option:selected").val());

        $('#StateName').val($("#StateList_MPConstituencyDetails option:selected").text());
        $('#ActiveFlagName').val($("#ActiveType_MPConstituencyDetails option:selected").text());

        if ($('#frmMPConstituencyLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/MPConstituencyReport/',
                type: 'POST',
                catche: false,
                data: $("#frmMPConstituencyLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadMPConstituencyReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    $("#MPConstituencyDetailsButton").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmMPConstituencyLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");

});
