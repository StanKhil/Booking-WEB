import Base64 from "./Base64.js";
//import { userCreate, userUpdateDelete, userDatabase, realtyCreate, realtyUpdateDelete, realtyDatabase } from "./resources.js";
//import { adminFillInUserTable, adminCreateUser, adminUpdateUser, adminDeleteUser } from "./admin.js";
//import { adminFillInRealtyTable, adminCreateRealty, adminUpdateRealty, adminDeleteRealty } from "./admin.js";

document.addEventListener('DOMContentLoaded', function ()
{
    for (let button of document.querySelectorAll('[data-nav]'))
    {
        button.onclick = navigate;
    }

    const editProfileBtn = document.getElementById("edit-profile-btn");
    if (editProfileBtn)
    {
        editProfileBtn.onclick = editProfileBtnClick;
    }

    const deleteProfileBtn = document.getElementById("delete-profile-btn");
    if (deleteProfileBtn) {
        deleteProfileBtn.onclick = deleteProfileBtnClick;
    }

    for (let button of document.querySelectorAll('[data-profile]'))
    {
        button.onclick = navigateProfile;
    } 

    const addCardButton = document.getElementById('add-card-button');
    if (addCardButton) {
        addCardButton.onclick = openAddCardForm;
    }
    const cancelAddCardButton = document.getElementById('cancel-add-card-button');
    if (cancelAddCardButton) {
        cancelAddCardButton.onclick = closeAddCardForm;
    }

});

document.addEventListener('submit', e => {
    const form = e.target;
    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!?@$&*])[A-Za-z\d@$!%*?&]{12,}$/;
    //const cardRegex = /^(?:4[0-9]{12}(?:[0-9]{3})?|[25][1-7][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0 - 5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11}) $/; // Visa, MasterCard, American Express, Diners Club, Discover, and JCB cards
    const expDateRegex = /^(0[1-9]|1[0-2])\/?([0-9]{4}|[0-9]{2})$/;
    if (form.id == 'sign-in-form') {
        e.preventDefault();

        const loginInput = form.querySelector('[name="user-login"]');
        if (!loginInput) {
            throw `Element [name="user-login"] was not found`;
        }
        const passwordInput = form.querySelector('[name="user-password"]');
        if (!passwordInput) {
            throw `Element [name="user-password"] was not found`;
        }
        if (loginInput.value.length == 0) {
            loginInput.classList.add('is-invalid');
            loginInput.nextElementSibling.innerHTML = 'Login cannot be empty';
        }
        else {
            loginInput.classList.remove('is-invalid');
            loginInput.classList.add('is-valid');
            loginInput.nextElementSibling.innerHTML = '';
        }


        if (passwordInput.value.length == 0) {
            passwordInput.classList.add('is-invalid');
            passwordInput.nextElementSibling.innerHTML = 'Password cannot be empty';
        }
        else {
            if (!(passwordRegex.test(passwordInput.value))) {
                passwordInput.classList.add('is-invalid');
                passwordInput.nextElementSibling.innerHTML = 'Password must be at least 12 characters long and contain lower, upper case letters, at least one number and at least one special character';
            }
            else {
                passwordInput.classList.remove('is-invalid');
                passwordInput.classList.add('is-valid');
                passwordInput.nextElementSibling.innerHTML = '';
            }
        }
        const credentials = new Base64().encode(`${loginInput.value}:${passwordInput.value}`);
        fetch('/Auth/LogIn', {
            method: 'GET',
            headers: {
                'Authorization': `Basic ${credentials}`
            }
        }).then(r => r.json())
            .then(j => {
                if (j.status == 200) {
                    window.accessToken = j.data;
                    window.location.href = "/Home/Index";
                }
                else {
                    const alertDiv = document.getElementById('login-alert');
                    if (!alertDiv) throw 'Element #login-alert was not found';
                    alertDiv.innerText = j.data;
                    alertDiv.style.display = 'flex';
                }
            })
    }
    if (form.id == 'add-card-form')
    {
        e.preventDefault();
        const cardholderInput = form.querySelector('[name="cardholder-name"]');
        if (!cardholderInput) throw "Element with name 'cardholder-name' was not found";

        const numberInput = form.querySelector('[name="card-number"]');
        if (!numberInput) throw "Element with name 'card-number' was not found";

        const expDateInput = form.querySelector('[name="exp-date"]');
        if (!expDateInput) throw "Element with name 'exp-date' was not found";

        if (cardholderInput.value.length == 0) {
            cardholderInput.classList.add('is-invalid');
            cardholderInput.nextElementSibling.innerHTML = "Cardholder's name cannot be empty";
        }
        else
        {
            cardholderInput.classList.remove('is-invalid');
            cardholderInput.classList.add('is-valid');
            cardholderInput.nextElementSibling.innerHTML = "";
        }

        if (numberInput.value.length != 16 || numberInput.value.length == 0)
        {
            numberInput.classList.add('is-invalid');
            numberInput.nextElementSibling.innerHTML = "Invalid card number";
        }
        else
        {
            numberInput.classList.remove('is-invalid');
            numberInput.classList.add('is-valid');
            numberInput.nextElementSibling.innerHTML = "";
        }

        if (expDateInput.value.length == 0)
        {
            expDateInput.classList.add('is-invalid');
            expDateInput.nextElementSibling.innerHTML = "Expiration date cannot be empty";
        }
        else
        {
            if (!(expDateRegex.test(expDateInput.value)))
            {
                expDateInput.classList.add('is-invalid');
                expDateInput.nextElementSibling.innerHTML = "Invalid expiration date format";
            }
            else
            {
                expDateInput.classList.remove('is-invalid');
                expDateInput.classList.add('is-valid');
                expDateInput.nextElementSibling.innerHTML = "";
            }
           
        }



        // ------- SOME BANK VALIDATION -------
        // ...
        // ...
        // ...



      
    }
    if (form.id == "product-add-form") {
        e.preventDefault();
        handleAddProduct(form)
    }    
});

