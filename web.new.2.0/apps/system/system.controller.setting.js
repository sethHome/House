define(['apps/system/system.controller',
    'apps/system/system.service.setting'], function (app) {

        app.controller("system.controller.setting", function ($scope, system_setting_service) {

            $scope.settings = system_setting_service.all().$object;

            $scope.settingChanged = function (setting) {
                $scope.currentSetting = setting;
            }

            $scope.setItemSource = function (item) {
                if (item.Propertys.DataType == 'Bool') {
                    item.Propertys.Value = item.Propertys.Value == "True";
                }
                else if (item.Propertys.DataType == 'Options') {
                    angular.forEach(item.ChildNodes, function (node) {
                        if (node.NodeName == "Values") {
                            item.Source = [];
                            for (var s in node.Propertys) {
                                item.Source.push({ id: s, text: node.Propertys[s] });
                            }
                        }
                    })
                }
            }

            $scope.save = function () {

                var obj = {};
                var flage = false;
                angular.forEach($scope.currentSetting.ChildNodes, function (node) {
                    var key = $scope.currentSetting.NodeName + "." + node.NodeName;

                    if ($scope.sysSettings[key].toString() != node.Propertys.Value.toString()) {
                        obj[key] = node.Propertys.Value;
                        $scope.sysSettings[key] = obj[key];

                        flage = true;
                    }
                });
                if (flage) {
                    system_setting_service.updateSettings(obj).then(function () {
                        bootbox.alert("保存成功");
                    });
                }
            }
        });
    });
