$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmMPR1');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmMPR1").toggle("slow");

    });

    loadColumns();

    $("#StateList_CompletedRoadDetails").change(function () {
        $("#ddlAgencyMPR1").empty();
        $.ajax({
            url: '/ECBriefReport/ECBriefReport/PopulateAgencies',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#StateList_CompletedRoadDetails").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Selected == true) {
                        $("#ddlAgencyMPR1").append("<option value='" + jsonData[i].Value + "' selected=true>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlAgencyMPR1").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }

                $.unblockUI();
            },
            error: function (err) {
                //alert("error " + err);
                $.unblockUI();
            }
        });

        loadDistrictCN1($("#StateList_CompletedRoadDetails").val());

    });

    $("#DistrictList_CompletedRoadDetails").change(function () {
        loadBlockCN1($("#StateList_CompletedRoadDetails").val(), $("#DistrictList_CompletedRoadDetails").val());

    });

    $("#BlockList_CompletedRoadDetails").change(function () {
        changeBlockCN1($("#BlockList_CompletedRoadDetails").val(), $("#BlockList_CompletedRoadDetails").val());

    });


    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });

    ////$("#loadReport").load("/MPR/MPR/MPR1Report/" + $("#ddlYearMPR1").val() + "$" + $("#ddlMonthMPR1").val() + "$" + $("#ddlCollabMPR1").val(), $.unblockUI());
    //if ($("#frmMPR1").valid()) {
    //    validate();
    //    $("#MonthName").val($("#ddlMonthMPR1 option:selected").text());
    //    $("#YearName").val($("#ddlYearMPR1 option:selected").text());
    //    $("#CollaborationName").val($("#ddlCollabMPR1 option:selected").text());
    //    $("#AgencyName").val($("#ddlAgencyMPR1 option:selected").text());
    //    if ($("#BlockList_CompletedRoadDetails").is(":visible")) {

    //        $("#BlockName").val($("#BlockList_CompletedRoadDetails option:selected").text());
    //    }
    //    $.ajax({
    //        url: "/MPR/MPR/MPR1ReportDynamic/",
    //        cache: false,
    //        type: "POST",
    //        async: false,
    //        data: $("#frmMPR1").serialize(),
    //        success: function (data) {
    //            $("#loadReport").html('');
    //            $("#loadReport").html(data);
    //        },
    //        error: function () {
    //            alert("error");
    //        }
    //    })
    //}

    $("#btnViewMPR1Report").click(function () {
        validate();
        //if ($("#ddlMonthMPR1").val() == 0) {
        //    alert("Please select Month.");
        //    return false;
        //}
        //if ($("#ddlYearMPR1").val() == 0) {
        //    alert("Please select Year.");
        //    return false;
        //}

        //$("#loadReport").html("");
        //$("#loadReport").load("/MPR/MPR/MPR1Report/" + $("#ddlYearMPR1").val() + "$" + $("#ddlMonthMPR1").val() + "$" + $("#ddlCollabMPR1").val());

        if ($("#frmMPR1").valid()) {

            $("#MonthName").val($("#ddlMonthMPR1 option:selected").text());
            $("#YearName").val($("#ddlYearMPR1 option:selected").text());
            $("#CollaborationName").val($("#ddlCollabMPR1 option:selected").text());
            $("#AgencyName").val($("#ddlAgencyMPR1 option:selected").text());
            if ($("#BlockList_CompletedRoadDetails").is(":visible")) {

                $("#BlockName").val($("#BlockList_CompletedRoadDetails option:selected").text());
            }
            $.ajax({
                url: "/Feedback/Feedback/MPR1Report/",
                cache: false,
                type: "POST",
                async: false,
                data: $("#frmMPR1").serialize(),
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
                url: '/ECBriefReport/ECBriefReport/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_CompletedRoadDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_CompletedRoadDetails').find("option[value='0']").remove();
                    //$("#DistrictList_CompletedRoadDetails").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_CompletedRoadDetails').val(0);

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
                url: '/ECBriefReport/ECBriefReport/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_CompletedRoadDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_CompletedRoadDetails").val($("#Mast_Block_Code").val());
                        // $("#BlockList_CompletedRoadDetails").attr("disabled", "disabled");
                        //$("#BlockList_CompletedRoadDetails").trigger('change');
                    }
                    //$('#BlockList_CompletedRoadDetails').find("option[value='0']").remove();
                    //$("#BlockList_CompletedRoadDetails").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_CompletedRoadDetails').val(0);
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
function changeBlockCN1(stateCode, districtCode) {
    $("#BlockName").val($("#BlockList_CompletedRoadDetails option:selected").text());
}

function validate() {
    //Get all selected values for Group Code
    if ($('#ColumnList :selected').length > 0) {
        //build an array of selected values
        var selectednumbers = [];
        $('#ColumnList :selected').each(function (i, selected) {
            selectednumbers[i] = $(selected).val();

            //append selected values as comma seperated and assign to hidden field
            if (i == 0) {
                if (selectednumbers[i] > 0) {
                    $("#hdColumnCode").val(selectednumbers[i]);
                }
            }
            else {
                if (selectednumbers[i] > 0) {
                    if ($("#hdColumnCode").val() != "") {
                        $("#hdColumnCode").val($("#hdColumnCode").val() + "," + selectednumbers[i]);
                    }
                    else {
                        {
                            $("#hdColumnCode").val(selectednumbers[i]);
                        }
                    }
                }
            }

            $("#showLevelError").html("");
            $("#showLevelError").removeClass("field-validation-error");
        });
    }
    else {
        //$("#showLevelError").html("Map at least one of the Levels");
        //$("#showLevelError").addClass("field-validation-error");
        alert("Map at least one of the Levels");
        return false;
    }
    //alert($("#hdColumnCode").val());
}

function loadColumns() {
    $.ajax({
        url: '/Feedback/Feedback/GetColumns/',
        type: 'GET',
        catche: false,
        async: false,
        success: function (response) {
            var strhtml = "<select name='example-optgroup' multiple='multiple' id='ColumnList'>";
            var count = 0;
            $.each(response, function (data, item) {
                if (item.Parent != 0) {
                    if (item.Parent == item.Child) {
                        strhtml += "<option style='margin-left:5px;' value='" + item.Id + "'>" + item.Name + "</option>";
                        count = 0;
                    }
                    else {
                        count++;
                        if (item.Child == 0) {
                            strhtml += "<optgroup label='" + item.Category + "'>";
                            if (item.Ancestor == 1 || item.Ancestor == 2 || item.Ancestor == 3) {
                                strhtml += "<option value='" + item.Id + "'class=A1>" + item.Name + "</option>";
                            }
                        }
                        if (item.Child == 0 && count == 0) { //item.Child == 0 && count == 0
                            strhtml += "</optgroup>";
                        }
                    }
                }
            });

            strhtml += "</select>";
            $('#dvColumnList').html(strhtml);

            //alert(strhtml);

            $("#ColumnList").multiselect({
                Width: 150,
                position: {
                    my: 'left bottom',
                    at: 'left top',
                }
            });

            $("#ColumnList").multiselect("checkAll");

            $('.A1').hide();

            $("input[type='checkbox']").siblings("span").css("margin-left", "5px");
        },
        error: function () {
            $.unblockUI();
            alert("Error ocurred");
            return false;
        },
    });
}