function openAddCardForm(e)
{
    e.preventDefault();
    const form = document.getElementById('add-card-form');
    if (!form) throw "Element 'add-card-form' was not found";
    const field = document.getElementById('add-card-field');
    if (!field) throw "Element 'add-card-field' was not found";
    form.classList.remove('d-none');
    field.classList.add('d-none');
}

function closeAddCardForm(e)
{
    e.preventDefault();
    const form = document.getElementById('add-card-form');
    if (!form) throw "Element 'add-card-form' was not found";
    const field = document.getElementById('add-card-field');
    if (!field) throw "Element 'add-card-field' was not found";
    form.reset();
    form.classList.add('d-none');
    field.classList.remove('d-none');
}

function navigateProfile(e)
{
    const targetButton = e.target.closest('[data-profile]');
    for (let page of document.querySelectorAll('[data-page]'))
    {
        if (targetButton.getAttribute('data-profile') == page.getAttribute('data-page'))
        {
            page.classList.remove('d-none');
        }
        else
        {
            page.classList.add('d-none');
        }
    }
}

function navigate(e)
{
    const targetButton = e.target.closest('[data-nav]');
    const route = targetButton.getAttribute('data-nav');
    if (!route) throw `Route was not found`;
    showPage(route);
}
function showPage(page)
{
    window.activePage = page;
    const spaContainer = document.getElementById('spa-container');
    if (!spaContainer) throw "Element #spa-container was not found";
    switch (page) {
        // ========== ADMIN ==========
        case 'user-create': spaContainer.innerHTML = userCreate; break;
        case 'user-update-delete': spaContainer.innerHTML = userUpdateDelete; break;
        case 'view-user-database': spaContainer.innerHTML = userDatabase; adminFillInUserTable(); break;
        case 'realty-create': spaContainer.innerHTML = realtyCreate; break;
        case 'realty-update-delete': spaContainer.innerHTML = realtyUpdateDelete; break;
        case 'view-realty-database': spaContainer.innerHTML = realtyDatabase; adminFillInRealtyTable(); break;

        // ========== PROFILE ==========
        default: spaContainer.innerHTML = `<div class="alert alert-danger" role="alert">Something went wrong!</div>`;
    }
    setupActions();
}
function setupActions()
{
    for (let button of document.querySelectorAll('[data-action]'))
    {
        switch (button.dataset.action)
        {
            case "create-user": button.onclick = adminCreateUser; break;
            case "update-user": button.onclick = adminUpdateUser; break;
            case "delete-user": button.onclick = adminDeleteUser; break;
            case "create-realty": button.onclick = adminCreateRealty; break;
            case "update-realty": button.onclick = adminUpdateRealty; break;
            case "delete-realty": button.onclick = adminDeleteRealty; break;
        }
    }
}

// ----------------TO REMOVE LATER--------------------

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



const getHtml = async (fileName, path) => {
    const response = await fetch(`/Resources/GetHtmlPage?fileName=${fileName}`);
    return await response.text();
};

const userCreate = await getHtml('UserCreatePage.txt');
const userUpdateDelete = await getHtml('UpdateDeleteUserPage.txt');
const userDatabase = await getHtml('UserDatabasePage.txt');

const realtyCreate = await getHtml('RealtyCreatePage.txt');
const realtyUpdateDelete = await getHtml('UpdateDeleteRealtyPage.txt');
const realtyDatabase = await getHtml('RealtyDatabasePage.txt');


// ---------------------------------------------

function editProfileBtnClick()
{
    let changes = [];
    for (let element of document.querySelectorAll('[data-editable]'))
    {
        if (element.getAttribute('contenteditable'))
        {
            element.removeAttribute('contenteditable');
            if (element.originalData != element.innerText)
            {
                changes.push({
                    field: element.getAttribute('data-editable'),
                    value: element.innerText
                });
            }
        }
        else
        {
            element.setAttribute('contenteditable', true);
            element.originalData = element.innerText;
        }
        if (changes.length > 0)
        {
            const message = changes.map(c => `${c.field}=${c.value}`).join(', ');
            if (confirm(`Confirm changes ${message}`))
            {
                fetch("/User/Edit", {
                    method: 'PATCH',
                    header: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(changes)
                }).then(r => r.json()).then(console.log);
                
            }
        }
    }
}

function deleteProfileBtnClick() {
    if (confirm(`Delete profile:`)) {
        let login = prompt("Enter your login to confirm deletion:");
        if (login == null || login.trim() == "") {
            alert("Deletion cancelled.");
            return;
        }
        fetch("/User/DeleteProfile", {
            method: 'DELETE',
            headers: {
                'Authentication-Control': new Base64().encodeUrl(login)
            },
        }).then(r => r.json()).then(j => {
            console.log(j);
            if (j.status == 200) {
                alert("Profile deleted successfully.");
                window.location = '/';
            } else {
                alert("Error deleting profile: " + j.data);
            }
        });
    }
}

function handleAddProduct(form) {
    console.log("Adding product:", form);
    fetch('/Realty/Create', {
        method: "POST",
        body: new FormData(form)
    }).then(r => r.json()).then(console.log).catch(console.error);
}