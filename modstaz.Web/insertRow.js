let storageAreaId;
isUserAuthenticated();
getColumnsToInsert();
insertInsertRowLinks();

function getColumnsToInsert() {
    let params = getGetParameters();
    let jwt = localStorage.getItem('JWT');

    if (params.ID != undefined) {
        storageAreaId = params.ID[0];
        var request = new XMLHttpRequest();
        request.open('POST', `${apiBaseUrl}/api/GetStorageAreaColumns`, true);

        let data = {
            "StorageAreaId": storageAreaId,
            "JWT": jwt
        };

        request.send(JSON.stringify(data));

        request.onload = function () {
            let resp = this.response;
            if (resp == "Invalid JWT")
            {
                redirectToLogin();
            }
            var html = jsonToForm(JSON.parse(resp), "storageAreaColumns", "insertRow");
            console.log(html);

            document.getElementById('columnsToUpdate').innerHTML = html;
        };

        request.onerror = function () { };
    } else {
        window.location.replace("../getStorageAreas.html");
    }

}

function insertRow(form) {

    let fieldsArrayString = "";
    let jwt = localStorage.getItem('JWT');

    for (var i = 0; i < form.length - 1; i++) {
        fieldsArrayString += `"${form[i].name}":"${form[i].value}",`;
    }

    fieldsArrayString = fieldsArrayString.slice(0, -1);

    console.log(JSON.parse(`{ ${fieldsArrayString} }`));

    let data = {
        "StorageAreaId": storageAreaId || 0,
        "FieldsArray": [JSON.parse(`{ ${fieldsArrayString} }`)],
        "JWT": jwt
    };

    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/InsertRows`, true);

    request.send(JSON.stringify(data));

    request.onload = function () {
        let resp = this.response;
        if (resp == "Invalid JWT")
        {
            redirectToLogin();
        }
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