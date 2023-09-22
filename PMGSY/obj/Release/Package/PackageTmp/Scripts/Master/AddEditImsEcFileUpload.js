var files;
$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmAddImsEcFileUpload');

    //if (!$("#btnSearch").is(":visible")) {
    //    alert('hi');
    //    $("#btnSearch").show();
    //}

    $("#ddlState").change(function () {
        loadAgencyList($("#ddlState").find(":selected").val());
        ClearMessage();
    });

    $("#dvhdAddNewPIUDetails").click(function () {

        if ($("#dvAddNewImsEcFileUploadDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvAddNewImsEcFileUploadDetails").slideToggle(300);
        }
        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvAddNewImsEcFileUploadDetails").slideToggle(300);
        }
    });


    $("#frmAddImsEcFileUpload").on('submit', function (event) {


        event.stopPropagation(); // Stop stuff happening
        event.preventDefault();

        var form_data = new FormData();

        var objFiles = $("input#file").prop("files");

        form_data.append("file", objFiles[0]);

        $("#fileDetails").val(form_data);

        var data = $("#frmAddImsEcFileUpload").serializeArray();

        for (var i = 0; i < data.length; i++) {
            form_data.append(data[i].name, data[i].value);
        }
        var stateCode = $("#ddlState option:selected").val();
        var year = $("#ddlPhaseYear option:selected").val();
        var batch = $("#ddlBatch option:selected").val();
        var agency = $("#ddlAgency option:selected").val();
        var fileType = $("#ddlImsFileType option:selected").val();
        $("#ddlState").attr("disabled", false);
       // console.log(form_data);
        $.ajax({
            type: 'POST',
            url: '/Master/AddEditImsEcFileUpload/',
            async: false,
            data: form_data,
            contentType: false,
            processData: false,
            success: function (data) {

                if (data.success == true) {
                    alert(data.message);
                    ClearDetails();
                    if ($("#ImsEcFileUploadAddDetails").is(":visible")) {
                        $('#ImsEcFileUploadAddDetails').hide('slow');
                        $('#btnSearch').hide();
                        $('#btnAdd').show();
                    }

                    if (!$("#ImsEcFileUploadSearchDetails").is(":visible")) {
                        $("#ImsEcFileUploadSearchDetails").show('slow');
                    }
                    SearchIMSECDetails(stateCode, agency, year, batch);
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
                    $("#ImsEcFileUploadAddDetails").html(data);
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



    });




    //$("#btnSave").click(function (e) {       


    //    if ($("#frmAddImsEcFileUpload").valid()) {

    //        $("#ddlState").attr("disabled", false);

    //        var stateCode = $("#ddlState option:selected").val();
    //        var year = $("#ddlPhaseYear option:selected").val();
    //        var batch = $("#ddlBatch option:selected").val();
    //        var agency = $("#ddlAgency option:selected").val();
    //        var fileType = $("#ddlImsFileType option:selected").val();

    //        $("#frmAddImsEcFileUpload").submit(function () {
    //            alert('success');
    //        });




    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    //$.ajax({
    //    type: 'POST',
    //    url: '/Master/AddEditImsEcFileUpload/',
    //    async: false,
    //    data: $("#frmAddImsEcFileUpload").serialize(),
    //    success: function (data) {

    //        if (data.success == true) {
    //            alert(data.message);
    //            ClearDetails();
    //            SearchIMSECDetails(stateCode, agency, year, batch);
    //            // $("#ddlState").attr("disabled", true);
    //        }
    //        else if (data.success == false) {

    //            if (data.message != "") {
    //                $('#message').html(data.message);
    //                $('#dvErrorMessage').show('slow');

    //            }
    //            if ($('#hdStatCode').val() > 0) {
    //                $("#ddlState").val($('#hdStatCode').val());
    //                $("#ddlState").attr("disabled", true);
    //            }
    //        }
    //        else {
    //            $("#ImsEcFileUploadAddDetails").html(data);
    //            //if ($('#stateCode').val() > 0) {
    //            //    $("#ddlState").val($('#stateCode').val());
    //            //    $("#ddlState").attr("disabled", true);
    //            //}
    //        }

    //        $.unblockUI();
    //    },
    //    error: function (xhr, ajaxOptions, thrownError) {
    //        alert(xhr.responseText);
    //        $.unblockUI();
    //        if ($('#hdStatCode').val() > 0) {
    //            $("#ddlState").val($('#hdStatCode').val());
    //            $("#ddlState").attr("disabled", true);
    //        }
    //    }
    //})
    //    }



    //});

    $("#btnCancel").click(function (e) {

        //$.ajax({
        //    url: "/Master/AddEditImsEcFileUpload",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        $("#ImsEcFileUploadAddDetails").html(data);
        //        $("#ImsEcFileUploadAddDetails").show();
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }

        //});
        if ($("#ImsEcFileUploadAddDetails").is(":visible")) {
            $('#ImsEcFileUploadAddDetails').hide('slow');
            $('#btnSearch').hide();
            $('#btnAdd').show();
        }

        if (!$("#ImsEcFileUploadSearchDetails").is(":visible")) {
            $("#ImsEcFileUploadSearchDetails").show('slow');
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


        if ($("#frmAddImsEcFileUpload").valid()) {

            $("#ddlState").attr("disabled", false);
            $("#ddlPhaseYear").attr("disabled", false);
            $("#ddlBatch").attr("disabled", false);
            $("#ddlAgency").attr("disabled", false);
            $("#ddlImsFileType").attr("disabled", false);
            var stateCode = $("#ddlState option:selected").val();
            var year = $("#ddlPhaseYear option:selected").val();
            var batch = $("#ddlBatch option:selected").val();
            var agency = $("#ddlAgency option:selected").val();
            var fileType = $("#ddlImsFileType option:selected").val();

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/EditImsEcFileUpload/',
                async: false,
                data: $("#frmAddImsEcFileUpload").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);

                        //$("#ImsEcFileUploadAddDetails").load("/Master/AddEditImsEcFileUpload");

                        //$('#tblImsEcFileUpload').trigger('reloadGrid');

                         ClearDetails();
                    if ($("#ImsEcFileUploadAddDetails").is(":visible")) {
                        $('#ImsEcFileUploadAddDetails').hide('slow');
                        $('#btnSearch').hide();
                        $('#btnAdd').show();
                    }

                    if (!$("#ImsEcFileUploadSearchDetails").is(":visible")) {
                        $("#ImsEcFileUploadSearchDetails").show('slow');
                    }
                    SearchIMSECDetails(stateCode, agency, year, batch);
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                            $("#ddlState").attr("disabled", true);
                            $("#ddlPhaseYear").attr("disabled", true);
                            $("#ddlBatch").attr("disabled", true);
                            $("#ddlAgency").attr("disabled", true);
                            $("#ddlImsFileType").attr("disabled", true);


                        }
                    }
                    else {
                        $("#ImsEcFileUploadAddDetails").html(data);
                        $("#ddlState").attr("disabled", true);
                        $("#ddlPhaseYear").attr("disabled", true);
                        $("#ddlBatch").attr("disabled", true);
                        $("#ddlAgency").attr("disabled", true);
                        $("#ddlImsFileType").attr("disabled", true);


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


    $("#frmAddImsEcFileUpload").bind('submitdone', function () {
       
    });



    // Add events
    $('input[type=file]').on('change', prepareUpload);




});
// Grab the files and set them to our variable
function prepareUpload(event) {
    files = event.target.files;
}


function SearchIMSECDetails(stateCode, agency, year, batch) {
    $('#ddlStateSerach').val(stateCode);
    $('#ddlStateSerach').trigger('change');
    setTimeout(function () {
        $('#ddlAgencySerach').val(agency);
        $('#ddlPhaseYearSerach').val(year);
        $('#ddlBatchSerach').val(batch);
        $('#tblImsEcFileUpload').setGridParam({
            url: '/Master/GetImsEcFileUploadList'
        });
        $('#tblImsEcFileUpload').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlStateSerach').val(), agency: $('#ddlAgencySerach').val(), year: $('#ddlPhaseYearSerach').val(), batch: $('#ddlBatchSerach').val() } });
        $('#tblImsEcFileUpload').trigger("reloadGrid", [{ page: 1 }]);
    }, 1000);
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
    $('#ddlBatch').val('0');
    $('#ddlAgency').val('0');
    $('#ddlImsFileType').val('%');
    $('#file').val('');
    $('#dvErrorMessage').hide('slow');
    $('#message').html('');


}

function loadAgencyList(statCode) {
    $("#ddlAgency").val(0);
    $("#ddlAgency").empty();
    if (statCode > 0) {
        if ($("#ddlAgency").length > 0) {
            $.ajax({
                url: '/Master/GetAgencyListByState',
                type: 'POST',
                data: { "stateCode": statCode, "IsAllSelected": false },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlAgency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }



                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
    else {

        $("#ddlAgency").append("<option value='0'>--Select--</option>");

    }
}

function FileUploadSuccess(data) {
    alert(data.success);
    if (data.success == false) {
        $("#dvErrorMessage").show('fade');
        $("#message").html(data.message);
        return false;
    }
}
function Failure() {
    alert('fail');
}





