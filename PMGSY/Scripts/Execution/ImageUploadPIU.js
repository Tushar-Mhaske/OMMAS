
$(function () {
    'use strict';
    //alert($("#NumberofFiles").val());
    // Initialize the jQuery File Upload widget:
    $('#fileupload').fileupload({
        maxNumberOfFiles: 20 - parseInt($("#NumberofFiles").val()),
        acceptFileTypes: /(\.|\/)(gif|pdf)$/i,
        // acceptFileTypes: /(\.|\/)(gif|jpeg|png|pdf|docx|docs|xls|jpg)$/i,
       // acceptFileTypes: /(\.|\/)(jpg|jpe?g)$/i,
        maxFileSize: 4000000
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

    $("#fileupload").bind('fileuploadfail', function (e, data) {
     
        $("#divGlobalProgress").html("");
    });



    $("#fileupload").bind("fileuploaddone", function (e, data)
    {
        $("#divImgUploadError").html("");
        $("#divImgUploadError").hide("slow");

        $("#divSuccess").show("slow");

        
        $("#tbFilesList").trigger('reloadGrid');

      

        //var RoadCode = $("#IMS_PR_ROAD_CODE").val();
        //alert("RoadCode = " + RoadCode);
        //ListQualityFiles(RoadCode);


        $("#dvAddMaintenanceAgreementAgainstRoad1").hide("slow");

        if ($("#accordion1").is(":visible"))
        {
            $('#accordion1').hide('slow');
        }

        var ATRId = $("#ATRId").val();
       // alert("ATR Id = " + ATRId)
        GetUploadPhotoByPIU(ATRId);

        //$("#tbFilesList").trigger('reloadGrid');
        $("#tb3TierInspectionList").trigger('reloadGrid')
        $("#tbMonitorsInspectionList").trigger('reloadGrid');
        $("#tbMonitorsObsList").trigger("reloadGrid");
        //$("#tblPresentation tbody tr").remove();    

        //$("#divGlobalProgress").html("");
    });

    $('#fileupload').bind('fileuploadsubmit', function (e, data)
    {

        var flagRemarks = true;

        $("#tblPresentation textarea[name='remark[]']").each(function () {
            var regExRemarks = /^[a-zA-Z0-9 a-zA-Z0-9 ,.()-]+$/;
            var Remarks = $(this).val().trim();
            var Remarks = "NA"; // Added this Line so as to not to enter remarks
            if (Remarks == "") {
                flagRemarks = false;
                $("#divImgUploadError").html("Please Enter Valid Image Description,Can only contains AlphaNumeric values and [,.()-].   value");
                $("#divImgUploadError").show('slow');

                $(this).focus();
                return false;
            }

            if (!regExRemarks.test(Remarks))
            {
                //alert("Please Enter Valid Image Description,Can only contains AlphaNumeric values and [,.()-].   value " + Remarks);
                alert(Remarks);
                $("#divImgUploadError").html("Please Enter Valid Image Description,Can only contains AlphaNumeric values and [,.()-].   value");
                $("#divImgUploadError").show('slow');

                flagRemarks = false;
                return false;
            }

        });

        //alert(flagRemarks);
        //return false;

        var inputs = data.context.find(':input');
        var text = data.context.find('textarea');
        data.formData = inputs.serializeArray();
        //$("#fileupload").serialize();

        if (flagRemarks) {
            return true;
        }
        return false;


    });

    $('#fileupload').bind('fileuploadadd', function (e, data)
    {
        $("#divSuccess").hide("slow");
       
    });
 
});



// View / Upload Image Uploaded By PIU
function GetUploadPhotoByPIU(ATRId) {

   
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/Execution/GetImageUploadByPIU/" + ATRId,
        type: "GET",
        async: false,
        cache: false,

        success: function (data) {
            $("#dvAddMaintenanceAgreementAgainstRoad1").html(data);
            $('#accordion1').show('slow');
            $('#dvAddMaintenanceAgreementAgainstRoad1').show('slow');
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("Error Occured.");
            //     alert(xhr.responseText);
            $.unblockUI();
        }

    });

}



