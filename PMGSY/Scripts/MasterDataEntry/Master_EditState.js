$(document).ready(function () {

    $('#btnUpdate').click(function (e) {

        e.preventDefault();


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/LocationMasterDataEntry/Edit",
            type: "POST",
            dataType: "html",
            data: $("form").serialize(),
            success: function (data) {

                $("#mainDiv").html(data);
                $.unblockUI();
               
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }

        });
    });

    $('#btnCancel').click(function (e) {

        $.ajax({
            url: "/LocationMasterDataEntry/Index",
            type: "GET",
            dataType: "html",
            success: function (data) {
                $("#mainDiv").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }

        });
    });

});