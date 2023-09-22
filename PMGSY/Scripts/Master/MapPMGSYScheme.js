$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmMapPMGSYScheme'));

    $('#btnMapScheme').click(function () {


        /*if (!(($("#chkPMGSY1").is(':checked')) || ($("#chkPMGSY2").is(':checked')) || ($("#chkRCPLWE").is(':checked')) || ($("#chkPMGSY3").is(':checked')) )) {
            alert('Please select a valid Scheme');
            return false;
        }
        if ($("#chkPMGSY1").is(':checked') && $("#chkPMGSY2").is(':checked') && $("#chkRCPLWE").is(':checked') &&  $("#chkPMGSY3").is(':checked')) {
            $('#MAST_PMGSY').val(14);

            console.log($('#MAST_PMGSY').val());
        }
        else if ($("#chkPMGSY1").is(':checked') && $("#chkPMGSY2").is(':checked') && $("#chkRCPLWE").is(':checked')) {
            $('#MAST_PMGSY').val(7);

            console.log($('#MAST_PMGSY').val());
        }
        else if ($("#chkPMGSY1").is(':checked') && $("#chkPMGSY2").is(':checked') && $("#chkPMGSY3").is(':checked')) {
            $('#MAST_PMGSY').val(12);

            console.log($('#MAST_PMGSY').val());
        }
        else if ($("#chkPMGSY1").is(':checked') && $("#chkRCPLWE").is(':checked') && $("#chkPMGSY3").is(':checked')) {
            $('#MAST_PMGSY').val(13);

            console.log($('#MAST_PMGSY').val());
        }
        else if ($("#chkPMGSY1").is(':checked') && $("#chkPMGSY2").is(':checked')) {
            $('#MAST_PMGSY').val(4);

            console.log($('#MAST_PMGSY').val());
        }
        else if ($("#chkPMGSY2").is(':checked') && $("#chkRCPLWE").is(':checked')) {
            $('#MAST_PMGSY').val(5);

            console.log($('#MAST_PMGSY').val());
        }
        else if ($("#chkPMGSY1").is(':checked') && $("#chkRCPLWE").is(':checked')) {
            $('#MAST_PMGSY').val(6);

            console.log($('#MAST_PMGSY').val());
        }
        else if ($("#chkPMGSY1").is(':checked') && $("#chkPMGSY3").is(':checked')) {
            $('#MAST_PMGSY').val(9);

            console.log($('#MAST_PMGSY').val());
        }
        else if ($("#chkPMGSY2").is(':checked') && $("#chkPMGSY3").is(':checked')) {
            $('#MAST_PMGSY').val(10);

            console.log($('#MAST_PMGSY').val());
        }
        else if ($("#chkRCPLWE").is(':checked') && $("#chkPMGSY3").is(':checked')) {
            $('#MAST_PMGSY').val(11);

            console.log($('#MAST_PMGSY').val());
        }
        else if ($("#chkPMGSY1").is(':checked')) {
            $('#MAST_PMGSY').val(1);

            console.log($('#MAST_PMGSY').val());
        }
        else if ($("#chkPMGSY2").is(':checked')) {
            $('#MAST_PMGSY').val(2);

            console.log($('#MAST_PMGSY').val());
        }
        else if ($("#chkRCPLWE").is(':checked')) {
            $('#MAST_PMGSY').val(3);

            console.log($('#MAST_PMGSY').val());
        }
        else if ($("#chkPMGSY3").is(':checked')) {
            $('#MAST_PMGSY').val(8);

            console.log($('#MAST_PMGSY').val());
        }*/

        //Added By Hrishikesh 26 - 07 - 2023 For Vibrant Village Mapp Scheme --start
        if (!(($("#chkPMGSY1").is(':checked')) || ($("#chkPMGSY2").is(':checked')) || ($("#chkRCPLWE").is(':checked')) || ($("#chkPMGSY3").is(':checked')) || ($("#chkVVP").is(':checked')))) {
            alert('Please select a valid Scheme');
            return false;
        }

        var concatVals = "";
        var schemeArr = ["#chkPMGSY1", "#chkPMGSY2", "#chkRCPLWE", "#chkPMGSY3", "#chkVVP"];

        for (var i = 0; i < schemeArr.length; i++) {
            if ($(schemeArr[i]).is(':checked')) {
                if (concatVals == "") {
                    concatVals = concatVals + $(schemeArr[i]).val();

                }
                else {
                    concatVals = concatVals + "," + $(schemeArr[i]).val();
                }
            }
        }

        $('#MAST_PMGSY').val(concatVals);
        console.log($('#MAST_PMGSY').val());
        //alert("Final val of-" + concatVals);
        //Added By Hrishikesh 26 - 07 - 2023 For Vibrant Village Mapp Scheme --end

        $.ajax({
            url: "/Master/UpdatePMGSYScheme/" + $('#EncryptedAgencyId').val(),
            type: "POST",
            dataType: "json",
            data: { MAST_PMGSY: $('#MAST_PMGSY').val(), __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() },
            success: function (data) {
                alert(data.message);
                setTimeout(function ()
                {
                    //$(this).closest('.ui-dialog-content').dialog('close');
                    $('.ui-icon-closethick').click();
                }, 1000);
                //jQuery("#tbMappedAgencyStateDistrictList").trigger('reloadgrid');
                //$(".ui-dialog").dialog("close");

                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
    });
});