var stateCode;
var selectedDistrictCode;
var selectedBlockCode;

jQuery.validator.addMethod("comparefieldvillagepopvalidator", function (value, element, param) {

    if (parseInt($('#MAST_VILLAGE_TOT_POP').val()) < parseInt($('#MAST_VILLAGE_SCST_POP').val())) {
     
        return false;
    }
    else {
      
        return true;
    }


});

jQuery.validator.unobtrusive.adapters.addBool("comparefieldvillagepopvalidator");

$(document).ready(function () {

    //Added By Abhishek kamble 4-Apr-2014 start
    $(function () {

        if ($("#RoleCode").val() == 22) {//PIU
            if ($("#EncryptedVillageCode").val() == "") {
                $("#ddlStates").val($("#SelectedStateCode").val());
                $("#ddlStates").attr("disabled", "disabled");
                $("#ddlDistricts").attr("disabled", "disabled");

                $("#ddlStates").trigger("change");
            }
        }
    });
    //Added By Abhishek kamble 4-Apr-2014 end


    //this should be added on document ready
    $.validator.unobtrusive.parse($('#frmCreateVillage'));

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });


    $('#btnSave').click(function (e) {
      //  e.preventDefault();
      
        if ($('#frmCreateVillage').valid()) {

            //Added By Abhishek kamble 4-Apr-2014 start
            $(function () {
                if ($("#RoleCode").val() == 22) {//PIU
                    $("#ddlStates").attr("disabled", false);
                    $("#ddlDistricts").attr("disabled", false);
                }
            });
            //Added By Abhishek kamble 4-Apr-2014 end

            var yesDisabled = false;
            var noDisabled = false;
            if ($('#rdoIsSchedule5Yes').is(':disabled')) {
                yesDisabled = true;
            }

            if ($('#rdoIsSchedule5No').is(':disabled')) {
                noDisabled = true;
            }

            $('#rdoIsSchedule5Yes').attr('disabled', false);
            $('#rdoIsIsSchedule5No').attr('disabled', false);
             stateCode = $('#ddlStates option:selected').val();
             selectedDistrictCode = $('#ddlDistricts option:selected').val();
             selectedBlockCode = $('#ddlBlocks option:selected').val();

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/LocationMasterDataEntry/CreateVillage",
                type: "POST",
                // dataType: "json",
                data: $("#frmCreateVillage").serialize(),
                success: function (data) {

                    //  $("#mainDiv").html(data);

                    if (data.success == true) {
                        alert(data.message);
                        ClearDetails();
                        if ($("#dvVillageDetails").is(":visible")) {
                            $('#dvVillageDetails').hide('slow');

                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();

                        }
                        if (!$("#dvSearchVillage").is(":visible")) {
                            $('#dvSearchVillage').show('slow');
                        }
                        SearchCreateVillageDetails(stateCode, selectedDistrictCode, selectedBlockCode);

                        setFormStateDist();
                    }
                    else if (data.success == false) {
                        if (data.message != "") {

                            if (yesDisabled == true) {
                                $('#rdoIsSchedule5Yes').attr('disabled', true);
                            }

                            if (noDisabled == true) {
                                $('#rdoIsIsSchedule5No').attr('disabled', true);
                            }

                            $('#errvlmessage').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                        setFormStateDist();
                    }
                    else {
                        $("#dvVillageDetails").html(data);
                        setFormStateDist();
                    }

                    $.unblockUI();

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                    setFormStateDist();
                }

            });

        }

    });


    $('#btnUpdate').click(function (e) {

        if ($('#frmCreateVillage').valid()) {
            $('#rdoIsSchedule5Yes').attr('disabled', false);
            $('#rdoIsIsSchedule5No').attr('disabled', false);
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $("#ddlStates").attr("disabled", false);
            $("#ddlDistricts").attr("disabled", false);
            $("#ddlBlocks").attr("disabled", false);

            stateCode = $('#ddlStates option:selected').val();
            selectedDistrictCode = $('#ddlDistricts option:selected').val();
            selectedBlockCode = $('#ddlBlocks option:selected').val();

            $.ajax({
                url: "/LocationMasterDataEntry/EditVillage",
                type: "POST",
               // dataType: "json",
                data: $("#frmCreateVillage").serialize(),
                success: function (data) {
                    
                    //$("#mainDiv").html(data);

                    if (data.success==true) {
                        alert(data.message);
                       
                        //$('#tbVillageList').trigger('reloadGrid');
                        //if ($("#RoleCode").val() == 36) {
                        //    $("#btnSearchView").trigger('click');
                        //}
                        //else {
                        //    $('#dvVillageDetails').load("/LocationMasterDataEntry/CreateVillage");
                        //}
                        if ($("#dvVillageDetails").is(":visible")) {
                            $('#dvVillageDetails').hide('slow');

                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();

                        }
                        if (!$("#dvSearchVillage").is(":visible")) {
                            $('#dvSearchVillage').show('slow');
                        }
                        //SearchCreateVillageDetails(stateCode, selectedDistrictCode, selectedBlockCode);

                        $('#tbVillageList').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlSearchStates').val(), districtCode: $("#ddlSearchDistrict").val(), blockCode: $("#ddlSearchBlocks").val() } });
                        $('#tbVillageList').trigger("reloadGrid"/*, [{ page: 1 }]*/);
                    

                        setFormStateDist();

                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $("#ddlStates").attr("disabled", true);
                            $("#ddlDistricts").attr("disabled", true);
                            $("#ddlBlocks").attr("disabled", true);

                            $('#errvlmessage').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvVillageDetails").html(data);
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
        //    url: "/LocationMasterDataEntry/CreateVillage",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        // $("#mainDiv").html(data);
        //        $('#rdoIsSchedule5Yes').attr('disabled', false);
        //        $('#rdoIsSchedule5dNo').attr('disabled', false);
        //        $("#dvVillageDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }

        //});
        if ($("#dvVillageDetails").is(":visible")) {
            $('#dvVillageDetails').hide('slow');

            $('#btnSearchView').hide();
            $('#btnCreateNew').show();

        }
        if (!$("#dvSearchVillage").is(":visible")) {
            $('#dvSearchVillage').show('slow');
        }
    });



    $('#btnReset').click(function (e) {
        //  ClearDetails();


        //Added By Abhishek kamble 20-Feb-2014 start
        if ($("#RoleCode").val() == 22) {//PIU
            
                e.preventDefault();
                $("input,select").removeClass("input-validation-error");
                $('.field-validation-error').html('');
            }
        //Added By Abhishek kamble 20-Feb-2014 end

        $('#dvErrorMessage').hide('slow');
        $('#rdoIsSchedule5Yes').attr('disabled', false);
        $('#rdoIsSchedule5dNo').attr('disabled', false);

    });


    $("#ddlStates").change(function () {

        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#errvlmessage').html('');
        }


        FillInCascadeDropdown({ userType: $("#ddlStates").find(":selected").val() },
                    "#ddlDistricts", "/LocationMasterDataEntry/GetDistrictsByStateCode?stateCode=" + $('#ddlStates option:selected').val());

        $('#ddlBlocks').empty();
        $('#ddlBlocks').append("<option value=0>--Select--</option>");

    }); //end function state change

    $("#ddlDistricts").change(function () {

        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#errvlmessage').html('');
        }

        $('#ddlBlocks').empty();
        FillInCascadeDropdown({ userType: $("#ddlDistricts").find(":selected").val() },
                    "#ddlBlocks", "/LocationMasterDataEntry/GetBlocksByDistrictCode?districtCode=" + $('#ddlDistricts option:selected').val());

    }); //end function District change
    $("#ddlBlocks").change(function () {
        $.ajax({
            url: "/LocationMasterDataEntry/GetIsShedule5?blockCode=" + $('#ddlBlocks option:selected').val(),
            type: "POST",
            // dataType: "json",
            data: $("#frmCreateVillage").serialize(),
            success: function (data) {
                if (data.schedule5 == 'Y') {
                   
                    $('#rdoIsSchedule5Yes').attr('checked', 'checked');
                    $('#rdoIsSchedule5Yes').attr('disabled', true);
                    $('#rdoIsIsSchedule5No').attr('disabled', true);

                }
                else {
                    $('#rdoIsSchedule5Yes').attr('disabled', false);
                    $('#rdoIsIsSchedule5No').attr('disabled', false);

                }
                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }

        });
    }); //end of function Block change for Schedule5 enabled and disabled

    $('#MAST_VILLAGE_NAME').change(function () {

        $('#dvErrorMessage').hide('slow');
        $('#errvlmessage').html('');
    });

    //for expand and collpase Document Details 
    $("#dvhdCreateNewVillageDetails").click(function () {

        if ($("#dvCreateNewVillageDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvCreateNewVillageDetails").slideToggle(300);
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvCreateNewVillageDetails").slideToggle(300);
        }
    });



});

function ClearDetails() {

    $('#MAST_VILLAGE_NAME').val('');
    $('#MAST_VILLAGE_TOT_POP').val('');
    $('#MAST_VILLAGE_SCST_POP').val('');
    $('#rdoIsSchedule5Yes').attr('disabled', false);
    $('#rdoIsSchedule5dNo').attr('disabled', false);
    $('#rdoIsIsSchedule5No').attr('checked', true);
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
    $('#errvlmessage').html('');
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
    }, "json").complete(function () {//Added By Abhishek kamble 4-Apr-2014 

        if ($("#RoleCode").val() == 22) {

            if (dropdown == '#ddlDistricts') {
                $("#ddlDistricts").val($("#SearchDistCode").val());
                setTimeout(function () {
                    $("#ddlDistricts").trigger("change");
                    //$("#btnSearch").trigger("click");
                }, 1000);
                //$("#SearchDistCode").val(null);
            }
        }

    });

    $.unblockUI();
} //end FillInCascadeDropdown()

