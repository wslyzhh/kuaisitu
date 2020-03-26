<!-- 添加联系人 -->
<template>
    <div class="body_gery">
        <ul class="form_list">
            <li class="flex flex_a_c">
                <label class="title"><span class="must">联系人</span></label>
                <input type="text" v-model="addData.co_name" placeholder="请输入主要联系人姓名">
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span class="must">联系号码</span></label>
                <input type="text" v-model="addData.co_number" placeholder="请输入客户的电话号码">
            </li>
        </ul>
        <top-nav :title="navTitle" text="保存" @rightClick="submit"></top-nav>
    </div>
</template>

<script>
import {mapActions,mapState} from 'vuex'
export default {
    name:"",
    data() {
       return {
           datails:null,
           type:null,
           navTitle:'添加联系人信息',
           addData:{}
       };
    },
    components: {},
    computed: {
        ...mapState({
            userInfo: state => state.user.userInfo
        })
    },
    created(){
        let {datails,type,msg} = this.$route.query
        this.datails = datails
        this.type = type
        if(type == 'EDIT'){
            let {co_name,co_number,co_id} = datails.contacts_list[0]
            this.navTitle = '编辑联系人信息'
            this.addData = {co_name,co_number,co_id}
        }
        if(type == 'msg' && msg){
            let {co_name,co_number,co_id} = msg
            this.navTitle = '编辑联系人信息'
            this.addData = {co_name,co_number,co_id}
        }
    },
    mounted() {

    },
    methods: {
        ...mapActions([
            'getLinkmanAdd',
            'getContactEdit'
        ]),
        submit(){
            let {co_name,co_number,co_id} = this.addData
            if(co_name == undefined){
                this.ddSet.setToast({text:'请输入主要联系人姓名'})
                return
            }
            if(!co_number == undefined){
                this.ddSet.setToast({text:'请选择客户的电话号码'})
                return
            }
            let {c_id} = this.datails
            let params = {
                co_name,
                co_number,
                managerid:this.userInfo.id    //测试ID
            }
            this.ddSet.showLoad()
            if(this.type == 'add'){
                params.c_id = c_id
                this.getLinkmanAdd(params).then(res => {
                    this.ddSet.hideLoad()
                    if(!res.data.status){
                        this.ddSet.setToast({text:res.data.msg})
                    }else{
                        this.ddSet.setToast({text:"提交成功"}).then(res => {
                            this.$router.go(-1)
                        })
                    }
                }).catch(err => {
                    this.ddSet.hideLoad()
                })
            }else{
                params.co_id = co_id
                this.getContactEdit(params).then(res => {
                    this.ddSet.hideLoad()
                        if(!res.data.status){
                            this.ddSet.setToast({text:res.data.msg})
                        }else{
                            this.ddSet.setToast({text:"编辑成功"}).then(res => {
                            this.$router.go(-1)
                        })
                        }
                }).catch(err => {
                    this.ddSet.hideLoad()
                })
            }
        }
    },
}
</script>

<style scoped lang='scss'>

</style>
