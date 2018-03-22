(function() {

    "use strict";

    function productResource($resource, appSettings) {
        return $resource(appSettings.serverPath + "/api/products/:id", null,
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