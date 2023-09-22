/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   ImageUpload.js
        * Description   :   Handles events, grid data in Image upload
        * Author        :   Shyam Yadav 
        * Creation Date :   10/Jun/2013
 **/


$(function () {
    'use strict';
    //alert($("#NumberofFiles").val());
    // Initialize the jQuery File Upload widget:
    $('#fileupload').fileupload({
        maxNumberOfFiles: 20 - parseInt($("#NumberofFiles").val()),
        acceptFileTypes: /(\.|\/)(jpg|jpe?g)$/i,
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
        $("#tb3TierInspectionList").trigger('reloadGrid')
        $("#tbMonitorsInspectionList").trigger('reloadGrid');
        $("#tbMonitorsObsList").trigger("reloadGrid");
        //$("#tblPresentation tbody tr").remove();    

        //$("#divGlobalProgress").html("");
    });

    $('#fileupload').bind('fileuploadsubmit', function (e, data) {

        var flagRemarks = true;

        $("#tblPresentation textarea[name='remark[]']").each(function () {
            var regExRemarks = /^[a-zA-Z0-9 a-zA-Z0-9 ,.()-]+$/;
            var Remarks = $(this).val().trim();

            if (Remarks == "") {
                flagRemarks = false;
                $("#divImgUploadError").html("Please Enter Valid Image Description,Can only contains AlphaNumeric values and [,.()-].   value");
                $("#divImgUploadError").show('slow');

                $(this).focus();
                return false;
            }

            if (!regExRemarks.test(Remarks)) {
                //alert("Please Enter Valid Image Description,Can only contains AlphaNumeric values and [,.()-].   value " + Remarks);

                $("#divImgUploadError").html("Please Enter Valid Image Description,Can only contains AlphaNumeric values and [,.()-].   value");
                $("#divImgUploadError").show('slow');

                flagRemarks = false;
                return false;
            }
        });
        if (!flagRemarks) {
            return false;
        }
        var flagLatitude = true;


        $("#tblPresentation textarea[name='latitude[]']").each(function () {
            var regExLatitude = /^s*?\d{1,6}(\.\d{1,12})?$/;
            var latitude = $(this).val().trim();

            if (latitude == "") {
                flagLatitude = false;
                $("#divImgUploadError").html("Please Enter Valid Latitude ,Can only contains Numeric values and 12 digit after decimal place.");
                $("#divImgUploadError").show('slow');

                $(this).focus();
                return false;
            }

            if (!regExLatitude.test(latitude)) {
                //alert("Please Enter Valid Image Description,Can only contains AlphaNumeric values and [,.()-].   value " + Remarks);

                $("#divImgUploadError").html("Please Enter Valid Latitude ,Can only contains Numeric values and 12 digit after decimal place");
                $("#divImgUploadError").show('slow');

                flagLatitude = false;
                return false;
            }

        });
        if (!flagLatitude) {
            return false;
        }

        var flagLongitude = true;


        $("#tblPresentation textarea[name='longitude[]']").each(function () {
            var regExLongitude = /^s*?\d{1,6}(\.\d{1,12})?$/;
            var longitude = $(this).val().trim();

            if (longitude == "") {
                flagLongitude = false;
                $("#divImgUploadError").html("Please Enter Valid Longitude ,Can only contains Numeric values and 12 digit after decimal place.");
                $("#divImgUploadError").show('slow');

                $(this).focus();
                return false;
            }

            if (!regExLongitude.test(longitude)) {
                //alert("Please Enter Valid Image Description,Can only contains AlphaNumeric values and [,.()-].   value " + Remarks);

                $("#divImgUploadError").html("Please Enter Valid Longitude ,Can only contains Numeric values and 12 digit after decimal place");
                $("#divImgUploadError").show('slow');

                flagLongitude = false;
                return false;
            }
        });

        if (!flagLongitude) {
            return false;
        }
        //alert(flagRemarks);
        //return false;

        var inputs = data.context.find(':input');
        var text = data.context.find('textarea');
        data.formData = inputs.serializeArray();
        //$("#fileupload").serialize();


        if (flagRemarks && flagLatitude && flagLongitude) {
            return true;
        }
        else {
            return false;
        }



    });

    $('#fileupload').bind('fileuploadadd', function (e, data) {
        $("#divSuccess").hide("slow");

        //$("#tbMonitorsObsList").trigger("reloadGrid");
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

    ListQualityFiles($("#QM_LAB_ID").val());

});

function doNothing() {
    return false;
}

function ListQualityFiles(QM_LAB_ID) {
    jQuery("#tbFilesList").jqGrid('GridUnload');

    jQuery("#tbFilesList").jqGrid({
        url: '/QualityMonitoring/ListLabFiles?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Image", "Description", "Latitude", "Longitude", "Download", "Edit", "Delete", "Action"], 
        colModel: [
                    { name: 'image', index: 'image', width: 125, sortable: false, align: "center", formatter: imageFormatter, search: false, editable: false },
                    { name: 'Description', index: 'Description', width: 150, sortable: false, align: "left", editable: true, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidateImageDescription } },
                    { name: 'Latitude', index: 'Latitude', width: 100, sortable: false, align: "left", editable: true, editoptions: { maxlength: 18 }, editrules: { custom: true, custom_func: ValidateLatitude } },
                    { name: 'Longitude', index: 'Longitude', width: 100, sortable: false, align: "left", editable: true, editoptions: { maxlength: 18 }, editrules: { custom: true, custom_func: ValidateLongitude } },
                    { name: 'download', index: 'download', width: 80, sortable: false, align: 'center', editable: false },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", editable: false },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", editable: false },
                    { name: 'Save', index: 'Save', width: 40, sortable: false, align: "center", editable: false, hidden: true }
                  ],
        postData: { "QM_LAB_ID": QM_LAB_ID, "value": Math.random() },
        pager: jQuery('#dvFilesListPager'),
        rowNum: 30,
        viewrecords: true,
        pgbuttons: false,
        pgtext: null,
        recordtext: '{2} records found',
        caption: "Image Details",
        height: '300',
        sortname: 'Image',
        rownumbers: true,
        loadComplete: function () {

            //if ($("#hdnRoleImgUpload").val() == 6 || $("#hdnRoleImgUpload").val() == 7) //for SQC hide these two columns
            //{
            //    $('#tb3TierScheduleList').jqGrid('hideCol', 'Delete');
            //}
            imagePreview();
        },
        editurl: "/QualityMonitoring/UpdateLabImageDetails",
        loadError: function (xhr, status, error) {
            alert(xhr.responseText);
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Session Timeout !!!");
                window.location.href = "/Login/LogIn";
            }
        }
    }); //end of grid    
}


