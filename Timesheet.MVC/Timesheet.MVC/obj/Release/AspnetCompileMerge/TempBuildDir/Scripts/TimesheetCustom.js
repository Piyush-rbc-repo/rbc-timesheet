
var parameters = {
    edit: false,
    editicon: "ui-icon-pencil",
    add: false,
    del: false,
    addicon: "ui-icon-plus",
    cancel: true,
    cancelicon: "ui-icon-cancel"

};

$(document).ready(function () {
    jQuery("#FromDate").datepicker({ dateFormat: 'dd/mm/yy' });
    jQuery("#Activitydate").datepicker({ dateFormat: 'dd/mm/yy' });
    jQuery("#ToDate").datepicker({ dateFormat: 'dd/mm/yy' });
    $("#ProjectId").change(function () {
      
       
        LoadCrs(this,'');
    });


    //Added by Sasmita from Old code
    $("#CrNumberId").change(function () {

        LoadPrjs(this, '');
    });

    loadTimesheetGrid();
    $('form#searchForm').trigger('submit');
    $("#LinkExport").click(function () {
        
        var formData = {            
           // "Resource": $("#Resources").val(),
           // "ResourceId": $("#ResourceId").val(),
            "Resource": $("#Resource").val(),
            "FromDate": $("#FromDate").val(),
            "ToDate": $("#ToDate").val()
        };
        // Commented By Sasmita to call new DownloadExcel function
       // window.open("/timesheet/excel?" + jQuery.param(formData));
        window.open("/timesheet/DownloadExcel?" + jQuery.param(formData));

    });

    //Added by Sasmita for DefaulterListDownalod | Starts

    $("#LinkExportDefaulter").click(function () {
      
        var formData = {            
            "FromDate": $("#FromDate").val(),
            "ToDate": $("#ToDate").val()
        };

        window.open("/timesheet/DownloadDefaulterList?" + jQuery.param(formData));

    });
    //Added by Sasmita for DefaulterListDownalod | Ends


});

//Commented by Sasmita to add LoadCrs() from Old code
//function LoadCrs(obj,CrNumber)
//{
//    $("#CrNumberId option[value!='']").remove();

//    if ($(obj).val() == "") {
        
//        $("#CrNumberId").attr('disabled', 'disabled');
//    }
//    else {

//        $.ajax(
//            {
//                url: '/timesheet/GetCrList',
//                data: { projectid: $(obj).val() },
//                success: function (data) {
//                    console.log(data);
//                    if (data.length > 0) {
//                        $.each(data, function (i) {
//                            $("#CrNumberId").append("<option value=" + data[i].Value + ">" + data[i].Text + "</option>");
//                        });
//                        if (data.length == 1) {
//                            $("#CrNumberId").val(data[0].Value);

//                        }
//                        if (CrNumber != undefined) {
//                            $("#CrNumberId option").filter(function () {
//                                //may want to use $.trim in here
//                                return $(this).text() == CrNumber;
//                            }).prop('selected', true)
//                        }
                        
//                       $("#CrNumberId").removeAttr('disabled');
//                    }
//                    else {                        
//                        $("#CrNumberId").attr('disabled', 'disabled');
//                        toastr["warning"]("No CR Number is mapped to selected project.");

//                    }



//                },
//                failure: onFailure
//            });
//    }


//}

