'use strict';

/**
 * MOBILE NAVBAR TOGGLE
 */
const showMenu = (toggleId, navId) => {
  const toggle = document.getElementById(toggleId),
    nav = document.getElementById(navId);

  toggle.addEventListener('click', () => {
    nav.classList.toggle('show-menu');
    toggle.classList.toggle('show-icon');
  })
}

showMenu('nav-toggle', 'nav-menu');

// Posts JSON using the browser fetch API and returns an object { status, body }
window.postJson = async function (url, data) {
  const res = await fetch(url, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(data)
  });
  const text = await res.text();
  return { status: res.status, body: text };
};