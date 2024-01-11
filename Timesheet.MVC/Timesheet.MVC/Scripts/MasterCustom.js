/// <reference path="jquery.jqGrid.js" />

/*======================================Global Variables======================================================================== */
var mastermodal = [];
var parameters = {
    edit: true,
    editicon: "ui-icon-pencil",
    add: true,
    del: true,
    addicon: "ui-icon-plus",
    url: 'clientArray',
    cancel: true,
    cancelicon: "ui-icon-cancel",

    closeAfterEdit: true
};




var emptyModal = {
    'ColModal': [],
    'ColNameModal': []
};



var ActivityMaster = {
    'ColModal': [
        { name: "s_MasterCode", width: 250, edittype: 'text', editrules: { required: true }, editable: true },
        { name: "b_IsActive", width: 130, edittype: 'select', editable: true, required: true, editoptions: { value: "true:Yes;false:No" } },
    ],
    'ColNameModal': ["Activity Name", "Active Status"],
    'button': true
},
    ProjectMaster = {
        'ColModal': [
            { name: "s_MasterCode", width: 250, edittype: 'text', editrules: { required: true }, editable: true },
            { name: "s_MasterName", width: 250, edittype: 'text', editrules: { required: true }, editable: true },
            { name: "value1", width: 250, edittype: 'text', editrules: { required: true, number: true }, editable: true },
            { name: "value2", width: 250, edittype: 'text', editrules: { required: true, number: true }, editable: true },
            { name: "value3", width: 250, edittype: 'text', editrules: { required: true, number: true }, editable: true },
            { name: "s_value5", width: 250, edittype: 'text', editrules: { required: true }, editable: true },
            { name: "s_value6", width: 130, edittype: 'select', editable: true, required: true, editoptions: { value: "Support:Support;Other:Other" } },
            { name: "b_IsActive", width: 130, edittype: 'select', editable: true, required: true, editoptions: { value: "true:Yes;false:No" } },



        ],
        'ColNameModal': ["Project Code", "Project Name", "SDLC Cost", "Dev Cost", "QA Cost", "Onsite Manager Name", "Project Type", "Active Status"],
        'button': true
    },
    // CrtypeMaster added as new module (accesible to admin role)
    CrTypeMaster = {
        'ColModal': [
            { name: "s_MasterCode", width: 250, edittype: 'text', editrules: { required: true }, editable: true },
            { name: "s_MasterName", width: 250, edittype: 'text', editrules: { required: true }, editable: true },
            { name: "b_IsActive", width: 130, edittype: 'select', editable: true, required: true, editoptions: { value: "true:Yes;false:No" } },

        ],
        'ColNameModal': ["CR Type Code", "CR Type Name", "Is Active"],
        'button': true
    },
    TasksMaster = {
        'ColModal': [
            { name: "s_MasterCode", width: 250, edittype: 'text', editrules: { required: true }, editable: true },
            { name: "s_MasterName", width: 250, edittype: 'text', editrules: { required: true }, editable: true },
            { name: "b_IsActive", width: 130, edittype: 'select', editable: true, required: true, editoptions: { value: "true:Yes;false:No" } },

        ],
        'ColNameModal': ["Task Id", "Task Name", "Is Active"],
        'button': true
            },

    CRMaster = {
        'ColModal': [
            
            { name: "s_MasterCode", width: 250, edittype: 'text', editrules: { required: true }, editable: true, editoptions: { size: 8, maxlength: 8 }},

            { name: "s_MasterName", width: 250, edittype: 'text', editrules: { required: true }, editable: true, editoptions: { size: 8, maxlength: 8 } },
            { name: "s_value5", width: 250, edittype: 'text', editrules: { required: true }, editable: true },

            { name: "n_RefId", index: 'n_RefId', width: 250, editable: true, hidden: true, edittype: 'select', editrules: { edithidden: true }, required: true, editoptions: { dataUrl: '/Master/GetDropDownValues?parentid=3&IsActive=true', custom_value: myvalue } },
            { name: "ParentName", index: 'ParentName', width: 250, editable: false, edittype: 'hidden', editrules: { edithidden: false }, required: true, editoptions: { dataUrl: '/Master/Master?parentid=3', custom_value: myvalue } },
            // Parent Id for CR Type  is = 4301 on Prod and 4272 on Dev in LookUp Master
            /*Column visible as CR type in CR Number*/
            { name: "s_value6", index: 's_value6', width: 250, editable: true, hidden: true, edittype: 'select', editrules: { edithidden: true }, required: true, editoptions: { dataUrl: '/Master/GetDropDownValues?parentid=4272&IsActive=true', custom_value: myvalue } },

            /*dropdown for CR type*/
            { name: "CRTypeName", index: 'CRTypeName', width: 250, editable: false, edittype: 'hidden', editrules: { edithidden: false }, required: true, editoptions: { dataUrl: '/Master/Master?parentid=4272', custom_value: myvalue } },



            { name: "value1", width: 250, edittype: 'text', editrules: { required: true, number: true }, editable: true },
            { name: "value2", width: 250, edittype: 'text', editrules: { required: true, number: true }, editable: true },
            { name: "value3", width: 250, edittype: 'text', editrules: { required: true, number: true }, editable: true },
            { name: "s_value4", width: 130, edittype: 'checkbox', editable: true, required: true, editoptions: { value: "true:false", defaultValue: "false" } },
            { name: "b_IsActive", width: 130, edittype: 'checkbox', editable: true, required: true, editoptions: { value: "true:false", defaultValue: "false" } },
        ],
        'ColNameModal': ["CR Code", "CR Name", "CR Long Name", "Project", "Project Name", "CR Type", "CR Type", "SDLC Cost", "Dev Cost", "QA Cost", "Is OLS CR", "IsActive"],
        'button': true
    }, 
