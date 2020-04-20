<!-- 添加客户 -->
<template>
    <div class="body_gery">
        <ul class="form_list form_list_noborder">
            <li class="li_auto flex">
                <label class="title"><span>订单号</span></label>
                {{addData.rpdoid}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>付款对象</span></label>
                <input type="text" :value="clientName" readonly>
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span>付款金额</span></label>
                <input type="text" v-model="addData.rpdmoney">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>预付日期</span></label>
                <input type="text" v-model="addData.rpdforedate" readonly>
            </li>
            <li class="li_auto flex">
                <label class="title"><span>银行账号</span></label>
                <textarea class="bankContent" v-model="addData.bankName"></textarea>
            </li>
            <li class="li_auto flex">
                <label class="title"><span>收款内容</span></label>
                <textarea class="rpContent" v-model="addData.rpdcontent"></textarea>
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
                <textarea v-model="addData.rpdremark"></textarea>
            </li>
        </ul>
        <top-nav :title='"审批付款明细"' :text='"保存"' @rightClick="submit" ></top-nav>
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
            rpdid:0,
            files:[],
            status:0,
            status_text:'待审批',
            typeList:[  //审批类型
                {
                    key:'部门审批',
                    value:'1'
                },
                {
                    key:'财务审批',
                    value:'2'
                },
                {
                    key:'总经理审批',
                    value:'3'
                },
            ],
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
        let _this = this
        let {type,rpd_id} = _this.$route.query
        _this.type = type
        _this.rpdid=rpd_id
        if(type == 'Check'){
            let params = {
                rpd_id:rpd_id,
                type:'check',
                managerid:_this.userInfo.id
            }
            this.getPayDetailEdit(params).then(res => {
                console.log(res.data)
                _this.addData = res.data
                _this.clientName=_this.addData.c_name
                _this.addData.rpdoid = _this.addData.rpd_oid
                _this.addData.rpdmoney=_this.addData.rpd_money
                _this.addData.rpdforedate = _this.addData.rpd_foredate
                _this.addData.rpdcontent = _this.addData.rpd_content
                _this.addData.rpdremark = _this.addData.remark                
                _this.addData.type_text = _this.typeList[_this.addData.type-1].key
                _this.status_text = _this.flagList[_this.addData.flag].key
                _this.files = _this.addData.albumlist
                _this.addData.bankName = _this.addData.bankName
            })
        }
    },
    mounted() {
    },
    methods: {
        ...mapActions([
            'getPayDetailEdit',
            'payDetailAudit'
        ]),
        submit(item){ //提交
            let _this = this
            if(!_this.status){
                _this.ddSet.setToast({text:'请选择审批状态'})
                return
            }
            _this.addData.managerid = _this.userInfo.id //测试ID
            _this.addData.rpdid=_this.addData.rpd_id
            _this.addData.status = _this.status
            _this.addData.remark = _this.addData.rpdremark
            _this.ddSet.showLoad()
            console.log(_this.addData)
            if(_this.type == 'Check'){
                _this.payDetailAudit(_this.addData).then(res => {
                    _this.ddSet.hideLoad()
                    if(res.data.status){
                        _this.ddSet.setToast({text:'审核成功'}).then(res => {
                            _this.$router.go(-1)
                        })
                    }else{
                        _this.ddSet.setToast({text:res.data.msg})
                    }
                }).catch(err => {
                    _this.ddSet.hideLoad()
                })
            }
        },
        changeFlag(){
            let _this = this
                let source =_this.flagList,selectedKey = _this.status_text          
                _this.ddSet.setChosen({source,selectedKey}).then(res => {
                    console.log(res.value)
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
        
    .newTitle{
        font-family: SimHei;
        font-weight: bolder;
    }
    .rpContent{
        height: 1.75rem;
    }
    .bankContent{
        height: 0.85rem;
    }
</style>

