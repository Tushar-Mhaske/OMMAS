var stateCode;
var agencyCode;
var year;
var batch;

$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmSearchImsEcFileUpload');

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    if ($('#hdStatCode').val() > 0) {

        $("#ddlStateSerach").attr("disabled", true);
    }


    $.validator.unobtrusive.parse('#frmSearchImsEcFileUpload');
    $("#btnImsEcFileUploadSearch").click(function () {
        stateCode = $('#ddlStateSerach option:selected').val();
        agencyCode = $('#ddlAgencySerach option:selected').val();
        year = $('#ddlPhaseYearSerach option:selected').val();
        batch = $('#ddlBatchSerach option:selected').val();

        searchDetails(stateCode, agencyCode, year, batch);
    });


    $("#btnImsEcFileUploadSearch").trigger('click')
    {
        LoadImsEcFileUploadGrid();
    }
    $('#ddlStateSerach').change(function () {
        loadSearchAgencyList($('#ddlStateSerach option:selected').val());

    });
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

function searchDetails(stateCode, agencyCode, year, batch) {

    $('#tblImsEcFileUpload').setGridParam({
        url: '/Master/GetImsEcFileUploadList', datatype: 'json'
    });

    $('#tblImsEcFileUpload').jqGrid("setGridParam", { "postData": { stateCode: stateCode, agency: agencyCode, year: year, batch: batch } });
    $('#tblImsEcFileUpload').trigger("reloadGrid", [{ page: 1 }]);
}

function FillInCascadeDropdown(map, dropdown, action) {

    var message = '';

    if (dropdown == '#District') {
        message = '<h4><label style="font-weight:normal"> Loading Districts... </label></h4>';
    }
    else {
        message = '<h4><label style="font-weight:normal"> Loading Agencies... </label></h4>';
    }
    $(dropdown).empty();
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
}

function loadSearchAgencyList(statCode) {
    $("#ddlAgencySerach").val(0);
    $("#ddlAgencySerach").empty();

    if (statCode > 0) {
        if ($("#ddlAgencySerach").length > 0) {
            $.ajax({
                url: '/Master/GetAgencyListByState',
                type: 'POST',
                data: { "stateCode": statCode, "IsAllSelected": true },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlAgencySerach").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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

        $("#ddlAgencySerach").append("<option value='0'>--All--</option>");
    }
}


