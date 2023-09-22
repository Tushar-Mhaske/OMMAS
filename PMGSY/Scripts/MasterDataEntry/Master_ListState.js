

$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

$(document).ready(function () {

    $('#btnCreateNew').click(function (e) {
       // $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');
             
        if ($("#dvSearchState").is(":visible")) {
            $('#dvSearchState').hide('slow');
        }

        if (!$("#dvStateDetails").is(":visible")) {

            $("#dvStateDetails").load("/LocationMasterDataEntry/CreateState/");

            $('#dvStateDetails').show('slow');

            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
        //    setTimeout(function () {               
        //        $('#ddlStateUTs').val($('#ddlSearchStateUTs').val());
        //        $('#ddlStateTypes').val($('#ddlSearchStateTypes').val());
        //    }, 500);

        //    $('#btnCreateNew').hide();
        //    $('#btnSearchView').show();
        //}
        }
    });

    
    $.ajax({
        url: "/LocationMasterDataEntry/SearchState/",
        type: "GET",
        dataType: "html",
        success: function (data) {
            $("#dvSearchState").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {

            alert(xhr.responseText);
        }

    });
   



   

    $('#btnSearchView').click(function (e) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvStateDetails").is(":visible")) {
            $('#dvStateDetails').hide('slow');

            $('#btnSearchView').hide();
            $('#btnCreateNew').show();

        }

        if (!$("#dvSearchState").is(":visible")) {

            $('#dvSearchState').load('/LocationMasterDataEntry/SearchState/', function (e) {
                //$("#ddlSearchStateUTs option[value='0']").remove();
                //$("#ddlSearchStateTypes option[value='0']").remove();

                //$("#ddlSearchStateUTs").append("<option value='0' selected='true'> All </option>");
                //$("#ddlSearchStateTypes").append("<option value='0' selected='true'>All</option>");
             
                $('#tbStateList').trigger('reloadGrid');

                var data = $('#tbStateList').jqGrid("getGridParam", "postData");
                if (!(data === undefined)) {

                    if (!(data.StateUT === undefined)) {
                        $('#ddlSearchStateUTs').val(data.StateUT);
                    }

                    if (!(data.StateType === undefined)) {
                        $('#ddlSearchStateTypes').val(data.StateType);
                    }
                }
                $('#dvSearchState').show('slow');

            });
        }
        $.unblockUI();
    });

});
function LoadGrid() {

    jQuery("#tbStateList").jqGrid({
        url: '/LocationMasterDataEntry/GetStateDetailsList',
        datatype: "json",
        mtype: "POST",
        colNames: ['State Name','Short Name', 'State/UT', 'State Type', 'Census Code', 'Action'],//'State Short Name'
        colModel: [
                            { name: 'StateName', index: 'StateName', height: 'auto', width: 200, align: "left", sortable: true },
                            { name: 'ShortName', index: 'ShortName', width: 100, sortable: true,align:"left" },
                            { name: 'StateUT', index: 'StateUT', height: 'auto', width: 90, sortable: true, align: "left" },
                            { name: 'StateType', index: 'StateType', width: 90, sortable: true },
                            { name: 'NICStateCode', index: 'NICStateCode', width: 70, sortable: true, hidden: false },
                            { name: 'a', width: 60, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        postData: { StateUT: $('#ddlSearchStateUTs option:selected').val(), StateType: $('#ddlSearchStateTypes option:selected').val() },
        pager: jQuery('#dvStateListPager'),
        rowNum: 15,
        //altRows: true,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'StateUT,StateName',
        sortorder: "asc",
        caption: "State List",
        height: 'auto',
        //width: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        loadComplete: function (data) {
            //$('#tbStateList_rn').html('Sr. No');
            $.unblockUI();
            var recordCount = jQuery('#tbStateList').jqGrid('getGridParam', 'reccount');
            if (recordCount > 0) {

                var button = '<input type="button" id="btnFinalizeState" name="btnFinalizeState" value="Finalize" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" title="Finalize State" tabindex="200" style="font-size:1em; margin-left:25px" onclick="FinalizeState()" />'
                $('#dvStateListPager_left').html(button);

            }
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

    }); //end of state details grid

}

function FormatColumn(cellvalue, options, rowObject) {

    
    //return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='Edit State Details' onClick ='EditStateDetails(\"" + cellvalue.toString() + "\");'></span></td><td style='border-color:white'><span class='ui-icon ui-icon-trash' title='Delete State Details' onClick ='DeleteStateDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit State Details' onClick ='EditStateDetails(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete State Details' onClick ='DeleteStateDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

    //return "<center><table><tr><td  style='border-color:white'><a href='#' title='Edit State Details' onClick ='EditStateDetails(\"" + cellvalue.toString() + "\");'>Edit</a></td></tr></table></center>";

}

function EditStateDetails(urlparamater) {
    $("#btnCreateNew").hide();
    
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/LocationMasterDataEntry/Edit/" + urlparamater,
        type: "GET",
        dataType: "html",
        async: false,
        cache: false,
        //data: $("form").serialize(),
        success: function (data) {
            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');
            //$("#mainDiv").html(data);          

            if ($("#dvSearchState").is(":visible")) {
                $('#dvSearchState').hide('slow');
            }
            $('#btnAddAgency').hide();
            $('#btnSearchView').show();

            $("#dvStateDetails").html(data);
            $("#dvStateDetails").show();
            $("#MAST_STATE_NAME").focus();

            //for edit purpose
           // $('#btnSave').attr('value', 'Update');
            //$('#btnSave').attr('id', 'Update');


            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });
}


function DeleteStateDetails(urlparamater) {

    //alert(paramater);

    if (confirm("Are you sure you want to delete State/UT details?") ) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/LocationMasterDataEntry/DeleteStateDetails/" + urlparamater,
            type: "GET",
            dataType: "json",
            //data: $("form").serialize(),
            success: function (data) {

                if (data.success) {
                    alert(data.message); 
                    //$('#tbStateList').trigger('reloadGrid');
                    if ($("#dvStateDetails").is(":visible")) {
                        $('#dvStateDetails').hide('slow');
                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();
                    }
                    if (!$("#dvSearchState").is(":visible")) {
                        $('#dvSearchState').show('slow');
                        $('#tbStateList').trigger('reloadGrid');
                    }           
                    else {
                        $('#tbStateList').trigger('reloadGrid');

                    }
                }
                else  {
                    alert(data.message);
                }

                $.unblockUI();

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
function FinalizeState()
{
    var id = $('#tbStateList').jqGrid('getGridParam', 'selrow');

    if ($('#tbStateList').jqGrid('getGridParam', 'selrow')) {

        $.ajax({
            type: 'POST',
            url: '/LocationMasterDataEntry/FinalizeState/' + id,
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true)
                {
                    alert('State Finalized Successfully.');
                    $("#tbStateList").trigger('reloadGrid');
                }
                else if (data.success == false)
                {
                    alert('Error occurred while processing your request.');
                }
            },
            error: function () { }
        });
    }
    else {
        alert('Please select state to finalize.');
    }
}