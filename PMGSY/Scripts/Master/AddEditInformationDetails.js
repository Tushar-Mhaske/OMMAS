jQuery.validator.addMethod("infostatevalidator", function (value, element, param) {
    var infoType = $("#InfoType").val();
    var infoState = $("#ddlState").val();
    if(infoType=="T" && infoState==0)
    {
        return false;
    }else{
        return true;
    }
});
jQuery.validator.unobtrusive.adapters.addBool("infostatevalidator");

$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmInfoDetails");

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#iconClose").click(function () {
        if ($("#dvAddInfoDetails").is(':visible')) {
            $("#dvAddInfoDetails").hide('slow');
            $("#btnCreateNew").show('slow');
            $("#trSearchDetails").hide();
            $("#dvSearchInfoDetails").show();

        }
    });

    $("#btnSaveInfoDetails").click(function (e) {

        if ($("#frmInfoDetails").valid()) {
            var stateCode = $('#ddlState').val();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/AddInfoDetails/',
                async: false,
                data: $("#frmInfoDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        $.unblockUI();
                        alert(data.message);
                      
                        $("#dvSearchInfoDetails").show();
                        $("#dvAddInfoDetails").hide('slow');
                        $("#btnCreateNew").show();
                        $("#trSearchDetails").hide();
                        //$("#dvAddInfoDetails").load('/Master/AddEditInfoDetails?id=' + $("#InfoType").val() + '');
                        if ($("#dvSearchInfoDetails").is(':visible')) {
                            searchCreateInfoDetails(stateCode);
                        }
                        else {
                            $('#tblInfoDetails').trigger('reloadGrid');
                        }
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                            $.unblockUI();
                        }
                    }
                    else {
                        $("#dvAddInfoDetails").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblockUI();
                    alert("Error occured while processing your request.");
                    return false;
                }
            })
        }
    });

    $('#btnUpdateInfoDetails').click(function (e) {

        if ($('#frmInfoDetails').valid()) {
            var stateCode = $('#ddlState').val();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/Master/EditInfoDetails/",
                type: "POST",
                data: $("#frmInfoDetails").serialize(),
                success: function (data) {
                    $.unblockUI();
                    if (data.success == true) {
                        alert(data.message);
                        //$('#tblInfoDetails').trigger('reloadGrid');                       
                        //$("#dvAddInfoDetails").load('/Master/AddEditInfoDetails?id=' + $("#InfoType").val() + '');
                        $("#dvSearchInfoDetails").show();
                        $("#dvAddInfoDetails").hide('slow');
                        $("#btnCreateNew").show();
                        $("#trSearchDetails").hide();
                        if ($("#dvSearchInfoDetails").is(':visible')) {
                            searchCreateInfoDetails(stateCode);
                        }
                        else {
                            $('#tblInfoDetails').trigger('reloadGrid');
                        }
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvAddInfoDetails").html(data);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblockUI();
                    alert("Error occured while processing your request.");
                    return false;
                }
            });
        }
    });

    //Clear form details
    $("#btnResetInfoDetails").click(function () {
        $("#dvErrorMessage").hide('slow');
    });
    
    $('#btnCancelInfoDetails').click(function (e) {
        //$.ajax({
        //    url: "/Master/AddEditInfoDetails?id=" + $("#InfoType").val(),
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        $("#dvAddInfoDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert("Error occured while processing your request.");
        //        return false;
        //    }
        //});
        $("#dvSearchInfoDetails").show();
        $("#dvAddInfoDetails").hide('slow');
        $("#btnCreateNew").show();
        $("#trSearchDetails").hide();
    });

    
});

function searchCreateInfoDetails(stateCode) {
    $("#ddlAllState").val(stateCode);
    $('#tblInfoDetails').setGridParam({
        url: '/Master/InfoDetailsList', datatype: 'json'
    });
    $('#tblInfoDetails').jqGrid("setGridParam", { "postData": { StateCode: $("#ddlAllState").val(), Type: $("#InfoType").val(), } });
    $('#tblInfoDetails').trigger("reloadGrid", [{ page: 1 }]);
}