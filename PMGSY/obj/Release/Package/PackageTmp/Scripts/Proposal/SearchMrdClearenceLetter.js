var stateCode;
var agencyCode;
var year;
var batch;
var collaboration;

$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmSearchMrdClearenceLetter');

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    if ($('#hdStatCode').val() > 0) {

        $("#ddlStateSerach").attr("disabled", true);
        $("#ddlAgencySerach option[value='0']").remove();
    }


    $.validator.unobtrusive.parse('#frmSearchMrdClearenceLetter');
    $("#btnMrdClearenceLetterSearch").click(function () {
        stateCode = $('#ddlStateSerach option:selected').val();
        agencyCode = $('#ddlAgencySerach option:selected').val();
        year = $('#ddlPhaseYearSerach option:selected').val();
        batch = $('#ddlBatchSerach option:selected').val();
        collaboration = $('#ddlCollaborationSerach option:selected').val();
        searchDetails(stateCode, agencyCode, year, batch,collaboration);
    });


    $("#btnMrdClearenceLetterSearch").trigger('click')
    {
        LoadMrdClearenceLetterGrid();      
    }
    $('#ddlStateSerach').change(function () {
        loadSearchAgencyList($('#ddlStateSerach option:selected').val());
        $("#dvSearchParameter").show();

    });
    $("#dvhdSearch").click(function () {

        if ($("#dvSearchParameter").is(":visible")) {

            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvSearchParameter").slideToggle(300);
        }

        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvSearchParameter").slideToggle(300);
        }
    });
});

function searchDetails(stateCode, agencyCode, year, batch,collaboration) {
    if ($("#dvAccordianMRD").is(":visible")) {
        CloseClearanceRevisionDetails();
    }
    $('#tblMrdClearenceLetter').setGridParam({
        url: '/Proposal/GetMrdClearenceLetterList', datatype: 'json'
    });

    $('#tblMrdClearenceLetter').jqGrid("setGridParam", { "postData": { stateCode: stateCode, agency: agencyCode, year: year, batch: batch, collaboration: collaboration } });
    $('#tblMrdClearenceLetter').trigger("reloadGrid", [{ page: 1 }]);
    $("#dvSearchParameter").show();
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
                url: '/Proposal/PopulateAgenciesByStateAndDepartmentwise',
                type: 'POST',
                data: { "StateCode": statCode, "IsAllSelected": true },
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

        $("#ddlAgencySerach").append("<option value='0'>--All Agency--</option>");
    }
}


