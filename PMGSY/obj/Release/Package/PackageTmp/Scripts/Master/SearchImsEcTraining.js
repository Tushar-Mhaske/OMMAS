var stateCode;
var desigCode;
var year;


$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmSearchImsEcTraining');

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    if ($('#hdStatCode').val() > 0) {

        $("#ddlStateSerach").attr("disabled", true);
    }


    $.validator.unobtrusive.parse('#frmSearchImsEcTraining');
    $("#btnImsEcTrainingSearch").click(function () {
        stateCode = $('#ddlStateSerach option:selected').val();
        desigCode = $('#ddlDesignationSerach option:selected').val();
        year = $('#ddlPhaseYearSerach option:selected').val();
      
        searchDetails(stateCode, desigCode, year);
    });


    $("#btnImsEcTrainingSearch").trigger('click')
    {
        LoadImsEcTrainingGrid();
    } 
    $("#dvhdSearch").click(function () {

        if ($("#dvSearchParameter").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvSearchParameter").slideToggle(300);
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvSearchParameter").slideToggle(300);
        }
    });
});

function searchDetails(stateCode, desigCode, year) {

    $('#tblImsEcTraining').setGridParam({
        url: '/Master/GetImsEcTrainingList', datatype: 'json'
    });

    $('#tblImsEcTraining').jqGrid("setGridParam", { "postData": { stateCode: stateCode, designation: desigCode, year: year } });
    $('#tblImsEcTraining').trigger("reloadGrid", [{ page: 1 }]);
}




