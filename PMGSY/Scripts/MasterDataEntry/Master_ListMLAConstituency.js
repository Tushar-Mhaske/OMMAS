
$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

$(document).ready(function () {

    $('#dvSearchMLAConstituency').load('/LocationMasterDataEntry/SearchMLAConstituency');  



    $('#btnCreateNew').click(function (e) {
        if ($("#dvSearchMLAConstituency").is(":visible")) {
            $('#dvSearchMLAConstituency').hide('slow');
        }

        if ($("#dvMapMLAConstituencyBlockDetails").is(":visible")) {
            $('#dvMapMLAConstituencyBlockDetails').hide('slow');
        }

        if ($("#dvMappedMLAConstituencyBlockDetails").is(":visible")) {
            $('#dvMappedMLAConstituencyBlockDetails').hide('slow');
        }


        if (!$("#dvMLAConstituencyDetails").is(":visible")) {
            $('#dvMLAConstituencyDetails').load("/LocationMasterDataEntry/CreateMLAConstituency");
            $('#dvMLAConstituencyDetails').show('slow');

            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
        }
        $('#tbMLAConstituencyList').jqGrid("setGridState", "visible");


    });

    $('#btnSearchView').click(function (e) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvMapMLAConstituencyBlockDetails").is(":visible")) {
            $('#dvMapMLAConstituencyBlockDetails').hide('slow');
        }

        if ($("#dvMappedMLAConstituencyBlockDetails").is(":visible")) {
            $('#dvMappedMLAConstituencyBlockDetails').hide('slow');
        }

        $('#dvMLAConstituencyDetails').hide('slow');

        $('#btnSearchView').hide();
        $('#btnCreateNew').show();

        //if ($("#dvMLAConstituencyDetails").is(":visible")) {
            

        //}

        $('#tbMLAConstituencyList').jqGrid("setGridState", "visible");

        if (!$("#dvSearchMLAConstituency").is(":visible")) {

            $('#dvSearchMLAConstituency').load('/LocationMasterDataEntry/SearchMLAConstituency', function () {

               // $('#tbMLAConstituencyList').trigger('reloadGrid');

                var data = $('#tbMLAConstituencyList').jqGrid("getGridParam", "postData");

                if (!(data === undefined)) {
                    $('#ddlSearchStates').val(data.stateCode);
                }

                $('#dvSearchMLAConstituency').show('slow');
            });
           
        }

        $.unblockUI();
       

    });
});

function FormatColumn(cellvalue, options, rowObject) {


    //return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='Edit State Details' onClick ='EditStateDetails(\"" + cellvalue.toString() + "\");'></span></td><td style='border-color:white'><span class='ui-icon ui-icon-trash' title='Delete State Details' onClick ='DeleteStateDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";

    //  return "<center><table><tr><td  style='border-color:white'><a href='#' title='Edit District Details' onClick ='EditDistrictDetails(\"" + cellvalue.toString() + "\");'>Edit</a></td></tr></table></center>";

    

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit MLA Constituency Details' onClick ='EditMLAConstituencyDetails(\"" + cellvalue.toString() + "\");'></span></td> <td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete MLA Constituency Details' onClick ='DeleteMLAConstituencyDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }
}

function FormatColumnMap(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none'><a href='#' title='Map Blocks' onClick ='MapMLAConstituency(\"" + cellvalue.toString() + "\");' >Map Blocks</a></td></tr></table></center>";
  
}

function FormatColumnMapped(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none'><a href='#' title='View Mapped Blocks' onClick ='MappedBlocks(\"" + cellvalue.toString() + "\");' >Mapped Blocks</a></td></tr></table></center>";

}

function EditMLAConstituencyDetails(parameter) {

    //alert(paramater);
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/LocationMasterDataEntry/EditMLAConstituency/" + parameter,
        type: "GET",
        async: false,
        cache: false,
        //data: $("form").serialize(),
        success: function (data) {

            // $("#mainDiv").html(data);

            if ($("#dvSearchMLAConstituency").is(":visible")) {
                $('#dvSearchMLAConstituency').hide('slow');
            }

            if ($("#dvMapMLAConstituencyBlockDetails").is(":visible")) {
                $('#dvMapMLAConstituencyBlockDetails').hide('slow');
            }

            if ($("#dvMappedMLAConstituencyBlockDetails").is(":visible")) {
                $('#dvMappedMLAConstituencyBlockDetails').hide('slow');
            }


            $("#dvMLAConstituencyDetails").html(data);
            $('#dvMLAConstituencyDetails').show('slow');
            $("#MAST_MLA_CONST_NAME").focus();

            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
            $('#trAddNewSearch').show();


            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });
}


