
$(document).ready(function () {

    // cHNAGE by Saurabh

    //if ($('#isUCSStrength7Days').attr("checked", "checked"))
    //{
    //    $('#isMpa28days').hide();
    //    $('#isUcs28days').hide();
    //    $('#isMpa7days').show(); //  'slow'
    //    $('#isUcs7days').show();
    //}

    //if ($('#isUCSStrength28Days').attr("checked", "checked")) {
    //    $('#isMpa7days').hide();
    //    $('#isUcs7days').hide();
    //    $('#isMpa28days').show();
    //    $('#isUcs28days').show(); // 'slow'
    //}

    //alert("isUCSPI7Days " + $('#isUCSPI7Days').val())
    //alert("isUCSPI28Days " + $('#isUCSPI28Days').val())

    //$('#LabelOtherCrackRelief').hide();
    //$('#TextOtherCrackRelief').hide();

    if ($('#isUCSPI7Days').val() == null || $('#isUCSPI7Days').val() == undefined || $('#isUCSPI7Days').val() == "") {
        $('#isMpa28days').show();
        $('#isUcs28days').show();
        $('#isMpa7days').hide();
        $('#isUcs7days').hide();
    }

    if ($('#isUCSPI28Days').val() == null || $('#isUCSPI28Days').val() == undefined || $('#isUCSPI28Days').val() == "") {
        $('#isMpa28days').hide();
        $('#isUcs28days').hide();
        $('#isMpa7days').show();
        $('#isUcs7days').show();
    }

    $("#isUCSStrength7Days").click(function () {
        if ($(this).is(":checked")) {
            $('#isUCSPI28Days').val('');
            $('#isMpa28days').hide();
            $('#isUcs28days').hide();
            $('#isMpa7days').show();
            $('#isUcs7days').show();
            $("#isUCSPI7Days").focus();
        } else {
            $("#isUCSPI28Days").attr("disabled", "disabled");
        }
    });
    $("#isUCSStrength28Days").click(function () {
        if ($(this).is(":checked")) {
            $('#isUCSPI7Days').val('');
            $('#isMpa7days').hide();
            $('#isUcs7days').hide();
            $('#isMpa28days').show();
            $('#isUcs28days').show();
            $("#isUCSPI28Days").focus();
        } else {
            $("#isUCSPI7Days").attr("disabled", "disabled");
        }
    });

    //if ($('#isUCSTestCube').attr("checked", "checked")) {
    //    $('#isUcsCylinder').hide();
    //    $('#isUcsCylinSample').hide();
    //    $('#isUcsCube').show();  // 'slow'
    //    $('#isUcsCubeSample').show();
    //}

    // isUCSTestCylindrical
    //if ($('#isUCSTestCylindrical').attr("checked", "checked")) {
    //    $('#isUcsCube').hide();
    //    $('#isUcsCubeSample').hide();
    //    $('#isUcsCylinder').show();
    //    $('#isUcsCylinSample').show(); // 'slow'
    //}

    if ($('#isUCSCube').val() == null || $('#isUCSCube').val() == undefined || $('#isUCSCube').val() == "") {
        $('#isUcsCube').hide();
        $('#isUcsCubeSample').hide();
        $('#isUcsCylinder').show();
        $('#isUcsCylinSample').show();
    }

    if ($('#isUCSCylindrical').val() == null || $('#isUCSCylindrical').val() == undefined || $('#isUCSCylindrical').val() == "") {
        $('#isUcsCylinder').hide();
        $('#isUcsCylinSample').hide();
        $('#isUcsCube').show();
        $('#isUcsCubeSample').show();
    }

    $("#isUCSTestCube").click(function () {
        if ($(this).is(":checked")) {
            $('#isUCSCylindrical').val('');
            $('#isUcsCylinder').hide();
            $('#isUcsCylinSample').hide();
            $('#isUcsCube').show();
            $('#isUcsCubeSample').show();
            $("#isUCSCube").focus();
        } else {
            $("#isUCSTestCylindrical").attr("disabled", "disabled");
        }
    });
    $("#isUCSTestCylindrical").click(function () {
        if ($(this).is(":checked")) {
            $('#isUCSCube').val('');
            $('#isUcsCube').hide();
            $('#isUcsCubeSample').hide();
            $('#isUcsCylinder').show();
            $('#isUcsCylinSample').show();
            $("#isUCSCylindrical").focus();
        } else {
            $("#isUCSTestCube").attr("disabled", "disabled");
        }
    });

    // Additive Content
    //if ($('#isAdditivePerc').attr("checked", "checked")) {
    //    $('#LabelAdditiveConMlCum').hide();
    //    $('#TextAdditiveMlCum').hide();
    //    $('#LabelAdditiveConPerc').show();
    //    $('#TextAdditiveConPerc').show();
    //}

    if ($('#InputAdditivePerc').val() == null || $('#InputAdditivePerc').val() == undefined || $('#InputAdditivePerc').val() == "") {
        $('#LabelAdditiveConPerc').hide();
        $('#TextAdditiveConPerc').hide();
        $('#LabelAdditiveConMlCum').show();
        $('#TextAdditiveMlCum').show();
    }

    if ($('#InputAdditiveMlCum').val() == null || $('#InputAdditiveMlCum').val() == undefined || $('#InputAdditiveMlCum').val() == "") {
        $('#LabelAdditiveConMlCum').hide();
        $('#TextAdditiveMlCum').hide();
        $('#LabelAdditiveConPerc').show();
        $('#TextAdditiveConPerc').show();
    }

    //$("input").removeAttr('disabled');
    $("#isAdditivePerc").click(function () {
        if ($(this).is(":checked")) {
            $('#InputAdditiveMlCum').val('');
            $('#LabelAdditiveConMlCum').hide();
            $('#TextAdditiveMlCum').hide();
            $('#LabelAdditiveConPerc').show();
            $('#TextAdditiveConPerc').show();
            $("#InputAdditivePerc").focus();
        } else {
            $("#InputAdditiveMlCum").attr("disabled", "disabled");
        }
    });

    $("#isAdditiveMlCum").click(function () {
        if ($(this).is(":checked")) {
            $('#InputAdditivePerc').val('');
            $('#LabelAdditiveConPerc').hide();
            $('#TextAdditiveConPerc').hide();
            $('#LabelAdditiveConMlCum').show();
            $('#TextAdditiveMlCum').show();
            $("#InputAdditiveMlCum").focus();
        } else {
            $("#InputAdditivePerc").attr("disabled", "disabled");
        }
    });

    // Trial Stretch UCS Value Cube or Cylindrical

    //if ($('#isTSUCSCube').attr("checked", "checked")) {
    //    $('#LabelUcsCylindrical').hide();
    //    $('#TextUcsCylinder').hide();
    //    $('#LabelUcsCube').show();
    //    $('#TextUcsCube').show();
    //}

    //if ($('#isTSUCSCylindrical').attr("checked", "checked")) {
    //    $('#LabelUcsCube').hide();
    //    $('#TextUcsCube').hide();
    //    $('#LabelUcsCylindrical').show();
    //    $('#TextUcsCylinder').show();
    //}

    if ($('#InputUcsCube').val() == null || $('#InputUcsCube').val() == undefined || $('#InputUcsCube').val() == "") {
        $('#LabelUcsCube').hide();
        $('#TextUcsCube').hide();
        $('#LabelUcsCylindrical').show();
        $('#TextUcsCylinder').show();
    }

    if ($('#InputUcsCylinder').val() == null || $('#InputUcsCylinder').val() == undefined || $('#InputUcsCylinder').val() == "") {
        $('#LabelUcsCylindrical').hide();
        $('#TextUcsCylinder').hide();
        $('#LabelUcsCube').show();
        $('#TextUcsCube').show();
    }

    $("#isTSUCSCube").click(function () {
        if ($(this).is(":checked")) {
            $('#InputUcsCylinder').val('');
            $('#LabelUcsCylindrical').hide();
            $('#TextUcsCylinder').hide();
            $('#LabelUcsCube').show();
            $('#TextUcsCube').show();
            $("#InputUcsCube").focus();
        } else {
            $("#InputUcsCylinder").attr("disabled", "disabled");
        }
    });

    $("#isTSUCSCylindrical").click(function () {
        if ($(this).is(":checked")) {
            $('#InputUcsCube').val('');
            $('#LabelUcsCube').hide();
            $('#TextUcsCube').hide();
            $('#LabelUcsCylindrical').show();
            $('#TextUcsCylinder').show();
            $("#InputUcsCylinder").focus();
        } else {
            $("#InputUcsCube").attr("disabled", "disabled");
        }
    });

    // Trial Stretch UCS Value 7D or 28D

    //if ($('#isTSUCSValu7D').attr("checked", "checked")) {
    //    $('#LabelUcs28D').hide();
    //    $('#TextUcs28D').hide();
    //    $('#LabelUcs7D').show();
    //    $('#TextUcs7D').show();
    //}

    //if ($('#isTSUCSValu28D').attr("checked", "checked")) {
    //    $('#LabelUcs7D').hide();
    //    $('#TextUcs7D').hide();
    //    $('#LabelUcs28D').show();
    //    $('#TextUcs28D').show();
    //}

    if ($('#InputUcs7D').val() == null || $('#InputUcs7D').val() == undefined || $('#InputUcs7D').val() == "") {
        $('#LabelUcs7D').hide();
        $('#TextUcs7D').hide();
        $('#LabelUcs28D').show();
        $('#TextUcs28D').show();
    }

    if ($('#InputUcs28D').val() == null || $('#InputUcs28D').val() == undefined || $('#InputUcs28D').val() == "") {
        $('#LabelUcs28D').hide();
        $('#TextUcs28D').hide();
        $('#LabelUcs7D').show();
        $('#TextUcs7D').show();
    }


    $("#isTSUCSValu7D").click(function () {
        if ($(this).is(":checked")) {
            $('#InputUcs28D').val('');
            $('#LabelUcs28D').hide();
            $('#TextUcs28D').hide();
            $('#LabelUcs7D').show();
            $('#TextUcs7D').show();
            $("#InputUcs7D").focus();
        } else {
            $("#InputUcs28D").attr("disabled", "disabled");
        }
    });

    $("#isTSUCSValu28D").click(function () {
        if ($(this).is(":checked")) {
            $('#InputUcs7D').val('');
            $('#LabelUcs7D').hide();
            $('#TextUcs7D').hide();
            $('#LabelUcs28D').show();
            $('#TextUcs28D').show();
            $("#InputUcs28D").focus();
        } else {
            $("#InputUcs7D").attr("disabled", "disabled");
        }
    });

    // Other Crack Relief Layer

    //if ($('#IsCrackRelifOtherLayer').attr("checked", "checked")) {
    //    $('#LabelOtherCrackRelief').show();
    //    $('#TextOtherCrackRelief').show();
    //}

    if ($('#InputOtherLayer').val() == null || $('#InputOtherLayer').val() == undefined || $('#InputOtherLayer').val() == "") {
        $('#LabelOtherCrackRelief').hide();
        $('#TextOtherCrackRelief').hide();
    }
    else {
        $('#LabelOtherCrackRelief').show();
        $('#TextOtherCrackRelief').show();
    }

    $("#IsCrackRelifOtherLayer").click(function () {
        if ($(this).is(":checked")) {
            $('#LabelOtherCrackRelief').show();
            $('#TextOtherCrackRelief').show();
            $("#InputOtherLayer").focus();
        } else {

        }
    });

    $("#crackRelifAggregate").click(function () {
        if ($(this).is(":checked")) {
            $('#InputOtherLayer').val('');
            $('#LabelOtherCrackRelief').hide();
            $('#TextOtherCrackRelief').hide();
        } else {

        }
    });
    $("#crackRelifAggBitumen").click(function () {
        if ($(this).is(":checked")) {
            $('#InputOtherLayer').val('');
            $('#LabelOtherCrackRelief').hide();
            $('#TextOtherCrackRelief').hide();
        } else {

        }
    });
    $("#crackRelifGeoCoat").click(function () {
        if ($(this).is(":checked")) {
            $('#InputOtherLayer').val('');
            $('#LabelOtherCrackRelief').hide();
            $('#TextOtherCrackRelief').hide();
        } else {

        }
    });


    // Change End



    $.validator.unobtrusive.parse($("#addTrailStrechForFDRFormId"));

    //Multiselect for selecting different level groups
    //---------------------------------------------------
    $("#additiveId").multiselect({
        minWidth: 150,
        position: {
            my: 'left bottom',
            at: 'left top'
        }
    });

    $("#additiveId").multiselect("uncheckAll");
    //$(".ui-icon.ui-icon-triangle-2-n-s").next("span").html("New HTML content");

    //---------------------------------------------------

    $('#date').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Date of construction of trial strech',
        maxDate: "0D",   //null,  to disable future dates
        //minDate: "0D",    //"0D",  to disable past dates
        buttonImageOnly: true,
        buttonText: 'Date of construction of trial strech',
        changeMonth: true,
        changeYear: true,
        stepMonths: true,
        onSelect: function (selectedDate) {

            $('#date').trigger('blur');
        }

    });

    //Save Data
    $("#btnSumit").click(function () {

        var sanctionedLength = parseFloat($("#sanctionedLength").text());
        var strechedLength = parseFloat($("#STRETCH_LENGTH").val());

        if (strechedLength > sanctionedLength) {
            alert("Length of trial strech in km should not be greated that sanctioned length");
            return false;
        }

        var form = $("#addTrailStrechForFDRFormId");
        var formadata = new FormData(form.get(0), form.get(1));
        //var arr = 'a,v,b';

        //ui-multiselect-additiveId-option-1

        /*        alert("len-" + $("#ADDITIVE_ID_LIST_Count").length);
                alert("html-" + $("#ADDITIVE_ID_LIST_Count").html());
                alert("text-" + $("#ADDITIVE_ID_LIST_Count").text());
        
        */
        //alert("val-" + $("#ADDITIVE_ID_LIST_Count").val());
        var multiselectlistSize = $("#ADDITIVE_ID_LIST_Count").val();
        var multiselectlistArr = "";
        //for (var i = 1; i <= multiselectlistSize; i++) {
        for (var i = 0; i < multiselectlistSize; i++) {
            var element = $('#ui-multiselect-additiveId-option-' + i);
            if (element.attr('aria-selected') === "true") {
                if (multiselectlistArr != "") {   //if (i != 1) {
                    //alert("in if");
                    //alert("#ui-multiselect-additiveId-option-" + i)
                    var nm = "#ui-multiselect-additiveId-option-" + i;
                    //alert("nm" + $(nm).val());
                    //alert("nm typeof" + typeof($(nm).val()));
                    multiselectlistArr = multiselectlistArr + "$" + $(nm).val();
                    //alert(multiselectlistArr);
                }
                else {
                    //alert("in else");
                    var nm1 = "#ui-multiselect-additiveId-option-" + i;
                    //alert("#ui-multiselect-additiveId-option-" + i)
                    //alert("else nm" + $(nm1).val());
                    multiselectlistArr = multiselectlistArr + $(nm1).val();
                    // alert(multiselectlistArr);
                }
            }
        }
        //alert("multiselectlistArr-" + multiselectlistArr)

        formadata.append("ADDITIVE_ID_List_Arr", multiselectlistArr)
        var saveFinalize = "S";
        formadata.append("SAVE_FINALIZE", saveFinalize);

        if (confirm("Are you sure to save trail strech FDR details?")) {
            if ($("#addTrailStrechForFDRFormId").valid()) {

                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

                $.ajax({
                    url: '/PMIS/PMIS/SaveAddTrailStrechForFDR/',
                    type: 'POST',
                    cache: false,
                    async: false,
                    contentType: false,
                    processData: false,
                    beforeSend: function () { },
                    data: formadata,
                    success: function (response) {
                        //alert(response.message);
                        if (response.Success) {
                            $("#addTrailStrechForFDRFormId")[0].reset();
                            alert(response.message);


                            /*$("#dvPMISRoads").show();
                            $("#accordion").hide();*/

                            //$("#tbPMISRoadList").jqGrid('setGridState', 'hidden');


                            $("#tbPMISRoadList").jqGrid('setGridState', 'visible');
                            $("#tbPMISRoadList").trigger('reloadGrid');

                            $('#idFilterDiv').trigger('click');
                            $('#btnRoadList').trigger('click');

                            $("#accordion").hide();
                            // $("#tbPMISRoadList").trigger('reloadGrid');


                        }
                        if (response.Success == false) {
                            alert(response.message);
                        }
                        $.unblockUI();
                    },
                    error: function () {
                        $.unblockUI();
                        alert("An Error Occured");
                        return false;
                    },
                });

            } else {
                return false;
            }
        }
        else {
            return false;
        }

    });

    //Save and Finalize Data
    $("#btnSaveFinalize").click(function () {

        var sanctionedLength = parseFloat($("#sanctionedLength").text());
        var strechedLength = parseFloat($("#STRETCH_LENGTH").val());

        if (strechedLength > sanctionedLength) {
            alert("Length of trial strech in km should not be greated that sanctioned length");
            return false;
        }

        var form = $("#addTrailStrechForFDRFormId");
        var formadata = new FormData(form.get(0), form.get(1));
        //var arr = 'a,v,b';

        //ui-multiselect-additiveId-option-1

        /*        alert("len-" + $("#ADDITIVE_ID_LIST_Count").length);
                alert("html-" + $("#ADDITIVE_ID_LIST_Count").html());
                alert("text-" + $("#ADDITIVE_ID_LIST_Count").text());
        
        */
        //alert("val-" + $("#ADDITIVE_ID_LIST_Count").val());
        var multiselectlistSize = $("#ADDITIVE_ID_LIST_Count").val();
        var multiselectlistArr = "";
        //for (var i = 1; i <= multiselectlistSize; i++) {
        for (var i = 0; i < multiselectlistSize; i++) {
            var element = $('#ui-multiselect-additiveId-option-' + i);
            if (element.attr('aria-selected') === "true") {
                if (multiselectlistArr != "") {   //if (i != 1) {
                    //alert("in if");
                    //alert("#ui-multiselect-additiveId-option-" + i)
                    var nm = "#ui-multiselect-additiveId-option-" + i;
                    //alert("nm" + $(nm).val());
                    //alert("nm typeof" + typeof($(nm).val()));
                    multiselectlistArr = multiselectlistArr + "$" + $(nm).val();
                    //alert(multiselectlistArr);
                }
                else {
                    //alert("in else");
                    var nm1 = "#ui-multiselect-additiveId-option-" + i;
                    //alert("#ui-multiselect-additiveId-option-" + i)
                    //alert("else nm" + $(nm1).val());
                    multiselectlistArr = multiselectlistArr + $(nm1).val();
                    // alert(multiselectlistArr);
                }
            }
        }
        //alert("multiselectlistArr-" + multiselectlistArr)

        formadata.append("ADDITIVE_ID_List_Arr", multiselectlistArr);
        var saveFinalize = "SF";
        formadata.append("SAVE_FINALIZE", saveFinalize);

        if (confirm("Are you sure To Save and Finalize trail strech FDR details?")) {
            if ($("#addTrailStrechForFDRFormId").valid()) {

                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

                $.ajax({
                    url: '/PMIS/PMIS/SaveAddTrailStrechForFDR/',
                    type: 'POST',
                    cache: false,
                    async: false,
                    contentType: false,
                    processData: false,
                    beforeSend: function () { },
                    data: formadata,
                    success: function (response) {
                        //alert(response.message);
                        if (response.Success) {
                            $("#addTrailStrechForFDRFormId")[0].reset();
                            alert(response.message);


                            /*$("#dvPMISRoads").show();
                            $("#accordion").hide();*/

                            //$("#tbPMISRoadList").jqGrid('setGridState', 'hidden');

                            $("#tbPMISRoadList").jqGrid('setGridState', 'visible');
                            $("#tbPMISRoadList").trigger('reloadGrid');


                            $('#idFilterDiv').trigger('click');
                            $('#btnRoadList').trigger('click');

                            $("#accordion").hide();
                            //$("#tbPMISRoadList").trigger('reloadGrid');


                        }
                        if (response.Success == false) {
                            alert(response.message);
                        }
                        $.unblockUI();
                    },
                    error: function () {
                        $.unblockUI();
                        alert("An Error Occured");
                        return false;
                    },
                });

            } else {
                return false;
            }
        }
        else {
            return false;
        }

    });

});//end ready function



