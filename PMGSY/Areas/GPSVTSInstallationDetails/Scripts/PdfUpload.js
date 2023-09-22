

$(document).ready(function () {
    $(function () {
        'use strict';
        // Initialize the jQuery File Upload widget:
        $('#fileupload').fileupload({
            acceptFileTypes: /(\.|\/)(pdf|PDF)$/i,
            /* maxFileSize: 10000000,*/
            maxFileSize: 4000000,
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
            var inputs = data.context.find(':input');
            if (inputs.filter('[required][value=""]').first().focus().length) {
                Alert("Please Enter Description");
                return false;
            }

            var Remarks = $("#txtRemark").val().trim();
            if (Remarks.length == 0) {
                Alert("Please Enter Description");
                return false;
            }

            if (!Remarks.match("^[a-zA-Z0-9 ,.()-]+$")) {
                Alert("Please Enter Valid Remarks,Can only contains AlphaNumeric values and [,.()-].");
                return false;
            }

            data.formData = inputs.serializeArray();
        });

        $("#fileupload").bind("fileuploaddone", function (e, data) {
            $("#divSuccess").show("slow");
            setTimeout(function () {
                $("#divSuccess").hide("slow"); 
            }, 4000);
            $("#tbPDFFilesList").trigger('reloadGrid');
          
           // $("#tblPresentation tbody tr").remove();
            $("#divGlobalProgress").html("");
        });

        $("#fileupload").bind('fileuploadfail', function (e, data) {
            $("#divGlobalProgress").html("");
        });

        $('#fileupload').bind('fileuploadadd', function (e, data) {
            $("#divSuccess").hide("slow");

        });

        $('#fileupload').bind('fileuploaddestroy', function (e, data) {
            //alert('Destroy');
        });

    });

    ListPDFFiles($("#IMS_PR_ROAD_CODE").val());
    
});

function ListPDFFiles(IMS_PR_ROAD_CODE) {
    blockPage();
    jQuery("#tbPDFFilesList").jqGrid({
        url: '/GPSVTSInstallationDetails/GPSVTSDetails/ListPDFFiles',
        datatype: "json",
        mtype: "POST",
        colNames: ["PDF", "Description", "Edit", "Delete", "Action"],
        colModel: [
            { name: 'PDF', index: 'PDF', width: 125, sortable: false, align: "center", formatter: AnchorFormatter, search: false, editable: false },
            { name: 'Description', index: 'Description', width: 350, sortable: false, align: "center", editable: true, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidatePDFDescription } },
            { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", editable: false },
            { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", editable: false },
            { name: 'Save', index: 'Save', width: 80, sortable: false, align: "center", editable: false, hidden: true }
        ],
        postData: { "IMS_PR_ROAD_CODE": IMS_PR_ROAD_CODE },
        pager: jQuery('#dvPDFFilesListPager'),
        rowList: [04, 08, 12],
        rowNum: 04,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Files",
        height: 'auto',
        sortname: 'PDF',
        //autowidth: true,
        rownumbers: true,
        editurl: "/GPSVTSInstallationDetails/GPSVTSDetails/UpdatePDFDetails",
        //
        //footerrow: true,
        //userDataOnFooter: true,
        loadComplete: function (data) {
            $("#dvPDFFilesListPager").css({ height: '31px' });
            $("#dvPDFFilesListPager_left").html("<input type='button' id='btnFinalize' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick ='Finalize();return false;' value='Finalize'/>")
            if (data['records'] > 0 && $("#isFinalized").val() == "N") {
                $("#btnFinalize").show('slow');
            }
            else {
                $("#btnFinalize").hide('slow');
                /*    $(".ui-icon-trash, .ui-icon-pencil").hide();*/
                //$('.ui-icon-pencil').addClass('ui-icon-locked').removeClass('ui-icon-pencil');
                //$('.ui-icon-trash').addClass('ui-icon-locked').removeClass('ui-icon-trash');

            }
            unblockPage();
        },
        //
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
        Alert('Details updated successfully.');
        return true;
    }
    else if (result.responseText != "") {
        Alert(result.responseText.replace('"', "").replace('"', ""));
        return false;
    }
}

function ValidatePDFDescription(value, colname) {
    if (value.trim().length == 0) {
        return ["Please Enter Description."];
    }
    else if ((/^[a-zA-Z0-9 ,.()-]+$/).test(value)) {
        return ["Invalid Description,Only Alphabets and Numbers are allowed."];
    }
    else {
        return [true, ""];
    }
}

function downloadFileFromAction(paramurl) {
    //window.location = paramurl;

    $.get(paramurl).done(function (response) {
        if (response.Success == 'false') {
            Alert('File Not Found.');
            return false;

        }
        else if (response.success) {
            //Alert("In success");
            var newTab = window.open(response.Message, '_blank');
            newTab.focus();
         
        }
    });



}

function AnchorFormatter(cellvalue, options, rowObject) {
    var url = "/GPSVTSDetails/DownloadFile/" + cellvalue;
    return "<a href='#' onclick=downloadFileFromAction('" + url + "'); return false;> <img style='height:16px;width:16px' height='16' width='16' border=0 src='../../Content/images/PDF.ico' /> </a>";
}

function DeletePDFFileDetails(urlparameter) {
   // alert("urlparameter" + urlparameter);
    //alert("IMS_PR_ROAD_CODE" + IMS_PR_ROAD_CODE);
    //alert("FILE_NAME" + FILE_NAME);
    Confirm("Are you sure to delete the PDF and it's details ? ", function (value) {
        if (value) {
        $.ajax({
            url: "/GPSVTSInstallationDetails/GPSVTSDetails/DeleteFileDetails",
            type: "POST",
            cache: false,
            data: { parameter: urlparameter },
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                unblockPage();
                if (response.Success) {
              
                    $("#NumberofFiles").val(parseFloat($("#NumberofFiles").val()) - 1);
                    Alert("PDF and it's Details Deleted Succesfully.");
                    $("#tbPDFFilesList").trigger('reloadGrid');
                }
                else {
                    Alert(response.ErrorMessage);
                }
            }
        });

    }
    else {
        return;
        }
    });
}

function Finalize() {
    Confirm("Are you sure to finalize the PDF and it's details ? ", function (value) {
        if (value) {
            $.ajax({
                url: "/GPSVTSInstallationDetails/GPSVTSDetails/FinalizeDetails",
                type: "POST",
                cache: false,
                data: { ROAD_CODE: $("#IMS_PR_ROAD_CODE").val() },
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    Alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },
                success: function (response) {
                    unblockPage();
                    if (response.Success) {

                        $("#isFinalized").val("Y");
                        $(".fileupload-buttonbar").hide();
                     /*   $(".ui-icon-trash, .ui-icon-pencil").hide();*/
                        //$('.ui-icon-pencil').addClass('ui-icon-locked').removeClass('ui-icon-pencil');
                        //$('.ui-icon-trash').addClass('ui-icon-locked').removeClass('ui-icon-trash');

                        /* Alert("Details Finilized Succesfully.");*/
                        Alert("Details Finalized Succesfully.");
                        //Reload Grid
                        Load_GPSVTS_RoadList()
                       //End reload Grid
                        $("#tbPDFFilesList").trigger('reloadGrid');
                    }
                    else {
                        Alert(response.ErrorMessage);
                    }
                }
            });

        }
        else {
            return;
        }
    });
}