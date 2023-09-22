$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmAddDesignation');


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });


    $("#btnSave").click(function (e) {

        var desigCode = $("#ddlDesigType").val();

        if ($("#frmAddDesignation").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                type: 'POST',
                url: '/Master/AddDesignation/',
                async: false,
                data: $("#frmAddDesignation").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                       // ClearDetails();
                        if ($("#desigAddDetails").is(":visible")) {
                            $('#desigAddDetails').hide('slow');        
                            $('#btnSearch').hide();
                            $('#btnAdd').show();
                        }

                        if (!$("#desigSearchDetails").is(":visible")) {
                            $("#desigSearchDetails").show('slow');
                        }
                        SearchDesignationCreateDetail(desigCode);

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
                        $("#desigAddDetails").html(data);
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

    $("#MAST_DESIG_NAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#MAST_DESIG_TYPE").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#dvhdAddNewDesigDetails").click(function () {

        if ($("#dvAddNewDesigDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvAddNewDesigDetails").slideToggle(300);
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvAddNewDesigDetails").slideToggle(300);
        }
    });


    $("#btnCancel").click(function (e) {

        //$.ajax({
        //    url: "/Master/AddDesignation",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
             
        //        $("#desigAddDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }
        //});
        if ($("#desigAddDetails").is(":visible")) {
            $('#desigAddDetails').hide('slow');
            $('#btnSearch').hide();
            $('#btnAdd').show();
        }

        if (!$("#desigSearchDetails").is(":visible")) {
            $("#desigSearchDetails").show('slow');
        }
       

    })

    $("#btnReset").click(function (e) {

        ClearDetails();
    });

    $("#btnUpdate").click(function (e) {

        if ($("#frmAddDesignation").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            var desigCode = $("#ddlDesigType").val();
            $.ajax({
                type: 'POST',
                url: '/Master/EditDesignation/',
                async: false,
                data: $("#frmAddDesignation").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                        //SearchDesignationCreateDetail(desigCode);
                        //$('#desigAddDetails').load("/Master/AddDesignation");
                        //$('#desigCategory').trigger('reloadGrid');
                        if ($("#desigAddDetails").is(":visible")) {
                            $('#desigAddDetails').hide('slow');
                            $('#btnSearch').hide();
                            $('#btnAdd').show();
                        }

                        if (!$("#desigSearchDetails").is(":visible")) {
                            $("#desigSearchDetails").show('slow');
                        }
                        SearchDesignationCreateDetail(desigCode);

                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#desigAddDetails").html(data);
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

function SearchDesignationCreateDetail(desigCode) {
    
    $("#Designation").val(desigCode);
    $('#desigCategory').setGridParam({
        url: '/Master/GetDesignationList'
    });
    $('#desigCategory').jqGrid("setGridParam", { "postData": { desigCode: $("#Designation").val() } });
    $('#desigCategory').trigger("reloadGrid", [{ page: 1 }]);


}

function ClearDetails() {
    $('#MAST_DESIG_NAME').val('');
    $('#MAST_DESIG_CODE').val('');
    $('#ddlDesigType').val("");
    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}