//btnView1
//View PDF1
$('#btnView1').click(function () {
    //alert($('#IMS_PR_ROAD_CODE').val());
    var getTimestamp = new Date().getTime();
    $.ajax({
        type: "POST",
        url: '/PMIS/PMIS/viewFdrFileTestResult?id=' + $('#IMS_PR_ROAD_CODE').val() + "&timestamp=" + getTimestamp,
        dataType: 'json',
        contentType: false,
        processData: false,
        cache: false,
        success: function (response) {
            if (response.success) {
                window.open(response.Message);
            }
            else {
                alert(response.Message);
            }
        },
        error: function (error) {
            alert("Some problem occured. Please try after sometime")

        }


    });
})

//View PDF2
$('#btnView2').click(function () {
    //alert($('#IMS_PR_ROAD_CODE').val());
    var getTimestamp = new Date().getTime();
    $.ajax({
        type: "POST",
        url: '/PMIS/PMIS/viewFdrFileJMF?id=' + $('#IMS_PR_ROAD_CODE').val() + "&timestamp=" + getTimestamp,
        dataType: 'json',
        contentType: false,
        processData: false,
        cache: false,
        success: function (response) {
            if (response.success) {
                window.open(response.Message);
            }
            else {
                alert(response.Message);
            }
        },
        error: function (error) {
            alert("Some problem occured. Please try after sometime")
        }


    });
})

