
export function adminCreateUser(form) {
    console.log(form);
    fetch("/api/user", {
        method: "POST",
        body: new FormData(form)
    }).then(r => r.json()).then(console.log);
}
export function adminUpdateUser(form) {
    const login = form.querySelector("input[name='user-former-login']").value;

    const data = {
        FormerLogin: form.querySelector("input[name='user-former-login']").value,
        FirstName: form.querySelector("input[name='user-first-name']").value,
        LastName: form.querySelector("input[name='user-last-name']").value,
        Email: form.querySelector("input[name='user-email']").value,
        Birthdate: form.querySelector("input[name='user-birthdate']").value || null,
        Login: form.querySelector("input[name='user-login']").value,
        Password: form.querySelector("input[name='user-password']").value,
        RoleId: form.querySelector("select[name='user-role']").value
    };

    fetch(`/api/user/${login}`, {
        method: "PATCH",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data) 
    })
        .then(r => r.json())
        .then(console.log)
        .catch(console.error);
}

export function adminDeleteUser(form) {
    const login = form.querySelector("input[name='user-delete-login']").value;
    fetch(`/api/user/${login}`, {
        method: "DELETE"
    })
        .then(r => {
            if (r.status === 204) {
                console.log("User deleted");
                return;
            }
            return r.json();
        })
        .then(data => {
            if (data) console.log(data);
        })
        .catch(console.error);
}

export function adminFillInUserTable() {
    const table = document.getElementById('admin-user-table');
    if (!table) throw "Element 'admin-user-table' was not found";
    getUserTableData("GetUsersTable").then(s => {
        const userData = s;
        table.querySelector('tbody').innerHTML = userData;
    });
}


export function adminCreateRealty(form) {
    console.log(form);
    fetch("/api/realty", {
        method: "POST",
        body: new FormData(form)
    }).then(r => r.json()).then(console.log);
}
export function adminUpdateRealty(form) {
    fetch("/api/realty", {
        method: "PATCH",
        body: new FormData(form)
    }).then(r => r.json()).then(console.log);
}
export function adminDeleteRealty(slug) {
    fetch(`/api/realty/${slug}`, {
        method: "DELETE",
    }).then(r => r.json()).then(console.log);
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