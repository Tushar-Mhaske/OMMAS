$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmMLABlockLayout'));

    if ($("#Mast_State_Code").val() > 0) {

        $("#StateList_MLABlockDetails").attr("disabled", "disabled");
    }
    $("#StateList_MLABlockDetails").change(function () {
        $("#MPConstituencyList_MLABlockDetails").val(0);
        $("#MPConstituencyList_MLABlockDetails").empty();
        if ($(this).val() > 0) {
            if ($("#MPConstituencyList_MLABlockDetails").length > 0) {
                $.ajax({
                    url: '/LocationSSRSReports/LocationSSRSReports/MLABlockLayout',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_MLABlockDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#MLAConstituencyList_MLABlockDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            $("#MLAConstituencyList_MLABlockDetails").append("<option value='0'>All Constituencies</option>");
        }
    });
    //$("#StateList_MPBlockDetails").trigger('change');

    $("#MLABlockDetailsButton").click(function () {
        $('#StateName').val($("#StateList_MLABlockDetails option:selected").val());
        $('#MLAConstituency').val($("#MLAConstituencyList_MLABlockDetails option:selected").val());
        $('#ActiveFlagName').val($("#ActiveType_MLABlockDetails option:selected").val());

        $('#StateName').val($("#StateList_MLABlockDetails option:selected").text());
        $('#MLAConstituencyName').val($("#MLAConstituencyList_MLABlockDetails option:selected").text());
        $('#ActiveFlagName').val($("#ActiveType_MLABlockDetails option:selected").text());

        if ($('#frmMLABlockLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/MLABlockReport/',
                type: 'POST',
                catche: false,
                data: $("#frmMLABlockLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadMLABlockReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }

    });

    $("#MLABlockDetailsButton").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmMLABlockLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");

});
