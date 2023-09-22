$(function () {

    $.validator.unobtrusive.parse($('#frmPanchayatLayout'));

    if ($("#Mast_State_Code").val() > 0) {

        $("#StateList_MLAConstituencyDetails").attr("disabled", "disabled");
    }
    $("#MLAConstituencyDetailsButton").click(function () {

        $('#State').val($("#StateList_MLAConstituencyDetails option:selected").val());
        $('#ActiveFlag').val($("#ActiveType_MLAConstituencyDetails option:selected").val());

        $('#StateName').val($("#StateList_MLAConstituencyDetails option:selected").text());
        $('#ActiveFlagName').val($("#ActiveType_MLAConstituencyDetails option:selected").text());

        if ($('#frmMLAConstituencyLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/MLAConstituencyReport/',
                type: 'POST',
                catche: false,
                data: $("#frmMLAConstituencyLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadMLAConstituencyReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    $("#MLAConstituencyDetailsButton").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmMLAConstituencyLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
    
});
