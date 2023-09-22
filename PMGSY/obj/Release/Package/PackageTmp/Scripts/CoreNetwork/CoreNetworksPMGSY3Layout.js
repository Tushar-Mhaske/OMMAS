$.validator.addMethod("comparestartchainge", function (value, element, params) {

    var startChainage = parseFloat($("#txtRoadFromChainage").val());
    if ($("#radioRoadLengthPartly").is(':checked')) {
        if ((parseFloat($("#ExistStartChainage").val()) <= startChainage)) {
            return true;
        }
    }

    if ($("#radioRoadLengthFully").is(':checked')) {
        if ((parseFloat($("#ExistStartChainage").val()) <= startChainage)) {
            return true;
        }
    }
    return false;
});
jQuery.validator.unobtrusive.adapters.addBool("comparestartchainge");

$.validator.addMethod("compareendchainge", function (value, element, params) {

    var endChainage = parseFloat($("#txtRoadToChainage").val());
    if ($("#radioRoadLengthPartly").is(':checked')) {
        if (endChainage <= (parseFloat($("#ExistEndChainage").val()))) {
            return true;
        }
    }

    if ($("#radioRoadLengthFully").is(':checked')) {
        if (endChainage <= (parseFloat($("#ExistEndChainage").val()))) {
            return true;
        }
    }

    return false;
});
jQuery.validator.unobtrusive.adapters.addBool("compareendchainge");

$.validator.addMethod("comparechainage", function (value, element, params) {

    var startChainage = parseFloat($("#txtRoadFromChainage").val());
    var endChainage = parseFloat($("#txtRoadToChainage").val());
    if ($("#radioRoadLengthPartly").is(':checked')) {
        if (startChainage <= endChainage) {
            return true;
        }
    }

    if ($("#radioRoadLengthFully").is(':checked')) {

        if (startChainage <= startChainage) {
            return true;
        }
    }
    return false;
});
jQuery.validator.unobtrusive.adapters.addBool("comparechainage");

$.validator.addMethod("comparehabitationstatusfrom", function (value, element, params) {

    if ($("#ddlRoadFrom option:selected").val() == "H") {
        if ($("#ddlFromHabitation option:selected").val() == '0') {
            return false;
        }
        else {
            return true;
        }
    }
    else {
        return true;
    }
});
jQuery.validator.unobtrusive.adapters.addBool("comparehabitationstatusfrom");

$.validator.addMethod("comparehabitationstatusto", function (value, element, params) {

    if ($("#ddlRoadTo option:selected").val() == "H") {
        if ($("#ddlToHabitation option:selected").val() == '0') {
            return false;
        }
        else {
            return true;
        }
    }
    else {
        return true;
    }
});
jQuery.validator.unobtrusive.adapters.addBool("comparehabitationstatusto");

$.validator.addMethod("compareroadstatusto", function (value, element, params) {

    if ($("#ddlRoadTo option:selected").val() != "H") {
        if ($("#ddlRoadNumTo option:selected").val() == '0') {
            return false;
        }
        else {
            return true;
        }
    }
    else {
        return true;
    }
});
jQuery.validator.unobtrusive.adapters.addBool("compareroadstatusto");

$.validator.addMethod("compareroadstatusfrom", function (value, element, params) {

    if ($("#ddlRoadFrom option:selected").val() != "H") {
        if ($("#ddlRoadNumFrom option:selected").val() == '0') {
            return false;
        }
        else {
            return true;
        }
    }
    else {
        return true;
    }
});
jQuery.validator.unobtrusive.adapters.addBool("compareroadstatusfrom");

$.validator.addMethod("isrequired", function (value, element, params) {

    var scheme = $("#Scheme").val();

    if (scheme == 1) {
        return true;
    }

    if (scheme == 2) {
        alert($("#TotalLengthOfCandidate").val());
        if ($("#TotalLengthOfCandidate").val() == "") {
            return false;
        }
        else {
            return true;
        }
    }
    return false;
});
jQuery.validator.unobtrusive.adapters.addBool("isrequired");

var isValid = true;

