<!-- 添加联系人 -->
<template>
    <div class="body_gery">
        <ul class="form_list">
            <li class="flex flex_a_c">
                <label class="title"><span class="must">银行账户</span></label>
                <input type="text" v-model="addData.cb_bankName" placeholder="请输入银行账户">
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span class="must">银行账号</span></label>
                <input type="text" v-model="addData.cb_bankNum" placeholder="请输入银行账号">
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span class="must">开户行</span></label>
                <input type="text" v-model="addData.cb_bank" placeholder="请输入开户行">
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span class="must">开户地址</span></label>
                <input type="text" v-model="addData.cb_bankAddress" placeholder="请输入开户地址">
            </li>
            <li class="flex flex_a_c flex_s_b" @click="chosen('enabled')">
                <label class="title"><span>启用</span></label>
                <input type="text" :value="enabledName" placeholder="请选择" readonly>
                <div class="icon_right arrows_right"></div>
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
           navTitle:'添加银行账号',
           addData:{},
            enabled:[   //启用
                {
                    key:'否',
                    value:false,
                },
                {
                    key:'是',
                    value:true
                }
            ],
            cb_flag:true,
            enabledName:'是',
            cid:0
       };
    },
    components: {},
    computed: {
        ...mapState({
            userInfo: state => state.user.userInfo
        })
    },
    created(){
        let {datails,type,index} = this.$route.query
        this.datails = datails
        this.type = type
        this.cid = datails.c_id
        if(type == 'EDIT'){
            let {cb_bankName,cb_bankNum,cb_bank,cb_bankAddress,cb_flag,cb_id} = datails.banks_list[index]
            this.navTitle = '编辑银行账号'
            this.addData = {cb_bankName,cb_bankNum,cb_bank,cb_bankAddress,cb_flag,cb_id}
            this.cb_flag=cb_flag
            this.enabledName=cb_flag?"是":"否"
        }
        if(type == 'add'){
            this.navTitle = '添加银行账号'
        }
    },
    mounted() {

    },
    methods: {
        ...mapActions([
            'getbankEdit'
        ]),
        submit(){
            let _this=this
            let {cb_bankName,cb_bankNum,cb_bank,cb_bankAddress,cb_flag,cb_id} = _this.addData
            if(!cb_bankName){
                _this.ddSet.setToast({text:'请输入银行账户'})
                return
            }
            if(!cb_bankNum){
                _this.ddSet.setToast({text:'请输入银行账号'})
                return
            }
            if(!cb_bank){
                _this.ddSet.setToast({text:'请输入开户行'})
                return
            }
            if(!cb_bankAddress){
                _this.ddSet.setToast({text:'请输入开户地址'})
                return
            }
            let params = {
                cb_bankName,
                cb_bankNum,
                cb_bank,
                cb_bankAddress,
                cb_flag,
                cb_id,                
                managerid:_this.userInfo.id    //测试ID
            }
            _this.ddSet.showLoad()
            params.cb_cid = _this.cid
            params.cb_flag = _this.cb_flag
            if(_this.type=="add"){
                params.cb_id=0
            }
            _this.getbankEdit(params).then(res => {
                _this.ddSet.hideLoad()
                if(!res.data.status){
                    _this.ddSet.setToast({text:res.data.msg})
                }else{
                    _this.ddSet.setToast({text:"提交成功"}).then(res => {
                        this.$router.go(-1)
                    })
                }
            }).catch(err => {
                _this.ddSet.hideLoad()
            })
        },
        chosen(item){
            let selectedKey,source
            if(item == 'enabled'){
                source = this.enabled
                selectedKey = this.enabledName
            }
            this.ddSet.setChosen({source,selectedKey}).then(res => {
                if(item == 'enabled'){
                    this.$set(this,'enabledName',res.key)
                    this.$set(this.addData,'cb_flag',res.value)
                }
            })
        }
    },
}
</script>

<style scoped lang='scss'>

</style>
