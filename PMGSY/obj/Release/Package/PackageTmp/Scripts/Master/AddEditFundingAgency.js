$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmAddFundingAgency');


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });



    $("#btnSave").click(function (e) {

        if ($("#frmAddFundingAgency").valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                type: 'POST',
                url: '/Master/AddFundingAgency/',
                async: false,
                data: $("#frmAddFundingAgency").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                        //ClearDetails();
                        //$('#fundCategory').trigger('reloadGrid');
                        $("#btnCreateNew").show('slow');
                        $('#fundDetails').hide('slow');
                        $('#fundCategory').trigger('reloadGrid');
                        $.unblockUI();
                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                            $.unblockUI();
                        }
                       
                    }
                    else {
                        $("#fundDetails").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            })
        }
    });

    $("#MAST_FUNDING_AGENCY_NAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });


    //$("#dvhdAddNewFundDetails").click(function () {

    //    if ($("#dvAddNewFundDetails").is(":visible")) {

    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

    //        $(this).next("#dvAddNewFundDetails").slideToggle(300);
    //    }

    //    else {
    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

    //        $(this).next("#dvAddNewFundDetails").slideToggle(300);
    //    }
    //});
    $("#spCollapseIconCN").click(function () {

        if ($("#fundDetails").is(":visible")) {
            $("#fundDetails").hide("slow");

            $("#btnCreateNew").show();
        }
    });

    $("#btnCancel").click(function (e) {

        //$.ajax({
        //    url: "/Master/AddFundingAgency",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
              
        //        $("#fundDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }

        //});
        $("#btnCreateNew").show('slow');
        $('#fundDetails').hide('slow');
       

    })

    $("#btnReset").click(function (e) {

        ClearDetails();
    });

    $("#btnUpdate").click(function (e) {
        if ($("#frmAddFundingAgency").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/EditFundingAgency/',
                async: false,
                data: $("#frmAddFundingAgency").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);           

                        //$("#fundDetails").load('/Master/AddFundingAgency');
                        //$('#fundCategory').trigger('reloadGrid');

                        $("#btnCreateNew").show('slow');
                        $('#fundDetails').hide('slow');
                        $('#fundCategory').trigger('reloadGrid');
                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#fundDetails").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            })
        }
    });
});

function ClearDetails() {
    $('#MAST_FUNDING_AGENCY_NAME').val('');
    $('#MAST_FUNDING_AGENCY_CODE').val('');

    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}