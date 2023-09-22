this.imagePreview = function () {

    xOffset = 10;
    yOffset = 10;
    var Mx = 1000;// $(document).width();
    var My = 600;// $(document).height();

    var callback = function (event, param) {
        var $img = $("#preview");

        var trc_x = xOffset + $img.width();
        var trc_y = yOffset + $img.height();

        trc_x = Math.min(trc_x + event.pageX, Mx);
        trc_y = Math.min(trc_y + event.pageY, My);
        $img
			.css("top", (trc_y - $img.height()) + "px")
			.css("left", (trc_x - $img.width()) + "px");
    };


    $("a.preview").hover(function (e) {
        Mx = $(this).offset().left + 40; // * 2;//600
        My = $(this).offset().top - 10; //600;

        var arrLink = this.href
        var lnkHref = arrLink;
        // ---------------------------------------------------------
        this.t = this.title;
        this.title = "";
        var c = (this.t != "") ? "<br/>" + this.t : "";
        $("body").append("<p id='preview'><img  style='height: 500px; width: 500px;' height='800' width='600' src='" + lnkHref + "' alt='Image Not Available' />" + c + "</p>");
        callback(e, 200);
        $("#preview").fadeIn("slow");
    },
		function () {
		    this.title = this.t;
		    $("#preview").remove();
		}
	)
};





var numberofphotos = 2;
$(document).ready(function () {
    
    $(function () {
        'use strict';
        // Initialize the jQuery File Upload widget:
        $('#fileupload').fileupload({
            maxNumberOfFiles: numberofphotos,
            acceptFileTypes: /(\.|\/)(jpeg|JPEG|jpg|JPG)$/i,
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

        //Load existing files:
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
            var regex = new RegExp("^[a-zA-Z0-9 ,.()-]+$");
            if ($("#txtRemark").val().trim() == "" ) {
                alert("Please Enter Remark");
                return false;
            }

            if (!regex.test($("#txtRemark").val()))
            {
                alert("Only alphabets and numbers allowed in remark");
                return false;
            }

                var inputs = data.context.find(':input');
            data.formData = inputs.serializeArray();
        });

        $("#fileupload").bind("fileuploaddone", function (e, data) {
            if (data.result.success) {
                $("#divSuccess").show();
                alert("Photograph saved.");
                $('#accordionMonitorsInspection').hide("slow");
                $("#divDisplayPhotographUploadView").hide("slow");
                $('#tbPanchyatList').trigger("reloadGrid");
                $('#tbPciForCNRoad').trigger("reloadGrid");
                
                $("#dvPanchyatList").show("slow");
            }
            else {
                alert(data.result.message);
                //$('#accordionMonitorsInspection').hide("slow");
                //$("#divDisplayPhotographUploadView").hide("slow");
                //$('#tbPanchyatList').trigger("reloadGrid");
                //$("#dvPanchyatList").show("slow");
            }
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
    ListImageFiles($("#PCIID").val());
});


function closeMonitorsInspectionDetails() {
    $('#accordionMonitorsInspection').hide("slow");
    $("#divDisplayPhotographUploadView").hide("slow");
    $("#tbPmgsyRoadList").jqGrid('setGridState', 'visible');
    $("#tbCNRoadList").jqGrid('setGridState', 'visible');
}

function ListImageFiles(PCIId) {
    blockPage();
    jQuery("#tbPDFFilesList").jqGrid({
        url: '/CoreNetwork/ListImageFiles',
        datatype: "json",
        mtype: "POST",
        colNames: ["Photograph", "Remark", "Edit Remark" , "Delete Photograph" , "Save Remark"],
        colModel: [
                    { name: 'PDF', index: 'PDF', width: 125, sortable: false, align: "center", formatter: imageDisplayFormatter, search: false, editable: false },
                    { name: 'Description', index: 'Description', width: 350, sortable: false, align: "center", editable: true, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidatePDFDescription } },
                    /*{ name: 'UploadedBy', index: 'UploadedBy', width: 150, sortable: false, align: "center", editable: false },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", editable: false },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", editable: false },
                    { name: 'Save', index: 'Save', width: 80, sortable: false, align: "center", editable: false, hidden: true },
                    { name: 'IsFinalised', index: 'IsFinalised', width: 60, sortable: false, align: "center", editable: false },
                    { name: 'Finalise', index: 'Finalised', width: 40, sortable: false, align: "center", editable: false },*/
                    { name: 'Edit', index: 'Edit', width: 80, sortable: false, align: "center", editable: false , hidden: true},
                    { name: 'Delete', index: 'Delete', width: 80, sortable: false, align: "center", editable: false },
                    { name: 'Save', index: 'Save', width: 80, sortable: false, align: "center", editable: false, hidden: true },
        ],
        postData: { PCIIDParam: PCIId },
        pager: jQuery('#dvPDFFilesListPager'),
        rowList: [04, 08, 12],
        rowNum: 04,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Uploaded Photographs",
        height: 'auto',
        sortname: 'PDF',
        //autowidth: true,
        rownumbers: true,
        editurl: "/CoreNetwork/UpdateImageRemarkDetails",
        loadComplete: function () {
            $('#tbPDFFilesList').jqGrid('setGridWidth', '600');
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

function ValidatePDFDescription(value, colname) {

    if (value.trim().length == 0) {
        return ["Please Enter Remark."];
    }
    else if ((/^[a-zA-Z0-9 ,.()-]+$/).test(value)) {
        return ["Invalid Remark,Only Alphabets and Numbers are allowed."];
    }
    else {
        return [true, ""];
    }
}

function imageDisplayFormatter(cellvalue, options, rowObject) {
    var PictureURL = cellvalue;
    console.log(cellvalue);
    return " <a href='" + PictureURL + "' onclick='doNothing(); return false;'  class='preview' target='_blank'><img style='height: 75px; width: 100px; border:solid 1px black;' src='" + PictureURL + "' alt='Image not Available'  /> </a>"
}
function EditPDFDetails(paramFileID) {
    console.log(paramFileID);
    $('#tbPDFFilesList').jqGrid('showCol', 'Save');
    jQuery("#tbPDFFilesList").editRow(paramFileID);
}
function CancelSavePDFDetails(paramFileID) {
    $('#tbPDFFilesList').jqGrid('hideCol', 'Save');
    jQuery("#tbPDFFilesList").restoreRow(paramFileID);
}

function SavePDFDetails(paramFileID) {

    jQuery("#tbPDFFilesList").saveRow(paramFileID, checksave);
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


function DeletePDFFileDetails(FileID) {

    if (confirm("Are you sure to delete Photograph ? ")) {

        $.ajax({
            url: "/CoreNetwork/DeleteMultipleInspFileDetails/",
            type: "POST",
            cache: false,
            data: {
                Fileid : FileID
            },
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
                    numberofphotos = numberofphotos - response.photocount;
                    $("#NumberofFiles").val(parseFloat($("#NumberofFiles").val()) - 1);
                    alert("Photograph Deleted Succesfully.");

                    $("#tbPDFFilesList").trigger('reloadGrid');
                    $('#tbPciForCNRoad').trigger("reloadGrid");

                   // closeMonitorsInspectionDetails();
                }
                else {
                    alert(response.ErrorMessage);
                }
            }
        });

    }
    else {
        return;
    }
}
