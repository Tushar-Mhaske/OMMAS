$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmBulkRoadSanction'));

    $("input[name='IMS_SANCTIONED']").click(function () {
        if ($("input[name='IMS_SANCTIONED']").val() != "N") {

            $("#trSanctionedBy").show("slow");
            $("#trSanctionRemark").show("slow");

            $("#txtSanctionBy").val("");
            $("#txtRemarks").val("");
        }
    });

    $(function () {
        $("#IMS_SANCTIONED_DATE").datepicker(
        {
            dateFormat: "dd-M-yy",
            changeMonth: true,
            changeYear: true,
            //minDate: -365,
            maxDate: "+0M +0D",
            showOn: 'button',
            buttonImage: '../../Content/images/calendar_2.png',
            buttonImageOnly: true,
            onClose: function () {
                $(this).focus().blur();
            }
        });
        $("#IMS_SANCTIONED_DATE").datepicker().attr('readonly', 'readonly');
        $("#IMS_SANCTIONED_DATE").datepicker("option", "maxDate", new Date());
    });

    $("#btnSanctionAll").click(function (evt) {
        evt.preventDefault();


        if ($('#frmBulkRoadSanction').valid()) {
           
            $.ajax({
                url: "/Proposal/BulkSanction/",
                type: "POST",
                cache: false,
                data: $("#frmBulkRoadSanction").serialize(),
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
                    if (response.Success) {

                        if (response.Message != undefined && response.Message != "") {
                            alert(response.Message);
                        }
                        else {
                            alert("Proposals Sanctioned Succesfully.");
                        }
                        CloseProposalDetails();
                        LoadMordProposals($("#ddlImsYear").val(), $("#ddlState").val(), $("#ddlDistrict").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val());

                    }
                    else {
                        alert(response.ErrorMessage);
                    }
                }
            });
        
        }
    });

});