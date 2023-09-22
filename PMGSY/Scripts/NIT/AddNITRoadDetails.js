$(document).ready(function () {


    $('#aHeading').text('Add NIT Road Details');

    $("#dvViewNITRoadDetails").dialog({
        autoOpen: false,
        height: 'auto',
        width: '820',
        modal: true,
        title: 'Road Details'
    });


    $('#btnCreateNew_NITRoadDetails').click(function () {

        if (!$('#dvNewNITRoadDetails').is(':visible')) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            var encryptedTendNITCode = $('#EncryptedTendNITCode').val();

            $.ajax({
                url: "/NIT/NITRoadDetails/" + encryptedTendNITCode,
                type: "GET",
                dataType: "html",
                async: false,
                success: function (data) {
                    $("#dvNITRoadDetails").html(data);
                    $('#dvNewNITRoadDetails').show('slow');
                    $('#dvNITRoadDetails').show('slow');
                    $('#EncryptedTendNITCode').val(encryptedTendNITCode);
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }

            });

        }
    });
    
    $('#btnNITRoadDetailsCancel').click(function () {


        if ($("#accordion").is(":visible")) {
            $('#accordion').hide('slow');
        }

        //ViewSearchDiv();
        $('#tbNITList').jqGrid("setGridState", "visible");

        $("#dvNIT").animate({
            scrollTop: 0
        });

    });

    $('#imgCloseAgreementDetails').click(function () {


        if ($("#accordion").is(":visible")) {
            $('#accordion').hide('slow');
        }

       // ViewSearchDiv();
        $('#tbNITList').jqGrid("setGridState", "visible");

        $("#dvNIT").animate({
            scrollTop: 0
        });

    });



   
});