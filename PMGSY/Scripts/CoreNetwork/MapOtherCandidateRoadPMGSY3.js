$.validator.addMethod("comparestartchainge", function (value, element, params) {

    var startChainage = parseFloat($("#txtStartChainage").val());
    if ($("#rdbPartly").is(':checked')) {
        if ((parseFloat($("#ExistStartChainage").val()) <= startChainage)) {
            return true;
        }
    }

    if ($("#rdbFully").is(':checked')) {
        if ((parseFloat($("#ExistStartChainage").val()) <= startChainage)) {
            return true;
        }
    }
    return false;
});
jQuery.validator.unobtrusive.adapters.addBool("comparestartchainge");

$.validator.addMethod("compareendchainge", function (value, element, params) {

    var endChainage = parseFloat($("#txtEndChainage").val());
    if ($("#rdbPartly").is(':checked')) {
        if (endChainage <= (parseFloat($("#ExistEndChainage").val()))) {
            return true;
        }
    }

    if ($("#rdbFully").is(':checked')) {
        if (endChainage <= (parseFloat($("#ExistEndChainage").val()))) {
            return true;
        }
    }

    return false;
});
jQuery.validator.unobtrusive.adapters.addBool("compareendchainge");

$.validator.addMethod("comparechainage", function (value, element, params) {

    var startChainage = parseFloat($("#txtStartChainage").val());
    var endChainage = parseFloat($("#txtEndChainage").val());
    if ($("#rdbPartly").is(':checked')) {
        if (startChainage <= endChainage) {
            return true;
        }
    }

    if ($("#rdbFully").is(':checked')) {

        if (startChainage <= startChainage) {
            return true;
        }
    }
    return false;
});
jQuery.validator.unobtrusive.adapters.addBool("comparechainage");

