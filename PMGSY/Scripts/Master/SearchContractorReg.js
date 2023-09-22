$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    //alert($('#roleCode').val());
    if ($('#stateCode').val() > 0) {

        $("#State").val($('#stateCode').val());
        if (parseInt($('#roleCode').val()) != 36) {
            $("#State").attr("disabled", true);
        }
        $("#State").trigger('change');

    }

    $("#spCollapseIconS").click(function () {
        if ($("#dvSearchParameter").is(":visible")) {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $("#dvSearchParameter").slideToggle(300);
        }
        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $("#dvSearchParameter").slideToggle(300);
        }
    });

    $('#btnSearchView').click(function (e) {
        SearchDetails();
    });

    setTimeout(function () {
        $('#btnSearchView').trigger('click');
    }, 300);




    $("#State").change(function () {


        //FillInCascadeDropdown({ userType: $("#State").find(":selected").val() },
        //            "#District", "/Master/GetDistrictsList?stateCode=" + $('#State option:selected').val());



    });

    if ($("#State").val() == 0) {

        $("#State").val($("#State")[0].options[1].value);
        //$("#State").trigger('change');
    }
    $('#btnSearchView').trigger('click')
    {

        LoadContractorRegistrationList();
    }
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


function SearchDetails() {

    $('#tblstContractorReg').setGridParam({
        url: '/Master/GetContractorRegList', datatype: 'json'
    });
    $('#tblstContractorReg').jqGrid("setGridParam", { "postData": { stateCode: $('#State').val(), status: $('#Status option:selected').val(), contractorName: $('#txtContractor').val(), conStatus: $('#ContractorStatus option:selected').val(), panNo: $("#txtPan").val(), classType: $("#ClassType option:selected").val(), regNo: $("#txtRegNo").val(), companyName: $("#txtCompanyName").val() } });

    $('#tblstContractorReg').trigger("reloadGrid", [{ page: 1 }]);
}
function FillInCascadeDropdown(map, dropdown, action) {

    var message = '';

    //if (dropdown == '#District') {
    //    message = '<h4><label style="font-weight:normal"> Loading Districts... </label></h4>';
    //}

    $(dropdown).empty();
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
}