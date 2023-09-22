$("#spCollapseIconCNCQC").click(function () {
    $('#formAddScore').show();
    $('#formAddScoreCQC').hide();
    $('#btnViewScore').show();
    $('#divListScore').show();

    $('#searchMonitor').show('slow');
    $('#formAddScore').hide();

    $('#tbScoreList').trigger('reloadGrid');
});

$("#btnBack").click(function () {
    $('#formAddScore').show();
    $('#formAddScoreCQC').hide();
    $('#btnViewScore').show();
    $('#divListScore').show();

    $('#tbScoreList').trigger('reloadGrid');
});

$('#btnSaveScoreCQC').click(function () {

    if ($("#formAddScoreCQC").valid()) {
        if (confirm("Are you sure to update score ?")) {
            if ($('#btnSaveScoreCQC').valid()) {
                var form = $('#formAddScoreCQC');
                var formadata = new FormData(form.get(0));

                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                $.ajax({
                    url: '/QualityMonitoring/EditProficiencyScoreDetails',
                    type: 'POST',
                    cache: false,
                    async: false,
                    contentType: false,
                    processData: false,
                    beforeSend: function () { },
                    data: formadata,
                    success: function (response) {
                        alert(response.message);
                        if (response.success) {
                            $("#formAddScoreCQC")[0].reset();
                            $('#formAddScore').show();
                            $('#formAddScoreCQC').hide();
                            $('#btnViewScore').show();
                            $('#divListScore').show();

                            $('#searchMonitor').show('slow');

                            $('#divAddScore').hide();
                            $('#formAddScore').hide('slow');

                            $('#tbScoreList').trigger('reloadGrid');
                        }
                        $.unblockUI();
                    },
                    error: function () {
                        $.unblockUI();
                        alert("An Error");
                        return false;
                    },
                });
            }
        }
    }

    else
        return false;

});

