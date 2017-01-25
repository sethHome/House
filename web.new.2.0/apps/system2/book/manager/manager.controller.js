define(['apps/system2/book/book',
    'apps/system2/book/book.service'], function (app) {

    app.module.controller("book.controller.manager", function ($scope, $uibModal, $filter, bookService) {

        $scope.bookFilter = {
            pagesize: $scope.pageSize,
            pageindex: 1,
            orderby: 'ID',
            status:1
        };

        $scope.books = [
            { type: 1, name: '莫斯科经历125年来最冷一夜', author:'唐亮', price: 15.62,barcode:'123456', count:6,number: 'B0213-12321', publish: '春风出版社', date: '2016-5', items: [1, 2, 3, 4, 5, 6] },
            { type: 2, name: '盘龙', author: '唐亮', price: 15.62, barcode: '123456', count: 6, number: 'B0213-12321', publish: '春风出版社', date: '2016-5', items: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11] },
            { type: 3, name: '书名3', author: '唐亮', price: 15.62, barcode: '123456', count: 6, number: 'B0213-12321', publish: '春风出版社', date: '2016-5', items: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17] },
            { type: 1, name: '书名4', author: '唐亮', price: 15.62, barcode: '123456', count: 6, number: 'B0213-12321', publish: '春风出版社', date: '2016-5', items: [1, 2, 3, 4, 5, 6] },
            { type: 3, name: '书名3', author: '唐亮', price: 15.62, barcode: '123456', count: 6, number: 'B0213-12321', publish: '春风出版社', date: '2016-5', items: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17] },
            { type: 3, name: '书名3', author: '唐亮', price: 15.62, barcode: '123456', count: 6, number: 'B0213-12321', publish: '春风出版社', date: '2016-5', items: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17] },
            { type: 2, name: '书名3', author: '唐亮', price: 15.62, barcode: '123456', count: 6, number: 'B0213-12321', publish: '春风出版社', date: '2016-5', items: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17] },
            { type: 3, name: '书名3', author: '唐亮', price: 15.62, barcode: '123456', count: 6, number: 'B0213-12321', publish: '春风出版社', date: '2016-5', items: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17] },
            { type: 3, name: '书名3', author: '唐亮', price: 15.62, barcode: '123456', count: 6, number: 'B0213-12321', publish: '春风出版社', date: '2016-5', items: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17] },
            { type: 1, name: '书名4', author: '唐亮', price: 15.62, barcode: '123456', count: 6, number: 'B0213-12321', publish: '春风出版社', date: '2016-5', items: [1, 2, 3, 4, 5, 6] },
            { type: 3, name: '书名3', author: '唐亮', price: 15.62, barcode: '123456', count: 6, number: 'B0213-12321', publish: '春风出版社', date: '2016-5', items: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17] },
            { type: 3, name: '书名3', author: '唐亮', price: 15.62, barcode: '123456', count: 6, number: 'B0213-12321', publish: '春风出版社', date: '2016-5', items: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17] },

            { type: 3, name: '书名3', author: '唐亮', price: 15.62, barcode: '123456', count: 6, number: 'B0213-12321', publish: '春风出版社', date: '2016-5', items: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17] },
            { type: 2, name: '书名4', author: '唐亮', price: 15.62, barcode: '123456', count: 6, number: 'B0213-12321', publish: '春风出版社', date: '2016-5', items: [1, 2, 3, 4, 5, 6] },
            { type: 2, name: '书名3', author: '唐亮', price: 15.62, barcode: '123456', count: 6, number: 'B0213-12321', publish: '春风出版社', date: '2016-5', items: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17] },
            { type: 3, name: '书名3', author: '唐亮', price: 15.62, barcode: '123456', count: 6, number: 'B0213-12321', publish: '春风出版社', date: '2016-5', items: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17] },

            { type: 3, name: '书名3', author: '唐亮', price: 15.62, barcode: '123456', count: 6, number: 'B0213-12321', publish: '春风出版社', date: '2016-5', items: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17] },
            { type: 1, name: '书名4', author: '唐亮', price: 15.62, barcode: '123456', count: 6, number: 'B0213-12321', publish: '春风出版社', date: '2016-5', items: [1, 2, 3, 4, 5, 6] },
            { type: 3, name: '书名3', author: '唐亮', price: 15.62, barcode: '123456', count: 6, number: 'B0213-12321', publish: '春风出版社', date: '2016-5', items: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17] },
            { type: 1, name: '书名3', author: '唐亮', price: 15.62, barcode: '123456', count: 6, number: 'B0213-12321', publish: '春风出版社', date: '2016-5', items: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17] },

        ];

        $scope.bookCategorys = [{
            text: '全分类',
            type: 'folder',
            state: { opened: true }
        }];

        $scope.bookCategoryChanges = function (e, data) {
            $scope.$safeApply(function () {
                var parent = "";
                if (data.node.original.Number) {

                    if (data.node.original.ParentFullKey) {
                        parent = data.node.original.ParentFullKey + "." + data.node.original.Number;
                    } else {
                        parent = data.node.original.Number;
                    }
                }

                $scope.bookFilter.category = parent;
            })
        }

        var loadBookCategorys = function () {

            var loadTreeNode = function (categorys, parents) {

                angular.forEach(categorys, function (a) {

                    a.text = "<span class='c-blue m-r-5'>[" + a.Number + "]</span>" + a.Name;
                    a.type = 'folder';
                    a.state = { opened: true };

                    a.ParentFullKey = parents.join('.');

                    if (a.Children && a.Children.length > 0) {

                        var ps = parents.concat();
                        ps.push(a.Number);

                        loadTreeNode(a.Children, ps);
                    }

                    a.children = a.Children;
                });
            }

            bookService.getBookCategorys().then(function (datas) {

                loadTreeNode(datas, []);

                $scope.bookCategorys[0].children = datas;
            });
        }

        var loadBooks = function () {
            bookService.getBooks($scope.bookFilter).then(function (result) {
                $scope.books = result.Source;
            });
        }

        $scope.setFilter = function (txt) {
            $scope.bookFilter.txtfilter = txt;
        }

        $scope.$watch("bookFilter", function (newval,oldval) {
            if (newval) {

                loadBooks();
            }
        },true);

        loadBookCategorys();
        
    });

});
