$(document).ready(function () {

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $('#btnAddReply').click(function () {

        $.ajax({
            type: 'POST',
            url: '/OnlineFund/AddConditionReply',
            async: false,
            cache: false,
            data:$('#frmAddReply').serialize(),
            success: function (data) {
                if (data.Success == true) {
                    alert('Condition reply added successfully.');
                    CloseProposalDetails();
                    $("#tblstRequests").trigger('reloadGrid');
                }
                else {
                    alert('Error occurred while processing your request.');
                }
            },
            error: function () {
                alert('Error occurred while processing your request.');
            }
        });

    });

    LoadConditionDetails();

});
function LoadConditionDetails() {
    $("#tblConditionDetails").jqGrid('GridUnload');

    jQuery("#tblConditionDetails").jqGrid({
        url: '/OnlineFund/GetConditionImposedList',
        datatype: "json",
        mtype: "POST",
        postData: { RequestId: $("#REQUEST_ID").val() },
        colNames: ['Condition Imposed By', 'Condition Date', 'Condition Description'],
        colModel: [

                            { name: 'Imposer', index: 'Imposer', height: 'auto', width: 400, align: "left", search: false },
                            { name: 'ConditionDate', index: 'ConditionDate', height: 'auto', width: 300, align: "center", search: false },
                            { name: 'Desc', index: 'Desc', height: 'auto', width: 400, align: "center", search: false },

        ],
        pager: jQuery('#pgConditionDetails'),
        rowNum: 10,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'Imposer',
        sortorder: "desc",
        caption: 'Condition List',
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        autowidth: false,
        shrinkToFit: false,
        cmTemplate: { title: false },
        grouping: false,
        loadComplete: function (data) {
            $("#tblConditionDetails").jqGrid('setGridWidth', $("#tblConditionDetails").width() - 100, true);
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }
    });
}