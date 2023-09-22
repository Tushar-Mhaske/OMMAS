$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmTendAgrDtls');


    $("#StateList_CompletedRoadDetails").change(function ()
    {
        loadDistrictCN1($("#StateList_CompletedRoadDetails").val());

    });



    $("#DistrictList_CompletedRoadDetails").change(function ()
    {
        loadBlockCN1($("#StateList_CompletedRoadDetails").val(), $("#DistrictList_CompletedRoadDetails").val());

    });



    $("#BlockList_CompletedRoadDetails").change(function ()
    {
        changeBlockCN1($("#BlockList_CompletedRoadDetails").val(), $("#BlockList_CompletedRoadDetails").val());

    });

    if ($("#frmTendAgrDtls").valid())
    {

        $("#YearName").val($("#ddlYearTendAgrDetails option:selected").text());

        $("#CollaborationName").val($("#ddlCollabTendAgrDetails option:selected").text());

        $("#DistName").val($("#DistrictList_CompletedRoadDetails option:selected").text());

        $.ajax({
            url: "/WorkAwardedArea/WorkAwarded/TendAgrDetailsReport/",
            cache: false,
            type: "POST",
            async: false,
            data: $("#frmTendAgrDtls").serialize(),
            success: function (data) {
                $("#loadReport").html('');
                $("#loadReport").html(data);
            },
            error: function () {
                alert("error");
            }
        })
    }

    $("#btnViewTendAgrDetailsReport").click(function ()
    {


        //$("#loadReport").html("");
        //$("#loadReport").load("/WorkAwardedArea/WorkAwarded/TendAgrDetailsReport/" + $("#ddlYearTendAgrDetails").val() + "$" + $("#ddlCollabTendAgrDetails").val());
        if ($("#frmTendAgrDtls").valid()) {


            $("#YearName").val($("#ddlYearTendAgrDetails option:selected").text());

            $("#CollaborationName").val($("#ddlCollabTendAgrDetails option:selected").text());

            $("#DistName").val($("#DistrictList_CompletedRoadDetails option:selected").text());
            
            

            $.ajax({
                url: "/WorkAwardedArea/WorkAwarded/TendAgrDetailsReport/",
                cache: false,
                type: "POST",
                async: false,
                data: $("#frmTendAgrDtls").serialize(),
                success: function (data) {
                    $("#loadReport").html('');
                    $("#loadReport").html(data);
                },
                error: function () {
                    alert("error");
                }
            })
        }
    });
    closableNoteDiv("divCompRoads", "spnCompRoads");
});

//State Change Fill District DropDown List
function loadDistrictCN1(statCode) {
    $("#DistrictList_CompletedRoadDetails").val(0);
    $("#DistrictList_CompletedRoadDetails").empty();
    $("#BlockList_CompletedRoadDetails").val(0);
    $("#BlockList_CompletedRoadDetails").empty();
    $("#BlockList_CompletedRoadDetails").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_CompletedRoadDetails").length > 0) {
            $.ajax({
                url: '/WorkAwardedArea/WorkAwarded/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_CompletedRoadDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                 

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_CompletedRoadDetails").val($("#Mast_District_Code").val());
                        // $("#DistrictList_CompletedRoadDetails").attr("disabled", "disabled");
                        $("#DistrictList_CompletedRoadDetails").trigger('change');
                    }
                    $("#StateName").val($("#StateList_CompletedRoadDetails option:selected").text());

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
    else {

        $("#DistrictList_CompletedRoadDetails").append("<option value='0'>All Districts</option>");
        $("#BlockList_CompletedRoadDetails").empty();
        $("#BlockList_CompletedRoadDetails").append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlockCN1(stateCode, districtCode) {
    $("#BlockList_CompletedRoadDetails").val(0);
    $("#BlockList_CompletedRoadDetails").empty();

    if (districtCode > 0) {
        if ($("#BlockList_CompletedRoadDetails").length > 0) {
            $.ajax({
                url: '/WorkAwardedArea/WorkAwarded/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_CompletedRoadDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_CompletedRoadDetails").val($("#Mast_Block_Code").val());
                        
                    }
                    $("#DistName").val($("#DistrictList_CompletedRoadDetails option:selected").text());

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_CompletedRoadDetails").append("<option value='0'>All Blocks</option>");
    }
}

//Block Change Fill
function changeBlockCN1(stateCode, districtCode)

{
    $("#BlockName").val($("#BlockList_CompletedRoadDetails option:selected").text());
}