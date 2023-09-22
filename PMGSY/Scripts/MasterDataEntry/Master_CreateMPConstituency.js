$(document).ready(function () {

    //this should be added on document ready
    $.validator.unobtrusive.parse($('#frmCreateMPConstituency'));

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#btnSave').click(function (e) {
        //e.preventDefault();


        if ($('#frmCreateMPConstituency').valid()) {

            var stateCode = $('#ddlStates option:selected').val();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/LocationMasterDataEntry/CreateMPConstituency",
                type: "POST",
               // dataType: "json",
                data: $("#frmCreateMPConstituency").serialize(),
                success: function (data) {

                    if (data.success==true) {
                        alert(data.message);
                        //ClearDetails();
                        if ($("#dvMapMPConstituencyBlockDetails").is(":visible")) {
                            $('#dvMapMPConstituencyBlockDetails').hide('slow');
                        }

                        if ($("#dvMappedMPConstituencyBlockDetails").is(":visible")) {
                            $('#dvMappedMPConstituencyBlockDetails').hide('slow');
                        }


                        $('#dvMPConstituencyDetails').hide('slow');

                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();
                        if (!$("#dvSearchMPConstituency").is(":visible")) {
                            $("#dvSearchMPConstituency").show('slow');
                        }

                        SearchDetails(stateCode);
                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }

                    }
                    else {
                        $("#dvMPConstituencyDetails").html(data);
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

      //  e.preventDefault();

        if ($('#frmCreateMPConstituency').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#ddlStates").attr("disabled", false);
            var stateCode = $('#ddlStates option:selected').val();

            $.ajax({
                url: "/LocationMasterDataEntry/EditMPConstituency",
                type: "POST",
               // dataType: "json",
                data: $("#frmCreateMPConstituency").serialize(),
                success: function (data) {


                    if (data.success==true) {
                        alert(data.message);
                       // SearchDetails();
                        //$('#tbMPConstituencyList').trigger("reloadGrid");
                        //$('#dvMPConstituencyDetails').load("/LocationMasterDataEntry/CreateMPConstituency");

                        if ($("#dvMapMPConstituencyBlockDetails").is(":visible")) {
                            $('#dvMapMPConstituencyBlockDetails').hide('slow');
                        }

                        if ($("#dvMappedMPConstituencyBlockDetails").is(":visible")) {
                            $('#dvMappedMPConstituencyBlockDetails').hide('slow');
                        }
                        $('#dvMPConstituencyDetails').hide('slow');

                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();
                        if (!$("#dvSearchMPConstituency").is(":visible")) {
                            $("#dvSearchMPConstituency").show('slow');
                        }

                        SearchDetails(stateCode);

                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $("#ddlStates").attr("disabled", true);
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvMPConstituencyDetails").html(data);
                        $("#ddlStates").attr("disabled", true);
                    }

                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {

                    $("#ddlStates").attr("disabled", true);
                    alert(xhr.responseText);
                    $.unblockUI();
                }

            });
        }
    });

    $('#btnCancel').click(function (e) {

        //$.ajax({
        //    url: "/LocationMasterDataEntry/CreateMPConstituency",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        //$("#mainDiv").html(data);
        //        $("#dvMPConstituencyDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }

        //});

        if ($("#dvMapMPConstituencyBlockDetails").is(":visible")) {
            $('#dvMapMPConstituencyBlockDetails').hide('slow');
        }

        if ($("#dvMappedMPConstituencyBlockDetails").is(":visible")) {
            $('#dvMappedMPConstituencyBlockDetails').hide('slow');
        }
        $('#dvMPConstituencyDetails').hide('slow');

        $('#btnSearchView').hide();
        $('#btnCreateNew').show();
        if (!$("#dvSearchMPConstituency").is(":visible")) {
            $("#dvSearchMPConstituency").show('slow');
        }
    });


    $('#btnReset').click(function () {
        ClearDetails();
    });


    //for expand and collpase Document Details 
    $("#dvhdCreateNewMPConstituencyDetails").click(function () {

        if ($("#dvCreateNewMPConstituencyDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvCreateNewMPConstituencyDetails").slideToggle(300);
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvCreateNewMPConstituencyDetails").slideToggle(300);
        }
    });

    $("#ddlStates").change(function () {

        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#message').html('');
        }

    });

    $('#MAST_MP_CONST_NAME').change(function () {

        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
});

function ClearDetails() {

    $('#MAST_MP_CONST_NAME').val('');
    $('#ddlStates').val('0');
    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}

function SearchDetails(stateCode) {
    $('#ddlSearchStates').val(stateCode);
    $('#tbMPConstituencyList').setGridParam({
        url: '/LocationMasterDataEntry/GetMPConstituencyDetailsList'
    });
   /* var data = $('#tbMPConstituencyList').jqGrid("getGridParam", "postData");
    data._search = false;
    data.searchField = "";*/
    // $('#tbDistrictList').jqGrid("setGridParam", { "postData": data });

    $('#tbMPConstituencyList').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlSearchStates option:selected').val() } });
    $('#tbMPConstituencyList').trigger("reloadGrid", [{ page: 1 }]);

}