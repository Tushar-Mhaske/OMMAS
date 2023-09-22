$(document).ready(function () {


  

       $('#btnCreate').click(function (evt) {
        debugger;
        evt.preventDefault();
        // ValidateRoadNumber();
      
       
    
    var DataToSend = [];
    $('#Table1 tr[id]').each(function(){
        DataToSend.push({
            "Month": $(this).attr('id'),
            "PMGSYILength": $(this).find("input[id$='pmgsyI']").val(),
            "PMGSYIILength": $(this).find("input[id$='pmgsyII']").val(),
            "RCPLWELength": $(this).find("input[id$='RCPLWE']").val(),
            "PMGSYIIILength": $(this).find( "input[id$='pmgsyIII']").val(),
            "state":$('#ddlStates').val(),
            "year": $('#ddlYear').val(),
            "months": []
        });
        });
      
        var state = $('#ddlStates').val();
        var year = $('#ddlYear').val();
       

                $.ajax({
                    url: '/Master/AddFinancialTarget',
                    type: "POST",
                    cache: false,
                    data: JSON.stringify(DataToSend),
                    //$("#frmCreateExistingRoad").serialize(),
                        contentType: 'application/json; charset=utf-8',
                    beforeSend: function () {
                        blockPage();
                    },
                    error: function (xhr, status, error) {
                        unblockPage();

                        Alert("Request can not be processed at this time,please try after some time!!!");
                        return false;
                    },

                    success: function (response) {

                        if (response.success === undefined) {
                            $("#divFinancialYearTargetForm").html(response);
                        }
                        else if (response.success) {
                          //  $('#tbFinancialYearTargetList').jqGrid('GridUnload');
                            alert(response.message);
                            $("#btnReset").trigger("click");
                          //  ResetForm();
                           // CloseExistingRoadsDetails();


                           // $('#tbFinancialYearTargetList').jqGrid('GridUnload');
                            //     LoadExistingRoads(MAST_BLOCK_CODE, MAST_ROAD_CAT_CODE);
                           // LoadFinancialYearTarget(state, year);

                            unblockPage();
                           // $("#tbFinancialYearTargetList").trigger("reloadGrid");

                           // $("#btnListExistingFinancialTarget").trigger('click');
                            /*$('#accordion').hide('slow');
                            $('#divExistingRoadsForm').hide('slow');
                            showFilter();
                            LoadExistingRoads1();*/
                            ShowExistingRoads();

                            $("#tbFinancialYearTargetList").trigger("reloadGrid");
                        }
                        else {
                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.message);
                            unblockPage();
                        }
                        unblockPage();
                    }
                });    //end of ajax 
    });
    
            //}
        //}
        //else {
        //    $('.qtip').show();
        //}


   

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
//});