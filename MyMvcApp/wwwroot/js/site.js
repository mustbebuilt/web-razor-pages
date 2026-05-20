document.addEventListener("DOMContentLoaded", function () {
	var nav = document.querySelector(".site-nav");
	var button = document.querySelector(".burger");
	var menu = document.querySelector("#primary-menu");

	if (!nav || !button || !menu) {
		return;
	}

	function setOpenState(isOpen) {
		nav.classList.toggle("is-open", isOpen);
		button.setAttribute("aria-expanded", String(isOpen));
	}

	button.addEventListener("click", function () {
		setOpenState(!nav.classList.contains("is-open"));
	});

	menu.addEventListener("click", function (event) {
		if (event.target instanceof HTMLAnchorElement && window.innerWidth < 768) {
			setOpenState(false);
		}
	});

	window.addEventListener("resize", function () {
		if (window.innerWidth >= 768) {
			setOpenState(false);
		}
	});
});
