async function shorten(url, pin, expiration) {
    let shortened = await fetch('Shorten/Create',
        {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({
                url: url,
                pin: pin.trim() !== '' ? pin : null,
                expiration: expiration.trim() !== '' ? new Date(expiration) : null
            })
        }
    );
    let resp = await shortened.json();
    document.querySelector('#shortened-result').value = `${getDomain(window.location.href)}/${resp.shortened}`;
}

function getDomain(url) {
    let chunked = url.split('/');
    return chunked.slice(0, 3).join('/');
}

function getDateString(d = new Date()) {
    return `${d.getFullYear()}-${(d.getMonth()+1).toString().padStart(2, '0')}-${d.getDate().toString().padStart(2, '0')}`
}

document.querySelector('#expiration').setAttribute('min', getDateString());