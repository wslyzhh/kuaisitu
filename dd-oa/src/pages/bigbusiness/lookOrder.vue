<!-- 查看订单 -->
<template>
    <div class="body_gery">
        <ul class="form_list">
            <li class="flex flex_a_c">
                <label class="title"><span>订单号</span></label>
                <h3 class="hint_1">系统自动生成</h3>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>下单人</span></label>
                <input type="text" placeholder="请输入下单人">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span class="must">客户</span></label>
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span>联系人</span></label>
                <input type="text" placeholder="请输入联系人">
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span>联系号码</span></label>
                <input type="text" placeholder="请输入联系号码">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>合同造价</span></label>
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span>活动内容</span></label>
                <input type="text" placeholder="请输入活动内容">
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span>活动名称</span></label>
                <input type="text" placeholder="请输入活动名称">
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span>活动地点</span></label>
                <input type="text" placeholder="请输入活动地点">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>活动日期</span></label>
                <input type="text" placeholder="请选择活动起始日期">
                <div class="icon_right time"></div>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>活动日期</span></label>
                <input type="text" placeholder="请选择活动结束日期">
                <div class="icon_right time"></div>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>活动归属地</span></label>
                <div class="icon_right"></div>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>报账人员</span></label>
                <div class="icon_right add"></div>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>策划人员</span></label>
                <div class="icon_right add"></div>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>执行人员</span></label>
                <div class="icon_right add"></div>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>订单状态</span></label>
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>推送上级审核</span></label>
                <div class="icon_right"></div>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>接单状态</span></label>
                <input type="text" placeholder="待定">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>锁单状态</span></label>
                <input type="text" placeholder="未锁">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>上级审批</span></label>
                <input type="text" placeholder="待审批">
            </li>
            <li class="li_auto flex">
                <label class="title"><span>备注</span></label>
                <textarea placeholder="请输入备注"></textarea>
            </li>
        </ul>
        <ul class="form_list">
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>一类活动文件</span></label>
                <div class="icon_right accessory"></div>
            </li>
        </ul>
        <ul class="form_list">
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>二类活动文件</span></label>
                <div class="icon_right accessory"></div>
            </li>
        </ul>
        <div class="hint">
            部分人员可查看，文件大小限制：51200KB</br> 
            文件类型：gif,jpg,jpeg,png,bmp,rar,zip,doc,xls,txt,docx,xlsx
        </div>
    </div>
</template>

<script>
export default {
    name:"",
    data() {
       return {

       };
    },
    components: {},
    computed: {},
    created(){
        this.ddSet.setTitleRight({title:'查看订单'}).then(res => {
            if(res){

            }
        })
        
    },
    mounted() {

    },
    methods: {

    },
}
</script>

<style scoped lang='scss'>
    .hint{
        padding: .3rem;
        font-size: .24rem;
        color: #fc0214;
        line-height: .4rem;
    }
    .looK_button_list{
        li{
            outline: none;
            background: none;
            border: none;
            width: 6.9rem;
            height: .8rem;
            line-height: .8rem;
            text-align: center;
            color: #FFF;
            font-size: .36rem;
            margin: 0 auto;
            border-radius: 4px;
            margin-bottom: .2rem;
        }
    }
</style>

