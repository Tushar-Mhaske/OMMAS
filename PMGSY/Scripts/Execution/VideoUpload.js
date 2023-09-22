$(function () {
    'use strict';

    // Initialize the jQuery File Upload widget:
    $('#fileupload').fileupload({
        maxNumberOfFiles: 5 - $("#NumberofFiles").val(),
        acceptFileTypes: /(\.|\/)(mp4|wmv)$/i,
        maxFileSize: 104857600
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

    $("#fileupload").bind('fileuploadfail', function (e, data) {

        //$("#tblPresentation").find('div.progress').find('div:eq(0)').css('width', '0px');
        //$("#tblPresentation").find('tr.template-upload').css('display','none');  

        //$('.template-upload').hide();

        //$("#tblPresentation tr.template-upload fade in").hide();
        //$("#divGlobalProgress").html("");

    });



    $("#fileupload").bind("fileuploaddone", function (e, data) {


        $("#divError").html("");
        $("#divError").hide("slow");

        $("#divSuccess").show("slow");

        $("#tbVideoFilesList").trigger('reloadGrid');
        //$("#tblPresentation tbody tr").remove();
        //$("#divGlobalProgress").html("");
        $(".preview").remove();


    });

    $('#fileupload').bind('fileuploadsubmit', function (e, data) {

        //var inputs = data.context.find(':input');
        var flagRemarks = true;
        //if (inputs.filter('[required][value=""]').first().focus().length) {
        //    return false;
        //}

        // new Changes by Vikram
        $("#tblPresentation textarea[name='remark[]']").each(function () {
            var regExRemarks = /^[a-zA-Z0-9 a-zA-Z0-9 ,.()-]+$/;
            var Remarks = $(this).val();
            if (Remarks == "") {
                flagRemarks = false;
                //alert("Please Enter Image Description");
                $("#divError").html("Please enter image description.");
                $("#divError").show('slow');
                $(this).focus();
                return false;
            }

            if (!regExRemarks.test(Remarks)) {
                //alert("Please Enter Valid Image Description,Can only contains AlphaNumeric values and [,.()-].   value " + Remarks);

                $("#divError").html("Please Enter Valid Image Description,Can only contains AlphaNumeric values and [,.()-].   value");
                $("#divError").show('slow');

                flagRemarks = false;
                return false;
            }

        });
        var inputs = data.context.find(':input');
        var text = data.context.find('textarea');
        data.formData = inputs.serializeArray();
        if (flagRemarks) {
            return true;
        }
        return false;

        //end of changes

        //data.formData = inputs.serializeArray();

        //not required
        //var chainage = $("#txtChainage").val();

        //start of old code
        //var Remarks = $("#txtRemark").val();

        //if (!Remarks.match("^[a-zA-Z0-9 ]+$")){
        //    $("#divError").show();
        //    $("#divError").html("Please Enter Valid Image Description, Only Alphabets and Numbers are allowed.");
        //    return false;
        //}

        //var regExAmount = /^\d{1,6}(\.\d{1,2})?$/;

        //end of old code

        //if (!regExAmount.test(chainage)) {
        //    alert("Please Enter valid Chainage,Only Numeric values,Total 7 Digits and 3 digits after decimal place are allowed.");
        //    return false;
        //}
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


        Mx = $(this).offset().left + 400; // * 2;//600
        My = $(this).offset().top - 50; //600;

        this.t = this.title;
        this.title = "";
        var c = (this.t != "") ? "<br/>" + this.t : "";
        $("body").append("<p id='preview'><img  style='height: 500px; width: 500px;' height='800' width='600' src='" + this.href + "' alt='Image Not Available' />" + c + "</p>");
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
    // $("#tbFilesList").trigger("reloadGrid");
    // imagePreview();
    ListProposalFiles($("#IMS_PR_ROAD_CODE").val());

});

function ListProposalFiles(IMS_PR_ROAD_CODE) {

    jQuery("#tbVideoFilesList").jqGrid({
        url: '/Execution/ListVideoFiles',
        datatype: "json",
        mtype: "POST",
        colNames: ["Video", "Description",  "Edit", "Delete", "Save"],
        colModel: [
                    //{ name: 'Name', index: 'Name', width: 125, sortable: false, align: "center", search: false, editable: false },
                    { name: 'Name', index: 'Name', width: 125, sortable: false, align: "center", formatter: AnchorFormatter, search: false, editable: false },
                    //{ name: 'Name', index: 'Name', width: 125, sortable: false, align: "center", editable: false },
                    //{ name: 'Size', index: 'Size', width: 50, sortable: false, align: "center", editable: false},                    
                    { name: 'Description', index: 'Description', width: 200, sortable: false, align: "center", editable: true, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidateImageDescription } },
                    //{ name: 'UploadDate', index: 'UploadDate', width: 125, sortable: false, align: "center", search: false, editable: false },
                    //{ name: 'download', index: 'download', width: 80, sortable: false, align: 'center', editable: false },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", editable: false },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", editable: false },
                    { name: 'Save', index: 'Save', width: 40, sortable: false, align: "center", editable: false }
        ],
        postData: { "IMS_PR_ROAD_CODE": IMS_PR_ROAD_CODE },
        pager: jQuery('#dvVideoFilesListPager'),
        rowNum: 4,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Execution Videos",
        height: 'auto',
        sortname: 'Name',
        //autowidth: true,
        cmTemplate: false,
        rownumbers: true,
        loadComplete: function () {
            imagePreview();
        },
        editurl: "/Execution/UpdateImageDetails",
        loadError: function (xhr, ststus, error) {
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
    var url = "/Execution/DownloadFile/" + cellvalue;
    downloadFileFromAction(url);
}

function doNothing() {
    return false;
}
function AnchorFormatter(cellvalue, options, rowObject) {

    var url = "/Execution/DownloadFile/" + cellvalue;

    return "<a href='#' onclick=downloadFileFromAction('" + url + "'); title='Click here to download video' return false;> <img style='height:16px;width:16px' height='20' width='20' border=0 src='../../Content/images/VideoIcon.jpg' /> </a>";
}
function imageFormatter(cellvalue, options, rowObject) {
    var PictureURL = cellvalue.replace('/thumbnails', '');

    return " <a href='" + PictureURL + "' onclick='doNothing(); return false;' class='preview'><img style='height: 75px; width: 100px; border:solid 1px black' src='" + cellvalue + "' alt='Image not Available' title=''  /> </a>";
}

function EditImageDetails(paramFileID) {
    jQuery("#tbVideoFilesList").editRow(paramFileID);
}

function SaveFileDetails(paramFileID) {
    jQuery("#tbVideoFilesList").saveRow(paramFileID, checksave);
}

function CancelSaveFileDetails(paramFileID) {
    jQuery("#tbVideoFilesList").restoreRow(paramFileID);
}

function checksave(result) {
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
    if (!value.match("^[a-zA-Z0-9 ]+$")) {
        return [" Invalid Video Description Only Alphabets and Numbers are allowed."];
    }
    else {
        return [true, ""];
    }
}

function DeleteFileDetails(param, IMS_FILE_NAME) {


    if (confirm("Are you sure to delete the Video and Video details ? ")) {

        $.ajax({
            url: "/Execution/DeleteFileDetails/",
            type: "POST",
            cache: false,
            data: {
                IMS_PR_ROAD_CODE: param, IMS_FILE_NAME: IMS_FILE_NAME, value: Math.random()
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
                //alert($("#Urlparameter").val());
                $("#tbVideoFilesList").trigger('reloadGrid');
                if (response.Success) {
                    alert("Video and Video Details Deleted Succesfully.");
                    //$("#rdoImage").trigger("click");
                    $("#divVideoUpload").load('/Execution/VideoUpload/' + $("#Urlparameter").val(), function (data) {
                        $.validator.unobtrusive.parse($('#fileupload'));
                        unblockPage();
                        if (data.success == false) {
                            alert(data.message);
                        }
                    });

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