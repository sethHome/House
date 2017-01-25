define(['apps/base/base.controller'],
    function (module) {

        module.controller('base.controller.chooseuser',
            function ($rootScope, $scope, $state, localStorageService, chatClient, imagePreviewUrl) {

                var allUser = localStorageService.get("all_user");
                $scope.users = [];

                //if (!allUser || allUser.length == 0) {
                //    $state.go("login");
                //}

                angular.forEach(allUser, function (id) {
                    var u = localStorageService.get("user_" + id);
                    $scope.users.push(u);
                });

                $scope.enter = function (user) {
                    $rootScope.currentUser = user;

                    // 使用新的登录用户连接到聊天服务器
                    chatClient.disconnect();
                    chatClient.connect($rootScope.currentUser);

                    $rootScope.$broadcast("$login", user);
                };

                $scope.logout = function (user) {
                    localStorageService.remove('user_' + user.Account.ID);

                    for (var i = 0; i < allUser.length; i++) {
                        if (allUser[i] == user.Account.ID) {
                            allUser.splice(i, 1);
                            $scope.users.splice(i, 1);
                            break;
                        }
                    }

                    // 断开当前登录用户的聊天连接
                    chatClient.disconnect();

                    localStorageService.set('all_user', allUser);

                    if (allUser.length == 0) {
                        $state.go("login");
                    }
                }

                $scope.$watch('$viewContentLoaded', function () {

                    //var circle = new ProgressBar.Circle('.loader', {
                    //    color: '#aaa',
                    //    strokeWidth: 5,
                    //    trailWidth: 5,
                    //    trailColor: 'rgba(255,255,255,0.9)',
                    //    easing: 'easeInOut',
                    //    duration: 2000,
                    //    from: {
                    //        color: '#319DB5',
                    //        width: 5
                    //    },
                    //    to: {
                    //        color: '#319DB5',
                    //        width: 5
                    //    },
                    //    // Set default step function for all animate calls
                    //    step: function (state, circle) {
                    //        circle.path.setAttribute('stroke', state.color);
                    //        circle.path.setAttribute('stroke-width', state.width);
                    //    }
                    //});

                    //circle.animate(1);

                })

            });
    });
