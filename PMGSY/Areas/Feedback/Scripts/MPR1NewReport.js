$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmMPR1');

    $("#spCollapseIconNewPMR").trigger('click');

    $("#spCollapseIconNewPMR").click(function () {

        $("#spCollapseIconNewPMR").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#loadFilters").toggle("slow");

    });

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });

    //$("#loadReport").load("/MPR/MPR/MPR1Report/" + $("#ddlYearMPR1").val() + "$" + $("#ddlMonthMPR1").val() + "$" + $("#ddlCollabMPR1").val(), $.unblockUI());
    if ($("#frmMPR1").valid()) {

        $("#CollaborationName").val($("#ddlCollabMPR1 option:selected").text());

        $.ajax({
            url: "/Feedback/Feedback/MPR1NewReport/",
            cache: false,
            type: "POST",
            async: false,
            data: $("#frmMPR1").serialize(),
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


    if ($('#Mast_State_Code').val() != 0) {
        FillInCascadeDropdown({ userType: $('#Mast_State_Code').val() },
                   "#ddlAgency", "/Feedback/Feedback/PopulateAgencies?stateCode=" + $('#Mast_State_Code').val());
    }

    $('#StateList_CompletedRoadDetails').change(function () {
        //$.ajax({
        //    type: "POST",
        //    data: { stateCode: $('#StateList_CompletedRoadDetails').val() },
        //    url: '/Feedback/Feedback/PopulateAgencies',
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

        $('#ddlAgency').empty();
        FillInCascadeDropdown({ userType: $("#StateList_CompletedRoadDetails").find(":selected").val() },
                    "#ddlAgency", "/Feedback/Feedback/PopulateAgencies?stateCode=" + $('#StateList_CompletedRoadDetails option:selected').val());

        //FillInCascadeDropdown({ userType: $("#ddlAgency").find(":selected").val() }, "#ddlAgency", "/Feedback/Feedback/PopulateAgencies?stateCode=" + $('#StateList_CompletedRoadDetails option:selected').val());
    });

    $("#btnViewMPR1Report").click(function () {
        if ($("#frmMPR1").valid()) {
            //if (($('#StateList_CompletedRoadDetails').is(":visible"))) {
            //    $("#AgencyName").val($("#ddlAgency option:selected").text());
            //}
            $("#AgencyName").val($("#ddlAgency option:selected").text());

            if ($("#Mast_State_Code").val() == "0") {
                $("#StateName").val($("#StateList_CompletedRoadDetails option:selected").text());
            }
            $("#CollaborationName").val($("#ddlCollabMPR1 option:selected").text());
          
             
            $.ajax({
                url: "/Feedback/Feedback/MPR1NewReport/",
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
        $.unblockUI();
    });

    closableNoteDiv("divCompRoads", "spnCompRoads");
});


function FillInCascadeDropdown(map, dropdown, action) {

    var message = '';

    //if (dropdown == '#ddlDistrict') {

    //    message = '<h4><label style="font-weight:normal"> Loading Districts... </label></h4>';
    //}
    //else if (dropdown == '#ddlBlocks') {
    //    message = '<h4><label style="font-weight:normal"> Loading Blocks... </label></h4>';
    //}

    $(dropdown).empty();
    //$(dropdown).append("<option value=0>--Select--</option>");

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    //$.blockUI({ message: message });

    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();

} //end FillInCascadeDropdown()