$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmAddCoreNetwork");

    // Road Category dropdown change
    $("#ddlRoadCategory").change(function (e) {

        FillInCascadeDropdown({ userType: $("#ddlRoadCategory").find(":selected").val() },
                   "#ddlRoadCode", "/CoreNetwork/GetRoadNameByRoadCodePMGSY3?roadName=" + $('#ddlRoadCategory option:selected').val() + "&blockName=" + $("#MAST_BLOCK_CODE").val());

    });

    $("#lblRoadCode").html("NA");
    $("#txtRoadLength").html($("#PLAN_RD_LENGTH").val());

    $("#ddlRoadNumFrom").hide();
    $("#lblStartAt").hide();
    $("#ddlRoadNumTo").hide();
    $("#lblEndAt").hide();
    $("#lblStartRequired").hide();
    $("#lblEndRequired").hide();

    // returns the road code according to the selected road name
    $("#ddlRoadCode").change(function (e) {

        var roadCode = $("#ddlRoadCode").val();

        //$('#txtRoadName').val($("#ddlRoadCode option:selected").text());

        $.ajax({
            type: 'POST',
            url: '/CoreNetwork/GetRoadNumberByExistRoadCode',
            data: { roadCode: roadCode },
            success: function (data) {
                if ($("#radioRoadLengthPartly").is(':checked')) {
                    $("#txtRoadFromChainage").val('');
                    $("#txtRoadToChainage").val('');
                }
                $("#lblRoadCode").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert('Error occurred');
            }
        })
    });

    if ($("#EncryptedRoadCode").val() != "") {
        $("#rowChainage").show();
    }
    $("#rowChainage").hide();

    // populates the Road From and Road To dropdown according to the route type selected
    $("#ddlRoadRoute").change(function (e) {

        if (($("#ddlRoadRoute").val()) == "T") {

            FillInCascadeDropdown({ userType: $("#ddlRoadRoute").find(":selected").val() },
                       "#ddlRoadFrom", "/CoreNetwork/GetRoadFromByRoadRoute?roadRoute=" + $('#ddlRoadRoute option:selected').val());

            FillInCascadeDropdown({ userType: $("#ddlRoadRoute").find(":selected").val() },
                       "#ddlRoadTo", "/CoreNetwork/GetRoadFromByRoadRoute?roadRoute=" + $('#ddlRoadRoute option:selected').val());

            FillInCascadeDropdown({ userType: $("#ddlRoadRoute").find(":selected").val() },
                       "#ddlRoadNumber", "/CoreNetwork/GetRoadNumberByRoadRoute?roadRoute=" + $('#ddlRoadRoute option:selected').val() + "&blockCode=" + $("#MAST_BLOCK_CODE").val() + "&cnCode=" + $("#PLAN_CN_ROAD_CODE").val());



            setTimeout(function () {
                var throughLength = $("#ddlRoadNumber option").size();
                if (throughLength == 1) {
                    alert('All through routes have been used for this road.');
                }
            }, 300);



        }
        else if (($("#ddlRoadRoute").val()) == "L") {

            FillInCascadeDropdown({ userType: $("#ddlRoadRoute").find(":selected").val() },
                       "#ddlRoadFrom", "/CoreNetwork/GetRoadFromByRoadRouteLink?roadRoute=" + $('#ddlRoadRoute option:selected').val());


            FillInCascadeDropdown({ userType: $("#ddlRoadRoute").find(":selected").val() },
                       "#ddlRoadTo", "/CoreNetwork/GetRoadToByRoadRoute?roadRoute=" + $('#ddlRoadRoute option:selected').val());

            FillInCascadeDropdown({ userType: $("#ddlRoadRoute").find(":selected").val() },
                   "#ddlRoadNumber", "/CoreNetwork/GetRoadNumberByRoadRoute?roadRoute=" + $('#ddlRoadRoute option:selected').val() + "&blockCode=" + $("#MAST_BLOCK_CODE").val() + "&cnCode=" + $("#PLAN_CN_ROAD_CODE").val());

            setTimeout(function () {
                var throughLengthTo = $("#ddlRoadNumber option").size();
                if (throughLengthTo == 1) {
                    alert('All link routes have been used for this road.');
                }
            }, 300);
        }
        else if (($("#ddlRoadRoute").val()) == "M") {

            FillInCascadeDropdown({ userType: $("#ddlRoadRoute").find(":selected").val() },
                       "#ddlRoadFrom", "/CoreNetwork/GetRoadFromByRoadRouteLink?roadRoute=" + $('#ddlRoadRoute option:selected').val());


            FillInCascadeDropdown({ userType: $("#ddlRoadRoute").find(":selected").val() },
                       "#ddlRoadTo", "/CoreNetwork/GetRoadToByRoadRoute?roadRoute=" + $('#ddlRoadRoute option:selected').val());

            FillInCascadeDropdown({ userType: $("#ddlRoadRoute").find(":selected").val() },
                   "#ddlRoadNumber", "/CoreNetwork/GetRoadNumberByRoadRoute?roadRoute=" + $('#ddlRoadRoute option:selected').val() + "&blockCode=" + $("#MAST_BLOCK_CODE").val() + "&cnCode=" + $("#PLAN_CN_ROAD_CODE").val());

            setTimeout(function () {
                var throughLengthTo = $("#ddlRoadNumber option").size();
                if (throughLengthTo == 1) {
                    alert('All Major Rural link routes have been used for this road.');
                }
            }, 300);
        }
        else if (($("#ddlRoadRoute").val()) == "N") {

            FillInCascadeDropdown({ userType: $("#ddlRoadRoute").find(":selected").val() },
                       "#ddlRoadFrom", "/CoreNetwork/GetRoadFromByRoadRouteLink?roadRoute=" + "M");


            FillInCascadeDropdown({ userType: $("#ddlRoadRoute").find(":selected").val() },
                       "#ddlRoadTo", "/CoreNetwork/GetRoadToByRoadRoute?roadRoute=" + "M");

            FillInCascadeDropdown({ userType: $("#ddlRoadRoute").find(":selected").val() },
                   "#ddlRoadNumber", "/CoreNetwork/GetRoadNumberByRoadRoute?roadRoute=" + $('#ddlRoadRoute option:selected').val() + "&blockCode=" + $("#MAST_BLOCK_CODE").val() + "&cnCode=" + $("#PLAN_CN_ROAD_CODE").val());

            setTimeout(function () {
                var throughLengthTo = $("#ddlRoadNumber option").size();
                if (throughLengthTo == 1) {
                    alert('All Major Rural link routes have been used for this road.');
                }
            }, 300);
        }
    });


    if ($("#EncryptedRoadCode").val() != "") {

        var startChainage = $("#ExistStartChainage").val();
        var endChainage = $("#ExistEndChainage").val();
        $("#lblStartChainage").html(startChainage);
        $("#lblEndChainage").html(endChainage);
        if ($("#radioRoadLengthPartly").is(':checked')) {
            $("#rowChainage").show('slow');
            $("#txtRoadToChainage").attr("readonly", false);
            $("#txtRoadFromChainage").attr("readonly", false);
        }

    }


    // Road Length:Partly - changes the Start chainage and end chainage textbox editable
    $("#radioRoadLengthPartly").change(function () {

        $("#txtRoadToChainage").val('');
        $("#txtRoadFromChainage").val('');

        if (($("#radioRoadLengthPartly").val()) == "P") {

            if ($("#EncryptedRoadCode").val() != "") {

                $("#txtRoadToChainage").attr("readonly", false);
                $("#txtRoadFromChainage").attr("readonly", false);
                $("#txtRoadToChainage").val($("#PLAN_RD_TO_CHAINAGE").val());
                $("#txtRoadFromChainage").val($("#PLAN_RD_FROM_CHAINAGE").val());
                $("#rowChainage").show('slow');
            }
            else {
                $("#txtRoadToChainage").val('');
                $("#txtRoadToChainage").attr("readonly", false);
                $("#txtRoadFromChainage").attr("readonly", false);
                $("#txtRoadLength").val('');
                $("#rowChainage").show('slow');
            }
        }
    });

    // Road Length:Fully - changes the Start chainage and end chainage textbox non editable
    $("#radioRoadLengthFully").change(function () {

        $("#txtRoadToChainage").val('');
        $("#txtRoadFromChainage").val('');
        $("#txtRoadLength").val('');
        $("#rowChainage").hide('slow');

        if (($("#radioRoadLengthFully").val()) == "F") {

            $("#txtRoadToChainage").attr("readonly", true);
            $("#txtRoadFromChainage").attr("readonly", true);
            $("#txtRoadLength").attr("readonly", true);
            PopulateChainageLength();
        }
    });

    //validation for entering correct value of start chainage and end chainage
    $("#txtRoadToChainage").blur(function () {
        ValidateChainage();
    });

    // display the dropdown according to the Road From dropdown value(i.e Through -- T1,T2.....)
    $("#ddlRoadFrom").change(function () {

        var valRoadFrom = $("#ddlRoadFrom").val();

        switch (valRoadFrom) {
            case "0":
                HideLabels();
                $("#rowBlock").hide();
                $("#lblRoadNumFrom").show();
                $("#listRoadNumFrom").show();
                $("#ddlRoadNumFrom").attr("disabled", true);
                $("#listRoadNumFrom").hide();
                break;
            case "B":
                HideLabels();
                $("#lblBlock").show();
                $("#opBlock").hide();
                $("#rowBlock").show();
                $("#ddlRoadNumFrom").val('');
                $("#ddlRoadNumFrom").attr("disabled", true);
                $("#ddlRoadNumFrom").hide();
                $("#ddlPreviousBlock").attr("disabled", false);
                $("#ddlPreRoadCode").attr("disabled", false);
                break;
            case "H":
                HideLabels();
                $("#colFromHabitation").show();
                $("#listRoadNumFrom").hide();
                $("#colListFromHabitation").show();
                $("#ddlFromHabitation").attr("disabled", false);
                if ($("#EncryptedRoadCode").val() == "") {
                    $("#ddlFromHabitation").find('option[value=0]').remove();
                    $("#ddlFromHabitation").prepend("<option value='0' selected>--Select--</option>");
                }
                break;
            case "L":
                HideLabels();
                $("#lblLink").show();
                $("#ddlRoadNumFrom").show();
                $("#ddlRoadNumFrom").attr("disabled", false);
                $("#ddlRoadNumFrom").show();
                $("#listRoadNumFrom").show();
                ChangeRoadNumFrom();
                break;
            case "M":
                HideLabels();
                $("#lblLink").show();
                $("#ddlRoadNumFrom").show();
                $("#ddlRoadNumFrom").attr("disabled", false);
                $("#ddlRoadNumFrom").show();
                $("#listRoadNumFrom").show();
                ChangeRoadNumFrom();
                break;
            case "T":
                HideLabels();
                $("#lblThroughRoute").show();
                $("#ddlRoadNumFrom").show();
                $("#ddlRoadNumFrom").attr("disabled", false);
                $("#ddlRoadNumFrom").show();
                $("#listRoadNumFrom").show();
                ChangeRoadNumFrom();
                break;
            case "D":
                HideLabels();
                $("#lblMasterDistrict").show();
                $("#ddlRoadNumFrom").show();
                $("#ddlRoadNumFrom").attr("disabled", false);
                $("#ddlRoadNumFrom").show();
                $("#listRoadNumFrom").show();
                ChangeRoadNumFrom();
                break;
            case "N":
                HideLabels();
                $("#lblNational").show();
                $("#ddlRoadNumFrom").show();
                $("#ddlRoadNumFrom").attr("disabled", false);
                $("#ddlRoadNumFrom").show();
                $("#listRoadNumFrom").show();
                ChangeRoadNumFrom();
                break;
            case "R":

                HideLabels();
                $("#lblRuralRoad").show();
                $("#ddlRoadNumFrom").show();
                $("#ddlRoadNumFrom").attr("disabled", false);
                $("#ddlRoadNumFrom").show();
                $("#listRoadNumFrom").show();
                ChangeRoadNumFrom();
                break;
            case "Z":
                HideLabels();
                $("#lblRuralRoadTrack").show();
                $("#ddlRoadNumFrom").show();
                $("#ddlRoadNumFrom").attr("disabled", false);
                $("#ddlRoadNumFrom").show();
                $("#listRoadNumFrom").show();
                ChangeRoadNumFrom();
                break;
            case "V":
                HideLabels();
                $("#lblRuralRoadVillage").show();
                $("#ddlRoadNumFrom").show();
                $("#ddlRoadNumFrom").attr("disabled", false);
                $("#ddlRoadNumFrom").show();
                $("#listRoadNumFrom").show();
                ChangeRoadNumFrom();
                break;
            case "S":
                HideLabels();
                $("#lblState").show();
                $("#ddlRoadNumFrom").show();
                $("#ddlRoadNumFrom").attr("disabled", false);
                $("#ddlRoadNumFrom").show();
                ChangeRoadNumFrom();
                break;
            case "O":
                HideLabels();
                $("#lblOthers").show();
                $("#ddlRoadNumFrom").show();
                $("#ddlRoadNumFrom").attr("disabled", false);
                $("#ddlRoadNumFrom").show();
                $("#listRoadNumFrom").show();
                ChangeRoadNumFrom();
                break;
        }
    });


    // display the dropdown according to the Road To dropdown value(i.e Through -- T1,T2.....)
    $("#ddlRoadTo").change(function () {

        var valRoadTo = $("#ddlRoadTo").val();

        switch (valRoadTo) {
            case "0":
                HideLabelsTo();
                $("#rowBlockTo").hide();
                $("#lblRoadNumTo").show();
                $("#listRoadNumTo").show();
                $("#ddlRoadNumTo").attr("disabled", true);
                $("#ddlRoadNumTo").hide();
                break;
            case "B":
                HideLabelsTo();
                $("#lblBlockTo").show();
                $("#rowBlockTo").show();
                $("#opBlockTo").hide();
                $("#ddlRoadNumTo").val('');
                $("#ddlRoadNumTo").attr("disabled", true);
                $("#ddlRoadNumTo").hide();
                $("#ddlNextBlock").attr("disabled", false);
                $("#ddlNextRoadCode").attr("disabled", false);
                break;
            case "H":
                HideLabelsTo();
                $("#colToHabitation").show();
                $("#listRoadNumTo").hide();
                $("#colListToHabitation").show();
                $("#ddlToHabitation").attr("disabled", false);
                if ($("#EncryptedRoadCode").val() == "") {
                    $("#ddlToHabitation").find('option[value=0]').remove();
                    $("#ddlToHabitation").prepend("<option value='0' selected>--Select--</option>");
                }

                break;
            case "L":

                HideLabelsTo();
                $("#lblLinkTo").show();
                $("#ddlRoadNumTo").show();
                $("#ddlRoadNumTo").attr("disabled", false);
                ChangeRoadNumTo();
                break;
            case "M":

                HideLabelsTo();
                $("#lblLinkTo").show();
                $("#ddlRoadNumTo").show();
                $("#ddlRoadNumTo").attr("disabled", false);
                ChangeRoadNumTo();
                break;
            case "T":

                HideLabelsTo();
                $("#lblThroughRouteTo").show();
                $("#ddlRoadNumTo").show();
                $("#ddlRoadNumTo").attr("disabled", false);
                ChangeRoadNumTo();
                break;
            case "D":

                HideLabelsTo();
                $("#lblMasterDistrictTo").show();
                $("#ddlRoadNumTo").show();
                $("#ddlRoadNumTo").attr("disabled", false);
                ChangeRoadNumTo();
                break;
            case "N":

                HideLabelsTo();
                $("#lblNationalTo").show();
                $("#ddlRoadNumTo").show();
                $("#ddlRoadNumTo").attr("disabled", false);
                ChangeRoadNumTo();
                break;
            case "R":

                HideLabelsTo();
                $("#lblRuralRoadTo").show();
                $("#ddlRoadNumTo").show();
                $("#ddlRoadNumTo").attr("disabled", false);
                ChangeRoadNumTo();
                break;
            case "Z":

                HideLabelsTo();
                $("#lblRuralRoadTrackTo").show();
                $("#ddlRoadNumTo").show();
                $("#ddlRoadNumTo").attr("disabled", false);
                ChangeRoadNumTo();
                break;
            case "V":

                HideLabelsTo();
                $("#lblRuralRoadVillageTo").show();
                $("#ddlRoadNumTo").show();
                $("#ddlRoadNumTo").attr("disabled", false);
                ChangeRoadNumTo();
                break;
            case "S":

                HideLabelsTo();
                $("#lblStateTo").show();
                $("#ddlRoadNumTo").show();
                $("#ddlRoadNumTo").attr("disabled", false);
                ChangeRoadNumTo();
                break;
            case "O":

                HideLabelsTo();
                $("#lblOthersTo").show();
                $("#ddlRoadNumTo").show();
                $("#ddlRoadNumTo").attr("disabled", false);
                ChangeRoadNumTo();
                break;
        }
    });


    // returns routes according to tke selected previous block
    $("#ddlPreviousBlock").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlPreviousBlock").find(":selected").val() },
                   "#ddlPreRoadCode", "/CoreNetwork/GetPreviousBlockByBlockCode?blockName=" + $('#ddlPreviousBlock option:selected').val());
    });

    // returns routes according to tke selected next block
    $("#ddlNextBlock").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlNextBlock").find(":selected").val() },
                   "#ddlNextRoadCode", "/CoreNetwork/GetPreviousBlockByBlockCode?blockName=" + $('#ddlNextBlock option:selected').val());

    });


    // Save button click
    $("#btnSave").click(function () {

        if ($("#ddlRoadCategory").val() == 0) {
            $("#msgValidationCategory").html("<span style='color:red'>Please select road category</span>");
            return false;
        }
        else {
            $("#msgValidationCategory").html('');
        }

        $("#PLAN_RD_LENGTH").val($("#txtRoadLength").html());

        ValidateChainage();

        if ($("#frmAddCoreNetwork").valid()) {
            //if (true) {
            $.ajax({
                type: 'POST',
                url: '/CoreNetwork/AddCoreNetworksPMGSY3/',
                async: false,
                data: $("#frmAddCoreNetwork").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                success: function (data) {
                    unblockPage();
                    if (data.success === undefined) {
                        $("#divAddForm").html(data);
                    }
                    else if (data.success) {
                        alert(data.message);
                        CloseProposalDetails();
                        searchDetails();
                        $("#networkCategory").trigger("reloadGrid");
                        unblockPage();
                    }
                    else {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        $.validator.unobtrusive.parse($('#mainDiv'));
                        unblockPage();
                    }

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert('Error occurred');
                }
            })

        }
    });


    //Update button click
    $("#btnUpdate").click(function () {

        $("#PLAN_RD_LENGTH").val($("#txtRoadLength").html());

        ValidateChainage();

        ///Commented on 14JUN2018 as per directions from Pankaj Sir.
        /*if ($("#unlockFlag").val() == "M") {
            CheckPavementLength($("#PLAN_CN_ROAD_CODE").val(), $("#txtRoadFromChainage").val(), $("#txtRoadToChainage").val());
            if (isValid == false) {
                return false;
            }
        }*/

        //checks whether old data in Road From dropdown
        if (($("#FROM_TYPE").val() != 'B') && ($("#FROM_TYPE").val() != 'H')) {
            if ($("#ddlRoadNumFrom").val() <= 0) {

                $("#ddlRoadFromMessage").html("<span style='color:red'><b>Please select Road other than this value.</b></span>");
                $("#ddlRoadFromMessage").show();
                return false;
            }
        }

        //checks whether old data in Road To dropdown
        if (($("#TO_TYPE").val() != 'B') && ($("#TO_TYPE").val() != 'H')) {
            if ($("#ddlRoadNumTo").val() <= 0) {

                $("#ddlRoadToMessage").html("<span style='color:red'><b>Please select Road other than this value.</b></span>");
                $("#ddlRoadToMessage").show();
                return false;
            }
        }

        $("#PLAN_RD_FROM_CHAINAGE").val(($("#txtRoadFromChainage").val()));
        $("#PLAN_RD_TO_CHAINAGE").val($("#txtRoadToChainage").val());

        if ($("#frmAddCoreNetwork").valid()) {

            $("#PLAN_RD_LENGTH").val($("#PLAN_RD_TO_CHAINAGE").val() - $("#PLAN_RD_FROM_CHAINAGE").val());
            $.ajax({
                type: 'POST',
                url: '/CoreNetwork/EditCoreNetworksPMGSY3/',
                async: false,
                data: $("#frmAddCoreNetwork").serialize(),
                success: function (data) {
                    if (data.success === undefined) {

                        $("#divAddForm").html(data)
                    } else if (data.success == true) {
                        unblockPage();
                        alert(data.message);
                        CloseProposalDetails();
                        $("#networkCategory").trigger("reloadGrid");
                    }
                    else {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert('Error occurred');
                }
            })
        }
    });

    // Cancel button click
    $("#btnCancel").click(function () {
        CloseProposalDetails();
    });

    $("#btnReset").click(function () {

        $('#ddlRoadNumFrom').find('option:first').attr('selected', 'selected');
        $("#ddlToHabitation").val('');
        $("#lblRoadCode").html('NA');
        $("#txtRoadLength").html('0');
        $("#divError").hide('slow');
        $("#rowChainage").hide();
    })

    // Populates Start Chainage and End Chainage textbox if Road Length selected is fully
    $("#ddlRoadCode").change(function () {

        if ($("#radioRoadLengthFully").val() == "F") {
            PopulateChainageLength();
        }
        else {
            $("#txtRoadFromChainage").val('');
            $("#txtRoadToChainage").val('');
            $("#txtRoadLength").val('');
        }
    });


    $("#ddlRoadFrom").trigger('change');
    $("#ddlRoadTo").trigger('change');


});

