define(['apps/base/base.controller',
	'apps/base/base.service.chatclient'],
    function (module) {

        module.controller('base.controller.chat',
            function ($rootScope,$scope, $sce, chatClient, $stateParams, $uibModalInstance, msgCountSrv, targets,
                currentChatTagrget, attachTypeService, attachDownloadUrl, imagePreviewUrl, messageService) {

                $scope.$sce = $sce;

                // 所有的聊天对象
                $scope.ChatTargets = targets;

                // 聊天服务关闭
                $scope.$on("$chat_server_close", function (a) {

                });

                $scope.$on("$chat_message_receive", function (a, message) {
                    if (message.MessageType == 104) {
                        if (!message.IsNew) {

                            // 更新组信息
                            var thisGroup = $scope.ChatTargets.find(function (g) { return g.GroupID == message.Group.GroupID });

                            thisGroup.GroupName = message.Group.GroupName;
                            thisGroup.GroupDesc = message.Group.GroupDesc;

                            thisGroup.UserIDs = message.Group.UserIDs;
                            thisGroup.Users = message.Group.UserIDs.map(function (u) {
                                return $rootScope.user_item.find(function (_u) {
                                    return _u.ID == u.EmpID;
                                })
                            });
                        }

                    } else if (message.MessageType == 105) {

                        $scope.ChatTargets.custRemove(function (g) {
                            return g.GroupID == message.GroupID
                        });

                        if ($scope.currentChatTagrget.GroupID == message.GroupID) {
                            if ($scope.ChatTargets.length == 0) {
                                $scope.cancel();
                            } else {
                                $scope.currentChatTagrget = $scope.ChatTargets[0];
                            }
                        }
                    } else if (message.MessageType == 106) {

                        // 退组
                        var thisGroup = $scope.ChatTargets.find(function (g) { return g.GroupID == message.GroupID });

                        thisGroup.UserIDs.custRemove(function (u) {
                            return u.EmpID == message.UserID
                        });

                        if (thisGroup.Users) {
                            thisGroup.Users.custRemove(function (u) {
                                return u.ID == message.UserID
                            });
                        }
                    }
                        
                });

                // 切换聊天对象
                $scope.changeTarget = function (target) {
                    $scope.currentChatTagrget = target;
                }

                // 监控聊天对象变化
                $scope.$watch('currentChatTagrget', function (newVal, oldVal) {
                    if (newVal) {

                        // 设置当前聊天对象
                        chatClient.setCurrentChatUser(newVal.ID);

                        // 获取当前聊天对象的聊天记录
                        if (newVal.unReadMsgCount > 0) {
                            //newVal.messages = chatClient.getUserMessages(newVal.ID);

                            msgCountSrv.set(newVal.unReadMsgCount);
                            newVal.unReadMsgCount = 0;
                        }

                        // 获取聊天记录
                        if (!newVal.messageHistory) {

                            var filter = {
                                pagesize: 50,
                                pageindex: 1,
                            };

                            if (newVal.IsGroup) {
                                filter.targetgroup = newVal.ID;
                            } else {
                                filter.targetuser = newVal.ID;
                            }

                            $scope.currentChatTagrget.pageFilter = filter;
                        }
                    }
                });

                $scope.isEnable = function (type) {
                    switch (type) {
                        case 1: return $scope.currentChatTagrget.pageFilter.pageindex == 1; 
                        case 2: return $scope.currentChatTagrget.pageFilter.pageindex == $scope.currentChatTagrget.messageHistory.PageCount;
                        default:
                    }
                }
                $scope.changePage = function (type) {
                    switch (type) {
                        case 1: $scope.currentChatTagrget.pageFilter.pageindex = 1; break;
                        case 2: $scope.currentChatTagrget.pageFilter.pageindex--; break;
                        case 3: $scope.currentChatTagrget.pageFilter.pageindex++; break;
                        case 4: $scope.currentChatTagrget.pageFilter.pageindex = $scope.currentChatTagrget.messageHistory.PageCount; break;
                        default:
                    }
                }

                $scope.$watch("currentChatTagrget.pageFilter", function (newval, oldval) {
                    if (newval) {
                        $scope.currentChatTagrget.messageHistory = messageService.getMyMessage(newval).$object;
                    }
                }, true);

                // 设置当前聊天对象
                $scope.currentChatTagrget = currentChatTagrget;

                // 关闭聊天对象
                $scope.closeTarget = function ($index) {

                    $scope.ChatTargets.splice($index, 1);
                    if ($scope.ChatTargets.length == 0) {
                        $scope.cancel();
                    } else {
                        $scope.currentChatTagrget = $scope.ChatTargets[0];
                    }

                }

                // 关闭聊天窗口
                $scope.cancel = function () {
                    $uibModalInstance.dismiss('cancel');
                };

                // 添加聊天对象
                $scope.setTarget = function (user) {
                    if (user.ID != $scope.currentUser.Account.ID) {
                        user.visiable = true;
                        user.messages = chatClient.getUserMessages(user.ID)

                        var t = $scope.ChatTargets.find(function (t) { return t.ID == user.ID; });

                        if (t) {
                            t.visiable = true;
                            t.messages = user.messages;
                            $scope.currentChatTagrget = t;

                        } else {
                            $scope.ChatTargets.push(user);
                            $scope.currentChatTagrget = user;
                        }
                    }
                }

                function newGuid() {
                    var guid = "";
                    for (var i = 1; i <= 32; i++) {
                        var n = Math.floor(Math.random() * 16.0).toString(16);
                        guid += n;
                        if ((i == 8) || (i == 12) || (i == 16) || (i == 20))
                            guid += "-";
                    }
                    return guid;
                }

                // 发送消息
                $scope.Send = function () {

                    if (!$scope.currentChatTagrget.messages) {
                        $scope.currentChatTagrget.messages = [];
                    }

                    var msg = {
                        ID: newGuid(),
                        IsSending: true,

                        UserIdentity: $scope.currentUser.Account.ID,
                        UserName: $scope.currentUser.Account.Name,

                        Date: new Date(),
                        MessageType: 300,
                        Message: $scope.currentChatTagrget.chatContent
                    };

                    if ($scope.currentChatTagrget.IsGroup) {

                        msg.TargetGroup = $scope.currentChatTagrget.ID;
                        chatClient.setLocalMessage($scope.currentChatTagrget.ID, msg);

                        $scope.$emit("$chat_cmd_sendGroupMsg", {
                            id: msg.ID,
                            msg: msg.Message,
                            target: $scope.currentChatTagrget.ID
                        });

                    } else {

                        msg.TargetUser = $scope.currentChatTagrget.ID;
                        chatClient.setLocalMessage($scope.currentChatTagrget.ID, msg);

                        $scope.$emit("$chat_cmd_sendUserMsg", {
                            id: msg.ID,
                            title: $scope.currentChatTagrget.Title,
                            msg: msg.Message,
                            target: $scope.currentChatTagrget.ID
                        });
                    }

                    $scope.currentChatTagrget.chatContent = "";
                };

                // 回车发送消息
                $scope.KeyUp = function ($event, chat) {
                    if ($event.keyCode == 13) {
                        $scope.Send();
                    }
                }

                // 发送图片和文件
                $scope.sendFiles = [];
          
                // 文件上传完毕后发送消息
                $scope.uploaded = function (result, file) {

                    $scope.sendFiles = [];

                    if (result.files && result.files.length > 0) {

                        var file = result.files[0];
                        var typeInfo = attachTypeService.getType(file.mediaType);

                        var newFile = { ID: file.id, FileName: file.name, FileSize: file.size, FileType: typeInfo.typeID }
                        var id = newGuid();

                        chatClient.setLocalMessage($scope.currentChatTagrget.ID, {
                            ID: id,
                            UserName: $scope.currentUser.Account.Name,
                            Date: new Date(),
                            UserIdentity: $scope.currentUser.Account.ID,
                            MessageType: 303,
                            IsSending: true,
                            Files: [newFile]
                        });

                        $scope.$emit("$chat_cmd_sendFile", {
                            id: id,
                            files: [newFile],
                            title: $scope.currentChatTagrget.Title,
                            target: $scope.currentChatTagrget.ID,
                            isGroup: $scope.currentChatTagrget.IsGroup
                        });
                    }
                };

                // 更新组信息
                $scope.updateGroup = function () {
                    $scope.currentChatTagrget.mgrBlock.block();

                    $scope.currentChatTagrget.Emps = $scope.currentChatTagrget.Users.map(function (u) { return u.ID; });

                    messageService.updateGroup($scope.currentChatTagrget).then(function () {
                        $scope.currentChatTagrget.mgrBlock.unblock();
                        //bootbox.alert('更新成功！');
                    });
                }

                // 删除组
                $scope.removeGroup = function ($index) {
                    bootbox.confirm("群组删除后不能恢复，确定继续删除么？", function (result) {
                        if (result) {

                            $scope.currentChatTagrget.mgrBlock.block();
                            messageService.removeGroup($scope.currentChatTagrget.GroupID).then(function () {
                                $scope.closeTarget($index);
                                $scope.currentChatTagrget.mgrBlock.unblock();
                            });
                        }
                    });
                }

                // 退组
                $scope.exitGroup = function ($index) {
                    bootbox.confirm("退群后将不能主动再次进入该群，确定继续退出么？", function (result) {
                        if (result) {
                            $scope.currentChatTagrget.mgrBlock.block();
                            messageService.exitGroup($scope.currentChatTagrget.GroupID).then(function () {
                                $scope.closeTarget($index);
                                $scope.currentChatTagrget.mgrBlock.unblock();
                            });
                        }
                    });
                }
            });
    });
