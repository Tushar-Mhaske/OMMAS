
$(document).ready(function () {

    if ($("#frmMasterContClassType") != null) {
        $.validator.unobtrusive.parse("#frmMasterContClassType");
    }

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    if ($('#stateCode').val() > 0) {
        $("#MAST_STATE_CODE").attr("disabled", true);
    }

    $('#btnSave').click(function (e) {

        if ($('#frmMasterContClassType').valid()) {

            $("#MAST_STATE_CODE").attr("disabled", false);
            var stateCode = $("#MAST_STATE_CODE").val();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/AddMasterContractorClassType/",
                type: "POST",
             
                data: $("#frmMasterContClassType").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                    
                        $('#btnReset').trigger('click');

                        if ($('#stateCode').val() > 0) {
                            $("#MAST_STATE_CODE").attr("disabled", true);
                        }

                        // $('#tblMasterContClassTypeList').trigger('reloadGrid');
                        if ($("#dvContClassDetails").is(":visible")) {
                            $('#dvContClassDetails').hide('slow');

                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();

                        }

                        if (!$("#dvSearchContrClass").is(":visible")) {
                            $("#dvSearchContrClass").show('slow');
                        }
                        SearchCreateContractorClassDetails(stateCode);
                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');

                            if ($('#stateCode').val() > 0) {
                                $("#MAST_STATE_CODE").attr("disabled", true);
                            }

                        }
                    }
                    else {
                        $("#dvContClassDetails").html(data);
                        if ($('#stateCode').val() > 0) {
                            $("#MAST_STATE_CODE").attr("disabled", true);
                        }

                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {

                    if ($('#stateCode').val() > 0) {
                        $("#MAST_STATE_CODE").attr("disabled", true);
                    }

                    alert(xhr.responseText);

                    $.unblockUI();
                }

            });

        }

    });

    $('#btnUpdate').click(function (e) {

        if ($('#frmMasterContClassType').valid()) {

            $("#MAST_STATE_CODE").attr('disabled', false);
            var stateCode = $("#MAST_STATE_CODE").val();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/EditMasterContractorClassType/",
                type: "POST",
           
                data: $("#frmMasterContClassType").serialize(),
                success: function (data) {

                    if (data.success==true) {
                        alert(data.message);
                        //$('#tblMasterContClassTypeList').trigger('reloadGrid');
                        //$("#dvContClassDetails").load("/Master/AddEditMasterContractorClassType");
                        if ($("#dvContClassDetails").is(":visible")) {
                            $('#dvContClassDetails').hide('slow');

                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();

                        }

                        if (!$("#dvSearchContrClass").is(":visible")) {
                            $("#dvSearchContrClass").show('slow');
                        }
                        SearchCreateContractorClassDetails(stateCode);
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvContClassDetails").html(data);
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

    $("#spCollapseIconCN").click(function () {

        if ($("#dvContClassDetails").is(":visible")) {
            $("#dvContClassDetails").hide("slow");

            $("#btnCreateNew").show();

            $("#dvSearchContrClass").show();            
            $("#btnSearchView").hide();            
        }
    });

    $('#btnCancel').click(function (e) {

        //$.ajax({
        //    url: "/Master/AddEditMasterContractorClassType",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {                
        //        $("#dvContClassDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }
        //});
        if ($("#dvContClassDetails").is(":visible")) {
            $('#dvContClassDetails').hide('slow');

            $('#btnSearchView').hide();
            $('#btnCreateNew').show();

        }

        if (!$("#dvSearchContrClass").is(":visible")) {
            $("#dvSearchContrClass").show('slow');
        }
       
    });

    $('#btnReset').click(function () {
        
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    

    
    //$("#dvhdCreateNewContClassDetails").click(function () {

    //    if ($("#dvCreateNewContClassDetails").is(":visible")) {

    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

        

    //        $(this).next("#dvCreateNewContClassDetails").slideToggle(300);
    //    }

    //    else {
    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

    //        $(this).next("#dvCreateNewContClassDetails").slideToggle(300);
    //    }
    //});

    $("#MAST_CON_CLASS_TYPE_NAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

});

function SearchCreateContractorClassDetails(stateCode) {
  
    $('#StateList').val(stateCode);  
    $('#tblMasterContClassTypeList').setGridParam({
        url: '/Master/GetMasterContractorClassTypeList'
        });     

    $('#tblMasterContClassTypeList').jqGrid("setGridParam", { "postData": { StateCode: $('#StateList option:selected').val() } });

        $('#tblMasterContClassTypeList').trigger("reloadGrid", [{ page: 1 }]);
 

}

