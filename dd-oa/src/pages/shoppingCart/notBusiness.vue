<!-- 新增非业务申请 -->
<template>
    <div class="body_gery">
        <ul class="form_list">
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>支付类别</span></label>
                <input type="text" placeholder="员工往来">
                <div class="icon_right"></div>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span class="must">支付用途</span></label>
                <input type="text" placeholder="业务活动执行备用金借款">
                <div class="icon_right"></div>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>用途说明</span></label>
                <input type="text" placeholder="请输入用途说明内容">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>收款银行</span></label>
                <input type="text" placeholder="请输入收款银行">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>账户名称</span></label>
                <input type="text" placeholder="请输入账户名称">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>收款账号</span></label>
                <input type="text" placeholder="请输入收款账号">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span class="must">金额</span></label>
                <input type="text" placeholder="请输入有效金额">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span class="must">预付日期</span></label>
                <input type="text" placeholder="请选择活动起始日期">
                <div class="icon_right time"></div>
            </li>
            <li class="li_auto flex">
                <label class="title"><span>备注</span></label>
                <textarea placeholder="请输入备注"></textarea>
            </li>
        </ul>
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
        this.ddSet.setTitleRight({title:'新增非业务申请',text:'保存'}).then(res => {
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

</style>
