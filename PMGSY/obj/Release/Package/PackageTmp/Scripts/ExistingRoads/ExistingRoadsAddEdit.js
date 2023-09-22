$.validator.addMethod("compareyear", function (value, element, params) {

    if (($("#MAST_CONS_YEAR").val()) < ($("#MAST_RENEW_YEAR").val())) {
        if ($("#MAST_ROAD_CAT_CODE").val() == 6) {
            return true;
        }
        return true;
    }
    return false;
});
jQuery.validator.unobtrusive.adapters.addBool("compareyear");

$.validator.addMethod("comparereason", function (value, element, params) {

    if ($("#radioHabNo").is(':checked')) {
        if ($("#ddlMastNoHabsReason").val() != 0) {
            return true;
        }
    }

    return false;
});
jQuery.validator.unobtrusive.adapters.addBool("comparereason");

$.validator.addMethod("checkroadtype", function (value, element, params) {

    if ($("#MAST_ROAD_CAT_CODE").val() == 6) {
        if ($("#MAST_CONS_YEAR").val() == 0) {
            return true;
        }

        if ($("#MAST_RENEW_YEAR").val() == 0) {
            return true;
        }
    }

    return false;
});
jQuery.validator.unobtrusive.adapters.addBool("checkroadtype");

var isValid = true;

