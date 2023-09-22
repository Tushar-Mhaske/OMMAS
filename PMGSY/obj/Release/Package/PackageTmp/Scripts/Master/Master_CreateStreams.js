$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmStreams");

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    //This method is fgor reset button click.
    $("#btnReset").click(function () {
        $("#dvErrorMessage").hide('slow');
    });

    $("#MAST_STREAM_NAME").focus(function () {
        $("#dvErrorMessage").hide(1000);
        $("#message").html('');
    });

    $("#ddlStreamType").focus(function () {
        $("#dvErrorMessage").hide(1000);
        $("#message").html('');

    });
    

    //This method is for cancel button click.
    $("#btnCancel").click(function () {
        // $("#dvDetails").load('/Master/AddtMasterStreams');
        if ($("#dvDetails").is(":visible")) {
            $('#dvDetails').hide('slow');
            $('#btnSearchView').hide();
            $('#btnCreateNew').show();

        }
        if (!$("#dvSearchStreams").is(":visible")) {
            $("#dvSearchStreams").show('slow')
        }
      
    });

    //This method is for maximising/minimising the list/form.
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
        $("#ddlError").show();
        if ($("#frmStreams").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            var streamCode = $("#ddlStreamType option:selected").val();
            $.ajax({
                url: "/Master/AddMasterStreams",
                type: "POST",
                data: $("#frmStreams").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                        //SearchCreateStreamDetails(streamCode);
                        //$("#btnReset").trigger('click');
                        if ($("#dvDetails").is(":visible")) {
                            $('#dvDetails').hide('slow');
                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();

                        }
                        if (!$("#dvSearchStreams").is(":visible")) {
                            $("#dvSearchStreams").show('slow')
                        }
                        SearchCreateStreamDetails(streamCode);
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
                    $.unblockUI();
                }
            });
        }
    });

    //This method is for update button click.
    $("#btnUpdate").click(function () {
        if ($('#frmStreams').valid()) {
            var streamCode = $("#ddlStreamType option:selected").val();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/EditMasterStreams",
                type: "POST",
                data: $("#frmStreams").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                       //$("#dvErrorMessage").hide(1000);
                        //$("#tblList").trigger('reloadGrid');
                        //$("#dvDetails").load('/Master/AddtMasterStreams');

                        if ($("#dvDetails").is(":visible")) {
                            $('#dvDetails').hide('slow');
                            $('#btnSearchView').hide();
                            $('#btnCreateNew').show();

                        }
                        if (!$("#dvSearchStreams").is(":visible")) {
                            $("#dvSearchStreams").show('slow')
                        }
                        SearchCreateStreamDetails(streamCode);

                    }
                    else if (data.success==false) {
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
                    $.unblockUI();
                }
            });
        }
    });
   
});

function SearchCreateStreamDetails(streamCode) {
    $('#StreamType').val(streamCode);
    $('#tblList').setGridParam({
        url: '/Master/GetMasterStreamsList', datatype: 'json'
    });
    $('#tblList').jqGrid("setGridParam", { "postData": { StreamType: $('#StreamType option:selected').val() } });
    $('#tblList').trigger("reloadGrid", [{ page: 1 }]);

}
