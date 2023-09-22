/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   ATRUpload.js
        * Description   :   Upload ATR Details
        * Author        :   Shyam Yadav 
        * Creation Date :   10/Jun/2013
 **/


$(document).ready(function () {
    $(function () {
        'use strict';
        // Initialize the jQuery File Upload widget:
        $('#fileupload').fileupload({
            maxNumberOfFiles: 1 - parseInt($("#NumberofPdfs").val()),
            acceptFileTypes: /(\.|\/)(pdf|PDF)$/i,
            maxFileSize: 10000000,
        });

        //Enable iframe cross-domain access via redirect option:
        $('#fileupload').fileupload(
            'option',
            'redirect',
            window.location.href.replace(
                /\/[^\/]*$/,
                '/cors/result.html?%s'
            )
        );

        // Load existing files:
        $.ajax({
            url: $('#fileupload').fileupload('option', 'url'),
            dataType: 'json',
            context: $('#fileupload')[0]
        }).done(function (result) {
            $(this).fileupload('option', 'done')
                .call(this, null, { result: result });
        });

        // For validation
        $('#fileupload').bind('fileuploadsubmit', function (e, data) {
            //var inputs = data.context.find(':input');
            //if (inputs.filter('[required][value=""]').first().focus().length) {
            //    alert("Please Enter Description");
            //    return false;
            //}

            //var Remarks = $("#txtRemark").val();

            //if (!Remarks.match("^[a-zA-Z0-9  ,.()-]+$")) {
            //    alert("Please Enter Valid Remarks,Can only contains AlphaNumeric values and [,.()-].");
            //    return false;
            //}

            //data.formData = inputs.serializeArray();
        });

        $("#fileupload").bind("fileuploaddone", function (e, data) {
            $("#divSuccess").show("slow");
            $("#tbPDFFilesList").trigger('reloadGrid');
            $("#tb3TierATRList").trigger('reloadGrid');

            $("#tblPresentation tbody tr").remove();
            $("#divGlobalProgress").html("");

            if ($('#roleId').val() == 22 || $('#roleId').val() == 38) {
                viewMaintenanceATRDetails();
            }
            else {
                viewATRDetails();
            }

        });

        $("#fileupload").bind('fileuploadfail', function (e, data) {
            //$("#tblPresentation").find('div.progress').find('div:eq(0)').css('width', '0px');
            //$("#tblPresentation").find('tr.template-upload').css('display', 'none');
            // $("#divGlobalProgress").html("");
        });

        $('#fileupload').bind('fileuploadadd', function (e, data) {
            $("#divSuccess").hide("slow");

        });

        $('#fileupload').bind('fileuploaddestroy', function (e, data) {
            //alert('Destroy');
        });

    });

    //Below Code Added on 30-01-2023
    $('#ATRFile1').change(function () {
        var i = $(this).prev('label').clone();
        var file = $('#ATRFile1')[0].files[0].name;
        $(this).prev('label').text(file);
    });

    ListPDFFiles($("#QM_OBSERVATION_ID").val());
    ViewVerificationATR_ListByRoadCode_Inspdate_ObsId_ATRId($(IMS_PR_ROAD_CODE).val() + "$" + $(QM_INSPECTION_DATE).val() + "$" + $(QM_OBSERVATION_ID).val() + "$" + $(QM_ATR_ID).val());
});

function ListPDFFiles(QM_OBSERVATION_ID) {
    //alert("inside ListPDFFiles");
    blockPage();
    jQuery("#tbPDFFilesList").jqGrid({
        url: '/QualityMonitoring/ListPDFFiles',
        datatype: "json",
        mtype: "POST",
        colNames: ["ATR File"],
        colModel: [
            { name: 'PDF', index: 'PDF', width: 400, sortable: false, align: "center", formatter: AnchorFormatter, search: false, editable: false }
        ],
        postData: { "QM_OBSERVATION_ID": QM_OBSERVATION_ID, "QM_ATR_ID": $('#QM_ATR_ID').val() },
        pager: jQuery('#dvPDFFilesListPager'),
        rowList: [04, 08, 12],
        rowNum: 04,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Files",
        height: 'auto',
        //autowidth: true,
        sortname: 'PDF',
        rownumbers: true,
        editurl: "/Proposal/UpdatePDFDetails",
        loadComplete: function () {
            //$("#gview_tbPDFFilesList > .ui-jqgrid-titlebar").hide();
            unblockPage();
        },
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                unblockPage();
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                unblockPage();
                alert("Session Timeout !!!");
                window.location.href = "/Login/LogIn";
            }
            unblockPage();
        }
    }); //end of grid    
}

