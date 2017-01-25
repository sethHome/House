define([
    'apps/system3/office/office',
    'apps/system3/office/news/news.service'], function (app) {

        app.module.controller("office.controller.news.maintain", function ($scope, $sce,$stateParams, $uibModal, $timeout, newsService) {
            
            $scope.setCurrentNews = function (news) {

                $scope.currentNews = news;
            }
          
            $scope.new = function () {
                $scope.currentNews = {};
            }

            $scope.save = function () {
                $scope.maintainPanel.block();
                if ($scope.currentNews.ID > 0) {
                    newsService.update($scope.currentNews).then(function (id) {

                        $scope.currentNews.HtmlContent = $sce.trustAsHtml($scope.currentNews.Content);
                        $scope.maintainPanel.unblock();
                    });
                } else {
                    newsService.create($scope.currentNews).then(function (id) {
                        $scope.currentNews.ID = id;
                        $scope.addNews($scope.currentNews);
                        $scope.maintainPanel.unblock();
                    });
                }
            }

            $scope.delete = function () {
                $scope.maintainPanel.block();
                if ($scope.currentNews.ID > 0) {
                    newsService.remove($scope.currentNews.ID).then(function (id) {
                        
                        $scope.removeNews($scope.currentNews);
                        $scope.currentNews = undefined;
                        $scope.maintainPanel.unblock();
                    });
                } 
            }

        });
    });
