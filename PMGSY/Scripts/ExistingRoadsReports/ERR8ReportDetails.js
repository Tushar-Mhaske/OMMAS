$(document).ready(function () {
    $('#ddSoilType_ERR8').change(function () {
        var roadType = $('#ddSoilType_ERR8').val();
        loadLevelWiseGrid(roadType);
    });

    $('#ddSoilType_ERR8').trigger('change');
   
    //$('#tblRptContents').bind('resize', function () {
    //    resizeJqGrid();
    //}).trigger('resize');


   

});

function loadLevelWiseGrid(soilType) {
    if ($("#hdnLevelId").val() == 6) //mord
    {
         ERR8StateReportListing(soilType);
    }
    else if ($("#hdnLevelId").val() == 4) //state
    {
        ERR8DistrictReportListing($("#MAST_STATE_CODE").val(), $("#STATE_NAME").val(), soilType);
    }
    else if ($("#hdnLevelId").val() == 5) //District
    {
        ERR8BlockReportListing($("#MAST_STATE_CODE").val(), $("#MAST_DISTRICT_CODE").val(), $("#BLOCK_NAME").val(), soilType);
    }

}

/*       STATE REPORT LISTING       */
function ERR8StateReportListing(soilType) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR8StateReportTable").jqGrid('GridUnload');
    $("#ERR8DistrictReportTable").jqGrid('GridUnload');
    $("#ERR8BlockReportTable").jqGrid('GridUnload');
    $("#ERR8FinalReportTable").jqGrid('GridUnload');

    $("#ERR8StateReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR8StateReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length'],
        colModel: [
            { name: 'StateName', width: 200, align: 'left',  height: 'auto', sortable: true },
            { name: 'Road_Hard', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Hard_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CHard', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CHard_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Soft', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Soft_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CSoft', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CSoft_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Sandy', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Sandy_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CSandy', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CSandy_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Red', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Red_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CRed', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CRed_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Other', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Other_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_COther', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_COther_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Total', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Total_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CTotal', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CTotal_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",   decimalPlaces: 3, defaulValue: "N.A" } }
        
        ],
        postData: { "SoilType": soilType },
        pager: $("#ERR8StateReportPager"),
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'StateName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: false,       
        height: '520',
        width: '1120',
        shrinkToFit: false ,
        viewrecords: true,
        caption: 'State  DRRP based on Soil type Details',
        loadComplete: function () {
            //Total of Columns
            var Road_HardT = $(this).jqGrid('getCol', 'Road_Hard', false, 'sum');
            var Road_Hard_LenT = $(this).jqGrid('getCol', 'Road_Hard_Len', false, 'sum');
            Road_Hard_LenT = parseFloat(Road_Hard_LenT).toFixed(3);
            var Road_CHardT = $(this).jqGrid('getCol', 'Road_CHard', false, 'sum');
            var Road_CHard_LenT = $(this).jqGrid('getCol', 'Road_CHard_Len', false, 'sum');
            Road_CHard_LenT = parseFloat(Road_CHard_LenT).toFixed(3);
            var Road_SoftT = $(this).jqGrid('getCol', 'Road_Soft', false, 'sum');
            var Road_Soft_LenT = $(this).jqGrid('getCol', 'Road_Soft_Len', false, 'sum');
            Road_Soft_LenT = parseFloat(Road_Soft_LenT).toFixed(3);
            var Road_CSoftT = $(this).jqGrid('getCol', 'Road_CSoft', false, 'sum');
            var Road_CSoft_LenT = $(this).jqGrid('getCol', 'Road_CSoft_Len', false, 'sum');
            Road_CSoft_LenT = parseFloat(Road_CSoft_LenT).toFixed(3);
            var Road_SandyT = $(this).jqGrid('getCol', 'Road_Sandy', false, 'sum');
            var Road_Sandy_LenT = $(this).jqGrid('getCol', 'Road_Sandy_Len', false, 'sum');
            Road_Sandy_LenT = parseFloat(Road_Sandy_LenT).toFixed(3);
            var Road_CSandyT = $(this).jqGrid('getCol', 'Road_CSandy', false, 'sum');
            var Road_CSandy_LenT = $(this).jqGrid('getCol', 'Road_CSandy_Len', false, 'sum');
            Road_CSandy_LenT = parseFloat(Road_CSandy_LenT).toFixed(3);
            var Road_RedT = $(this).jqGrid('getCol', 'Road_Red', false, 'sum');
            var Road_Red_LenT = $(this).jqGrid('getCol', 'Road_Red_Len', false, 'sum');
            Road_Red_LenT = parseFloat(Road_Red_LenT).toFixed(3);
            var Road_CRedT = $(this).jqGrid('getCol', 'Road_CRed', false, 'sum');
            var Road_CRed_LenT = $(this).jqGrid('getCol', 'Road_CRed_Len', false, 'sum');
            Road_CRed_LenT = parseFloat(Road_CRed_LenT).toFixed(3);
            var Road_OtherT = $(this).jqGrid('getCol', 'Road_Other', false, 'sum');
            var Road_Other_LenT = $(this).jqGrid('getCol', 'Road_Other_Len', false, 'sum');
            Road_Other_LenT = parseFloat(Road_Other_LenT).toFixed(3);
            var Road_COtherT = $(this).jqGrid('getCol', 'Road_COther', false, 'sum');
            var Road_COther_LenT = $(this).jqGrid('getCol', 'Road_COther_Len', false, 'sum');
            Road_COther_LenT = parseFloat(Road_COther_LenT).toFixed(3);
            var Road_TotalT = $(this).jqGrid('getCol', 'Road_Total', false, 'sum');
            var Road_Total_LenT = $(this).jqGrid('getCol', 'Road_Total_Len', false, 'sum');
            Road_Total_LenT = parseFloat(Road_Total_LenT).toFixed(3);
            var Road_CTotalT = $(this).jqGrid('getCol', 'Road_CTotal', false, 'sum');
            var Road_CTotal_LenT = $(this).jqGrid('getCol', 'Road_CTotal_Len', false, 'sum');
            Road_CTotal_LenT = parseFloat(Road_CTotal_LenT).toFixed(3);

            //

            $(this).jqGrid('footerData', 'set', { StateName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { Road_Hard: Road_HardT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Hard_Len: Road_Hard_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CHard: Road_CHardT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CHard_Len: Road_CHard_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Soft: Road_SoftT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Soft_Len: Road_Soft_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CSoft: Road_CSoftT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CSoft_Len: Road_CSoft_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Sandy: Road_SandyT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Sandy_Len: Road_Sandy_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CSandy: Road_CSandyT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CSandy_Len: Road_CSandy_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Red: Road_RedT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Red_Len: Road_Red_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CRed: Road_CRedT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CRed_Len: Road_CRed_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Other: Road_OtherT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Other_Len: Road_Other_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_COther: Road_COtherT }, true);
            $(this).jqGrid('footerData', 'set', { Road_COther_Len: Road_COther_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Total: Road_TotalT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Total_Len: Road_Total_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CTotal: Road_CTotalT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CTotal_Len: Road_CTotal_LenT }, true);
            $("#ERR8StateReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#ERR8StateReportTable_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }
        //resizeStop: function () {
        //    var $self = $(this),
        //        shrinkToFit = $self.jqGrid("getGridParam", "shrinkToFit");

        //    $self.jqGrid("setGridWidth", this.grid.newWidth, shrinkToFit);
        //    setHeaderWidth.call(this);
        //},
        //gridComplete: function () {
        //    var grid = this;
        //}
    });

    $("#ERR8StateReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          //{ startColumnName: 'Road_Hard', numberOfColumns: 2, titleText: '<em>DRRP</em>' },
          //{ startColumnName: 'Road_CHard', numberOfColumns: 2, titleText: '<em>Included in CN </em>' },
          //{ startColumnName: 'Road_Soft', numberOfColumns: 2, titleText: '<em>DRRP</em>' },
          //{ startColumnName: 'Road_CSoft', numberOfColumns: 2, titleText: '<em>Included in CN </em>' },
          //{ startColumnName: 'Road_Sandy', numberOfColumns: 2, titleText: '<em>DRRP</em>' },
          //{ startColumnName: 'Road_CSandy', numberOfColumns: 2, titleText: '<em>Included in CN </em>' },
          //{ startColumnName: 'Road_Red', numberOfColumns: 2, titleText: '<em>DRRP</em>' },
          //{ startColumnName: 'Road_CRed', numberOfColumns: 2, titleText: '<em>Included in CN </em>' },
          //{ startColumnName: 'Road_Other', numberOfColumns: 2, titleText: '<em>DRRP</em>' },
          //{ startColumnName: 'Road_COther', numberOfColumns: 2, titleText: '<em>Included in CN </em>' },
          //{ startColumnName: 'Road_Total', numberOfColumns: 2, titleText: '<em>DRRP</em>' },
          //{ startColumnName: 'Road_CTotal', numberOfColumns: 2, titleText: '<em>Included in CN </em>' }
          {
              startColumnName: 'Road_Hard', numberOfColumns: 4,
         titleText: '<table style="width:100%;border-spacing:0px"' +
                   '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Hard Oil</td>  </tr>' +
                   '<tr>' +
                       '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +
                      
                       '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                   '</tr>' +
                   '</table>'
          },

            {
                startColumnName: 'Road_Soft', numberOfColumns: 4,
                titleText: '<table style="width:100%;border-spacing:0px ;" ' +
                          '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;"  colspan="4">Soft Oil</td>  </tr>' +
                          '<tr>' +
                              '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                              '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                          '</tr>' +
                          '</table>'
            },

            {
                startColumnName: 'Road_Sandy', numberOfColumns: 4,
                titleText: '<table style="width:100%;border-spacing:0px ;" ' +
                        '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;"  colspan="4">Sandy Oil</td>  </tr>' +
                        '<tr>' +
                            '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                            '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                        '</tr>' +
                         '</table>'
            },
            {
                startColumnName: 'Road_Red', numberOfColumns: 4,
                titleText: '<table style="width:100%;border-spacing:0px ;" ' +
                            '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;"  colspan="4">Red Oil</td>  </tr>' +
                            '<tr>' +
                                '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                                '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                            '</tr>' +
                            '</table>'
            },
            {
                startColumnName: 'Road_Other', numberOfColumns: 4,
                titleText: '<table style="width:100%;border-spacing:0px ;" ' +
                            '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;"  colspan="4">Other</td>  </tr>' +
                            '<tr>' +
                                '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                                '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                            '</tr>' +
                            '</table>'
            },
              {
                  startColumnName: 'Road_Total', numberOfColumns: 4,
                  titleText: '<table style="width:100%;border-spacing:0px ;" ' +
                              '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;"  colspan="4">Total</td>  </tr>' +
                              '<tr>' +
                                  '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                                  '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                              '</tr>' +
                              '</table>'
              },

        ]
    });



    //$("th[title=ERR8ReportDetail]").removeAttr("title");
    //$("#h0").css({
    //    borderBottomWidth: "1px",
    //    // borderBottomColor: "none", // the color from jQuery UI which you use
    //  //  borderBottomColor: "Red",
    //    bordertop: "0px none",
    //    borderBottomStyle: "solid",
    //    padding: "4px 0 6px 0"
    //});
    //$("#h1").css({
    //    borderRightWidth: "1px",
    //    // borderRightColor: "#c5dbec", // the color from jQuery UI which you use
    //    borderRightColor: "inherit",
    //    borderRightStyle: "solid",
    //    padding: "4px 0 4px 0"
    //});
    //$("#h2").css({
    //    borderRightWidth: "1px",
    //    // borderRightColor: "#c5dbec", // the color from jQuery UI which you use
    //    borderRightColor: "inherit",
    //    borderRightStyle: "solid",
    //    padding: "4px 0 4px 0"
    //});
    //$("#h3").css({
    //    borderRightWidth: "1px",
    //    borderRightColor: "#c5dbec", // the color from jQuery UI which you use       
    //    borderRightStyle: "solid",
    //    padding: "4px 0 4px 0"
    //});
    //$("#h4").css({
    //    padding: "4px 0 4px 0"
    //});
    //setHeaderWidth.call(grid[0]);
}

