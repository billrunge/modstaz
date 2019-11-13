var apiBaseUrl = "http://localhost:7071";

function createInstance() {
    console.log("Creating instance!");
    var request = new XMLHttpRequest();
    request.open('GET', `${apiBaseUrl}/api/DatabaseSetup`, true);

    request.onload = function () {
        var resp = this.response;
        console.log(resp);
    };

    request.onerror = function () { };
    request.send();
}

function destroyInstance() {
    console.log("Destroying instance!");
    var request = new XMLHttpRequest();
    request.open('GET', `${apiBaseUrl}/api/DatabaseDestroy`, true);

    request.onload = function () {
        var resp = this.response;
        console.log(resp);
    };

    request.onerror = function () { };
    request.send();
}

function createColumn(form) {
    var storageAreaId = form.storageAreaId.value;
    var displayName = form.displayName.value;
    var columnTypeId = form.columnTypeId.value;
    console.log(`storage area id = ${storageAreaId}, display name = ${displayName}, columnTypeID = ${columnTypeId}`);

    console.log("Creating Field!");
    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/CreateColumns`, true);

    let data = {
        "StorageAreaID": storageAreaId,
        "ColumnArray": [
            {
                ColumnTypeID: columnTypeId,
                DisplayName: displayName
            }
        ],

    };

    request.send(JSON.stringify(data));

    request.onload = function () {
        var resp = this.response;
        console.log(resp);
    };

    request.onerror = function () { };
}


function createStorageArea(form) {
    var userId = form.userId.value;
    var storageAreaName = form.storageAreaName.value;
    console.log(`user id = ${userId}, storage area name = ${storageAreaName}`);

    console.log("Creating Storage area!");
    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/CreateStorageArea`, true);

    let data = {
        "StorageAreaName": storageAreaName,
        "UserId": userId
    };

    request.send(JSON.stringify(data));

    request.onload = function () {
        var resp = this.response;
        console.log(resp);
    };

    request.onerror = function () { };
}

function deleteStorageArea(form) {
    var storageAreaId = form.storageAreaId.value;
    console.log(`storage area id = ${storageAreaId}`);

    console.log("Deleting Storage Area!");
    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/DeleteStorageArea`, true);

    let data = {
        "StorageAreaID": storageAreaId
    };

    request.send(JSON.stringify(data));

    request.onload = function () {
        var resp = this.response;
        console.log(resp);
    };

    request.onerror = function () { };
}

function getStorageAreas(form) {
    var userId = form.userId.value;
    console.log(`user id = ${userId}`);

    console.log("Getting Storage Areas!");
    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/GetStorageAreas`, true);

    let data = {
        "UserId": userId
    };

    request.send(JSON.stringify(data));

    request.onload = function () {
        var html = jsonToTable(JSON.parse(this.response), "storageAreasList");
        console.log(html);

        document.getElementById('storageAreas').innerHTML = html;
    };

    request.onerror = function () { };
}

function getStorageArea(form) {
    var storageAreaId = form.storageAreaId.value;
    console.log(`storage area id = ${storageAreaId}`);

    console.log("Getting Storage Areas!");
    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/GetStorageArea`, true);

    let data = {
        "StorageAreaId": storageAreaId
    };

    request.send(JSON.stringify(data));

    request.onload = function () {
        var html = jsonToTable(JSON.parse(this.response), "storageAreaList");
        console.log(html);

        document.getElementById('storageAreaItems').innerHTML = html;
    };

    request.onerror = function () { };
}

function getStorageAreaColumns(form) {
    var storageAreaId = form.storageAreaId.value;
    console.log(`storage area id = ${storageAreaId}`);

    console.log("Getting Storage Area Columns!");
    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/GetStorageAreaColumns`, true);

    let data = {
        "StorageAreaId": storageAreaId
    };

    request.send(JSON.stringify(data));

    request.onload = function () {
        var html = jsonToTable(JSON.parse(this.response), "storageAreaColumnsList");
        console.log(html);

        document.getElementById('storageAreaColumns').innerHTML = html;
    };

    request.onerror = function () { };
}

function createUser(form) {
    var emailAddress = form.emailAddress.value;
    console.log(`email address = ${emailAddress}`);

    console.log("Creating User!");
    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/CreateUser`, true);

    let data = {
        "EmailAddress": emailAddress
    };

    request.send(JSON.stringify(data));

    request.onload = function () {
        var resp = this.response;
        console.log(resp);
    };

    request.onerror = function () { };
}


function getColumnsToInsert(form) {
    var storageAreaId = form.storageAreaId.value;
    localStorage.setItem('storageAreaId', storageAreaId);
    console.log(`storage area id = ${storageAreaId}`);

    console.log("Getting Storage Area Columns!");
    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/GetStorageAreaColumns`, true);

    let data = {
        "StorageAreaId": storageAreaId
    };

    request.send(JSON.stringify(data));

    request.onload = function () {
        var html = jsonToForm(JSON.parse(this.response), "storageAreaColumns", "insertRow");
        console.log(html);

        document.getElementById('columnsToUpdate').innerHTML = html;
    };

    request.onerror = function () { };
}

function insertRow(form) {
    console.log("Inserting Row!");

    let fieldsArrayString = "";

    for (var i = 0; i < form.length - 1; i++) {
        fieldsArrayString += `"${form[i].name}": "${form[i].value}",`;
    }

    fieldsArrayString = fieldsArrayString.slice(0, -1);

    let data = {
        "StorageAreaId": localStorage.getItem('storageAreaId') || 0,
        "FieldsArray": [ `{${JSON.parse(fieldsArrayString)}}` ]
    };

    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/InsertRows`, true);

    request.send(JSON.stringify(data));

    request.onload = function () {
        console.log(this.response);
    };
    request.onerror = function () { };
}

function getColumnsToEdit(form) {
    var storageAreaId = form.storageAreaId.value;
    var rowId = form.rowId.value;
    localStorage.setItem('storageAreaId', storageAreaId);
    localStorage.setItem('rowId', rowId);
    console.log(`storage area id = ${storageAreaId}, row id = ${rowId}`);

    console.log("Getting Storage Area Columns to Edit!");
    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/GetRow`, true);

    let data = {
        "StorageAreaId": storageAreaId,
        "RowId": rowId,
        "OnlyShowEditable": true
    };

    request.send(JSON.stringify(data));

    request.onload = function () {
        var resp = this.response;
        var json = JSON.parse(resp);
        var html = jsonToFormEdit(json, "editRow", "editRow");
        console.log(html);

        document.getElementById('columnsToUpdate').innerHTML = html;
    };

    request.onerror = function () { };
}

function editRow(form){
    console.log("bananas!");
}