function MapMLAConstituency(parameter) {
   // alert(parameter);

  
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

//    $('#btnSearchView').trigger('click');

    $.ajax({
        url: "/LocationMasterDataEntry/MapMLAConstituencyBlocks/" + parameter,
        type: "GET",
        async: false,
        cache: false,

        success: function (data) {
            
            $("#dvMapMLAConstituencyBlockDetails").html(data);
            $('#dvMapMLAConstituencyBlockDetails').show('slow');

            $('#btnSearch_Map').trigger('click');

            $('#trAddNewSearch').hide();
            $('#dvSearchMLAConstituency').hide();
            $('#dvMLAConstituencyDetails').hide();

            if ($("#dvMappedMLAConstituencyBlockDetails").is(":visible")) {
                $('#dvMappedMLAConstituencyBlockDetails').hide('slow');
            }

            //$('#tbMLAConstituencyList').setGridParam({ hidegrid: true });
            $('#tbMLAConstituencyList').jqGrid("setGridState", "hidden");
            
            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });


}


function DeleteMLAConstituencyDetails(urlparamater) {

    if (confirm("Are you sure you want to delete MLA constituency details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvMapMLAConstituencyBlockDetails").is(":visible")) {
            $('#dvMapMLAConstituencyBlockDetails').hide('slow');
        }

        if ($("#dvMappedMLAConstituencyBlockDetails").is(":visible")) {
            $('#dvMappedMLAConstituencyBlockDetails').hide('slow');
        }

        //$('#btnSearchView').trigger('click');
        //$('#trAddNewSearch').show();

        $.ajax({
            url: "/LocationMasterDataEntry/DeleteMLAConstituencyDetails/" + urlparamater,
            type: "GET",
            dataType: "json",
            success: function (data) {

                if (data.success) {
                    alert(data.message);

                    //if ($("#dvSearchMLAConstituency").is(":visible")) {

                    //    $('#btnSearch').trigger('click');

                    //}
                    //else {
                    //    $('#tbMLAConstituencyList').trigger('reloadGrid');
                    //}
                    if ($("#dvMapMLAConstituencyBlockDetails").is(":visible")) {
                        $('#dvMapMLAConstituencyBlockDetails').hide('slow');
                    }
                    if ($("#dvMappedMLAConstituencyBlockDetails").is(":visible")) {
                        $('#dvMappedMLAConstituencyBlockDetails').hide('slow');
                    }
                    $('#dvMLAConstituencyDetails').hide('slow');
                    $('#btnSearchView').hide();
                    $('#btnCreateNew').show();
                    if (!$("#dvSearchMLAConstituency").is(":visible")) {
                        $("#dvSearchMLAConstituency").show('slow');
                        $('#tbMLAConstituencyList').trigger('reloadGrid');
                    }
                    else {
                        $('#tbMLAConstituencyList').trigger('reloadGrid');
                    }

                }
                else {
                    alert(data.message);
                }

                $.unblockUI();

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }

        });

        if (!$("#dvMLAConstituencyDetails").is(':visible')) {
            $('#btnSearchView').trigger('click');
            $('#trAddNewSearch').show();
        }
    }
    else {
        return false;
    }
}


function MappedBlocks(parameter) {
    // alert(parameter);


    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

   
    $.ajax({
        url: "/LocationMasterDataEntry/MappedMLAConstituencyBlocks/" + parameter,
        type: "GET",
        async: false,
        cache: false,

        success: function (data) {

            $("#dvMappedMLAConstituencyBlockDetails").html(data);
            $('#dvMappedMLAConstituencyBlockDetails').show('slow');

            $('#trAddNewSearch').hide();
            $('#dvSearchMLAConstituency').hide();
            $('#dvMLAConstituencyDetails').hide();
            
            if ($("#dvMapMLAConstituencyBlockDetails").is(":visible")) {
                $('#dvMapMLAConstituencyBlockDetails').hide('slow');
            }

            $('#tbMLAConstituencyList').jqGrid("setGridState", "hidden");

            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });


}

function loadGrid() {
    jQuery("#tbMLAConstituencyList").jqGrid({
        url: '/LocationMasterDataEntry/GetMLAConstituencyDetailsList',
        datatype: "json",
        mtype: "POST",
        postData:{ stateCode: $('#ddlSearchStates option:selected').val() },
        colNames: ['MLA Constituency Name', 'State Name', 'Map Blocks', 'Mapped Blocks', 'Action'],
        colModel: [
                            { name: 'MLAConstituencyName', index: 'MLAConstituencyName', height: 'auto', width: 180, align: "left", sortable: true },
                            { name: 'StateName', index: 'StateName', height: 'auto', width: 150, sortable: true, align: "left" },
                            { name: 'Map', width: 100, sortable: false, resize: false, formatter: FormatColumnMap, align: "center" },
                            { name: 'Mapped', width: 100, sortable: false, resize: false, formatter: FormatColumnMapped, align: "center" },
                            { name: 'a', width: 60, sortable: false, resize: false, formatter: FormatColumn, align: "center" }
        ],
        pager: jQuery('#dvMLAConstituencyListPager'),
        rowNum: 15,
        // altRows: true,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'StateName,MLAConstituencyName',
        sortorder: "asc",
        caption: "MLA Constituency List",
        height: 'auto',
        // width: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: true,
        /*grouping: true,
        groupingView:
            {
                groupField: ['StateName'],
                groupColumnShow: [false],
                groupText: ['<b>State: {0}</b>'],
                //groupCollapse: true,
                groupOrder: ['asc']
            },*/
        loadComplete: function () {

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
        }

    })
}