this.imagePreview = function () {

    /* CONFIG */
    xOffset = 10;
    yOffset = 10;
    // these 2 variable determine popup's distance from the cursor
    // you might want to adjust to get the right result
    var Mx = 1000;// $(document).width();
    var My = 600;// $(document).height();

    /* END CONFIG */
    var callback = function (event, param) {
        var $img = $("#preview");

        // top-right corner coords' offset
        var trc_x = xOffset + $img.width();
        var trc_y = yOffset + $img.height();

        trc_x = Math.min(trc_x + event.pageX, Mx);
        trc_y = Math.min(trc_y + event.pageY, My);

        //alert("left: " + (trc_y - $img.height()) + "   Top " + (trc_x - $img.width()));

        $img
			.css("top", (trc_y - $img.height()) + "px")
			.css("left", (trc_x - $img.width()) + "px");
    };


    $("a.preview").hover(function (e) {

        //------------  Through Url split Latlongs & Href Link Part & Display Map as per appropriate LatLongs--------
        var arrLink = this.href.split("$$$");
        var lnkHref = arrLink[0];

        //var latLong = arrLink[1];
        //var latLongArr = latLong.split("$$"); //0th is Latitude and 1st is Longitude
        //if (latLongArr[0] != 0 && latLongArr[1] != 0) {
        //    displayMap(latLongArr[0], latLongArr[1]);
        //}
        // ---------------------------------------------------------

        Mx = $(this).offset().left + 400; // * 2;//600
        My = $(this).offset().top - 50; //600;

        this.t = this.title;
        this.title = "";
        var c = (this.t != "") ? "<br/>" + this.t : "";
       // $("body").append("<p id='preview'><img  style='height: 500px; width: 500px;' height='800' width='600' src='" + lnkHref + "' alt='Image Not Available' />" + c + "</p>");
        callback(e, 200);
        $("#preview").fadeIn("slow");
    },
		function () {
		    this.title = this.t;
		    $("#preview").remove();
		}
	)
    //.mousemove(callback);
};

$(document).ready(function () {

    ListQualityFiles($("#IMS_PR_ROAD_CODE").val());


});



$('#btnCancelImageUpload').click(function () {

    $("#dvAddMaintenanceAgreementAgainstRoad1").hide("slow");

    if ($("#accordion1").is(":visible")) {
        $('#accordion1').hide('slow');
    }

    $('#tbCDWorksList').jqGrid("setGridState", "visible");

    $("#dvAgreement").animate({
        scrollTop: 0
    });
});

function doNothing() {
    return false;
}

function ListQualityFiles(IMS_PR_ROAD_CODE) {
   

    var FinalizedByPIU = $("#IsFinalized").val();

    jQuery("#tbFilesList").jqGrid('GridUnload');

    jQuery("#tbFilesList").jqGrid({
        url: '/Execution/ListFilesByPIU?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["File", 'Accepted',"RSA Action Taken", 'Description of File',"Download", "Delete"],
        colModel: [
                    { name: 'image', index: 'image', width: 125, sortable: false, align: "center", formatter: imageFormatter, search: false, editable: false }, //, , hidden: isHiddenImage()
                    //{ name: 'Description', index: 'Description', width: 200, sortable: false, align: "left", editable: true, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidateImageDescription } },
                   { name: 'EXEC_RSA_REC_ACCP', index: 'EXEC_RSA_REC_ACCP', width: 150, sortable: false, align: 'center', editable: false },
                   { name: 'EXEC_RSA_ACTION_TAKEN', index: 'EXEC_RSA_ACTION_TAKEN', width: 200, sortable: false, align: 'center', editable: false },
                   { name: 'EXEC_RSA_ATR_FILE_DESC', index: 'EXEC_RSA_ATR_FILE_DESC', width: 150, sortable: false, align: 'center', editable: false },

                   { name: 'download', index: 'download', width: 80, sortable: false, align: 'center', editable: false, hidden: true, title: false },
                   { name: 'Delete', index: 'Delete', width: 80, sortable: false, align: "center", editable: false, hidden: isHiddenByPIU(FinalizedByPIU), title: false }

        ],
        postData: { "IMS_PR_ROAD_CODE": IMS_PR_ROAD_CODE, "value": Math.random() },
        pager: jQuery('#dvFilesListPager'),
        rowNum: 30,
        viewrecords: true,
        pgbuttons: false,
        pgtext: null,
        recordtext: '{2} records found',
        caption: "File Details uploaded By PIU",
        height: '100',
        sortname: 'Image',
        rownumbers: true,
        loadComplete: function () {

            if ($("#hdnRoleImgUpload").val() == 6 || $("#hdnRoleImgUpload").val() == 7) //for SQC hide these two columns
            {
                $('#tb3TierScheduleList').jqGrid('hideCol', 'Delete');
            }
            imagePreview();
        },
        // editurl: "/QualityMonitoring/UpdateImageDetails",
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                //alert(xhr.responseText);
                alert("Error Occured.");
                window.location.href = "/Login/Login";
            }
            else {
                alert("Session Timeout !!!");
                window.location.href = "/Login/LogIn";
            }
        }
    }); //end of grid    
}

//   @Html.HiddenFor(model => model.DocumentName)



function isHiddenImage() {
    var Count = $('#NumberofFiles').val();

    if (Count == 0) {
        return true;
    }
    else {
        return false;
    }
}


function isHiddenByPIU(FinalizedByPIU) {

    if (FinalizedByPIU == 'Y')
    {
        return true;
    }
    else
    {
        return false;
    }
}



function downloadFileFromAction(paramurl) {
    window.location = paramurl;
}

function DownLoadImage(cellvalue) {
    //alert("cellvalue = " + cellvalue)
    var url = "/Execution/DownloadFileUploadedByAuditor/" + cellvalue;
    downloadFileFromAction(url);
}

