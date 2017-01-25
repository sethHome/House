define(['apps/system3/production/production.controller',
        'apps/system3/production/note/note.service'], function (app) {

            app.controller('production.controller.note.maintain', function ($scope, noteService, $uibModalInstance, maintainParam) {

                $scope.noteInfo = maintainParam.entityInfo ? maintainParam.entityInfo : {};

                if (maintainParam.engID > 0) {
                    $scope.hasEngineering = true;
                    $scope.noteInfo.Engineering = { ID: maintainParam.engID };
                }
                

                // 加载工程
                $scope.loadEngineering = function (textFilter) {

                    return noteService.loadEngSource({ txtfilter: textFilter });
                };

                $scope.save = function () {
                    
                    if ($scope.noteInfo.ID > 0) {

                        noteService.update($scope.noteInfo);

                        $uibModalInstance.dismiss('cancel');

                    } else {
                        $scope.noteInfo.EngineeringID = $scope.noteInfo.Engineering.ID;

                        noteService.create($scope.noteInfo).then(function (id) {
                            $scope.noteInfo.ID = id;
                            $uibModalInstance.close($scope.noteInfo);
                        });
                    }
                }

                // 关闭编辑模式
                $scope.close = function () {
                    $uibModalInstance.dismiss('cancel');
                }
            });


        });