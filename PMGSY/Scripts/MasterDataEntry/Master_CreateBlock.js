var IsIAP;
$(document).ready(function () {

    //this should be added on document ready
    $.validator.unobtrusive.parse($('#frmCreateBlock'));

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
    
    $('#btnSave').click(function (e) {
        //e.preventDefault();

        var yesDisabled = false;
        var noDisabled = false;
        if ($('#rdoIsIAPYes').is(':disabled')) {
            yesDisabled = true;
        }

        if ($('#rdoIsIAPNo').is(':disabled')) {
            noDisabled = true;
        }


        $('#rdoIsIAPYes').attr('disabled', false);
        $('#rdoIsIAPNo').attr('disabled', false);
        if ($('#frmCreateBlock').valid()) {

            var stateCode = $('#ddlStates option:selected').val();
            var districtCode = $('#ddlDistricts option:selected').val();

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            //if ($('#frmCreateDistrict').valid()) {
            $.ajax({
                url: "/LocationMasterDataEntry/CreateBlock",
                type: "POST",
               // dataType: "json",
                data: $("#frmCreateBlock").serialize(),
                success: function (data) {

                    if (data.success==true) {
                        alert(data.message);
                        // ClearDetails();
                        $('#btnReset').trigger("click");
                        if ($("#dvBlockDetails").is(":visible")) {
                            $('#dvBlockDetails').hide('slow');

                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();

                        }
                        if (!$("#dvSearchBlock").is(":visible")) {
                            $('#dvSearchBlock').show('slow');
                        }
                        SearchCreateBlockDetails(stateCode,districtCode);

                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');

                            if (yesDisabled == true) {
                                $('#rdoIsIAPYes').attr('disabled', true);
                            }

                            if (noDisabled == true) {
                                $('#rdoIsIAPNo').attr('disabled', true);
                            }
                        }

                    }
                    else {
                        $("#dvBlockDetails").html(data);
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

        
        if ($('#frmCreateBlock').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#ddlStates").attr("disabled", false);
            $("#ddlDistricts").attr("disabled", false);
            var stateCode = $('#ddlStates option:selected').val();
            var districtCode = $('#ddlDistricts option:selected').val();

            $.ajax({
                url: "/LocationMasterDataEntry/EditBlock",
                type: "POST",
               // dataType: "json",
                data: $("#frmCreateBlock").serialize(),
                success: function (data) {

                    if (data.success==true) {
                        alert(data.message);                       
                        //$('#tbBlockList').trigger("reloadGrid");
                        //if ($("#RoleCode").val() == 36) {
                        //    $("#btnSearchView").trigger('click');
                        //}
                        //else {
                        //    $('#dvBlockDetails').load("/LocationMasterDataEntry/CreateBlock");
                        //}
                        $('#btnReset').trigger("click");
                        if ($("#dvBlockDetails").is(":visible")) {
                            $('#dvBlockDetails').hide('slow');

                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();

                        }
                        if (!$("#dvSearchBlock").is(":visible")) {
                            $('#dvSearchBlock').show('slow');
                        }
                        SearchCreateBlockDetails(stateCode,districtCode);


                    }
                    else if (data.success == false) {

                        if (data.message != "") {
                            $("#ddlStates").attr("disabled", true);
                            $("#ddlDistricts").attr("disabled", true);
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvBlockDetails").html(data);
                        $("#ddlStates").attr("disabled", true);
                        $("#ddlDistricts").attr("disabled", true);
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
        //    url: "/LocationMasterDataEntry/CreateBlock",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        $("#dvBlockDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }

        //});
        $('#btnReset').trigger("click");
        if ($("#dvBlockDetails").is(":visible")) {
            $('#dvBlockDetails').hide('slow');

            $('#btnSearchView').hide();
            $('#btnCreateNew').show();

        }
        if (!$("#dvSearchBlock").is(":visible")) {
            $('#dvSearchBlock').show('slow');
        }
    });



    $('#btnReset').click(function () {
        // ClearDetails();
        $('#dvErrorMessage').hide('slow');
    });


    $("#ddlStates").change(function () {

        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#message').html('');
        }

        FillInCascadeDropdown({ userType: $("#ddlStates").find(":selected").val() },
                    "#ddlDistricts", "/LocationMasterDataEntry/GetDistrictsByStateCode?stateCode=" + $('#ddlStates option:selected').val());

    }); //end function District change


    //for expand and collpase Document Details 
    $("#dvhdCreateNewBlockDetails").click(function () {

        if ($("#dvCreateNewBlockDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvCreateNewBlockDetails").slideToggle(300);
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvCreateNewBlockDetails").slideToggle(300);
        }
    });

    $('#MAST_BLOCK_NAME').change(function () {

        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });


    $("#ddlDistricts").change(function () {
        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#message').html('');
        }

        $.ajax({
            url: "/LocationMasterDataEntry/IsDistrictIAP?districtCode=" + $('#ddlDistricts option:selected').val(),
            type: "POST",
            // dataType: "json",
            data: $("#frmCreateBlock").serialize(),
            success: function (data) {

                //Added By Abhishek kamble 25-Feb-2014
                IsIAP = data.isIAP;

                if (data.isIAP == 'Y') {


                    //$('#rdoIsIAPYes').attr('checked', 'checked');
                    $('#rdoIsIAPYes').attr('disabled', false);
                    $('#rdoIsIAPNo').attr('disabled', false);

                }
                else {
                    $('#rdoIsIAPYes').attr('disabled', true);
                    $('#rdoIsIAPNo').attr('disabled', true);

                }
                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }

        });

    });

    

});

function ClearDetails() {

    $('#MAST_BLOCK_NAME').val('');   
    $('#ddlStates').val('0');

    if ($("#ddlDistricts option").length > 1) {

        FillInCascadeDropdown({ userType: $("#ddlStates").find(":selected").val() },
                      "#ddlDistricts", "/LocationMasterDataEntry/GetDistrictsByStateCode?stateCode=" + $('#ddlStates option:selected').val());
    }

    $('#rdoIsPMGSYIncludedNo').attr('checked', true);
    $('#rdoIsDESERTNo').attr('checked', true);
    $('#rdoIsBADBNo').attr('checked', true);
    $('#rdoIsTRIBALNo').attr('checked', true);
    $('#rdoIsIsSchedule5No').attr('checked', true);

    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}

function FillInCascadeDropdown(map, dropdown, action) {

   var message = '';

      if (dropdown == '#ddlDistricts') {
        message = '<h4><label style="font-weight:normal"> Loading Districts... </label></h4>';
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

function SearchCreateBlockDetails(stateCode,districtCode) {
    //alert("SerachUPdate" + $('#ddlSearchDistrict option:selected').val());
    $('#ddlSearchDistrict').val(0);
    $('#ddlSearchStates').val(stateCode);
  
    setTimeout(function () {
        $('#ddlSearchStates').trigger('change');       

    }, 1000);
    setTimeout(function () {
        $("#ddlSearchDistrict").val(districtCode);
    }, 1200);
   
    setTimeout(function () {
        $('#tbBlockList').setGridParam({
            url: '/LocationMasterDataEntry/GetBlockDetailsList'
        });
        /* var data = $('#tbBlockList').jqGrid("getGridParam", "postData");
         data._search = false;
         data.searchField = ""; */

        $('#tbBlockList').jqGrid("setGridParam", { "postData": { stateCode: stateCode, districtCode: $('#ddlSearchDistrict option:selected').val() } });

        $('#tbBlockList').trigger("reloadGrid", [{ page: 1 }]);
    }, 1500);

}