function storageAreaJsonToTable(json, className, storageAreaId) {
    let cols;
    if (json[0] === undefined) {
        cols = [];
    } else {
        cols = Object.keys(json[0]);
    }

    let headerRow = '';
    let bodyRows = '';

    headerRow += `<th></th>`;

    cols.map(function (col) {
        headerRow += `<th>${col}</th>`;
    });

    headerRow += `<th></th>`;


    json.map(function (row) {
        bodyRows += '<tr>';
        bodyRows += `<td><a href="/editRow.html?StorageAreaId=${storageAreaId}&RowId=${row["Id"]}">Edit</a></td>`;
        cols.map(function (colName) {
            bodyRows += `<td>${row[colName]}</td>`;
        })

        bodyRows += `<td><button onclick="deleteRow(${row["Id"]})" type="button">Delete</button></td>`;
        bodyRows += '</tr>';
    });
    return `<table class="${className}"><tr>${headerRow}</tr>${bodyRows}</table>`;
}


function storageAreasJsonToTable(json, className) {

    let cols;
    if (json[0] === undefined) {
        cols = [];
    } else {
        cols = Object.keys(json[0]);
    }

    let headerRow = '';
    let bodyRows = '';

    headerRow += `<th></th>`;

    cols.map(function (col) {
        if (col != 'Id') {
            headerRow += `<th>${col}</th>`;
        }
    });

    json.map(function (row) {
        bodyRows += '<tr>';
        cols.map(function (colName) {
            if (colName == 'Name') {
                bodyRows += `<td><a href="../GetStorageArea.html?ID=${row["Id"]}">${row[colName]}</a></td>`;
            } else if (colName != 'Id') {
                bodyRows += `<td>${row[colName]}</td>`;
            } else {
                bodyRows += `<td></td>`;
            }
        })
        bodyRows += `<td><button onclick="deleteStorageArea(${row["Id"]})" type="button">Delete</button></td>`;
        bodyRows += '</tr>';
    });
    return `<table class="${className}"><tr>${headerRow}</tr>${bodyRows}</table>`;
}



function columnsJsonToTable(json, className) {

    let cols;
    if (json[0] === undefined) {
        cols = [];
    } else {
        cols = Object.keys(json[0]);
    }
    let headerRow = '';
    let bodyRows = '';

    cols.map(function (col) {
        headerRow += `<th>${col}</th>`;
    });

    json.map(function (row) {
        bodyRows += '<tr>';
        cols.map(function (colName) {
            bodyRows += `<td>${row[colName]}</td>`;
        })
        if (row["IsEditable"] === true) {
            bodyRows += `<td><button onclick="deleteColumn(${row["Id"]})" type="button">Delete</button></td>`;
        } else {
            bodyRows += `<td></td>`;
        }

        bodyRows += '</tr>';
    });
    return `<table class="${className}"><tr>${headerRow}</tr>${bodyRows}</table>`;
}