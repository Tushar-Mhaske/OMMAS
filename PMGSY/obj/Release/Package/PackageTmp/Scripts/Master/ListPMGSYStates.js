$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
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

            $("#dvDetails").load("/Master/AddPmgsyStates", function () {
                $('#dvDetails').show();
                $('#btnCreateNewSQC').hide();
                $('#btnSearchViewSQC').show();
                $.unblockUI();
            });
        }
    });


    $('#tblList').jqGrid({

        url: '/Master/GetPmgsyStateDetails',
        datatype: "json",
        mtype: "GET",
        colNames: ['State Name', 'Effective Date'],
        colModel: [
                            { name: 'MAST_STATE_NAME', index: 'QcName', height: 'auto', width: 675, align: "left", sortable: true },

                            { name: 'PMGSY3_DATE', index: 'StateName', height: 'auto', width: 675, align: "center", sortable: true },
                  ],


        pager: jQuery('#divPager'),
        rowNum: 10,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'StateName',
        sortorder: "asc",
        caption: "PMGSY III States List",
        height: 'auto',
        width: 'auto',
        //autowidth: true,
        rownumbers: true,
        shrinkToFit: false,
        hidegrid: false,
        loadComplete: function (cellvalue, rowObject) {
        }
    });
});