function ChangeRoadNumFrom() {

    FillInCascadeDropdown({ userType: $("#ddlRoadFrom").find(":selected").val() },
                       "#ddlRoadNumFrom", "/CoreNetwork/GetRoadNumFromByRoadFrom?roadFrom=" + $('#ddlRoadFrom option:selected').val() + "&blockCode=" + $("#MAST_BLOCK_CODE").val() + "&selectedCode=" + $("#NUM_FROM").val());

    if ($("#EncryptedRoadCode").val() != null && $("#NUM_FROM").val() == "") {

        var fromRoadType = $("#FROM_TYPE").val();
        if ($("#ddlRoadFrom").val() == fromRoadType) {
            $("#ddlRoadNumFrom").append("<option value='0' selected>" + $("#RD_FROM").val() + "</option>");
        }
    }
}

function ChangeRoadNumTo() {

    FillInCascadeDropdown({ userType: $("#ddlRoadTo").find(":selected").val() },
                       "#ddlRoadNumTo", "/CoreNetwork/GetRoadNumFromByRoadTo?roadFrom=" + $('#ddlRoadTo option:selected').val() + "&blockCode=" + $("#MAST_BLOCK_CODE").val() + "&selectedCode=" + $("#NUM_TO").val());

    if ($("#EncryptedRoadCode").val() != null && $("#NUM_TO").val() == "") {

        var toRoadType = $("#TO_TYPE").val();

        if ($("#ddlRoadTo").val() == toRoadType) {

            $("#ddlRoadNumTo").append("<option value='0' selected>" + $("#RD_TO").val() + "</option>");
        }
    }
}

