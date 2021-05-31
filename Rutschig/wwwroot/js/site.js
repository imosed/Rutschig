const result = document.querySelector('#shortened-result');

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
    
    if (resp.shortened === null) return;
    
    const linkResult = `${getDomain(window.location.href)}/${resp.shortened}`;
    while (result.hasChildNodes()) {
        result.removeChild(result.firstChild);
    }
    let newLink = document.createElement('a');
    newLink.setAttribute('href', linkResult);
    newLink.setAttribute('id', 'shortened-link');
    newLink.textContent = linkResult;
    result.appendChild(newLink);
}

function getDomain(url) {
    let chunked = url.split('/');
    return chunked.slice(0, 3).join('/');
}

function getDateString(d) {
    return `${d.getFullYear()}-${(d.getMonth()+1).toString().padStart(2, '0')}-${d.getDate().toString().padStart(2, '0')}`
}

function addDays(dateTime, days) {
    return new Date(dateTime.getFullYear(), dateTime.getMonth(), dateTime.getDate() + days);
}

document
    .querySelector('#expiration')
    .setAttribute(
        'min',
        getDateString(
            addDays(new Date(), 1)
        )
    );