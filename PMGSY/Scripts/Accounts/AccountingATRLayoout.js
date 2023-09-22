/// <reference path="../jquery-1.9.1-vsdoc.js" />
/// <reference path="../i18n/jquery.jqGrid.src.js" />

$(function () {
    //$("#accordion").accordion({
    //    icons: false,
    //    heightStyle: "content",
    //    autoHeight: false
    //});
});
var ThirdGridId;
$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmATRDetails'));

    $('#btnAdd').click(function () {
        if ($('#observationDiv').is(':visible')) {
            $('#observationDiv').hide();
        }
        $('#tblATRDetails tr:gt(4)').hide();
       // $('#tblATRDetails tr:lt(5)').show();

        //    $('#bgOperation').html('&nbsp;Add Observation');

        $('#accordion').show('slow');
        $('#lstStates').focus();


        $("#tbAccAtrList").jqGrid("setGridState", "hidden"); // make the observation grid hide

        if ($('#pdfparentdiv').is(":visible"))  //pdf 
        {
            $('#observationDiv').show('slow');
            $('#observationDiv').removeClass('ui-accordion-content ui-helper-reset ui-widget-content ui-corner-bottom');
        }

        //Make All option Visible and set to default

        $('option', '#lstStates').show();
        $('option', '#lstAgency').show();
        $('option', '#lstYear').show();
        $('#lstStates').val(0);
        $('#lstAgency').val(0);
        $('#lstYear').val(0);

    });

    $('#lstStates').change(function () {
        FillAgency($('#lstStates option:selected').val());
    });

    $('#btnView').click(function () {

        if ($('#frmATRDetails').valid()) {
            getBalanceSheet();
        }
    });

    $('#imgCloseAgreementDetails').click(function () {
        $('#accordion').hide(1000, 'linear');
        $('#frmATRDetails').get(0).reset();
        $('#imgClosePdf').trigger('click');

        $('#spCollapseIconCN').trigger('click');

        $('#pdfWeapper').hide();
        $('#pdfparentdiv').hide();

        $("#tbAccAtrList").jqGrid("setGridState", "visible"); // make the observation grid visible

        //reset Agency drop down
        $('#lstAgency').empty();
        $('#lstAgency').append("<option value='0'>Select Agency</option>")

    });

    $('#imgClosePdf').click(function () {
        //  $('#dvAddAtrDetails').hide(1000,'linear');
        $('#dvAddAtrDetails').slideToggle();

        if ($('#imgClosePdf').hasClass('ui-icon ui-icon-circle-triangle-n')) {

            $('#imgClosePdf').removeClass('ui-icon ui-icon-circle-triangle-n').addClass('ui-icon ui-icon-circle-triangle-s');
        }
        else {
            $('#imgClosePdf').removeClass('ui-icon ui-icon-circle-triangle-s').addClass('ui-icon ui-icon-circle-triangle-n');
        }
    });


    setTimeout(ShowATRDetails(), 2000);
});

