
$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmRegion");

    $("#MAST_REGION_NAME").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#btnSave').click(function (e) {
        if ($('#frmRegion').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            var stateCode = $('#ddlStateNames option:selected').val();
            $.ajax({
                url: "/Master/AddMasterRegion/",
                type: "POST",
                data: $("#frmRegion").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        //ClearDetails();
                        //$('#tblRegionList').jqGrid("setGridParam", { "postData": { stateCode: stateCode } });
                        //$('#tblRegionList').trigger('reloadGrid');

                        if ($("#dvMapRegionDistrictsDetails").is(":visible")) {
                            $('#dvMapRegionDistrictsDetails').hide('slow');
                        }

                        if ($("#dvMappedRegionDistrictsDetails").is(":visible")) {
                            $('#dvMappedRegionDistrictsDetails').hide('slow');
                        }

                        $('#dvRegionDetails').hide('slow');

                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();
                        if (!$("#dvSearchRegion").is(":visible")) {
                            $("#dvSearchRegion").show('slow');
                        }
                        SearchRegionCreateDetails(stateCode);
                    }
                    else if (data.success == false) {
                        if (data.message != "") {

                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvRegionDetails").html(data);
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
        if ($('#frmRegion').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $("#ddlStateNames").attr("disabled", false);
            var stateCode = $('#ddlStateNames option:selected').val();
            $.ajax({
                url: "/Master/EditMasterRegion/",
                type: "POST",
                data: $("#frmRegion").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        //$('#tblRegionList').trigger('reloadGrid');
                        //$("#dvRegionDetails").load("/Master/AddEditMasterRegion");
                        if ($("#dvMapRegionDistrictsDetails").is(":visible")) {
                            $('#dvMapRegionDistrictsDetails').hide('slow');
                        }

                        if ($("#dvMappedRegionDistrictsDetails").is(":visible")) {
                            $('#dvMappedRegionDistrictsDetails').hide('slow');
                        }

                        $('#dvRegionDetails').hide('slow');

                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();
                        if (!$("#dvSearchRegion").is(":visible")) {
                            $("#dvSearchRegion").show('slow');
                        }
                        SearchRegionCreateDetails(stateCode);
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $("#ddlStateNames").attr("disabled", true);
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvRegionDetails").html(data);
                        $("#ddlStateNames").attr("disabled", true);
                    }

                    $.unblockUI();

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $("#ddlStateNames").attr("disabled", true);
                    alert(xhr.responseText);
                    $.unblockUI();
                }

            });
        }
    });

    $('#btnCancel').click(function (e) {

        //$.ajax({
        //    url: "/Master/AddEditMasterRegion",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        $("#dvRegionDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }
        //});
        if ($("#dvMapRegionDistrictsDetails").is(":visible")) {
            $('#dvMapRegionDistrictsDetails').hide('slow');
        }

        if ($("#dvMappedRegionDistrictsDetails").is(":visible")) {
            $('#dvMappedRegionDistrictsDetails').hide('slow');
        }

        $('#dvRegionDetails').hide('slow');

        $('#btnSearchView').hide();
        $('#btnCreateNew').show();
        if (!$("#dvSearchRegion").is(":visible")) {
            $("#dvSearchRegion").show('slow');
        }
       
    });

    $('#btnReset').click(function () {
        ClearDetails();
    });


    //for expand and collpase Document Details 
    $("#dvhdCreateNewRegionDetails").click(function () {

        if ($("#dvCreateNewRegionDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");


            $(this).next("#dvCreateNewRegionDetails").slideToggle(300);
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvCreateNewRegionDetails").slideToggle(300);
        }
    });

    $("#ddlStateNames").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#MAST_REGION_NAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

});

function ClearDetails() {
    $('#ddlStateNames').val('0');
    $('#MAST_REGION_NAME').val('');

    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}

function SearchRegionCreateDetails(stateCode) {

    $('#ddlSearchStates').val(stateCode);

    $('#tblRegionList').setGridParam({
        url: '/Master/GetMasterRegionList/'
    });
    $('#tblRegionList').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlSearchStates option:selected').val() } });
    $('#tblRegionList').trigger("reloadGrid", [{ page: 1 }]);

}