define(['apps/system3/production/production.controller',
        'apps/system3/production/engworking/engworking.service'], function (app) {

            app.controller('production.controller.specialty.plan', function ($scope, engworkingService, $uibModalInstance, maintainParams) {
                
                $scope.engInfo = maintainParams.entityInfo;

                // 拷贝专业信息
                var Specialtys = $scope.getBaseData("Specialty");
                $scope.SpecialtysCopy = angular.copy(Specialtys);

               
                // 获取工程已策划专业
                engworkingService.getEngineeringSpecils($scope.engInfo.ID).then(function (data) {
                    debugger;
                    angular.forEach($scope.SpecialtysCopy, function (item) {
                        item.Manager = undefined;
                        item.ProcessModel = undefined;
                        item.StartDate = undefined;
                        item.EndDate = undefined;
                        item.Note = undefined;

                        var obj = data.get(function (s) { return s.SpecialtyID == item.Value });

                        if (obj) {
                            item.Manager = obj.Manager;
                            item.ProcessModel = obj.ProcessModel;
                            item.StartDate = obj.StartDate;
                            item.EndDate = obj.EndDate;
                            item.Note = obj.Note;
                        }

                        item.isSelected = obj != undefined;

                    });
                });

                $scope.haveChat = function (row) {

                    if (row.entity.Engineering == undefined) {

                        for (var i in row.entity) {
                            if (row.entity[i].label == "ag_manager") {
                                $scope.openChat(row.entity[i].rendered, "[专业策划] - " + row.entity[i].entity.Engineering.Name);
                                break;
                            }
                        }
                    } else {
                        $scope.openChat(row.entity.Manager, "[专业策划] - " + row.entity.Engineering.Name + " 专业 :" + $scope.Specialty_enum[row.entity.SpecialtyID]);
                    }
                }

                // 保存专业信息
                var saveSpecialtys = function () {
                    var result = [];

                    angular.forEach($scope.SpecialtysCopy, function (item) {
                        if (item.isSelected) {
                            item.SpecialtyID = item.Value;
                            result.push(item);
                        }
                    });

                    specialtyService.updateSpecialtys($scope.engInfo.ID, result).then(function () {
                        bootbox.alert("工程策划保存成功！");
                    })
                };

                $scope.save = function () {
                    switch ($scope.currentIndex) {
                        case 2: {
                            saveSpecialtys();
                            break;
                        }
                    }
                }

                // 关闭编辑模式
                $scope.closeMaintain = function () {
                    $uibModalInstance.dismiss('cancel');
                }
            });
        });