//Added by Sasmita from Old code
function LoadCrs(obj, CrNumber) {
    $("#CrNumberId option[value!='']").remove();
   
    if ($(obj).val() == "") {       
        //$("#CrNumberId").attr('disabled', 'disabled');
        $.ajax(
            {
                url: '/timesheet/GetCrList',
                data: { projectid: 0 },
                success: function (data) {
                    console.log(data);
                    if (data.length > 0) {
                        $.each(data, function (i) {
                            $("#CrNumberId").append("<option value=" + data[i].Value + ">" + data[i].Text + "</option>");
                        });
                        if (data.length == 1) {
                            $("#CrNumberId").val(data[0].Value);

                        }
                        if (CrNumber != undefined) {
                            $("#CrNumberId option").filter(function () {
                                //may want to use $.trim in here
                                return $(this).text() == CrNumber;
                            }).prop('selected', true)
                        }
                        //$("#CrNumberId").removeAttr('disabled');
                    }
                    else {
                        //$("#CrNumberId").attr('disabled', 'disabled');
                        toastr["warning"]("No CR Number is mapped to selected project.");

                    }



                },
                failure: onFailure
            });

        $.ajax(
            {
                url: '/timesheet/GetProjectList',
                data: { CrNumber: 0 },
                success: function (data) {
                    console.log(data);
                    if (data.length > 0) {
                        $.each(data, function (i) {
                            $("#ProjectId").append("<option value=" + data[i].Value + ">" + data[i].Text + "</option>");
                        });
                        if (data.length == 1) {
                            $("#ProjectId").val(data[0].Value);

                        }
                        if (PrjNumber != undefined) {
                            $("#ProjectId option").filter(function () {
                                //may want to use $.trim in here
                                return $(this).text() == PrjNumber;
                            }).prop('selected', true)
                        }
                        //$("#CrNumberId").removeAttr('disabled');
                    }
                    else {
                        //$("#CrNumberId").attr('disabled', 'disabled');
                        toastr["warning"]("No Project is mapped to selected CR Number.");

                    }



                },
                failure: onFailure
            });

    }
    else {       

        $.ajax(
            {
                url: '/timesheet/GetCrList',
                data: { projectid: $(obj).val() },
                success: function (data) {
                    console.log(data);
                    if (data.length > 0) {
                        $.each(data, function (i) {
                            $("#CrNumberId").append("<option value=" + data[i].Value + ">" + data[i].Text + "</option>");
                        });
                        if (data.length == 1) {
                            $("#CrNumberId").val(data[0].Value);

                        }
                        if (CrNumber != undefined) {
                            $("#CrNumberId option").filter(function () {
                                //may want to use $.trim in here
                                return $(this).text() == CrNumber;
                            }).prop('selected', true)
                        }
                        //$("#CrNumberId").removeAttr('disabled');
                    }
                    else {
                        //$("#CrNumberId").attr('disabled', 'disabled');
                        toastr["warning"]("No CR Number is mapped to selected project.");

                    }



                },
                failure: onFailure
            });
    }


}

//Added by Sasmita from Old code
function LoadPrjs(obj, PrjNumber) {
    $("#ProjectId option[value!='']").remove();

    if ($(obj).val() == "") {

        //$("#CrNumberId").attr('disabled', 'disabled');
        $.ajax(
            {
                url: '/timesheet/GetProjectList',
                data: { CrNumber: 0 },
                success: function (data) {
                    console.log(data);
                    if (data.length > 0) {
                        $.each(data, function (i) {
                            $("#ProjectId").append("<option value=" + data[i].Value + ">" + data[i].Text + "</option>");
                        });
                        if (data.length == 1) {
                            $("#ProjectId").val(data[0].Value);

                        }
                        if (PrjNumber != undefined) {
                            $("#ProjectId option").filter(function () {
                                //may want to use $.trim in here
                                return $(this).text() == PrjNumber;
                            }).prop('selected', true)
                        }
                        //$("#CrNumberId").removeAttr('disabled');
                    }
                    else {
                        //$("#CrNumberId").attr('disabled', 'disabled');
                        toastr["warning"]("No Project is mapped to selected CR Number.");

                    }



                },
                failure: onFailure
            });

        $.ajax(
            {
                url: '/timesheet/GetCrList',
                data: { projectid: 0 },
                success: function (data) {
                    console.log(data);
                    if (data.length > 0) {
                        $.each(data, function (i) {
                            $("#CrNumberId").append("<option value=" + data[i].Value + ">" + data[i].Text + "</option>");
                        });
                        if (data.length == 1) {
                            $("#CrNumberId").val(data[0].Value);

                        }
                        if (CrNumber != undefined) {
                            $("#CrNumberId option").filter(function () {
                                //may want to use $.trim in here
                                return $(this).text() == CrNumber;
                            }).prop('selected', true)
                        }
                        //$("#CrNumberId").removeAttr('disabled');
                    }
                    else {
                        //$("#CrNumberId").attr('disabled', 'disabled');
                        toastr["warning"]("No CR Number is mapped to selected project.");

                    }



                },
                failure: onFailure
            });
    }
    else {

        $.ajax(
            {
                url: '/timesheet/GetProjectList',
                data: { CrNumber: $(obj).val() },
                success: function (data) {
                    console.log(data);
                    if (data.length > 0) {
                        $.each(data, function (i) {
                            $("#ProjectId").append("<option value=" + data[i].Value + ">" + data[i].Text + "</option>");
                        });
                        if (data.length == 1) {
                            $("#ProjectId").val(data[0].Value);

                        }
                        if (PrjNumber != undefined) {
                            $("#ProjectId option").filter(function () {
                                //may want to use $.trim in here
                                return $(this).text() == PrjNumber;
                            }).prop('selected', true)
                        }
                        //$("#CrNumberId").removeAttr('disabled');
                    }
                    else {
                        //$("#CrNumberId").attr('disabled', 'disabled');
                        toastr["warning"]("No Project is mapped to selected CR Number.");

                    }



                },
                failure: onFailure
            });
    }
}