//not use this function
var grid = $("#ERR8StateReportTable"),
  setHeaderWidth = function () {
      var $self = $(this),
          colModel = $self.jqGrid("getGridParam", "colModel"),
          cmByName = {},
          ths = this.grid.headers, // array with column headers
          cm,
          i,
          l = colModel.length;

      // save width of every column header in cmByName map
      // to make easy access there by name
      for (i = 0; i < l; i++) {
          cm = colModel[i];
          cmByName[cm.name] = $(ths[i].el).outerWidth();
      }
      // resize headers of additional columns based on the size of
      // the columns below the header
      //$("#h1").width(cmByName.No + cmByName.Date + cmByName.total - 1);
      $("#h1").width(cmByName.No + cmByName.Date - 1);
      //$("#h2").width(cmByName.in_Rs - 1);
      //$("#h3").width(cmByName.in_Rs - 1);
      //$("#h4").width(cmByName.in_Rs - 1);
  };

/**/

/*       DISTRICT REPORT LISTING       */
function ERR8DistrictReportListing(stateCode, stateName, soilType) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR8StateReportTable").jqGrid('setSelection', stateCode);
    $("#ERR8StateReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR8DistrictReportTable").jqGrid('GridUnload');
    $("#ERR8BlockReportTable").jqGrid('GridUnload');
    $("#ERR8FinalReportTable").jqGrid('GridUnload');

    $("#ERR8DistrictReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR8DistrictReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length'],
        colModel: [
            { name: 'DistrictName', width: 200, align: 'left',  height: 'auto', sortable: true },
             { name: 'Road_Hard', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Hard_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CHard', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CHard_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Soft', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Soft_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CSoft', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CSoft_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Sandy', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Sandy_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CSandy', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CSandy_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Red', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Red_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CRed', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CRed_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Other', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Other_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_COther', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_COther_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Total', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Total_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CTotal', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CTotal_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",   decimalPlaces: 3, defaulValue: "N.A" } }

        ],
        pager: $("#ERR8DistrictReportPager"),
        postData: { 'StateCode': stateCode, "SoilType": soilType },
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'DistrictName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: false,
        height: '460',
        width: '1120',
        shrinkToFit: false,
        viewrecords: true,
        caption: 'District  DRRP based on Soil type Details for ' + stateName,
        loadComplete: function () {
            //Total of Columns
            var Road_HardT = $(this).jqGrid('getCol', 'Road_Hard', false, 'sum');
            var Road_Hard_LenT = $(this).jqGrid('getCol', 'Road_Hard_Len', false, 'sum');
            Road_Hard_LenT = parseFloat(Road_Hard_LenT).toFixed(3);
            var Road_CHardT = $(this).jqGrid('getCol', 'Road_CHard', false, 'sum');
            var Road_CHard_LenT = $(this).jqGrid('getCol', 'Road_CHard_Len', false, 'sum');
            Road_CHard_LenT = parseFloat(Road_CHard_LenT).toFixed(3);
            var Road_SoftT = $(this).jqGrid('getCol', 'Road_Soft', false, 'sum');
            var Road_Soft_LenT = $(this).jqGrid('getCol', 'Road_Soft_Len', false, 'sum');
            Road_Soft_LenT = parseFloat(Road_Soft_LenT).toFixed(3);
            var Road_CSoftT = $(this).jqGrid('getCol', 'Road_CSoft', false, 'sum');
            var Road_CSoft_LenT = $(this).jqGrid('getCol', 'Road_CSoft_Len', false, 'sum');
            Road_CSoft_LenT = parseFloat(Road_CSoft_LenT).toFixed(3);
            var Road_SandyT = $(this).jqGrid('getCol', 'Road_Sandy', false, 'sum');
            var Road_Sandy_LenT = $(this).jqGrid('getCol', 'Road_Sandy_Len', false, 'sum');
            Road_Sandy_LenT = parseFloat(Road_Sandy_LenT).toFixed(3);
            var Road_CSandyT = $(this).jqGrid('getCol', 'Road_CSandy', false, 'sum');
            var Road_CSandy_LenT = $(this).jqGrid('getCol', 'Road_CSandy_Len', false, 'sum');
            Road_CSandy_LenT = parseFloat(Road_CSandy_LenT).toFixed(3);
            var Road_RedT = $(this).jqGrid('getCol', 'Road_Red', false, 'sum');
            var Road_Red_LenT = $(this).jqGrid('getCol', 'Road_Red_Len', false, 'sum');
            Road_Red_LenT = parseFloat(Road_Red_LenT).toFixed(3);
            var Road_CRedT = $(this).jqGrid('getCol', 'Road_CRed', false, 'sum');
            var Road_CRed_LenT = $(this).jqGrid('getCol', 'Road_CRed_Len', false, 'sum');
            Road_CRed_LenT = parseFloat(Road_CRed_LenT).toFixed(3);
            var Road_OtherT = $(this).jqGrid('getCol', 'Road_Other', false, 'sum');
            var Road_Other_LenT = $(this).jqGrid('getCol', 'Road_Other_Len', false, 'sum');
            Road_Other_LenT = parseFloat(Road_Other_LenT).toFixed(3);
            var Road_COtherT = $(this).jqGrid('getCol', 'Road_COther', false, 'sum');
            var Road_COther_LenT = $(this).jqGrid('getCol', 'Road_COther_Len', false, 'sum');
            Road_COther_LenT = parseFloat(Road_COther_LenT).toFixed(3);
            var Road_TotalT = $(this).jqGrid('getCol', 'Road_Total', false, 'sum');
            var Road_Total_LenT = $(this).jqGrid('getCol', 'Road_Total_Len', false, 'sum');
            Road_Total_LenT = parseFloat(Road_Total_LenT).toFixed(3);
            var Road_CTotalT = $(this).jqGrid('getCol', 'Road_CTotal', false, 'sum');
            var Road_CTotal_LenT = $(this).jqGrid('getCol', 'Road_CTotal_Len', false, 'sum');
            Road_CTotal_LenT = parseFloat(Road_CTotal_LenT).toFixed(3);
            //

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { Road_Hard: Road_HardT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Hard_Len: Road_Hard_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CHard: Road_CHardT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CHard_Len: Road_CHard_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Soft: Road_SoftT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Soft_Len: Road_Soft_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CSoft: Road_CSoftT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CSoft_Len: Road_CSoft_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Sandy: Road_SandyT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Sandy_Len: Road_Sandy_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CSandy: Road_CSandyT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CSandy_Len: Road_CSandy_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Red: Road_RedT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Red_Len: Road_Red_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CRed: Road_CRedT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CRed_Len: Road_CRed_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Other: Road_OtherT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Other_Len: Road_Other_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_COther: Road_COtherT }, true);
            $(this).jqGrid('footerData', 'set', { Road_COther_Len: Road_COther_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Total: Road_TotalT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Total_Len: Road_Total_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CTotal: Road_CTotalT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CTotal_Len: Road_CTotal_LenT }, true);
            $("#ERR8DistrictReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#ERR8DistrictReportTable_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }
    });

    $("#ERR8DistrictReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
        //{ startColumnName: 'Road_Hard', numberOfColumns: 2, titleText: '<em>DRRP</em>' },
        //{ startColumnName: 'Road_CHard', numberOfColumns: 2, titleText: '<em>Included in CN </em>' },
        //{ startColumnName: 'Road_Soft', numberOfColumns: 2, titleText: '<em>DRRP</em>' },
        //{ startColumnName: 'Road_CSoft', numberOfColumns: 2, titleText: '<em>Included in CN </em>' },
        //{ startColumnName: 'Road_Sandy', numberOfColumns: 2, titleText: '<em>DRRP</em>' },
        //{ startColumnName: 'Road_CSandy', numberOfColumns: 2, titleText: '<em>Included in CN </em>' },
        //{ startColumnName: 'Road_Red', numberOfColumns: 2, titleText: '<em>DRRP</em>' },
        //{ startColumnName: 'Road_CRed', numberOfColumns: 2, titleText: '<em>Included in CN </em>' },
        //{ startColumnName: 'Road_Other', numberOfColumns: 2, titleText: '<em>DRRP</em>' },
        //{ startColumnName: 'Road_COther', numberOfColumns: 2, titleText: '<em>Included in CN </em>' },
        //{ startColumnName: 'Road_Total', numberOfColumns: 2, titleText: '<em>DRRP</em>' },
        //{ startColumnName: 'Road_CTotal', numberOfColumns: 2, titleText: '<em>Included in CN </em>' }
        {
            startColumnName: 'Road_Hard', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                      '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Hard Oil</td>  </tr>' +
                      '<tr>' +
                          '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                          '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                      '</tr>' +
                      '</table>'
        },

          {
              startColumnName: 'Road_Soft', numberOfColumns: 4,
              titleText: '<table style="width:100%;border-spacing:0px ;" ' +
                        '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;"  colspan="4">Soft Oil</td>  </tr>' +
                        '<tr>' +
                            '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                            '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                        '</tr>' +
                        '</table>'
          },

          {
              startColumnName: 'Road_Sandy', numberOfColumns: 4,
              titleText: '<table style="width:100%;border-spacing:0px ;" ' +
                      '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;"  colspan="4">Sandy Oil</td>  </tr>' +
                      '<tr>' +
                          '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                          '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                      '</tr>' +
                       '</table>'
          },
          {
              startColumnName: 'Road_Red', numberOfColumns: 4,
              titleText: '<table style="width:100%;border-spacing:0px ;" ' +
                          '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;"  colspan="4">Red Oil</td>  </tr>' +
                          '<tr>' +
                              '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                              '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                          '</tr>' +
                          '</table>'
          },
          {
              startColumnName: 'Road_Other', numberOfColumns: 4,
              titleText: '<table style="width:100%;border-spacing:0px ;" ' +
                          '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;"  colspan="4">Other</td>  </tr>' +
                          '<tr>' +
                              '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                              '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                          '</tr>' +
                          '</table>'
          },
            {
                startColumnName: 'Road_Total', numberOfColumns: 4,
                titleText: '<table style="width:100%;border-spacing:0px ;" ' +
                            '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;"  colspan="4">Total</td>  </tr>' +
                            '<tr>' +
                                '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                                '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                            '</tr>' +
                            '</table>'
            },

        ]

    });
}
/**/

