$(document).ready(function () {

    //$("#excel").click(function () {
    //    alert('excel');
    //});

    //$("#pdf").click(function () 
    //{
    //    alert('pdf');
    //});
    
    loadGrid('P');

    $("#ChartOfAccount").parents('div.ui-jqgrid-bdiv').css("max-height", "590px");

    $('input:radio').change(function () {

       // $('#mainDiv').load('/ChartOfAccount/GetChartOfAccounts/' + $(this).attr('value'))

        $("#ChartOfAccount").jqGrid().setGridParam
                  ({ url: '/ChartOfAccount/GetChartOfAccountsList/' + $(this).attr('value')  }).trigger("reloadGrid");
           

    })



 

});//end of the document.ready

function loadGrid(FundType)
{
    jQuery("#ChartOfAccount").jqGrid({

        url: '/ChartOfAccount/GetChartOfAccountsList/' + FundType,
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        //height: "1000px",
        rowNum: 1000,
        width:1200,
        //width: 'auto',
        //autoWidth:true,
        pginput: false,
        //rowList: [10, 20, 30],
        colNames: ['S.NO', 'parent Head', 'Head Of Account', 'Credit/Debit Balance', 'Major Head  Number', 'Account Head Number', 'Entry To be made by'],
        colModel: [
            {
                name: 'head_code_ref',
                index: 'head_code_ref',
                width: 60,
                align: "Center"

            },
            {
                name: 'parent_head_name',
                index: 'parent_head_name',
                width: 10,
               
            },
        {
            name: 'head_name',
            index: 'head_name',
            width: 420,
           
        },
            {
                name: 'credit_Debit',
                index: 'credit_Debit',
                width: 120,
                align: "Center"

            }, {
                name: 'major_head',
                index: 'major_head',
                width: 120,
                align: "Center"

            }, {
                name: 'account_head',
                index: 'account_head',
                width: 130,
                align: "Center"


            }, {
                name: 'Entry_to_be_made',
                index: 'Entry_to_be_made',
                width: 120,
                align: "Center",

            }
        ],
        pager: "#pager",
        viewrecords: true,
        sortname: 'parent_head_name',
        grouping: true,
        groupingView: {
            groupField: ['parent_head_name'],
            groupColumnShow: [false],
            groupText: ['<b>{0}</b>'],
            groupCollapse: true,
            groupOrder: ['desc']
        },
        caption: "CHART OF ACCOUNTS FOR PMGSY"
    });

   

}