$(document).ready(function () {


    $("#ddlMastNoHabsReason option").each(function (i) {
        $(this).attr('title', $(this).text());
    });


    $.validator.unobtrusive.parse($('frmCreateExistingRoad'));


    //set year default to 1950
    if ($("#EncryptedRoadCode").val() == "") {
        $('#MAST_CONS_YEAR option:last').attr("selected", "selected");
        $('#MAST_RENEW_YEAR option:last').attr("selected", "selected");

    }




    $("#reasonLabel").hide();
    $("#reasonDdl").hide()
    $("#reasonTD").show();

    if ($("#EncryptedRoadCode").val() != "") {
        if ($("#radioHabNo").is(':checked')) {
            $("#reasonLabel").show("slow");
            $("#reasonDdl").show("slow");
            $("#reasonTD").hide();
        }
    }

    $("input:radio[id=radioHabNo]").live('click', function () {

        if ($(this).attr("checked")) {
            $("#reasonLabel").show("slow");
            $("#reasonDdl").show("slow");
            $("#reasonTD").hide();
        }
        else {

            $("#reasonTD").show();
            $("#reasonLabel").hide();
            $("#reasonDdl").hide();
            $("#ddlMastNoHabsReason").val("");

        }

    });

    if ($("#EncryptedRoadCode").val() != "") {




        $("#radioHabNo").click(function () {

            $.ajax({
                url: '/ExistingRoads/CheckMapHabitation/' + $("#EncryptedRoadCode").val(),
                type: 'POST',
                beforeSend: function () {
                    blockPage();
                },
                success: function (jsonData) {

                    unblockPage();
                    if (jsonData.success == false) {
                        alert("The Habitations are already mapped to this road.So please delete the habitation to change the status.");
                        $("#radioHabYes").attr("checked", 'checked');
                        $("#reasonTD").show();
                        $("#reasonLabel").hide();
                        $("#reasonDdl").hide();
                        $("#ddlMastNoHabsReason").val("");
                        return false;
                    }
                },
                error: function (err) {
                    alert("error " + err);
                    unblockPage();
                }
            });


            if ($("#radioHabNo").is(':checked')) {
                $("#reasonLabel").show("slow");
                $("#reasonDdl").show("slow");
                $("#reasonTD").hide();

            }

        });

    }




    $("input:radio[id=radioHabYes]").live('click', function () {
        $("#reasonTD").show();
        $("#reasonLabel").hide();
        $("#reasonDdl").hide();
        $("#ddlMastNoHabsReason").val("");
    });

    //set Bro Road Owner
    if ($("#MAST_ROAD_CAT_CODE").val() == 8) {
        PopulateBroRoadOwners();
    }

    //BRO EDIT Disabled ROAD Owner
    var selectedValue = $("#MAST_ROAD_CAT_CODE option:selected").text();
    if (selectedValue == "Border Roads") {
        $("#MAST_ER_ROAD_OWNER").attr('disabled', true);
    }


    //Rural Road(Track)
    if (selectedValue == "Rural Road(Track)") {
        $("#MAST_CONS_YEAR").attr('disabled', true);
        $("#MAST_RENEW_YEAR").attr('disabled', true);
        $("#showYear").hide();

    }

    $("input[type='reset']").on("click", function (event) {
        ResetForm();
    });

    if ($("#EncryptedRoadCode").val() == null) {

        $('#MAST_ROAD_CAT_CODE option[selected="selected"]').each(
        function () {
            $(this).removeAttr('selected');
        }
    );
        $("#MAST_ROAD_CAT_CODE option:first").attr('selected', 'selected');

    }

    if ($("#EncryptedRoadCode").val() == "") {
        $('#MAST_ROAD_CAT_CODE option:first').attr("selected", "selected");
    }


    //$('select option:first').attr("selected", "selected");

    $("#btnReset").click(function () {

        //enabled dropdown
        if ($('#MAST_CONS_YEAR').is(':disabled')) {
            $("#MAST_CONS_YEAR").attr('disabled', false);
        }
        if ($('#MAST_RENEW_YEAR').is(':disabled')) {
            $("#MAST_RENEW_YEAR").attr('disabled', false);
        }

        if ($('#MAST_ER_ROAD_OWNER').is(':disabled')) {
            $("#MAST_ER_ROAD_OWNER").attr('disabled', false);
        }

        //hide error alert
        if ($("#divError").is(":visible")) {
            $("#divError").hide("slow");
        }

        $("#RoadEndChainage").html('');
        PopulateRoadOwners();

    });

    if ($("#EncryptedRoadCode").val() == "") {
        $(function () {
            $("#MAST_ER_ROAD_NUMBER").focus();
        });
    }


    $("#MAST_ROAD_CAT_CODE").change(function () {
        //alert($('#hdnPMGSYScheme').val());  //&& $('#EncryptedRoadCode').val() != ''
        //alert($('#MAST_ER_ROAD_CODE_PMGSY1').val());

        //if ($('#EncryptedRoadCode').val() != null && ((parseInt($('#MAST_ROAD_CAT_CODE').val()) > 0 && parseInt($('#MAST_ER_ROAD_CODE_PMGSY1').val()) > 0) || $('#hdnPMGSYScheme').val() == 1)) {
        ///Changed by SAMMED A. PATIL on 01JAN2018 to allow changing of Road Category on edit for PMGSY1 & 2 unconditionally
        if ($('#EncryptedRoadCode').val() != null && $('#EncryptedRoadCode').val() != '') {
            //if ($("#MAST_ROAD_CAT_CODE option:selected").val() <= 0)
            //{
            //    alert('Please select a valid Road Category');
            //    $("#MAST_ROAD_CAT_CODE").val($('#hdnRoadCategoryCode').val());
            //    return false;
            //}

            $('#MAST_ER_SHORT_DESC').show();
            $('#MAST_ER_ROAD_NUMBER').val('');
            $('#MAST_ER_ROAD_NUMBER').val('');
            $('#MAST_ER_ROAD_NUMBER').show()
            $('#lb_MAST_ER_ROAD_NUMBER').hide();

        }

        $.ajax({
            url: '/ExistingRoads/GetRoadShortName',
            type: 'POST',
            beforeSend: function () {
                blockPage();
            },
            data: { roadCategoryCode: $("#MAST_ROAD_CAT_CODE").val() },
            success: function (jsonData) {
                $("#MAST_ER_SHORT_DESC").val(jsonData.RoadShortName);
                unblockPage();
            },
            error: function (err) {
                alert("error " + err);
                unblockPage();
            }
        });

        var selectedValue = $("#MAST_ROAD_CAT_CODE option:selected").text();

        if (selectedValue == "Rural Road(Track)") {

            $("#MAST_CONS_YEAR").val("");
            $("#MAST_RENEW_YEAR").val("");

            $("#MAST_CONS_YEAR").attr('disabled', true);
            $("#MAST_RENEW_YEAR").attr('disabled', true);
            $("#showYear").hide('slow');
        }
        else {

            if ($('#MAST_CONS_YEAR').is(':disabled')) {
                $("#MAST_CONS_YEAR").attr('disabled', false);
                $("#MAST_RENEW_YEAR").attr('disabled', false);
                $("#showYear").show();
            }
        }

        if (selectedValue == "Border Roads") {
            PopulateBroRoadOwners();
            $("#MAST_ER_ROAD_OWNER").attr('disabled', true);
        }
        else {
            $("#MAST_ER_ROAD_OWNER").attr('disabled', false);
            PopulateRoadOwners();
        }
    });

    $('#btnCreate').click(function (evt) {

        evt.preventDefault();
        ValidateRoadNumber();

        var MAST_ROAD_CAT_CODE = $("#MAST_ROAD_CAT_CODE option:selected").val();
        if (MAST_ROAD_CAT_CODE == 6) {
            $('#MAST_CONS_YEAR').val('1950');
            $('#MAST_RENEW_YEAR').val('1950');
        }

        if ($('#frmCreateExistingRoad').valid()) {

            if (validateForm() == true) {
                var selectedValue = $("#MAST_ROAD_CAT_CODE option:selected").text();
                if (selectedValue == "Border Roads") {
                    $("#MAST_ER_ROAD_OWNER").attr('disabled', false);
                }

                var CategoryCodeOfRoad = $("#MAST_ROAD_CAT_CODE").val();
                $('#MAST_CONS_YEAR').attr('disabled', false);
                $('#MAST_RENEW_YEAR').attr('disabled', false);

                $.ajax({
                    url: '/ExistingRoads/AddExistingRoads',
                    type: "POST",
                    cache: false,
                    data: $("#frmCreateExistingRoad").serialize(),
                    beforeSend: function () {
                        blockPage();
                    },
                    error: function (xhr, status, error) {
                        unblockPage();

                        if ($("#MAST_ROAD_CAT_CODE").val() == 6 || $("#MAST_ROAD_CAT_CODE").text() == "Rural Road(Track)") {
                            $('#MAST_CONS_YEAR').attr('disabled', true);
                            $('#MAST_RENEW_YEAR').attr('disabled', true);
                        }

                        //BRO
                        if (selectedValue == "Border Roads") {
                            $("#MAST_ER_ROAD_OWNER").attr('disabled', true);
                        }

                        Alert("Request can not be processed at this time,please try after some time!!!");
                        return false;
                    },

                    success: function (response) {

                        if ($("#MAST_ROAD_CAT_CODE").val() == 6 || $("#MAST_ROAD_CAT_CODE").text() == "Rural Road(Track)") {
                            $('#MAST_CONS_YEAR').attr('disabled', true);
                            $('#MAST_RENEW_YEAR').attr('disabled', true);
                        }
                        if (response.success === undefined) {
                            $("#divExistingRoadsForm").html(response);
                        }
                        else if (response.success) {
                            // $('#tbExistingRoadsList').jqGrid('GridUnload');
                            alert(response.message);
                            $("#btnReset").trigger("click");
                            //ResetForm();
                            //CloseExistingRoadsDetails();


                            //     $('#tbExistingRoadsList').jqGrid('GridUnload');
                            //     LoadExistingRoads(MAST_BLOCK_CODE, MAST_ROAD_CAT_CODE);

                            unblockPage();
                            //$("#tbExistingRoadsList").trigger("reloadGrid");

                            //$("#btnListExistingRoads").trigger('click');
                            /*$('#accordion').hide('slow');
                            $('#divExistingRoadsForm').hide('slow');
                            showFilter();
                            LoadExistingRoads1();*/
                            ShowExistingRoads();

                            $("#tbExistingRoadsList").trigger("reloadGrid");
                        }
                        else {
                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.message);
                            //disabled start
                            if ($("#MAST_ROAD_CAT_CODE").val() == 6 || $("#MAST_ROAD_CAT_CODE").text() == "Rural Road(Track)") {
                                $('#MAST_CONS_YEAR').attr('disabled', true);
                                $('#MAST_RENEW_YEAR').attr('disabled', true);
                            }
                            //BRO
                            if (selectedValue == "Border Roads") {
                                $("#MAST_ER_ROAD_OWNER").attr('disabled', true);
                            }
                            //disabled end
                            unblockPage();
                        }
                        unblockPage();
                    }
                });//end of ajax
            }
        }
        else {
            $('.qtip').show();
        }


    });

    $('#btnUpdate').click(function (evt) {

        var MAST_BLOCK_CODE = $("#ddlBlocks option:selected").val();
        var MAST_ROAD_CAT_CODE = $("#MAST_ROAD_CAT_CODE option:selected").val();
        $("#ddlRoadCategory").val(MAST_ROAD_CAT_CODE);

        evt.preventDefault();

        if (MAST_ROAD_CAT_CODE == 6) {
            $('#MAST_CONS_YEAR').val('1950');
            $('#MAST_RENEW_YEAR').val('1950');
        }




        if ($('#frmCreateExistingRoad').valid()) {

            if (validateForm() == true) {

                //BRO Edit Road Owner
                var selectedValue = $("#MAST_ROAD_CAT_CODE option:selected").text();
                if (selectedValue == "Border Roads") {
                    $("#MAST_ER_ROAD_OWNER").attr('disabled', false);
                }

                if ($("#isSurfaceCbrDetails").val() != 0) {
                    $("#MAST_ER_ROAD_STR_CHAIN").attr("disabled", false);
                    $("#MAST_ER_ROAD_END_CHAIN").attr("disabled", false);
                }

                $('#MAST_CONS_YEAR').attr('disabled', false);
                $('#MAST_RENEW_YEAR').attr('disabled', false);


                // when the details are provided after unlocking by MoRD (LockUnlockFlag = M)
                $("#MAST_TERRAIN_TYPE_CODE").attr('disabled', false);
                $("#MAST_SOIL_TYPE_CODE").attr('disabled', false);
                $("#MAST_CONS_YEAR").attr('disabled', false);
                $("#MAST_RENEW_YEAR").attr('disabled', false);
                $("#MAST_ER_ROAD_L_WIDTH").attr('disabled', false);
                $("#MAST_ER_ROAD_F_WIDTH").attr('disabled', false);
                $("#MAST_ER_ROAD_C_WIDTH").attr('disabled', false);
                $("#MAST_ER_ROAD_STR_CHAIN").attr('disabled', false);
                $("#MAST_ER_ROAD_END_CHAIN").attr('disabled', false);

                $("#MAST_ROAD_CAT_CODE").attr("disabled", false);

                $.ajax({
                    url: '/ExistingRoads/EditExistingRoads',
                    type: "POST",
                    cache: false,
                    data: $("#frmCreateExistingRoad").serialize(),
                    beforeSend: function () {
                        blockPage();
                    },
                    error: function (xhr, status, error) {

                        //BRO Edit Road Owner
                        var selectedValue = $("#MAST_ROAD_CAT_CODE option:selected").text();
                        if (selectedValue == "Border Roads") {
                            $("#MAST_ER_ROAD_OWNER").attr('disabled', true);
                        }
                        if ($("#isSurfaceCbrDetails").val() != 0) {
                            $("#MAST_ER_ROAD_STR_CHAIN").attr("disabled", true);
                            $("#MAST_ER_ROAD_END_CHAIN").attr("disabled", true);
                        }
                        //Track Road
                        if ($("#MAST_ROAD_CAT_CODE").val() == 6 || $("#MAST_ROAD_CAT_CODE").text() == "Rural Road(Track)") {
                            $('#MAST_CONS_YEAR').attr('disabled', true);
                            $('#MAST_RENEW_YEAR').attr('disabled', true);
                        }
                        $("#MAST_ROAD_CAT_CODE").attr("disabled", false);


                        unblockPage();
                        Alert("Request can not be processed at this time,please try after some time!!!");
                        return false;
                    },
                    success: function (response) {
                        $("#MAST_ROAD_CAT_CODE").attr("disabled", false);
                        if (response.success) {
                            alert(response.message);
                            $("#btnReset").trigger("click");
                            //ResetForm();
                            // CloseExistingRoadsDetails();
                            ShowExistingRoads();

                            $("#tbExistingRoadsList").trigger("reloadGrid");

                            //$('#tbExistingRoadsList').jqGrid('GridUnload');
                            //LoadExistingRoads(MAST_BLOCK_CODE, MAST_ROAD_CAT_CODE);                         

                            unblockPage();
                        }
                        else {
                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.message);

                            //disable start 

                            //BRO Edit Road Owner
                            var selectedValue = $("#MAST_ROAD_CAT_CODE option:selected").text();
                            if (selectedValue == "Border Roads") {
                                $("#MAST_ER_ROAD_OWNER").attr('disabled', true);
                            }
                            if ($("#isSurfaceCbrDetails").val() != 0) {
                                $("#MAST_ER_ROAD_STR_CHAIN").attr("disabled", true);
                                $("#MAST_ER_ROAD_END_CHAIN").attr("disabled", true);
                            }
                            //Track Road
                            if ($("#MAST_ROAD_CAT_CODE").val() == 6 || $("#MAST_ROAD_CAT_CODE").text() == "Rural Road(Track)") {
                                $('#MAST_CONS_YEAR').attr('disabled', true);
                                $('#MAST_RENEW_YEAR').attr('disabled', true);
                            }
                            $("#MAST_ROAD_CAT_CODE").attr("disabled", true);

                            //disable end

                            unblockPage();


                        }
                        unblockPage();
                    }
                });//end of ajax

            }
        }
        else {
            $('.qtip').show();
        }




    });

    //allow only digits and .
    $("#MAST_ER_ROAD_END_CHAIN").keypress(function (e) {
        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 46 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });
    $("#MAST_ER_ROAD_STR_CHAIN").keypress(function (e) {
        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 46 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });


    $("#MAST_ER_ROAD_L_WIDTH").keypress(function (e) {
        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 46 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });
    $("#MAST_ER_ROAD_F_WIDTH").keypress(function (e) {
        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 46 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });
    $("#MAST_ER_ROAD_C_WIDTH").keypress(function (e) {
        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 46 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });

    //compare logic for MAST_ER_ROAD_STR_CHAIN < MAST_ER_ROAD_END_CHAIN 
    $("#MAST_ER_ROAD_END_CHAIN").blur(function () {

        var startChainage = parseFloat($("#MAST_ER_ROAD_STR_CHAIN").val());
        var endChainage = parseFloat($("#MAST_ER_ROAD_END_CHAIN").val());

        if (startChainage != "" && startChainage >= endChainage) {
            //$("#divError").show("slow");
            //$("#divError span:eq(1)").html('<strong>Alert: </strong>' + "Road end chainage should be greater than road start chainage.");

            $("#RoadEndChainage").show("slow");
            $("#RoadEndChainage").html("<span style='color:red'><b>Road end chainage should be greater than road start chainage.</b></span>");

        }
        //else {
        //    //if ($("#divError").is(":visible")) {
        //    //    $("#divError").hide("slow");
        //    //    $("#RoadEndChainage").hide("slow");
        //    //}
        //    $("#RoadEndChainage").hide("slow");
        //}
    });

    //compare logic for MAST_ER_ROAD_STR_CHAIN < MAST_ER_ROAD_END_CHAIN 


    $("#MAST_ER_ROAD_NUMBER").blur(function () {

        ValidateRoadNumber();

    });

    $("#MAST_ER_ROAD_STR_CHAIN").blur(function () {

        var startChainage = parseFloat($("#MAST_ER_ROAD_STR_CHAIN").val());
        var endChainage = parseFloat($("#MAST_ER_ROAD_END_CHAIN").val());

        if (endChainage != "" && startChainage >= endChainage) {
            // $("#divError").show("slow");
            //$("#divError span:eq(1)").html('<strong>Alert: </strong>' + "Road start chainage should be less than road end chainage.");
            $("#RoadStartChainage").show("slow");
            $("#RoadStartChainage").html("<span style='color:red'><b>Road start chainage should be less than road end chainage.</b></span>");

        }
        //else {
        //    //if ($("#divError").is(":visible")) {
        //    //    $("#divError").hide("slow");
        //    //}
        //    $("#RoadStartChainage").hide("slow");
        //}
    });

    $("#MAST_ER_ROAD_F_WIDTH").blur(function () {

        var landWidth = parseFloat($("#MAST_ER_ROAD_L_WIDTH").val());
        var formationWidth = parseFloat($("#MAST_ER_ROAD_F_WIDTH").val());
        var carriagewayWidth = parseFloat($("#MAST_ER_ROAD_C_WIDTH").val());

        if (landWidth != "" && landWidth < formationWidth) {

            //$("#divError").show("slow");
            //$("#divError span:eq(1)").html('<strong>Alert: </strong>' + "Formation Width should be less than land width.");
            $("#formationWidth").show("slow");
            $("#formationWidth").html("<span style='color:red'><b>Formation Width should be less than or equal to land width.</b></span>");

        }
        if (carriagewayWidth != "" && formationWidth < carriagewayWidth) {
            //$("#divError").show("slow");
            //$("#divError span:eq(1)").html('<strong>Alert: </strong>' + "Formation width should be greater than carriageway width.");
            $("#formationWidth").show("slow");
            $("#formationWidth").html("<span style='color:red'><b>Formation width should be greater than or equal to carriageway width.</b></span>");
        }
    });

    $("#MAST_ER_ROAD_L_WIDTH").blur(function () {

        var landWidth = parseFloat($("#MAST_ER_ROAD_L_WIDTH").val());
        var formationWidth = parseFloat($("#MAST_ER_ROAD_F_WIDTH").val());

        if (formationWidth != "" && landWidth < formationWidth) {
            $("#landWidth").show("slow");
            $("#landWidth").html("<span style='color:red'><b>Land Width should be greater than or equal to formation width.</b></span>");
        }
        //else {
        //    if ($("#divError").is(":visible")) {
        //        $("#divError").hide("slow");
        //    }
        //}
    });

    $("#MAST_ER_ROAD_C_WIDTH").blur(function () {

        var carriagewayWidth = parseFloat($("#MAST_ER_ROAD_C_WIDTH").val());
        var formationWidth = parseFloat($("#MAST_ER_ROAD_F_WIDTH").val());

        if (formationWidth != "" && formationWidth < carriagewayWidth) {

            $("#carriagewayWidth").show("slow");
            $("#carriagewayWidth").html("<span style='color:red'><b>Carriageway width should be less than or equal to formation width.</b></span>");

            //$("#divError").show("slow");
            //$("#divError span:eq(1)").html('<strong>Alert: </strong>' + "Carriageway width should be less than formation width.");
        }
        //else {
        //    if ($("#divError").is(":visible")) {
        //        $("#divError").hide("slow");
        //    }
        //}
    });


    //btnCancel
    $("#btnCancel").click(function () {
        ShowExistingRoads();
    });

    $(":checkbox").change(function () {

        if ($(this).attr("checked")) {
            $("#reasonLabel").show("slow");
            $("#reasonDdl").show("slow");
            $("#reasonTD").hide();
        }
        else {

            $("#reasonTD").show();
            $("#reasonLabel").hide();
            $("#reasonDdl").hide();
            //ddlMastNoHabsReason
            $("#ddlMastNoHabsReason").val("");

        }
    });


    $("#ddlMastNoHabsReason").mouseover(
    function (event) {
        if ($("#ddlMastNoHabsReason").val() != 0) {

            $('#ddlMastNoHabsReason').attr('title', $("#ddlMastNoHabsReason option:selected").text());

        }
    });

    if ($("#LockUnlockFlag").val() == "M") {
        $("#MAST_TERRAIN_TYPE_CODE").attr('disabled', 'disabled');
        $("#MAST_SOIL_TYPE_CODE").attr('disabled', 'disabled');
        $("#MAST_CONS_YEAR").attr('disabled', 'disabled');
        $("#MAST_RENEW_YEAR").attr('disabled', 'disabled');
        $("#MAST_ER_ROAD_L_WIDTH").attr('disabled', 'disabled');
        $("#MAST_ER_ROAD_F_WIDTH").attr('disabled', 'disabled');
        $("#MAST_ER_ROAD_C_WIDTH").attr('disabled', 'disabled');
        //$("#MAST_ER_ROAD_STR_CHAIN").attr('disabled', 'disabled');
        //$("#MAST_ER_ROAD_END_CHAIN").attr('disabled', 'disabled');
    }

    if ($("#EncryptedRoadCode").val() != "") {

        $("#radioCNNo").click(function () {
            ValidateCoreNetwork($("#EncryptedRoadCode").val());
        });
    }
});


