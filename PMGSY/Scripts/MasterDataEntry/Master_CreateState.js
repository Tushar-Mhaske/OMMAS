$(document).ready(function () {

    //this should be added on document ready
    $.validator.unobtrusive.parse($('#frmCreateState'));

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#btnSave').click(function (e) {
        //e.preventDefault();
       

        if ($('#frmCreateState').valid()) {
            var StateUT = $('#ddlStateUTs').val();
            var StateType=$('#ddlStateTypes').val();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/LocationMasterDataEntry/Create",
                type: "POST",
               // dataType: "json",
                data: $("#frmCreateState").serialize(),
                success: function (data) {

                    if (data.success==true) {
                        alert(data.message);
                       ClearStateDetails();
                        // $('#tbStateList').trigger('reloadGrid');
                       if ($("#dvStateDetails").is(":visible")) {
                           $('#dvStateDetails').hide('slow');
                           $('#btnSearchView').hide();
                           $('#btnCreateNew').show();
                       }
                       if (!$("#dvSearchState").is(":visible")) {
                           $('#dvSearchState').show('slow');
                       }
                       SearchCreateStateDetails(StateUT, StateType);
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }

                    }
                    else {
                        $("#dvStateDetails").html(data);
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

       // e.preventDefault();

        if ($('#frmCreateState').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            var StateUT = $('#ddlStateUTs').val();
            var StateType = $('#ddlStateTypes').val();
            $.ajax({
                url: "/LocationMasterDataEntry/Edit",
                type: "POST",
               // dataType: "json",
                data: $("#frmCreateState").serialize(),
                success: function (data) {

                    if (data.success==true) {
                        //alert(data.message);
                        //$('#tbStateList').trigger('reloadGrid');
                        //$('#dvStateDetails').load("/LocationMasterDataEntry/CreateState");
                        alert(data.message);
                        ClearStateDetails();                        
                        if ($("#dvStateDetails").is(":visible")) {
                            $('#dvStateDetails').hide('slow');
                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();
                        }
                        if (!$("#dvSearchState").is(":visible")) {
                            $('#dvSearchState').show('slow');
                        }
                        SearchCreateStateDetails(StateUT, StateType);

                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvStateDetails").html(data);
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
        //    url: "/LocationMasterDataEntry/CreateState",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        // $("#mainDiv").html(data);
        //        $("#dvStateDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }

        //});
        ClearStateDetails();
        if ($("#dvStateDetails").is(":visible")) {
            $('#dvStateDetails').hide('slow');
            $('#btnSearchView').hide();
            $('#btnCreateNew').show();
        }
        if (!$("#dvSearchState").is(":visible")) {
            $('#dvSearchState').show('slow');
        }

        
    });

    $('#btnReset').click(function () {
        ClearStateDetails();
      
       
    });

    ////for expand and collpase Document Details 
    //$("#dvhdCreateNewStateDetails").click(function () {

    //    if ($("#dvCreateNewStateDetails").is(":visible")) {

    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

    //        //$("#dvDocumentDetails").css('margin-bottom','10px');

    //        $(this).next("#dvCreateNewStateDetails").slideToggle(300);
    //    }

    //    else {
    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

    //        $(this).next("#dvCreateNewStateDetails").slideToggle(300);
    //    }
    //});


    $("#spCollapseIconCN").click(function () {

        if ($("#dvStateDetails").is(":visible")) {
           $("#dvStateDetails").hide("slow");
           $('#tblAddNewButton').attr('style', 'padding-top:0em; width:100%;');
           $("#btnCreateNew").show();
        }
    });

    $("#dvhdCreateNewStateDetails").click(function () {

        if ($("#dvCreateNewStateDetails").is(":visible")) {
            $(this).next("#dvCreateNewStateDetails").slideToggle(300);
        }
        else {
            $(this).next("#dvCreateNewStateDetails").slideToggle(300);
        }
    });




    $('#MAST_STATE_NAME').change(function () {
        
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

});

function ClearStateDetails() {
    //$('#mainDiv text').val('');
   // $('#frmCreateState input[type=text]').attr('value', '');
    $('#MAST_STATE_NAME').val('');
    $('#MAST_NIC_STATE_CODE').val('');
    $('#ddlStateUTs').val('0');
    $('#ddlStateTypes').val('0');

    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}
function SearchCreateStateDetails(StateUT, StateType) {

    $('#tbStateList').setGridParam({
        url: '/LocationMasterDataEntry/GetStateDetailsList'
    });
    $('#ddlSearchStateUTs').val(StateUT);
    $('#ddlSearchStateTypes').val(StateType);
    /*var data = $('#tbDistrictList').jqGrid("getGridParam", "postData");
    data._search = false;
    data.searchField = "";
    $('#tbDistrictList').jqGrid("setGridParam", { "postData": data });*/

    $('#tbStateList').jqGrid("setGridParam", { "postData": { StateType: StateType, StateType: StateType } });
    $('#tbStateList').trigger("reloadGrid", [{ page: 1 }]);

}