ResourceMaster = {
    'ColModal': [
        { name: "s_MasterCode", width: 250, edittype: 'text', editrules: { required: true }, editable: true },
        { name: "s_MasterName", width: 250, edittype: 'text', editrules: { required: true }, editable: true },
        { name: "n_RefId", index: 'n_RefId', width: 250, editable: true, hidden: true, edittype: 'select', editrules: { edithidden: true }, required: true, editoptions: { dataUrl: '/Master/Master?parentid=5', custom_value: myvalue } },
        { name: "ParentName", index: 'ParentName', width: 250, editable: false, edittype: 'hidden', editrules: { edithidden: false }, required: true, editoptions: { dataUrl: '/Master/Master/parentid=5', custom_value: myvalue } },
        { name: "value1", width: 130, edittype: 'select', editable: true, required: true, editoptions: { value: "Developer:Developer;Quality Analyst:Quality Analyst;Project Manager:Project Manager;Admin:Admin " } },
        { name: "s_value5", width: 130, edittype: 'select', editable: true, required: true, editoptions: { value: "Developer:Developer;Sr.Developer:Sr.Developer;Sr.TSA:Sr.TSA;Sr.QA:Sr.QA;Sr.QA Analyst:Sr.QA Analyst;Sr.Tester:Sr.Tester;UI Developer:UI Developer;Tech Lead:Tech Lead;Sr.Tech Lead:Sr.Tech Lead;Test Lead:Test Lead;Team Leader:Team Leader;Project Manager:Project Manager;Project Administrator:Project Administrator;Sr.Solutions Architect:Sr.Solutions Architect;Salesforce Tester:Salesforce Tester" } },
        { name: "s_value6", width: 130, edittype: 'select', editable: true, required: true, editoptions: { value: "Support:Support;TNM:TNM;Other:Other" } },
        { name: "value2", width: 250, edittype: 'text', editrules: { required: true, number: true }, editable: true },
        { name: "value3", width: 130, edittype: 'select', editable: true, required: true, editoptions: { dataUrl: '/Master/GetLocations', custom_value: myvalue } },
        { name: "s_value4", width: 250, edittype: 'text', editrules: { required: false, number: true }, editable: true },
        { name: "b_IsActive", width: 130, edittype: 'checkbox', editable: true, required: true, editoptions: { value: "true:Yes;false:No" } },
    ],
    'ColNameModal': ["Login Id", "Resource Name", "Manager Name", "Manager Name", "Team","RBC Role","Resource Type", "Per Day Cost", "Location", "OLS Rate", "Active Status"],
    'button': true
},

    YearMaster = {
        'ColModal': [
            { name: "s_MasterCode", width: 250, edittype: 'text', editrules: { required: true }, editable: true },
            { name: "s_MasterName", width: 250, edittype: 'text', editrules: { required: true }, editable: true },
            {
                name: "value1", width: 250, edittype: 'text', editrules: { required: true }, editable: true, editoptions: {
                    size: 10, maxlengh: 10,
                    dataInit: function (element) {
                        $(element).datepicker({ dateFormat: 'mm/dd/yy' })
                    }
                }
            },
            {
                name: "value2", width: 250, edittype: 'text', editrules: { required: true }, editable: true, editoptions: {
                    size: 10, maxlengh: 10,
                    dataInit: function (element) {
                        $(element).datepicker({ dateFormat: 'mm/dd/yy' })
                    }
                }
            },

            { name: "b_IsActive", width: 130, edittype: 'checkbox', editable: true, required: true, editoptions: { value: "true:Yes;false:No" } },
        ],
        'ColNameModal': ["Month Code", "Month Name", "Start Date", "End Date", "Active Status"],
        'button': true
    },
    //WeekMaster = {
    //    'ColModal': [
    //                { name: "s_MasterCode", width: 250 },
    //                { name: "value1", width: 250 },
    //                { name: "value2", width: 250 },
    //                { name: "b_IsActive", width: 130 }
    //    ],
    //    'ColNameModal': ["Week#", "Start Day", "End Day", "Active Status"],
    //    'button': false
    //},
    StatusMaster = {
        'ColModal': [
            { name: "s_MasterCode", width: 250 },
            { name: "s_MasterName", width: 250 },
            { name: "value1", width: 250, hidden: true },
            { name: "value2", width: 250 },
            { name: "value3", width: 250 },
            { name: "b_IsActive", width: 130 }
        ],
        'ColNameModal': ["Action Name", "Display Name", "Status Val", "Status For", "Group Code ", "Active Status"],
        'button': false
    },
    MniMaxDayMaster = {
        'ColModal': [
            { name: "s_MasterCode", width: 250, edittype: 'text', editrules: { required: true }, editable: true },
            { name: "s_MasterName", width: 250, edittype: 'text', editrules: { required: true }, editable: true },
            { name: "value1", width: 250, edittype: 'text', editrules: { required: true }, editable: true },
                { name: "b_IsActive", width: 130 ,edittype: 'text', editrules: { required: true }, editable: true}
        ],
        'ColNameModal': ["Code", "Name", "Value", "Active Status"],
        'button': true
    },
    HolidayMaster = {
        'ColModal': [

                { name: "s_MasterCode", width: 250, edittype: 'text', editrules: { required: true }, editable: true, editoptions: {
                    size: 10, maxlengh: 10,
                    dataInit: function (element) {
                        $(element).datepicker({ dateFormat: 'mm/dd/yy' })
                    }
                }
            },
            { name: "s_MasterName", idth: 250, edittype: 'text', editrules: { required: true }, editable: true },

            // { name: "value2", width: 250, edittype: 'text', editrules: { required: true }, editable: true },
            { name: "value3", width: 130, edittype: 'select', editable: true, required: true, editoptions: { dataUrl: '/Master/GetLocations', custom_value: myvalue } },
            { name: "b_IsActive", width: 130, edittype: 'text', editrules: { required: true }, editable: true },

        ],
        'ColNameModal': ["Holiday Date", "Holiday Description", "Location", "Active Status"],
        'button': true
    }