function showBulkUplodDialog() {


    $("#divbulk").dialog({width:'320px'});
}

function loadTimesheetGrid() {
    jQuery("#list").jqGrid({
        datatype: "local",
        autowidth: true,
        
        altRows: false,
        colNames: ["Id", "Activity Date", "Resource Name", "Project Name", "Cr Name", "Activity", "Sub Activity",
                    "Efforts (Hrs.)", "Efforts (Days)", "Billable", "Comments",
                    "Status", "n_status", "RejectionReason", "Edit","Copy", "Delete"],
        colModel: [
            { index: 'n_id', name: "n_id", width: 10, hidden: true, key: true },
            { index: 'ActivityDate', name: "ActivityDate", width: 80 },
            { index: 'ResourceName', name: "ResourceName", width: 10, hidden: true },
            { index: 'ProjectName', name: "ProjectName", width: 200 },
            { index: 'CrNumber', name: "CrNumber", width: 70 },
            { index: 'Activity', name: "Activity", width: 150 },
            { index: 'SubActivity', name: "SubActivity", width: 150 },
            { index: 'Efforts', name: "Efforts", width: 80, align: "right" },
            { index: 'Efforts_Days', name: "Efforts_Days", width: 80, align: "right" },
            { index: 'Billable', name: "Billable", width: 50, },
            { index: 'Comments', name: "Comments", width: 10, hidden: true },
            { index: 'Status', name: "Status", width: 60 ,title:false},
            { index: 'n_status', name: "n_status", width: 1, hidden: true },
            { index: 'RejectionReason', name: "RejectionReason", width: 1, hidden: true },
           { index: 'Edit', name: "Edit", editable: true, align: 'center', formatter: EditFormatter, width: 40 },
           { index: 'Copy', name: "Copy", editable: true, align: 'center', formatter: CopyFormatter, width: 40 },
           { index: 'Delete', name: "Delete", editable: true, align: 'center', formatter: DeletFormatter, width: 40 },
        ],
        pager: "#listpager",
        rowNum: 100,
        
        onSelectAll: function (aRowids, status) {
            if (status = false) {
                return;
            }

            var i, rows = aRowids.slice();
            for (i = 0; i < rows.length; i++) {
                obj = $('#list').jqGrid('getRowData', rows[i]);
                if (obj.IsSubmitted == "Submitted") {

                    //jQuery("#jqg_list_" + aRowids[i]).attr("checked", false);
                    var rowid = rows[i];
                    $('#list').jqGrid('setSelection', rowid, false);


                }

            }
        },
        beforeSelectRow: function (rowid, e) {

            obj = $('#list').jqGrid('getRowData', rowid);
            if (obj.IsSubmitted == "Submitted") {

                //jQuery("#jqg_list_" + rows[i]).attr("checked", false);
                $(grid).jqGrid('setSelection', rowid, false);
                return false;
            }
            else {
                return true;

            }
        },
        height: "400px",
        multiselect: false,
        viewrecords: true,
        gridview: true,
        autoencode: true,
        caption: "",
        footerrow: true,
        loadComplete: function () {
            var rows = $(this).getDataIDs();

            for (var i = 0; i < rows.length; i++) {
                var row = $(this).getRowData(rows[i]);
                if (row.Status == 'Rejected') {

                    var cm = $('#list').jqGrid("getGridParam", "colModel");
                    $(this.rows[i + 1].cells[11]).attr('title', row.RejectionReason);
                }
            }
        },
        gridComplete: function () {

            // var colSum = $("#list").jqGrid('getCol', 'Efforts', false, 'sum');
            var colsum = 0.00;
            var NonBillable = 0.00;
            var rows = $("#list").getDataIDs();
            for (var i = 0; i < rows.length; i++) {
                var Billable = $("#list").getCell(rows[i], "Billable");
                
                if (Billable == "Yes") {
                    $("#list").jqGrid('setRowData', rows[i], false, { color: 'black', background: '#E3F6CE' });
                    colsum = colsum + parseFloat($("#list").getCell(rows[i], "Efforts"));
                }
                else
                {
                    NonBillable = NonBillable + parseFloat($("#list").getCell(rows[i], "Efforts"));

                    
                }
                
            }
            $("#list").jqGrid('footerData', 'set', { 'CrNumber': NonBillable, 'ProjectName': 'Non Billable Hours', 'SubActivity': 'Billable Hours', 'Efforts': colsum }, false);

        }

    });
   
}
function confirmDelete(id) {
    var ids = []
    ids.push(id);
    if (confirm('do you really want to delete the record?')) {
        submitDelet(ids, "D");

    }

}
function submit() {
    submitDelet($("#list").getDataIDs(), "s","false");
   // submitDelet(jQuery("#list").jqGrid('getGridParam', 'selarrrow'), "S");
}
function submitDelet(rowids, mode,partialSubmit) {
    ClearCreateForm();
    if (rowids.length == 0) {
        toastr["error"]("Please select record to update");
        return false;
    }
    var s = rowids;
    var submitmethod = 'SubmitRecords';
    var deletemethod = 'DeleteRecords'

    $.ajax({
        url: 'timesheet/' + (mode == "D" ? deletemethod : submitmethod),
        data: { i: s.join(), partialSubmit: partialSubmit},
        method: 'post',
        success: function (ResultObj) {
            if (ResultObj.result.pn_Error == true) {
                console.log(ResultObj);
                toastr["error"](ResultObj.result.ps_Msg);
                
                if(mode=="s" && (ResultObj.CauseList!=undefined || ResultObj.CauseList.length>0))
                {
                    var stringResult = "";
                    for (var i = 0; i < ResultObj.CauseList.length; i++)
                    {
                        stringResult = stringResult + ResultObj.CauseList[i].Cause;

                    }

                    var dynamicDialog = $('<div id="rundialog"><table style="border:1px solid #d9d99">' + stringResult + '</table></div>');
                    dynamicDialog.dialog({
                        title: "Invalid Timesheet",
                        modal: true,
                        width: '800px',
                        height:'auto',
                        buttons: {
                            'Close': function () {
                                $(this).dialog("close");
                            }
                        }
                    });
                }
            }
            else {
                if (ResultObj.type == "D") {
                    $.each(s, function () {
                        $("#list").delRowData(this).trigger('reloadGrid');
                    });
                }
                else {
                    var griddata = $("#list").jqGrid('getGridParam', 'data');

                    for (var i = 0; i < ResultObj.Idarray.split(',').length; i++) {

                        var obj = $.each(griddata, function (j) {

                            if  ((this.n_id == ResultObj.Idarray.split(',')[i]) && this.Status != "Approved" )
                                this.Status = "Submitted";
                                 this.n_status = 1;
                        });
                        //$("#list").getRowData(ResultObj.Idarray.split(',')[i]);
                        //obj.IsSubmitted = "Submitted";
                        //$('#list').jqGrid('setRowData', ResultObj.Idarray.split(',')[i], obj);
                        //$('#list').setRowData(ResultObj.Idarray.split(',')[i], obj);
                    }

                    $("#list").jqGrid('setGridParam', { data: griddata }).trigger('reloadGrid');
                }
                toastr.options.positionClass = 'toast-top-center';
                toastr.options.timeOut = '1000';
                toastr["success"](ResultObj.result.ps_Msg);
            }

        },
        failure: onFailure

    })

}
$("#btnSubmitPartially").click(function () {
    submitDelet($("#list").getDataIDs(), "s","true");
});