$('#btnView3').click(function () {
    //alert($('#IMS_PR_ROAD_CODE').val());
    var getTimestamp = new Date().getTime();
    $.ajax({
        type: "POST",
        url: '/PMIS/PMIS/viewFdrFileMixDesign?id=' + $('#IMS_PR_ROAD_CODE').val() + "&timestamp=" + getTimestamp,
        dataType: 'json',
        contentType: false,
        processData: false,
        cache: false,
        success: function (response) {
            if (response.success) {
                window.open(response.Message);
            }
            else {
                alert(response.Message);
            }
        },
        error: function (error) {
            alert("Some problem occured. Please try after sometime")
        }
    });
})

//delete records.
$("#btnDelete").click(function () {
    //alert($('#IMS_PR_ROAD_CODE').val());
    var roadCode = $('#IMS_PR_ROAD_CODE').val();
    //alert($('input[name=__RequestVerificationToken]').val());

    if (confirm("Are you sure to delete Trail Strech FDR Details?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            //url: '/PMIS/PMIS/DeleteTrialStrechFDR/' + $('#IMS_PR_ROAD_CODE').val(),
            url: '/PMIS/PMIS/DeleteTrialStrechFDR',
            type: "POST",
            cache: false,
            async: false,
            //contentType: false,
            //data: { __RequestVerificationToken: $('input[name=__RequestVerificationToken]').val() },
            data: { RoadCode: roadCode },
            success: function (response) {
                if (response.success) {
                    alert(" Trail Strech FDR deleted successfully.");
                    //LoadPMISBridgeList();


                    $("#tbPMISRoadList").jqGrid('setGridState', 'visible');
                    $("#tbPMISRoadList").trigger('reloadGrid');

                    $('#idFilterDiv').trigger('click');
                    $('#btnRoadList').trigger('click');
                    $("#accordion").hide();

                }
                else {
                    alert(response.errorMessage);

                    $("#tbPMISRoadList").jqGrid('setGridState', 'visible');
                    $("#tbPMISRoadList").trigger('reloadGrid');

                    $('#idFilterDiv').trigger('click');
                    $('#btnRoadList').trigger('click');
                    $("#accordion").hide();
                }
                $.unblockUI();
            },
            error: function () {

                $.unblockUI();
                alert("Error : " + error);
                return false;
            }
        });

    }

});


