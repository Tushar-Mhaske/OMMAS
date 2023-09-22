$(document).ready(function () {

    //this should be added on document ready
    $.validator.unobtrusive.parse($('#frmCreatePanchayat'));

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#btnSave').click(function (e) {
       
    
        if ($('#frmCreatePanchayat').valid()) {

        var stateCode = $('#ddlStates option:selected').val();
        //Added By Abhishek kamble 11-feb-2014
        var SelectedDistrictCode = $('#ddlDistricts option:selected').val();
        var selectedBlockCode = $('#ddlBlocks option:selected').val();


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/LocationMasterDataEntry/CreatePanchayat",
            type: "POST",
           // dataType: "json",
            data: $("#frmCreatePanchayat").serialize(),
            success: function (data) {

               // $("#mainDiv").html(data);
                if (data.success==true) {
                    alert(data.message);
                    ClearDetails();
                    if ($("#dvMapPanchayatHabitationsDetails").is(":visible")) {
                        $('#dvMapPanchayatHabitationsDetails').hide('slow');
                    }

                    if ($("#dvMappedPanchayatHabitationDetails").is(":visible")) {
                        $('#dvMappedPanchayatHabitationDetails').hide('slow');
                    }
                    $('#dvPanchayatDetails').hide('slow');
                    $('#btnSearchView').hide();
                    $('#btnCreateNew').show();
                    if (!$("#dvSearchPanchayat").is(":visible")) {
                        $("#dvSearchPanchayat").show('slow');
                    }
                    SearchDetails(stateCode,SelectedDistrictCode,selectedBlockCode);
                }
                else if (data.success==false) {
                    if (data.message != "") {
                        $('#message').html(data.message);
                        $('#dvErrorMessage').show('slow');
                    }

                }
                else {
                    $("#dvPanchayatDetails").html(data);
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

        
        if ($('#frmCreatePanchayat').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $("#ddlStates").attr("disabled", false);
            $("#ddlDistricts").attr("disabled", false);
            $("#ddlBlocks").attr("disabled", false);
            var stateCode = $('#ddlStates option:selected').val();
            //Added By Abhishek kamble 11-feb-2014
            var SelectedDistrictCode = $('#ddlDistricts option:selected').val();
            var selectedBlockCode = $('#ddlBlocks option:selected').val();
            $.ajax({
                url: "/LocationMasterDataEntry/EditPanchayat",
                type: "POST",
              //  dataType: "json",
                data: $("#frmCreatePanchayat").serialize(),
                success: function (data) {

                    //$("#mainDiv").html(data);

                    if (data.success==true) {
                        alert(data.message);
                      
                        //$('#tbPanchyatList').trigger('reloadGrid');
                        //$('#dvPanchayatDetails').load("/LocationMasterDataEntry/CreatePanchayat");
                        ClearDetails();
                        if ($("#dvMapPanchayatHabitationsDetails").is(":visible")) {
                            $('#dvMapPanchayatHabitationsDetails').hide('slow');
                        }

                        if ($("#dvMappedPanchayatHabitationDetails").is(":visible")) {
                            $('#dvMappedPanchayatHabitationDetails').hide('slow');
                        }
                        $('#dvPanchayatDetails').hide('slow');
                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();
                        if (!$("#dvSearchPanchayat").is(":visible")) {
                            $("#dvSearchPanchayat").show('slow');
                        }
                        SearchDetails(stateCode, SelectedDistrictCode, selectedBlockCode);

                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $("#ddlStates").attr("disabled", true);
                            $("#ddlDistricts").attr("disabled", true);
                            $("#ddlBlocks").attr("disabled", true);

                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvPanchayatDetails").html(data);
                        $("#ddlStates").attr("disabled", true);
                        $("#ddlDistricts").attr("disabled", true);
                        $("#ddlBlocks").attr("disabled", true);

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

        //$.ajax({
        //    url: "/LocationMasterDataEntry/CreatePanchayat",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        // $("#mainDiv").html(data);
        //        $('#dvPanchayatDetails').html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }

        //});

        if ($("#dvMapPanchayatHabitationsDetails").is(":visible")) {
            $('#dvMapPanchayatHabitationsDetails').hide('slow');
        }

        if ($("#dvMappedPanchayatHabitationDetails").is(":visible")) {
            $('#dvMappedPanchayatHabitationDetails').hide('slow');
        }
        $('#dvPanchayatDetails').hide('slow');
        $('#btnSearchView').hide();
        $('#btnCreateNew').show();
        if (!$("#dvSearchPanchayat").is(":visible")) {
            $("#dvSearchPanchayat").show('slow');
        }
    });


    $('#btnReset').click(function () {
        ClearDetails();
    });


    $("#ddlStates").change(function () {

        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#message').html('');
        }


        FillInCascadeDropdown({ userType: $("#ddlStates").find(":selected").val() },
                    "#ddlDistricts", "/LocationMasterDataEntry/GetDistrictsByStateCode?stateCode=" + $('#ddlStates option:selected').val());

        $('#ddlBlocks').empty();
        $('#ddlBlocks').append("<option value=0>--Select--</option>");

    }); //end function state change

    $("#ddlDistricts").change(function () {

        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#message').html('');
        }

        $('#ddlBlocks').empty();
        FillInCascadeDropdown({ userType: $("#ddlDistricts").find(":selected").val() },
                    "#ddlBlocks", "/LocationMasterDataEntry/GetBlocksByDistrictCode?districtCode=" + $('#ddlDistricts option:selected').val());

    }); //end function District change


    $('#MAST_PANCHAYAT_NAME').change(function () {

        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    //for expand and collpase  
    $("#dvhdCreateNewPanchayatDetails").click(function () {

        if ($("#dvCreateNewPanchayatDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvCreateNewPanchayatDetails").slideToggle(300);
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvCreateNewPanchayatDetails").slideToggle(300);
        }
    });

});

function ClearDetails() {

    $('#MAST_PANCHAYAT_NAME').val('');
   
    $('#ddlStates').val('0');

    if ($("#ddlDistricts option").length > 1) {

        FillInCascadeDropdown({ userType: $("#ddlStates").find(":selected").val() },
                      "#ddlDistricts", "/LocationMasterDataEntry/GetDistrictsByStateCode?stateCode=" + $('#ddlStates option:selected').val());

        //FillInCascadeDropdown({ userType: $("#ddlDistricts").find(":selected").val() },
        //           "#ddlBlocks", "/LocationMasterDataEntry/GetBlocksByDistrictCode?districtCode=" + $('#ddlDistricts option:selected').val());

    }

    $('#ddlBlocks').empty();
    $('#ddlBlocks').append("<option value=0>--Select--</option>");

    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}

function FillInCascadeDropdown(map, dropdown, action) {

    var message = '';

    if (dropdown == '#ddlDistricts') {
        message = '<h4><label style="font-weight:normal"> Loading Districts... </label></h4>';
    }
    else if (dropdown == '#ddlBlocks') {
        message = '<h4><label style="font-weight:normal"> Loading Blocks... </label></h4>';
    }

    $(dropdown).empty();
    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
} //end FillInCascadeDropdown()

function SearchDetails(stateCode, SelectedDistrictCode, selectedBlockCode) {
    $('#ddlSearchStates').val(stateCode);
    $('#ddlSearchStates').trigger('change');
    setTimeout(function () {
        $("#ddlSearchDistrict").val(SelectedDistrictCode);
    }, 1200);
    setTimeout(function () {
        $("#ddlSearchDistrict").trigger('change');
    }, 1500);
    setTimeout(function () {
        $("#ddlSearchBlocks").val(selectedBlockCode);
    }, 1800);
    setTimeout(function () {
        $('#tbPanchyatList').setGridParam({
            url: '/LocationMasterDataEntry/GetPanchayatDetailsList'
        });
        /*var data = $('#tbPanchyatList').jqGrid("getGridParam", "postData");
        data._search = false;
        data.searchField = "";*/
        //Modified by Abhishek kamble 11-Feb-2014
        // $('#tbPanchyatList').jqGrid("setGridParam", { "postData": { stateCode: stateCode, districtCode: "0", blockCode: "0" } });
        $('#tbPanchyatList').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlSearchStates').val(), districtCode: $("#ddlSearchDistrict").val(), blockCode: $("#ddlSearchBlocks").val() } });
        $('#tbPanchyatList').trigger("reloadGrid", [{ page: 1 }]);
    }, 2500);

}