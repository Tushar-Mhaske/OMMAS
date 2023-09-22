$(document).ready(function () {
    // $(':input[placeholder]').placeholder();

    


    $('#ddlState').change(function ()
    {
        $("#ddlDistrict").val(0);
        $("#ddlDistrict").empty();

        //if Block dropdown exists then only request to populate it.
        if ($("#ddlDistrict").length > 0) {
            $.ajax({
                url: '/CommonFilters/GetDistricts',
                type: 'POST',
                data: { selectedState: $("#ddlState").val(), value: Math.random() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                },
                error: function (err) {
                    alert("error " + err);
                }
            });
        }

    }); //ddlState Change ends here


}); //doc.ready() ends here






