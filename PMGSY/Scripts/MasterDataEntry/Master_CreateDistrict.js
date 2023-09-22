$(document).ready(function () {

    //this should be added on document ready
    $.validator.unobtrusive.parse($('#frmCreateDistrict'));

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) { 
            return false;
        }
    });



    $('#btnSave').click(function (e) {
        //e.preventDefault();
      

        if ($('#frmCreateDistrict').valid()) {
            var stateCode = $('#ddlStates option:selected').val();

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/LocationMasterDataEntry/CreateDistrict",
                type: "POST",
               // dataType: "json",
                data: $("#frmCreateDistrict").serialize(),
                success: function (data) {

                    if (data.success==true) {
                        alert(data.message);
                        ClearDetails();
                        if ($("#dvDistrictDetails").is(":visible")) {
                            $('#dvDistrictDetails').hide('slow');

                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();

                        }
                        if (!$("#dvSearchDistrict").is(":visible")) {
                            $('#dvSearchDistrict').show('slow');

                        }
                        SearchCreateDistrictDetails(stateCode);                     
                    }   
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                        
                    }
                    else {
                        $("#dvDistrictDetails").html(data);
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

        e.preventDefault();

        if ($('#frmCreateDistrict').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#ddlStates").attr("disabled", false);
            var stateCode = $('#ddlStates option:selected').val();

            $.ajax({
                url: "/LocationMasterDataEntry/EditDistrict",
                type: "POST",
               // dataType: "json",
                data: $("#frmCreateDistrict").serialize(),
                success: function (data) {
                    // $("#dvDistrictDetails").html(data);

                    if (data.success==true) {
                        alert(data.message);
                       
                        //$('#tbDistrictList').trigger('reloadGrid');
                        //if ($("#RoleCode").val() == 36) {
                        //    $("#btnSearchView").trigger('click');
                        //}
                        //else {
                        //    $('#dvDistrictDetails').load("/LocationMasterDataEntry/CreateDistrict");
                        //}

                        ClearDetails();
                        if ($("#dvDistrictDetails").is(":visible")) {
                            $('#dvDistrictDetails').hide('slow');

                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();

                        }
                        if (!$("#dvSearchDistrict").is(":visible")) {
                            $('#dvSearchDistrict').show('slow');

                        }
                        SearchCreateDistrictDetails(stateCode);

                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $("#ddlStates").attr("disabled", true);
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvDistrictDetails").html(data);
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
        //    url: "/LocationMasterDataEntry/CreateDistrict",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        //$("#mainDiv").html(data);
        //        $("#dvDistrictDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }

        //});
        ClearDetails();
        if ($("#dvDistrictDetails").is(":visible")) {
            $('#dvDistrictDetails').hide('slow');

            $('#btnSearchView').hide();
            $('#btnCreateNew').show();

        }
        if (!$("#dvSearchDistrict").is(":visible")) {
            $('#dvSearchDistrict').show('slow');

        }
       
    });


    $('#btnReset').click(function () {
       // ClearDetails();
    });


    //for expand and collpase div
    $("#dvhdCreateNewDistrictDetails").click(function () {

        if ($("#dvCreateNewDistrictDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvCreateNewDistrictDetails").slideToggle(300);
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvCreateNewDistrictDetails").slideToggle(300);
        }
    });

    $("#ddlStates").change(function () {

        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#message').html('');
        }

    }); //end function District change
});

function ClearDetails() {
    //$('#mainDiv text').val('');
    $('#MAST_DISTRICT_NAME').val('');
    //$('#ddlStates').val('0'); 
    $('#ddlStates').val('0');

    $('#rdoIsPMGSYIncludedNo').attr('checked', true);
    $('#rdoIsIAPDistrictNo').attr('checked', true);
    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}

function SearchCreateDistrictDetails(stateCode) {

    $('#tbDistrictList').setGridParam({
        url: '/LocationMasterDataEntry/GetDistrictDetailsList'
    });
    /*var data = $('#tbDistrictList').jqGrid("getGridParam", "postData");
    data._search = false;
    data.searchField = "";
    $('#tbDistrictList').jqGrid("setGridParam", { "postData": data });*/

    $('#tbDistrictList').jqGrid("setGridParam", { "postData": { stateCode: stateCode } });
    $('#tbDistrictList').trigger("reloadGrid", [{ page: 1 }]);

}