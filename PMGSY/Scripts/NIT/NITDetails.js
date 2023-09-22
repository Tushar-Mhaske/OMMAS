$(document).ready(function () {
    LoadNITDetails();

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

   
});


function LoadNITDetails() {

    jQuery("#tbNITList").jqGrid({
        url: '/NIT/GetNITDetailsList',
        datatype: "json",
        mtype: "POST",
        colNames: ['NIT Number', 'Form Issue Start Date', 'Form Issue End Date','Publication Date' ,'Rate','Add/View Road', 'Status', 'Edit', 'Delete'],
        colModel: [
                            { name: 'NITNumber', index: 'NITNumber', height: 'auto', width: 15, align: "left", sortable: true },
                            { name: 'IssueStartDate', index: 'IssueStartDate', height: 'auto', width: 10, sortable: true, },
                            { name: 'IssueEndDate', index: 'IssueEndDate', width: 10, sortable: true },
                            { name: 'PublicationDate', index: 'PublicationDate', width: 5, sortable: true },
                            { name: 'Rate', index: 'Rate', width: 5, sortable: true, align: 'center' },
                            { name: 'AddRoad', index: 'RoadLength', height: 'auto', width: 3, sortable: false, align: "center" },//formatter: FormatColumnAddRoad
                             { name: 'Status', index: 'Collaboration', width: 3, sortable: false },
                             { name: 'Edit', index: 'Edit', width: 2, sortable: false, formatter: FormatColumnEdit, align: "center" },
                            { name: 'Delete', index: 'Edit', width: 2, sortable: false, align: "center", formatter: FormatColumnDelete }
                          
        ],
        pager: jQuery('#dvNITListPager'),
        rowNum: 15,
        rowList: [10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "NIT List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: true,
        sortname: 'NITNumber,IssueStartDate,IssueEndDate',
        sortorder: "asc",
        loadComplete: function () {

            //var reccount = $('#tbNITList').getGridParam('reccount');
            //if (reccount > 0) {
            //    $('#dvNITListPager_left').html('[<b> Note</b>: 1. All Amounts are in Lakhs. 2.All Lengths are in Kms ]');
            //}
            var button = '<input type="button" id="btnAddNIT" name="btnAddNIT" value="Add NIT" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" title="Add NIT" tabindex="100" style="font-size:1em; margin-left:25px" onclick="AddNIT()" />'

            $('#dvNITListPager_left').html(button);

            if ($('#selectedRow').val() != '') {
                jQuery('#tbNITList').jqGrid('setSelection', $('#selectedRow').val());
            }

            //$('#btnAddNIT').removeClass();
            //$('#btnAddNIT').addClass('jqueryButton');
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                // alert(xhr.responseText);
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        },
        onSelectRow: function (rowid) {
            
            $('#selectedRow').val(rowid);
        }

    }); //end of grid
}

function FormatColumnEdit(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit NIT Details' onClick ='EditNITDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}

function FormatColumnDelete(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete NIT Details' onClick ='DeleteNITDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}


function FormatColumnAddRoad(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus' title='Add Road' onClick ='AddRoad(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    //else {
        

    //    return "<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-zoomin' title='View NIT Roads ' onClick ='ViewNITRoads(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    //}
    else
    {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}

function AddNIT() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/NIT/AddNITDetails",
        type: "GET",
        async: false,
        cache: false,
        success: function (data) {

           // $('#accordion').empty();
            $("#dvAddNIT").html(data);
            $('#accordion').show('slow');
            $('#dvAddNIT').show('slow');
            $('#dvNITDetails').show('slow');
            //if ($("#dvSearchProposedRoad").is(":visible")) {
            //    $('#dvSearchProposedRoad').hide('slow');
            //}


            $('#tbNITList').jqGrid("setGridState", "hidden");

            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });

}


function AddRoad(urlparameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/NIT/AddNITRoadDetails/" + urlparameter,
        type: "GET",
        async: false,
        cache: false,
        success: function (data) {

            //$('#accordion').empty();
            $("#dvAddNIT").html(data);
            $('#accordion').show('slow');
            $('#dvAddNIT').show('slow');

            //if ($("#dvSearchProposedRoad").is(":visible")) {
            //    $('#dvSearchProposedRoad').hide('slow');
            //}


            $('#tbNITList').jqGrid("setGridState", "hidden");

            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });


}

function ViewNITRoads(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/NIT/AddNITRoadDetails/" + urlparameter,
        type: "GET",
        async: false,
        cache: false,
        success: function (data) {

            //$('#accordion').empty();
            $("#dvAddNIT").html(data);
            $('#accordion').show('slow');
            $('#dvAddNIT').show('slow');

            $('#tblCreateNewRoad').hide();
            $('#dvNITRoadDetails').empty();
            $('#tblCreateNewRoad').empty();

            $('#tbNITList').jqGrid("setGridState", "hidden");

            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });

}





function EditNITDetails(urlparameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

   
    $.ajax({
        type: 'GET',
        url: '/NIT/EditNITDetails/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            $("#dvAddNIT").html(data);
            $('#accordion').show('slow');
            $('#dvAddNIT').show('slow');
            $('#dvNITDetails').show('slow');
            $('#tbNITList').jqGrid("setGridState", "hidden");
            $('#TEND_NIT_NUMBER').focus();

          

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}

function DeleteNITDetails(urlparameter) {
    if (confirm("Are you sure you want to delete NIT details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/NIT/DeleteNITDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);

                    $('#tbNITList').trigger('reloadGrid');

                    if ($('#dvAddNIT').is(':visible') && $('#EncryptedTendNITCode').val() != '') {
                        
                        $('#accordion').hide();
                        $("#dvAddNIT").load("/NIT/AddNITDetails" , function () {
                            $('#dvAddNIT').show('slow');
                            $('#dvNITDetails').show('slow');
                          
                        });
                        
                    }

                    $.unblockUI();
                }
                else {
                    alert(data.message);
                    $.unblockUI();
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
    }
    else {
        return false;
    }
}

function PublishNIT(urlparameter) {

    if (confirm("Are you sure you want to 'Publish' NIT ?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/NIT/PublishNIT/" + urlparameter,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $("#tbNITList").trigger('reloadGrid');

                    if ($('#btnUpdateNITDetails').is(':visible')) {

                        $("#btnCancelNITDetails").trigger('click');
                    }

                }
                else {
                    alert(data.message);
                }
                $.unblockUI();
            },
            error: function (xht, ajaxOptions, throwError) {
                alert(xht.responseText);
                $.unblockUI();
            }

        });
    }
    else {
        return false;
    }
}