function FillEdit(object) {
    ClearCreateForm();
    var obj = $("#list").jqGrid('getRowData', object);
    var Activity = obj.Activity;
    var ActivityDate = obj.ActivityDate;
    var Billable = obj.Billable;
    var Comments = obj.Comments;
    var CrNumber = obj.CrNumber;
    var Efforts = obj.Efforts;
    var IsSubmitted = obj.IsSubmitted;
    var ProjectName = obj.ProjectName;
    var ResourceName = obj.ResourceName;
    var SubActivity = obj.SubActivity;
    var n_id = obj.n_id;
    console.log(CrNumber);
    $("#Id").val(n_id);
    $('#ActivityId option').filter(function () {
        //may want to use $.trim in here
        return $(this).text() == Activity;
    }).prop('selected', true);
    $("#ResourceId option").filter(function () {
        //may want to use $.trim in here
        return $(this).text() == ResourceName;
    }).prop('selected', true);

    $("#ProjectId option").filter(function () {
        //may want to use $.trim in here
        return $(this).text() == ProjectName;
    }).prop('selected', true);

    
    $("#IsBillable option").filter(function () {
        //may want to use $.trim in here
        return $(this).text() == Billable;
    }).prop('selected', true);

    $("#SubActivity").val(SubActivity);
    $("#Efforts").val(Efforts);
    $("#Comments").val(Comments);
    $("#IsSubmit").prop("checked", ((IsSubmitted == "Yes" ? true : false)));

    $("#Activitydate").val(ActivityDate);
    $("#save").val("Update");
    if (CrNumber != undefined) {
        LoadCrs($("#ProjectId"), CrNumber);
    }

    

    

}