function EditPDFDetails(paramFileID) {
    $('#tbPDFFilesList').jqGrid('showCol', 'Save');
    jQuery("#tbPDFFilesList").editRow(paramFileID);
}

function SavePDFDetails(paramFileID) {

    jQuery("#tbPDFFilesList").saveRow(paramFileID, checksave);
}

function CancelSavePDFDetails(paramFileID) {
    $('#tbPDFFilesList').jqGrid('hideCol', 'Save');
    jQuery("#tbPDFFilesList").restoreRow(paramFileID);
}

function checksave(result) {
    $('#tbPDFFilesList').jqGrid('hideCol', 'Save');
    if (result.responseText == "true") {
        alert('Details updated successfully.');
        return true;
    }
    else if (result.responseText != "") {
        alert(result.responseText.replace('"', "").replace('"', ""));
        return false;
    }
}

function ValidatePDFDescription(value, colname) {

    //alert((/^[a-zA-Z0-9  ,.()-]+$/).test(value));
    //alert((/^[a-zA-Z0-9  ,.()-]+$/).test(value)  + " "+ value);
    if ((/^[a-zA-Z0-9 ,.()-]+$/).test(value)) {
        return [" Invalid Description,Only Alphabets and Numbers are allowed."];
    }
    else {
        return [true, ""];
    }
}

function downloadFileFromAction(paramurl) {
    window.location = paramurl;
}

function AnchorFormatter(cellvalue, options, rowObject) {
    var url = "/QualityMonitoring/DownloadFile/" + cellvalue;
    return "<a href='#' onclick=downloadFileFromAction('" + url + "'); return false;> <img style='height:16px;width:16px' height='16' width='16' border=0 src='../../Content/images/PDF.ico' /> </a>";
}


/*Added on 30-01-2023*/

