$(document).ready(function () {


    //LoadGrid("", "", "", "Load");


   




    $("#btnViewAllocateRoadsToNQM").click(function () {
        $.validator.unobtrusive.parse($('#frmAllocateRoadsToNqm'));
        if ($('#frmAllocateRoadsToNqm').valid()) {

            if ($("#ddlState").val() == "0") {
                alert("Please Select State");
                return;
            }

            if ($("#ddlMonth").val() == "0") {
                alert("Please Select Month");
                return;
            }

            if ($("#ddlYear").val() == "0") {
                alert("Please Select Year");
                return;
            }


            LoadGrid($("#ddlState").val(), $("#ddlMonth").val(), $("#ddlYear").val(), "");
        }

    });


});


function AssignRoadToNQM(AutoScheduledID) {

    
  var token = $('input[name=__RequestVerificationToken]').val();
    if (confirm("Are you sure you want to Assign Road to NQM?")) {

        $.ajax({
            type: "POST",
            dataType: "json",
            url: "/QualityMonitoring/AssignRoadToNQM/" + AutoScheduledID,
            //  data: { "AutoScheduledID": AutoScheduledID, "__RequestVerificationToken": token },
            data: { "__RequestVerificationToken": token },
            cache: false,
            error: function (xhr, status, error) {
                //unBlockPage();
                alert("Error in processing,Please try Again");
                return false;

            },
            success: function (data) {

                if (data.success) {
                    alert("Road Successfully Assign to NQM");
                    $('#tblAllocateRoadsToNQMGrid').trigger('reloadGrid');
                } else {
                    alert(data.ErrMessage);
                }


            
            }

        });


    }
    
}


function ViewAllocateroadToNQM(AUTO_SCHEDULE_ID) {
    //alert(AUTO_SCHEDULE_ID);
}

function LoadGrid(state, month, year, Load) {

    $("#tblAllocateRoadsToNQMGrid").jqGrid('GridUnload');
    blockPage();

    jQuery("#tblAllocateRoadsToNQMGrid").jqGrid({
        url: '/QualityMonitoring/AllocateRoadsToNQmList/',
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        rowNum: 15,
        postData: { 'state': state, 'month': month, 'year': year, 'Load': Load },
        rownumbers: true,
        autowidth: true,
        pginput: false,
        rowList: [15, 20, 30],
        //colNames: ['NQM Name', 'State Name', 'Schedule Month', 'Schedule Year', 'District1 Name', 'District2 Name', 'Assign Road', 'View Road'],
        colNames: ['NQM Name', 'State Name', 'Schedule Month', 'Schedule Year', 'District1 Name', 'District2 Name', 'Assign Road'],
        colModel: [

           {
               name: 'NQM Name',
               index: 'NQM Name',
               width: 90,
               align: "center",
               frozen: true

           },
           {
               name: 'State Name',
               index: 'State Name',
               width: 80,
               align: "center",
               frozen: true,

           },


             {
                 name: 'Schedule Month',
                 index: 'Schedule Month',
                 width: 40,
                 align: "center"

             },

           {
               name: 'Schedule Year',
               index: 'Schedule Year',
               width: 40,
               align: "center"

           },

             {
                 name: 'District1 Name',
                 index: 'District1 Name',
                 width: 40,
                 align: "Center"

             },
              {
                  name: 'District2 Name',
                  index: 'District2 Name',
                  width: 40,
                  align: "Center"

              },
               {
                   name: 'Assign Road',
                   index: 'Assign Road',
                   width: 40,
                   align: "Center"

               },

            //{
            //    name: 'View Road',
            //    index: 'View Road',
            //    width: 40,
            //    align: "Center"

            //}





        ],
        pager: "#divAllocateRoadsToNQMPager",
        viewrecords: true,
        loadComplete: function (xhr, st, err) {
            unblockPage();


            $("#tblAllocateRoadsToNQMGrid").jqGrid('setLabel', "rn", "Sr.</br> No");



            //$("#ddlState").change(function () {

            //    $("#dvAllocateRoadsToNQMGrid").hide();

            //});

            //$("#ddlMonth").change(function () {
            //    $("#dvAllocateRoadsToNQMGrid").hide();
            //});

            //$("#ddlYear").change(function () {
            //    $("#dvAllocateRoadsToNQMGrid").hide();
            //});



        },

        sortname: 'NQM Name',
        sortorder: "asc",
        caption: "Allocate Raods To NQM List"


    });




}
