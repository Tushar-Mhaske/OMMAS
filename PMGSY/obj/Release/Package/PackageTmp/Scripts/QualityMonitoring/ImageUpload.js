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
        maxNumberOfFiles: 25 - parseInt($("#NumberofFiles").val()),
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


    // Loads:
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
        //   $("#tblPresentation").find('tr.template-upload').css('display','none');   //changes  
        //alert("e" + JSON.stringify(e.type))
        //if (e.type == "fileuploadfail")
        //{

        //    $("#tblPresentation").find('tr.template-upload').css('display', 'none');
        //}

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
        //alert("in single upload");
        //alert("e" +JSON.stringify(e))
        //alert("data" + JSON.stringify(data))
        //  alert("name"+$("#fileupload-fp").val)

        //alert('2');


        var flagRemarks = true;





        $("#tblPresentation textarea[name='remark[]']").each(function () {
            var regExRemarks = /^[a-zA-Z0-9 a-zA-Z0-9 ,.()-]+$/;
            var Remarks = $(this).val().trim();

            if (Remarks == "") {
                flagRemarks = false;
                $("#divImgUploadError").html("Please Enter Valid Image Description, Can only contains AlphaNumeric values and ,.()-   value");
                $("#divImgUploadError").show('slow');

                $(this).focus();
                return false;
            }

            if (!regExRemarks.test(Remarks)) {
                //alert("Please Enter Valid Image Description,Can only contains AlphaNumeric values and [,.()-].   value " + Remarks);
                //alert(Remarks);
                //$("#divImgUploadError").html("Please Enter Valid Image Description,Can only contains AlphaNumeric values and [,.()-].   value");
                $("#divImgUploadError").html("Please Enter Valid Image Description, Can only contains AlphaNumeric values and ,.()-   value");
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

    ListQualityFiles($("#ADMIN_SCHEDULE_CODE").val(), $("#IMS_PR_ROAD_CODE").val(), $("#QM_OBSERVATION_ID").val());

});

function doNothing() {
    return false;
}

function ListQualityFiles(ADMIN_SCHEDULE_CODE, IMS_PR_ROAD_CODE, QM_OBSERVATION_ID) {
    jQuery("#tbFilesList").jqGrid('GridUnload');

    jQuery("#tbFilesList").jqGrid({
        url: '/QualityMonitoring/ListFiles?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Image", "Description", "Download", "Edit", "Action", "Latitude", "Longitude", "Upload Missing Images"],
        colModel: [
                    { name: 'image', index: 'image', width: 125, sortable: false, align: "center", formatter: imageFormatter, search: false, editable: false },
                    { name: 'Description', index: 'Description', width: 200, sortable: false, align: "left", editable: true, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidateImageDescription } },
                    { name: 'download', index: 'download', width: 80, sortable: false, align: 'center', editable: false },
                    { name: 'Edit', index: 'Edit', width: 80, sortable: false, align: "center", editable: false, hidden: true },
                    //{ name: 'Delete', index: 'Delete', width: 80, sortable: false, align: "center", editable: false },
                    { name: 'Save', index: 'Save', width: 80, sortable: false, align: "center", editable: false, hidden: true },
                    ///Added by SAMMED A. PATIL on 27DEC2017 for Adding missing files
                    { name: 'Latitude', index: 'Save', width: 80, sortable: false, align: "center", editable: false, hidden: true },
                    { name: 'Longitude', index: 'Save', width: 80, sortable: false, align: "center", editable: false, hidden: true },
                    { name: 'MissingFiles', index: 'MissingFiles', width: 80, sortable: false, align: 'center', editable: false, hidden: true },
        ],
        postData: { "ADMIN_SCHEDULE_CODE": ADMIN_SCHEDULE_CODE, "IMS_PR_ROAD_CODE": IMS_PR_ROAD_CODE, "QM_OBSERVATION_ID": QM_OBSERVATION_ID, "value": Math.random() },
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

            if ($("#hdnRoleImgUpload").val() == 6 || $("#hdnRoleImgUpload").val() == 7) //for SQC hide these two columns
            {
                $('#tb3TierScheduleList').jqGrid('hideCol', 'Delete');
            }
            imagePreview();
        },
        editurl: "/QualityMonitoring/UpdateImageDetails",
        loadError: function (xhr, status, error) {
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

function downloadFileFromAction(paramurl) {
    window.location = paramurl;
}

function DownLoadImage(cellvalue) {
    //var url = "/QualityMonitoring/DownloadFile/" + cellvalue;
    //downloadFileFromAction(url);

    $.ajax({
        url: '/QualityMonitoring/DownloadFile/' + cellvalue,
        type: "GET",
        cache: false,
        async: false,

        success: function (response) {

            if (response.Error) {
                alert("File Not Exist")
            }
            else {
                var url = "/QualityMonitoring/DownloadFile/" + cellvalue;
                downloadFileFromAction(url);
            }
        }


    });
}

function doNothing() {
    return false;
}

function imageFormatter(cellvalue, options, rowObject) {
    //alert(cellvalue);
    //  var PictureURL = cellvalue.replace('/thumbnails', '');
    var PictureURL = cellvalue          // changes done by Saurabh for this line only
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
    //alert("paramFileID" + paramFileID)
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

function DeleteFileDetails(param, QM_FILE_NAME, scheduleCode, prRoadCode) {

    if (confirm("Are you sure to delete the Image and Image details ? ")) {

        $.ajax({
            url: "/QualityMonitoring/DeleteFileDetails/",
            type: "POST",
            cache: false,
            data: {
                QM_OBSERVATION_ID: param, QM_FILE_NAME: QM_FILE_NAME, value: Math.random()
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
                    alert("Image and Image Details Deleted Succesfully.");

                    $("#tb3TierInspectionList").trigger('reloadGrid')
                    $("#tbMonitorsInspectionList").trigger('reloadGrid');
                    $("#tbMonitorsObsList").trigger("reloadGrid");


                    if ($("#hdnRoleImgUpload").val() == 5 || $("#hdnRoleImgUpload").val() == 8) {

                        UploadQMFile(param, scheduleCode, prRoadCode);
                    }
                    else if ($("#hdnRoleImgUpload").val() == 6 || $("#hdnRoleImgUpload").val() == 7) {
                        $("#accordionObsMonitors div").html("");
                        $("#accordionMonitorsInspection div").html("");

                        if ($.isFunction(window.UploadObsFile)) {
                            UploadObsFile(scheduleCode, prRoadCode);
                        }
                        else if ($.isFunction(window.UploadMonitorsFile)) {
                            UploadMonitorsFile(param, scheduleCode, prRoadCode);
                        }

                    }
                    else if ($("#hdnRoleImgUpload").val() == 9) {
                        UploadCqcFile(scheduleCode, prRoadCode);
                    }



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

///Added by SAMMED A. PATIL on 27DEC2017 for Adding missing files
function uploadMissingImage(param) {
    //$("#dvQMImageUploadModal").load('/QualityMonitoring/UploadMissingImageLayout');
    //$("#dvQMImageUploadModal").dialog('open');
    //var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();

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
            alert(xhr.responseText);
        }
    });
}

function closeDiv() {
    //alert("closeMonitorsScheduleDetails");
    $("#dvQMImageUploadModal").html('');
    $("#tbFilesList").jqGrid('setGridState', 'visible');
}