function ViewVerificationATR_ListByRoadCode_Inspdate_ObsId_ATRId(roadCode_inspdate_ObsId_ATRId) {
    //alert("inside: + ViewVerificationATR_ListByRoadCode_Inspdate_ObsId_ATRId  :" + roadCode_inspdate_ObsId_ATRId);
    //passing NQM Observation ID and ATR Id for finalize
    const NQM_ObsId_ATrCode = roadCode_inspdate_ObsId_ATRId;//.split("$");

    $('#tblVerificationATR_Grid').jqGrid('GridUnload');

    jQuery("#tblVerificationATR_Grid").jqGrid({
        url: '/QualityMonitoring/ViewVerificationATR_ListByRoadCode_Inspdate_ObsId_ATRId',
        datatype: "json",
        mtype: "GET",
        multiselect: true,
        colNames: ["ObservationId", "RoadCode", "Monitor", "State", "District", "Block", "Package", "Sanction Year", "Road Name", "Type",
            "Start Chainage (Km.)", "End Chainage (Km.)", "Inspection Date", "Total Length (Road(km)/ LSB(mtr)) ", "Road Status (Current Status)", /*"Enquiry Inspection",*/ "Ground Verification Inspection",
            "Scheme", "Overall Grade"],
        colModel: [
            { key: true, name: 'ObservationId', index: 'ObservationId', width: 100, hidden: true, sortable: false, align: "left" },
            { name: 'RoadCode', index: 'RoadCode', width: 100, hidden: true, sortable: false, align: "left" },
            { name: 'Monitor', index: 'Monitor', width: 100, sortable: false, align: "left" },
            { name: 'State', index: 'State', width: 75, sortable: false, align: "left", search: false },
            { name: 'District', index: 'District', width: 75, sortable: false, align: "left", search: false },
            { name: 'Block', index: 'Block', width: 75, sortable: false, align: "left", search: false },
            { name: 'Package', index: 'Package', width: 75, sortable: false, align: "left", search: false },
            { name: 'SanctionYear', index: 'SanctionYear', width: 65, sortable: false, align: "left", search: false },
            { name: 'RoadName', index: 'RoadName', width: 100, sortable: false, align: "left", search: false },
            { name: 'PropType', index: 'PropType', width: 50, sortable: false, align: "left", search: false },
            { name: 'InspFrmChainage', index: 'InspFrmChainage', width: 75, sortable: false, align: "left", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: " ", decimalPlaces: 3, defaultValue: '0.00' } },
            { name: 'InspToChainage', index: 'InspToChainage', width: 75, sortable: false, align: "left", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: " ", decimalPlaces: 3, defaultValue: '0.00' } },
            { name: 'InspDate', index: 'InspDate', width: 100, sortable: false, align: "left", search: false, formatter: 'date', formatoptions: { srcformat: 'ISO8601Long', newformat: 'd.m.Y' } },
            { name: 'TotalLength', index: 'TotalLength', width: 75, sortable: false, align: "left", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: " ", decimalPlaces: 3, defaultValue: '0.00' } },
            { name: 'RdStatus', index: 'RdStatus', width: 100, sortable: false, align: "left", search: false },
            //{ name: 'EnquiryInspection', index: 'EnquiryInspection', width: 100, sortable: false, align: "center", search: false },            
            { name: 'Ground_Verification_Inspection', index: 'Ground_Verification_Inspection', width: 100, sortable: false, align: "left", search: false },
            { name: 'Scheme', index: 'Scheme', width: 80, sortable: false, align: "left", search: false },
            { name: 'OverallGrade', index: 'OverallGrade', width: 100, sortable: false, align: "left", search: false },
        ],
        postData: { "VerificationATRCode": roadCode_inspdate_ObsId_ATRId },
        pager: '#divPagerVerificationATR_Grid',
        rowNum: 10,
        rowList: [10, 15, 20],
        rownumbers: true,
        pgbuttons: true,
        //pgtext: null,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Verification ATR List",
        //width: 'auto',
        //height: '250',
        autowidth: true,
        shrinkToFit: true,
        jsonReader: {
            root: "rows",
            page: "page",
            total: "total",
            records: "records",
            repeatitems: false,
            Id: "ObservationId",
        },
        loadComplete: function (data) {
            //$('.dataTable').css({ display: 'none' });

            $('#dvVerificationATR_Grid').show('slow');
            $('#tblVerificationATR_Grid').jqGrid("setGridState", "visible");

            // disabled multiselect as per requirement
            $('#cb_tblVerificationATR_Grid').attr("disabled", true);

            // Present Codes for ATR Already finalzed in Table Returned from jsonData
            var userdata = jQuery("#tblVerificationATR_Grid").getGridParam('userData');

            for (var i = 0; i < userdata.ids.length; i++) {
                if ($('#' + userdata.ids[i] + ' input[type=checkbox]').attr('checked', true)) {
                    jQuery("#jqg_tblVerificationATR_Grid_" + userdata.ids[i]).attr("disabled", true);
                }
            }
            $("#tblVerificationATR_Grid #divPagerVerificationATR_Grid").css({ height: '31px' });
            if (data["records"] > 0 && userdata.ids.length == 0 && userdata.ids.length < 1) {

                /* userdata.ids.length != data["records"]*/

                $('#divPagerVerificationATR_Grid_left').html("<input type='button' style='margin-left:50px; border: 2px outset green;' id='btnFinalizedVerifyATR' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick ='finalizedVerificationATR(\"" + NQM_ObsId_ATrCode + "\"); return false;' value='Finalize'/>");
            }
            unblockPage();
        },
        onSelectAll: function (aRowids, status) {

            // Present Codes in Payment Table Returned from jsonData
            var userdata = jQuery("#tblVerificationATR_Grid").getGridParam('userData');

            for (var i = 0; i < userdata.ids.length; i++) {
                if ($('#' + userdata.ids[i] + ' input[type=checkbox]').attr('checked', true)) {
                    jQuery("#jqg_tblVerificationATR_Grid_" + userdata.ids[i]).attr("disabled", true);
                    //$("#ATRFile1").attr("disabled", true);
                }
            }
        },
        // Disabled to already checked check box
        beforeSelectRow: function (rowId, e) {

            if ($("#jqg_tblVerificationATR_Grid_" + rowId).attr("disabled")) {
                return false;
            }
            else
                return true;
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
        }
    }); //end of grid
} // end of func

