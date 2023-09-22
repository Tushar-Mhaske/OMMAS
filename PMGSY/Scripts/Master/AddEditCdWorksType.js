$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmAddCdWorks');


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });


    $("#btnSave").click(function (e) {

        if ($("#frmAddCdWorks").valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                type: 'POST',
                url: '/Master/AddCdWorksType/',
                async: false,
                data: $("#frmAddCdWorks").serialize(),
                success: function (data) {
                    if (data.success==true) {
                    
                        alert(data.message);
                        //ClearDetails();
                        //$('#cdWorksType').trigger('reloadGrid');
                        $("#btnCreateNew").show();
                        $("#cdWorksDetails").hide('slow');
                        $('#cdWorksType').trigger('reloadGrid');
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
                        $("#cdWorksDetails").html(data);
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

    $("#btnReset").click(function () {
        ClearDetails();

    });

    //$("#dvhdAddNewCdWorksDetails").click(function () {

    //    if ($("#dvAddNewCdWorksDetails").is(":visible")) {

    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

    //        $(this).next("#dvAddNewCdWorksDetails").slideToggle(300);
    //    }

    //    else {
    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

    //        $(this).next("#dvAddNewCdWorksDetails").slideToggle(300);
    //    }
    //});

    $("#spCollapseIconCN").click(function () {

        if ($("#cdWorksDetails").is(":visible")) {
            $("#cdWorksDetails").hide("slow");

            $("#btnCreateNew").show();
        }
    });
    $("#btnCancel").click(function (e) {

        //$.ajax({
        //    url: "/Master/AddCdWorksType",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
           
        //        $("#cdWorksDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }
        //});
        $("#btnCreateNew").show();
        $("#cdWorksDetails").hide('slow');       

    })

    $("#MAST_CDWORKS_NAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#btnUpdate").click(function (e) {

        if ($("#frmAddCdWorks").valid()) {
             $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
             $.ajax({
                 type: 'POST',
                 url: '/Master/EditCdWorksType/',
                 async: false,
                 data: $("#frmAddCdWorks").serialize(),
                 success: function (data) {
                     if (data.success==true) {
                         alert(data.message);
                     
                         //$("#cdWorksDetails").load("/Master/AddCdWorksType");
                         //$('#cdWorksType').trigger('reloadGrid');
                         $("#btnCreateNew").show();
                         $("#cdWorksDetails").hide('slow');
                         $('#cdWorksType').trigger('reloadGrid');
                     }
                     else if (data.success==false) {
                         if (data.message != "") {
                             $('#message').html(data.message);
                             $('#dvErrorMessage').show('slow');
                         }
                     }
                     else {
                         $("#cdWorksDetails").html(data);
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
    $('#MAST_CDWORKS_NAME').val('');
    $('#MAST_CDWORKS_CODE').val('');
  

    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}