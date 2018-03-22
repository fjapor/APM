(function() {

    "use strict";

    function productResource($resource, appSettings) {
        return $resource(appSettings.serverPath + "/api/fr/products/:id", null,
            {
                'update': { method: 'PUT'}
            });
    }

    angular.module("common.services")
        .factory("productResource",
            ["$resource",
            "appSettings",
            productResource]);
}());