function SearchCreateVillageDetails(stateCode, selectedDistrictCode, selectedBlockCode) {
    $('#ddlSearchStates').val(stateCode);
    $('#ddlSearchStates').trigger('change');   
    setTimeout(function () {
        $("#ddlSearchDistrict").val(selectedDistrictCode);
    }, 1000);
    setTimeout(function () {
        $("#ddlSearchDistrict").trigger('change');
    }, 1200);
    setTimeout(function () {
        $("#ddlSearchBlocks").val(selectedBlockCode);
    }, 1500);
    setTimeout(function () {
    $('#tbVillageList').setGridParam({
        url: '/LocationMasterDataEntry/GetVillageDetailsList'
    });
   /* var data = $('#tbVillageList').jqGrid("getGridParam", "postData");
    data._search = false;
    data.searchField = "";*/

    $('#tbVillageList').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlSearchStates').val(), districtCode: $("#ddlSearchDistrict").val(), blockCode: $("#ddlSearchBlocks").val() } });
    $('#tbVillageList').trigger("reloadGrid", [{ page: 1 }]);
    }, 2000);

}

function setFormStateDist() {

    //Added By Abhishek kamble 4-Apr-2014 start
    $(function () {
        if ($("#RoleCode").val() == 22) {//PIU
            $("#ddlStates").val($("#SelectedStateCode").val());
            $("#ddlStates").attr("disabled", "disabled");
            $("#ddlDistricts").attr("disabled", "disabled");
            $("#ddlStates").trigger("change");
        }
    });
    //Added By Abhishek kamble 4-Apr-2014 end

}