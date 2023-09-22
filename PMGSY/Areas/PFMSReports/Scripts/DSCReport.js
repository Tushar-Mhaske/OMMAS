
$(document).ready(function () {
    $.validator.unobtrusive.parse($('#DscRegForm'));
    $('#StateList').change(function () {

        LoadDistrict()
    })
    

    $('#btnViewDscReport').click(function () {
        LoadDSCReport();
    })
})

function LoadDistrict() {
    $("#DistrictList").empty();

    $.ajax({
        url: '/PFMSReports/PFMSReports/PopulateDistrictList',
        type: 'POST',
        beforeSend: function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        },
        data: { stateCode: $("#StateList").val(), value: Math.random() },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
               
                $("#DistrictList").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }

            $.unblockUI();
        },
        error: function (err) {
            //alert("error " + err);
            $.unblockUI();
        }
    });
}
 

function LoadDSCReport() {
    $("#StateName").val($("#StateList option:selected").text());
    $("#DistrictName").val($("#DistrictList option:selected").text());
    $.ajax({
        url: '/PFMSReports/PFMSReports/DscReport/',
        type: 'POST',
        cache: false,
        data: $("#DscRegForm").serialize(),
        async: false,
        success: function (response) {
            $.unblockUI();
            $("#loadDscReport").html('');
            $("#loadDscReport").html(response);
        },
        error: function () {
            $.unblockUI();
            alert("An error occured while processing your request.");
            return false;
        },
    });
}
