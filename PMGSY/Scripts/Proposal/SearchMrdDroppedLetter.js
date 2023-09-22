var stateCode;
var agencyCode;
var year;
var batch;
var collaboration;

$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmSearchMrdDropLetter');

    $("#accordion").accordion({
        icons: false,
        heightStyle: "content",
        autoHeight: false
    });

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    if ($('#hdStatCode').val() > 0) {

        $("#ddlMrdDropState").attr("disabled", true);
        $("#ddlMrdDropAgency option[value='0']").remove();
    }


    $.validator.unobtrusive.parse('#frmSearchMrdDropLetter');
    $("#btnMrdDropSearch").click(function () {
        $('#dvLoadDroppedLetters').html('');
        stateCode = $('#ddlMrdDropState option:selected').val();
        agencyCode = $('#ddlMrdDropAgency option:selected').val();
        year = $('#ddlMrdDropPhaseYear option:selected').val();
        batch = $('#ddlMrdDropBatch option:selected').val();
        collaboration = $('#ddlMrdDropCollaboration option:selected').val();
        LoadMrdClearanceGrid();
    });


    $("#btnMrdDropSearch").trigger('click');

    $('#ddlMrdDropState').change(function () {
        loadSearchAgencyList($('#ddlMrdDropState option:selected').val());
        $("#dvMrdDropSearch").show();

    });
    $("#dvhdSearch").click(function () {

        if ($("#dvMrdDropSearch").is(":visible")) {

            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvMrdDropSearch").slideToggle(300);
        }

        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvMrdDropSearch").slideToggle(300);
        }
    });
});


