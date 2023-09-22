
$(document).ready(function () {
        $.ajax({
            url: "/Master/SearchReasonType/",
            type: "GET",
            dataType: "html",
            success: function (data) {
                $("#dvSearchReason").html(data);
           },
            error: function (xhr, ajaxOptions, thrownError) {

                alert(xhr.responseText);
            }

        });

     $('#tblMasterReasonList').jqGrid({
            url: '/Master/GetMasterReasonList',
            datatype: 'json',
            mtype: "POST",
            colNames: ['Reason Name', 'Reason Type', 'Action'],
            colModel: [
             { name: 'MAST_REASON_NAME', index: 'MAST_REASON_NAME', height: 'auto', width: 400, align: "left", sortable: true },
             { name: 'MAST_REASON_TYPE', index: 'MAST_REASON_TYPE', height: 'auto', width: 140, align: "left",sortable:true},
             { name: 'a', width: 80, sortable: false, resize: false, formatter: FormatColumn, align: "left", sortable: false, search: false }
            ],
            pager: jQuery('#divPagerMasterReason'),
            rowNum: 15,
            rowList: [10, 15, 20, 30],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'MAST_REASON_NAME',
            sortorder: "asc",
            caption: 'Reason List',
            height: '100%',
            autowidth: true,
            grouping: false,
            groupingView:
            {
                groupField: ['MAST_REASON_TYPE'],
                groupColumnShow: [false],
                groupText: ['<b>Reason Type:{0}</b>'],
                groupCollapse: false,
                groupOrder: ['asc']
            },

            rownumbers: true,
            hidegrid: false,
        
            loadComplete: function () { },

            loadError: function (xhr, status, error) {

                if (xhr.responseText == "session expired") {

                    alert(xht.responseText);
                    window.location.href = "Login/login";
                }
                else {
                    alert("Invalid Data. Please Check and Try Again!");
                }
            }
        });
  

     $('#btnAddReason').click(function (e) {

            if ($("#dvSearchReason").is(":visible")) {
                $('#dvSearchReason').hide('slow');
            }

            if (!$("#dvReasonDetails").is(":visible")) {

                $("#dvReasonDetails").load("/Master/AddEditMasterReason/");

                $('#dvReasonDetails').show('slow');

                $('#btnAddReason').hide();
                $('#btnSearchView').show();
            }

     });

      $('#btnSearchView').click(function (e) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            if ($("#dvReasonDetails").is(":visible")) {
                $('#dvReasonDetails').hide('slow');

                $('#btnSearchView').hide();
                $('#btnAddReason').show();
           }

        if (!$("#dvSearchReason").is(":visible")) {

                $('#dvSearchReason').load('/Master/SearchReasonType/', function () {

                    $('#tblMasterReasonList').trigger('reloadGrid');
                    var data = $('#tblMasterReasonList').jqGrid("getGridParam", "postData");
                    if (!(data === undefined)) {

                        $('#ReasonType').val(data.ReasonType);
                    }
                    $('#dvSearchReason').show('slow');

                });
            }
            $.unblockUI();
        });

});

function FormatColumn(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Reason Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Reason Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}
   function editData(id) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Master/EditMasterReason/" + id,
            type: "GET",
            async: false,
            dataType: "html",
            catche: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if ($("#dvSearchReason").is(":visible")) {
                    $('#dvSearchReason').hide('slow');
                }
                $('#btnAddReason').hide();
                $('#btnSearchView').show();
                $("#dvReasonDetails").html(data);
                $("#dvReasonDetails").show();
                $("#MAST_REASON_NAME").focus();
            
            },
            error: function (xht, ajaxOptions, throwError) {
                alert(xht.responseText);
           }
        });
        $.unblockUI();
    }


    function deleteData(urlParam) {
        if (confirm("Are you sure you want to delete Reason details?")) {
            $.ajax({
                url: "/Master/DeleteMasterReason/" + urlParam,
                type: "POST",
                dataType: "json",
                success: function (data) {
                    if (data.success) {
                        alert(data.message);
                        //$("#tblMasterReasonList").trigger('reloadGrid');
                        //$("#dvReasonDetails").load("/Master/AddEditMasterReason");
                        if ($("#dvReasonDetails").is(":visible")) {
                            $('#dvReasonDetails').hide('slow');
                            $('#btnSearchView').hide();
                            $('#btnAddReason').show();
                        }

                        if (!$("#dvSearchReason").is(":visible")) {
                            $("#dvSearchReason").show('slow');
                        }
                        $("#tblMasterReasonList").trigger('reloadGrid');
                    }
                    else {
                        alert(data.message);
                    }
                    //$("#dvReasonDetails").load("/Master/AddEditMasterReason/");
                },
                error: function (xht, ajaxOptions, throwError)
                { alert(xht.responseText); }

            });
        }
        else {
            return false;
        }
}
