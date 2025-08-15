import Base64 from "./Base64.js";
import { userCreate, userUpdateDelete, userDatabase, realtyCreate, realtyUpdateDelete, realtyDatabase } from "./resources.js";
import { adminFillInUserTable, adminCreateUser, adminUpdateUser, adminDeleteUser } from "./admin.js";
import { adminFillInRealtyTable, adminCreateRealty, adminUpdateRealty, adminDeleteRealty } from "./admin.js";

document.addEventListener('DOMContentLoaded', function ()
{
    for (let button of document.querySelectorAll('[data-nav]'))
    {
        button.onclick = navigate;
    }

    const editProfileBtn = document.getElementById("edit-profile-btn");
    if (editProfileBtn) {
        editProfileBtn.onclick = editProfileBtnClick;
    }
});

document.addEventListener('submit', e =>
{
    const form = e.target;
    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!?@$&*])[A-Za-z\d@$!%*?&]{12,}$/;
    if (form.id == 'sign-in-form')
    {
        e.preventDefault();

        const loginInput = form.querySelector('[name="user-login"]');
        if (!loginInput)
        {
            throw `Element [name="user-login"] was not found`;
        }
        const passwordInput = form.querySelector('[name="user-password"]');
        if (!passwordInput)
        {
            throw `Element [name="user-password"] was not found`;
        }
        if (loginInput.value.length == 0)
        {
            loginInput.classList.add('is-invalid');
            loginInput.nextElementSibling.innerHTML = 'Login cannot be empty';
        }
        else
        {
            loginInput.classList.remove('is-invalid');
            loginInput.classList.add('is-valid');
            loginInput.nextElementSibling.innerHTML = '';
        }


        if (passwordInput.value.length == 0)
        {
            passwordInput.classList.add('is-invalid');
            passwordInput.nextElementSibling.innerHTML = 'Password cannot be empty';
        }
        else
        {
            if (!(passwordRegex.test(passwordInput.value)))
            {
                passwordInput.classList.add('is-invalid');
                passwordInput.nextElementSibling.innerHTML = 'Password must be at least 12 characters long and contain lower, upper case letters, at least one number and at least one special character';
            }
            else
            {
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
                if (j.status == 200)
                {
                    window.accessToken = j.data;
                    console.log(window.accessToken);
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
})

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
        case 'user-create': spaContainer.innerHTML = userCreate; break;
        case 'user-update-delete': spaContainer.innerHTML = userUpdateDelete; break;
        case 'view-user-database': spaContainer.innerHTML = userDatabase; adminFillInUserTable(); break;
        case 'realty-create': spaContainer.innerHTML = realtyCreate; break;
        case 'realty-update-delete': spaContainer.innerHTML = realtyUpdateDelete; break;
        case 'view-realty-database': spaContainer.innerHTML = realtyDatabase; adminFillInRealtyTable(); break;
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

const getUserTableData = async (actionName) => {
    const response = await fetch(`/Administrator/${actionName}`, { method: "GET" });
    return await response.text();
}

// ---------------------------------------------
function editProfileBtnClick() {
    for (let elem of document.querySelectorAll('[data-editable]')) {
        if (elem.getAttribute('contenteditable')) {
            elem.removeAttribute('contenteditable');
        }
        elem.setAttribute('contenteditable', true);
    }
}