function LoadMrdClearanceGrid() {
    if ($("#frmSearchMrdDropLetter").valid()) {
        $("#tblMrdClearanceList").jqGrid('GridUnload');
        var pageWidth = $("#tblMrdClearanceList").parent().width() - 100;  //by pp
       
        jQuery("#tblMrdClearanceList").jqGrid({
            url: '/MRDProposal/GetMrdClearanceList',
            datatype: "json",
            mtype: "POST",
            colNames: ['Add Dropped Letter', 'State', 'Agency', 'Clearance Date', 'Clearance Number', 'Collaboration', 'Phase', 'Batch', 'Sanction Cost (MoRD Share Rs. in Cr.)', 'Sanction Cost (State Share Rs. in Cr.)', 'Total Sanction Cost (Rs. in Cr.)', 'No. of Roads', 'Road Length in Kms.', 'No. of Bridges', 'LSB Length in Mtrs.)',
                        'Hab >1000', 'Hab >500', 'Hab >250', 'Hab >100'], //26
            colModel: [

                        { name: 'Add', index: 'Add', height: 'auto', width: (pageWidth * (5/100)), align: "center", sortable: false },
                        { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: (pageWidth * (7 / 100)), align: "left", sortable: true },
                        { name: 'Agency', index: 'Agency', height: 'auto', width: (pageWidth * (6 / 100)), align: "center", sortable: true },
                        { name: 'Date', index: 'Date', height: 'auto', width: (pageWidth * (5 / 100)), align: "center", sortable: false, hidden: false },
                        { name: 'Number', index: 'Number', height: 'auto', width: (pageWidth * (6 / 100)), align: "center", sortable: false, hidden: false },
                        { name: 'Collaboration', index: 'Collaboration', height: 'auto', width: (pageWidth * (5 / 100)), align: "center", sortable: true },
                        { name: 'Year', index: 'Year', height: 'auto', width: (pageWidth * (5 / 100)), align: "center", sortable: true },
                        { name: 'Batch', index: 'Batch', height: 'auto', width: (pageWidth * (5 / 100)), align: "center", sortable: true },
                        { name: 'MORD_SHARE_AMT', index: 'MRD_ROAD_MORD_SHARE_AMT', height: 'auto', width: (pageWidth * (5 / 100)), align: "center", sortable: false },
                        { name: 'STATE_SHARE_AMT', index: 'MRD_ROAD_STATE_SHARE_AMT', height: 'auto', width: (pageWidth * (5 / 100)), align: "center", sortable: false },
                        { name: 'TOTAL_AMT', index: 'MRD_ROAD_TOTAL_AMT', height: 'auto', width: (pageWidth * (5 / 100)), align: "center", sortable: false },

                        { name: 'MRD_TOTAL_ROADS', index: 'MRD_TOTAL_ROADS', height: 'auto', width: (pageWidth * (5 / 100)), align: "center", sortable: false },
                        { name: 'MRD_TOTAL_ROAD_LENGTH', index: 'MRD_TOTAL_ROAD_LENGTH', height: 'auto', width: (pageWidth * (5 / 100)), align: "center", sortable: false },
                        { name: 'MRD_TOTAL_LSB', index: 'MRD_TOTAL_LSB', height: 'auto', width: (pageWidth * (5 / 100)), align: "center", sortable: false },
                        { name: 'MRD_TOTAL_LSB_LENGTH', index: 'MRD_TOTAL_LSB_LENGTH', height: 'auto', width: (pageWidth * (5 / 100)), align: "center", sortable: false },

                        { name: 'MRD_HAB_1000', index: 'MRD_HAB_1000', height: 'auto', width: (pageWidth * (5 / 100)), align: "center", sortable: false },
                        { name: 'MRD_HAB_500', index: 'MRD_HAB_500', height: 'auto', width: (pageWidth * (5 / 100)), align: "center", sortable: false },
                        { name: 'MRD_HAB_250_ELIGIBLE', index: 'MRD_HAB_250_ELIGIBLE', height: 'auto', width: (pageWidth * (5 / 100)), align: "center", sortable: false },
                        { name: 'MRD_HAB_100_ELIGIBLE', index: 'MRD_HAB_100_ELIGIBLE', height: 'auto', width: (pageWidth * (5 / 100)), align: "center", sortable: false },

            ],
            postData: { stateCode: $('#ddlMrdDropState option:selected').val(), agency: $('#ddlMrdDropAgency option:selected').val(), year: $('#ddlMrdDropPhaseYear option:selected').val(), batch: $('#ddlMrdDropBatch option:selected').val(), collaboration: $('#ddlMrdDropCollaboration option:selected').val() },
            pager: jQuery('#divMrdClearancepager'),
            rowNum: 10,
            rowList: [10, 15, 20, 30],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'MAST_STATE_NAME',
            sortorder: "asc",
            caption: "Clearance List",
            height: 'auto',
            autowidth: true,
            //width:'250',
            shrinkToFit: false,
            rownumbers: true,
            cmTemplate: { title: false },
            footerrow: true,
            loadComplete: function () {
                //$("#tblMrdClearenceLetter").jqGrid('setGridWidth', $("#MrdClearenceLetterList").width(), true);
                //Total of Columns
                var MORD_SHARE_AMT_T = $(this).jqGrid('getCol', 'MORD_SHARE_AMT', false, 'sum');
                var STATE_SHARE_AMT_T = $(this).jqGrid('getCol', 'STATE_SHARE_AMT', false, 'sum');
                var TOTAL_AMT_T = $(this).jqGrid('getCol', 'TOTAL_AMT', false, 'sum');

                var MRD_TOTAL_ROADS_T = $(this).jqGrid('getCol', 'MRD_TOTAL_ROADS', false, 'sum');
                var MRD_TOTAL_ROAD_LENGTH_T = $(this).jqGrid('getCol', 'MRD_TOTAL_ROAD_LENGTH', false, 'sum');
                var MRD_TOTAL_LSB_T = $(this).jqGrid('getCol', 'MRD_TOTAL_LSB', false, 'sum');
                var MRD_TOTAL_LSB_LENGTH_T = $(this).jqGrid('getCol', 'MRD_TOTAL_LSB_LENGTH', false, 'sum');

                var MRD_HAB_1000_T = $(this).jqGrid('getCol', 'MRD_HAB_1000', false, 'sum');
                var MRD_HAB_500_T = $(this).jqGrid('getCol', 'MRD_HAB_500', false, 'sum');
                var MRD_HAB_250_ELIGIBLE_T = $(this).jqGrid('getCol', 'MRD_HAB_250_ELIGIBLE', false, 'sum');
                var MRD_HAB_100_ELIGIBLE_T = $(this).jqGrid('getCol', 'MRD_HAB_100_ELIGIBLE', false, 'sum');
                //
                $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });

                $(this).jqGrid('footerData', 'set', { MORD_SHARE_AMT: parseFloat(MORD_SHARE_AMT_T).toFixed(4) }, true);
                $(this).jqGrid('footerData', 'set', { STATE_SHARE_AMT: parseFloat(STATE_SHARE_AMT_T).toFixed(4) }, true);
                $(this).jqGrid('footerData', 'set', { TOTAL_AMT: parseFloat(TOTAL_AMT_T).toFixed(4) }, true);

                $(this).jqGrid('footerData', 'set', { MRD_TOTAL_ROADS: parseFloat(MRD_TOTAL_ROADS_T).toFixed(4) }, true);
                $(this).jqGrid('footerData', 'set', { MRD_TOTAL_ROAD_LENGTH: parseFloat(MRD_TOTAL_ROAD_LENGTH_T).toFixed(4) }, true);
                $(this).jqGrid('footerData', 'set', { MRD_TOTAL_LSB: parseFloat(MRD_TOTAL_LSB_T).toFixed(4) }, true);
                $(this).jqGrid('footerData', 'set', { MRD_TOTAL_LSB_LENGTH: parseFloat(MRD_TOTAL_LSB_LENGTH_T).toFixed(4) }, true);

                $(this).jqGrid('footerData', 'set', { MRD_HAB_1000: MRD_HAB_1000_T }, true);
                $(this).jqGrid('footerData', 'set', { MRD_HAB_500: MRD_HAB_500_T }, true);
                $(this).jqGrid('footerData', 'set', { MRD_HAB_250_ELIGIBLE: MRD_HAB_250_ELIGIBLE_T }, true);
                $(this).jqGrid('footerData', 'set', { MRD_HAB_100_ELIGIBLE: MRD_HAB_100_ELIGIBLE_T }, true);

                $('#tblMrdClearanceList_rn').html('Sr.<br/>No.');


            },
            loadError: function (xhr, ststus, error) {

                if (xhr.responseText == "session expired") {
                    alert(xhr.responseText);
                    window.location.href = "/Login/Login";
                }
                else {
                    alert("Invalid data.Please check and Try again!")

                }
            },



        });
    }
}
function CloseDroppedDetails() {
    $('#accordion').hide('slow');
    $("#tblMrdClearanceList").jqGrid('setGridState', 'visible');
    $("#tblMrdClearanceList").trigger('reloadGrid');
    //$("#dvLoadDroppedLetters").hide("slow");
    $("#dvLoadDroppedLetters").html('');

    //$("#dvhdSearch").trigger('click');
    $("#dvMrdDropSearch").show();
}