function HideLabels() {
    $("#rowBlock").hide();
    $("#lblLink").hide();
    $("#lblRoadNumFrom").hide();
    $("#lblThroughRoute").hide();
    $("#lblMasterDistrict").hide();
    $("#lblNational").hide();
    $("#lblRuralRoad").hide();
    $("#lblRuralRoadTrack").hide();
    $("#lblRuralRoadVillage").hide();
    $("#lblState").hide();
    $("#colFromHabitation").hide();
    $("#lblBlock").hide();
    $("#lblOthers").hide();
    if (($("#colListFromHabitation").is(":visible"))) {
        $("#colListFromHabitation").hide();
        $("#listRoadNumFrom").show();
    }

}

function HideLabelsTo() {
    $("#rowBlockTo").hide();
    $("#lblLinkTo").hide();
    $("#lblRoadNumTo").hide();
    $("#lblThroughRouteTo").hide();
    $("#lblMasterDistrictTo").hide();
    $("#lblNationalTo").hide();
    $("#lblRuralRoadTo").hide();
    $("#lblRuralRoadTrackTo").hide();
    $("#lblRuralRoadVillageTo").hide();
    $("#lblStateTo").hide();
    $("#colToHabitation").hide();
    $("#lblBlockTo").hide();
    $("#lblOthersTo").hide();
    if (($("#colListToHabitation").is(":visible"))) {
        $("#colListToHabitation").hide();
        $("#listRoadNumTo").show();
    }
}

