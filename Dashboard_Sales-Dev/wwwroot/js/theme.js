(function () {
  var root = document.documentElement;

  function readTheme() {
    try {
      return localStorage.getItem('theme');
    } catch (e) {
      return null;
    }
  }

  function saveTheme(theme) {
    try {
      localStorage.setItem('theme', theme);
    } catch (e) {
    }
  }

  function apply(theme) {
    if (theme === 'light') {
      root.setAttribute('data-theme', 'light');
    } else {
      root.removeAttribute('data-theme');
    }
    var button = document.querySelector('[data-theme-toggle]');
    if (button) {
      button.setAttribute('aria-pressed', theme === 'light' ? 'true' : 'false');
    }
  }

  apply(readTheme());

  document.addEventListener('DOMContentLoaded', function () {
    apply(readTheme());
  });

  document.addEventListener('click', function (event) {
    if (!event.target.closest('[data-theme-toggle]')) {
      return;
    }
    var next = root.getAttribute('data-theme') === 'light' ? 'dark' : 'light';
    saveTheme(next);
    apply(next);
  });
})();
