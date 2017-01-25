define(['apps/base/base.service'],
    function (module) {

        module.factory("tagService", function (Restangular, userApiUrl, userApiVersion) {

            var restSrv = Restangular.withConfig(function (configSetter) {
                configSetter.setBaseUrl(userApiUrl + userApiVersion);
            });

            return {
                getTags: function (filter,objectKey) {
                    return restSrv.all("tag").getList({ textfilter: filter, objectkey: objectKey });
                },
                object: {
                    saveTags: function (objectKey, objectID, tags) {
                        return restSrv.one("object", objectKey + "/" + objectID).customPOST(tags, 'tag');
                    }
                }
            }
        });
    });
