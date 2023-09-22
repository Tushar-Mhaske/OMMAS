/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   ImageUpload.js
        * Description   :   Handles events, grid data in Image upload
        * Author        :   
        * Creation Date :   
 **/


$(function () {
    'use strict';
    //alert($("#NumberofFiles").val());
    // Initialize the jQuery File Upload widget:
    $('#fileupload').fileupload({
        maxNumberOfFiles: 20 - parseInt($("#NumberofFiles").val()),
        acceptFileTypes: /(\.|\/)(gif|pdf)$/i,
        //  acceptFileTypes: /(\.|\/)(jpg|jpe?g)$/i,
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
        //$("#tblPresentation").find('div.progress').find('div:eq(0)').css('width', '0px');
        //$("#tblPresentation").find('tr.template-upload').css('display','none');       
        $("#divGlobalProgress").html("");
    });



    $("#fileupload").bind("fileuploaddone", function (e, data) {
        $("#divImgUploadError").html("");
        $("#divImgUploadError").hide("slow");

        $("#divSuccess").show("slow");
      

        $("#tbFilesList").trigger('reloadGrid');

    });



    $('#fileupload').bind('fileuploadsubmit', function (e, data)
    {
        var flagRemarks = true;

        $("#tblPresentation textarea[name='remark[]']").each(function ()
        {
            var regExRemarks = /^[a-zA-Z0-9 a-zA-Z0-9 ,.()-]+$/;
            var Remarks = $(this).val().trim();
            var Remarks = "NA"; // Added this Line so as to not to enter remarks
            if (Remarks == "")
            {
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

      

        var inputs = data.context.find(':input');
        var text = data.context.find('textarea');
        data.formData = inputs.serializeArray();
        //$("#fileupload").serialize();

        if (flagRemarks)
        {
            return true;
        }
        return false;


    });

    $('#fileupload').bind('fileuploadadd', function (e, data) {
        $("#divSuccess").hide("slow");

       
    });

});


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
        $("body").append("<p id='preview'><img  style='height: 500px; width: 500px;' height='800' width='600' src='" + lnkHref + "' alt='Image Not Available' />" + c + "</p>");
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
    //alert("Inside Doc Ready")

    if ($("#hdnRoleImgUpload").val() == 25 || $("#hdnRoleImgUpload").val() == 22) {
        // For MORD 25 , and For PIU 22 Hide Image

        $("#AddOne").hide();
        $("#AddTwo").hide();

        $("#SamePDFAsPIU").hide();
    }



    ListQualityFiles($("#FEED_ID").val(), $("#REP_ID").val());

    

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

function ListQualityFiles(FEED_ID,REP_ID) {

  //  alert("Hre")
    var FinalizedByPIU = $("#FinalizedByPIU").val();

    jQuery("#tbFilesList").jqGrid('GridUnload');

    jQuery("#tbFilesList").jqGrid({
        url: '/FeedbackDetails/ListPDFFilesBySQC?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["PDF", 'Uploaded By','Description of PDF',"Download", "Delete PDF"],
        colModel: [
                    { name: 'image', index: 'image', width: 125, sortable: false, align: "center", formatter: imageFormatter1, search: false, editable: false },

                   
                      { name: 'PIU_SQC', index: 'PIU_SQC', width: 200, sortable: false, align: 'center', editable: false },
                      { name: 'FILE_DESC', index: 'FILE_DESC', width: 200, sortable: false, align: 'center', editable: false },

                   //{ name: 'Description', index: 'Description', width: 200, sortable: false, align: "left", editable: true, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidateImageDescription } },
                      { name: 'a', index: 'a', width: 60, sortable: false, align: 'center', editable: false, hidden: isHiddenImage(), title: false },
                      { name: 'b', index: 'b', width: 60, sortable: false, align: "center", editable: false, hidden: isHidden(FinalizedByPIU), title: false }

        ],
        postData: { "FEED_ID": FEED_ID, "REP_ID": REP_ID, "value": Math.random() },
        pager: jQuery('#dvFilesListPager'),
        rowNum: 30,
        viewrecords: true,
        pgbuttons: false,
        pgtext: null,
        recordtext: '{2} records found',
        caption: "PDF Details (SQC)",
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
                alert("Error Occured.");
                //    alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Session Timeout !!!");
                window.location.href = "/Login/LogIn";
            }
        }
    }); //end of grid    
}
// hidden: isHidden(nameRole)


//NumberofFiles
function imageFormatter1(cellvalue, options, rowObject) {


    var PictureURL = cellvalue.replace('/thumbnails', '');
    var abc = '/Content/images/PDF.png';
    //  onclick='doNothing(); return false;'
    // return " <a href='" + PictureURL + "' target='_blank'; class='preview'> <img style='height: 75px; width: 100px; border:solid 1px black' src='" + cellvalue + "' title=''  /> </a>";



    //if ($("#DocumentName").val() == "NA") {
    //    return "-";
    //}
    //else {
    return " <a href='" + PictureURL + "' target='_blank'; class='preview'> <img style='height: 75px; width: 100px; border:solid 1px black' src='" + abc + "' title='Click here to View / Download File'  /> </a>";
    // }


    //alert(cellvalue);
    //var PictureURL = cellvalue.replace('/thumbnails', '');

    //var arrLinkSrc = cellvalue.split("$$$");
    //var lnkHrefSrc = arrLinkSrc[0];

    //return " <a href='" + PictureURL + "' onclick='doNothing(); return false;' class='preview'><img style='height: 75px; width: 100px; border:solid 1px black' src='" + lnkHrefSrc + "' alt='Image not Available.' title=''  /> </a>";
}
//isHiddenImage

function isHiddenImage() {
    var Count = $('#NumberofFiles').val();

    if (Count == 0)
    {
        return true;
    }
    else
    {
        return false;
    }
}

function isHidden(FinalizedByPIU)
{


    var FinalizedBySQC = $("#FinalizedBySQC").val();

    if (FinalizedBySQC == 'Y') {
        return true;
    }



    if ($("#hdnRoleImgUpload").val() == 25 || $("#hdnRoleImgUpload").val() == 22) {
        // For MORD 25 , and For PIU 22 Hide PDF

        return true;
    }


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

function DownLoadPDFSQC(cellvalue) {
    //alert("cellvalue = " + cellvalue)
    var url = "/FeedbackDetails/DownloadPDFFileUploadedBySQC/" + cellvalue;
    downloadFileFromAction(url);
}

function doNothing() {
    return false;
}

function imageFormatter(cellvalue, options, rowObject)
{
    //alert(cellvalue);

    //alert("cellvalue = " + cellvalue)
    //alert("options = " + options)
    //alert("rowObject = " + rowObject)

    var PictureURL = cellvalue.replace('/thumbnails', '');

 
    var arrLinkSrc = cellvalue.split("$$$");
    var lnkHrefSrc = arrLinkSrc[0];

  
    return " <a href='" + PictureURL + "' onclick='doNothing(); return false;' class='preview'> <img style='height: 75px; width: 100px; border:solid 1px black' src='" + lnkHrefSrc + "' alt='Image not Available.' title=''  /> </a>";
    
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
        //  alert(result.responseText.replace('"', "").replace('"', ""));
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


function DeletePDFFileDetailsSQC(cellvalue) {

    if (confirm("Are you sure to delete the PDF and its details ? ")) {

        $.ajax({
            url: "/FeedbackDetails/DeletePDFFileDetailsBySQC/" + cellvalue,
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
                    alert("PDF and its details deleted Succesfully.");
                    ListQualityFiles($("#FEED_ID").val(), $("#REP_ID").val());
             }
                else
                {
                    alert(response.ErrorMessage);
                }
            }
        });

    }
    else {
        return;
    }
}



function closeDiv() {
    $("#dvQMImageUploadModal").html('');
    $("#tbFilesList").jqGrid('setGridState', 'visible');
}





$('#SamePDFAsPIU').click(function () {

    SamePDFAsPIUDetails();
});

function SamePDFAsPIUDetails() {

    if (confirm("Are you sure to upload same PDF as PIU ? ")) {

        $.ajax({
            url: "/FeedbackDetails/UploadSamePDFAsPIUDetails/",
            type: "POST",
            cache: false,
            data: {
                FEED_ID: $("#FEED_ID").val(), REP_ID: $("#REP_ID").val(), value: Math.random()
            },
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
                    alert("PDF details uploaded Succesfully.");
                    ListQualityFiles($("#FEED_ID").val(), $("#REP_ID").val());
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