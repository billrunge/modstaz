function storageAreaJsonToTable(json, className) {
    let cols;
    if (json[0] === undefined){
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
        bodyRows += `<td><button onclick="deleteRow(${row["ID"]})" type="button">Delete</button></td>`; 
        bodyRows += '</tr>';
    });
    return `<table class="${className}"><tr>${headerRow}</tr>${bodyRows}</table>`;
}


function storageAreasJsonToTable(json, className) {

    let cols;
    if (json[0] === undefined){
        cols = [];
    } else {
        cols = Object.keys(json[0]);
    }

    let headerRow = '';
    let bodyRows = '';

    headerRow += `<th></th>`;

    cols.map(function (col) {
        if (col != 'ID') {
            headerRow += `<th>${col}</th>`;
        }
    });    

    json.map(function (row) {
        bodyRows += '<tr>';
        cols.map(function (colName) {
            if (colName == 'Name') {
                bodyRows += `<td><a href="../GetStorageArea.html?ID=${row["ID"]}">${row[colName]}</a></td>`;
            } else if (colName != 'ID'){
                bodyRows += `<td>${row[colName]}</td>`;                
            }
        })
        bodyRows += `<td><button onclick="deleteStorageArea(${row["ID"]})" type="button">Delete</button></td>`; 
        bodyRows += '</tr>';
    });
    return `<table class="${className}"><tr>${headerRow}</tr>${bodyRows}</table>`;
}

function columnsJsonToTable(json, className) {

    let cols;
    if (json[0] === undefined){
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
        bodyRows += `<td><button onclick="deleteColumn(${row["ID"]})" type="button">Delete</button></td>`; 
        bodyRows += '</tr>';
    });
    return `<table class="${className}"><tr>${headerRow}</tr>${bodyRows}</table>`;
}