$(document).ready(function () {


    $("#rowChainage").hide();

    $("#txtLengthOfRoad").val('');
    $("#txtStartChainage").val('');
    $("#txtEndChainage").val('');
    $("#ddlBlock").change(function () {
        if ($("#ddlRoadCategories").val() > 0) {
            FillInCascadeDropdown({ userType: $("#ddlRoadCategories").find(":selected").val() },
                     "#ddlDRRP", "/CoreNetwork/GetRoadNameByRoadCodePMGSY3?roadName=" + $('#ddlRoadCategories option:selected').val() + "&blockName=" + $("#ddlBlock option:selected").val());

        }
        else {
            $("#ddlDRRP").val(0);
            $("#ddlDRRP").empty();
            $("#ddlDRRP").append("<option value='0'>-Select Road Name-</option>");
        }
    });
    $("#ddlRoadCategories").change(function () {

        if ($("#ddlBlock option:selected").val() == 0) {
            alert('Please select Block.');
        }
        else {
            FillInCascadeDropdown({ userType: $("#ddlRoadCategories").find(":selected").val() },
                           "#ddlDRRP", "/CoreNetwork/GetRoadNameByRoadCodePMGSY3?roadName=" + $('#ddlRoadCategories option:selected').val() + "&blockName=" + $("#ddlBlock option:selected").val());
        }

    });

    //$("#ddlBlock").change(function () {

    //    if ($("#ddlRoadCategories option:selected").val() == 0) {
    //        alert('Please select Road Category.');
    //    }
    //    else {
    //        FillInCascadeDropdown({ userType: $("#ddlBlocks").find(":selected").val() },
    //                       "#ddlDRRP", "/CoreNetwork/GetRoadNameByRoadCode?roadName=" + $('#ddlRoadCategories option:selected').val() + "&blockName=" + $("#ddlBlock option:selected").val());
    //    }

    //});

    $("#ddlDRRP").change(function () {

        if ($("#rdbFully").val() == "F") {

            PopulateChainageLength();
        }
        else {
            $("#txtStartChainage").val('');
            $("#txtEndChainage").val('');
            $("#txtLengthOfRoad").val('');
        }
    });

    $("#rdbPartly").click(function () {

        $("#txtStartChainage").attr("readOnly", false);
        $("#txtEndChainage").attr("readOnly", false);


        $("#txtRoadToChainage").val($("#StartChainage").val());
        $("#txtRoadFromChainage").val($("#EndChainage").val());
        $("#rowChainage").show('slow');

    });

    $("#rdbFully").click(function () {
        PopulateChainageLength();
        $("#rowChainage").hide('slow');
    });



    LoadMappedRoadsList($("#CNCode").val());

    $("#btnSaveCandidateRoad").click(function () {

        if ($("#frmMapDRRP").valid()) {

            blockPage();

            $.ajax({
                type: 'POST',
                url: '/CoreNetwork/MapCandidateRoadPMGSY3/',
                async: false,
                data: $("#frmMapDRRP").serialize(),
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
                        $("#tblListMappedRoads").trigger("reloadGrid");
                        $("#btnReset").trigger('click');
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
                    alert(xhr.responseText);
                }
            })

        }

    });

    $("#closeMappingDetails").click(function () {
        $("#dvMapDRRP").hide('slow');
    });

    $("#finalize").click(function ()
    {
        if (confirm("Once Finalized,Another DRRP Roads will not be able to map.\n Are you sure you want to finalize DRRP Details?")) {
            $.ajax({
                type: 'POST',
                url: '/CoreNetWork/FinalizeMappedDRRPDetailsPMGSY3?id=' + $("#CNCode").val(),
                dataType: 'json',
                async: false,
                cache: false,
                data: { __RequestVerificationToken: $("#frmMapDRRP input[name=__RequestVerificationToken]").val() },
                success: function (data) {
                    if (data.success) {
                        //alert("DRRP details finalized successfully ");
                        alert(data.message);
                        $("#tblListMappedRoads").trigger('reloadGrid');
                        $("#dvMapDRRP").hide('slow');
                    }
                    else {
                        //alert("Error occurred while processing your request.");
                        alert(data.message);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("Error occurred while processing the request.");
                }
            });
        }
        else {
            return false;
        }
    });

    $("#definalize").click(function ()
    {
        if (confirm("Are you sure you want to definalize DRRP Details?")) {
            $.ajax({
                type: 'POST',
                url: '/CoreNetWork/DeFinalizeMappedDRRPDetailsPMGSY3?id=' + $("#CNCode").val(),
                dataType: 'json',
                data: { __RequestVerificationToken: $("#frmMapDRRP input[name=__RequestVerificationToken]").val() },
                async: false,
                cache: false,
                success: function (data) {
                    if (data.success) {
                        alert("DRRP details definalized successfully ");
                        $("#tblListMappedRoads").trigger('reloadGrid');
                        $("#dvMapDRRP").show('slow');
                    }
                    else {
                        alert("Error occurred while processing your request.");
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("Error occurred while processing the request.");
                }
            });
        }
        else {
            return false;
        }
    });

    $("#btnReset").click(function () {

        $("#divError").hide('slow');
        $("#rowChainage").hide();
    });

    $("#txtEndChainage").blur(function () {

        var roadTo = $("#txtStartChainage").val();
        var roadFrom = $("#txtEndChainage").val();
        var roadLength = roadFrom - roadTo;
        $("#txtLengthOfRoad").val(parseFloat(roadLength).toFixed(3));

    });
});
//for cascade dropdown filling
function FillInCascadeDropdown(map, dropdown, action) {
    var message = '';

    if (dropdown == '#ddlRoadCategories') {
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
//for automatically populating start chainage and end chainage textboxes
function PopulateChainageLength() {

    var existingRoadCode = $("#ddlDRRP option:selected").val();
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
            $("#txtStartChainage").attr("readOnly", true);
            $("#txtEndChainage").attr("readOnly", true);
            $("#txtLengthOfRoad").attr("readOnly", true);
            $("#txtStartChainage").val(startChainage);
            $("#txtEndChainage").val(endChainage);
            $("#txtLengthOfRoad").val(parseFloat(roadLength).toFixed(3));
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
//Grid for listing the mapped roads with the particular candidate road
function LoadMappedRoadsList(CNCode) {
    $("#tblListMappedRoads").jqGrid('GridUnload');
    jQuery("#tblListMappedRoads").jqGrid({
        url: '/CoreNetwork/GetMappedCandidateRoadListPMGSY3',
        datatype: "json",
        mtype: "GET",
        postData: { RoadCode: CNCode },
        colNames: ['Block', 'Category', 'Road Name', 'Length (in Kms)', 'Partial/Full', 'Edit', 'Delete', 'Map Habitation'],
        colModel: [
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 150, align: "left", search: false },
                            { name: 'MAST_ROAD_CAT_NAME', index: 'MAST_ROAD_CAT_NAME', height: 'auto', width: 300, align: "left", search: true },
                            { name: 'MAST_ER_ROAD_NAME', index: 'MAST_ER_ROAD_NAME', height: 'auto', width: 300, align: "left", search: false },
                            { name: 'PLAN_RD_LENGTH', index: 'PLAN_RD_LENGTH', height: 'auto', width: 100, align: "right", search: false },
                            { name: 'PLAN_RD_LENG', index: 'PLAN_RD_LENG', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'c', width: 50, sortable: false, resize: false, align: "center", search: false, hidden: true },
                            { name: 'a', width: 50, sortable: false, resize: false, align: "center", search: false },
                            { name: 'b', width: 50, sortable: false, resize: false, align: "center", search: false, hidden: true },
        ],
        pager: jQuery('#pgrListMappedRoads').width(20),
        rowNum: 10,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "desc",
        sortname: 'MAST_ER_ROAD_CODE',
        caption: "&nbsp;&nbsp; Mapped DRRP Road List",
        height: 'auto',
        //autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
            //alert(jQuery("#tblListMappedRoads").jqGrid('getGridParam', 'records'));

            if (data.IsFinalized == "Y") {
                $("#tblDeFinalize").show('slow');
                $("#tblFinalize").hide();
                $("#dvMapDRRP").hide();
            }
            else if (data.IsFinalized == "N") {
                $("#tblFinalize").show('slow');
                $("#tblDeFinalize").hide();
            }
            else if ($("#tblFinalize").is(':visible')) {
                $("#tblFinalize").hide();
                $("#tblDeFinalize").hide('slow');
            }

            //alert(parseFloat($('#lblTotLength').text()));
            //alert(parseFloat(jQuery("#tblListMappedRoads").jqGrid('getCol', 'PLAN_RD_LENGTH', false, 'sum')));

            $("#pgrListMappedRoads_left").html("<label style='margin-left:8%;'><b>Balance Length: </b> " + (parseFloat($('#lblTotLength').text()) - parseFloat(jQuery("#tblListMappedRoads").jqGrid('getCol', 'PLAN_RD_LENGTH', false, 'sum'))).toFixed(3) + " <label/>")
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });
}
function DeleteCandidateRoad(urlparameter) {
    if (confirm("Are you sure you want to delete DRRP Details?")) {
        $.ajax({
            type: 'POST',
            url: '/CoreNetWork/DeleteMappedDRRPDetailsPMGSY3/' + urlparameter,
            dataType: 'json',
            data: { __RequestVerificationToken: $("#frmSearchCoreNetworks input[name=__RequestVerificationToken]").val() },
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert("DRRP details deleted successfully ");
                    $("#tblListMappedRoads").trigger('reloadGrid');
                    $("#btnReset").trigger('click');
                }
                else {
                    alert("Error occurred while processing your request.");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Error occurred while processing the request.");
            }
        });
    }
    else {
        return false;
    }
}
function MapOrDeleteHabitations(urlParam) {
    LoadHabitationList(urlParam);
    $("#divDialogHabitations").dialog("open");
}
function LoadHabitationList(urlParam) {
    jQuery("#tblstHabitations").jqGrid({
        url: '/CoreNetwork/GetDRRPMappedHabitationList',
        datatype: "json",
        mtype: "POST",
        postData: { RoadCode: urlParam },
        colNames: ['Name of Habitation', 'Block Name', 'Road Number', 'Census Year', 'Total Population', 'SC/ST Population', 'Delete'], //'SC/ST Population', 'Primary School', 'Middle Schools', 'High Schools', 'Intermediate Schools', 'Degree College', 'Health Services','Dispensaries'],//,'MCW Centers','PHCS','Vetarnary Hospitals','Telegraph Office','Telephone Connections','Bus Service','Railway Stations','Electricity','Panchayat Head Quarters','Tourist Place'],
        colModel: [
                            { name: 'MAST_HAB_NAME', index: 'MAST_HAB_NAME', width: 300, align: "left", sortable: true },
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', width: 100, align: "left", sortable: true },
                            { name: 'MAST_ER_ROAD_NUMBER', index: 'MAST_ER_ROAD_NUMBER', width: 100, sortable: true, align: "center" },
                            { name: 'MASTER_CENSUS_YEAR', index: 'MASTER_CENSUS_YEAR', width: 100, sortable: true, align: "center" },
                            { name: 'MAST_HAB_TOT_POP', index: 'MAST_HAB_TOT_POP', width: 100, sortable: true, align: "center" },
                            { name: 'MAST_HAB_SCST_POP', index: 'MAST_HAB_SCST_POP', width: 100, sortable: true, align: "center" },
                            { name: 'a', width: 50, sortable: false, resize: false, align: "center", search: false },
        ],
        pager: jQuery('#dvPagerHabitations'),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_HAB_NAME',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Mapped Habitation List",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        loadComplete: function (data) {

        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });
}
function DeleteHabitation(urlparam) {
    if (confirm("Are you sure you want to delete Habitation Details?")) {
        $.ajax({
            type: 'POST',
            url: '/CoreNetWork/DeleteMappedDRRPHabitation/' + urlparam,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert("Habitation details deleted successfully ");
                    $("#tblstHabitations").trigger('reloadGrid');
                }
                else {
                    alert("Error occurred while processing your request.");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Error occurred while processing the request.");
            }
        });
    }
    else {
        return false;
    }
}
function EditCandidateRoad(urlparameter) {

    $("#dvMapDRRP").load('/CoreNetWork/EditMappedDRRPDetails/' + urlparameter, function () {
        $.validator.unobtrusive.parse($('#dvMapDRRP'));
    });
}