//----------------- ADDED BY ROHIT BORSE for ATR MARK VERIFICATION
function finalizedVerificationATR(NQM_ObsId_ATrCode) {

    // Present Codes in Payment Table Returned from jsonData
    var userdata = jQuery("#tblVerificationATR_Grid").getGridParam('userData');

    if (userdata.ids.length == 1) {

        alert("Only one SQM inspection ATR can be finilize !!");
    }
    else {

        var submitArray = [];

        var selRowIds = jQuery('#tblVerificationATR_Grid').jqGrid('getGridParam', 'selarrrow');

        // alert("submitted array include Observation Id: " + selRowIds + "" + "selRowIds.length " + selRowIds.length);

        if (selRowIds.length > 0) {

            if (selRowIds.length == 1) {
                for (var i = 0; i < selRowIds.length; i++) {

                    rowdata = jQuery("#tblVerificationATR_Grid").getRowData(selRowIds[i]);
                    if (!$("#jqg_tblVerificationATR_Grid_" + selRowIds[i]).attr("disabled")) {
                        submitArray.push(rowdata["ObservationId"]);
                    }
                }
                // calling save finalized ATRS
                savefinalizedMarkVerificationATR(NQM_ObsId_ATrCode, submitArray);

            } else {
                alert("please select only one record for ATR verification finalize");
            }
        }
        else
            alert("No records to submit, please select any one record to finalize");
    }

}

//----------------- ADDED BY ROHIT BORSE for ATR MARK VERIFICATION
function savefinalizedMarkVerificationATR(NQM_ObsId_ATrCode, submitArray) {

    //alert("submitArray: " + submitArray);
    //alert("NQM_ObsId_ATrCode: " + NQM_ObsId_ATrCode);

    var fileUpload = $("#ATRFile1").get(0);
    var files = fileUpload.files;

    // Create FormData object  
    var fileData = new FormData();

    // Looping over all files and add it to FormData object  
    for (var i = 0; i < files.length; i++) {
        fileData.append(files[i].name, files[i]);
    }

    // Adding one more key to FormData object  
    fileData.append('nqmobsidatrid', NQM_ObsId_ATrCode);
    fileData.append('submitarray', submitArray);
    fileData.append('missFileflag', "N");//To identify new file or missing file is uploading


    if (confirm("Are you sure to finalize Mark For ATR Verification ?")) {
        $.ajax({
            url: "/QualityMonitoring/savefinalizedMarkVerificationATRFile",
            type: "POST",
            //contentType: "application/json; charset=utf-8",
            contentType: false,
            processData: false,
            dataType: "json",
            //data: JSON.stringify({ 'nqmobsidatrid': NQM_ObsId_ATrCode, 'submitarray': submitArray }), //{'submitSQMId': submitSQMId, 'NQM_ObsId_ATrCode': NQM_ObsId_ATrCode},
            data: fileData, //{'submitSQMId': submitSQMId, 'NQM_ObsId_ATrCode': NQM_ObsId_ATrCode},
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            success: function (data) {
                if (data.success == true) {
                    alert(data.message);

                    $('#tblVerificationATR_Grid').trigger('reloadGrid');

                    $('#btnFinalizedVerifyATR').css({ display: 'none' });

                    $('.ATRFile1').css({ display: 'none' });

                    $.blockUI();
                    view_ATR_Details();//use to reload grid


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
}

//Below Code Added on 30-01-2023
$('#ATRFile1').change(function () {
    var i = $(this).prev('label').clone();
    var file = $('#ATRFile1')[0].files[0].name;
    $(this).prev('label').text(file);
}); 