/*      BLOCK REPORT LISTING       */

function ERR8BlockReportListing(stateCode, districtCode, districtName, soilType) {

    var distname;
    if (districtName == '')
        distname = $("#DISTRICT_NAME").val();
    else
        distname = districtName;
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR8DistrictReportTable").jqGrid('setSelection', districtCode);
    $("#ERR8DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR8StateReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR8BlockReportTable").jqGrid('GridUnload');
    $("#ERR8FinalReportTable").jqGrid('GridUnload');

    $("#ERR8BlockReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR8BlockReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Block Name', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length', 'Number', 'Length'],
        colModel: [
            { name: 'BlockName', width: 200, align: 'left',  height: 'auto', sortable: true },
            { name: 'Road_Hard', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Hard_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CHard', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CHard_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Soft', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Soft_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CSoft', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CSoft_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Sandy', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Sandy_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CSandy', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CSandy_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Red', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Red_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CRed', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CRed_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Other', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Other_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_COther', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_COther_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Total', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_Total_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CTotal', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'Road_CTotal_Len', width: 70, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",",   decimalPlaces: 3, defaulValue: "N.A" } }

        ],
        pager: $("#ERR8BlockReportPager"),
        postData: { 'StateCode': stateCode, 'DistrictCode': districtCode ,"SoilType": soilType},
        rowNum: '2147483647',
        footerrow: true,
        pgbuttons: true,
        sortname: 'BlockName',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: false,
        height: '410',
        width: '1120',
        shrinkToFit: false,
        viewrecords: true,
        caption: 'Block  DRRP based on Soil type Details for ' + distname,
        loadComplete: function () {
            //Total of Columns
            var Road_HardT = $(this).jqGrid('getCol', 'Road_Hard', false, 'sum');
            var Road_Hard_LenT = $(this).jqGrid('getCol', 'Road_Hard_Len', false, 'sum');
            Road_Hard_LenT = parseFloat(Road_Hard_LenT).toFixed(3);
            var Road_CHardT = $(this).jqGrid('getCol', 'Road_CHard', false, 'sum');
            var Road_CHard_LenT = $(this).jqGrid('getCol', 'Road_CHard_Len', false, 'sum');
            Road_CHard_LenT = parseFloat(Road_CHard_LenT).toFixed(3);
            var Road_SoftT = $(this).jqGrid('getCol', 'Road_Soft', false, 'sum');
            var Road_Soft_LenT = $(this).jqGrid('getCol', 'Road_Soft_Len', false, 'sum');
            Road_Soft_LenT = parseFloat(Road_Soft_LenT).toFixed(3);
            var Road_CSoftT = $(this).jqGrid('getCol', 'Road_CSoft', false, 'sum');
            var Road_CSoft_LenT = $(this).jqGrid('getCol', 'Road_CSoft_Len', false, 'sum');
            Road_CSoft_LenT = parseFloat(Road_CSoft_LenT).toFixed(3);
            var Road_SandyT = $(this).jqGrid('getCol', 'Road_Sandy', false, 'sum');
            var Road_Sandy_LenT = $(this).jqGrid('getCol', 'Road_Sandy_Len', false, 'sum');
            Road_Sandy_LenT = parseFloat(Road_Sandy_LenT).toFixed(3);
            var Road_CSandyT = $(this).jqGrid('getCol', 'Road_CSandy', false, 'sum');
            var Road_CSandy_LenT = $(this).jqGrid('getCol', 'Road_CSandy_Len', false, 'sum');
            Road_CSandy_LenT = parseFloat(Road_CSandy_LenT).toFixed(3);
            var Road_RedT = $(this).jqGrid('getCol', 'Road_Red', false, 'sum');
            var Road_Red_LenT = $(this).jqGrid('getCol', 'Road_Red_Len', false, 'sum');
            Road_Red_LenT = parseFloat(Road_Red_LenT).toFixed(3);
            var Road_CRedT = $(this).jqGrid('getCol', 'Road_CRed', false, 'sum');
            var Road_CRed_LenT = $(this).jqGrid('getCol', 'Road_CRed_Len', false, 'sum');
            Road_CRed_LenT = parseFloat(Road_CRed_LenT).toFixed(3);
            var Road_OtherT = $(this).jqGrid('getCol', 'Road_Other', false, 'sum');
            var Road_Other_LenT = $(this).jqGrid('getCol', 'Road_Other_Len', false, 'sum');
            Road_Other_LenT = parseFloat(Road_Other_LenT).toFixed(3);
            var Road_COtherT = $(this).jqGrid('getCol', 'Road_COther', false, 'sum');
            var Road_COther_LenT = $(this).jqGrid('getCol', 'Road_COther_Len', false, 'sum');
            Road_COther_LenT = parseFloat(Road_COther_LenT).toFixed(3);
            var Road_TotalT = $(this).jqGrid('getCol', 'Road_Total', false, 'sum');
            var Road_Total_LenT = $(this).jqGrid('getCol', 'Road_Total_Len', false, 'sum');
            Road_Total_LenT = parseFloat(Road_Total_LenT).toFixed(3);
            var Road_CTotalT = $(this).jqGrid('getCol', 'Road_CTotal', false, 'sum');
            var Road_CTotal_LenT = $(this).jqGrid('getCol', 'Road_CTotal_Len', false, 'sum');
            Road_CTotal_LenT = parseFloat(Road_CTotal_LenT).toFixed(3);
            //

            $(this).jqGrid('footerData', 'set', { DistrictName: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { Road_Hard: Road_HardT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Hard_Len: Road_Hard_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CHard: Road_CHardT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CHard_Len: Road_CHard_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Soft: Road_SoftT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Soft_Len: Road_Soft_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CSoft: Road_CSoftT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CSoft_Len: Road_CSoft_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Sandy: Road_SandyT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Sandy_Len: Road_Sandy_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CSandy: Road_CSandyT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CSandy_Len: Road_CSandy_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Red: Road_RedT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Red_Len: Road_Red_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CRed: Road_CRedT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CRed_Len: Road_CRed_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Other: Road_OtherT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Other_Len: Road_Other_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_COther: Road_COtherT }, true);
            $(this).jqGrid('footerData', 'set', { Road_COther_Len: Road_COther_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Total: Road_TotalT }, true);
            $(this).jqGrid('footerData', 'set', { Road_Total_Len: Road_Total_LenT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CTotal: Road_CTotalT }, true);
            $(this).jqGrid('footerData', 'set', { Road_CTotal_Len: Road_CTotal_LenT }, true);
            $("#ERR8BlockReportPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Length in Kms.</font>");
            $('#ERR8BlockReportTable_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }
    });

    $("#ERR8BlockReportTable").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
        //{ startColumnName: 'Road_Hard', numberOfColumns: 2, titleText: '<em>DRRP</em>' },
        //{ startColumnName: 'Road_CHard', numberOfColumns: 2, titleText: '<em>Included in CN </em>' },
        //{ startColumnName: 'Road_Soft', numberOfColumns: 2, titleText: '<em>DRRP</em>' },
        //{ startColumnName: 'Road_CSoft', numberOfColumns: 2, titleText: '<em>Included in CN </em>' },
        //{ startColumnName: 'Road_Sandy', numberOfColumns: 2, titleText: '<em>DRRP</em>' },
        //{ startColumnName: 'Road_CSandy', numberOfColumns: 2, titleText: '<em>Included in CN </em>' },
        //{ startColumnName: 'Road_Red', numberOfColumns: 2, titleText: '<em>DRRP</em>' },
        //{ startColumnName: 'Road_CRed', numberOfColumns: 2, titleText: '<em>Included in CN </em>' },
        //{ startColumnName: 'Road_Other', numberOfColumns: 2, titleText: '<em>DRRP</em>' },
        //{ startColumnName: 'Road_COther', numberOfColumns: 2, titleText: '<em>Included in CN </em>' },
        //{ startColumnName: 'Road_Total', numberOfColumns: 2, titleText: '<em>DRRP</em>' },
        //{ startColumnName: 'Road_CTotal', numberOfColumns: 2, titleText: '<em>Included in CN </em>' }
        {
            startColumnName: 'Road_Hard', numberOfColumns: 4,
            titleText: '<table style="width:100%;border-spacing:0px"' +
                      '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;" colspan="4">Hard Oil</td>  </tr>' +
                      '<tr>' +
                          '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                          '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                      '</tr>' +
                      '</table>'
        },

          {
              startColumnName: 'Road_Soft', numberOfColumns: 4,
              titleText: '<table style="width:100%;border-spacing:0px ;" ' +
                        '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;"  colspan="4">Soft Oil</td>  </tr>' +
                        '<tr>' +
                            '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                            '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                        '</tr>' +
                        '</table>'
          },

          {
              startColumnName: 'Road_Sandy', numberOfColumns: 4,
              titleText: '<table style="width:100%;border-spacing:0px ;" ' +
                      '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;"  colspan="4">Sandy Oil</td>  </tr>' +
                      '<tr>' +
                          '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                          '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                      '</tr>' +
                       '</table>'
          },
          {
              startColumnName: 'Road_Red', numberOfColumns: 4,
              titleText: '<table style="width:100%;border-spacing:0px ;" ' +
                          '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;"  colspan="4">Red Oil</td>  </tr>' +
                          '<tr>' +
                              '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                              '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                          '</tr>' +
                          '</table>'
          },
          {
              startColumnName: 'Road_Other', numberOfColumns: 4,
              titleText: '<table style="width:100%;border-spacing:0px ;" ' +
                          '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;"  colspan="4">Other</td>  </tr>' +
                          '<tr>' +
                              '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                              '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                          '</tr>' +
                          '</table>'
          },
            {
                startColumnName: 'Road_Total', numberOfColumns: 4,
                titleText: '<table style="width:100%;border-spacing:0px ;" ' +
                            '<tr><td id="h0"  style="border-bottom-width: 1px; border-bottom-style: solid;"  colspan="4">Total</td>  </tr>' +
                            '<tr>' +
                                '<td id="h1" colspan="2" style="width: 22%; border-right-width: 1px; border-right-color: inherit; border-right-style: solid; padding: 4px 0px;">DRRP</td>' +

                                '<td id="h2" colspan="2" style="width: 22%;  border-right-color: inherit;  padding: 4px 0px;">Included in CN</td>' +
                            '</tr>' +
                            '</table>'
            },

        ]

    });
}

