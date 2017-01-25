define(['apps/system/system.controller',
    'apps/system/system.service.user',
    'apps/system/system.service.business'], function (app) {
        app.controller("system.controller.user.production", function ($scope, system_user_service) {

            //$scope.SpecialtysCopy = angular.copy($scope.Specialty_item);
            //$scope.ProcessCopy = angular.copy($scope.ProcessModel_item);

            $scope.currentProcess = $scope.ProcessModel_item[0];
            $scope.currentSpecialty = $scope.getBaseData("Specialty")[0];

            $scope.setCurrentSpec = function (s) {
                $scope.currentSpecialty = s;
                setTasks();
            }
            $scope.setCurrentProcess = function (p) {
                $scope.currentProcess = p;
                setTasks();
            }

            $scope.$watch("currentUser", function (newval,oldval) {
                if (newval) {
                    setTasks();
                }
            });

            $scope.save = function () {

                $scope.flowPanel.block();

                var tasks = [];
                angular.forEach( $scope.ProcessModel_enum[$scope.currentProcess.Key].Tasks,function(task){
                    if(task.isSelected){
                        tasks.push(task.ID);
                    }
                });

                system_user_service.setProduction($scope.currentUser.ID, {
                    SpeciltyID: $scope.currentSpecialty.Value,
                    ProcessKey: $scope.currentProcess.Key,
                    TaskIDs: tasks.join(',')
                }).then(function () {


                    $scope.flowPanel.unblock();

                    bootbox.alert("保存成功");
                });
            }

            var setTasks = function () {

                if ($scope.currentSpecialty && $scope.currentProcess && $scope.currentUser) {
                    var s = $scope.SpecProcessUsers[$scope.currentUser.ID];

                    angular.forEach($scope.ProcessModel_enum[$scope.currentProcess.Key].Tasks, function (task) {
                        if (s && s[$scope.currentSpecialty.Value]) {

                            var tasks = s[$scope.currentSpecialty.Value][$scope.currentProcess.Key];
                            if (tasks) {
                                task.isSelected = tasks.split(',').contains(function (t) { return t == task.ID });
                            }
                        } else {
                            task.isSelected = false;
                        }
                    });
                }
            }
        });
    });
