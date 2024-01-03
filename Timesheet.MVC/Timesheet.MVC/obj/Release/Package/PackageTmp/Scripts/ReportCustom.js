/*Global Variables*/

var SummaryData=[];
var parameters = {
    edit: false,
    editicon: "ui-icon-pencil",
    add: false,
    del: false,
    addicon: "ui-icon-plus",
    cancel: true,
    cancelicon: "ui-icon-cancel"

};
var Approvebtnparameter = { caption: "", buttonicon: "ui-icon-check", onClickButton: ApproveAll, position: "last", title: "", cursor: "pointer" };
var Rejectbtnparameter = { caption: "", buttonicon: "ui-icon-closethick", onClickButton: RejectAll, position: "last", title: "", cursor: "pointer" };
var Savebtnparameter = { caption: "", buttonicon: "ui-icon-disk", onClickButton: SaveData, position: "last", title: "", cursor: "pointer" };
$(document).ready(function () {
   

    LoadSummaryGrid();
    LoadDetailGridEditable();
    LoadCrProjectBasedGrid();
    $('form#searchForm').trigger('submit');
    
    $("#LinkExport").click(function () {
        var formData = {
            "ResourceId": $("#ResourceId").val(),
            "MonthId": $("#ddlMonthId").val(),
            "WeekId": $("#ddlWeekId").val(),
            "statusId": $("#statusId").prop("checked")
        };
        var noOfRes = 0;

        $.post('/Report/GetNoOfResource/', formData, function (data, status, xhr) {
            if (status == "success") {

                var noOfResource = parseInt(data);
                
                for (i = 0; i < noOfResource; i++) {
                    //Commented By Sasmita to call new DownloadExcel function
                   // window.open("/report/excel?" + jQuery.param(formData) + "&noOfResource=" + i);
                    window.open("/report/DownloadExcel?" + jQuery.param(formData) + "&noOfResource=" + i);                 

                }
            }
        });
  
    });

    //Added By Sasmita | Monthly Export | Multiple Worksheets

    $("#LinkMonthlyExport").click(function () {   
      
        var formData = {
            "MonthId": $("#ddlMonthId").val(),
        };

        window.open("/report/MonthlyExport?" + jQuery.param(formData));

    });


});
function LoadSummaryGrid()
{
    
    jQuery("#listSummary").jqGrid({
        datatype: "local",
        data:SummaryData,
        
        altRows: false,
        colNames: ["CrId", "ProjectId", "Project Name", "Cr#", "Billable(Hrs)", "Billable(Days)", "NonBillable(Days)", "Approved Cost"],
        colModel: [
            
            { index: 'crId', name: "crId", width: 120, hidden: true},
            { index: 'ProjectId', name: "ProjectId", width: 150, hidden: true},
            { index: 'ProjectName', name: "ProjectName", width: 200 },
            { index: 'CrNumber', name: "CrNumber", width: 60 },
            
            { index: 'BilledHrs', name: "BilledHrs", width: 70, formatter: 'number' ,formatoptions: { decimalSeparator: '.', decimalPlaces: 3}},
            { index: 'BilledDays', name: "BilledDays", width: 80, formatter: 'number' ,formatoptions: { decimalSeparator: '.', decimalPlaces: 3}},
            { index: 'UnBilledDays', name: "UnBilledDays", width: 50, formatter: 'number', formatoptions: { decimalSeparator: '.', decimalPlaces: 3 } },
            { index: 'ApprovedCost', name: "ApprovedCost", width: 50, formatter: 'number', formatoptions: { decimalSeparator: '.', decimalPlaces: 3 } }
           // { index: 'Detail', name: "Detail", editable: true,width: 25  ,align: 'center', formatter: DetailsFormatter,title:false },
           //{ index: 'CrDB', name: "CrDB", editable: true, width: 25, align: 'center', formatter: CrDbFormatter, title: false },
        ],
        pager: "#listSummarypager",
        rowNum: 30,
        loadonce: true,
        width: 650,
        rowList: [30, 20, 10],
        multiSort: true,
        sortname: "ProjectName,CrNumber",
        sortorder: "Asc",
        height: "200px",
        multiselect: false,
        viewrecords: true,
        gridview: true,
        autoencode: true,
        caption: "",
        footerrow: true,
        onSelectRow: function(id){
            var obj = $('#listSummary').jqGrid('getRowData', id);
            GetProjectCrLevelDashboard(obj.crId, obj.ProjectId, $("#ddlMonthId").val());
            fetchdetails(obj.crId ,obj.ProjectId );

            
        },
        gridComplete: function () {

            var BillableHrs = 0.00;
            var BillableDays = 0.00;
            var NonBillableDays = 0.00;
            var TotalApprovedCost = 0.00;
            var rows = $("#listSummary").getDataIDs();

            for (var i = 0; i < rows.length; i++) {
                
                BillableHrs = BillableHrs + parseFloat($("#listSummary").getCell(rows[i], "BilledHrs"));
                BillableDays = BillableDays + parseFloat($("#listSummary").getCell(rows[i], "BilledDays"));
                NonBillableDays = NonBillableDays + parseFloat($("#listSummary").getCell(rows[i], "UnBilledDays"));
                TotalApprovedCost = TotalApprovedCost + parseFloat($("#listSummary").getCell(rows[i], "ApprovedCost"));
            }
            $("#listSummary").jqGrid('footerData', 'set',
                        { 'ProjectName': 'Total', 'BilledHrs': BillableHrs, 'BilledDays': BillableDays, 'UnBilledDays': NonBillableDays , 'ApprovedCost' : TotalApprovedCost   },
                false);



        }

    });
    $('#listSummary').jqGrid('navGrid', '#listSummarypager',
                parameters).jqGrid('navButtonAdd', "#listSummarypager", Approvebtnparameter).jqGrid('navButtonAdd', "#listSummarypager", Rejectbtnparameter);

}
function checkBox(obj) {
   // $('.check').prop('checked', obj.checked);
}

