define(['apps/base/base.service'],
    function (module) {

        module.factory("attachService", function (Restangular, userApiUrl, userApiVersion) {

            var restSrv = Restangular.withConfig(function (configSetter) {
                configSetter.setBaseUrl(userApiUrl + userApiVersion);
            });

            return {
                getAttach: function (id) {
                    return restSrv.one("attach", id).get();
                },
                getAttachs: function (ids, withTags) {
                    return restSrv.all("attach").getList({ ids: ids, withtag: withTags });
                },
                getObjAttachs: function (objKey, objID, withTags) {
                    return restSrv.all("attach").getList({ objkey: objKey, objid: objID, withtag: withTags });
                },
                object: {
                    getAttachIDs: function (objectKey, objectID) {
                        return restSrv.one("object", objectKey).customGET(objectID + '/attach');
                    },
                    addAttach: function (objectKey, objectID, attachID) {
                        return restSrv.one("attach", attachID).customPOST({
                            ObjectKey: objectKey,
                            ObjectID: objectID
                        }, 'object');
                    },
                    removeAttach: function (attachID, objName) {
                        return restSrv.one("attach", attachID).customDELETE('object/' + objName);
                    },
                    removeAttachs: function (attachIDs, objName) {
                        return restSrv.all("attach").customDELETE('object/' + objName, { ids: attachIDs });
                    }
                }
            }
        });
    });
