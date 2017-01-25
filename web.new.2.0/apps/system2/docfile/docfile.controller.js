define([
'apps/system2/docfile/docfile'], function (app) {

    app.module.controller("docfile.controller.fieldcondition", function ($scope, conditionService, $uibModalInstance) {
        $scope.fields = conditionService.fieldList;
        $scope.conditions = conditionService.conditions;
        $scope.condition = {};


        $scope.addCondition = function () {

            $scope.conditions.push(angular.copy($scope.condition));
        }

        var getCondtions1 = function () {

            return
        }

        $scope.query = function () {

            if ($scope.currentIndex == 1) {

                var items = $scope.fields.where(function (f) {
                    return f.Operator != undefined;
                });

                var conditins = items.map(function (f) {
                    return {
                        Field: f,
                        Value: f.FilterValue,
                        Operator: f.Operator,
                        LogicOperation: 'AND'
                    }
                });

                $uibModalInstance.close(conditins);
            }
            else {
                $uibModalInstance.close($scope.conditions);
            }
        }
        // 清空查询条件
        $scope.clear = function () {
            conditionService.fieldList = conditionService.fieldList.map(function (f) {
                f.FilterValue = undefined;
                f.Operator = undefined;
            });
            $scope.condition = {};
            $scope.conditions = [];
            conditionService.clearConditions();
        };
        // 关闭
        $scope.close = function () {
            $uibModalInstance.dismiss('cancel');
        }
    })

});
