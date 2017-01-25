define(['apps/base/base.controller'],
    function (module) {

        module.controller('base.controller.createchatgroup',
            function ($scope, $rootScope, $uibModalInstance, messageService) {
                $scope.groupInfo = {};

                $scope.create = function () {
                    //Chat.MaxGroupCount

                   

                    messageService.createChatGroup($scope.groupInfo).then(function (id) {

                        $scope.groupInfo.GroupID = "G" + id;
                        $scope.groupInfo.GroupName = $scope.groupInfo.Name;
                        $scope.groupInfo.UserIDs = $scope.groupInfo.Emps;
                        $scope.groupInfo.Users = $scope.users;

                        $uibModalInstance.close($scope.groupInfo);
                    });
                }

                $scope.$watchCollection("users", function (newval, oldval) {
                    if (newval) {

                        $scope.groupInfo.Emps = newval.map(function (u) { return u.ID; });
                    }
                });

                $scope.close = function () {
                    $uibModalInstance.dismiss('cancel');
                }
            });
    });