//finalize data
$("#btnFinalize").click(function () {
    //alert($('#IMS_PR_ROAD_CODE').val());
    var roadCode = $('#IMS_PR_ROAD_CODE').val();
    //alert($('input[name=__RequestVerificationToken]').val());

    if (confirm("Are you want to finalize Trail Strech FDR ?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            //url: '/PMIS/PMIS/DeleteTrialStrechFDR/' + $('#IMS_PR_ROAD_CODE').val(),
            url: '/PMIS/PMIS/FinalizeTrailStrechFDR',
            type: "POST",
            cache: false,
            async: false,
            //contentType: false,
            //data: { __RequestVerificationToken: $('input[name=__RequestVerificationToken]').val() },
            data: { RoadCode: roadCode },
            success: function (response) {
                if (response.success) {
                    alert(" Trail strech FDR details finalized successfully.");

                    $("#tbPMISRoadList").jqGrid('setGridState', 'visible');
                    $("#tbPMISRoadList").trigger('reloadGrid');

                    $('#idFilterDiv').trigger('click');
                    $('#btnRoadList').trigger('click');

                    $("#accordion").hide();

                }
                else {
                    alert(response.errorMessage);


                    $("#tbPMISRoadList").jqGrid('setGridState', 'visible');
                    $("#tbPMISRoadList").trigger('reloadGrid');

                    $('#idFilterDiv').trigger('click');
                    $('#btnRoadList').trigger('click');

                    $("#accordion").hide();
                }
                $.unblockUI();
            },
            error: function () {

                $.unblockUI();
                alert("Error : " + error);
                return false;
            }
        });

    }

});

