
//jQuery.validator.addMethod("checkmastcontstatus", function (value, element, param) {

//    if ($("#rdoExpired").is(":checked")) {

//        if (($("#MAST_CON_LEGAL_HEIR_FNAME").val() == "") && ($("#MAST_CON_LEGAL_HEIR_MNAME").val() == "") && ($("#MAST_CON_LEGAL_HEIR_LNAME").val() == "")) {
//            return false;
//        }
//        else {
//            return true;
//        }
//    }
//    else {
//        return true;
//    }
//});

//jQuery.validator.unobtrusive.adapters.addBool("checkmastcontstatus");

$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmAddCdWorksConstruction');

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#btnSave").click(function (e) {

        if ($("#frmAddCdWorksConstruction").valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                type: 'POST',
                url: '/Master/AddConstructionType/',
                async: false,
                data: $("#frmAddCdWorksConstruction").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                        //ClearDetails();
                        //$('#constructionCategory').trigger('reloadGrid');
                        $("#btnCreateNew").show('slow');
                        $('#constructionDetails').hide('slow');
                        $('#constructionCategory').trigger('reloadGrid');
                        $.unblockUI();
                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                            $.unblockUI();
                        }
                       
                    }
                    else {
                        $("#constructionDetails").html(data);
                    }

                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            })
        }
    });

    $("#MAST_CDWORKS_NAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#btnReset").click(function () {
        ClearDetails();

    });
    
    //$("#dvhdAddNewConstructionDetails").click(function () {

    //    if ($("#dvAddNewConstructionDetails").is(":visible")) {

    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

    //        $(this).next("#dvAddNewConstructionDetails").slideToggle(300);
    //    }

    //    else {
    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

    //        $(this).next("#dvAddNewConstructionDetails").slideToggle(300);
    //    }
    //});

    $("#spCollapseIconCN").click(function () {

        if ($("#constructionDetails").is(":visible")) {
            $("#constructionDetails").hide("slow");

            $("#btnCreateNew").show();
        }
    });

    $("#btnCancel").click(function (e) {

        //$.ajax({
        //    url: "/Master/AddConstructionType",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
           
        //        $("#constructionDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }

        //});
        $("#btnCreateNew").show('slow');
        $('#constructionDetails').hide('slow');       

    })

    $("#btnUpdate").click(function (e) {
        if ($("#frmAddCdWorksConstruction").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/EditConstructionType/',
                async: false,
                data: $("#frmAddCdWorksConstruction").serialize(),
                success: function (data) {
                    if (data.success) {
                       alert(data.message);
                 
                        //$("#constructionDetails").load("/Master/AddConstructionType");
                        //$('#constructionCategory').trigger('reloadGrid');

                       $("#btnCreateNew").show('slow');
                       $('#constructionDetails').hide('slow');
                       $('#constructionCategory').trigger('reloadGrid');
                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#constructionDetails").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            })
        }
    });
});

function ClearDetails() {
    $('#MAST_CDWORKS_CODE').val('');
    $('#MAST_CDWORKS_NAME').val('');

    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}