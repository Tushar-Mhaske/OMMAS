
$(document).ready(function () {

    if ($("#frmMasterGrowthScoreType") != null) {
        $.validator.unobtrusive.parse("#frmMasterGrowthScoreType");
    }


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    
    $('#btnSave').click(function (e) {

        if ($('#frmMasterGrowthScoreType').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/AddMasterGrowthScoreType/",
                type: "POST",

                data: $("#frmMasterGrowthScoreType").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $('#btnReset').trigger('click');
                        $("#dvGrowthScoreDetails").hide('slow');
                        $('#btnView').hide();
                        $('#btnAdd').show();
                        $('#tblMasterGrowthScoreList').trigger('reloadGrid');
                        $.unblockUI();

                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvGrowthScoreDetails").html(data);
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



    $('#btnUpdate').click(function (e) {
        
        if ($('#frmMasterGrowthScoreType').valid()) {
        
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/Master/EditGrowthScoreType/",
                type: "POST",

                data: $("#frmMasterGrowthScoreType").serialize(),
                success: function (data) {

                    if (data.success == true) {
                        alert(data.message);
                        $('#tblMasterGrowthScoreList').trigger('reloadGrid');
                        $("#dvGrowthScoreDetails").load("/Master/AddEditMasterGrowthScoreType");
                        $('#dvGrowthScoreDetails').hide('slow');
                        $('#btnView').hide();
                        $('#btnAdd').show();
                    }
                    else if (data.success == false) {

                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }

                    }
                    else {
                        $("#dvGrowthScoreDetails").html(data);
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

    $('#btnCancel').click(function (e) {

        $.ajax({
            url: "/Master/AddEditMasterGrowthScoreType",
            type: "GET",
            dataType: "html",
            success: function (data) {
                $("#dvGrowthScoreDetails").html(data);
                $('#dvGrowthScoreDetails').hide('slow');
                $('#btnView').hide();
                $('#btnAdd').show();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }
        });
    });

    $('#btnReset').click(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    $("#dvhdCreateNewScoreDetails").click(function () {

        if ($("#dvCreateNewScoreDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $("#dvCreateNewScoreDetails").hide('slow');
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $("#dvCreateNewScoreDetails").show('slow');
        }
    });

    $("#IMS_SC_FD_NAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#IMS_SC_FD_TYPE").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

});
function SearchDetails(sfType) {

    $('#tblMasterScourFoundationTypeList').setGridParam({
        url: '/Master/GetMasterScourFoundationTypeList/'
    });

    $('#tblMasterScourFoundationTypeList').jqGrid("setGridParam", { "postData": { SfTypeCode: sfType } });

    $('#tblMasterScourFoundationTypeList').trigger("reloadGrid", [{ page: 1 }]);

}


function EditGrowthScore() {

    if ($('#frmMasterGrowthScoreType').valid()) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/Master/EditMasterGrowthScoreType/",
            type: "POST",

            data: $("#frmMasterGrowthScoreType").serialize(),
            success: function (data) {
                if (data.success == true) {
                    
                    $('#dvGrowthScoreDetails').hide('slow');
                    $('#tblMasterGrowthScoreList').trigger('reloadGrid');

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
