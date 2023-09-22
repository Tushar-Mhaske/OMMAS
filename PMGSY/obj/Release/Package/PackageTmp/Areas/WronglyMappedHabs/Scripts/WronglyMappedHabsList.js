$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

  

    $('#tblList').jqGrid({
        url: '/WronglyMappedHabs/WronglyMappedHabitations/GetListofWronglyMappedHabs',
        datatype: 'json',
        mtype: "POST",                                                                                                                                                                                                       //,'Delete'  -->Removed by pradip 09-03-2017
        colNames: ['State Name', 'District Name', 'Block Name', 'Road Name', 'Sanctined Year', 'Batch', 'Package ID', 'Stage', 'Stage Phase', 'Upgrade / New Connectivity', 'Habitation Name', 'Habitation Population', 'Is Connected ?', 'Delete'],
        colModel: [
         { name: 'State', index: 'State', height: 'auto', width: 40, align: "center", sortable: true },
         { name: 'District', index: 'District', height: 'auto', width: 40, align: "center", sortable: true },
          { name: 'Block', index: 'Block', height: 'auto', width: 40, align: "center", sortable: true },
         { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', height: 'auto', width: 100, align: "center", sortable: true },
          { name: 'IMS_YEAR', index: 'IMS_YEAR', height: 'auto', width: 40, align: "center", sortable: true },
         { name: 'IMS_BATCH', index: 'IMS_BATCH', height: 'auto', width: 15, align: "center", sortable: true },
          { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 30, align: "center", sortable: true },
         { name: 'Stage', index: 'Stage', height: 'auto', width: 30, align: "center", sortable: false },
         { name: 'StagePhase', index: 'StagePhase', height: 'auto', width: 30, align: "center", sortable: false },
          { name: 'UpgradeConnect', index: 'UpgradeConnect', height: 'auto', width: 30, align: "center", sortable: false },
         { name: 'Habitation', index: 'Habitation', height: 'auto', width: 60, align: "center", sortable: false },

          { name: 'HabPop', index: 'HabPop', height: 'auto', width: 30, align: "right", sortable: false },
         { name: 'HabConnect', index: 'HabConnect', height: 'auto', width: 50, align: "center", sortable: false },
         //{ name: 'Delete', index: 'Delete', height: 'auto', width: 40, align: "left", sortable: false }
        { name: 'a', width: 20, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }  //<==Removed by pradip 09-03-2017
        ],
        pager: jQuery('#PagerList'),
        rowNum: 15,
        rowList: [25,50,75,100],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'IMS_ROAD_NAME',
        sortorder: "asc",
        caption: 'Wrongly Mapped Habitations List',
        height: '100%',
        rownumbers: true,
        hidegrid: false,
        autowidth: true,
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

   

});

function FormatColumn(cellvalue, options, rowObject) {
    if (cellvalue.toString() == "") {
        //return "<center><table><tr><td style='border:none;'><span>-</span></td><td style='border:none;'><span>-</span></td></tr></table></center>";
        return "<center><table><tr><td  style='aadadborder:none'><span class='ui-icon ui-icon-locked');'></span></td><td style='border:none'><span class='ui-icon ui-icon-locked');'></span></td></tr></table></center>";
    }

    return "<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Wrongly Mapped Hab Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}



//<td  style='border:none'> <span class='ui-icon ui-icon-pencil' title='Edit Agency Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span> </td>



function deleteData(urlParam) {
    if (confirm("Are you sure you want to delete Wrongly Mapped Habs?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/WronglyMappedHabs/WronglyMappedHabitations/DeleteHabs?id=" + urlParam,
            type: "POST",
            //dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $("#tblList").trigger('reloadGrid');
                   // $("#dvAgencyDetails").load("/Master/AddEditMasterAgency");
                    $.unblockUI();
                }
                else {
                    alert(data.message);
                    $.unblockUI();
                }
              //  $("#dvAgencyDetails").load("/Master/AddEditMasterAgency/");

            },
            error: function (xht, ajaxOptions, throwError)
            { alert(xht.responseText); $.unblockUI(); }

        });
    }
    else {
        return false;
    }
}
