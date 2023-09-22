
$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmMasterChecklist");

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

      $('#btnSave').click(function (e) {

        if ($('#frmMasterChecklist').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({    
                url: "/Master/AddMasterChecklist/",
                type: "POST",
             
                data: $("#frmMasterChecklist").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);

                        //ClearChecklistDetails();
                        //$('#tblMasterChecklistList').trigger('reloadGrid');

                        $("#dvChecklistDetails").hide('slow');
                        $("#btnCreateNew").show('slow');
                        $('#tblMasterChecklistList').trigger('reloadGrid');
                      }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvChecklistDetails").html(data);
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

        if ($('#frmMasterChecklist').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/EditMasterChecklist/",
                type: "POST",
           
                data: $("#frmMasterChecklist").serialize(),
                success: function (data) {

                    if (data.success==true) {
                        alert(data.message);
                        //$('#tblMasterChecklistList').trigger('reloadGrid');
                        //$("#dvChecklistDetails").load("/Master/AddEditMasterChecklist");

                        $("#dvChecklistDetails").hide('slow');
                        $("#btnCreateNew").show('slow');
                        $('#tblMasterChecklistList').trigger('reloadGrid');
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvChecklistDetails").html(data);
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

          if ($("#dvChecklistDetails").is(":visible")) {
              $("#dvChecklistDetails").hide("slow");
             
              $("#btnCreateNew").show();
          }
      });
    $('#btnCancel').click(function (e) {

        //$.ajax({
        //    url: "/Master/AddEditMasterChecklist",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        $("#dvChecklistDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }
        //});

        $("#dvChecklistDetails").hide('slow');
        $("#btnCreateNew").show('slow');
       
    });

    $('#btnReset').click(function () {
        ClearChecklistDetails();
    });

     $("#dvhdCreateNewChecklistDetails").click(function () {

        if ($("#dvCreateNewChecklistDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

         
            $(this).next("#dvCreateNewChecklistDetails").slideToggle(300);
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvCreateNewChecklistDetails").slideToggle(300);
        }
    });

    $("#MAST_CHECKLIST_ISSUES").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

});

function ClearChecklistDetails() {
    $('#MAST_CHECKLIST_ISSUES').val('');
    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}