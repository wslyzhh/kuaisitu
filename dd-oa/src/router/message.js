//消息列表
import messageList from '../pages/message/index'
//消息详情
import messageDetails from '../pages/message/detail'

const routes = [
    {
        path:'/messageList',
        component:messageList,
    },
    {
        path:'/messageDetails',
        component:messageDetails,
    },
]

export default routes 