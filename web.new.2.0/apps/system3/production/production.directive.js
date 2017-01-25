define(['apps/system3/production/production'], function (app) {

    app.module.factory("engineeringFlowService", function ($rootScope, Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            getEngTreeTasks: function (id) {
                return restSrv.all("object/engineering/tree").getList({engid : id, task : 1 });
            },
        }
    });

    app.module.directive("engineeringFlowColor", function () {
        return {
            restrict: 'AC',
            template: ' <div class="row m-0 p-10"><div class="color bd-full" style="background:{{item.color}}" ng-repeat="item in items">{{item.name}}</div></div> ',
            scope: true,
            controller: function ($scope) {
                $scope.items = [
                    {
                        name: '待办',
                        color: '#c4c9cc'
                    },
                    {
                        name: '已办',
                        color: '#229ae2'
                    },
                    {
                        name: '超期',
                        color: '#ff0018'
                    }
                ];
            },
            link: function ($scope, element, $attrs) {
               
            }
        };
    });
    app.module.directive("engineeringFlow", function () {
        return {
            restrict: 'A',
            scope: {
                'engid' : '=?',
                'flowusers': '='
            },

            controller: function ($scope, engineeringFlowService, $filter) {

                $scope.$watch("engid", function (oldval,newval) {
                    if (oldval > 0) {

                        engineeringFlowService.getEngTreeTasks(oldval).then(function (result) {

                            if (!$scope.flowusers) {
                                $scope.flowusers = [];
                            }

                            $scope.flowusers.push({
                                id: result[0].Manager,
                                tags: ['工程负责人']
                            });
                            

                            $scope.source = [
                                {
                                    key: result[0].ObjectID,
                                    fill: result[0].IsTimeOut ? "#ff0018" : result[0].Status == 4 ? "#229ae2" : "#e3f1a0",
                                    size: new go.Size(120, 120),
                                    text: result[0].ObjectText + "\r\r" + $filter("enumMap")(result[0].Manager, 'user')
                                }
                            ];

                            var parentKey = result[0].ObjectID;

                            angular.forEach(result[0].Children, function (item) {
                                if (item.ObjectKey == "Specialty") {

                                    var user = $scope.flowusers.find(function (u) { return u.id == item.Manager; });
                                    if (user == null) {
                                        $scope.flowusers.push({
                                            id: item.Manager,
                                            tags: [item.ObjectText + '负责人']
                                        });
                                    } else {
                                        user.tags.push(item.ObjectText + '负责人');
                                    }
                                    

                                    $scope.source.push({
                                        key: item.ObjectID,
                                        parent: parentKey,
                                        fill: item.IsTimeOut ? "#ff0018" : item.IsDone ? "#229ae2" : "#e3f1a0",
                                        size: new go.Size(80, 80),
                                        text: item.ObjectText + "\r\r" + $filter("enumMap")(item.Manager, 'user')
                                    });

                                    angular.forEach(item.Children, function (item1) {
                                        if (item1.ObjectKey == "Volume") {


                                            $scope.source.push({
                                                key: item1.ObjectID,
                                                parent: item.ObjectID,
                                                fill: item.IsTimeOut ? "#ff0018" : item1.IsDone ? "#229ae2" : "#e3f1a0",
                                                size: new go.Size(60, 60),
                                                text: item1.ObjectText 
                                            });

                                            var preID = item1.ObjectID;

                                            angular.forEach(item1.Tasks, function (item2) {

                                                var user = $scope.flowusers.find(function (u) { return u.id == item2.UserID; });
                                                if (user == null) {
                                                    $scope.flowusers.push({
                                                        id: item2.UserID,
                                                        tags: [item2.Name]
                                                    });
                                                } else if (user.tags.count(function (t) { return t == item2.Name; }) == 0) {

                                                    user.tags.push(item2.Name);
                                                }

                                                $scope.source.push({
                                                    key: item2.ID,
                                                    parent: preID,
                                                    fill: getTaskColor(item2),
                                                    size: new go.Size(60, 60),
                                                    text: item2.Name + "\r\r" + $filter("enumMap")(item2.UserID, 'user')
                                                });

                                                preID = item2.ID;
                                            })
                                        }
                                    })
                                }
                            })

                            $scope.init();
                        });

                    }
                })
                
                var getTaskColor = function (task) {
                    if (task.Status == 0) {
                        return "#c4c9cc";
                    } else if (task.Status == 1) {
                        return "#229ae2";
                    } else if (task.Status == 2) {
                        return "#229ae2";
                    }
                }
            },

            link: function ($scope, element, $attrs) {

                $scope.init =  function() {
                    //if (window.goSamples) goSamples();  // init for these samples -- you don't need to call this
                    var $ = go.GraphObject.make;  // for conciseness in defining templates

                    myDiagram =
                      $(go.Diagram, element[0],  // must be the ID or reference to div
                        {
                            initialAutoScale: go.Diagram.UniformToFill,
                            layout: $(go.TreeLayout,
                                      { comparer: go.LayoutVertex.smartComparer }) // have the comparer sort by numbers as well as letters
                            // other properties are set by the layout function, defined below
                        });

                    // define the Node template
                    myDiagram.nodeTemplate =
                      $(go.Node, "Spot",
                        { locationSpot: go.Spot.Center },
                        new go.Binding("text", "text"),  // for sorting
                        $(go.Shape, "Ellipse",
                          {
                              fill: "lightgray",  // the initial value, but data-binding may provide different value
                              stroke: null,
                              desiredSize: new go.Size(30, 30)
                          },
                          new go.Binding("desiredSize", "size"),
                          new go.Binding("fill", "fill")),
                        $(go.TextBlock,
                          new go.Binding("text", "text"))
                      );

                    // define the Link template
                    myDiagram.linkTemplate =
                      $(go.Link,
                        {
                            routing: go.Link.Orthogonal,
                            selectable: false
                        },
                        $(go.Shape,
                          { strokeWidth: 1, stroke: "#333" }));

                    // create and assign a new model
                    myDiagram.model = new go.TreeModel($scope.source);

                    // update the diagram layout customized by the various control values
                    layout();
                }

                var layout = function() {
                    myDiagram.startTransaction("change Layout");
                    var lay = myDiagram.layout;
                    lay.treeStyle = go.TreeLayout.StyleLayered;
                    lay.layerStyle = go.TreeLayout.LayerIndividual;
                    lay.angle = parseFloat(0, 10);;
                    lay.alignment = go.TreeLayout.AlignmentCenterChildren;
                    lay.nodeSpacing = parseFloat(20, 10);
                    lay.nodeIndent = parseFloat(0, 10);
                    lay.nodeIndentPastParent = parseFloat(0, 10);
                    lay.layerSpacing = parseFloat(50, 10);
                    lay.layerSpacingParentOverlap = parseFloat(0, 10);
                    lay.sorting = go.TreeLayout.SortingForwards;
                    lay.compaction = go.TreeLayout.CompactionBlock;
                    lay.breadthLimit = parseFloat(0, 10);
                    lay.rowSpacing = parseFloat(25, 10);
                    lay.rowIndent = parseFloat(10, 10);
                    lay.setsPortSpot = true;
                    lay.setsChildPortSpot = true;

                    myDiagram.commitTransaction("change Layout");
                }
            }
        };
    });
});
