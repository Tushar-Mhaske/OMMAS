$(document).ready(function () {
        $.ajax({
            url: "/Master/SearchStreamType/",
            type: "GET",
            dataType: "html",
            success: function (data) {
                $("#dvSearchStreams").html(data);
                $('#btnSearch').trigger('click');


            },
            error: function (xhr, ajaxOptions, thrownError) {

                alert(xhr.responseText);
            }

        });
     
        $('#tblList').jqGrid({

            url: '/Master/GetMasterStreamsList',
            datatype: "json",
            mtype: "POST",
            colNames: ['Stream Name','Stream Type' ,'Action'],
            colModel: [
                                { name: 'StreamsName', index: 'StreamsName', height: 'auto', width: 100, align: "left",sortable:true },
                                { name: 'StreamsType', index: 'StreamsType', height: 'auto', width: 70, align: "left",sortable:true },
                                { name: 'a', width: 60, sortable: false, resize: false, formatter: FormatColumn, align: "center" }
            ],
            pager: jQuery('#divPager'),
            rowNum: 15,
            rowList: [10, 15, 20, 30],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'StreamsType',
            sortorder: "asc",
            caption: "Stream List", 
            height: '100%', 
            autowidth:true,
            rownumbers: true,
            shrinkToFit: true,
            hidegrid: false,
       
            loadComplete: function () {

            },
  });
    $('#btnCreateNew').click(function (e) {
        if ($("#dvSearchStreams").is(":visible")) {
            $('#dvSearchStreams').hide('slow');
        }
        if (!$("#dvDetails").is(":visible")) {
            $("#dvDetails").load("/Master/AddtMasterStreams/");
            $('#dvDetails').show('slow');
            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
        }
    });

    $('#btnSearchView').click(function (e) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        if ($("#dvDetails").is(":visible")) {
            $('#dvDetails').hide('slow');
            $('#btnSearchView').hide();
            $('#btnCreateNew').show();

        }
        if (!$("#dvSearchStreams").is(":visible")) {
            $('#dvSearchStreams').load('/Master/SearchStreamType/', function () {
                $('#tblList').trigger('reloadGrid');
                var data = $('#tblList').jqGrid("getGridParam", "postData");
                if (!(data === undefined)) {
                    $('#StreamType').val(data.StreamType);
                }
                $('#dvSearchStreams').show('slow');

            });
        }
        $.unblockUI();
    });

 
});

function editMasterStream(urlParam) {
  

    $.ajax({
        url: "/Master/EditMasterStream/" + urlParam,
        type: "GET",
        dataType: "html",
        async: false,
        catche: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if ($("#dvSearchStreams").is(":visible")) {
                $('#dvSearchStreams').hide('slow');
            }
            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
            $("#dvDetails").html(data);
            $("#dvDetails").show();
            $("#MAST_STREAM_NAME").focus();
        },
        error: function (xht, ajaxOptions, throwError) {

            alert(xht.responseText);
        }

    });

}
function deleteMasterStream(urlParam) {

   
    $("#alertMsg").hide(1000);
    if (confirm("Are you sure you want to delete Stream details?")) {
        $.ajax({

            url: "/Master/DeleteMasterStreams/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {

                    alert(data.message);
                    //if ($("#dvSearchStreams").is(":visible")) {
                    //    $('#btnSearch').trigger('click');
                    //}
                    //else {
                    //    $("#dvDetails").load("/Master/AddtMasterStreams/");
                    //    $('#tblList').trigger('reloadGrid');
                    //}
                    if ($("#dvDetails").is(":visible")) {
                        $('#dvDetails').hide('slow');
                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();

                    }
                    if (!$("#dvSearchStreams").is(":visible")) {
                        $("#dvSearchStreams").show('slow')
                    }
                    $('#tblList').trigger('reloadGrid');
                }
               
                else {
                    alert(data.message);
                }


            },
            error: function (xht, ajaxOptions, throwError)
            { alert(""); alert(xht.responseText); }

        });
    }
    else {
        return false;
    }
}


function FormatColumn(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-pencil' title='Edit Stream Details' onClick ='editMasterStream(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-trash' title='Delete Stream Details' onClick =deleteMasterStream(\"" + cellvalue.toString() + "\");></span></td></tr></table></center>";

}