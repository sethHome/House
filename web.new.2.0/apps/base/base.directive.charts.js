define(['apps/base/base.directive', 'apps/base/base.service.statistics'],
    function (app) {

        var yearNow = new Date().getFullYear();
        var months = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];

        app.directive('barEngCount', function ($rootScope, statisticsService) {
            return {
                restrict: 'C',

                link: function (scope, elem, attr) {
                    var EngineeringTypes = $rootScope.getBaseData("EngineeringType");
                    var EngineeringPhases = $rootScope.getBaseData("EngineeringPhase");

                    var categorys = {
                        Type: EngineeringTypes.map(function (v) { return v.Text }),
                        Phase: EngineeringPhases.map(function (v) { return v.Text }),
                        month: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月']
                    };

                    var initChart = function (source) {
                        // 基于准备好的dom，初始化echarts实例
                        var myChart = echarts.init(elem[0]);

                        // 指定图表的配置项和数据
                        var option = {
                            title: {
                                text: attr["title"] ? attr["title"] : ""
                            },
                            tooltip: {},
                            legend: {
                                data: ['数量']
                            },
                            xAxis: {
                                data: categorys[attr.category] ? categorys[attr.category] : categorys.Type
                            },
                            yAxis: {},
                            series: [{
                                name: '数量',
                                type: 'bar',
                                data: source
                            }]
                        };

                        // 使用刚指定的配置项和数据显示图表。
                        myChart.setOption(option);
                    }

                    if (attr.category == "month") {

                        statisticsService.getMonthEngineeringCount(attr.year ? attr.year : new Date().getFullYear()).then(function (list) {

                            var source = months.map(function (m) {
                                var value = 0;

                                for (var i = 0; i < list.length; i++) {
                                    if (list[i].Key == m) {
                                        value = list[i].Count;
                                        break;
                                    }
                                }

                                return value;
                            });

                            initChart(source);
                        });

                    } else if (attr.category == "year") {

                        statisticsService.getYearEngineeringCount().then(function (list) {

                            categorys.year = list.map(function (m) {
                                return m.Key;
                            });

                            var source = list.map(function (m) {
                                return m.Count;
                            });

                            initChart(source);
                        });
                    } else {

                        statisticsService.getEngineeringCount(attr.category).then(function (list) {
                            var source = $rootScope.getBaseData("Engineering" + attr.category);

                            var data = source.map(function (v) {

                                var value = 0;

                                for (var i = 0; i < list.length; i++) {
                                    if (list[i].Key + "" == v.Key) {
                                        value = list[i].Count;
                                        break;
                                    }
                                }

                                return value;

                            });

                            initChart(data);

                        });
                    }
                }
            }
        });

        app.directive('lineEngCount', function ($rootScope, statisticsService) {
            return {
                restrict: 'C',

                link: function (scope, elem, attr) {
                    if (attr.year) {

                        statisticsService.getMonthEngineeringCount(attr.year).then(function (list) {

                            var months = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];

                            var source = months.map(function (m) {
                                var value = 0;

                                for (var i = 0; i < list.length; i++) {
                                    if (list[i].Key == m) {
                                        value = list[i].Count;
                                        break;
                                    }
                                }

                                return value;
                            });

                            initChat(source, "month");
                        });

                    } else {


                        statisticsService.getYearEngineeringCount().then(function (list) {



                        });
                    }



                    var initChat = function (source, category) {
                        var x = {
                            year: [2014, 2015, 2016],
                            month: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月']
                        };

                        // 基于准备好的dom，初始化echarts实例
                        var myChart = echarts.init(elem[0]);

                        // 指定图表的配置项和数据
                        var option = {
                            title: {
                                text: attr["title"] ? attr["title"] : ""
                            },
                            tooltip: {},
                            legend: {
                                data: ['数量']
                            },
                            xAxis: {
                                data: x[category]
                            },
                            yAxis: {},
                            series: [{
                                name: '数量',
                                type: 'line',
                                data: source
                            }]
                        };

                        // 使用刚指定的配置项和数据显示图表。
                        myChart.setOption(option);
                    }
                }
            }
        });

        app.directive('pieEngCount', function ($rootScope, statisticsService) {
            return {
                restrict: 'C',

                link: function (scope, elem, attr) {

                    var initChart = function (source) {
                        // 基于准备好的dom，初始化echarts实例
                        var myChart = echarts.init(elem[0]);

                        // 指定图表的配置项和数据
                        var option = {
                            title: {
                                text: attr["title"] ? attr["title"] : ""
                            },
                            tooltip: {},

                            series: [
                               {
                                   name: '数量',
                                   type: 'pie',
                                   radius: '55%',
                                   data: source
                               }
                            ]
                        };


                        // 使用刚指定的配置项和数据显示图表。
                        myChart.setOption(option);
                    }

                    statisticsService.getEngineeringCount(attr.category).then(function (list) {
                        var source = $rootScope.getBaseData("Engineering" + attr.category);
                        var data = source.map(function (v) {

                            var value = 0;
                            var percent = 0;
                            for (var i = 0; i < list.length; i++) {
                                if (list[i].Key + "" == v.Key) {
                                    value = list[i].Count;
                                    percent = (list[i].Count / list[i].TotalCount * 100).toFixed(2)
                                    break;
                                }
                            }

                            return { name: v.Text + "（" + percent + "%）", value: value }

                        });

                        initChart(data);

                    });
                }
            }
        });


        app.directive('chartConCount', function ($rootScope, statisticsService) {
            return {
                restrict: 'A',

                link: function (scope, elem, attr) {

                    var ContractTypes = $rootScope.getBaseData("ContractType");

                    var categorys = {
                        Type: ContractTypes.map(function (v) { return v.Text }),
                        month: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月']
                    };

                    var initChart = function (source) {
                        // 基于准备好的dom，初始化echarts实例
                        var myChart = echarts.init(elem[0]);

                        // 指定图表的配置项和数据
                        var option = {
                            title: {
                                text: attr["title"] ? attr["title"] : ""
                            },
                            tooltip: {},
                            legend: {
                                data: ['数量']
                            },
                            xAxis: {
                                data: categorys[attr.category] ? categorys[attr.category] : categorys.Type
                            },
                            yAxis: {},
                            series: [{
                                name: '数量',
                                type: attr.chartConCount,
                                data: source
                            }]
                        };

                        // 使用刚指定的配置项和数据显示图表。
                        myChart.setOption(option);
                    }

                    if (attr.category == "month") {
                        statisticsService.getMonthContractCount(attr.year ? attr.year : yearNow).then(function (list) {
                            var source = months.map(function (m) {
                                var value = 0;

                                for (var i = 0; i < list.length; i++) {
                                    if (list[i].Key == m) {
                                        value = list[i].Count;
                                        break;
                                    }
                                }

                                return value;
                            });

                            initChart(source);
                        });
                    } else {
                        statisticsService.getYearContractCount().then(function (list) {
                            categorys.year = list.map(function (m) {
                                return m.Key;
                            });

                            var source = list.map(function (m) {
                                return m.Count;
                            });

                            initChart(source);
                        });
                    }
                    
                }
            }
        });

        app.directive('chartConFee', function ($rootScope, statisticsService) {
            return {
                restrict: 'A',

                link: function (scope, elem, attr) {
                    var ContractTypes = $rootScope.getBaseData("ContractType");
                    var categorys = {
                        Type: ContractTypes.map(function (v) { return v.Text }),
                        month: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月']
                    };

                    var initChart = function (source) {
                        // 基于准备好的dom，初始化echarts实例
                        var myChart = echarts.init(elem[0]);

                        // 指定图表的配置项和数据
                        var option = {
                            title: {
                                text: attr["title"] ? attr["title"] : ""
                            },
                            tooltip: {},
                            legend: {
                                data: ['金额']
                            },
                            xAxis: {
                                data: categorys[attr.category] ? categorys[attr.category] : categorys.Type
                            },
                            yAxis: {},
                            series: [{
                                name: '金额',
                                type: attr.chartConFee,
                                data: source
                            }]
                        };

                        // 使用刚指定的配置项和数据显示图表。
                        myChart.setOption(option);
                    }

                    if (attr.category == "month") {
                        statisticsService.getMonthContractMoney(attr.feeType, attr.year ? attr.year : yearNow).then(function (list) {
                            var source = months.map(function (m) {
                                var value = 0;

                                for (var i = 0; i < list.length; i++) {
                                    if (list[i].Key == m) {
                                        value = list[i].Money;
                                        break;
                                    }
                                }

                                return value;
                            });

                            initChart(source);
                        });
                    } else {
                        statisticsService.getYearContractMoney(attr.feeType).then(function (list) {
                            categorys.year = list.map(function (m) {
                                return m.Key;
                            });

                            var source = list.map(function (m) {
                                return m.Money;
                            });

                            initChart(source);
                        });
                    }

                }
            }
        });
    });
