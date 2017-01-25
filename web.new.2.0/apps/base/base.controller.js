define(['angularAMD',

    'angular-moment',
    'angular-ui-select',
    'angular-ui-grid',
    'angular-validation',

    'ng-tagsinput',
    'rate',

    'apps/base/base.service',
    'apps/base/base.service.permission',
    'apps/base/base.service.enum',
    'apps/base/base.service.notification',
    'apps/base/base.service.attach',
    'apps/base/base.service.tag',
    'apps/base/base.service.process',
    'apps/base/base.service.message',
    'apps/base/base.service.settings',

    'apps/base/base.directive.book',
    'apps/base/base.directive.editor',
    'apps/base/base.directive.tagsinput',
    'apps/base/base.directive.popover',
    'apps/base/base.directive.panel',
    'apps/base/base.directive.icheck',
    'apps/base/base.directive.tree',
    'apps/base/base.directive.split',
    'apps/base/base.directive.drawer',
    'apps/base/base.directive.wizard',
    'apps/base/base.directive.permission',
    'apps/base/base.directive.datepicker',
    'apps/base/base.directive.plUpload',
    'apps/base/base.directive.notification',
    'apps/base/base.directive.process',
    'apps/base/base.directive.user',
    'apps/base/base.directive.charts'],

    function (angularAMD) {

        var controller = angular.module('base.controller', [
            'base.service','angularMoment',
            'ngCkeditor',
            'ngAnimate',
            'ui.bootstrap',
            'ngSanitize',
            'ngTouch',
            'ngTagsInput',
            'ngRateIt',
            'ui.router',
            'ct.ui.router.extras',
            'ui.select',
            'ui.grid',
            'ui.grid.selection',
            'ui.grid.treeView',
            'ui.grid.edit',
            'ui.grid.cellNav',
            'ui.grid.infiniteScroll',
            'ui.grid.pagination',
            'ui.grid.exporter',
            'ui.grid.autoResize',
            'ui.grid.grouping',
            'ui.grid.resizeColumns',
            'ui.grid.moveColumns',
            'ui.grid.pinning',
            'ui.grid.cellNav',
            'validation', 'validation.rule',
            'ui.bootstrap.timepicker',
            'ui.bootstrap.popover',
            'ui.bootstrap.progressbar',
            'ui.bootstrap.typeahead',
            'ui.bootstrap.modal',
            'ui.bootstrap.accordion',
            'ui.bootstrap.tabs']);

        controller.controller('base.controller.page',
            function ($rootScope, $scope, $location, $state, $timeout, $uibModal, menuService,
                localStorageService, notificationService, userService, chatClient,
                imagePreviewUrl, userApiUrl, userApiVersion, moment,Restangular) {

                moment.locale("zh-cn");

                // 菜单栏最小化后的鼠标事件
                $scope.initSilder = handSilderHover;

                $scope.imageBaseSrc = imagePreviewUrl;

                $scope.changeBusiness = function (b) {
                    $rootScope.currentBusiness = b;
                }

                $scope.$watch("currentBusiness", function (newVal, oldVal) {

                    if(newVal && newVal.tabs.length > 0){
                        var firstTab = newVal.tabs[0];

                        $state.go(firstTab.state, firstTab.params);
                    }
                   
                    $rootScope.$broadcast("businessChanged", newVal);
                });

               
                $rootScope.filterUsers = function (filter) {

                    var result = [];
                    angular.forEach($rootScope.user_item, function (user) {

                        if (filter.length == 0 || user.Name.indexOf(filter) >= 0) {
                            result.push({
                                ID: user.ID,
                                Name: user.Name,
                                Dept: user.Dept,
                                PhotoImg: user.PhotoImg
                            });
                        }
                    })

                    return result;
                }

                var getMyGroupCount = function () {
                    var count = 0;

                    angular.forEach($scope.chatGroups, function (g) {
                        if (g.CreateEmpID == $scope.currentUser.Account.ID) {
                            count++;
                        }
                    });

                    return count;
                }

                $scope.$watch("maxGroupCount", function (newval, oldval) {
                    if (newval ) {
                        $scope.canCreateGroup = getMyGroupCount() < newval;
                    }
                });
                $scope.$watchCollection("chatGroups", function (newval, oldval) {
                    if (newval) {
                        $scope.canCreateGroup = getMyGroupCount() < $scope.maxGroupCount;
                    }
                });

                $scope.openNotifyWindow = function (notify) {
                    $uibModal.open({
                        animation: false,
                        templateUrl: 'apps/base/view/notification-list.html',
                        size: 'lg',

                        controller: function ($scope, $uibModal, $uibModalInstance, currentNotify) {

                            $scope.notifications = notificationService.getEffects(1000).$object;
                            $scope.setCurrentNotify = function (notify) {
                                $scope.currentNotify = notify;
                            };
                            $scope.$watch("currentNotify", function (newval, oldval) {
                                if (newval && !newval.IsRead) {

                                    newval.IsRead = true;

                                    // 设置提醒已读
                                    notificationService.read(newval.ID);
                                }
                            });
                            $scope.viewObject = function () {

                                var info = {};
                                var objItems = $rootScope.getBaseData("Object");
                                var objTags = objItems.find(function (item) {
                                    return item.Tags["name"] == $scope.currentNotify.SourceName;
                                });
                                info.controller = objTags["controller"];
                                info.template = objTags["template"];
                                info.controllerUrl = objTags["controllerUrl"];

                                //angular.forEach($rootScope.Object_item, function (item) {
                                //    if (item.Tags["name"] == $scope.currentNotify.SourceName) {
                                //        info.controller = item.Tags["controller"];
                                //        info.template = item.Tags["template"];
                                //        info.controllerUrl = item.Tags["controllerUrl"];
                                //    }
                                //});

                                $uibModal.open({
                                    animation: false,
                                    size: 'lg',
                                    windowTopClass: 'fade',
                                    templateUrl: info.template,
                                    controller: info.controller,
                                    resolve: {
                                        loadController: angularAMD.$load(info.controllerUrl),
                                        objParam: function () {
                                            return {
                                                id: $scope.currentNotify.SourceID,
                                                view: true
                                            }
                                        },
                                    }
                                });
                            };
                            $scope.close = function () {
                                $uibModalInstance.dismiss('cancel');
                            }

                            $scope.currentNotify = currentNotify;
                        },
                        resolve: {
                            currentNotify: function () {
                                return notify;
                            }
                        }
                    });
                };

                $scope.settings = function () {
                    $uibModal.open({
                        animation: false,
                        templateUrl: 'apps/base/view/personal-settings.html',
                        size: 'lg',

                        controller: function ($scope, $uibModal, $uibModalInstance, userService) {

                            $scope.chooseImg1 = function (id) {
                                $scope.currentUser.Account.PhotoImg = id + ".jpg";

                                userService.setUserImage(id + ".jpg", id + "x.jpg");
                            }

                            $scope.chooseImg2 = function (id) {
                                $scope.currentUser.Account.PhotoImg = 'avatar' + id + ".png";

                                userService.setUserImage('avatar' + id + ".png", 'avatar' + id + "_big.png");
                            }

                            $scope.close = function () {
                                $uibModalInstance.dismiss('cancel');
                            }

                        }
                    });
                }

                $scope.isActive = function (viewLocation) {
                    return viewLocation === $location.path();
                };


                $scope.addToQuickLink = function(){
                    
                    userService.addUserConfig({
                        ConfigName  : 'QuickLink',
                        ConfigKey : $state.current.name,
                        ConfigValue : JSON.stringify($state.params),
                        ConfigText :  $state.current.text,
                    }).then(function(result){

                        if(result == 3){
                            bootbox.alert( $state.current.text + "重复添加");
                        }else{
                            bootbox.alert( $state.current.text + "加入快速访问成功");
                        }
                    });
                }

                $scope.goQuick = function(item){
                   
                    $state.go(item.ConfigKey,JSON.parse(item.ConfigValue))
                }

                // 注销
                $scope.logout = function () {

                    localStorageService.remove('user_' + $rootScope.currentUser.Account.ID);

                    var allUser = localStorageService.get("all_user");

                    for (var i = 0; i < allUser.length; i++) {
                        if (allUser[i] == $rootScope.currentUser.Account.ID) {
                            allUser.splice(i, 1);
                            break;
                        }
                    }

                    localStorageService.set('all_user', allUser);

                    // 断开当前登录用户的聊天连接
                    chatClient.disconnect();

                    if (allUser.length == 0) {
                        $state.go("login");
                    } else {
                        $state.go("chooseuser");
                    }
                }

                // 切换用户
                $scope.chooseuser = function () {

                    $state.go("chooseuser");
                }

                // 访问的历史记录
                $scope.viewHistorys = [];

                $scope.openMenu = function (menu) {

                    $scope.viewHistorys.push(menu);
                }

                $scope.goHis = function (m) {

                    $state.go(m.Href, m.Param);
                }

                // 聊天对象
                $scope.ChatTargets = [];
                // 在线用户列表
                $rootScope.onLineUsers = [];

                $scope.msgCount = 0;
                $scope.groupMsgCount = 0;
                $scope.userMsgCount = 0;

                // 聊天消息到达
                $scope.$on("$chat_message_receive", function (a, message) {
                    $timeout(function () {
                        if (message.MessageType == 300 ||
                            message.MessageType == 303) {

                            if (!message.isRead) {
                                $scope.msgCount++;
                            }

                            var user = $rootScope.user_item.find(function (uu) { return uu.ID == message.UserIdentity });

                            if (user) {

                                user.Title = message.Title;
                                message.UserPhotoImg = user.PhotoImg;

                                if (message.TargetUser > 0) {
                                    if (!message.isRead) {
                                        $scope.userMsgCount++;
                                        if (user.unReadMsgCount) {
                                            user.unReadMsgCount++;
                                        } else {
                                            user.unReadMsgCount = 1;
                                        }
                                    }

                                    user.lastMsg = message;

                                } else {
                                    var group = $scope.chatGroups.find(function (uu) { return uu.GroupID == message.TargetGroup; });
                                    if (!message.isRead) {
                                        $scope.groupMsgCount++;
                                        if (group.unReadMsgCount) {
                                            group.unReadMsgCount++;
                                        } else {
                                            group.unReadMsgCount = 1;
                                        }

                                        group.lastMsg = message;
                                    }
                                }
                            }
                        }
                        else if (message.MessageType == 302) {
                            // 发送的消息服务器已接收

                        } else if (message.MessageType == 100) {

                            // 新的用户连接到聊天服务
                            $rootScope.onLineUsers[message.NewConnecedUserID] = message.NewConnecedUserName;

                        } else if (message.MessageType == 102) {
                            // 用户离线
                            $rootScope.onLineUsers[message.OutlineUserIdentity] = undefined;
                        } else if (message.MessageType == 103) {

                            $scope.msgCount += message.UnReadMessage.length;

                            // 用户离线消息
                            angular.forEach(message.UnReadMessage, function (ms) {

                                var user = $rootScope.user_item.find(function (uu) { return uu.ID == ms.UserIdentity });

                                if (user) {

                                    ms.UserPhotoImg = user.PhotoImg;

                                    if (ms.TargetUser > 0) {

                                        $scope.userMsgCount++;
                                        if (user.unReadMsgCount) {
                                            user.unReadMsgCount++;
                                        } else {
                                            user.unReadMsgCount = 1;
                                        }

                                        user.lastMsg = ms;
                                    }
                                }
                            });

                        }
                        else if (message.MessageType == 104) {

                            if (message.IsNew) {
                                $scope.chatGroups.push(message.Group);
                            } else {
                                // 更新组信息
                                
                                //var thisGroup = $scope.chatGroups.find(function (g) { return g.GroupID == message.Group.GroupID }) ;
                                    
                                //thisGroup= message.Group;

                                for (var i = 0; i < $scope.chatGroups.length; i++) {
                                    if ($scope.chatGroups[i].GroupID == message.Group.GroupID) {
                                        $scope.chatGroups[i] = message.Group;
                                        break;
                                    }
                                }
                            }

                        } else if (message.MessageType == 105) {

                            // 删除组
                            $scope.chatGroups.custRemove(function (g) {
                                return g.GroupID == message.GroupID
                            });
                        } else if (message.MessageType == 106) {

                            // 退组
                            var thisGroup = $scope.chatGroups.find(function (g) { return g.GroupID == message.GroupID });
                            
                            thisGroup.UserIDs.custRemove(function (u) {
                                return u.EmpID == message.UserID
                            });

                            if (thisGroup.Users) {
                                thisGroup.Users.custRemove(function (u) {
                                    return u.ID == message.UserID
                                });
                            }

                        } else if (message.MessageType == 107) {

                            // 用户所在的组
                            $scope.chatGroups = message.Groups;
                        } else if (message.MessageType == 108) {
                            // 在线用户列表
                            $rootScope.onLineUsers = message.Users;
                        } else if (message.MessageType == 100) {
                            // 有新的用户登录
                            $rootScope.onLineUsers[message.NewConnecedUserID] = message.NewConnecedUserName;
                        }
                    });
                });
                // 断开聊天服务
                $scope.$on("$chat_server_close", function (a, message) {
                    $rootScope.onLineUsers = [];
                });

                // 添加聊天对象到tab中
                var addChatObject = function (target) {

                    var t = $scope.ChatTargets.find(function (t) { return t.ID == target.ID; });

                    if (t && !t.visiable) {
                        target.messages = t.messages;

                        $scope.ChatTargets.push(target);
                        $scope.ChatTargets.removeObj(t);

                    } else if (!t) {
                        $scope.ChatTargets.push(target);
                    }
                }

                // 打开聊天窗口
                $rootScope.openChat = function (user, title) {

                    if (!user.ID) {
                        user = $scope.user_item.find(function (u) { return u.ID == user });
                    }

                    $scope.msgCount = $scope.msgCount - user.unReadMsgCount;
                    $scope.userMsgCount = $scope.userMsgCount - user.unReadMsgCount;
                    user.visiable = true;
                    
                    user.unReadMsgCount = 0;
                    user.messages = chatClient.getUserMessages(user.ID);
                    if (title) {
                        user.Title = title;
                    }
                    
                    addChatObject(user);

                    var modalInstance = $uibModal.open({
                        animation: false,
                        dialogClass: 'dragable',
                        templateUrl: 'apps/base/view/chat.html',
                        controller: 'base.controller.chat',
                        size: 'lg',
                        resolve: {
                            targets: function () {
                                return $scope.ChatTargets;
                            },
                            currentChatTagrget: function () {
                                return user;
                            },
                            msgCountSrv: function () {
                                return {
                                    set: function (unReadMsgCount) {
                                        $scope.msgCount = $scope.msgCount - unReadMsgCount;
                                        $scope.userMsgCount = $scope.userMsgCount - unReadMsgCount;
                                    }
                                };
                            }
                        }
                    });

                    modalInstance.result.then(function () {
                        //success

                        // 设置当前聊天对象
                        chatClient.setCurrentChatUser(0);
                    }, function () {
                        //dismissed

                        // 设置当前聊天对象
                        chatClient.setCurrentChatUser(0);
                    });
                };

                // 打开组聊天窗口
                $rootScope.openGroupChat = function (group) {

                    $scope.msgCount = $scope.msgCount - group.unReadMsgCount;
                    $scope.groupMsgCount = $scope.groupMsgCount - group.unReadMsgCount;

                    group.ID = group.GroupID;
                    //group.Name = group.GroupName;
                    //group.PhotoImg = 'assets/global/images/avatars/' + (group.IsPublic ? 'b2x.jpg' : 'ba2x.jpg');
                    group.IsGroup = true;
                    group.visiable = true;
                    group.unReadMsgCount = 0;
                    group.messages = chatClient.getUserMessages(group.GroupID)

                    if (group.IsPublic) {
                        group.Users = $rootScope.user_item;
                    } else if (group.UserIDs.length > 0 && !group.Users) {
                       
                        group.Users = group.UserIDs.map(function (u) {
                            return $rootScope.user_item.find(function (_u) {
                                return _u.ID == u.EmpID;
                            })
                        });
                    }

                    addChatObject(group);

                    var modalInstance = $uibModal.open({
                        animation: false,
                        dialogClass: 'dragable',
                        templateUrl: 'apps/base/view/chat.html',
                        controller: 'base.controller.chat',
                        size: 'lg',
                        resolve: {
                            targets: function () {
                                return $scope.ChatTargets;
                            },
                            currentChatTagrget: function () {
                                return group;
                            },
                            msgCountSrv: function () {
                                return {
                                    set: function (unReadMsgCount) {
                                        $scope.msgCount = $scope.msgCount - unReadMsgCount;
                                        $scope.groupMsgCount = $scope.groupMsgCount - unReadMsgCount;
                                    }
                                };
                            }
                        }
                    });

                    modalInstance.result.then(function () {
                        //success

                        // 设置当前聊天对象
                        chatClient.setCurrentChatUser(0);
                    }, function () {
                        //dismissed

                        // 设置当前聊天对象
                        chatClient.setCurrentChatUser(0);
                    });
                }

                // 创建聊天分组
                $scope.createChatGroup = function () {

                    var modalInstance = $uibModal.open({
                        animation: true,
                        dialogClass: 'dragable',
                        templateUrl: 'apps/base/view/crate-chatgroup.html',
                        controller: 'base.controller.createchatgroup'
                    });
                }

                // 修改密码窗口
                $scope.changePassword = function () {
                    $uibModal.open({
                        animation: true,
                        templateUrl: 'apps/base/view/change-password.html',
                        controller: 'base.controller.changePassword'
                    });
                }

                // 页面参数
                $scope.lang = 'zh-cn';
                $scope.pageSize = 20;
                $scope.pageSizes = [20, 50, 100];

                // 保存文件流
                $rootScope.saveAs = function (res, fileName) {
                    var myFile = streamSaver.createWriteStream(fileName);
                    let reader = res.body.getReader()
                    let pump = () => {
                        return reader.read().then(({ value, done }) => {
                            if (done) {
                                myFile.close()
                                return
                            }
                            myFile.write(value)
                            return pump()
                        })
                    }
                    pump()
                }

                Restangular.setErrorInterceptor(function (response, deferred, responseHandler) {
                    if (response.status === 500) {

                        notificationService.showException(response.data.ExceptionMessage,response.data.ExceptionType,response.data.StackTrace);

                        return false; // error handled
                    }

                    return true; // error not handled
                });
            });

        return controller;

    });
