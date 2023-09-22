$(function () {
    'use strict';

    // Initialize the jQuery File Upload widget:
    $('#fileupload').fileupload({
        maxNumberOfFiles: 5 - parseInt($("#NumberofImages").val()),
        acceptFileTypes: /(\.|\/)(jpg|jpe?g)$/i,
        maxFileSize: 4000000
        //maxFileSize: 4096,
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
        $("#divError").html("");
        $("#divError").hide("slow");

        $("#divSuccess").show("slow");

        $("#tbNewsImageFilesList").trigger('reloadGrid');
        //$("#tblPresentation tbody tr").remove();    

        //$("#divGlobalProgress").html("");
    });

    $('#fileupload').bind('fileuploadsubmit', function (e, data) {

        $("#divError").hide('slow');
        $("#divSuccess").hide('slow');

        var flagChainage = true;
        var flagRemarks = true;

        //$("#tblPresentation input[name='chainageValue[]']").each(function () {

        //    var regExAmount = /^\d{1,6}(\.\d{1,3})?$/;
        //    var Chainage = $(this).val();

        //    if (Chainage == "") {
        //        flagChainage = false;
        //        //alert("Please Enter Chainage");
        //        $("#divError").html("Please Enter valid Chainage,Only Numeric values,Total 7 Digits and 3 digits after decimal place are allowed.");
        //        $("#divError").show('slow');
        //        $(this).focus();
        //        return false;
        //    }

        //    if (!regExAmount.test(Chainage)) {
        //        //alert("Please Enter valid Chainage,Only Numeric values,Total 7 Digits and 3 digits after decimal place are allowed.");
        //        $("#divError").html("Please Enter valid Chainage,Only Numeric values,Total 7 Digits and 3 digits after decimal place are allowed.");
        //        $("#divError").show('slow');
        //        flagChainage = false;
        //        return false;
        //    }

        //});

        $("#tblPresentation textarea[name='remark[]']").each(function () {
            var regExRemarks = /^[a-zA-Z0-9 a-zA-Z0-9 ,.()-]+$/;
            var Remarks = $(this).val().trim();

            if (Remarks == "") {
                flagRemarks = false;
                //alert("Please Enter Image Description");
                $("#divError").html("Please Enter Valid Image Description,Can only contains AlphaNumeric values and [,.()-].   value");
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


        //if (flagChainage && flagRemarks) {
        if (flagRemarks) {
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
    //alert("load");
    ListProposalFiles($("#News_Id").val());

});

function ListProposalFiles(News_Id) {

    jQuery("#tbNewsImageFilesList").jqGrid({
        url: '/NewsDetails/ListImageFiles',
        datatype: "json",
        mtype: "POST",
        colNames: ["Image", "Description", "Download", "Edit", "Delete", "Action"],
        colModel: [
                    { name: 'image', index: 'image', width: 125, sortable: false, align: "center", formatter: imageFormatter, search: false, editable: false },
                    //{ name: 'Name', index: 'Name', width: 125, sortable: false, align: "center", editable: false },
                    //{ name: 'Size', index: 'Size', width: 50, sortable: false, align: "center", editable: false},
                    //{ name: 'Chainage', index: 'Chainage', width: 70, sortable: false, align: "center", editable: true, editoptions: { maxlength: 8 } },
                    { name: 'Description', index: 'Description', width: 200, sortable: false, align: "center", editable: true, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidateImageDescription } },
                    { name: 'download', index: 'download', width: 80, sortable: false, align: 'center', editable: false },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", editable: false },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", editable: false },
                    { name: 'Save', index: 'Save', width: 40, sortable: false, align: "center", editable: false, hidden: true }
        ],
        postData: { "News_Id": News_Id },
        pager: jQuery('#dvNewsImageFilesListPager'),
        rowList: [05, 10, 15],
        rowNum: 05,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Images Files",
        height: 'auto',
        //autowidth: true,
        sortname: 'image',
        rownumbers: true,
        loadComplete: function () {
            imagePreview();

        },
        editurl: "/NewsDetails/UpdateImageDetails",
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
    var url = "/NewsDetails/DownloadFile/" + cellvalue;
    downloadFileFromAction(url);
}

function doNothing() {
    return false;
}

function imageFormatter(cellvalue, options, rowObject) {
    var PictureURL = cellvalue.replace('/thumbnails', '');

    return " <a href='" + PictureURL + "' onclick='doNothing(); return false;' class='preview'><img style='height: 75px; width: 100px; border:solid 1px black' src='" + cellvalue + "' alt='Image not Available' title=''  /> </a>";
}

function EditImageDetails(paramFileID) {
    jQuery("#tbNewsImageFilesList").editRow(paramFileID);
    $('#tbNewsImageFilesList').jqGrid('showCol', 'Save');
}

function SaveFileDetails(paramFileID) {
    jQuery("#tbNewsImageFilesList").saveRow(paramFileID, checksave);
}

function CancelSaveFileDetails(paramFileID) {
    $('#tbNewsImageFilesList').jqGrid('hideCol', 'Save');
    jQuery("#tbNewsImageFilesList").restoreRow(paramFileID);
}

function checksave(result) {
    $('#tbNewsImageFilesList').jqGrid('hideCol', 'Save');
    if (result.responseText == "true") {
        alert('Details Updated Successfully.');
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

function DeleteFileDetails(param, IMS_FILE_NAME) {
    //alert(IMS_FILE_NAME);
    if (confirm("Are you sure to Delete the Image and Image details ? ")) {

        $.ajax({
            url: "/NewsDetails/DeleteFileDetails/",
            type: "POST",
            cache: false,
            data: {
                News_Id: param, NEWS_FILE_NAME: IMS_FILE_NAME, ISPF_TYPE: 'I', value: Math.random() //IMS_PR_ROAD_CODE: param, IMS_FILE_NAME: IMS_FILE_NAME, ISPF_TYPE: 'I', value: Math.random()
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
                $("#tbNewsImageFilesList").trigger('reloadGrid');
                if (response.Success) {
                    alert("News Image and Image Details Deleted Succesfully.");
                    $("#rdbtnImage").trigger("click");
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

