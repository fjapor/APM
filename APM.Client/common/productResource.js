(function() {

    "use strict";

    function productResource($resource, appSettings, lang) {
        return $resource(appSettings.serverPath + "/api/" + lang + "/products/:id", null,
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