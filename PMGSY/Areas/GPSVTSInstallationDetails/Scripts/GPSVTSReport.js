$('#ddlState').change(function () {
    $("#ddlDistrict").empty();
    // alert("hi");

    $.ajax({
        url: '/GPSVTSDetails/PopulateDistrictsbyStateCode',
        type: 'POST',
        async: false,
        cache: false,
        beforeSend: function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        },
        data: { stateCode: $("#ddlState").val(), },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {

                $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");

            }
            districtCode = $('#ddlDistrict option:selected').val();
            $.unblockUI();
        },
        error: function (err) {
            $.unblockUI();
        }
    });

});



$('#ddlDistrict').change(function () {
    $("#ddlBlock").empty();
    // alert("hi");

    $.ajax({
        url: '/GPSVTSDetails/PopulateBlocksbyDistrictCode',
        type: 'POST',
        async: false,
        cache: false,
        beforeSend: function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        },
        data: { districtCode: $("#ddlDistrict").val(), },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {

                $("#ddlBlock").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");

            }
            BlockCode = $('#ddlBlock option:selected').val();
            $.unblockUI();
        },
        error: function (err) {
            $.unblockUI();
        }
    });

});



$("#btnGPSVTS").click(function () {
    debugger;
    if ($("#GPSVTSForm").valid()) {
        $.ajax({
            url: '/GPSVTSDetails/GetGPSVTSReport',
            cache: false,
            type: "POST",
            async: false,
            data: $("#GPSVTSForm").serialize(),
            success: function (data) {
                if (data.success == false) {
                    alert(data.message);
                }
                else {
                    $("#loadReport").html('');
                    $("#loadReport").html(data);
                }
            },
            error: function () {
                alert("error");
            }
        })
    }
});




