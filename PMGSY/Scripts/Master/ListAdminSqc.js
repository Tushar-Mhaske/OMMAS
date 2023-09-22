$(document).ready(function () {
      
     $("input[type=text]").bind("keypress", function (e) {
         if (e.keyCode == 13) {
             return false;
         }
     });

    
     $.ajax({
         url: "/Master/SearchSQC",
         type: "GET",
         dataType: "html",
         success: function (data) {
             $("#dvSearchSQC").html(data);
             $('#btnSearch').trigger('click');

         },
         error: function (xhr, ajaxOptions, thrownError) {
             alert(xhr.responseText);
         }

     });

    //function for loading Add form.
     $('#btnCreateNewSQC').click(function () {
         if ($("#dvSearchSQC").is(":visible")) {
             $('#dvSearchSQC').hide('slow');
         }
         if (!$('#dvDetails').is(':visible')) {
             $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
             $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');
             $("#dvDetails").load("/Master/AddAdminSqc", function () {
                 $('#dvDetails').show();
                 $('#btnCreateNewSQC').hide();
                 $('#btnSearchViewSQC').show();
                 $.unblockUI();
             });
         }
     });

     //function for loading search form.   
     $('#btnSearchViewSQC').click(function (e) {
         $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
         if ($("#dvDetails").is(":visible")) {
             $('#dvDetails').hide('slow');
            // $('#btnSearchViewSQC').hide();
             $('#btnCreateNewSQC').show();
         }
         if (!$("#dvSearchSQC").is(":visible")) {
             $('#dvSearchSQC').load('/Master/SearchSQC', function () {
                
             var data = $('#tblList').jqGrid("getGridParam", "postData");
                 if (!(data === undefined)) {
                     $('#ddlSearchStates').val(data.stateCode);
                     $('#ddlSearchStatus').val(data.status);
                 }
                 $('#dvSearchSQC').show('slow');
                 //console.log(data);
             });
         }

         $.unblockUI();
         //alert("listadmin");
         

     });
   
    //Method for jqgrid
    $('#tblList').jqGrid({

        url: '/Master/GetAdminSqcDetails',
        datatype: "local",
        mtype: "GET",
        //colNames: ['Quality Controller Name','State Name','Designation','Address','Phone Number 1','Phone Number 2', 'FAX', 'Mobile Number','Email','Remark','Status','Action'],
        //colModel: [                       
        //                    { name: 'QcName', index: 'QcName', height: 'auto', width: 200, align: "left", sortable: true },
        //                    { name: 'StateName', index: 'StateName', height: 'auto', width: 150, align: "left", sortable: true },
        //                    { name: 'QcDesignation', index: 'QcDesignation', height: 'auto', width: 180, align: "left", sortable: true },
        //                    { name: 'QcAddr1', index: 'QcAddr1', height: 'auto', width: 220, align: "left", sortable: true },
        //                    { name: 'StdNo1', index: 'StdNo1', height: 'auto', width: 100, align: "left", sortable: true },
        //                    { name: 'PhNo1', index: 'PhNo1', height: 'auto', width: 100, align: "left", sortable: true },
        //                    { name: 'Fax', index: 'Fax', height: 'auto', width: 80, align: "left", sortable: true },
        //                    { name: 'MobileNo', index: 'MobileNo', height: 'auto', width: 100, align: "left", sortable: true },
        //                    { name: 'Email', index: 'Email', height: 'auto', width: 120, align: "left", sortable: true },
        //                    { name: 'Remark', index: 'Remark', height: 'auto', width: 80, align: "left", sortable: true },
        //                    { name: 'Status', index: 'Status', height: 'auto', width: 80, align: "left", sortable: false },
        //                    { name: 'a', width: 100, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        //],

        colNames: ['Quality Controller Name','State Name','Department','Designation','Address','Contact Detail','Remark','Status','Action'],
        colModel: [                       
                            { name: 'QcName', index: 'QcName', height: 'auto', width: 200, align: "left", sortable: true },
                            { name: 'StateName', index: 'StateName', height: 'auto', width: 150, align: "left", sortable: true },
                            { name: 'Department', index: 'Department', height: 'auto', width: 150, align: "left", sortable: true },
                            { name: 'QcDesignation', index: 'QcDesignation', height: 'auto', width: 180, align: "left", sortable: true },
                            { name: 'QcAddr1', index: 'QcAddr1', height: 'auto', width: 220, align: "left", sortable: true },
                            { name: 'ContactDetail', index: 'Email', height: 'auto', width: 220, align: "left", sortable: false },
                            { name: 'Remark', index: 'Remark', height: 'auto', width: 80, align: "left", sortable: true },
                            { name: 'Status', index: 'Status', height: 'auto', width: 80, align: "left", sortable: true },
                            { name: 'a', width: 100, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],


        pager: jQuery('#divPager'),
        rowNum: 10,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'StateName,QcName',
        sortorder: "asc",
        caption: "Quality Controller List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        shrinkToFit: false,
        hidegrid: false,
        loadComplete: function (cellvalue, rowObject) {
            console.log(cellvalue);
            console.log(rowObject);
        }
    });
});

//function for edit functionality.
function editQualityController(urlParam) {

   
    $.ajax({
        url: "/Master/EditAdminSqc/" + urlParam,
        type: "GET",
        dataType: "html",
        async: false,
        catche: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
                if ($("#dvSearchSQC").is(":visible")) {
                $('#dvSearchSQC').hide('slow');
            }
                $('#btnCreateNewSQC').hide();
                $('#btnSearchViewSQC').show();

            $("#dvDetails").html(data);
            $("#dvDetails").show('slow');
            $("#ADMIN_QC_NAME").focus();
        },
        error: function (xht, ajaxOptions, throwError) {

            alert(xht.responseText);
        }

    });

}

//Function for delete functionality.
function deleteQualityController(urlParam) {
$("#alertMsg").hide(1000);
if (confirm("Are you sure you want to delete Quality Controller details?")) {
    var token = $('input[name=__RequestVerificationToken]').val();
    //headers['__RequestVerificationToken'] = token;
    //console.log(token);
        $.ajax({
            data: { "__RequestVerificationToken": token },
            url: "/Master/DeleteAdminSqc/" + urlParam,
            type: "POST",
            headers:token,
            dataType: "json",
             success: function (data) {
                if (data.success) {
                    alert(data.message);
                //if ($("#dvSearchSQC").is(":visible")) {
                //         $('#btnSearch').trigger('click');
                //    }
                //    else {
                //        $("#tblList").trigger('reloadGrid');
                    //    }

                    if ($("#dvDetails").is(":visible")) {
                        $('#dvDetails').hide('slow');
                        $('#btnSearchViewSQC').hide();
                        $('#btnCreateNewSQC').show();
                    }
                    if (!$("#dvSearchSQC").is(":visible")) {
                        $("#dvSearchSQC").show();
                        $("#tblList").trigger('reloadGrid');
                    }
                    else {
                        $("#tblList").trigger('reloadGrid');
                    }
                }
                else {
                    alert(data.message);
                }
             },
            error: function (xht, ajaxOptions, throwError)
            { alert(xht.responseText); }

        });
    }
    else {
        return false;
    }
}
function FormatColumn(cellvalue, options, rowObject) {
    console.log(cellvalue);
    return "<center><table><tr><td  style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-pencil' title='Edit SQC Details' onClick ='editQualityController(\"" + cellvalue.toString() + "\")'></span></td><td style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-trash' title='Delete SQC Details' onClick =deleteQualityController(\"" + cellvalue.toString() + "\");></span></td></tr></table></center>";
}

