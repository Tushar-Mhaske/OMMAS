$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmTaxSchedule');

    $("#rdbPer").click(function () {
        $('.per').show('slow');
        $('.lmsm').hide('slow');
    });

    $("#rdbLmsm").click(function () {
        $('.per').hide('slow');
        $('.lmsm').show('slow');
    });

    displayTaxType();

    $("#idFilterDivTax").click(function () {
        $("#idFilterDivTax").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
        $("#frmTaxSchedule").toggle("slow");
    });

    $("#btnSaveTax").click(function () {
        //alert($('#ddlMinorItem').val());
        if ($('#frmTaxSchedule').valid()) {

            if ($('#ddlMinorItem option:selected').val() > 0) {
                $('#itemCode').val($('#ddlMinorItem').val());
            }
            else if ($('#ddlMajorItem option:selected').val() > 0) {
                $('#itemCode').val($('#ddlMajorItem').val());
            }
            else {
                if ($('#ItemCode').val() > 0) {
                    $('#itemCode').val($('#ItemCode').val());
                }
            }
            //alert($('itemCode').val());
            $('#User_Action').val('A');
            $.ajax({
                url: '/ARRR/AddScheduleTaxDetails/',
                async: false,
                type: 'POST',
                //data: form_data,
                data: $("#frmTaxSchedule").serialize(),
                success: function (data) {
                    alert(data.message);
                    if (data.success == true) {

                        $("#btnResetTax").trigger('click');
                        LoadTaxScheduleGrid();
                        //$('#dvLoadMaterialRate').hide('slow');
                        //$("#btnAdd").show('slow');
                    }
                }
            })
        }
    });
});


function displayTaxType(urlparameter) {
    if ($('input[id=rdbPer]').attr("checked") == 'checked') {
        $("#rdbPer").trigger('click');
    }
    else {
        $("#rdbLmsm").trigger('click');
    }
};
