$(document).ready(function () {


   // $("#rdoPdf").click(function () {
      

        $("#divPDF").show('slow');
    var roadCode = $("#RoadCode").val();
    var isFinalized = $("#isFinalized").val(); // Replace with the actual value of the other parameter

    var url = '/GPSVTSDetails/PdfFileUpload/?roadCode=' + roadCode + '&isFinalized=' + isFinalized;

   

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        //$("#divPDF").load('/GPSVTSDetails/PdfFileUpload/' + $("#RoadCode").val(), function () {
    $("#divPDF").load(url, function () {
            $.validator.unobtrusive.parse($('#fileupload'));
            $.unblockUI();
        });
        $.unblockUI();
    //});


  
});