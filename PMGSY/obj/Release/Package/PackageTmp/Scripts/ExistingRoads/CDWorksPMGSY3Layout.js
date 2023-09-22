
$(document).ready(function () {
    $.validator.unobtrusive.parse(('#frmCdWorks'));
    
    GetCdWorksPMGSY3($("#MAST_ER_ROAD_CODE").val());

    if ($("#Operation").val() == "A") {
        $("#rowAdd").show();
        $("#rowUpdate").hide();
    } else {
        $("#rowUpdate").show();
        $("#rowAdd").hide();
    }


    if ($("#Operation").val() == "A") {

        $(function () {
            $("#MAST_CD_LENGTH").focus();
        });
    }

    $("#btnReset").click(function () {
        //alert(3);
        //hide error alert
        if ($("#divError").is(":visible")) {
            $("#divError").hide("slow");
        }

        $(".pmgsy-textbox").removeClass('input-validation-error');
        $(".field-validation-error").html('');
        $("#MAST_CDWORKS_CODE").removeClass('input-validation-error');
        $("#MAST_CONSTRUCT_YEAR").removeClass('input-validation-error');
        $("#MAST_REHAB_YEAR").removeClass('input-validation-error');
        resetForm();
    });


    //allow only digits and .

    $("#MAST_CD_LENGTH").keypress(function (e) {

        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 46 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });
    $("#MAST_CD_DISCHARGE").keypress(function (e) {
        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 46 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });
    $("#MAST_CD_CHAINAGE").keypress(function (e) {
        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 46 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });

    $("#MAST_ER_SPAN").keypress(function (e) {
        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 46 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });
    $("#MAST_CARRIAGE_WAY").keypress(function (e) {
        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 46 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });

    $("#MAST_REHAB_YEAR").change(function () {

       
        var RoadConstructionYear =$("#MAST_CONSTRUCT_YEAR option:selected").val();
        var RoadRehabilitationYear =$("#MAST_REHAB_YEAR option:selected").val();



        if (RoadConstructionYear != 0 && RoadConstructionYear >= RoadRehabilitationYear) {
            $("#roadRehabilitationYear").show("slow");
            $("#roadRehabilitationYear").html("<span style='color:red'><b>Road Rehabilitation Year must be greater than road construction year.</b></span>");
        }
    });

    $("#MAST_CONSTRUCT_YEAR").change(function () {

        var RoadConstructionYear = $("#MAST_CONSTRUCT_YEAR option:selected").val();
        var RoadRehabilitationYear = $("#MAST_REHAB_YEAR option:selected").val();

        if (RoadRehabilitationYear != 0 && (RoadConstructionYear >= RoadRehabilitationYear)) {
            $("#roadConstructionYear").show("slow");
            $("#roadConstructionYear").html("<span style='color:red'><b>Road Construction Year must be less than road rehabilitation year.</b></span>");
        }
    });

    $('#btnSave3').click(function() {
        //alert(2);
        if ($('#frmCdWorks').valid()) {
            //if (validateForm() == true) {

                $.ajax({
                    url: "/ExistingRoads/AddCdWorkDetailsPMGSY3/",
                    type: "POST",
                    cache: false,
                    data: $("#frmCdWorks").serialize(),
                    beforeSend: function () {
                        blockPage();
                    },
                    error: function (xhr, status, error) {
                        unblockPage();
                        Alert("Request can not be processed at this time,please try after some time!!!");
                        return false;
                    },
                    success: function (response) {
                        unblockPage();
                        if (response.success == true) {
                            alert(response.message);
                            $("#tbCdWorks").trigger('reloadGrid');
                            $("#frmCdWorks").trigger('reset');
                            $("#divError").hide("slow");
                            $("#divCdWorks").html('');
                            //$("#divCdWorks").load('/ExistingRoads/CdWorkAddEdit?id=' + $("#EncryptedRoadCode").val(), function () {
                            //    $.validator.unobtrusive.parse("divCdWorks");
                            //});
                            LoadAddView();
                        }
                        else {
                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.message);
                            unblockPage();
                        }
                    }
                });
            //}

        }
        else {
            
        }
            
    });

    $('#btnUpdate').click(function (evt) {
        evt.preventDefault();

        $("#divError").hide("slow");
        if ($('#frmCdWorks').valid()) {
                $.ajax({
                    url: "/ExistingRoads/EditCdWorkDetailsPMGSY3/",
                type: "POST",
                cache: false,
                data: $("#frmCdWorks").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    Alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },
                success: function (response) {  
                    unblockPage();

                    $("#tbCdWorks").trigger('reloadGrid');

                    $("#frmCdWorks").trigger('reset');
                          
                    if (response.success == true) {
                        alert(response.message);
                        $("#Operation").val("A");
                        $("#rowAdd").show();
                        $("#rowUpdate").hide();
                        resetForm();
                        $("#divCdWorks").html('');
                        //$("#divCdWorks").load('/ExistingRoads/CdWorkAddEdit?id=' + $("#EncryptedRoadCode").val(), function () {
                        //    $.validator.unobtrusive.parse("divCdWorks");
                        //});
                        LoadAddView();
                       
                    }
                    else {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.message);
                    }
                }
            });
        }
    });

    $('#btnCancel').click(function () {
        $("#Operation").val("A");
        $("#rowAdd").show();
        $("#rowUpdate").hide();
        $("#MAST_CDWORKS_CODE").attr('disabled', false);
        resetForm();

    });

});


function validateForm()
{
    var RoadConstructionYear = $("#MAST_CONSTRUCT_YEAR option:selected").val();
    var RoadRehabilitationYear = $("#MAST_REHAB_YEAR option:selected").val();

    if (RoadConstructionYear != 0 && RoadConstructionYear >= RoadRehabilitationYear) {
        $("#roadRehabilitationYear").show("slow");
        $("#roadRehabilitationYear").html("<span style='color:red'><b>Road Rehabilitation Year must be greater than road construction year.</b></span>");
        return false;
    }
    else {
        $("#roadRehabilitationYear").html('');
        return true;
    }
}

function resetForm()
{   
    $("#MAST_CDWORKS_CODE").val("");
    $("#MAST_CD_LENGTH").val("");
    $("#MAST_CD_DISCHARGE").val("");
    $("#MAST_CD_CHAINAGE").val("");
    $("#MAST_CONSTRUCT_YEAR").val("");
    $("#MAST_REHAB_YEAR").val("");
    $("#MAST_ER_SPAN").val("");
    $("#MAST_CARRIAGE_WAY").val("");
    $("#roadRehabilitationYear").html('');
    $("#roadConstructionYear").html('');

}