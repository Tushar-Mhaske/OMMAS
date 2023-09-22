$(document).ready(function () {
    

    $('#ddlState').change(function () 
    {
        $("#ddlDistrict").val(0);
        $("#ddlBlock").val(0);
        $("#ddlVillage").val(0);
        $("#ddlHab").val(0);

        $("#ddlDistrict").empty();
        $("#ddlBlock").empty();
        $("#ddlVillage").empty();
        $("#ddlHab").empty();

        $("#ddlBlock").append("<option value='0'>Select Block</option>");
        $("#ddlVillage").append("<option value='0'>Select Village</option>");
        $("#ddlHab").append("<option value='0'>Select Habitation</option>");

        //if Block dropdown exists then only request to populate it.
        if ($("#ddlDistrict").length > 0) {
            $.ajax({
                url: '/CommonFilters/GetDistricts',
                type: 'POST',
                data: { selectedState: $("#ddlState").val(), value: Math.random() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                },
                error: function (err) {
                    alert("error " + err);
                }
            });
        }

    }); //ddlState Change ends here




    $('#ddlDistrict').change(function () 
    {
        $("#ddlBlock").val(0);
        $("#ddlVillage").val(0);
        $("#ddlHab").val(0);

        $("#ddlBlock").empty();
        $("#ddlVillage").empty();
        $("#ddlHab").empty();
        
        $("#ddlVillage").append("<option value='0'>Select Village</option>");
        $("#ddlHab").append("<option value='0'>Select Habitation</option>");

        //If dropdown of state is not rendered, then take stateCode from Session
        var selectedState = "";
        if ($("#ddlState").length == 0)
            selectedState = "AP";           // take stateCode from Session
        else
            selectedState = $("#ddlState").val();


        //if Block dropdown exists then only request to populate it.
        if ($("#ddlBlock").length > 0) {      

            $.ajax({
                url: '/CommonFilters/GetBlocks',
                type: 'POST',
                data: { selectedState: selectedState, selectedDistrict: $("#ddlDistrict").val(), value: Math.random() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlBlock").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                },
                error: function (err) {
                    alert("error " + err);
                }
            });
        }

    }); //ddlDistrict Change ends here




    $('#ddlBlock').change(function () 
    {
        $("#ddlVillage").val(0);
        $("#ddlHab").val(0);

        $("#ddlVillage").empty();
        $("#ddlHab").empty();

        $("#ddlHab").append("<option value='0'>Select Habitation</option>");

        //If dropdown of state,district is not rendered, then take stateCode,districtCode from Session
        var selectedState = "";
        var selectedDistrict = "";
        //if ($("#ddlState").length == 0)
        //    selectedState = "AP";           // take stateCode from Session
        //else
            selectedState = $("#ddlState").val();
        

        //if ($("#ddlDistrict").length == 0)
        //    selectedDistrict = 1;          // take districtCode from Session      
        //else                                        
            selectedDistrict = $("#ddlDistrict").val();


        //if Village dropdown exists then only request to populate it.
        if ($("#ddlVillage").length > 0) {      

            $.ajax({
                    url: '/CommonFilters/GetVillages',
                    type: 'POST',
                    data: { selectedState: selectedState, selectedDistrict: selectedDistrict, selectedBlock: $("#ddlBlock").val(), value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#ddlVillage").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (err) {
                        alert("error " + err);
                    }
            });
         }

    }); //ddlBlock Change ends here


    $('#ddlVillage').change(function () 
    {
        $("#ddlHab").val(0);
        $("#ddlHab").empty();

        //if ($("#ddlState").length == 0)
        //    selectedState = "AP";           // take stateCode from Session
        //else
            selectedState = $("#ddlState").val();

        //if ($("#ddlDistrict").length == 0)
        //    selectedDistrict = 1;          // take districtCode from Session      
        //else
            selectedDistrict = $("#ddlDistrict").val();

        //if ($("#ddlBlock").length == 0)
        //    selectedBlock = 1;          // take blockCode from Session      
        //else
            selectedBlock = $("#ddlBlock").val();



        //if Habitation dropdown exists then only request to populate it.
        if ($("#ddlHab").length > 0) {

            $.ajax({
                url: '/CommonFilters/GetHabs',
                type: 'POST',
                data: { selectedState: selectedState, selectedDistrict: selectedDistrict, selectedBlock: selectedBlock, selectedVillage: $("#ddlVillage").val(), value: Math.random() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlHab").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                },
                error: function (err) {
                    alert("error " + err);
                }
            });
        }

    }); //ddlVillage Change ends here


    $('#btnView').click(function () 
    {
        //if($("#ddlState").val() == "0")
        //{
        //      alert("Please select state");
        //}

        alert(" State : " + $("#ddlState option:selected").text() + "\n District : " + $("#ddlDistrict option:selected").text() + "\n Block : " + $("#ddlBlock option:selected").text() + "\n Village : " + $("#ddlVillage option:selected").text()
              + "\n Habitation : " + $("#ddlHab option:selected").text() + "\n Stream : " + $("#ddlStream option:selected").text() + "\n Batch : " + $("#ddlBatch option:selected").text() + "\n Year : " + $("#ddlYear option:selected").text() + "\n Month : " + $("#ddlMonth option:selected").text()
              + "\n Proposals : " + $("#ddlProposal option:selected").text());

    }); //btnView Click ends here


});