define(['apps/base/base.controller',
    'apps/base/base.service.login'],
    function (module, login) {

        module.controller('base.controller.login',
            function ($rootScope, $scope, $state,$filter, $location, $window, loginService, localStorageService, localService,enumService,processService,
                chatClient, $futureState, menuService, userService, settingsService, notificationService,
                attachDownloadUrl, imagePreviewUrl, userApiUrl, userApiVersion) {

                $scope.messages = [];
                
                // 登陆
                var login = function () {

                    $scope.messages.push({ content: $state.params.account + "登录中", color: "c-blue" });

                    loginService.login($state.params.account, $state.params.password)
                       .then(function (user) {

                           if (user.Error == 0) {
                              
                               $scope.messages.push({ content: "账号验证通过", color: "c-green" });
                               
                               $scope.messages.push({ content: "令牌将在：" + new Date(user.InvalidDate).format('yyyy/MM/dd hh:mm') + " 后过期,剩余：" + $filter('strDateLeft')(user.InvalidDate), color: "c-green" });

                               $scope.messages.push({ content: "存储用户信息", color: "c-blue" });

                               var allUser = localStorageService.get("all_user");
                               if (allUser) {

                                   if (!RegExp("\\b" + user.Account.ID + "\\b").test(allUser)) {
                                       allUser.push(user.Account.ID);
                                   }

                               } else {
                                   allUser = [user.Account.ID];
                               }

                               localStorageService.set('pm_user', user);

                               $rootScope.currentUser = user;

                               loadSettings();

                           } else {

                               if (user.Error == 1) {
                                   $scope.messages.push({ content: "帐号不存在,3秒后返回登陆页面", color: "c-red" });
                               } else if (user.Error == 2) {
                                   $scope.messages.push({ content: "密码错误,3秒后返回登陆页面", color: "c-red" });
                               } else if (user.Error == 3) {
                                   $scope.messages.push({ content: "帐号已被禁用,3秒后返回登陆页面", color: "c-red" });
                               }

                               setTimeout(function () {
                                   window.location.href = "login-v2.html";
                               }, 3000);
                           }
                       });
                }

                // 加载路由
                var loadState = function () {

                    $scope.messages.push({ content: "加载路由", color: "c-blue" });

                    menuService.getModules().then(function (modules) {

                        $rootScope.modules = {};
                        $rootScope.modulesSys = {};
                        $rootScope.moduleContents = [];

                        angular.forEach(modules, function (app) {

                            $rootScope.moduleContents.push(app.Name);
                            $rootScope.modules[app.Name] = app.SubModules;
                            $rootScope.modulesSys[app.Name] = app.System;

                            $futureState.futureState({
                                "stateName": app.Name,
                                "urlPrefix": "/" + app.Name,
                                "type": "ngload",
                                "src": app.Src
                            });
                        });

                        $scope.messages.push({ content: "路由加载成功", color: "c-green" });

                        // 菜单
                        loadMenus();
                    });
                }

                // 加载用户列表
                var loadUsers = function () {

                    $scope.messages.push({ content: "加载用户列表", color: "c-blue" });

                    // 获取公司员工列表
                    $rootScope.nameCharGroup = [];
                    $rootScope.department = [];
                    $rootScope.uses = {};

                    userService.getUsersEx().then(function (data) {
                        $rootScope.user_item = data;

                        var noDept = {
                            key: "Z",
                            name: "无部门",
                            users: []
                        };
                        $rootScope.user_enum = {};
                        $rootScope.user_item = data;

                        angular.forEach($rootScope.user_item, function (user) {

                            $rootScope.user_enum[user.ID] = user.Name;

                            if (user.Dept) {
                                var dept = $rootScope.department.find(function (d) { return d.key == user.Dept.Key; });

                                if (dept == undefined) {
                                    $rootScope.department.push({
                                        id: user.Dept.ID,
                                        key: user.Dept.Key,
                                        name: user.Dept.Name,
                                        users: [user]
                                    });
                                } else {
                                    dept.users.push(user);
                                }
                            } else {
                                noDept.users.push(user);
                            }

                            //var c = user.Name.substr(0, 1);

                            //if ($scope.nameCharGroup.indexOf(c) == -1) {
                            //    $scope.nameCharGroup.push(c);

                            //    $scope.uses[c] = [user];
                            //} else {
                            //    $scope.uses[c].push(user);
                            //}

                        });

                        if (noDept.users.length > 0) {
                            $rootScope.department.push(noDept);
                        }

                        $rootScope.enum_users_map = {};
                        $rootScope.enum_depts_map = {};

                        $rootScope.enum_users = data.map(function (u) {
                            $rootScope.enum_users_map[u.ID] = u.Name;
                            return { Value: u.ID, Text: u.Name }
                        });
                        $rootScope.enum_depts = $rootScope.department.map(function (d) {

                            $rootScope.enum_depts_map[d.id] = d.name;
                           
                            return {
                                Value: d.id,
                                Text: d.name
                            }
                        });

                        $scope.messages.push({ content: "用户列表已加载", color: "c-green" });

                        loadEnum();
                    });

                    //userService.getUsers().then(function (data) {


                    //    $rootScope.user_enum = {};
                    //    $rootScope.user_item = data;

                    //    angular.forEach(data, function (user) {

                    //        $rootScope.user_enum[user.ID] = user.Name;
                    //    });

                    //});
                }

                // 加载系统设置
                var loadSettings = function () {

                    $scope.messages.push({ content: "加载系统设置", color: "c-blue" });

                    settingsService.getSettings().then(function (settings) {

                        $scope.messages.push({ content: "系统设置已获取", color: "c-green" });

                        $rootScope.sysSettings = settings;

                        loadNotifys();
                    })
                }

                // 加载菜单
                var loadMenus = function () {

                    $scope.messages.push({ content: "加载菜单", color: "c-blue" });

                    // 获取菜单
                    $rootScope.SubMenus = {};

                    menuService.getAllMenus().then(function (result) {
                        if (result.length == 0) {

                            $scope.messages.push({ content: "此账号没有任何业务系统的访问权限,3秒后返回登陆页面", color: "c-red" });

                            setTimeout(function () {
                                window.location.href = "login-v2.html";
                            }, 3000);

                            return;
                        }

                        angular.forEach(result, function (bus) {

                            // 每个业务系统的标签栏
                            bus.tabs = [];

                            angular.forEach(bus.Menus, function (menu) {

                                $rootScope.SubMenus[bus.Key + "_" + menu.Name] = menu.SubMenus;
                            });
                        });

                        $rootScope.Businesss = result;

                      

                        $scope.messages.push({ content: "菜单加载成功", color: "c-green" });

                        loadUserConfig();
                    });
                }

                // 加载系统通知
                var loadNotifys = function () {
                    $scope.messages.push({ content: "加载系统通知", color: "c-blue" });

                    notificationService.getEffects(10, 2).then(function (data) {
                        $rootScope.notifications = data;
                        $scope.messages.push({ content: "已获取系统通知", color: "c-green" });


                        loadUsers();

                    });
                }

                // 加载用户设置
                var loadUserConfig = function () {

                    $scope.messages.push({ content: "加载用户设置", color: "c-blue" });

                    $rootScope.userConfig = {};

                    userService.getUserConfig().then(function (result) {
                       
                        // 用户的快速访问
                        $rootScope.userConfig.quicklink = result.where(function (item) {
                           return item.ConfigName == 'QuickLink'
                        });

                        // 用户的工程关注
                        $rootScope.userConfig.engFollows = result.where(function (item) {
                            return item.ConfigName == 'EngineeringFollow'
                        });

                        $scope.messages.push({ content: "用户配置加载成功", color: "c-green" });

                        $scope.messages.push({ content: "即将导航到桌面", color: "c-green" });

                        goFirstState("book.maintain");
                    })
                }

                // 加载基础数据
                
                var baseItem = {};
                var baseSource = {};

                var loadEnum = function () {

                    $scope.messages.push({ content: "加载基础数据", color: "c-blue" });

                    $rootScope.baseEnum = {};

                    enumService.all().then(function (result) {

                        angular.forEach(result, function (sys) {

                            $rootScope.baseEnum[sys.Key] = {
                                "Dept": $rootScope.enum_depts_map,
                                "User": $rootScope.enum_users_map
                            };
                            baseItem[sys.Key] = {
                                "Dept": $rootScope.enum_depts,
                                "User": $rootScope.enum_users
                            };
                            baseSource[sys.Key] = [{ "Key": "Dept", "Name": " 部门" }, { "Key": "User", "Name": "用户" }];

                            angular.forEach(sys.Enums, function (data) {

                                var itemHash = {};
                                if (data.Name == "Object") {
                                    angular.forEach(data.Items, function (item) {
                                        itemHash[item.Tags["name"]] = item.Text;
                                    });
                                } else {
                                    angular.forEach(data.Items, function (item) {
                                        
                                        itemHash[item.Value] = item.Text;
                                       
                                        item.Value = parseInt(item.Value);
                                        item.Key = parseInt(item.Key);
                                    });
                                }

                                $rootScope.baseEnum[sys.Key][data.Name] = itemHash;
                                baseItem[sys.Key][data.Name] = data.Items;
                                baseSource[sys.Key].push({
                                    Key: data.Name,
                                    Name: data.Text
                                });
                            });
                        });

                        $scope.messages.push({ content: "基础数据加载完成", color: "c-green" });

                        loadState();
                    });

                    $scope.messages.push({ content: "加载流程", color: "c-blue" });
                    processService.getModels().then(function (result) {

                        var itemHash = {};
                        var items = [];

                        angular.forEach(result, function (item) {
                            itemHash[item.Key] = { Key: item.Key, Value: item.Value, Tasks: item.Tasks };
                            items.push({ Key: item.Key, Value: item.Value, Tasks: item.Tasks });
                        });

                        $rootScope['ProcessModel_enum'] = itemHash;
                        $rootScope['ProcessModel_item'] = items;

                        $scope.messages.push({ content: "流程加载完成", color: "c-green" });
                    });

                    processService.getFlowUsers().then(function (result) {
                        $rootScope['SpecProcessUsers'] = result;
                    });
                }

                var setParams = function () {

                    $rootScope.lang = 'zh-cn';
                    $rootScope.pageSize = 20;
                    $rootScope.pageSizes = [20, 50, 100];

                    $rootScope.attachDownloadUrl = attachDownloadUrl;
                    $rootScope.imagePreviewUrl = imagePreviewUrl;
                    $rootScope.backupUrl = userApiUrl + userApiVersion;

                    $rootScope.getBaseData = function (name) {
                        return baseItem[$rootScope.currentBusiness.Key][name];
                    }

                    $rootScope.getBaseEnum = function (name) {
                        return $rootScope.baseEnum[$rootScope.currentBusiness.Key][name];
                    }

                    $rootScope.getBaseSource = function () {
                        return baseSource[$rootScope.currentBusiness.Key];
                    }

                    var headerHeight = 50;
                    var height = $window.innerHeight;
                    
                    $rootScope.grid_lang = 'zh-cn';

                    $rootScope.panelHeight = height - headerHeight - 150;

                    // 列表表格高度
                    $rootScope.gridHeight = height - headerHeight - 200;

                    // 编辑弹出窗口高度
                    $rootScope.modalHeight = (height - headerHeight)  / 2;
                    
                    // 筛选panel宽度
                    $rootScope.filterPanelWidth = 400;

                    bootbox.setDefaults({
                        locale: "zh_CN",
                    });
                }

                var start = function () {
                    // 如果有账号密码，则登陆
                    if ($state.params.account && $state.params.password) {

                        login();

                    } else {

                        $scope.messages.push({ content: "尝试从本地缓存中获取用户数据", color: "c-blue" });

                        var pm_user = localStorageService.get("pm_user");

                        if (pm_user) {

                            $scope.messages.push({ content: "获取用户：" + pm_user.Account.Name, color: "c-green" });
                            
                            $scope.messages.push({ content: "令牌将在：" + new Date(pm_user.InvalidDate).format('yyyy/MM/dd hh:mm') + " 后过期,剩余：" + $filter('strDateLeft')(pm_user.InvalidDate), color: "c-green" });
                            
                            if (new Date(pm_user.InvalidDate) <= new Date()) {

                                // 用户令牌失效，重新登陆
                                $scope.messages.push({ content: "令牌过期，用户重新登陆", color: "c-red" });

                                setTimeout(function () {
                                    window.location.href = "login-v2.html";
                                }, 3000);

                                return;
                            }

                            $rootScope.currentUser = pm_user;
                            
                            // 开始加载系统数据
                            loadSettings();

                        } else {
                            $scope.messages.push({ content: "无法取得用户信息，返回登陆页面", color: "c-red" });

                            setTimeout(function () {
                                window.location.href = "login-v2.html";
                            }, 3000);
                        }
                    }
                }

                // 根据当前路由决定当前是哪个业务系统
                var goFirstState = function (goState) {
                  
                    var system = $rootScope.Businesss.find(function (m) { return m.Key == $rootScope.modulesSys[goState.split('.')[0]] });

                    if (system) {
                        $rootScope.currentBusiness = system ? system : $rootScope.Businesss[0];
                        $state.go(goState);
                    } else {
                        $scope.messages.push({ content: "没有此路由的所属系统访问权限，无法导航到预期地址", color: "c-red" });
                    }
                }

                setParams();

                start();
            });
    });
