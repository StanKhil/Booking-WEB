class Base64 {
    static #textEncoder = new TextEncoder();
    static #textDecoder = new TextDecoder();

    // https://datatracker.ietf.org/doc/html/rfc4648#section-4
    encode = (str) => btoa(String.fromCharCode(...Base64.#textEncoder.encode(str)));
    decode = (str) => Base64.#textDecoder.decode(Uint8Array.from(atob(str), c => c.charCodeAt(0)));
    // https://datatracker.ietf.org/doc/html/rfc4648#section-5
    encodeUrl = (str) => this.encode(str).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
    decodeUrl = (str) => this.decode(str.replace(/\-/g, '+').replace(/\_/g, '/'));

    jwtEncodeBody = (header, payload) => this.encodeUrl(JSON.stringify(header)) + '.' + this.encodeUrl(JSON.stringify(payload));
    jwtDecodePayload = (jwt) => JSON.parse(this.decodeUrl(jwt.split('.')[1]));
}


document.addEventListener('DOMContentLoaded', function ()
{
    const tabButtons = document.querySelectorAll('.tab-nav .tab-button');
    tabButtons.forEach(button =>
    {
        button.addEventListener('click', function (event)
        {
            event.preventDefault(); 
            tabButtons.forEach(btn => btn.classList.remove('active'));
            this.classList.add('active');
        });
    });
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

//Method for test RealtyController.Create()
function createTestMethod() {
    //example data
    const testData = {
        "name": "Luxury Villa",
        "description": "Sea view villa with pool",
        "slug": "luxury-villa2",
        "imageUrl": "villa.jpg",
        "price": 1999.99,
        "cityId": "0d156354-89f1-4d58-a735-876b7add59d2",
        "countryId": "bdf41cd9-c0f1-4349-8a44-4e67755d0415",
        "groupId": "6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"
    }

    try {
        fetch('/Realty/Create', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(testData)
        });
        console.log('Test realty created successfully');
    }
    catch (e) {
        console.error('Error creating test realty:', e);
    }
}

function updateTestMethod() {
    const updatedData = {
        "id": "072b79fe-3c89-44d8-90f8-6d0ad54a91d4",
        "name": "Luxury Villa (Updated)",
        "description": "Updated description: Sea view villa with sauna",
        "slug": "luxury-villa2",
        "imageUrl": "villa-updated.jpg",
        "price": 2199.99,
        "cityId": "0d156354-89f1-4d58-a735-876b7add59d2",
        "countryId": "bdf41cd9-c0f1-4349-8a44-4e67755d0415",
        "groupId": "6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"
    };

    fetch('/Realty/Update', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(updatedData)
    })
        .then(response => response.json())
        .then(data => {
            console.log('Update response:', data);
        })
        .catch(error => {
            console.error('Error updating realty:', error);
        });
}

function deleteTestMethod() {
    const idToDelete = "072b79fe-3c89-44d8-90f8-6d0ad54a91d4";

    fetch('/Realty/Delete', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ id: idToDelete })
    })
        .then(response => response.json())
        .then(data => {
            console.log('Delete response:', data);
        })
        .catch(error => {
            console.error('Error deleting realty:', error);
        });
}
