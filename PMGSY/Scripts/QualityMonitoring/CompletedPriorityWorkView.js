$('#ddlStates').change(function () {
    $("#ddlDistrict").empty();

    $.ajax({
        url: '/QualityMonitoring/PopulateAllDistrictsbyStateCode',
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

$('#btnListDetailsCQC').click(function () {
    $.ajax({
        url: '/QualityMonitoring/ViewCompletedPriorityWorkReport',
        type: 'POST',
        async: false,
        cache: false,
        beforeSend: function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        },
        data: { stateCode: $("#ddlStates").val(), districtCode: $("#ddlDistrict").val() },
        success: function (jsonData) {
            //for (var i = 0; i < jsonData.length; i++) {

            //    $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");

            //}
            //districtCode = $('#ddlDistrict option:selected').val();
            //$("#btnListDetails").trigger('click');

            $('#viewReport').html(jsonData);

            $.unblockUI();
        },
        error: function (err) {
            $.unblockUI();
        }
    });
});