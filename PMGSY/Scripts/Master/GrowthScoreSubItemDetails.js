
$(document).ready(function () {

    if ($("#frmScoreSubItemDetails") != null) {
        $.validator.unobtrusive.parse("#frmScoreSubItemDetails");
    }


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });


    $('#btnSaveSubItem').click(function (e) {

        if ($('#frmScoreSubItemDetails').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/AddGrowthScoreSubItem/",
                type: "POST",

                data: $("#frmScoreSubItemDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $('#btnReseSubItemt').trigger('click');
                        $("#dvGrowthScoreSubItemDetails").hide('slow');
                        $('#btnViewSubItem').hide();
                        $('#btnAddSubItem').show();
                        $('#tblGrowthScoreSubItemList').trigger('reloadGrid');

                        $.unblockUI();

                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#messageSubItem').html(data.message);
                            $('#dvErrorMessageSubItem').show('slow');
                        }
                    }
                    else {
                        $("#dvGrowthScoreSubItemDetails").html(data);
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



    $('#btnUpdateSubItem').click(function (e) {

        if ($('#frmScoreSubItemDetails').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/Master/EditGrowthScoreSubItemDetails/",
                type: "POST",

                data: $("#frmScoreSubItemDetails").serialize(),
                success: function (data) {

                    if (data.success == true) {
                        alert(data.message);
                        $('#tblGrowthScoreSubItemList').trigger('reloadGrid');
                        $("#dvGrowthScoreSubItemDetails").load("/Master/AddEditMasterGrowthScoreType");
                        $('#dvGrowthScoreSubItemDetails').hide('slow');
                        $('#btnViewSubItem').hide();
                        $('#btnAddSubItem').show();
                    }
                    else if (data.success == false) {

                        if (data.message != "") {
                            $('#messageSubItem').html(data.message);
                            $('#dvErrorMessageSubItem').show('slow');
                        }

                    }
                    else {
                        $("#dvGrowthScoreSubItemDetails").html(data);
                        //$('#IMS_SC_FD_TYPE').attr('disabled', true);
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

    $('#btnCancelSubItem').click(function (e) {

        $('#dvGrowthScoreSubItemDetails').hide('slow');
        $('#btnViewSubItem').hide();
        $('#btnAddSubItem').show();

        //$.ajax({
        //    url: "/Master/GrowthScoreSubItemDetails?scoreId=" + parseInt($('#hdnScoreCode').val()),
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        $("#dvGrowthScoreSubItemDetails").html(data);
        //        $('#dvGrowthScoreSubItemDetails').hide('slow');
        //        $('#btnViewSubItem').hide();
        //        $('#btnAddSubItem').show();
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }
        //});
    });

    $('#btnResetSubItem').click(function () {
        $('#dvErrorMessageSubItem').hide('slow');
        $('#messageSubItem').html('');
    });
    $("#dvhdScoreSubItemDetails").click(function () {

        if ($("#dvScoreSubItemDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $("#dvScoreSubItemDetails").hide('slow');
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $("#dvScoreSubItemDetails").show('slow');
        }
    });

});

function EditGrowthScore() {
    if ($('#frmScoreSubItemDetails').valid()) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/Master/EditMasterGrowthScoreType/",
            type: "POST",

            data: $("#frmScoreSubItemDetails").serialize(),
            success: function (data) {
                if (data.success == true) {

                    $('#dvGrowthScoreSubItemDetails').hide('slow');
                    $('#tblGrowthScoreSubItemList').trigger('reloadGrid');

                    $.unblockUI();

                }
                else if (data.success == false) {
                    if (data.message != "") {
                        $('#message').html(data.message);
                        $('#dvErrorMessage').show('slow');
                    }
                }
                else {
                    //$("#dvGrowthScoreDetails").html(data);
                }
                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }

        });

    }
}
