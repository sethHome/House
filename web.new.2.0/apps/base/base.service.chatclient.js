define(['apps/base/base.service'],
    function (module) {

        module.factory("chatClient", function ($window, $rootScope, wsAddress, $timeout, notifyClient) {

            // ws连接对象
            var ws = undefined;

            // 所有消息
            var _clientMessages = {};

            // 未读消息
            var _clientUnReadMessages = {};

            // 当前聊天用户
            var _currentChatUser = 0;

            // 当前用户所在的组
            var _UserGroups = [];

            // 发送群聊消息
            var _sendGroupMessage = function (ms) {

                if (ms.msg.length == 0) {
                    return;
                }

                var msg = {
                    ID: ms.id,
                    TargetGroup: ms.target,
                    Message: ms.msg,
                    MessageType: 300
                };

                ws.send(JSON.stringify(msg));
            };

            // 发送私聊消息
            var _sendUserMessage = function (ms) {
                if (ms.msg.length == 0) {
                    return;
                }

                var msg = {
                    ID: ms.id,
                    Title: ms.title,
                    TargetUser: ms.target,
                    Message: ms.msg,
                    MessageType: 300
                };

                ws.send(JSON.stringify(msg));
            };

            // 发送文件
            var _sendFile = function (ms) {

                if (ms.files.length == 0) {
                    return;
                }

                var msg = {
                    ID: ms.id,
                    Title: ms.title,
                    Files: ms.files,
                    MessageType: 303
                };

                if (ms.isGroup) {
                    msg.TargetGroup = ms.target;
                } else {
                    msg.TargetUser = ms.target;
                }

                ws.send(JSON.stringify(msg));
            };

            // 获取在线用户列表
            var _getOnlineUsers = function () {
                ws.send(JSON.stringify({
                    MessageType: 108,
                }));
            };

            // 缓存连接未打开时发来的聊天命令
            var _cmdList = {};

            $rootScope.$on("$chat_cmd_sendGroupMsg", function (e, data) {
                if (ws && ws.readyState == WebSocket.OPEN) {
                    _sendGroupMessage(data)
                } else {

                    if (_cmdList.$chat_cmd_sendGroupMsg) {
                        _cmdList.$chat_cmd_sendGroupMsg.push({
                            cmd: '$chat_cmd_sendGroupMsg',
                            arg: data
                        });
                    } else {
                        _cmdList.$chat_cmd_sendGroupMsg = [{
                            cmd: '$chat_cmd_sendGroupMsg',
                            arg: data
                        }];
                    }
                }
            });

            $rootScope.$on("$chat_cmd_sendUserMsg", function (e, data) {
                if (ws && ws.readyState == WebSocket.OPEN) {

                    _sendUserMessage(data)
                } else {

                    if (_cmdList.$chat_cmd_sendUserMsg) {
                        _cmdList.$chat_cmd_sendUserMsg.push({
                            cmd: '$chat_cmd_sendUserMsg',
                            arg: data
                        });
                    } else {
                        _cmdList.$chat_cmd_sendUserMsg = [{
                            cmd: '$chat_cmd_sendUserMsg',
                            arg: data
                        }];
                    }
                }
            });

            $rootScope.$on("$chat_cmd_sendFile", function (e, data) {
                if (ws && ws.readyState == WebSocket.OPEN) {

                    _sendFile(data)
                } else {

                    if (_cmdList.$chat_cmd_sendFile) {
                        _cmdList.$chat_cmd_sendFile.push({
                            cmd: '$chat_cmd_sendFile',
                            arg: data
                        });
                    } else {
                        _cmdList.$chat_cmd_sendFile = [{
                            cmd: '$chat_cmd_sendFile',
                            arg: data
                        }];
                    }
                }
            });

            $rootScope.$on("$chat_cmd_getOnlineUsers", function (e, data) {
                if (ws && ws.readyState == WebSocket.OPEN) {
                    _getOnlineUsers(data)
                } else {

                    if (_cmdList.$chat_cmd_getOnlineUsers) {
                        _cmdList.$chat_cmd_getOnlineUsers.push({
                            cmd: '$chat_cmd_getOnlineUsers',
                            arg: data
                        });
                    } else {
                        _cmdList.$chat_cmd_getOnlineUsers = [{
                            cmd: '$chat_cmd_getOnlineUsers',
                            arg: data
                        }];
                    }
                }
            });

            var client = {

                connect: function (user) {

                    if (!ws || ws.readyState != WebSocket.OPEN) {

                        console.log("connect to " + wsAddress + " " + reConnectTime + " time");

                        var serverAddress = wsAddress + 'chat?identity=' + user.Account.ID + '&name=' + user.Account.Name;
                        //var serverAddress = 'ws:/127.0.0.1:5000/chat?identity=' + user.Account.ID + '&name=' + user.Account.Name;

                        connect(window.encodeURI(serverAddress));
                    }

                    notifyClient.connect(user);
                },

                disconnect: function () {
                    if (ws) {
                        ws.close();
                    }

                    notifyClient.disconnect();
                },

                // 当前客户端收到的所有消息
                getClientMessages: function () {
                    return _clientMessages;
                },

                // 获取未读消息
                getUnReadGroupMessages: function (groupID) {
                    var result = {};
                    if (groupID) {

                        angular.forEach(_clientMessages[groupID], function (message) {

                            if (!message.isRead) {
                                if (result[message.TargetGroup]) {
                                    result[message.TargetGroup].push(message);
                                } else {
                                    result[message.TargetGroup] = [message];
                                }
                            }
                        })

                    } else {

                        angular.forEach(_clientMessages, function (group) {

                            angular.forEach(group, function (message) {

                                if (!message.isRead) {
                                    if (result[message.TargetGroup]) {
                                        result[message.TargetGroup].push(message);
                                    } else {
                                        result[message.TargetGroup] = [message];
                                    }
                                }
                            })

                        });
                    }

                    return result;
                },

                getUserMessages: function (userID) {
                    if (!_clientMessages[userID]) {
                        _clientMessages[userID] = [];
                    }

                    return _clientMessages[userID];
                },

                getGroupMessages: function (groupID) {
                    if (!_clientMessages[groupID]) {
                        _clientMessages[groupID] = [];
                    }

                    return _clientMessages[groupID];
                },

                // 获取未读个人消息
                getUnReadMessages: function (userID) {
                    return _clientUnReadMessages[userID];
                    //var result = [];
                    //if (userID) {

                    //    angular.forEach(_clientMessages[userID], function (message) {

                    //        if (!message.isRead) {
                    //            result.push(message);
                    //        }
                    //    })

                    //} 
                    //return result;
                },

                // 设置用户消息已读
                msgReaded: function (userID) {
                    if (userID) {
                        angular.forEach(_clientMessages[userID], function (message) {

                            message.isRead = true;
                        })
                    } else {
                        angular.forEach(_clientMessages, function (group) {

                            angular.forEach(group, function (message) {

                                message.isRead = true;
                            })

                        });
                    }

                    $rootScope.$broadcast("$chat_message_readed", userID);
                },

                // 获取用户所属的组
                getUserGroups: function () {
                    return _UserGroups;
                },

                // 设置当前聊天用户
                setCurrentChatUser: function (user) {
                    _currentChatUser = user;
                },

                setLocalMessage: function (user, message) {
                    if (_clientMessages[user]) {
                        _clientMessages[user].push(message);
                    } else {
                        _clientMessages[user] = [message];
                    }
                }
            };

            var reConnectTime = 0;

            // 连接到聊天服务
            var connect = function (serverAddress) {

                var support = "MozWebSocket" in window ? 'MozWebSocket' : ("WebSocket" in window ? 'WebSocket' : null);

                if (support == null) {
                    alert("浏览器不支持WebSocket协议");
                    return;
                }

                ws = new window[support](serverAddress);

                ws.onmessage = function (evt) {

                    var message = JSON.parse(evt.data);

                    // 缓存聊天消息
                    if (message.MessageType == 300 || message.MessageType == 303) {

                        // 不是自己发送的消息发送提醒
                        if (message.UserIdentity != $rootScope.currentUser.Account.ID) {

                            if (PmEx && !PmEx.IsWindowActive()) {

                                PmEx.FlashTaskBar();

                            } else if (document.visibilityState != 'visible') {

                                Notification.requestPermission(function (perm) {

                                    //判断是否允许桌面通知 "granted"：允许 "denied":拒绝 "default": 等于拒绝，但用户是还未选择状态

                                    if (perm == "granted") {
                                        var notification = new Notification('来自[' + message.UserName + ']的消息', {

                                            tag: "message", //标签
                                            icon: "http://localhost/pm_std_service/api/v1/image/238", //图片
                                            body: message.MessageType == 303 ? "文件或图片" : message.Message //内容
                                        });

                                        //三秒之后关闭
                                        notification.onshow = function () {
                                            setTimeout(function () {
                                                notification.close();
                                            }, 3000);
                                        }
                                    }
                                });
                            }
                        }

                        if (_currentChatUser == message.UserIdentity ||
                            _currentChatUser == message.TargetGroup) {
                            // 如果正在和当前聊天对象聊天，则消息为已读
                            message.isRead = true;
                        } else {
                            message.isRead = false;
                        }

                        var target = message.UserIdentity;

                        if (message.TargetGroup) {
                            target = message.TargetGroup;
                        }

                        if (_clientMessages[target]) {
                            _clientMessages[target].push(message)
                        } else {
                            _clientMessages[target] = [message];
                        }

                    } else if (message.MessageType == 103) {

                        // 用户离线消息
                        angular.forEach(message.UnReadMessage, function (ms) {

                            ms.isRead = false;
                            ms.UserIdentity = ms.UserIdentity.trim();
                            ms.UserName = ms.UserName.trim();

                            if (_clientMessages[ms.UserIdentity]) {
                                _clientMessages[ms.UserIdentity].push(ms)
                            } else {
                                _clientMessages[ms.UserIdentity] = [ms];
                            }
                        });

                    } else if (message.MessageType == 107) {

                        // 用户所在的组
                        _UserGroups = message.Groups;

                    } else if (message.MessageType == 302) {
                        // 发送的消息已接收
                        if (message.Target) {
                            var ms = _clientMessages[message.Target].find(function (m) { return m.ID == message.ID });
                            ms.IsSending = false;
                        }
                    }

                    $rootScope.$broadcast("$chat_message_receive", message);

                    $rootScope.$apply();
                };

                ws.onopen = function () {

                    // 未处理的命令
                    if (_cmdList.$chat_cmd_openGroup && _cmdList.$chat_cmd_openGroup.length > 0) {
                        angular.forEach(_cmdList.$chat_cmd_openGroup, function (cmd) {
                            _openGroup(cmd.arg.groupID, cmd.arg.withMessageHistory);
                        })

                        _cmdList.$chat_cmd_openGroup = [];
                    }
                    if (_cmdList.$chat_cmd_openGroups && _cmdList.$chat_cmd_openGroups.length > 0) {
                        angular.forEach(_cmdList.$chat_cmd_openGroups, function (cmd) {

                            _openGroups(cmd.arg.groups, cmd.arg.withMessageHistory);
                        })

                        _cmdList.$chat_cmd_openGroups = [];
                    }
                    if (_cmdList.$chat_cmd_sendGroupMsg && _cmdList.$chat_cmd_sendGroupMsg.length > 0) {
                        angular.forEach(_cmdList.$chat_cmd_sendGroupMsg, function (cmd) {
                            _sendGroupMessage(cmd.arg)
                        })

                        _cmdList.$chat_cmd_sendGroupMsg = [];
                    }
                    if (_cmdList.$chat_cmd_sendUserMsg && _cmdList.$chat_cmd_sendUserMsg.length > 0) {
                        angular.forEach(_cmdList.$chat_cmd_sendUserMsg, function (cmd) {
                            _sendUserMessage(cmd.arg)
                        })

                        _cmdList.$chat_cmd_sendUserMsg = [];
                    }
                    if (_cmdList.$chat_cmd_sendFile && _cmdList.$chat_cmd_sendFile.length > 0) {
                        angular.forEach(_cmdList.$chat_cmd_sendFile, function (cmd) {
                            _sendFile(cmd.arg)
                        })

                        _cmdList.$chat_cmd_sendFile = [];
                    }

                    if (_cmdList.$chat_cmd_getOnlineUsers && _cmdList.$chat_cmd_getOnlineUsers.length > 0) {
                        angular.forEach(_cmdList.$chat_cmd_getOnlineUsers, function (cmd) {
                            _getOnlineUsers(cmd.arg);
                        })

                        _cmdList.$chat_cmd_getOnlineUsers = [];
                    }
                    $rootScope.chatOnline = true;

                    $rootScope.$broadcast("$chat_server_connected");

                    $rootScope.$apply();
                };

                ws.onclose = function () {

                    $rootScope.chatOnline = false;

                    $rootScope.$broadcast("$chat_server_close");

                    //if ($rootScope.sysSettings["Chat.AutoConnect"].toUpperCase() == "TRUE") {

                    //    // 自增重连数量
                    //    reConnectTime++;

                    //    // 服务断开后尝试重新连接服务
                    //    $timeout(function () { client.connect($rootScope.currentUser); }, reConnectTime * 1000);
                    //}
                }
            }

            return client;
        });

        module.factory("notifyClient", function (wsAddress, moment) {
            // ws连接对象
            var notifyWs = undefined;

            var client = {

                connect: function (user) {

                    if (!notifyWs || notifyWs.readyState != WebSocket.OPEN) {

                        var serverAddress = wsAddress + 'notify?identity=' + user.Account.ID;

                        connect(window.encodeURI(serverAddress));
                    }
                },

                disconnect: function () {
                    if (notifyWs) {
                        notifyWs.close();
                    }
                }
            };

            // 连接到聊天服务
            var connect = function (serverAddress) {

                var support = "MozWebSocket" in window ? 'MozWebSocket' : ("WebSocket" in window ? 'WebSocket' : null);

                if (support == null) {
                    alert("浏览器不支持WebSocket协议");
                    return;
                }

                notifyWs = new window[support](serverAddress);

                notifyWs.onmessage = function (evt) {

                    var message = JSON.parse(evt.data);

                    if (message.outline) {

                        angular.forEach(message.notifys, function (notify) {

                            if (PmEx) {
                                PmEx.Notify(notify.Head, notify.Title, notify.Content + '离线时间：' + notify.CreateDate.toTDate('yyyy/MM/dd hh:mm'));
                            }
                        });

                    } else {

                        if (PmEx) {
                            PmEx.Notify(message.Head, message.Title, message.Content);
                        }
                    }
                };

                notifyWs.onclose = function () {

                    ws = null;
                }
            }

            return client;
        })
    });