mastermodal.push(ActivityMaster);
mastermodal.push(CRMaster);
/*CR Type pushed to masterModal */
mastermodal.push(CrTypeMaster);

mastermodal.push(HolidayMaster);
mastermodal.push(MniMaxDayMaster);
mastermodal.push(ProjectMaster);
mastermodal.push(ResourceMaster);
mastermodal.push(StatusMaster);
//mastermodal.push(WeekMaster);
mastermodal.push(TasksMaster);
mastermodal.push(YearMaster);




/*======================================Global Variables End======================================================================== */

/*======================================Page Level======================================================================== */
$(document).ready(function () {
    $("#MasterName").change(function () {
        
        $.jgrid.gridUnload('list');
        loadGrid(mastermodal[$("#MasterName option:selected").index() - 1]);
        $('#list').trigger('reloadGrid');
        $('form#searchForm').trigger('submit');

    });
    loadGrid(emptyModal);

});
/*======================================Page Level Ends======================================================================== */


/*======================================Name Functions ======================================================================== */
function loadGrid(modal) {
/*    alert("Hello");
*/    modal.ColNameModal.push("n_Id")
    modal.ColModal.push({ name: "n_Id", hidden: true, edittype: 'input', editable: true, editrules: { custom: true, custom_func: validateRef, required: true }, editoptions: { defaultValue: $("#MasterName").val() } });

    modal.ColNameModal.push("Master")
    modal.ColModal.push({ name: "n_ParentId", hidden: true, edittype: 'input', editable: true, editrules: { required: true }, editoptions: { defaultValue: $("#MasterName").val() } });

    jQuery("#list").jqGrid({

        datatype: "local",
        autowidth: true,
        altRows: false,
        colNames: modal.ColNameModal,
        colModel: modal.ColModal,
        pager: "#listPager",
        rowNum: 100,
        height: 500,
        viewrecords: true,
        gridview: true,
        autoencode: true,
        caption: "",
        editurl: '/Master/AddUpdateMaster',
        edit: {

            closeAfterEdit: true,

        }

    });
    if (modal.button == true) {
        /*        alert("Hello1");
        */
        $('#list').jqGrid('navGrid', '#listPager', parameters, {
            closeAfterEdit: true, afterSubmit: function (response) {
                var resObj = JSON.parse(response.responseText);

                if (resObj.pn_Error) {
                    toastr["error"](resObj.ps_Msg);
                    return [false, resObj.ps_Msg, null];
                }
                else {
                    toastr["success"](resObj.ps_Msg);
                    $('form#searchForm').trigger('submit');
                    //$('#cData').trigger('click');
                    return [true, resObj.ps_Msg, null];


                }
                //console.log(response);
                //alert(response);

            }
        },
            {
                afterSubmit: function (response) {
                    var resObj = JSON.parse(response.responseText);
                    console.log(response)
                    if (resObj.pn_Error) {
                        toastr["error"](resObj.ps_Msg);
                        return [false, resObj.ps_Msg, null];
                    }
                    else {
                        toastr["success"](resObj.ps_Msg);
                        $('form#searchForm').trigger('submit');
                        //$('#cData').trigger('click');

                        return [true, resObj.ps_Msg, null];
                    }
                    //console.log(response);
                    //alert(response);

                }, closeAfterAdd: true,


            });
    }
}
function customelement(value, options) {
    var el = document.createElement("label");
    el.innerText = $("#MasterName option:selected").text();
    // el.textContent = $("#MasterName option:selected").text();
    return el;

}


function myvalue(elem, operation, value) {
    if (operation === 'get') {
        return $(elem).val();
    } else if (operation === 'set') {
        $('input', elem).val(value);
    }
}


function validateRef(value, ColName, NextRowId, PrevRowId) {   
    var SelfId = value;
    //var ManagerId = $("#n_RefId option:selected").val();
    /*check with n_id*/
    //if (SelfId == ManagerId) {
    //    return [false, "Parent could not be same as user"];

    //}
    //else {
    return [true, ""];
    //}


}
function FillData(data) {
    jQuery("#divdata").html(data.Data);

}
function ToggleCreate() {
    $("#divcreate").slideToggle();

}


/*======================================Named Functions End======================================================================== */

/*======================================Anonymouos Functions Events======================================================================== */

var onSuccess = function (result) {
    if (Array.isArray(result))
        jQuery("#list").jqGrid('setGridParam', { data: result }).trigger("reloadGrid");
    else
        window.location.href = window.location.origin +
            "/Account/Logout?DisplayMessage=Session Timed out! Please Login Again!";
}



/*======================================Anonymouos Functions Events======================================================================== */