/*       FINAL BLOCK REPORT LISTING       */

function ERR8FinalReportListing(blockCode, districtCode, stateCode, blockName, soilType) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#ERR8BlockReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR8DistrictReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR8StateReportTable").jqGrid('setGridState', 'hidden');
    $("#ERR8BlockReportTable").jqGrid('setSelection', blockCode);
    $("#ERR8FinalReportTable").jqGrid('GridUnload');

    $("#ERR8FinalReportTable").jqGrid({
        url: '/ExistingRoadsReports/ERR8FinalReportListing?' + Math.random(),
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Road Code', 'Road Name', 'Road Category', 'Road Type', 'Length (Kms.)', 'Year of Construction', 'Included in Core Network (Y/N)', 'Habitations Status (Y/N)', 'Habitation Name', 'Population', 'Soil type', 'Terrain Type'],
        colModel: [
            { name: 'PlannedRoadNumber', width: 150, align: 'left',  height: 'auto', sortable: true },
            { name: 'PlannedRoadName', width: 250, align: 'left',  height: 'auto', sortable: false },
            { name: 'MAST_ROAD_CAT_CODE', width: 120, align: 'left',  height: 'auto', sortable: false },
            { name: 'MAST_ER_ROAD_TYPE', width: 150, align: 'left',  height: 'auto', sortable: false },
            { name: 'ROAD_LENGTH', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".",   decimalPlaces: 3, thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MAST_CONS_YEAR', width: 120, align: 'center',  height: 'auto', sortable: false },
            { name: 'MAST_CORE_NETWORK', width: 120, align: 'center',  height: 'auto', sortable: false },
            { name: 'MAST_HAB_STATUS', width: 120, align: 'center',  height: 'auto', sortable: false },
            { name: 'MAST_HAB_NAME', width: 120, align: 'left',  height: 'auto', sortable: false },
            { name: 'MAST_HAB_TOT_POP', width: 120, align: 'right',  height: 'auto', sortable: false, formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
            { name: 'MAST_SOIL_TYPE_NAME', width: 200, align: 'left',  height: 'auto', sortable: false },
            { name: 'MAST_TERRAIN_TYPE_NAME', width: 200, align: 'left',  height: 'auto', sortable: false }

        ],

        postData: { "BlockCode": blockCode, "StateCode": stateCode, "DistrictCode": districtCode, "SoilType": soilType },
        rowNum: '2147483647',
        pager: $("#ERR8FinalReportPager"),
        footerrow: true,
        pgbuttons: true,
        sortname: 'PlannedRoadNumber',
        sortorder: 'asc',
        rownumbers: true,
        autowidth: true,
        height: '420',
        viewrecords: true,
        caption: 'DRRP based on Soil type Details for ' + blockName,
        loadComplete: function () {
            //Set Footer Total
            var ROAD_LENGTHT = $(this).jqGrid('getCol', 'ROAD_LENGTH', false, 'sum');
            ROAD_LENGTHT = parseFloat(ROAD_LENGTHT).toFixed(3);
            var NoHablitisationT = $(this).jqGrid('getCol', 'MAST_HAB_TOT_POP', false, 'sum');        

            //
            $(this).jqGrid('footerData', 'set', { PlannedRoadNumber: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { ROAD_LENGTH: ROAD_LENGTHT });
            $(this).jqGrid('footerData', 'set', { MAST_HAB_TOT_POP: NoHablitisationT });
            $('#ERR8FinalReportTable_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }
    });
}
