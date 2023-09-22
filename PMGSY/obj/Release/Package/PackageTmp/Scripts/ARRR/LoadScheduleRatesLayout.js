$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmLoadScheduleRates');

    $("#idFilterDivSch").click(function () {
        $("#idFilterDivSch").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
        $("#frmLoadScheduleRates").toggle("slow");
    });

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $('#ddlChapter').change(function () {
        LoadChapterItemsGrid();
    });

    LoadChapterItemsGrid();
});

function LoadChapterItemsGrid() {
    if ($("#frmLoadScheduleRates").valid()) {
        $("#tblLoadScheduleChapterItemsList").jqGrid('GridUnload');

        jQuery("#tblLoadScheduleChapterItemsList").jqGrid({
            url: '/ARRR/GetScheduleChapterItemsList',
            datatype: "json",
            mtype: "POST",
            colNames: ['Item', 'Action'], //26
            colModel: [
                        { name: 'Item', index: 'Item', height: 'auto', width: 1100, align: "left", sortable: true },
                        { name: 'Action', index: 'Action', height: 'auto', width: 70, align: "center", sortable: true },
            ],
            //            postData: { stateCode: $('#ddlMrdDropState option:selected').val(), agency: $('#ddlMrdDropAgency option:selected').val(), year: $('#ddlMrdDropPhaseYear option:selected').val(), batch: $('#ddlMrdDropBatch option:selected').val(), collaboration: $('#ddlMrdDropCollaboration option:selected').val() },
            postData: { chapter: $("#ddlChapter").val() },
            pager: jQuery('#dvLoadScheduleChapterItemspager'),
            rowNum: 10,
            rowList: [10, 15, 20, 30],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'MAST_ARRR_CODE',
            sortorder: "asc",
            caption: "Items List",
            height: 'auto',
            autowidth: true,
            //width:'250',
            shrinkToFit: false,
            rownumbers: true,
            loadComplete: function () {
                //$("#tblMrdClearenceLetter").jqGrid('setGridWidth', $("#MrdClearenceLetterList").width(), true);
                //unblockPage();
                $('#dvLoadScheduleChapterItemspager_left').html("<input type='button' style='margin-left:30px' id='btnFinalize' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'FinalizeScheduleLMM(\"" + $('#ddlChapter option:selected').val() + "\");return false;' value='Finalize'/>");
            },
            grouping: false,
            cmTemplate: { title: false },
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

function loadScheduleItems(urlparameter) {
    //alert(urlparameter);
    $('#idFilterDivSch').trigger('click');
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Schedule Items</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseScheduleDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        $.ajax({
            type: 'POST',
            url: '/ARRR/LoadScheduleItemsLayout/' + urlparameter,
            //dataType: 'json',
            //async: false,
            //cache: false,
            success: function (data) {
                
                $('#dvLoadScheduleMMItems').html('');
                $('#dvLoadScheduleMMItems').html(data);
                $('#dvLoadScheduleMMItems').show('slow');
                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
    });
}


function CloseScheduleDetails() {
    //$("#accordion h3").html('');
    $("#accordion").hide('slow');
    $('#dvLoadScheduleMMItems').hide('slow');
    $('#idFilterDivSch').trigger('click');
    //$('#dvLoadScheduleItems').html('');
    //$("#dvTabs").tabs().removeClass("ui-widget");
}


function FinalizeScheduleLMM(urlparameter) {
    alert(urlparameter);
    if (confirm("Are you sure you want to finalize Schedule details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/ARRR/ScheduleFinalization/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {

                    alert(data.message);
                    //$("#btnCancel").trigger('click');
                    LoadChapterItemsGrid();
                    $.unblockUI();
                }
                else {
                    alert(data.message);
                    $.unblockUI();
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
    }
    else {
        return false;
    }
}
