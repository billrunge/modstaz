let storageAreaId;
getColumnsToInsert();
insertInsertRowLinks();

function getColumnsToInsert() {
    let params = getGetParameters();

    if (params.ID != undefined) {
        storageAreaId = params.ID[0];
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
    } else {
        window.location.replace("../getStorageAreas.html");
    }

}

function insertRow(form) {
    console.log("Inserting Row!");

    let fieldsArrayString = "";

    for (var i = 0; i < form.length - 1; i++) {
        fieldsArrayString += `"${form[i].name}":"${form[i].value}",`;
    }

    fieldsArrayString = fieldsArrayString.slice(0, -1);

    console.log(JSON.parse(`{ ${fieldsArrayString} }`));

    let data = {
        "StorageAreaId": storageAreaId || 0,
        "FieldsArray": [JSON.parse(`{ ${fieldsArrayString} }`)]
    };

    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/InsertRows`, true);

    request.send(JSON.stringify(data));

    request.onload = function () {
        console.log(this.response);
    };
    request.onerror = function () { };
}

function insertInsertRowLinks() {
    let params = getGetParameters();

    if (params.ID != undefined) {
        let storageAreaId = params.ID[0];
        let html = "";

        html += `<a href="/createColumn.html?ID=${storageAreaId}">Create Column</a><br>`;
        html += `<a href="/getStorageArea.html?ID=${storageAreaId}">Return to Storage Area</a><br>`;
        html += `<a href="/getStorageAreas.html">Storage Areas</a><br>`;
        html += `<a href="/index.html">Home</a><br>`;
        document.getElementById('insertRowLinks').innerHTML = html;

    }

}