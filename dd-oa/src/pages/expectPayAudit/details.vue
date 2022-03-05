<!-- 添加客户 -->
<template>
    <div class="body_gery">
        <ul class="form_list form_list_noborder">
            <li class="flex flex_a_c flex_s_b" >
                <label class="title"><span>付款对象</span></label>
                <input type="text" :value="clientName" readonly>
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span>付款金额</span></label>
                <input type="text" v-model="addData.rpmoney">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>预付日期</span></label>
                <input type="text" v-model="addData.rpforedate" readonly >
            </li>
            <li class="li_auto flex">
                <label class="title"><span>银行账号</span></label>
                <textarea class="bankContent" v-model="addData.bankName"></textarea>
            </li>
			<li class="flex flex_a_c flex_s_b">
			    <label class="title"><span>付款方式</span></label>
			    <input type="text" readonly v-model="addData.rp_method_text">
			</li>
            <li class="li_auto flex">
                <label class="title"><span>付款内容</span></label>
                <textarea v-model="addData.rpcontent" readonly></textarea>
            </li>     
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>附件</span></label>
            </li>
    		<li class="flex flex_a_c flex_s_b" v-for="(f,index) in files" :key="index">
    			<a :href="'/' + f.pp_filePath">{{f.pp_fileName}}</a>
    			<span>{{f.pp_size}}K</span>
    		</li>       
            <li class="flex flex_a_c flex_s_b">
			    <label class="title newTitle"><span>审批类型</span></label>
			    <input type="text" readonly :value="addData.type_text">
			</li>
            <li class="flex flex_a_c flex_s_b" @click="changeFlag">
                <label class="title newTitle"><span>审批状态</span></label>
                <input type="text" readonly :value="status_text">
			    <div class="icon_right arrows_right"></div>
            </li>
            <li class="li_auto flex">
                <label class="title newTitle"><span>审批备注</span></label>
                <textarea v-model="addData.rpremark"></textarea>
            </li>
        </ul>
        <top-nav :title='"审批预付款"' :text='"保存"' @rightClick="submit" ></top-nav>
    </div>
</template>

<script>
import {mapActions,mapState} from 'vuex'
export default {
    name:"",
    data() {
        return {
            addData:{},
            clientName:'',  
            type:'',
            rpid:0,           
            status:0,
            files:[],
            status_text:'待审批', 
            flagList:[  //审批状态
                {
                    key:'待审批',
                    value:'0'
                },
                {
                    key:'审批未通过',
                    value:'1'
                },
                {
                    key:'审批成功',
                    value:'2'
                },
            ]
        };
    },
    components: {},
    computed: {
        ...mapState({
            userInfo: state => state.user.userInfo
        })
    },
    created(){
        let {type,rp_id} = this.$route.query
        this.type = type
        this.rpid=rp_id
        if(type == 'check'){
            let params = {
                rp_id,
                type,
                managerid:this.userInfo.id
            }
            this.getReceiptDetails(params).then(res => {
                this.addData = res.data
                this.clientName=this.addData.c_name
                this.clientId=this.addData.c_id
                this.addData.rpmoney=this.addData.rp_money
                this.addData.rpforedate = this.addData.rp_foredate
                this.addData.rpmethod=this.addData.rp_method
                this.addData.rp_method_text= this.addData.pm_name
                this.addData.rpcontent = this.addData.rp_content
                this.addData.type_text=this.addData.typeText
                this.addData.ctype=this.addData.type
                this.addData.rpremark = this.addData.remark
                this.status_text = this.flagList[this.addData.flag].key
                this.status = this.addData.flag
                this.files = this.addData.picList
            })
        }
    },
    mounted() {
       
    },
    methods: {
        ...mapActions([
            'getReceiptDetails',
            'getPayAudit'
        ]),
        submit(item){ //提交
            if(!this.status){
                this.ddSet.setToast({text:'请选择审批状态'})
                return
            }
            this.addData.remark = this.addData.rpremark
            this.addData.status = this.status
            this.addData.managerid = this.userInfo.id //测试ID
            this.ddSet.showLoad()
            console.log(this.addData)
            this.getPayAudit(this.addData).then(res => {
                this.ddSet.hideLoad()
                if(res.data.status){
                    this.ddSet.setToast({text:'审核成功'}).then(res => {
                        this.$router.go(-1)
                    })
                }else{
                    this.ddSet.setToast({text:res.data.msg})
                }
            }).catch(err => {
                this.ddSet.hideLoad()
            })
        },
        changeFlag(){
            let _this = this
                let source =_this.flagList,selectedKey = _this.status_text          
                _this.ddSet.setChosen({source,selectedKey}).then(res => {
                    _this.$set(_this,'status',res.value)
                    _this.$set(_this,'status_text',res.key)
                })
        }
    },
    beforeDestroy(){
        
    }
}
</script>

<style scoped lang="scss">
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
    .bankContent{
        height: 0.85rem;
    }
</style>

