import Base64 from "./Base64.js";

document.addEventListener('DOMContentLoaded', function ()
{
    for (let button of document.querySelectorAll('[data-nav]'))
    {
        button.onclick = navigate;
    }

    //const tabButtons = document.getElementsByName('header-nav-button');
    //tabButtons.forEach(button =>
    //{
    //    button.addEventListener('click', function (event)
    //    {
    //        tabButtons.forEach(btn => btn.classList.remove('active'));
    //        button.classList.add('active');
    //        console.log(button);
    //    });
    //});
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
        fetch('/User/SignIn', {
            method: 'GET',
            headers: {
                'Authorization': `Basic ${credentials}`
            }
        }).then(r => r.json())
            .then(j => {
                console.log(j);
                if (j.status == 200) {
                    window.location.reload();
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
        case 'user-update-delete': spaContainer.innerHTML = `<b>Privacy</b>`; break;
        case 'view-user-database': spaContainer.innerHTML = `<b>Settings</b>`; break;
        case 'realty-create': spaContainer.innerHTML = `<b>Home1</b>`; break;
        case 'realty-update-delete': spaContainer.innerHTML = `<b>Privacy1</b>`; break;
        case 'view-realty-database': spaContainer.innerHTML = `<b>Settings1</b>`; break;
        default: spaContainer.innerHTML = `<div class="alert alert-danger" role="alert">Something went wrong!</div>`;
    }
}

// ------------ COLLECTION OF PAGES ------------

const userCreate = `<div>

<form>
<div class="mb-3">
	<label for="user-first-name" class="form-label">First Name</label>
	<input type="text" name="user-first-name" class="form-control @classAddon" id="user-first-name" aria-describedby="FirstName" placeholder="Enter your first name">
	<div class="invalid-feedback">@errorMessage</div>
</div>

<div class="mb-3">
	
	<label for="user-last-name" class="form-label">Last Name</label>
	<input type="text" name="user-last-name" class="form-control @classAddon" id="user-last-name" aria-describedby="LastName" placeholder="Enter your last name">
	<div class="invalid-feedback">@errorMessage</div>
</div>

<div class="mb-3">
	
	<label for="user-email" class="form-label">Email address</label>
	<input type="email" name="user-email"  class="form-control @classAddon" id="user-email" aria-describedby="Email" placeholder="Enter your email address">
	<div class="invalid-feedback">@errorMessage</div>
</div>

<div class="mb-3">
	
	<label for="user-login" class="form-label">Login</label>
	<input type="text" name="user-login"  class="form-control @classAddon" id="user-login" aria-describedby="Login" placeholder="Enter your login">
	<div class="invalid-feedback">@errorMessage</div>
</div>

<div class="mb-3">

	<label for="user-birthdate" class="form-label">Date of birth</label>
	<input type="date" name="birthdate"  class="form-control @classAddon" id="user-birthdate" aria-describedby="Birthdate" placeholder="Enter your birthdate">
	<div class="invalid-feedback">@errorMessage</div>
</div>

<div class="mb-3">
	
	<label for="user-password" class="form-label">Password</label>
	<input type="password" name="user-password" class="form-control @classAddon" id="user-password" aria-describedby="Password" placeholder="Enter your password">
	<div class="invalid-feedback">@errorMessage</div>
</div>

</form>

</div>`






// ------------ ------------------- ------------