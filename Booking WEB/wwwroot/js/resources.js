const getHtml = async (fileName) => {
    const response = await fetch(`/Resources/GetHtmlPage?fileName=${fileName}`);
    return await response.text();
};

export const userCreate = await getHtml('UserCreatePage.txt');
export const userUpdateDelete = await getHtml('UpdateDeleteUserPage.txt');
export const userDatabase = await getHtml('UserDatabasePage.txt');

export const realtyCreate = await getHtml('RealtyCreatePage.txt');
export const realtyUpdateDelete = await getHtml('UpdateDeleteRealtyPage.txt');
export const realtyDatabase = await getHtml('RealtyDatabasePage.txt');