function LoadDetailGridEditable() {    
   
    $("#list").jqGrid("clearGridData", true).trigger("reloadGrid");
    jQuery("#list").jqGrid({
        datatype: "local",
        autowidth: true,

        altRows: false,
        colNames: ["Id", "Approver Status", "Date", "Resource Name", "ProjectId", "Project Name","CrNumberId", "Cr#", "ActivityId", "Activity", "Sub Activity",
                    "Entered", "Approved", "Billable", "Status"],
        colModel: [
            { index: 'n_id', name: "n_id", width: 1, hidden: true, key: true },
            { index: 'n_status', name: "n_status", width: 1, hidden: true },
            { index: 'ActivityDate', name: "ActivityDate", width: 75},
            { index: 'ResourceName', name: "ResourceName", width: 115, hidden: false },
            { index: 'ProjectId', name: "ProjectId", width: 150, hidden: true },
            {
                index: 'ProjectName', name: "ProjectName", width: 290, edittype: 'select', editable: true, required: true, editoptions: {
                    dataUrl: '/report/GetProjectList',
                    buildSelect: function (data) {
                        var response = jQuery.parseJSON(data);
                        var s = "<select>";
                        jQuery.each(response, function(i, item) {
                            s += '<option value="' + response[i].Value + '">' + response[i].Text + '</option>';
                        })
                        return s + '</select>';
                    },
                    dataEvents: [
                    {
                        type: 'change',
                        fn: function (e) {

                            var id = e.target.id.split("_");
                            $("#list").jqGrid('setCell', id[0], "ProjectId", $(this).val());
                        }
                    }]
                }
            },
            { index: 'CrNumberId', name: "CrNumberId", width: 150, hidden: true },
            {
                index: 'CrNumber', name: "CrNumber", width: 100, edittype: 'select', editable: true, required: true, editoptions: {
                    dataUrl: '/report/GetCrList',
                    buildSelect: function (data) {                       
                        var response = jQuery.parseJSON(data);
                        var s = "<select>";
                        jQuery.each(response, function (i, item) {
                            s += '<option value="' + response[i].Value + '">' + response[i].Text + '</option>';
                           
                        })
                        return s + '</select>';
                    },
                    dataEvents: [
                    {
                        type: 'change',
                            fn: function (e) {                                
                                var id = e.target.id.split("_");                               
                                $("#list").jqGrid('setCell', id[0], "CrNumberId", $(this).val());
                        }
                    }]
                }
            },
            //{ index: 'CrNumber', name: "CrNumber", width: 60 },
             { index: 'ActivityId', name: "ActivityId", width: 150, hidden: true },
                    {
                        index: 'Activity', name: "Activity", width: 200, edittype: 'select', editable: true, required: true, editoptions: {
                            dataUrl: '/report/GetActivityList',
                            buildSelect: function (data) {
                                var response = jQuery.parseJSON(data);
                                var s = "<select>";
                                jQuery.each(response, function (i, item) {
                                    s += '<option value="' + response[i].Value + '">' + response[i].Text + '</option>';
                                })
                                return s + '</select>';
                            },
                            dataEvents: [
                            {
                                type: 'change',
                                fn: function (e) {

                                    var id = e.target.id.split("_");
                                    $("#list").jqGrid('setCell', id[0], "ActivityId", $(this).val());
                                }
                            }]
                        }
                    },
            //{ index: 'Activity', name: "Activity", width: 150 },
            { index: 'SubActivity', name: "SubActivity", width: 180, editable: true, required: true},
            { index: 'Efforts', name: "Efforts", width: 60, align: "right", edittype: 'text', editrules: { align: 'right', required: true, number: true, }, formatoptions: { decimalSeparator: '.', decimalPlaces: 1 } },
            { index: 'ActualEfforts', name: "ActualEfforts", width: 65, align: "right", required: true , editable: true}, 
            { index: 'Billable', name: "Billable", width: 50, edittype: 'select', editable: true,required: true, editoptions: { value: "1:Yes;0:No" } },
            { index: 'Status', name: "Status", width: 65 },
           


        ],
        pager: "#listpager",
        rowNum: 30,
        cache: false,
       
        rowList: [30, 20, 10],
        sortname: "ActivityDate",
        viewrecords: true,
        autoencode: true,
        caption: "",
        height: 300,
        editurl: 'clientArray',
        footerrow: true,
        multiselect: true,
        onSelectRow: function (id) {
            if (id && id !== lastSel) {
                jQuery('#list').restoreRow(lastSel);
                lastSel = id;
            }
             jQuery('#list').editRow(id, true);
        },
        
        gridComplete: function () {

            var colsum = 0.00;
            var TotalHrs = 0.00;
            var rows = $("#list").getDataIDs();
            for (var i = 0; i < rows.length; i++) {

                var Billable = $("#list").getCell(rows[i], "Billable");

                TotalHrs = TotalHrs + parseFloat($("#list").getCell(rows[i], "Efforts"));
                if (Billable == "Yes") {
                    $("#list").jqGrid('setRowData', rows[i], false, { color: '#000', background: '#78BFAC' });
                    colsum = colsum + parseFloat($("#list").getCell(rows[i], "Efforts"));

                }
                var n_status = parseInt($("#list").getCell(rows[i], "n_status"));
                if (n_status == -1) {
                    $("#list").jqGrid('setRowData', rows[i], false, { color: '#FF0000'});
                }
                jQuery('#list').editRow(rows[i], true);
                //}


            }
            $("#list").jqGrid('footerData', 'set', { 'CrNumber': TotalHrs.toFixed(1), 'ProjectName': 'Total Hrs', 'SubActivity': 'Total Billable Hrs', 'Efforts': colsum.toFixed(1) }, false);

        },

    });

    $('#list').jqGrid('navGrid', '#listpager',
                 parameters).jqGrid('navButtonAdd', "#listpager", Savebtnparameter).jqGrid('navButtonAdd', "#listpager", { caption: "", buttonicon: "ui-icon-check", onClickButton: ApproveSelected, position: "last", title: "", cursor: "pointer" }).jqGrid('navButtonAdd', "#listpager", { caption: "", buttonicon: "ui-icon-closethick", onClickButton: RejectAllSelected, position: "last", title: "", cursor: "pointer" });

}

