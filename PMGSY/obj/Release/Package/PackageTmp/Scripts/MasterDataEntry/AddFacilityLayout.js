$(document).ready(function () {

    //this should be added on document ready
    $.validator.unobtrusive.parse($('#frmCreatePanchayat'));

    //("input[type=text]").bind("keypress", function (e) {
    //    if (e.keyCode == 13) {
    //        return false;
    //    }
    //});

    $("#tbPincode").val("");

    $('#tbAddress').css('resize', 'none');

    $("#trFirstRow").removeClass("ui-state-hover");


    //Method to populate habitations according to blocks
    $("#ddlBlocks").change(function () {
        var BlockCode = $('#ddlBlocks option:selected').val();
        //console.log(BlockCode);
        $.ajax({
            url: '/LocationMasterDataEntry/GetHabitationBlockCode/',
            data: { blockCode: BlockCode },
            type: 'POST',
            catche: false,
            error: function (xhr, status, error) {
                alert('An Error occured while processig your request.')
                return false;
            },
            success: function (data) {
                $('#ddlHabitation').empty();
                $.each(data, function () {
                    $('#ddlHabitation').append("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            }
        });
        checkLockStatus();
     });
    console.log($('#DistrictCode').val());

    $('#ddlFacility').change(function () {
        LoadFacilityName();
    });

    // Not needed Avinash
    $.ajax({
        url: "/LocationMasterDataEntry/GetBlocksByDistrictCode?districtCode=" + $('#DistrictCode').val(),
        //data: { districtCode: $('#DistrictCode option:selected').val() },
        type: 'POST',
        catche: false,
        error: function (xhr, status, error) {
            alert('An Error occured while processig your request.')
            return false;
        },
        success: function (data) {
            //$('#ddlBlocks').empty();
            //$.each(data, function () {
            //    $('#ddlBlocks').append("<option value=" + this.Value + ">" + this.Text + "</option>");
            //});
        }
    });

  $('#btnSave').click(function (e) {
      //$("#ddlFacility option:selected").val($("#facilityCode").val());
      //$("#ddlFacilityNameList option:selected").val($("#FacilityName").val());
      $("#facilityCode").val($("#ddlFacility option:selected").val());
      $("#FacilityName").val($("#ddlFacilityNameList option:selected").val());
    if ($('#frmCreatePanchayat').valid()) {
    $.validator.unobtrusive.parse($("#frmCreatePanchayat"));
    var SelectedDistrictCode = $('#ddlDistricts option:selected').val();
    var selectedBlockCode = $('#ddlBlocks option:selected').val();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/LocationMasterDataEntry/SaveFacilityDetails",
                type: "POST",
                // dataType: "json",
                data: $("#frmCreatePanchayat").serialize(),
                success: function (data) {

                    // $("#mainDiv").html(data);
                    if (data.success == true) {
                        $('#btnReset').trigger("click");
                        $("#tbPincode").val("");
                        alert(data.message);
                        ClearDetails();
                        LoadBlock();
                        //$('#tbPanchyatList').trigger('reloadGrid', { fromServer: true, page: 1 });
                        //$('#tbPanchyatList').setGridParam({ datatype: 'json', page: 1 }).trigger('reloadGrid');
                        if ($("#dvMapPanchayatHabitationsDetails").is(":visible")) {
                            $('#dvMapPanchayatHabitationsDetails').hide('slow');
                        }

                        if ($("#dvMappedPanchayatHabitationDetails").is(":visible")) {
                            $('#dvMappedPanchayatHabitationDetails').hide('slow');
                        }
                        $('#dvPanchayatDetails').hide('slow');
                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();

                        //if (!$("#dvSearchPanchayat").is(":visible")) {
                        //    $("#dvSearchPanchayat").hide();

                        //}
                        SearchDetails();
                        $.ajax({
                            url: "/LocationMasterDataEntry/AddFacilityLayout",
                            type: "GET",
                            success: function (data) {
                                $("#dvPanchayatDetails").html(data);
                                $('#dvSearchPanchayat').hide();
                                $("#dvPanchayatDetails").show()
                            }
                      });

                   }
                    else if (data.success == false) {
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
                    alert("An Errror Occurred, Details Could not be saved");
                    $.unblockUI();
                }

            });

        }

    });

    $('#btnUpdate').click(function (e) {
        if ($('#frmCreatePanchayat').valid()) {
        //    $('#ddlHabitation option:selected').val($("#HabCodeEdit").val());
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                $("#ddlStates").attr("disabled", false);
                $("#ddlDistricts").attr("disabled", false);
                $("#ddlBlocks").attr("disabled", false);
                var stateCode = $('#ddlStates option:selected').val();
                //$("#ddlFacilityElse option:selected").val($("#facilityCode").val());
            //$("#ddlFacilityNameListElse option:selected").val($("#FacilityName").val());

                $("#facilityCode").val($('#ddlFacility option:selected').val());
                $("#FacilityName").val($('#ddlFacilityNameList option:selected').val());


                var SelectedDistrictCode = $('#ddlDistricts option:selected').val();
                var selectedBlockCode = $('#ddlBlocks option:selected').val();

         //   alert("Final")
            
                $.ajax({
                    url: "/LocationMasterDataEntry/EditFacilityPost/" + $('#FacilityCodeToEdit').val(),
                    type: "POST",
                    //  dataType: "json",
                    data: $("#frmCreatePanchayat").serialize(),
                    success: function (data) {
                    if (data.success == true) {
                            alert(data.message);
                            $('#btnReset').trigger("click");
                            $("#tbPincode").val("");
                            ClearDetails();
                            LoadBlock();

                            if ($("#dvMapPanchayatHabitationsDetails").is(":visible")) {
                                $('#dvMapPanchayatHabitationsDetails').hide('slow');
                            }

                            if ($("#dvMappedPanchayatHabitationDetails").is(":visible")) {
                                $('#dvMappedPanchayatHabitationDetails').hide('slow');
                            }
                            $('#dvPanchayatDetails').hide('slow');

                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();
                       
                            SearchDetails();
                            $.ajax({
                                url: "/LocationMasterDataEntry/AddFacilityLayout",
                                type: "GET",
                                success: function (data) {
                                    $("#dvPanchayatDetails").html(data);
                                    $('#dvPanchayatDetails').show('slow');
                                }
                            });
          
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
                        alert("An Error Occurred while processing Request");
                        $.unblockUI();
                    }

                });
            }
  });

    $('#btnCancel').click(function (e) {


        if ($("#dvMapPanchayatHabitationsDetails").is(":visible")) {
            $('#dvMapPanchayatHabitationsDetails').hide('slow');
        }

        if ($("#dvMappedPanchayatHabitationDetails").is(":visible")) {
            $('#dvMappedPanchayatHabitationDetails').hide('slow');
        }
        $('#dvPanchayatDetails').hide('slow');
        $('#btnSearchView').hide();
        $('#btnCreateNew').show();
        $('#btnReset').trigger("click");
        $("#tbPincode").val("");
        //if (!$("#dvSearchPanchayat").is(":visible")) {
        //    $("#dvSearchPanchayat").show('slow');
        //}
        $.ajax({
            url: "/LocationMasterDataEntry/AddFacilityLayout",
            type: "GET",
            success: function (data) {
                $("#dvPanchayatDetails").html(data);
                $('#dvPanchayatDetails').show('slow');
            }

        });
    });


    $('#btnReset').click(function () {
      ClearDetails();
    });

  $("#ddlDistricts").change(function () {

        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#message').html('');
        }
        $('#ddlBlocks').empty();
        FillInCascadeDropdown({ userType: $("#ddlDistricts").find(":selected").val() },
                               "#ddlBlocks", "/LocationMasterDataEntry/GetBlocksByDistrictCode?districtCode=" + $('#ddlDistricts option:selected').val());

    }); //end function District change

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
    $("#tbPincode").val("");
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

function SearchDetails() {
    //$('#ddlSearchStates').val(stateCode);
    //$('#ddlSearchStates').trigger('change');
    //setTimeout(function () {
    //    $("#ddlSearchDistrict").val(SelectedDistrictCode);
    //}, 1200);
    //setTimeout(function () {
    //    $("#ddlSearchDistrict").trigger('change');
    //}, 1500);
    //setTimeout(function () {
    //    $("#ddlSearchBlocks").val(selectedBlockCode);
    //}, 1800);
    setTimeout(function () {
        $('#tbPanchyatList').setGridParam({
            url: '/LocationMasterDataEntry/GetFacilityDetailsList'
        });
        /*var data = $('#tbPanchyatList').jqGrid("getGridParam", "postData");
        data._search = false;
        data.searchField = "";*/
        //Modified by Abhishek kamble 11-Feb-2014
        // $('#tbPanchyatList').jqGrid("setGridParam", { "postData": { stateCode: stateCode, districtCode: "0", blockCode: "0" } });
        //$('#tbPanchyatList').jqGrid("setGridParam");
        $('#tbPanchyatList').trigger("reloadGrid", [{ page: 1 }]);
    }, 1000);

}
function LoadBlock() {
    $.ajax({
        url: "/LocationMasterDataEntry/GetBlocksByDistrictCode?districtCode=" + $('#DistrictCode').val(),
        //data: { districtCode: $('#DistrictCode option:selected').val() },
        type: 'POST',
        catche: false,
        error: function (xhr, status, error) {
            alert('An Error occured while processig your request.')
            return false;
        },
        success: function (data) {
            $('#ddlBlocks').empty();
            $.each(data, function () {
                $('#ddlBlocks').append("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
        }
    });
}

function LoadFacilityName() {
    $.ajax({
        url: "/LocationMasterDataEntry/GetFacilityNameByFacilityCode",
        data: { facilityCode: $('#ddlFacility option:selected').val() },
        type: 'POST',
        catche: false,
        error: function (xhr, status, error) {
            alert('An Error occurred while processing your request.')
            return false;
        },
        success: function (data) {
            $('#ddlFacilityNameList').empty();
            $.each(data, function () {
                $('#ddlFacilityNameList').append("<option value=" + this.Value + ">" + this.Text + "</option>");
                //console.log("appended");
            });
        }
    });
}

//
function checkLockStatus() {

    $.ajax({

        type: 'GET',
        url: '/LocationMasterDataEntry/CheckLockStatusPMGSY3/',
        async: false,
        cache: false,
        data: { blockCode: $('#ddlBlocks option:selected').val(), },
        success: function (data) {
            if (data.status == false) {
                $('#btnSave').hide('slow');
                $('#lbFinalized').show('slow');
            }
            else {
                $('#btnSave').show('slow');
                $('#lbFinalized').hide('slow');
            }
        }


    });
}