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
            if (resp == "Invalid JWT") {
                redirectToLogin();
            }
            var html = jsonToForm(JSON.parse(resp), "storageAreaColumns", "insertRow");
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
        if (resp == "Invalid JWT") {
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

function jsonToForm(json, className, functionName) {

    let html = "";

    json.map(function (row) {
        if (row['IsEditable'] === true) {
            switch (row['ColumnTypeID']) {
                case 1:
                    html += `${row['DisplayName']}: <select name="${row['ID']}">
                    <option value="NULL">Unset</option>
                    <option value="1">Yes</option>
                    <option value="0">No</option>
                  </select>`
                    break;
                case 2:
                    html += `${row['DisplayName']}: <input type="number" name="${row['ID']}"><br>`;
                    break;
                case 3:
                    html += `${row['DisplayName']}: <input type="number" name="${row['ID']}"><br>`;
                    break;
                case 4:
                    html += `${row['DisplayName']}: <input type="text" name="${row['ID']}"><br>`;
                    break;
                case 5:
                    html += `${row['DisplayName']}: <input type="text" name="${row['ID']}"><br>`;
                    break;
                case 6:
                    html += `${row['DisplayName']}: <input type="text" name="${row['ID']}"><br>`;
                    break;
                case 7:
                    html += `${row['DisplayName']}: <input type="text" name="${row['ID']}"><br>`;
                    break;
                case 8:
                    html += `${row['DisplayName']}: <input type="date" name="${row['ID']}"><br>`;
                    break;
            }
        }
    });

    return `<form id="${className}">${html}
    <br>
    <button onclick="${functionName}(this.form)"type="button">Save</button>
    </form>`;
}