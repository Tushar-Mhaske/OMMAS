
var IsScheduleFive;
var stateCode;
var SelectedDistrictCode ;
var SelectedBlockCode;
var villName;
$(document).ready(function () {


    //Added By Abhishek kamble 4-Apr-2014 start
    $(function () {

        if ($("#RoleCode").val() == 22) {//PIU
            if ($("#EncryptedHabitationCode").val() == "") {
                $("#ddlStates").val($("#SelectedStateCode").val());
                $("#ddlStates").attr("disabled", "disabled");
                $("#ddlDistricts").attr("disabled", "disabled");

                $("#ddlStates").trigger("change");
            }
        }
    });
    //Added By Abhishek kamble 4-Apr-2014 end


    //this should be added on document ready
    $.validator.unobtrusive.parse($('#frmCreateHabitation'));


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });


    $('#btnSave').click(function (e) {
        // e.preventDefault();

        var yesDisabled = false;
        var noDisabled = false;
        if ($('#rdoIsSchedule5Yes').is(':disabled')) {
            yesDisabled = true;
        }

        if ($('#rdoIsSchedule5No').is(':disabled')) {
            noDisabled = true;
        }


        if ($('#frmCreateHabitation').valid()) {

            //Added By Abhishek kamble 4-Apr-2014 start
            $(function () {
                if ($("#RoleCode").val() == 22) {//PIU
                    $("#ddlStates").attr("disabled", false);
                    $("#ddlDistricts").attr("disabled", false);
                }
            });
            //Added By Abhishek kamble 4-Apr-2014 end


            $('#rdoIsSchedule5Yes').attr('disabled', false);
            $('#rdoIsSchedule5dNo').attr('disabled', false);
             stateCode = $('#ddlStates option:selected').val();
             SelectedDistrictCode = $('#ddlDistricts option:selected').val();
             SelectedBlockCode = $('#ddlBlocks option:selected').val();
             villName = $("#ddlVillages option:selected").text();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/LocationMasterDataEntry/CreateHabitation",
                type: "POST",
                // dataType: "json",
                data: $("#frmCreateHabitation").serialize(),
                success: function (data) {

                    // $("#mainDiv").html(data);

                    if (data.success == true) {
                        alert(data.message);
                        ClearDetails();
                       
                        if ($("#dvHabitationDetails").is(":visible")) {
                            
                            $("#dvHabitationDetails").hide('slow');
                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();
                        }
                        if (!$("#dvSearchHabitation").is(":visible")) {
                            $("#dvSearchHabitation").show('slow');
                        }
                        SearchDetails1(stateCode, SelectedDistrictCode, SelectedBlockCode, villName);
                        setFormStateDist();
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');

                            if (yesDisabled == true) {
                                $('#rdoIsSchedule5Yes').attr('disabled', true);
                            }

                            if (noDisabled == true) {
                                $('#rdoIsIsSchedule5No').attr('disabled', true);
                            }

                        }
                        setFormStateDist();
                    }
                    else {
                        $("#dvHabitationDetails").html(data);
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

        } else { }
    });


    $('#btnUpdate').click(function (e) {

        // e.preventDefault();
        $('#rdoIsSchedule5Yes').attr('disabled', false);
        $('#rdoIsSchedule5dNo').attr('disabled', false);
        if ($('#frmCreateHabitation').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#ddlStates").attr("disabled", false);
            $("#ddlDistricts").attr("disabled", false);
            $("#ddlBlocks").attr("disabled", false);
            $("#ddlVillages").attr("disabled", false);
            $("#ddlMPContituency").attr("disabled", false);
            $("#ddlMLAContituency").attr("disabled", false);

            stateCode = $('#ddlStates option:selected').val();
            SelectedDistrictCode = $('#ddlDistricts option:selected').val();
            SelectedBlockCode = $('#ddlBlocks option:selected').val();
            villName = $("#ddlVillages option:selected").text();

            $.ajax({
                url: "/LocationMasterDataEntry/EditHabitation",
                type: "POST",
                // dataType: "json",
                data: $("#frmCreateHabitation").serialize(),
                success: function (data) {

                    //$("#mainDiv").html(data);

                    if (data.success == true) {
                        alert(data.message);
                       
                        //$('#tbHabitationList').trigger('reloadGrid');
                        //if ($("#RoleCode").val() == 36) {
                        //    $("#btnSearchView").trigger('click');
                        //}
                        //else {
                        //    $('#dvHabitationDetails').load("/LocationMasterDataEntry/CreateHabitation", function () {
                        //        $.unblockUI();
                        //    }
                        //    );
                        //}
                        ClearDetails();
                        if ($("#dvHabitationDetails").is(":visible")) {

                            $("#dvHabitationDetails").hide('slow');
                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();
                        }
                        if (!$("#dvSearchHabitation").is(":visible")) {
                            $("#dvSearchHabitation").show('slow');
                        }
                        SearchDetails1(stateCode, SelectedDistrictCode, SelectedBlockCode, villName);
                        setFormStateDist();

                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $("#ddlStates").attr("disabled", true);
                            $("#ddlDistricts").attr("disabled", true);
                            $("#ddlBlocks").attr("disabled", true);
                            $("#ddlVillages").attr("disabled", true);
                            $("#ddlMPContituency").attr("disabled", true);
                            $("#ddlMLAContituency").attr("disabled", true);

                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');

                        }
                    }
                    else {

                        $("#dvHabitationDetails").html(data);
                        $("#ddlStates").attr("disabled", true);
                        $("#ddlDistricts").attr("disabled", true);
                        $("#ddlBlocks").attr("disabled", true);
                        $("#ddlVillages").attr("disabled", true);
                        $("#ddlMPContituency").attr("disabled", true);
                        $("#ddlMLAContituency").attr("disabled", true);
                    }

                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }

            });
        } else {//Added By Abhishek kamble 25-Feb-2014
            if (IsScheduleFive == "Y") {
                $('#rdoIsSchedule5Yes').attr('disabled', true);
                $('#rdoIsSchedule5dNo').attr('disabled', true);
            }
        }
    });

    $('#btnCancel').click(function (e) {

        //$.ajax({
        //    url: "/LocationMasterDataEntry/CreateHabitation",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        // $("#mainDiv").html(data);
        //        $('#rdoIsSchedule5Yes').attr('disabled', false);
        //        $('#rdoIsSchedule5dNo').attr('disabled', false);
        //        $('#dvHabitationDetails').html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }

        //});
        if ($("#dvHabitationDetails").is(":visible")) {

            $("#dvHabitationDetails").hide('slow');
            $('#btnSearchView').hide();
            $('#btnCreateNew').show();
        }
        if (!$("#dvSearchHabitation").is(":visible")) {
            $("#dvSearchHabitation").show('slow');
        }
       
    });


    $('#btnReset').click(function (e) {

        //Added By Abhishek kamble 20-Feb-2014 start
        if ($("#RoleCode").val() == 22) {//PIU
            {
                e.preventDefault();
                $("input,select").removeClass("input-validation-error");
                $('.field-validation-error').html('');
            }
            //Added By Abhishek kamble 20-Feb-2014 end

            $('#rdoIsSchedule5Yes').attr('disabled', false);
            $('#rdoIsSchedule5dNo').attr('disabled', false);
            ClearDetails();
        }
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

        $('#ddlVillages').empty();
        $('#ddlVillages').append("<option value=0>--Select--</option>");

        $('#ddlMPContituency').empty();
        $('#ddlMPContituency').append("<option value=0>--Select--</option>");

        $('#ddlMLAContituency').empty();
        $('#ddlMLAContituency').append("<option value=0>--Select--</option>");

    }); //end function state change

    $("#ddlDistricts").change(function () {

        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#message').html('');
        }

        $('#ddlBlocks').empty();
        FillInCascadeDropdown({ userType: $("#ddlDistricts").find(":selected").val() },
                    "#ddlBlocks", "/LocationMasterDataEntry/GetBlocksByDistrictCode?districtCode=" + $('#ddlDistricts option:selected').val());

        $('#ddlVillages').empty();
        $('#ddlVillages').append("<option value=0>--Select--</option>");

        $('#ddlMPContituency').empty();
        $('#ddlMPContituency').append("<option value=0>--Select--</option>");

        $('#ddlMLAContituency').empty();
        $('#ddlMLAContituency').append("<option value=0>--Select--</option>");

    }); //end function District change

    $("#ddlBlocks").change(function () {

        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#message').html('');
        }

        $('#ddlVillages').empty();
        FillInCascadeDropdown({ userType: $("#ddlBlocks").find(":selected").val() },
                    "#ddlVillages", "/LocationMasterDataEntry/GetVillagesByBlockCode?blockCode=" + $('#ddlBlocks option:selected').val());

        $('#ddlMPContituency').empty();
        FillInCascadeDropdown({ userType: $("#ddlBlocks").find(":selected").val() },
                    "#ddlMPContituency", "/LocationMasterDataEntry/GetMPContituencyByBlockCode?blockCode=" + $('#ddlBlocks option:selected').val());


        $('#ddlMLAContituency').empty();
        FillInCascadeDropdown({ userType: $("#ddlBlocks").find(":selected").val() },
                    "#ddlMLAContituency", "/LocationMasterDataEntry/GetMLAContituencyByBlockCode?blockCode=" + $('#ddlBlocks option:selected').val());

    }); //end function block change

    $("#ddlVillages").change(function () {
        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#message').html('');
        }

        $.ajax({
            url: "/LocationMasterDataEntry/GetIsShedule5?villageCode=" + $('#ddlVillages option:selected').val(),
            type: "POST",
            // dataType: "json",
            data: $("#frmCreateVillage").serialize(),
            success: function (data) {

                //Added By Abhishek kamble 25-Feb-2014
                IsScheduleFive = data.schedule5;

                if (data.schedule5 == 'Y') {


                    $('#rdoIsSchedule5Yes').attr('checked', 'checked');
                    $('#rdoIsSchedule5Yes').attr('disabled', true);
                    $('#rdoIsSchedule5dNo').attr('disabled', true);

                }
                else {
                    $('#rdoIsSchedule5Yes').attr('disabled', false);
                    $('#rdoIsSchedule5dNo').attr('disabled', false);

                }
                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }

        });

    });   //end of function Village change for Schedule5 enabled and disabled

    $('#MAST_HAB_NAME').change(function () {

        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });


    //for expand and collpase  
    $("#dvhdCreateNewHabitationDetails").click(function () {

        if ($("#dvCreateNewHabitationDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvCreateNewHabitationDetails").slideToggle(300);
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvCreateNewHabitationDetails").slideToggle(300);
        }
    });


    //});
});

