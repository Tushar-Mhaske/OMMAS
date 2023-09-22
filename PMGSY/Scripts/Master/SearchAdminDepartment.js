var stateCode;
var agencyCode;

$(document).ready(function () {
 
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    if ($('#stateCode').val() > 0) {

        $("#State").val($('#stateCode').val());
        $("#State").attr("disabled", true);
        if ($('#RoleCode').val() == 47 || $('#RoleCode').val() == 36) //RoleCode ITNOOA-47 && ITNO-36
        {
            $("#Agency").attr("disabled", true);
        }

        $("#State").trigger('change');

    } else {
        $("#State").val($("#State")[0].options[1].value);
        $("#State").trigger('change');
    }


$.validator.unobtrusive.parse('#frmSearchAdmin');
$("#btnAdminSearch").click(function () {
    stateCode= $('#State option:selected').val();
    agencyCode = $('#Agency').val();
    //alert(stateCode);
    searchDetails(stateCode,agencyCode);
    });

$("#btnAdminSearch").trigger('click')
{
    loadGrid();
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

function searchDetails(stateCode, agencyCode) {

    $('#adminCategory').setGridParam({
        url: '/Master/GetDepartmentList', datatype: 'json'
    });
  

    $('#adminCategory').jqGrid("setGridParam", { "postData": { stateCode: stateCode, agency: agencyCode } });
    $('#adminCategory').trigger("reloadGrid", [{ page: 1 }]);
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
