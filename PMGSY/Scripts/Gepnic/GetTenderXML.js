$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmGepnicTenderDetailsLayout");

    //PopulateRefNoList();

    //$('#txtRefNo').change(function () {
    //    PopulateRefNoList();
    //});


    //LoadTenderList();



});


$("#btnView").click(function () {
    if ($("#frmGepnicTenderDetailsLayout").valid()) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/Gepnic/GetTenderXMLFromGepenic",
            type: "POST",
            data: $("#frmGepnicTenderDetailsLayout").serialize(),
            success: function (data)
            {
                if (data.success == true)
                {
                    //alert(data.message);
                    alert("File Generated Successfully on Desktop.")
                  //  ClearText();
                  //  LoadTenderList();
                    $.unblockUI();
                }
                else if (data.success == false)
                {
                    if (data.message != "")
                    {
                        alert("Error occured while processing your requset.")
                        //alert(data.message);
                        //LoadTenderList();
                        //$.unblockUI();
                    }
                }
                else {

                }
                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }


        });
    }
});

//ClearText(function () {
//    alert("ahskjfdgwkjhfdsg");
//});