//UPDATE DATA 
$("#btnUpdate").click(function () {

    var sanctionedLength = parseFloat($("#sanctionedLength").text());
    var strechedLength = parseFloat($("#STRETCH_LENGTH").val());

    if (strechedLength > sanctionedLength) {
        alert("Length of trial strech in km should not be greater than sanctioned length");
        return false;
    }

    var form = $("#addTrailStrechForFDRFormId");
    var formadata = new FormData(form.get(0), form.get(1));

    var multiselectlistSize = $("#ADDITIVE_ID_LIST_Count").val();
    var multiselectlistArr1 = "";
    //for (var i = 1; i <= multiselectlistSize; i++) {
    for (var i = 0; i < multiselectlistSize; i++) {
        var element = $('#ui-multiselect-additiveId-option-' + i);
        if (element.attr('aria-selected') === "true") {
            if (multiselectlistArr1 != "") {
                var nm = "#ui-multiselect-additiveId-option-" + i;
                multiselectlistArr1 = multiselectlistArr1 + "$" + $(nm).val();
            }
            else {
                var nm1 = "#ui-multiselect-additiveId-option-" + i;
                multiselectlistArr1 = multiselectlistArr1 + $(nm1).val();
            }
        }
    }

    formadata.append("ADDITIVE_ID_List_Arr", multiselectlistArr1)
    var saveFinalize = "S";
    formadata.append("SAVE_FINALIZE", saveFinalize);

    if (confirm("Do you want to update trail strech FDR details")) {
        if ($("#addTrailStrechForFDRFormId").valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: '/PMIS/PMIS/UpdateTrialStrechFDRDetails/',
                type: 'POST',
                cache: false,
                async: false,
                contentType: false,
                processData: false,
                beforeSend: function () { },
                data: formadata,
                success: function (response) {
                    if (response.Success) {
                        $("#addTrailStrechForFDRFormId")[0].reset();
                        alert(response.message);

                        $("#tbPMISRoadList").jqGrid('setGridState', 'visible');
                        $("#tbPMISRoadList").trigger('reloadGrid');

                        $('#idFilterDiv').trigger('click');
                        $('#btnRoadList').trigger('click');

                        $("#accordion").hide();


                    }
                    if (response.Success == false) {
                        alert(response.message);
                    }
                    $.unblockUI();
                },
                error: function () {
                    $.unblockUI();
                    alert("An Error Occured");
                    return false;
                },
            });

        }
        else {
            return false;
        }

    }
    else {
        return false;
    }
    //wait code for 10 sec and after 10 sec it will print alert
    /*                setTimeout(function () {
                        $.unblockUI();
                        alert("SETTIMEOUT")
                    }, 10000);*/



    //}
});
