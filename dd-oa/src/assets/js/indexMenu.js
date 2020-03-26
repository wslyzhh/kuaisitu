
let list = [
    {
        title:'业务管理',
        list:[
            // {
            //     imgUrl:'',
            //     link:'test',
            //     name:'测试管理'
            // },
            {
                imgUrl:require('../img/nav_list/icon_client.png'),
                link:'customerManage',
                name:'客户管理'
            },
            {
                imgUrl:require('../img/nav_list/icon_addOrder.png'),
                link:'addOrders',
                name:'新增订单'
            },
            {
                imgUrl:require('../img/nav_list/icon_orderQuery.png'),
                link:'shoppingCart',
                name:'订单查询',
            },
            {
                imgUrl:require('../img/nav_list/icon_Approva.png'),
                link:'businessReview',
                name:'业务审批',
                code:['0603']
            },
            {
                imgUrl:require('../img/nav_list/icon_business.png'),
                link:'bigbusiness',
                name:'特大业务查询',
                code:['0401','0601','0604','0603']
            },
            {
                imgUrl:require('../img/nav_list/icon_NonOperative.png'),
                link:'unbusinessManage',
                name:'非业务支付申请'
            },
        ]
    },
    {
        title:'财务管理',
        list:[
            {
                imgUrl:require('../img/nav_list/icon_finance_1.png'),
                link:'notAudit',
                name:'业务支付审核',
                code:['0603','0402','0601']
            },
            {
                imgUrl:require('../img/nav_list/icon_finance_2.png'),
                link:'unbusinessAudit',
                name:'非业务支付审核',
                code:['0603','0402','0601']
            },
            {
                imgUrl:require('../img/nav_list/icon_finance_3.png'),
                link:'invoiceAudit',
                name:'发票审核',
                code:['0603','0402']
            },
            {
                imgUrl:require('../img/nav_list/icon_finance_4.png'),
                link:'expectPayAudit',
                name:'预付款审批',
                code:['0402','0601']
            },
        ]
    },
    {
        title:'通知管理',
        list:[
            {
                imgUrl:require('../img/nav_list/icon_notice_1.png'),
                link:'receipt',
                name:'已收款'
            },
            {
                imgUrl:require('../img/nav_list/icon_notice_2.png'),
                link:'pay',
                name:'已付款'
            },
            {
                imgUrl:require('../img/nav_list/icon_notice_3.png'),
                link:'billManage',
                name:'已开票'
            },
        ]
    },
    {
        title:'个人业务结算',
        list:[
            {
                imgUrl:require('../img/nav_list/icon_balance_1.png'),
                link:'unpaidManage',
                name:'未收款订单'
            },
            {
                imgUrl:require('../img/nav_list/icon_balance_2.png'),
                link:'overpayManage',
                name:'多付款订单'
            },
            {
                imgUrl:require('../img/nav_list/icon_balance_3.png'),
                link:'unauditManage',
                name:'审核未通过订单'
            },
        ]
    },
    {
        title:'业务统计分析',
        list:[
            // {
            //     imgUrl:require('../img/nav_list/icon_statistics.png'),
            //     link:'reportManage',
            //     name:'业务统计'
            // },
            {
                imgUrl:require('../img/nav_list/icon_statistics.png'),
                link:'performanceStat',
                name:'业绩统计'
            },
        ]
    },
]

export default list