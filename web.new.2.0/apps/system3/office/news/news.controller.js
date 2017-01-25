define([
'apps/system3/office/office',
'apps/system3/office/news/news.service'], function (app) {

    app.module.controller("office.controller.news", function ($scope,$sce, $state, $stateParams, $uibModal, $timeout, newsService) {

        $scope.$sce = $sce;

        $scope.currentStateName = $state.current.name;

        $scope.$watch("currentNews", function (newval, oldval) {
            if (newval && $scope.viewScroll) {
                $scope.viewScroll.init();
            }
        });
        
        $scope.setCurrentNews = function (news) {

            $scope.currentNews = news;
        }

        $scope.addNews = function (n) {
            $scope.news.TotalCount++;
            $scope.news.Source.push(n);
        }

        $scope.removeNews = function (n) {
            $scope.news.TotalCount--;
            $scope.news.Source.removeObj(n);
        }

        newsService.getNewsList().then(function (result) {
            $scope.news = result;

            if ($stateParams.id > 0) {
                angular.forEach(result.Source, function (n) {
                    if (n.ID == $stateParams.id) {
                        $scope.setCurrentNews(n);
                    }
                });
            }
        });
    });
});
