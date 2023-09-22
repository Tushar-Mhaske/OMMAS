$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmProposalsWithoutUploads'));

    $("#spCollapseIconCN").click(function () {
        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmProposalsWithoutUploads").toggle("slow");
    });

    $('#ddlState').change(function () {
        $.ajax({
            url: '/ProposalSSRSReports/ProposalSSRSReports/DistrictByStateCode',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { StateCode: $("#ddlState").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
                $.unblockUI();
            },
            error: function (err) {
                //alert("error " + err);
                $.unblockUI();
            }
        });
    });

    $('#btnView').click(function () {
        if ($('#frmProposalsWithoutUploads').valid()) {
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/ProposalsWithoutUploadsReport/',
                type: 'POST',
                catche: false,
                data: $("#frmProposalsWithoutUploads").serialize(),
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
    });
});