function doNothing() {
    return false;
}

function imageFormatter(cellvalue, options, rowObject)
{
  

    var PictureURL = cellvalue.replace('/thumbnails', '');
    var abc = '/Content/images/PDF.png';
    //  onclick='doNothing(); return false;'
    // return " <a href='" + PictureURL + "' target='_blank'; class='preview'> <img style='height: 75px; width: 100px; border:solid 1px black' src='" + cellvalue + "' title=''  /> </a>";



    if ($("#DocumentName").val() == "NA")
    {
      return "-";
    }
    else
    {
        return " <a href='" + PictureURL + "' target='_blank'; class='preview'> <img style='height: 75px; width: 100px; border:solid 1px black' src='" + abc + "' title='Click here to View / Download File'  /> </a>";
    }


    //alert(cellvalue);
    //var PictureURL = cellvalue.replace('/thumbnails', '');

    //var arrLinkSrc = cellvalue.split("$$$");
    //var lnkHrefSrc = arrLinkSrc[0];

    //return " <a href='" + PictureURL + "' onclick='doNothing(); return false;' class='preview'><img style='height: 75px; width: 100px; border:solid 1px black' src='" + lnkHrefSrc + "' alt='Image not Available.' title=''  /> </a>";
}

function EditImageDetails(paramFileID) {
    var parts = paramFileID.split('$');
    jQuery("#tbFilesList").editRow(parts[0]);
    $('#tbFilesList').jqGrid('showCol', 'Save');
}

function SaveFileDetails(paramFileID) {
    var parts = paramFileID.split('$');
    //alert(parts[0]);
    jQuery("#tbFilesList").saveRow(parts[0], checksave);
}

function CancelSaveFileDetails(paramFileID) {
    var parts = paramFileID.split('$');
    $('#tbFilesList').jqGrid('hideCol', 'Save');
    jQuery("#tbFilesList").restoreRow(parts[0]);
}

function checksave(result) {
    //alert(result.responseText);
    $('#tbFilesList').jqGrid('hideCol', 'Save');
    if (result.responseText == "true") {
        alert('Details updated successfully.');
        return true;
    }
    else if (result.responseText != "") {
        alert("Error Occured.");
        //      alert(result.responseText.replace('"', "").replace('"', ""));
        return false;
    }
}

function ValidateImageDescription(value, colname) {

    if (value.trim().length == 0) {
        return ["Please Enter Image Description."];
    }
    else if (!value.match("^[a-zA-Z0-9 a-zA-Z0-9 ,.()-]+$")) {
        return [" Invalid Image Description Only Alphabets and Numbers are allowed."];
    }
    else {
        return [true, ""];
    }
}


function DeleteFileDetails(cellvalue) {

    if (confirm("Are you sure to delete the Image and Image details ? ")) {

        $.ajax({
            url: "/Execution/DeleteFileDetailsByPIU/" + cellvalue,
            type: "POST",
            cache: false,
            //data: {
            //    QM_FILE_NAME: QM_FILE_NAME, prRoadCode: prRoadCode, value: Math.random()
            //},
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                unblockPage();
                $("#tbFilesList").trigger('reloadGrid');
                if (response.Success) {
                    alert("File Deleted Succesfully.");

                    $("#dvAddMaintenanceAgreementAgainstRoad1").hide("slow");

                    if ($("#accordion1").is(":visible"))
                    {
                        $('#accordion1').hide('slow');
                    }

                    var ATRId = $("#ATRId").val();
                    // alert("ATR Id = " + ATRId)
                    GetUploadPhotoByPIU(ATRId);
                }
                else {
                    alert(response.ErrorMessage);

                    $("#dvAddMaintenanceAgreementAgainstRoad1").hide("slow");

                    if ($("#accordion1").is(":visible")) {
                        $('#accordion1').hide('slow');
                    }
                }
            }
        });

    }
    else {
        return;
    }
}


function uploadMissingImage(param) {


    $("#tbFilesList").jqGrid('setGridState', 'hidden');
    $.ajax({
        url: "/QualityMonitoring/UploadMissingImageLayout?id=" + param,
        type: "GET",
        dataType: "html",
        data: { qmSchCode: $('#ADMIN_SCHEDULE_CODE').val(), prRoadCode: $('#IMS_PR_ROAD_CODE').val(), qmObsId: $('#QM_OBSERVATION_ID').val() },
        success: function (data) {
            //$("#dvQMImageUploadModal").dialog('open');
            $("#dvQMImageUploadModal").html('');
            $("#dvQMImageUploadModal").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("Error Occured.");
            // alert(xhr.responseText);
        }
    });
}

function closeDiv() {
    //alert("closeMonitorsScheduleDetails");
    $("#dvQMImageUploadModal").html('');
    $("#tbFilesList").jqGrid('setGridState', 'visible');
}