function ClearDetails() {

    $('#MAST_HAB_NAME').val('');

    //Modiifed By Abhishek kamble 4-Apr-2014  start
    if ($("#RoleCode").val() != 22) {//PIU

        $('#ddlStates').val('0');

        if ($("#ddlDistricts option").length > 1) {

            FillInCascadeDropdown({ userType: $("#ddlStates").find(":selected").val() },
                          "#ddlDistricts", "/LocationMasterDataEntry/GetDistrictsByStateCode?stateCode=" + $('#ddlStates option:selected').val());

            //FillInCascadeDropdown({ userType: $("#ddlDistricts").find(":selected").val() },
            //           "#ddlBlocks", "/LocationMasterDataEntry/GetBlocksByDistrictCode?districtCode=" + $('#ddlDistricts option:selected').val());

        }

        $('#ddlBlocks').empty();
        $('#ddlBlocks').append("<option value=0>--Select--</option>");

        $('#ddlVillages').empty();
        $('#ddlVillages').append("<option value=0>--Select--</option>");


        $('#ddlMPContituency').empty();
        $('#ddlMPContituency').append("<option value=0>--Select--</option>");

        $('#ddlMLAContituency').empty();
        $('#ddlMLAContituency').append("<option value=0>--Select--</option>");

    }
    //Modiifed By Abhishek kamble 4-Apr-2014 end

    $('#rdoIsSchedule5Yes').attr('disabled', false);
    $('#rdoIsSchedule5dNo').attr('disabled', false);
    $('#rdoIsSchedule5dNo').attr('checked', true);

    $('#dvErrorMessage').hide("slow");

}

