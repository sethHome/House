define(['apps/base/base.directive'],
    function (app) {

    	app.directive('pDatatable', function () {
    		return {
    			restrict: 'EA',
    			link: function (scope, element, attrs) {
    			    var a = $(element).puidatatable(scope[attrs.options]);
    			    alert(a.setTotalRecords);
    			}
    		};
    	})

    	app.directive('pMenubar', function () {
    	    return {
    	        restrict: 'EA',
    	        link: function (scope, element, attrs) {
    	            $(element).puimenubar();
    	        }
    	    };
    	})

    	app.directive('pOverlaypanel', function () {
    	    return {
    	        restrict: 'EA',
    	        link: function (scope, element, attrs) {

    	            $(element).puioverlaypanel({
    	                target: attrs.pOverlaypanel,
    	                showEvent: "mouseover",
    	                hideEvent: "mousedown",
    	                showCloseIcon: true,
    	                showEffect: "blind",
    	                dismissable: false
    	            });
    	        }
    	    };
    	})
    });
