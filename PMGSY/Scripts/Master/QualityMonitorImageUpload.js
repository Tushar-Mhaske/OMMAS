$(function () {
'use strict';
    
    // Initialize the jQuery File Upload widget:
    $('#fileupload').fileupload({
        maxNumberOfFiles: 1 - $("#NumberofFiles").val(),
        acceptFileTypes: /(\.|\/)(gif|jpe?g|png)$/i,
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

    });



    $("#fileupload").bind("fileuploaddone", function (e, data) {
        $("#divError").html("");
        $("#divError").hide("slow");

        $("#divSuccess").show("slow");

        $("#tbFilesList").trigger('reloadGrid');
        $("#tblPresentation tbody tr").remove();

        $("#divGlobalProgress").html("");
    });

    $('#fileupload').bind('fileuploadsubmit', function (e, data) {

        var inputs = data.context.find(':input');

        if (inputs.filter('[required][value=""]').first().focus().length) {
            return false;
        }
        data.formData = inputs.serializeArray();
        
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

        //alert(this.href);
    },
		function () {
		    this.title = this.t;
		    $("#preview").remove();
		}
	)
    //.mousemove(callback);
};

$(document).ready(function () {
    
    ListQualityMonitorFiles($("#ADMIN_QualityMonitor_CODE").val());
   
});

//Changed by deendayal on 05/19/2017
function ListQualityMonitorFiles(ADMIN_QM_CODE) {

    jQuery("#tbFilesList").jqGrid({
        url: '/Master/ListFiles/?' + (new Date().getTime()),
        datatype: "json",
        mtype: "POST",
        cache: true,
        colNames: ["Image", "Download", "Delete"],
        colModel: [
                    { name: 'image', index: 'image', width: 225, sortable: false, align: "center", formatter: imageFormatter, search: false, editable: false },
                    { name: 'download', index: 'download', width: 120, sortable: false, align: 'center', editable: false },
                    { name: 'Delete', index: 'Delete', width: 120, sortable: false, formatter: deleteFormatter, align: "center", editable: false },
        ],
        postData: { "ADMIN_QM_CODE": ADMIN_QM_CODE },
        pager: jQuery('#dvFilesListPager'),
        rowNum: 4,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Quality Monitor Image File",
        height: 'auto',
        rownumbers: true,
        loadComplete: function () {
            imagePreview();
        },
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

    $.ajax({
        url: paramurl,
        aysnc: false,
        catche: false,
        error: function (xhr, status, msg) {
            alert("An Error occured while processing your request.");
            return false;
        },
        success: function (responce) {

            if (responce.Success == "false") {
                alert("Image not available.");
                return false;
            }
            else {
                window.location = paramurl;
            }
        }
    });

    //window.location = paramurl;
}

function DownLoadImage(cellvalue) {
    var url = "/Master/DownloadFile/" + cellvalue;
    downloadFileFromAction(url);
}

function doNothing(){
    return false;
}

function imageFormatter(cellvalue, options, rowObject) {
    var PictureURL = cellvalue.replace('/thumbnails', '');
    
    return " <a href='" + PictureURL + "' onclick='doNothing(); return false;' class='preview'><img style='height: 75px; width: 100px; border:solid 1px black' src='" + cellvalue + "' alt='Image not Available' title=''  /> </a>";
}
    
//added by deendayal on 05/19/2017
function deleteFormatter(rowObject) {

    return "<a href='#' id='imageId' title='Click here to delete the File Details' class='ui-icon ui-icon-trash ui-align-center' onclick = DeleteFileDetails()  return false;>Delete</a>";

}
//Changed by deendayal on 05/19/2017
function DeleteFileDetails() {
    //jQuery(document).empty();

    var rowId = $('#imageId').parent().parent().attr('id');
    var param = rowId.split(' ')[0];
    var IMS_FILE_NAME = rowId.replace(param + " ", '');


    //alert(param +" "+ ImageName);
    //var param = cellvalue.ADMIN_QM_CODE;
    //var FILE_NAME = cellvalue.IMS_FILE_NAME;

    if (confirm("Are you sure to delete the Image? ")) {

        $.ajax({
            url: "/Master/DeleteFileDetails/?" + (new Date().getTime()),
            type: "POST",
            cache: true,
            data: {
                ADMIN_QM_CODE: param, FILE_NAME: IMS_FILE_NAME, FILE_TYPE: 'I', value: Math.random()
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
                    alert("Quality Monitor Image Deleted Succesfully.");
                    $("#rdoImage").trigger("click");
                    //Added By Abhishek kamble 21-feb-2014
                    //closeFileUpload();
                    $("#tblQualityMonitorListDetails").trigger('reloadGrid');
                    $('#dvhdFileUpload').show('slow', function () {
                        blockPage();
                        $("#divQualityMonitorForm").load('/Master/QualityMonitorFileUpload/' + param, function () {
                            unblockPage();
                        });
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