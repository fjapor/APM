(function () {
    "use strict";

    function productListCtrl(productResource, $rootScope) {
        var vm = this;

        vm.searchCriteria = "GDN"; // could be bound to a user input on the front end


        $rootScope.$on('fill-product', function (event, data) {
            vm.products = data;
        });
    }

    angular
        .module("productManagement")
        .controller("ProductListCtrl",
        productListCtrl,
        ["productResource",
            "$rootScope",
            productListCtrl]);
}());
