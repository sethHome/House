define(['apps/base/base.service'],
    function (module) {

        module.factory("localService", function ($rootScope) {
           
            var local =  {
                global: {
                        index: '序号',
                        add: '新增',
                        insert: '添加',
                        edit: '编辑',
                        remove: '删除',
                        list: '列表',
                        close: '关闭',
                        plan: '策划',
                        preview: '预览',
                        'export': '导出',
                        query: '查询',
                        filter: '筛选',
                        choose: '选择',
                        input: '输入',
                        reset: '重置',
                        save: '保存',
                        info: '信息',
                        savepreview: '保存并预览',
                        operate: '操作',
                        beign: '开始',
                        end: '结束',
                        to: '到',
                        addTag: '添加标签',
                        download: '下载',
                        selectall: '全选',
                        unselect: '反选',
                        clickup: '点击上传',
                        choosetag: '选择标签',
                        multiChoose: '多选',
                        moneyicon: '￥',
                        unit: '万元'
                },
                msg: {
                    confirmBatchDelete: '确定批量删除？',
                    confirmDelete: '确定删除选中对象？',
                    confirmBackup: '确定恢复选中对象？',
                    noRowSelected: '未选中任何对象',
                    success: '成功',
                    failure: '失败'
                },
                business: {
                    project: '项目',
                    engineering: '工程',
                    attach: '附件',
                    con: '合同',
                    customer: '客户',
                    person: '联系人',
                    volume: '卷册',
                    bid: '招投标',
                    note: '工程记事',
                    provide: '专业提资'
                },
                bid: {
                    name: '项目名称',
                    manager: '招标人',
                    agency: '代理机构',
                    bidStatus: '投标状态',
                    bidFee: '标价',
                    deposit: '担保金',
                    depositStatus: '担保金状态',
                    servicePercent: '服务费百分比',
                    serviceFee: '代理服务费',
                    returnFee: '应退',
                    limitFee: '限价',
                    isTentative: '含暂定金',
                    bidDate: '投标日期',
                    successfulBidDate: '中标日期',
                    note: '备注'
                },
                proj: {
                    name: '项目名称',
                    number: '项目编号',
                    kind: '项目类型',
                    type: '项目分类',
                    vollevel: '电压等级',
                    manager: '项目经理',
                    secretlevel: '秘级',
                    customer: '客户名称',
                    createdate: '立项日期',
                    deliverydate: '完成日期',
                    note: '备注',
                    choosecustomer: '输入或者选择客户'
                },
                eng: {
                    name: '工程名称',
                    number: '工程编号',
                    manager: '工程经理',
                    type: '工程类型',
                    phase: '工程阶段',
                    createdate: '创建日期',
                    deliverydate: '计划完成',
                    startdate: '启动日期',
                    finishdate: '实际完成',
                    status: '工程状态',
                    note: '工程备注',
                    tasktype: '任务类型',
                    vollevel: '电压等级',
                    proj: '所属项目'
                },
                con: {
                    name: '合同名称',
                    number: '合同编号',
                    status: '合同状态',
                    type: '合同类型',
                    signdate: '签订日期',
                    createdate: '创建日期',
                    fee: '合同金额',
                    customer: '客户名称',
                    note: '备注',
                    engineering: '包含工程',
                    },
                conPay: {
                    reDate: '计划日期',
                    acDate: '收费日期',
                    blDate: '开票日期',
                    fee: '金额',
                    note: '备注',
                    invoicetype: '开票类型',
                    re: '计划收费',
                    ac: '实际收费',
                    bl: '实际开票'
                },
                cust: {
                    name: '客户名称',
                    address: '客户地址',
                    tel: '联系电话',
                    rating: '客户评价',
                    },
                person: {
                    name: '姓名',
                    phone: '联系电话',
                    email: '邮箱'
                },
                specil: {
                    name: '专业',
                    manager: '负责人',
                    startDate: '开始日期',
                    endDate: '结束日期',
                    note: '备注'
                },
                user: {
                    manage: '负责人',
                    check: '校核人',
                    design: '主设人',
                    approve: '审核人',
                    agree: '批准人'
                },
                volume: {
                    name: '卷册名称',
                    number: '卷册编号'
                },
                check: {
                    context: '校审意见',
                    type: '错误类型',
                    user: '提出人',
                    date: '提出时间',
                    isCorrect: '已修改',
                    pass: '已通过'
                },
                note: {
                    user: '记录人',
                    content: '记事内容',
                    date: '记录时间',
                    type: '记事类型'
                },
                resource: {
                    user: '登记人',
                    name: '资料名称',
                    content: '资料内容',
                    date: '登记时间',
                    },
                provide: {
                    sendSpecialty: '提资专业',
                    receiveSpecialty: '收资专业',
                    sendUser: '提资人',
                    docName: '资料名称',
                    docContent: '资料内容',
                    limitDate: '收资期限',
                    createDate: '提资时间',
                    receiveUsers: '收资人'
                }
            }

            $rootScope.local = local;

            return local;
        });
    });
