(function () {

    "use strict";

    var factory = {
        locale: 'en',
    }

    function productResource($resource, appSettings) {
        factory.build = function (locale) {
            var opt_locale = locale || this.locale;
            return $resource(appSettings.serverPath + "/api/" + opt_locale + "/products/:id", null,
                {
                    'update': { method: 'PUT' }
                });
        }
        return factory;
    }

    angular.module("common.services")
        .factory("productResource",
        ["$resource",
            "appSettings",
            productResource]);
}());


/*(function() {

    "use strict";

    function productResource($resource, appSettings) {
        return $resource(appSettings.serverPath + "/api/" + appSettings.locale + "/products/:id", null,
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
*/