
$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmAutonomousBody");

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    if ($('#stateCode').val() > 0) {
        $("#ddlStateNames").attr("disabled", true);
    }

    $('#btnSave').click(function (e) {
        if ($('#frmAutonomousBody').valid()) {
            var stateCode = $("#ddlStateNames").val();
            $("#ddlStateNames").attr("disabled", false);
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/AddMasterAdminAutonomousBody/",
                type: "POST",
            
                data: $("#frmAutonomousBody").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                      
                 
                        //$('#btnReset').trigger('click');                       
                        //$("#dvAutonomousBodyDetails").load("/Master/AddEditMasterAdminAutonomousBody");
                        //$('#tblAutonomousBodyList').trigger('reloadGrid');
                        if ($("#dvAutonomousBodyDetails").is(":visible")) {
                            $('#dvAutonomousBodyDetails').hide('slow');

                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();

                        }
                        if (!$("#dvSearchAutonomousBody").is(":visible")) {
                            $("#dvSearchAutonomousBody").show('slow');
                        }
                        searchCreateDesig(stateCode);
                    }
                    else if (data.success==false) {
                        if (data.message != "") {                            
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');

                            if ($('#stateCode').val() > 0) {
                                $("#ddlStateNames").attr("disabled", true);
                            }
                        }
                    }
                    else {
                        $("#dvAutonomousBodyDetails").html(data);
                        if ($('#stateCode').val() > 0) {
                            $("#ddlStateNames").attr("disabled", true);
                        }
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {                    
                    if ($('#stateCode').val() > 0) {
                        $("#ddlStateNames").attr("disabled", true);
                    }
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            });
        }
    });
    

    $('#btnUpdate').click(function (e) {

        if ($('#frmAutonomousBody').valid()) {
            var stateCode = $("#ddlStateNames").val();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/EditMasterAdminAutonomousBody/",
                type: "POST",
           
                data: $("#frmAutonomousBody").serialize(),
                success: function (data) {

                    if (data.success==true) {
                        alert(data.message);
                        //$('#tblAutonomousBodyList').trigger('reloadGrid');
                        //$("#dvAutonomousBodyDetails").load("/Master/AddEditMasterAdminAutonomousBody");
                        if ($("#dvAutonomousBodyDetails").is(":visible")) {
                            $('#dvAutonomousBodyDetails').hide('slow');

                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();

                        }
                        if (!$("#dvSearchAutonomousBody").is(":visible")) {
                            $("#dvSearchAutonomousBody").show('slow');
                        }
                        searchCreateDesig(stateCode);
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvAutonomousBodyDetails").html(data);
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
        //    url: "/Master/AddEditMasterAdminAutonomousBody",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {                
        //        $("#dvAutonomousBodyDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }
        //});
        if ($("#dvAutonomousBodyDetails").is(":visible")) {
            $('#dvAutonomousBodyDetails').hide('slow');

            $('#btnSearchView').hide();
            $('#btnCreateNew').show();

        }
        if (!$("#dvSearchAutonomousBody").is(":visible")) {
            $("#dvSearchAutonomousBody").show('slow');
        }
       
    });

   
    $("#dvhdCreateNewAutonomousBodyDetails").click(function () {

        if ($("#dvCreateNewAutonomousBodyDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");


            $(this).next("#dvCreateNewAutonomousBodyDetails").slideToggle(300);
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvCreateNewAutonomousBodyDetails").slideToggle(300);
        }
    });

    //$("#spCollapseIconCN").click(function () {

    //    if ($("#dvAutonomousBodyDetails").is(":visible")) {
    //        $("#dvAutonomousBodyDetails").hide("slow");
    //        $('#tblAddNewButton').attr('style', 'padding-top:0em; width:100%;');
    //        $("#btnCreateNew").show();
    //    }
    //});

    $("#ddlStateNames").focus(function () {
        $('#message').html(data.message);
        $('#dvErrorMessage').show('slow');
    });

    $("#ADMIN_AUTONOMOUS_BODY1").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });


    $('#btnReset').click(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');

    });

});
function searchCreateDesig(stateCode) {
    $('#ddlSearchStates').val(stateCode);
    $('#tblAutonomousBodyList').setGridParam({
        url: '/Master/GetMasterAdminAutonomousBodyList', datatype: 'json'
    });
    $('#tblAutonomousBodyList').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlSearchStates option:selected').val() } });
    $('#tblAutonomousBodyList').trigger("reloadGrid", [{ page: 1 }]);
}