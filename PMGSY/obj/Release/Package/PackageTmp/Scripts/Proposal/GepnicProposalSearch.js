

var jsLoadCount = 1;
$(document).ready(function () {
    var HeaderID;


    $(".groupHeader").click(function () {
        //        alert("checkbox");
    });
    //RegenrateCB();
    //$('#tbGepnicProposalList').on("change", "input[type=checkbox]", function (e) {
    //    var isDisabledFlag = false;
    //    var currentCB = $(this);
    //    var grid = jQuery('#tbGepnicProposalList');
    //    var isChecked = this.checked;

    //    if (currentCB.is(".groupHeader")) {

    //        HeaderID = currentCB.closest('tr').attr("id").split('_');

    //        // var tillElementID = parseInt(HeaderID[2]) + 1;
    //         var tillElementID = parseInt(HeaderID[2]) + 1;

    //        var checkboxes = currentCB.closest('tr').nextUntil('tr#tbGepnicProposalListghead_0_' + tillElementID).find('.cbox[type="checkbox"]');
    //        checkboxes.each(function () {
    //            if (!isChecked) {
    //                grid.setSelection($(this).closest('tr').attr('id'), true);
    //            }
    //            else {
    //                grid.setSelection($(this).closest('tr').attr('id'), false);
    //            }
    //        });
    //        if (isDisabledFlag) {
    //            return;
    //        }
    //        for (var i = 0 ; i < checkboxes.length ; i++) {
    //            $("#" + checkboxes[i].id).attr("disabled", true);
    //        }

    //    }
    //    else {
    //        var allCbs = currentCB.closest('tr').prevAll("tr.gridghead_0:first").nextUntil('tr.gridghead_0').andSelf().find('[type="checkbox"]');
    //        var allSlaves = allCbs.filter('.cbox');
    //        var headerCB = allCbs.filter(".groupHeader");
    //        var allChecked = !isChecked ? false : allSlaves.filter(":checked").length === allSlaves.length;
    //        headerCB.prop("checked", allChecked);
    //        for (var i = 0 ; i < allSlaves.length ; i++) {
    //            if ($("#" + allSlaves[i].id).is(":disabled")) {
    //                allSlaves[i].disabled = false;
    //            }
    //        }
    //    }
    //    jsLoadCount++;
    //});

    $("#dvGepnicProposalModal").dialog({
        autoOpen: false,
        height: '300',
        width: "600",
        modal: true,
        title: 'Organisation for Gepnic'
    });

    $('#btnListProposal').click(function () {

        if ($('#frmGepnicFilterForm').valid()) {
            LoadGepnicProposalList($('#ddlDisticts option:selected').val(), $('#ddlSanctionYears option:selected').val(), $('#ddlBlocks option:selected').val(), $('#ddlProposalTypes option:selected').val(), $('#ddlPackages option:selected').val());
            //    if ($(".ui-pg-input").val() % 2 == 0) {
            //        alert($(".ui-pg-input").val());
            //        RegenrateCB();
            //    }

        }

    });

    $('#ddlState').change(function () {

        if ($('#ddlState option:selected').val() > 0) {
            $.ajax({
                url: '/ECBriefReport/ECBriefReport/PopulateDistricts',
                type: 'POST',
                beforeSend: function () {
                    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
                },
                data: { stateCode: $("#ddlState option:selected").val(), value: Math.random() },
                success: function (jsonData) {
                    $("#ddlDisticts").empty();
                    for (var i = 0; i < jsonData.length; i++) {
                        if (jsonData[i].Selected == true) {
                            $("#ddlDisticts").append("<option value='" + jsonData[i].Value + "' selected=true>" + jsonData[i].Text + "</option>");
                        }
                        else {
                            $("#ddlDisticts").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    }

                    $.unblockUI();
                },
                error: function (err) {
                    //alert("error " + err);
                    $.unblockUI();
                }
            });
        }
        else {
            $("#ddlDisticts").empty();
            $("#ddlDisticts").append("<option value='0'>All Districts</option>");
        }
    });

    $('#ddlDisticts').change(function () {
        //isDistChange = true;
        if ($('#ddlDisticts option:selected').val() > 0) {
            $.ajax({
                url: '/Proposal/PopulateBlocks',
                type: 'POST',
                beforeSend: function () {
                    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
                },
                data: { districtCode: $("#ddlDisticts option:selected").val(), value: Math.random() },
                success: function (jsonData) {
                    $("#ddlBlocks").empty();
                    for (var i = 0; i < jsonData.length; i++) {
                        if (jsonData[i].Selected == true) {
                            $("#ddlBlocks").append("<option value='" + jsonData[i].Value + "' selected=true>" + jsonData[i].Text + "</option>");
                        }
                        else {
                            $("#ddlBlocks").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    }

                    $.unblockUI();
                },
                error: function (err) {
                    //alert("error " + err);
                    $.unblockUI();
                }
            });

            // $("#ddlSanctionYears").empty();
            //  $("#ddlSanctionYears").append("<option value='" + 0 + "' selected=true>" + "All Stuff" + "</option>");

            $.ajax({
                url: '/Proposal/PopulateSanctionedYearOnChnageofDistrict',
                type: 'POST',
                beforeSend: function () {
                    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
                },
                // data: { districtCode: $("#ddlDisticts option:selected").val(), value: Math.random() },
                success: function (jsonData) {
                    $("#ddlSanctionYears").empty();
                    for (var i = 0; i < jsonData.length; i++) {
                        if (jsonData[i].Selected == true) {
                            $("#ddlSanctionYears").append("<option value='" + jsonData[i].Value + "' selected=true>" + jsonData[i].Text + "</option>");
                        }
                        else {
                            $("#ddlSanctionYears").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    }

                    $.unblockUI();
                },
                error: function (err) {
                    //alert("error " + err);
                    $.unblockUI();
                }
            });

        } else {



            $.ajax({
                url: '/Proposal/PopulateSanctionedYear',
                type: 'POST',
                beforeSend: function () {
                    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
                },
                // data: { districtCode: $("#ddlDisticts option:selected").val(), value: Math.random() },
                success: function (jsonData) {
                    $("#ddlSanctionYears").empty();
                    for (var i = 0; i < jsonData.length; i++) {
                        if (jsonData[i].Selected == true) {
                            $("#ddlSanctionYears").append("<option value='" + jsonData[i].Value + "' selected=true>" + jsonData[i].Text + "</option>");
                        }
                        else {
                            $("#ddlSanctionYears").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    }

                    $.unblockUI();
                },
                error: function (err) {
                    //alert("error " + err);
                    $.unblockUI();
                }
            });
        }

    });
});

function LoadGepnicProposalList(District, Year, Block, ProposalType, Package) {
    blockPage();

    jQuery("#tbGepnicProposalList").jqGrid('GridUnload');
    //RegenrateCB();
    jQuery("#tbGepnicProposalList").jqGrid({
        url: '/Proposal/GetGepnicProposalList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Proposal Type', 'District', 'Block', 'Sanction Year', "Package Number", "Road / Bridge / Building Name ", "Length", "Sanction Cost", "Stage Construction", 'Proposal Status', 'Enable Proposal for repushing to Gepnic'],
        colModel: [

                    { name: 'WorkType', index: 'WorkType', width: 20, sortable: false, align: "center" },
                    { name: 'District', index: 'District', width: 30, sortable: false, align: "center" },
                    { name: 'Block', index: 'Block', width: 30, sortable: false, align: "center" },
                    { name: 'SanctionYear', index: 'SanctionYear', width: 30, sortable: false, align: "center" },
                    { name: 'PackageNumber', index: 'PackageNumber', width: 50, sortable: false, align: "center" },
                    { name: 'RoadName', index: 'RoadName', width: 80, sortable: false, align: "left" },
                    { name: 'Length', index: 'Length', width: 20, sortable: false, align: "right" },
                    { name: 'SanctionCost', index: 'SanctionCost', width: 20, sortable: false, align: "right" },
                    { name: 'StageConstruction', index: 'StageConstruction', width: 25, sortable: false, align: "right" },//added by abhinav on 9-july-2019
                    { name: 'IsProposalSent', index: 'IsProposalSent', hidden: false, width: 30, sortable: false, align: "center" }, //  hidden : true,

                    { name: 'UploadDetails', index: 'UploadDetails', width: 20, sortable: false, hidden: true, align: "center" }
      //  { name: 'UploadDetails', index: 'UploadDetails', width: 20, sortable: false, align: "center", hidden: parseInt($('#hdnRole').val()) == 25 ? false : true } show for mrd2 and hide for srrda login
        ],
        postData: { State: $('#ddlState option:selected').val(), "District": District, "Year": Year, "Block": Block, "ProposalType": ProposalType, "Package": Package },
        navOptions: { reloadGridOptions: { fromServer: true } },
        pager: jQuery('#dvGepnicProposalListPager'),
        rowList: [15, 30, 45],
        rowList: [10, 20, 30, 40, 50],
        rowNum: 15,
        loadonce: false,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Road Proposals",
        height: 'auto',
        width: 'auto',
        autowidth: true,
        sortname: 'Block',
        rownumbers: true,
        multiselect: true,
        footerrow: true,
        beforeSelectRow: function (rowid, e) {
            return false;
        },
        onSelectAll: function (rowIds, allChecked) {
            alert($("input.groupHeader").attr('checked', allChecked))
        },
        grouping: true,
        groupingView: {
            groupField: ['PackageNumber'],
            groupDataSorted: true,
            //groupText: ["<input type='checkbox' class='groupHeader'/> <b>  {0}  </b>", "Check to select all roads."
            //],
            groupText: [


             "<input  type='checkbox' id=\"cb1" + "{0}" + "\" class='groupHeader' onClick ='checkEvHandler(\"" + "{0}" + "\")' /><b> Package Number :  {0} </b>" + " " + "<input type='button' id=\"file" + "{0}" + "\"  class='btnSendProposal1' onClick = 'GetGepnicOrg()' value='Send Proposal to Gepnic'/>"
           + "  " + '<input type="button"   class="unsend2" id=\'fi' + "{0}" + '\' title="After finalizing all individual roads, Click here to finalize Package."  value="Enable Proposal for repushing to Gepnic" onclick="checkplus(\'' + "{0}" + '\')">'

            ],
            groupColumnShow: [false],
        },
        caption: "Proposal List         Note : Only one package can be selected at a time."
        ,


        loadComplete: function (data) {
            for (var i = 0; i < data.rows.length; i++) {
                var rowData = data.rows[i];
                if (rowData.cell[8] == 'True') {
                    //update this to have your own check
                    var checkbox = $("#jqg_tbGepnicProposalList_" + rowData['id']);//update this with your own grid name
                    //checkbox.css("visibility", "hidden");
                    checkbox.attr("disabled", true);

                }
                $("#cb_tbGepnicProposalList").attr("disabled", true);

                if (jsLoadCount != 1) {
                    //alert(jsLoadCount);
                    //RegenrateCB();
                }
            }

            if (data["records"] > 0) {

                $("#tbGepnicProposalList #dvGepnicProposalListPager").css({ height: '31px' });


                // $("#dvGepnicProposalListPager_left").html("<input type='button' style='margin-left:27px' id='btnSendProposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'GetGepnicOrg();return false;' value=''/>");
            }
            $(".cbox").attr("disabled", true);
            unblockPage();
            if (District > 0) {
                jQuery("#tbGepnicProposalList").unbind("change");
                RegenrateDistrictCB();
            }
            else {
                jQuery("#tbGepnicProposalList").unbind("change");
                RegenrateCB();
            }


            //  Ajinkya More 
            $(".btnSendProposal1").attr('disabled', true);
            $(".unsend2").attr('disabled', true);


        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                //alert(xhr.responseText);
                window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert("Session Timeout !!!");
                window.location.href = "/Login/SessionExpire";
            }
        }
    }); //end of grid     ("#tbGepnicProposalList").trigger('reloadGrid');

    unblockPage();
}




//Ajinkya More
function UploadFile(urlParameter) {
    //alert("Value : " + JSON.stringify(urlParameter));
    // alert("Value : " + JSON.stringify(urlParameter.Value));
    //alert(District)
    var roadcode = JSON.stringify(urlParameter);
    // alert(roadcode.Value);
    //roadcode = JSON.stringify({ 'roadcode': roadcode });
    // alert(roadcode)
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' })
    $.ajax({
        url: "/Proposal/EnableRepush/",

        type: "POST",
        async: false,
        dataType: "json",
        catche: false,
        contentType: "application/json; charset=utf-8",
        data: roadcode,
        success: function (data) {

            $('#tbGepnicProposalList').trigger('reloadGrid');
            alert("This work is enabled for repushing to Gepnic.");
        },
        error: function (xht, ajaxOptions, throwError) {

            //alert(xht.responseText);
            // alert("This work is not enabled for repushing to Gepnic.");
            alert("Proposal Re-enable to Sent to Gepnic");
            $('#tbGepnicProposalList').trigger('reloadGrid');
        }

    });
    $.unblockUI();
}
// Ajinkya More
function checkEvHandler(id) {

    $("#cb1" + id).change(function () {
        if (this.checked) {
            //for button desabled
            $("#file" + id).attr('disabled', false);
            $("#fi" + id).attr('disabled', false);

            //disable all check button except selected one
            $(".groupHeader").attr('disabled', true);
            $("#cb1" + id).attr('disabled', false);
        }
        else {
            $("#file" + id).attr('disabled', true);
            $("#fi" + id).attr('disabled', true);
            $(".groupHeader").attr('disabled', false);
        }
    });




}
// Ajinkya More
function checkplus(id) {

    var selectedIds = $('#tbGepnicProposalList').jqGrid('getGridParam', 'selarrrow').toString().split(',');

    if (selectedIds != "") {

        $.ajax({
            type: 'POST',
            url: '/Proposal/RoadCodeCheck?id=' + id,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                // alert(data.value);
                if (JSON.stringify(data) == '{"roadcode":[]}') {
                    alert("Only those package already pushed to Gepnic can be enabled for repushing to Gepnic");
                }
                else {
                    UploadFile(data)
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.responseText);
            }
        });
    }
    else {
        alert("Please Select Package Number");
    }
}



function SendGepnicProposal() {
    cellValues = [];

    var selectedIds = $('#tbGepnicProposalList').jqGrid('getGridParam', 'selarrrow').toString().split(','); //jQuery('#tbGepnicProposalList').jqGrid('getGridParam', 'selarrrow');

    $.ajax({
        type: 'POST',
        url: '/Proposal/InsertGepnicProposals',
        data: { proposals: selectedIds },
        async: false,
        cache: false,
        success: function (data) {
            if (data.success == true) {
                alert(data.message);
                $('#tbGepnicProposalList').trigger('reloadGrid');
            }
            else {
                alert(data.message);
            }
        },
        error: function () {
        }
    });
}


function GetGepnicOrg() {

    cellValues = [];
    var selectedIds = $('#tbGepnicProposalList').jqGrid('getGridParam', 'selarrrow').toString().split(',');
    if (selectedIds != "") {
        $.ajax({
            type: 'POST',
            url: '/Proposal/GepnicOrgansationsLayout',
            data: { proposals: selectedIds, State: $('#ddlState option:selected').val() },
            async: false,
            cache: false,
            success: function (data) {

                $("#dvGepnicProposalModal").html(data);
                $("#dvGepnicProposalModal").dialog('open');

            },
            error: function () {
                alert('Error ocurred');
            }
        });
    }
    else {

        alert("Please Select Package Number");
    }
}

function RegenrateCB() {

    //jQuery("#tbGepnicProposalList").trigger("reloadGrid")
    $('#tbGepnicProposalList').on("change", "input[type=checkbox]", function (e) {
        var isDisabledFlag = false;
        var currentCB = $(this);
        var grid = jQuery('#tbGepnicProposalList');
        var isChecked = this.checked;
        if (currentCB.is(".groupHeader")) {
            HeaderID = currentCB.closest('tr').attr("id").split('_');
            var tillElementID = parseInt(HeaderID[2]) + 1;
            var checkboxes = currentCB.closest('tr').nextUntil('tr#tbGepnicProposalListghead_0_' + tillElementID).find('.cbox[type="checkbox"]');
            checkboxes.each(function () {
                if (!isChecked) {
                    $(".groupHeader").not(currentCB).attr("disabled", false);

                    grid.setSelection($(this).closest('tr').attr('id'), true);
                }
                else {
                    $(".groupHeader").not(currentCB).attr("disabled", true);
                    grid.setSelection($(this).closest('tr').attr('id'), false);
                }
            });
            if (isDisabledFlag) {
                return;
            }
            for (var i = 0 ; i < checkboxes.length ; i++) {
                $("#" + checkboxes[i].id).attr("disabled", true);
            }

            HeaderCBArray.push("tr#tbGepnicProposalListghead_0_" + HeaderID[2]);
            //UncheckPreviousCB(HeaderCBArray[0]);
            return;

        }
        else {
            var allCbs = currentCB.closest('tr').prevAll("tr.gridghead_0:first").nextUntil('tr.gridghead_0').andSelf().find('[type="checkbox"]');
            var allSlaves = allCbs.filter('.cbox');
            var headerCB = allCbs.filter(".groupHeader");
            var allChecked = !isChecked ? false : allSlaves.filter(":checked").length === allSlaves.length;
            headerCB.prop("checked", allChecked);
            for (var i = 0 ; i < allSlaves.length ; i++) {
                if ($("#" + allSlaves[i].id).is(":disabled")) {
                    allSlaves[i].disabled = false;
                }
            }
        }
    });
    if ($(".ui-pg-input").val() % 2 == 0) {
        return;
    }
}
function RegenrateDistrictCB() {
    //jQuery("#tbGepnicProposalList").trigger("reloadGrid")

    $('#tbGepnicProposalList').on("change", "input[type=checkbox]", function (e) {
        var isDisabledFlag = false;
        var currentCB = $(this);
        var grid = jQuery('#tbGepnicProposalList');
        var isChecked = this.checked;
        if (currentCB.is(".groupHeader")) {

            HeaderID = currentCB.closest('tr').attr("id").split('_');

            var tillElementID = parseInt(HeaderID[2]) + 1;

            var checkboxes = currentCB.closest('tr').nextUntil('tr#tbGepnicProposalListghead_0_' + tillElementID).find('.cbox[type="checkbox"]');
            checkboxes.each(function () {
                if (!isChecked) {
                    grid.setSelection($(this).closest('tr').attr('id'), true);
                }
                else {
                    grid.setSelection($(this).closest('tr').attr('id'), false);
                }
            });
            if (isDisabledFlag) {
                return;
            }
            for (var i = 0 ; i < checkboxes.length ; i++) {

                $("#" + checkboxes[i].id).attr("disabled", true);
            }
            return;

        }
        else {

            var allCbs = currentCB.closest('tr').prevAll("tr.gridghead_0:first").nextUntil('tr.gridghead_0').andSelf().find('[type="checkbox"]');
            var allSlaves = allCbs.filter('.cbox');
            var headerCB = allCbs.filter(".groupHeader");
            var allChecked = !isChecked ? false : allSlaves.filter(":checked").length === allSlaves.length;
            headerCB.prop("checked", allChecked);
            for (var i = 0 ; i < allSlaves.length ; i++) {
                if ($("#" + allSlaves[i].id).is(":disabled")) {
                    allSlaves[i].disabled = false;
                }
            }
        }
    });
    if ($(".ui-pg-input").val() % 2 == 0) {
        return;
    }
}

