define(['apps/system3/production/production.controller',
        'apps/system3/production/volume/volume.controller.plan',
        'apps/system3/production/volume/volume.service'], function (app) {

            app.controller('production.controller.progress.detail', function ($scope,$uibModalInstance, volParams,volumeService, processService) {

                $scope.flowInfo = processService.getFlowDetailByObj('Volume', volParams.volumeInfo.ID).$object;

                // 关闭编辑模式
                $scope.close = function () {
                    $uibModalInstance.dismiss('cancel');
                }

                $scope.volumeStatistics = volumeService.getVolumeStatisticsInfo(volParams.volumeInfo.ID).$object;

            });
        });