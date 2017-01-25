define(['apps/system3/home/home.controller',
        'apps/system3/home/desktop/desktop.service', ], function (app) {

            app.filter('statusfilter', function () {
                return function (input, status) {
                    if (input != undefined) {
                        return input.where(function (item) {
                            return (status.process && item.Status == 1) || (status.finish && item.Status == 2);
                        });
                    }
                    
                };
            });

            app.filter('namefilter', function () {
                return function (input, names) {
                    if (input != undefined) {
                        
                        return input.where(function (item) {

                            return names[item.Name].selected;
                        });
                    }
                };
            });

            app.filter('formNameFilter', function () {
                return function (input, names) {
                    if (input != undefined) {

                        return input.where(function (item) {

                            return names[item.ObjectKey].selected;
                        });
                    }
                };
            });

            app.filter('engnamefilter', function () {
                return function (input, name) {
                    if (input != undefined && name != undefined && name.trim() != "") {
                        return input.where(function (item) {
                            return item.Engineering.Name.indexOf(name) >= 0
                        });
                    } else {
                        return input;
                    }
                };
            });

            app.filter('protaskFilter', function () {
                return function (input, node) {

                    if (input != undefined && node != undefined && node.EngID != undefined) {
                        return input.where(function (item) {
                            if(node.VolumeID){
                                return item.Engineering.ID == node.EngID && item.Volume.SpecialtyID == node.SpecialtyID && item.Volume.ID == node.VolumeID;
                            }else if(node.SpecialtyID){
                                return item.Engineering.ID == node.EngID && item.Volume.SpecialtyID == node.SpecialtyID;
                            }else{
                                return item.Engineering.ID == node.EngID;
                            }
                        });
                    } else {
                        return input;
                    }
                };
            });


            app.controller("home.controller.mytask", function ($scope, $filter, $stateParams, desktopService) {

                $scope.currentIndex = $stateParams.type;

                // 工程生产任务
                $scope.proTaskStatus = { process: true, finish: false };
                $scope.proTaskStatusCount = { 1: 0, 2: 0 };
                $scope.proTaskNames = {};
                
                desktopService.getMyProductionTasks().then(function (result) {

                    $scope.productionTasks = result;
                    $scope.proEngNodes = getEngTreeNodes(result.Source);

                    angular.forEach(result.Source, function (task) {
                        $scope.proTaskStatusCount[task.Status]++;

                        if ($scope.proTaskNames[task.Name] == undefined) {
                            $scope.proTaskNames[task.Name] = {
                                name: task.Name,
                                count: 1,
                                selected:true
                            }
                        } else {
                            $scope.proTaskNames[task.Name].count++;
                        }
                    });

                });

                var getEngTreeNodes = function (tasks) {

                    var engs = [];
                    var enumMap = $filter("enumMap");

                    angular.forEach(tasks, function (task) {

                        var specName = enumMap(task.Volume.SpecialtyID, 'Specialty');

                        var eng = engs.find(function (item) { return item.EngID == task.Engineering.ID; });

                        if (eng) {

                            var spec = eng.children.find(function (item) { return item.SpecialtyID == task.Volume.SpecialtyID; });

                            if (spec) {

                                var volume = spec.children.find(function (item) { return item.VolumeID == task.Volume.ID; });

                                if (volume == undefined) {
                                    spec.children.push({
                                        EngID: task.Engineering.ID,
                                        SpecialtyID: task.Volume.SpecialtyID,
                                        VolumeID: task.Volume.ID,
                                        text: task.Volume.Name ,
                                        type: "file"
                                    });
                                }

                            } else {
                                eng.children.push({
                                    EngID: task.Engineering.ID,
                                    SpecialtyID: task.Volume.SpecialtyID,
                                    VolumeID: task.Volume.ID,
                                    text: specName,
                                    type: "star",
                                    state: { 'opened': true },
                                    children: [{
                                        EngID: task.Engineering.ID,
                                        SpecialtyID: task.Volume.SpecialtyID,
                                        VolumeID: task.Volume.ID,
                                        text: task.Volume.Name,
                                        type: "file"
                                    }]
                                });
                            }

                        } else {

                            var engNode = {
                                EngID : task.Engineering.ID,
                                text : task.Engineering.Name,
                                type : "folder",
                                state: { 'opened': true },
                                children: [{
                                    EngID: task.Engineering.ID,
                                    SpecialtyID: task.Volume.SpecialtyID,
                                    text: specName,
                                    type: "star",
                                    state: { 'opened': true },
                                    children: [{
                                        EngID: task.Engineering.ID,
                                        SpecialtyID: task.Volume.SpecialtyID,
                                        VolumeID: task.Volume.ID,
                                        text: task.Volume.Name,
                                        type: "file"
                                    }]
                                }]
                            };

                            engs.push(engNode);

                        }
                    });

                    return [{
                        text: "我的任务",
                        type: "folder",
                        state: { 'opened': true },
                        children: engs
                    }];
                }

                $scope.proEngChanged = function (e, data) {
                    if (data.node && data.node.state.selected) {

                        $scope.$safeApply(function () {

                            $scope.currentProEng = data.node.original;

                        });
                    }
                }

                // 表单任务
                $scope.formTaskStatus = { process: true, finish: false };
                $scope.formTaskStatusCount = { 1: 0, 2: 0 };
                $scope.formNames = {};
                desktopService.getMyFormTasks().then(function (result) {
                    $scope.formTasks = result;

                    angular.forEach(result.Source, function (task) {
                        $scope.formTaskStatusCount[task.Status]++;

                        if ($scope.formNames[task.ObjectKey] == undefined) {
                            $scope.formNames[task.ObjectKey] = {
                                name: task.ObjectKey,
                                count: 1,
                                selected: true
                            }
                        } else {
                            $scope.formNames[task.ObjectKey].count++;
                        }
                    });
                });

                // 提资任务
                $scope.provideTaskStatus = { process: true, finish: false };
                $scope.provideTaskStatusCount = { 1: 0, 2: 0 };
                desktopService.getMyProvideTasks().then(function (result) {
                    $scope.provideTasks = result;
                    angular.forEach(result.Source, function (task) {
                        $scope.provideTaskStatusCount[task.Status]++;

                    });
                });
                
            });

        });