function Duplicate(object)
{
    ClearCreateForm();
    var obj = $("#list").jqGrid('getRowData', object);
    var Activity = obj.Activity;
    var ActivityDate = obj.ActivityDate;
    var Billable = obj.Billable;
    var Comments = obj.Comments;
    var CrNumber = obj.CrNumber;
    var Efforts = obj.Efforts;
    var IsSubmitted = obj.IsSubmitted;
    var ProjectName = obj.ProjectName;
    var ResourceName = obj.ResourceName;
    var SubActivity = obj.SubActivity;

    $('#ActivityId option').filter(function () {
        //may want to use $.trim in here
        return $(this).text() == Activity;
    }).prop('selected', true);
    $("#ResourceId option").filter(function () {
        //may want to use $.trim in here
        return $(this).text() == ResourceName;
    }).prop('selected', true);

    $("#ProjectId option").filter(function () {
        //may want to use $.trim in here
        return $(this).text() == ProjectName;
    }).prop('selected', true);

    $("#CrNumberId option").filter(function () {
        //may want to use $.trim in here
        return $(this).text() == CrNumber;
    }).prop('selected', true);

    $("#IsBillable option").filter(function () {
        //may want to use $.trim in here
        return $(this).text() == Billable;
    }).prop('selected', true);

    $("#SubActivity").val(SubActivity);
    $("#Efforts").val(Efforts);
    $("#Comments").val(Comments);
    $("#Activitydate").val(ActivityDate);
    

}
function EditFormatter(cellvalue, options, rowObject) {
    if (parseInt(rowObject.n_status) >= 1) {
        return "";
    }
    else
    {
        return "<center><input type='button' class='ui-icon ui-icon-pencil' style='height:17px' onclick='FillEdit(" + rowObject.n_id + ")' value='edit'/></center>";
    }

}
function CopyFormatter(cellvalue, options, rowObject) {
    if (parseInt(rowObject.n_status) >= 1) {
        return "";
    }
    else {
        return "<center><input type='button' class='ui-icon ui-icon-copy' style='height:17px' onclick='Duplicate(" + rowObject.n_id + ")' value='Copy'/></center>";
    }

}
function DeletFormatter(cellvalue, options, rowObject) {
    if (parseInt(rowObject.n_status) >= 1) {
        return "";
    }
    else {
        return "<center><input type='button' class='ui-icon ui-icon-trash' style='height:17px' onclick='confirmDelete(" + rowObject.n_id + ")' value='Delete'/></center>";
    }
}
function getSelectedRows() {
    var grid = $("#list");
    var rowKey = grid.getGridParam("selrow");

    if (!rowKey)
        alert("No rows are selected");
    else {
        var selectedIDs = grid.getGridParam("selarrrow");
        var result = "";
        for (var i = 0; i < selectedIDs.length; i++) {
            result += selectedIDs[i] + ",";
        }

        alert(result);
    }
}
var onSuccess = function (resultobj) {

    toastr.options.positionClass = 'toast-top-center';
    toastr.options.timeOut = '1000';
    toastr["success"](resultobj.msg);
    var result = resultobj.model;
    var obj = jQuery("#list").jqGrid('getGridParam', 'data');
    var resourceName = $('#ResourceId option:selected').text();

    var ProjName = $('#ProjectId option:selected').text();

    var CrNumber = ($('#CrNumberId option:selected').val() == "" ? "" : $('#CrNumberId option:selected').text());

    var Activity = $('#ActivityId option:selected').text();


    var newobj = {
        n_id: result.Id, ActivityDate: result.Activitydate, ResourceName: resourceName, ProjectName: ProjName, CrNumber: CrNumber, Activity: Activity,
        SubActivity: result.SubActivity, Efforts: result.Efforts, Efforts_Days: parseFloat(result.Efforts) / 8.0, Billable: (result.IsBillable ? 'Yes' : 'No'), Comments: result.Comments,
        Status: 'Saved',
        /*IsSubmitted: (result.IsSubmit ? 'Submitted' : 'Saved'),*/
    }
    if (result.mode == "I") {
        obj.push(newobj);
        jQuery("#list").jqGrid('setGridParam', { data: obj }).trigger("reloadGrid");
    }
    else {
        var griddata = $("#list").jqGrid('getGridParam', 'data');
        var obj = $.each(griddata, function (j) {

            if (this.n_id == result.Id)
                griddata[j] = newobj;
        });
        $("#list").jqGrid('setGridParam', { data: griddata }).trigger('reloadGrid');
        //$("#list").getRowData(ResultObj.Idarray.split(',')[i]);
        //obj.IsSubmitted = "Submitted";
        //$('#list').jqGrid('setRowData', ResultObj.Idarray.split(',')[i], obj);
        //$('#list').setRowData(ResultObj.Idarray.split(',')[i], obj);
    }



    //$('#list').jqGrid('setRowData', result.Id, newobj);
    ClearCreateForm();
}

