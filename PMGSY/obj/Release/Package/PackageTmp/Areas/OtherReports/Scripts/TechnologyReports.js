
$(document).ready(function () {
    $.validator.unobtrusive.parse($('#RndTargetsForm'));
    $('#StateList').change(function () {

        LoadDistrict()
    })

    $('#DistrictList').change(function () {

        LoadBlock();
    })

    LoadRndtargetReportCall();

    $('#btnRndViewStateListRoad').click(function () {
        LoadRndtargetReportCall();
    })
})

function LoadDistrict() {
    $("#DistrictList").empty();

    $.ajax({
        url: '/OtherReports/OtherReports/PopulateDistrictList',
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

function LoadBlock(){
   
    $("#BlockList").empty();
        $.ajax({
            url: '/OtherReports/OtherReports/PopulateblockList',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { DistrictCode: $("#DistrictList").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    
                    $("#BlockList").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }

                $.unblockUI();
            },
            error: function (err) {
                //alert("error " + err);
                $.unblockUI();
            }
        });
}


function LoadRndtargetReportCall() {
    $.ajax({
        url: '/OtherReports/OtherReports/RndTargestAchievementListing/',
        type: 'POST',
        cache: false,
        data: $("#RndTargetsForm").serialize(),
        async: false,
        success: function (response) {
            $.unblockUI();
            $("#loadRndReport").html(response);

        },
        error: function () {
            $.unblockUI();
            alert("An error occured while processing your request.");
            return false;
        },
    });
}
