/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMPahseInspectionProgress.js
        * Description   :   Handles click event
        * Creation Date :   29/Aug/2014
 **/
$(document).ready(function () {


    $('#yearLabelId').hide();
    $('#yearId').hide();

    $.validator.unobtrusive.parse($('#frmQMPahseInsp'));

    if ($("#hdnRoleCode").val() == "8") {
        $("#StateName").val($('#ddlQMPahse option:selected').text());
        loadReport();
    }

    $("#btnQMPahseInsp").click(function () {
        if ($('#frmQMPahseInsp').valid()) {

            $("#loadReport1").html("");
            $("#loadReport2").html("");
            $("#loadReport").html("");
            $("#LoadQMInspDetailsReport").html("");
            $("#StateName").val($('#ddlQMPahse option:selected').text());
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            loadReport();
        }
    });

    $('input[type = radio][name = rdReportPart]').change(function () {
      //  alert("change");
        if (this.value == 1 ) {
			  $('#ddlYear').val("0");
            $('#yearLabelId').hide();
            $('#yearId').hide();
        }
        else {
            $('#yearLabelId').show();
            $('#yearId').show();
        }


    });


});

function loadReport() {

    var stateCode = $('#ddlQMPahse').val();
    if (stateCode > 0) {
        $.ajax({
            url: '/QualityMonitoring/QMPhaseProgressInspectionReport/',
            type: 'POST',
            catche: false,
            data: $("#frmQMPahseInsp").serialize(),
            async: false,
            success: function (response) {
                $.unblockUI();
                $("#loadReport").html(response);

            },
            error: function () {
                $.unblockUI();
                alert("An Error");
                return false;
            },
        });
    }
    else {
        alert("Please select a valid state");
    }
}