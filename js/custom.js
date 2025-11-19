/**
 * Custom JS
 */

 // Sidebar

$(function() {

	// Init perfect scrollbar
	$(".sidebar").perfectScrollbar({
		suppressScrollX: true
	});

	// Sidebar: Toggle user info
	$(".sidebar-user__info").click(function() {
		$(".sidebar-user__nav").slideToggle(300, function() {
			$(".sidebar").perfectScrollbar("update");
		});
		return false;
	});

	// Sidebar: Toggle sidebar dropdown
	$(".sidebar-nav__dropdown > a").click(function() {
		$(this).parent("li").toggleClass("open");
		$(this).parent("li").find(".sidebar-nav__submenu").slideToggle(300, function() {
			$(".sidebar").perfectScrollbar("update");
		});
		return false;
	});

	// Sidebar: Toggle sidebar
	$("#sidebar__toggle, .sidebar__close").click(function() {
		$(".wrapper").toggleClass("alt");
		return false;
	});

});

// Counto To

$(function() {

	if ( $(".count-to").length ) {

		$(".count-to").countTo({
			refreshInterval: 20
		});

	}

});


// Smart alerts

$(function() {
	if ($(".smart-alert").length) {

		// Init smart alerts
		var smartAlerts = new SmartAlerts();

		// Generate alerts (ui_alerts.html example)
			$(".smart-alert").each(function() {
				var alertType = $(this).data("alert-type");
				var alertContent = $(this).data("alert-content");

				$(this).click(function() {
					smartAlerts.generate(alertType, alertContent);
					return false;
				});
			});
			
	}
});


// Datatables

$(function() {

	if ( $("#datatables__example").length ) {

		$("#datatables__example").DataTable({
			dom: "f"
		});

	}

});


// Inbox

$(function() {

	if ( $(".inbox__table").length ) {

		$(".inbox__table").DataTable({
		});

	}

});


// Orders

$(function() {

	if ( $("#orders__table").length ) {

		$("#orders__table").DataTable({
		});

	}

});


// Collapse plugin

$("[data-toggle='collapse']").click(function(e) {
	e.preventDefault();
});