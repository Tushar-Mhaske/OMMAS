/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMPhaseInspectionViewDetails.js
        * Description   :   Handles click event, Dependant dropdown population of District ,Block and Agency.
        * Author        :   Rohit Jadhav. 
        * Creation Date :   01/Sept/2014
 **/

$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmDistRating'));

    $("#StateList_DistRatingDetail").change(function () {
        loadDistrict($("#StateList_DistRatingDetail").val());
        loadAgency($("#StateList_DistRatingDetail").val());
        
    });

    $("#DistrictList_DistRatingDetail").change(function () {
        loadBlock($("#StateList_DistRatingDetail").val(), $("#DistrictList_DistRatingDetail").val());

    });


    $("#btnViewDistRatingDetail").click(function () {


        if ($('#frmDistRating').valid()) {

            $("#loadReport1").html("");
            $("#loadReport2").html("");
            $("#loadReport3").html("");
            $("#LoadQMInspDetailsReport").html("");
            
            $("#AgencyName").val($("#AgencyList_DistRatingDetail option:selected").text());
            $("#StatusName").val($("#StatusList_DistRatingDetail option:selected").text());
            $("#YearName").val($("#PhaseYearList_DistRatingDetail option:selected").text());
            $("#BatchName").val($("#BatchList_DistRatingDetail option:selected").text());
            if ($("#StateList_DistRatingDetail").is(":visible")) {

                $("#StateName").val($("#StateList_DistRatingDetail option:selected").text());
            }

            if ($("#DistrictList_DistRatingDetail").is(":visible")) {

            
                $("#DistName").val($("#DistrictList_DistRatingDetail option:selected").text());
            }
            if ($("#BlockList_DistRatingDetail").is(":visible")) {

                $("#BlockName").val($("#BlockList_DistRatingDetail option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/QualityMonitoring/QMPhaseInspectionViewModelReport/',
                type: 'POST',
                catche: false,
                data: $("#frmDistRating").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#LoadQMInspDetailsReport").html(response);

                },
                //complete: function () {
                //    $('#frmDistRating').each(function () {
                //        this.reset();   //Here form fields will be cleared.
                //    });
                //},
                error: function () {
                    $.unblockUI();
                    alert("An Error");
                    return false;
                },
            });

        }
        else {

        }
    });


    if ($('#Mast_State_Code').val() > 0) {
        $("#btnViewDistRatingDetail").trigger('click');
    }
    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});

function loadDistrict(statCode) {
    $("#DistrictList_DistRatingDetail").val(0);
    $("#DistrictList_DistRatingDetail").empty();
    $("#BlockList_DistRatingDetail").val(0);
    $("#BlockList_DistRatingDetail").empty();
    $("#BlockList_DistRatingDetail").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_DistRatingDetail").length > 0) {
            $.ajax({
                url: '/QualityMonitoring/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_DistRatingDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                   

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_DistRatingDetail").val($("#Mast_District_Code").val());
                     
                        $("#DistrictList_DistRatingDetail").trigger('change');
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

        $("#DistrictList_DistRatingDetail").append("<option value='0'>All Districts</option>");
        $("#BlockList_DistRatingDetail").empty();
        $("#BlockList_DistRatingDetail").append("<option value='0'>All Blocks</option>");

    }
}

//State Change Results in the Population of the Agencies
function loadAgency(statCode) {
    $("#AgencyList_DistRatingDetail").val(0);
    $("#AgencyList_DistRatingDetail").empty();
    

    if (statCode > 0) {
        if ($("#AgencyList_DistRatingDetail").length > 0) {
            $.ajax({
                url: '/QualityMonitoring/AgencyDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#AgencyList_DistRatingDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }


                    //For Disable if District Login
                    if ($("#Mast_Agency_Code").val() > 0) {
                        $("#AgencyList_DistRatingDetail").val($("#Mast_Agency_Code").val());

                        $("#AgencyList_DistRatingDetail").trigger('change');
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

        $("#DistrictList_DistRatingDetail").append("<option value='0'>All Districts</option>");
        $("#BlockList_DistRatingDetail").empty();
        $("#BlockList_DistRatingDetail").append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_DistRatingDetail").val(0);
    $("#BlockList_DistRatingDetail").empty();

    if (districtCode > 0) {
        if ($("#BlockList_DistRatingDetail").length > 0) {
            $.ajax({
                url: '/QualityMonitoring/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_DistRatingDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_DistRatingDetail").val($("#Mast_Block_Code").val());
                      
                    }
                

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_DistRatingDetail").append("<option value='0'>All Blocks</option>");
    }
}