function UploadFile(btn)
{
    
    
    var _file = document.getElementById("file1"),
           _progress = $('#uploadProgressBar');

    if (_file.files.length === 0)
    {
        toastr["error"]("Please select a tab separated file first");
        $(btn).removeAttr('disabled');
    }
    else
    {
        var formData = new FormData();
        formData.append('file', _file.files[0]);
        formData.append('overWriteExistsing', $("#chb_overwrite").is(':checked'));
        $(_progress).css("width", "0%"); //reset Progress bar
        $(_progress).css("display", "block");

        $.ajax({
            url: '/timesheet/UploadExcel',
            type: "POST",
            data: formData,
            contentType: false,
            cache: false,
            processData: false,
            xhr: function () {
                //upload Progress
                var xhr = $.ajaxSettings.xhr();
                if (xhr.upload) {
                    xhr.upload.addEventListener('progress', function (event) {
                        var percent = 0;
                        var position = event.loaded || event.position;
                        var total = event.total;
                        if (event.lengthComputable) {
                            percent = Math.ceil(position / total * 100);
                        }
                        //update progressbar
                        $(_progress).css("width", +percent + "%");
                      //  $(progress_bar_id + " .status").text(percent + "%");
                    }, true);
                }
                return xhr;
            },
            mimeType: "multipart/form-data",
            success: function (result) {
                 var res = JSON.parse(result);
                console.log("success");
                console.log(res);
                $(_progress).css("width", "0%"); //reset Progress bar
                $(_progress).css("display", "none");
                if (res.pn_Error == true) {
                    toastr["error"](res.ps_Msg);
                    if(res.data.length>0)   
                    {
                        var str = "<tr><th style='border:1px solid #e1e1e1;padding:3px'>Activity Date</th>";
                        str += "<th style='border:1px solid #e1e1e1;padding:3px'>Resource Name</th>";
                        str += "<th style='border:1px solid #e1e1e1;padding:3px'>Project Name</th>";
                        str += "<th style='border:1px solid #e1e1e1;padding:3px'>Cr Number</th>";
                        str += "<th style='border:1px solid #e1e1e1;padding:3px'>Activity</th>";
                        str += "<th style='border:1px solid #e1e1e1;padding:3px'>Sub Activity</th>";
                        str += "<th style='border:1px solid #e1e1e1;padding:3px'>Efforts Hrs</th>";
                        str += "<th style='border:1px solid #e1e1e1;padding:3px'>Efforts Days</th>";
                        str += "<th style='border:1px solid #e1e1e1;padding:3px'>Status</th>";
                        for (var i = 0 ;i<res.data.length;i++)
                        {

                            str = str + "<tr >";
                            str = str + "<td style='border:1px solid #e1e1e1;padding:3px'> " + res.data[i].ActivityDate + "</td>"
                            str = str + "<td style='border:1px solid #e1e1e1;padding:3px'> " + res.data[i].ResourceName + "</td>"
                            str = str + "<td style='border:1px solid #e1e1e1;padding:3px'> " + res.data[i].ProjectName + "</td>"
                            str = str + "<td style='border:1px solid #e1e1e1;padding:3px'> " + res.data[i].CrNumber + "</td>"
                            str = str + "<td style='border:1px solid #e1e1e1;padding:3px'> " + res.data[i].Activity + "</td>"
                            str = str + "<td style='border:1px solid #e1e1e1;padding:3px'> " + res.data[i].SubActivity + "</td>"
                            str = str + "<td style='border:1px solid #e1e1e1;padding:3px'> " + res.data[i].Efforts + "</td>"
                            str = str + "<td style='border:1px solid #e1e1e1;padding:3px'> " + res.data[i].Efforts_Days + "</td>"
                            str = str + "<td style='border:1px solid #e1e1e1;padding:3px'> " + res.data[i].Status + "</td>"
                            str = str + "</tr>";

                        }
                        
                        var wheight = window.screen.availHeight - 150;
                        console.log(wheight);
                        var dynamicDialog = $('<div id="rundialog"><table style="border:1px solid #d9d99">' + str + '</table></div>');
                        dynamicDialog.dialog({
                            title: "Invalid Timesheet",
                            modal: true,
                            width: '90%',
                            maxHeight: wheight ,

                        });
                    }
                }
                else {
                    toastr.options.positionClass = 'toast-top-center';
                    toastr.options.timeOut = '1000';
                    toastr["success"](res.ps_Msg);
                    jQuery("#list").jqGrid('clearGridData').trigger('reloadGrid');
                    $("#list").jqGrid('setGridParam', { data: res.data }).trigger('reloadGrid');

                    _file.files = null;
                    $('#file1').val("");
                    $('#upload-file-info').html("");
                    $("#divbulk").dialog("close");
                }

                
                //$(result_output).html(res); //output response from server
                $(btn).removeAttr('disabled');
            }
        }).done(function (res) { //
            $(btn).removeAttr('disabled');
            
        })
        .fail(function (err) {
            
            console.log(err);
            toastr["error"](err);
        });
    }

}


var onSearchSuccess = function (result) {
    jQuery("#list").jqGrid('clearGridData').trigger('reloadGrid');
    var data = [];
    if (result.length == undefined) {
        data.push(result);

    }
    else {
        data = result;

    }
    if (data.length == 0) {
        toastr.options.positionClass = 'toast-top-center';
        toastr.options.timeOut = '1000';
            toastr["error"]("No record found...");
    }
    else {


        jQuery("#list").jqGrid('setGridParam', { data: result }).trigger("reloadGrid");
    }

}


function ClearCreateForm() {
    $("#Id").val("");
    $('#ActivityId option').removeAttr('selected');
    $("#ResourceId option").removeAttr('selected');

    $("#ProjectId option").removeAttr('selected');

    $("#CrNumberId option").removeAttr('selected');

    $("#IsBillable option").removeAttr('selected');

    $("#SubActivity").val("");
    $("#Efforts").val("");
    $("#Comments").val("");
    $("#IsSubmit").removeAttr("checked");
    $("#Activitydate").val("");
    $("#save").val("Save");
}