function FillInCascadeDropdown(map, dropdown, action) {


    var message = '';

    if (dropdown == '#ddlDistricts') {
        message = '<h4><label style="font-weight:normal"> Loading Districts... </label></h4>';
    }
    else if (dropdown == '#ddlBlocks') {
        message = '<h4><label style="font-weight:normal"> Loading Blocks... </label></h4>';
    }
    else if (dropdown == '#ddlVillages') {
        message = '<h4><label style="font-weight:normal"> Loading Villages... </label></h4>';
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
                }, 1000);
            }
        }

    });
    $.unblockUI();
} //end FillInCascadeDropdown()

function SearchDetails1(stateCode, SelectedDistrictCode, SelectedBlockCode, villName) {

    $('#ddlSearchStates').val(stateCode);
    $('#ddlSearchStates').trigger('change');
    setTimeout(function () {
        $("#ddlSearchDistrict").val(SelectedDistrictCode);
    }, 1200);
    setTimeout(function () {
        $("#ddlSearchDistrict").trigger('change');
    }, 1500);
    setTimeout(function () {
        $("#ddlSearchBlocks").val(SelectedBlockCode);
    }, 1800);

    setTimeout(function () {
        $('#tbHabitationList').setGridParam({
            url: '/LocationMasterDataEntry/GetHabitationDetailsList'
        });
        /* var data = $('#tbHabitationList').jqGrid("getGridParam", "postData");
         data._search = false;
         data.searchField = "";*/
        //Modified By Abhishek kamble 11-feb-2014
        //$('#tbHabitationList').jqGrid("setGridParam", { "postData": { stateCode: stateCode, districtCode: "0", blockCode: "0", villageName: "" } });
        $('#tbHabitationList').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlSearchStates').val(), districtCode: $("#ddlSearchDistrict option:selected").val(), blockCode: $("#ddlSearchBlocks option:selected").val(), villageName: "%" } });

        //$('#tbHabitationList').trigger("reloadGrid", [{ page: 1 }]);
        $('#tbHabitationList').trigger("reloadGrid");
    }, 2500);
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