$(document).ready(function () {
$.validator.unobtrusive.parse("#frmMasterReason");
$("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
});

$('#btnSave').click(function (e) {

        if ($('#frmMasterReason').valid()) {
            var reasonCode = $("#ddlReasonType option:selected").val();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/AddMasterReason/",
                type: "POST",
           
                data: $("#frmMasterReason").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                        //ClearReasonDetails();
                        //SearchCreateReason(reasonCode);
                        if ($("#dvReasonDetails").is(":visible")) {
                            $('#dvReasonDetails').hide('slow');

                            $('#btnSearchView').hide();
                            $('#btnAddReason').show();
                        }

                        if (!$("#dvSearchReason").is(":visible")) {
                            $("#dvSearchReason").show('slow');
                        }
                        SearchCreateReason(reasonCode);
                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvReasonDetails").html(data);
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
    if ($('#frmMasterReason').valid()) {
        var reasonCode = $("#ddlReasonType option:selected").val();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/EditMasterReason/",
                type: "POST",
               data: $("#frmMasterReason").serialize(),
                success: function (data) {
                 if (data.success==true) {
                        alert(data.message);
                       // $('#tblMasterReasonList').trigger('reloadGrid');
                     // $("#dvReasonDetails").load("/Master/AddEditMasterReason");
                        if ($("#dvReasonDetails").is(":visible")) {
                            $('#dvReasonDetails').hide('slow');
                            $('#btnSearchView').hide();
                            $('#btnAddReason').show();
                        }

                        if (!$("#dvSearchReason").is(":visible")) {
                            $("#dvSearchReason").show('slow');
                        }
                        SearchCreateReason(reasonCode);
                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvReasonDetails").html(data);
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
    //        url: "/Master/AddEditMasterReason",
    //        type: "GET",
    //        dataType: "html",
    //        success: function (data) {
    //            $("#dvReasonDetails").html(data);
    //        },
    //        error: function (xhr, ajaxOptions, thrownError) {
    //            alert(xhr.responseText);
    //        }
        //    });
        if ($("#dvReasonDetails").is(":visible")) {
            $('#dvReasonDetails').hide('slow');
            $('#btnSearchView').hide();
            $('#btnAddReason').show();
        }

        if (!$("#dvSearchReason").is(":visible")) {
            $("#dvSearchReason").show('slow');
        }
    });

 $('#btnReset').click(function () {
        ClearReasonDetails();
 });

$("#dvhdCreateNewReasonDetails").click(function () {

        if ($("#dvCreateNewReasonDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvCreateNewReasonDetails").slideToggle(300);
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvCreateNewReasonDetails").slideToggle(300);
        }
    });

    $("#MAST_REASON_NAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
    $("#MAST_REASON_TYPE").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

});

function ClearReasonDetails() {
    $('#MAST_REASON_NAME').val('');
    $("#ddlReasonType").val("");
    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}

function SearchCreateReason(reasonCode) {
    $('#ReasonType').val(reasonCode);
$('#tblMasterReasonList').setGridParam({
        url: '/Master/GetMasterReasonList', datatype: 'json'
    });
$('#tblMasterReasonList').jqGrid("setGridParam", { "postData": { ReasonType: $('#ReasonType option:selected').val() } });
    $('#tblMasterReasonList').trigger("reloadGrid", [{ page: 1 }]);
}