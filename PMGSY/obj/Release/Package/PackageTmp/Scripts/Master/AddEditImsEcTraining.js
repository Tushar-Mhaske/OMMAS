
$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmAddImsEcTraining');
    $("#dvhdAddNewPIUDetails").click(function () {

        if ($("#dvAddNewImsEcTrainingDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvAddNewImsEcTrainingDetails").slideToggle(300);
        }
        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvAddNewImsEcTrainingDetails").slideToggle(300);
        }
    });


    $("#btnSave").click(function (e) {

        if ($("#frmAddImsEcTraining").valid()) {

            $("#ddlState").attr("disabled", false);

            var stateCode = $("#ddlState option:selected").val();
            var year = $("#ddlPhaseYear option:selected").val();
            var batch = $("#ddlBatch option:selected").val();
            var designation = $("#ddlDesignation option:selected").val();

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                type: 'POST',
                url: '/Master/AddEditImsEcTraining/',
                async: false,
                data: $("#frmAddImsEcTraining").serialize(),
                success: function (data) {

                    if (data.success == true) {
                        alert(data.message);
                        //ClearDetails();
                        if ($("#ImsEcTrainingAddDetails").is(":visible")) {
                            $('#ImsEcTrainingAddDetails').hide('slow');
                            $('#btnSearch').hide();
                            $('#btnAdd').show();
                        }
                        if (!$("#ImsEcTrainingSearchDetails").is(":visible")) {
                            $("#ImsEcTrainingSearchDetails").show('slow');
                        }
                        SearchDetails(stateCode, designation, year);
                        // $("#ddlState").attr("disabled", true);
                    }
                    else if (data.success == false) {

                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                        if ($('#hdStatCode').val() > 0) {
                            $("#ddlState").val($('#hdStatCode').val());
                            $("#ddlState").attr("disabled", true);
                        }
                    }
                    else {
                        $("#ImsEcTrainingAddDetails").html(data);
                        //if ($('#stateCode').val() > 0) {
                        //    $("#ddlState").val($('#stateCode').val());
                        //    $("#ddlState").attr("disabled", true);
                        //}
                    }

                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                    if ($('#hdStatCode').val() > 0) {
                        $("#ddlState").val($('#hdStatCode').val());
                        $("#ddlState").attr("disabled", true);
                    }
                }
            })
        }



    });

    $("#btnCancel").click(function (e) {

        //$.ajax({
        //    url: "/Master/AddEditImsEcTraining",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        $("#ImsEcTrainingAddDetails").html(data);
        //        $("#ImsEcTrainingAddDetails").show();
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }

        //});
        if ($("#ImsEcTrainingAddDetails").is(":visible")) {
            $('#ImsEcTrainingAddDetails').hide('slow');
            $('#btnSearch').hide();
            $('#btnAdd').show();
        }
        if (!$("#ImsEcTrainingSearchDetails").is(":visible")) {
            $("#ImsEcTrainingSearchDetails").show('slow');
        }
       

    })

    $("#btnReset").click(function (e) {
        //Added By Abhishek kamble 20-Feb-2014
        $("input,select").removeClass("input-validation-error");
        $('.field-validation-error').html('');

        e.preventDefault();
        ClearDetails();
        //if ($('#stateCode').val() > 0) {

        //    $("#ddlState").val($('#stateCode').val());
        //    $("#ddlState").attr("disabled", true);
        //    $("#ddlState").trigger('change');

        //}
    });

    $("#btnUpdate").click(function (e) {


        if ($("#frmAddImsEcTraining").valid()) {

            $("#ddlState").attr("disabled", false);
            $("#ddlPhaseYear").attr("disabled", false);
            $("#ddlBatch").attr("disabled", false);
            $("#ddlDesignation").attr("disabled", false);
            var stateCode = $("#ddlState option:selected").val();
            var year = $("#ddlPhaseYear option:selected").val();
            var batch = $("#ddlBatch option:selected").val();
            var designation = $("#ddlDesignation option:selected").val();

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/EditImsEcTraining/',
                async: false,
                data: $("#frmAddImsEcTraining").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);

                        //$("#ImsEcTrainingAddDetails").load("/Master/AddEditImsEcTraining");

                        //$('#tblImsEcTraining').trigger('reloadGrid');
                        if ($("#ImsEcTrainingAddDetails").is(":visible")) {
                            $('#ImsEcTrainingAddDetails').hide('slow');
                            $('#btnSearch').hide();
                            $('#btnAdd').show();
                        }
                        if (!$("#ImsEcTrainingSearchDetails").is(":visible")) {
                            $("#ImsEcTrainingSearchDetails").show('slow');
                        }
                        SearchDetails(stateCode, designation, year);
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                            $("#ddlState").attr("disabled", true);
                            $("#ddlPhaseYear").attr("disabled", true);                          
                            $("#ddlDesignation").attr("disabled", true);

                        }
                    }
                    else {
                        $("#ImsEcTrainingAddDetails").html(data);
                        $("#ddlState").attr("disabled", true);
                        $("#ddlPhaseYear").attr("disabled", true);                      
                        $("#ddlDesignation").attr("disabled", true);

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

function SearchDetails(stateCode, designation, year) {

    $('#ddlStateSerach').val(stateCode);
    $('#ddlDesignationSerach').val(designation);
    $('#ddlPhaseYearSerach').val(year);
    $('#tblImsEcTraining').setGridParam({
        url: '/Master/GetImsEcTrainingList'
    });

    $('#tblImsEcTraining').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlStateSerach option:selected').val(), designation: $('#ddlDesignationSerach option:selected').val(), year: $('#ddlPhaseYearSerach option:selected').val() } });
    $('#tblImsEcTraining').trigger("reloadGrid", [{ page: 1 }]);
}

function ClearMessage() {
    if ($("#dvErrorMessage").is(":visible")) {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    }

}

function ClearDetails() {
    $('#ddlState').val('0');
    if ($('#hdStatCode').val() > 0) {
        $("#ddlState").val($('#hdStatCode').val());
        $("#ddlState").attr("disabled", true);
    }
    $('#ddlPhaseYear').val('0');
    $('#ddlDesignation').val('0');
    $('#IMS_TOTAL_PERSON').val('');

    $('#dvErrorMessage').hide('slow');

    $('#message').html('');


}









