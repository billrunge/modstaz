function getColumnsToEdit(form) {
    var storageAreaId = form.storageAreaId.value;
    var rowId = form.rowId.value;
    let jwt = localStorage.getItem('JWT');
    localStorage.setItem('storageAreaId', storageAreaId);
    localStorage.setItem('rowId', rowId);
    console.log(`storage area id = ${storageAreaId}, row id = ${rowId}`);

    console.log("Getting Storage Area Columns to Edit!");
    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/GetRow`, true);

    let data = {
        "StorageAreaId": storageAreaId,
        "RowId": rowId,
        "OnlyShowEditable": true,
        "JWT": jwt
    };

    request.send(JSON.stringify(data));

    request.onload = function () {
        var resp = this.response;
        if (resp == "Invalid JWT")
        {
            redirectToLogin();
        }
        var json = JSON.parse(resp);
        var html = jsonToFormEdit(json, "editRow", "editRow");
        console.log(html);

        document.getElementById('columnsToUpdate').innerHTML = html;
    };

    request.onerror = function () { };
}

function editRow(form) {
    let fields = "";
    let jwt = localStorage.getItem('JWT');

    for (var i = 0; i < form.length - 1; i++) {
        fields += `"${form[i].name}": "${form[i].value}",`;
    }
    fields = fields.slice(0, -1);

    data = {
        "StorageAreaId": localStorage.getItem('storageAreaId'),
        "JWT": jwt,
        "RowFieldsArray": [{
            RowId: localStorage.getItem('rowId'),
            Fields: JSON.parse(`{${fields}}`)
        }]
    };

    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/EditRows`, true);

    request.send(JSON.stringify(data));

    request.onload = function () {
        var resp = this.response;
        if (resp == "Invalid JWT")
        {
            redirectToLogin();
        }
        console.log(resp);
    };

    request.onerror = function () { };
}