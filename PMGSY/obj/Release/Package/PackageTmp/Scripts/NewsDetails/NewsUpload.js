$(document).ready(function () {
    $("#rdbtnFile").click(function () {
        //alert($("#hdnnewsId").val());
        blockPage();
        $("#dvNewsImageUpload").html('');
        $("#dvNewsFile").load('/NewsDetails/NewsPDFUpload?newsId=' + $("#hdnnewsId").val(), function () {
            //  $.validator.unobtrusive.parse($('#divFBDetailsForm'));
            $("#dvNewsFile").show();
            $("#dvNewsImageUpload").hide();
            unblockPage();
        });
    });

    $("#rdbtnImage").click(function () {
        blockPage();
        $("#dvNewsFile").html('');
        $("#dvNewsImageUpload").load('/NewsDetails/NewsImageUpload?newsId=' + $("#hdnnewsId").val(), function () {
            //  $.validator.unobtrusive.parse($('#divFBDetailsForm'));
            $("#dvNewsImageUpload").show();
            $("#dvNewsFile").hide();
            unblockPage();
        });
    });
});