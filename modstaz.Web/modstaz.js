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

// function createStorageArea(form) {
//     var userId = form.userId.value;
//     var storageAreaName = form.storageAreaName.value;
//     console.log(`user id = ${userId}, storage area name = ${storageAreaName}`);

//     console.log("Creating Storage area!");
//     var request = new XMLHttpRequest();
//     request.open('POST', `${apiBaseUrl}/api/CreateStorageArea`, true);

//     let data = {
//         "StorageAreaName": storageAreaName,
//         "UserId": userId
//     };

//     request.send(JSON.stringify(data));

//     request.onload = function () {
//         var resp = this.response;
//         console.log(resp);
//     };

//     request.onerror = function () { };
// }

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

function editRow(form) {
    fields = "";
    for (var i = 0; i < form.length - 1; i++) {
        fields += `"${form[i].name}": "${form[i].value}",`;
    }
    fields = fields.slice(0, -1);

    data = {
        StorageAreaId: localStorage.getItem('storageAreaId'),
        RowFieldsArray: [{
            RowId: localStorage.getItem('rowId'),
            Fields: JSON.parse(`{${fields}}`)
        }]
    };

    console.log("Editing Columns!");
    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/EditRows`, true);

    request.send(JSON.stringify(data));

    request.onload = function () {
        var resp = this.response;
        console.log(resp);
    };

    request.onerror = function () { };
}


function isUserAuthenticated() {
    var jwt = localStorage.getItem('JWT');
    console.log(jwt);

    if (jwt === null) {
        window.location.href = "/login.html";
    }

    console.log("validating JWT!");
    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/ParseJWT`, true);

    let data = {
        "JWT": jwt
    };

    request.send(JSON.stringify(data));

    request.onload = function () {
        var resp = JSON.parse(this.response);
        console.log(resp.userId);
        userId = resp.userId;
    };

    request.onerror = function () { };
}


function redirectToLogin() {
    window.location.replace("/login.html");
}


function getGetParameters() {
    var qd = {};
    if (location.search) location.search.substr(1).split("&").forEach(function (item) {
        var s = item.split("="),
            k = s[0],
            v = s[1] && decodeURIComponent(s[1]); //  null-coalescing / short-circuit
        //(k in qd) ? qd[k].push(v) : qd[k] = [v]
        (qd[k] = qd[k] || []).push(v) // null-coalescing / short-circuit
    })

    return qd;
}



