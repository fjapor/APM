(function () {
    "use strict";

    angular
        .module("productManagement")
        .controller("ProductEditCtrl", 
        ["$rootScope","productResource", ProductEditCtrl]);

    function ProductEditCtrl($rootScope, productResource) {
        var vm = this;
        vm.lang = 'en';
        vm.product = {};
        vm.message = '';

        vm.change = function () {
            productResource.locale = vm.lang;

            productResource.build().get({ id: 5 },
                function (data) {
                    vm.product = data;
                    vm.originalProduct = angular.copy(data);
                },
                function (response) {
                    vm.message = response.statusText + "\r\n";
                    if (response.data.exceptionMessage) {
                        vm.message += response.data.exceptionMessage;
                    }
                });

            if (vm.product && vm.product.productId) {
                vm.title = "Edit: " + vm.product.productName;
            }
            else {
                vm.title = "New Product";
            }

            productResource.build().query(function (data) {
                $rootScope.$broadcast('fill-product', data);
            });
        }

        vm.change();

        vm.submit = function () {
            vm.message = '';
            if (vm.product.productId) {
                vm.product.$update({ id: vm.product.productId },
                    function (data) {
                        vm.message = "... Save Complete";
                    },
                    function (response) {
                        vm.message = response.statusText + "\r\n";

                        if (response.data.modelState) {
                            for (var key in response.data.modelState) {
                                vm.message += response.data.modelState[key] + "\r\n";
                            }
                        }

                        if (response.data.exceptionMessage) {
                            vm.message += response.data.exceptionMessage;
                        }
                    })
            } else {
                vm.product.$save(
                    function (data) {
                        vm.originalProduct = angular.copy(data);
                        vm.message = "... Save Complete";
                    },
                    function (response) {
                        vm.message = response.statusText + "\r\n";

                        if (response.data.modelState) {
                            for (var key in response.data.modelState) {
                                vm.message += response.data.modelState[key] + "\r\n";
                            }
                        }

                        if (response.data.exceptionMessage) {
                            vm.message += response.data.exceptionMessage;
                        }
                    })
            }
        };

        vm.cancel = function (editForm) {
            editForm.$setPristine();
            vm.product = angular.copy(vm.originalProduct);
            vm.message = "";
        };

    }
}());
