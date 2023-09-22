$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmStateAchievement');

    $("#spCollapseIconNewPMR").trigger('click');

    $("#spCollapseIconNewPMR").click(function () {

        $("#spCollapseIconNewPMR").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#loadFilters").toggle("slow");

    });

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });

    //$("#loadReport").load("/MPR/MPR/MPR1Report/" + $("#ddlYearMPR1").val() + "$" + $("#ddlMonthMPR1").val() + "$" + $("#ddlCollabMPR1").val(), $.unblockUI());
    if ($("#frmStateAchievement").valid()) {

        $("#StateName").val($("#ddlState option:selected").text());
        $("#DistName").val($("#ddlDistrict option:selected").text());
        $("#BlockName").val($("#ddlBlock option:selected").text());
        $("#CollabName").val($("#ddlCollab option:selected").text());
        //$("#YearName").val($("#ddlYear option:selected").text());

        $.ajax({
            url: "/ProgressReport/Progress/AchievementStateReport/",
            cache: false,
            type: "POST",
            async: false,
            data: $("#frmStateAchievement").serialize(),
            success: function (data) {
                $("#loadReport").html('');
                $("#loadReport").html(data);
                $.unblockUI();
            },
            error: function () {
                alert("error");
            }
        })
    }
    else {
        $.unblockUI();
    }


    $('#ddlState').change(function () {
        //$.ajax({
        //    type: "POST",
        //    data: { stateCode: $('#ddlState').val() },
        //    url: '/ProgressReport/Progress/PopulateAgencies',
        //    dataType: 'json',
        //    success: function (json) {
        //        var $el = $("#ddlAgency");
        //        $el.empty(); // remove old options
        //        $el.append($("<option></option>").attr("value", '').text('All Agencies'));
        //        $.each(json, function (value, key) {
        //            alert(value, key);
        //            $el.append($("<option></option>").attr("value", value).text(key));
        //        });
        //    }
        //});

        $('#ddlDistrict').empty();
        FillInCascadeDropdown({ userType: $("#ddlState").find(":selected").val() },
                    "#ddlDistrict", "/ProgressReport/Progress/PopulateDistricts?stateCode=" + $('#ddlState option:selected').val());

        //FillInCascadeDropdown({ userType: $("#ddlAgency").find(":selected").val() }, "#ddlAgency", "/ProgressReport/Progress/PopulateAgencies?stateCode=" + $('#ddlState option:selected').val());
    });

    $('#ddlDistrict').change(function () {
        $('#ddlBlock').empty();
        FillInCascadeDropdown({ userType: $("#ddlDistrict").find(":selected").val() },
                    "#ddlBlock", "/ProgressReport/Progress/PopulateBlocks?distCode=" + $('#ddlDistrict option:selected').val());
    });

    $("#btnViewAchievementStateReport").click(function () {

        if ($("#frmStateAchievement").valid()) {
            if (($('#ddlStateList').is(":visible"))) {
                //$("#AgencyName").val($("#ddlAgency option:selected").text());
            }
            if ($("#Mast_State_Code").val() == "0") {
                $("#StateName").val($("#ddlState option:selected").text());
            }
            //$("#StateName").val($("#ddlState option:selected").text());
            $("#DistName").val($("#ddlDistrict option:selected").text());
            $("#BlockName").val($("#ddlBlock option:selected").text());
            $("#CollabName").val($("#ddlCollab option:selected").text());
            //$("#YearName").val($("#ddlYear option:selected").text());
            $.ajax({
                url: "/ProgressReport/Progress/AchievementStateReport/",
                cache: false,
                type: "POST",
                async: false,
                data: $("#frmStateAchievement").serialize(),
                success: function (data) {
                    $("#loadReport").html('');
                    $("#loadReport").html(data);
                },
                error: function () {
                    alert("error");
                }
            })
        }
        $.unblockUI();
    });

    closableNoteDiv("divCompRoads", "spnCompRoads");
});


function FillInCascadeDropdown(map, dropdown, action) {

    var message = '';
    $(dropdown).empty();

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();

} //end FillInCascadeDropdown()


