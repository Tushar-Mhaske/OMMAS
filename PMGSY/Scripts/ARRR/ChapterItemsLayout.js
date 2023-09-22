$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmChapterItems');

    $("#rdbItem").click(function () {
        $('.item').show('slow');

        $('.Majitem').hide('slow');
        $('.Minitem').hide('slow');
        $('.MinMajitem').hide('slow');
        if ($("#User_Action").val() != 'E') {
            
            //$("#ddlChapter").trigger('change');
            $("#ddlItem").empty();
            $("#ddlItem").append("<option value='0'>All</option>");
            $("#ddlMajorItem").empty();
            $("#ddlMajorItem").append("<option value='0'>All</option>");
        }
    });
    //if ($("#User_Action").val() != 'E') {
    $("#rdbItem").trigger('click');
    //}

    $("#rdbMajorItem").click(function () {

        $('.Majitem').show('slow');
        $('.MinMajitem').show('slow');

        $('.item').hide('slow');
        $('.Minitem').hide('slow');
        if ($("#User_Action").val() != 'E') {
            
            //$("#ddlItem").trigger('change');
            $("#ddlChapter").trigger('change');
            $("#ddlMajorItem").empty();
            $("#ddlMajorItem").append("<option value='0'>All</option>");
        }
    });

    $("#rdbMinorItem").click(function () {
        $('.Minitem').show('slow');
        $('.MinMajitem').show('slow');

        $('.item').hide('slow');
        $('.Majitem').hide('slow');

        $("#ddlItem").trigger('change');
    });

    $("#idFilterDiv").click(function () {
        $("#idFilterDiv").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
        $("#frmChapterItems").toggle("slow");
    });

    $("#ddlChapter").change(function () {
        if (!($('input[id=rdbItem]').attr("checked") == 'checked')) {
            $("#ddlItem").empty();
            //alert($('#ItemType').val());
            //if ($("#DistrictList_CommitmentDetails").length > 0) {
            $.ajax({
                url: '/ARRR/ItemDetails/',
                type: 'POST',
                data: { "headCode": $("#ddlChapter").val() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlItem").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    ////For Disable if District Login
                    //if ($("#Mast_District_Code").val() > 0) {
                    //    $("#DistrictList_CommitmentDetails").val($("#Mast_District_Code").val());
                    //    // $("#DistrictList_CommitmentDetails").attr("disabled", "disabled");
                    //    $("#DistrictList_CommitmentDetails").trigger('change');
                    //}
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
            //}
        }
    });

    $("#ddlItem").change(function () {
        if ($('input[id=rdbMinorItem]').attr("checked") == 'checked') {
            $("#ddlMajorItem").empty();
            //alert($('#ItemType').val());
            //if ($("#DistrictList_CommitmentDetails").length > 0) {
            $.ajax({
                url: '/ARRR/MajorItemDetails/',
                type: 'POST',
                data: { "ItemCode": $("#ddlItem").val() },
                //data: { "headCode": $("#ddlChapter").val() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlMajorItem").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
            //}
        }
    });

    $("#btnSave").click(function () {
        if ($('#frmChapterItems').valid()) {
            $('#User_Action').val('A');
            $.ajax({
                url: '/ARRR/AddEditChapterItemDetails/',
                async: false,
                type: 'POST',
                //data: form_data,
                data: $("#frmChapterItems").serialize(),
                //contentType: false,
                //processData: false,
                success: function (data) {
                    alert(data.message);
                    if (data.success == true) {

                        $("#btnCancel").trigger('click');
                        LoadChapterGrid();
                        $('#dvLoadChapterItems').hide('slow');
                        $("#btnAdd").show('slow');
                    }

                }
            })
        }
    });

    $('#btnUpdate').click(function () {
        if ($('#frmChapterItems').valid()) {
            $('#User_Action').val('E');
            $.ajax({
                url: '/ARRR/AddEditChapterItemDetails/',
                async: false,
                type: 'POST',
                //data: form_data,
                data: $("#frmChapterItems").serialize(),
                success: function (data) {
                    alert(data.message);
                    if (data.success == true) {

                        $("#btnCancel").trigger('click');
                        LoadChapterGrid();
                        $('#dvLoadChapterItems').hide('slow');
                        $("#btnAdd").show('slow');
                    }
                }
            })
        }
    })

    $("#btnCancel").click(function () {
        $('#dvChapterItems').hide('slow');
        $("#btnAdd").show('slow');
    });

    $("#btnReset").click(function () {
        $("#rdbItem").trigger('click');
    });
});