function FillInCascadeDropdown(map, dropdown, action) {
    var message = '';

    if (dropdown == '#ddlRoadCode') {
        message = '<h4><label style="font-weight:normal"> Loading RoadNames... </label></h4>';
    }

    $(dropdown).empty();

    $.post(action, map, function (data) {
        $.each(data, function () {

            if (this.Selected == true) {
                $(dropdown).append("<option selected value=" + this.Value + ">" + this.Text + "</option>");
            }
            else {
                $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            }
        });
    }, "json");
}


// returns the Start Chainage and End Chainage values according to the road name selected
function PopulateChainageLength() {

    if ($("#EncryptedRoadCode").val() != "") {
        $("#txtRoadFromChainage").attr("readOnly", true);
        $("#txtRoadToChainage").attr("readOnly", true);
        $("#txtRoadFromChainage").val($("#ExistStartChainage").val());
        $("#txtRoadToChainage").val($("#ExistEndChainage").val());
        $("#txtRoadLength").html(parseFloat($("#ExistEndChainage").val() - $("#ExistStartChainage").val()).toFixed(3));
        $("#lblStartChainage").html($("#ExistStartChainage").val());
        $("#lblEndChainage").html($("#ExistEndChainage").val());
    }
    else {

        var existingRoadCode = $("#ddlRoadCode option:selected").val();
        $.ajax({
            type: 'POST',
            url: '/CoreNetWork/GetChainageLength',
            data: { roadCode: existingRoadCode },
            async: false,
            cache: false,
            success: function (data) {

                var startChainage = data.startChainage;
                var endChainage = data.endChainage;
                var roadLength = data.roadLength;
                $("#txtRoadFromChainage").attr("readOnly", true);
                $("#txtRoadToChainage").attr("readOnly", true);
                $("#txtRoadFromChainage").val(startChainage);
                $("#txtRoadToChainage").val(endChainage);
                $("#txtRoadLength").html(parseFloat(roadLength).toFixed(3));
                $("#lblStartChainage").html(startChainage);
                $("#lblEndChainage").html(endChainage);
                $("#ExistStartChainage").val(startChainage);
                $("#ExistEndChainage").val(endChainage);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(thrownError);
                unblockPage();
            }
        })
    }
}
function ValidateChainage() {

    var roadTo = $("#txtRoadToChainage").val();
    var roadFrom = $("#txtRoadFromChainage").val();
    if (roadFrom == "") {
        $("#validFromChainage").show();
        return false;
    }
    else if (roadTo < roadFrom) {
        $("#validToChainage").show();
        return false;
    }
    else {
        
        var roadLength = roadTo - roadFrom;
        if (parseFloat($('#lblEndChainage').text()) > parseFloat(roadLength)) {
            $("#txtRoadLength").html(parseFloat(roadLength).toFixed(3));
        }
    }
}

function CheckPavementLength(proposalCode, startChainage, endChainage) {
    var chainage = parseFloat(endChainage) - parseFloat(startChainage)
    var data = proposalCode + ',' + chainage;

    $.ajax({
        type: 'POST',
        url: '/CoreNetwork/CheckPavementLength?id=' + data,
        async: false,
        cache: false,
        success: function (data) {
            if (data.success == false) {
                isValid = false;
                //alert('Chainage can not be changed as Proposal is already made against this length.');
                alert('Please verify the length of the proposals already mapped, if any');
            }
            else {
                isValid = true;
            }
        },
        error: function ()
        { }
    });
}
