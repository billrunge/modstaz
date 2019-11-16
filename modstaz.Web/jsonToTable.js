function jsonToTable(json, className) {

    let cols = Object.keys(json[0]);
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
        bodyRows += '</tr>';
    });
    return `<table class="${className}"><tr>${headerRow}</tr>${bodyRows}</table>`;
}


function storageAreasJsonToTable(json, className) {

    let cols = Object.keys(json[0]);
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
        bodyRows += `<td><button onclick="deleteStorageArea(${row["ID"]})" type="button">Delete</button></td>`; 
        cols.map(function (colName) {
            if (colName == 'Name') {
                bodyRows += `<td><a href="../GetStorageArea.html?ID=${row["ID"]}">${row[colName]}</a></td>`;
            } else if (colName != 'ID'){
                bodyRows += `<td>${row[colName]}</td>`;                
            }
        })
        bodyRows += '</tr>';
    });
    return `<table class="${className}"><tr>${headerRow}</tr>${bodyRows}</table>`;
}