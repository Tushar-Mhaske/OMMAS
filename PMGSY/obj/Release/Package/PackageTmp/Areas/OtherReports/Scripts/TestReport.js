$(document).ready(function () {
    $.validator.unobtrusive.parse($('#TestForm'));
   TestReportCall();

   $("#StateForVillage").change(function () {
       $("#DistrictForVillage").empty();

       $.ajax({
           url: '/ECBriefReport/ECBriefReport/PopulateDistricts',
           type: 'POST',
           beforeSend: function () {
               $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
           },
           data: { stateCode: $("#StateForVillage").val(), value: Math.random() },
           success: function (jsonData) {
               for (var i = 0; i < jsonData.length; i++) {
                   if (jsonData[i].Value == 2) {
                       $("#DistrictForVillage").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                   }
                   else {
                       $("#DistrictForVillage").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                   }
               }

               $.unblockUI();
           },
           error: function (err) {
               //alert("error " + err);
               $.unblockUI();
           }
       });
   });


   $("#DistrictForVillage").change(function () {
       $("#BlockForVillage").empty();

       $.ajax({
           url: '/ECBriefReport/ECBriefReport/PopulateBlocks',
           type: 'POST',
           beforeSend: function () {
               $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
           },
           data: { DistrictCode: $("#DistrictForVillage").val(), value: Math.random() },
           success: function (jsonData) {
               for (var i = 0; i < jsonData.length; i++) {
                   if (jsonData[i].Value == 2) {
                       $("#BlockForVillage").append("<option value='" + jsonData[i].Value + "'selected>" + jsonData[i].Text + "</option>");
                   }
                   else {
                       $("#BlockForVillage").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                   }
               }

               $.unblockUI();
           },
           error: function (err) {
               //alert("error " + err);
               $.unblockUI();
           }
       });
   });

    $('#TestDetaisButton').click(function () {
       
        TestReportCall();
    });
});

function TestReportCall() {
    if ($('#TestForm').valid()) {
        $.blockUI({ message: null });
        $.ajax({
            url: '/OtherReports/OtherReports/TestReportListing/',
            type: 'POST',
            catche: false,
            data: $("#TestForm").serialize(),
            async: false,
            success: function (response) {
                $.unblockUI();
                $("#TestReportDiv").html(response);

            },
            error: function () {
                $.unblockUI();
                alert("An Error");
                return false;
            },
        });
    }
}