function FillAgency(State) {
    var message = '';
    // message = '<h4><label style="font-weight:normal"> Loading Agency... </label></h4>';

    $('#lstAgency').empty();
    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.blockUI({ message: message });

    $.post('/Accounts/GetAgencies', { State: State }, function (data) {
        $.each(data, function () {
            $('#lstAgency').append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
}

function getBalanceSheet() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: '/Accounts/GetBalanceSheet/',
        method: 'GET',
        cache: false,
        beforeSend: function () { },
        data: $('#frmATRDetails').serialize(),
        success: function (jsonData, status, xhr) {
            $(".w100").remove();
            $htmlobj = $('<iframe>');
            $htmlobj.attr("id", "frameBal");
            $htmlobj.attr("src", "data:application/pdf;base64," + jsonData);
            $htmlobj.addClass("w100");
            $("#dvAddAtrDetails").css("height", "400px");
            $("#dvAddAtrDetails").append($htmlobj);
            // $('#imgClosePdf').trigger('click');

            $('#pdfWeapper').show();
            $('#pdfparentdiv').show();
            if ($('#imgClosePdf').hasClass('ui-icon ui-icon-circle-triangle-n')) {
                $('#imgClosePdf').removeClass('ui-icon ui-icon-circle-triangle-n').addClass('ui-icon ui-icon-circle-triangle-s');
                $('#dvAddAtrDetails').slideToggle();
            }
            ///show from 

            $('#observationDiv').show('slow');
            $('#observationDiv').removeClass('ui-accordion-content ui-helper-reset ui-widget-content ui-corner-bottom');
            $('#Subject').focus();
            //end
            $.unblockUI();
        },
        error: function (xhr, status, err) {
            alert("Error Occured while procesing your request");
            $.unblockUI();
        }
    });
}


function ShowATRDetails() {

    //   $('#dvAccATRList').css("margin-left", "25%"); //Make the grig centerd aligned
    $("#tbAccAtrList").jqGrid('GridUnload');
    $("#tbAccAtrList").jqGrid({
        url: '/Accounts/GetObservationList',
        mtype: "POST",
        datatype: "json",
        colNames: ['Observation Id', 'State', 'Agency', 'Year', 'Upload Pdf', 'View Files'],
        colModel: [
            { label: 'Observation Id', name: 'ObservId', index: "ObservId", key: true, width: 105, hidden: true },
            { label: 'State', name: 'State', index: "State", width: 370, sortable: true, resizable: false, align: 'center' },
            { label: 'Agency', name: 'Agency', index: "Agency", width: 370, sortable: false, resizable: false, align: 'center' },
            { label: 'Year', name: 'Year', index: "Year", width: 370, sortable: false, resizable: false, align: 'center' },
            { label: 'Upload Pdf', name: 'pdf', index: "pdf", width: 150, sortable: false, resizable: false, align: 'center' },
            { label: 'View Files', name: 'Viewfiles', index: "Viewfiles", width: 150, sortable: false, resizable: false, align: 'center' },
        ],
        //  loadonce: true,
        postData: { __RequestVerificationToken: $('#frmATRDetails input[name=__RequestVerificationToken]').val() },
        pager: "#dvAccAtrPager",
        width: 'auto',
        height: 'auto',
        rowNum: 10,
        rowList: [10, 15, 20],
        viewrecords: true,
        recordtext: "{2} records found",
        sortorder: "asc",
        caption: "Observation List",
        sortname: "obsevId",
        rownumbers: true,
        shrinkToFit: true,
        subGrid: true, // set the subGrid property to true to show expand buttons for each row
        subGridRowExpanded: showChildGrid, // javascript function that will take care of showing the child grid
        loadComplete: function (data) {

        }
    });
    //for (var i = 0; i <= datastrr.length; i++)
    //  jQuery("#tbAccAtrList").jqGrid('addRowData', i + 1, datastrr[i]);
};

//the event handler on expanding parent row receives two parameters
//the ID of the grid tow  and the primary key of the row
function showChildGrid(parentRowID, parentRowKey) {

    var childGridID = parentRowID + "_table";
    var childGridPagerID = parentRowID + "_pager";
    // send the parent row primary key to the server so that we know which grid to show

    // add a table and pager HTML elements to the parent grid row - we will render the child grid here
    $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');

    $("#" + childGridID).jqGrid({
        url: '/Accounts/GetDetailObservationList/' + parentRowKey,
        mtype: "POST",
        datatype: "json",
        page: 1,
        colNames: ['Observ Id', 'Subject', 'Observation', 'Observation Date', 'Reply', 'Delete'],
        colModel: [
            { label: 'ObservID', name: 'ObservID', key: true, width: 75, hidden: true },
            { label: 'Subject', name: 'Subject', width: 251, sortable: true, resizable: false, align: 'center' },
            { label: 'Observation', name: 'Observation', width: 352, sortable: false, resizable: false, align: 'left' },
            { label: 'Observation Date', name: 'ObservDate', width: 250, sortable: false, resizable: false, align: 'center' },
           // { label: 'Reply', name: 'Reply', width: 90, sortable: false, resizable: false, align: 'center', formatter: FormatColumnReply, formatoptions: { gridId: childGridID } },
            { label: 'Reply', name: 'Reply', width: 200, sortable: false, resizable: false, align: 'center' },
            { label: 'Delete', name: 'Delete', width: 190, sortable: false, resizable: false, align: 'center' }
        ],
        //loadonce: true,
        postData: { __RequestVerificationToken: $('#frmATRDetails input[name=__RequestVerificationToken]').val() },
        pager: "#" + childGridPagerID,
        width: 'auto',
        rowNum: 10,
        rowList: [10, 15, 20],
        hidegrid: true,
        height: 'auto',
        viewrecords: true,
        recordtext: "{2} records found",
        sortorder: "asc",
        caption: "Observation Details",
        sortname: "subject",
        rownumbers: true,
        shrinkToFit: true,
        subGrid: true, // set the subGrid property to true to show expand buttons for each row
        subGridRowExpanded: showThirdLevelChildGrid, // javascript function that will take care of showing the child grid
        loadComplete: function (data) {
            if (data.IsSRRADA == "N") {
                $(this).jqGrid("hideCol", ["Reply"]);  // NRRDA cannot give reply to own Observation(First Reply)
            } else {
                $(this).jqGrid("hideCol", ["Delete"])  //SRRDA cannot delete the NRRDA obdervation
            }

        }
    });
    //for (var i = 0; i <= subgridData.length; i++)
    //    $("#" + childGridID).jqGrid('addRowData', i + 1, subgridData[i]);

}

//// the event handler on expanding parent row receives two parameters
//// the ID of the grid tow  and the primary key of the row
function showThirdLevelChildGrid(parentRowID, parentRowKey) {
    var childGridID = parentRowID + "_table";
    var childGridPagerID = parentRowID + "_pager";

    //alert('parent key ='+parentRowKey);
    // send the parent row primary key to the server so that we know which grid to show

    // add a table and pager HTML elements to the parent grid row - we will render the child grid here
    $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');

    $("#" + childGridID).jqGrid({
        url: '/Accounts/GetObservationReplyList/' + parentRowKey,
        mtype: "POST",
        datatype: "json",
        colNames: ['Observ Id', 'Subject', 'Observation', 'Observation By', 'Observation Date', 'Reply', 'Delete', 'Other Details'],
        colModel: [
            { label: 'ObservID', name: 'ObservID', key: true, width: 75, hidden: true },
            { label: 'Subject', name: 'Subject', width: 150, sortable: true, resizable: false, align: 'center' },
            { label: 'Observation', name: 'Observation', width: 230, sortable: false, resizable: false, align: 'left' },
            { label: 'Observation By', name: 'ObservationBy', width: 230, sortable: false, resizable: false, align: 'center' },
            { label: 'Observation Date', name: 'ObservDate', width: 150, sortable: false, resizable: false, align: 'center' },
         // { label: 'Reply', name: 'Reply', width: 90, sortable: false, resizable: false, align: 'center', formatter: FormatColReply, formatoptions: { gridId: childGridID } },
            { label: 'Reply', name: 'Reply', width: 120, sortable: false, resizable: false, align: 'center' },
            { label: 'Delete', name: 'Delete', width: 120, sortable: false, resizable: false, align: 'center' },
            { label: 'OtherDetails', name: 'OtherDetails', width: 100, sortable: false, resizable: false, hidden: true }
        ],
        pager: "#" + childGridPagerID,
        //loadonce: true,
        postData: { __RequestVerificationToken: $('#frmATRDetails input[name=__RequestVerificationToken]').val() },
        width: 'auto',
        height: '100%',
        rowNum: 10,
        rowList: [10, 15, 20],
        hidegrid: true,
        height: 'auto',
        viewrecords: true,
        recordtext: "{2} records found",
        sortorder: "asc",
        caption: "Observation Reply Details",
        sortname: "subject",
        rownumbers: true,
        shrinkToFit: true,
        loadComplete: function (data) {
            if (data.IsSRRADA == "N") {
                // $(this).jqGrid("hideCol", ["Reply"]);
            }
            ThirdGridId = childGridID;
            //  alert(ThirdGridId + " and  " + childGridID)
        },
    });

    //for (var i = 0; i <= ThriGridData.length; i++)
    //    $("#" + childGridID).jqGrid('addRowData', i + 1, ThriGridData[i]);
}


//function FormatColumnReply(cellvalue, options, rowObject) {
//    if (cellvalue != '') {
//       // return "<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-arrowreturnthick-1-w' title='Reply' onClick ='ReplyObservation(\"" + cellvalue.toString() + "\",\"" + options.colModel.formatoptions.gridId + "\");'></span></td> </tr></table></center>";
//        return "<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-arrowreturnthick-1-w' title='Reply' onClick ='ReplyObservation(\"" + cellvalue.toString() + "\",\"" + rowObject[0] + "\");'></span></td> </tr></table></center>";
//    }
//    else {
//        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
//    }
//}

//function FormatColReply(cellvalue, options, rowObject) {
//    if (cellvalue != '') {
//        // return "<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-arrowreturnthick-1-w' title='Reply' onClick ='ReplyObservation(\"" + cellvalue.toString() + "\",\"" + options.colModel.formatoptions.gridId + "\");'></span></td> </tr></table></center>";
//        return "<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-arrowreturnthick-1-w' title='Reply' onClick ='ReplyObservation(\"" + cellvalue.toString() + "\",\"" + rowObject[0] + "\");'></span></td> </tr></table></center>";
//    }
//    else {
//        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
//    }
//}

function ReplyObservation(urlparameter, ObservationId, event) {

   

    $('#bgOperation').html('&nbsp;Add Observation');

    $('#accordion').show('slow');//new
    $('#observationDiv').show('slow');
    $('#observationDiv').removeClass('ui-accordion-content ui-helper-reset ui-widget-content ui-corner-bottom ui-accordion-content-active');

    //For SRRDA Login
    if (!$('#btnAdd').is(":visible")) {

        $('#tblATRDetails tr:gt(3)').hide();
        $('#tblATRDetails tr:lt(4)').show();

        $('#accordion').css('border', 'none');
        $('#dvSearchParameter').hide();
    }
    else {
 
        $('#tblATRDetails tr:gt(4)').hide();
        $('#tblATRDetails tr:lt(5)').show();

        $("#replyAnchor").closest("td").trigger('click');
        var SelRowId = $("#" + ThirdGridId).jqGrid('getGridParam', 'selrow');
        var OtherInfo = $("#" + ThirdGridId).getCell(SelRowId, "OtherDetails");
        var filterValues = atob(OtherInfo).split(':');
        $('#lstStates').val(filterValues[0])//.prop("disabled", true);
        $('#lstStates').trigger('change');
        setTimeout(function () { $('#lstAgency').val(filterValues[1]);/*.prop("disabled", true)*/ }, 200);
        $('#lstYear').val(filterValues[2]);//.prop("disabled", true);

        $('option', '#lstStates').not('#lstStates', 'option:gt(0)').siblings().css('display', 'none');
        setTimeout(function () { $('option', '#lstAgency').not('#lstAgency', 'option:gt(0)').siblings().css('display', 'none') }, 200);
        $('option', '#lstYear').not('#lstYear', 'option:gt(0)').siblings().css('display', 'none');

    }
    $('#hdnMasterObId').val(urlparameter);


}

function UploadFile(urlparameter) {

    
    $('#bgOperation').html('&nbsp;Upload File');
    $('#masterObId').val(urlparameter);

    $('#accordion').show('slow');//new
    $('#observationDiv').show('slow');
    $('#observationDiv').removeClass('ui-accordion-content ui-helper-reset ui-widget-content ui-corner-bottom ui-accordion-content-active');
    //[For SRRDA Login]
    if (!$('#btnAdd').is(":visible")) {
        $('#tblATRDetails tr:lt(4)').hide();
        $('#tblATRDetails tr:gt(3)').show();

        $('#accordion').css('border', 'none');
        $('#dvSearchParameter').hide();
    }
    else {
        // [For NRRDA Login]
        $('#tblATRDetails tr:lt(5)').hide();
        $('#tblATRDetails tr:gt(4)').show();
    }
    // Set the filter Values
    var filterValues = atob(urlparameter).split(':');
    $('#lstStates').val(filterValues[0])//.prop("disabled", true);
    $('#lstStates').trigger('change');
    setTimeout(function () { $('#lstAgency').val(filterValues[1]);/*.prop("disabled", true)*/ }, 100);
    $('#lstYear').val(filterValues[2]);//.prop("disabled", true);

    $('option', '#lstStates').not('#lstStates', 'option:gt(0)').siblings().css('display', 'none');
    setTimeout(function () { $('option', '#lstAgency').not('#lstAgency', 'option:gt(0)').siblings().css('display', 'none') }, 200);
    $('option', '#lstYear').not('#lstYear', 'option:gt(0)').siblings().css('display', 'none');

}


function ViewUploadedFiles(urlparameter) {
    $('#masterObIdforFileList').val(urlparameter);
    ViewFileList();// File List Grid [grid is in ObservationDetails.js]
}

function DeleteObservation(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    if (!confirm('Are you sure to delete observation?')) {
        $.unblockUI();
        return false;
    }
    $.ajax({
        url: '/Accounts/DeleteObservation/',
        method: 'POST',
        cache: false,
        beforeSend: function () { },
        dataType: 'json',
        data: AddAntiForgeryToken({ urlparameter: urlparameter }),
        success: function (jsonData, status, xhr) {
            alert(jsonData.message);
            if (jsonData.success)
                jQuery("#tbAccAtrList").trigger('reloadGrid');
            $.unblockUI();
        },
        error: function (xhr, status, err) {
            alert("Error Occured while procesing your request");
            $.unblockUI();
        }
    });

}


/*
function DeleteChildObservation(urlparameter)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    if (!confirm('Are you sure to delete observation?')) {
        $.unblockUI();
        return false;
    }

    $.ajax({
        url: '/Accounts/DeleteObservation/', //DeleteChildObservation
        method: 'POST',
        cache: false,
        beforeSend: function () { },
        dataType: 'json',
        data: AddAntiForgeryToken({ urlparameter: urlparameter }),
        success: function (jsonData, status, xhr) {
            alert(jsonData.message);
            if (jsonData.success)
                jQuery("#tbAccAtrList").trigger('reloadGrid');
            $.unblockUI();
        },
        error: function (xhr, status, err) {
            alert("Error Occured while procesing your request");
            $.unblockUI();
        }
    });
}
*/
AddAntiForgeryToken = function (data) {
    debugger;
    data.__RequestVerificationToken = $('#frmATRDetails input[name=__RequestVerificationToken]').val();
    return data;
};