function PopulateBroRoadOwners() {

    $("#MAST_ER_ROAD_OWNER").val(0);
    $("#MAST_ER_ROAD_OWNER").empty();
    $.ajax({
        url: '/ExistingRoads/PopulateBroRoadOwner/',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        success: function (jsonData) {

            for (var i = 0; i < jsonData.length; i++) {
                $("#MAST_ER_ROAD_OWNER").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
            unblockPage();
        },
        error: function (err) {
            alert("error" + err);
            unblockPage();
        }
    });
}

function PopulateRoadOwners() {

    $("#MAST_ER_ROAD_OWNER").val(0);
    $("#MAST_ER_ROAD_OWNER").empty();
    $.ajax({
        url: '/ExistingRoads/PopulateRoadOwner/',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        success: function (jsonData) {

            for (var i = 0; i < jsonData.length; i++) {
                $("#MAST_ER_ROAD_OWNER").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
            unblockPage();
        },
        error: function (err) {
            alert("error" + err);
            unblockPage();
        }
    });
}

function ResetForm() {
    $(':input', '#frmCreateExistingRoad').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');
    $('.qtip').hide();

}

function validateForm() {

    if (($("#chkBenifitedHabitation").is(":checked")) && ($("#ddlMastNoHabsReason").val() == 0)) {

        // alert("please select Reason");
        $("#errMastNoHabsReason").show("slow");
        $("#errMastNoHabsReason").html("<span style='color:red'><b>please select Reason.</b></span>");

        return false;
    }
    else {

        //alert($("#ddlMastNoHabsReason").val());
        $("#errMastNoHabsReason").hide("slow");
        return true;
    }

    return false;

    if ($("#EncryptedRoadCode").val() == null) {
        var roadShortDesc = $("#MAST_ER_SHORT_DESC").val();
        var roadNumber = $("#MAST_ER_ROAD_NUMBER").val();

        if ((roadShortDesc.length + roadNumber.length) > 15) {
            var maxLength = 15;
            var roadLen = maxLength - roadShortDesc.length;
            $("#ValRoadNumber").show("slow");
            $("#ValRoadNumber").html("<span style='color:red'><b>Road number should be less than " + roadLen + " alphanumeric characters</b></span>");
            return false;
        }
        else {
            $("#ValRoadNumber").hide("slow");
        }
    }

    var RoadConstructionYear = $("#MAST_CONS_YEAR option:selected").val();
    var RoadRenewalYear = $("#MAST_RENEW_YEAR option:selected").val();

    if (RoadConstructionYear > RoadRenewalYear) {

        //$("#divError").show("slow");
        //$("#divError span:eq(1)").html('<strong>Alert: </strong>' + "Road Renewal Year must be greater than Road Construction Year.");

        $("#roadRenewalYear").show("slow");
        $("#roadRenewalYear").html("<span style='color:red'><b>Road Renewal Year must be greater than road construction year.</b></span>");

        return false;
    }
    else {
        //if ($("#divError").is(":visible")) {
        //    $("#divError").hide("slow");
        //}
        // return true;
        $("#roadRenewalYear").hide("slow");
    }


    var landWidth = parseFloat($("#MAST_ER_ROAD_L_WIDTH").val());
    var formationWidth = parseFloat($("#MAST_ER_ROAD_F_WIDTH").val());
    var carriagewayWidth = parseFloat($("#MAST_ER_ROAD_C_WIDTH").val());

    if (landWidth < formationWidth) {
        //$("#divError").show("slow");
        //$("#divError span:eq(1)").html('<strong>Alert: </strong>' + "Formation Width should be less than land width.");

        $("#formationWidth").show("slow");
        $("#formationWidth").html("<span style='color:red'><b>Formation Width should be less than or equal to land width.</b></span>");

        return false;
    } else {
        //if ($("#divError").is(":visible")) {
        //    $("#divError").hide("slow");
        //}
        $("#formationWidth").hide("slow");
    }
    if (formationWidth < carriagewayWidth) {

        //$("#divError").show("slow");
        //$("#divError span:eq(1)").html('<strong>Alert: </strong>' + "carriageway width should be less than formation width.");

        $("#carriagewayWidth").show("slow");
        $("#carriagewayWidth").html("<span style='color:red'><b>Carriageway Width should be less than or equal to formation width.</b></span>");

        return false;
    } else {
        //if ($("#divError").is(":visible")) {
        //    $("#divError").hide("slow");
        //}
        $("#carriagewayWidth").hide("slow");
    }

    var startChainage = parseFloat($("#MAST_ER_ROAD_STR_CHAIN").val());
    var endChainage = parseFloat($("#MAST_ER_ROAD_END_CHAIN").val());
    if (startChainage != "" && startChainage >= endChainage) {
        //$("#divError").show("slow");
        //$("#divError span:eq(1)").html('<strong>Alert: </strong>' + "Road end chainage should be greater than road start chainage.");

        $("#RoadEndChainage").show("slow");
        $("#RoadEndChainage").html("<span style='color:red'><b>Road End Chainage should be greater than road start chainage.</b></span>");

        return false;
    } else {
        //if ($("#divError").is(":visible")) {
        //    $("#divError").hide("slow");
        //}
        $("#RoadEndChainage").hide("slow");
        return true;
    }

    //return true;
}

function LoadExistingRoads1(/*MAST_BLOCK_CODE, MAST_ROAD_CAT_CODE*/) {

    //var MAST_BLOCK_CODE = $("#ddlBlocks option:selected").val();
    //var MAST_ROAD_CAT_CODE = $("#ddlRoadCategory option:selected").val();

    jQuery("#tbExistingRoadsList").jqGrid('GridUnload');
    jQuery("#dvExistingRoadsListPager").empty();

    jQuery("#tbExistingRoadsList").jqGrid({
        url: '/ExistingRoads/GetExistingRoadsList/',
        datatype: "json",
        mtype: "POST",
        colNames: ['Existing Road System Id', 'Road Category', 'Road Number', "Road Name", 'Road Type', 'Road Length [in Km]', "Road Owner", "Included in Core Network", "CD Works", "Surface Types", "Habitations", "Traffic Intensity", "CBR Value", "View", "PMGSY1 DRRP Roads", "Edit", "Delete"],
        colModel: [
                        { name: 'ERCode', index: 'HabitationCode', height: 'auto', width: 60, align: "left", sortable: true, search: false },
                        { name: 'MAST_ROAD_SHORT_DESC', index: 'MAST_ROAD_SHORT_DESC', width: 60, sortable: true, align: "left", search: false }, //New
                        { name: 'MAST_ER_ROAD_NUMBER', index: 'MAST_ER_ROAD_NUMBER', width: 90, sortable: true, align: "left", search: false },
                        { name: 'MAST_ER_ROAD_NAME', index: 'MAST_ER_ROAD_NAME', width: 200, sortable: true, align: "left", search: true },
                        { name: 'MAST_ER_ROAD_TYPE', index: 'MAST_ER_ROAD_TYPE', width: 90, sortable: true, align: "left", search: false }, //New
                        { name: 'MAST_ER_ROAD_LENGTH', index: 'MAST_ER_ROAD_LENGTH', width: 100, sortable: true, align: "left", search: false },
                        { name: 'MAST_ER_ROAD_OWNER', index: 'MAST_ER_ROAD_OWNER', width: 80, sortable: true, align: "left", search: false },
                        { name: 'MAST_CORE_NETWORK', index: 'MAST_CORE_NETWORK', width: 97, sortable: true, align: "center", search: false },
                        { name: 'a', width: 70, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: ($("#RoleCode").val() == 22 ? false : true) },
                        { name: 'SurfaceTypes', width: 70, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: ($("#RoleCode").val() == 22 ? false : true) },
                        { name: 'HabitationsMapped', width: 70, sortable: false, resize: false, falign: "center", sortable: false, search: false, hidden: ($("#RoleCode").val() == 22 ? false : true) },
                        { name: 'TrafficIntensity', width: 70, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: ($("#RoleCode").val() == 22 ? false : true) },
                        { name: 'CBRValue', width: 70, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: ($("#RoleCode").val() == 22 ? false : true) },
                        { name: 'ShowDetails', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false },
                        { name: 'MapDRRP', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: (($("#PMGSYScheme").val() == "2" && ($("#RoleCode").val() == 25 || $("#RoleCode").val() == 22)) ? false : true) /*hidden: true*/ },
                        { name: 'Edit', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false, /*hidden: ($("#RoleCode").val() == 22 ? false : true)*/ },
                        { name: 'Delete', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: (($("#RoleCode").val() == 22 || $("#RoleCode").val() == 25) ? ($("#PMGSYScheme").val() == "2" ? true : false) : true) },
        ],
        //postData: { "MAST_BLOCK_CODE": MAST_BLOCK_CODE, "MAST_ROAD_CAT_CODE": MAST_ROAD_CAT_CODE },
        postData: { stateCode: $("#ddlStates option:selected").val(), districtCode: $("#ddlDistricts option:selected").val(), "MAST_BLOCK_CODE": $("#ddlBlocks option:selected").val(), "MAST_ROAD_CAT_CODE": $("#ddlRoadCategory option:selected").val() },
        pager: jQuery('#dvExistingRoadsListPager'),
        rowNum: 10,
        sortname: "MAST_ER_ROAD_NUMBER",
        sortorder: "asc",
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Existing Roads List",
        height: 'auto',
        //width: '100%',
        autowidth: true,
        rownumbers: true,
        //shrinkToFit: false,
        loadComplete: function () {
            $("#tbExistingRoadsList #dvExistingRoadsListPager").css({ height: '31px' });
            unblockPage();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Session Timeout !!!");
                window.location.href = "/Login/LogIn";
            }
        }

    }); //end of grid

}


function ShowExistingRoads() {
    $('#accordion').hide('slow');
    $('#divExistingRoadsForm').hide('slow');
    $("#tbExistingRoadsList").jqGrid('setGridState', 'visible');
    showFilter();
}

function ValidateRoadNumber() {

    var roadShortDesc = $("#MAST_ER_SHORT_DESC").val();
    var roadNumber = $("#MAST_ER_ROAD_NUMBER").val();

    if ((roadShortDesc.length + roadNumber.length) > 15) {
        var maxLength = 15;
        var roadLen = maxLength - roadShortDesc.length;
        $("#ValRoadNumber").show("slow");
        $("#ValRoadNumber").html("<span style='color:red'><b>Road number should be less than " + roadLen + " alphanumeric characters</b></span>");
        return false;
    }
    else
        return true;

}

function ValidateCoreNetwork(RoadCode) {
    var data = RoadCode;

    $.ajax({

        type: 'POST',
        url: '/ExistingRoads/ValidateCoreNetwork?id=' + data,
        async: false,
        cache: false,
        success: function (data) {
            if (data.success == false) {
                isValid = false;
                alert('Status can not be changed as core network is already prepared against this road.');
                $("#radioCNYes").attr('checked', true);
                $("#radioCNNo").attr('checked', false);
            }
            else {
                isValid = true;
            }
        }


    });
}