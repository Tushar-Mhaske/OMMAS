$('#ddlStates').change(function () {
    $("#ddlDistrict").empty();

    $.ajax({
        url: '/MaintenanceAgreement/PopulateAllDistrictsbyStateCode',
        type: 'POST',
        async: false,
        cache: false,
        beforeSend: function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        },
        data: { stateCode: $("#ddlStates").val(), },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {

                $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");

            }
            districtCode = $('#ddlDistrict option:selected').val();
            $("#btnListDetails").trigger('click');
            //LoadExecutionGrid();

            $.unblockUI();
        },
        error: function (err) {
            $.unblockUI();
        }
    });

});




$('#btnWorkStatusReport').click(function () {

    if ($("#searchExecution").valid()) {
        $.ajax({
            url: '/MaintenanceAgreement/ViewWorkStatusReport',
            type: 'POST',
            async: false,
            cache: false,
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlStates").val(), districtCode: $("#ddlDistrict").val(), Scheme: $("#ddlScheme").val() },
            success: function (jsonData) {

                $('#viewReport').html(jsonData);

                $.unblockUI();
            },
            error: function (err) {
                $.unblockUI();
            }
        });
    }

});