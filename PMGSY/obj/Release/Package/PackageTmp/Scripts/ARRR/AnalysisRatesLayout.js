$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmAnalysisRates');

    if ($('#User_Action').val() == "A") {
        $('#txtQuantity').val('');
        $('#txtRate').val('');

    }

    $("#txtQuantity").blur(function () {
        var q = parseFloat($('#txtQuantity').val());
        var a = parseFloat($('#txtRate').val());

        if (q > 0 && a > 0) {
            $('#txtAmount').show('slow');
            $('#txtAmount').val(parseFloat(q * a).toFixed(2));
        }
        else {
            $('#lblAmountVal').hide('slow');
        }
    });

    $('#txtDate, #Date').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose date',
        buttonImageOnly: true,
        changeMonth: true, //for month selection
        changeYear: true, //for year selection
        buttonText: "select date",

        //minDate: $('#Date').val(),
        //maxDate: new Date(year, 11, 31),
        onSelect: function (selectedDate) {

        },
        onClose: function () {
            $(this).focus().blur();
        }
    });

    $("#ddlChapter").change(function () {

        $("#ddlItem").empty();

        $("#ddlMajorItem").empty();
        $("#ddlMajorItem").append("<option value='0'>All</option>");

        $("#ddlMinorItem").empty();
        $("#ddlMinorItem").append("<option value='0'>All</option>");

        //alert($('#ItemType').val());
        $.ajax({
            url: '/ARRR/ItemDetails/',
            type: 'POST',
            data: { "headCode": $("#ddlChapter").val() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    $("#ddlItem").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    });

    $("#ddlMajorItem").change(function () {
        if ($('input[id=rdbMinorItem]').attr("checked") == 'checked') {
            $("#ddlMinorItem").empty();
            //alert($('#ItemType').val());
            //if ($("#DistrictList_CommitmentDetails").length > 0) {
            $.ajax({
                url: '/ARRR/MinorItemDetails/',
                type: 'POST',
                data: { "ItemCode": $("#ddlMajorItem").val() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlMinorItem").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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


    $("#rdbItem").click(function () {
        $('.item').show('slow');
        $(".maj").hide('slow');
        $(".min").hide('slow');

        //$("#ddlItem").empty();
        //$("#ddlItem").append("<option value='0'>All</option>");
        $("#ddlMajorItem").empty();
        $("#ddlMajorItem").append("<option value='0'>All</option>");
    });

    $("#rdbItem").trigger('click');

    $("#rdbMajorItem").click(function () {
        $(".maj").show('slow');
        $(".min").hide('slow');

        if ($("#ddlItem").val() > 0) {
            $("#ddlItem").trigger('change');
        }
            //$("#ddlChapter").trigger('change');
        else {
            $("#ddlMajorItem").empty();
            $("#ddlMajorItem").append("<option value='0'>All</option>");
        }
    });

    $("#rdbMinorItem").click(function () {
        $(".maj").show('slow');
        $(".min").show('slow');

        if ($("#ddlMajorItem").val() > 0) {
            $("#ddlMajorItem").trigger('change');
        }
        //$("#ddlChapter").trigger('change');

        $("#ddlItem").trigger('change');
    });

    $("#idFilterDiv").click(function () {
        $("#idFilterDiv").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
        $("#frmAnalysisRates").toggle("slow");
    });


    $("#ddlItem").change(function () {
        if ($('input[id=rdbMajorItem]').attr("checked") == 'checked' || $('input[id=rdbMinorItem]').attr("checked") == 'checked') {
            $("#ddlMajorItem").empty();
            //alert($('#ItemType').val());
            //if ($("#DistrictList_CommitmentDetails").length > 0) {
            $.ajax({
                url: '/ARRR/MajorItemDetails/',
                type: 'POST',
                data: { "ItemCode": $("#ddlItem").val() },
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
        }
        //}
    });

    $("#rdbLabour").click(function () {
        $('#lbllmmType').text('Labour');
        $('#txtRate').val('');

        //if ($('input[id=rdbMinorItem]').attr("checked") == 'checked') {
        $("#ddlLMM").empty();
        $.ajax({
            url: '/ARRR/lmmTypeDetails/',
            type: 'POST',
            data: { "lmmType": 1 },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    $("#ddlLMM").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
        //}
    });


    $("#rdbMachinery").click(function () {
        $('#lbllmmType').text('Machinery');
        $('#txtRate').val('');

        //if ($('input[id=rdbMinorItem]').attr("checked") == 'checked') {
        $("#ddlLMM").empty();
        $.ajax({
            url: '/ARRR/lmmTypeDetails/',
            type: 'POST',
            data: { "lmmType": 2 },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    $("#ddlLMM").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
        //}
    });
    $("#rdbMaterial").click(function () {
        $('#lbllmmType').text('Material');
        $('#txtRate').val('');

        //if ($('input[id=rdbMinorItem]').attr("checked") == 'checked') {
        $("#ddlLMM").empty();
        $.ajax({
            url: '/ARRR/lmmTypeDetails/',
            type: 'POST',
            data: { "lmmType": 3 },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    $("#ddlLMM").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
        //}
    });

    $('#ddlLMM').change(function () {
        $.ajax({
            url: '/ARRR/GetRateDetails/',
            type: 'POST',
            data: { "typeCode": $('#ddlLMM').val() },
            success: function (jsonData) {
                if (jsonData.success) {
                    $('#txtRate').val(jsonData.rate);
                    $('#rateCode').val(jsonData.Code);
                    //alert(jsonData.Code);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    });

    $("#btnSave").click(function () {
        if ($('#frmAnalysisRates').valid()) {
            $('#User_Action').val('A');
            $.ajax({
                url: '/ARRR/AddEditAnalysisRatesDetails/',
                async: false,
                type: 'POST',
                //data: form_data,
                data: $("#frmAnalysisRates").serialize(),
                //contentType: false,
                //processData: false,
                success: function (data) {
                    alert(data.message);
                    if (data.success == true) {

                        $("#btnCancel").trigger('click');
                        LoadAnalysisRatesGrid();
                        $('#dvLoadAnalysisRates').hide('slow');
                        $("#btnAdd").show('slow');
                    }
                }
            })
        }
    });

    $('#btnUpdate').click(function () {
        if ($('#frmAnalysisRates').valid()) {
            $('#User_Action').val('E');
            $.ajax({
                url: '/ARRR/AddEditAnalysisRatesDetails/',
                async: false,
                type: 'POST',
                //data: form_data,
                data: $("#frmAnalysisRates").serialize(),
                success: function (data) {
                    alert(data.message);
                    if (data.success == true) {

                        $("#btnCancel").trigger('click');
                        LoadAnalysisRatesGrid();
                        $('#dvLoadAnalysisRates').hide('slow');
                        $("#btnAdd").show('slow');
                    }
                }
            })
        }
    })

    $("#btnCancel").click(function () {
        $('#dvLoadAnalysisRates').hide('slow');
        $("#btnAdd").show('slow');
    });

    $("#btnReset").click(function () {
        $("#rdbItem").trigger('click');

        $('#lblAmountVal').text('');
        $('#lblAmountVal').hide('slow');
    });
});