function FillInCascadeDropdown(map, dropdown, action) {

    var message = '';

    if (dropdown == '#District') {
        message = '<h4><label style="font-weight:normal"> Loading Districts... </label></h4>';
    }
    else {
        message = '<h4><label style="font-weight:normal"> Loading Agencies... </label></h4>';
    }
    $(dropdown).empty();
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
}

function loadSearchAgencyList(statCode) {
    $("#ddlMrdDropAgency").val(0);
    $("#ddlMrdDropAgency").empty();

    if (statCode > 0) {
        if ($("#ddlMrdDropAgency").length > 0) {
            $.ajax({
                url: '/Proposal/PopulateAgenciesByStateAndDepartmentwise',
                type: 'POST',
                data: { "StateCode": statCode, "IsAllSelected": true },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlMrdDropAgency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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

        $("#ddlMrdDropAgency").append("<option value='0'>All Agencies</option>");
    }
}


function loadMrdDroppedLetterDetails(param) {
    //alert(param);
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >MRD Dropped Letter</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseDroppedDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {

        $("#dvhdSearch").trigger('click');
        $('#tblMrdClearanceList').jqGrid('setGridState', 'hidden');

        //alert(param);
        $.ajax({
            url: '/MRDProposal/LoadMrdDroppedLetter/' + param,
            type: 'POST',
            data: $("#frmSearchMrdDropLetter").serialize(),
            success: function (jsonData) {
                $('#dvLoadDroppedLetters').html('');
                $('#dvLoadDroppedLetters').html(jsonData);

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    });
}
