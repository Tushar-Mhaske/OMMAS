$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmExecution");

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    //This method is for reset button click.
    $("#btnReset").click(function () {

        $("#dvErrorMessage").hide();
    });

    $("#MAST_HEAD_DESC").focus(function () {

        $("#dvErrorMessage").hide(1000);
        $("#message").html('');
    });

    $("#MAST_HEAD_SH_DESC").focus(function () {

        $("#dvErrorMessage").hide(1000);
        $("#message").html('');
    });

    $("#radioHeadType").focus(function () {

        $("#dvErrorMessage").hide(1000);
        $("#message").html('');
    });

    //This method is for cancel button click.
    $("#btnCancel").click(function () {
        //$("#dvDetails").load('/Master/AddMasterExecution');
        if ($("#dvDetails").is(":visible")) {
            $('#dvDetails').hide('slow');
            $('#btnSearchView').hide();
            $('#btnCreateNew').show();
        }

        if (!$("#dvSearchHeadType").is(":visible")) {
            $("#dvSearchHeadType").show('slow');
        }
    });

    //This method is for maximising/minimising form and list.
    $("#dvhdCreateNewDetails").click(function () {

        if ($("#dvCreateNewDetails").is(":visible")) {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $(this).next("#dvCreateNewDetails").slideToggle(300);
        }
        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
            $(this).next("#dvCreateNewDetails").slideToggle(300);
        }
    });

    //This method is for save button click.
    $("#btnSave").click(function () {
        $("#ErrorMessage").show();
        if ($("#frmExecution").valid()) {
            var itemType = $('input[name=MAST_HEAD_TYPE]:checked').val();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/AddMasterExecution",
                type: "POST",
               
                data: $("#frmExecution").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                     
                      //  ExcectionCreateSeachDetails(itemType);
                        //$("#btnReset").trigger('click');
                        if ($("#dvDetails").is(":visible")) {
                            $('#dvDetails').hide('slow');
                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();
                        }

                        if (!$("#dvSearchHeadType").is(":visible")) {
                            $("#dvSearchHeadType").show('slow');
                        }
                        ExcectionCreateSeachDetails(itemType);
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvDetails").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }
            });
        }
    });

    //This method is for update button click.
    $("#btnUpdate").click(function () {
        var itemType = "";
        if ($("#frmExecution").valid()) {
             itemType = $('#MAST_HEAD_TYPE').val();           
           
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/EditMasterExecution",
                type: "POST",
           
                data: $("#frmExecution").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        //$('#tblList').trigger('reloadGrid');
                        //$("#dvDetails").load('/Master/AddMasterExecution');
                        //$("#dvErrorMessage").hide();
                        if ($("#dvDetails").is(":visible")) {
                            $('#dvDetails').hide('slow');
                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();
                        }

                        if (!$("#dvSearchHeadType").is(":visible")) {
                            $("#dvSearchHeadType").show('slow');
                        }
                        ExcectionCreateSeachDetails(itemType);
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    } else {
                        $("#dvDetails").html(data);
                    }

                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }
            });
        }
      });
    
});


function  ExcectionCreateSeachDetails(itemType) {
    $('#ddlSearchType').val(itemType);
    $('#tblList').setGridParam({
        url: '/Master/GetMasterExecutionDetails', datatype: 'json'
    });
    $('#tblList').jqGrid("setGridParam", { "postData": { typeCode: $('#ddlSearchType option:selected').val() } });
    $('#tblList').trigger("reloadGrid", [{ page: 1 }]);

}