function RejectAllSelected() {
   var Ids = $("#list").jqGrid('getGridParam', 'selarrrow');
   if (Ids.length == 0)
   {
       toastr.options.positionClass = 'toast-top-center';
       toastr.options.timeOut = '1000';
       toastr["error"]("Please select a record")
       return;
   }
    $("#rejectionDialog").dialog({
        modal: true,
        resizable: false,
        height: "auto",
        width: 300,
        title: 'Reject',
        buttons: {
            "Reject": function () {
                if ($("#txtreject").val().length != 0)
                {
                RejectSelected();
                $(this).dialog("close");
                }
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        }
    });



}

function RejectSelected() {
    //var myGrid = $('#list'), i, rowData, Ids = "",
    //         rowIds = myGrid.jqGrid("getDataIDs"),
    //         n = rowIds.length;
    //for (i = 0; i < n; i++) {
    //    rowData = myGrid.jqGrid("getLocalRow", rowIds[i]);
    //    if (rowData.active) {
    //        Ids += rowData.n_id + ',';
    //    }
    //}

    var Ids = $("#list").jqGrid('getGridParam', 'selarrrow');

    var IsApproved = false;
    var RejectionReason = $("#txtreject").val();
    var status = false;
    var modal = {
        Ids: Ids.toString(),
        IsApproved: -1,
        RejectionReason: RejectionReason,
        status: status

    }
    $.ajax({
        url: '/report/ApproveReject',
        datatype: 'JSON',
        data: modal,
        success: function (result) {
            if (result.pn_Error == false) {

                toastr["success"](result.ps_Msg)
                //setTimeout(
                //    function () {

                //        window.location.reload()

                //    }, 3000
                //    );
                var griddata = $("#list").jqGrid('getGridParam', 'data');

                for (var i = 0; i < Ids.length; i++) {
                    //$('#list').delRowData(Ids[i]);

                    var obj = $.each(griddata, function (j) {

                        if ((this.n_id == Ids[i])) {
                            this.Status = "Rejected";
                            this.n_status = -1;
                        }
                    });
                }
  
            }
            else {
                toastr["error"](result.ps_Msg)
            }
            $("#list").trigger('reloadGrid');
        },
        failure: onFailure
    });



}

function ApproveSelected()
{
    var Ids = $("#list").jqGrid('getGridParam', 'selarrrow');
    if (Ids.length == 0) {
        toastr.options.positionClass = 'toast-top-center';
        toastr.options.timeOut = '1000';
        toastr["error"]("Please select a record")
        return;
    }
    for (var i = 0; i < Ids.length; i++) {
        rowData = $("#list").jqGrid("getRowData", Ids[i]);
        if (rowData.Status == "Rejected")
        {
            toastr.options.positionClass = 'toast-top-center';
            toastr.options.timeOut = '1000';
            toastr["error"]("Please unselect the Rejected record")
            return;
        }

    }
    var IsApproved = 2;
    var RejectionReason = null;
    var status = false;
    var modal = {
        Ids: Ids.toString(),
        IsApproved: IsApproved,
        RejectionReason: RejectionReason,
        status: status

    }
    $.ajax({
        url: '/report/ApproveReject',
        datatype: 'JSON',
        data: modal,
        success: function (result) {
            if (result.pn_Error == false) {

                toastr["success"](result.ps_Msg)
                var griddata = $("#list").jqGrid('getGridParam', 'data');

                for (var i = 0; i < Ids.length; i++) {
                    //$('#list').delRowData(Ids[i]);

                    var obj = $.each(griddata, function (j) {

                        if ((this.n_id == Ids[i])) {
                            this.Status = "Approved";
                            this.n_status = 2;
                        }
                    });
                }
                //setTimeout(
                //    function () {

                        //window.location.reload()
                        //$("#list").jqGrid('delRowData', Ids)
                        //for (var i = Ids.length - 1; i >= 0; i--) {
                        //    $('#list').delRowData(Ids[i]);
                        //}
                    //}, 3000
                    //);

            }
            else {
                toastr["error"](result.ps_Msg)
            }
            $("#list").trigger('reloadGrid');
        },
        failure: onFailure
    });



}

function clearAll()
{

    $("#list").jqGrid("clearGridData", true).trigger("reloadGrid");
    $("#divCrSum").hide();
    $("#detaildatalist").hide();
    $("#listSummary").jqGrid("clearGridData", true).trigger("reloadGrid");
    $("#crPrjSummary").jqGrid("clearGridData", true).trigger("reloadGrid");
    return true;
}
onSuccess = function (data) {
    
    
    clearAll();
    var objArray = [];
    if (data != null) {
        
        if (data.Summary.length == undefined) {
            objArray.push(data.Summary);
        }
        else {
            if (data.Summary.length == 0 || data.Summary == null) {
                toastr.options.positionClass = 'toast-top-center';
                toastr.options.timeOut = '1000';
                toastr["warning"]("No data found!!!");


            }
            else {
                console.log(data.Summary);
                objArray = data.Summary;
            }
        }
    }
    
    
    jQuery("#listSummary").jqGrid('setGridParam', { data: objArray }).trigger('reloadGrid');
}

function DetailsFormatter(cellvalue, options, rowObject)
{
    return "<span onclick='fetchdetails(" + rowObject.crId + "," + rowObject.ProjectId + ")' class='ui-icon ui-icon-pencil' style='border:0px;width:15px; background-color:#0078ae'></span>";
}
function fetchdetails(crId,PrjId)
{
    $.ajax({

        url: '/Report/GetDetails',
        method: 'POST',
        datatype: 'JSON,',
        data: { CrNumberId: crId, ProjectId: PrjId ,ResourceId:$("#ResourceId").val() ,MonthId:$("#ddlMonthId").val() ,WeekId:$("#ddlWeekId").val(),statusId:$("#statusId").prop("checked")},
        success: function (data) {
            jQuery("#list").jqGrid('clearGridData');
            var objArray = [];
            if (data.length == undefined)
            {
                objArray.push(data);
            }
            else
            {
                objArray = data;
            }
            
            jQuery("#list").jqGrid('setGridParam', { data: objArray }).trigger('reloadGrid');
            $("#detaildatalist").show();
        },
        failure: onFailure

    });

}
function RejectAll()
{
    
            $("#rejectionDialog").dialog({
                modal: true,
                resizable: false,
                height: "auto",
                width: 300,
                title:'Reject',
                buttons: {
                    "Reject": function () {
                        Reject();
                        $(this).dialog("close");
                    },
                    Cancel: function () {
                        $(this).dialog("close");
                    }
                }
            });
            
        
    
}
function Reject()
{
    var IsApproved = false;
    var RejectionReason = $("#txtreject").val();
    var status = false;
    var modal = {
        IsApproved: -1,
        RejectionReason: RejectionReason,
        status: status

    }
    $.ajax({
        url: '/report/ApproveReject',
        datatype: 'JSON',
        data: modal,
        success: function (result) {
            if (result.pn_Error == false) {
                toastr["success"](result.ps_Msg)
                $("#txtreject").val("");
                setTimeout(
                   function () { window.location.reload() }, 3000
                    );
                
            }
            else {
                toastr["error"](result.ps_Msg)
            }
            $("#list").trigger('reloadGrid');
        },
        failure: onFailure
    });

}
function ApproveAll()
{
    var IsApproved = 2;
    var RejectionReason = null;
    var status = false;
    var modal = {
        IsApproved: IsApproved,
        RejectionReason: RejectionReason,
        status: status

    }
    $.ajax({
        url: '/report/ApproveReject',
        datatype: 'JSON',
        data: modal,
        success:function(result)
        {
            if (result.pn_Error == false)
            {
                
                toastr["success"](result.ps_Msg)
                setTimeout(
                    function () {

                        window.location.reload()

                    }, 3000
                    );
                
            }
            else
            {
                toastr["error"](result.ps_Msg)
            }
            $("#list").trigger('reloadGrid');
        },
        failure:onFailure
    });
    
}

function SaveData()
{
    
    var rows = $("#list").getDataIDs();
    for (var i = 0; i < rows.length; i++) {
        jQuery("#list").saveRow(rows[i], false, 'clientArray');
       // jQuery("#list").saveRow(rows[i]);

    }
    //$("#list").trigger('reloadGrid');
    var obj = $("#list").jqGrid('getGridParam', 'data');
    
        var modal = obj;
    $.ajax({
        
        url: '/report/ModifyData',
        type:"POST",
        data: '{ modal : '+JSON.stringify(modal)+'}',
        contentType: "application/json; charset=utf-8",
        traditional: true,
        success: function (result) {
            if (result.pb_Error == true) {                
                toastr["error"](result.ps_Msg);
            }
            {                
                toastr["success"](result.ps_Msg);
                $('form#searchForm').trigger('submit');
            }
        },
        failure: onFailure

    });

}

function LoadCrProjectBasedGrid()
{
    $("#crPrjSummary").jqGrid("clearGridData", true).trigger("reloadGrid");
    jQuery("#crPrjSummary").jqGrid({
        datatype: "local",
        autowidth: true,
        altRows: false,
        colNames: ["Resource", "Billed Hrs", "Rate", "Cost Accured", "Earlier Cost", "Total Cost"],
        colModel: [
            { index: 'FullName', name: "FullName", width: 120 },
            { index: 'TotalBilledThisMonth', name: "TotalBilledThisMonth", width: 60, formatter: 'number', align: 'right', formatoptions: { decimalSeparator: '.', decimalPlaces: 1 } },
            { index: 'CostPerHr', name: "CostPerHr", width: 60, formatter: 'number', align: 'right', formatoptions: { decimalSeparator: '.', decimalPlaces: 1 } },  
            { index: 'CostThisMonth', name: "CostThisMonth", width: 60, formatter: 'number', align: 'right' },
            { index: 'EarlierCost', name: "EarlierCost", width: 80, formatter: 'number', align: 'right' },
            { index: 'TotalCost', name: "TotalCost", width: 70, formatter: 'number', align: 'right' },
        ],
        pager: "#crPrjSummarypager",
        rowNum: 30,
        height: "165px",
        footerrow: true,
        rowList: [30, 20, 10],
        gridComplete: function () {
        var BilledHrs = 0.00;
        var Cost = 0.00;
        var EarlierCost = 0.00;
        var TotalCost = 0.00;
        var rows = $("#crPrjSummary").getDataIDs();
        for (var i = 0; i < rows.length; i++) {
            BilledHrs = BilledHrs + parseFloat($("#crPrjSummary").getCell(rows[i], "TotalBilledThisMonth"));
            Cost = Cost + parseFloat($("#crPrjSummary").getCell(rows[i], "CostThisMonth"));
            EarlierCost = EarlierCost + parseFloat($("#crPrjSummary").getCell(rows[i], "EarlierCost"));
            TotalCost = TotalCost + parseFloat($("#crPrjSummary").getCell(rows[i], "TotalCost"));
            
        }
        $("#crPrjSummary").jqGrid('footerData', 'set', {
            'FullName': 'Total',
            'TotalBilledThisMonth':BilledHrs.toFixed(1),
            'CostThisMonth': Cost.toFixed(2),
            'EarlierCost': EarlierCost.toFixed(2),
            'TotalCost': TotalCost.toFixed(2)

        }, false);

    }
    });
}

function GetProjectCrLevelDashboard(crId, PrjId, Month) {

    var modal = {
        CrId: crId,
        ProjectId: PrjId,
        month: Month
    }
    $.ajax({

        url: '/Report/GetProjectCrLevelDashboard',
        data: modal,
        datatype: 'JSON',
        success: function (data) {
            console.log(data);
            jQuery("#crPrjSummary").jqGrid("clearGridData", true).trigger("reloadGrid");
            jQuery("#crPrjSummary").jqGrid('setGridParam', { data: data.resourceCostModal }).trigger('reloadGrid');
            jQuery("#SDLC").text(data.costModal.SDLC);
            jQuery("#QA").text(data.costModal.QA);
            jQuery("#appName").html(data.costModal.Name);
            jQuery("#Dev").text(data.costModal.Dev);
            $("#divCrSum").show();
        },
        failure: onFailure
    });

}

function getPostObj(obj)
{
    var modal = [];
    $.each(obj, function (i) {
        modal.push({

               n_id: obj[i].n_id,
               Efforts: obj[i].Efforts,
               Billable: obj[i].Billable,

        });

    });

    return JSON.stringify(modal);

}

function CrDbFormatter(cellvalue, options, rowObject)
{
    return "<input type='button' onclick='GetProjectCrLevelDashboard(" + rowObject.crId + "," + rowObject.ProjectId + "," + $("#ddlMonthId").val() + ")' class='ui-icon   ui-icon-arrow-4' style='border:0px;width:15px; background-color:#0078ae''/>";
}


function createexceltextbox(cellvalue, options, rowObject) {
    return "<input type='text' onchange='UpdateGridCellValue(this,\"Efforts\",\"" + rowObject.n_id + "\")' value='" + cellvalue + "' style='width:20px;text-align:right' />";
}
function UpdateGridCellValue(obj,colname,rowid)
{
    jQuery("#list").saveRow(rowid, false, 'clientArray');
    $("#list").jqGrid('setCell', rowid, colname, $(obj).val());
    $("#list").trigger('reloadGrid');
}
function CreateSelectBox(cellvalue, options, rowObject) {
    return "<select onchange='UpdateGridCellValue(this,\"Billable\",\"" + rowObject.n_id + "\")'><option value='1' " + (cellvalue == 'Yes' ? 'selected' : '') + ">Yes</option><option value='0' " + (cellvalue == 'No' ? 'Selected' : '') + ">No</value></select>";
}
function LoadCrBasedGrid()
{


}

 $("#ddlMonthId").change(function () {
    var month= $(this).find("option:selected").text();
    $.post('/Report/GetWeekIdList/', {month : month}, function (data, status, xhr) {
        if (status == "success") {
            $('#ddlWeekId').empty();
                $(data).each(function () {
                var option = $("<option />");
 
                option.html(this.Text);
                option.val(this.Value);
                $('#ddlWeekId').append(option);
            });       
        }
    });

    
    });