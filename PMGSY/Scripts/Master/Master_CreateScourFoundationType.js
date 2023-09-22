
$(document).ready(function () {

    if ($("#frmMasterScourFoundationType") != null) {
        $.validator.unobtrusive.parse("#frmMasterScourFoundationType");
    }


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#btnSave').click(function (e) {

        if ($('#frmMasterScourFoundationType').valid()) {
            var sfType = $('#IMS_SC_FD_TYPE option:selected').val();

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/AddMasterScourFoundationType/",
                type: "POST",
            
                data: $("#frmMasterScourFoundationType").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                        //$('#btnReset').trigger('click');
                        //SearchCreateScourFounDetails(sfType);

                        if ($("#dvScourFoundationDetails").is(":visible")) {
                            $('#dvScourFoundationDetails').hide('slow');
                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();
                        }                    

                        if (!$("#dvSearchScourFoundation").is(":visible")) {
                            $("#dvSearchScourFoundation").show('slow');
                        }
                        SearchCreateScourFounDetails(sfType);

                        $.unblockUI();
                                         
                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvScourFoundationDetails").html(data);
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

        if ($('#frmMasterScourFoundationType').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $('#IMS_SC_FD_TYPE').attr('disabled', false);
            var sfType = $('#IMS_SC_FD_TYPE option:selected').val();

            $.ajax({
                url: "/Master/EditMasterScourFoundationType/",
                type: "POST",
             
                data: $("#frmMasterScourFoundationType").serialize(),
                success: function (data) {
                  
                    if (data.success==true) {
                        alert(data.message);
                        //$('#tblMasterScourFoundationTypeList').trigger('reloadGrid');
                        //$("#dvScourFoundationDetails").load("/Master/AddEditMasterScourFoundationType");
                        if ($("#dvScourFoundationDetails").is(":visible")) {
                            $('#dvScourFoundationDetails').hide('slow');
                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();
                        }

                        if (!$("#dvSearchScourFoundation").is(":visible")) {
                            $("#dvSearchScourFoundation").show('slow');
                        }
                        SearchCreateScourFounDetails(sfType);
                    }
                    else if (data.success == false) {

                        if (data.message != "") {
                            $('#IMS_SC_FD_TYPE').attr('disabled', true);
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }

                    }
                    else {
                        $("#dvScourFoundationDetails").html(data);
                        $('#IMS_SC_FD_TYPE').attr('disabled', true);
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
        //    url: "/Master/AddEditMasterScourFoundationType",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {                
        //        $("#dvScourFoundationDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }
        //});

        if ($("#dvScourFoundationDetails").is(":visible")) {
            $('#dvScourFoundationDetails').hide('slow');
            $('#btnSearchView').hide();
            $('#btnCreateNew').show();
        }

        if (!$("#dvSearchScourFoundation").is(":visible")) {
            $("#dvSearchScourFoundation").show('slow');
        }
      
    });

    $('#btnReset').click(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');        
    });
    $("#dvhdCreateNewScourFoundationDetails").click(function () {

        if ($("#dvCreateNewScourFoundationDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvCreateNewScourFoundationDetails").slideToggle(300);
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvCreateNewScourFoundationDetails").slideToggle(300);
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
function SearchCreateScourFounDetails(sfType) {
    $('#ddlScourFoundationType').val(sfType);
    $('#tblMasterScourFoundationTypeList').setGridParam({
        url: '/Master/GetMasterScourFoundationTypeList/'
    });

    $('#tblMasterScourFoundationTypeList').jqGrid("setGridParam", { "postData": { SfTypeCode: $('#ddlScourFoundationType option:selected').val() } });

    $('#tblMasterScourFoundationTypeList').trigger("reloadGrid", [{ page: 1 }]);

}
