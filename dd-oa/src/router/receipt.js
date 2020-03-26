
//收款通知列表
import receipt from '../pages/receipt/index'  
//收款通知明细
import receiptDetails from '../pages/receipt/receiptDetails'  

const routes = [
    {
        path:'/receipt',
        component:receipt,
    },
    {
        path:'/receiptDetails',
        component:receiptDetails,
    }
]

export default routes