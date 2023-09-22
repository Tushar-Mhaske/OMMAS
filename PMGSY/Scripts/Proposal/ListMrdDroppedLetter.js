
function LoadMrdDroppedLetterGrid() {
    if ($("#frmSearchMrdDropLetter").valid()) {
        jQuery("#tblMrdDroppedLetter").jqGrid({
            url: '/Proposal/GetMrdDroppedLetterList',
            datatype: "json",
            mtype: "POST",
            colNames: ['State', 'Sanctioned Year', 'Batch', 'Agency', 'Collaboration', 'Clerance Letter Number', 'Clearance Date', 'Final Revised Clearance Letter Number', 'Final Revised Clearance Date', 'Number of  Roads', 'Total Road Length (in KM.)', 'Number of  Bridges', 'Total Bridge  Length (in Mtr.)', 'Road MoRD share (in Cr.)', 'Road State share (in Cr.)',
                       'Total Road Sanctioned Amount (in Cr.)', 'Bridge MoRD share (in Cr.)', 'Bridge  State share (in Cr.)', 'Total Bridge Sanctioned Amount (in Cr.)', 'Total MoRD share (in Cr.)', 'Total State share (in Cr.)', 'Total Sanctioned Amount (in Cr.)',
                        'Hab >1000', 'Hab >500', 'Hab >250', 'Hab >100', 'Download File', 'Clearance Revision', 'Edit', 'Delete', 'View'], //26
            colModel: [

                        { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 80, align: "left", sortable: true },
                        { name: 'Year', index: 'Year', height: 'auto', width: 80, align: "center", sortable: true },
                        { name: 'Batch', index: 'Batch', height: 'auto', width: 60, align: "center", sortable: true },
                        { name: 'Agency', index: 'Agency', height: 'auto', width: 100, align: "center", sortable: true },
                        { name: 'Collaboration', index: 'Collaboration', height: 'auto', width: 100, align: "center", sortable: true },
                        { name: 'CleranceLetterNo', index: 'CleranceLetterNo', height: 'auto', width: 100, align: "center", sortable: false }, //new1
                        { name: 'MRD_CLEARANCE_DATE', index: 'MRD_CLEARANCE_DATE', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_CLEARANCE_REVISED_Number', index: 'MRD_CLEARANCE_REVISED_Number', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_CLEARANCE_REVISED_DATE', index: 'MRD_CLEARANCE_REVISED_DATE', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_TOTAL_ROADS', index: 'MRD_TOTAL_ROADS', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_TOTAL_ROAD_LENGTH', index: 'MRD_TOTAL_ROAD_LENGTH', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_TOTAL_LSB', index: 'MRD_TOTAL_LSB', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_TOTAL_LSB_LENGTH', index: 'MRD_TOTAL_LSB_LENGTH', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_ROAD_MORD_SHARE_AMT', index: 'MRD_ROAD_MORD_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_ROAD_STATE_SHARE_AMT', index: 'MRD_ROAD_STATE_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_ROAD_TOTAL_AMT', index: 'MRD_ROAD_TOTAL_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_LSB_MORD_SHARE_AMT', index: 'MRD_LSB_MORD_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_LSB_STATE_SHARE_AMT', index: 'MRD_LSB_STATE_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_LSB_TOTAL_AMT', index: 'MRD_LSB_TOTAL_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_TOTAL_MORD_SHARE_AMT', index: 'MRD_TOTAL_MORD_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_TOTAL_STATE_SHARE_AMT', index: 'MRD_TOTAL_STATE_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_TOTAL_SANCTIONED_AMT', index: 'MRD_TOTAL_SANCTIONED_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_HAB_1000', index: 'MRD_HAB_1000', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_HAB_500', index: 'MRD_HAB_500', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_HAB_250_ELIGIBLE', index: 'MRD_HAB_250_ELIGIBLE', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_HAB_100_ELIGIBLE', index: 'MRD_HAB_100_ELIGIBLE', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'DownLoad', index: 'DownLoad', height: 'auto', width: 50, align: "left", sortable: false, hidden: false },
                        { name: 'AddClearanceRevision', index: 'AddClearanceRevision', height: 'auto', width: 50, align: "left", sortable: false, hidden: false },
                        { name: 'Edit', index: 'Edit', height: 'auto', width: 50, align: "center", sortable: false, hidden: false },
                        { name: 'Delete', index: 'Delete', width: 50, resize: false },
                        { name: 'View', index: 'View', width: 50, resize: false }

            ],
            postData: { stateCode: $('#ddlMrdDropState option:selected').val(), agency: $('#ddlMrdDropAgency option:selected').val(), year: $('#ddlMrdDropPhaseYear option:selected').val(), batch: $('#ddlMrdDropBatch option:selected').val(), collaboration: $('#ddlMrdDropCollaboration option:selected').val() },
            pager: jQuery('#divMrdDroppedLetterpager'),
            rowNum: 10,
            rowList: [10, 15, 20, 30],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'MAST_STATE_NAME',
            sortorder: "asc",
            caption: "Clearance List",
            height: 'auto',
            autowidth: true,
            //width:'250',
            shrinkToFit: false,
            rownumbers: true,
            cmTemplate: { title: false },
            loadComplete: function () {
                //$("#tblMrdClearenceLetter").jqGrid('setGridWidth', $("#MrdClearenceLetterList").width(), true);



            },
            loadError: function (xhr, ststus, error) {

                if (xhr.responseText == "session expired") {
                    alert(xhr.responseText);
                    window.location.href = "/Login/Login";
                }
                else {
                    alert("Invalid data.Please check and Try again!")

                }
            },



        });
    }



}
