(function () {
    "use strict";

    function productListCtrl(productResource) {
        var vm = this;

        vm.searchCriteria = "GDN"; // could be bound to a user input on the front end

        productResource.query(function (data) {
            vm.products = data;
        });
    }

    angular
        .module("productManagement")
        .controller("ProductListCtrl",
                     productListCtrl,
                     ["productResource",
                         productListCtrl]);
}());
