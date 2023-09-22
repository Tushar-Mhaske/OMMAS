$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmstateprofile'));

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    LoadGrid();

    $("#btnViewSPDetails").click(function () {
       
        if ($("#ddlSPState").val() <= 0) {
            alert("Please Select State");
            return;
        }
        LoadGrid();
    });


});

function FormatColumn(cellvalue, options, rowObject) {
    if (cellvalue.toString() == "") {
        //return "<center><table><tr><td style='border:none;'><span>-</span></td><td style='border:none;'><span>-</span></td></tr></table></center>";
        return "<center><table><tr><td  style='aadadborder:none'><span class='ui-icon ui-icon-locked');'></span></td><td style='border:none'><span class='ui-icon ui-icon-locked');'></span></td></tr></table></center>";
    }

    return "<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Wrongly Mapped Hab Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function LoadGrid() {
  
    $('#tblList').jqGrid('GridUnload');
    $('#tblList').jqGrid({

        url: '/MPRFileDownload/MPRFileDownloads/MPRFileUploadGetGridView',
        datatype: 'json',
        mtype: "POST",
        colNames: ['Upload Date', 'File Name', 'File Description', 'File Type'],
        postData: { stateCode:$("#ddlSPState").val() },
        colModel: [
         { name: 'MPR_FILE_UPLOAD_DATE', index: 'MPR_FILE_UPLOAD_DATE', height: 'auto', width: 25, align: "left", sortable: true },

         { name: 'MPR_FILE_NAME', index: 'MPR_FILE_NAME', height: 'auto', width: 40, align: "left", sortable: true },

         { name: 'MPR_FILE_DESC', index: 'MPR_FILE_DESC', height: 'auto', width: 40, align: "left", sortable: true },

         { name: 'MPR_FILE_TYPE', index: 'MPR_FILE_TYPE', height: 'auto', width: 35, align: "left", sortable: true },

        //{ name: 'a', width: 50, sortable: false, resize: false, formatter: FormatColumn, align: "left", sortable: false }
        ],
        pager: jQuery('#PagerList'),
        rowNum: 15,
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MPR_FILE_UPLOAD_DATE',
        sortorder: "desc",
        caption: 'MPR File Details',
        height: '100%',
        rownumbers: true,
        hidegrid: false,
        autowidth: true,
        cmTemplate: { title: false },
        emptyrecords: 'No Records Found',
        loadComplete: function () { },

        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {

                alert(xht.responseText);
                window.location.href = "Login/login";
            }
            else {
                alert("Invalid Data. Please Check and Try Again");
            }
        }
    });


}

function downloadFileFromAction(paramurl)
{
    window.location = paramurl;
}

function DownLoadFile(cellvalue)
{
    //alert(cellvalue);
   // alert(encodeURIComponent(cellvalue.replace(/\#/g, ' ')));
     
    var FileName = encodeURIComponent(cellvalue.replace(/\#/g, ' '));

   // alert(FileName);
    //replace('&', '%26');
   // return false;

    var url = "/MPRFileDownload/MPRFileDownloads/DownloadFiles?id=" + FileName;//DAL-Space replace by # ,Js - # replace by space
   
    window.location = url;

   
}

function DownLoadFileNotExist()
{
    alert("This file dose not exist.");
    return false;
}