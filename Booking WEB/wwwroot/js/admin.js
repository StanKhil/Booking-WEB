export function adminCreateUser() {
    console.log("USER CREATE");
}
export function adminUpdateUser() {
    console.log("USER UPDATE");
}
export function adminDeleteUser() {
    console.log("USER DELETE");
}
export function adminFillInUserTable() {
    const table = document.getElementById('admin-user-table');
    if (!table) throw "Element 'admin-user-table' was not found";
    getUserTableData("GetUsersTable").then(s => {
        const userData = s;
        table.querySelector('tbody').innerHTML = userData;
    });
}


export function adminCreateRealty() {
    console.log("REALTY CREATE");
}
export function adminUpdateRealty() {
    console.log("REALTY UPDATE");
}
export function adminDeleteRealty() {
    console.log("REALTY DELETE");
}
export function adminFillInRealtyTable() {
    const table = document.getElementById('admin-realty-table');
    if (!table) throw "Element 'admin-realty-table' was not found";
    getUserTableData("GetRealtiesTable").then(s => {
        const userData = s;
        table.querySelector('tbody').innerHTML = userData;
    });
}



const getUserTableData = async (actionName) => {
    const response = await fetch(`/Administrator/${actionName}`, { method: "GET" });
    return await response.text();
}