function DownLoadImage(cellvalue) {
    var url = "/QualityMonitoring/DownloadLabFile/" + cellvalue;
    $.ajax({
        url: url,
        aysnc: false,
        catche: false,
        error: function (xhr, status, msg) {
            alert("An Error occured while processing your request.");
            return false;
        },
        success: function (responce) {

            if (responce.Success == "false") {
                alert("File not available.");
                return false;
            }
            else {
                window.location = url;
            }
        }
    });

}

function doNothing() {
    return false;
}

function imageFormatter(cellvalue, options, rowObject) {
    var PictureURL = cellvalue.replace('/thumbnails', '');

    var arrLinkSrc = cellvalue.split("$$$");
    var lnkHrefSrc = arrLinkSrc[0];

    return " <a href='" + PictureURL + "' onclick='doNothing(); return false;' class='preview'><img style='height: 75px; width: 100px; border:solid 1px black' src='" + lnkHrefSrc + "' alt='Image not Available' title=''  /> </a>";
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
        alert('Lab Image details updated successfully.');
        return true;
    }
    else if (result.responseText != "") {
        alert(result.responseText.replace('"', "").replace('"', ""));
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
function ValidateLatitude(value, colname) {
 
    if (value.trim().length == 0) {
        return ["Please Enter Latitude."];
    }
    else if (!value.match("^s*?\d{1,6}(\.\d{1,12})?$")) {
        return [" Invalid Latitude, Can only contains Numeric values and 12 digit after decimal place."];
    }
    else {
        return [true, ""];
    }
}

function ValidateLongitude(value, colname) {

    if (value.trim().length == 0) {
        return ["Please Enter Longitude."];
    }
    else if (!value.match("^s*?\d{1,6}(\.\d{1,12})?$")) {
        return [" Invalid Longitude, Can only contains Numeric values and 12 digit after decimal place."];
    }
    else {
        return [true, ""];
    }
}


function DeleteFileDetails(param, QM_FILE_NAME, scheduleCode, prRoadCode) {

    if (confirm("Are you sure to delete the Image and Image details ? ")) {

        $.ajax({
            url: "/QualityMonitoring/DeleteLabImagesDetails/",
            type: "POST",
            cache: false,
            data: {
                QM_LAB_DETAIL_ID: param, value: Math.random()
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
                if (response.Success == true) {
                    alert("Image and Image Details Deleted Succesfully.");
                    $("#divSuccess").hide("slow");
                    $("#tbFilesList").trigger('reloadGrid');
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