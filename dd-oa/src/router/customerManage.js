
// const customerManage = resolve => require(['../pages/customerManage/index'],resolve)
//客户管理首页
import customerManage from '../pages/customerManage/index'  
//添加客户
import addClient from '../pages/customerManage/addClient'
//客户详情
import clientDetails from '../pages/customerManage/clientDetails'
//添加联系人
import addLinkman from '@/pages/customerManage/addLinkman' 
//添加银行账号
import addBank from '@/pages/customerManage/addBank'
const routes = [
    {
        path:'/customerManage',
        component:customerManage,
    },
    {
        path:'/addClient',
        component:addClient,
    },
    {
        path:'/clientDetails',
        component:clientDetails,
    },
    {
        path:'/addLinkman',
        component:addLinkman,
    },
    {
        path:'/addBank',
        component:addBank,
    },
]

export default routes