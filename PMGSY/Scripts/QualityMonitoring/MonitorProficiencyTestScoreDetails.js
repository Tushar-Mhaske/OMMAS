$('#monitorList').chosen();

$("#spCollapseIconCN").click(function () {
    $('#formScore').hide();
    $('#AddScore').show();
    $('#divListScore').show();
    $('#DownloadTemplate').show();

    $('#tbScoreList').trigger('reloadGrid');
});

$('#dateOfExam').datepicker({
    dateFormat: 'dd/mm/yy',
    showOn: "button",
    buttonImage: "/Content/Images/calendar_2.png",
    buttonImageOnly: true,
    maxDate: "+0D",
    changeMonth: true,
    changeYear: true,
    buttonText: 'Date of Exam',
    onSelect: function (selectedDate) {
        jQuery.validator.methods["date"] = function (value, element) { return true; }
        $('#dateOfExam').trigger('blur');
    },
});

$('#monitorType').change(function () {
    if ($('#monitorType').val() == "S")
        $('.stateDropdown').show();
    else {
        $('#STATE_CODE').val('0');
        $('.stateDropdown').hide();
    }

});

$('#btnSaveScore').click(function () {

    if ($("#formAddScore").valid()) {
        if (confirm("Are you sure to add proficiency test score details ?")) {
            if ($('#btnSaveScore').valid()) {
                var form = $('#formAddScore');
                var formadata = new FormData(form.get(0));

                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                $.ajax({
                    url: '/QualityMonitoring/GetUploadDetails',
                    type: 'POST',
                    cache: false,
                    async: false,
                    contentType: false,
                    processData: false,
                    beforeSend: function () { },
                    data: formadata,
                    success: function (response) {
                        alert(response.message);
                        $("#formAddScore")[0].reset();
                        if (response.success) {

                            $('#formScore').hide();
                            $('#AddScore').show();
                            $('#DownloadTemplate').show();
                            $('#divListScore').show();

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

$('#btnEditScore').click(function () {

    if ($("#formAddScore").valid()) {
        if (confirm("Are you sure to update proficiency test score details ?")) {
            if ($('#btnEditScore').valid()) {
                var form = $('#formAddScore');
                var formadata = new FormData(form.get(0));

                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                $.ajax({
                    url: '/QualityMonitoring/EditProficiencyTestScore',
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
                            $("#formAddScore")[0].reset();

                            $('#formScore').hide();
                            $('#AddScore').show();
                            $('#DownloadTemplate').show();
                            $('#divListScore').show();

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

