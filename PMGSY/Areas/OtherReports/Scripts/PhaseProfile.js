
$(document).ready(function () {
    $.validator.unobtrusive.parse($('#phaseProfileForm'));

    $('#StateDPIU').change(function () {

        LoadAgency()
        LoadCollaboration();
    })

    $('#btnPhaseProfile').click(function () {
        console.log($("#phaseProfileForm").valid());
        //alert($("#phaseProfileForm").valid());
        if ($("#phaseProfileForm").valid())
        {
            LoadPhaseProfileReportCall();
        }
    })
})

function LoadAgency() {
    $("#AgencgyDPIU").empty();

    $.ajax({
        url: '/OtherReports/OtherReports/PopulateAgencies',
        type: 'POST',
        beforeSend: function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        },
        data: { stateCode: $("#StateDPIU").val(), value: Math.random() },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {

                $("#AgencgyDPIU").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }

            $.unblockUI();
        },
        error: function (err) {
            //alert("error " + err);
            $.unblockUI();
        }
    });
}

function LoadCollaboration() {

    $("#Collaboration").empty();
    $.ajax({
        url: '/OtherReports/OtherReports/PopulateCollaborations',
        type: 'POST',
        beforeSend: function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        },
        data: { stateCode: $("#StateDPIU").val(), value: Math.random() },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {

                $("#Collaboration").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }

            $.unblockUI();
        },
        error: function (err) {
            //alert("error " + err);
            $.unblockUI();
        }
    });
}

function LoadPhaseProfileReportCall() {
    $.ajax({
        url: '/OtherReports/OtherReports/LoadPhaseProfileListing/',
        type: 'POST',
        cache: false,
        data: $("#phaseProfileForm").serialize(),
        async: false,
        success: function (response) {
            $.unblockUI();
            $("#loadPhaseProfileReport").html(response);

        },
        error: function () {
            $.unblockUI();
            alert("An error occured while processing your request.");
            return false;
        },
    });
}
